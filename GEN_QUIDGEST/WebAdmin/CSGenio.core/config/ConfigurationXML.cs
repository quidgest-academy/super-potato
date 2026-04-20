using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GenioServer.security;
using System.Security;
using GenioServer.framework;
using CSGenio.config;
using System.Security.Cryptography;

namespace CSGenio
{
    /// <summary>
    /// Classe de Configuração to plataforma C#
    /// </summary>
	[XmlRoot("ConfigurationXML")]
    public class ConfigurationXML
    {
        public string servico="";
        public string email_pp="";
        public string nome_pp="";
        public string anoDefault = "";
        public string omiteAnos = "";
        public string pathReports = "";
        public string dominio = "";
        public string tipoLogin = "";
        public string googlemapsKey = "";
        public int qaEnvironment = 0;
        public int defaultDBeditRows = 0;
        public int maxLogRowDays = 0;
		public bool connEncrypt = false;
        public bool connWithDomainUser = false;

		/*
            XML Attributes
         */

        [XmlAttribute("configVersion")]
        public string ConfigVersion { get; set; }

        /*
            XML elements
        */
        [XmlElement("elasticsearch")]
        public ElasticsearchXml Elasticsearch { get; set; }

        [XmlElement("ChatBotConfig")]
        public ChatBotCfg ChatBotConfig { get; set; }

        public SsrsServerXml ssrsServer { get; set; }

        [XmlArray("DataSystems")]
        [XmlArrayItem("DataSystem")]
        public List<DataSystemXml> DataSystems { get; set; }

        [XmlArray("AuxSystems")]
        [XmlArrayItem("DataSystem")]
        public List<DataSystemXml> AuxSystems { get; set; }

        public SerializableDictionary<string, string> maisPropriedades = null;

        private messagequeueing m_mq = null;
        [XmlElement("MessageQueueing")]
        public messagequeueing MessageQueueing
        {
            get { return m_mq; }
            set { m_mq = value; }
        }

        private List<SecurityCfgEl> m_security = null;
        [XmlArray("Security")]
        [XmlArrayItem("AppSecurity")]
        public List<SecurityCfgEl> Security
        {
            get { return m_security; }
            set { m_security = value; }
        }

        public SecurityCfgEl GetSecurity(string appId)
        {
            SecurityCfgEl element = Security.Find(x => x.Application == appId);
            if (element == null)
            {
                SecurityCfgEl newElement = new SecurityCfgEl();

				//Some variables were not initialized for WebAdmin project (e.g. PasswordStrenght)
                if (appId == "WebAdmin" && Security.Count != 0)
                    newElement = (SecurityCfgEl) Security[0].Clone();

                newElement.IdentityProviders = new List<IdentityProviderCfgEl>();
                newElement.RoleProviders = new List<RoleProviderCfgEl>();
                newElement.Users = new List<UserCfgEl>();

                //Set default values for security configuration
                newElement.AuthenticationMode = GenioServer.security.AuthenticationMode.OneButtonPerProvider;

                IdentityProviderCfgEl identProvid = new IdentityProviderCfgEl();
                identProvid.Name = "quidgest";
                identProvid.Type = "GenioServer.security.QuidgestIdentityProvider";

                newElement.IdentityProviders.Add(identProvid);

                RoleProviderCfgEl roleProvid = new RoleProviderCfgEl();
                roleProvid.Type = "GenioServer.security.QuidgestRoleProvider";
                roleProvid.Precond = "providerName=quidgest";
                roleProvid.Name = "quidgest";
                newElement.RoleProviders.Add(roleProvid);

                UserCfgEl userCfg = new UserCfgEl();
                userCfg.AutoLogin = true;
                userCfg.Name = "guest";
                userCfg.Type = UserType.Guest;
                newElement.Users.Add(userCfg);
                newElement.Application = appId;
                Security.Add(newElement);
                return newElement;
            }
            else
                return element;
        }

        /// <summary>
        /// Checks if there is at least one application with 2FA
        /// </summary>
        public bool HasAnyApplicationWith2FA()
        {
            return Security.Any(s => s.IdentityProviders.Any(p => p.Is2FA));
        }

