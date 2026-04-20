using CSGenio.persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace CSGenio.framework
{
    public class MQXml
    {
        private static string get_type(Field cp)
        {
            return get_type(cp.FieldType);
        }

        private static string get_type(FieldType cp)
        {
            switch (cp.GetFormatting())
            {
                case FieldFormatting.CARACTERES:
                case FieldFormatting.TEMPO:
                case FieldFormatting.GUID:
                case FieldFormatting.BINARIO:
                case FieldFormatting.JPEG:
                    return "C";
                case FieldFormatting.DATA:
				case FieldFormatting.DATAHORA:
                case FieldFormatting.DATASEGUNDO:
                    return "D";
                case FieldFormatting.FLOAT:
                case FieldFormatting.INTEIRO:
                    return "N";
                case FieldFormatting.LOGICO:
                    return "L";
                default:
                    throw new FrameworkException(null, "MQXml.get_type", "Format undefined: " + cp.GetFormatting().ToString());
            }
        }

        private static FieldType get_tipoCampo(Type cp)
        {
            switch(cp.Name)
            {
                case "String":
                    return FieldType.TEXT;
                case "DateTime":
                    return FieldType.DATE;
                case "float":
                case "int":
                    return FieldType.NUMERIC;
                case "bool":
                    return FieldType.LOGIC;
                default:
					throw new FrameworkException(null, "MQXml.get_tipoCampo", "Type undefined: " + cp.Name);
            }
        }

        private static XmlElement FieldPropertieAdd(XmlDocument xml, string name, string Qvalue)
        {
            XmlElement xml_elem_propertie = xml.CreateElement(name);
            XmlText xml_text = xml.CreateTextNode(Qvalue);
            xml_elem_propertie.AppendChild(xml_text);
            return xml_elem_propertie;
        }

        private static XmlElement FieldPropertieValueAdd(XmlDocument xml, string name, object Qvalue, FieldFormatting tipo)
        {
            XmlElement xml_elem_propertie = xml.CreateElement(name);
            if (tipo == FieldFormatting.CARACTERES)
            {
                XmlCDataSection node = xml.CreateCDataSection(EncodeXML(DBConversion.ToString(Qvalue)));
                xml_elem_propertie.AppendChild(node);
            }
			else if (tipo == FieldFormatting.TEMPO)
            {
                XmlCDataSection node = xml.CreateCDataSection(DBConversion.ToString(Qvalue));
                xml_elem_propertie.AppendChild(node);
            }
            else if (tipo == FieldFormatting.DATA || tipo == FieldFormatting.DATAHORA || tipo == FieldFormatting.DATASEGUNDO)
            {
                DateTime data;
                string data_nova = "";

                if (Qvalue == null || Qvalue.ToString().Length == 0 || DateTime.Parse(Qvalue.ToString()) == DateTime.MinValue)
                    //antes colocava data 01-01-1900
                    data = DateTime.MinValue;
                else
                    data = DateTime.Parse(Qvalue.ToString());

                string format = "";
                if (tipo == FieldFormatting.DATA)
                    format = "dd/MM/yyyy";
                else if (tipo == FieldFormatting.DATAHORA)
                    format = "dd/MM/yyyy HH:mm";
                else if (tipo == FieldFormatting.DATASEGUNDO)
                    format = "dd/MM/yyyy HH:mm:ss";				

                //foi reestruturado to ficar coerente com o backoffice
                //se a data for inválida, o Qvalue deve ir vazio to o xml
                if (data != DateTime.MinValue)
                    data_nova = data.ToString(format, System.Globalization.CultureInfo.InvariantCulture);// String.Format("{0:MM/dd/yyyy}", data);

                XmlText xml_text = xml.CreateTextNode(data_nova);// data.ToString("dd/MM/yyyy"));//esta porra esta a meter "-" em vez de "/"... resolver...!!!!!!!!                xml_elem_propertie.AppendChild(xml_text);
                xml_elem_propertie.AppendChild(xml_text);
            }
            else if (tipo == FieldFormatting.FLOAT)
            {
                //acrescentada a formatação da string com InvariantCulture to garantir coerência com a receção da queue
                var strVal = DBConversion.ToNumeric(Qvalue).ToString(System.Globalization.CultureInfo.InvariantCulture);
                XmlText xml_text = xml.CreateTextNode(strVal);
                xml_elem_propertie.AppendChild(xml_text);
            }
            else
            {
                XmlText xml_text = xml.CreateTextNode(DBConversion.ToString(Qvalue));
                xml_elem_propertie.AppendChild(xml_text);
            }
            return xml_elem_propertie;
        }
		
        public static string EncodeXML(string unencodedText) 
        {
            StringBuilder encodedText = new StringBuilder();
  	        for (int i=0; i<unencodedText.Length; i++)
	        {
                if ((unencodedText[i] > 126 || unencodedText[i] < 32) && unencodedText[i] != '\t' && unencodedText[i] != '\n' && unencodedText[i] != '\r')
                    encodedText.AppendFormat("&#{0:d5}", (uint)unencodedText[i]);
                else
                    encodedText.AppendFormat("{0}", unencodedText[i]);
	        }
	        return encodedText.ToString();
        }

		public static string DecodeXML(string encodedText)
        {
            StringBuilder unencodedText = new StringBuilder();
            char wch;
            for (int i = 0; i < encodedText.Length; i++)
            {
                if (encodedText[i] == '&' && i + 6 <= encodedText.Length && encodedText[i + 1] == '#')
                {
                    wch = (char)UInt32.Parse(encodedText.Substring(i + 2, 5));
                    unencodedText.Append(wch);
                    i += 6;
                }
                else
                    unencodedText.Append(encodedText[i]);
            }
            return unencodedText.ToString();
        }

        public static XmlElement FieldAdd(XmlDocument xml, string name, object value)
        {
            return FieldAdd(xml, name, get_tipoCampo(value.GetType()), value, 500);
        }

        public static XmlElement FieldAdd(XmlDocument xml, string name, Field Qfield, object value)
        {
            return FieldAdd(xml, name, Qfield.FieldType, value, Qfield.FieldSize);
        }
		
		public static XmlElement FieldAdd(XmlDocument xml, string name, FieldType type, object value, int lenght)
        {
            XmlElement xml_elem = xml.CreateElement(name);

            xml_elem.AppendChild(FieldPropertieAdd(xml, "TIPO", MQXml.get_type(type)));
            xml_elem.AppendChild(FieldPropertieAdd(xml, "COMP", lenght.ToString()));

            if (type == FieldType.NUMERIC || type == FieldType.CURRENCY)
                xml_elem.AppendChild(FieldPropertieAdd(xml, "DC", value.ToString()));

            xml_elem.AppendChild(FieldPropertieValueAdd(xml, "VL", value, type.GetFormatting()));

            return xml_elem;
        }

        //RS 2020.03.05 Temporary drop support for MSMQ, this code should change to a provider based code
        // where a channel provider concrete implementation is passed into this class and handles the actual
        // broadcast of the message, and this class only build up the message to send.
		//public static void SendMSMQ(string endpoint, byte[] m)
  //      {
  //          System.Messaging.MessageQueue messageQ = new System.Messaging.MessageQueue(endpoint);
  //          System.Messaging.Message messageM = new System.Messaging.Message();
  //          messageM.BodyStream = new MemoryStream(m);
  //          messageQ.Send(messageM);
  //      }

        public static string GetTaskXml(string schemaMapping, string queue_guid, string taskName, List<KeyValuePair<string, object>> arguments)
        {
            XmlDocument xml = new XmlDocument();
            XmlElement xmlMainElem;//nós de 1º level

            XmlElement xml_root = xml.DocumentElement;

            var configQueue = Configuration.MessageQueueing.Queues.Find(x => x.queue == taskName && x.Qyear == schemaMapping);
            //string queue_guid = Guid.NewGuid().ToString("N");

            XmlDeclaration xml_decl;
            if (configQueue.Unicode)
                xml_decl = xml.CreateXmlDeclaration("1.0", "UTF-16", null);
            else
                xml_decl = xml.CreateXmlDeclaration("1.0", "ISO-8859-1", null);

            xmlMainElem = xml.CreateElement("", "mqrec", "");
            xml.InsertBefore(xml_decl, xml_root);
            xmlMainElem.SetAttribute("table", taskName.ToUpperInvariant());
            xmlMainElem.SetAttribute("guid", queue_guid);
            xmlMainElem.SetAttribute("sistema", Configuration.Program);
            xmlMainElem.SetAttribute("queue", taskName);
            xmlMainElem.SetAttribute("tp", "F");
            xmlMainElem.SetAttribute("year", schemaMapping);

            foreach (var arg in arguments)//percorre area corrente
            {
                xmlMainElem.AppendChild(MQXml.FieldAdd(xml, arg.Key, arg.Value));
                xml.AppendChild(xmlMainElem);
            }

            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            xml.WriteTo(tx);

            return sw.ToString();
        }
    }
}
