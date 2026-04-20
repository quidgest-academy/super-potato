using System;

namespace CSGenio.reporting.serialization
{
    /// <summary>
    /// child of DataSet
    /// </summary>
    [Serializable()]
    public class Field
    {
        [System.Xml.Serialization.XmlAttribute]
        public string Name;
        public string DataField;
        public string TypeName;
    }
}