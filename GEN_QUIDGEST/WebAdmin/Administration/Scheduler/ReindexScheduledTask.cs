using CSGenio.core.scheduler;
using CSGenio.framework;
using CSGenio.persistence;
using DbAdmin;

namespace Administration;

/// <summary>
/// Processes a reindex command for this database
/// </summary>
public class ReindexScheduledTask : IScheduledTask
{

    /// <inheritdoc/>
    public List<ScheduledTaskOption> GetOptions() {
        return [
            new ScheduledTaskOption {
                PropertyName = "yearapp",
                DisplayName = "Year",
                Optional = true,
                Description = "Database year. Black for the default year."
        },
            new ScheduledTaskOption {
                PropertyName = "timeout",
                DisplayName = "Timeout",
                Optional = true,
                Description = "Timeout for each operation"
        },
            new ScheduledTaskOption {
                PropertyName = "zero",
                DisplayName = "Zero",
                Optional = true,
                Description = "True to ask for a full recalculation and reverification of schema."
        },
            new ScheduledTaskOption {
                PropertyName = "scripts",
                DisplayName = "Scritps",
                Optional = true,
                Description = "Specify only certain scripts to run. Blank for the standard reindex."
        },

        ];
    }

    /// <inheritdoc/>
    public async Task Process(Dictionary<string, string> options, CancellationToken stoppingToken)
    {
        var year = ScheduledTaskExtensions.GetStringOption(options, "yearapp", Configuration.DefaultYear);
        var timeout = ScheduledTaskExtensions.GetIntOption(options, "timeout", 300);
        var zero = ScheduledTaskExtensions.GetBoolOption(options, "zero", false);
        var scripts = ScheduledTaskExtensions.GetCsvOption(options, "scripts");

        var sp = PersistentSupport.getPersistentSupport(year);

        //If maintenance is already running, error out
        Maintenance.GetMaintenanceStatus(sp);
        if (Maintenance.Current.IsActive)
        {
            throw new Exception(Resources.Resources.A_MAINTENANCE_TASK_H03437);
        }
        if (Maintenance.Current.IsScheduled)
        {
            throw new Exception(Resources.Resources.A_MAINTENANCE_TASK_I22024);
        }

        //In case the maintenance fails, return error
        if (!Maintenance.ScheduleMaintenance(sp, DateTime.Now))
        {
            throw new Exception(Resources.Resources.THERE_HAS_BEEN_AN_IN10114);
        }


        DBMaintenance dbMaintenance = new DBMaintenance(AppDomain.CurrentDomain.BaseDirectory);

        //fetch username and pass from the configuration
        var ds = Configuration.ResolveDataSystem(year, Configuration.DbTypes.NORMAL);

        var rdxParam = new ExecuteQueryCore.RdxParamUpgradeSchema()
        {
            Username = ds.LoginDecode(),
            Password = ds.PasswordDecode(),
            Year = year,
            Zero = zero,
            Origin = "Scheduled reindexation"
        };

        //this could be so much simpler if StartReindexation did not start its own thread.
        //Initializations and calculations should be isolated into utility functions, instead of just wrapping the UpgradeScheme function.
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        ExecuteQueryCore.ChangedEventHandler ceh = (object sender, EventArgs e, ExecuteQueryCore.RdxStatus status) => {
            if(status.IsFinished())
                tcs.TrySetResult(true);
        };

        dbMaintenance.StartReindexation(rdxParam, null, scripts, null, ceh, stoppingToken, timeout);

        await tcs.Task;

        Maintenance.DisableMaintenance(sp);
    }
}