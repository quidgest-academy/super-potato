using CSGenio.framework;
using CSGenio.persistence;
using NUnit.Framework;

namespace WebTest
{
    public class TestConnection
    {
        [SetUp]
        public void Setup()
        {
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
    }
}
