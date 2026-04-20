using CSGenio.core.messaging;
using NUnit.Framework;

namespace CSGenio.core.Test.Messaging;

/// <summary>
/// Tests the equivalence of MemoryMq to other providers
/// </summary>
public class TestMessageProviderBase
{
    protected readonly MessageMetadata metadata1 = new MessageMetadata
    {
        Publishers = [
                new PublisherMetadata
                {
                    Name = "INVENTORY",
                    Group = "TEST",
                    Description = "Inventory",
                    Version = 2,
                    Ack = new AckCaptureProcessor(),
                }
            ],

        Subscribers = [
                new SubscriberMetadata
                {
                    Name = "INVENTORY",
                    Group = "TEST",
                    Description = "Test subscription",
                    Version = 2,
                    UseAck = true,
                    Processor = new MessageCaptureProcessor(),
                }
            ]
    };


    protected void TestSimpleSendMessage(IMessageProvider mq, MessageCaptureHandler handler)
    {
        mq.Start(true);

        var pub = metadata1.GetPublisher("TEST.INVENTORY");
        mq.SendMessage(pub, new QueueMessage
        {
            Envelope = new QueueMessageEnvelope
            {
                Dataset = "0",
                Id = Guid.NewGuid().ToString("N"),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Process = pub.Id,
                Version = pub.Version
            },
            Tables = [
                new QueueTable {
                    Id = "equip",
                    Columns = [
                        new QueueField { Id = "Pk" },
                        new QueueField { Id = "Name" },
                        new QueueField { Id = "Date" },
                        new QueueField { Id = "Quantity" },
                        new QueueField { Id = "Discontinued" },
                    ],
                    Rows = [
                        ["1", "xpto", "2024-01-02", 23.4m, true]
                    ]
                }
            ]
        });
        //wait a tiny bit so we have time to receive the messages before we close the connection
        handler.WaitForMessages(1, 500);
        mq.Stop();

        Assert.That(handler.ReceivedMessages.Count, Is.EqualTo(1));
        Assert.That(handler.ReceivedMessages[0].Tables.Count, Is.EqualTo(1));
        Assert.That(handler.ReceivedMessages[0].Tables[0].Rows.Count, Is.EqualTo(1));

        var row0 = handler.ReceivedMessages[0].Tables[0].Rows[0];

        Assert.That(handler.ReceivedMessages[0].Tables[0].Columns.Count, Is.EqualTo(5));
        Assert.That(row0[0], Is.EqualTo("1"));
        Assert.That(row0[1], Is.EqualTo("xpto"));
        Assert.That(row0[2], Is.EqualTo("2024-01-02"));
        Assert.That(row0[3], Is.EqualTo(23.4m));
        Assert.That(row0[4], Is.EqualTo(true));
    }


    protected void TestSimpleSendAck(IMessageProvider mq, MessageCaptureHandler handler)
    {
        mq.Start(true);

        var sub = metadata1.GetSubscriber("TEST.INVENTORY");
        mq.SendAck(sub, new AckMessage
        {
            Target = "OTHER",
            OriginId = Guid.NewGuid().ToString("N"),
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Status = AckStatus.Error,
            RowsPk =
            {
                { "INVOICE", [
                    Guid.NewGuid().ToString("N")
                    ]
                }
            },
            ErrorList = [
                new AckError {
                    Id = Guid.NewGuid().ToString("N"),
                    Table = "INVOICE",
                    Message = "some error"
                }
            ],
            Progress = 0,
        });

        //wait a tiny bit so we have time to receive the messages before we close the connection
        handler.WaitForMessages(1, 500);
        mq.Stop();

        Assert.That(handler.ReceivedAcks.Count, Is.EqualTo(1));
    }


    protected void TestMessagesReceivedInSequence(IMessageProvider mq, MessageCaptureHandler handler)
    {
        mq.Start(true);

        var pub = metadata1.GetPublisher("TEST.INVENTORY");
        mq.SendMessage(pub, new QueueMessage
        {
            Envelope = new QueueMessageEnvelope
            {
                Dataset = "0",
                Id = "1",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Process = pub.Id,
                Version = pub.Version
            },
            Tables = []
        });
        mq.SendMessage(pub, new QueueMessage
        {
            Envelope = new QueueMessageEnvelope
            {
                Dataset = "0",
                Id = "2",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Process = pub.Id,
                Version = pub.Version
            },
            Tables = []
        });
        mq.SendMessage(pub, new QueueMessage
        {
            Envelope = new QueueMessageEnvelope
            {
                Dataset = "0",
                Id = "3",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Process = pub.Id,
                Version = pub.Version
            },
            Tables = []
        });
        //wait a tiny bit so we have time to receive the messages before we close the connection
        handler.WaitForMessages(3, 500);
        mq.Stop();

        Assert.That(handler.ReceivedMessages.Count, Is.EqualTo(3));

        Assert.That(handler.ReceivedMessages[0].Envelope.Id, Is.EqualTo("1"));
        Assert.That(handler.ReceivedMessages[1].Envelope.Id, Is.EqualTo("2"));
        Assert.That(handler.ReceivedMessages[2].Envelope.Id, Is.EqualTo("3"));
    }
}
