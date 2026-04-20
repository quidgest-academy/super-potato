using CSGenio.core.messaging;
using NUnit.Framework;

namespace CSGenio.core.Test.Messaging;

/// <summary>
/// Tests the equivalence of MemoryMq to other providers
/// </summary>
public class TestMemoryMq : TestMessageProviderBase
{

    [Test]
    public void SimpleSendMessage()
    {
        var handler = new MessageCaptureHandler();
        var mq = new MemoryMqProvider(metadata1, handler);
        TestSimpleSendMessage(mq, handler);
    }


    [Test]
    public void SimpleSendAck()
    {
        var handler = new MessageCaptureHandler();
        var mq = new MemoryMqProvider(metadata1, handler);
        TestSimpleSendAck(mq, handler);
    }


    [Test]
    public void MessagesReceivedInSequence()
    {
        var handler = new MessageCaptureHandler();
        var mq = new MemoryMqProvider(metadata1, handler);
        TestMessagesReceivedInSequence(mq, handler);
    }
}
