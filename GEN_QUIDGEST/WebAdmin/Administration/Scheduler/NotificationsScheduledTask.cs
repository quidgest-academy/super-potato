using CSGenio.business;
using CSGenio.core.scheduler;
using CSGenio.framework;
using CSGenio.persistence;
using DbAdmin;

namespace Administration;

/// <summary>
/// Processes any pending notifications
/// </summary>
public class NotificationsScheduledTask : IScheduledTask
{

    /// <inheritdoc/>
    public List<ScheduledTaskOption> GetOptions() {
        return [
            new ScheduledTaskOption {
                PropertyName = "notificationid",
                DisplayName = "Notification Id",
                Optional = true,
                Description = "Database year."
        },
        ];
    }

    /// <inheritdoc/>
    public Task Process(Dictionary<string, string> options, CancellationToken stoppingToken)
    {
        var notifid = ScheduledTaskExtensions.GetStringOption(options, "notificationid", "");
        var year = Configuration.DefaultYear;

        PersistentSupport sp = null;
        try
        {
            var user = SysConfiguration.CreateWebAdminUser(year);
            sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            sp.openTransaction();

            var notifications = PersistentSupport.getNotifications();

            //if the id is filled then we run just that one notification
            if (notifid != null && notifid != string.Empty)
            {
                if (notifications.ContainsKey(notifid))
                {
                    var viewModel = (Notification)notifications[notifid];
                    viewModel.RunOpen(sp, user);
                }
                else
                {
                    Log.Error("notification id not found: " + notifid);
                }
            }
            //if the id is empty then we run all the notifications
            else
            {
                foreach (Notification notification in notifications.Values)
                    notification.RunOpen(sp, user);
            }

            sp.closeTransaction();
        }
        catch (Exception ex)
        {
            sp?.rollbackTransaction();
            Log.Error($"Error handling WebApi call: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}