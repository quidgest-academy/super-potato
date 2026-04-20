using Cronos;
using System;
using System.Threading.Tasks;

namespace CSGenio.core.scheduler
{
    /// <summary>
    /// Current internal state of a task in the scheduler
    /// </summary>
    public class ScheduleState
    {
        /// <summary>
        /// The job configuration
        /// </summary>
        public SchedulerJobXml Job { get; set; }
        /// <summary>
        /// The parsed Cron schedule
        /// </summary>
        public CronExpression Cron { get; set; }
        /// <summary>
        /// Last time it ran, in case it ran
        /// </summary>
        public DateTime LastRun { get; set; }
        /// <summary>
        /// Next time its planned to run
        /// </summary>
        public DateTime NextRun { get; set; }
        /// <summary>
        /// True if its currently actively running
        /// </summary>
        public Task Running { get; set; }
        /// <summary>
        /// The execution processor for this task
        /// </summary>
        public IScheduledTask Process {get; set;}
    }
}