        private List<PathCfgEl> m_path = null;
        [XmlArray("Paths")]
        [XmlArrayItem("AppPath")]
        public List<PathCfgEl> Paths
        {
            get { return m_path; }
            set { m_path = value; }
        }

        public PathCfgEl GetPath(string appId)
        {
            PathCfgEl element = Paths.Find(x => x.Application == appId);
            if (element == null)
            {
                PathCfgEl newElement = new PathCfgEl();
                newElement.Application = appId;
                Paths.Add(newElement);
                return newElement;
            }
            else
                return element;
        }

        [XmlArray("EmailProperties")]
        [XmlArrayItem("EmailServer")]
        public List<EmailServer> EmailProperties
        {
            get;set;
        }

        private NumberFormatXml m_numberFormat = null;
        [XmlElement("numberFormat")]
        public NumberFormatXml NumberFormat
        {
            get { return m_numberFormat; }
            set { m_numberFormat = value; }
        }
        private DateFormatXml m_dateFormat = null;
        [XmlElement("dateFormat")]
        public DateFormatXml DateFormat
        {
            get { return m_dateFormat; }
            set { m_dateFormat = value; }
        }

        private AuditCfgEl m_audit = null;
        [XmlElement("audit")]
        public AuditCfgEl Audit
        {
            get { return m_audit; }
            set { m_audit = value; }
        }

        [XmlElement("GoogleMapsKey")]
        public string GoogleMapsKey
        {
            get { return googlemapsKey; }
            set { googlemapsKey = value; }
        }

        [CliProperty("qa-env", "Quality assurance environment mode (0=production, 1=QA environments)")]
        [XmlElement("QAEnvironment")]
        public int QAEnvironment
        {
            get { return qaEnvironment; }
            set { qaEnvironment = value; }
        }

        [XmlElement("UserRegistrationEmail")]
        public string UserRegistrationEmail {
            get; set;
        }

        [XmlElement("PasswordRecoveryEmail")]
        public string PasswordRecoveryEmail
        {
            get; set;
        }

        [XmlElement("EventTracking")]
        public bool EventTracking
        {
            get; set;
        }

        [XmlElement("Messaging")]
        public MessagingXml Messaging { get; set; } = new MessagingXml();


        [XmlElement("Scheduler")]
        public SchedulerXml Scheduler { get; set; } = new SchedulerXml();

        /*
            Functions
        */
        [Obsolete("Use IConfigurationManager.StoreConfig instead")]
        public void writeXML(string filename)
        {
            using(System.IO.StreamWriter output = new System.IO.StreamWriter(filename))
            {
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(this.GetType());
                s.Serialize(output, this);
            }
        }

        public void FillMissingConfigs()
        {
            //inicializamos também as secções que tenham ficado vazias
            if (DataSystems == null)
            {
                DataSystems = new List<DataSystemXml>();
            }

            if (Security == null)
            {
                Security = new List<SecurityCfgEl>();
            }

            if (Paths == null)
            {
                Paths = new List<PathCfgEl>();
            }

            if (ssrsServer == null)
            {
                ssrsServer = new SsrsServerXml();
            }

            if (maisPropriedades == null)
            {
                ExtraProperties.AddMissingValues(maisPropriedades);
            }

            if (Elasticsearch == null)
            {
                Elasticsearch = new ElasticsearchXml();
                Elasticsearch.Colours = new List<CoreXml>();
            }

            if (Messaging == null)
            {
                Messaging = new MessagingXml();
            }

            // Ensure ChatBotConfig has a JWT encryption key when JWT mode is enabled
            if (ChatBotConfig != null && ChatBotConfig.MCPSecurityMode == MCPSecurityMode.JWT && string.IsNullOrEmpty(ChatBotConfig.JWTEncryptionKey))
            {
                ChatBotConfig.JWTEncryptionKey = GenerateRandomJwtKey();
            }
        }
        
        /// <summary>
        /// Generates a cryptographically secure random JWT encryption key
        /// </summary>
        /// <returns>A base64-encoded 256-bit random key suitable for JWT signing</returns>
        private static string GenerateRandomJwtKey()
        {
            byte[] keyBytes = new byte[32]; // 256 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }
            return Convert.ToBase64String(keyBytes);
        }

