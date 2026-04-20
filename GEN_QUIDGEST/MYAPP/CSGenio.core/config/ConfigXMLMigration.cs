using CSGenio;
using CSGenio.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using Quidgest.Persistence;
using System.Xml.Serialization;
using System.Xml.Linq;
using CSGenio.config;

namespace GenioServer.framework
{
    public class ConfigXMLMigration
    {

        public static int CurConfigurationVerion = 14;

        public static void Migration(IConfigurationManager configManager, int fileConfigVersion)
        {            
            //Migration routine is expecting direct access to the file
            var fileConfigManager = (FileConfigurationManager)configManager;
            string pathConfig = fileConfigManager.GetFileLocation();

            //will make a copy of the old file before migrating to the new one
            makeCopyConfig(pathConfig);

            //reads the actual file
            string configFileTxt = System.IO.File.ReadAllText(pathConfig);

            if (fileConfigVersion != CurConfigurationVerion)
            {
                //insert here your routine to migrate version
                configFileTxt = migrateConfigToVersion1(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion2(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion3(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion4(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion5(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion6(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion7(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion8(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion9(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion10(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion11(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigToVersion12(fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigTo(Version13, 13, fileConfigVersion, configFileTxt);
                configFileTxt = migrateConfigTo(Version14, 14, fileConfigVersion, configFileTxt);
            }

            //write the final file
            System.IO.File.WriteAllText(pathConfig, configFileTxt);
        }

        /// <summary>
        /// Standard migration pattern for version upgrade methods
        /// </summary>
        /// <param name="migrationFunction">Upgrade method</param>
        /// <param name="version">Target version</param>
        /// <param name="fileVersion">Current config version</param>
        /// <param name="configFileTxt">Current config content</param>
        /// <returns>The upgraded config content</returns>
        private static string migrateConfigTo(Func<string, string> migrationFunction, int version, int fileVersion, string configFileTxt)
        {
            //if file already on right version doesn't migrate to that version
            if (fileVersion >= version)
                return configFileTxt;

            //else will do migration
            try
            {
                var txt = migrationFunction(configFileTxt);
                return changeVersion(txt, version.ToString());
            }
            catch (Exception) 
            {
                return configFileTxt;
            }
        }

        //With Xdocument this function is not necessary! It's only .ToString()
        private static string PrintXML(string xml)
        {
            string result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                string formattedXml = sReader.ReadToEnd();

                result = formattedXml;
            }
            catch (XmlException) { }

            mStream.Close();
            writer.Close();

            return result;
        }

        private static void makeCopyConfig(string pathConfig)
        {
            string pathToConfig = Path.GetDirectoryName(pathConfig);
            string pathConfigDest = Path.Combine(pathToConfig, "Configuracoes" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml");

            System.IO.File.Copy(pathConfig, pathConfigDest);
        }

        private static string changeVersion(string text, string versionToMigrate)
        {
            int configVersionPosition = text.IndexOf("configVersion=\"", StringComparison.InvariantCultureIgnoreCase);
            if (configVersionPosition != -1)
            {
                string aux = text.Substring(0, configVersionPosition + 15);
                aux += versionToMigrate;
                aux += text.Substring(text.IndexOf("\"", configVersionPosition + 16));

                return aux;
            }

            return text;
        }

        /// <summary>
        /// Will add new attribute with name "configVersion" to save the version for configuration file
        /// </summary>
        /// <param name="fileVersion">Receive the actual file version</param>
        /// <param name="configFileTxt">String with configuration file content</param>
        /// <returns>String with replacement of configuration file</returns>
        private static string migrateConfigToVersion1(int fileVersion, string configFileTxt)
        {
            //if file already on right version doesn't migrate to that version
            if (fileVersion >= 1)
                return configFileTxt;

            //else will do migration
            XmlDocument configFile = new XmlDocument();
            try
            {
                configFile.LoadXml(configFileTxt);

                XmlNode configurationXMLNode = configFile.ChildNodes[1]; //At this version the first child is where the attribute will be
                XmlAttribute configVersionAtt = configFile.CreateAttribute("configVersion");
                configVersionAtt.Value = "1";
                configurationXMLNode.Attributes.Append(configVersionAtt);

                //at this moment the file is XML so I will save like that
                return PrintXML(configFile.OuterXml);
            }
            catch (Exception) { }

            return configFileTxt;
        }

        /// <summary>
        /// Will do the migration to new functions names in english
        /// </summary>
        /// <param name="fileVersion">Receive the actual file version</param>
        /// <param name="configFileTxt">String with configuration file content</param>
        /// <returns>String with replacement of configuration file</returns>
        private static string migrateConfigToVersion2(int fileVersion, string configFileTxt)
        {
            //if file already on right version doesn't migrate to that version
            if (fileVersion >= 2)
                return configFileTxt;

            //else will do migration
            XmlDocument configFile = new XmlDocument();
            try
            {
                configFile.LoadXml(configFileTxt);

                //Change ConfiguracaoXML to ConfigurationXML
                configFile.InnerXml = configFile.InnerXml.Replace("ConfiguracaoXML", "ConfigurationXML");

                //e.g.
                //"Servidor.security.QuidgestIdentityProvider"  > "GenioServer.security.QuidgestIdentityProvider"
                //"Servidor.security.QuidgestRoleProvider" > "GenioServer.security.QuidgestRoleProvider"
                XmlNodeList oldServerIdentificationNodeLst = configFile.SelectNodes("//*[@*[contains(.,'Servidor')]]");
                foreach (XmlNode node in oldServerIdentificationNodeLst)
                    node.Attributes["type"].Value = node.Attributes["type"].Value.Replace("Servidor.", "GenioServer.");

                //at this moment the file is XML so I will save like that
                return PrintXML(changeVersion(configFile.OuterXml, "2"));
            }
            catch (Exception) { }

            return configFileTxt;
        }

        #region Migration Aux Functions

        /* *********** Migration: Version 3 **************/

        private static string getValueFromXml(XmlDocument xmldoc, string xpath)
        {
            XmlNode node = xmldoc.SelectSingleNode(xpath);
            if (node != null && node.FirstChild != null)
                return node.FirstChild.Value;
            return "";
        }

        /// <summary>
        /// Preenche os dados das bds dada uma configuração legacy
        /// </summary>
        /// <param name="lerXML">A configuração</param>
        /// <returns>A lista de acessos</returns>
        private static List<DataSystemXml> PreencherBds(XmlDocument xmlConfig)
        {
            List<DataSystemXml> res = new List<DataSystemXml>();

            foreach (string Qyear in getValueFromXml(xmlConfig, "/ConfigurationXML/anos").Split('|'))
            {
                DataSystemXml legacy = new DataSystemXml();
                legacy.Name = Qyear;
                if (string.IsNullOrEmpty(legacy.Name))
                    legacy.Name = "0";
                legacy.Login = getValueFromXml(xmlConfig, "/ConfigurationXML/login");
                legacy.Password = getValueFromXml(xmlConfig, "/ConfigurationXML/password");
                legacy.Type = getValueFromXml(xmlConfig, "/ConfigurationXML/tpServidor");
                legacy.Server = getValueFromXml(xmlConfig, "/ConfigurationXML/servidor");
                legacy.Port = getValueFromXml(xmlConfig, "/ConfigurationXML/porto");
                legacy.TnsName = getValueFromXml(xmlConfig, "/ConfigurationXML/tnsname");
                legacy.Service = getValueFromXml(xmlConfig, "/ConfigurationXML/servico");
				legacy.ServiceName = getValueFromXml(xmlConfig, "/ConfigurationXML/serviceName");

                DataXml schema = new DataXml();
                schema.Id = Configuration.Program;
                if (getValueFromXml(xmlConfig, "/ConfigurationXML/omiteAnos").Equals("S"))
                    schema.Schema = getValueFromXml(xmlConfig, "/ConfigurationXML/div");
                else
                    schema.Schema = getValueFromXml(xmlConfig, "/ConfigurationXML/div") + Qyear;
                legacy.Schemas = new List<DataXml>();
                legacy.Schemas.Add(schema);

                res.Add(legacy);
            }

            return res;
        }

        /// <summary>
        /// Preenche os dados da db auxiliar
        /// </summary>
        /// <param name="dadosBds">A configuração legacy de bds auxiliares</param>
        /// <returns>A lista de acessos</returns>
        private static List<DataSystemXml> PreencherBds_auxiliares(string dadosBds, string dbType)
        {
            List<DataSystemXml> res = new List<DataSystemXml>();

            if (!string.IsNullOrEmpty(dadosBds))
            {
                string[] bds = dadosBds.Split('|');
                for (int i = 0; i < bds.Length; i++)
                {
                    //(servidor|name da db|login|password)
                    string[] parse = bds[i].Split('[');

                    DataSystemXml legacy = new DataSystemXml();
                    legacy.Name = parse[0];
                    legacy.Login = parse[3];
                    legacy.Password = parse[4];
                    legacy.Type = dbType; //dataSystems[0].Type; //isto Ã© por compatibilidade
                    legacy.Server = parse[1];
                    legacy.Port = "";

                    DataXml schema = new DataXml();
                    schema.Id = Configuration.Program;
                    schema.Schema = parse[2];
                    legacy.Schemas = new List<DataXml>();
                    legacy.Schemas.Add(schema);

                    res.Add(legacy);
                }
            }

            return res;
        }
        /* *********** End Migration: Version 3 **************/

        #endregion

        private static string migrateConfigToVersion3(int fileVersion, string configFileTxt)
        {
            //if file already on right version doesn't migrate to that version
            if (fileVersion >= 3)
                return configFileTxt;

            //else will do migration
            XmlDocument configFile = new XmlDocument();
            try
            {
                configFile.LoadXml(configFileTxt);

                //The configuration file have to have 2 childs
                if (configFile.ChildNodes.Count != 2)
                    return configFileTxt;

                //convert legacy to new dataSytems
                XmlNode node = configFile.SelectSingleNode("/ConfigurationXML/DataSystems");
                if (node == null || node.ChildNodes.Count == 0)
                {
                    if (node == null) //if not exist will create
                    {
                        XmlNode auxNode = configFile.CreateElement("DataSystems");
                        configFile.DocumentElement.AppendChild(auxNode);

                        node = auxNode;
                    }

                    List<DataSystemXml> lstDB = PreencherBds(configFile);
                    foreach (DataSystemXml db in lstDB)
                    {
                        using (var stringwriter = new System.IO.StringWriter())
                        {
                            var stringwriterSettings = new XmlWriterSettings();
                            stringwriterSettings.OmitXmlDeclaration = true;
                            stringwriterSettings.Indent = false;

                            using (var writer = XmlWriter.Create(stringwriter, stringwriterSettings))
                            {
                                var serializer = new System.Xml.Serialization.XmlSerializer(db.GetType());
                                serializer.Serialize(writer, db);
                                node.InnerXml += stringwriter.ToString();
                            }
                        }
                    }
                }
                //******************************************************

                //convert node DB Aux
                node = configFile.SelectSingleNode("/ConfigurationXML/bds_auxiliares");
                if (node != null)
                {
                    XmlNode auxSystemsNode = configFile.SelectSingleNode("/ConfigurationXML/AuxSystems");
                    if (auxSystemsNode == null) //if not exist will create
                    {
                        XmlNode auxNode = configFile.CreateElement("AuxSystems");
                        configFile.DocumentElement.AppendChild(auxNode);

                        auxSystemsNode = auxNode;
                    }

                    //get old structure and will migrate them
                    if (node.FirstChild != null)
                    {
                        List<DataSystemXml> lstDBAux = PreencherBds_auxiliares(node.FirstChild.Value, "SQLSERVER");
                        foreach (DataSystemXml dbAux in lstDBAux)
                        {
                            using (var stringwriter = new System.IO.StringWriter())
                            {
                                var stringwriterSettings = new XmlWriterSettings();
                                stringwriterSettings.OmitXmlDeclaration = true;
                                stringwriterSettings.Indent = false;

                                using (var writer = XmlWriter.Create(stringwriter, stringwriterSettings))
                                {
                                    var serializer = new System.Xml.Serialization.XmlSerializer(dbAux.GetType());
                                    serializer.Serialize(writer, dbAux);
                                    auxSystemsNode.InnerXml += stringwriter.ToString();
                                }
                            }
                        }
                    }
                }
                //******************************************************

                //delete legacy nodes
                node = configFile.SelectSingleNode("/ConfigurationXML/anos"); if (node != null) configFile.ChildNodes[1].RemoveChild(node); //remover anos
                node = configFile.SelectSingleNode("/ConfigurationXML/login"); if (node != null) configFile.ChildNodes[1].RemoveChild(node); //remover user
                node = configFile.SelectSingleNode("/ConfigurationXML/password"); if (node != null) configFile.ChildNodes[1].RemoveChild(node); //remover password
                node = configFile.SelectSingleNode("/ConfigurationXML/tpServidor"); if (node != null) configFile.ChildNodes[1].RemoveChild(node); //remover tpServidor
                node = configFile.SelectSingleNode("/ConfigurationXML/servidor"); if (node != null) configFile.ChildNodes[1].RemoveChild(node); //remover servidor
                node = configFile.SelectSingleNode("/ConfigurationXML/porto"); if (node != null) configFile.ChildNodes[1].RemoveChild(node); //remover porto
                node = configFile.SelectSingleNode("/ConfigurationXML/tnsname"); if (node != null) configFile.ChildNodes[1].RemoveChild(node); //remover tnsname
                node = configFile.SelectSingleNode("/ConfigurationXML/div"); if (node != null) configFile.ChildNodes[1].RemoveChild(node); //remover div

                node = configFile.SelectSingleNode("/ConfigurationXML/bds_auxiliares"); if (node != null) configFile.ChildNodes[1].RemoveChild(node); //remover bds_auxiliares

                return PrintXML(changeVersion(configFile.OuterXml, "3"));
            }
            catch (Exception) { }

            return configFileTxt;
        }

        private static string migrateConfigToVersion4(int fileVersion, string configFileTxt)
        {
            //if file already on right version doen't migrate to that version
            if (fileVersion >= 4)
                return configFileTxt;

            //else will do migration
            XmlDocument configFile = new XmlDocument();
            configFile.LoadXml(configFileTxt);
            var configNode = configFile.SelectSingleNode("/ConfigurationXML");


            //Migrate security
            XmlNode securityNode = configFile.SelectSingleNode("/ConfigurationXML/security");
            XmlElement newSecurity = configFile.CreateElement("Security");
            if(securityNode != null)
            {
                //For each app copy current security contents
                foreach(var app in ClientApplication.Applications)
                {
                    XmlNode appSecurity = configFile.CreateElement("AppSecurity");
                    appSecurity.InnerXml = securityNode.InnerXml;
                    for (int i = 0; i < securityNode.Attributes.Count; i++)
                        appSecurity.Attributes.Append((XmlAttribute) securityNode.Attributes[i].CloneNode(false));

                    XmlAttribute appid = configFile.CreateAttribute("appid");
                    appid.Value = app.Id;
                    appSecurity.Attributes.Prepend(appid);

                    newSecurity.AppendChild(appSecurity);
                }
                configNode.InsertBefore(newSecurity, securityNode);
                configNode.RemoveChild(securityNode);
            }

            //Migrate paths
            XmlNode documentPathNode = configFile.SelectSingleNode("/ConfigurationXML/pathDocuments");
            XmlElement newPaths = configFile.CreateElement("Paths");
            if (documentPathNode != null && !String.IsNullOrEmpty(documentPathNode.InnerText))
            {
                //For each app copy current document path
                foreach (var app in ClientApplication.Applications)
                {
                    XmlNode appPath = configFile.CreateElement("AppPath");

                    XmlAttribute appid = configFile.CreateAttribute("appid");
                    appid.Value = app.Id;
                    appPath.Attributes.Prepend(appid);

                    appPath.AppendChild(documentPathNode.Clone());
                    newPaths.AppendChild(appPath);
                }
                configNode.InsertBefore(newPaths, documentPathNode);
                configNode.RemoveChild(documentPathNode);
            }

            return PrintXML(changeVersion(configFile.OuterXml, "4"));
        }

        /****************************************************************
         *
         * [APM]
         * This migration is meant to encrypt (as base64) the username and
         * password of SQL Server Reporting Services in the configuration file.
         *
         ****************************************************************/
        private static string migrateConfigToVersion5(int fileVersion, string configFileTxt)
        {
            // if the file is already on the right version, doesn't migrate.
            if (fileVersion >= 5)
                return configFileTxt;

            // else does the migration.
            XmlDocument configFile = new XmlDocument();
            try
            {
                configFile.LoadXml(configFileTxt);

                XmlNode ssrsConfig = configFile.SelectSingleNode("/ConfigurationXML/ssrsServer");
				if(ssrsConfig != null)
                {
					foreach (XmlAttribute attr in ssrsConfig.Attributes)
					{
						// converts the username and password of SQL Server Reporting Services to Base64.
						if (attr.Name.Equals("username") || attr.Name.Equals("password"))
							attr.Value = Convert.ToBase64String(Encoding.Unicode.GetBytes(attr.Value));
					}
				}

                return PrintXML(changeVersion(configFile.OuterXml, "5"));
            }
            catch (Exception)
            {
                return configFileTxt;
            }
        }

        /****************************************************************
         *
         * [APM]
         * This migration is meant to fix the incoherences in nomenclature
         * of the "Paths" tag in the configuration file.
         *
         ****************************************************************/
        private static string migrateConfigToVersion6(int fileVersion, string configFileTxt)
        {
            // if the file is already on the right version, doesn't migrate.
            if (fileVersion >= 6)
                return configFileTxt;

            // else does the migration.
            XmlDocument configFile = new XmlDocument();
            try
            {
                configFile.LoadXml(configFileTxt);

                // fixes the incoherences in the migration of document paths.
                XmlNode pathsNode = configFile.SelectSingleNode("/ConfigurationXML/Paths");
                if (pathsNode != null && !string.IsNullOrEmpty(pathsNode.InnerText))
                {
                    foreach (XmlNode app in pathsNode.ChildNodes)
                    {
                        if (app.SelectSingleNode("pathApp") == null)
                        {
                            XmlNode appPath = configFile.CreateElement("pathApp");
                            app.AppendChild(appPath);
                        }

                        // change the app id from attribute to node.
                        XmlNode appId = app.Attributes.GetNamedItem("appid");
                        if (appId != null)
                        {
                            XmlNode application = configFile.CreateElement("Application");
                            application.InnerText = appId.Value;
                            app.Attributes.RemoveNamedItem("appid");
                            app.AppendChild(application);
                        }
                    }
                }

                return PrintXML(changeVersion(configFile.OuterXml, "6"));
            }
            catch (Exception)
            {
                return configFileTxt;
            }
        }

        private static string migrateConfigToVersion7(int fileVersion, string configFileTxt)
        {
            // if the file is already on the right version, doesn't migrate.
            if (fileVersion >= 7)
                return configFileTxt;

            // else does the migration.
            XmlDocument configFile = new XmlDocument();
            try
            {
                configFile.LoadXml(configFileTxt);

                XmlNode auxNode = configFile.CreateElement("QAEnvironment");
                auxNode.InnerText = "0";
                configFile.DocumentElement.AppendChild(auxNode);

                return PrintXML(changeVersion(configFile.OuterXml, "7"));
            }
            catch (Exception)
            {
                return configFileTxt;
            }
        }

        /// <summary>
        /// Create an EmailServer object from a row with pmail data
        /// </summary>
        /// <param name="data">The data matrix with all fields of the table</param>
        /// <param name="row">The row number we want to check</param>
        /// <param name="id">The id that new object will have</param>
        private static EmailServer CreateEmailServerFromRow(DataMatrix data, int row, string id)
        {
            var email = new EmailServer()
            {
                Id = id,
                Codpmail = data.GetString(row, "pmail.codpmail"),
                Name = data.GetString(row, "pmail.Dispname"),
                From = data.GetString(row, "pmail.From"),
                SMTPServer = data.GetString(row, "pmail.SMTPServer"),
                Port = data.GetInteger(row, "pmail.Port"),
                SSL = data.GetLogic(row, "pmail.SSL") == 1,
                AuthType = data.GetLogic(row, "pmail.Auth") == 1 ? CSGenio.config.AuthType.BasicAuth : CSGenio.config.AuthType.None,
                Username = data.GetString(row, "pmail.Username"),
                Password = data.GetString(row, "pmail.Password")
            };
            return email;
        }

        /// <summary>
        /// Create email server configurations
        /// </summary>
        private static string migrateConfigToVersion8(int fileVersion, string configFileTxt)
        {
            // if the file is already on the right version, doesn't migrate.
            if (fileVersion >= 8)
                return configFileTxt;

            // else does the migration.
            XmlDocument configFile = new XmlDocument();
            List<EmailServer> emailProperties = new List<EmailServer>();

            try
            {
                configFile.LoadXml(configFileTxt);

                //It's very possible that persistent support is not accessible, wrap it all in a try/ catch
                try
                {
                    //Try to obtain the persistent support
                    PersistentSupport sp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);

                    //Obtain old hardcoded table data
                    //The old data layer for this table was removed, so the query can't have static references
                    SelectQuery query = new SelectQuery().Select("pmail", "codpmail")
                   .Select("pmail", "Dispname")
                   .Select("pmail", "From")
                   .Select("pmail", "SMTPServer")
                   .Select("pmail", "Port")
                   .Select("pmail", "SSL")
                   .Select("pmail", "Auth")
                   .Select("pmail", "Username")
                   .Select("pmail", "Password")
                   .From("NotificationEmailProperties", "pmail")
                   .Where(CriteriaSet.And().Equal(new FieldRef("pmail", "zzstate"), "0"))
                   .OrderBy(new FieldRef("pmail", "codpmail"), SortOrder.Ascending);

                    var data = sp.Execute(query);

                    //Read data to email properties
                    if (data.NumRows == 0)
                    {
                        //Create a default configuration
                        var email = Configuration.NewEmailServer();
                        email.Id = "DEFAULT";
                        emailProperties.Add(email);
                    }
                    else if (data.NumRows == 1)
                    {
                        //Migrate a configuration and name it default
                        var email = CreateEmailServerFromRow(data, 0, "DEFAULT");
                        emailProperties.Add(email);
                    }
                    else
                    {
                        //Migrate each property with id SERVER{n}
                        for (int i = 0; i < data.NumRows; i++)
                        {
                            string id = $"SERVER{i + 1}";
                            var email = CreateEmailServerFromRow(data, 0, id);
                            //Necessary check because it can happen that the migration is executed twice:
                            if(!emailProperties.Any(x=>x.Id == id))
                                emailProperties.Add(email);
                        }
                    }


                }
                catch (Exception)
                {
                    Log.Error("An error ocurred in configuration migration to version 8. It wasn't possible to read email configurations from the database.");
                    //Create a default configuration
                    emailProperties.Clear();
                    var email = Configuration.NewEmailServer();
                    email.Id = "DEFAULT";
                    emailProperties.Add(email);
                }

                //Convert emailProperties to xml string and add it to the document
                using (StringWriter textWriter = new StringWriter())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(emailProperties.GetType());
                    xmlSerializer.Serialize(textWriter, emailProperties);
                    string emailNodes = textWriter.ToString();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(emailNodes);
                    var element = configFile.CreateElement("EmailProperties");
                    element.InnerXml = doc.DocumentElement.InnerXml;
                    configFile.DocumentElement.AppendChild(element);
                }

                //Add the user registration email configuration
                var registNode = configFile.CreateElement("UserRegistrationEmail");
                //If less than one we can assume the default. If there were more we can't know which one it is.
                if(emailProperties.Count == 1)
                {
                    registNode.InnerText = "DEFAULT";
                }
                configFile.DocumentElement.AppendChild(registNode);

                return PrintXML(changeVersion(configFile.OuterXml, "8"));
            }
            catch (Exception)
            {
                return configFileTxt;
            }
        }

        /// <summary>
        /// Change Queue element attribute "ano" to "year"
        /// </summary>
        /// <param name="fileVersion"></param>
        /// <param name="configFileTxt"></param>
        /// <returns></returns>
        private static string migrateConfigToVersion9(int fileVersion, string configFileTxt)
        {
            // if the file is already on the right version, doesn't migrate.
            if (fileVersion >= 9)
                return configFileTxt;

            try
            {
                XDocument xdoc = XDocument.Parse(configFileTxt);
                var queues = xdoc.Root.Elements("MessageQueueing").Elements("Queue");
                foreach (var element in queues)
                {
                    var attList = element.Attributes().ToList();
                    var oldAtt = element.Attribute("ano");
                    if (oldAtt != null)
                    {
                        XAttribute newAtt = new XAttribute("year", oldAtt.Value);
                        attList.Add(newAtt);
                        attList.Remove(oldAtt);
                        element.ReplaceAttributes(attList);
                    }
                }
                string changedXml_Text = xdoc.Declaration.ToString() + Environment.NewLine + xdoc.ToString();
                return changeVersion(changedXml_Text, "9");
            }
            catch (Exception)
            {
                return configFileTxt;
            }
        }

        /// <summary>
        /// Change element solr to elasticsearch and add new property to the element
        /// </summary>
        /// <param name="fileVersion"></param>
        /// <param name="configFileTxt"></param>
        /// <returns></returns>
        private static string migrateConfigToVersion10(int fileVersion, string configFileTxt)
        {
            // if the file is already on the right version, doesn't migrate.
            if (fileVersion >= 10)
                return configFileTxt;

            try
            {
                XDocument xdoc = XDocument.Parse(configFileTxt);
                var solr_nodes = xdoc.Descendants("solr");
                foreach (var node in solr_nodes)
                {
                    // change from solr to elasticsearch
                    node.Name = "elasticsearch";

                    // add the new attribute index
                    var core_nodes = node.Descendants("core");
                    foreach (var core_node in core_nodes)
                    {
                        var attIndex = core_node.Attribute("index");
                        if (attIndex != null)
                        {
                            var attId = core_node.Attribute("id"); // by default will have the same value as this one
                            XAttribute attribute = new XAttribute("index", attId.Value);
                            var attributes = core_node.Attributes().ToList();
                            attributes.Insert(0, attribute);
                            core_node.Attributes().Remove();
                            core_node.Add(attributes);
                        }
                    }
                }
                string changedXml_Text = xdoc.Declaration.ToString() + Environment.NewLine + xdoc.ToString();
                return changeVersion(changedXml_Text, "10");
            }
            catch (Exception)
            {
                return configFileTxt;
            }
        }

        /// <summary>
        /// Extend authentification type for e-mail server
        /// </summary>
        /// <param name="fileVersion"></param>
        /// <param name="configFileTxt"></param>
        /// <returns></returns>
        private static string migrateConfigToVersion11(int fileVersion, string configFileTxt)
        {
            // if the file is already on the right version, doesn't migrate.
            if (fileVersion >= 11)
                return configFileTxt;

            try
            {
                XDocument xdoc = XDocument.Parse(configFileTxt);
                var emailServers = xdoc.Root.Elements("EmailProperties").Elements("EmailServer");
                foreach (var element in emailServers)
                {
                    var oldElement = element.Element("Auth");
                    if (oldElement != null)
                    {
                        bool oldValueParsed = bool.Parse(oldElement.Value);
                        XElement newElement = new XElement("AuthType", oldValueParsed ? CSGenio.config.AuthType.BasicAuth : CSGenio.config.AuthType.None);
                        oldElement.ReplaceWith(newElement);
                    }
                }
                string changedXml_Text = xdoc.Declaration.ToString() + Environment.NewLine + xdoc.ToString();
                return changeVersion(changedXml_Text, "11");
            }
            catch (Exception)
            {
                return configFileTxt;
            }
        }

        /// <summary>
        /// Convert all password advanced properties value to Base64 format
        /// </summary>
        /// <param name="fileVersion"></param>
        /// <param name="configFileTxt"></param>
        /// <returns></returns>
        private static string migrateConfigToVersion12(int fileVersion, string configFileTxt)
        {
            // if the file is already on the right version, doesn't migrate.
            if (fileVersion >= 12)
                return configFileTxt;

            try
            {
                XDocument xdoc = XDocument.Parse(configFileTxt);
                var moreProperties = xdoc.Root.Elements("maisPropriedades").Elements("item");
                foreach (var element in moreProperties)
                {                    
                    var oldElement = element;
                    var key = element.Attribute("key");
                    var keyValue = element.Attribute("value");

                    if (ExtraProperties.IsPasswordType(key.Value))
                    {
                        XElement newElement = new XElement("item");
                        newElement.SetAttributeValue("key", key.Value);
                        newElement.SetAttributeValue("value", Convert.ToBase64String(Encoding.Unicode.GetBytes(keyValue.Value)));
                        oldElement.ReplaceWith(newElement);
                    } 
                }
                string changedXml_Text = xdoc.Declaration.ToString() + Environment.NewLine + xdoc.ToString();
                return changeVersion(changedXml_Text, "12");
            }
            catch (Exception)
            {
                return configFileTxt;
            }
        }


        /// <summary>
        /// Change provider properties to key value pairs
        /// </summary>
        /// <param name="fileVersion"></param>
        /// <param name="configFileTxt"></param>
        /// <returns></returns>
        private static string Version13(string configFileTxt)
        {
            XDocument xdoc = XDocument.Parse(configFileTxt);

            //convert identity provider config into a key value dictionary
            var identityProviders = xdoc.Root.Elements("Security")
                .Elements("AppSecurity")
                .Elements("identityProviders")
                .Elements("identityProvider");
            foreach(var element in identityProviders)
            {
                var iconfig = element.Attribute("config");
                if (iconfig == null)
                    continue;

                var sconfig = iconfig.Value;
                iconfig.Remove();

                var optionsElem = new XElement("options");

                string[] keyValues = sconfig.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string keyValue in keyValues)
                {
                    string key = System.Web.HttpUtility.UrlDecode(keyValue.Substring(0, keyValue.IndexOf("=")));
                    string value = System.Web.HttpUtility.UrlDecode(keyValue.Substring(keyValue.IndexOf("=") + 1));

                    if (value.StartsWith("{"))
                    {
                        //json properties
                        var jsonOp = Newtonsoft.Json.Linq.JObject.Parse(value);
                        foreach (var jchild in jsonOp.Descendants())
                            if (jchild is Newtonsoft.Json.Linq.JProperty jprop)
                            {
                                //arrays get turned into semicolon separated strings
                                if (jprop.Value is Newtonsoft.Json.Linq.JArray array)
                                {
                                    var itemElem = new XElement("item",
                                        new XAttribute("key", jprop.Name),
                                        new XAttribute("value", string.Join(";", array.Select(a => a.ToString())))
                                    );
                                    optionsElem.Add(itemElem);
                                }
                                //normal properties just get turned into strings
                                else
                                {
                                    var itemElem = new XElement("item", new XAttribute("key", jprop.Name), new XAttribute("value", jprop.Value));
                                    optionsElem.Add(itemElem);
                                }
                            }
                    }
                    else
                    {
                        //normal properties
                        var itemElem = new XElement("item", new XAttribute("key", key), new XAttribute("value", value));
                        optionsElem.Add(itemElem);
                    }
                }

                element.Add(optionsElem);
            }

            //convert identity provider config into a key value dictionary
            var roleProviders = xdoc.Root.Elements("Security")
                .Elements("AppSecurity")
                .Elements("roleProviders")
                .Elements("roleProvider");
            foreach (var element in roleProviders)
            {
                //precond attribute has been deprecated
                var iprecond = element.Attribute("precond");
                iprecond?.Remove();

                //transform the config attribute into the Options element
                var iconfig = element.Attribute("config");
                if (iconfig == null)
                    continue;

                var sconfig = iconfig.Value;
                iconfig.Remove();

                var optionsElem = new XElement("options");

                string[] keyValues = sconfig.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string keyValue in keyValues)
                {
                    string key = System.Web.HttpUtility.UrlDecode(keyValue.Substring(0, keyValue.IndexOf("=")));
                    string value = System.Web.HttpUtility.UrlDecode(keyValue.Substring(keyValue.IndexOf("=") + 1));
                    //only normal properties were supported
                    var itemElem = new XElement("item", new XAttribute("key", key), new XAttribute("value", value));
                    optionsElem.Add(itemElem);
                }

                element.Add(optionsElem);

            }

            return xdoc.Declaration.ToString() + Environment.NewLine + xdoc.ToString();
        }

        /// <summary>
        /// Move Activate2FA into a identiy provider line
        /// </summary>
        /// <param name="fileVersion"></param>
        /// <param name="configFileTxt"></param>
        /// <returns></returns>
        private static string Version14(string configFileTxt)
        {
            XDocument xdoc = XDocument.Parse(configFileTxt);

            var securityList = xdoc.Root.Elements("Security")
                .Elements("AppSecurity");
            foreach (var element in securityList)
            {
                var iactivate = element.Attribute("activate2FA");
                if (iactivate == null)
                    continue;

                var vactivate = iactivate.Value;
                iactivate.Remove();

                if (vactivate == "TOTP")
                {
                    //create the provider declaration
                    var provider = new XElement("identityProvider",
                        new XAttribute("name", "TOTP"),
                        new XAttribute("description", "TOTP"),
                        new XAttribute("type", "GenioServer.security.TOTPIdentityProvider"),
                        new XAttribute("is2fa", "true")
                        );
                    var optionsElem = new XElement("options");
                    optionsElem.Add(new XElement("item",
                        new XAttribute("key", "Issuer"),
                        new XAttribute("value", Configuration.Program)
                        ));
                    provider.Add(optionsElem);
                    element.Element("identityProviders").Add(provider);
                }
                else if (vactivate == "WebAuth")
                {
                    //create the provider declaration
                    var provider = new XElement("identityProvider",
                        new XAttribute("name", "Webauth"),
                        new XAttribute("description", "WebAuthN"),
                        new XAttribute("type", "GenioServer.security.WebauthnIdentityProvider, GenioServer"),
                        new XAttribute("is2fa", "true")
                        );
                    var optionsElem = new XElement("options");
                    //note, its impossible to know the actual server deployment url, so we generate a default placeholder
                    optionsElem.Add(new XElement("item",
                        new XAttribute("key", "Origin"),
                        new XAttribute("value", "https://localhost:5173")
                        ));
                    provider.Add(optionsElem);
                    element.Element("identityProviders").Add(provider);
                }
            }

            return xdoc.Declaration.ToString() + Environment.NewLine + xdoc.ToString();
        }

    }
}