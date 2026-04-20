using NUnit.Framework;
using CSGenio.framework;

namespace CSGenio.core.Test;

/// <summary>
///This is a test class for Test and is intended
///to contain all Test Unit Tests
///</summary>
public class TestFlashConversion
{

    /// <summary>
    /// Teste à função ToDateTime
    /// </summary>
    [Test]
    public void TestToDateTime()
    {
        DateTime res;
        DateTime expected;

        // null --> DateTime.MinValue
        res = FlashConversion.ToDateTime(null);
        Assert.That(res, Is.EqualTo(DateTime.MinValue));

        // DateTime.MinValue --> DateTime.MinValue
        res = FlashConversion.ToDateTime(DateTime.MinValue);
        Assert.That(res, Is.EqualTo(DateTime.MinValue));

        // DateTime.Now ok
        expected = DateTime.Now;
        res = FlashConversion.ToDateTime(expected);
        Assert.That(res, Is.EqualTo(expected));

        // DateTime.Today ok
        expected = DateTime.Today;
        res = FlashConversion.ToDateTime(expected);
        Assert.That(res, Is.EqualTo(expected));

        // 1900-01-01 ok
        expected = new DateTime(1900, 1, 1);
        res = FlashConversion.ToDateTime(expected);
        Assert.That(res, Is.EqualTo(expected));

        // 1999-12-31 ok
        expected = new DateTime(1999, 12, 31);
        res = FlashConversion.ToDateTime(expected);
        Assert.That(res, Is.EqualTo(expected));

        // 2012-12-22 ok [the world has ended yet]
        expected = new DateTime(2012, 12, 22);
        res = FlashConversion.ToDateTime(expected);
        Assert.That(res, Is.EqualTo(expected));

        // DateTime.MaxValue ok
        res = FlashConversion.ToDateTime(DateTime.MaxValue);
        Assert.That(res, Is.EqualTo(DateTime.MaxValue));

        // a string --> exception
        Assert.That(() => FlashConversion.ToDateTime("blablabla"), Throws.Exception);

        // an integer --> exception
        Assert.That(() => FlashConversion.ToDateTime(20110411), Throws.Exception);
    }

    /// <summary>
    ///Teste à função FromDateTime
    /// </summary>
    [Test]
    public void TestFromDateTime()
    {
        string? res = FlashConversion.FromDateTime(DateTime.MinValue, true, true);
        Assert.That(res, Is.EqualTo(""));

        res = FlashConversion.FromDateTime(new DateTime(1900, 2, 1), true, true);
        Assert.That(res, Is.EqualTo("1900/02/01 00:00:00"));
    }


}
