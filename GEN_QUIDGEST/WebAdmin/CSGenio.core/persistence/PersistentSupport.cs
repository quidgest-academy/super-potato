using CSGenio.business;
using CSGenio.core.di;
using CSGenio.core.messaging;
using CSGenio.framework;
using CSGenio.framework.Geography;
using ExecuteQueryCore;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using GenericSortOrder = Quidgest.Persistence.GenericQuery.SortOrder;

namespace CSGenio.persistence
{
    /// <summary>
    /// Abstract class for the implementation of a generic database persistent support.
    /// </summary>
    public abstract class PersistentSupport : IPersistentSupport
    {
        protected bool m_QueueMode = false;

        public bool QueueMode
        {
            get { return m_QueueMode; }
            set { m_QueueMode = value; }
        }

        public class ControlQueryDefinition
        {
            public IList<SelectField> SelectFields
            {
                get;
                private set;
            }

            public ITableSource FromTable
            {
                get;
                private set;
            }

            public IList<TableJoin> Joins
            {
                get;
                private set;
            }

            public CriteriaSet WhereConditions
            {
                get;
                private set;
            }

            public bool Distinct
            {
                get;
                private set;
            }

            public ControlQueryDefinition(
                IList<SelectField> selectFields, ITableSource fromTable, IList<TableJoin> joins, CriteriaSet whereConditions)
                : this (selectFields, fromTable, joins, whereConditions, false)
            {
            }

            public ControlQueryDefinition(
                IList<SelectField> selectFields, ITableSource fromTable, IList<TableJoin> joins, CriteriaSet whereConditions, bool distinct)
            {
                SelectFields = selectFields;
                FromTable = fromTable;
                Joins = joins;
                WhereConditions = whereConditions;
                Distinct = distinct;
            }

            public SelectQuery ToSelectQuery()
            {
                SelectQuery result = new SelectQuery();

                if (SelectFields != null)
                    foreach (SelectField f in SelectFields)
                        result.SelectFields.Add(f);

                result.FromTable = FromTable;

                if (Joins != null)
                    foreach (TableJoin j in Joins)
                        result.Joins.Add(j);

                result.WhereCondition = WhereConditions.Clone() as CriteriaSet;

                result.Distinct(Distinct);

                return result;
            }
        }

        /// <summary>
        /// Check sp Timeout
        /// </summary>
        public virtual int Timeout { get; set; }

        public virtual IDbConnection Connection { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the connection is closed.
        /// </summary>
        public virtual bool ConnectionIsClosed { get { return Connection?.State == ConnectionState.Closed; } }

        public virtual IDbTransaction Transaction { get; protected set; }

        /// <summary>
        /// Check if Transaction is null (closed)
        /// </summary>
        public virtual bool TransactionIsClosed { get { return Transaction == null; } }

        public virtual DatabaseType DatabaseType { get; protected set;}

        protected static IDictionary<string, ControlQueryDefinition> controlQueries;
        protected static Dictionary<int, PersistentSupport> ligacoes;
        public delegate SelectQuery overrideDbeditQuery(User user, string module, CriteriaSet conditions, IList<ColumnSort> orderBy, PersistentSupport sp);
        protected static IDictionary<string, overrideDbeditQuery> controlQueriesOverride;
        protected static Hashtable manualQueries;
		protected static Hashtable notifications;

		protected static List<CSGenioAnotificationemailsignature> emailSignatures;

        public virtual QuerySchemaMapping SchemaMapping { get; protected set;} = new QuerySchemaMapping();

        /// <summary>
        /// Sql syntax dialect for the current provider
        /// </summary>
        /// <returns>The syntax dialect</returns>
        public virtual Dialect Dialect { get; protected set; }

        /// <summary>
        /// Dataset Id that created this persistent support
        /// </summary>
        public string Id { get; protected set; }
        /// <summary>
        /// logged client from "frontend" app
        /// </summary>
		public string ClientId { get; protected set; }
        /// <summary>
        /// Only read only commands are allows to this persistence store
        /// </summary>
        public virtual bool ReadOnly { get; protected set; }
        /// <summary>
        /// If this connection is to be established as a master connection
        /// </summary>
        public virtual bool IsMaster { get; protected set; }

        /// <summary>
        /// Enable for support of database side primary key allocation during inserts.
        /// Disable for the application to generate or persist primary key sequences in separate requests.
        /// </summary>
        public bool DatabaseSidePk { get; protected set; } = false;

        /**
         * Static Constructor
         * - The hashtable controls are keyed to the identifier of the loaded menu option and
         * as the corresponding querie object. These queries are generated by GENIO.
         * - connectionpool objects are filled with
         * data read from the web.xml to the Configuration object.
         */
        static PersistentSupport()
        {
            //queries generated by GENIO
            InitControlQueries();
            InitManualQueries();
            ligacoes = new Dictionary<int, PersistentSupport>();
			InitNotifications();

        }

        private static void InitManualQueries()
        {
            
        }

        public static Hashtable getManualQueries()
        {
            return manualQueries;
        }

		private static void InitNotifications()
        {
            
        }

        public static Hashtable getNotifications()
        {
            return notifications;
        }

		private void InitEmailSignatures(string year)
        {
            PersistentSupport sp = PersistentSupport.getPersistentSupport(year);

            String SignaturesArea = "notificationemailsignature";
            User user = new User("Q_NOTIFS", "", Configuration.DefaultYear)
            {
                CurrentModule = "NOT"
            };
            user.AddModuleRole(user.CurrentModule, Role.ADMINISTRATION);
            emailSignatures = CSGenioAnotificationemailsignature.searchList(sp, user, CriteriaSet.And().Equal(new Quidgest.Persistence.FieldRef(SignaturesArea, "zzstate"), 0));

        }

        public List<CSGenioAnotificationemailsignature> getEmailSignatures(string year)
        {
            InitEmailSignatures(year);
            return emailSignatures;
        }


        /// <summary>
        /// MH - Initialization of elements that cannot be initialized in CSGenio.core due to the use of GlobalFunctions and CSGenioA
        /// </summary>
        /// <param name="in_controlQueries"></param>
        /// <param name="in_controlQueriesOverride"></param>
        public static void SetControlQueries(IDictionary<string, ControlQueryDefinition> in_controlQueries, IDictionary<string, overrideDbeditQuery> in_controlQueriesOverride)
        {
            controlQueries = in_controlQueries;
            controlQueriesOverride = in_controlQueriesOverride;
        }

        private static void InitControlQueries()
        {
            controlQueries = new ControlQueryDictionary();
            controlQueriesOverride = new Dictionary<string, overrideDbeditQuery>();
/*

*/
        }

        /// <summary>
        /// Gets the select command from the SelectQuery,
        /// so that a DataReader can be created from that select command
        /// </summary>
        /// <returns>IDbCommand created from the SelectQuery</returns>
        /// <remarks>
        /// Created by [CJP] at [2016.07.04]
        /// </remarks>
        public IDbCommand GetSelectCommand(SelectQuery query)
        {
            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;

            var sql = renderer.GetSql(query);
            var parameters = renderer.ParameterList;

            IDbDataAdapter adapter = CreateAdapter(sql);

            AddParameters(adapter.SelectCommand, parameters);

            return adapter.SelectCommand;
        }


        /// <summary>
        /// Executes the query in the persistent support
        /// </summary>
        /// <returns>the <see cref="CSGenio.persistence.DataMatrix"/> With the results</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public DataMatrix Execute(SelectQuery query)
        {
            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;

            var sql = renderer.GetSql(query);
			long st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryExecuteSelect] {0}.", sql) + Environment.NewLine + renderer.PrintParameters());

            var parameters = renderer.ParameterList;

            IDbDataAdapter adapter = CreateAdapter(sql);

            AddParameters(adapter.SelectCommand, parameters);

            DataSet ds = new DataSet();
            adapter.Fill(ds);

			if (Log.IsDebugEnabled) Log.Debug("[QueryExecuteSelect] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

            return new DataMatrix(ds);
        }

        /// <summary>
        /// Executes the query in the persistent support
        /// </summary>
        /// <returns>The query result</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public object Execute(InsertQuery query)
        {
            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;

            var sql = renderer.GetSql(query);
			long st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryExecuteInsert] {0}.", sql) + Environment.NewLine + renderer.PrintParameters());
            var parameters = renderer.ParameterList;

            IDbCommand command = CreateCommand(sql, parameters);

            object result = command.ExecuteScalar();
			if (Log.IsDebugEnabled) Log.Debug("[QueryExecuteInsert] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
			return result;
        }

        /// <summary>
        /// Executes the query in the persistent support
        /// </summary>
        /// <returns>The query result</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public int Execute(DeleteQuery query)
        {
            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;

            var sql = renderer.GetSql(query);
			long st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryExecuteDelete] {0}.", sql) + Environment.NewLine + renderer.PrintParameters());
            var parameters = renderer.ParameterList;

            IDbCommand command = CreateCommand(sql, parameters);

            int result = command.ExecuteNonQuery();
			if (Log.IsDebugEnabled) Log.Debug("[QueryExecuteDelete] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
			return result;
        }

        /// <summary>
        /// Executes the query in the persistent support
        /// </summary>
        /// <returns>The query result</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public int Execute(UpdateQuery query)
        {
            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;

            var sql = renderer.GetSql(query);
			long st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryExecuteUpdate] {0}.", sql) + Environment.NewLine + renderer.PrintParameters());
            var parameters = renderer.ParameterList;

            IDbCommand command = CreateCommand(sql, parameters);

            int result = command.ExecuteNonQuery();
			if (Log.IsDebugEnabled) Log.Debug("[QueryExecuteUpdate] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
			return result;
        }


        /// <summary>
        /// Adds a control to the controls hastable without repeating IDs.
        /// </summary>
        /// <param name="id">Control ID</param>
        /// <param name="controlo">Query</param>
        private static void adicionaControlo(string id, ControlQueryDefinition controlo)
        {
            if (!controlQueries.ContainsKey(id))
            {
                controlQueries.Add(id, controlo);
            }
        }

        /// <summary>
        /// Check the database service for connectivity
        /// </summary>
        /// <param name="id">The datasystem to connect to</param>
        /// <returns>True if a connection is available, false otherwise</returns>
        public static bool TestDBConnection(string id)
        {
            try
            {
                var sp = getPersistentSupport(id, timeout: 1);
                sp.openConnection();
                sp.closeConnection();
                return true;
            }
            catch (Exception)
            {
                // Ignorar
            }
            return false;
        }

        /// <summary>
        /// Check the database service for connectivity
        /// </summary>
        /// <param name="dataSystem">The datasystem xml to connect to</param>
        /// <returns>True if a connection is available, false otherwise</returns>
        public static bool TestServerConnection(DataSystemXml dataSystem)
        {
            try
            {
                var sp = GenioDI.SpFactory(dataSystem.GetDatabaseType());
                sp.DatabaseType = dataSystem.GetDatabaseType();
                sp.Id = dataSystem.Name;
                sp.BuildConnection(dataSystem);
                sp.openConnection();
                sp.closeConnection();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error when testing the connection");
                return false;
            }
        }

        /// <summary>
        /// Gets the default backups location.
        /// </summary>
        /// <returns>A string representing the path to the default backups location.</returns>
        public static string GetDefaultBackupsLocation()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbs", "backup");
        }

