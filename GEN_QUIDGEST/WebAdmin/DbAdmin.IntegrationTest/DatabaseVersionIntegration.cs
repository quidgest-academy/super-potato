using CSGenio.core.persistence;
using CSGenio.framework;
using CSGenio.persistence;
using NUnit.Framework;
using System;


namespace DbAdmin.IntegrationTest
{
    public class DatabaseVersionIntegration 
    {
        protected PersistentSupport sp;


        [SetUp]
        public void SetUp()
        {
            CSGenio.GenioDIDefault.UseLog();
            CSGenio.GenioDIDefault.UseDatabase();
            sp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);
        }


        [Test]
        public void GetDbVersion()
        {
            var reader = new DatabaseVersionReader(sp);

            var version = reader.GetDbVersion();

            Assert.Greater(version, 0);

        }


        [Test]
        public void GetDbIndexVersion()
        {
            var reader = new DatabaseVersionReader(sp);

            var version = reader.GetDbIndexVersion();

            Assert.Greater(version, 0);
        }

        [Test]
        public void GetDbUpgradeVersion()
        {
            var reader = new DatabaseVersionReader(sp);

            var version = reader.GetDbIndexVersion();

            Assert.Greater(version, 0);
        }

        [Test]
        public void WithOpenConnection()
        {
            var reader = new DatabaseVersionReader(sp);

            sp.openConnection();
            var version = reader.GetDbIndexVersion();
            sp.closeConnection();
            Assert.Greater(version, 0);
        }

        [Test]
        public void WithClosedConnection() 
        {
            var reader = new DatabaseVersionReader(sp);

            sp.closeConnection();
            var version = reader.GetDbIndexVersion();

            Assert.Greater(version, 0);


        }

        [Test]
        public void WithNoDatabase()
        {
            var emptySp = DatabaseUtils.GetUnexistingDatabase();
            var reader = new DatabaseVersionReader(emptySp);

            DateTime start = DateTime.Now;

            Assert.Throws<FrameworkException>(() =>
            {
                var version = reader.GetDbIndexVersion();
            });
            //Ensure that this is quick. Previous implementations took up to 5s in this scenario.
            Assert.That((start - DateTime.Now).TotalMilliseconds < 500);
        }
    }
}
