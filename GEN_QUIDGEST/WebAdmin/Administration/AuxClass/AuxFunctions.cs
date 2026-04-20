using CSGenio.framework;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using CSGenio.config;
using IConfigurationManager = CSGenio.config.IConfigurationManager;

namespace Administration.AuxClass
{
    public static class AuxFunctions
    {


        public static decimal GetDBSize(string year, string Schema)
        {
            decimal sizeIdxDb = 0;
            try
            {
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(year);
                sp.openConnection();
                string qs;
                if (sp.DatabaseType == DatabaseType.ORACLE)
                {
                    qs = "select BYTES/1024/1024 SizeMB from SYS.DBA_DATA_FILES WHERE TABLESPACE_NAME = '" + Schema + "'";
                }
                else if (sp.DatabaseType == DatabaseType.MYSQL)
                {
                    qs = "SELECT ROUND(SUM(data_length + index_length) / 1024 / 1024, 1) SizeMB FROM information_schema.tables where table_schema = '" + Schema + "' GROUP BY table_schema";
                }
                else if (sp.DatabaseType == DatabaseType.POSTGRES)
                {
                    qs = "select pg_database_size('" + Schema + "')/ 1024 / 1024 SizeMB;";
                }
                else
                {
                    qs = "SELECT (size * 8) / 1024 SizeMB FROM sys.master_files WHERE database_id = DB_ID('" + Schema + "')";
                }

                sizeIdxDb = CSGenio.persistence.DBConversion.ToNumeric(sp.executeScalar(qs));

                sp.closeConnection();
            }
            catch
            {
                //we ignore errors for now (version will look as 0)
            }

            return sizeIdxDb;
        }	

		public static int GetConfigVersion(IConfigurationManager configManager)
        {
            try
            {
                var config = configManager.GetExistingConfig();
                return int.Parse(config.ConfigVersion);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        
        public static bool CheckXMLIsValid(IConfigurationManager configManager)
        {

            //check if file exists
            if (!configManager.Exists())
                return false;

            int version = GetConfigVersion(configManager);
            if (version == -1 || version != GenioServer.framework.ConfigXMLMigration.CurConfigurationVerion)
                return false;

            return true;
        }
		
		public static CSGenio.persistence.PersistentSupport GetPersistentSupport(IConfigurationManager configManager, string year)
        {
            var conf = configManager.GetExistingConfig();
            var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == year);
            return CSGenio.persistence.PersistentSupport.getPersistentSupport(dataSystem.Name);
        }


        #region Helpers
        private static string GetEnumDisplayName<TEnum>(object item)
        {
            string res = item.ToString();
            var da = ((DisplayAttribute)(typeof(TEnum).GetField(res).GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault()));
            return da == null ? res : da.GetName();
        }

        public class SelectlistElement
		{
			public SelectlistElement() { }
			public string Text { get; set; }
			public object Value { get; set; }
			public bool Selected { get; set; }
		}

		public static IEnumerable<SelectlistElement> ToSelectList<TEnum>(object selected = null)
		{
			return Enum.GetValues(typeof(TEnum)).Cast<IFormattable>().Select(v =>
			new SelectlistElement()
			{
				Text = GetEnumDisplayName<TEnum>(v),
				Value = Convert.ToInt32(v.ToString("d", null)),
				Selected = selected == null ? false : (v.ToString() == selected.ToString())
			});
		}
        #endregion
    }
}
