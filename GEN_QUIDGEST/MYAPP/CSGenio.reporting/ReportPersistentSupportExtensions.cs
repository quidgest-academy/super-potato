using CSGenio.framework;
using CSGenio.persistence;
using System;
using System.Data;

namespace CSGenio.reporting
{
    public static class ReportPersistentSupportExtensions
    {
        public static DataTable getDataSourceForLocalSSRS(this PersistentSupport sp, CSGenio.reporting.serialization.DataSet ds)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("getDataSourceForLocalSSRS - [QueryDataSet] {0}.", ds.Query.CommandText));

                IDbDataAdapter da = sp.CreateAdapter(ds.Query.CommandText);

                if (ds.Query.QueryParameters.Count != 0)
                {
                    foreach (var param in ds.Query.QueryParameters)
                    {
                        string paramName = param.Name.Replace("@", "");
                        var p = sp.CreateParameter(paramName, param.Value);
                        switch (param.DataType)
                        {
                            case "Text":
                                p.DbType = !Configuration.IsDbUnicode ? DbType.AnsiString : DbType.String;
                                break;
                            case "Boolean":
                                p.DbType = DbType.Binary;
                                break;
                            case "DateTime":
                                p.DbType = DbType.DateTime;
                                break;
                            case "Integer":
                                p.DbType = DbType.Int32;
                                break;
                            case "Float":
                                p.DbType = DbType.Decimal;
                                break;
                        }
                        da.SelectCommand.Parameters.Add(p);
                    }
                }

                if(ds.Query.CommandType== "StoredProcedure")
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;

                var dataSet = new DataSet();
                da.Fill(dataSet);

                var dt = dataSet.Tables[0];
                dt.TableName = ds.Name;

                return dt;
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.getDataSourceForLocalSSRS", "Error: " + ex.Message, ex);
            }
        }
    }
}
