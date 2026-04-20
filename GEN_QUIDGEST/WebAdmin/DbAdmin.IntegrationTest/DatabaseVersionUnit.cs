using CSGenio.core.persistence;
using CSGenio.framework;
using CSGenio.persistence;
using Moq;
using NUnit.Framework;
using Quidgest.Persistence.GenericQuery;
using System;


namespace DbAdmin.IntegrationTest
{
    public class DatabaseVersionUnit
    {

        [Test]
        public void GetDbVersionSimple()
        {
            var mock = MockReturnValue(241);
            var reader = new DatabaseVersionReader(mock.Object);

            var version = reader.GetDbVersion();

            Assert.AreEqual(241, version);
        }

        [Test]
        public void GetDbIndexVersion()
        {
            var mock = MockReturnValue(242);
            var reader = new DatabaseVersionReader(mock.Object);

            var version = reader.GetDbIndexVersion();

            Assert.AreEqual(242, version);
        }

        [Test]
        public void GetDbUpgradeVersion()
        {
            var mock = MockReturnValue(243);
            var reader = new DatabaseVersionReader(mock.Object);

            var version = reader.GetDbUpgradeVersion();

            Assert.AreEqual(243, version);
        }

        [Test]
        public void GetDbVersionNoDatabase()
        {
            Mock<PersistentSupport> mock = new Mock<PersistentSupport>();
            mock.Setup(sp => sp.CheckIfDatabaseExists(It.IsAny<string>())).Returns(false);
            var reader = new DatabaseVersionReader(mock.Object);

            Assert.Throws<FrameworkException>(() =>
                reader.GetDbVersion()
            );

        }

        [Test]
        public void GetDbVersionNull()
        {
            var mock = MockReturnValue<object>(null);
            var reader = new DatabaseVersionReader(mock.Object);

            var version = reader.GetDbVersion();

            Assert.AreEqual(0, version);
        }


        [Test]
        public void GetDbVersionDBNull()
        {
            var mock = MockReturnValue(DBNull.Value);
            var reader = new DatabaseVersionReader(mock.Object);

            var version = reader.GetDbVersion();

            Assert.AreEqual(0, version);
        }

        [Test]
        public void GetDbVersionOrZero()
        {
            Mock<PersistentSupport> mock = new Mock<PersistentSupport>();
            mock.Setup(sp => sp.Connection.State).Throws(new Exception());
            var reader = new DatabaseVersionReader(mock.Object);

            var version = reader.GetDbVersionOrZero();

            Assert.AreEqual(0, version);
        }

        /// <summary>
        /// Creates a mock that will return the specified value when executeScalar is called
        /// </summary>
        /// <typeparam name="T">Type of the return value</typeparam>
        /// <param name="value">The value to be returned in executeScalar call</param>
        /// <returns>A mock of PersistentSupport</returns>        
        private Mock<PersistentSupport> MockReturnValue<T>(T value)
        {
            Mock<PersistentSupport> mock = new Mock<PersistentSupport>();
            mock.Setup(sp => sp.CheckIfDatabaseExists(It.IsAny<string>())).Returns(true);
            mock.Setup(sp => sp.ExecuteScalar(It.IsAny<SelectQuery>()))
                .Returns(value);
            return mock;
        }

    }
}
