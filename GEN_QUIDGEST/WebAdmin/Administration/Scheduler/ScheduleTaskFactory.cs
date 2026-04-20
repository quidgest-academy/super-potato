using CSGenio.core.business;
using CSGenio.core.scheduler;

namespace Administration;

/// <summary>
/// Supplies a registry of all the available Scheduled Task processors
/// </summary>
public static class ScheduleTaskFactory
{
    private static Dictionary<string, IScheduledTask> m_taskList = new()
    {
        { "FormulaGroup", new FormulaGroupScheduledTask() },
        { "Reindex", new ReindexScheduledTask() },
        { "AsyncProcess", new AsyncProcessScheduledTask() },
        { "TransferLogs", new TransferLogsScheduledTask() },
        { "Notifications", new NotificationsScheduledTask() },
        { "AuditCapture", new AuditCaptureScheduledTask() }
    };

    public static IScheduledTask GetScheduleTask(string id)
    {
        if (m_taskList.TryGetValue(id, out var res))
            return res;
        else
            throw new InvalidOperationException($"Uknown scheduled task id {id}");
    }

    public static IEnumerable<string> GetAvailableTasks() => m_taskList.Keys;

    public static IDictionary<string, List<ScheduledTaskOption>> GetTaskOptions() =>
        m_taskList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetOptions());
}
