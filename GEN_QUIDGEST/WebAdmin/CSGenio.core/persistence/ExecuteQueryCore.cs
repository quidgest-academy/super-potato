using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Xml.Serialization;
using System.Xml;
using System.Threading;
using System.Text.Json.Serialization;
using System.Linq;

namespace ExecuteQueryCore
{
    /// <summary>
    /// Formato de mudança de linha
    /// </summary>
    public enum NewLineFormat
    {
        Windows,
        Unix,
        Default
    }
    
    /// <summary>
    /// Tipo de conexão ao servidor
    /// </summary>
    public enum ConnectionType
    {
        Normal,
        Admin,
        Log
    }

    public delegate void ChangedEventHandler(object sender, EventArgs e, RdxStatus status);

    public class ReindexProgressEventArgs : EventArgs
    {
        public string CurrentScriptName { get; private set; }
        public int TotalScripts { get; private set; }
        public int CurrentScript { get; private set; }

        public int TotalBlocks { get; private set; }
        public int CurrentBlock { get; private set; }

        public ReindexProgressEventArgs(string currentScriptName, int totalScript, int currentScript, int totalBlocks, int currentBlock)
        {
            this.CurrentScriptName = currentScriptName;
            this.TotalScripts = totalScript;
            this.CurrentScript = currentScript;

            this.TotalBlocks = totalBlocks;
            this.CurrentBlock = currentBlock;
        }
    }

    public class RdxScriptPair
    {
        public string ScriptName { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public Action<CancellationToken> Execute { get; set; }
        public string MinDbVersion { get; set; }
        public string MaxDbVersion { get; set; }
        public ConnectionType Connection { get; set; }
        public int Timeout { get; set; }
    }

    public class RdxScript
    {
        public string Script { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public Action<CancellationToken> Execute { get; set; }
        public string MinDbVersion { get; set; }
        public string MaxDbVersion { get; set; }
        public ConnectionType Connection { get; set; }
        public int Timeout { get; set; }
    }

    public class RdxParamUpgradeSchema
    {
        //events
        public event ChangedEventHandler ChangedExecutionScript;
        // Invoke the Changed event; called whenever list changes
        public virtual void OnChangedExecutionScript(EventArgs e, RdxStatus status)
        {
            if (ChangedExecutionScript != null)
                ChangedExecutionScript(this, e, status);
        }

        public RdxParamUpgradeSchema()
        {
            this.Progress = new RdxStatus();
        }

        //This method will not clone complex objects (since those are "copied" by reference)
        public RdxParamUpgradeSchema Clone() {
            RdxParamUpgradeSchema returnItem = new RdxParamUpgradeSchema();
            returnItem.DirFilestream = this.DirFilestream;
            returnItem.Origin = this.Origin;
            returnItem.Password = this.Password;
            returnItem.Path = this.Path;
            returnItem.Username = this.Username;
            returnItem.Year = this.Year;
            returnItem.Zero = this.Zero;
            returnItem.Progress = this.Progress.Clone();
            return returnItem;
        }

        //arguments
        public string Year { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<RdxScript> OrderExec { get; set; }
        public string Path { get; set; }
        public string DirFilestream { get; set; }
        public bool Zero { get; set; }
        public string Origin { get; set; }

        //progress
        public RdxStatus Progress { get; set; }
    }

    public enum RdxProgressStatus
    {
        NOT_STARTED,
        RUNNING,
        CANCELLED,
        SUCCESS,
        ERROR,
        FINISHED
    }

    public class RdxStatus
    {
        public int ActualScriptNum { get; set; }
        public string ActualScript { get; set; }
        public int TotalScriptNum { get; set; }
        public string Message { get; set; }
        public RdxProgressStatus State { get; set; }

        public RdxStatus() {
            this.ActualScriptNum = 0;
            this.ActualScript = "";
            this.TotalScriptNum = 0;
            this.Message = "";
            this.State = RdxProgressStatus.NOT_STARTED;
        }

        public RdxStatus(int ActualScriptNum, string ActualScript, int TotalScriptNum, string Message, RdxProgressStatus state = RdxProgressStatus.NOT_STARTED) {
            this.ActualScriptNum = ActualScriptNum;
            this.ActualScript = ActualScript;
            this.TotalScriptNum = TotalScriptNum;
            this.Message = Message;
            this.State = state;
        }

        public RdxStatus Clone() {
            RdxStatus returnItem = new RdxStatus();
            returnItem.ActualScriptNum = this.ActualScriptNum;
            returnItem.ActualScript = this.ActualScript;
            returnItem.TotalScriptNum = this.TotalScriptNum;
            returnItem.Message = this.Message;
            returnItem.State = this.State;
            return returnItem;
        }

        public double Percentage() {
            if(TotalScriptNum == 0) return 0;
            
            return ActualScriptNum * 100 / (double)TotalScriptNum;
        }

        public bool IsFinished()
        {
            return State != RdxProgressStatus.RUNNING && State != RdxProgressStatus.NOT_STARTED;
        }
    }

    public class RdxParamExecuteServer
    {
        //eventos
        public event ChangedEventHandler ChangedExecuteServer;
        // Invoke the Changed event; called whenever list changes
        public virtual void OnChangedExecuteServer(EventArgs e, RdxStatus status)
        {
            if (ChangedExecuteServer != null)
                ChangedExecuteServer(this, e, status);
        }


        //argumentos
        public IDbConnection Conn { get; set; }
        public IDbConnection AdmConn { get; set; }
        public IDbConnection LogConn { get; set; }
        public IDbConnection DefConn { get; set; }
        public bool ContinueAfterError { get; set; }
        public string Origin { get; set; }
        public string DataSystem { get; set; }
    }

