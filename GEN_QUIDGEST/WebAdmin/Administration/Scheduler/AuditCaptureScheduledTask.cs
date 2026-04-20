using CSGenio.core.scheduler;
using CSGenio.framework;
using CSGenio.persistence;

namespace Administration;

/// <summary>
/// Aggregates all the captured audit data into archive tables
/// </summary>
public class AuditCaptureScheduledTask : IScheduledTask
{

    /// <inheritdoc/>
    public List<ScheduledTaskOption> GetOptions() {
        return [
            new ScheduledTaskOption {
                PropertyName = "yearapp",
                DisplayName = "Year",
                Optional = true,
                Description = "Database year. Blank for the default year."
        }
        ];
    }

    /// <inheritdoc/>
    public Task Process(Dictionary<string, string> options, CancellationToken stoppingToken)
    {
        var year = ScheduledTaskExtensions.GetStringOption(options, "yearapp", Configuration.DefaultYear);
        var sp = PersistentSupport.getPersistentSupport(year);

        sp.ExecuteProcedure("ArchiveAllCdc");

        return Task.CompletedTask;
    }
}