        /// <summary>
        /// Creates a backup of the specified database schema for a given year.
        /// </summary>
        /// <param name="year">The year identifier for the database configuration.</param>
        /// <param name="username">The username for the database connection.</param>
        /// <param name="password">The password for the database connection.</param>
        /// <param name="location">Optional. The directory path where the backup file will be saved. 
        /// If not provided, the default backup location will be used.</param>
        /// <returns>The full path to the created backup file.</returns>
        /// <exception cref="PersistenceException">Thrown when there is an error during the backup process.</exception>
        public static string Backup(string year, string username, string password, string location = "")
        {
            try
            {
                DataSystemXml dataSystem = Configuration.ResolveDataSystem(year, Configuration.DbTypes.NORMAL);
                var sp = getPersistentSupportMaster(year, username, password);

                return sp.Backup(dataSystem.Schemas[0].Schema, location);
            }
            catch (FrameworkException ex)
            {
                if (ex.UserMessage == null)
                    throw new PersistenceException("Erro ao criar o backup.", "PersistentSupport.Backup", "Error while backing up the database: " + ex.Message, ex);
                else
                    throw new PersistenceException("Erro ao criar o backup: " + ex.UserMessage, "PersistentSupport.Backup", "Error while backing up the database: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Performs the backup operation for the specified database schema.
        /// </summary>
        /// <param name="schema">The name of the database schema to back up.</param>
        /// <param name="location">Optional. The directory path where the backup file will be saved. 
        /// If not provided, the default backup location will be used.</param>
        /// <returns>The full path to the created backup file.</returns>
        public virtual string Backup(string schema, string location = "")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Restores the database by the backup indicated in the path variable
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="username">User db</param>
        /// <param name="password">password</param>
        /// <param name="path">path to the backup of the db</param>
        public static void Restore(string year, string username, string password, string path)
        {
            try
            {
                DataSystemXml dataSystem = Configuration.ResolveDataSystem(year, Configuration.DbTypes.NORMAL);
                var sp = getPersistentSupportMaster(year, username, password);

                sp.Restore(dataSystem.Schemas[0].Schema, path);
            }
            catch (FrameworkException ex)
            {
				if (ex.UserMessage == null)
					throw new PersistenceException("Erro ao restaurar base de dados.", "PersistentSupport.Restore", "Error restoring the database: " + ex.Message, ex);
				else
					throw new PersistenceException("Erro ao restaurar base de dados: " + ex.UserMessage, "PersistentSupport.Restore", "Error restoring the database: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Restores a database from a specified backup file.
        /// </summary>
        /// <param name="schema">The name of the target database to be restored.</param>
        /// <param name="path">The full path to the backup file.</param>
        public virtual void Restore(string schema, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Path to the reindex profiling log
        /// </summary>
        public static string LogReindexPath(string path = "")
        {
            if (string.IsNullOrEmpty(path))
                path = AppDomain.CurrentDomain.BaseDirectory;

            return System.IO.Path.Combine(path, "temp", "logReindex.xml");
        }

        /// <summary>
        /// Database Reindex
        /// </summary>
        public static void upgradeSchema(string year, string username, string password, List<RdxScript> orderExec, string defPath = "", string confPath = "")
        {
            if(string.IsNullOrEmpty(defPath))
                defPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", Configuration.Program, "_ReIdx", "Reindex");

            upgradeSchema(year, username, password, orderExec, defPath, "", true);
        }

		/// <summary>
        /// Database Reindex
        /// </summary>
        public static void upgradeSchema(string year, string username, string password, List<RdxScript> orderExec, string path, string dirFilestream, bool zero)
        {
            RdxParamUpgradeSchema param = new RdxParamUpgradeSchema();
            param.Year = year;
            param.Username = username;
            param.Password = password;
            param.OrderExec = orderExec;
            param.Path = path;
            param.DirFilestream = dirFilestream;
            param.Zero = zero;
            param.Origin = "External";

            upgradeSchema(param);
        }

        /// <summary>
        /// Database Reindex
        /// </summary>
        public static void upgradeSchema(RdxParamUpgradeSchema param, CancellationToken cToken)
        {
            DataSystemXml dataSystem = Configuration.ResolveDataSystem(param.Year, Configuration.DbTypes.NORMAL);
            upgradeSchema(param, dataSystem, cToken);
        }

        /// <summary>
        /// Database Reindex
        /// </summary>
        public static void upgradeSchema(RdxParamUpgradeSchema param)
        {
            DataSystemXml dataSystem = Configuration.ResolveDataSystem(param.Year, Configuration.DbTypes.NORMAL);
            upgradeSchema(param, dataSystem, new CancellationToken());
        }

        /// <summary>
        /// Database Reindex
        /// </summary>
        public static void upgradeSchema(RdxParamUpgradeSchema param, DataSystemXml dataSystem, string auxSrcDBSchema = null)
        {
            upgradeSchema(param, dataSystem, new CancellationToken(), auxSrcDBSchema);
        }

        /// <summary>
        /// Database Reindex
        /// </summary>
        /// <param name="param">Reindex Upgrade Schema Parameters</param>
        /// <param name="dataSystem">Target DataSystem</param>
        /// <param name="auxSrcDBSchema">Aux Database schema (W_GnSrcBD) - Used in year change scripts.</param>
        public static void upgradeSchema(RdxParamUpgradeSchema param, DataSystemXml dataSystem, CancellationToken cToken, string auxSrcDBSchema = null)
        {
            if (param == null || dataSystem == null)
                return;
            string schema    = dataSystem.Schemas[0].Schema;
            string schemaLog = dataSystem.DataSystemLog != null && dataSystem.DataSystemLog.Schemas.Count != 0 ? dataSystem.DataSystemLog.Schemas[0].Schema : "";

            int year = 0;
            if (!int.TryParse(param.Year, out year))
                param.Year = "0";

            //configure the list of replaces to the scripts
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            result.Add(new KeyValuePair<string, string>("W_GnBD", schema));
            result.Add(new KeyValuePair<string, string>("W_GnTBS", schema));
            result.Add(new KeyValuePair<string, string>("W_GnIDX_TBS", schema));
            result.Add(new KeyValuePair<string, string>("W_GnAppSigla", CSGenio.framework.Configuration.Acronym));
            result.Add(new KeyValuePair<string, string>("W_GnReUse", "0"));
            result.Add(new KeyValuePair<string, string>("W_GnLogBD", schemaLog));

            result.Add(new KeyValuePair<string, string>("W_GnUser", param.Username));
            result.Add(new KeyValuePair<string, string>("W_GnPSW", param.Password));
            result.Add(new KeyValuePair<string, string>("W_PathFS", param.DirFilestream));
            result.Add(new KeyValuePair<string, string>("W_AppAno", param.Year));
            result.Add(new KeyValuePair<string, string>("W_GnAppAno", param.Year));
            result.Add(new KeyValuePair<string, string>("W_GnZeroTrue", param.Zero ? "1" : "0")); //this must be chosen by the user

            if(!string.IsNullOrEmpty(auxSrcDBSchema)) // Source BD used in Qyear change scripts
                result.Add(new KeyValuePair<string, string>("W_GnSrcBD", auxSrcDBSchema));

            string logfile = LogReindexPath();
            if(System.IO.File.Exists(logfile))
            {
                var fi = new System.IO.FileInfo(logfile);
                if (fi.Length > 10 * 1024 * 1024) //10 mb
                {
                    string logbackupfile = System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(logfile),
                        System.IO.Path.GetFileNameWithoutExtension(logfile) +
                        DateTime.Now.ToString("yyyyMMdd_HHmm") + "." +
                        System.IO.Path.GetExtension(logfile)
                    );
                    System.IO.File.Move(logfile, logbackupfile);
                }
            }
            var eq = new ExecuteQueryCore.ExecuteQueryWorker(param.Path, param.OrderExec, result, ExecuteQueryCore.NewLineFormat.Unix, logfile);

            try
            {
                PersistentSupport sp = getPersistentSupport(dataSystem.Name, param.Username, param.Password);
                using(IDbConnection conn = sp.Connection,
                    AdmConn = getPersistentSupportMaster(dataSystem.Name, param.Username, param.Password).Connection,
                    LogConn = getPersistentSupportLog(dataSystem.Name, param.Username, param.Password).Connection)
                {
                    RdxParamExecuteServer paramEx = new RdxParamExecuteServer();
                    paramEx.Conn = conn;
                    paramEx.AdmConn = AdmConn;
                    paramEx.LogConn = LogConn;
                    paramEx.ContinueAfterError = false;
                    paramEx.Origin = param.Origin;
                    paramEx.DataSystem = dataSystem.Name;

                    paramEx.ChangedExecuteServer += (sender, eventArgs, status) =>
                    {
                        param.OnChangedExecutionScript(EventArgs.Empty, status);
                    };

                    bool dbExists = sp.CheckIfDatabaseExists();
                    eq.ExecuteServer(paramEx, cToken, dbExists);
				}
            }
            catch (OperationCanceledException e) { throw e; }
			catch (GenioException ex)
			{
				if (ex.UserMessage == null)
					throw new PersistenceException("Erro ao atualizar o schema da base de dados.", "PersistentSupport.upgradeSchema", "Error upgrading database schema: " + ex.Message, ex);
				else
					throw new PersistenceException(ex.UserMessage, "PersistentSupport.upgradeSchema", "Error upgrading database schema: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                throw new PersistenceException("Erro ao atualizar o schema da base de dados.", "PersistentSupport.upgradeSchema", "Error upgrading database schema: " + ex.Message, ex);
            }
        }

		/// <summary>
        /// Transfer MSMQ log data from the system DB to the system log DB
        /// Called from log database PersistentSupport
        /// </summary>
        /// <param name="all">True to transfer all log data</param>
		public virtual void transferMSMQLog(bool all)
        {
			throw new NotImplementedException();
		}

        /// <summary>
        /// Transfer log data from the system DB to the system log DB
        /// Called from log database PersistentSupport
        /// </summary>
        /// <param name="all">True to transfer all log data</param>
        /// <param name="job">The transfer log job.</param>
        public virtual void transferLog(bool all, ExecuteQueryCore.TransferLogOperation job)
        {
            // Get system database PersistentSupport
            PersistentSupport systemSp = PersistentSupport.getPersistentSupport(this.SchemaMapping.Name);

            string table = "log" + Configuration.Program + "all";

            // Filter rows by date (specified in configuration file)
            CriteriaSet filter    = CriteriaSet.And();
            CriteriaSet filterMem = CriteriaSet.And();
            if (!all && Configuration.MaxLogRowDays > 0)
            {
                DateTime lastDate = DateTime.Today.AddDays(-Configuration.MaxLogRowDays);
                filter.LesserOrEqual(table, "date", lastDate);
                filterMem.LesserOrEqual(CSGenioAmem.FldAltura, lastDate);
            }

            try
            {
                // Open transactions
                systemSp.openTransaction();
                this.openTransaction();

                // ----------------------------------------------
                // LogGENall transfer
                // ----------------------------------------------

                // Row count
                SelectQuery query = new SelectQuery()
                    .Select(SqlFunctions.Count("1"), "count")
                    .From(table)
                    .Where(filter);

                DataMatrix values = systemSp.Execute(query);
                int count = values.GetInteger(0, 0);

                if (count <= 0)
                    throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupport.transferLog", "No logs to transfer.");

                int page = 1;

                // Insert rows into log database
                while (count > 0)
                {
                    query = new SelectQuery()
                        .Select(table, "cod")
                        .Select(table, "date")
                        .Select(table, "who")
                        .Select(table, "op")
                        .Select(table, "logtable")
                        .Select(table, "logfield")
                        .Select(table, "val")
                        .PageSize(10000)
                        .Page(page)
                        .From(table)
                        .Where(filter)
                        .OrderBy(2, GenericSortOrder.Ascending);

                    values = systemSp.Execute(query);

                    if (values.NumRows <= 0)
                        break;

                    for (int i = 0; i < values.NumRows; i++)
                    {
                        InsertQuery insert = new InsertQuery()
                            .Into(table)
                            .Value("cod",       values.GetString(i, 0))
                            .Value("date",      values.GetDate(i, 1))
                            .Value("who",       values.GetString(i, 2))
                            .Value("op",        values.GetString(i, 3))
                            .Value("logtable",  values.GetString(i, 4))
                            .Value("logfield",  values.GetString(i, 5))
                            .Value("val",       values.GetString(i, 6));

                        this.Execute(insert);
                    }

                    count -= values.NumRows;
                    page++;
                }

                // Delete rows from system database
                DeleteQuery delete = new DeleteQuery()
                    .Delete(table)
                    .Where(filter);

                systemSp.Execute(delete);

                // ----------------------------------------------
                // MEM transfer
                // ----------------------------------------------

                query = new SelectQuery()
                    .Select(SqlFunctions.Count("1"), "count")
                    .From(Area.AreaMEM)
                    .Where(filterMem);

                values = systemSp.Execute(query);
                count = values.GetInteger(0, 0);
                page = 1;

                // Insert rows into log database
                while (count > 0)
                {
                    query = new SelectQuery()
                        .Select(CSGenioAmem.FldCodmem)
                        .Select(CSGenioAmem.FldLogin)
                        .Select(CSGenioAmem.FldAltura)
                        .Select(CSGenioAmem.FldRotina)
                        .Select(CSGenioAmem.FldObs)
                        .Select(CSGenioAmem.FldHostid)
                        .Select(CSGenioAmem.FldZzstate)
                        .PageSize(10000)
                        .Page(page)
                        .From(Area.AreaMEM)
                        .Where(filterMem)
                        .OrderBy(CSGenioAmem.FldAltura, GenericSortOrder.Ascending);

                    values = systemSp.Execute(query);

                    if (values.NumRows <= 0)
                        break;

                    for (int i = 0; i < values.NumRows; i++)
                    {
                        InsertQuery insert = new InsertQuery()
                            .Into(Area.AreaMEM.Table)
                            .Value(CSGenioAmem.FldCodmem,   values.GetString(i, 0))
                            .Value(CSGenioAmem.FldLogin,    values.GetString(i, 1))
                            .Value(CSGenioAmem.FldAltura,   values.GetDate(i, 2))
                            .Value(CSGenioAmem.FldRotina,   values.GetString(i, 3))
                            .Value(CSGenioAmem.FldObs,      values.GetString(i, 4))
                            .Value(CSGenioAmem.FldHostid,   values.GetString(i, 5))
                            .Value(CSGenioAmem.FldZzstate,  values.GetString(i, 6));
                        this.Execute(insert);
                    }

                    count -= values.NumRows;
                    page++;
                }

                // Delete rows from system database
                delete = new DeleteQuery()
                    .Delete(Area.AreaMEM)
                    .Where(filterMem);

                systemSp.Execute(delete);

                // ----------------------------------------------

                // Close transactions
                systemSp.closeTransaction();
                this.closeTransaction();
            }
			catch (GenioException ex)
			{
				// Rollback transactions
                systemSp.rollbackTransaction();
                this.rollbackTransaction();
				if (ex.ExceptionSite == "PersistentSupport.transferLog")
					throw;
				if (ex.UserMessage == null)
					throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupport.transferLog", "Error transfering log data from the database to the log: " + ex.Message, ex);
				else
					throw new PersistenceException("Erro durante a transferência de logs: " + ex.UserMessage, "PersistentSupport.transferLog", "Error transfering log data from the database to the log: " + ex.Message, ex);
			}
            catch (Exception ex)
            {
                // Rollback transactions
                systemSp.rollbackTransaction();
                this.rollbackTransaction();
                throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupport.transferLog", "Error transfering log data from the database to the log: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Method to return the persistent support subclass depending on the type of connection
        /// </summary>
        /// <param name="id">Datasystem identificator</param>
        /// <param name="user">User name to use as audit</param>
        /// <param name="readOnly">is the connection only reading data and never writing</param>
        /// <param name="dbType">the category of datasystem being requested</param>
        /// <param name="timeout">set a specific timeout for database connection to be established</param>
        /// <returns>Returns an instance of PersistentSupport</returns>
        public static PersistentSupport getPersistentSupport(string id, string user = null, string password = null, bool readOnly = false, Configuration.DbTypes dbType = Configuration.DbTypes.NORMAL, int timeout = 0)
        {
            var ds = Configuration.ResolveDataSystem(id, dbType);
            var res = GenioDI.SpFactory(ds.GetDatabaseType());
            res.DatabaseSidePk = ds.DatabaseSidePk;
            if (res.DatabaseSidePk && !res.Dialect.SupportsOutput)
                throw new PersistenceException("Configuration is requesting database side primary keys, but this sql dialect does not support it.", "PersistentSupport.getPersistentSupport", "Configuration is requesting database side primary keys, but this sql dialect does not support it.");
            res.DatabaseType = ds.GetDatabaseType();
            res.Id = id;
            res.ClientId = user;
            res.ReadOnly = readOnly;
            res.MapSchemas(ds);
            res.BuildConnection(ds, user, password, connectionTimeout:timeout);
            return res;
        }

        /// <summary>
        /// Method to return the persistent support subclass depending on the type of connection
        /// </summary>
        /// <param name="id">The auxiliary database id</param>
        /// <returns>Returns an instantiation of PersistentSupport</returns>
        public static PersistentSupport getPersistentSupportAux(string id, string user = null, string password = null)
        {
            return getPersistentSupport(id, user, password, false, Configuration.DbTypes.AUXILIAR);
        }

        /// <summary>
        /// Method to return the persistent support subclass depending on the type of connection
        /// </summary>
        /// <param name="id">The auxiliary database id</param>
        /// <returns>Returns an instantiation of PersistentSupport</returns>
        public static PersistentSupport getPersistentSupportLog(string id, string user = null, string password = null)
        {
            return getPersistentSupport(id, user, password, false, Configuration.DbTypes.LOG);
        }

        /// <summary>
        /// Method to return a master level permission persistent support depending on the type of connection
        /// </summary>
        /// <param name="id">The id of the datasystem to connect to</param>
        /// <param name="login">The database user login</param>
        /// <param name="password">the database user password</param>
        /// <returns>Returns an instance of PersistentSupport</returns>
        public static PersistentSupport getPersistentSupportMaster(string id, string login, string password)
        {
            var ds = Configuration.ResolveDataSystem(id, Configuration.DbTypes.NORMAL);
            var res = GenioDI.SpFactory(ds.GetDatabaseType());
            res.DatabaseType = ds.GetDatabaseType();
            res.Id = id;
            res.ClientId = login;
            res.IsMaster = true;
            res.MapSchemas(ds);
            res.BuildConnection(ds, login, password);
            return res;
        }

        private void MapSchemas(DataSystemXml ds)
        {
            SchemaMapping.Name = ds.Name;
            foreach (DataXml schema in ds.Schemas)
                SchemaMapping.AddMapping(schema.Id.ToUpperInvariant(), TransformSchemaName(schema.Schema));
        }

        /// <summary>
        /// Build the connection to the database according to the instanced provider
        /// </summary>
        /// <param name="dataSystem">The datasystem metadata</param>
        /// <param name="login">An optional login override to the database. Null to use the datasystem specified login.</param>
        /// <param name="password">An optional password override to the database. Null to use the datasystem specified password.</param>
        /// <param name="connectionTimeout">The connection establishment timeout. 0 to use the database default</param>
        protected abstract void BuildConnection(DataSystemXml dataSystem, string login = null, string password = null, int connectionTimeout = 0);

        /// <summary>
        /// Takes a schema name and formats in a way that the instanced provider will accept.
        /// The schema represents the database qualified name.
        /// </summary>
        /// <param name="schema">the schema name</param>
        /// <returns>The transformed schema name</returns>
        protected virtual string TransformSchemaName(string schema)
        {
            return schema;
        }

        /// <summary>
        /// Function that opens a connection
        /// </summary>
        public virtual void openConnection()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Log.Debug("Abre a conexão à base de dados.");
                    Connection.Open();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Não foi possível estabelecer ligação à base de dados.", "PersistentSupport.openConnection", "Error opening connection: " + ex.Message, ex);
            }
        }


        /// <summary>
        /// Method to close a connection
        /// </summary>
        public virtual void closeConnection()
        {
            try
            {
                if (Connection.State != ConnectionState.Closed)
                {
				    SendDeferedQueues();
                    SendDeferredMessages();
                    Log.Debug("Fecha a conexão à base de dados.");
                    Connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Erro ao fechar a ligação à base de dados.", "PersistentSupport.closeConnection", "Error closing connection: " + ex.Message, ex);
            }
            finally
            {
                ClearDeferedQueues();
                ClearDeferredMessages();
            }
        }

        /// <summary>
        /// Opens a transaction
        /// </summary>
        public virtual void openTransaction()
        {
            try
            {
			    // [RC] 17/05/2017 - There is no need for this test here because the function openConnection already tests for opened connections
                //if (Connection.State != ConnectionState.Open) //check if the connection is open
                openConnection();
                if (Transaction == null)
                {
                    Log.Debug("Inicia a transacção à base de dados.");
                    Transaction = Connection.BeginTransaction();
                }
            }
            catch (Exception ex)
            {
                rollbackTransaction();
                throw new PersistenceException("Falha na ligação à base de dados.", "PersistentSupport.abrirTransaccao", "Error begining transaction: " + ex.Message, ex);
            }
        }


        /// <summary>
        /// Closes a transaction
        /// </summary>
        public virtual void closeTransaction()
        {
            try
            {
                if (Transaction != null)
                {
                    Log.Debug("commit da transacção à base de dados.");
                    SendDeferredMessages();
                    Transaction.Commit();
					SendDeferedQueues();
                    Transaction.Dispose();
                    Transaction = null;
					ClearDeferedQueues();
                    ClearDeferredMessages();
                    closeConnection();
                }
            }
            catch (Exception ex)
            {
                rollbackTransaction();
                throw new PersistenceException("Falha na ligação à base de dados.", "PersistentSupport.fecharTransaccao", "Error ending transaction: " + ex.Message, ex);
            }
			finally
            {
                ClearDeferedQueues();
                ClearDeferredMessages();
            }
        }

        /// <summary>
        /// Rollback a transaction
        /// </summary>
        public virtual void rollbackTransaction()
        {
            try
            {
                if (Transaction != null)
                {
                    Log.Debug("rollback da transacção à base de dados.");
                    Transaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                //Don't rethrow exception here, if the rollback fails there is nothing more we can do other than close the connection.
                //Throwing exception here leads to code with infinite chains of try catch to handle the rollback exception of the rollback exception of the....
                Log.Error("Error rolling back transaction: " + ex.Message);
            }
            finally
            {
				ClearDeferedQueues();
                ClearDeferredMessages();
                closeConnection();
				if (Transaction != null)
                {
					Transaction.Dispose();
					Transaction = null;
				}
            }
        }

		private List<IArea> m_deferedQueues = new List<IArea>();

        /// <summary>
        /// Defers the insert of the queue message in the database until the whole transaction is commited
        /// </summary>
        /// <param name="mqqueue">The message to insert in the queue</param>
        public void DeferQueueToCommit(Area mqqueue)
        {
            m_deferedQueues.Add(mqqueue);
        }

        /// <summary>
        /// Clears the list of defered queues
        /// </summary>
        public void ClearDeferedQueues()
        {
            m_deferedQueues.Clear();
        }

        /// <summary>
        /// Send all the defered queues sql commands to the database
        /// </summary>
        public void SendDeferedQueues()
        {
            foreach (var q in m_deferedQueues)
                insertPseud(q);
        }

        //---------------------------------------------------------
        private class DeferedMessageEntry
        {
            public PublisherMetadata pub;
            public AreaDataset dataset;
        }

        private List<DeferedMessageEntry> m_deferedMessages = new List<DeferedMessageEntry>();

        private AreaDatasetTable GetDeferedDatatable(PublisherMetadata pub, PublisherTable table)
        {
            //if there isnt a dataset for this publisher yet, create one
            var entry = m_deferedMessages.Find(x => x.pub == pub);
            if (entry == null)
            {
                entry = new DeferedMessageEntry
                {
                    pub = pub,
                    dataset = new AreaDataset()
                };
                m_deferedMessages.Add(entry);
            }

            //ensure the table is added to the dataset
            return entry.dataset.AddTable(table.Table);
        }

        /// <summary>
        /// Defers the update message to be send when the transaction is commited
        /// </summary>
        /// <param name="pub">The publication of the message</param>
        /// <param name="table">The table being updated</param>
        /// <param name="row">The row to update</param>
        public void DeferMessageUpdate(PublisherMetadata pub, PublisherTable table, Area row)
        {
            AreaDatasetTable dst = GetDeferedDatatable(pub, table);

            // if the row is already in the dataset rows update it   
            if (dst.Updated.ContainsKey(row.QPrimaryKey))
                dst.Updated[row.QPrimaryKey] = row;
            // if the row is already in the dataset rows add the row
            else
                dst.Updated.Add(row.QPrimaryKey, row);
        }

        /// <summary>
        /// Defers the delete message to be send when the transaction is commited
        /// </summary>
        /// <param name="pub">The publication of the message</param>
        /// <param name="table">The table being deleted</param>
        /// <param name="row">The row to delete</param>
        public void DeferMessageDelete(PublisherMetadata pub, PublisherTable table, Area row)
        {
            AreaDatasetTable dst = GetDeferedDatatable(pub, table);

            // if the row is already in the updated rows we need to remove it
            dst.Updated.Remove(row.QPrimaryKey);

            // add this primary key to the list of removed rows
            if(!dst.Deleted.Contains(row.QPrimaryKey))
                dst.Deleted.Add(row.QPrimaryKey);
        }

        private void SendDeferredMessages()
        {
            MessagingService messaging = GenioDI.Messaging;
            foreach (var entry in m_deferedMessages)
                messaging.SendMessage(entry.pub, entry.dataset, Id);
        }

        private void ClearDeferredMessages()
        {
            m_deferedMessages.Clear();
        }
        //---------------------------------------------------------

		/// <summary>
        /// Encapsulates a generic Action in a retryable transaction
        /// If the action throws a retryable exception, try to perform the action again up to a limit of n attempts
        /// </summary>
        /// <param name="a">The action to be taken</param>
        /// <param name="maxRetry">Maximum numer of retries</param>
        /// <example>
        /// sp.TransactionRetry(() => { model.Save(); });
        /// </example>
        public void TransactionRetry(Action a, int maxRetry=5)
        {
            int retry = 0;
            bool sucess = false;

            while (!sucess)
            {
                try
                {
                    openTransaction();
                    a();
                    closeTransaction();
                    sucess = true;
                }
                catch (Exception e)
                {
                    rollbackTransaction();
                    closeConnection();

                    //search in innerExceptions for an exception of the persistent media type that is Retryable
                    bool retryable = false;
                    Exception level = e;
                    while (level != null)
                    {
                        if(level is PersistenceException ep && ep.IsRetryable)
                        {
                            retryable = true;
                            break;
                        }
                        if(IsErrorTransient(level))
                        {
                            retryable = true;
                            break;
                        }
                        level = level.InnerException;
                    }

                    //retry a few times
                    if (retryable)
                    {
                        if (CSGenio.framework.Log.IsDebugEnabled) Log.Debug("RETRY from error" + (level == null ? "" : level.Message));
                        retry++;
                        if (retry >= maxRetry)
                            throw;
                        //transient failures can improve their probability of success if we wait a tiny bit before retrying
                        Thread.Sleep(50*retry);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Evaluates if a low level exception should be classified as transient when giving origin to a PersisteceException
        /// </summary>
        /// <param name="ex">The exception to evaluate</param>
        /// <returns>True if the error is considered transiente</returns>
        /// <remarks>
        /// Transient errors are those that have a chance of sucess if they are executed again with the same imputs.
        /// For example, a deadlock exception is a transiente error.
        /// </remarks>
        public abstract bool IsErrorTransient(Exception ex);


        /// <summary>
        /// Check if a record exists with a given field value
        /// </summary>
        /// <param name="Qfield">The field name</param>
        /// <param name="table">The table name</param>
        /// <param name="fieldValue">Field value</param>
        /// <returns>True if the record exits</returns>
        public bool Exists(string Qfield, string table, object fieldValue)
        {
            return Exists(Qfield, null, table, fieldValue);
        }

         /// <summary>
        /// Check if a record exists with a given field value
        /// </summary>
        /// <param name="Qfield">The field name</param>
        /// <param name="schema">The schema for this table</param>
        /// <param name="table">The table name</param>
        /// <param name="fieldValue">Field value</param>
        /// <returns>True if the record exits</returns>
        public bool Exists(string Qfield, string schema, string table, object fieldValue)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Check if record exists. [table] {0} [field] {1} [key] {2}", table, Qfield, fieldValue));

                SelectQuery query = new SelectQuery()
                    .Select(new SqlValue(1), "exists")
                    .From(schema, table, table)
                    .Where(CriteriaSet.And()
                        .Equal(table, Qfield, fieldValue))
                        .PageSize(1);

                DataMatrix mx = Execute(query);

                return mx != null && mx.NumRows > 0;
            }
            catch (PersistenceException ex)
            {
                if (ex.UserMessage == null)
                    throw new PersistenceException("O registo não foi encontrado.", "PersistentSupport.existe",
                        "Error trying to find record with value " + fieldValue + " in field " + Qfield + " in table " + table + ": " + ex.Message, ex);
                else
                    throw new PersistenceException("The record was not found: " + ex.UserMessage, "PersistentSupport.existe",
                        "Error trying to find record with value " + fieldValue + " in field " + Qfield + " in table " + table + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException("O registo não foi encontrado.", "PersistentSupport.existe",
                    "Error trying to find record with value " + fieldValue + " in field " + Qfield + " in table " + table + ": " + ex.Message, ex);
            }
        }


        /// <summary>
        ///Checks whether there is a record on the table with primary key equal to the given
        ///This function assumes the existence of an open connection.
        /// </summary>
        /// <param name="campo">name of the field that is the primary key of the table</param>
        /// <param name="tabela">from which we are checking whether the registration exists</param>
        /// <param name="valorCampo">primary key value</param>
        /// <param name="formatacao">if it is string, date, number,...</param>
        /// <returns>true if exists and false if it does not exist</returns>
        public bool exists(string[] fields, string table, object[] fieldsvalues)
        {
            return exists(fields, null, table, fieldsvalues);
        }

        private bool exists(string[] fields, string schema, string table, object[] fieldsvalues)
        {
            try
            {
              if (Log.IsDebugEnabled) Log.Debug(string.Format("Verifica se existe o registo. [tabela] {0} [campo] {1} [codigo] {2}", table, fields.ToString(), fieldsvalues.ToString()));

                SelectQuery query = new SelectQuery()
                    .Select(new SqlValue(1), "exists")
                    .From(table);
                CriteriaSet conditions = CriteriaSet.And();
                for (int i = 0; i < fields.Length; i++)
                {
                    conditions.Equal(table, fields[i], fieldsvalues[i]);
                }
                query.Where(conditions);

                DataMatrix mx = Execute(query);

                return mx != null && mx.NumRows > 0;
            }
            catch (PersistenceException ex)
            {
                if (ex.UserMessage == null)
					throw new PersistenceException("Os registos não foram encontrados.", "PersistentSupport.existe",
						"Error trying to find records in table " + table + ": " + ex.Message, ex);
				else
					throw new PersistenceException("Os registos não foram encontrados: " + ex.UserMessage, "PersistentSupport.existe",
						"Error trying to find records in table " + table + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Os registos não foram encontrados.", "PersistentSupport.existe",
					"Error trying to find records in table " + table + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Determines the value of the internal code of the records of a child table that have a
        /// relationship with parent table through relatedFields.
        /// This function assumes the existence of an open connection.
        /// </summary>
        /// <param name="camposRelacionados">establish the connection of the daughter table with the mother table</param>
        /// <param name="areaFilha">instance of the daughter area</param>
        /// <param name="valorCodigoMae">primary key value of the daughter table</param>
        /// <returns>Arraylist with the built-in codes of the daughter tables</returns>
        public ArrayList existsChild(string[] relatedFields, IArea areaChild, string parentCodeValue)
        {
          try
          {
            if (Log.IsDebugEnabled) Log.Debug(string.Format("Procura registos relacionados na tabela abaixo. [tabela] {0}", areaChild.TableName));

            ArrayList Qresult = new ArrayList();

            SelectQuery query = new SelectQuery()
                .Select(areaChild.Alias, areaChild.PrimaryKeyName)
                .From(areaChild.QSystem, areaChild.TableName, areaChild.Alias);

            CriteriaSet conditions = CriteriaSet.Or();
            for (int i = 0; i < relatedFields.Length; i++)
            {
                conditions.Equal(areaChild.Alias, relatedFields[i], parentCodeValue);
            }
            query.Where(conditions);

            Qresult = executeReaderOneColumn(query);

                return Qresult;
            }
            catch (PersistenceException ex)
            {
                //closeConnection();
                if (ex.UserMessage == null)
					throw new PersistenceException("Não foi possível encontrar os registos relacionados.", "PersistentSupport.existeFilha", "Error finding related records in child table: " + areaChild.TableName + ": " + ex.Message, ex);
				else
					throw new PersistenceException("Não foi possível encontrar os registos relacionados: " + ex.UserMessage, "PersistentSupport.existeFilha", "Error finding related records in child table: " + areaChild.TableName + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                //closeConnection();
                throw new PersistenceException("Não foi possível encontrar os registos relacionados.", "PersistentSupport.existeFilha", "Error finding related records in child table: " + areaChild.TableName + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        ///  Determines the value of the internal code of the records of a child table that have a
        ///  foreign key to the registration of the parent table.
        ///  This function assumes the existence of an open connection
        /// </summary>
        /// <param name="campoRelacionado">establishes the relationship between the mother and daughter table</param>
        /// <param name="codigoInternoFilha">primary key name of the daughter table</param>
		/// <param name="sistemaFilha">child table prefix schema</param>
        /// <param name="tabelaFilha">name of the daughter table</param>
		/// <param name="aliasFilha">alias of daughter table</param>
        /// <param name="valorCodigoMae">value of the primary key of the parent table</param>
        /// <returns>Arraylist with the built-in codes of the daughter tables</returns>
        [Obsolete]
        public ArrayList existsChild(string relatedField, string childInternalCode, string childSystem, string childTable, string aliasChild, object parentCodeValue)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Procura registo relacionado na tabela abaixo. [tabela] {0}", childTable));

                ArrayList Qresult = new ArrayList();
                //string query = queryExistsChild(relatedField, childInternalCode, childTable, aliasChild, parentCodeValue);
                //Qresult = executeReaderOneColumn(query);
                SelectQuery query = queryExistsChild(relatedField, childInternalCode, childSystem, childTable, aliasChild, parentCodeValue);
                Qresult = executeReaderOneColumn(query);
                return Qresult;
            }
            catch (PersistenceException ex)
            {
                closeConnection();
                if (ex.UserMessage == null)
					throw new PersistenceException("Não foi possível encontrar os registos relacionados.", "PersistentSupport.existeFilha", "Error finding related records in child area " + childTable + ": " + ex.Message, ex);
				else
					throw new PersistenceException("Não foi possível encontrar os registos relacionados: " + ex.UserMessage, "PersistentSupport.existeFilha", "Error finding related records in child area " + childTable + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                closeConnection();
                throw new PersistenceException("Não foi possível encontrar os registos relacionados.", "PersistentSupport.existeFilha", "Error finding related records in child area " + childTable + ": " + ex.Message, ex);
            }
        }

        public object returnField(IArea area, string Qfield, object codIntValue)
        {
            return returnField(area.QSystem, area.TableName, Qfield, area.PrimaryKeyName, codIntValue);
        }

        /// <summary>
        /// Returns a record field of a table that the internal code passed as a parameter
        /// Assumes an open connection
        /// </summary>
		/// <param name="schema">Prefix schema of the table to which the field belongs</param>
        /// <param name="tabela">Table to which the field belongs</param>
        /// <param name="campo">Field of the table to be returned</param>
        /// <param name="nomeCodInt">Name of the internal table code</param>
        /// <param name="valorCodInt">Value of the internal table code</param>
        /// <returns>returns the value of the field</returns>
        public object returnField(string schema, string table, string Qfield, string codIntName, object codIntValue)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Devolve o valor do campo. [tabela] {0} [campo] {1} [codigo] {2}", table, Qfield, codIntValue));

                SelectQuery query = new SelectQuery()
                    .Select(table, Qfield)
                    .From(schema, table, table)
                    .Where(CriteriaSet.And()
                        .Equal(table, codIntName, codIntValue));

                DataMatrix mx = Execute(query);
                if (mx == null || mx.NumRows < 1)
                {
                    return null;
                }

                return mx.GetDirect(0, 0);
            }
            catch (PersistenceException ex)
            {
                closeConnection();
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCampo", "Error returning field " + Qfield + " from table " + table + ": " + ex.Message, ex);

            }
            catch (Exception ex)
            {
                closeConnection();
                throw new PersistenceException(null, "PersistentSupport.devolveCampo", "Error returning field " + Qfield + " from table " + table + ": " + ex.Message, ex);
            }
        }


        public object returnFieldCondition(IArea area, string Qfield, CriteriaSet condition)
        {
            return returnFieldCondition(area.QSystem, area.TableName, Qfield, condition);
        }


        public object returnFieldCondition(string schema, string table, string Qfield, CriteriaSet condition)
        {
            try
            {
                string field = Qfield;
                if (field.IndexOf('.') != -1)
                {
                    field = field.Split('.')[1];
                }

                if (Log.IsDebugEnabled) Log.Debug(string.Format("Devolve o valor do campo. [tabela] {0} [campo] {1} [condicao] {2}", table, Qfield, condition));

                SelectQuery query = new SelectQuery()
                    .Select(table, field)
                    .From(schema, table, table)
                    .Where(condition);

                DataMatrix mx = Execute(query);
                if (mx == null || mx.NumRows < 1)
                {
                    return null;
                }

                return mx.GetDirect(0, 0);
            }
            catch (PersistenceException ex)
            {
                closeConnection();
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCampo", "Error returning field " + Qfield + " from table " + table + "where " + condition.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                closeConnection();
				throw new PersistenceException(null, "PersistentSupport.devolveCampo", "Error returning field " + Qfield + " from table " + table + "where " + condition.ToString() + ": " + ex.Message, ex);
            }
        }


        public ArrayList returnFieldsListConditions(string[] fieldsToGet, string table, string[] fieldsCondition, object[] fieldsvalues)
        {
            return returnFieldsListConditions(fieldsToGet, null, table, fieldsCondition, fieldsvalues);
        }

        public ArrayList returnFieldsListConditions(string[] fieldsToGet, string schema, string table, string[] fieldsCondition, object[] fieldsvalues)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Verifica se existe o registo. [tabela] {0} [campo] {1} [codigo] {2}", table, fieldsCondition.ToString(), fieldsvalues.ToString()));

                SelectQuery query = new SelectQuery();
                foreach (string field in fieldsToGet)
                {
                    query.Select(table, field);
                }
                query.From(schema, table, table);
                CriteriaSet criteria = CriteriaSet.And();
                for (int i = 0; i < fieldsCondition.Length; i++)
                {
                    criteria.Equal(table, fieldsCondition[i], fieldsvalues[i]);
                }
                query.Where(criteria);

                return executeReaderOneRow(query);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCamposListaCondicoes", "Error returning fields " + fieldsvalues.ToString() + " from table " + table + "where " + fieldsCondition.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.devolveCamposListaCondicoes", "Error returning fields " + fieldsvalues.ToString() + " from table " + table + "where " + fieldsCondition.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Returns a record field of a table that the internal code passed as a parameter
        /// Assumes an open connection
        /// </summary>
        /// <param name="area">Area to consult</param>
        /// <param name="listaCamposcampo">Fields of the table to be returned</param>
        /// <param name="nomeCodInt">Name of the internal table code</param>
        /// <param name="valorCodInt">Value of the internal table code</param>
        /// <returns>returns the value of the fields</returns>
        public ArrayList returnFields(IArea area, SelectField[] fieldsList, string codIntName, object codIntValue)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Devolve o valor dos campos. [tabela] {0} [campos] {1} [codigo] {2}", area.TableName, fieldsList, codIntValue));

                SelectQuery query = new SelectQuery();
                foreach (SelectField field in fieldsList)
                {
                    query.SelectFields.Add(field);
                }
                query.From(area.QSystem, area.TableName, area.TableName)
                    .Where(CriteriaSet.And()
                        .Equal(area.TableName, codIntName, codIntValue));

                return executeReaderOneRow(query);
            }
            catch (PersistenceException ex)
            {
                closeConnection();
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCampos", "Error returning fields " + fieldsList + " from table " + area.TableName + "where " + codIntName + "=" + codIntValue.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                closeConnection();
				throw new PersistenceException(null, "PersistentSupport.devolveCampos", "Error returning fields " + fieldsList + " from table " + area.TableName + "where " + codIntName + "=" + codIntValue.ToString() + ": " + ex.Message, ex);
            }
        }

        public DataMatrix returnValuesDocums(IArea area, string fieldName, SelectField[] Qvalues, CriteriaSet condition)
        {
            return returnValuesDocums(area, fieldName, Qvalues, condition, null);
        }

        public DataMatrix returnValuesDocums(IArea area, string fieldName, SelectField[] Qvalues, CriteriaSet condition, ColumnSort[] order)
        {
            return returnValuesDocums(area, fieldName, true, Qvalues, condition, order);
        }

        public DataMatrix returnValuesDocums(IArea area, string fieldName, bool isForeignKey, SelectField[] Qvalues, CriteriaSet condition, ColumnSort[] order)
        {
            DataMatrix res = null;
            string tabelaDocums = "docums";
            object primaryKeyValue = area.returnValueField(area.Alias + "." + area.PrimaryKeyName);
            object chaveDocums = null;

            string dbFieldName = fieldName;
            if (isForeignKey)
                dbFieldName += "fk";

            if (area.DBFields.ContainsKey(dbFieldName))
            {
                chaveDocums = area.returnValueField(area.Alias + "." + dbFieldName);
            }
            else
            {
                SelectQuery qs1 = new SelectQuery()
                    .Select(area.Alias, dbFieldName)
                    .From(area.QSystem, area.TableName, area.Alias)
                    .Where(CriteriaSet.And()
                        .Equal(area.Alias, area.PrimaryKeyName, primaryKeyValue));

                DataMatrix mx = Execute(qs1);
                if (mx != null && mx.NumRows > 0)
                {
                    chaveDocums = mx.GetDirect(0, 0);
                }
            }

            if (chaveDocums == null || chaveDocums == DBNull.Value || chaveDocums.Equals(""))
            {
                return res;
            }
            else
            {
                SelectQuery qs2 = new SelectQuery();
                foreach (SelectField field in Qvalues)
                {
                    qs2.SelectFields.Add(field);
                }
                qs2.From(tabelaDocums, "docums")
                    .Where(condition);
                if (order != null)
                {
                    foreach (ColumnSort sort in order)
                    {
                        qs2.OrderByFields.Add(sort);
                    }
                }

                res = Execute(qs2);

                return res;
            }
        }

        public object returnValueDocums(IArea area, string fieldName)
        {
            string tabelaDocums = "docums";
            object primaryKeyValue = area.returnValueField(area.Alias + "." + area.PrimaryKeyName);
            Object chaveDocums = null;

            if (area.Fields.ContainsKey(fieldName + "fk"))
            {
                chaveDocums = area.returnValueField(area.Alias + "." + fieldName + "fk");
            }
            else
            {

                SelectQuery qs1 = new SelectQuery()
                    .Select(area.Alias, fieldName + "fk")
                    .From(area.QSystem, area.TableName, area.Alias)
                    .Where(CriteriaSet.And()
                        .Equal(area.Alias, area.PrimaryKeyName, primaryKeyValue));

                DataMatrix mx = Execute(qs1);
                if (mx != null && mx.NumRows > 0)
                {
                    chaveDocums = mx.GetDirect(0, 0);
                }
            }

            if (chaveDocums == null || chaveDocums == DBNull.Value || chaveDocums.Equals(""))
            {
                return "";
            }
            else
            {
                SelectQuery qs2 = new SelectQuery()
                    .Select(tabelaDocums, "document")
                    .From(tabelaDocums)
                    .Where(CriteriaSet.And()
                        .Equal(tabelaDocums, "coddocums", chaveDocums));

                DataMatrix mx = Execute(qs2);

                if (mx == null || mx.NumRows < 1 || mx.GetDirect(0, 0) == null || mx.GetDirect(0, 0) == DBNull.Value)
                {
                    return "";
                }
                else
                {
                    return mx.GetDirect(0, 0);
                }
            }
        }

        /// <summary>
        /// Determines the value of the internal code of the records of a child table that
        /// have a relationship with the parent table through a related fields.
        /// This function assumes the existence of an open connection.
        /// </summary>
        /// <param name="camposRelacionado">establishes the connection of the daughter table with the mother table</param>
        /// <param name="codigoInternoFilha">name of the field that is primary key of the daughter table</param>
        /// <param name="tabelaFilha">name of the daughter table</param>
		/// <param name="aliasFilha">alias of daughter table</param>
        /// <param name="valorCodigoMae">primary key value of the daughter table</param>
        /// <returns>string corresponding to SQL question</returns>
        [Obsolete]
        public SelectQuery queryExistsChild(string relatedField, string childInternalCode, string childTable, string aliasChild, object parentCodeValue)
        {
            return queryExistsChild(relatedField, childInternalCode, null, childTable, aliasChild, parentCodeValue);
        }

        [Obsolete]
        public SelectQuery queryExistsChild(string relatedField, string childInternalCode, string childSystem, string childTable, string aliasChild, object parentCodeValue)
        {
            SelectQuery query = new SelectQuery()
                .Select(aliasChild, childInternalCode)
                .From(childSystem, childTable, aliasChild)
                .Where(CriteriaSet.And()
                    .Equal(aliasChild, relatedField, parentCodeValue));

            return query;
        }

        /// <summary>
        /// Method to return EPH values depending on user
        /// </summary>
        /// <param name="codpsw">internal unique user code</param>
        /// <param name="condition">EPH to fetch values for</param>
        /// <returns>list with the EPH values for this user</returns>
        public string[] ValuesEPH(string codpsw, EPHCondition condition)
        {
            try
            {
                AreaInfo tabelaEPH = Area.GetInfoArea(condition.EPHTable);
                SelectQuery query = new SelectQuery();
                //TODO: This field needs to be modeled instead of hardcoded to allow external user directories to work with EPH
                var useridField = "codpsw";

                if (tabelaEPH.TableName == condition.TableName)
                {
                    //Get the values from the same table
                    query.Select(tabelaEPH.Alias, condition.EPHField)
                        .From(condition.TableSystem, condition.TableName, tabelaEPH.Alias)
                        .Where(CriteriaSet.And()
                            .Equal(condition.AliasTable, useridField, codpsw));
                }
                else
                {
                    //The values we are after are in a table above this one, so we need to follow the relation
                    AreaInfo tabelaAssoc = Area.GetInfoArea(condition.AliasTable);
                    Relation rel = tabelaAssoc.ParentTables[tabelaEPH.Alias];

                    //If the EPHfield we are after is the primary key of the Value table
                    //then we can use the foreign key of the relation to that table and avoid the inner join
                    if (condition.EPHField == rel.TargetIntKey)
                    {
                        query.Select(tabelaEPH.Alias, rel.SourceRelField)
                            .From(condition.TableSystem, condition.TableName, tabelaEPH.Alias)
                            .Where(CriteriaSet.And()
                                .Equal(condition.AliasTable, useridField, codpsw));
                    }
                    else
                    {
                        query.Select(tabelaEPH.Alias, condition.EPHField)
                            .From(condition.TableSystem, condition.TableName, condition.AliasTable)
                            .Join(tabelaEPH.TableName, tabelaEPH.Alias)
                            .On(CriteriaSet.And().Equal(rel.AliasSourceTab, rel.SourceRelField, rel.AliasTargetTab, rel.TargetIntKey))
                            .Where(CriteriaSet.And()
                                .Equal(condition.AliasTable, useridField, codpsw));
                    }
                }
                ArrayList valorChaves = executeReaderOneColumn(query);
                if (valorChaves == null)
                    return [];
                List<string> Qvalues = new List<string>();
                foreach (object chaveBD in valorChaves)
                    Qvalues.Add(DBConversion.ToString(chaveBD));
                return Qvalues.ToArray();
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCampos", "Error returning EPHs for user with password " + codpsw + "where " + condition.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.devolveCampos", "Error returning EPHs for user with password " + codpsw + "where " + condition.ToString() + ": " + ex.Message, ex);
            }
        }

		/// <summary>
        /// Return initial eph values
        /// </summary>
        /// <param name="condition">The initial EPH to fetch values for</param>
        /// <param name="values">values to filter</param>
        /// <returns>list with the initial EPH values for this user</returns>
        public string[] ValuesEphInitial(EPHCondition condition, string[] values)
        {
            try
            {
                AreaInfo tabelaEPH = Area.GetInfoArea(condition.EPHTable);
                string primaryKeyName = tabelaEPH.PrimaryKeyName;
                SelectQuery query = new SelectQuery();

                if (tabelaEPH.TableName == condition.TableName)
                {
                    //If the values we want are exactly the values of the primary keys we got, then just return them as is
                    if (condition.EPHField == primaryKeyName)
                        return values;

                    //otherwise we need to exchange the primary key values for another field of this same table
                    query.Select(tabelaEPH.Alias, condition.EPHField)
                        .From(condition.TableSystem, condition.TableName, tabelaEPH.Alias)
                        .Where(CriteriaSet.And()
                            .In(condition.AliasTable, primaryKeyName, new List<string>(values)));
                }
                else
                {
                    //The values we are after are in a table above this one, so we need to follow the relation
                    AreaInfo tabelaAssoc = Area.GetInfoArea(condition.AliasTable);
                    Relation rel = tabelaAssoc.ParentTables[tabelaEPH.Alias];

                    //If the EPHfield we are after is the primary key of the Value table,
                    //then we again already have the values we are looking for
                    if (condition.EPHField == rel.TargetIntKey)
                        return values;                    

                    query.Select(tabelaEPH.Alias, condition.EPHField)
                        .From(condition.TableSystem, condition.TableName, condition.AliasTable)
                        .Join(tabelaEPH.TableName, tabelaEPH.Alias)
                        .On(CriteriaSet.And().Equal(rel.AliasSourceTab, rel.SourceRelField, rel.AliasTargetTab, rel.TargetIntKey))
                        .Where(CriteriaSet.And()
                            .In(condition.AliasTable, primaryKeyName, new List<string>(values)));
                }

                ArrayList valorChaves = executeReaderOneColumn(query);
                if (valorChaves == null)
                    return [];
                List<string> Qvalues = new List<string>();
                foreach (object chaveBD in valorChaves)
                    Qvalues.Add(DBConversion.ToString(chaveBD));
                return Qvalues.ToArray();
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.devolveCampos", "Error returning EPHs for " + condition.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.devolveCampos", "Error returning EPHs for " + condition.ToString() + ": " + ex.Message, ex);
            }
        }

        /****************************************GETÚNICO************************************************/
        /// <summary>
        /// Function that returns a database record when it is the only one that meets the conditions
        /// </summary>
        /// <param name="condicoes">conditions for reading the register</param>
        /// <param name="area">area to which the register belongs</param>
        /// <param name="identificador">request identifier</param>
        /// <returns> area with the values of the fields given</returns>
        public void selectSingle(CriteriaSet conditions, IArea area, string identifier)
        {
            try
            {
                CriteriaSet allConditions = conditions;
                SelectQuery query = null;

                //check if there is a query override to the LED (query done in manual routine)
                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    query = controlQueriesOverride[identifier](area.User, area.User.CurrentModule, conditions, null, this);
                }
                else
                {
                    //NH(01.10.2010) - Adds the conditions defined in the query to the control but removes the part of zzstate = 0
                    /*Não funcionava to os casos de um dbedit que tivesse um tipo de limitação FIXO*/
                    ControlQueryDefinition queryGenio = controlQueries[identifier];
                    CriteriaSet condicoesAux = null;

                    if (queryGenio.WhereConditions != null)
                    {
                        condicoesAux = CriteriaSet.And();
                        foreach (Criteria criteria in queryGenio.WhereConditions.Criterias)
                        {
                            // exclude ZZSTATE criteria
                            if (!(criteria.LeftTerm is ColumnReference
                                && String.Equals(((ColumnReference)criteria.LeftTerm).ColumnName, "ZZSTATE", StringComparison.InvariantCultureIgnoreCase)))
                            {
                                condicoesAux.Criterias.Add(criteria);
                            }
                        }
                        foreach (CriteriaSet subSet in queryGenio.WhereConditions.SubSets)
                        {
                            condicoesAux.SubSet(subSet);
                        }

                        if (condicoesAux.Criterias.Count > 0 || condicoesAux.SubSets.Count > 0)
                        {
                            allConditions.SubSet(condicoesAux);
                        }
                    }

                    query = querySeleccionaUm(allConditions, null, area);

                    SelectQuery queryCount = QueryUtils.buildQueryCount(query);
                    DataMatrix mx = Execute(queryCount);

                    int nr = 0;
                    if (mx != null && mx.NumRows > 0)
                    {
                        nr = mx.GetInteger(0, 0);
                    }

                    if (nr != 1)//if there are no records
                    {
                        Hashtable hresVaz = new Hashtable();
                        for (int i = 0; i < query.SelectFields.Count; i++)
                        {
                            hresVaz.Add(query.SelectFields[i].Alias, null);
                        }
                        fillAreaSelectOne(hresVaz, area);
                        return;//as it is an LED should return to empty area
                    }
                }

                ArrayList Qresult = executeReaderOneRow(query);
                Hashtable hresUnico = new Hashtable();
                for (int i = 0; i < Qresult.Count; i++)
                {
                    hresUnico.Add(query.SelectFields[i].Alias, Qresult[i]);
                }

                fillAreaSelectOne(hresUnico, area);
                return;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionaUnico",
					"Error selecting unique record from area " + area.ToString() + " where " + conditions.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.seleccionaUnico",
					"Error selecting unique record from area " + area.ToString() + " where " + conditions.ToString() + ": " + ex.Message, ex);
            }
        }

        /****************************************GET1************************************************/

        /// <summary>
        /// Fetches a row from the database and loads it into the supplied area
        /// </summary>
        /// <param name="conditions">Conditions to search for in the table</param>
        /// <param name="sorting">Sorting to apply if multiple rows obey the condition. Only top row is loaded.</param>
        /// <param name="area">The area instance to fill with the data</param>
        /// <param name="identifier">Query identifier associated with this query. Allows for overriding standard framework queries</param>
        /// <param name="pageSize">Reserved. Must be 1.</param>
        /// <exception cref="PersistenceException"></exception>
        public void selectOne(CriteriaSet conditions, IList<ColumnSort> sorting, IArea area, string identifier, int pageSize = 1)
        {
            try
            {
                SelectQuery query = null;
                //check if the request is an LED
                bool isLed = false;
                if (identifier.Length >= 3)
                    isLed = identifier.Substring(0, 3).Equals("LED");

                //in case it is an LED, check if there is a query override (query done in manual routine)
                if (isLed && controlQueriesOverride.ContainsKey(identifier))//if it's an LED
                    query = controlQueriesOverride[identifier](area.User, area.User.CurrentModule, conditions, sorting, this);
                else
                {
                    query = querySeleccionaUm(conditions, sorting, area, pageSize);
                    if (isLed)
                    {
                        ControlQueryDefinition queryGenio = controlQueries[identifier];

                        if (queryGenio.WhereConditions != null)
                        {
                            //Find a criteria for the area's primary key because it means we have a specific selection
                            Criteria critPrimaryKey = conditions.FindCriteria(area.Alias, area.PrimaryKeyName, CriteriaOperator.Equal, CriteriaSet.FindVariable.Any);
                            if (critPrimaryKey is null || critPrimaryKey.RightTerm is null)
                            {
                                foreach (CriteriaSet sub in conditions.SubSets)
                                {
                                    critPrimaryKey = sub.FindCriteria(area.Alias, area.PrimaryKeyName, CriteriaOperator.Equal, CriteriaSet.FindVariable.Any);
                                    if (critPrimaryKey != null && critPrimaryKey.RightTerm != null)
                                        break;
                                }
                            }

                            CriteriaSet where = CriteriaSet.And();
                            //When there isn't a specific selection we include the zzstate condition to ensure the returned records are valid
                            //Use case: QWeb autocomplete searches should not include zzstate invalid records
                            if (critPrimaryKey is null && queryGenio.WhereConditions.Criterias.Count > 0)
                            {
                                foreach (Criteria crit in queryGenio.WhereConditions.Criterias)
                                    where.Criterias.Add(crit);
                            }
                            //When there's a specific selection there's no need to include the zzstate = 0 condition (previously handled on the selection moment)
                            //This is particularly important when inserting a record through a table list of a pseudo-new record (it ensures the relation is correctly mapped)
                            else if (queryGenio.WhereConditions.Criterias.Count > 1)
                            {
                                for (int i = 1; i < queryGenio.WhereConditions.Criterias.Count; i++)
                                    where.Criterias.Add(queryGenio.WhereConditions.Criterias[i]);
                            }

                            if (queryGenio.WhereConditions.SubSets.Count > 0)
                            {
                                foreach (CriteriaSet subSet in queryGenio.WhereConditions.SubSets)
                                    where.SubSets.Add(subSet);
                            }

                            if (where.Criterias.Count > 0 || where.SubSets.Count > 0)
                                query.WhereCondition.SubSet(where);
                        }
                    }

                    SelectQuery queryCount = QueryUtils.buildQueryCount(query);
                    DataMatrix nrLinhas = Execute(queryCount);
                    int nr = 0;
                    if (nrLinhas != null && nrLinhas.NumRows > 0)
                    {
                        nr = nrLinhas.GetInteger(0, 0);
                    }

                    if (pageSize > 0)
                    {
                        if (nr > 1 && !isLed)
                        {
                            throw new PersistenceException(null, "PersistentSupport.seleccionaUm", "There is more than one record in table " + area.ToString() + " satisfying the conditions " + conditions.ToString() + ".");
                        }
                        else if (nr == 0)//if there are no records
                        {
                            if (!isLed)//if it's not an LED should give error
                                throw new PersistenceException(null, "PersistentSupport.seleccionaUm", "There are no records in table " + area.ToString() + " satisfying the conditions " + conditions.ToString() + ".");
                            else
                            {   //if it is an LED should return the area with empty values
                                Hashtable hresVaz = new Hashtable();
                                for (int i = 0; i < query.SelectFields.Count; i++)
                                {
                                    hresVaz.Add(query.SelectFields[i].Alias, null);
                                }
                                fillAreaSelectOne(hresVaz, area);
                                return;
                            }
                        }
                    }
                }

                //if the override Qresult (manual routine) comes to empty
                ArrayList Qresult = executeReaderOneRow(query);
                Hashtable hres = new Hashtable();
                int countresult = Qresult.Count;
                if (countresult == 0)
                    countresult = query.SelectFields.Count;

                for (int i = 0; i < countresult; i++)
                {
                    if (Qresult.Count == 0)
                        hres.Add(query.SelectFields[i].Alias, null);
                    else
                        hres.Add(query.SelectFields[i].Alias, Qresult[i]);
                }

                fillAreaSelectOne(hres, area);
                return;
            }
            catch (PersistenceException ex)
            {
				if (ex.ExceptionSite == "PersistentSupport.seleccionaUm")
					throw;
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionaUm",
					                           string.Format("Error selecting one record from area - [condicoes] {0}; [ordenacao] {1}; [area] {2}; [identificador] {3}: ", conditions.ToString(), sorting.ToString(), area.ToString(), identifier) + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.seleccionaUm",
					                           string.Format("Error selecting one record from area - [condicoes] {0}; [ordenacao] {1}; [area] {2}; [identificador] {3}: ", conditions.ToString(), sorting.ToString(), area.ToString(), identifier) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Place the data taken from the BD in the corresponding Area object
        /// </summary>
        /// <param name="qResult">Query result</param>
        /// <param name="area">Area to be filled</param>
        /// <returns>Filled area</returns>
        private void fillAreaSelectOne(Hashtable qResult, IArea area)
        {
            if (qResult.Count != 0)
            {
                string pkFullname = area.Information.DBFields[area.PrimaryKeyName].FullName;
                //fill in the value of the primaryKey
                //if the primary key is in the query is the 1st value
                //to be able to be used to fill in other fields namely images
                if (area.Fields.ContainsKey(pkFullname))
                    area.insertNameValueField(pkFullname , qResult[pkFullname]);

                //AV 20090309 foreign keys to a table docums
                if (area.Information.DocumsForeignKeys != null)
                    for (int j = 0; j < area.Information.DocumsForeignKeys.Count; j++)
                        if (area.Fields.ContainsKey(area.Alias + "." + area.Information.DocumsForeignKeys[j]))
                            area.insertNameValueField(area.Alias + "." + area.Information.DocumsForeignKeys[j],
                                qResult[area.Alias + "." + area.Information.DocumsForeignKeys[j]]);

                foreach(RequestedField campoPedido in area.Fields.Values)
                    if (!campoPedido.FullName.Equals(pkFullname)
                        && (area.Information.DocumsForeignKeys == null || !area.Information.DocumsForeignKeys.Contains(campoPedido.Name)))
                    {
                        area.insertNameValueField(campoPedido.FullName, qResult[campoPedido.FullName]);
                    }

                return;
            }
            else
				throw new PersistenceException(null, "PersistentSupport.preencheAreaSeleccionaUm", "Argument 'resultado' is empty.");
        }

        /// <summary>
        /// Method that creates the query to respond to a GET1 request
        /// </summary>
        /// <param name="conditions">filter condition</param>
        /// <param name="sorting">row sorting</param>
        /// <param name="area">area to select from</param>
        /// <param name="pageSize">Reserved. Must be 1</param>
        /// <returns>Returns QuerySelect with field values</returns>
        private SelectQuery querySeleccionaUm(CriteriaSet conditions, IList<ColumnSort> sorting, IArea area, int pageSize = 1)
        {
            //List of strings that correspond to tables with relationships that appear in the request
            List<string> tabelasAcima = new List<string>();

            foreach (RequestedField campoPedido in area.Fields.Values)
            {
                //RequestedField campoPedido = (RequestedField)enumCampos.Current;
                if (!campoPedido.BelongsArea && !campoPedido.WithoutArea)
                {
                    if (!tabelasAcima.Contains(campoPedido.Area))
                        tabelasAcima.Add(campoPedido.Area);
                }
            }
            //list of query relationships
            List<Relation> relations = QueryUtils.tablesRelationships(tabelasAcima, area);

            SelectQuery query = construcaoQuerySeleccionaUm(area, conditions, sorting, relations);
            return query;
        }

        /// <summary>
        /// Helper method to create querySelect of the function type GET_UM
        /// </summary>
        /// <param name="area">table that where the record will be read</param>
        /// <param name="conditions">query conditions</param>
        /// <param name="sorting">query ordering</param>
        /// <param name="relations">relationships to other table that are part of the query</param>
        /// <param name="pageSize">Reserved. Must be 1</param>
        /// <returns>A complete query to be executed</returns>
        private SelectQuery construcaoQuerySeleccionaUm(IArea area, CriteriaSet conditions, IList<ColumnSort> sorting, List<Relation> relations, int pageSize = 1)
        {
            SelectQuery query = new SelectQuery();

            //if there is primary key in the query it must be the first field to be requested
            if (area.Fields.ContainsKey(area.Alias + "." + area.PrimaryKeyName))
            {
                query.Select(area.Alias, area.PrimaryKeyName);
            }

            //AV 20090306 foreign keys to a table docums
            if (area.Information.DocumsForeignKeys != null)
            {
                for (int i = 0; i < area.Information.DocumsForeignKeys.Count; i++)
                {
                    string fieldName = area.Information.DocumsForeignKeys[i];
                    if (area.Fields.ContainsKey(area.Alias + "." + fieldName))
                    {
                        query.Select(area.Alias, fieldName);
                    }
                }
            }

            //fields of the area
            foreach (RequestedField campoPedido in area.Fields.Values)
            {
                if (campoPedido.WithoutArea)
                    continue;
                if (campoPedido.Area == area.Alias && campoPedido.Name == area.PrimaryKeyName)
                    continue;
                if (area.Information.DocumsForeignKeys?.Contains(campoPedido.Name) ?? false)
                    continue;
                query.Select(campoPedido.Area, campoPedido.Name);
            }
            QueryUtils.setFromTabDirect(query, relations, area);

            if (conditions != null)
            {
                query.Where(conditions);
            }
            if (sorting != null && sorting.Count > 0)
            {
                foreach (ColumnSort sort in sorting)
                {
                    query.OrderBy(sort.Expression, sort.Order);
                }
            }
            query.PageSize(pageSize);
            return query;
        }

        /// <summary>
        /// Reorders a field within a subset from startPos to N maintaining the relative order of the records
        /// </summary>
        /// <param name="area">The table to reorder</param>
        /// <param name="orderField">The field to reorder</param>
        /// <param name="partition">The partition corresponding to the rows to be reordered</param>
        public void ReorderSequence(AreaRef area, FieldRef orderField, CriteriaSet partition, int startPos = 1)
        {
            AreaInfo areaInfo = Area.GetInfoArea(area.Alias);
            ReorderSequence(areaInfo, orderField.Field, partition, startPos);
        }

        /// <summary>
        /// Reorders a field within a subset from startPos to N maintaining the relative order of the records
        /// </summary>
        /// <param name="area">The table to reorder</param>
        /// <param name="orderField">The field to reorder</param>
        /// <param name="partition">The partition corresponding to the rows to be reordered</param>
        public void ReorderSequence(Area area, Field orderField, CriteriaSet partition, int startPos = 1)
        {
            ReorderSequence(area.Information, orderField.Name, partition, startPos);
        }

        /// <summary>
        /// Reorders a field within a subset from startPos to N maintaining the relative order of the records
        /// </summary>
        /// <param name="area">The table to reorder</param>
        /// <param name="orderField">The field to reorder</param>
        /// <param name="partition">The partition corresponding to the rows to be reordered</param>
        public void ReorderSequence(AreaInfo area, string orderField, CriteriaSet partition, int startPos = 1)
        {
            // UPDATE [GENNOV0].[dbo].[gencmpbd]
            // SET [num] = [renum_campo].[new_num]
            // FROM [GENNOV0].[dbo].[gencmpbd] [campo]
            // JOIN (SELECT  (ROW_NUMBER() OVER (ORDER BY [campo].[num]  ASC)) + startPos - 1 AS [new_num],  ([campo].[codcmpbd]) AS [pk]
            //          FROM [GENNOV0].[dbo].[gencmpbd] AS [campo]
            //          WHERE ([campo].[codtabel] = @param1)) AS [renum_campo]
            // ON ([renum_campo].[pk] = [campo].[codcmpbd])

            ColumnReference orderingFieldColumn = new(area.Alias, orderField);
            ColumnSort[] orderBy = [new ColumnSort(orderingFieldColumn, GenericSortOrder.Ascending)];

            //RowNumber starts at 1 so, add startPos - 1 to RowNumber to start numbering at startPos
            SelectQuery sq = new SelectQuery()
                .Select(SqlFunctions.Add(SqlFunctions.RowNumber(orderBy), startPos - 1), "new_num")
                .Select(area.Alias, area.PrimaryKeyName, "pk")
                .From(area.TableName, area.Alias)
                .Where(partition);

            string renumArea = "renum_" + area.Alias;
            UpdateQuery up = new UpdateQuery().Update(area.TableName)
                .Set(orderField, new ColumnReference(renumArea, "new_num"))
                .Join(sq, renumArea, TableJoinType.Inner)
                .On(CriteriaSet.And()
                    .Equal(renumArea, "pk", area.TableName, area.PrimaryKeyName));

            Execute(up);
        }

		/// <summary>
        /// Get the highest value of a field
        /// </summary>
        /// <param name="area">The table</param>
        /// <param name="field">The field</param>
        /// <param name="partition">The partition corresponding to the rows</param>
        public int GetMaxFieldValue(Area area, Field field, CriteriaSet partition)
        {
            SelectQuery maxOrderQuery = new SelectQuery()
                .Select(SqlFunctions
                    .Max(new ColumnReference(area.Alias, field.Name)), "maxOrder")
                .From(area.TableName, area.Alias)
                .Where(partition);

            DataMatrix maxOrderRows = Execute(maxOrderQuery);

            //No records or fields
            if (maxOrderRows.NumRows == 0 || maxOrderRows.NumCols == 0)
                throw new Exception("This table does not have any records that match the conditions given.");

            return maxOrderRows.GetInteger(0, 0);
        }

        /*********************************INSERIR DADOS*************************************/

        /// <summary>
        /// Function to introduce data in an area
        /// </summary>
        /// <param name="area">Area of the object you want to introduce</param>
        /// <param name="condicoes">insertion conditions</param>
        /// <param name="utilizador">user who is introducing the registration</param>
        /// <param name="isTabelaBase">to indicate whether it is a table base or not</param>
        /// <returns>Returns the inserted area</returns>
        public void insertPseud(IArea area)
        {
            //if we are allocating pk's in the application then make sure we have one before building the query
            if (!DatabaseSidePk && string.IsNullOrEmpty(area.QPrimaryKey))
                area.insertNameValueField(area.PrimaryKeyName, codIntInsertion(area, false));

            InsertQuery query = new InsertQuery();
            QueryUtils.buildQueryInsert(query, area, DatabaseSidePk);
            var res = Execute(query);

            //if we are allocating pk's in the database then read back the result of the query
            if(DatabaseSidePk)
                area.insertNameValueField(area.PrimaryKeyName, DBConversion.ToKey(res), fromDatabase: true);
        }

        /// <summary>
        /// Bulk insert records into the database
        /// </summary>
        /// <param name="rows">The list of rows to insert</param>
        public virtual void bulkInsert(IEnumerable<IArea> rows)
        {
            if (!rows.Any())
                return;
            var info = rows.First().Information;

            //the first row will define the columns (every following row needs to have the exact same number of columns)
            var columns = new List<Field>();
            foreach (RequestedField rf in rows.First().Fields.Values)
                columns.Add(info.DBFields[rf.Name]);

            //batch the rows so that the number of parameter in each batch in under 500
            //Max parameters is around 2000 but sql server degrades performance if too many parameters are passed
            //so its actually faster to send more batches and less parameters. There is a tradeoff with the
            //number of roundtrips, so the optimal spot seems to be around 200 - 500 parameters.
            int batchSize = 500 / columns.Count;

            var rowsIterator = rows.GetEnumerator();
            while(rowsIterator.MoveNext())
            {
                List<InsertQuery> inserts = new List<InsertQuery>();
                int batchIndex = 0;
                do
                {
                    var row = rowsIterator.Current;
                    InsertQuery query = new InsertQuery();
                    QueryUtils.buildQueryInsert(query, row);
                    inserts.Add(query);
                    batchIndex++;
                } while(batchIndex < batchSize && rowsIterator.MoveNext());

                var renderer = new QueryRenderer(this);
                renderer.SchemaMapping = SchemaMapping;

                var sql = renderer.GetSql(inserts);
                long st = DateTime.Now.Ticks;
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[bulkInsert] {0}.", sql) + Environment.NewLine + renderer.ParameterList.Count + " parameter sent.");
                var parameters = renderer.ParameterList;

                IDbCommand command = CreateCommand(sql, parameters);

                command.ExecuteNonQuery();
                if (Log.IsDebugEnabled) Log.Debug("[bulkInsert] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
            }
        }

        /// <summary>
        /// Bulk updates records in the database
        /// </summary>
        /// <param name="rows">The list of rows to update</param>
        public virtual void bulkUpdate(IEnumerable<IArea> rows)
        {
            //non optimized version of bulk update (providers should specialize and optimize this method)
            foreach (var row in rows)
                change(row);
        }

        /// <summary>
        /// Bulk delete records in the database
        /// </summary>
        /// <param name="rows">The list of rows to delete</param>
        public virtual void bulkDelete(IEnumerable<IArea> rows)
        {
            //non optimized version of bulk delete (providers should specialize and optimize this method)
            foreach (var row in rows)
                deleteRecord(row, row.QPrimaryKey);
        }


        /// <summary>
        /// Function that returns the internal code to insert a new record
        /// </summary>
        /// <param name="area">Name of the area to which the record that will be inserted belongs.
        /// Does not assume an open connection and closes the connection</param>
        /// <param name="shadow">Get a primary key for the shadow table instead</param>
        /// <returns>returns the new internal code</returns>
        public string codIntInsertion(IArea area, bool shadow)
        {
            try
            {
                Field chaveinfo = area.DBFields[area.PrimaryKeyName];
                if(shadow)
                    return generatePrimaryKey(area.ShadowTabName, area.ShadowTabKeyName, chaveinfo.FieldSize, area.Information.KeyType);
                else
                    return generatePrimaryKey(area.TableName, area.PrimaryKeyName, chaveinfo.FieldSize, area.Information.KeyType);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.codIntInsercao", "Error getting internal code to insert in area " + area.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.codIntInsercao", "Error getting internal code to insert in area " + area.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Gets a new primary key to a particular object
        /// </summary>
        /// <param name="id_object">The object to which you want to generate a key, typically the name of the table</param>
        /// <param name="id_field">The field in the object to generate the key for, typically the name of the primary key</param>
        /// <param name="size">The size of the key to generate to the case of internal code</param>
        /// <param name="format">The key format to generate</param>
        /// <returns>A single primary key</returns>
        public abstract string generatePrimaryKey(string id_object, string id_field, int size, FieldType format);

        /// <summary>
        /// Gets a range of new primary keys in bulk to a particular object
        /// </summary>
        /// <param name="id_object">The object to which you want to generate a key, typically the name of the table</param>
        /// <param name="id_field">The field in the object to generate the key for, typically the name of the primary key</param>
        /// <param name="size">The size of the key to generate to the case of internal code</param>
        /// <param name="format">The key format to generate</param>
        /// <param name="range">The number of primary keys to obtain in bulk</param>
        /// <returns>A single primary key</returns>
        public virtual List<string> generatePrimaryKey(string id_object, string id_field, int size, FieldType format, int range)
        {
            // This is a unoptimized generic version of bulk alocation of primary keys
            // Each provider is responsible to implement a more efficient version.
            if (range < 1)
                throw new ArgumentException("range must be 1 or larger", nameof(range));

            List<string> codes = new List<string>();
            for (int i = 0; i < range; i++)
                codes.Add(generatePrimaryKey(id_object, id_field, size, format));
            return codes;
        }

        public object insertValueDocums(IArea area, string fieldName, string fileName, string extension, byte[] file)
        {
            string tabelaDocums = "docums";
            object primaryKeyValue = area.returnValueField(area.Alias + "." + area.PrimaryKeyName);
            if (area.DBFields[area.PrimaryKeyName].FieldType == FieldType.KEY_GUID)
                primaryKeyValue = primaryKeyValue.ToString().Replace("-", "");
            Field chaveDocums = CSGenioAdocums.GetInformation().DBFields["coddocums"];
            var fieldType = CSGenioAdocums.GetInformation().KeyType;
            string valorChavePrimariaDocumsStr = generatePrimaryKey(tabelaDocums, "coddocums", chaveDocums.FieldSize, fieldType);
            object valorChavePrimariaDocums = fieldType == FieldType.KEY_INT
	                ? DBConversion.ToInteger(valorChavePrimariaDocumsStr)
                    : valorChavePrimariaDocumsStr;


            //RS(2010.09.16) The table docums starts to gardar several verses and the author of the document
            InsertQuery query = new InsertQuery()
                .Into(tabelaDocums)
                .Value("coddocums", valorChavePrimariaDocums)
                .Value("documid", valorChavePrimariaDocums)
                .Value("document", file)
                .Value("tabela", area.TableName)
                .Value("campo", fieldName)
                .Value("chave", primaryKeyValue)
                .Value("datacria", DateTime.Now)
                .Value("opercria", area.User.Name)
                .Value("nome", fileName)
                .Value("versao", "1")
                .Value("tamanho", file.Length)
                .Value("extensao", extension)
                .Value("opermuda", area.User.Name)
                .Value("datamuda", DateTime.Now)
                .Value("zzstate", 0);

            Execute(query);

            return valorChavePrimariaDocums;
        }


        /// <summary>
        /// Method that duplicates docums table records and updates foreign keys in duplicate area
        /// </summary>
        /// <param name="area">Area where the file fields will be replaced in BD</param>
        /// <param name="forCheckout">If the duplication is to checkout of the file or if it is a normal plug duplication</param>
        /// <param name="documField">Docum field used when forCheckout == true</param>
        public void duplicateFilesDB(IArea area, bool forCheckout=false, string documField = "")
        {
            try
            {
                string tabelaDocums = "docums";
                string valorChavePrimariaDocums  ="";

                if (area.Information.DocumsForeignKeys != null)
                {
                    object primaryKeyValueAux = area.QPrimaryKey;
                    if (area.DBFields[area.PrimaryKeyName].FieldType == FieldType.KEY_GUID)
                        primaryKeyValueAux = primaryKeyValueAux.ToString().Replace("-", "");

                    var formatacaoChaveDocums = CSGenioAdocums.GetInformation().KeyType;
                    foreach(var documForeignKey in area.Information.DocumsForeignKeys)
                    {
                        if(area.Fields.TryGetValue(area.Alias + "." + documForeignKey, out RequestedField campoPedido))
                        {
                            // if the key value is blank, nothing is done because there is no document
                            // if this gets out of here to out, you can do -> Field.isEmptyValue()
                            if (string.IsNullOrEmpty(campoPedido.Value.ToString()) || (forCheckout && !campoPedido.FullName.Equals(documField)) )
                                continue;

                            valorChavePrimariaDocums = generatePrimaryKey(tabelaDocums, "coddocums", area.DBFields[documForeignKey].FieldSize, formatacaoChaveDocums);

                            SelectQuery qs = new SelectQuery()
                                .Select(CSGenioAdocums.FldDocumid)
                                .Select(CSGenioAdocums.FldDocument)
                                .Select(CSGenioAdocums.FldTabela)
                                .Select(CSGenioAdocums.FldCampo)
                                .Select(CSGenioAdocums.FldNome)
                                .Select(CSGenioAdocums.FldTamanho)
                                .Select(CSGenioAdocums.FldExtensao)
                                .From(tabelaDocums)
                                .Where(CriteriaSet.And()
                                    .Equal(CSGenioAdocums.FldDocumid, campoPedido.Value))
                                .OrderBy(CSGenioAdocums.FldVersao, GenericSortOrder.Descending);

                            var matrix = this.Execute(qs);
                            if (matrix.NumRows > 0)
                            {
                                // if it's to checkout keeps the documid
                                // if it is to duplicate the plug the documid is equal to the primary key
                                string documid = forCheckout ? matrix.GetString(0, CSGenioAdocums.FldDocumid) : valorChavePrimariaDocums;
                                string extension = matrix.GetString(0, CSGenioAdocums.FldExtensao);

                                //Insert the record with the duplicated values
                                InsertQuery insert = new InsertQuery().Into(tabelaDocums);
                                insert.Value(CSGenioAdocums.FldCoddocums, valorChavePrimariaDocums);
                                insert.Value(CSGenioAdocums.FldDocumid, documid);
                                //This client stores documents in the database
                                insert.Value(CSGenioAdocums.FldDocument, matrix.GetBinary(0, CSGenioAdocums.FldDocument));
                                insert.Value(CSGenioAdocums.FldTabela, matrix.GetString(0, CSGenioAdocums.FldTabela));
                                insert.Value(CSGenioAdocums.FldCampo, matrix.GetString(0, CSGenioAdocums.FldCampo));
                                insert.Value(CSGenioAdocums.FldChave, primaryKeyValueAux);
                                insert.Value(CSGenioAdocums.FldDatacria, DateTime.Now);
                                insert.Value(CSGenioAdocums.FldNome, matrix.GetString(0, CSGenioAdocums.FldNome));
                                // if it is to checkout the status is "CHECKOUT"
                                // if it is to duplicate the version resets to 1
                                insert.Value(CSGenioAdocums.FldVersao, forCheckout ? "CHECKOUT" : "1");
                                insert.Value(CSGenioAdocums.FldZzstate, 0);
                                insert.Value(CSGenioAdocums.FldOpercria, area.User.Name);
                                insert.Value(CSGenioAdocums.FldTamanho, matrix.GetNumeric(0, CSGenioAdocums.FldTamanho));
                                insert.Value(CSGenioAdocums.FldExtensao, extension);
                                Execute(insert);

                                //update the record that references the document
                                area.insertNameValueField(campoPedido.FullName, documid, fromDatabase: true);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new FrameworkException($"Error duplicating documents on table {area.TableName}", "duplicateDocumentDB", ex.Message);
            }
        }

        /// <summary>
        /// Updates the Chave field in the Docums table with the primary key of this record.
        /// This can be necessary when the PK is only knows after the insert operation.
        /// </summary>
        public void AfterDuplicateFilesDB(IArea area)
        {
            if (area.Information.DocumsForeignKeys == null || area.Information.DocumsForeignKeys.Count == 0)
                return;
            
            List<string> documsFks = new List<string>();
            foreach (var documForeignKey in area.Information.DocumsForeignKeys)
            {
                string fk = area.returnValueField(documForeignKey) as string;
                if(!string.IsNullOrEmpty(fk))
                    documsFks.Add(fk);
            }

            if (documsFks.Count == 0)
                return;

            UpdateQuery uq = new UpdateQuery()
                .Update("docums")
                .Set("chave", area.QPrimaryKey)
                .Where(CriteriaSet.And()
                .In("docums", "documid", documsFks));

            Execute(uq);
        }

        /*********************************ELIMINAR DADOS*************************************/

        /// <summary>
        /// Method to eliminate a record
        /// </summary>
        /// <param name="area">Area to which the register belongs</param>
        /// <returns>returns the area after the record is deleted</returns>
        public void eliminate(IArea area)
        {
            object valorCodigoObj = area.returnValueField(area.Alias + "." + area.PrimaryKeyName);
            deleteRecord(area, (string)valorCodigoObj);
        }

        /// <summary>
        /// Function that allows you to eliminate a plug
        /// </summary>
        /// <param name="area"> Area to which the plug belongs that will be erased</param>
        /// <param name="valorCodigo">value of the internal code of the plug that will be erased</param>
        /// <returns>true if the plug is erased and false otherwise</returns>
        public bool deleteRecord(IArea area, string codeValue)
        {
            bool Qresult = false;
            DeleteQuery queryDelete = new DeleteQuery()
                .Delete(area.QSystem, area.TableName)
                .Where(CriteriaSet.And()
                    .Equal(area.TableName, area.PrimaryKeyName, codeValue));

            int linha = Execute(queryDelete);

            if (linha != 0)
                Qresult = true;
            else
				throw new PersistenceException("O registo não foi encontrado.", "PersistentSupport.apagarFicha", "Error deleting record with code " + codeValue + " from area " + area.ToString() + ": the query returned 0.");
            //here does not close connection, because I'm erasing cascading chips
            return Qresult;
        }

        /// <summary>
        /// Function that allows you to delete a relationship, that is, it puts the value of the foreign key to ""
        /// </summary>
        /// <param name="area">Area to which the token belongs whose relationship will be deleted</param>
        /// <param name="foreignCodeName">Foreign key code name</param>
        /// <param name="codeValue">valro of the foreign key code</param>
        public void deleteRelationship(IArea area, string foreignCodeName, string codeValue)
        {
            UpdateQuery query = new UpdateQuery()
                .Update(area.TableName)
                .Set(foreignCodeName, null)
                .Where(CriteriaSet.And().Equal(area.TableName, foreignCodeName, codeValue));
            Execute(query);
        }

        public void deleteRecordDocums(object keyNameDocum,object keyValueDocums)
        {
            if (keyValueDocums != null && keyValueDocums.ToString() != String.Empty)
            {
                //RS(2010.09.16) The table docums starts to gardar several verses and the author of the document
                string tableName = "docums";
                DeleteQuery query = new DeleteQuery()
                    .Delete(tableName)
                    .Where(CriteriaSet.And()
                        .Equal(tableName, Convert.ToString(keyNameDocum), keyValueDocums));
                Execute(query);
            }
        }

        /*********************************ALTERAR DADOS*****************************************/

        /// <summary>
        /// Function that allows you to change a database record
        /// </summary>
        /// <param name="area">Area to which the registry belongs</param>
        /// <param name="condicao">Condition that allows you to identify which record</param>
        /// <returns>The Pair (status,message) about the change</returns>
        public void change(IArea area)
        {
            // RR 24/02/2011
            // here goes on to build the update query instead of calling the function
            // buildChange, to be able to access the parameters

            UpdateQuery query = new UpdateQuery();
            QueryUtils.fillQueryUpdate(query, area);

            //there is no reason to execute a update to 0 columns
            if (query.SetValues.Count == 0)
                return;

            int linha = Execute(query);
            if (linha == 0)
            {
				throw new PersistenceException("Erro na alteração do registo.", "PersistentSupport.alterar", "Error updating record from area " + area.ToString() + ": the query returned 0.");
            }
        }

        /// <summary>
        /// Method to construct a query to change the data
        /// </summary>
        /// <param name="area">Area a change</param>
        /// <returns>string with the query</returns>
        public UpdateQuery buildGenericQueryChange(IArea area)
        {
            UpdateQuery query = new UpdateQuery();
            QueryUtils.fillQueryUpdate(query, area);

            return query;
        }

        /// <summary>
        /// Method to change the value of a field in all records that check the condition
        /// </summary>
		/// <param name="sistema">Register table prefix schema changed</param>
        /// <param name="tabela">Name of the changed register table</param>
        /// <param name="campo">Field that will be changed</param>
        /// <param name="valor">New field value</param>
        /// <param name="condicao">Condition that identifies the records that will be changed</param>
        public void changeFieldValue(string system, string table, string Qfield, object Qvalue, CriteriaSet condition)
        {
            if (Log.IsDebugEnabled) Log.Debug(string.Format("Altera o valor do campo na tabela. [tabela] {0} [campo] {1} [valor] {2}", table, Qfield, Qvalue));

            UpdateQuery query = new UpdateQuery()
                .Update(system, table)
                .Set(Qfield, Qvalue)
                .Where(condition);

            Execute(query);
        }

        public void changeValueDocums(object keyValueDocums, byte[] file, string fileName, string extension, string Qversion, string operChange)
        {
            changeValueDocums(keyValueDocums, file, fileName, extension, Qversion, operChange, "DEFAULT");
        }

        public void changeValueDocums(object keyValueDocums, byte[] file, string fileName, string extension, string Qversion, string operChange, string alias)
        {
            if (Log.IsDebugEnabled)
                Log.Debug(string.Format("Altera o documento. [ficheiro] {0}", fileName));

            // RS(2010.09.16) The table docums starts to save several versions and the author of the document
            string tabelaDocums = "docums";

            UpdateQuery query = new UpdateQuery()
                .Update(tabelaDocums)
                .Set("document", file)
                .Set("docpath", DBNull.Value)
                .Set("nome", fileName)
                .Set("tamanho", file.Length)
                .Set("versao", Qversion)
                .Set("opermuda", operChange)
                .Set("extensao", extension)
                .Set("datamuda", DateTime.Now)
                .Where(CriteriaSet.And()
                    .Equal(tabelaDocums, "coddocums", keyValueDocums));

            Execute(query);
        }

        /********************************GET***********************************/

        public Listing anotherSelect(string identifier, Listing Qlisting, CriteriaSet conditions, int nrRecords, int offset, Area area)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("PersistentSupport.seleccionar. [id] {0}", identifier));

                SelectQuery querySelect = new SelectQuery();
                ControlQueryDefinition queryGenio = controlQueries[identifier];

                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](Qlisting.User, Qlisting.Module, conditions, Qlisting.QuerySort, this);
                    //AV 2009/11/20 The query goes on to just get the ordering that comes from the genio
                    //when no one has been set on the override
                    if (querySelect.OrderByFields.Count == 0 && Qlisting.QuerySort != null)
                    {
                        foreach (ColumnSort sort in Qlisting.QuerySort)
                        {
                            querySelect.OrderByFields.Add(sort);
                        }
                    }
                    if (nrRecords > 0)
                    {
                        querySelect.PageSize(nrRecords);
                    }
                }
                else
                {
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, nrRecords, conditions, Qlisting.QuerySort, queryGenio.Distinct);
                }
                querySelect.Offset(offset);
                QueryUtils.SetInnerJoins(Qlisting.RequestedFields, conditions, area, querySelect);

                DataMatrix ds = Execute(querySelect);
                Qlisting.DataMatrix = ds.DbDataSet;
                Qlisting.LastFilled = ds.NumRows;
                if (Qlisting.obterTotal)
                {
                    Qlisting.TotalRecords = DBConversion.ToInteger(ExecuteScalar(QueryUtils.buildQueryCount(querySelect)));
                }
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.anotherSelect",
					"Error selecting " + nrRecords.ToString() + " records with listing " + Qlisting.ToString() + " where " + conditions.ToString() + " inner join with area " + area.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.anotherSelect",
					"Error selecting " + nrRecords.ToString() + " records with listing " + Qlisting.ToString() + " where " + conditions.ToString() + " inner join with area " + area.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Fills the foreign area with the information of the requested fields, by positioning the query in a baseArea.
        /// This allows to get information about upper tables.
        /// </summary>
        /// <param name="foreignArea">The area that we want to fill</param>
        /// <param name="baseArea">The base area of the query</param>
        /// <param name="conditions">Criteriaset</param>
        /// <param name="requestedFields">The requested fields</param>
        public void fillInfoForForeignKey(Area foreignArea, Area baseArea, CriteriaSet conditions, List<string> requestedFields)
        {
            try
            {
                SelectQuery query = new SelectQuery();
                query.SelectDatabaseFields(baseArea, requestedFields.ToArray());
                query.From(baseArea.QSystem, baseArea.TableName, baseArea.TableName)
                    .Where(conditions);

                QueryUtils.SetInnerJoins(requestedFields.ToArray(), conditions, baseArea, query);

                DataMatrix mx = Execute(query);

                for (int i = 0; i < mx.NumRows; i++)
                {
                    // we assume all requested fields belong to the area!
                    for (int j = 0; j < mx.NumCols; j++)
                    {
                        // Don't take this condition off. Explanation: The user can remove columns from an area in the settings
                        // and the database still to be had on the corresponding table. If the user querys with "*", the
                        // below call would try to search in the corresponding Area structure all fields returned by SQL query.
                        // Since this field no longer exists in the structure, an exception would obviously arise from this.
                        if (foreignArea.DBFields.ContainsKey(query.SelectFields[j].Alias.Split('.')[1])) //I'm assuming that the keys are the long names and that's never going to change
                        {
                            foreignArea.insertNameValueField(query.SelectFields[j].Alias, mx.GetDirect(i, j));
                        }
                    }
                }
            }
            catch (PersistenceException ex)
            {
                closeConnection();
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.fillInfoForForeignKey",
					"Error filling info on fields " + requestedFields + " from table " + baseArea.TableName + " where " + conditions.ToString() + " into table " + foreignArea.TableName + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                closeConnection();
				throw new PersistenceException(null, "PersistentSupport.fillInfoForForeignKey",
					"Error filling info on fields " + requestedFields + " from table " + baseArea.TableName + " where " + conditions.ToString() + " into table " + foreignArea.TableName + ": " + ex.Message, ex);
            }
        }

        public void PrepareQuerySelect(string identifier, string[] fieldsRequested, IList<ColumnSort> sorting, bool distinct, int numRegs, int offset, Area area)
        {
            SelectQuery querySelect = new SelectQuery();

            if (!controlQueries.ContainsKey(identifier))
            {
                // Set requested fields
                querySelect.SelectDatabaseFields(area, fieldsRequested);

                // Set order by
                querySelect.OrderByFields.Clear();
                if (sorting != null)
                {
                    foreach (ColumnSort sort in sorting)
                    {
                        querySelect.OrderByFields.Add(sort);
                    }
                }

                // Distinct set
                querySelect.Distinct(distinct);

                // Set pagination
                if (numRegs > 0)
                {
                    querySelect.PageSize(numRegs + 1);
                    querySelect.Offset(offset);
                }

                ControlQueryDefinition cqd = new ControlQueryDefinition(querySelect.SelectFields, querySelect.FromTable, querySelect.Joins, querySelect.WhereCondition);

                adicionaControlo(identifier, cqd);
            }
        }

        public Listing select(string identifier, Listing Qlisting, CriteriaSet conditions, int nrRecords, Boolean noLock)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("PersistentSupport.seleccionar. [id] {0}", identifier));

                SelectQuery querySelect = new SelectQuery();
				querySelect.noLock= noLock;
                ControlQueryDefinition queryGenio = controlQueries[identifier];
                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](Qlisting.User, Qlisting.Module, conditions, Qlisting.QuerySort, this);
                    //AV 2009/11/20 The query goes on to just get the ordering that comes from the genio
                    //when no one has been set on the override
                    if (querySelect.OrderByFields.Count == 0 && Qlisting.QuerySort != null)
                    {
                        foreach (ColumnSort sort in Qlisting.QuerySort)
                        {
                            querySelect.OrderByFields.Add(sort);
                        }
                    }
                    if (nrRecords > 0)
                    {
                        querySelect.PageSize(nrRecords);
                    }
                }
                else
                {
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, nrRecords, conditions, Qlisting.QuerySort, queryGenio.Distinct);
                }

                DataMatrix ds = Execute(querySelect);
                Qlisting.DataMatrix = ds.DbDataSet;
                Qlisting.LastFilled = ds.NumRows;
                if(Qlisting.obterTotal)
                {
                    Qlisting.TotalRecords = DBConversion.ToInteger(ExecuteScalar(QueryUtils.buildQueryCount(querySelect)));
                }
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionar",
					"Error selecting " + nrRecords.ToString() + " records with listing " + Qlisting.ToString() + " where " + conditions + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.seleccionar",
					"Error selecting " + nrRecords.ToString() + " records with listing " + Qlisting.ToString() + " where " + conditions + ": " + ex.Message, ex);
            }
        }

        /********************************GETNIVEL***********************************/
        /// <summary>
        /// Function to obtain a set of records
        /// </summary>
        /// <param name="areaNivel">area used in the level of the tree being requested</param>
        /// <param name="camposPedido">tree columns</param>
        /// <param name="listagem">Qlisting of desired objects</param>
        /// <param name="condicoes">conditions</param>
        /// <param name="condChavePai">condition to identify which branch has expanded</param>
        /// <returns>a Qlisting with the objects filled in</returns>
        public Listing selectLevel(IArea areaLevel, IList<SelectField> fieldsRequested, Listing Qlisting, CriteriaSet conditions, string parentKeyCond)
        {
            //Last updated by [CJP] at [30.09.2014] - Support for new Qweb_ctldados.js
            try
            {
                Log.Debug("Selecciona registos para um nível da árvore.");

                SelectQuery querySel = new SelectQuery();
                StringBuilder fields = new StringBuilder();

                //Assume that the parent-owned condition is the first condition
                string[] keyValue = parentKeyCond.Replace("[","").Split('=');

                string[] nomeChave;
                string chavePai;
                bool paiIsNivel = false;
                if (keyValue[0] != "")
                {
                    nomeChave = keyValue[0].Split('.');
                    if (nomeChave[0] != areaLevel.Alias)
                    {
                        chavePai = areaLevel.ParentTables[nomeChave[0]].SourceRelField;
                    }
                    else
                    {
                        paiIsNivel = true;
                        chavePai = nomeChave[1];
                    }
                }
                else
                {
                    chavePai = "";
                }

                querySel.From(areaLevel.QSystem, areaLevel.TableName, areaLevel.Alias)
                    .Where(CriteriaSet.And());
                if (areaLevel.Information.TreeTable == null) //it's not table in tree
                {
                    if (parentKeyCond != null && !keyValue[1].Equals("''")) //exists a previous level
                    {
                        querySel.WhereCondition.Equal(areaLevel.Alias, chavePai, keyValue[1].Trim('\''));
                    }

                }
                else //table in tree
                {
                    //WARNING - In Queries, only the field designation can be
                    //          Insert usValue, one must enter [table designation]. [field designation]
                    string levelFullname = areaLevel.Information.TreeTable.RecordLevelField;
                    string[] level = levelFullname.Split('.');
                    if (!keyValue[1].Equals("''")) //exists a previous level
                    {
                        if (paiIsNivel)
                        {
                            string sigla = areaLevel.Information.TreeTable.DesignationField;
                            string[] pai = areaLevel.Information.TreeTable.ParentTableField.Split('.');
                            areaLevel.insertNameValueField(keyValue[0], null);
                            areaLevel.insertNameValueField(sigla, null);
                            areaLevel.insertNameValueField(levelFullname, null);

                            //Devlove the father's record, from his primary key
                            getRecord(areaLevel, keyValue[1].Trim('\''));
                            querySel.WhereCondition.Equal(areaLevel.Alias, level[1], Convert.ToInt32(areaLevel.returnValueField(levelFullname)) + 1);
                            querySel.WhereCondition.Equal(areaLevel.Alias, pai[1], areaLevel.returnValueField(sigla));
                        }
                        else
                        {
                            querySel.WhereCondition.Equal(areaLevel.Alias, chavePai, keyValue[1].Trim('\''));
                            querySel.WhereCondition.Equal(areaLevel.Alias, level[1], 1);
                        }
                    }
                    else //this is the first level of the tree
                    {
                        querySel.WhereCondition.Equal(areaLevel.Alias, level[1], 1);
                    }
                }

                foreach (SelectField field in fieldsRequested)
                {
                    querySel.SelectFields.Add(field);
                }
                foreach (ColumnSort sort in Qlisting.QuerySort)
                {
                    querySel.OrderByFields.Add(sort);
                }

                //Removes the first condition from the query (the condition of the parent key)
                conditions.SubSets.RemoveAt(0);
                querySel.WhereCondition.SubSet(conditions);

                DataMatrix ds = Execute(querySel);
                Qlisting.DataMatrix = ds.DbDataSet;
                Qlisting.LastFilled = ds.NumRows;
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionarNivel",
											   "Error selecting records - " + string.Format("[areaNivel] {0}; [camposPedido] {1}; [listagem] {2}; [condicoes] {3}; [condChavePai] {4}: ",
																							areaLevel.ToString(), fieldsRequested.ToString(), Qlisting.ToString(), conditions.ToString(), parentKeyCond) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.seleccionarNivel",
											   "Error selecting records - " + string.Format("[areaNivel] {0}; [camposPedido] {1}; [listagem] {2}; [condicoes] {3}; [condChavePai] {4}: ",
																							areaLevel.ToString(), fieldsRequested.ToString(), Qlisting.ToString(), conditions.ToString(), parentKeyCond) + ex.Message, ex);
            }
        }

        /********************************GET+***********************************/
        /// <summary>
        /// Function to obtain a set of records
        /// </summary>
        /// <param name="identificador">identifier of the query that the case generates</param>
        /// <param name="listagem">Qlisting of desired objects</param>
        /// <param name="condicoes">conditions</param>
        /// <param name="nrRegistos">number of desired objects</param>
		/// <param name="ultimaLida">last row read</param>
		/// <param name="chavePrimaria">primary key of the last sheet read</param>
        /// <returns>a Qlisting with the objects filled in</returns>
        public Listing selectMore(string identifier, Listing Qlisting, CriteriaSet conditions, int nrRecords, int lastRead, string primaryKey)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Selecciona mais registos. [id] {0}", identifier));
                ControlQueryDefinition queryGenio = controlQueries[identifier];
                SelectQuery querySelect = new SelectQuery();

                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](Qlisting.User, Qlisting.Module, conditions, Qlisting.QuerySort, this);
                    //AV 2009/11/20 The query goes on to just get the ordering that comes from the genio
                    //when no one has been set on the override
                    if (querySelect.OrderByFields.Count == 0)
                    {
                        foreach (ColumnSort sort in Qlisting.QuerySort)
                        {
                            querySelect.OrderByFields.Add(sort);
                        }
                    }
                    if (nrRecords > 0)
                    {
                        querySelect.PageSize(nrRecords);
                    }
                }
                else
                {
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, nrRecords, conditions, Qlisting.QuerySort, queryGenio.Distinct);
                }
                querySelect.Offset(lastRead);

                DataMatrix ds = Execute(querySelect);
                Qlisting.DataMatrix = ds.DbDataSet;
                Qlisting.LastFilled = ds.NumRows;
                if(Qlisting.obterTotal)
                {
                    Qlisting.TotalRecords = DBConversion.ToInteger(ExecuteScalar(QueryUtils.buildQueryCount(querySelect)));
                }
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.seleccionarMais",
											   "Error selecting records - " + string.Format("[identificador] {0}; [listagem] {1}; [condicoes] {2}; [nrRegistos] {3}; [ultimaLida] {4}; [chavePrimaria] {5}: ",
																							identifier, Qlisting.ToString(), conditions.ToString(), nrRecords.ToString(), lastRead.ToString(), primaryKey) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.seleccionarMais",
											   "Error selecting records - " + string.Format("[identificador] {0}; [listagem] {1}; [condicoes] {2}; [nrRegistos] {3}; [ultimaLida] {4}; [chavePrimaria] {5}: ",
																							identifier, Qlisting.ToString(), conditions.ToString(), nrRecords.ToString(), lastRead.ToString(), primaryKey) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Method that returns the number of records in a query
        /// </summary>
        /// <param name="identificador">query identifier</param>
        /// <param name="listagem">Qlisting with Qresult data</param>
        /// <param name="condicoes">conditions</param>
        /// <returns>nr of records</returns>
        public int count(string identifier, Listing Qlisting, CriteriaSet conditions)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("Conta o número de registos. [id] {0}", identifier));

                ControlQueryDefinition queryGenio = controlQueries[identifier];
                SelectQuery querySelect = new SelectQuery();

                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](Qlisting.User, Qlisting.Module, conditions, null, this);
                }
                else
                {
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, 1, conditions, null, queryGenio.Distinct);
                }

