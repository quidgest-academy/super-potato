using CSGenio.framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace CSGenio.core.messaging
{


    /// <summary>
    /// Memory simulator transport implementation for messages
    /// </summary>
    /// <remarks>
    /// For testing purposes ONLY.
    /// </remarks>
    public class MemoryMqProvider : IMessageProvider
    {
        private class MemoryMqQueue
        {
            public string Id { get; set; }
            public Channel<byte[]> Queue { get; set; } = Channel.CreateUnbounded<byte[]>();
            public Task Consumer { get; set; }
        }

        private class MemoryMqExchange
        {
            public string Id { get; set; }
            public List<MemoryMqQueue> Bindings { get; set; } = new List<MemoryMqQueue>();
        }

        private Dictionary<string, MemoryMqExchange> Exchanges = new Dictionary<string, MemoryMqExchange>();


        private readonly MessageMetadata m_metadata;

        private readonly IProviderMessageHandler m_handler;


        public MemoryMqProvider(MessageMetadata meta, IProviderMessageHandler handler)
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

        private void SetupPublications()
        {
            foreach (var pub in m_metadata.Publishers)
            {
                //declare message exchange
                var exchange = pub.Group + ".msg." + pub.Name;

                Exchanges[exchange] = new MemoryMqExchange
                {
                    Id = exchange
                };
            }
        }

        private void SetupAcks()
        {
            foreach (var pub in m_metadata.Publishers)
            {
                //declare ack reply exchange and queue
                if (pub.Ack != null)
                {
                    var exchange = pub.Group + ".ack." + pub.Name;
                    Exchanges[exchange] = new MemoryMqExchange
                    {
                        Id = exchange
                    };

                    var q = new MemoryMqQueue();
                    q.Id = exchange;
                    q.Consumer = ListenForAck(q, pub);
                    Exchanges[exchange].Bindings.Add(q);
                }
            }
        }


        private void SetupSubscriptions()
        {
            foreach (var sub in m_metadata.Subscribers)
            {
                //check if the exchange we are expecting in the source exists
                var exchangeName = sub.Group + ".msg." + sub.Name;
                if(!Exchanges.ContainsKey(exchangeName))
                {
                    Exchanges[exchangeName] = new MemoryMqExchange
                    {
                        Id = exchangeName
                    };
                }

                // declare a server-named queue
                // The name has to be unique and persistent across the entire topology
                var queueName = exchangeName + "." + Configuration.Program;

                var q = new MemoryMqQueue();
                q.Id = queueName;
                q.Consumer = ListenForMessage(q, sub);
                Exchanges[exchangeName].Bindings.Add(q);
            }
        }


        /// <inheritdoc/>
        public void SendMessage(PublisherMetadata pub, QueueMessage msg)
        {
            var exchange_name = pub.Group + ".msg." + pub.Name;

            var mb = msg.ToBytes().ToArray();
            foreach (var queue in Exchanges[exchange_name].Bindings)
                queue.Queue.Writer.TryWrite(mb);
        }

        private async Task ListenForAck(MemoryMqQueue queue, PublisherMetadata pub)
        {
            while (await queue.Queue.Reader.WaitToReadAsync())
            {
                var body = await queue.Queue.Reader.ReadAsync();
                AckMessage msg = AckMessage.FromBytes(body) ?? throw new Exception("Error deserializing ack");

                //callback to business handler
                await m_handler.OnReceiveAck(pub, msg);
            }
        }

        private async Task ListenForMessage(MemoryMqQueue queue, SubscriberMetadata sub)
        {
            while (await queue.Queue.Reader.WaitToReadAsync())
            {
                var body = await queue.Queue.Reader.ReadAsync();
                QueueMessage msg = QueueMessage.FromBytes(body) ?? throw new Exception("Error deserializing message");

                //callback to business handler
                await m_handler.OnReceiveMessage(sub, msg);
            }
        }

        /// <inheritdoc/>
        public void SendAck(SubscriberMetadata sub, AckMessage ack)
        {
            if (!sub.UseAck)
                return;

            string ack_exchange_name = sub.Group + ".ack." + sub.Name;
            var mb = ack.ToBytes().ToArray();
            foreach (var queue in Exchanges[ack_exchange_name].Bindings)
                queue.Queue.Writer.TryWrite(mb);
        }

        /// <inheritdoc/>
        public void Stop()
        {
            foreach (var x in Exchanges)
                foreach (var q in x.Value.Bindings)
                {
                    q.Queue.Writer.Complete();
                    q.Consumer.Wait();
                }
        }
    }

}