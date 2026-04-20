using System;
using System.Collections.Generic;
using System.IO;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace CSGenio.business.async
{
    using ArgumentsMap = Dictionary<String, AsyncProcessArgument>;
    using Process = CSGenioAs_apr;
    
    /// <summary>
    /// Created by JGF at 2017.04.17
    /// Class responsible for managing the processes. All changes to a process should be made through this class.
    /// This class is the base class of Decorator pattern.
    /// </summary>
    public abstract class GenioProcessManager
    {
        protected PersistentSupport sp;
        protected User user;
        private GenioProgressUpdater updater;

        public abstract void NotifyProcess(Process process);

        public abstract void ScheduleProcessNotification(Process process);

        public abstract void CancelProcess(CSGenioAs_apr process);

        public abstract void UndoProcess(CSGenioAs_apr process);

        public abstract void DeleteProcess(string codlogpr);

        public abstract void AbortProcess(Process process, string motivo = "");

        public abstract void SetExecuting(Process process);

        public abstract void Finish(Process process, Response result);

        public abstract bool AllocateProcess(Process process);

        public abstract string Agenda(ProcessScheduler agendamento, string userid, string codpsw, bool repeat);

        private bool EqualArguments(ArgumentsMap schedulerArgs, List<CSGenioAs_arg> processArgs)
        {
            int nArgs = schedulerArgs.Values.Sum(x => x.Value.Count);
            bool schedulerContainsDocuments = schedulerArgs.Any(a => a.Value.Docum);
            //compare arguments
            //same number of arguments and same information
            //if there are documents in argument list then we cannot compare because we do not have access to the content
            if (nArgs == processArgs.Count() && !schedulerContainsDocuments)
            {
                List<CSGenioAs_arg> pArgsToProcess = new List<CSGenioAs_arg>(processArgs);
                foreach (var arg in schedulerArgs)
                {
                    AsyncProcessArgument argument = arg.Value;
                    string id = arg.Key;
                    foreach (var value in argument.Value)
                    {
                        var pArg = pArgsToProcess.Find(a => a.ValId.Equals(id, StringComparison.InvariantCultureIgnoreCase) && a.ValValor.Equals(value, StringComparison.InvariantCultureIgnoreCase));
                        if (pArg == null)
                            return false;
                        else
                            pArgsToProcess.Remove(pArg);
                    }
                }
                //if there are no more arguments to process then we can confirm that they are equal to the scheduled arguments
                if (pArgsToProcess.Count == 0)
                    return true;
            }

            return false;
        }


        protected bool EqualProcessInQueue(string type, string mode, ArgumentsMap arguments)
        {
            SelectQuery qProc = new SelectQuery()
                    .Select(CSGenioAs_apr.FldCodascpr)
                    .From(Area.AreaS_APR)
                    .Where(CriteriaSet.And()
                        .Equal(CSGenioAs_apr.FldType, type)
                        .Equal(CSGenioAs_apr.FldModoproc, mode)
                        .Equal(CSGenioAs_apr.FldStatus, ArrayS_prstat.E_FE_2));

            DataMatrix equalProcesses = sp.Execute(qProc);
            if (equalProcesses.NumRows > 0)
            {
                //compare arguments only if exists similar processes in queue
                //get all the args of all similiar processes (avoid multi interactions with database) and group args by process
                var allArgs = CSGenioAs_arg.searchList(sp, user, CriteriaSet.And().In(CSGenioAs_arg.FldCods_apr, qProc));
                //Check process by process in memory and evaluate the arguments of each process
                for (int row = 0; row < equalProcesses.NumRows; row++)
                {
                    string codlogpr = equalProcesses.GetKey(row, CSGenioAs_apr.FldCodascpr);
                    var pArgs = allArgs.Where(a => a.ValCods_apr.Equals(codlogpr, StringComparison.InvariantCultureIgnoreCase));
                    if (EqualArguments(arguments, pArgs.ToList()))
                        return true;
                }
            }

            return false;
        }



        /// <summary>
        /// Starts a thread that updates the process with changes to the status;
        /// </summary>
        /// <param name="process"></param>
        public void StartProgressUpdater(Process process, ProgressStatus status)
        {
            if (updater != null)
                throw new InvalidOperationException("Progress manager does not  support multiple progress update definitions.");

            updater = new GenioProgressUpdater(process, status, user);
            updater.Start();
        }

        public void StopProgressUpdater()
        {
            updater?.Stop();
        }

        /// <summary>
        /// Returns the most basic, undecorated process manager.
        /// </summary>
        public static GenioProcessManager SimpleProcessManager(PersistentSupport sp, User user)
        {
            return GenioConcreteProcessManager.SimpleProcessManager(sp, user);
        }

        /// <summary>
        /// Returns a process manager that immediately commits to the database every change it makes.
        /// </summary>
        public static GenioProcessManager PersistProcessManager(User user)
        {
            return GenioProcessManagerPersistor.PersistProcessManager(user);
        }
    }

    /// <summary>
    /// Created by JGF at 2017.04.17
    /// Decorator class implementing an immediately persisting process manager. All changes made by the process manager are immediately commited to the database.
    /// </summary>
    class GenioProcessManagerPersistor : GenioProcessManager
    {
        private GenioProcessManager decorated;

        private GenioProcessManagerPersistor(GenioProcessManager toDecorate, PersistentSupport sp, User user)
        {
            decorated = toDecorate;
            this.sp = sp;
            this.user = user;

        }

        public new static GenioProcessManager PersistProcessManager(User user)
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            GenioProcessManager simple = SimpleProcessManager(sp, user);
            return new GenioProcessManagerPersistor(simple, sp, user);
        }


        public override void CancelProcess(CSGenioAs_apr process)
        {
            try
            {
                sp.openTransaction();
                decorated.CancelProcess(process);
                sp.closeTransaction();
            }
            catch (Exception e)
            {                
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error canceling process {process.ValId}", "ProcessManagerPersistor.CancelProcess", e.Message));
                sp.rollbackTransaction();
                throw;
            }
        }

        public override void UndoProcess(CSGenioAs_apr process)
        {
            try
            {
                sp.openTransaction();
                decorated.UndoProcess(process);
                sp.closeTransaction();
            }
            catch (Exception e)
            {                
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error undoing process {process.ValId}", "ProcessManagerPersistor.UndoProcess", e.Message));
                sp.rollbackTransaction();
                throw;
            }
        }

        public override void AbortProcess(Process process, string motivo)
        {
            try
            {
                sp.openTransaction();
                decorated.AbortProcess(process, motivo);
                sp.closeTransaction();
            }
            catch (Exception e)
            {                
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error aborting process {process.ValId}", "ProcessManagerPersistor.AbortProcess", e.Message));
                sp.rollbackTransaction();
                throw;
            }
        }

        public override void DeleteProcess(string codlogpr)
        {
            try
            {
                sp.openTransaction();
                decorated.DeleteProcess(codlogpr);
                sp.closeTransaction();
            }
            catch (Exception e)
            {                
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error deleting process with code {codlogpr}", "ProcessManagerPersistor.DeleteProcess", e.Message));
                sp.rollbackTransaction();
                throw;
            }
        }

        public override void SetExecuting(Process process)
        {
            try
            {
                sp.openTransaction();
                decorated.SetExecuting(process);
                sp.closeTransaction();
            }
            catch (Exception e)
            {                
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error setting process {process.ValId} to execute", "ProcessManagerPersistor.SetExecuting", e.Message));
                sp.rollbackTransaction();
                throw;
            }
        }

        public override void Finish(Process process, Response result)
        {
            try
            {
                sp.openTransaction();
                decorated.Finish(process, result);
                sp.closeTransaction();
            }
            catch (Exception e)
            {                
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error finishing process {process.ValId}", "ProcessManagerPersistor.Finish", e.Message));
                sp.rollbackTransaction();
                throw;
            }
        }

        public override string Agenda(ProcessScheduler agendamento, string userid, string codpsw, bool repeat)
        {
            try
            {
                sp.openTransaction();
                string jobId = decorated.Agenda(agendamento, userid, codpsw, repeat);
                sp.closeTransaction();
                return jobId;
            }
            catch (Exception e)
            {                
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error scheduling process {agendamento.getProcessType()}", "ProcessManagerPersistor.Agenda", e.Message));
                sp.rollbackTransaction();
                throw;
            }
        }

        public override bool AllocateProcess(Process process)
        {
            bool result = false;
            try
            {
                sp.openTransaction();
                result = decorated.AllocateProcess(process);
                sp.closeTransaction();
            }
            catch (Exception e)
            {                
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error allocating process {process.ValId}", "ProcessManagerPersistor.AllocateProcess", e.Message));
                sp.rollbackTransaction();
                throw;
            }

            return result;
        }

        public override void NotifyProcess(Process process)
        {
            try
            {
                sp.openTransaction();
                decorated.NotifyProcess(process);
                sp.closeTransaction();
            }
            catch
            {
                sp.rollbackTransaction();
                //neste caso o erro já é registado no level mais abaixo fazendo throw to aqui to ser possivel fazer rollback
                //não é relevante enviar o erro to cima visto que não é significativo abortar devido a esta opção
            }
        }

        public override void ScheduleProcessNotification(Process process)
        {
            try
            {
                sp.openTransaction();
                decorated.ScheduleProcessNotification(process);
                sp.closeTransaction();
            }
            catch
            {
                sp.rollbackTransaction();
                //neste caso o erro já é registado no level mais abaixo fazendo throw to aqui to ser possivel fazer rollback
                //não é relevante enviar o erro to cima visto que não é significativo abortar devido a esta opção
            }
        }
    }



    /// <summary>
    /// Class that actually implements the ProcessManager base functionality
    /// </summary>
    class GenioConcreteProcessManager : GenioProcessManager
    {

        public new static GenioProcessManager SimpleProcessManager(PersistentSupport sp, User user)
        {
            return new GenioConcreteProcessManager(sp, user);
        }


        private GenioConcreteProcessManager(PersistentSupport sp, User utilizador)
        {
            user = utilizador;
            this.sp = sp;
        }


        public override void CancelProcess(CSGenioAs_apr process)
        {
            CancelProcess(process, sp, user);
        }
        public override void UndoProcess(CSGenioAs_apr process)
        {
            UndoProcess(process, sp, user);
        }

        public override void DeleteProcess(string codlogpr)
        {
            DeleteProcess(codlogpr, sp, user);
        }

        public static void CancelProcess(CSGenioAs_apr process, PersistentSupport sp, User user)
        {
            lock (process)
            {
                if (process.ValStatus == ArrayS_prstat.E_AC_8)
                {
                    process.ValStatus = ArrayS_prstat.E_C_5;
                    process.ValMotivo = $"Cancelado pelo utilizador {process.ValOpershut}";
                }
                process.ValLastupdt = DateTime.Now;
                process.update(sp);
            }
        }

        public static void UndoProcess(CSGenioAs_apr process, PersistentSupport sp, User user)
        {
            lock (process)
            {
                ChangeState(process, ArrayS_prstat.E_FE_2);
                process.ValInitprc = DateTime.MinValue;
                process.update(sp);
            }
        }

        private static void DeleteProcess(string codlogpr, PersistentSupport sp, User user)
        {
            CSGenioAs_apr processo = CSGenioAs_apr.search(sp, codlogpr, user);
            if (processo.ValFinished == 1 || processo.ValRtstatus == ArrayS_prstat.E_NR_6)
            {
                processo.delete(sp);
            }
            else
            {
                string raw_message = Translations.Get("MSG_ERROR_CANNOT_DELETE_PROCESS", user.Language);
                string msg = raw_message.Replace("@estado", ArrayS_prstat.CodToDescricao(processo.ValRtstatus));
                throw new BusinessException(msg, "DeleteProcess", "O processo estava no estado: " + processo.ValStatus);
            }
        }

        public override string Agenda(ProcessScheduler agendamento, string userid, string codpsw, bool repeat)
        {
            try
            {
                //Agendar os processos, escrevendo os argumentos
                var arguments = agendamento.GetArgumentsValues();
                return ScheduleProcess(agendamento.getProcessType(), agendamento.getMode(), userid, codpsw, arguments);
            }
            catch (Exception e)
            {                
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error scheduling process {agendamento.getProcessType()}", "ProcessManagerPersistor.Agenda", e.Message));
                throw;
            }
        }

        public string ScheduleProcess(string tipo, string mode, string userid, string codpsw, ArgumentsMap arguments)
        {
            Process proc = new Process(user)
            {
                ValType = tipo,
                ValModoproc = mode,
                ValCodentit = userid,
                ValCodpsw = codpsw,
                ValStatus = ArrayS_prstat.E_FE_2,
                ValLastupdt = DateTime.Now
            };
            proc.insert(sp);

            foreach (var pair in arguments)
            {
                AsyncProcessArgument argument = pair.Value;
                string nomeArg = GetHumanName(pair.Key, argument);
                string key = GetKey(pair.Key, argument);

                Dictionary<String, String> descriptions = null;
                if (argument.Field != null && !argument.Docum)
                    descriptions = GetDescriptions(argument, key);

                foreach (var value in argument.Value)
                {
                    CSGenioAs_arg pArg = new CSGenioAs_arg(user);
                    pArg.ValCods_apr = proc.ValCodascpr;
                    pArg.ValId = pair.Key;
                    pArg.ValValor = value;
                    pArg.ValTipo = nomeArg;

                    if (argument.Field != null && descriptions != null)
                    {
                        if (descriptions.ContainsKey(value))
                            pArg.ValDesignac = descriptions[value];
                        else
                            argument.Hide = true;
                    }
                    else if (argument.Docum)
                    {
                        //quando é documento a designação é o name do documento
                        string file = value;
                        int idxContext = value.IndexOf("pathdocu");
                        if (idxContext != -1)
                            file = value.Substring(0, idxContext - 2) + Path.GetExtension(value);
                        pArg.ValDesignac = file;
                    }
                    else
                        pArg.ValDesignac = pArg.ValValor;

                    if (argument.Array != null)
                        pArg.ValDesignac = GetArrayDescription(argument.Array, pArg.ValDesignac);
                    if (argument.Function != null)
                        pArg.ValDesignac = (string)argument.Function.Invoke(null, new object[] { sp, user, pArg.ValDesignac });
                    if (argument.Hide)
                        pArg.ValHidden = 1;
                    pArg.insert(sp);
                }
            }

            return proc.QPrimaryKey;
        }

        private string GetHumanName(string key, AsyncProcessArgument argument)
        {
            if (argument.Name != null)
                return argument.Name;
            else
                return key;
        }

        private string GetKey(string key, AsyncProcessArgument argument)
        {
            if (argument.KeyName != null)
                return argument.KeyName;
            else
                return key;
        }

        private Dictionary<string, string> GetDescriptions(AsyncProcessArgument argument, string key)
        {
            Dictionary<string, string> descriptions = new Dictionary<string, string>();
            //apenas podemos considerar os argumentos que não sejam vazio
            //temos o problema dos vazio com as GUIDs
            //TODO: Cada classe deveria implementar este tratamento podendo assim gerar autonomamente as excepções
            var values = argument.Value.Where(x => !string.IsNullOrEmpty(x));
            if (values.Count() > 0)
            {
                FieldRef field = argument.Field;
                FieldRef chave = new FieldRef(field.Area, key);
                SelectQuery query = new SelectQuery();
                query.Select(chave).Select(field)
                   .From(Area.GetInfoArea(field.Area).TableName, field.Area)
                   .Where(CriteriaSet.And().In(chave, values));
                try
                {
                    var matrix = sp.Execute(query);
                    for (int i = 0; i < matrix.NumRows; i++)
                    {
                        string valKey = matrix.GetKey(i, 0);
                        string valField = matrix.GetString(i, 1);
                        descriptions[valKey] = valField;
                    }
                }
                catch
                {
                    Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", "Erro ao obter o valor do campo do argumento " + argument.Name, "Argument.GetDescriptions", "Erro ao obter o valor do campo do argumento " + argument.Name));                    
                }
            }
            return descriptions;
        }

        private string GetArrayDescription(ArrayInfo info, string cod)
        {
            return info.GetDescription(cod, user.Language);
        }

        public override void AbortProcess(Process process, string motivo = "")
        {
            lock (process)
            {
                if (motivo == "")
                    process.ValMotivo = "Ocorreu um erro na execução do processo";
                //process.ValMotivo = GlobalFunctions.Message(UserMsg.DEFAULT_ABORT_MSG, user.Language);
                else
                    process.ValMotivo = motivo;

                ChangeState(process, ArrayS_prstat.E_AB_7);
                process.ValOpershut = "WebAdmin";
                process.update(sp);
            }
        }

        public override void SetExecuting(Process process)
        {
            lock (process)
            {
                ChangeState(process, ArrayS_prstat.E_EE_1);
                process.ValInitprc = DateTime.Now;
                process.update(sp);
            }
        }

        /// <summary>
        /// Terminates the process, writing the result and attaching the corresponding files.
        /// </summary>
        public override void Finish(Process process, Response result)
        {
            //if exists a large amount of data to attach to the process, then this could take time to execute
            //and that will delay the progress and eventually the process will not respond
            //so first we will attach de results to the process, for this we do not need to lock the process
            //and only after we will update the process status and finish the process
            AttachResult(process, result);
            lock (process)
            {
                ChangeState(process, ArrayS_prstat.E_T_4);
                process.ValEndprc = DateTime.Now;
                process.ValRsltmsg = result.Status.Message;

                if (result.Status.IsError())
                    process.ValResult = ArrayS_resul.E_ER_2;
                else if (result.Status.IsWarning())
                    process.ValResult = ArrayS_resul.E_WA_3;
                else if (result.Status.IsOk())
                    process.ValResult = ArrayS_resul.E_OK_1;
                else
                    process.ValResult = result.Status.Status.ToString();

                if (process.ValStatus == ArrayS_prstat.E_C_5)
                    process.ValMotivo = $"Cancelado pelo utilizador {process.ValOpershut}";

                process.update(sp);
            }
        }


        private void AttachResult(Process process, Response result)
        {
            //foreach (RelatMsg relatorio in result.RelatoriosErro)
            //{
            //    CSGenioArelhe relBd = relatorio.CreateData(user);
            //    relBd.ValCodlogpr = process.ValCodlogpr;
            //    relBd.update(sp);
            //}

            foreach (Resource resource in result.QResources)
            {
                CSGenioAs_pax anxpr = new CSGenioAs_pax(user);
                anxpr.ValCods_apr = process.ValCodascpr;
                anxpr.insert(sp);

                byte[] data = resource.GetContent(sp);
                anxpr.commitDocum(sp, CSGenioAs_pax.FldDocument.Field, data, resource.Name + "_", "");

                ResourceFile r = resource as ResourceFile;
                if (r != null)
                {
                    try
                    {
                        if (File.Exists(r.FilePath))
                            File.Delete(r.FilePath);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Aloca um processo como estando a ser executado
        /// </summary>
        /// <returns>true se conseguiu alocar com sucesso</returns>
        public override bool AllocateProcess(Process process)
        {
            //Colocar o processo como agendado to execução na BD
            Process validationProcess = Process.search(sp, process.ValCodascpr, user);

            if (validationProcess.ValStatus == ArrayS_prstat.E_FE_2)
            {
                process.ValStatus = ArrayS_prstat.E_AG_3;
                process.ValLastupdt = DateTime.Now;
                process.update(sp);
            }
            else
            {
                //Could not allocate the process
                process.ValStatus = validationProcess.ValStatus;
                return false;
            }
            return true;
        }


        private static void ChangeState(Process process, string status)
        {
            process.ValStatus = status;
            process.ValLastupdt = DateTime.Now;
        }

        public override void NotifyProcess(Process process)
        {
            // We only notify when there process was scheduled by someone in the system.
            //if (GlobalFunctions.emptyG(process.ValCodpesoa) == 1)
            //    return;
            //try
            //{
            //    JobFinder finder = new JobFinder();
            //    ExecutableJob job = finder.ObtainJob(process);
            //    if (job.MustNotify())
            //    {
            //        NotificacaoProcesso notificacao = new NotificacaoProcesso(process.ValCodlogpr);
            //        notificacao.Execute(sp, user);
            //    }
            //}
            //catch (Exception e)
            //{
            //    string erro = $"Não foi possível executar a notificação do processo com o ID - {process.ValId}";
            //    GlobalFunctions.RegistarErro(erro, "ConcreteProcessManager.NotifyProcess", e.Message);
            //    throw;
            //}
        }
        public override void ScheduleProcessNotification(Process process)
        {
        //    // We only notify when there process was scheduled by someone in the system.
        //    if (GlobalFunctions.emptyG(process.ValCodpesoa) == 1)
        //        return;
        //    try
        //    {
        //        JobFinder finder = new JobFinder();
        //        ExecutableJob job = finder.ObtainJob(process);
        //        if (job.MustNotify())
        //        {
        //            NotificacaoProcesso notificacao = new NotificacaoProcesso(process.ValCodlogpr);
        //            notificacao.AgendarAuto(sp, user, true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string erro = $"Não foi possível agendar a notificação do processo com o ID - {process.ValId}";
        //        GlobalFunctions.RegistarErro(erro, "ConcreteProcessManager.ScheduleProcessNotification", ex.Message);
        //        throw;
        //    }
        }
    }

    class GenioProgressUpdater
    {
        private Process process;
        private ProgressStatus status;
        private PersistentSupport sp;
        private User user;
        private Task updateTask;
        private CancellationTokenSource cancellationTokenSource;


        public GenioProgressUpdater(Process process, ProgressStatus status, User user)
        {
            this.process = process;
            this.status = status;
            this.user = user;
            sp = PersistentSupport.getPersistentSupport(user.Year);
        }

        public void Start()
        {
            if (updateTask == null || updateTask.IsCompleted)
            {
                cancellationTokenSource = new CancellationTokenSource();
                updateTask = Task.Run(() => RunProgressUpdater(cancellationTokenSource.Token), cancellationTokenSource.Token);
            }
        }

        public void Stop()
        {
            if (updateTask != null && !updateTask.IsCompleted)
            {
                cancellationTokenSource.Cancel();

                try
                {
                    Task.WhenAny(updateTask, Task.Delay(TimeSpan.FromSeconds(60))).Wait();
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                }
            }
        }

        private async Task RunProgressUpdater(CancellationToken token)
        {
            int sleep = 500;
            int maxAttempts = 10;
            int attempts = 0;
            while (process.ValFinished == 0 && !token.IsCancellationRequested)
            {
                try
                {
                    lock (process)
                    {
                        ReadChanges();
                        WriteChanges();
                        attempts = 0;
                    }
                }
                catch
                {
                    attempts++;
                    if (attempts >= maxAttempts)
                        return;
                }

                try
                {
                    await Task.Delay(sleep, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            try
            {
                WriteChanges();
            }
            catch
            {
                //não faz tratamento simplesmente termina
            }
        }

        private void WriteChanges()
        {
            process.ValPercenta = (decimal)status.Percentage;
            //process.ValPhases = status.Phase;
            process.ValInfo = status.Info;
            process.ValLastupdt = DateTime.Now;

            try
            {
                sp.openTransaction();
                process.update(sp);
                sp.closeTransaction();
            }
            catch (Exception e)
            {   
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error writing changes on process {process.ValId}", "ProgressUpdater.WriteChanges", e.Message));
                sp.rollbackTransaction();
                throw;
            }
        }

        private void ReadChanges()
        {
            try
            {
                sp.openConnection();
                Process DBprocess = Process.search(sp, process.ValCodascpr, user);

                //GlobalFunctions.CopyDataFromArea(DBprocess, process); Uma função desse género a nível da própria area
                foreach (string key in DBprocess.Fields.Keys)
                {
                    //vamos buscar o Qfield que estamos a ler
                    RequestedField Qfield = DBprocess.Fields[key];
                    //criar um Qfield mas com o alias da area que estamos a criar
                    FieldRef campoRef = new FieldRef(process.Alias, Qfield.Name);
                    process.insertNameValueField(campoRef.FullName, Qfield.Value);
                }

                status.SetEstado(process.ValStatus);
                sp.closeConnection();
            }
            catch (Exception e)
            {                
                Log.Error(string.Format("Excepção de Negócio. [mensagem] {0} [local] {1} [causa] {2}", $"Error reading changes from process {process.ValId}", "ProgressUpdater.ReadChanges", e.Message));
                sp.closeConnection();
                throw;
            }
        }
    }
}