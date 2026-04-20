using CSGenio.business.async;

namespace DbAdmin.Test.Unit
{
    [TestFixture]
    public class WorkUnitTests
    {
        [Test]
        public void Constructor_WithNullWorkPath_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WorkUnit((WorkPath?)null));
        }

        [Test]
        public void Constructor_WithNullMaybePath_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new WorkUnit((string?)null));
        }

        [Test]
        public void Constructor_WithEmptyMaybePath_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new WorkUnit(""));
        }

        [Test]
        public void Constructor_WithWhitespaceMaybePath_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new WorkUnit("   "));
        }

        [Test]
        public void Constructor_WithValidWorkPath_ShouldSetScope()
        {
            var workPath = new WorkPath("global/project/scope");
            var workUnit = new WorkUnit(workPath);

            Assert.That(workUnit.Scope.Value, Is.EqualTo(workPath.Value));
        }

        [Test]
        public void Constructor_WithMaybePathStartingWithGlobal_ShouldNotAddPrefix()
        {
            string maybePath = "global/orgs/quidgest/projects/gen";
            var workUnit = new WorkUnit(maybePath);

            Assert.That(workUnit.Scope.Value, Is.EqualTo(maybePath));
        }

        [Test]
        public void Constructor_WithMaybePathNotStartingWithGlobal_ShouldPrefixWithGlobal()
        {
            string maybePath = "orgs/quidgest/projects/gen";
            var workUnit = new WorkUnit(maybePath);

            Assert.That(workUnit.Scope.Value, Is.EqualTo($"global/{maybePath}"));
        }

        [Test]
        public void CollidesWith_IntersectingScopes_ReturnsTrue()
        {
            // "project/scope" becomes "global/project/scope"
            // "project/scope/subscope" becomes "global/project/scope/subscope"
            var unit1 = new WorkUnit("project/scope");
            var unit2 = new WorkUnit("project/scope/subscope");

            Assert.Multiple(() =>
            {
                // They intersect because one scope is an ancestor of the other.
                Assert.That(unit1.CollidesWith(unit2), Is.True);
                Assert.That(unit2.CollidesWith(unit1), Is.True);
            });
        }

        [Test]
        public void CollidesWith_NonIntersectingScopes_ReturnsFalse()
        {
            // "project/scope" becomes "global/project/scope"
            // "anotherProject/scope" becomes "global/anotherProject/scope"
            var unit1 = new WorkUnit("project/scope");
            var unit2 = new WorkUnit("anotherProject/scope");

            Assert.Multiple(() =>
            {
                Assert.That(unit1.CollidesWith(unit2), Is.False);
                Assert.That(unit2.CollidesWith(unit1), Is.False);
            });
        }

        [Test]
        public void CollidesWith_WhenOtherIsNull_ThrowsArgumentNullException()
        {
            var unit = new WorkUnit("project/scope");

            Assert.Throws<ArgumentNullException>(() => unit.CollidesWith(null));
        }

        [Test]
        public void ToString_ReturnsScopeString()
        {
            string maybePath = "global/project/scope";
            var unit = new WorkUnit(maybePath);

            Assert.That(unit.ToString(), Is.EqualTo(maybePath));
        }
    }
}
