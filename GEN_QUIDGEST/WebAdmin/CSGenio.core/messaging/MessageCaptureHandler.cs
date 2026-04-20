using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSGenio.core.messaging;

/// <summary>
/// Helper handler to capture the reception events from Message Providers
/// </summary>
public class MessageCaptureHandler : IProviderMessageHandler
{
    public List<QueueMessage> ReceivedMessages { get; private set; } = new List<QueueMessage>();
    public List<AckMessage> ReceivedAcks { get; private set; } = new List<AckMessage>();

    private ManualResetEventSlim _signal = new ManualResetEventSlim();

    public Task OnReceiveAck(PublisherMetadata pub, AckMessage ack)
    {
        ReceivedAcks.Add(ack);
        _signal.Set();
        return Task.CompletedTask;
    }

    public Task OnReceiveMessage(SubscriberMetadata sub, QueueMessage msg)
    {
        ReceivedMessages.Add(msg);
        _signal.Set();
        return Task.CompletedTask;
    }

    public void WaitForMessages(int count, int timeout)
    {
        var start = DateTime.UtcNow;
        var remaining = timeout;
        while (ReceivedMessages.Count+ReceivedAcks.Count < count && remaining > 0)
        {
            _signal.Wait(remaining);
            var ellapsed = DateTime.UtcNow - start;
            remaining = timeout - (int)ellapsed.TotalMilliseconds;
        }
    }
}

/// <summary>
/// Dummy/Null Ack processor
/// </summary>
public class AckCaptureProcessor : IAckProcessor
{
    public AckMessage ReceivedAck { get; set; }
    private ManualResetEventSlim _signal = new ManualResetEventSlim();

    public void Process(AckMessage ack)
    {
        ReceivedAck = ack;
        _signal.Set();
    }

    public void WaitForMessages(int timeout)
    {
        var start = DateTime.UtcNow;
        var remaining = timeout;
        while (ReceivedAck == null && remaining > 0)
        {
            _signal.Wait(remaining);
            var ellapsed = DateTime.UtcNow - start;
            remaining = timeout - (int)ellapsed.TotalMilliseconds;
        }
    }
}

/// <summary>
/// Dummy/Null Message processor
/// </summary>
public class MessageCaptureProcessor : IMessageProcessor
{
    public AreaDataset ReceivedDataset { get; set; }
    private ManualResetEventSlim _signal = new ManualResetEventSlim();

    public void Process(IProcessorResponse response, AreaDataset dataset, SubscriberMetadata meta)
    {
        ReceivedDataset = dataset;
        _signal.Set();
    }

    public void WaitForMessages(int timeout)
    {
        var start = DateTime.UtcNow;
        var remaining = timeout;
        while (ReceivedDataset == null && remaining > 0)
        {
            _signal.Wait(remaining);
            var ellapsed = DateTime.UtcNow - start;
            remaining = timeout - (int)ellapsed.TotalMilliseconds;
        }
    }
}