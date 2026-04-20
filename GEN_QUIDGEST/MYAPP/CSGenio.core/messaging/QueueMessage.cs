using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CSGenio.core.messaging
{

    public class QueueMessage
    {
        public QueueMessageEnvelope Envelope { get; set; }
        public List<QueueTable> Tables { get; set; } = new List<QueueTable>();

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

        public static QueueMessage FromBytes(ReadOnlySpan<byte> message)
        {
            var res = JsonSerializer.Deserialize<QueueMessage>(message);
            // Low level parsing to unwrap the JsonElement object uppon unwrapping the message.
            // Only supports the json base types: bool, number, string and null.
            // Other data types should be converted to these base types before transport layer is called.
            // Ex: dates should be converted to strings before serialization, and parsed after deserialization by this class.
            foreach (var t in res.Tables)
                foreach (var row in t.Rows)
                    for (int i = 0; i < row.Length; i++)
                        row[i] = ParseJsonElement(row[i]);

            return res;
        }

        private static object ParseJsonElement(object value)
        {
            if (value is JsonElement jo)
            {
                if (jo.ValueKind == JsonValueKind.String)
                    return jo.GetString() ?? "";
                else if (jo.ValueKind == JsonValueKind.Number)
                    return jo.GetDecimal();
                else if (jo.ValueKind == JsonValueKind.True)
                    return true;
                else if (jo.ValueKind == JsonValueKind.False)
                    return false;
                else if (jo.ValueKind == JsonValueKind.Null)
                    return null;
            }

            return null;
        }
    }

    public class QueueMessageEnvelope
    {
        public string Id { get; set; }
        public long Timestamp { get; set; }
        public string Process { get; set; }
        public string Dataset { get; set; }
        public int Version { get; set; }
    }

    public class QueueTable
    {
        /// <summary>
        /// Name of the table
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Updated or inserted rows
        /// </summary>
        public List<object[]> Rows { get; set; } = new List<object[]>();
        /// <summary>
        /// List of deleted rows
        /// </summary>
        public List<string> RowsDelete { get; set; } = new List<string>();
        /// <summary>
        /// Column metadata built into the message to allow
        ///  for some level of retro-compatibility
        /// </summary>
        public List<QueueField> Columns { get; set; } = new List<QueueField>();
    }

    public class QueueField
    {
        public string Id { get; set; }
        public string DataType { get; set; }
    }




}