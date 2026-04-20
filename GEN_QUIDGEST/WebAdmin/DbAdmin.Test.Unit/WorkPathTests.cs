using CSGenio.business.async;

namespace DbAdmin.Test.Unit
{
    [TestFixture]
    public class WorkPathTests
    {
        [Test]
        public void Constructor_WithNullPath_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WorkPath(null));
        }

        [Test]
        public void Constructor_WithValidScope_ShouldSetValue()
        {
            string scope = "project/scope";
            var wp = new WorkPath(scope);

            Assert.That(wp.Value, Is.EqualTo(scope));
        }

        [Test]
        public void ToString_ReturnsScope()
        {
            string scope = "project/scope";
            var wp = new WorkPath(scope);

            Assert.That(wp.ToString(), Is.EqualTo(scope));
        }

        [Test]
        public void Intersects_SameScopes_ReturnsTrue()
        {
            string scope = "project/scope";
            var wp1 = new WorkPath(scope);
            var wp2 = new WorkPath(scope);

            Assert.Multiple(() =>
            {
                Assert.That(wp1.Intersects(wp2), Is.True);
                Assert.That(wp2.Intersects(wp1), Is.True);
            });
        }

        [Test]
        public void Intersects_AncestorDescendant_ReturnsTrue()
        {
            // "project/scope" is an ancestor of "project/scope/subscope"
            var ancestor = new WorkPath("project/scope");
            var descendant = new WorkPath("project/scope/subscope");

            Assert.Multiple(() =>
            {
                Assert.That(ancestor.Intersects(descendant), Is.True);
                Assert.That(descendant.Intersects(ancestor), Is.True);
            });
        }

        [Test]
        public void Intersects_TrailingSlashVariations_ReturnsTrue()
        {
            // One scope has a trailing slash while the other does not.
            var wp1 = new WorkPath("project/scope");
            var wp2 = new WorkPath("project/scope/");

            Assert.Multiple(() =>
            {
                Assert.That(wp1.Intersects(wp2), Is.True);
                Assert.That(wp2.Intersects(wp1), Is.True);
            });
        }

        [Test]
        public void Intersects_CaseInsensitive_ReturnsTrue()
        {
            var wp1 = new WorkPath("Project/Scope");
            var wp2 = new WorkPath("project/scope/subscope");

            Assert.Multiple(() =>
            {
                Assert.That(wp1.Intersects(wp2), Is.True);
                Assert.That(wp2.Intersects(wp1), Is.True);
            });
        }

        [Test]
        public void Intersects_SiblingPath_ReturnsFalse()
        {
            // "project/scope" is not an ancestor of "project/scopeExtra"
            var wp1 = new WorkPath("project/scope");
            var wp2 = new WorkPath("project/scopeExtra");

            Assert.Multiple(() =>
            {
                Assert.That(wp1.Intersects(wp2), Is.False);
                Assert.That(wp2.Intersects(wp1), Is.False);
            });
        }

        [Test]
        public void Intersects_DifferentScopes_ReturnsFalse()
        {
            var wp1 = new WorkPath("project/scope");
            var wp2 = new WorkPath("anotherProject/otherScope");

            Assert.Multiple(() =>
            {
                Assert.That(wp1.Intersects(wp2), Is.False);
                Assert.That(wp2.Intersects(wp1), Is.False);
            });
        }

        [Test]
        public void Intersects_WhenOtherIsNull_ThrowsArgumentNullException()
        {
            var wp = new WorkPath("project/scope");

            Assert.Throws<ArgumentNullException>(() => wp.Intersects(null));
        }
    }
}