                DataMatrix mx = Execute(QueryUtils.buildQueryCount(querySelect));
                return mx.GetInteger(0, 0);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.contar",
											   string.Format("Error counting records - [identificador] {0}; [listagem] {1}; [condicoes] {2}: ", identifier, Qlisting.ToString(), conditions.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.contar",
											   string.Format("Error counting records - [identificador] {0}; [listagem] {1}; [condicoes] {2}: ", identifier, Qlisting.ToString(), conditions.ToString()) + ex.Message, ex);
            }
        }

        /*****************************************GETP***********************************************/
		/// <summary>
        /// Function to obtain a set of records
        /// </summary>
		/// <param name="utilizador">user who triggered the request</param>
		/// <param name="modulo">module that triggered the request</param>
		/// <param name="area">Qlisting area</param>
        /// <param name="ordenacao">columns to sort</param>
		/// <param name="valorChavePrimaria">value of the primary key</param>
        /// <param name="condicoes">conditions</param>
		/// <param name="identificador">identifier of the query that the case generates</param>
        /// <returns>a Qlisting with the objects filled in</returns>
        public virtual int getRecordPos(User user, string module, IArea area, IList<ColumnSort> sorting, string primaryKeyValue, CriteriaSet conditions, string identifier)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("getRecordPos. [id] {0}", identifier));

                ControlQueryDefinition queryGenio = controlQueries[identifier];
                SelectQuery querySelect = new SelectQuery();

                if (controlQueriesOverride.ContainsKey(identifier))
                {
                    querySelect = controlQueriesOverride[identifier](user, module, conditions, sorting, this);
                }
                else
                {
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, 0, conditions, null, queryGenio.Distinct);
                }

                SelectQuery posQuery = new SelectQuery();

                var colSortPK = new ColumnSort(new ColumnReference(area.Alias, area.PrimaryKeyName), GenericSortOrder.Ascending);
                if (!sorting.Contains(colSortPK))
                    sorting.Add(colSortPK);
                var orderby = new ColumnSort[sorting.Count];
                for(int i = 0; i < sorting.Count; i++) { orderby[i] = sorting[i]; }// Can be replaced with Linq

                querySelect.Select(SqlFunctions.RowNumber(orderby), "order");

                posQuery.Select("subq", "order");
                posQuery.From(querySelect, "subq");
                posQuery.Where(CriteriaSet.And().Equal("subq", area.Alias + "." +area.PrimaryKeyName, primaryKeyValue));

                DataMatrix mx = Execute(posQuery);
                return mx.NumRows == 0 ? 0 : mx.GetInteger(0, 0);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions.ToString(), identifier) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions.ToString(), identifier) + ex.Message, ex);
			}
        }

        /// <summary>
        /// Gets the query of the row number for getting position of the record in the listing so that it can determine the page where it is located
        /// </summary>
        /// <param name="areaBase">Area base of the list</param>
        /// <param name="pkValue">Primary key value of the record to find</param>
        /// <param name="orderby">Order by clause</param>
        /// <param name="where">Where clause</param>
        /// <param name="ephs">EPH condition</param>
        /// <returns>-1 or the row number if found</returns>
        public SelectQuery getQueryPagingPos(AreaInfo areaBase, string pkValue, List<ColumnSort> orderby, CriteriaSet where, CriteriaSet ephs)
        {
            if (areaBase == null) return null;

            if (orderby == null) orderby = new List<ColumnSort>();

            var pk = new ColumnReference(areaBase.Alias, areaBase.PrimaryKeyName);
            var colSortPK = new ColumnSort(pk, GenericSortOrder.Ascending);

            var containsPK = orderby.Contains(colSortPK);
            var sortsCount = orderby.Count + (containsPK ? 0 : 1);

            var sorts = new ColumnSort[sortsCount];
            orderby.CopyTo(sorts);
            if (!containsPK) sorts[sortsCount - 1] = colSortPK;

            where.SubSet(ephs);

            var subQuery = new SelectQuery()
                            .Select(SqlFunctions.RowNumber(sorts), "rn")
                            .From(areaBase.TableName, areaBase.Alias)
                            .Where(where);
            subQuery.noLock = true;

            // Joins
            var requestedFields = new string[] { };
            if (orderby.Count > 0)
            {
                requestedFields = new string[orderby.Count];
                for (int i = 0; i < orderby.Count; i++)
                {
                    var reqField = (ColumnReference)orderby[i].Expression;
                    requestedFields[i] = reqField.TableAlias + "." + reqField.ColumnName;
                }
            }
            QueryUtils.SetInnerJoins(requestedFields, where, Area.createArea(areaBase.Alias, null, null), subQuery);

            return subQuery;
        }

        /// <summary>
        /// Gets the position of the record in the listing so that it can determine the page where it is located
        /// </summary>
        /// <param name="areaBase">Area base of the list</param>
        /// <param name="pkValue">Primary key value of the record to find</param>
        /// <param name="orderby">Order by clause</param>
        /// <param name="where">Where clause</param>
        /// <param name="ephs">EPH condition</param>
        /// <returns>-1 or the row number if found</returns>
        public int getPagingPos(AreaInfo areaBase, string pkValue, List<ColumnSort> orderby, CriteriaSet where, CriteriaSet ephs, IList<TableJoin> Joins = null, FieldRef firstVisibleColumn = null)
        {
            // 'orderby' may arrive null.
            if (orderby == null)
                orderby = new List<ColumnSort>();

            // No user-selected sorting method
            if (orderby.Count == 0)
            {
                // Condition for field type added because sorting by an image field causes an error
                if (firstVisibleColumn != null
                    && CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.IMAGE
                    && CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.GEOGRAPHY_POINT
                    && CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.GEOGRAPHY_SHAPE
                    && CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.GEOMETRY_SHAPE)
				{
                    ColumnSort sortFirstVisibleColumn = new ColumnSort(new ColumnReference(firstVisibleColumn), GenericSortOrder.Ascending);
                    orderby.Add(sortFirstVisibleColumn);
                }
            }

            SelectQuery subQuery = getQueryPagingPos(areaBase, pkValue, orderby, where, ephs);
            if (subQuery == null) return -1;

			if (Joins != null)
                subQuery.Join(Joins);

            // The select of the primary key is isolated for allow reuse the getQueryPagingPos if it is needed.
            var pk = new ColumnReference(areaBase.Alias, areaBase.PrimaryKeyName);
            subQuery.Select(pk, "pk");

            var query = new SelectQuery()
                .Select("x", "rn", "rowNumber")
                    .From(subQuery, "x")
                        .Where(CriteriaSet.And().Equal("x", "pk", pkValue));

            return Convert.ToInt32(this.ExecuteScalar(query) ?? -1);
        }

        /// <summary>
        /// Method that returns the hashtable with the querys generated by the case
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, ControlQueryDefinition> getControlQueries()
        {
            return controlQueries;
        }

        /// <summary>
        /// Method that returns the hashtable with the querys generated by the case
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, overrideDbeditQuery> getControlQueriesOverride()
        {
            return controlQueriesOverride;
        }


        /*****************************Tabelas shadow***********************************/

        /// <summary>
        /// Copys the record into the shadow table
        /// </summary>
        /// <param name="area">Area of the record</param>
		/// <param name="user">Name of the user</param>
		/// <param name="functionType">Function type</param>
        public void requestTabShadow(IArea area, string user, FunctionType functionType)
        {
            try
            {
                //Create the primary key
                string cod = codIntInsertion(area, true);

                //build the insert query
                InsertQuery query = new InsertQuery();
                QueryUtils.buildQueryInsertShadow(query, area, functionType, user);

                //shadow primary key
                query.Value(area.ShadowTabKeyName, QueryUtils.ToValidDbValue(cod, area.DBFields[area.PrimaryKeyName]));

                //insert into the database
                Execute(query);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.pedidoTabSombra",
				                               string.Format("Error duplicating record - [area] {0}; [utilizador] {1}; [tipoFuncao] {2}: ", area.ToString(), user, functionType.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.pedidoTabSombra",
				                               string.Format("Error duplicating record - [area] {0}; [utilizador] {1}; [tipoFuncao] {2}: ", area.ToString(), user, functionType.ToString()) + ex.Message, ex);
            }
        }


        public virtual List<A> searchListWhere<A>(CriteriaSet condition, User user, string[] fields) where A : IArea
        {
            return searchListWhere<A>(condition, user, fields, false);
        }

        public virtual List<A> searchListWhere<A>(CriteriaSet condition, User user, string[] fields, bool distinct) where A : IArea
        {
            return searchListWhere<A>(condition, user, fields, distinct, false);
        }

        public virtual List<A> searchListWhere<A>(CriteriaSet condition, User user, string[] fields, bool distinct, bool noLock) where A : IArea
        {
            try
            {
                List<A> Qresult = new List<A>();
                A area = (A)Activator.CreateInstance(typeof(A), user);

                SelectQuery qs = new SelectQuery();
                qs.Distinct(distinct);
                qs.SelectDatabaseFields(area, fields);

                if (condition != null)
                {
                    qs.Where(condition);
                }

                List<string> tabelasAcima = new List<string>();
                if (condition != null)
                {
                    QueryUtils.checkConditionsForForeignTables(condition, area as Area, tabelasAcima);
                }

                List<Relation> relations = QueryUtils.tablesRelationships(tabelasAcima, area);
                QueryUtils.setFromTabDirect(qs, relations, area);

                qs.noLock = noLock;

                DataMatrix mx = Execute(qs);
                for (int i = 0; i < mx.NumRows; i++)
                {
                    area = (A)Activator.CreateInstance(typeof(A), user);
                    for (int j = 0; j < mx.NumCols; j++)
                    {
                        // Don't take this condition off. Explanation: The user can remove columns from an area in the settings
                        // and the database still to be had on the corresponding table. If the user querys with "*", the
                        // below call would try to search in the corresponding Area structure all fields returned by SQL query.
                        // Since this field no longer exists in the structure, an exception would obviously arise from this.
                        if (area.DBFields.ContainsKey(qs.SelectFields[j].Alias.Split('.')[1])) //I'm assuming that the keys are the long names and that's never going to change
                        {
                            area.insertNameValueField(qs.SelectFields[j].Alias, mx.GetDirect(i, j), fromDatabase: true);
                        }
                    }
                    Qresult.Add(area);
                }

                return Qresult;
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, $"PersistentSupport.searchListWhere for {typeof(A).Name}",
				                               string.Format("Error getting records - [condicao] {0}; [utilizador] {1}; [campos] {2}; [distinct] {3}: ", condition?.ToString(), user.ToString(), fields, distinct) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, $"PersistentSupport.searchListWhere for {typeof(A).Name}",
				                               string.Format("Error getting records - [condicao] {0}; [utilizador] {1}; [campos] {2}; [distinct] {3}: ", condition?.ToString(), user.ToString(), fields, distinct) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Generics invocation of the searchListWhere method
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="condition">The condition.</param>
        /// <param name="user">The user.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <param name="noLock">if set to <c>true</c> [no lock].</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public virtual List<Area> genericSearchListWhere<A>(CriteriaSet condition, User user, string[] fields, bool distinct, bool noLock) where A : IArea
        {
            return searchListWhere<A>(condition, user, fields, distinct, noLock).Cast<Area>().ToList();
        }

        /// <summary>
        /// Run a select query over specified listing, while applying the given conditions.
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="condicao">The CriteriaSet holding the conditions</param>
        /// <param name="listing">The ListingMVC</param>
        public virtual void searchListAdvancedWhere<A>(CriteriaSet condition, ListingMVC<A> listing) where A : IArea
        {
            try
            {
                int totalRec = 0;
                SelectQuery qs = null;
                List<A> Qresult = null;

                Type funcObj = typeof(GenioServer.framework.OverrideQuery);
                MethodInfo funcOver = null;

                // TODO: Tem em conta a possibilidade da existencia do Override ao obter os valores das colunas abaixo
                if(!string.IsNullOrEmpty(listing.identifier))
                    funcOver = funcObj.GetMethod(listing.identifier);

                if (funcOver != null)
                {
                    if (funcOver.ContainsGenericParameters)
                        funcOver = funcOver.MakeGenericMethod(typeof(A));

                    object[] parameters = new object[4];
                    parameters[0] = listing;
                    if(listing.PagingPosEPHs != null)
                        condition.SubSet(listing.PagingPosEPHs);
                    parameters[1] = condition;//CriteriaSet
                    parameters[2] = this;//PersistentSupport
                    parameters[3] = 0; //this will be an output parameter after invoke of the reflection?s method

                    GenioServer.framework.OverrideQuery ovr = new GenioServer.framework.OverrideQuery();
                    Qresult = (List<A>)funcOver.Invoke(ovr, parameters);
                    totalRec = (int) parameters[3];
                }
                else
                {
                    qs = getSelectQueryFromListingMVC(condition, listing);
                    DataMatrix mx = Execute(qs);
                    Qresult = mx.GetList<A>(listing.User);
                }

                listing.Rows = Qresult;// One more record is always selected to check if exist more pages.

                // Having totalizers enabled for this listing means that it is always necessary to do a separate query that adds the column values.
                if (listing.FieldsWithTotalizer.Count > 0)
                {
                    A baseArea = (A)Activator.CreateInstance(typeof(A), listing.User);
                    DataMatrix data = Execute(QueryUtils.buildQueryTotalizers(qs, listing.FieldsWithTotalizer, listing.SelectedRecords, baseArea));

                    listing.SetCountAndTotalizers(data);
                }
                else
                {
                    //RS(16.06.2017) If we fetch all the records there is no need to execute a seperate count query
                    // The same can be done in the first page if we return less records then the page size
                    if (listing.NumRegs <= 0 || (listing.Offset == 0 && listing.Rows.Count < listing.NumRegs))
                        listing.TotalRecords = listing.Rows.Count;
                    else if (listing.GetTotal && funcOver == null)
                    {
                        listing.TotalRecords = DBConversion.ToInteger(ExecuteScalar(QueryUtils.buildQueryCount(qs)));
                    }
                    else
                        listing.TotalRecords = totalRec;
                }
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, $"PersistentSupport.searchListWhere for {typeof(A).Name}",
                                               string.Format("Error getting records - [condicao] {0}; [listing] {1}: ", condition?.ToString(), listing.ToString()) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, $"PersistentSupport.searchListWhere for {typeof(A).Name}",
                                               string.Format("Error getting records - [condicao] {0}; [listing] {1}: ", condition?.ToString(), listing.ToString()) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Builds the SelectQuery necessary to search in the given listing, while applying the necessary conditions.
        ///
        /// If the base area of the search, does not have any relationship to one of its related areas (specified in the fields or in the conditions),
        /// the base search must change, and the unrelated area will be the area to search. This is necessary for queries on upper tables,
        /// but applying conditions from lower tables, these lower tables are the one that have relantionships to the upper, and therefore the inner joins will be
        /// applied.
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="condicao">The CriteriaSet holding the conditions</param>
        /// <param name="listing">The ListingMVC</param>
        /// <returns>SelectQuery</returns>
        public SelectQuery getSelectQueryFromListingMVC<A>(CriteriaSet condition, ListingMVC<A> listing) where A : IArea
        {
            A area = (A)Activator.CreateInstance(typeof(A), listing.User);

            SelectQuery qs = new SelectQuery();

			//NH(2016.09.26) - Set the nolock option
			qs.noLock = listing.NoLock;

            qs.Join(listing.Joins);

            // Sets the fields of the SelectQuery
            qs.SelectDatabaseFields(area, listing.RequestFields);

            // Sets the conditions of the SelectQuery
            setWhereCondition(condition, qs);

            // Checks for foreign tables in fields and conditions
            List<string> relatedTables = checkRelations<A>(condition, listing, area);

            // If the base area does not have relationship with all other foreign areas, then the base area should be changed
            bool areaHasRelationsWithAllOtherAreas = checkPathToRelations(relatedTables, area);

            if(!areaHasRelationsWithAllOtherAreas)
                setFromReversed<A>(relatedTables, area, qs);
            else
                setFrom<A>(relatedTables, area, qs);

            // Sets the ordering, distinct if any, and pagination of the selectQuery
            setOrderDistinctAndPagination<A>(listing, qs);

            return qs;
        }

        /// <summary>
        /// Sets the where condition in a select query, if any.
        /// </summary>
        /// <param name="condicao">The CriteriaSet condition</param>
        /// <param name="qs">The SelectQuery</param>
        private static void setWhereCondition(CriteriaSet condition, SelectQuery qs)
        {
            if (condition != null)
            {
                qs.Where(condition);
            }
        }

        /// <summary>
        /// Checks on conditions and fields used on the selectquery for tables diferent from the base area
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="condicao">The CriteriaSet holding the conditions</param>
        /// <param name="listing">The ListingMVC</param>
        /// <param name="area">The base area</param>
        /// <returns>A list of strings with all related areas</returns>
        private List<string> checkRelations<A>(CriteriaSet condition, ListingMVC<A> listing, A area) where A : IArea
        {
            List<string> otherTables = new List<string>();
            if (listing.RequestFields != null)
                QueryUtils.checkFieldsForForeignTables(listing.RequestFieldsAsStringArray, area as Area, otherTables);
            if (condition != null)
                QueryUtils.checkConditionsForForeignTables(condition, area as Area, otherTables);
            return otherTables;
        }

        /// <summary>
        /// Checks if all the given relations are related to the base area, if it is returns true, false otherwise
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="relatedTables">List of other tables</param>
        /// <param name="area"></param>
        /// <returns>True or false</returns>
        private bool checkPathToRelations<A>(List<string> relatedTables, A area) where A : IArea
        {
            foreach (string otherTable in relatedTables)
            {
                List<Relation> relations = area.Information.GetRelations(otherTable);
                if (relations == null || relations.Count == 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the from table on the select query, and also all the necessary joins to perform the query
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="relatedTables">List of other tables</param>
        /// <param name="area">The base area</param>
        /// <param name="qs">The select query</param>
        private void setFrom<A>(List<string> relatedTables, A area, SelectQuery qs) where A : IArea
        {
            List<Relation> relations = QueryUtils.tablesRelationships(relatedTables, area);

            QueryUtils.setFromTabDirect(qs, relations, area);
        }

        /// <summary>
        /// At this stage there should be only 1 area that on the other tables, but the base area does
        /// not have any relationship with it. But, the other area has a relationship to the base area,
        /// which means that, that one area should be considered for the base area for select query.
        /// All other relationships between these 2 areas should be applied.
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="relatedTables">List of other tables</param>
        /// <param name="area">The base area</param>
        /// <param name="qs">The select query</param>
        private void setFromReversed<A>(List<string> otherTables, A area, SelectQuery qs) where A : IArea
        {
            if(otherTables.Count == 1)
            {
                string principalArea = otherTables[0];
                //Type anotherArea = Type.GetType("CSGenio.business.CSGenioA" + principalArea + ", GenioServer");
                //IArea anotherIArea = Activator.CreateInstance(anotherArea, area.User) as IArea;
                IArea anotherIArea = Area.createArea(principalArea, area.User, area.User.CurrentModule);

                List<Relation> relations = invertRelacoesTabelas(area, anotherIArea);
                QueryUtils.setFromTabDirect(qs, relations, anotherIArea);
            }
            else
                setFrom<A>(otherTables, area, qs);
        }

        /// <summary>
        /// Returns all the relationships between the otherArea and area
        /// </summary>
        /// <param name="area">The base area</param>
        /// <param name="anotherIArea">The other area</param>
        /// <returns>List of Relation between anotherArea and area</returns>
        private List<Relation> invertRelacoesTabelas(IArea area, IArea anotherIArea)
        {
            List<Relation> relations = new List<Relation>();

            AreaInfo ai = anotherIArea.Information;
            List<Relation> upperRelations = ai.GetRelations(area.Alias);
            if (upperRelations != null)
            {
                foreach (Relation rel in upperRelations)
                    if (!relations.Contains(rel))
                    {
                        relations.Add(rel);
                    }
            }

            return relations;
        }

        /// <summary>
        /// Sets the order fields, distinct results and pagination to a selectQuery
        /// </summary>
        /// <typeparam name="A">The IArea which this search will be applied to</typeparam>
        /// <param name="listing">The ListingMVC</param>
        /// <param name="qs">SelectQuery</param>
        private static void setOrderDistinctAndPagination<A>(ListingMVC<A> listing, SelectQuery qs) where A : IArea
        {
            qs.OrderByFields.Clear();
            if (listing.Sorts != null)
            {
                foreach (ColumnSort sort in listing.Sorts)
                {
                    qs.OrderByFields.Add(sort);
                }
            }

            qs.Distinct(listing.Distinct);

            if (listing.NumRegs > 0)
            {
                qs.PageSize(listing.NumRegs + 1);
                qs.Offset(listing.Offset);
            }
        }

        /// <summary>
        /// Gets data from a record from the primary key
        /// </summary>
        /// <param name="area">The area to be filled with the values</param>
        /// <param name="internalCodeValue">The value of the primary key with which we position the record</param>
        /// <param name="fields">The fields to fill in the area. Null to get all fields</param>
        /// <param name="forUpdate">True if you are preparing to update this record, false otherwise</param>
        /// <param name="bookmarkOnly">True if the current values should be perserved and only oldvalues or missing values should be read</param>
        /// <returns>True if the record was correctly positioned, false otherwise</returns>
        public virtual bool getRecord(IArea area, object internalCodeValue, string[] fields=null, bool forUpdate=false, bool bookmarkOnly=false)
        {
            try
            {
                string[] fields2 = fields;
                if (bookmarkOnly)
                    fields2 = GetFieldsForBookmark(area, fields, forUpdate);

                SelectQuery select = new SelectQuery();
                if (forUpdate)
                    select.updateLock = true;

                select.SelectDatabaseFields(area, fields2);

                //if we end up with zero fields to read then we don't need to execute the query
                if (select.SelectFields.Count == 0)
                    return true;

                select.From(area.QSystem, area.TableName, area.Alias);
                var pk = QueryUtils.ToValidDbValue(internalCodeValue, area.DBFields[area.PrimaryKeyName]);
                select.Where(CriteriaSet.And()
                    .Equal(area.Alias, area.PrimaryKeyName, pk));

                DataMatrix mx = Execute(select);
                if (mx.NumRows == 0)
                    return false;

                for (int i = 0; i < mx.NumCols; i++)
                {
                    // Don't take this condition off. Explanation: The user can remove columns from an area in the settings
                    // and the database still to be had on the corresponding table. If the user querys with "*", the
                    // below call would try to search in the corresponding Area structure all fields returned by SQL query.
                    // Since this field no longer exists in the structure, an exception would obviously arise from this.
                    if(area.DBFields.TryGetValue(select.SelectFields[i].Alias.Split('.')[1], out var fieldInfo))
                    {
                        var internalValue = DBConversion.ToInternal(mx.GetDirect(0, i), fieldInfo.FieldFormat);
                        if (area.Fields.TryGetValue(fieldInfo.FullName, out var dbfield) && bookmarkOnly)
                            dbfield.OldValue = internalValue;
                        else 
                            area.insertNameValueField(select.SelectFields[i].Alias, internalValue, fromDatabase: true);
                    }
                }

                //primary key is always the same
                area.insertNameValueField(area.PrimaryKeyName, internalCodeValue, fromDatabase: true);
                area.IsBookmarkLocked = forUpdate;

                return true;
            }
			catch (Exception ex)
            {
                string usermessage = (ex as GenioException)?.UserMessage;
                throw new PersistenceException(usermessage
                    , "PersistentSupport.getRecord"
                    , "Error selecting fields " + fields + " from table " + area.TableName + " where code is " + internalCodeValue.ToString() + ": " + ex.Message
                    , ex);
            }
        }

        private static string[] GetFieldsForBookmark(IArea area, string[] fields, bool forUpdate)
        {
            //add only non-bookmarked fields
            List<string> bookmarkFields = [];
            foreach (var fieldInfo in area.DBFields.Values)
            {
                //primary key cannot change, we already have it, so we never need to read it
                if (fieldInfo.Name == area.PrimaryKeyName)
                    continue;
                //if this is a update critical operation and the current bookmark was not update locked
                // we need to read all fields again
                //otherwise we read the fields that have not been bookmarked yet
                if (!(forUpdate && !area.IsBookmarkLocked)
                    && area.Fields.TryGetValue(fieldInfo.FullName, out var reqField) && reqField.IsBookmarked)
                    continue;
                //skip non-requested fields
                if (fields is not null && !fields.Contains(fieldInfo.Name))
                    continue;

                bookmarkFields.Add(fieldInfo.Name);
            }
            return bookmarkFields.ToArray();
        }

        /// <summary>
        /// Function that executes a query that returns a field
        /// </summary>
        /// <param name="query">query to run</param>
        /// <returns>the object returned by the query</returns>
        public virtual object executeScalar(string query, int timeout)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryEscalar] {0}.", query));

                IDbCommand comando = CreateCommand(query);

                return comando.ExecuteScalar();
            }
            catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaEscalar",
                                               "Error executing query '" + query +
                                               //(parameters == null ? "" : "' with parameters " + parameters.ToString()) +
                                                ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaEscalar",
                                               "Error executing query '" + query +
                                               //(parameters == null ? "" : "' with parameters " + parameters.ToString()) +
                                                ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that executes a query that returns a field
        /// </summary>
        /// <param name="query">query to run</param>
        /// <returns>the object returned by the query</returns>
        public virtual object executeScalar(string query)
        {
			return executeScalar(query, null);
        }

        /// <summary>
        /// Function that executes a query with parameters that returns a field
        /// </summary>
        /// <param name="query">query to run</param>
        /// <param name="parameters">query parameters</param>
        /// <returns>the object returned by the query</returns>
        public virtual object executeScalar(string query, List<IDbDataParameter> parameters)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryEscalar] {0}.", query));

                IDbCommand comando = CreateCommand(query, parameters);
                return comando.ExecuteScalar();
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaEscalar",
				                               "Error executing query '" + query +
											   (parameters == null ? "" : "' with parameters " + parameters.ToString())
											   + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaEscalar",
				                               "Error executing query '" + query +
											   (parameters == null ? "" : "' with parameters " + parameters.ToString())
											   + ": " + ex.Message, ex);
            }
        }

		public virtual object executeScalar(SelectQuery query)
        {
            return ExecuteScalar(query);
        }

        public virtual object ExecuteScalar(SelectQuery query)
        {
            try
            {
                DataMatrix mx = Execute(query);
                if (mx == null || mx.NumRows == 0)
                {
                    return null;
                }

                return mx.GetDirect(0, 0);
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.ExecuteScalar",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.ExecuteScalar",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
        }

		public virtual object ExecuteScalar(string query)
        {
            return executeScalar(query);
        }


        public ArrayList executeReaderOneColumn(SelectQuery query)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryUmaColuna] {0}.", query));

                ArrayList Qresult = new ArrayList();
                DataMatrix mx = Execute(query);
                for (int i = 0; i < mx.NumRows; i++)
                {
                    Qresult.Add(mx.GetDirect(i, 0));
                }
                return Qresult;
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaReaderUmaColuna",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaReaderUmaColuna",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
        }


        public ArrayList executeReaderOneRow(SelectQuery query)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryUmaLinha] {0}.", query));

                ArrayList Qresult = new ArrayList();
                DataMatrix mx = Execute(query);
                if (mx != null && mx.NumRows > 0)
                {
                    for (int i = 0; i < mx.NumCols; i++)
                    {
                        Qresult.Add(mx.GetDirect(0, i));
                    }
                }
                return Qresult;
            }
			catch (GenioException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaReaderUmaLinha",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaReaderUmaLinha",
				                               "Error executing query '" + query.ToString() + "': " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that performs a stored procedure that returns a field, with a default timeout of 300
        /// </summary>
        /// <param name="query">a stored procedure to run</param>
        /// <returns>value returned by executeNonQuery</returns>
        public virtual int executeStoredProcedure(string query)
        {
            int timeout = 300;
            return executeStoredProcedure(query, timeout);
        }

        /// <summary>
        /// Function that performs a stored procedure that returns a field
        /// </summary>
        /// <param name="query">a stored procedure to run</param>
        /// <param name="timeout">timeout value in ms</param>
        /// <returns>value returned by executeNonQuery</returns>
        public virtual int executeStoredProcedure(string query, int timeout)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[StoredProcedure] {0}.", query));

                IDbCommand comando = CreateCommand(query);
                //In the case of a stored procedure there will only be time out after 5 min
                comando.CommandTimeout = timeout;
                return comando.ExecuteNonQuery();
            }
			catch (GenioException ex)
            {
				throw new PersistenceException(ex.UserMessage, "PersistentSupport.executaStoredProcedure",
				                               "Error executing stored procedure '" + query + "' with timeout " + timeout.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(ex.Message, "PersistentSupport.executaStoredProcedure",
				                               "Error executing stored procedure '" + query + "' with timeout " + timeout.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Executes a stored procedure and returns a dataset if any is available by the procedure
        /// </summary>
        /// <param name="procedure">The name of the procedure to invoque</param>
        /// <param name="parameters">The parameters of the procedure to invoque</param>
        /// <param name="timeout"></param>
        /// <returns>A dataset with the result set of the stored procedure. It can be empty or ignored for write only stored procedures.</returns>
        public virtual DataMatrix ExecuteProcedure(string procedure, IList<IDbDataParameter> parameters = null, int timeout = 0)
        {
            IDbCommand command = CreateCommand("dbo." + procedure, parameters);
            command.CommandType = CommandType.StoredProcedure;
            if(timeout > 0)
                command.CommandTimeout = timeout;

            IDbDataAdapter adapter = CreateAdapter("dbo." + procedure);
            adapter.SelectCommand = command;
            DataSet ds = new DataSet();

            var st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug(string.Format("[ExecuteProcedure] {0}.", procedure) + Environment.NewLine);
            adapter.Fill(ds);
            if (Log.IsDebugEnabled) Log.Debug("[ExecuteProcedure] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

            return new DataMatrix(ds);
        }

        /// <summary>
        /// Create a command from a query using the current transaction if it exists
        /// </summary>
        /// <param name="query">The query with which to construct the command</param>
        /// <returns>The database command</returns>
        protected virtual IDbCommand CreateCommand(string query)
        {
            return CreateCommand(query, null);
        }

        /// <summary>
        /// Create a command from a query using the current transaction if it exists
        /// </summary>
        /// <param name="query">The query with which to construct the command</param>
        /// <param name="parameters">List of parameters to the command</param>
        /// <returns>The database command</returns>
        protected virtual IDbCommand CreateCommand(string query, IList<IDbDataParameter> parameters)
        {
            IDbCommand comando = Connection.CreateCommand();
            if (Transaction != null)
                comando.Transaction = Transaction;
            comando.CommandText = query;
            AddParameters(comando, parameters);

            //CHN + MA timeout read by XML file
            if (Configuration.ExistsProperty("CommandTimeout"))
            {
                comando.CommandTimeout = Convert.ToInt32(Configuration.GetProperty("CommandTimeout"));
            }

            //CHN applies sp global connection timeout (if set)
            if(Timeout>0)
                comando.CommandTimeout = Timeout;

            return comando;
        }

        public abstract IDbDataParameter CreateParameter();

        public virtual IDbDataParameter CreateParameter(object value)
        {
            var p = CreateParameter();
            if (value is GeographicPoint gp)
                value = gp.ToString();
            p.Value = value ?? DBNull.Value;
			if (!Configuration.IsDbUnicode && value != null && value.GetType() == typeof(string))
                p.DbType = DbType.AnsiString;
            return p;
        }

        public virtual IDbDataParameter CreateParameter(string name, object value)
        {
            IDbDataParameter p = CreateParameter(value);
            p.ParameterName = (Dialect.UseNamedPrefixInParameter ? Dialect.NamedPrefix : "") + name;
            return p;
        }

        public virtual void AddParameters(IDbCommand command, IList<IDbDataParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (IDbDataParameter param in parameters)
                {
                    command.Parameters.Add(param);
                }
            }
        }

        /// <summary>
        /// read a single database record into a business area
        /// </summary>
        /// <param name="area">The area to read</param>
        /// <param name="internalCodeValue">The primary key of the record</param>
        /// <param name="forUpdate">True if you are preparing to update this record, false otherwise</param>
        public virtual void getRecord(IArea area, object internalCodeValue, bool forUpdate)
        {
            getRecord(area, internalCodeValue, null, forUpdate);
        }

        /// <summary>
        /// Update the bookmark (old values) of a record with their current database value
        /// </summary>
        /// <remarks>Any missing current value will be set as equal to the read bookmark, and existing current value will be perserved</remarks>
        /// <param name="area">The area to read</param>
        public virtual void getBookmark(IArea area)
        {
            getRecord(area, area.QPrimaryKey, null, true, true);
        }

        /// <summary>
        /// Function that executes a query that returns a database record
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns>Area with filled values</returns>
        public virtual void getRecord(string query, IList<IDbDataParameter> parameters, IArea area)
        {
            try
            {
                Log.Debug("PersistentSupport.getRecord");

                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryReader] {0}.", query));

                IDbCommand comando = CreateCommand(query, parameters);
                IDataReader dr = comando.ExecuteReader();
                if (dr.Read())
                {
                    int nrColunas = dr.FieldCount;
                    for (int i = 0; i < nrColunas; i++)
                    {
                        // Don't take this condition off. Explanation: The user can remove columns from an area in the settings
                        // and the database still to be had on the corresponding table. If the user querys with "*", the
                        // below call would try to search in the corresponding Area structure all fields returned by SQL query.
                        // Since this field no longer exists in the structure, an exception would obviously arise from this.
                        string colName = dr.GetName(i).ToLower();
                        if (area.DBFields.TryGetValue(colName, out var fieldInfo))
                            area.insertNameValueField(fieldInfo.FullName, dr.GetValue(i), fromDatabase: true);
                    }
                }
                dr.Close();
                return;
            }
			catch (GenioException ex)
            {
				if (ex.UserMessage == null)
					throw new PersistenceException("Erro ao obter registo.", "PersistentSupport.getRecord",
												   "Error executing query '" + query + "' on area " + area.ToString() + ": " + ex.Message, ex);
				else
					throw new PersistenceException("Erro ao obter registo: " + ex.UserMessage, "PersistentSupport.getRecord",
												   "Error executing query '" + query + "' on area " + area.ToString() + ": " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // closeConnection(); to have a uniform behavior with other catch of other sp functions and not close the connection for no apparent reason to this.
                throw new PersistenceException("Erro ao obter registo.", "PersistentSupport.getRecord",
				                               "Error executing query '" + query + "' on area " + area.ToString() + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Function that executes a query
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns>the dataSet with the data</returns>
        public virtual DataMatrix executeQuery(string query)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryDataSet] {0}.", query));

                //initialize the data set
                DataSet ds = new DataSet();

                //run the Qresult query
                IDbDataAdapter da = CreateAdapter(query);
                da.Fill(ds);
                return new DataMatrix(ds);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.executaQuery",
				                               "Error executing query '" + query + "': " + ex.Message, ex);
            }
        }

		/// <summary>
        /// Function that executes a query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="parameters">List of parameters</param>
        /// <returns></returns>
        public DataMatrix executeQuery(string query, List<IDbDataParameter> parameters)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryDataSet] {0}.", query));

                IDbDataAdapter adapter = CreateAdapter(query);
                AddParameters(adapter.SelectCommand, parameters);

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                return new DataMatrix(ds);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupport.executaQuery",
                                               "Error executing query '" + query + "': " + ex.Message, ex);
            }
        }


        /// <summary>
        /// Function that executes a query
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <param name="paramList">query parameters</param>
        /// <returns>the dataSet with the data</returns>
        public virtual DataMatrix executeQuery(string query, IDictionary<string, ParameterQuery> paramList)
        {
            try
            {
                if (Log.IsDebugEnabled) Log.Debug(string.Format("[QueryDataSet] {0}.", query));

                //initialize the data set
                DataSet ds = new DataSet();

                //run the Qresult query
                List<IDbDataParameter> parameters = new List<IDbDataParameter>();

                foreach (var param in paramList)
                    parameters.Add(CreateParameter(param.Key,param.Value.Value));

                IDbDataAdapter adapter = CreateAdapter(query);

                AddParameters(adapter.SelectCommand, parameters);

                adapter.Fill(ds);

                return new DataMatrix(ds);
            }
            catch (Exception ex)
            {
				throw new PersistenceException(null, "PersistentSupport.executaQuery",
				                               "Error executing query '" + query + "' with parameters " + paramList.ToString() + ": " + ex.Message, ex);
            }
        }



        /*********MÉTODOS ABSTRACT*****************************************************/

        /// <summary>
        /// Instantiates a new Sql adapter
        /// </summary>
        /// <param name="query">The adapter boot query</param>
        /// <returns>A sql adapter</returns>
        public abstract IDbDataAdapter CreateAdapter(string query);

        /// <summary>
        /// Returns a connection to the server, without the database. Useful for when the database doesn't exist.
        /// </summary>
        public abstract IDbConnection GetConnectionToServer();


        /// <summary>
        /// Checks if a database exists
        /// </summary>
        /// <param name="database">The database name. If empty the default for this Persistent Support will be used.</param>
        /// <returns>True if the database exists</returns>
        public abstract bool CheckIfDatabaseExists(string database="");

        /// <summary>
        /// Drops the specified database
        /// </summary>
        /// <param name="schema">Database name</param>
        public abstract void Drop(string schema);

        /*********FIM DOS MÉTODOS ABSTRACT*****************************************************/

        /**************************** Ficheiros no disco **************************************/
        #region Get/Set do File no disco
        /// <summary>
        /// Burn the file to disk
        /// </summary>
        /// <param name="valorChaveDocums">Docums PK</param>
        /// <param name="domain">Domain table name"</param>
        /// <param name="ficheiro">Byte array corresponding to file</param>
        /// <param name="extensao">File extension</param>
        /// <returns>The path "relative" to file. ([DOMAIN]\[N_FOLDER]\File)</returns>
        private string saveFileToDisk(object keyValueDocums, Byte[] file, string domain, string extension)
        {
            const int MAX_FILES = 7500;
            // Validation of parameters
            if (string.IsNullOrEmpty(Configuration.PathDocuments))
                throw new PersistenceException("Não é possível gravar o ficheiro.", "PersistentSupport.saveFileToDisk", "The file path is not defined.");
            if (string.IsNullOrEmpty(domain) || file == null)
                throw new PersistenceException("Não é possível gravar o ficheiro.", "PersistentSupport.saveFileToDisk", "Arguments [DOMAIN] and/or [ficheiro] are null.");

            // Assign a unique identifier to the file
            string primaryKey = Convert.ToString(keyValueDocums);
            if (string.IsNullOrEmpty(primaryKey)) primaryKey = Guid.NewGuid().ToString();
            else primaryKey = primaryKey.TrimStart(' ');
            string fileName = string.Format("{0}.{1}", primaryKey, extension);


            // Target path construction (PathDocuments\[DOMAIN]\[YEAR]\[N_FOLDER])
            string basePath = System.IO.Path.Combine(Configuration.PathDocuments, domain.ToUpper());
            System.IO.Directory.CreateDirectory(basePath);
            string subFolder = string.Empty;
            string folderPath = string.Empty;
            int year = DateTime.Now.Year;

            SelectQuery query = new SelectQuery()
                .Select(SqlFunctions.Count("1"), "count")
                .From("docums")
                .Where(CriteriaSet.And()
                 .Equal(CSGenioAdocums.FldTabela, domain)
                 .Equal(CSGenioAdocums.FldZzstate, 0)
                 .Equal(SqlFunctions.Year(CSGenioAdocums.FldDatacria), year));
            query.noLock = true;

            int int_folder = (DBConversion.ToInteger(executeScalar(query)) / MAX_FILES) + 1;
            do
            {
                subFolder = System.IO.Path.Combine(string.Format("{0}", year), string.Format("{0}", int_folder));
                folderPath = System.IO.Path.Combine(basePath, subFolder);
                System.IO.Directory.CreateDirectory(folderPath);
                int_folder++;
            } while (System.IO.Directory.GetFiles(folderPath).Length > MAX_FILES);


            // File writing on disk
            string path = System.IO.Path.Combine(folderPath, fileName);
            System.IO.File.WriteAllBytes(path, file);

            // Return of the path "relative" to file. ([DOMAIN]\[N_FOLDER]\File)
            return System.IO.Path.Combine(domain.ToUpper(), System.IO.Path.Combine(subFolder, fileName));
        }
        /// <summary>
        /// MH (15/03/2017) - Get the file burned to disk
        /// </summary>
        /// <param name="filepath">The path "relative" to file. ([AREA]\[N_FOLDER]\File)</param>
        /// <returns></returns>
        public static Byte[] getFileFromDisk(string filepath)
        {
            // Validation of parameters
            if (string.IsNullOrEmpty(Configuration.PathDocuments))
                throw new PersistenceException("Não é possível obter o ficheiro: " + filepath, "PersistentSupport.getFileFromDisk", "The file path is not defined.");

            if (!string.IsNullOrEmpty(filepath))
            {
                string path = System.IO.Path.Combine(Configuration.PathDocuments, filepath);
                if (System.IO.File.Exists(path))
                    return System.IO.File.ReadAllBytes(path);
            }

            return new Byte[0];
        }

        /// <summary>
        /// MH (15/03/2017) - Get the file burned to the disc.
        /// Alternative version of get
        /// </summary>
        /// <param name="coddocums">Docums registry key value</param>
        /// <returns></returns>
        public Byte[] _getFileFromDisk(string coddocums)
        {
            // Validation of parameters
            if (string.IsNullOrEmpty(Configuration.PathDocuments))
                throw new PersistenceException("Não é possível obter o ficheiro.", "PersistentSupport._getFileFromDisk", "The file path is not defined.");

            try
            {
                string tableName = "docums";
                SelectQuery qs = new SelectQuery()
                .Select(tableName, "document")
                .From(tableName)
                .Where(CriteriaSet.And()
                    .Equal(tableName, "coddocums", coddocums))
                .PageSize(1);

                ArrayList results = executeReaderOneRow(qs);
                return PersistentSupport.getFileFromDisk(DBConversion.ToString(results[0]));
            }
            catch (Exception en)
            {
                throw new BusinessException("Não é possível obter o ficheiro.", "PersistentSupport._getFileFromDisk", "Error getting file " + coddocums + ": " + en.Message, en);
            }
        }

        private string changeFileOnDisk(object keyValueDocums, Byte[] file, string area, string extension)
        {
            // Validation of parameters
            if (string.IsNullOrEmpty(Configuration.PathDocuments))
                throw new PersistenceException("Não é possível alterar o ficheiro.", "PersistentSupport.changeFileOnDisk", "The file path is not defined.");
            if (keyValueDocums == null || file == null)
                throw new PersistenceException("Não é possível alterar o ficheiro.", "PersistentSupport.changeFileOnDisk", "Arguments area valorChaveDocums and/or ficheiro are null.");

            try
            {
                // Read from BD the current path to file
                string tableName = "docums";
                SelectQuery qs = new SelectQuery()
                    .Select(tableName, "document")
                    .From(tableName)
                    .Where(CriteriaSet.And()
                        .Equal(tableName, "coddocums", keyValueDocums));

                ArrayList results = executeReaderOneRow(qs);
                string path = System.IO.Path.Combine(Configuration.PathDocuments, DBConversion.ToString(results[0]));

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                return saveFileToDisk(keyValueDocums, file, area, extension);
            }
            catch (Exception en)
            {
				throw new PersistenceException("Não é possível alterar o ficheiro.", "PersistentSupport.changeFileOnDisk",
				                               string.Format("Error changing document - [valorChaveDocums] {0}; [area] {1}; [extensao] {2}", keyValueDocums.ToString(), area, extension) + en.Message, en);
            }
        }

        private string duplicateFileOnDisk(object keyValueDocums, string sourceFilePath, string area, string extension)
        {
            return saveFileToDisk(keyValueDocums, getFileFromDisk(sourceFilePath), area, extension);
        }

        private void removeFileFromDisk(object keyNameDocum, object keyValueDocums)
        {
            // Validation of parameters
            if (string.IsNullOrEmpty(Configuration.PathDocuments))
                throw new PersistenceException("Não é possível apagar o ficheiro.", "PersistentSupport.removeFileFromDisk", "The file path is not defined.");
            // Read from BD the current path to file
            try
            {
                string tableName = "docums";
                SelectQuery qs = new SelectQuery()
                    .Select(tableName, "docpath")
                    .From(tableName)
                    .Where(CriteriaSet.And()
                        .Equal(tableName, Convert.ToString(keyNameDocum), keyValueDocums));

                ArrayList results = executeReaderOneRow(qs);
                string filePath = DBConversion.ToString(results[0]);
                string path = System.IO.Path.Combine(Configuration.PathDocuments, filePath);
                if(System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            catch (Exception en)
            {
                throw new PersistenceException("Não é possível apagar o ficheiro.", "PersistentSupport.removeFileFromDisk",
				                               string.Format("Error removing document - [nomeChaveDocum] {0}; [valorChaveDocums] {1}", keyNameDocum.ToString(), keyValueDocums.ToString()) + en.Message, en);
            }
        }
        #endregion
    }
}
