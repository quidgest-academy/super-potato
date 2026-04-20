using CSGenio.business;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CSGenio.core.messaging
{

    public class MessageMetadata
    {
        public List<PublisherMetadata> Publishers { get; set; } = new List<PublisherMetadata>();
        public List<SubscriberMetadata> Subscribers { get; set; } = new List<SubscriberMetadata>();

        public PublisherMetadata GetPublisher(string id) => Publishers.Find(x => x.Id == id);
        public SubscriberMetadata GetSubscriber(string id) => Subscribers.Find(x => x.Id == id);
    }


    public class PublisherMetadata
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public bool NoReexport { get; set; }
        [JsonIgnore]
        public IAckProcessor Ack {  get; set; }

        public string Id => Group + "." + Name;

        public List<PublisherTable> Tables { get; set; } = new List<PublisherTable>();
    }

    public class PublisherTable
    {
        /// <summary>
        /// Name that will be sent on the message
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// Areas that represent the same table (domain)
        /// </summary>
        public List<string> Areas { get; set; }
        /// <summary>
        /// Optionally filter out rows before adding them to the message
        /// </summary>
        [JsonIgnore]
        public InternalOperationFormula Filter {  get; set; } 
        /// <summary>
        /// This table is not sent when its saved, its sent in anex to other (non-anex) published tables
        /// </summary>
        public bool IsAnex { get; set; }
        public HashSet<string> Fields { get; set; } = new HashSet<string>();
    }


    public class SubscriberMetadata
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public bool UseAck { get; set; }
        [JsonIgnore]
        public IMessageProcessor Processor { get; set; }

        public string Id => Group + "." + Name;

        public List<SubscriberTable> Tables { get; set; } = new List<SubscriberTable>();
    }

    public class SubscriberTable
    {
        /// <summary>
        /// Name of the table in this system
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Name of the table in the message
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// Optionally automatically ignore rows
        /// Condition is true for rows that must be processed, false to filter them.
        /// </summary>
        [JsonIgnore]
        public InternalOperationFormula Filter { get; set; }
    }

}
