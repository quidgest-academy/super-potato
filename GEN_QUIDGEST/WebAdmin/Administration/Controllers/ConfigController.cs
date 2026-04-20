using Administration.AuxClass;
using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using System.Globalization;
using System.Text;
using System.Net;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using DbAdmin;
using CSGenio.config;
using System.Data.SqlClient;
using CSGenio.persistence;
using System.Data;
using Quidgest.Persistence.GenericQuery;
using CSGenio.core.messaging;
using Administration.Models;
using GenioServer.security;

namespace Administration.Controllers
{
    public class ConfigController(CSGenio.config.IConfigurationManager configManager) : ControllerBase
    {

        [HttpGet]
        public IActionResult Index()
        {
            var appId = FromQuery("appId");
            return Index(null, appId);
        }

        private IActionResult Index(string resultMsg, string appId, string alertType = null)
        {
            //If configuration file doesn't exist, redirect to no config to be created
            if(!configManager.Exists())
                return Json(new { redirect = "no_configuration" });

            if (!AuxFunctions.CheckXMLIsValid(configManager))
                return Json(new { redirect = "config_migration" });

            var model = new Models.ConfigModel();
            model.Applications = ClientApplication.Applications;
            model.AlertType = alertType ?? "";
            model.ResultMsg = resultMsg ?? "";

            try
            {
                var conf = configManager.GetExistingConfig();

                //----------------
                // Database
                //----------------
                DataSystemXml dataSystem;
                if (!conf.DataSystems.Any()) // Se não houver nenhum DataSystem, cria um default.
                    dataSystem = createDataSystem(Configuration.DefaultYear, $"{Configuration.Program}{Configuration.DefaultYear}", conf);
                else 
                    dataSystem = Configuration.ResolveDataSystem(CurrentYear, Configuration.DbTypes.NORMAL);

                //----------------
                // Read System and Log Database Configurations
                //----------------
                ReadDataBaseConfig(model, dataSystem, conf);

                model.DefaultYear = CSGenio.framework.Configuration.DefaultYear;

                //----------------
                // Security
                //----------------
                model.Security = new Dictionary<string, Models.SecurityCfg>();
                foreach (var app in model.Applications)
                {
                    if (conf.Security.Find(x => x.Application == app.Id) != null)
                        app.Security = true;
                    model.Security[app.Id] = ReadSecurityConfig(app.Id, conf);
                }

                //----------------
                // Message Queues list
                //----------------
                ReadMessageQueuesList(model, conf);
                
                //----------------
                // ACK list
                //----------------
                model.MQueues.Acks = new List<Models.QueueACK>();

                int rownum = 0;
                if (conf.MessageQueueing != null)
                {
                    foreach (var q in conf.MessageQueueing.ACKS)
                    {
                        model.MQueues.Acks.Add(new Models.QueueACK(q) { Rownum = rownum ++ });
                    }
                }

                //----------------
                //Messaging (will replace MSMQ)
                //----------------
                if (conf.Messaging != null)
                {
                    model.Messaging = conf.Messaging;
                    //decode the username and remove the password before sending to client side
                    model.Messaging.Host.Username = model.Messaging.Host.UsernameDecode();
                    model.Messaging.Host.Password = "";
                }
                else
                {
                    model.Messaging = new MessagingXml();
                }
                model.MessagingMetadata = CSGenio.messaging.MessageMetadataFactory.GeneratedMetadata();

                //----------------
                // Scheduler
                //----------------
                model.Scheduler = conf.Scheduler ?? new SchedulerXml();

                //----------------
                // Others [PATHS , FORMATS , Elasticsearch]
                //----------------

                model.Paths = new Dictionary<string, Models.PathCfg>();
                foreach (var app in model.Applications)
                {
                    if (conf.Paths.Find(x => x.Application == app.Id) != null)
                        app.Path = true;
                    model.Paths[app.Id] = ReadPathConfig(app.Id, conf);
                }
                model.pathReports = conf.pathReports;
                model.ssrsServer = conf.ssrsServer.url;
                model.ssrsServerPath = conf.ssrsServer.path;
                model.isLocalReports = conf.ssrsServer.isLocalReports;
                model.ssrsServerDomain = conf.ssrsServer.Domain;
                model.ssrsServerUsername = conf.ssrsServer.UsernameDecode ?? string.Empty;
                
                if (string.IsNullOrEmpty(conf.ssrsServer.Password)) model.hasSsrsServerPassword = false;
                else model.hasSsrsServerPassword = true;

                model.DateFormat = new Models.DateFormatCfg();
                if (conf.DateFormat != null)
                {
                    model.DateFormat.date = conf.DateFormat.Date;
                    model.DateFormat.dateTime = conf.DateFormat.DateTime;
                    model.DateFormat.dateTimeSeconds = conf.DateFormat.DateTimeSeconds;
                    model.DateFormat.time = conf.DateFormat.Time;
                }

                model.QAEnvironment = Convert.ToBoolean(conf.QAEnvironment);

                var decimalSeparator = HardCodedLists.DisplayNumberFormatDecimal.Dot;
                var groupSeparator = HardCodedLists.DisplayNumberFormatGroup.None;
				var negativeFormat = HardCodedLists.DisplayNumberFormatNegative.Minus;
                if (conf.NumberFormat != null)
                {
                    Enum.TryParse(conf.NumberFormat.DecimalSeparator, out decimalSeparator);
                    model.DecimalSeparator = decimalSeparator;
                    switch (conf.NumberFormat.DecimalSeparator)
                    {
                        case ".":
                            model.DecimalSeparator = HardCodedLists.DisplayNumberFormatDecimal.Dot;
                            break;
                        case ",":
                            model.DecimalSeparator = HardCodedLists.DisplayNumberFormatDecimal.Comma;
                            break;
                        default:
                            model.DecimalSeparator = HardCodedLists.DisplayNumberFormatDecimal.Dot;
                            break;
                    }

                    Enum.TryParse(conf.NumberFormat.GroupSeparator, out groupSeparator);
                    model.GroupSeparator = groupSeparator;
                    switch (conf.NumberFormat.GroupSeparator)
                    {
                        case "":
                            model.GroupSeparator = HardCodedLists.DisplayNumberFormatGroup.None;
                            break;
                        case " ":
                            model.GroupSeparator = HardCodedLists.DisplayNumberFormatGroup.Blank;
                            break;
                        case ".":
                            model.GroupSeparator = HardCodedLists.DisplayNumberFormatGroup.Dot;
                            break;
                        case ",":
                            model.GroupSeparator = HardCodedLists.DisplayNumberFormatGroup.Comma;
                            break;
                        default:
                            model.GroupSeparator = HardCodedLists.DisplayNumberFormatGroup.None;
                            break;
                    }

                    Enum.TryParse(conf.NumberFormat.NegativeFormat, out negativeFormat);
                    model.NegativeFormat = negativeFormat;
                    switch (conf.NumberFormat.NegativeFormat)
                    {
                        case "-":
                            model.NegativeFormat = HardCodedLists.DisplayNumberFormatNegative.Minus;
                            break;
                        case "()":
                            model.NegativeFormat = HardCodedLists.DisplayNumberFormatNegative.Parentheses;
                            break;
                        default:
                            model.NegativeFormat = HardCodedLists.DisplayNumberFormatNegative.Minus;
                            break;
                    }
                }

				// Convert dictionary to list
                foreach (var mp in conf.maisPropriedades)
                {
                    model.AdvancedProperties.Add(new Models.MorePropertyCfg(mp.Key, mp.Value));
                }

                // Elasticsearch List/table
                model.Cores = new List<Models.CoreCfg>();
                rownum = 0;
                if (conf.Elasticsearch != null)
                {
                    foreach (var c in conf.Elasticsearch.Colours)
                        model.Cores.Add(new Models.CoreCfg(c) { Rownum = rownum++ });
                }
                else
                {
                    conf.Elasticsearch = new ElasticsearchXml
                    {
                        Colours = new List<CoreXml>()
                    };
                }

                //----------------
                // Audit
                //----------------
                if (conf.Audit != null)
                {
                    model.RegistActions = conf.Audit.RegistActions;
                    model.RegistLoginOut = conf.Audit.RegistLoginOut;
					model.AuditInterface = conf.Audit.AuditInterface;
                }
                else
                {
                    model.RegistActions = false;
                    model.RegistLoginOut = false;
					model.AuditInterface = false;
                }

                // Event tracing feature
                model.EventTracking = conf.EventTracking;

                model.UrlAPIBackend = conf.ChatBotConfig?.APIEndpoint;
                model.MCPSecurityMode = conf.ChatBotConfig?.MCPSecurityMode ?? MCPSecurityMode.JWT;
                model.JWTEncryptionKey = conf.ChatBotConfig?.JWTEncryptionKey;
                model.UrlMCP = conf.ChatBotConfig?.AppMCPEndpoint;
            }
            catch (Exception e)
            {
                model.Security = new Dictionary<String, Models.SecurityCfg>();
                model.MQueues = new Models.MessageQueue();
                model.MQueues.Queues = new List<Models.QueueCfg>();

                model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                model.AlertType = "danger";
            }

            return Ok(model);
        }

