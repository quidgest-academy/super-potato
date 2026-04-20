using CSGenio.core.scheduler;
using CSGenio.framework;
using ExecuteQueryCore;

namespace Administration;

/// <summary>
/// Archives the audit logs to another database
/// </summary>
public class TransferLogsScheduledTask : IScheduledTask
{

    /// <inheritdoc/>
    public List<ScheduledTaskOption> GetOptions() {
        return [
            new ScheduledTaskOption {
                PropertyName = "yearapp",
                DisplayName = "Year",
                Optional = true,
                Description = "Database year."
        },
        ];
    }

    /// <inheritdoc/>
    public Task Process(Dictionary<string, string> options, CancellationToken stoppingToken)
    {
        var year = ScheduledTaskExtensions.GetStringOption(options, "yearapp", Configuration.DefaultYear);

        // Call log transfer from the destination database
        CSGenio.persistence.PersistentSupport logSp = CSGenio.persistence.PersistentSupport.getPersistentSupportLog(year, "");
        logSp.transferLog(false, new TransferLogOperation());

        return Task.CompletedTask;
    }
}