using Cronos;
using CSGenio.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSGenio.core.scheduler
{
    /// <summary>
    /// Scheduler service implementation.
    /// To keep it environment independent it requires an external host to instance it and manage it.
    /// Runs in a loop until its canceled by the host.
    /// Allows for job metadata updates while running.
    /// </summary>
    public class SchedulerService
    {
        private List<ScheduleState> m_states = new List<ScheduleState>();

        private ManualResetEventSlim m_taskEndedEvent = new ManualResetEventSlim(false);

        private List<SchedulerJobXml> m_jobsUpdate = null;
        private readonly object m_jobsUpdateLock = new object();

        private Func<string, IScheduledTask> m_processFactory = null;

        /// <summary>
        /// Notifies the scheduler to update the running job schedules
        /// </summary>
        /// <param name="config">The job configuration to follow</param>
        /// <param name="processFactory">A function that converts any task name into a IScheduleTask that can be called</param>
        public void UpdateJobs(List<SchedulerJobXml> config, Func<string, IScheduledTask> processFactory)
        {
            if(config == null) throw new ArgumentNullException(nameof(config));
            if(processFactory == null) throw new ArgumentNullException(nameof(processFactory));

            lock(m_jobsUpdateLock)
            {
                m_processFactory = processFactory;
                //if this reference becomes problematic then clone it
                m_jobsUpdate = config;
            }
        }

        private void CheckForUpdates()
        {
            lock(m_jobsUpdateLock)
            {
                if(m_jobsUpdate == null)
                    return;

                //make sure no task is running
                if(m_states.Any(x => x.Running != null))
                    return;

                //create a new list to avoid concurrency problems
                List<ScheduleState> res = new List<ScheduleState>();
                
                foreach(var job in m_jobsUpdate)
                    try 
                    {
                        if(!job.Enabled)
                            continue;

                        var previousJob = m_states.Find(x => x.Job.Id == job.Id);
                        DateTime last = previousJob == null ? DateTime.UtcNow : previousJob.LastRun;

                        var schedule = CronExpression.Parse(job.Cron, CronFormat.IncludeSeconds);
                        DateTime next = schedule.GetNextOccurrence(last, TimeZoneInfo.Local) ?? throw new Exception("Schedule " + "x" + " has no future run dates.");

                        res.Add(new ScheduleState {
                            Job = job,
                            Cron = schedule,
                            LastRun = last,
                            NextRun = next,
                            Running = null,
                            Process = m_processFactory(job.TaskType)
                        });
                    }
                    catch(Exception ex)
                    {
                        Log.Error(ex.Message);
                    }

                m_states = res;
                m_jobsUpdate = null;
            }
        }

        /// <summary>
        /// Begins the scheduler loop and checking for tasks in condition to run
        /// </summary>
        /// <param name="stoppingToken">A cancelation token that will signal the host want this process to stop</param>
        /// <returns>The asyncronous task of the scheduler loop</returns>
        public async Task Run(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    //check for state updates
                    CheckForUpdates();

                    //base time for this iteration
                    var now = DateTime.UtcNow;
                    //max iteration time
                    var minWait = TimeSpan.FromMinutes(1);

                    //don't run scheduled tasks during maintenance state
                    if(!Maintenance.Current.IsActive)
                        //See if any of the active tasks is in conditions to be launched
                        foreach(var taskState in m_states)
                        {
                            //only run each job once at a time
                            if(taskState.Running != null)
                                continue;

                            //if its time to run this task we launch it
                            if(taskState.NextRun <= now)
                            {
                                RunTask(taskState, stoppingToken);
                            }
                            //otherwise we keep the next lowest time to calculate the thread sleep
                            else
                            {
                                var wait  = taskState.NextRun - now;
                                if(wait < minWait)
                                    minWait = wait;
                            }
                            //if there are no tasks we will just wait the 1 minute
                            //to either timeout or be signaled off by a terminating task
                        }

                    if(minWait > TimeSpan.Zero)
                    {
                        //This needs to run on another thread, we cannot block this thread on a loop
                        // or the website will never get a chance to even run.
                        //Awaiting something async on this infinite loop is *mandatory*!
                        await Task.Run( () => {
                            try { 
                                m_taskEndedEvent.Wait(minWait, stoppingToken); 
                            }
                            catch(OperationCanceledException) { 
                                //ignore this exception
                            }
                            m_taskEndedEvent.Reset();
                        }, stoppingToken).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }

            //give the opportunity for any currently running tasks to finish
            await Task.WhenAll(m_states.Select(x => x.Running).Where(x => x != null));
        }

        private void RunTask(ScheduleState state, CancellationToken stoppingToken)
        {
            //launch the background task
            state.Running = Task.Run( async () => {
                //run the process
                if(Log.IsDebugEnabled) Log.Debug($"Job {state.Job.Id} started.");

                if(state.Process != null)
                    try 
                    {
                        using (var _ = CSGenio.framework.Log.SetContext("ActionName", "SchedulerService." + state.Job.Id))
                        {
                            await state.Process.Process(state.Job.Options ?? new SerializableDictionary<string, string>(), stoppingToken);
                        }
                    }
                    catch(Exception ex)
                    {
                        Log.Error(ex.Message);
                    }

                //when the run ends then mark the task as stopped
                var now = DateTime.UtcNow;
                state.Running = null;
                state.LastRun = now;

                //and calculate the next time it should run            
                state.NextRun = state.Cron.GetNextOccurrence(now, TimeZoneInfo.Local) ?? DateTime.MaxValue;
                if(Log.IsDebugEnabled) Log.Debug($"Job {state.Job.Id} finished at {now} next run at {state.NextRun}");

                //let the scheduler know the states have been updated
                m_taskEndedEvent.Set();
            }, stoppingToken);
        }
    }
}



