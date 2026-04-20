using CSGenio.core.di;
using CSGenio.core.messaging;
using CSGenio.framework;
using CSGenio.messaging;

namespace Administration;

public class MessagingServiceHost : IHostedService
{
    private MessagingService _messagingService;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if(Configuration.Messaging.Enabled)
        {
            _messagingService = GenioDI.Messaging;
            _messagingService.Start(
                metadata: MessageMetadataFactory.GeneratedMetadata(),
                providerType: Configuration.Messaging.Host.Provider,
                enableSubscribe: true
            );
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _messagingService?.Close();
        return Task.CompletedTask;
    }
}
