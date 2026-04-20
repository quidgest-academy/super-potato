using System.Runtime.Serialization;
using GenioServer.business;

namespace Administration.Models
{
    [DataContract]
    public class QApiCallAck
    {
        [DataMember]
        public string MsgId { get; set; }

        [DataMember]
        public string Desc { get; set; }

        [DataMember]
        public CallAck Ack { get; set; }

        [DataMember]
        public string AckQueue { get; set; }
    }
}