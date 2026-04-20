using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSGenio.framework;
using CSGenio.persistence;


namespace CSGenio.business.async
{
    using Process = CSGenioAs_apr;

    public class GenioWorker
    {
        private const int MaxWaitTimeInMilliseconds = 5000;

        private readonly User user;

        public GenioWorker(User user)
        {
            this.user = user;
        }

        public bool Work(CancellationToken cancellationToken = default)
        {
            List<Task> activeTasks = new List<Task>();
            bool didWork = false;

            while (true)
            {
                // Exit loop if cancellation is requested or maintenance is active
                if (cancellationToken.IsCancellationRequested || Maintenance.Current.IsActive)
                    break;

                SchedulerBroker scheduler = SchedulerBroker.GetBroker();
                IGenioWork work = scheduler.GetWork(user);

                if (work != null)
                {
                    didWork = true;

                    var task = Task.Run(() =>
                    {
                        try
                        {
                            work.DoWork(user);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Work item failed: {ex}");
                        }
                    }, cancellationToken);

                    activeTasks.Add(task);
                }

                // Wait if no new work is immediately available
                if (work == null)
                {
                    if (activeTasks.Count == 0)
                        break;

                    try
                    {
                        var timeoutTask = Task.Delay(MaxWaitTimeInMilliseconds, cancellationToken);
                        var allWorkTask = Task.WhenAll(activeTasks);
                        Task.WaitAny(new Task[] { allWorkTask, timeoutTask }, cancellationToken: cancellationToken);

                        // Keep only unfinished tasks
                        activeTasks = activeTasks.Where(t => !t.IsCompleted).ToList();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }

            return didWork;
        }
    }


    public interface IGenioWork
    {
        void DoWork(User user);

        string Owner
        {
            get;
        }
    }

    public class AbortedWorkerException : Exception
    {
        public AbortedWorkerException() : base() { }
        public AbortedWorkerException(string message) : base(message) { }
    }

    public class NotAvailableWorkerException : Exception
    {
        public NotAvailableWorkerException() : base() { }
        public NotAvailableWorkerException(string message) : base(message) { }
    }

    public enum JobPriority { HIGH = 1, NORMAL, LOW }

    /// <summary>
    /// Represents a work unit to be executed by the worker.
    /// Contains both the job and the process
    /// </summary>
    public class GenioWork : IGenioWork, IComparable<GenioWork>
    {
        public GenioWork(Process process, GenioExecutableJob job)
        {
            this.process = process;
            this.job = job;
        }

        private Process process;
        private GenioExecutableJob job;

        public JobPriority Priority { get { return job.Priority; } }

        public void DoWork(User user)
        {
            PersistentSupport workSp = null;
            GenioProcessManager manager = null;
            bool undoProcess = false;
            try
            {
                try
                {
                    manager = GenioProcessManager.PersistProcessManager(user);
                    workSp = PersistentSupport.getPersistentSupport(user.Year, this.Owner);
                    workSp.openTransaction();
                    if (job.GetPermission(workSp, user))
                    {
                        if (job is GenioExternalJob)
                            ExecuteExternalJob(workSp, user);
                        else if (job is GenioServerJob)
                            ExecuteLocalJob(workSp, user, manager);
                        else if (job is GenioServerJobAsync)
                            ExecuteLocalJobAsync(workSp, user, manager);
                    }
                    else
                    {
                        throw new BusinessException(Translations.Get("MSG_NO_PERMISSION_PROCESS", user.Language), "Work.DoWork", "");
                    }

                    workSp.closeTransaction();
                }
                finally
                {
                    manager?.StopProgressUpdater();
                }

            }
            catch (NotAvailableWorkerException e)
            {
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", e.Message, "Worker.DoWork", e.Message));
                workSp?.rollbackTransaction();
                undoProcess = true;
                manager?.UndoProcess(process);
            }
            catch (AbortedWorkerException)
            {
                workSp?.rollbackTransaction();
                manager?.CancelProcess(process);
            }
            catch (BusinessException e)
            {
                workSp?.rollbackTransaction();
                manager?.AbortProcess(process, e.UserMessage ?? e.Message);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", e.Message, "Worker.DoWork", e.Message));
                workSp?.rollbackTransaction();
                manager?.AbortProcess(process);
                throw;
            }
            //We need to ensure that the server job is allways notified, even if there is an exception
            finally
            {
                //Notify the scheduler that we are done. Not strictly required but speeds up the next iteration.
                if (job is GenioServerJob)
                {
                    SchedulerBroker.GetBroker().TerminatedProcess(process);

                    //se foi feito undo ao processo, ou seja, voltou to a fila de espera, então não faz sentido notificar
                    if (!undoProcess)
                        manager?.NotifyProcess(process); //notificar o termino do processo apenas fazemos to server job porque os external são terminados no backoffice
                }
            }
        }

        private void ExecuteExternalJob(PersistentSupport sp, User user)
        {
            //Parameters that are passed as command line arguments
            string parameters = "/auto:99 /codigo=" + process.ValCodascpr + " /ano=" + user.Year;
            using (var proc = ExecuteADM(parameters, sp, user))
            {
                bool flag = false;
                bool exit = false;
                while (!exit)
                {
                    bool exitADM = proc.WaitForExit(3000);
                    flag = ReadFlag(sp, user);
                    exit = flag || exitADM;
                }

                if (!flag)
                    throw new NotAvailableWorkerException($"Worker: Não foi possível executar o processo {process.ValId}. ADM bloqueado");

            }

        }

        private bool ReadFlag(PersistentSupport sp, User user)
        {
            CSGenioAs_apr logp0 = CSGenioAs_apr.search(sp, process.ValCodascpr, user);
            //return logp0.ValExternal != 0;
            return true;
        }

        /// <summary>
        /// Executes a process in the C# server
        /// </summary>
        void ExecuteLocalJob(PersistentSupport sp, User user, GenioProcessManager manager)
        {
            GenioServerJob serverJob = (GenioServerJob)job;
            GenioProcessManager outer = GenioProcessManager.SimpleProcessManager(sp, user);

            manager.SetExecuting(process);
            manager.StartProgressUpdater(process, serverJob.Progress);
            ProcessResponse result = new ProcessResponse(null, process);
            job.SetResponse(result);
            serverJob.Execute(sp, user);
            outer.Finish(process, job.Result);
        }

        void ExecuteLocalJobAsync(PersistentSupport sp, User user, GenioProcessManager manager)
        {
            GenioServerJobAsync serverJob = (GenioServerJobAsync)job;
            GenioProcessManager outer = GenioProcessManager.SimpleProcessManager(sp, user);

            manager.SetExecuting(process);
            manager.StartProgressUpdater(process, serverJob.Progress);
            ProcessResponse result = new ProcessResponse(null, process);
            job.SetResponse(result);
            var t = serverJob.ExecuteAsync(sp, user, process).Result;
            outer.Finish(process, job.Result);
        }

        //private static SecureString GetSecureStringAE5(string encrypted)
        //{
        //    //As secure strings têm de ser preenchidas um caracter de cada vez...
        //    SecureString securePassword = new SecureString();
        //    foreach (Char c in GlobalFunctions.GetPassword(encrypted))
        //    {
        //        securePassword.AppendChar(c);
        //    }
        //    return securePassword;
        //}


        public string Owner
        {
            get { return process.ValOpercria; }
        }

        public Process Process
        {
            get { return process; }
        }

        public GenioExecutableJob Job
        {
            get { return job; }
        }


        /// <summary>
        /// Creates a new ADM process without starting it. Dispose should be called after executing the process.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sp"></param>
        /// <param name="utilizador"></param>
        /// <returns></returns>
        public static System.Diagnostics.Process GetNewADMProcess(string parameters, PersistentSupport sp, User user)
        {
            string path;
            try
            {
                path = Configuration.GetProperty("AdmPath");
            }
            catch
            {
                string message = Translations.Get("MSG_ERROR_NO_ADM_PATH", user.Language);
                throw new BusinessException(message, "Worker.Execute", message);
            }

            if (path.Length == 0 || !File.Exists(path))
            {
                string message = Translations.Get("MSG_ERROR_WRONG_ADM_PATH", user.Language);
                throw new BusinessException(message, "Worker.Execute", message);
            }

            System.Diagnostics.Process sysProcess = new System.Diagnostics.Process();
            sysProcess.StartInfo.FileName = path;
            sysProcess.StartInfo.Arguments = parameters;
            sysProcess.StartInfo.UseShellExecute = false;

            //CSGenioAglob glob = GlobalFunctions.SearchListUnique<CSGenioAglob>(sp, null, user, false);
            //if (glob.ValUserproc.Length != 0)
            //{

            //    sysProcess.StartInfo.UserName = glob.ValUserproc;

            //    SecureString securePassword = GetSecureStringAE5(glob.ValPswproc);
            //    sysProcess.StartInfo.Password = securePassword;
            //    sysProcess.StartInfo.Domain = glob.ValDominio;
            //}

            return sysProcess;
        }

        /// <summary>
        /// Executes ADM and returns a process which should be disposed after used.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sp"></param>
        /// <param name="utilizador"></param>
        /// <returns></returns>
        public static System.Diagnostics.Process ExecuteADM(string parameters, PersistentSupport sp, User user)
        {
            var sysProcess = GetNewADMProcess(parameters, sp, user);

            try
            {
                //Most of the time the process throws an exception instead of returning false.                 
                if (!sysProcess.Start())
                    //Handle not starting as an exception
                    throw new Exception("Process couldn't be started.");

            }
            catch (Exception e)
            {
                string message = Translations.Get("MSG_ERROR_START_ADM", user.Language);
                throw new BusinessException(message, "Worker.ExecuteExternalJob", e.Message);
            }

            return sysProcess;
        }

        public bool FulfillRequirements(PersistentSupport sp, User user)
        {
            return Job.AreRequirementsMet(sp, user);
        }

        public int CompareTo(GenioWork other)
        {
            if (other == null)
                return 1; // Treat null as lowest value

            // Compare by Priority first
            int priorityComparison = Priority.CompareTo(other.Priority);
            if (priorityComparison != 0)
                return priorityComparison;

            // If Priority is equal, compare by Process.ValId
            return Process.ValId.CompareTo(other.Process.ValId);
        }
    }
}
