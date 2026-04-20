using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;


namespace CSGenio.core.persistence
{

    /// <summary>
    /// Class that obtains the various version information from the database
    /// </summary>
    public class DatabaseVersionReader : IVersionReader
    {

        private PersistentSupport sp;

        /// <summary>
        /// Creates a database version reader object. The Persistent Support is expected to be open.
        /// </summary>
        public DatabaseVersionReader(PersistentSupport sp)
        {
            this.sp = sp;
        }

        private object GetValueFromCfg(string columnName)
        {
            if (sp.CheckIfDatabaseExists())
            {
                string tableName = Configuration.Program + "cfg";
                SelectQuery query = new SelectQuery()
                    .Select(tableName, columnName)
                    .From(tableName);
                var value = sp.ExecuteScalar(query);
                return value;
            }
            else
            {
                throw new FrameworkException("The database doesn't exist", "", "");
            }
        }

        /// <summary>
        /// Returns the current database version for this database
        /// </summary>
        public int GetDbVersion()
        {
            var value = GetValueFromCfg("versao");
            var versionDb = DBConversion.ToInteger(value);
            return versionDb;
        }

        /// <summary>
        /// Returns the last version of the upgrade script executed for this function
        /// </summary>
        public int GetDbUpgradeVersion()
        {
            var value = GetValueFromCfg("upgrindx");
            int version = DBConversion.ToInteger(value);
            return version;
        }

        /// <summary>
        /// Returns the current version of the indexes in this database
        /// </summary>
        public int GetDbIndexVersion()
        {
            var value = GetValueFromCfg("versindx");
            var versionDb = DBConversion.ToInteger(value);
            return versionDb;
        }

        /// <summary>
        /// Returns the current version of the indexes in this database or 0 if there is an error.
        /// This method never throws an exception.
        /// </summary>
        public int GetDbVersionOrZero()
        {
            try
            {
                return GetDbVersion();
            }
            catch 
            {
                return 0;
            }

        }
		
		/// <summary>
		/// Returns the current version of the user settings in this database
		/// </summary>
		public int GetDbUserSettingsVersion()
		{
			try
			{
				var value = GetValueFromCfg("usrsetv");
				return DBConversion.ToInteger(value);
			}
			catch
			{
				return 0;
			}
		}

        /// <summary>
        /// Checks if the database is up to date
        /// </summary>
        /// <param name="user">The current user</param>
        /// <returns>true or false</returns>
        public static bool IsDatabaseUpToDate(User user)
        {
            if (user == null) return false;

            bool isValidVersion = Configuration.GetDbVersion(user.Year) == Configuration.VersionDbGen;
            bool isValidIndex = Configuration.GetDbUpgrIndx(user.Year) >= Configuration.VersionUpgrIndxGen;

            return isValidVersion && isValidIndex;
        }

        /// <summary>
        /// Checks if the configuration is up to date
        /// </summary>
        /// <returns>true or false</returns>
        public static bool IsConfigurationUpToDate()
        {
            return Configuration.ConfigVersion == GenioServer.framework.ConfigXMLMigration.CurConfigurationVerion.ToString();
        }

    }
}
