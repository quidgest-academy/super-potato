using CSGenio.business;
using CSGenio.business.async;
using CSGenio.framework;

namespace DbAdmin.Test.Unit.async
{
    [TestFixture]
    public class GenioWorkTests
    {
        private User _user;

        [SetUp]
        public void SetUp()
        {
            _user = SysConfiguration.CreateWebAdminUser(Configuration.DefaultYear, "TestUser");
        }

        [Test]
        public void CompareWorks_HighPriorityComesFirst()
        {
            var process1 = new CSGenioAs_apr(_user, _user.CurrentModule);
            var process2 = new CSGenioAs_apr(_user, _user.CurrentModule);

            var job1 = new HighPriorityJob();
            var job2 = new NormalPriorityJob();

            var work1 = new GenioWork(process1, job1);
            var work2 = new GenioWork(process2, job2);

            Assert.That(work1.CompareTo(work2), Is.LessThan(0)); // High should come before Normal
        }

        [Test]
        public void CompareWorks_NormalPriorityComesBeforeLow()
        {
            var process1 = new CSGenioAs_apr(_user, _user.CurrentModule);
            var process2 = new CSGenioAs_apr(_user, _user.CurrentModule);

            var job1 = new NormalPriorityJob();
            var job2 = new LowPriorityJob();

            var work1 = new GenioWork(process1, job1);
            var work2 = new GenioWork(process2, job2);

            Assert.That(work1.CompareTo(work2), Is.LessThan(0)); // Normal should come before Low
        }

        [Test]
        public void CompareWorks_SamePriorityHigherValIdComesFirst()
        {
            var process1 = new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 30 };
            var process2 = new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 10 };

            var job1 = new HighPriorityJob();
            var job2 = new HighPriorityJob();

            var work1 = new GenioWork(process1, job1);
            var work2 = new GenioWork(process2, job2);

            Assert.That(work1.CompareTo(work2), Is.GreaterThan(0)); // Lower ValId should come first
        }

        [Test]
        public void CompareWorks_SortingList_CorrectOrder()
        {
            var works = new List<GenioWork>
            {
                new(new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 10 }, new NormalPriorityJob()),
                new(new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 20 }, new HighPriorityJob()),
                new(new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 30 }, new LowPriorityJob()),
                new(new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 40 }, new HighPriorityJob()),
                new(new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 50 }, new NormalPriorityJob()),
            };

            works = [.. works.OrderBy(w => w)];

            var expectedOrder = new List<GenioWork>
            {
                new(new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 20 }, new HighPriorityJob()),
                new(new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 40 }, new HighPriorityJob()),
                new(new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 10 }, new NormalPriorityJob()),
                new(new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 50 }, new NormalPriorityJob()),
                new(new CSGenioAs_apr(_user, _user.CurrentModule) { ValId = 30 }, new LowPriorityJob()),
            };

            for (int i = 0; i < works.Count; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(works[i].Priority, Is.EqualTo(expectedOrder[i].Priority));
                    Assert.That(works[i].Process.ValId, Is.EqualTo(expectedOrder[i].Process.ValId));
                });
            }
        }
    }

    public class HighPriorityJob : GenioExecutableJob
    {
        public override JobPriority Priority => JobPriority.HIGH;
    }

    public class NormalPriorityJob : GenioExecutableJob
    {
        public override JobPriority Priority => JobPriority.NORMAL;
    }

    public class LowPriorityJob : GenioExecutableJob
    {
        public override JobPriority Priority => JobPriority.LOW;
    }
}