        /// <summary>
        /// Populates the message queues list within the configuration model based on the XML.
        /// </summary>
        /// <param name="model">Config Model</param>
        /// <param name="conf">XML system configuration</param>
        private void ReadMessageQueuesList(Models.ConfigModel model, ConfigurationXML conf)
        {
            int rownum = 0;
            model.MQueues = new Models.MessageQueue();
            model.MQueues.Queues = new List<Models.QueueCfg>();
            if (conf.MessageQueueing != null)
            {
                model.MQueues.Journaltimeout = conf.MessageQueueing.Journaltimeout.ToString();
                model.MQueues.Maxsendnumber = conf.MessageQueueing.Maxsendnumber.ToString();

                foreach (var q in conf.MessageQueueing.Queues)
                {
                    model.MQueues.Queues.Add(new Models.QueueCfg(q) { Rownum = rownum ++ });
                }
            }
            else
            {
                conf.MessageQueueing = new messagequeueing();
            }
        }

        /// <summary>
        /// Extracts database configuration details from the XML and integrates them into the configuration model.
        /// </summary>
        /// <param name="model">Configuration Model</param>
        /// <param name="dataSystem">XML representation of database system config</param>
        /// <param name="conf">XML system configuration</param>
        private void ReadDataBaseConfig(Models.ConfigModel model, DataSystemXml dataSystem, ConfigurationXML conf)
        {
            if (dataSystem != null)
            {
                model.Schema = dataSystem.Schemas[0].Schema; //<-- TODO: Support datasystem configurations with shared DBs
                model.ConnEncrypt = dataSystem.Schemas[0].ConnEncrypt;
                model.ConnWithDomainUser = dataSystem.Schemas[0].ConnWithDomainUser;

                model.HideYears = conf.omiteAnos.ToUpper() == "S";  //<-- Only this one goes to the conf? does that make sense?
                model.DbUser = Encoding.Unicode.GetString(Convert.FromBase64String(dataSystem.Login ?? string.Empty));
                
                if (string.IsNullOrEmpty(dataSystem.Password)) model.HasDbPsw = false;
                else model.HasDbPsw = true;

                model.Server = dataSystem.Server;
                model.Service = dataSystem.Service;
                model.ServiceName = dataSystem.ServiceName;
                model.Port = dataSystem.Port;

                Enum.TryParse(dataSystem.GetDatabaseType().ToString(), out HardCodedLists.DBMS serverType);// Default: SQLSERVER
                model.ServerType = serverType;
                model.DatabaseSidePk = dataSystem.DatabaseSidePk;

                /*
                *  Read Log Database config
                */
                if (dataSystem.DataSystemLog != null && dataSystem.DataSystemLog.Schemas.Count > 0)
                {
                    model.Log_Schema = dataSystem.DataSystemLog.Schemas[0].Schema;
                    model.Log_ConnEncrypt = dataSystem.DataSystemLog.Schemas[0].ConnEncrypt;
                    model.Log_ConnWithDomainUser = dataSystem.DataSystemLog.Schemas[0].ConnWithDomainUser;
                    model.Log_DbUser = Encoding.Unicode.GetString(Convert.FromBase64String(dataSystem.DataSystemLog.Login ?? string.Empty));
                    
                    if (string.IsNullOrEmpty(dataSystem.DataSystemLog.Password)) model.Log_HasDbPsw = false;
                    else model.Log_HasDbPsw = true;

                    model.Log_Server = dataSystem.DataSystemLog.Server ?? string.Empty;
                    model.Log_Port = dataSystem.DataSystemLog.Port ?? string.Empty;
                    model.Log_Service = dataSystem.DataSystemLog.Service ?? string.Empty;
                    model.Log_ServiceName = dataSystem.DataSystemLog.ServiceName ?? string.Empty;
                }

            }
        }


