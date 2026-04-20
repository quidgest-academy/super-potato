using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using CSGenio.core.messaging;
using CSGenio.core.persistence;
using CSGenio.config;

namespace CSGenio.framework
{
    /// <summary>
    /// Contains the Genio generated system identification and versioning information and all the configurations.
    /// Exclusive use as a static class to supply a singleton access point for this data.
    /// </summary>
    public static class Configuration
    {
        public enum LoginTypes { AD, NORMAL, PUREAD, MANUAL };
		public enum PassEncTypes { ARG, QUI };
        public enum DbTypes { NORMAL, AUXILIAR, LOG, DEFINITION };

        /// <summary>
        /// Config version
        /// </summary>
        public static string ConfigVersion { get; private set; }

        /// <summary>
        /// Datasystems list
        /// </summary>
        public static List<DataSystemXml> DataSystems { get; private set; }

        /// <summary>
        /// List of auxiliary datasystems
        /// </summary>
        private static List<DataSystemXml> auxSystems;

        /// <summary>
        /// dicionario com as propriedades extra mapeadas por nome
        /// </summary>
        private static SerializableDictionary<string, string> maisPropriedades;

        //----------------------------------------------
        // System Identification and Versioning
        // Note: Version-related constants are defined in VersionInfo.cs.vm
        //----------------------------------------------

        /// <summary>
        /// Acronym of the client
        /// </summary>
        public static string Acronym { get; } = "QUIDGEST";

        /// <summary>
        /// Application version
        /// </summary>
        public static int Version { get; } = VersionInfo.Version;

        /// <summary>
        /// System id
        /// </summary>
        public static string Program { get; } = "FOR";

        /// <summary>
        /// Application subsystem
        /// </summary>
        public static readonly ClientApplication Application = ClientApplication.MYAPP;

        /// <summary>
        /// Modules
        /// </summary>
        public static readonly List<string> Modules = ["FOR"];

        /// <summary>
        /// Currency
        /// </summary>
        public static string Currency { get; } = "EUR";

        /// <summary>
        /// Login Type
        /// </summary>
        public static LoginTypes LoginType { get; private set; } = LoginTypes.NORMAL;

        /// <summary>
        /// Are fields saved as unicode in the database
        /// </summary>
        public const bool IsDbUnicode = false;

        /// <summary>
        /// Version of the database
        /// </summary>
        public const int VersionDbGen = VersionInfo.DatabaseSchema;

        /// <summary>
        /// Version of the database indexes
        /// </summary>
        public const int VersionIdxDbGen = VersionInfo.DatabaseIndex;

        /// <summary>
        /// Version of the latest upgrade index version
        /// </summary>
        public const int VersionUpgrIndxGen = VersionInfo.ChangeRoutines;

		/// <summary>
		/// Version of the latest user settings format
		/// </summary>
		public const int UserSettingsVersion = VersionInfo.UserSettings;

        /// <summary>
        /// Genio generator version
        /// </summary>
        public const string GenioVersion = VersionInfo.GenioVersion;

        /// <summary>
        /// Solution build version
        /// </summary>
        public static readonly int BuildVersionGen = VersionInfo.Build;

        /// <summary>
        /// Solution release version
        /// </summary>
        public static readonly string VersionTrackChangesGen = VersionInfo.Release;

        /// <summary>
        /// Agregate versioning information string
        /// </summary>
        public static string GenAssemblyVersion
        {
            get
            {
                return VersionInfo.GenAssemblyVersion;
            }
        }

        /// <summary>
        /// Should documents be saved on disk. False to save on the database
        /// </summary>
        public static bool Files2Disk { get; private set; } =  false;

        /// <summary>
        /// AI Configuration properties
        /// </summary>
        public static ChatBotCfg AiConfig { get; private set; } = null; 

        //----------------------------------------------
        // System services
        //----------------------------------------------

        /// <summary>
        /// Message queueing configuration
        /// </summary>
        public static messagequeueing MessageQueueing { get; private set; } = null;

        /// <summary>
        /// Security configuration
        /// </summary>
        public static SecurityCfgEl Security { get; private set; } = null;

        /// <summary>
        /// Audit configuration
        /// </summary>
        public static AuditCfgEl AuditTag { get; private set; }

        /// <summary>
        /// Log directory path
        /// </summary>
        public static string Log { get; private set; }

        /// <summary>
        /// Max number of days until the log is archived
        /// </summary>
        public static int MaxLogRowDays { get; private set; } = 0;

        /// <summary>
        /// GoogleMaps API Key
        /// </summary>
        public static string GoogleMapsKey { get; private set; }

