using CSGenio.framework;
using System;
using System.IO;

namespace CSGenio.config
{
    public class FileConfigurationManager : IConfigurationManager
    {

        private string _basePath;
        private const string CONFIG_FILE = "Configuracoes.xml";

        public FileConfigurationManager(string basePath)
        {
            _basePath = basePath;
        }

        public string GetFileLocation()
        {
            string pathConfig = _basePath;
            pathConfig = Path.Combine(pathConfig, CONFIG_FILE);

            if (!File.Exists(pathConfig))
            {
                //Check env for config path
                string envPath = Environment.GetEnvironmentVariable("CONFIG_PATH");
                if (envPath != null)
                    return Path.Combine(envPath, CONFIG_FILE);
#if DEBUG
                string debugPath = GetDebugPath();
                if (!String.IsNullOrEmpty(debugPath))
                    pathConfig = debugPath;
#endif
            }            
            return pathConfig;
        }

        public bool Exists()
        {
            var pathConfig = GetFileLocation();
            return File.Exists(pathConfig);
        }

        public ConfigurationXML GetExistingConfig()
        {
            //ler o file
            using (StreamReader input = new StreamReader(GetFileLocation()))
            {
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationXML));
                ConfigurationXML conf = (ConfigurationXML)s.Deserialize(input);
                return conf;
            }

        }

        public void StoreConfig(ConfigurationXML config)
        {
            string path = GetFileLocation();
            using (StreamWriter output = new StreamWriter(path))
            {
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(config.GetType());
                s.Serialize(output, config);
            }
        }

        public ConfigurationXML CreateNewConfig()
        {
            if(Exists())
            {
                throw new FrameworkException("Can't create a new configuration over an existing one", "CreateNewConfig", "Configuration file already exists");
            }

            var config = new ConfigurationXML();
            config.Init();
            StoreConfig(config);
            return config;
        }

        private string GetDebugPath()
        {
            var directory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            //Procurar a pasta de WebAdmin
            for (int i = 0; i <= 5 && directory != directory.Root; i++)
            {
                string webAdminDir = Path.Combine(directory.FullName, "WebAdmin");
                if (Directory.Exists(webAdminDir))
                {
                    return Path.Combine(webAdminDir, "Administration", "Configuracoes.xml");
                }
                directory = directory.Parent;
            }
            return string.Empty;
        }
    }
}