        private Models.SecurityCfg ReadSecurityConfig(String appId, ConfigurationXML conf)
        {
            var model = new Models.SecurityCfg();

            var security = conf.GetSecurity(appId);

            model.AuthenticationMode = security.AuthenticationMode;
            model.AllowMultiSessionPerUser = security.AllowMultiSessionPerUser;
            model.AllowAuthenticationRecovery = security.AllowAuthenticationRecovery;
			model.Mandatory2FA = security.Mandatory2FA;
            model.ExpirationDateBool = security.ExpirationDateBool;
            model.ExpirationDate = security.ExpirationDate;
            model.MinCharacters = Convert.ToInt32(security.MinCharacters);
            model.PasswordStrength = security.PasswordStrength;
            model.PasswordAlgorithms = security.PasswordAlgorithms;
            model.SessionTimeOut = security.SessionTimeOut;
            model.UsePasswordBlacklist = security.UsePasswordBlacklist;
            model.MaxAttempts = security.MaxAttempts;

            model.IdentityProviders = new List<Models.IdentityProviderCfg>();
            int rownum = 0;
            foreach (var ip in security.IdentityProviders)
                model.IdentityProviders.Add(new Models.IdentityProviderCfg(ip) { Rownum = rownum++ });

            model.RoleProviders = new List<Models.RoleProviderCfg>();
            rownum = 0;
            foreach (var rp in security.RoleProviders)
                model.RoleProviders.Add(new Models.RoleProviderCfg(rp) { Rownum = rownum++ });

            model.Users = new List<Models.UserCfg>();
            rownum = 0;
            foreach (var u in security.Users)
                model.Users.Add(new Models.UserCfg(u) { Rownum = rownum++ });

            return model;
        }

        private Models.PathCfg ReadPathConfig(String appId, ConfigurationXML conf)
        {
            var model = new Models.PathCfg();
            var paths = conf.Paths.Find(x => x.Application == appId);
            if (paths != null)
            {
                model.pathApp = paths.pathApp;
                model.pathDocuments = paths.pathDocuments;
            }
            return model;
        }

        [HttpPost]
        public IActionResult CreateDataSystem([FromBody] JsonObject data)
        {
            ConfigurationXML conf = configManager.GetExistingConfig();
            string year = (string)data["year"];
            string schema = (string)data["schema"];
            string type = (string)data["type"] ?? "";
            string server = (string)data["server"] ?? "";
            createDataSystem(year, schema, conf, type, server);
            configManager.StoreConfig(conf);
            Configuration.ReadConfiguration(conf);
            return Json(new { system = year });
        }

        private DataSystemXml createDataSystem(string year, string schemaName, ConfigurationXML conf, string dsType = "", string dsServer = "")
        {
            //in case the user set a name that already existed just return that one
            var dataSystem = conf.DataSystems.FirstOrDefault(ds => ds.Name == year);
            if (dataSystem != null)
                return dataSystem;
            else
            {
                //find the default datasystem in case it exists, otherwise creat a new one
                var defaultDs = conf.DataSystems.FirstOrDefault(ds => ds.Name == conf.anoDefault);
                dataSystem = defaultDs?.ShallowCopy() ?? new DataSystemXml();
                // Type and server are set when duplicating a data system
                dataSystem.Name = year;
                if(!string.IsNullOrEmpty(dsType))
                    dataSystem.Type = dsType;
                if (!string.IsNullOrEmpty(dsServer))
                    dataSystem.Server = dsServer;
            }

            //Add the main schema in case there is none yet
            if (dataSystem.Schemas is null || dataSystem.Schemas.Count == 0)
            {
                var schema = new DataXml();
                schema.Id = CSGenio.framework.Configuration.Program;
                schema.Schema = schemaName;
                schema.ConnEncrypt = conf.connEncrypt;
                schema.ConnWithDomainUser = conf.connWithDomainUser;
                dataSystem.Schemas = new List<DataXml>() { schema };
            }
            else //there was already one, we need to override the main schema database
            {
                dataSystem.Schemas[0].Schema = schemaName;
            }

            conf.DataSystems.Add(dataSystem);
            return dataSystem;
        }

        [HttpPost]
        public IActionResult DeleteDataSystem([FromBody] string year)
        {
            try
            {
                ConfigurationXML conf = configManager.GetExistingConfig();
                
                conf.DataSystems = conf.DataSystems.Where(ds => ds.Name != year).ToList();
                configManager.StoreConfig(conf);
                Configuration.ReadConfiguration(conf);

                return Json(new { system = year });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Json(new { ResultMsg = Resources.Resources.NAO_E_POSSIVEL_APAGA39444 });
            }
        }