        /// <summary>
        /// QA Environment
        /// </summary>
        public static int QAEnvironment { get; private set; }

        /// <summary>
        /// Path for persisting documents
        /// </summary>
        public static string PathDocuments { get; private set; }

        /// <summary>
        /// Ldap domain
        /// </summary>
        public static string Domain { get; private set; }

        /// <summary>
        /// Reporting Services server configuration
        /// </summary>
        public static SsrsServerXml SSRSServer { get; private set; }

        /// <summary>
        /// Path for managing the reports
        /// </summary>
        public static string PathReports { get; private set; }

        //----------------------------------------------
        // Email configuration
        //----------------------------------------------

        /// <summary>
        /// Email properties list that it's deserialized
        /// </summary>
        public static List<EmailServer> EmailProperties { get; private set; }

        public static EmailServer NewEmailServer()
        {
            var email = new EmailServer();
            int codpmail =  CSGenio.framework.Configuration.EmailProperties.Count + 1;
            email.Codpmail =  codpmail.ToString();
            return email;
        }

        /// <summary>
        /// Id of email server configuration used for UserRegistration
        /// </summary>
        public static string UserRegistrationEmail { get; private set; }

        /// <summary>
        /// Id of email server configuration used for passowrd recovery
        /// </summary>
        public static string PasswordRecoveryEmail { get; private set; }

        /// <summary>
        /// SMTP host address
        /// </summary>
        public static string PP_Email { get; private set; }

        /// <summary>
        /// Email default 'sent from' address
        /// </summary>
        public static string PP_Name { get; private set; }

        //----------------------------------------------
        // Data display configuration
        //----------------------------------------------

        /// <summary>
        /// Max number of login attempts
        /// </summary>
        public static int MaxAttempts { get; private set; } = 0;

        /// <summary>
        /// Default number of records per list page
        /// </summary>
        public static int NrRegDBedit { get; private set; } = 10;

        /// <summary>
        /// Number formating
        /// </summary>
        public static NumberFormatXml NumberFormat { get; private set; } = null;

        /// <summary>
        /// Date formating
        /// </summary>
        public static DateFormatXml DateFormat { get; private set; } = null;

        /// <summary>
        /// Event tracing feature
        /// </summary>
        public static bool EventTracking { get; private set; } = false;

        /// <summary>
        /// Publisher/Subscriber Messaging
        /// </summary>
        public static MessagingXml Messaging { get; private set; } = new MessagingXml();

        /// <summary>
        /// Scheduler configuration
        /// </summary>
        public static SchedulerXml Scheduler { get; private set; } = new SchedulerXml();


        //-----------------------------------------------
        /// <summary>
        /// ESTA PROPRIEDADE É PARA APAGAR O MAIS RAPIDO POSSIVEL!!!
        /// APENAS SERVE PARA QUE ALGUMAS FUNCIONALIDADES LEGACY CONTINUEM A COMPILAR.
        /// </summary>
        [Obsolete("O tipo de base de dados depende da conexao, não pode ser global")]
        public static DatabaseType LegacyConnectionType
        {
            get
            {
                return DataSystems[0].GetDatabaseType();
            }
        }
        //-----------------------------------------------


        /// <summary>
        /// File watcher that allows the system to ensure that the application pool is restarted when the configurations are changed
        /// </summary>
        public static FileSystemWatcher ConfigWatcher { get; private set; }

        /// <summary>
        /// Evaluate the redirection file and returns the redirected location
        /// </summary>
        /// <param name="redirectFilePath"></param>
        /// <returns>The location of the Configurations.Xml file</returns>
        static FileRedirect GetRedirectionPath(string redirectFilePath)
        {
            try
            {
                RedirectXML redirect = RedirectXML.ReadRedirectFile(redirectFilePath);
                if (redirect.files.Length == 1)
                    return redirect.files[0];
                else
                    return redirect.files.First(x => x.file.ToLower() == "configuracoes.xml");
            }
            catch (Exception)
            {
                throw new FrameworkException("The redirection file doens't contain path",
                "GetRedirectionPath",
                "Configuracoes.redirect.xml doesn't have a valid syntax");
            }
        }

