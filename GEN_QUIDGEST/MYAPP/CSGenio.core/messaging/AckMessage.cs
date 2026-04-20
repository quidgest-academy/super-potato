using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CSGenio.core.messaging 
{

    public class AckMessage
    {
        public string OriginId { get; set; }
        public AckStatus Status { get; set; }
        public int Progress { get; set; }
        public string Target { get; set; }
        public long Timestamp { get; set; }

        public List<AckError> ErrorList { get; set; } = new List<AckError>();

        public Dictionary<string, List<string>> RowsPk { get; set; } = new Dictionary<string, List<string>>();

        public ReadOnlyMemory<byte> ToBytes()
        {
            using (MemoryStream ms = new MemoryStream(8192))
            {
                JsonSerializer.Serialize(ms, this);
                //avoid the memory copy of ToArray that trims excess buffer
                var body = ms.GetBuffer().AsMemory(0, (int)ms.Length);
                return body;
            }
        }
        public static AckMessage FromBytes(ReadOnlySpan<byte> message)
        {
            return JsonSerializer.Deserialize<AckMessage>(message);
        }
    }

    public class AckError
    {
        public string Table { get; set; }
        public string Id { get; set; }    
        public string Message { get; set; }

        public override string ToString() => $"{Table} {Id}: {Message}";
    }


    public enum AckStatus {
        Ok, //Everything was processed
        PartialError, //Somethings were processed others were not
        Progress, //The operation is in still in progress
        Ignore, //This operation should not reply with an business ack
        Error //Nothing was processed
    }

}