        [HttpPost]
        public IActionResult SaveConfigDataSystems([FromBody] Models.ConfigModel model)
        {
            var conf = configManager.GetExistingConfig();

            conf.anoDefault = model.DefaultYear ?? "0";

            // In case the default data system changes, reorder databases
            if (conf.anoDefault != Configuration.DefaultYear)
            {
                DataSystemXml tempDefDS = conf.DataSystems.FirstOrDefault(ds => ds.Name == conf.anoDefault);
                conf.DataSystems.Remove(tempDefDS);
                conf.DataSystems.Insert(0, tempDefDS);
            }

            conf.omiteAnos = model.HideYears ? "S" : "";

            configManager.StoreConfig(conf);
            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);

            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult TestDBConnection([FromBody] Models.ConfigModel model)
        {
            try
            {
                bool connectionSuccess = PersistentSupport.TestServerConnection(model.GetDataSystemXml());

                if (connectionSuccess)
                {
                    return Json(new { Success = true, Message = "Connection success", AlertType = "success" });
                }
                else
                {
                    return Json(new { Success = false, Message = "Connection failed", AlertType = "danger" });
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        [HttpPost]
        public IActionResult SaveConfigDatabase([FromBody]Models.ConfigModel model)
        {
            var appId = FromQuery("appId");
            bool hasLogDB = false;
            string year = CurrentYear; 

            try
            {
                SysConfiguration sysConfiguration = new SysConfiguration(configManager);

                model.ResultMsg = string.Empty;
				if (!ModelState.IsValid)
                {
                    model.AlertType = "danger";
                    string err = Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860 + Environment.NewLine + string.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    throw new BusinessException(err, "ConfigController.reindex", err);
                }

                if (string.IsNullOrEmpty(model.DbPsw) || (model.DbPsw != model.DbCheckPsw))
                {
                    model.ResultMsg += Resources.Resources.A_PASSWORD_NAO_COINC35287;
                    model.AlertType = "danger";
                }
                //Check log database user input
                if(!string.IsNullOrEmpty(model.Log_Server) || !string.IsNullOrEmpty(model.Log_Schema) || 
                    !string.IsNullOrEmpty(model.Log_DbPsw) || !string.IsNullOrEmpty(model.Log_DbCheckPsw) || !string.IsNullOrEmpty(model.Log_DbUser))
                {                    
                    if (string.IsNullOrEmpty(model.Log_Server) || string.IsNullOrEmpty(model.Log_Schema) ||
                    string.IsNullOrEmpty(model.Log_DbPsw) || string.IsNullOrEmpty(model.Log_DbCheckPsw) || string.IsNullOrEmpty(model.Log_DbUser))
                    {
                        model.AlertType = "danger";
                        throw new BusinessException(Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860, "ConfigController.reindex", Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860);
                    }
                    if (model.Log_DbPsw != model.Log_DbCheckPsw)
                    {
                        model.AlertType = "danger";
                        model.ResultMsg += Resources.Resources.A_PASSWORD_NAO_COINC35287;
                    }
					hasLogDB = true;
                }   

                if(hasLogDB && model.Schema.ToLower() == model.Log_Schema.ToLower())
                {
                    model.AlertType = "danger";
                    throw new BusinessException(Resources.Resources.THE_LOG_DATABASE_CAN31596, "ConfigController.reindex", Resources.Resources.THE_LOG_DATABASE_CAN31596);
                }
                if (string.IsNullOrEmpty(model.ResultMsg))
                {
                    //Configure main database
                    sysConfiguration.SaveDatabaseConfig(model.DbUser, model.DbPsw, model.Server, model.ServerType.ToString(), model.Schema, 
                    model.Port, model.ConnEncrypt, model.ConnWithDomainUser, year, model.DatabaseSidePk);

                    // Configure log database
                    if(hasLogDB) {
                        sysConfiguration.SaveLogDatabaseConfig(model.Log_DbUser, model.Log_DbPsw, model.Log_Server, model.ServerType.ToString(), 
                            model.Log_Schema, model.Log_Port, model.ConnEncrypt, model.ConnWithDomainUser, year);                    
                    }
                    model.AlertType = "success";
                    model.ResultMsg = Resources.Resources.FICHEIRO_DE_CONFIGUR18806 + " " + Resources.Resources.SERA_REDIRECIONADO_E06592;
                }
            }
            catch (Exception e)
            {
                return Index(Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper()), appId, "danger");
            }

			return Index(model.ResultMsg, appId, model.AlertType);
        }


        [HttpPost]
        public IActionResult SaveIdentityProvider([FromBody]Models.IdentityProviderCfg model)
        {
            var appId = FromQuery("appId");
            var conf = configManager.GetExistingConfig();
            SecurityCfgEl security = conf.GetSecurity(appId);
            if (model.FormMode == "delete")
            {
                security.IdentityProviders.RemoveAt(model.Rownum);
            }
            if (model.FormMode == "edit")
            {
                security.IdentityProviders[model.Rownum] = model.obj;
            }
            if (model.FormMode == "new")
            {
                security.IdentityProviders.Add(model.obj);
            }

            configManager.StoreConfig(conf);
            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);

            security = conf.GetSecurity(appId);
            var rownum = security.IdentityProviders.FindIndex(u => u.Name == model.Name);
            Models.IdentityProviderCfg identityProvider = model.FormMode != "delete" ? new Models.IdentityProviderCfg(security.IdentityProviders[rownum]) { Rownum = rownum } : null;
            

            return Json(new { success = true, identityProvider });
        }

        [HttpPost]
        public IActionResult SaveRoleProvider([FromBody]Models.RoleProviderCfg model)
        {
            var appId = FromQuery("appId");
            var conf = configManager.GetExistingConfig();
            SecurityCfgEl security = conf.GetSecurity(appId);
            if (model.FormMode == "delete")
            {
                security.RoleProviders.RemoveAt(model.Rownum);
            }
            if (model.FormMode == "edit")
            {
                security.RoleProviders[model.Rownum] = model.obj;
            }
            if (model.FormMode == "new")
            {
                security.RoleProviders.Add(model.obj);
            }

            configManager.StoreConfig(conf);
            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);

            security = conf.GetSecurity(appId);
            var rownum = security.RoleProviders.FindIndex(rp => rp.Name == model.Name);
            Models.RoleProviderCfg roleProvider = model.FormMode != "delete" ? new Models.RoleProviderCfg(security.RoleProviders[rownum]) { Rownum = rownum } : null;

            return Json(new { success = true, roleProvider });
        }