        /// <summary>
        /// Returns the path to the folder with configuracoes.xml file
        /// </summary>
        /// <returns></returns>
        public static string GetConfigPath()
        {
            string defaultPath = AppDomain.CurrentDomain.BaseDirectory;

            //Check for config in BaseDirectory first
            string defaultConfig = Path.Combine(defaultPath, "Configuracoes.xml");
            if (File.Exists(defaultConfig))
                return defaultPath;

            //Check for redirect file in this same directory
            string defaultRedirect = Path.Combine(defaultPath, "configuracoes.redirect.xml");
            if (File.Exists(defaultRedirect))
            {
                FileRedirect redirect = GetRedirectionPath(defaultRedirect);
                string path = redirect.GetFullPath(defaultPath);

                //Try to get the file with the default path first
                if (File.Exists(Path.Combine(path, "Configuracoes.xml")))
                    return path;

                //If it does not exist, try with environment variable
                //This will happen when the CLI is in debug
                path = redirect.GetFullPath(Environment.CurrentDirectory);
                if (File.Exists(Path.Combine(path, "Configuracoes.xml")))
                    return path;
            }

            // Check for a custom path defined in env variable
            string envPath = Environment.GetEnvironmentVariable("CONFIG_PATH");
            if(envPath != null)
            {
                envPath = Path.Combine(envPath, "Configuracoes.xml");
                if (File.Exists(envPath)) return envPath;
            }

            return defaultPath;
        }

