using System;

namespace CSGenio.reporting.serialization
{
    /// <summary>
    /// child of Query
    /// </summary>
    public class QueryParameter
    {
        [System.Xml.Serialization.XmlAttribute]
        public string Name;
        public string DataType;
        public string Value;
    }
}