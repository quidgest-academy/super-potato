using System;
using System.Collections.Generic;
using System.Linq;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Reflection;
using System.Threading.Tasks;

namespace CSGenio.business.async
{  
    using Process = CSGenio.business.CSGenioAs_apr;

    public interface IGenioJob
    {
        Response Result
        {
            get;
        }

        void SetResponse(Response response);

    }

    /// <summary>
    /// Created by JGF at 2017.04.17
    /// An executable job represents a process that can be both scheduled and executed.
    /// </summary>
    public abstract class GenioExecutableJob : ProcessScheduler, IGenioJob
    {
        private Response response;

        public Response Result
        {
            get { return response; }
        }

        public void SetResponse(Response response)
        {
            this.response = response;
        }

        protected Dictionary<Type, PartitionPolicy> specificPolicies = new Dictionary<Type, PartitionPolicy>();
        protected PartitionPolicy defaultPolicy;

        public PartitionPolicy GetPartitionPolicy(GenioExecutableJob job)
        {
            Type type = job.GetType();
            if (specificPolicies.ContainsKey(type))
                return specificPolicies[type];
            else
                return defaultPolicy;
        }

        /// <summary>
        /// Separates the job into smaller sub-units to be compared with other jobs
        /// </summary>
        public virtual void SetPartitionPolicies()
        {
            defaultPolicy = new GlobalPartition();
        }

        /// <summary>
        /// Indicates the priority of the process. Override to change the priority
        /// </summary>
        public virtual JobPriority Priority
        {
            get { return JobPriority.NORMAL; }
        }

        public void FillArguments(PersistentSupport sp, User user, Process process)
        {
            var arguments = GetArguments();
            List<CSGenioAs_arg> allBdArgs = CSGenioAs_arg.searchList(sp, user,
                CriteriaSet.And().Equal(CSGenioAs_arg.FldCods_apr, process.ValCodascpr));
            foreach (var member in arguments)
            {
                List<CSGenioAs_arg> bdArgs = allBdArgs.FindAll(a => a.ValId.ToLower() == member.Name.ToLower());

                object value = GetValue(bdArgs, member);

                SetValue(member, value);
            }
        }

        public void FillArguments(Dictionary<string, string> args)
        {
            var arguments = GetArguments();
            foreach (var member in arguments)
            {
                String memberName = member.Name.ToUpper();
                String valueStr = args[memberName];
                List<String> valueList = valueStr.Split(';').ToList();
                object value = GetValue(valueList, member);
                SetValue(member, value);
            }
        }