    /**
     * Auxiliary class to extract the essential information from a RdxOperationLog instance.
     */
    public class RdxOperationInfo
    {
        public int Id { get; set; }
        public string StartTime { get; set; }
        public int Duration { get; set; }
        public string DataSystem { get; set; }
        public string Database { get; set; }
        public string Origin { get; set; }
        public bool Success { get; set; }
    }

    public class RdxOperationLog
    {
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string Database { get; set; }
        public string DataSystem { get; set; }
        public string Origin { get; set; }
        public List<RdxScriptLog> ScriptDetails { get; set; }

        public static List<RdxOperationLog> readAggregateXML(string filename)
        {
            var res = new List<RdxOperationLog>();
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;

                using (XmlReader input = XmlReader.Create(filename, settings))
                {
                    while (!input.EOF)
                    {
                        System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(RdxOperationLog));
                        RdxOperationLog obj = (RdxOperationLog)s.Deserialize(input);
                        res.Add(obj);
                    }
                }
            }
            catch (Exception)
            {
                //We just ignore any error here and just return whatever we were able to read
            }
            return res;
        }

        public static RdxOperationLog FindXML(int logIndex, string filename)
        {
            if (logIndex < 0)
                return null;

            List<RdxOperationLog> logs = readAggregateXML(filename);

            return logIndex >= logs.Count ? null : logs[logIndex];
        }

        public void appendXML(string filename)
        {
            /*
            using (System.IO.StreamWriter output = new System.IO.StreamWriter(filename))
            {
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(this.GetType());
                s.Serialize(output, this);
            }
            */
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.UTF8;
            //settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Indent = true;

            using (StreamWriter output = new System.IO.StreamWriter(filename, true))
            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                XmlSerializer s = new XmlSerializer(this.GetType());
                s.Serialize(writer, this);
            }
        }
    }

    public class RdxScriptLog
    {
        [XmlAttribute]
        public string ScriptId { get; set; }
        [XmlAttribute]
        public DateTime StartTime { get; set; }
        [XmlAttribute]
        public int Duration { get; set; }
        public string Result { get; set; }
        public List<RdxScriptLog> ScriptDetails { get; set; }

        public RdxScriptLog() 
        {
            this.ScriptId = string.Empty;
            this.StartTime = DateTime.Now;
            this.Duration = 0;
            this.Result = string.Empty;
            this.ScriptDetails = new List<RdxScriptLog>();
        }

        public RdxScriptLog(string scriptId, DateTime startTime, int duration, string res)
        {
            this.ScriptId = scriptId;
            this.StartTime = startTime;
            this.Duration = duration;
            this.Result = res;
            this.ScriptDetails = new List<RdxScriptLog>();
        }
    }
	
	/// <summary>
    /// Transfer log operation
    /// </summary>
    public class TransferLogOperation
    {
        /// <summary>
        /// Gets or sets amount the rows copied.
        /// </summary>
        /// <value>
        /// The rows copied.
        /// </value>
        public long Copied { get; set; }

        /// <summary>
        /// Gets or sets the total amount of rows to copy.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public long Total { get; set; }

