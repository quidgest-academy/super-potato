using CSGenio.framework;
using NUnit.Framework;

namespace CSGenio.core.Test;

public class TestCrypto
{
    [Test]
    public void EncryptDecryptPayloadRoundtrip()
    {
        const string message = "xpto";
        var cypher = QResources.CreatePayloadEncryptedBase64(message);
        var plain = QResources.DecryptPayloadBase64(cypher);
        Assert.That(plain, Is.EqualTo(message));
    }

    [Test]
    public void EncryptDecryptTicketRoundtrip()
    {
        ResourceUser message = new ResourceUser("xpto", "12345");

        var cypher = QResources.CreateTicketEncryptedBase64("xpto", "localhost", message, true);
        var plain = QResources.DecryptTicketBase64(cypher);
        Assert.That(plain[0], Is.EqualTo("xpto"));
        Assert.That(plain[1], Is.EqualTo("localhost"));

        var res = plain[2] as ResourceUser;
        Assert.That(res, Is.Not.Null);
        if (res is null)
            return;

        Assert.That(res.ID, Is.EqualTo("12345"));
        Assert.That(res.Name, Is.EqualTo("xpto"));

        Assert.That(plain[3], Is.EqualTo(true));
    }

}