        [HttpPost]
        public IActionResult SetupProviders()
        {
            var appId = FromQuery("appId");
            var conf = configManager.GetExistingConfig();
            SecurityCfgEl security = conf.GetSecurity(appId);
            foreach (var provider in security.RoleProviders)
            {
                var providerInstance = SecurityFactory.ParseRoleProvider(provider);
                if(providerInstance.HasUserDirectory)
                    providerInstance.SetupUserDirectory();
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult SaveUserCfg([FromBody]Models.UserCfg model)
        {
            var appId = FromQuery("appId");
            var conf = configManager.GetExistingConfig();
            SecurityCfgEl security = conf.GetSecurity(appId);
            var index = security.Users.FindIndex(u => u.Name == model.Name);
            if (model.FormMode == "delete")
            {
                security.Users.RemoveAt(index);
            }
            if (model.FormMode == "edit")
            {
                security.Users[index] = model.obj;
            }
            if (model.FormMode == "new")
            {
                security.Users.Add(model.obj);
            }

            configManager.StoreConfig(conf);
            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);
            security = conf.GetSecurity(appId);
            // var rownum = security.Users.FindIndex(u => u.Name == model.Name);
            // Models.UserCfg user = model.FormMode != "delete" ? new Models.UserCfg(security.Users[rownum]) { Rownum = rownum } : null;

            if (security.Users == null) security.Users = new List<UserCfgEl>();
            var users = security.Users.Select(u => new Models.UserCfg(u));

            return Json(new { success = true, users });
        }

        [HttpPost]
        public IActionResult SaveQueue([FromBody] Models.QueueCfg model)
        {
            var conf = configManager.GetExistingConfig();
            if (conf.MessageQueueing == null)
            {
                conf.MessageQueueing = new messagequeueing();
            }

            if (model.FormMode == "delete")
            {
                conf.MessageQueueing.Queues.RemoveAt(model.Rownum);
            }
            if (model.FormMode == "edit")
            {
                conf.MessageQueueing.Queues[model.Rownum] = model.obj;
            }
            if (model.FormMode == "new")
            {
                conf.MessageQueueing.Queues.Add(model.obj);
            }

            configManager.StoreConfig(conf);
            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);

            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult SaveQueueACK([FromBody] Models.QueueACK model)
        {
            var conf = configManager.GetExistingConfig();
            if (conf.MessageQueueing == null)
            {
                conf.MessageQueueing = new messagequeueing();
            }

            if (model.FormMode == "delete")
            {
                conf.MessageQueueing.ACKS.RemoveAt(model.Rownum);
            }
            if (model.FormMode == "edit")
            {
                conf.MessageQueueing.ACKS[model.Rownum] = model.obj;
            }
            if (model.FormMode == "new")
            {
                conf.MessageQueueing.ACKS.Add(model.obj);
            }

            configManager.StoreConfig(conf);
            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);

            return Json(new { Success = true });
        }

        [HttpGet]
        public IActionResult ReloadMQueues()
        {
            var conf = configManager.GetExistingConfig();
            //----------------
            // Queues list
            //----------------
            var MQueues = new Models.MessageQueue();
            MQueues.Queues = new List<Models.QueueCfg>();

            int rownum = 0;
            if (conf.MessageQueueing != null)
            {
                conf.MessageQueueing.Journaltimeout = GenFunctions.atoi(MQueues.Journaltimeout);
                conf.MessageQueueing.Maxsendnumber = GenFunctions.atoi(MQueues.Maxsendnumber);

                foreach (var q in conf.MessageQueueing.Queues)
                {
                    MQueues.Queues.Add(new Models.QueueCfg(q) { Rownum = rownum++ });
                }
            }
            else
            {
                conf.MessageQueueing = new messagequeueing();
            }

            //----------------
            // ACK list
            //----------------
            MQueues.Acks = new List<Models.QueueACK>();

            rownum = 0;
            if (conf.MessageQueueing != null)
            {
                foreach (var q in conf.MessageQueueing.ACKS)
                {
                    MQueues.Acks.Add(new Models.QueueACK(q) { Rownum = rownum++ });
                }
            }
            return Json(new { Success = true, MQueues });
        }

        [HttpPost]
        public IActionResult SaveCoreCfg([FromBody]Models.CoreCfg model)
        {
            var conf = configManager.GetExistingConfig();
            if (conf.Elasticsearch == null)
            {
                conf.Elasticsearch = new ElasticsearchXml
                {
                    Colours = new List<CoreXml>()
                };
            }

            if (model.FormMode == "delete")
            {
                conf.Elasticsearch.Colours.RemoveAt(model.Rownum);
            }
            if (model.FormMode == "edit" || model.FormMode == "new")
            {
                if (!string.IsNullOrEmpty(model.Obj.Password))
                {
                    byte[] pass_bytes = System.Text.Encoding.UTF8.GetBytes(model.Obj.Password ?? "");
                    model.Obj.Password = Convert.ToBase64String(pass_bytes, Base64FormattingOptions.None);
                }
                if (model.FormMode == "edit")
                {
                    conf.Elasticsearch.Colours[model.Rownum] = model.Obj;
                }
                if (model.FormMode == "new")
                {
                    conf.Elasticsearch.Colours.Add(model.Obj);
                }
            }

            configManager.StoreConfig(conf);
            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);

            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult SaveConfigMessaging([FromBody]MessagingXml model)
        {
            var conf = configManager.GetExistingConfig();

            model.Host.Username = Convert.ToBase64String(Encoding.Unicode.GetBytes(model.Host.Username));
            if(!string.IsNullOrEmpty(model.Host.Password))
                model.Host.Password = Convert.ToBase64String(Encoding.Unicode.GetBytes(model.Host.Password));
            else
                model.Host.Password = conf.Messaging.Host.Password;

            conf.Messaging = model;
            configManager.StoreConfig(conf);

            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);

            return Json(new { Success = true });
        }

        public class FormRecordOperation<T>
        {
            public T Data { get; set; }
            public string FormMode { get; set; }
        }

        public IActionResult SaveScheduledJob([FromBody]FormRecordOperation<SchedulerJobXml> model)
        {
            var row = model.Data;

            var conf = configManager.GetExistingConfig();
            var existing = conf.Scheduler.Jobs.Find(x => x.Id == row.Id);

            switch(model.FormMode)
            {
                case "delete":
                    if(existing is not null)
                        conf.Scheduler.Jobs.Remove(existing);
                    break;
                case "new":
                case "edit":
                    //verify Cron is not empty
                    if (row.Cron == "")
                        return Json(new { Success = false, Message = Resources.Resources.CRON_E_NECESSARIO07773 });
                    //validate Cron
                    if(!Cronos.CronExpression.TryParse(row.Cron, Cronos.CronFormat.IncludeSeconds, out var _))
                        return Json(new { Success = false, Message = Resources.Resources.EXPRESSAO_CRON_INVAL33136  });

                    //trim unused/empty options
                    if(row.Options != null)
                    {
                        foreach(var kvp in row.Options)
                            if(string.IsNullOrEmpty(kvp.Value))
                                row.Options.Remove(kvp.Key);
                        if(row.Options.Count == 0)
                            row.Options = null;
                    }

                    //update the job list
                    if(existing is not null)
                        conf.Scheduler.Jobs.Remove(existing);
                    conf.Scheduler.Jobs.Add(row);
                    break;
                default:
                    Log.Error("Unknown form operation in SaveScheduledJob.");
                    break;
            }

            configManager.StoreConfig(conf);

            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);