        /// <summary>
        /// Indicates if the configuration was actually loaded from the file or not
        /// </summary>
        public static bool Loaded
        {
            get; private set;
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static Configuration()
        {
            Reload();
        }

        /// <summary>
        /// Reloads the configuration from disk
        /// </summary>
        public static void Reload()
        {
           var configManager = new FileConfigurationManager(GetConfigPath());
           string path = configManager.GetFileLocation();
           if (configManager.Exists())
           {
               try
               {
                   ConfigurationXML readXML = configManager.GetExistingConfig();
                   ReadConfiguration(readXML);

                   //Add a file watcher that allows the system to ensure that the application pool is restarted when the configurations are changed
                   //The change event is configured in the Global.asax file

                   string directory = Path.GetDirectoryName(path);
                   ConfigWatcher = new FileSystemWatcher(directory, "Configuracoes.xml");
                   ConfigWatcher.EnableRaisingEvents = true;
                   Loaded = true;
               }
               catch (Exception ex)
                {
                    throw new FrameworkException("Erro ao ler o ficheiro de configurações:" + ex.Message, "Configuration.Configuration", "Error reading configuration file: " + ex.Message, ex);
                }
            }
            else
            {
                //JGF 2024.04.24 Reviewed this use case to not throw exception,
                //otherwise WebAdmin will not work while the configuration is not created
                //and there is no safe way to recover from exceptions in static constructors.
                //In this case a Configuration object with the default values is created.
                Loaded = false;
            }
        }

        /// <summary>
        /// Takes a parsed configuration file and sets its configurations as the active ones
        /// </summary>
        /// <param name="readXML">The parsed configuration file</param>
        public static void ReadConfiguration(ConfigurationXML readXML)
        {
            var path = readXML.GetPath(Configuration.Application.Id);
            PP_Email = readXML.email_pp;
            PP_Name = readXML.nome_pp;
            AiConfig = readXML.ChatBotConfig;
            Log = path.pathApp;
            GoogleMapsKey = readXML.googlemapsKey;
            QAEnvironment = Convert.ToInt16(readXML.qaEnvironment);
            if(!string.IsNullOrEmpty(readXML.tipoLogin))
                LoginType = (LoginTypes)Enum.Parse(typeof(LoginTypes), readXML.tipoLogin);
            PathReports = readXML.pathReports;
            PathDocuments = path.pathDocuments;
            Domain = readXML.dominio;
            SSRSServer = readXML.ssrsServer;

            maisPropriedades = readXML.maisPropriedades;

            NrRegDBedit = readXML.defaultDBeditRows;
            if (NrRegDBedit == 0)
                NrRegDBedit = 10;

            MaxLogRowDays = readXML.maxLogRowDays;

            ConfigVersion = readXML.ConfigVersion;

            //--------------------------------------------------
            DataSystems = readXML.DataSystems;
            auxSystems = readXML.AuxSystems;
			//--------------------------------------------------

            MessageQueueing = readXML.MessageQueueing;
            NumberFormat = readXML.NumberFormat ?? new NumberFormatXml();
            DateFormat = readXML.DateFormat ?? new DateFormatXml();
            //------------------------------------------------

            Security = readXML.GetSecurity(Application.Id);

            MaxAttempts = Security.MaxAttempts;

            //------------------------------------------------
            // audit
            AuditTag = readXML.Audit;
            // default values
            if (AuditTag == null)
            {
				AuditTag = new AuditCfgEl();
                AuditTag.RegistActions = false;
                AuditTag.RegistLoginOut = false;
            }

            EmailProperties = readXML.EmailProperties;
            UserRegistrationEmail = readXML.UserRegistrationEmail;
            PasswordRecoveryEmail = readXML.PasswordRecoveryEmail;

            EventTracking = readXML.EventTracking;

            Messaging = readXML.Messaging ?? new MessagingXml();
            Scheduler = readXML.Scheduler ?? new SchedulerXml();
        }

        /// <summary>
        /// Resolves a datasystem id into the corresponding information
        /// </summary>
        public static DataSystemXml ResolveDataSystem(string id, DbTypes db)
        {
			// USE /[MANUAL FOR RESOLVEDATASYSTEM]/
            DataSystemXml ds;
            switch (db)
            {
                case DbTypes.AUXILIAR:
                    ds = auxSystems.Find(x => x.Name == id);
                    break;
                case DbTypes.LOG:
                    ds =  DataSystems.Find(x => x.Name == id).DataSystemLog;
                    if (ds == null || ds.Schemas.Count == 0)
                        ds = DataSystems.Find(x => x.Name == id);
                    break;
                case DbTypes.NORMAL:
                default:
                    ds = DataSystems.Find(x => x.Name == id);
                    break;
            }
            if (ds == null)
                throw new FrameworkException("Erro ao configurar o sistema de dados.", "Configuration.ResolveDataSystem", "Data system with Id " + id + " and type " + db.ToString() + " not found.");
            return ds;
        }

        /// <summary>
        /// List of datasystem id's (aka Years)
        /// </summary>
        public static List<string> Years
        {
            get
            {
                List<string> res = new List<string>();
				if (DataSystems != null)
				{
					foreach (DataSystemXml ds in DataSystems)
						res.Add(ds.Name);
				}
                return res;
            }
        }

        /// <summary>
        /// Default datasystem id (aka Year)
        /// </summary>
        public static string DefaultYear
        {
            get { return DataSystems?.Count > 0 ? DataSystems[0].Name : "0"; }
        }

        /// <summary>
        /// Retrieve a property using its key.
        /// Throws exception if property key is not found.
        /// </summary>
        /// <param name="name">property key name</param>
        /// <returns>property value</returns>
        public static string GetProperty(string name)
        {
            if (!maisPropriedades.ContainsKey(name))
                throw new FrameworkException("Erro nas configurações.", "Configuration.GetProperty", "Property not found: " + name);
            return maisPropriedades[name];
        }


        /// <summary>
        /// Retrieve a property using its key.
        /// Returns the default value if property key is not found.
        /// </summary>
        /// <param name="name">property key name</param>
        /// <param name="default">property default value</param>
        /// <returns>property value</returns>
        public static string GetProperty(string name, string defaultValue)
        {
            if (!maisPropriedades.ContainsKey(name))
                return defaultValue;
            return maisPropriedades[name];
        }

        /// <summary>
        /// Check if a property exists.
        /// </summary>
        /// <param name="name">property key name</param>
        /// <returns>true if property exists, false otherwise</returns>
        public static bool ExistsProperty(string name)
        {
            return maisPropriedades.ContainsKey(name);
        }

        /// <summary>
        /// Queries the database for its currently set versioning information
        /// </summary>
        /// <param name="year">Datasystem id to check</param>
        /// <returns>The current database version number</returns>
        /// <remarks>This method will be marked as obsolete. Use the DatabaseVersionReader instead</remarks>
        public static int GetDbVersion(string year)
        {
            int versionDb;
            try
            {
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(year);
                var dvr = new DatabaseVersionReader(sp);
                sp.openConnection();
                versionDb = (int)dvr.GetDbVersion();
                sp.closeConnection();
            }
            catch
            {
                //we ignore errors for now (version will look as 0)
                versionDb = 0;
            }
            return versionDb;
        }

        /// <summary>
        /// Queries the database for its currently set upgrade index information
        /// </summary>
        /// <param name="year">Datasystem id to check</param>
        /// <returns>The current database upgrade index number</returns>
        /// <remarks>This method will be marked as obsolete. Use the DatabaseVersionReader instead</remarks>
		public static int GetDbUpgrIndx(string year)
        {
            try
            {
                var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(year);
                var dvr = new DatabaseVersionReader(sp);
                sp.openConnection();
                int upgrIndx = dvr.GetDbUpgradeVersion();
                sp.closeConnection();
                return upgrIndx;
            }
            catch
            {
                //we ignore errors for now (version will look as 0)
                return 0;
            }
        }

        /// <summary>
        /// Checks if there is any LdapIdentityProvider configured in the application
        /// </summary>
        /// <returns>True if there is any</returns>
        public static bool HasLdapIdentityProvider()
        {
            return Configuration.Security.IdentityProviders.Exists(p => p.Type.Equals("GenioServer.security.LdapIdentityProvider"));
        }
    }
}
