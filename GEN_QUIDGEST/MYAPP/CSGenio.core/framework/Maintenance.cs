using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Threading;

namespace CSGenio.framework
{

    public class MaintenanceStatus {
        public MaintenanceStatus() { 
            IsActive = false;
            IsScheduled = false;
            Schedule = DateTime.MinValue;
        }

        public bool IsActive { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime Schedule { get; set; }
    }

    public class Maintenance
    {

        private static MaintenanceStatus Status = new MaintenanceStatus();
        private static DateTime lastPoll = DateTime.MinValue;
        // Semáforo que garante a exclusividade de leitura e escrita sobre o dicionário das sessões
        private static ReaderWriterLockSlim s_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public static MaintenanceStatus Current
        {
            get
            {
                s_lock.EnterReadLock();
                MaintenanceStatus result = Status;
                s_lock.ExitReadLock();

                return result;
            }
        }

        public static void GetMaintenanceStatus(PersistentSupport sp) {

            DateTime now = DateTime.Now;
            double seconds = now.Subtract(lastPoll).TotalSeconds;
            if (seconds > 30) {
                s_lock.EnterWriteLock();
                lastPoll = now;
                try
                {
                    Status.IsActive = false;
                    Status.IsScheduled = false;

                    sp.openConnection();
                    SelectQuery query = new SelectQuery()
                        .Select(Configuration.Program + "cfg", "manutdat")
                        .From(Configuration.Program + "cfg")
                        .OrderBy(Configuration.Program + "cfg", "checkdat", SortOrder.Descending);
                    Status.Schedule = CSGenio.persistence.DBConversion.ToDateTime(sp.ExecuteScalar(query));
                    sp.closeConnection();

                    if (Status.Schedule != null && Status.Schedule != DateTime.MinValue)
                    {
                        if (Status.Schedule <= now) // After the sheduled time 
                            Status.IsActive = true;
                        else // Before the sheduled time 
                            Status.IsScheduled = true;
                    }
                }
                catch (Exception e) {
                    sp.closeConnection();
                    Log.Debug("[GetMaintenanceStatus] Error getting maintenance scheduling: " + e.Message );
                }
                s_lock.ExitWriteLock();
            }
        }

        public static bool DisableMaintenance(PersistentSupport sp)
        {
            bool result = true;
            s_lock.EnterWriteLock();
            try
            {
                Status.IsActive = false;
                Status.IsScheduled = false;

                sp.openConnection();
                sp.executeScalar("UPDATE " + Configuration.Program + "cfg  SET MANUTDAT = NULL ");
                sp.closeConnection();
            }
            catch (Exception e)
            {
                sp.closeConnection();
                result = false;
                Log.Debug("[DisableMaintenance] Error setting maintenance scheduling: " + e.Message);
            }
            s_lock.ExitWriteLock();

            return result;
        }

        public static bool ScheduleMaintenance(PersistentSupport sp, DateTime Schedule)
        {
            bool result = true;
            s_lock.EnterWriteLock();
            try
            {
                Status.IsActive = false;
                Status.IsScheduled = true;

                sp.openConnection();
                string sqlDate = CSGenio.persistence.DBConversion.FromDateTime(Schedule);
                sp.executeScalar("UPDATE " + Configuration.Program + "cfg  SET MANUTDAT = "+ sqlDate);
                sp.closeConnection();
            }
            catch (Exception e)
            {
                sp.closeConnection();
                result = false;
                Log.Debug("[ScheduleMaintenance] Error setting maintenance scheduling: " + Schedule.ToString() + " - "+ e.Message);
            }
            s_lock.ExitWriteLock();

            return result;
        }
    }
}
