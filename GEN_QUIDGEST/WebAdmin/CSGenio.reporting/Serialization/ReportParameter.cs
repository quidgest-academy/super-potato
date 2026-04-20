using System;

namespace CSGenio.reporting.serialization
{
    /// <summary>
    /// child of Report
    /// </summary>
    [Serializable()]
    public class ReportParameter
    {
        [System.Xml.Serialization.XmlAttribute]
        public string Name;
        public string DataType;
    }
}