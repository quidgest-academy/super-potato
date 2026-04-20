using NUnit.Framework;

namespace DbAdmin.IntegrationTest
{
    internal class DatabaseTransactionTest : DatabaseTransactionFixture
    {
        [Test]
        public void CheckCommitTransactionFails()
        {
            Assert.Throws<AssertionException>(() =>
                sp.closeTransaction()
                );
        }

        [Test]
        public void CheckCloseConnectionFails()
        {
            Assert.Throws<AssertionException>(() =>
                sp.closeConnection()
                );
        }
        
        
    }
}
