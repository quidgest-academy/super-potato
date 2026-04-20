using System.Threading.Tasks;

namespace CSGenio.core.messaging
{
    /// <summary>
    /// Message transport service interface    
    /// </summary>
    /// <remarks>
    /// Transports require the metadata configuration to setup queues during Start()
    /// Also should require a IProviderMessageHandler to be called during internal reception of messages.
    /// These should be sent through the constructor.
    /// </remarks>
    public interface IMessageProvider
    {
        /// <summary>
        /// Establishes transport connections and declares topology
        /// </summary>
        /// <param name="enableSubscribe">Setups connection to subscribe to messages and ack's</param>
        void Start(bool enableSubscribe);
        /// <summary>
        /// Stops all the transport connections
        /// </summary>
        void Stop();

        /// <summary>
        /// Sends a message to the specified publication
        /// </summary>
        /// <param name="pub">The publication to send message to</param>
        /// <param name="msg">The message to send</param>
        void SendMessage(PublisherMetadata pub, QueueMessage msg);

        /// <summary>
        /// Sends a reply ack to the associated reply channel this subscription
        /// </summary>
        /// <param name="sub">The subscription we are replying to</param>
        /// <param name="ack">The ack message to send</param>
        void SendAck(SubscriberMetadata sub, AckMessage ack);
    }

    /// <summary>
    /// Callback methods for reception of messages
    /// </summary>
    /// <remarks>
    /// Message providers call these to handle the business side of message processing
    /// This keeps all business logic independent of transport technology
    /// </remarks>
    public interface IProviderMessageHandler
    {
        /// <summary>
        /// Callback for a received message
        /// </summary>
        /// <param name="sub">The subscription we received the message for</param>
        /// <param name="msg">The message received</param>
        Task OnReceiveMessage(SubscriberMetadata sub, QueueMessage msg);
        /// <summary>
        /// Callback for a received ack
        /// </summary>
        /// <param name="pub">The publication we received the ack for</param>
        /// <param name="ack">The ack received</param>
        Task OnReceiveAck(PublisherMetadata pub, AckMessage ack);
    }

}