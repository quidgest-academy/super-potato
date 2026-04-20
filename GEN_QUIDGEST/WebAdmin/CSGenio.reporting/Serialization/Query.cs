using System;
using System.Collections.Generic;

namespace CSGenio.reporting.serialization
{
    /// <summary>
    /// child of DataSet
    /// </summary>
    [Serializable()]
    public class Query
    {
        public string DataSourceName;
        public List<QueryParameter> QueryParameters = new List<QueryParameter>();
		public string CommandType;
        public string CommandText;
    }
}