using CSGenio.core.scheduler;
using CSGenio.framework;

namespace Administration;

/// <summary>
/// Hosts the scheduler service within the AspNet Core services dependency injection.
/// Links the configuration and processor registry to the scheduler, so it can remain agnostic to these details.
/// </summary>
public class SchedulerServiceHost : BackgroundService
{

    private readonly SchedulerService _service = new SchedulerService();
    
    private bool _wasEnabled = false;
    private CancellationTokenSource _childCts = null;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {

        try
        {
            //we create a child cancelation so we can stop the service without exiting the host
            _childCts?.Dispose();
            _childCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                //memorize the last enabled state to avoid unnecessary restarts during no changes
                _wasEnabled = Configuration.Scheduler.Enabled;

                //if the service is not enabled we need to wait for either someone to enable it, or for the cancel                
                if(!_wasEnabled)
                {
                    await Task.Delay(5000, _childCts.Token).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
                }
                //otherwise just run the service and look out for disable message
                else
                {
                    UpdateJobs();
                    await _service.Run(_childCts.Token);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            //if(OperatingSystem.IsWindows())
            //    Environment.Exit(1);
        }
        finally
        {
            _childCts?.Dispose();
        }
    }

    /// <summary>
    /// Communicates to the service a new configuration of jobs
    /// </summary>
    public void UpdateJobs() 
    {
        var jobs = Configuration.Scheduler.Jobs;
        _service.UpdateJobs(jobs, ScheduleTaskFactory.GetScheduleTask);
    }

    /// <summary>
    /// Communicates to the service a enable/disable change of the service
    /// </summary>
    public void UpdateEnable() 
    {
        //Cancel the task if the enabled state has changed
        if(Configuration.Scheduler.Enabled != _wasEnabled)
            _childCts?.Cancel();
    }

}
