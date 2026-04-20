using CSGenio.business;
using CSGenio.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGenio.core.messaging
{
    /// <summary>
    /// B2B messaging service
    /// Publishes and subscribes messages according to configuration
    /// </summary>
    /// <remarks>
    /// Meant to be used as a Singleton to share the same provider connections.
    /// The Metadata property must be set before the Instance is called.
    /// </remarks>
    public class MessagingService : IProviderMessageHandler
    {
        private IMessageProvider m_provider;

        /// <summary>
        /// Current metadata in use. The value is set during service start method.
        /// </summary>
        public MessageMetadata Metadata { get; private set; } = new MessageMetadata();

        /// <summary>
        /// Starts the connection to the message provider, making messaging methods available
        /// </summary>
        /// <param name="metadata">The metadata to use for provider routing setup.</param>
        /// <param name="providerType">The provider type to use.</param>
        /// <param name="enableSubscribe">True if this process should also subscribe to messages, false if it should only publish.</param>
        public void Start(MessageMetadata metadata, string providerType, bool enableSubscribe)
        {
            if(metadata == null) throw new ArgumentNullException("metadata");
            Metadata = metadata;

            switch (providerType)
            {
                case "RabbitMq":
                    m_provider = new RabbitMqProvider(Metadata, this);
                    break;
                case "MemoryMq":
                    m_provider = new MemoryMqProvider(Metadata, this);
                    break;
                default:
                    throw new Exception("Unknown message provider type");
            }

            m_provider.Start(enableSubscribe);
        }


        public void SendMessage(PublisherMetadata pub, AreaDataset data, string dataset)
        {
            if (m_provider == null)
                throw new InvalidOperationException("Messaging service is not initialized.");

            //message envelope
            QueueMessage msg = new QueueMessage
            {
                Envelope = new QueueMessageEnvelope
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Process = pub.Group + pub.Name,
                    Dataset = dataset,
                    Version = pub.Version
                }
            };

            //convert the dataset into serializer friendly structure
            msg.FromDataset(data, pub);

            m_provider.SendMessage(pub, msg);
        }

        public void Close()
        {
            m_provider?.Stop();
            m_provider = null;
        }

        /// <inheritdoc/>
        public Task OnReceiveAck(PublisherMetadata pub, AckMessage msg)
        {
            //log errors
            if (msg.ErrorList != null && msg.ErrorList.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"ack {pub.Id} id: {msg.OriginId}");
                foreach (var error in msg.ErrorList)
                {
                    sb.Append('\t');
                    sb.AppendLine(error.ToString());
                }
                Log.Error(sb.ToString());
            }

            //we need to call the Ack processor here
            pub.Ack.Process(msg);

            return Task.CompletedTask;
        }

        private const int _retryInterval = 100;
        private const int _maxRetries = 3;

        /// <inheritdoc/>
        public async Task OnReceiveMessage(SubscriberMetadata sub, QueueMessage msg)
        {
            //The ack will gather the reply context during processing
            AckMessage ack = new AckMessage
            {
                Status = AckStatus.Ok,
                Target = Configuration.Program,
                OriginId = msg.Envelope.Id
            };

            //Parse the message into an internal dataset
            var dataset = msg.ToDataSet(sub, new User("msmq", "", ""));

            //Initialize the ack reply
            foreach (var table in dataset.Tables) 
            {
                var st = sub.Tables.First(t => t.Name == table.Key);

                var ackPkList = new List<string>(table.Value.Updated.Values.Select(a => a.QPrimaryKey));
                ack.RowsPk.Add(st.Alias, ackPkList);

                //filter out rows if there is a condition
                if (st.Filter != null)
                {
                    table.Value.Updated = table.Value.Updated.Where(row =>
                    {
                        FormulaDbContext fdc = new FormulaDbContext(row.Value);
                        fdc.AddFormulaSources(st.Filter);
                        return (bool)st.Filter.calculateInternalFormula(row.Value, null, fdc, FunctionType.ALT);
                    }).ToDictionary(i => i.Key, i => i.Value);
                }
            }

            //Ack context will gather multiple responses that the processing may generate
            //It also provides a way for intermediate progress to be reported back to the sender
            var responseContext = new AckResponseContext(ack, msg.Envelope, onProgress: x => {
                if (sub.UseAck)
                    m_provider.SendAck(sub, x);
            });

            //Call the respective processor
            var proc = sub.Processor;
            int retries = 0;
            while (true)
            {
                try
                {
                    //process may use the responseContext to change the ack status
                    proc.Process(responseContext, dataset, sub);
                    if (ack.Status == AckStatus.Ignore)
                    {
                        //ignore means we return without sending an ack regardless of config
                        return;
                    }
                    //if it returns an error or ok we follow through to send an ack
                    break;
                }
                catch (CSGenio.persistence.PersistenceException e)
                {
                    //Only certain exception types are recoverable errors:
                    // Connection failures, deadlock victim, etc.
                    //A recoverable error is one that has a time dependency.
                    //Business logic errors are not considered recoverable.
                    retries++;
                    if(e.IsRetryable && retries <= _maxRetries)
                    {
                        //linear increase retry timeout interval
                        await Task.Delay(_retryInterval * retries);
                        continue;
                    }

                    //add a top level error to the error list
                    ack.Status = AckStatus.Error;
                    ack.ErrorList.Add(new AckError { Id = "", Table = "", Message = e.Message });
                }
                catch (Exception e)
                {
                    ack.Status = AckStatus.Error;
                    ack.ErrorList.Add(new AckError { Id = "", Table = "", Message = e.Message });
                }
                break;
            }

            //signal the end of processing
            ack.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (sub.UseAck)
            {
                //fix error message table aliases before replying with the ack.
                //error messages are put based on the dataset internal name. But the sender names might not match.
                foreach (var err in ack.ErrorList)
                    if (!string.IsNullOrEmpty(err.Table))
                    {
                        var st = sub.Tables.Find(t => t.Name == err.Table);
                        if (st != null)
                            err.Table = st.Alias;
                    }

                m_provider.SendAck(sub, ack);
            }
        }

    }
}