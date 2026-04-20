using CSGenio;
using CSGenio.framework;
using GenioServer.framework;
using NUnit.Framework;
using CSGenio.config;
using System.Reflection.PortableExecutable;

namespace DbAdmin.IntegrationTest
{
    public class ConfigurationXmlFile
    {
        private static string workspace = Path.Combine(Path.GetTempPath(), "IntegrationTests");
        private static string configPathEnv;

        [SetUp]
        public void Setup()
        {
            CSGenio.GenioDIDefault.UseLog();
            if (Directory.Exists(workspace))
            {
                Directory.Delete(workspace, recursive: true);
            }
            Directory.CreateDirectory(workspace);

            // Save env variables that are currently declared in the machine
            // This is to make sure the tests don't mess up the value
            configPathEnv = Environment.GetEnvironmentVariable("CONFIG_PATH");

#if(DEBUG)
            //This tests don't execute in debug compilation, because this mode fetches the configuration from aditional places and the reliability can't be ensured
            Assert.Ignore("Skipping test because it's running in Debug mode.");
#endif
        }

        [TearDown]
        public void Teardown()
        {
            // Set the machine value of the envs back to the original
            Environment.SetEnvironmentVariable("CONFIG_PATH", configPathEnv);
        }

        [Test]
        public void CheckIfNotExists()
        {        
            var envVar = Environment.GetEnvironmentVariable("CONFIG_PATH");
            Assert.IsNull(envVar, $"CONFIG_PATH environment variable is not null: {envVar}");

            FileConfigurationManager manager = new FileConfigurationManager("C:\\invalidPath");

            bool result = manager.Exists();

            Assert.IsFalse(result, $"A file was found in {manager.GetFileLocation()}");
        }


        [Test]
        public void CreateNewConfigFile()
        {
            var workingSpaceDir = Path.Combine(workspace, "newFile");
            CreateConfig(workingSpaceDir, out FileConfigurationManager manager);

            string destination = Path.Combine(workingSpaceDir, "Configuracoes.xml");
            Assert.IsTrue(File.Exists(destination));

            var readConfig = manager.GetExistingConfig();
            Assert.AreEqual(ConfigXMLMigration.CurConfigurationVerion.ToString(), readConfig.ConfigVersion);
        }

        [Test]
        public void CreateNewConfigFileEnv()
        {
            var workingSpaceDir = Path.Combine(workspace, "newFileEnv");
            Environment.SetEnvironmentVariable("CONFIG_PATH", workingSpaceDir);
            Directory.CreateDirectory(workingSpaceDir);

            CreateConfig("C:\\invalidPath", out FileConfigurationManager manager);

            string destination = Path.Combine(workingSpaceDir, "Configuracoes.xml");
            Assert.IsTrue(File.Exists(destination));

            var readConfig = manager.GetExistingConfig();
            Assert.AreEqual(ConfigXMLMigration.CurConfigurationVerion.ToString(), readConfig.ConfigVersion);
        }

        [Test]
        public void CreateOverExistingConfig()
        {
            var workingSpaceDir = Path.Combine(workspace, "createExisting");
            CreateConfig(workingSpaceDir, out FileConfigurationManager manager);

            //Act and Assert
            Assert.Throws<FrameworkException>(()=>
                manager.CreateNewConfig()
            );
        }

        [Test]
        public void StoreConfig()
        {
            //Arrange
            var workingSpaceDir = Path.Combine(workspace, "storeFile");
            var config = CreateConfig(workingSpaceDir, out FileConfigurationManager manager);

            //Act
            config.ConfigVersion = "3";
            manager.StoreConfig(config);

            //Assert
            string destination = Path.Combine(workingSpaceDir, "Configuracoes.xml");
            Assert.IsTrue(File.Exists(destination));
            var readConfig = manager.GetExistingConfig();      
            Assert.AreEqual("3", readConfig.ConfigVersion);
        }

        private ConfigurationXML CreateConfig(string workingSpaceDir)
        {
            return CreateConfig(workingSpaceDir, out FileConfigurationManager _);
        }

        private ConfigurationXML CreateConfig(string workingSpaceDir, out FileConfigurationManager manager)
        {
            Directory.CreateDirectory(workingSpaceDir);
            manager = new FileConfigurationManager(workingSpaceDir);
            
            return manager.CreateNewConfig();
        }
    }
}
