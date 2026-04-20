using CommandLine;
using CSGenio;
using CSGenio.config;
using DbAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

namespace AdminCLI
{
    [Verb("dbconfig-write", HelpText = "Database Configuration")]
    class WriteConfigurationOptions
    {
        [Option('u', "username", Required = true, HelpText = "Database instance username")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, HelpText = "Database instance password")]
        public string Password { get; set; }

        [Option("server", Required = true, HelpText = "Server name or IP where the instance is located")]
        public string Server { get; set; }

        [Option("port", HelpText = "Server port (default is 1433)")]
        public string Port { get; set; }

        [Option("type", Required = true, HelpText = "Database server type")]
        public string Type { get; set; }

        [Option("schema", Required = true, HelpText = "The database schema/name")]
        public string Schema { get; set; }

        [Option("encrypt-connection", HelpText = "Encrypt the database connection")]
        public bool EncryptConnection { get; set; }

        [Option("domain-user", HelpText = "Specify if it's a domain user")]
        public bool DomainUser { get; set; }

        [Option("is-log", HelpText = "Specify if its a log database configuration")]
        public bool IsLog { get; set; }

        [Option("path", HelpText = "The location of the configuration file. Creates a new one if not exists")]
        public string Path { get; set; }

        [Option("is-db-pk", HelpText = "Specify if the primary keys are to be calculated on the database side")]
        public bool IsDbPk { get; set; }
    }

    [Verb("dbconfig-read", HelpText = "Database Configuration")]
    class ReadConfigurationOptions
    {
        [Option('y', "year", HelpText = "Database year")]
        public string Year { get; set; }

        [Option('s', "schema", HelpText = "The database schema/name")]
        public string Schema { get; set; }
    }


    [Verb("create-redirect", HelpText = "Create a new redirect file")]
    class CreateRedirectOptions
    {
        [Option("config-path", Required=true, HelpText = "The path with the Configuracoes.xml")]
        public string ConfigPath { get; set; }

        [Option("redirect-path", HelpText = "The path where the new Configuracoes.redirect.xml will be created")]
        public string RedirectPath { get; set; }
    }

    [Verb("config", HelpText = "Get or set configuration properties")]
    public class ConfigOptions
    {
        [Value(0, MetaName = "operation", Required = true, HelpText = "Operation to perform: 'get', 'set', or 'list'")]
        public string Operation { get; set; }

        [Value(1, MetaName = "property", Required = false, HelpText = "Property to get/set (e.g. --qa-env, --chatbot-url)")]
        public string Property { get; set; }

        [Value(2, MetaName = "value", Required = false, HelpText = "Value to set (only required for 'set' operation)")]
        public string Value { get; set; }

        [Option('p', "path", HelpText = "The location of the configuration file. Uses default if not specified")]
        public string Path { get; set; }
    }

    partial class AdminCLI
    {
        /// <summary>
        /// Fetches the available database types
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static List<string> FetchDatabaseTypes()
        {
            return new List<string>(Enum.GetNames(typeof(CSGenio.framework.DatabaseType)));
        }

        /// <summary>
        /// Validates the configuration options that are passed by the user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>        
        private static bool ValidateOptions(WriteConfigurationOptions options)
        {
            //Check if the inserted DB type exists
            if(string.IsNullOrEmpty(options.Type) || !FetchDatabaseTypes().Any(x => x.ToLower() == options.Type.ToLower()))
            {
                Console.WriteLine("The inserted Database Type does not exist. " + 
                    "Please make sure it is one of the following: [" + string.Join(", ", FetchDatabaseTypes().ToArray()) + "]");
                return false;
            }

            //Check if the port is valid
            if(!string.IsNullOrEmpty(options.Port))
            {
                try
                {
                    int port = Convert.ToInt32(options.Port);

                    //Check if the port number is within the range
                    if(port <= 0 || port > 65535)
                    {
                        Console.WriteLine("The port number you have inserted is invalid, please make sure it is between 1 and 65535");
                        return false;
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine("Please make sure the server port is a valid number!");
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Configures a database
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int WriteConfiguration(WriteConfigurationOptions options)
        {
            if (!ValidateOptions(options))
                return 1;

            SysConfiguration sysConfig = sysConfiguration;
            if(!String.IsNullOrEmpty(options.Path))
            {
                _configManager = new FileConfigurationManager(options.Path);
                sysConfig = new SysConfiguration(_configManager);
            }

            try
            {
                if(options.IsLog)
                    sysConfig.SaveLogDatabaseConfig(options.Username, options.Password, options.Server, options.Type, options.Schema,
                        options.Port, options.EncryptConnection, options.DomainUser);
                else
                    sysConfig.SaveDatabaseConfig(options.Username, options.Password, options.Server, options.Type, options.Schema,
                        options.Port, options.EncryptConnection, options.DomainUser, "", options.IsDbPk);
            }
            catch (Exception e)
            {
                Console.WriteLine("The following error has ocurred: \n" + e.Message);
            }
            
            Console.WriteLine("Configuration saved successfully!");
            return 0;
        }

        /// <summary>
        /// Reads the current configuration
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int ReadConfiguration(ReadConfigurationOptions options)
        {
            DataSystemXml dataSystem = sysConfiguration.ReadDatabaseConfig(options.Year, options.Schema);

            if (dataSystem == null)
            {
                Console.WriteLine("There is no data to display.");
                return 1;
            }

            DisplayDataSystem(dataSystem);
            return 0;
        }

        /// <summary>
        /// Creates a new redirect file
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static int CreateNewRedirect(CreateRedirectOptions options)
        {            
            try
            {
                var redirect = sysConfiguration.CreateRedirect(options.ConfigPath);
                string path = Path.Combine(options.RedirectPath, "Configuracoes.redirect.xml");
                RedirectXML.WriteRedirectFile(redirect, path);
                return 0;
            }
            catch (Exception ex) {             
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Displays a data system
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static void DisplayDataSystem(DataSystemXml dataSystem)
        {
            Console.WriteLine($"Name: {dataSystem.Name}");
            Console.WriteLine($"Type: {dataSystem.Type}");
            Console.WriteLine($"Server: {dataSystem.Server}");
            Console.WriteLine($"Port: {dataSystem.Port}");
            Console.WriteLine($"TnsName: {dataSystem.TnsName}");
            Console.WriteLine($"Service: {dataSystem.Service}");
            Console.WriteLine($"Login: {Encoding.UTF8.GetString(Convert.FromBase64String(dataSystem.Login))}");
            Console.WriteLine($"Password: {Encoding.UTF8.GetString(Convert.FromBase64String(dataSystem.Password))}\n");

            Console.WriteLine("Schemas:");
            foreach (DataXml schema in dataSystem.Schemas)
            {
                Console.WriteLine($"ID: {schema.Id}");
                Console.WriteLine($"Schema: {schema.Schema}");
                Console.WriteLine("Encrypt Connection: " + ((schema.ConnEncrypt == true) ? "Yes" : "No"));
                Console.WriteLine("Domain User: " + ((schema.ConnWithDomainUser == true) ? "Yes" : "No"));
                Console.WriteLine(); //Leave a blank space as a separator
            }

            Console.WriteLine("Log Schemas:");
            foreach (DataXml schema in dataSystem.DataSystemLog.Schemas)
            {
                Console.WriteLine($"ID: {schema.Id}");
                Console.WriteLine($"Schema: {schema.Schema}");
                Console.WriteLine("Encrypt Connection: " + ((schema.ConnEncrypt == true) ? "Yes" : "No"));
                Console.WriteLine("Domain User: " + ((schema.ConnWithDomainUser == true) ? "Yes" : "No"));
                Console.WriteLine(); //Leave a blank space as a separator
            }

            Console.WriteLine(); //Leave a blank space as a separator
        }

        /// <summary>
        /// Handles the config verb operations
        /// </summary>
        private static int HandleConfig(ConfigOptions options)
        {
            if (!string.IsNullOrEmpty(options.Path)) {
                _configManager = new FileConfigurationManager(options.Path);
            }
            
            switch (options.Operation.ToLower())
            {
                case "list":
                    DisplayAvailableProperties();
                    return 0;
                case "get":
                    if (string.IsNullOrEmpty(options.Property))
                    {
                        Console.WriteLine("Error: Property name is required for get operation");
                        return 1;
                    }
                    return GetConfigProperty(options.Property);
                case "set":
                    if (string.IsNullOrEmpty(options.Property))
                    {
                        Console.WriteLine("Error: Property name is required for set operation");
                        return 1;
                    }
                    if (string.IsNullOrEmpty(options.Value))
                    {
                        Console.WriteLine("Error: Value is required for set operation");
                        return 1;
                    }
                    return SetConfigProperty(options.Property, options.Value);
                default:
                    Console.WriteLine("Error: Invalid operation. Use 'get', 'set', or 'list'");
                    return 1;
            }
        }

        /// <summary>
        /// Gets all configuration types that might contain CLI-configurable properties
        /// </summary>
        private static HashSet<Type> GetConfigurationTypes()
        {
            var configTypes = new HashSet<Type>();

            // Add the main configuration class
            configTypes.Add(typeof(ConfigurationXML));

            // Find all properties in ConfigurationXML that are classes and might have CLI properties
            foreach (var prop in typeof(ConfigurationXML).GetProperties())
            {
                var propType = prop.PropertyType;
                if (propType.IsClass && propType.Namespace == "CSGenio")
                {
                    configTypes.Add(propType);
                }
            }

            return configTypes;
        }

        /// <summary>
        /// Displays all available configuration properties and their descriptions
        /// </summary>
        private static void DisplayAvailableProperties()
        {
            Console.WriteLine("Available configuration properties:");
            Console.WriteLine("=================================");

            var configTypes = GetConfigurationTypes();

            foreach (var type in configTypes)
            {
                foreach (var prop in type.GetProperties())
                {
                    var attr = prop.GetCustomAttribute<CliPropertyAttribute>();
                    if (attr != null)
                    {
                        Console.WriteLine($"{attr.Name.PadRight(20)}{attr.Description}");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Available project specific properties:");
            Console.WriteLine("=================================");
            var properties = ExtraProperties.GetAdvancedProperties().ToList();
            int maxIdLength = properties.Any() ? properties.Max(p => p.Id.Length) + 3 : 0;

            foreach (var property in properties)
            {
                Console.WriteLine($"{property.Id.PadRight(maxIdLength)}{property.Label}");
            }
        }

        /// <summary>
        /// Gets the value of a configuration property
        /// </summary>
        private static int GetConfigProperty(string propertyName)
        {
            var config = _configManager.GetExistingConfig();
            var property = FindPropertyByCommandName(propertyName);

            if (property != null)
                return PrintStandardProperty(property, propertyName, config);

            if (ExtraProperties.GetAdvancedProperties().Any(p => p.Id == propertyName))
            {
                var value = config.maisPropriedades[propertyName];
                Console.WriteLine($"{propertyName}: {value}");
                return 0;
            }

            Console.WriteLine($"Error: Property '{propertyName}' not found");
            return 1;
        }

        /// <summary>
        /// Sets the value of a configuration property
        /// </summary>
        private static int SetConfigProperty(string propertyName, string value)
        {
            var config = _configManager.GetExistingConfig();
            var property = FindPropertyByCommandName(propertyName);

            if (property != null)
                return SetStandardProperty(property, propertyName, value, config);

            if (ExtraProperties.GetAdvancedProperties().Any(p => p.Id == propertyName))
            {
                config.maisPropriedades[propertyName] = value;
                _configManager.StoreConfig(config);
                Console.WriteLine($"Successfully set {propertyName} to {value}");
                return 0;
            }

            Console.WriteLine($"Error: Property '{propertyName}' not found");
            return 1;
        }

        private static int PrintStandardProperty(PropertyInfo property, string propertyName, ConfigurationXML config)
        {
            try
            {
                var instance = GetInstanceForProperty(property, config);
                if (instance == null)
                {
                    Console.WriteLine($"Error: Could not find configuration instance for property '{propertyName}'");
                    return 1;
                }

                var value = property.GetValue(instance);
                Console.WriteLine($"{propertyName}: {value}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading property '{propertyName}': {ex.Message}");
                return 1;
            }
        }

        private static int SetStandardProperty(PropertyInfo property, string propertyName, string value, ConfigurationXML config)
        {
            try
            {
                var instance = GetInstanceForProperty(property, config);
                if (instance == null)
                {
                    Console.WriteLine($"Error: Could not find configuration instance for property '{propertyName}'");
                    return 1;
                }

                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(instance, convertedValue);
                _configManager.StoreConfig(config);
                Console.WriteLine($"Successfully set {propertyName} to {value}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting property '{propertyName}': {ex.Message}");
                return 1;
            }
        }


        /// <summary>
        /// Gets the appropriate configuration instance for a property
        /// </summary>
        private static object GetInstanceForProperty(PropertyInfo property, ConfigurationXML config)
        {
            var declaringType = property.DeclaringType;
            if (declaringType == typeof(ConfigurationXML))
            {
                return config;
            }
            
            // Find the property in ConfigurationXML that holds this type
            var containerProp = typeof(ConfigurationXML).GetProperties()
                .FirstOrDefault(p => p.PropertyType == declaringType);
            
            if (containerProp != null)
            {
                return containerProp.GetValue(config);
            }
            return null;
        }

        /// <summary>
        /// Finds a property by its CLI command name
        /// </summary>
        private static PropertyInfo FindPropertyByCommandName(string commandName)
        {
            var configTypes = GetConfigurationTypes();

            foreach (var type in configTypes)
            {
                foreach (var prop in type.GetProperties())
                {
                    var attr = prop.GetCustomAttribute<CliPropertyAttribute>();
                    if (attr != null && attr.Name == commandName)
                    {
                        return prop;
                    }
                }
            }

            return null;
        }
    }
}