            // Dynamically update the scheduler service with the new configuration
            var service = this.HttpContext.RequestServices.GetRequiredService<SchedulerServiceHost>();
            service.UpdateJobs();


            return Json(new { Success = true });
        }


		[HttpGet]
        public IActionResult GetNewMorePropertyCfg()
        {
            return Json(new Models.MorePropertyCfg() { Rownum = -1 });
        }

        #region Empty objects
        [HttpGet]
        public IActionResult GetNewUserCfg()
        {
            return Json(new Models.UserCfg() { Rownum = -1 });
        }

        [HttpGet]
        public IActionResult GetNewIdentityProviderCfg()
        {
            return Json(new Models.IdentityProviderCfg() { Rownum = -1 });
        }

        [HttpGet]
        public IActionResult GetNewRoleProviderCfg()
        {
            return Json(new Models.RoleProviderCfg() { Rownum = -1 });
        }

        [HttpGet]
        public IActionResult GetNewCoreCfg()
        {
            return Json(new Models.CoreCfg() { Rownum = -1 });
        }

        [HttpGet]
        public IActionResult GetNewQueue()
        {
            return Json(new Models.QueueCfg() { Rownum = -1 });
        }

        [HttpGet]
        public IActionResult GetNewAck()
        {
            return Json(new Models.QueueACK() { Rownum = -1 });
        }
        #endregion

        [HttpPost]
        public IActionResult SaveConfigSecurity([FromBody]Models.SecurityCfg model)
        {
            var appId = FromQuery("appId");
            var conf = configManager.GetExistingConfig();

            foreach (var security in ClientApplication.Applications.Select(x=> conf.GetSecurity(x.Id)))
            {
				try
				{
                    if (appId == security.Application)
                    {
                        security.AllowAuthenticationRecovery = model.AllowAuthenticationRecovery;
                        security.AllowMultiSessionPerUser = model.AllowMultiSessionPerUser;
                        security.AuthenticationMode = model.AuthenticationMode;
                        security.Mandatory2FA = model.Mandatory2FA;
                        security.SessionTimeOut = model.SessionTimeOut;
                    }
                    //this variables will be the same for all modules
					security.ExpirationDateBool = model.ExpirationDateBool;
					security.ExpirationDate = model.ExpirationDate;
					security.MinCharacters = model.MinCharacters.ToString();
					security.PasswordStrength = model.PasswordStrength;
					security.PasswordAlgorithms = model.PasswordAlgorithms;
					security.MaxAttempts = model.MaxAttempts;
                    security.UsePasswordBlacklist = model.UsePasswordBlacklist;

					configManager.StoreConfig(conf);
				}
				catch (Exception e)
				{
					var resultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
					return Json(new { Success = false, Message = resultMsg });
				}
			}

            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);

            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult SaveIntegrationConfig([FromBody] ConfigModel model)
        {
            var conf = configManager.GetExistingConfig();

            if (model.Messaging != null)
            {
                model.Messaging.Host.Username = Convert.ToBase64String(Encoding.Unicode.GetBytes(model.Messaging.Host.Username));
                    model.Messaging.Host.Password = Convert.ToBase64String(Encoding.Unicode.GetBytes(model.Messaging.Host.Password));

                conf.Messaging = model.Messaging;
            }

            if (model.MQueues != null)
            {
                if (string.IsNullOrEmpty(model.MQueues.Journaltimeout) != string.IsNullOrEmpty(model.MQueues.Maxsendnumber))
                {
                    return Json(new
                    {
                        Status = "ERROR",
                        Message = Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860,
                        AlertType = "danger"
                    });
                }

                if (conf.MessageQueueing == null)
                    conf.MessageQueueing = new messagequeueing();

                conf.MessageQueueing.Journaltimeout = int.Parse(model.MQueues.Journaltimeout);
                conf.MessageQueueing.Maxsendnumber = int.Parse(model.MQueues.Maxsendnumber);
            }

            try
            {
                configManager.StoreConfig(conf);
                CSGenio.framework.Configuration.ReadConfiguration(conf);
            }
            catch (Exception e)
            {
                var resultMsg = e.Message;
                return Json(new { Status = "ERROR", Message = resultMsg, AlertType = "danger" });
            }
            var message = string.IsNullOrEmpty(model.ResultMsg) ? Resources.Resources.FICHEIRO_DE_CONFIGUR18806 : model.ResultMsg;

            return Json(new { Status = "OK", Message = message, AlertType = "success" });
        }

        [HttpPost]
        public IActionResult SaveSystemConfig([FromBody] ConfigModel model)
        {
            var conf = configManager.GetExistingConfig();

            conf.Audit = new AuditCfgEl();
            conf.Audit.RegistActions = model.RegistActions;
            conf.Audit.RegistLoginOut = model.RegistLoginOut;
            conf.Audit.AuditInterface = model.AuditInterface;

            conf.EventTracking = model.EventTracking;

            if (model.Scheduler != null)
            {
                conf.Scheduler.Enabled = model.Scheduler.Enabled;
            }

            try
            {
                configManager.StoreConfig(conf);

                Configuration.ReadConfiguration(conf);

                var service = this.HttpContext.RequestServices.GetRequiredService<SchedulerServiceHost>();
                service.UpdateEnable();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = Translations.Get(ex.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper())
                });
            }

            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult SaveConfigOthers([FromBody]Models.ConfigModel model)
        {
            var conf = configManager.GetExistingConfig();
            conf.DateFormat = new DateFormatXml();
            conf.NumberFormat = new NumberFormatXml();
            conf.ChatBotConfig = new ChatBotCfg();
            try
            {
                conf.pathReports = model.pathReports;
                conf.ssrsServer.url = model.ssrsServer;
                conf.ssrsServer.path = model.ssrsServerPath;
                conf.ssrsServer.isLocalReports = model.isLocalReports;
                conf.ssrsServer.Domain = model.ssrsServerDomain;
                conf.ssrsServer.UsernameDecode = model.ssrsServerUsername;
                
                if (!string.IsNullOrEmpty(model.ssrsServerUsername) && string.IsNullOrEmpty(model.ssrsServerPassword))
                    throw new BusinessException("SSR Password field is empty.", "EmailPropertiesModel.MapToModel", "SSR Password field is empty.");

                if (string.IsNullOrEmpty(model.ssrsServerUsername) && !string.IsNullOrEmpty(model.ssrsServerPassword))
                    throw new BusinessException("SSR Username field is empty.", "EmailPropertiesModel.MapToModel", "SSR Username field is empty.");

                // Convert new password to base64
                conf.ssrsServer.PasswordDecode = model.ssrsServerPassword ?? "";

                conf.DateFormat.Date = model.DateFormat.date;
                conf.DateFormat.DateTime = model.DateFormat.dateTime;
                conf.DateFormat.DateTimeSeconds = model.DateFormat.dateTimeSeconds;
                conf.DateFormat.Time = model.DateFormat.time;

                conf.ChatBotConfig.APIEndpoint = model.UrlAPIBackend;
                conf.ChatBotConfig.MCPSecurityMode = model.MCPSecurityMode;
                conf.ChatBotConfig.JWTEncryptionKeyDecode = model.JWTEncryptionKey;
                conf.ChatBotConfig.AppMCPEndpoint = model.UrlMCP;

                conf.QAEnvironment = Convert.ToInt32(model.QAEnvironment);

                switch (model.DecimalSeparator.ToString())
                {
                    case "Dot":
                        conf.NumberFormat.DecimalSeparator = ".";
                        break;
                    case "Comma":
                        conf.NumberFormat.DecimalSeparator = ",";
                        break;
                    default:
                        conf.NumberFormat.DecimalSeparator = ".";
                        break;
                }
                switch (model.GroupSeparator.ToString())
                {
                    case "": // none
                        conf.NumberFormat.GroupSeparator = "";
                        break;
                    case "Comma":
                        conf.NumberFormat.GroupSeparator = ",";
                        break;
                    case "Dot":
                        conf.NumberFormat.GroupSeparator = ".";
                        break;
                    case "Blank":
                        conf.NumberFormat.GroupSeparator = " ";
                        break;
                    default: // none
                        conf.NumberFormat.GroupSeparator = "";
                        break;
                }
                switch (model.NegativeFormat.ToString())
                {
                    case "Minus":
                        conf.NumberFormat.NegativeFormat = "-";
                        break;
                    case "Parentheses":
                        conf.NumberFormat.NegativeFormat = "()";
                        break;
                    default:
                        conf.NumberFormat.NegativeFormat = "-";
                        break;
                }
                // check if they have the same value
                if (model.DecimalSeparator.ToString() == model.GroupSeparator.ToString())
                    throw new BusinessException(Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860, "ConfigController.reindex", Resources.Resources.ALGUNS_CAMPOS_ESTAO_27860);

                conf.FillMissingConfigs();
                configManager.StoreConfig(conf);

				// Reload Configuration static instance in server with the new Configuracoes.xml data
                CSGenio.framework.Configuration.ReadConfiguration(conf);
            }
            catch (Exception e)
            {
                var resultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                return Json(new { Success = false, Message = resultMsg, AlertType = "danger" });
            }

            return Json(new { Success = true });
        }

        [HttpPost]
        public IActionResult SavePathCfg([FromBody] Models.PathCfg model)
        {
            var appId = FromQuery("appId");
            ConfigurationXML conf = configManager.GetExistingConfig();
            PathCfgEl path = conf.GetPath(appId);
            path.pathApp = model.pathApp;
            path.pathDocuments = model.pathDocuments;
            configManager.StoreConfig(conf);

            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);
            return Json(new { Success = true });
        }

		[HttpPost]
        public IActionResult SaveMoreProperty([FromBody]Models.MorePropertyCfg model)
        {
            var conf = configManager.GetExistingConfig();
            var initProp = false;

			if (String.IsNullOrEmpty(model.Key)) { return Json(new { emptyKey = true }); }

            if (String.IsNullOrEmpty(model.Val)) { return Json(new { emptyVal = true }); }

            var valueContent = model.Val;
            if (ExtraProperties.IsPasswordType(model.Key))
                valueContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(model.Val));

            if (model.FormMode == "delete")
            {
                initProp = ExtraProperties.HasDefaultValue(model.Key);
                
                conf.maisPropriedades.Remove(model.Key);
            }
            if (model.FormMode == "edit")
            {
                conf.maisPropriedades[model.Key] = valueContent;
            }
            if (model.FormMode == "new")
            {
                if (conf.maisPropriedades.ContainsKey(model.Key)) { return Json(new { success = false }); }
                conf.maisPropriedades.Add(model.Key, valueContent);
            }

            configManager.StoreConfig(conf);
            // Reload Configuration static instance in server with the new Configuracoes.xml data
            CSGenio.framework.Configuration.ReadConfiguration(conf);

            List<MorePropertyCfgEl> morePropertyList = new List<MorePropertyCfgEl>();
            foreach (var mp in conf.maisPropriedades)
            {
                MorePropertyCfgEl mpe = new MorePropertyCfgEl();
                mpe.Key = mp.Key;
                mpe.Val = mp.Value;
                morePropertyList.Add(mpe);
            }

            var rownum = morePropertyList.FindIndex(u => u.Key == model.Key);
            Models.MorePropertyCfg moreProperty = model.FormMode != "delete" ? new Models.MorePropertyCfg(morePropertyList[rownum]) { Rownum = rownum } : null;

            return Json(new { success = true, moreProperty, initProp = initProp });
        }

        [HttpGet]
        public FileResult DownloadRedirect()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            var sysConfig = new SysConfiguration(configManager);
            var redirect = sysConfig.CreateRedirect(path);

            var dataStream = new MemoryStream();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(RedirectXML));
            serializer.Serialize(dataStream, redirect);
            dataStream.Position = 0;

            return File(dataStream, "application/octet-stream", "Configuracoes.redirect.xml");
        }

        [HttpPost]
        public IActionResult VerifyDocPathConfig([FromBody] Models.PathCfg model)
        {
            ConfigurationXML conf = configManager.GetExistingConfig();

            for(int i = 0; i < conf.Paths.Count; i++){
                if(conf.Paths[i].pathDocuments != model.pathDocuments){
                    return Json(new { Success = false });
                }
            }

            return Json(new { Success = true });            
        }


        private int CountBlacklistedPasswords(PersistentSupport sp)
        {
            try
            {
                SelectQuery select = new SelectQuery()
                    .Select(SqlFunctions.Count(1), "COUNT")
                    .From("PswBlacklist");
                return DBConversion.ToInteger(sp.executeScalar(select));
            }
            catch
            {
                //if there is an error or the db does not exist yet, return 0
                return 0;
            }
        }

        [HttpGet]
        public IActionResult ManagePasswordBlacklist()
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(CurrentYear);
            sp.openConnection();
            var numPasswords = CountBlacklistedPasswords(sp);
            sp.closeConnection();

            return Json(new { Success = true, numPasswords});
        }

        [HttpPost]
        public IActionResult BlacklistUpload(IFormFile file)
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(CurrentYear);
            sp.openConnection();

            using var stream = new StreamReader(file.OpenReadStream());

            if(sp.DatabaseType == DatabaseType.SQLSERVER || sp.DatabaseType == DatabaseType.SQLSERVERCOMPAT)
            {
                string ?line;
                DataTable dt = new DataTable();
                var col0 = new DataColumn("pass");
                dt.Columns.Add(col0);
                while((line = stream.ReadLine()) != null) 
                {
                    var row = dt.NewRow();
                    row.SetField(col0, line.ToLowerInvariant());
                    dt.Rows.Add(row);
                }

                using var copy = new SqlBulkCopy(sp.Connection as SqlConnection);
                copy.DestinationTableName = "PswBlacklist";
                copy.WriteToServer(dt);
            }
            else
            {
                string ?line;
                while((line = stream.ReadLine()) != null) 
                {
                    InsertQuery ins = new InsertQuery()
                        .Into("PswBlacklist")
                        .Value("pass", line.ToLowerInvariant());
                    sp.Execute(ins);
                }
            }

            var numPasswords = CountBlacklistedPasswords(sp);
            sp.closeConnection();
            return Json(new { Success = true, numPasswords });
        }

        [HttpGet]
        public IResult BlacklistDownload()
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(CurrentYear);
            sp.openConnection();

            SelectQuery select = new SelectQuery()
                .Select("PswBlacklist", "pass")
                .From("PswBlacklist");

            var rows = sp.executeReaderOneColumn(select);
            var memory = new MemoryStream();
            using var stream = new StreamWriter(memory, Encoding.UTF8);
            foreach(var row in rows)
                stream.WriteLine(row.ToString());
            stream.Close();

            var memout = new MemoryStream(memory.GetBuffer(), false);
            return Results.File(memout, "text/plain", "blacklist.txt");
        }

        public record PasswordReq(string password);

        [HttpPost]
        public IActionResult BlacklistPasswordCheck([FromBody] PasswordReq req)
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(CurrentYear);
            sp.openConnection();
            User user = SysConfiguration.CreateWebAdminUser();
            var factory = new GenioServer.security.UserFactory(sp, user);
            var error = factory.CheckBlacklisted(req.password);
            sp.closeConnection();

            return Json(new { Success = true, found = !string.IsNullOrEmpty(error) });
        }

        [HttpPost]
        public IActionResult BlacklistPasswordAdd([FromBody] PasswordReq req)
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(CurrentYear);
            sp.openConnection();
            InsertQuery ins = new InsertQuery()
                .Into("PswBlacklist")
                .Value("pass", req.password);
            sp.Execute(ins);
            var numPasswords = CountBlacklistedPasswords(sp);
            sp.closeConnection();
            return Json(new { Success = true, numPasswords });
        }

        [HttpPost]
        public IActionResult BlacklistPasswordClear()
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(CurrentYear);
            sp.openConnection();
            DeleteQuery del = new DeleteQuery().Delete("PswBlacklist");
            sp.Execute(del);
            var numPasswords = CountBlacklistedPasswords(sp);
            sp.closeConnection();
            return Json(new { Success = true, numPasswords });
        }

        [HttpPost]
        public IActionResult ServicePasswordCheck()
        {
            List<string> errors = [];
            var conf = configManager.GetExistingConfig();
            string pass;

            PersistentSupport sp = PersistentSupport.getPersistentSupport(CurrentYear);
            sp.openConnection();
            User user = SysConfiguration.CreateWebAdminUser();
            var factory = new GenioServer.security.UserFactory(sp, user);

            void CheckOnePassword(string label, string pass)
            {
                if(!string.IsNullOrEmpty(pass))
                {
                    string error = factory.CheckPasswordRules(pass);
                    if(!string.IsNullOrEmpty(error))
                        errors.Add(label + " : " + error);
                }
            }

            DataSystemXml dataSystem = Configuration.ResolveDataSystem(CurrentYear, Configuration.DbTypes.NORMAL);
            if(dataSystem != null)
            {
                pass = Encoding.Unicode.GetString(Convert.FromBase64String(dataSystem.Password ?? string.Empty));
                CheckOnePassword("Current Data System", pass);

                if(dataSystem.DataSystemLog != null)
                {
                    pass = Encoding.Unicode.GetString(Convert.FromBase64String(dataSystem.DataSystemLog.Password ?? string.Empty));
                    CheckOnePassword("Log Data System", pass);
                }
            }

            if(conf.ssrsServer != null)
            {
                pass = Encoding.Unicode.GetString(Convert.FromBase64String(conf.ssrsServer.Password ?? string.Empty));
                CheckOnePassword("Reporting services", pass);
            }

            if(conf.EmailProperties != null)
                foreach(var server in conf.EmailProperties)
                {
                    pass = Encoding.UTF8.GetString(Convert.FromBase64String(server.Password ?? string.Empty));
                    CheckOnePassword("Email Server" + " " + server.Name, pass);
                }

            sp.closeConnection();

            return Json(new { Success = true, resultList = errors });
        }
    }
}
