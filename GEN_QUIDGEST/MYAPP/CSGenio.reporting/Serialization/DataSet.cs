using System;
using System.Collections.Generic;

namespace CSGenio.reporting.serialization
{
    [Serializable()]
    public class DataSet
    {
        [System.Xml.Serialization.XmlAttribute]
        public string Name;
        public Query Query = new Query();
        public List<Field> Fields = new List<Field>();

        /// <summary>
        /// copy the query parameters values (probably from the QueryString) into this report's parameters (in this object)
        /// </summary>
        /// <param name="webParameters"></param>
        public void AssignParameters(Dictionary<string,string> webParameters)
        {
            foreach (QueryParameter param in this.Query.QueryParameters)
            {
                //If the parameter is a literal, the value is already defined
                if(param.Value.StartsWith("="))
                {
                    //gets the mapped report parameter (mapping is in the following format =Parameters!gproc_codgproc.Value)
                    string mappedParameter = param.Value.Replace("=Parameters!", "").Replace(".Value", "");

                    //if the mapped parameter was passed to the report, then populate it
                    if (webParameters.ContainsKey(mappedParameter) && webParameters[mappedParameter]!=null)
                        param.Value = webParameters[mappedParameter].ToString();
                }
            }
        }

    }
}