        /// <summary>
        /// Gets or sets the current table being processed.
        /// </summary>
        /// <value>
        /// The current table.
        /// </value>
        public string CurrentTable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TransferLogOperation"/> is completed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if completed; otherwise, <c>false</c>.
        /// </value>
        public bool Completed { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Classe que permite ler vários ficheiros de scripts SQL, aplicar substituições com base
    /// num dicionário, e executa-los ordenadamente sobre uma BD, ou então gravá-los em disco
    /// </summary>
    public class ExecuteQueryWorker
    {
        public event EventHandler<ReindexProgressEventArgs> CurrentScriptChanged;
        public event EventHandler<ReindexProgressEventArgs> CurrentScriptBlockChanged;
        public event EventHandler<ReindexProgressEventArgs> OnComplete;

        private string m_currentScriptName;

        public int TotalScripts { get; private set; }
        public int CurrentScriptNumber { get; private set; }
        public int TotalScriptBlock { get; private set; }
        public int CurrentBlockScript { get; private set; }

        private string currentBlock;

        // directoria base de trabalho
        private string m_baseDirectory;

        // nome do ficheiro com a lista de scripts (para eliminar futuramente)
        private string m_orderToExecute;

        // lista de scripts a executar (nova implementação alternativa ao ficheiro order2exec.dat)
        public List<RdxScript> m_LstOrderToExec;

        // nome do ficheiro de dicionario (substituições) (uso exclusivo com m_replaces)
        private string m_dictionary;

        private string m_logFile;

        // lista de substituições (uso exclusivo com m_dictionary)
        List<KeyValuePair<string, string>> m_replaces;

        // lista de filtros a ser aplicados aos scripts (strings vão ser eliminadas)
        List<string> m_filters = new List<string>();

        //Adiciona um filtro ao parser de scripts
        public void AddFilter(string s)
        {
            m_filters.Add(s);
        }

        // formato de mudança de linha
        private NewLineFormat m_newlineFormat;

        /// <summary>
        /// Nome do ficheiro com script que está a ser executado actualmente
        /// </summary>
        public string CurrentScriptName
        {
            get { return this.m_currentScriptName; }
            private set
            {
                this.m_currentScriptName = value;
                if (this.CurrentScriptChanged != null)
                    this.CurrentScriptChanged(this, new ReindexProgressEventArgs(this.CurrentScriptName,
                                                                                 this.TotalScripts,
                                                                                 this.CurrentScriptNumber,
                                                                                 this.TotalScriptBlock,
                                                                                 this.CurrentBlockScript));
            }
        }

        /// <summary>
        /// Bloco de código do script que está a ser executado actualmente
        /// </summary>
        public string CurrentBlockStr
        {
            get { return this.currentBlock; }
            private set
            {
                this.currentBlock = value;
                if (this.CurrentScriptBlockChanged != null)
                    this.CurrentScriptBlockChanged(this, new ReindexProgressEventArgs(this.CurrentScriptName,
                                                                                 this.TotalScripts,
                                                                                 this.CurrentScriptNumber,
                                                                                 this.TotalScriptBlock,
                                                                                 this.CurrentBlockScript));
            }
        }

        /// <summary>
        /// Indica se a thread ainda está a correr
        /// </summary>

        // separador dos blocos de scripts para os dados de entrada
        // (separador utilizado pelo genio na geração)
        private static string m_genSeparator = "GO";

        // mapeamento dos formatos de mudança de linha e respectivos caracteres
        private static string[] m_newlineChars = new string[] { "\r\n", "\n", Environment.NewLine };

        private ExecuteQueryWorker(string baseDirectory, List<RdxScript> orderToExecute, string logfile, NewLineFormat newlineFormat)
        {
            this.m_baseDirectory = baseDirectory;
            this.m_LstOrderToExec = orderToExecute;
            this.m_dictionary = null;
            this.m_replaces = null;
            this.m_newlineFormat = newlineFormat;
            this.m_logFile = logfile.Length == 0 ? Path.Combine(m_baseDirectory, "LogFile.dat") : logfile;
            CurrentBlockStr = "";
        }

        private ExecuteQueryWorker(string baseDirectory, string orderToExecute, string logfile, NewLineFormat newlineFormat)
        {
            this.m_baseDirectory = baseDirectory;
            this.m_orderToExecute = orderToExecute;
            this.m_LstOrderToExec = new List<RdxScript>();
            this.m_dictionary = null;
            this.m_replaces = null;
            this.m_newlineFormat = newlineFormat;
            this.m_logFile = logfile.Length == 0 ? Path.Combine(m_baseDirectory, "LogFile.dat") : logfile;
            CurrentBlockStr = "";
        }

        /// <summary>
        /// Cria um ExecuteQueryWorker que utiliza um ficheiro de dicionário para efectuar os replaces
        /// (Utilizado pelo ExecuteQuery)
        /// </summary>
        /// <param name="baseDirectory">directoria base de trabalho</param>
        /// <param name="orderToExecute">nome do ficheiro com a lista de scripts</param>
        /// <param name="dictionary">nome do ficheiro de dicionario (substituições)</param>
        /// <param name="newlineFormat">formato de mudança de linha</param>
        public ExecuteQueryWorker(string baseDirectory, string orderToExecute, string dictionary, NewLineFormat? newlineFormat, string logfile)
            : this(baseDirectory, orderToExecute, logfile, newlineFormat ?? NewLineFormat.Default)
        {
            this.m_dictionary = dictionary;
        }

        /// <summary>
        /// Cria um ExecuteQueryWorker que utiliza uma lista de elementos chave/valor especificada por parametro para efectuar os replaces
        /// (Utilizado pelo Qconfig)
        /// </summary>
        /// <param name="baseDirectory">directoria base de trabalho</param>
        /// <param name="orderToExecute">nome do ficheiro com a lista de scripts</param>
        /// <param name="dictionary"></param>
        /// <param name="newlineFormat">formato de mudança de linha</param>
        public ExecuteQueryWorker(string baseDirectory, string orderToExecute, List<KeyValuePair<string, string>> dictionary, NewLineFormat? newlineFormat, string logfile)
            : this(baseDirectory, orderToExecute, logfile, newlineFormat ?? NewLineFormat.Default)
        {
            this.m_replaces = dictionary;
        }

        /// <summary>
        /// Cria um ExecuteQueryWorker que utiliza uma lista de elementos chave/valor especificada por parametro para efectuar os replaces
        /// (Utilizado pelo Qconfig)
        /// </summary>
        /// <param name="baseDirectory">directoria base de trabalho</param>
        /// <param name="orderToExecute">lista de scripts</param>
        /// <param name="dictionary"></param>
        /// <param name="newlineFormat">formato de mudança de linha</param>
        public ExecuteQueryWorker(string baseDirectory, List<RdxScript> orderToExecute, List<KeyValuePair<string, string>> dictionary, NewLineFormat? newlineFormat, string logfile)
            : this(baseDirectory, orderToExecute, logfile, newlineFormat ?? NewLineFormat.Default)
        {
            this.m_replaces = dictionary;
        }

        // lê o conteúdo do ficheiro com a lista de scripts
        private List<RdxScript> ReadOrderToExecute()
        {
            List<RdxScript> result = new List<RdxScript>();
            string order_path = Path.Combine(m_baseDirectory, m_orderToExecute);

            //TSX (14/05/2013) - permitir que o streamreader verifique qual o enconding em que o ficheiro se encontre para permitir a leitura de ficheiros em UTF-8 
            using (StreamReader sr = new StreamReader(order_path, true))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                    result.Add(new RdxScript() { Script = line.Split(';')[0], Connection = (line.Split(';')[1] == "true") ? ConnectionType.Admin : ConnectionType.Normal });

                sr.Close();
            }

            return result;
        }

        // Obtem a representação da mudança de linha
        private string GetNewlineChars()
        {
            return m_newlineChars[(int)m_newlineFormat];
        }

        // lê o conteúdo do ficheiro de dicionario (substituições)
        // TODO: 
        // quando se passar a receber parâmetros do QConfig vai ter de se adaptar esta função, porque
        // não faz sentido ler as substituições de um ficheiro, os dados já existem nas configurações
        // sugestão: passar esta classe a parcial e criar duas filhas que re-implementam esta função (será que vale a pena?)
        private List<KeyValuePair<string, string>> GetReplaceDictionary()
        {
            // se a lista de replaces foi especificada (pelo Qconfig, por exemplo), devolve-a, se não lê o conteúdo do ficheiro
            if (m_replaces != null)
                return m_replaces;

            // lista de pares para as substituições
            // a chave corresponde ao texto a encontrar e o valor ao texto que substitui a chave
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            string dictionary_path = Path.Combine(m_baseDirectory, m_dictionary);

            //TSX (14/05/2013) - permitir que o streamreader verifique qual o enconding em que o ficheiro se encontre para permitir a leitura de ficheiros em UTF-8
            using (StreamReader sr = new StreamReader(dictionary_path, true))
            {
                string searchFor, replaceWith, text;

                while ((text = sr.ReadLine()) != null)
                {
                    // são separados pelo primeiro espaço da linha
                    searchFor = text.Substring(0, text.IndexOf(' '));
                    replaceWith = text.Substring(text.IndexOf(' ') + 1);
                    result.Add(new KeyValuePair<string, string>(searchFor, replaceWith));
                }

                sr.Close();
            }

            return result;
        }

        // lê os scripts e aplica os replaces (incluindo as mudanças de linha)
        private List<RdxScriptPair> ReadScriptsApplyReplaces(List<RdxScript> orderToExec, List<KeyValuePair<string, string>> replaces)
        {
            // lista de scripts
            // a chave corresponde ao nome do ficheiro, o valor corresponde ao conteúdo do script
            List<RdxScriptPair> result = new List<RdxScriptPair>();

            foreach (RdxScript script in orderToExec)
            {
                string scriptPath = Path.Combine(m_baseDirectory, script.Script);
                string content;

                //TSX (14/05/2013) - permitir que o streamreader verifique qual o enconding em que o ficheiro se encontre para permitir a leitura de ficheiros em UTF-8
                // lê o script
                if(script.Type == null || script.Type == "SQL")
                {
                    using (StreamReader sr = new StreamReader(scriptPath, true))
                    {
                        content = sr.ReadToEnd();
                        sr.Close();
                    }
                }
                else 
                {
                    content = "";
                }
                

                // aplica os replaces do dicionário
                foreach (KeyValuePair<string, string> replace in replaces)
                {
                    // Last updated by [CJP] at [2016.07.04]
                    // Alterar o valor da tag do schema quando o script utiliza a conexão da base de dados de auditoria
                    if (script.Connection == ConnectionType.Log && (replace.Key == "W_GnBD" || replace.Key == "W_GnTBS"))
                        content = content.Replace(replace.Key, replaces.Find(x => x.Key == "W_GnLogBD").Value);
                    else
                        content = content.Replace(replace.Key, replace.Value);
                }

                // aplica os replaces para uniformizar as mudanças de linha
                // NOTA: os formato de mudança de linha para Macs não estão a ser tratados,
                // porque não se justifica. Caso seja necessário, acrescenta-se um replace
                // para \r que é o caracter de mudança de linha para Macs
                string newline = GetNewlineChars();

                content = content.Replace("\r\n", "\n"); // após este replace, assume-se que só já existem \n
                content = content.Replace("\n", newline);

                // filtros expeciais por plataforma (por ex. MySql)
                foreach(var f in m_filters)
                    content = content.Replace(f, "");

                result.Add(new RdxScriptPair { ScriptName = Path.GetFileName(scriptPath), Content = content, Connection = script.Connection, Timeout = script.Timeout, Type = script.Type, MinDbVersion = script.MinDbVersion, MaxDbVersion = script.MaxDbVersion, Execute = script.Execute, Version = script.Version });

            }

            return result;
        }

        // lê todos os scripts da lista de scripts e aplicar as substituições
        private List<RdxScriptPair> ReadScriptsWithReplaces()
        {
            // codigo de transição order2exec.dat para estrutura XML
            if (m_LstOrderToExec.Count == 0)
                m_LstOrderToExec = ReadOrderToExecute();

            List<KeyValuePair<string, string>> replaces = GetReplaceDictionary();
            return ReadScriptsApplyReplaces(m_LstOrderToExec, replaces);
        }

        //  verifica se uma string tem conteúdo válido
        private static bool hasContent(string str)
        {
            string str2 = str.Replace(" ", "");
            str2 = str2.Replace("\r", "");
            str2 = str2.Replace("\n", "");
            return str2.Length > 0;
        }

        /// <summary>
        /// Grava os scripts para a pasta indicada, utilizando o separador 
        /// específicado por argumento e aplicando as substituições do dicionário
        /// </summary>
        /// <param name="path">directoria de output</param>
        /// <param name="outputSeparator">separador de scripts, por  norma para SQL Server usa-se "GO", para Oracle usa-se "/"</param>
        public void SaveToDir(string path, string outputSeparator)
        {
            // obtém a lista de scripts já com as substituições efectuadas
            List<RdxScriptPair> scripts = ReadScriptsWithReplaces();
            string newline = GetNewlineChars();

            foreach (RdxScriptPair script in scripts)
            {
                string scriptContent = script.Content;
                // para cada script divide pelo separador com que foram gerados ("GO")
                string[] scriptSplited = scriptContent.Split(new string[] { "\n" + m_genSeparator }, StringSplitOptions.None);
                StringBuilder sb = new StringBuilder(scriptContent + scriptSplited.Length * (outputSeparator.Length + newline.Length));

                foreach (string scriptBlock in scriptSplited)
                {
                    if (hasContent(scriptBlock))
                    {
                        // acrescenta cada bloco de instruções ao
                        // output e utiliza o separador específicado
                        sb.Append(scriptBlock);
                        sb.Append(newline);
                        sb.Append(outputSeparator);
                        sb.Append(newline);
                    }
                }

                string fileName = script.ScriptName;
                string outputPath = Path.Combine(path, fileName);

                // grava para o ficheiro o conteúdo final
                File.WriteAllText(outputPath, sb.ToString(), Encoding.Default);
            }
        }

        /// <summary>
        /// Executa scripts sobre uma base de dados, aplicando as substituições do dicionário
        /// </summary>
        /// <param name="conn">Conexão à bd (deve estar aberta!)</param>
        /// <param name="continueAfterError">Se é para continuar a executar os scripts caso ocorra algum erro</param>
        public void ExecuteServer(IDbConnection conn, bool continueAfterError)
        {
            ExecuteServer(conn, null, null, continueAfterError);
        }

        /// <summary>
        /// Executa scripts sobre uma base de dados, aplicando as substituições do dicionário
        /// </summary>
        /// <param name="conn">Conexão à bd (deve estar aberta!)</param>
        public void ExecuteServer(IDbConnection conn)
        {
            ExecuteServer(conn, null, null, false);
        }

        /// <summary>
        /// Executa scripts sobre uma base de dados, aplicando as substituições do dicionário
        /// </summary>
        /// <param name="conn">Conexão à bd</param>
        /// <param name="Admconn">Conexão à bd com user com privilegios de admin</param>
        public void ExecuteServer(IDbConnection conn, IDbConnection AdmConn)
        {
            ExecuteServer(conn, AdmConn, null, false);
        }

        /// <summary>
        /// Executa scripts sobre uma base de dados, aplicando as substituições do dicionário
        /// </summary>
        /// <param name="conn">Conexão à bd</param>
        /// <param name="continueAfterError">Se é para continuar a executar os scripts caso ocorra algum erro</param>
        public void ExecuteServer(IDbConnection conn, IDbConnection AdmConn, IDbConnection logConn, bool continueAfterError)
        {
            RdxParamExecuteServer param = new RdxParamExecuteServer();
            param.Conn = conn;
            param.AdmConn = AdmConn;
            param.LogConn = logConn;
            param.ContinueAfterError = continueAfterError;

            ExecuteServer(param);
        }

        public void ExecuteServer(RdxParamExecuteServer param)
        {
            ExecuteServer(param, new CancellationToken());
        }

        /// <summary>
        /// Executa scripts sobre uma base de dados, aplicando as substituições do dicionário
        /// </summary>
        public void ExecuteServer(RdxParamExecuteServer param, CancellationToken cToken, bool dbExists = true)
        {
            bool execError = false;
            RdxOperationLog log = new RdxOperationLog();
            log.StartTime = DateTime.Now;
            log.Database = param.Conn.Database;
            log.ScriptDetails = new List<RdxScriptLog>();
            log.Origin = param.Origin;
            log.DataSystem = param.DataSystem;

            RdxScriptLog scriptLog;
            RdxScriptLog blockLog;
            RdxScriptPair script = null;
            RdxStatus status = new RdxStatus();
            List<RdxScriptPair> scripts = null;
            int rdxNum = 0;

            try
            {
                //string sqlConnectionString = SQL.GetConnectionString(txtServer.Text, cmbBD.SelectedItem.ToString(), txtUser.Text, txtPass.Text);

                // RR 24-08-2011 a ligação é suposto estar aberta!
                //// abre a ligação e chama a função do worker que vai executar os scripts na BD
                //conn.Open();
                // obtém a lista de scripts já com as substituições efectuadas
                scripts = ReadScriptsWithReplaces();

                TotalScripts = scripts.Count;
                CurrentScriptNumber = 0;
                List<int> incrementScripts = new List<int>();
                for (rdxNum = 0; rdxNum < scripts.Count; rdxNum++)
                {
                    script = scripts[rdxNum];

                    status = new RdxStatus(rdxNum, script.ScriptName, scripts.Count, null, RdxProgressStatus.RUNNING);
                    param.OnChangedExecuteServer(EventArgs.Empty, status); //Trigger events

                    //If the cancelation token is activated, then end the reindexation
                    cToken.ThrowIfCancellationRequested();

                    // Don't run update client scripts if the database doesn't exist
                    // There is no data to migrate and BeforeSchema routines would fail to run obviously
                    if ((script.ScriptName == "UpgradeClient.sql" || script.ScriptName == "UpgradeClient") && !dbExists)
                        continue;

                    scriptLog = new RdxScriptLog(script.ScriptName, DateTime.Now, 0, "");

                    // actualiza a variável global com o nome do ficheiro a ser executado actualmente
                    this.CurrentScriptName = script.ScriptName;
                    string scriptContent = script.Content;
                    string[] scriptSplited = scriptContent.Split(new string[] { "\n" + m_genSeparator }, StringSplitOptions.None);

                    this.CurrentScriptNumber++;

                    this.TotalScriptBlock = scriptSplited.Length;
                    this.CurrentBlockScript = 0;


                    if (script.Type == null || script.Type.ToUpper() == "SQL")
                    {
                        int currentLine = 1;
                        foreach (string scriptBlock in scriptSplited)
                        {
                            this.CurrentBlockScript++;
                            this.CurrentBlockStr = scriptBlock;
                            if (hasContent(scriptBlock))
                            {

                                blockLog = new RdxScriptLog();
                                blockLog.ScriptId = currentLine.ToString();
                                blockLog.StartTime = DateTime.Now;

                                try
                                {
                                    string command = scriptBlock;
                                    if (script.Version != 0) command = command.Replace("[W_GnVER]", script.Version.ToString());

                                    if (script.Connection == ConnectionType.Admin && param.AdmConn != null)                                        
                                        ExecuteCommand(param.AdmConn, command, script.Timeout);
                                    else if (script.Connection == ConnectionType.Log && param.LogConn != null)
                                        ExecuteCommand(param.LogConn, command, script.Timeout);
                                    else ExecuteCommand(param.Conn, command, script.Timeout);
                                }
                                catch (Exception e)
                                {
                                    execError = true;
                                    blockLog.Result = e.Message;
                                    scriptLog.Result = e.Message;

                                    if (!param.ContinueAfterError)
                                    {
                                        scriptLog.Duration = (int)(DateTime.Now - scriptLog.StartTime).TotalMilliseconds;
                                        log.ScriptDetails.Add(scriptLog);
                                        throw e;
                                    }
                                }
                                finally
                                {
                                    blockLog.Duration = (int)(DateTime.Now - blockLog.StartTime).TotalMilliseconds;
                                    scriptLog.ScriptDetails.Add(blockLog);
                                }
                            }

                            currentLine += CountLines(scriptBlock) + 1;
                        }
                    }
                    else
                    {
                        this.CurrentBlockScript++;
                        this.CurrentBlockStr = script.ScriptName;

                        blockLog = new RdxScriptLog();
                        blockLog.ScriptId = (script.Version != 0) ? script.Version.ToString() : rdxNum.ToString();
                        blockLog.StartTime = DateTime.Now;

                        try
                        {
                            script.Execute.Invoke(cToken);
                        }
                        catch (Exception e)
                        {
                            execError = true;
                            blockLog.Result = e.Message;
                            scriptLog.Result = e.Message;

                            if (!param.ContinueAfterError)
                            {
                                scriptLog.Duration = (int)(DateTime.Now - scriptLog.StartTime).TotalMilliseconds;
                                log.ScriptDetails.Add(scriptLog);
                                throw e;
                            }
                        }
                        finally
                        {
                            blockLog.Duration = (int)(DateTime.Now - blockLog.StartTime).TotalMilliseconds;
                            scriptLog.ScriptDetails.Add(blockLog);
                        }
                    }

                    //If this is the upgrade client routine we need to update the upgrindx
                    //With the script version that we just ran
                    if((script.ScriptName == "UpgradeClient.sql" || script.ScriptName == "UpgradeClient") && !execError)
                    {
                        ExecuteCommand(param.Conn,
                            @"UPDATE " + CSGenio.framework.Configuration.Program + "cfg SET upgrindx = " + script.Version.ToString());              
                    }

                    scriptLog.Duration = (int)(DateTime.Now - scriptLog.StartTime).TotalMilliseconds;
                    log.ScriptDetails.Add(scriptLog);  
                }

                /*
                 If the databaes doesn't exist at the start of the reindexation, we won't run the version migration
                 scripts but we still want to update the version to the last so that they don't run unnecessarly in 
                 the future.
                 */
                if(!dbExists)
                {
                    string tableName = CSGenio.framework.Configuration.Program + "cfg";
                    // Check if the CFG table exists first since there is a chance that may not be the case
                    IDataReader reader = ExecuteCommand(param.Conn, 
                        @"SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES
                        WHERE TABLE_NAME = '" + tableName + "' "/* +
                        "AND TABLE_CATALOG = '" + param.Conn.Database + "'"*/, null, true);

                    if(reader.Read())
                    {
                        bool tableExists = reader.GetInt32(0) > 0;

                        // We need to get rid of these before doing new query
                        reader.Close();
                        reader.Dispose();

                        if (tableExists)
                        {
                            ExecuteCommand(param.Conn,
                                @"UPDATE " + tableName + " SET upgrindx = " + CSGenio.framework.Configuration.VersionUpgrIndxGen);
                        }
                    }
                }

                //Send finished status
                RdxStatus statusFinish = new RdxStatus(rdxNum, script?.ScriptName ?? "", scripts?.Count ?? 0, 
                    null, RdxProgressStatus.SUCCESS);
                param.OnChangedExecuteServer(EventArgs.Empty, statusFinish);
            }
            catch (OperationCanceledException e) 
            {
                status.State = RdxProgressStatus.CANCELLED;
                param.OnChangedExecuteServer(EventArgs.Empty, status);
                throw e; 
            }
            catch(Exception e)
            {
                RdxStatus eStatus = new RdxStatus(rdxNum, script?.ScriptName ?? "", scripts?.Count ?? 0,
                    e.Message, RdxProgressStatus.ERROR);
                param.OnChangedExecuteServer(EventArgs.Empty, eStatus);
            }
            finally
            {
                log.Duration = (int)(DateTime.Now - log.StartTime).TotalMilliseconds;
                log.appendXML(this.m_logFile);

                if (this.OnComplete != null)
                    this.OnComplete(this, new ReindexProgressEventArgs(this.CurrentScriptName,
                                                                       this.TotalScripts,
                                                                       this.CurrentScriptNumber,
                                                                       this.TotalScriptBlock,
                                                                       this.CurrentBlockScript));
            }
        }

        private IDataReader ExecuteCommand(IDbConnection conn, string command, int? timeout = null, bool isSelectQuery = false)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = command;
                if(timeout != null) cmd.CommandTimeout = (int)timeout;

                if(!isSelectQuery) cmd.ExecuteNonQuery();
                else return cmd.ExecuteReader();
            }

