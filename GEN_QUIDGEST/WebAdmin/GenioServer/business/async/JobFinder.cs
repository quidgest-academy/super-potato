using System;
using System.Collections.Generic;
using CSGenio.framework;
using System.Reflection;


namespace CSGenio.business.async
{

    using Process = CSGenioAs_apr;

    /// <summary>
    /// Returns jobs ready to execute based on existing database records
    /// </summary>
    public abstract class JobFinder
    {

        protected abstract Dictionary<String, Type> GetJobTypes();

        public GenioExecutableJob ObtainJob(Process process)
        {
            QCacheInstance cache = QCache.Instance.ManualCode;

            Dictionary<String, Type> loaded = cache.Get("loadedProcesses") as Dictionary<String, Type>;
            if (loaded == null)
            {
                loaded = GetJobTypes();
            }


            string type = process.ValType;
            string mode = process.ValModoproc;
            string key = type + ";" + mode;

            Type processType = loaded[key];
            if (processType != null)
            {
                return createJob(processType, process);
            }
            else
            {
                throw new BusinessException("Erro ao correr o processo agendado.", "ObtainJob", "Não foi encontrada uma classe que implemente o tipo e argumento deste agendamento.");
            }
        }


        private GenioExecutableJob createJob(Type agendamento, Process process)
        {
            ConstructorInfo construtor = agendamento.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
            if (construtor == null)
                throw new BusinessException("Falha na execução do processo.", "createJob", "Não existe nenhum construtor vazio no processo.");
            GenioExecutableJob job = construtor.Invoke(null) as GenioExecutableJob;
            return job;
        }

        protected static void AddType(Dictionary<string, Type> loadedProcesses, Type tipo)
        {
            //Verificar se tem o tipo correto
            GenioProcessType[] pTypes = tipo.GetCustomAttributes(typeof(GenioProcessType), true) as GenioProcessType[];
            if (pTypes.Length == 1)
            {
                //Verificar se tem o mode correto
                GenioProcessMode[] pModes = tipo.GetCustomAttributes(typeof(GenioProcessMode), true) as GenioProcessMode[];
                if (pModes.Length == 1)
                {
                    string key = pTypes[0].Id + ";" + pModes[0].id;
                    loadedProcesses[key] = tipo;
                }
            }
        }
    }


    /// <summary>
    /// Gets jobs from database process records using reflection on the attributes
    /// </summary>
    public class ReflectionJobFinder : JobFinder 
    {
        protected override Dictionary<string, Type> GetJobTypes()
        {
            Dictionary<String, Type> loadedProcesses = new Dictionary<String, Type>();
            
            var assemblyList = new List<Assembly>
            {
                Assembly.Load("GenioServer"),
                Assembly.Load("CSGenio.core"),
            };
            foreach(var assembly in assemblyList)
            {
                Type[] allTypes = assembly.GetTypes();
                foreach (Type tipo in allTypes)
                {
                    AddType(loadedProcesses, tipo);
                }
            }

            QCache.Instance.ManualCode.Put("loadedProcesses", loadedProcesses);
            return loadedProcesses;
        }
    }
}