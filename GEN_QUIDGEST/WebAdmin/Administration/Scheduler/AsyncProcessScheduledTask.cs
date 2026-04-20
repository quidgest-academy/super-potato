using CSGenio.core.scheduler;
using CSGenio.framework;
using DbAdmin;

namespace Administration;

/// <summary>
/// Processes the asyncronous process queue tables for this system
/// </summary>
public class AsyncProcessScheduledTask : IScheduledTask
{

    /// <inheritdoc/>
    public List<ScheduledTaskOption> GetOptions()
    {
        return [
            new ScheduledTaskOption {
                PropertyName = "yearapp",
                DisplayName = "Year",
                Optional = true,
                Description = "Database year."
            }
        ];
    }

    /// <inheritdoc/>
    public Task Process(Dictionary<string, string> options, CancellationToken stoppingToken)
    {
        var year = ScheduledTaskExtensions.GetStringOption(options, "yearapp", Configuration.DefaultYear);

        try
        {
            var user = SysConfiguration.CreateWebAdminUser(year);

            var worker = new CSGenio.business.async.GenioWorker(user);
            worker.Work(stoppingToken);
        }
        catch (Exception ex)
        {
            Log.Error($"Unexpected error while processing scheduled task: {ex}");
        }

        return Task.CompletedTask;
    }
}
