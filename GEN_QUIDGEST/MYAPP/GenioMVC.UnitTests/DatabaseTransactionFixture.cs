using CSGenio.persistence;
using NUnit.Framework;

using CSGenio.framework;
using System;
using Castle.DynamicProxy;

namespace GenioMVC.UnitTests
{
    /// <summary>
    /// This test fixture arranges a PersistentSupport object for the default datasystem that starts with an opened transaction.
    /// In the end transactions are rolled back to revert the database to the original state;
    /// Also, any call made to commitTransaction or closeConnection will also automatically fail the test
    /// </summary>
    [TestFixture]
    public abstract class DatabaseTransactionFixture
    {
        protected PersistentSupport sp;
        protected User _user;

        [SetUp] 
        public void SetUp() 
        {
            _user = CreateDeveloperUser();

            CSGenio.GenioDIDefault.UseLog();
            CSGenio.GenioDIDefault.UseDatabase();

            //Builds a persistent support that will throw an exception if someone tries to commit a transaction
            var baseSp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);
            sp = BuildNoCommitSp(baseSp);
            

            //From here forward, if someone tries to build a PersistentSupport it will throw an error
            CSGenio.core.di.GenioDI.SpFactory = ThrowErrorBuildSp;
            sp.openTransaction();

        }

        [TearDown]
        public void TearDown()
        {
            //Rollback anything that was done
            sp.rollbackTransaction();
        }

        private User CreateDeveloperUser()
        {
            User user = new User("GenioTesterDev", "", "");
            user.Year = Configuration.DefaultYear;
            user.AddModuleRole("GWP", Role.ADMINISTRATION);
            user.CurrentModule = "GWP";
            user.Language = "en-US";
            return user;
        }

        private PersistentSupport ThrowErrorBuildSp(DatabaseType dbType)
        {
            Assert.Fail("Tried to create a persistent support outside of the test scope");
            throw new InvalidOperationException("Can't create new PersistentSupport in this test case");
        }

        /// <summary>
        /// Builds an sp that throws an exception when someone calls "closeTransaction"
        /// </summary>
        /// <param name="baseSp">An pre built persistent support</param>
        /// <returns></returns>
        private PersistentSupport BuildNoCommitSp(PersistentSupport baseSp)
        {
            ProxyGenerator generator = new ProxyGenerator();
            var sp = generator.CreateClassProxyWithTarget<PersistentSupport>(baseSp, new NoCommitInterceptor());
            return sp;
        }

    }

    /// <summary>
    /// Interceptor class that intercepts methods that close connections and commit transactions and fails the test
    /// </summary>
    public class NoCommitInterceptor : IInterceptor
    {

        public void Intercept(IInvocation invocation)
        {
            if(invocation.Method.Name == "closeTransaction" || invocation.Method.Name == "closeConnection")
            {
                //Fail the test on these calls
                Assert.Fail($"Can't commit transactions");
            }
            else
            {
                invocation.Proceed();
            }
        }


    }


}
