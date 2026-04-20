using CSGenio.framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CSGenio.core.messaging
{
    /// <summary>
    /// RabbitMq transport implementation for messages
    /// </summary>
    public class RabbitMqProvider : IMessageProvider
    {
        private IConnection m_connectionPub;
        private IConnection m_connectionSub;
        private readonly MessageMetadata m_metadata;
        private IModel m_ackChannel;
        private IModel m_channelSub;

        private readonly IProviderMessageHandler m_handler;

        private readonly ConcurrentBag<IModel> m_channelPool = new ConcurrentBag<IModel>();

        private IModel RentPubChannel()
        {
            if (m_channelPool.TryTake(out var item))
                return item;
            else
            {
                var channel = m_connectionPub.CreateModel();
                //any channel initialization configuration can be done here
                channel.ConfirmSelect();
                return channel;
            }
        }

        private void ReturnPubChannel(IModel model)
        {
            //if the channel is in a reusable state then return it to the pool
            if (!model.IsClosed)
                m_channelPool.Add(model);
        }

        public RabbitMqProvider(MessageMetadata meta, IProviderMessageHandler handler)
        {
            m_metadata = meta;
            m_handler = handler;
        }

        /// <inheritdoc/>
        public void Start(bool enableSubscribe)
        {
            SetupPublications();
            if (enableSubscribe)
            {
                SetupAcks();
                SetupSubscriptions();
            }
        }

        private ConnectionFactory GetFactory() => new ConnectionFactory
        {
            Uri = new Uri(Configuration.Messaging.Host.Endpoint),
            UserName = Configuration.Messaging.Host.UsernameDecode(),
            Password = Configuration.Messaging.Host.PasswordDecode(),
            DispatchConsumersAsync = true
        };

        private void SetupPublications()
        {
            //setup connections
            m_connectionPub = GetFactory().CreateConnection(Configuration.Program + " Pub");

            //temporary channel just to create the topology
            using (var channel = m_connectionPub.CreateModel())
            {
                foreach (var pub in m_metadata.Publishers)
                {
                    //declare message exchange
                    var exchange = pub.Group + ".msg." + pub.Name;
                    channel.ExchangeDeclare(exchange, "fanout", true, false);
                }
            }
        }

        private void SetupAcks()
        {
            //acks are received on the publication connection to avoid congesting the subscription connection
            m_ackChannel = m_connectionPub.CreateModel();
            m_ackChannel.BasicQos(0, 5, false);

            //temporary channel just to create the topology
            using (var channel = m_connectionPub.CreateModel())
            {
                foreach (var pub in m_metadata.Publishers)
                {
                    //declare ack reply exchange and queue
                    if (pub.Ack != null)
                    {
                        var exchange = pub.Group + ".ack." + pub.Name;
                        channel.ExchangeDeclare(exchange, "fanout", durable: true, autoDelete: false);

                        _ = channel.QueueDeclare(exchange,
                            durable: true,
                            exclusive: false,
                            autoDelete: false);

                        channel.QueueBind(queue: exchange,
                              exchange: exchange,
                              routingKey: string.Empty);


                        //subscribe to the ack queue
                        var consumer = new AsyncEventingBasicConsumer(m_ackChannel);
                        consumer.Received += ReceiveAck;

                        m_ackChannel.BasicConsume(queue: exchange,
                                 autoAck: false,
                                 consumerTag: pub.Group + "." + pub.Name,
                                 consumer: consumer);
                    }
                }
            }
        }


        private void SetupSubscriptions()
        {
            //setup connections
            m_connectionSub = GetFactory().CreateConnection(Configuration.Program + " Sub");
            m_channelSub = m_connectionSub.CreateModel();
            //Rate limit the rabbitmq delivery, otherwise it will just dump
            //messages into our workers as fast as it can, assuming its the worker's problem now.
            //We want the queues to actually queue things.
            //We can't use AutoAck or the consumer sends Ack before the processor even begins
            m_channelSub.BasicQos(0, 5, false);


            foreach (var sub in m_metadata.Subscribers)
            {
                //check if the exchange we are expecting in the source exists
                var exchangeName = sub.Group + ".msg." + sub.Name;
                m_channelSub.ExchangeDeclare(exchangeName, "fanout", durable: true, autoDelete: false);

                // declare a server-named queue
                // The name has to be unique and persistent across the entire topology
                var queueName = exchangeName + "." + Configuration.Program;
                m_channelSub.QueueDeclare(queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                m_channelSub.QueueBind(queue: queueName,
                                    exchange: exchangeName,
                                    routingKey: string.Empty);

                //hook up message handlers        
                var consumer = new AsyncEventingBasicConsumer(m_channelSub);
                consumer.Received += ReceiveMessage;

                m_channelSub.BasicConsume(queue: queueName,
                                        autoAck: false,
                                        consumerTag: sub.Id,
                                        consumer: consumer);
            }
        }


        /// <inheritdoc/>
        public void SendMessage(PublisherMetadata pub, QueueMessage msg)
        {
            if (m_connectionPub == null)
                throw new InvalidOperationException("trying to send message but publications are disabled");

            var exchange_name = pub.Group + ".msg." + pub.Name;

            //channels are thread unsafe, but should be pooled and reused for better performance
            var channel = RentPubChannel();

            try
            {
                //serialize and send the message
                channel.BasicPublish(exchange_name, "", null, msg.ToBytes());

                //if it becomes frequent to send more than 1 message per transaction
                //this should be done only once at the end of that transaction
                channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
            }
            finally
            {
                //we are done with the channel, we can return it to the pool
                ReturnPubChannel(channel);
            }
        }

        private async Task ReceiveAck(object channel, BasicDeliverEventArgs ea)
        {
            AckMessage msg = AckMessage.FromBytes(ea.Body.Span) ?? throw new Exception("Error deserializing ack");

            //parse the tag
            var pub = m_metadata.GetPublisher(ea.ConsumerTag);

            //callback to business handler
            await m_handler.OnReceiveAck(pub, msg);

            //let rabbitMq know we are done with this message
            m_ackChannel.BasicAck(ea.DeliveryTag, false);
        }

        private async Task ReceiveMessage(object channel, BasicDeliverEventArgs ea)
        {
            QueueMessage msg = QueueMessage.FromBytes(ea.Body.Span) ?? throw new Exception("Error deserializing message");;

            //parse the tag
            var sub = m_metadata.GetSubscriber(ea.ConsumerTag);

            //callback to business handler
            await m_handler.OnReceiveMessage(sub, msg);

            //send ack to RabbitMq so it knows we are free to handle more messages
            m_channelSub.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        /// <inheritdoc/>
        public void SendAck(SubscriberMetadata sub, AckMessage ack)
        {
            if (m_connectionPub == null)
                throw new InvalidOperationException("trying to send ack but subscriptions are disabled");

            if (!sub.UseAck)
                return;

            string ack_exchange_name = sub.Group + ".ack." + sub.Name;
            //reuse the same channel we receive from subscribers to send the ack message
            //not sure its thread safe to do this, depends if processes are parallel or not
            //if not, then this should use the channel pool instead.
            m_channelSub.BasicPublish(ack_exchange_name, "", null, ack.ToBytes());
        }

        /// <inheritdoc/>
        public void Stop()
        {
            m_channelSub?.Close();
            m_connectionSub?.Close();

            m_ackChannel?.Close();
            while(m_channelPool.TryTake(out var channel))
                channel.Close();
            m_connectionPub?.Close();
        }
    }

}