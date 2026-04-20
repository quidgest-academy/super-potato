using System;
using System.Collections.Generic;
using System.Linq;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business.async
{
    using Process = CSGenioAs_apr;

    /// <summary>
    /// Singleton class responsible for distributing jobs to workers.
    /// Be careful, every public method must deal with possible concurrency.
    /// </summary>
    public class SchedulerBroker
    {
        private static SchedulerBroker instance = null;
        private readonly object lockProcess = new object();

        private readonly Dictionary<string, List<Process>> allProcess = new Dictionary<string, List<Process>>();
        private readonly Dictionary<string, DateTime> lastCheck;
        private List<Process> processes;

        private readonly GenioScheduler scheduler;

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private SchedulerBroker() : this(null) { }

        /// <summary>
        /// Initializes the scheduler broker with a job finder.
        /// </summary>
        private SchedulerBroker(JobFinder jobFinder)
        {
            scheduler = jobFinder != null ? new GenioScheduler(jobFinder) : null;
            lastCheck = new Dictionary<string, DateTime>();
            processes = new List<Process>();
        }

        /// <summary>
        /// Retrieves the singleton instance of the broker.
        /// </summary>
        public static SchedulerBroker GetBroker()
        {
            lock (typeof(SchedulerBroker))
            {
                if (instance == null)
                {
                    instance = new SchedulerBroker(new ReflectionJobFinder());
                }
                return instance;
            }
        }

        /// <summary>
        /// Configures the broker with a specific job finder.
        /// </summary>
        public static void SetupBroker(JobFinder jobFinder)
        {
            instance = new SchedulerBroker(jobFinder);
        }

        /// <summary>
        /// Finds and returns the next executable job for a given user.
        /// Returns null if no work is available.
        /// </summary>
        public IGenioWork GetWork(User user)
        {
            PersistentSupport sp = null;

            try
            {
                sp = PersistentSupport.getPersistentSupport(user.Year);
                sp.openTransaction();

                lock (lockProcess)
                {
                    processes = GetProcess(sp, user);
                    KillUnresponsive(user);

                    if (!CanWork())
                        return null;

                    GenioWork mostUrgent = scheduler.GetWork(processes, sp, user);
                    if (mostUrgent == null)
                        return null;

                    var manager = GenioProcessManager.PersistProcessManager(user);
                    return manager.AllocateProcess(mostUrgent.Process) ? mostUrgent : null;
                }
            }
            catch (Exception ex) when (ex is InvalidProcessException || ex is BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                sp?.rollbackTransaction();
                string message = Translations.Get("MSG_ERROR_OBTAIN_NEXT_PROCESS", user.Language);
                throw new BusinessException(message, "Scheduler.GetWork", ex.Message);
            }
            finally
            {
                sp?.closeTransaction();
            }
        }

        /// <summary>
        /// Terminates a process and removes it from active tracking.
        /// </summary>
        public void TerminatedProcess(Process processo)
        {
            lock (lockProcess)
            {
                processes.RemoveAll(x => x.ValCodascpr == processo.ValCodascpr);
            }
        }

        /// <summary>
        /// Identifies and aborts unresponsive processes.
        /// </summary>
        private void KillUnresponsive(User user)
        {
            var unit = MonitorUtils.GetTimeUnit("S");
            var manager = GenioProcessManager.PersistProcessManager(user);
            double timeout = Convert.ToDouble(Configuration.GetProperty("inactivitytime", "120"));

            foreach (var proc in processes.Where(NotResponding))
            {
                var elapsedTime = DateTime.Now - proc.ValLastupdt;
                if (!MonitorUtils.CompareTimeDiff(elapsedTime, timeout, unit)) continue;

                string msg = string.Format(
                    Translations.Get("MSG_ABORT_PROCESS_AUTO", user.Language),
                    (int)MonitorUtils.GetUnitTimeSpan(unit, elapsedTime),
                    MonitorUtils.GetTimeUnitAsString(unit));

                manager.AbortProcess(proc, msg);
                manager.NotifyProcess(proc);
            }
        }

        /// <summary>
        /// Determines if additional processes can be executed based on concurrency rules.
        /// </summary>
        private bool CanWork()
        {
            string concurrencyType = Configuration.ExistsProperty("concurrencytype")
                ? Configuration.GetProperty("concurrencytype")
                : null;

            switch (concurrencyType)
            {
                case "L":
                    int maxProcesses = Configuration.ExistsProperty("maxprocess")
                        ? Conversion.string2Int(Configuration.GetProperty("maxprocess"))
                        : 1;
                    return processes.Count(IsExecuting) < maxProcesses;
                case "I":
                    return true;
                default:
                    return !processes.Exists(IsExecuting);
            }
        }

        /// <summary>
        /// Retrieves the list of available processes for execution.
        /// </summary>
        private List<Process> GetProcess(PersistentSupport sp, User user)
        {
            TimeSpan checkInterval = TimeSpan.FromMilliseconds(500);

            if (!lastCheck.ContainsKey(user.Year) || DateTime.Now - lastCheck[user.Year] > checkInterval)
            {
                var results = Process.searchList(sp, user,
                    CriteriaSet.And()
                        .Equal(Process.FldFinished, 0)
                        .Equal(Process.FldZzstate, 0)
                        .SubSet(CriteriaSet.NotAnd()
                            .Equal(Process.FldRtstatus, ArrayS_prstat.E_AC_8)
                            .Equal(Process.FldRtstatus, ArrayS_prstat.E_NR_6))
                );

                lastCheck[user.Year] = DateTime.Now;
                allProcess[user.Year] = results;
                return results;
            }

            return allProcess[user.Year];
        }

        /// <summary>
        /// Checks if a process is currently executing.
        /// </summary>
        public static bool IsExecuting(Process processo) =>
            processo.ValRtstatus == ArrayS_prstat.E_AG_3 ||
            processo.ValRtstatus == ArrayS_prstat.E_EE_1 ||
            processo.ValRtstatus == ArrayS_prstat.E_AC_8;

        /// <summary>
        /// Checks if a process is unresponsive.
        /// </summary>
        private static bool NotResponding(Process processo) =>
            processo.ValRtstatus == ArrayS_prstat.E_NR_6;
    }
}
