namespace CSGenio.core.messaging
{
    /// <summary>
    /// Message subscription processor
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Processes a message arrived by through the subscription channel
        /// The results of the process should be collected in the response.
        /// </summary>
        /// <param name="response">Interface to the response context, including the envelope of the message</param>
        /// <param name="dataset">The parsed data retreived from the message according to the subscribed schema</param>
        /// <param name="meta">Metadata information about the subscription</param>
        void Process(IProcessorResponse response, AreaDataset dataset, SubscriberMetadata meta);
    }

    /// <summary>
    /// Message Ack processor
    /// </summary>
    public interface IAckProcessor
    {
        /// <summary>
        /// Processes a reply to a journaled publication
        /// </summary>
        /// <param name="ack">The collected results of the remote processing</param>
        void Process(AckMessage ack);
    }


    /// <summary>
    /// Methods and action to collect results of the ongoing processing
    /// </summary>
    public interface IProcessorResponse
    {
        /// <summary>
        /// The envelope information about the message that was sent
        /// </summary>
        QueueMessageEnvelope Envelope { get; }

        /// <summary>
        /// Add an error message to the result. Changes the ack state to error.
        /// Only available when the subscription is journaled.
        /// </summary>
        /// <param name="table">Optional table to which the error relates to</param>
        /// <param name="pk">Optional primary key of the row where the error occured</param>
        /// <param name="message">Error message</param>
        void AddError(string table, string pk, string message);

        /// <summary>
        /// Sends an intermediate result of the progress of the process back through the ack channel.
        /// Only available when the subscription is journaled.
        /// </summary>
        /// <param name="message">The progress message</param>
        /// <param name="progress">The progress percentage expressed from 0-100</param>
        void SendProgress(string message, int progress);
        /// <summary>
        /// Inhibits the return of a Ack, even if this message is journaled.
        /// Only necessary for jounaled messages.
        /// </summary>
        void Ignore();
    }
}