        private void SetValue(MemberInfo member, object value)
        {
            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)member;
                field.SetValue(this, value);
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo property = (PropertyInfo)member;
                property.SetValue(this, value, null);
            }
        }

        private string Value(object arg)
        {
            if (arg?.GetType() == typeof(CSGenioAs_arg))
                return ((CSGenioAs_arg)arg).ValValor;
            else
                return (String)arg;

        }

        private object GetValue<T>(List<T> args, MemberInfo member)
        {
            Type type = null;
            if (member.MemberType == MemberTypes.Field)
                type = ((FieldInfo)member).FieldType;
            else if (member.MemberType == MemberTypes.Property)
                type = ((PropertyInfo)member).PropertyType;

            if (type == null)
                throw new FrameworkException("Data conversion error.", "Job.GetValue", "Unidentified field type.");

            if (type.GetInterfaces().Contains(typeof(ICollection<String>)))
            {
                List<String> lista = new List<String>();
                foreach (T arg in args)
                {
                    lista.Add(Value(arg));
                }
                if (type == typeof(List<String>))
                    return lista;
                else
                    return System.Convert.ChangeType(lista, type);
            }
            else
            {
                return ParseString(Value(args.Count > 0 ? args[0] : default), type);
            }
        }

        Object ParseString(string value, Type type)
        {
            if (type.Equals(typeof(System.Int32)))
                return Conversion.string2Int(value);
            else if (type == typeof(System.Double) || type == typeof(System.Decimal))
                return Conversion.string2Numeric(value);
            else if (type == typeof(System.DateTime))
                return Conversion.dateTimeString2DateTime(value);
            else if (type == typeof(System.Boolean))
                return Conversion.string2Bool(value);
            else if (type == typeof(System.String))
                return value;
            else if (type.IsEnum)
                return Enum.Parse(type, value);
            else
                throw new FrameworkException("Erro na conversão de dados.", "Job.ParseString", "Conversão de dados não implementada");

        }

        private static ICollection<String> Convert(Type type, object value)
        {
            List<String> lista = new List<string>();
            if (type.Equals(typeof(System.Int32)))
                lista.Add(Conversion.internalInt2String(value));
            else if (type.Equals(typeof(System.Double)) || type.Equals(typeof(System.Decimal)))
                lista.Add(Conversion.internalNumeric2String(value));

            else if (type.Equals(typeof(System.DateTime)))
                lista.Add(Conversion.internalDateTime2String(value, FieldFormatting.DATASEGUNDO));

            else if (type.Equals(typeof(System.String)) || type.Equals(typeof(System.Guid)))
                lista.Add(Conversion.internalString2String(value));

            else if (type.Equals(typeof(System.Boolean)))
                lista.Add(Conversion.internalInt2String(Conversion.string2Int(value)));

            else if (value is ICollection<String>)
                return (ICollection<String>)value;


            if (lista.Count == 0)
                throw new FrameworkException("Erro na conversão de tipo de campo interno para string.", "Conversao.interno2InternoString", "Erro na conversão de tipo de campo interno para string, o tipo de formatação do campo não está definido");
            else
                return lista;
        }

        public virtual bool AreRequirementsMet(PersistentSupport sp, User user)
        {
            return WasDailyUpdateExecuted(sp);
        }

        private static DateTime? GetLastUpdate(PersistentSupport sp)
        {            
            var query = "SELECT TOP 1 CHECKDAT FROM FORCFG";
            DataMatrix mat = sp.executeQuery(query);
            if (mat.NumRows > 0)
                return mat.GetDate(0, 0);

            return null;
        }

        public static bool WasDailyUpdateExecuted(PersistentSupport sp)
        {
            var lastUpdate = GetLastUpdate(sp);

            //return !(lastUpdate == null || (DateTime.Now.Date - lastUpdate.Value.Date).TotalDays >= 1);
            return true;
        }
    }

    /// <summary>
    /// Created by JGF at 2017.04.17
    /// An external job is executed by an external application. 
    /// </summary>
    public abstract class GenioExternalJob : GenioExecutableJob
    {

    }


    /// <summary>
    /// Created by JGF at 2017.04.17
    /// Server side jobs are implemented and executed in the server. All subclasses must implement the Run() method.
    /// </summary>
    public abstract class GenioServerJob : GenioExecutableJob
    {
        protected ProgressStatus progress;

        public GenioServerJob() : base()
        {
            this.progress = new ProgressStatus();
        }

        public abstract void Execute(PersistentSupport sp, User user);

        public ProgressStatus Progress { get { return progress; } }
    }

    public abstract class GenioServerJobAsync : GenioExecutableJob
    {
        protected ProgressStatus progress;

        public GenioServerJobAsync() : base()
        {
            this.progress = new ProgressStatus();
        }

        [Obsolete]
        public virtual void Execute(PersistentSupport sp, User user)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<string> ExecuteAsync(PersistentSupport sp, User user, Process process)
        {
            return await Task.FromResult("");
        }

        public ProgressStatus Progress { get { return progress; } }
    }


    public class ProgressStatus
    {
        private double percentage;
        private int totalJobs;
        private int numJobs;
        public string extraInfo;
        private int numPhases;
        public string phaseText = "";
        private int totalPhases;
        private string estado;

        /// <summary>
        /// Restarts the current percentage of the progress status;
        /// </summary>
        public void Init()
        {
            percentage = 0;
        }

        /// <summary>
        /// Restarts the job count and sets a new maximum of jobs from which the percentage will be calculated.
        /// The percentage should be incremented using the Increment function.
        /// </summary>
        public void InitRange(int totalJobs)
        {
            this.numJobs = 0;
            this.percentage = 0;
            this.totalJobs = totalJobs;
        }

        public void NextJob(String info)
        {
            Increment();
            SetInfo(info);
        }

        /// <summary>
        /// Sets the current percentage to a specified value
        /// </summary>
        public void SetPercentage(double percentage)
        {
            this.percentage = percentage;
        }

        /// <summary>
        /// Increments the number of jobs and updates the percentage accordingly.
        /// InitRange() is a precondition to work correctly
        /// </summary>
        public void Increment()
        {
            numJobs++;
            if (totalJobs != 0)
                percentage = ((double)numJobs) / (double)totalJobs * 100.0;
        }

        public void SetInfo(string info)
        {
            this.extraInfo = info;
        }

        /// <summary>
        /// Sets the total number of phases and the text of the first phase. 
        /// This method should only be used one time per process.
        /// </summary>
        /// <param name="numPhases">Total number of phases</param>
        /// <param name="firstPhase">The text of the first phase</param>
        public void SetPhases(int numPhases, string firstPhase)
        {
            this.numPhases = 0;
            this.totalPhases = numPhases;
            this.phaseText = firstPhase;
        }

        /// <summary>
        /// Increments the phase counter and updates the phase text.
        /// </summary>
        /// <param name="phaseText">The text of the next phase</param>
        public void NextPhase(string phaseText, double percentage)
        {
            this.numPhases++;
            this.phaseText = phaseText;
            this.percentage = percentage;
        }

        /// <summary>
        /// Increments the phase counter and updates the phase text.
        /// </summary>
        /// <param name="phaseText">The text of the next phase</param>
        public void NextPhase(string phaseText)
        {
            this.numPhases++;
            this.phaseText = phaseText;
        }

        /// <summary>
        /// Increments the phase counter.
        /// The phase text is the same as before.
        /// </summary>
        public void NextPhase()
        {
            this.numPhases++;
        }

        public double Percentage
        {
            get { return percentage; }
        }

        public string Info
        {
            get { return extraInfo; }
        }

        public string Phase
        {
            get
            {
                if (!string.IsNullOrEmpty(phaseText))
                    return phaseText + " (" + numPhases + "/" + totalPhases + ")";

                //avoiding 0/0 when phasetext is empty  
                if (numPhases > 0 || totalPhases > 0)
                    return numPhases + "/" + totalPhases;

                return "";
            }
        }

        // Useful for single-phase processes
        public string Job
        {
            get
            {
                if (!string.IsNullOrEmpty(extraInfo))
                    return extraInfo + " (" + numJobs + "/" + totalJobs + ")";

                //avoiding 0/0 when phasetext is empty  
                if (numJobs > 0 || totalJobs > 0)
                    return numJobs + "/" + totalJobs;

                return "";
            }
        }

        public void ThrowIfCancel()
        {
            if (CheckCancel())
                throw new AbortedWorkerException();
        }

        public bool CheckCancel()
        {
            return estado == ArrayS_prstat.E_AB_7 || estado == ArrayS_prstat.E_AC_8;
        }

        internal void SetEstado(string valEstado)
        {
            this.estado = valEstado;
        }
    }

    /// <summary>
    /// Class that encapsulates a job response
    /// </summary>
    public class Response
    {
        //protected List<RelatMsg> relatorioErros;
        private List<Resource> recursos;
        private StatusMessage status;

        public Response(StatusMessage status)
        {
            this.status = status;
            //relatorioErros = new List<RelatMsg>();
            recursos = new List<Resource>();
        }

        public List<Resource> QResources
        {
            get { return recursos; }
        }

        //public List<RelatMsg> RelatoriosErro
        //{
        //    get { return relatorioErros; }
        //}

        //public void AddRelatorioErros(RelatMsg relatorio)
        //{
        //    relatorioErros.Add(relatorio);
        //}

        public StatusMessage Status
        {
            get { return status; }
            set { this.status = value; }
        }

        public bool IsOk()
        {
            if (Status != null)
            {
                return Status.IsOk();
            }

            return false;
        }

        //public virtual RelatMsg CreateLazyReport(string titulo, RelatSortType sort)
        //{
        //    LazyRelatMsg relat = new LazyRelatMsg(titulo, sort);
        //    relatorioErros.Add(relat);
        //    return relat;
        //}

        //public virtual RelatMsg CreatePersistentReport(string titulo, RelatSortType sort, User user)
        //{
        //    PersistRelatMsg relat = new PersistRelatMsg(user, titulo, sort, null);
        //    relatorioErros.Add(relat);
        //    return relat;
        //}
    }


    /// <summary>
    /// Class to be used by a process response
    /// </summary>
    public class ProcessResponse : Response
    {
        private Process process;

        public ProcessResponse(StatusMessage status, Process process) : base(status)
        {
            this.process = process;
        }

        //public override RelatMsg CreatePersistentReport(string titulo, RelatSortType sort, User user)
        //{
        //    PersistRelatMsg relat = new PersistRelatMsg(user, titulo, sort, process);
        //    relatorioErros.Add(relat);
        //    return relat;
        //}

    }

}