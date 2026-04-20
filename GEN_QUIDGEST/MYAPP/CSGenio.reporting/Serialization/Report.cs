using System;
using System.Collections.Generic;

namespace CSGenio.reporting.serialization
{
    [Serializable(), System.Xml.Serialization.XmlRoot("Report")]
    public class Report : SerializableBase
    {
        public List<DataSet> DataSets = new List<DataSet>();
        public List<ReportParameter> ReportParameters = new List<ReportParameter>();

        //override the constructor, so you don't have to cast it after deserializing it
        public static new Report Deserialize(string xml, Type type)
        {
            Report re;
            re = (Report)SerializableBase.Deserialize(xml, type);

            //copy the type-names from the ReportParameters to the QueryParameters
            re.ResolveParameterTypes();

            return re;
        }

        ///<summary>
        ///Gets a Report object based on the XML within the specified file
        ///</summary>
        public static Report GetReportFromFile(string reportFileName)
        {
            Report re = new Report();
            string xml;

            try
            {
                xml = System.IO.File.ReadAllText(reportFileName);
                re = (Report)Report.Deserialize(xml, typeof(Report));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                //ErrorHandling.ErrorLogger.LogException(ex, "ReportFileName=" & ReportFileName)
                throw;
            }

            return re;
        }

        /// <summary>
        /// map the report parameters to the query parameters
        /// </summary>
        private void ResolveParameterTypes()
        {
            foreach (ReportParameter rParam in this.ReportParameters)
            {
                foreach (DataSet ds in this.DataSets)
                    foreach (QueryParameter qParam in ds.Query.QueryParameters)
                    {
                        if (qParam.Value == "=Parameters!" + rParam.Name + ".Value")
                        {
                            qParam.DataType = rParam.DataType;
                        }
                    }
            }
        }
    }
}