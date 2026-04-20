using CSGenio.business;
using CSGenio.business.async;
using CSGenio.framework;
using CSGenio.persistence;
using NUnit.Framework;
using Quidgest.Persistence.GenericQuery;

namespace DbAdmin.IntegrationTest
{
    /// <summary>
    /// All testes in this class assume that:
    /// 1. There are now pending processes in the database
    /// 2. There ins't aren't concurrent calls to the scheduler
    /// </summary>
    public class SchedulerTest 
    {
        private PersistentSupport sp;
        private User _user;


        [SetUp] 
        public void SetUp() 
        {
            CSGenio.GenioDIDefault.UseLog();
            CSGenio.GenioDIDefault.UseDatabase();
            sp = PersistentSupport.getPersistentSupport(Configuration.DefaultYear);
            _user = SysConfiguration.CreateWebAdminUser(Configuration.DefaultYear, "TestUser");
            
            //Register any test class here, since they can't be found by the default JobFinder which only searches in GenioServer assembly
            var jobFinder = new TestJobFinder();
            jobFinder.RegisterType(typeof(TestSuccessProcess));
            jobFinder.RegisterType(typeof(TestAsyncProcess));
            SchedulerBroker.SetupBroker(jobFinder);
            
        }

        [Test]
        public void CheckNoJobExists()
        {
            SchedulerBroker scheduler = SchedulerBroker.GetBroker();

            var work = scheduler.GetWork(_user);

            Assert.IsNull(work);            
        }

        [Test]
        public void JobExecutes()
        {
            var job = new TestSuccessProcess();

            sp.openTransaction();
            var jobId = job.Schedule(sp, _user);
            sp.closeTransaction();

            Assert.That(jobId, Is.Not.Null);
            Assert.That(jobId, Is.Not.Empty);

            SchedulerBroker scheduler = SchedulerBroker.GetBroker();
            GenioWork work = (GenioWork) scheduler.GetWork(_user);
            Assert.IsNotNull(work);
            Assert.AreEqual(ArrayS_prstat.E_AG_3, work.Process.ValRtstatus);            
            Assert.That(jobId, Is.EqualTo(work.Process.QPrimaryKey));

            work.DoWork(_user);
            Assert.AreEqual(ArrayS_prstat.E_T_4, work.Process.ValRtstatus);

            //Teardown
            sp.openTransaction();
            var process = CSGenioAs_apr.search(sp, jobId, _user);
            process.delete(sp);
            sp.closeTransaction();
        }

        [Test]
        public void JobExecutesAsync()
        {
            var job = new TestAsyncProcess();

            sp.openTransaction();
            var jobId = job.Schedule(sp, _user);
            sp.closeTransaction();

            Assert.That(jobId, Is.Not.Null);
            Assert.That(jobId, Is.Not.Empty);

            SchedulerBroker scheduler = SchedulerBroker.GetBroker();
            GenioWork work = (GenioWork)scheduler.GetWork(_user);
            Assert.IsNotNull(work);
            Assert.AreEqual(ArrayS_prstat.E_AG_3, work.Process.ValRtstatus);
            Assert.That(jobId, Is.EqualTo(work.Process.QPrimaryKey));

            work.DoWork(_user);
            Assert.AreEqual(ArrayS_prstat.E_T_4, work.Process.ValRtstatus);

            //Teardown
            sp.openTransaction();
            var process = CSGenioAs_apr.search(sp, jobId, _user);
            process.delete(sp);
            sp.closeTransaction();
        }

        [Test]
        public void InvalidJobIsIgnored()
        {
            var validJob = new TestAsyncProcess();
            var invalidJob = new InvalidAsyncProcess();

            sp.openTransaction();
            var invalidJobId = invalidJob.Schedule(sp, _user);
            var jobId = validJob.Schedule(sp, _user);        
            sp.closeTransaction();

            Assert.That(jobId, Is.Not.Null);
            Assert.That(jobId, Is.Not.Empty);

            SchedulerBroker scheduler = SchedulerBroker.GetBroker();
            GenioWork work = (GenioWork)scheduler.GetWork(_user);
            Assert.IsNotNull(work);
            Assert.AreEqual(ArrayS_prstat.E_AG_3, work.Process.ValRtstatus);
            Assert.That(jobId, Is.EqualTo(work.Process.QPrimaryKey));

            work.DoWork(_user);
            Assert.AreEqual(ArrayS_prstat.E_T_4, work.Process.ValRtstatus);

            //Teardown
            sp.openTransaction();
            var validProcess = CSGenioAs_apr.search(sp, jobId, _user);
            validProcess.delete(sp);
            var invalidProcess = CSGenioAs_apr.search(sp, invalidJobId, _user);
            invalidProcess.delete(sp);
            sp.closeTransaction();
        }
    }
}