        /// <summary>
        /// Encodes a secret value for secure storage
        /// </summary>
        /// <param name="value">The plain text value to encode</param>
        /// <returns>Encoded string suitable for storage/returns>
        public static string EncodeSecret(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Convert.ToBase64String(Encoding.Unicode.GetBytes(value));
        }

        /// <summary>
        /// Decodes a secret value from storage
        /// </summary>
        /// <param name="encodedValue">The encoded value from storage</param>
        /// <returns>Decoded plain text string</returns>
        public static string DecodeSecret(string encodedValue)
        {
            if (string.IsNullOrEmpty(encodedValue))
                return encodedValue;

            try
            {
                return Encoding.Unicode.GetString(Convert.FromBase64String(encodedValue));
            }
            catch (FormatException)
            {
                // The value is not valid
                return null;
            }
        }

        public void Init()
        {
            DataSystems = new List<DataSystemXml>();
            Security = new List<SecurityCfgEl>();
            Paths = new List<PathCfgEl>();
            ssrsServer = new SsrsServerXml();
            Elasticsearch = new ElasticsearchXml();
            Elasticsearch.Colours = new List<CoreXml>();
            maisPropriedades = ExtraProperties.GetInitialValues();
            ConfigVersion = ConfigXMLMigration.CurConfigurationVerion.ToString();
            ChatBotConfig = new ChatBotCfg();
            Messaging = new MessagingXml();
        }

        [Obsolete("Use IConfigurationManager.GetExistingConfig instead")]
        public static ConfigurationXML readXML(string filename)
        {
            ConfigurationXML conf;
            try
            {
                //ler o file
                using(System.IO.StreamReader input = new System.IO.StreamReader(filename))
                {
                    System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationXML));
                    conf = (ConfigurationXML)s.Deserialize(input);
                }
            }
            catch (Exception)
            {
                //Caso não seja possivel abrir do disco, inicializamos uma configuração vazia
                conf = new ConfigurationXML();
            }

            conf.FillMissingConfigs();

