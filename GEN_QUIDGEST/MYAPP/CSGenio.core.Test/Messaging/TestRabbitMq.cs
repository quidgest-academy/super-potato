using CSGenio.core.messaging;
using CSGenio.framework;
using NUnit.Framework;

namespace CSGenio.core.Test.Messaging;

public class TestRabbitMq : TestMessageProviderBase
{

    [SetUp]
    public void SetupTest()
    {
        if (!Configuration.Messaging.Enabled)
            Assert.Ignore("Message queueing is not enabled");
        if (Configuration.Messaging.Host.Provider != "RabbitMq")
            Assert.Ignore("Message queueing is not setup for RabbitMq");
    }


    [Test]
    public void SimpleSendMessage()
    {
        var handler = new MessageCaptureHandler();
        var mq = new RabbitMqProvider(metadata1, handler);
        TestSimpleSendMessage(mq, handler);
    }


    [Test]
    public void SimpleSendAck()
    {
        var handler = new MessageCaptureHandler();
        var mq = new RabbitMqProvider(metadata1, handler);
        TestSimpleSendAck(mq, handler);
    }


    [Test]
    public void MessagesReceivedInSequence()
    {
        var handler = new MessageCaptureHandler();
        var mq = new RabbitMqProvider(metadata1, handler);
        TestMessagesReceivedInSequence(mq, handler);
    }
}
