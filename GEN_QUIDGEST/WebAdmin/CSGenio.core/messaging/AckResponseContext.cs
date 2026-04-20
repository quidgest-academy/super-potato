using System;

namespace CSGenio.core.messaging
{
    /// <inheritdoc/>
    public class AckResponseContext : IProcessorResponse
    {
        private readonly AckMessage m_ack;
        /// <inheritdoc/>
        public QueueMessageEnvelope Envelope { get; private set; }
        private readonly Action<AckMessage> m_onProgress;

        /// <inheritdoc/>
        public AckResponseContext(AckMessage ack, QueueMessageEnvelope envelope, Action<AckMessage> onProgress)
        {
            m_onProgress = onProgress;
            m_ack = ack;
            Envelope = envelope;
        }

        /// <inheritdoc/>
        public void AddError(string table, string pk, string message)
        {
            m_ack.ErrorList.Add(new AckError
            {
                Table = table,
                Id = pk,
                Message = message
            });
            if(m_ack.Status != AckStatus.Error)
                m_ack.Status = AckStatus.PartialError;
        }

        /// <inheritdoc/>
        public void Ignore()
        {
            m_ack.Status = AckStatus.Ignore;
        }

        /// <inheritdoc/>
        public void SendProgress(string message, int progress)
        {
            //create a new ack for the progress report so the aggregation ack is not changed
            var p_ack = new AckMessage
            {
                OriginId = m_ack.OriginId,
                RowsPk = m_ack.RowsPk,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Status = AckStatus.Progress
            };
            p_ack.ErrorList.Add(new AckError
            {
                Message = message,
            });
            //passing this as a Action allows us to keep this class reusable by different providers            
            m_onProgress?.Invoke(p_ack);
        }
    }
}
