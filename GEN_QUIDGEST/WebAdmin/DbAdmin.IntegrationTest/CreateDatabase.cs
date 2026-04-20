using CSGenio.persistence;
using NUnit.Framework;
using System;
using CSGenio.framework;
using System.Diagnostics;
using ExecuteQueryCore;

namespace DbAdmin.IntegrationTest
{
    public class CreateDatabase
    {

        private IConfiguration config;


        [SetUp]
        public void Setup()
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            CSGenio.GenioDIDefault.UseLog();
            CSGenio.GenioDIDefault.UseDatabase();
        }

        [Test]
        public void AccessToServer()
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);
            var connection = sp.GetConnectionToServer();
            connection.Open();
            Assert.IsTrue(connection.State == System.Data.ConnectionState.Open);
        }

        [Test]
        public void ConnectToDatabase()
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);
            sp.openConnection();
            Assert.IsTrue(sp.Connection.State == System.Data.ConnectionState.Open);
        }

        private void AddDummySystem(string database) {
            var currentDataSystem = CSGenio.framework.Configuration.DataSystems[0];

            var dataSystem = currentDataSystem.ShallowCopy();
            dataSystem.Name = database;
            dataSystem.Schemas[0].Schema = database;
            CSGenio.framework.Configuration.DataSystems.Add(dataSystem);                
        }

        private RdxParamUpgradeSchema GetNewDatabaseParameters(string newDB)
        {
            var rdxParam = new RdxParamUpgradeSchema();
            var datasystem = CSGenio.framework.Configuration.ResolveDataSystem(newDB, CSGenio.framework.Configuration.DbTypes.NORMAL);
            rdxParam.Username = datasystem.LoginDecode();
            rdxParam.Password = datasystem.PasswordDecode();
            rdxParam.Year = newDB;
            rdxParam.DirFilestream = config.GetValue<string>("NewDB:FileStreamLocation");
            rdxParam.Origin = "Integration Test";
            return rdxParam;
        }

        [Test]
        public void CreateDatabaseOnly()
        {
            //Arrange
            string newDB = config["NewDB:DatabaseName"];
            AddDummySystem(newDB);
            PersistentSupport sp = PersistentSupport.getPersistentSupport(newDB);
            //Delete existing databases if any
            if (sp.CheckIfDatabaseExists(newDB))
            {
                sp.Drop(newDB);
            }
            var rdxParams = GetNewDatabaseParameters(newDB);
            var dbMaintenance = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory);

            //Act: Reindex
            dbMaintenance.StartReindexation(rdxParams, "CREATEDB", null, "");
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            int timeout = config.GetValue<int?>("NewDB:Timeout") ?? 60;
            while (!rdxParams.Progress.IsFinished() && stopWatch.Elapsed.TotalSeconds < timeout)
                Thread.Sleep(50);

            //Assert: Check if database exists
            Assert.IsTrue(sp.CheckIfDatabaseExists(newDB));
            Assert.AreEqual(ExecuteQueryCore.RdxProgressStatus.SUCCESS, rdxParams.Progress.State, rdxParams.Progress.Message);

            //Teardown
            sp.Drop(newDB);
        }


        [Test]
        public void CreateDatabaseDefaultScripts()
        {
            string newDb = config["NewDB:DatabaseName"];
            AddDummySystem(newDb);
            PersistentSupport sp = PersistentSupport.getPersistentSupport(newDb);
            //Arrange: Delete existing databases if any
            if (sp.CheckIfDatabaseExists(newDb))
            {
                sp.Drop(newDb);
            }
            var rdxParams = GetNewDatabaseParameters(newDb);
            var dbMaintenance = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory);

            //Act: Reindex
            dbMaintenance.StartReindexation(rdxParams, "", new List<string>(), "");
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var timeout = config.GetValue<int?>("NewDB:Timeout") ?? 60;
            while (!rdxParams.Progress.IsFinished() && stopWatch.Elapsed.TotalSeconds < timeout)
                Thread.Sleep(50);

            
            //Assert: Check if database exists
            Assert.IsTrue(sp.CheckIfDatabaseExists(newDb));
            Assert.AreEqual(ExecuteQueryCore.RdxProgressStatus.SUCCESS, rdxParams.Progress.State, rdxParams.Progress.Message);
            Assert.AreEqual(Configuration.VersionDbGen, Configuration.GetDbVersion(newDb));
            Assert.AreEqual(Configuration.VersionUpgrIndxGen, Configuration.GetDbUpgrIndx(newDb));
            
            //Teardown
            sp.Drop(newDb);
        }
    }
}