            return conf;
        }
    }

    public class ElasticsearchXml
    {
        [XmlElement("core")]
        public List<CoreXml> Colours { get; set; }

        public CoreXml GetCore(string id)
        {
            CoreXml c = null;
            for (int i = 0; i < Colours.Count; i++)
                if (Colours[i].Name == id)
                    c = Colours[i];
            return c;
        }
    }

    public enum MCPSecurityMode
    {
        JWT,
        None
    }

    [XmlRoot("ChatBotCfg")]
    public class ChatBotCfg
    {
        [CliProperty("ai-url", "Base URL for the AI service API endpoint. Should end in /api")]
        [XmlElement("apiURL")]
        public string APIEndpoint { get; set; }

        [CliProperty("mcp-security-mode", "Security mode for MCP (JWT or None)")]
        [XmlElement("MCPSecurityMode")]
        public MCPSecurityMode MCPSecurityMode { get; set; } = MCPSecurityMode.JWT;

        [XmlElement("JWTEncryptionKey")]
        /// <summary>
        /// Key used for JWT symmetric encription in MCP
        /// </summary>
        public string JWTEncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets the decoded JWT encryption key
        /// </summary>
        [CliProperty("jwt-encryption-key", "JWT encryption key for MCP security")]
        [XmlIgnore]
        public string JWTEncryptionKeyDecode
        {
            get
            {
                return ConfigurationXML.DecodeSecret(JWTEncryptionKey);
            }
            set
            {
                JWTEncryptionKey = ConfigurationXML.EncodeSecret(value);
            }
        }

        [CliProperty("mcp-url", "Mcp URL to be used by the AI service. Should end in /mcp")]
        [XmlElement("mcpURL")]
        public string AppMCPEndpoint { get; set; }
    }

    [XmlRoot("core")]
    public class CoreXml
    {
        [XmlAttribute("index")]
        public string Index { get; set; }

        [XmlAttribute("id")]
        public string Name { get; set; }

        [XmlAttribute("area")]
        public string Area { get; set; }

        [XmlAttribute("fscrawler")]
        public string Urlfscrawler { get; set; }

        [XmlText]
        public string Url { get; set; }

        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlAttribute("password")]
        public string Password
        {
            get => encodedPassword;
            set
            {
                encodedPassword = value;
                if (!string.IsNullOrEmpty(encodedPassword))
                {
                    var decodedPassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedPassword));
                    if (decodedPassword != null)
                    {
                        PasswordSecured = StringHelper.GetSecureString(decodedPassword);
                    }
                }
            }
        }

        private string encodedPassword;

        [XmlIgnore]
        public SecureString PasswordSecured { get; set; }
    }

    [XmlRoot("ssrsServer")]
    public class SsrsServerXml
    {
        [XmlText]
        public string url { get; set; }

        [XmlAttribute("path")]
        public string path { get; set; }

        [XmlAttribute("isLocalReports")]
        public bool isLocalReports { get; set; }

        // Report Server Credentials
        public bool ContainsCredentials()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Domain);
        }

        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlIgnore]
        public string UsernameDecode
        {
            get
            {
                return ConfigurationXML.DecodeSecret(Username);
            }
            set
            {
                Username = ConfigurationXML.EncodeSecret(value);
            }
        }

        [XmlAttribute("password")]
        public string Password { get; set; }

        [XmlIgnore]
        public string PasswordDecode
        {
            get
            {
                return ConfigurationXML.DecodeSecret(Password);
            }
            set
            {
                Password = ConfigurationXML.EncodeSecret(value);
            }
        }

        [XmlAttribute("domain")]
        public string Domain { get; set; }
    }

    [XmlRoot("DataSystem")]
    public class DataSystemXml
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        public string Type { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string TnsName { get; set; }
        public string Service { get; set; }
		public string ServiceName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Project { get; set; }
        public string Div { get; set; }
        public bool DatabaseSidePk { get; set; } = false;

		public DataSystemXml ShallowCopy() {

            var result = (DataSystemXml)MemberwiseClone();
            result.Schemas = Schemas.Select(x => (DataXml)x.ShallowCopy()).ToList();
            return result;
        }

        public string PasswordDecode()
        {
            return ConfigurationXML.DecodeSecret(Password);
        }
        public string LoginDecode()
        {
            return ConfigurationXML.DecodeSecret(Login);
        }

        public framework.DatabaseType GetDatabaseType()
        {
            if (string.IsNullOrEmpty(Type) || Type == "SQLSERVER2000" || Type == "SQLSERVER2005" || Type == "SQLSERVER2008")
                return framework.DatabaseType.SQLSERVERCOMPAT;
            return (framework.DatabaseType)Enum.Parse(typeof(framework.DatabaseType), Type, true);
        }

        [XmlElement("Data")]
        public List<DataXml> Schemas { get; set; }

        public DataSystemXml DataSystemLog { get; set; }

        [XmlIgnore]
        public List<string> UsersList { get; set; }
    }

    [XmlRoot("Data")]
    public class DataXml
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }
        [XmlAttribute("Schema")]
        public string Schema { get; set; }
		[XmlAttribute("ConnEncrypt")]
        public bool ConnEncrypt { get; set; }
		[XmlAttribute("ConnWithDomainUser")]
        public bool ConnWithDomainUser { get; set; }

		public DataXml ShallowCopy() { return (DataXml)MemberwiseClone(); }
    }

    [XmlRoot("MessageQueueing")]
    public class messagequeueing
    {
        private List<Queue> m_queues;
        private List<ACK> m_ACK;

        [XmlAttribute("journaltimeout")]
        public int Journaltimeout { get; set; }

        [XmlAttribute("maxsendnumber")]
        public int Maxsendnumber { get; set; }

        [XmlElement("Queue")]
        public List<Queue> Queues
        {
            get { return m_queues; }
            set { m_queues = value; }
        }
        [XmlElement("ACK")]
        public List<ACK> ACKS
        {
            get { return m_ACK; }
            set { m_ACK = value; }
        }

        public messagequeueing()
        {
            m_queues = new List<Queue>();
            m_ACK = new List<ACK>();
        }
    }


    [XmlRoot("Queue")]
    public class Queue
    {
        private string m_queue;
		private string m_channelId;
        private string m_queue_path;
        private string m_year;
        private bool m_unicode = false;
        private bool m_usesMsmq = false;
        private bool m_journal = false;
        private int m_block_size;

        public Queue() { }
        public Queue(string queue, string queue_path)
        {
            m_queue = queue;
            m_queue_path = queue_path;
        }

        [XmlAttribute(AttributeName = "id")]
        public string queue
        {
            get { return m_queue; }
            set { m_queue = value; }
        }

        [XmlAttribute(AttributeName = "channelId")]
        public string channelId
        {
            get { return string.IsNullOrEmpty(m_channelId) ? m_queue : m_channelId; }
            set { m_channelId = value; }
        }

        [XmlAttribute(AttributeName = "path")]
        public string path
        {
            get { return m_queue_path; }
            set { m_queue_path = value; }
        }

        [XmlAttribute(AttributeName = "year")]
        public string Qyear
        {
            get { return m_year; }
            set { m_year = value; }
        }

        [XmlAttribute(AttributeName = "unicode")]
        public bool Unicode
        {
            get { return m_unicode; }
            set { m_unicode = value; }
        }

        [XmlAttribute(AttributeName = "usesMsmq")]
        public bool UsesMsmq
        {
            get { return m_usesMsmq; }
            set { m_usesMsmq = value; }
        }

        [XmlAttribute(AttributeName = "journal")]
        public bool Journal
        {
            get { return m_journal; }
            set { m_journal = value; }
        }

        [XmlAttribute(AttributeName = "blocksize")]
        public int BlockSize
        {
            get { return m_block_size; }
            set { m_block_size = value; }
        }
    }

    [XmlRoot("ACK")]
    public class ACK
    {
        private string m_queue_source;
        private string m_queue_ack;
        private int m_block_size;

        public ACK() { }
        public ACK(string queueSource, string queueACK)
        {
            m_queue_source = queueSource;
            m_queue_ack = queueACK;
        }

        [XmlAttribute(AttributeName = "queuesource")]
        public string Source
        {
            get { return m_queue_source; }
            set { m_queue_source = value; }
        }

        [XmlAttribute(AttributeName = "ackqueue")]
        public string ACKqueue
        {
            get { return m_queue_ack; }
            set { m_queue_ack = value; }
        }

        [XmlAttribute(AttributeName = "blocksize")]
        public int BlockSize
        {
            get { return m_block_size; }
            set { m_block_size = value; }
        }
    }

    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable, ICloneable
    {
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                object ak = reader.GetAttribute("key");
                object av = reader.GetAttribute("value");
                var isEmpty = reader.IsEmptyElement;

                reader.ReadStartElement("item");

                //no attributes means we are using elements instead
                if (ak == null)
                {
                    reader.ReadStartElement("key");
                    ak = keySerializer.Deserialize(reader);
                    reader.ReadEndElement();
                }
                if (av == null)
                {
                    reader.ReadStartElement("value");
                    av = valueSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                }
                this.Add((TKey)ak, (TValue)av);

                if (!isEmpty)
                    reader.ReadEndElement();
                reader.MoveToContent();

            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool simplified = typeof(TKey) == typeof(string) && typeof(TValue) == typeof(string);

            foreach (TKey key in this.Keys)
            {
                //string dictionarys can be simplified a single element with attributes
                if (simplified)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("key", key.ToString());
                    writer.WriteAttributeString("value", this[key].ToString());
                    writer.WriteEndElement();
                }
                else //otherwise do a full key and value serialization
                {
                    writer.WriteStartElement("item");
                    writer.WriteStartElement("key");
                    keySerializer.Serialize(writer, key);
                    writer.WriteEndElement();
                    writer.WriteStartElement("value");
                    TValue value = this[key];
                    valueSerializer.Serialize(writer, value);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
        }

        #endregion
        
        public object Clone()
        {
            var res = new SerializableDictionary<TKey, TValue>();
            foreach (var kvp in this)
                res.Add(kvp.Key, kvp.Value);
            return res;
        }
    }

	[Serializable]
    [XmlRoot("AppSecurity")]
    public class SecurityCfgEl: ICloneable
    {
        private AuthenticationMode m_authenticationMode;
        private MultiSessionMode m_allowMultiSessionPerUser;
		private bool m_mandatory2FA = false;
        private bool m_allowAuthenticationRecovery;
        private bool m_expirationDateBool;
        private string m_expirationDate;
        private PasswordStrength m_passwordStrength;
        private PasswordAlgorithms m_passwordAlgorithms = PasswordAlgorithms.QUI; //tipo de encriptação de passwords
        private string m_minCaracters;
        private List<IdentityProviderCfgEl> m_identityProviders;
        private List<RoleProviderCfgEl> m_roleProviders;
        private List<UserCfgEl> m_users;
        private int m_maxAttempts = 0;
        public int m_sessionTimeOut;

        public SecurityCfgEl()
        {

        }

        public object Clone()
        {
            SecurityCfgEl security = new SecurityCfgEl();
            security.m_authenticationMode = m_authenticationMode;
            security.m_allowMultiSessionPerUser = m_allowMultiSessionPerUser;
            security.m_mandatory2FA = m_mandatory2FA;
            security.m_allowAuthenticationRecovery = m_allowAuthenticationRecovery;
            security.m_expirationDateBool = m_expirationDateBool;
            security.m_expirationDate = m_expirationDate;
            security.m_passwordStrength = m_passwordStrength;
            security.m_passwordAlgorithms = m_passwordAlgorithms;
            security.m_minCaracters = m_minCaracters;
            security.m_identityProviders = m_identityProviders.Select(identity => (IdentityProviderCfgEl)identity.Clone()).ToList();
            security.m_roleProviders = m_roleProviders.Select(identity => (RoleProviderCfgEl)identity.Clone()).ToList();
            security.m_users = m_users.Select(identity => (UserCfgEl)identity.Clone()).ToList();
            security.m_maxAttempts = m_maxAttempts;
            security.m_sessionTimeOut = m_sessionTimeOut;
            security.UsePasswordBlacklist = UsePasswordBlacklist;

            return security;
        }

        [XmlAttribute("application")]
        public String Application
        {
            get; set;
        }

        [XmlAttribute("authenticationMode")]
        public AuthenticationMode AuthenticationMode
        {
            get { return m_authenticationMode; }
            set { m_authenticationMode = value; }
        }

        [XmlAttribute("allowAuthenticationRecovery")]
        public bool AllowAuthenticationRecovery
        {
            get { return m_allowAuthenticationRecovery; }
            set { m_allowAuthenticationRecovery = value; }
        }

		[XmlAttribute("ExpirationDateBool")]
        public bool ExpirationDateBool
        {
            get { return m_expirationDateBool; }
            set { m_expirationDateBool = value; }
        }

        [XmlAttribute("ExpirationDate")]
        public string ExpirationDate
        {
            get { return m_expirationDate; }
            set { m_expirationDate = value; }
        }

        [XmlAttribute("MinCaracters")]
        public string MinCharacters
        {
            get
            {
                if (String.IsNullOrEmpty(m_minCaracters) || String.IsNullOrWhiteSpace(m_minCaracters))
                    return "7";
                else
                    return m_minCaracters;
            }
            set { m_minCaracters = value; }
        }

        [XmlAttribute("PasswordStrength")]
        public PasswordStrength PasswordStrength
        {
            get { return m_passwordStrength; }
            set { m_passwordStrength = value; }
        }

        [XmlAttribute("PasswordAlgorithms")]
        public PasswordAlgorithms PasswordAlgorithms
        {
            get { return m_passwordAlgorithms; }
            set { m_passwordAlgorithms = value; }
        }

        [XmlAttribute("allowMultiSessionPerUser")]
        public MultiSessionMode AllowMultiSessionPerUser
        {
            get { return m_allowMultiSessionPerUser; }
            set { m_allowMultiSessionPerUser = value; }
        }

		[XmlAttribute("Mandatory2FA")]
        public bool Mandatory2FA
        {
            get { return m_mandatory2FA; }
            set { m_mandatory2FA = value; }
        }

        [XmlArray("users"), XmlArrayItem("user", typeof(UserCfgEl))]
        public List<UserCfgEl> Users
        {
            get { return m_users; }
            set { m_users = value; }
        }

        [XmlArray("identityProviders"), XmlArrayItem("identityProvider", typeof(IdentityProviderCfgEl))]
        public List<IdentityProviderCfgEl> IdentityProviders
        {
            get { return m_identityProviders; }
            set { m_identityProviders = value; }
        }

        [XmlArray("roleProviders"), XmlArrayItem("roleProvider", typeof(RoleProviderCfgEl))]
        public List<RoleProviderCfgEl> RoleProviders
        {
            get { return m_roleProviders; }
            set { m_roleProviders = value; }
        }

        [XmlAttribute("maxAttempts")]
        public int MaxAttempts
        {
            get { return m_maxAttempts; }
            set { m_maxAttempts = value; }
        }

        [XmlAttribute("sessionTimeOut")]
        public int SessionTimeOut
        {
            get { return m_sessionTimeOut; }
            set { m_sessionTimeOut = value; }
        }
        
        [XmlAttribute("usePasswordBlacklist")]
        public bool UsePasswordBlacklist { get; set; } = false;
    }

    [XmlRoot("AppPath")]
    public class PathCfgEl
    {
        public string Application { get; set; }

        public string pathDocuments = "";
        public string pathApp = "";
        public SerializableDictionary<string, string> maisPropriedades = null;
    }

    public enum UserType
    {
        Regular,
        Guest,
        Admin
    }

    [XmlRoot("numberFormat")]
	public class NumberFormatXml
    {
        [XmlAttribute("decimalSeparator")]
        public string DecimalSeparator { get; set; } = ",";

        [XmlAttribute("groupSeparator")]
        public string GroupSeparator {get; set;} = " ";

        [XmlAttribute("negativeFormat")]
        public string NegativeFormat { get; set; } = "-";
    }


    [XmlRoot("dateFormat")]
    public class DateFormatXml
    {
        private string m_date = "dd/MM/yyyy";
        private string m_dateTime = "dd/MM/yyyy HH:mm";
        private string m_dateTimeSeconds = "dd/MM/yyyy HH:mm:ss";
        private string m_time = "HH:mm";

        [XmlAttribute("date")]
        public string Date
        {
            get { return m_date; }
            set { m_date = value; }
        }

        [XmlAttribute("dateTime")]
        public string DateTime
        {
            get { return m_dateTime; }
            set { m_dateTime = value; }
        }

        [XmlAttribute("dateTimeSeconds")]
        public string DateTimeSeconds
        {
            get { return m_dateTimeSeconds; }
            set { m_dateTimeSeconds = value; }
        }

        [XmlAttribute("time")]
        public string Time
        {
            get { return m_time; }
            set { m_time = value; }
        }
    }

	[Serializable]
    [XmlRoot("user")]
    public class UserCfgEl: ICloneable
    {
        private string m_name;
        private string m_password;
        private UserType m_type = UserType.Regular;
        private bool m_autoLogin = false;

        public object Clone()
        {
            UserCfgEl user = new UserCfgEl();
            user.m_name = m_name;
            user.m_password = m_password;
            user.m_type = m_type;
            user.m_autoLogin = m_autoLogin;

            return user;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        [XmlAttribute("password")]
        public string Password
        {
            get { return m_password; }
            set { m_password = value; }
        }

        [XmlAttribute("type")]
        [DefaultValueAttribute(UserType.Regular)]
        public UserType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        [XmlAttribute("autoLogin")]
        [DefaultValueAttribute(false)]
        public bool AutoLogin
        {
            get { return m_autoLogin; }
            set { m_autoLogin = value; }
        }
    }

    [Serializable]
    [XmlRoot("identityProvider")]
    public class IdentityProviderCfgEl : ICloneable
    {
        public object Clone()
        {
            IdentityProviderCfgEl identity = new IdentityProviderCfgEl
            {
                Name = Name,
                Description = Description,
                Type = Type,
                Is2FA = Is2FA,
                Options = Options.Clone() as SerializableDictionary<string, string>
            };

            return identity;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("is2fa")]
        public bool Is2FA { get; set; } = false;
        
        [XmlElement("options")]
        public SerializableDictionary<string, string> Options { get; set; } = new SerializableDictionary<string, string>();
    }

	[Serializable]
    [XmlRoot("roleProvider")]
    public class RoleProviderCfgEl: ICloneable
    {
        public object Clone()
        {
            RoleProviderCfgEl role = new RoleProviderCfgEl()
            {
                Name = Name,
                Type = Type,
                Options = Options.Clone() as SerializableDictionary<string, string>,
                Precond = Precond
            };

            return role;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("options")]
        public SerializableDictionary<string, string> Options { get; set; } = new SerializableDictionary<string, string>();

        [XmlAttribute("precond")]
        public string Precond { get; set; }
    }

	[XmlRoot("moreProperty")]
    public class MorePropertyCfgEl
    {
        private string m_key;
        private string m_value;

        [XmlAttribute("key")]
        public string Key
        {
            get { return m_key; }
            set { m_key = value; }
        }

        [XmlAttribute("value")]
        public string Val
        {
            get { return m_value; }
            set { m_value = value; }
        }
    }

    [XmlRoot("audit")]
    public class AuditCfgEl
    {
        private bool m_registLogin;
		private bool m_auditInterface;
        private bool m_registActions;

        [XmlElement("loginout")]
        public bool RegistLoginOut
        {
            get { return m_registLogin; }
            set { m_registLogin = value; }
        }

        [XmlElement("auditInterface")]
        public bool AuditInterface
        {
            get { return m_auditInterface; }
            set { m_auditInterface = value; }
        }

        [XmlElement("actions")]
        public bool RegistActions
        {
            get { return m_registActions; }
            set { m_registActions = value; }
        }
    }

    [XmlRoot]
    public class EmailServer
    {

        [XmlAttribute("id")]
        public string Id;

        [XmlAttribute("codpmail")]
        public string Codpmail;

        [XmlElement]
        public string Name;

        [XmlElement]
        public string From;

        [XmlElement]
        public string SMTPServer;

        [XmlElement]
        public int Port;

        /// <summary>
        /// Use STARTTLS
        /// </summary>
        [XmlElement]
        public bool SSL;

        [XmlElement]
        public AuthType AuthType;

        [XmlElement]
        public string Username;

        [XmlElement]
        public string Password;

        [XmlElement(IsNullable = false)]
        public OAuth2Options OAuth2Options = null;

        [XmlArray("NotificationMessages")]
        [XmlArrayItem("NotificationMessagesPK")]
        public List<string> NotificationMessages;

        public EmailServer()
        {

        }

    }

    [XmlRoot("Messaging")]
    public class MessagingXml
    {
        [XmlAttribute]
        public bool Enabled { get; set; } = false;

        [XmlElement]
        public MessagingHostXml Host { get; set; } = new MessagingHostXml();

        [XmlArray("Publications")]
        [XmlArrayItem("Pub")]
        public List<string> EnabledPublications { get; set; } = new List<string>();

        [XmlArray("Subscriptions")]
        [XmlArrayItem("Sub")]
        public List<string> EnabledSubscriptions { get; set; } = new List<string>();
    }

    [XmlRoot("Host")]
    public class MessagingHostXml
    {
        [XmlElement]
        public string Provider { get; set; } = "RabbitMq";
        [XmlElement]
        public string Endpoint { get; set; } = "amqp://localhost";
        [XmlElement]
        public string Username { get; set; } = string.Empty;
        [XmlElement]
        public string Password { get; set; } = string.Empty;

        public string PasswordDecode()
        {
            return ConfigurationXML.DecodeSecret(Password);
        }
        public string UsernameDecode()
        {
            return ConfigurationXML.DecodeSecret(Username);
        }

    }

    [XmlRoot("Scheduler")]
    public class SchedulerXml
    {
        [XmlAttribute]
        public bool Enabled { get; set; } = false;

        [XmlArray("Jobs")]
        [XmlArrayItem("Job")]
        public List<SchedulerJobXml> Jobs { get; set; } = new List<SchedulerJobXml>();
    }

    [XmlRoot("Job")]
    public class SchedulerJobXml
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string TaskType { get; set; }
        /// <summary>
        /// Format is:
        ///   Second Minute Hour DayOfMonth Month DayOfWeek
        ///     * for all values
        ///     x-y for a range between x and y
        ///     x,y for a list of x and y values
        ///     */n for running at every n'th value
        /// </summary>
        /// <seealso cref="https://github.com/HangfireIO/Cronos"/>
        [XmlAttribute]
        public string Cron { get; set; }
        [XmlAttribute]
        public bool Enabled { get; set; } = true;

        public SerializableDictionary<string, string> Options { get; set; } = null;
    }

}