            return null;
        }

        private int CountLines(string str)
        {
            if(str == null || str.Length == 0)
                return 0;
            int count = 0;
            for (int i = 0; i < str.Length; i++)
                if (str[i] == '\n')
                    count++;
            return count;
        }
    }

    public static class SQL
    {

        /// <sumary>constrói a connection string</sumary>
        /// <param name="server">Servidor</param>
        /// <param name="service">Serviço</param>
        /// <param name="user">Username</param>
        /// <param name="pass">Password</param>
        /// <param name="sysdba">SysDBA</param>
        public static string GetConnectionString(string server, string service, string user, string pass, bool sysdba)
        {
            return "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + server + ")" +
                "(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=" + service + ")));User Id=" + user + ";Password=" + pass + ";" + ((sysdba) ? "DBA Privilege=SYSDBA;" : "");
        }

        /// <sumary>constrói a connection string</sumary>
        /// <param name="server">Servidor</param>
        /// <param name="initialCatalog">Inicial Catalog</param>
        /// <param name="user">Username</param>
        /// <param name="pass">Password</param>
        public static string GetConnectionString(string server, string initialCatalog, string user, string pass)
        {
            return "Data Source=" + server + ";Initial Catalog=" + initialCatalog + ";User Id=" + user + ";Password=" + pass + ";";
        }

        /// <sumary>devolve query para login</sumary>
        /// <param name="server">Servidor</param>
        /// <param name="user">Username</param>
        /// <param name="pass">Password</param>
        public static string GetLoginQuery(string server, string user, string pass)
        {
            return "data source=" + server + ";user id=" + user + "; password=" + pass; // "ZPH2LAB";
        }



        /// <sumary>devolve query para login</sumary>
        /// <param name="server">Servidor</param>
        /// <param name="user">Username</param>
        /// <param name="pass">Password</param>
        public static string GetDBListQuery()
        {
            return "select name from master.dbo.sysdatabases";
        }

        public static List<string> DBList(string conexao)
        {
            List<string> lista = new List<string>();
            try
            {
                SqlConnection servidor = new SqlConnection(conexao);
                servidor.Open();

                string sql = SQL.GetDBListQuery();
                SqlCommand comm = new SqlCommand(sql, servidor);
                SqlDataReader reader = comm.ExecuteReader();

                //cmbBD.Items.Clear();
                //cmbBD.BeginUpdate();
                while (reader.Read())
                    lista.Add(reader.GetString(0));

                reader.Close();
                servidor.Close();
            }
            catch (Exception)
            {
            }
            return lista;
        }

        public static bool TryConnection(string query)
        {
            try
            {
                //string conexao = "";
                //conexao = SQL.GetLoginQuery(server, username, password);
                SqlConnection teste = new SqlConnection(query);
                teste.Open();
                teste.Close();
                //MessageBox.Show("Login com sucesso.");

                //LerBDs(conexao);
                //txtPass.Enabled = false;
                //txtUser.Enabled = false;
                //txtServer.Enabled = false;
                //btnLogin.Enabled = false;
                //btnCarregar.Enabled = true;
                return true;
            }
            catch (Exception)
            {

                return false;
                //MessageBox.Show(ex.Message, "Não foi possível estabelecer a conexão.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }


    public static class MySqlCore
    {
        /// <sumary>constrói a connection string</sumary>
        /// <param name="server">Servidor</param>
        /// <param name="initialCatalog">Inicial Catalog</param>
        /// <param name="user">Username</param>
        /// <param name="pass">Password</param>
        public static string GetConnectionString(string server, string initialCatalog, string user, string pass)
        {
            //Nota: O Allow User Variables serve para permitir variaveis declaradas com @ no meio dos scripts de mysql
            return "Server=" + server + ";Uid='" + user + "';Pwd='" + pass + "';Database='" +initialCatalog+ "';Allow User Variables=True;";
        }

        /// <sumary>devolve connection para login</sumary>
        /// <param name="server">Servidor</param>
        /// <param name="user">Username</param>
        /// <param name="pass">Password</param>
        public static string GetLogin(string server, string user, string pass)
        {
            return "Server=" + server + ";Uid='" + user + "';Pwd='" + pass + "';";
        }

        /// <summary>
        /// Lista as bases de dados do servidor
        /// </summary>
        /// <param name="conn">Connection string</param>
        /// <returns>bases de dados do servidor</returns>
        public static List<string> GetDatabases(IDbConnection conn)
        {
            List<string> res = new List<string>();

            IDbCommand command = conn.CreateCommand();
            command.CommandText = "SHOW DATABASES;";

            IDataReader Reader;
            Reader = command.ExecuteReader();
            while (Reader.Read())
                res.Add(Reader.GetString(0));

            return res;
        }
    }


    public static class ORACLE
    {
        public static string GetConnectionString(string server, int port, string service, string user, string pass)
        {
            string conexao = "Data Source=(DESCRIPTION=(ADDRESS_LIST=";
            conexao += "(ADDRESS=(PROTOCOL=TCP)(HOST=" + server + ")(PORT=" + port + ")))";
            conexao += "(CONNECT_DATA=(SID=" + service + ")));";
            conexao += "User Id=" + user + ";Password=" + pass + ";";
            return conexao;
        }

        public static bool TryConnection(string query)
        {
            try
            {
#pragma warning disable 618
                OracleConnection testConn = new OracleConnection(query);
#pragma warning restore 618
                testConn.Open();
                testConn.Close();
                return false;
                //Connection = new OracleConnection(conf.Conexao);

            }
            catch (Exception)
            {

                return false;
                //MessageBox.Show(ex.Message, "Não foi possível estabelecer a conexão.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }

    public class ReIndexFunction
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Selected")]
        public bool Selected { get; set; }
        
        [XmlAttribute("Selectable")]
        public bool Selectable { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Action<CancellationToken> Execute { get; set; }

        [XmlAttribute("Help")]
        public string Help { get; set; } //description of the item

        [XmlAttribute("Connection")]
        public ConnectionType Connection { get; set; }
        
        [XmlAttribute("TimeoutMult")]
        public int TimeoutMult { get; set; }

        [XmlArray("Dependencies")]
        [XmlArrayItem("Dependencie")]
        public List<string> Dependencies { get; set; }

        [XmlArray("Scripts")]
        [XmlArrayItem("Script")]
        public List<ReIndexFile> Scripts { get; set; }
    }

    /**
     * Class that structures information of a ReindexFunction that was run during maintenance (based on its RdxOperationLog).
     */
    public class ReindexFunctionItem
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public bool Value { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public Action Callback { get; set; }
        public DateTime LastRun { get; set; }
        public int Duration { get; set; }
        public string Origin { get; set; }
        public string Result { get; set; }
        public bool Selectable { get; set; }
        public List<RdxScriptLog> Details { get; set; }

        public void Load(ReIndexFunction function, RdxOperationLog log = null)
        {
            Description = function.Name;
            Id = function.Id;
            Value = function.Selected;
            Duration = 0;
            Result = "";
            LastRun = DateTime.MinValue;
            Origin = log != null ? log.Origin : "";
            Details = new List<RdxScriptLog>();
            Selectable = function.Selectable;

            if (log != null)
            {
                foreach (var script in function.Scripts)
                {
                    var l = log.ScriptDetails.Find(x => x.ScriptId == script.Name);
                    if (l != null)
                    {
                        if (LastRun == DateTime.MinValue)
                            LastRun = l.StartTime;
                        Duration += l.Duration;
                        Result += l.Result ?? "";
                        Details.Add(l);
                    }
                }
            }
        }
    }

    public class ReIndexFile
    {
        [XmlText]
        public string Name { get; set; }

        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlAttribute("Version")]
        public int Version { get; set; }

        [XmlAttribute("MinDbVersion")]
        public string MinDbVersion { get; set; } //Minimum database version

        [XmlAttribute("MaxDbVersion")]
        public string MaxDbVersion { get; set; } //Maximum database version
    }

    public class ReIndexGroup
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlArray("GroupItems")]
        [XmlArrayItem("ReIndexItem")]
        public List<string> GroupItems { get; set; }
    }

    public class ReindexOrder
    {
        private List<ReIndexGroup> reindexgroups { get; set; }
        private List<ReIndexFunction> reIndexItems { get; set; }
        private int selectFunctions { get; set; }
        
        [XmlArray("ReIndexItems")]
        [XmlArrayItem("Function")]
        public List<ReIndexFunction> ReIndexItems
        {
            get { return reIndexItems; }
            set { reIndexItems = value; }
        }

        [XmlArray("ReIndexGroups")]
        [XmlArrayItem("ReIndexGroup")]
        public List<ReIndexGroup> Reindexgroups
        {
            get { return reindexgroups; }
            set { reindexgroups = value; }
        }

        [XmlIgnore]
        public int SelectFunctions
        {
            get { return selectFunctions; }
            private set { selectFunctions = value; }
        }

        [XmlIgnore]
        public int timeout { get; set; }

        public static ReindexOrder readXML(string filename)
        {
            try
            {
                using(System.IO.StreamReader input = new System.IO.StreamReader(filename))
                {
                    System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(ReindexOrder));
                    ReindexOrder ReindexOrderXML = (ReindexOrder)s.Deserialize(input);
                    ReindexOrderXML.timeout = 300; //TODO: READ FROM XML
                    return ReindexOrderXML;
                }
            }
            catch (Exception)
            {
                return new ReindexOrder();
            }
        }

        public void CalculateOrder()
        {
            List<ReIndexFunction> SelectedFunctions = this.ReIndexItems.FindAll(delegate(ReIndexFunction RdxFunction)
            { return RdxFunction.Selected && RdxFunction.Dependencies.Count > 0; });

            Action<string> CheckDependencies = null;
            CheckDependencies = (@d) =>
            {
                ReIndexFunction ridxFunc = this.ReIndexItems.Find(delegate(ReIndexFunction RdxFunction) { return RdxFunction.Id == @d; });
                ridxFunc.Selected = true;

                foreach (string depend in ridxFunc.Dependencies)
                {
                    if (!this.ReIndexItems.Find(delegate(ReIndexFunction RdxFunction) { return RdxFunction.Id == @d; }).Selected)
                        CheckDependencies(depend);
                }
            };

            foreach (var func in SelectedFunctions)
                foreach (string dependencie in func.Dependencies)
                    CheckDependencies(dependencie);
        }

        public List<RdxScript> GetOrderToExecute()
        {
            List<RdxScript> order2exec = new List<RdxScript>();

            foreach (var ReIndexItem in this.ReIndexItems.FindAll(delegate(ReIndexFunction RdxFunction) { return RdxFunction.Selected; }))
            {
                int fileTimeout = timeout;
                if (ReIndexItem.TimeoutMult>1)
                {
                    fileTimeout = timeout * ReIndexItem.TimeoutMult;
                }

                foreach (var ScriptFile in ReIndexItem.Scripts.OrderBy(e => e.Version))
                    order2exec.Add(new RdxScript { Script = ScriptFile.Name, Connection = ReIndexItem.Connection, Timeout = fileTimeout, Type = ScriptFile.Type, MinDbVersion = ScriptFile.MinDbVersion, MaxDbVersion = ScriptFile.MaxDbVersion, Execute = ReIndexItem.Execute, Version = ScriptFile.Version }); //TODO: add multpliier for complex scripts
            }

            return order2exec;
        }
    }
}
