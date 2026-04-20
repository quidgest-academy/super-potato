using CSGenio.framework;
using NUnit.Framework;

namespace CSGenio.core.Test;

public class TestQCache
{
    private QCacheInstance qCache;

    [SetUp]
    public void SetUp()
    {
        qCache = new QCacheInstance();
    }


    [Test]
    public void PutSet()
    {
        qCache.Put("testKey", "testValue");
        var value = qCache.Get("testKey");
        Assert.That(value, Is.EqualTo("testValue"));
    }


    [Test]
    public void InvalidateCache()
    {
        qCache.Put("testKey", "testValue");

        qCache.Invalidate("testKey");
        var value = qCache.Get("testKey");

        Assert.That(value, Is.Null);
    }

    [Test]
    public void TimeoutInPut()
    {
        qCache.Put("testKey", "testValue", TimeSpan.FromMilliseconds(50));

        var value = qCache.Get("testKey");
        Assert.That(value, Is.EqualTo("testValue"));

        Thread.Sleep(50);

        value = qCache.Get("testKey");
        Assert.That(value, Is.Null);
    }
}
