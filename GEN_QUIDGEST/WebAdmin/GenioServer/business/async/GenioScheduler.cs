using System;
using System.Collections.Generic;
using System.Linq;
using CSGenio.framework;
using CSGenio.persistence;

namespace CSGenio.business.async
{
    using Process = CSGenioAs_apr;

    /// <summary>
    /// Manages the scheduling and execution of Genio work items
    /// </summary>
    public class GenioScheduler
    {
        private readonly JobFinder _jobFinder;
        private readonly List<GenioWork> _works;

        public GenioScheduler(JobFinder jobFinder)
        {
            _jobFinder = jobFinder ?? throw new ArgumentNullException(nameof(jobFinder));
            _works = new List<GenioWork>();
        }

        public GenioWork GetWork(List<Process> processes, PersistentSupport sp, User user)
        {
            if (processes == null) throw new ArgumentNullException(nameof(processes));
            if (sp == null) throw new ArgumentNullException(nameof(sp));
            if (user == null) throw new ArgumentNullException(nameof(user));

            UpdateWorks(processes, sp, user);

            for (int i = 0; i < _works.Count; i++)
            {
                if (!IsWorkItemEligible(_works[i], sp, user))
                    continue;

                if (CanExecuteWorkItem(_works[i], i, sp))
                    return _works[i];
            }

            return null;
        }

        private bool IsWorkItemEligible(GenioWork work, PersistentSupport sp, User user)
        {
            return work.Process.ValRtstatus == ArrayS_prstat.E_FE_2 &&
                   work.FulfillRequirements(sp, user);
        }

        private bool CanExecuteWorkItem(GenioWork work, int currentIndex, PersistentSupport sp)
        {
            for (int j = currentIndex - 1; j >= 0; j--)
            {
                if (Collision(work.Job, _works[j].Job, sp))
                    return false;
            }
            return true;
        }

        public bool Collision(GenioExecutableJob first, GenioExecutableJob second, PersistentSupport sp)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (sp == null) throw new ArgumentNullException(nameof(sp));

            var firstPolicy = first.GetPartitionPolicy(second);
            var secondPolicy = second.GetPartitionPolicy(first);

            var firstUnits = firstPolicy.GetWorkUnits(sp);
            var secondUnits = secondPolicy.GetWorkUnits(sp);

            // Check if any work unit in the first set conflicts with any work unit in the second set
            return firstUnits.Any(workUnit => secondUnits.Any(otherWorkUnit => workUnit.CollidesWith(otherWorkUnit)));
        }

        private void UpdateWorks(List<Process> processes, PersistentSupport sp, User user)
        {
            RemoveFinishedWorks(processes);
            UpdateExistingAndAddNewWorks(processes, sp, user);
            ReorderWorks();
        }

        private void RemoveFinishedWorks(List<Process> processes)
        {
            var finished = _works.Select(x => x.Process.ValCodascpr)
                                .Except(processes.Select(x => x.ValCodascpr));

            if (finished.Any())
                _works.RemoveAll(x => finished.Contains(x.Process.ValCodascpr));
        }

        private void UpdateExistingAndAddNewWorks(List<Process> processes, PersistentSupport sp, User user)
        {
            Log.Debug("Updating existing schedulled processes");
            foreach (var process in processes)
            {
                var existing = _works.Find(x => x.Process.ValCodascpr == process.ValCodascpr);

                if (existing != null)
                {
                    UpdateExistingWork(existing, process);
                }
                else
                {
                    try
                    {
                        using (Log.SetContext("ProcessId", process.ValId))
                        {
                            var newWork = LoadWork(process, sp, user);
                            _works.Add(newWork);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error and continue processing other works
                        Log.Error($"Error loading work for process {process.ValId}: {ex.Message}");                        
                    }
                }
            }
        }

        private void UpdateExistingWork(GenioWork work, Process process)
        {
            if (work.Process.ValExternal == 1 && work.Process.ValRtstatus != process.ValRtstatus)
            {
                work.Process.ValRtstatus = process.ValRtstatus;
            }
        }

        private GenioWork LoadWork(Process process, PersistentSupport sp, User user)
        {
            var job = _jobFinder.ObtainJob(process);
            job.FillArguments(sp, user, process);
            job.SetPartitionPolicies();
            return new GenioWork(process, job);
        }

        private void ReorderWorks()
        {
            var executing = _works.Where(w => SchedulerBroker.IsExecuting(w.Process));
            var notExecuting = _works.Where(w => !SchedulerBroker.IsExecuting(w.Process)).OrderBy(w => w).ToList();

            _works.Clear();
            _works.AddRange(executing.Concat(notExecuting));
        }
    }

    /// <summary>
    /// Exception thrown when a process is invalid
    /// </summary>
    public class InvalidProcessException : BusinessException
    {
        public InvalidProcessException(string message, string localErro, string causaErro)
            : base(message, localErro, causaErro)
        {
        }
    }

    /// <summary>
    /// Represents time units for monitoring purposes
    /// </summary>
    public enum TimeUnit
    {
        Seconds,
        Minutes,
        Hours
    }

    /// <summary>
    /// Provides utility methods for time-based monitoring operations
    /// </summary>
    public static class MonitorUtils
    {
        private static readonly Dictionary<TimeUnit, string> TimeUnitStrings = new Dictionary<TimeUnit, string>
        {
            { TimeUnit.Seconds, "segundos" },
            { TimeUnit.Minutes, "minutos" },
            { TimeUnit.Hours, "horas" }
        };

        private static readonly Dictionary<string, TimeUnit> StringToTimeUnit = new Dictionary<string, TimeUnit>
        {
            { "H", TimeUnit.Hours },
            { "M", TimeUnit.Minutes },
            { "S", TimeUnit.Seconds }
        };

        public static string GetTimeUnitAsString(TimeUnit unit)
        {
            return TimeUnitStrings.TryGetValue(unit, out var result) ? result : string.Empty;
        }

        public static bool CompareTimeDiff(TimeSpan span, double value, TimeUnit unit)
        {
            var spanValue = GetUnitTimeSpan(unit, span);
            return spanValue > value;
        }

        public static double GetUnitTimeSpan(TimeUnit unit, TimeSpan span)
        {
            switch (unit)
            {
                case TimeUnit.Seconds: return span.TotalSeconds;
                case TimeUnit.Minutes: return span.TotalMinutes;
                case TimeUnit.Hours: return span.TotalHours;
                default: return span.TotalMinutes;
            }
        }

        public static TimeUnit GetTimeUnit(string unit)
        {
            return StringToTimeUnit.TryGetValue(unit, out var result) ? result : TimeUnit.Seconds;
        }
    }
}
