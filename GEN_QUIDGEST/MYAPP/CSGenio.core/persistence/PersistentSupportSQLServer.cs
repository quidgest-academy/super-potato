using CSGenio.business;
using CSGenio.framework;
using Quidgest.Persistence.Dialects;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace CSGenio.persistence
{
    /// <summary>
    /// Microsoft SQL Server persistent support.
    /// </summary>
    public class PersistentSupportSQLServer : PersistentSupport
    {
        private static readonly Dialect m_dialect_singleton = new SqlServerDialect();

        /// <summary>
        /// Contructor
        /// </summary>
        public PersistentSupportSQLServer()
        {
            Dialect = m_dialect_singleton;
        }

        /// <inheritdoc/>
        public override IDbDataParameter CreateParameter()
        {
            return new SqlParameter();
        }

        /// <inheritdoc/>
        public override IDbDataParameter CreateParameter(object value)
        {
            var p = CreateParameter() as SqlParameter;
            p.Value = value ?? DBNull.Value;
            if (!Configuration.IsDbUnicode && value != null && value.GetType() == typeof(string))
                p.DbType = DbType.AnsiString;
            if (value is IEnumerable<string> ev)
            {
                //convert to table value parameter
                DataTable tableValueParam = QueryUtils.CreateKeyListType(ev);

                p.SqlDbType = SqlDbType.Structured;
                p.TypeName = tableValueParam.TableName;
                p.Value = tableValueParam;
            }
            if(value is DataTable dt)
            {
                p.SqlDbType = SqlDbType.Structured;
                p.TypeName = "dbo." + dt.TableName;
            }
            return p;
        }

        protected override void BuildConnection(DataSystemXml dataSystem, string login, string password, int connectionTimeout = 0)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();

            string port = string.Empty;
            if (!string.IsNullOrEmpty(dataSystem.Port))
                port = "," + dataSystem.Port;

            csb.DataSource = dataSystem.Server + port;
            csb.InitialCatalog = IsMaster ? "Master" : dataSystem.Schemas[0].Schema;
            if (login == null || password == null)
            {
                csb.UserID = dataSystem.LoginDecode();
                csb.Password = dataSystem.PasswordDecode();
            }
            else
            {
                csb.UserID = login;
                csb.Password = password;
            }
            if (!string.IsNullOrEmpty(ClientId))
                csb.WorkstationID = ClientId;
            if (ReadOnly)
                csb.ApplicationIntent = ApplicationIntent.ReadOnly;
            if (dataSystem.Schemas[0].ConnWithDomainUser)
                csb.IntegratedSecurity = true;
            if (dataSystem.Schemas[0].ConnEncrypt)
            {
                csb.Encrypt = true;
                csb.TrustServerCertificate = true;
            }
            if (connectionTimeout > 0)
                csb.ConnectTimeout = connectionTimeout;

            Connection = new SqlConnection(csb.ToString());
        }

        protected override string TransformSchemaName(string schema)
        {
            return schema + ".dbo";
        }

        /// <inheritdoc/>
        public override bool IsErrorTransient(Exception ex)
        {
            if(ex is SqlException se)
            {
                switch(se.ErrorCode)
                {
                    case 1205:  //Deadlock victim
                    case 4060:  //Cannot open database(transient if due to resource constraints)
                    case 10928: //Resource limit reached(Azure SQL Database)
                    case 10929: //Too many sessions(Azure SQL Database)
                    case 40197: //Azure SQL service error(transient)
                    case 40501: //Service is busy(transient)
                    case 233:   //Connection lost(network issue)
                    case 64:    //Network - related error
                    case 10053: //Network connection aborted
                    case 10054: //Connection reset by peer
                    case 10060: //Network timeout
                        return true;
                }

                //Consider query timeout as a transient error.
                //Note this should only be true for querys that might be temporarily blocked by others.
                //If a query is consistently slow and you retry it, it will certainly fail again.
                if(se.Number == -2)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Instancia um novo adaptador de Sql
        /// </summary>
        /// <param name="query">A query de inicialização do adaptador</param>
        /// <returns>Um adaptador de sql</returns>
        public override IDbDataAdapter CreateAdapter(string query)
        {
            IDbCommand command = CreateCommand(query);
            return new SqlDataAdapter((SqlCommand)command);
        }

        /// <inheritdoc/>
        public override string generatePrimaryKey(string id_object, string id_field, int size, FieldType format)
        {
			if (format == FieldType.KEY_GUID)
                return Guid.NewGuid().ToString();

            //database side keys use sequences instead of the Codigos_Sequenciais table
            Int64 codigoNovo;
            if (DatabaseSidePk || Configuration.ExistsProperty("SYS_PK_SEQUENCES"))
            {
                var codParam = new SqlParameter("range_first_value", SqlDbType.Variant) { Direction = ParameterDirection.Output };
                ExecuteProcedure("sp_sequence_get_range", [
                    new SqlParameter("sequence_name", "SEQ_"+id_object+"_"+id_field),
                    new SqlParameter("range_size", 1),
                    codParam
                    ]);
                codigoNovo = Convert.ToInt64(codParam.Value);
            }
            else
            {
                string sql = "dbo.updateCod '" + id_object + "', 1";
                codigoNovo = Convert.ToInt64(executeScalar(sql));
            }

            if (codigoNovo < 1)
            {
                throw new PersistenceException(null, "PersistentSupportSQLServer.generatePrimaryKey",
				                               "The primary key generated for object with id " + id_object + ", with size " + size + " and format " + format.ToString() + " is invalid: " + codigoNovo.ToString());
            }

			if (format == FieldType.KEY_VARCHAR)
            {
                return codigoNovo.ToString().PadLeft(size);
            }

            return codigoNovo.ToString();
        }

        /// <inheritdoc/>
        public override List<string> generatePrimaryKey(string id_object, string id_field, int size, FieldType format, int range)
        {
            if (range < 1)
                throw new ArgumentException("range must be 1 or larger", nameof(range));

            List<string> codes = new List<string>();

            if (format == FieldType.KEY_GUID)
            {
                for (int i = 0; i < range; i++)
                    codes.Add(Guid.NewGuid().ToString());
                return codes;
            }

            //database side keys use sequences instead of the Codigos_Sequenciais table
            Int64 codeStart;
            if (DatabaseSidePk)
            {
                var codParam = new SqlParameter("range_first_value", SqlDbType.Variant) { Direction = ParameterDirection.Output };
                ExecuteProcedure("sp_sequence_get_range", [
                    new SqlParameter("sequence_name", "SEQ_"+id_object+"_"+id_field),
                    new SqlParameter("range_size", range),
                    codParam
                    ]);
                codeStart = Convert.ToInt64(codParam.Value);
            }
            else
            {
                string sql = "dbo.updateCod '" + id_object + "', " + range;
                codeStart = Convert.ToInt64(executeScalar(sql));
            }

            for (int i = 0; i < range; i++)
                codes.Add((codeStart + i).ToString());

            if (format == FieldType.KEY_VARCHAR)
            {
                for (int i = 0; i < range; i++)
                    codes[i] = codes[i].PadLeft(size);
            }

            return codes;
        }

        /// <inheritdoc/>
        public override string Backup(string schema, string location = "")
        {
            try
            {
                openConnection();

                IDbCommand c = Connection.CreateCommand();

                string backupsFolder = string.IsNullOrEmpty(location)
                    ? GetDefaultBackupsLocation()
                    : location;

                // Ensure the backups folder exists.
                // TODO: We could probably move the responsibility of creating this folder elsewhere
                // and throw an exception here if it does not exist.
                Directory.CreateDirectory(backupsFolder);

                string fileName = $"{schema}_{DateTime.Now:yyyy_MM_dd_HHmm}.bak",
                    fullPath = Path.Combine(backupsFolder, fileName);

                c.CommandText = "BACKUP DATABASE @databasename TO DISK = @path WITH INIT, COPY_ONLY";
                c.Parameters.Add(new SqlParameter("@databasename", schema));
                c.Parameters.Add(new SqlParameter("@path", fullPath));

                // Last updated by [CJP] at [2016.07.27]
                // Remove timeout from command, in order to complete the database backup
                c.CommandTimeout = 0;

                c.ExecuteNonQuery();

                closeConnection();

                return fullPath;
            }
            catch (Exception e)
            {
                throw new PersistenceException("Erro ao criar o backup.", "PersistentSupportSQLServer.Backup", "Error while backing up the database: " + e.Message, e);
            }
        }

        /// <summary>
        /// Drop database
        /// </summary>
        /// <param name="schema">Database name</param>
        public override void Drop(string schema)
        {
            if (string.IsNullOrEmpty(schema))
                throw new ArgumentNullException("schema", "This argument is Mandatory") ;

            try
            {
                //Open a connection to the server
                var connection = GetConnectionToServer();
                connection.Open();
                IDbCommand c = connection.CreateCommand();
                IDbCommand c1 = connection.CreateCommand();

                c.CommandText = "declare @dynsql nvarchar(1000) = N'USE Master ALTER DATABASE ' + QUOTENAME(@databaseName) + N' SET Single_User WITH Rollback Immediate' EXEC(@dynsql)";
                c.Parameters.Add(new SqlParameter("@databasename", schema));
                c.ExecuteNonQuery();

                c1.CommandText = "declare @dynsql nvarchar(1000) = N'USE Master DROP DATABASE ' + QUOTENAME(@databaseName) EXEC(@dynsql)";
                c1.Parameters.Add(new SqlParameter("@databasename", schema));
                c1.ExecuteNonQuery();
                connection.Close();

            }
            catch (Exception e)
            {
                throw new PersistenceException("Error droping the database.", "PersistentSupportSQLServer.Drop", "Error while droping the database: " + e.Message, e);
            }
        }

        /// <inheritdoc/>
        public override void Restore(string schema, string path)
        {
            bool targetDbExists = false;

            try
            {
                targetDbExists = CheckIfDatabaseExists(schema);

                openConnection();

                if (targetDbExists)
                    SetDatabaseSingleUserMode(schema);

                ExecuteRestoreCommand(schema, path);
            }
            catch (Exception e)
            {
                throw new PersistenceException("Erro ao restaurar a base de dados.", "PersistentSupportSQLServer.Restore", "Error restoring the database: " + e.Message, e);
            }
            finally
            {
                if (targetDbExists)
                    SetDatabaseMultiUserMode(schema);

                closeConnection();
            }
        }

        private void SetDatabaseSingleUserMode(string schema)
        {
            IDbCommand setSingleUserModeCmd = Connection.CreateCommand();
            setSingleUserModeCmd.CommandText = @"
                declare @dynsql nvarchar(1000) = N'USE Master ALTER DATABASE ' + QUOTENAME(@databaseName) + N' SET Single_User WITH Rollback Immediate'
                EXEC(@dynsql)";
            setSingleUserModeCmd.Parameters.Add(new SqlParameter("@databaseName", schema));
            setSingleUserModeCmd.ExecuteNonQuery();
        }

        private void ExecuteRestoreCommand(string schema, string path)
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = @"
                USE Master;
                DECLARE @dataFileName NVARCHAR(256);
                DECLARE @logFileName NVARCHAR(256);
                DECLARE @defaultDataPath NVARCHAR(512);
                DECLARE @defaultLogPath NVARCHAR(512);
                DECLARE @filelist TABLE
                (
                    LogicalName NVARCHAR(256),
                    PhysicalName NVARCHAR(512),
                    [Type] VARCHAR(1),
                    [FileGroupName] VARCHAR(128),
                    [Size] VARCHAR(128),
                    [MaxSize] VARCHAR(128),
                    [FileId] VARCHAR(128),
                    [CreateLSN] VARCHAR(128),
                    [DropLSN] VARCHAR(128),
                    [UniqueId] VARCHAR(128),
                    [ReadOnlyLSN] VARCHAR(128),
                    [ReadWriteLSN] VARCHAR(128),
                    [BackupSizeInBytes] VARCHAR(128),
                    [SourceBlockSize] VARCHAR(128),
                    [FileGroupId] VARCHAR(128),
                    [LogGroupGUID] VARCHAR(128),
                    [DifferentialBaseLSN] VARCHAR(128),
                    [DifferentialBaseGUID] VARCHAR(128),
                    [IsReadOnly] VARCHAR(128),
                    [IsPresent] VARCHAR(128),
                    [TDEThumbprint] VARCHAR(128),
                    [SnapshotUrl] VARCHAR(128)
                );

                INSERT INTO @filelist
                EXEC('RESTORE FILELISTONLY FROM DISK = ''' + @path + '''');

                SELECT @dataFileName = LogicalName FROM @filelist WHERE Type = 'D';
                SELECT @logFileName = LogicalName FROM @filelist WHERE Type = 'L';

                SELECT @defaultDataPath = CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS NVARCHAR(512));
                SELECT @defaultLogPath = CAST(SERVERPROPERTY('InstanceDefaultLogPath') AS NVARCHAR(512));

                DECLARE @restoreQuery NVARCHAR(1000);
                SET @restoreQuery =
                    'RESTORE DATABASE ' + @databaseName + ' FROM DISK = ''' + @path + '''' +
                    ' WITH REPLACE, MOVE ''' + @dataFileName + ''' TO ''' + @defaultDataPath + '\' + @databaseName + '.mdf'', MOVE ''' +
                    @logFileName + ''' TO ''' + @defaultLogPath + '\' + @databaseName + '.ldf''';

                EXEC sp_executesql @restoreQuery;
            ";
            cmd.Parameters.Add(new SqlParameter("@path", path));
            cmd.Parameters.Add(new SqlParameter("@databaseName", schema));
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();
        }

        private void SetDatabaseMultiUserMode(string schema)
        {
            IDbCommand setMultiUserModeCmd = Connection.CreateCommand();
            setMultiUserModeCmd.CommandText = @"
                declare @dynsql nvarchar(1000) = N'USE Master ALTER DATABASE ' + QUOTENAME(@databaseName) + N' SET Multi_User'
                EXEC(@dynsql)";
            setMultiUserModeCmd.Parameters.Add(new SqlParameter("@databaseName", schema));
            setMultiUserModeCmd.ExecuteNonQuery();
        }

		/// <summary>
        /// Transfer log data from the system DB to the system log DB
        /// Called from log database PersistentSupport
        /// Uses SQL Bulk Copy (destination database must be SQL Server)
        /// </summary>
        /// <param name="all">True to transfer all log data</param>
        public override void transferMSMQLog(bool all)
        {
            // Get system database PersistentSupport
            PersistentSupport systemSp = PersistentSupport.getPersistentSupport(this.SchemaMapping.Name);
            CriteriaSet filterMQQueues = CriteriaSet.And();
            if (!all && Configuration.MaxLogRowDays > 0)
            {
                DateTime lastDate = DateTime.Today.AddDays(-Configuration.MaxLogRowDays);
                filterMQQueues.LesserOrEqual("FORMQQueues_History", "DATASTATUS", lastDate);
            }

            try
            {
                // Open connections
                systemSp.openConnection();
                this.openConnection();

                // Open system databas transaction
                systemSp.openTransaction();

                // Open log database transaction
                using (System.Data.SqlClient.SqlTransaction logTransaction = (System.Data.SqlClient.SqlTransaction)this.Connection.BeginTransaction())
                {
                    try
                    {
                        // MQQueues row selection query
                        SelectQuery query = new SelectQuery()
                            .Select("FORMQQueues_History", "CODMQQUEUES", "CODMQQUEUES")
                            .Select("FORMQQueues_History", "QUEUEID", "QUEUEID")
							.Select("FORMQQueues_History", "CHANNELID", "CHANNELID")
                            .Select("FORMQQueues_History", "ANO", "ANO")
                            .Select("FORMQQueues_History", "USERNAME", "USERNAME")
                            .Select("FORMQQueues_History", "TABELA", "TABELA")
                            .Select("FORMQQueues_History", "TABELACOD", "TABELACOD")
                            .Select("FORMQQueues_History", "QUEUEKEY", "QUEUEKEY")
                            .Select("FORMQQueues_History", "QUEUE", "QUEUE")
                            .Select("FORMQQueues_History", "MQSTATUS", "MQSTATUS")
                            .Select("FORMQQueues_History", "DATASTATUS", "DATASTATUS")
                            .Select("FORMQQueues_History", "DATACRIA", "DATACRIA")
                            .Select("FORMQQueues_History", "OPERACAO", "OPERACAO")
                            .Select("FORMQQueues_History", "RESPOSTA", "RESPOSTA")
                            .Select("FORMQQueues_History", "SENDNUMBER", "SENDNUMBER")
                            .Select("FORMQQueues_History", "ZZSTATE", "ZZSTATE")
                            .From("FORMQQueues_History")
                            .Where(filterMQQueues)
                            .OrderBy("FORMQQueues_History", "DATACRIA", Quidgest.Persistence.GenericQuery.SortOrder.Ascending);

                        // Create MQQueues row selection query data reader
                        IDbCommand command = systemSp.GetSelectCommand(query);
                        using (IDataReader dr = command.ExecuteReader())
                        {
                            // Create bulk copy object
                            using (System.Data.SqlClient.SqlBulkCopy bulkCopy = new System.Data.SqlClient.SqlBulkCopy(
                                (System.Data.SqlClient.SqlConnection)this.Connection,
                                System.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity,
                                logTransaction))
                            {
                                bulkCopy.DestinationTableName = "dbo.FORMQQueues_History";
                                bulkCopy.ColumnMappings.Add("CODMQQUEUES", "CODMQQUEUES");
                                bulkCopy.ColumnMappings.Add("QUEUEID", "QUEUEID");
								bulkCopy.ColumnMappings.Add("CHANNELID", "CHANNELID");
                                bulkCopy.ColumnMappings.Add("ANO", "ANO");
                                bulkCopy.ColumnMappings.Add("USERNAME", "USERNAME");
                                bulkCopy.ColumnMappings.Add("TABELA", "TABELA");
                                bulkCopy.ColumnMappings.Add("TABELACOD", "TABELACOD");
                                bulkCopy.ColumnMappings.Add("QUEUEKEY", "QUEUEKEY");
                                bulkCopy.ColumnMappings.Add("QUEUE", "QUEUE");
                                bulkCopy.ColumnMappings.Add("MQSTATUS", "MQSTATUS");
                                bulkCopy.ColumnMappings.Add("DATASTATUS", "DATASTATUS");
                                bulkCopy.ColumnMappings.Add("DATACRIA", "DATACRIA");
                                bulkCopy.ColumnMappings.Add("OPERACAO", "OPERACAO");
                                bulkCopy.ColumnMappings.Add("RESPOSTA", "RESPOSTA");
                                bulkCopy.ColumnMappings.Add("SENDNUMBER", "SENDNUMBER");
                                bulkCopy.ColumnMappings.Add("ZZSTATE", "ZZSTATE");

                                bulkCopy.BulkCopyTimeout = 0;
                                bulkCopy.BatchSize = 10000;
                                bulkCopy.WriteToServer(dr);
                                bulkCopy.Close();
                            }
                        }

                        // Delete log rows on system database
                        DeleteQuery delete = new DeleteQuery()
                            .Delete("FORMQQueues_History")
                            .Where(filterMQQueues);

                        systemSp.Execute(delete);

                        // Commit transaction
                        logTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        logTransaction.Rollback();
                        systemSp.rollbackTransaction();
                        throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupportSQLServer.transferMSMQLog", "Error transfering MSMQ log data from the database to the log: " + e.Message, e);
                    }


                }
                // Close connections
                systemSp.closeTransaction();
                systemSp.closeConnection();
                this.closeConnection();
            }
            catch (Exception e)
            {
                systemSp.rollbackTransaction();
                systemSp.closeConnection();
                this.closeConnection();
                throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupportSQLServer.transferMSMQLog", "Error transfering MSMQ log data from the database to the log: " + e.Message, e);
            }
        }

        /// <summary>
        /// Transfer log data from the system DB to the system log DB
        /// Called from log database PersistentSupport
        /// Uses SQL Bulk Copy (destination database must be SQL Server)
        /// </summary>
        /// <param name="all">True to transfer all log data</param>
        /// <param name="job">The transfer log job.</param>
        public override void transferLog(bool all, ExecuteQueryCore.TransferLogOperation job)
        {
            // Get system database PersistentSupport
            PersistentSupport systemSp = PersistentSupport.getPersistentSupport(this.SchemaMapping.Name);

            string tableLogs = "log" + Configuration.Program + "all";

            // Filter rows by date (specified in configuration file)
            CriteriaSet filter = CriteriaSet.And();
            CriteriaSet filterMem = CriteriaSet.And();
            if (!all && Configuration.MaxLogRowDays > 0)
            {
                DateTime lastDate = DateTime.Today.AddDays(-Configuration.MaxLogRowDays);
                filter.LesserOrEqual(tableLogs, "date", lastDate);
                filterMem.LesserOrEqual(CSGenioAmem.FldAltura, lastDate);
            }

            try
            {
                // Open connections
                systemSp.openConnection();
                this.openConnection();

                // Open system database transaction
                systemSp.openTransaction();

                // Open log database transaction
                using (System.Data.SqlClient.SqlTransaction logTransaction = (System.Data.SqlClient.SqlTransaction)this.Connection.BeginTransaction())
                {
                    try
	                {
						int[] totalRows = new[] { 0, 0 };

                        // logGENall Row count
                        SelectQuery query = new SelectQuery()
                            .Select(SqlFunctions.Count("*"), "count")
                            .From(tableLogs)
                            .Where(filter);

                        DataMatrix values = systemSp.Execute(query);
                        if (values.NumRows != 0)
                            totalRows[0] = values.GetInteger(0, 0);

                        // MEM Row count
                        query = new SelectQuery()
                            .Select(SqlFunctions.Count("*"), "count")
                            .From(Area.AreaMEM)
                            .Where(filterMem);

                        values = systemSp.Execute(query);
                        if (values.NumRows != 0)
                            totalRows[1] = values.GetInteger(0, 0);

                        // Total rows to be copied
                        job.Total = totalRows[0] + totalRows[1];

                        // ----------------------------------------------
                        // LogGENall transfer
                        // ----------------------------------------------

                        if (totalRows[0] != 0)
                        {
							job.CurrentTable = tableLogs;

                            // Log row selection query
                            query = new SelectQuery()
                                .Select(tableLogs, "cod", "COD")
                                .Select(tableLogs, "date", "DATE")
                                .Select(tableLogs, "who", "WHO")
                                .Select(tableLogs, "op", "OP")
                                .Select(tableLogs, "logtable", "LOGTABLE")
                                .Select(tableLogs, "logfield", "LOGFIELD")
                                .Select(tableLogs, "val", "VAL")
                                .From(tableLogs)
                                .Where(filter)
                                .OrderBy(2, Quidgest.Persistence.GenericQuery.SortOrder.Ascending);

                            // Create log row selection query data reader
                            IDbCommand command = systemSp.GetSelectCommand(query);
                            using (IDataReader dr = command.ExecuteReader())
                            {

                                // Create bulk copy object
                                using (System.Data.SqlClient.SqlBulkCopy bulkCopy = new System.Data.SqlClient.SqlBulkCopy(
                                    (System.Data.SqlClient.SqlConnection) this.Connection,
                                    System.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity,
                                    logTransaction))
                                {
                                    bulkCopy.DestinationTableName = tableLogs;
                                    bulkCopy.ColumnMappings.Add("COD", "COD");
                                    bulkCopy.ColumnMappings.Add("DATE", "DATE");
                                    bulkCopy.ColumnMappings.Add("WHO", "WHO");
                                    bulkCopy.ColumnMappings.Add("OP", "OP");
                                    bulkCopy.ColumnMappings.Add("LOGTABLE", "LOGTABLE");
                                    bulkCopy.ColumnMappings.Add("LOGFIELD", "LOGFIELD");
                                    bulkCopy.ColumnMappings.Add("VAL", "VAL");
									bulkCopy.BulkCopyTimeout = 0;
                                    bulkCopy.BatchSize = 10000;

                                    // Capture the progress of the event
                                    bulkCopy.NotifyAfter = 10000;
                                    bulkCopy.SqlRowsCopied += (sender, eventArgs) =>
                                    {
                                        job.Copied += bulkCopy.NotifyAfter;
									};

                                    // Write from the source to the destination.
                                    bulkCopy.WriteToServer(dr);
									job.Copied = totalRows[0];
                                }
                            }

                            // Delete log rows on system database
                            DeleteQuery delete = new DeleteQuery()
                                .Delete(tableLogs)
                                .Where(filter);

                            systemSp.Execute(delete);
                        }

                        // ----------------------------------------------
                        // MEM transfer
                        // ----------------------------------------------

                        if (totalRows[1] != 0)
                        {
							job.CurrentTable = Configuration.Program + "mem";

                            // MEM row selection query
                            query = new SelectQuery()
								.Select(CSGenioAmem.FldCodmem, "CODMEM")
								.Select(CSGenioAmem.FldLogin, "LOGIN")
								.Select(CSGenioAmem.FldAltura, "ALTURA")
								.Select(CSGenioAmem.FldRotina, "ROTINA")
								.Select(CSGenioAmem.FldObs, "OBS")
								.Select(CSGenioAmem.FldHostid, "HOSTID")
								.Select(CSGenioAmem.FldClientid, "CLIENTID")
								.Select(CSGenioAmem.FldZzstate, "ZZSTATE")
								.From(Area.AreaMEM)
								.Where(filterMem)
								.OrderBy(CSGenioAmem.FldAltura, Quidgest.Persistence.GenericQuery.SortOrder.Ascending);

                            // Create MEM row selection query data reader
                            IDbCommand command = systemSp.GetSelectCommand(query);
                            using (IDataReader dr = command.ExecuteReader())
                            {
                                // Create bulk copy object
                                using (System.Data.SqlClient.SqlBulkCopy bulkCopy = new System.Data.SqlClient.SqlBulkCopy(
                                    (System.Data.SqlClient.SqlConnection)this.Connection,
                                    System.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity,
                                    logTransaction))
                                {
                                    bulkCopy.DestinationTableName = Area.AreaMEM.Table;
                                    bulkCopy.ColumnMappings.Add("CODMEM", "CODMEM");
                                    bulkCopy.ColumnMappings.Add("LOGIN", "LOGIN");
                                    bulkCopy.ColumnMappings.Add("ALTURA", "ALTURA");
                                    bulkCopy.ColumnMappings.Add("ROTINA", "ROTINA");
                                    bulkCopy.ColumnMappings.Add("OBS", "OBS");
                                    bulkCopy.ColumnMappings.Add("HOSTID", "HOSTID");
									bulkCopy.ColumnMappings.Add("CLIENTID", "CLIENTID");
                                    bulkCopy.ColumnMappings.Add("ZZSTATE", "ZZSTATE");
									bulkCopy.BulkCopyTimeout = 0;
                                    bulkCopy.BatchSize = 10000;

                                    // Capture the progress of the event
                                    bulkCopy.NotifyAfter = 10000;
                                    bulkCopy.SqlRowsCopied += (sender, eventArgs) =>
                                    {
                                        job.Copied += bulkCopy.NotifyAfter;
									};

                                    // Write from the source to the destination.
                                    bulkCopy.WriteToServer(dr);
									job.Copied = job.Total;
                                    job.Completed = true;
                                }
                            }

                            // Delete log rows on system database
                            DeleteQuery delete = new DeleteQuery()
                                .Delete(Area.AreaMEM)
                                .Where(filterMem);

                            systemSp.Execute(delete);
                        }

                        // Commit transaction
                        logTransaction.Commit();

	                }
					catch (GenioException ex)
					{
						logTransaction.Rollback();
                        systemSp.rollbackTransaction();

						job.Completed = true;
						job.ErrorMessage = "Erro durante a transferência de logs: " + ex.UserMessage;

						if (ex.ExceptionSite == "PersistentSupportSQLServer.transferLog")
							throw;
						if (ex.UserMessage == null)
							throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupportSQLServer.transferLog", "Error transfering log data from the database to the log: " + ex.Message, ex);
						else
							throw new PersistenceException("Erro durante a transferência de logs: " + ex.UserMessage, "PersistentSupportSQLServer.transferLog", "Error transfering log data from the database to the log: " + ex.Message, ex);
					}
	                catch (Exception e)
	                {
		                logTransaction.Rollback();
                        systemSp.rollbackTransaction();

						job.Completed = true;
                        job.ErrorMessage = "Erro durante a transferência de logs: " + e.Message;

		                throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupportSQLServer.transferLog", "Error transfering log data from the database to the log: " + e.Message, e);
	                }
                }

                // Close connections
                systemSp.closeTransaction();
				systemSp.closeConnection();
                this.closeConnection();
            }
            catch (Exception e)
            {
                // Rollback transactions
				// [RC] 06/06/2017 Someone forgot to actually close the transaction?
                systemSp.rollbackTransaction();
                systemSp.closeConnection();
                this.closeConnection();
                throw new PersistenceException("Erro durante a transferência de logs.", "PersistentSupportSQLServer.transferLog", "Error transfering log data from the database to the log: " + e.Message, e);
            }
        }

        public override int getRecordPos(User user, string module, IArea area, IList<ColumnSort> sorting, string primaryKeyValue, CriteriaSet conditions, string identifier)
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
                    QueryUtils.increaseQuery(querySelect, queryGenio.SelectFields, queryGenio.FromTable, queryGenio.Joins, queryGenio.WhereConditions, 1, conditions, null, queryGenio.Distinct);
                }

                QueryUtils.setWhereGetPos(querySelect, sorting, area, primaryKeyValue);

                DataMatrix mx = Execute(QueryUtils.buildQueryCount(querySelect));
                return mx.GetInteger(0, 0);
            }
            catch (PersistenceException ex)
            {
                throw new PersistenceException(ex.UserMessage, "PersistentSupportSQLServer.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions, identifier) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupportSQLServer.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions, identifier) + ex.Message, ex);
            }
        }

        /// <summary>
        /// Returns a connection to the server, without the database. Useful for when the database doesn't exist.
        /// </summary>
        public override IDbConnection GetConnectionToServer()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Connection.ConnectionString);

            if(String.IsNullOrEmpty(builder.Password))
            {
                //After a connection is opened the first time, it's password is cleared.
                //If we have no password either it was opened before (we don't need a new connection), or it was never given, and it will fail either way.
                return this.Connection;
            }

            builder.InitialCatalog = "";
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            return connection;
        }


        /// <summary>
        /// Checks if a database exists
        /// </summary>
        /// <param name="database">The database name</param>
        /// <returns>True if the database exists</returns>
        public override bool CheckIfDatabaseExists(string database)
        {

            if (String.IsNullOrEmpty(database))
                database = this.Connection.Database;

            //If a connection was already opened, we already know that it exists
            if (database == this.Connection.Database && this.Connection.State == ConnectionState.Open)
                return true;

            string query = $"SELECT database_id FROM sys.databases WHERE Name = '{database}'";

            //Open a new one to the server
            var connection = (SqlConnection) GetConnectionToServer();
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                object result = command.ExecuteScalar();

                // If result is not null, the database exists
                connection.Close();
                return result != null;
            }
        }

        /// <inheritdoc/>
        public override void bulkInsert(IEnumerable<IArea> rows)
        {
            if (!rows.Any())
                return;
            var info = rows.First().Information;

            SqlBulkCopy copy = new SqlBulkCopy(Connection as SqlConnection, SqlBulkCopyOptions.Default, Transaction as SqlTransaction);
            copy.DestinationTableName = info.TableName;

            //Could not make this work, it ignored me marking the Guids in the schema
            //var reader = new CSGenio.core.persistence.CSAreaDataReader<A>(rows);
            //for (int i = 0; i < reader.FieldCount; i++)
            //    copy.ColumnMappings.Add(reader.GetName(i), reader.GetName(i).ToUpperInvariant());
            //copy.WriteToServer(reader);

            //SqlBulkCopy is case sensitive in the column mappings if you use manual mappings
            //SqlBulkCopy is order sensitive in the column mappings if you use auto mappings
            //So we need to fetch the real names of the columns in the database for the mapping to work correctly
            // This is a specialized query for SQL server only
            Dictionary<string, string> realNames = QCache.Instance.AdminReindexation.Get("bulk_schema_" + info.TableName) as Dictionary<string, string>;
            if (realNames is null)
            {
                realNames = new Dictionary<string, string>();
                using (var cmd = CreateCommand("SELECT name FROM sys.columns WHERE object_id = OBJECT_ID(@p1) ORDER BY column_id", [
                    CreateParameter("p1", info.TableName)
                    ]))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        realNames.Add(reader.GetString(0).ToLowerInvariant(), reader.GetString(0));
                }
                QCache.Instance.AdminReindexation.Put("bulk_schema_" + info.TableName, realNames);
            }
            foreach (var col in info.DBFields)
                copy.ColumnMappings.Add(col.Key, realNames[col.Key]);

            DataTable dt = SetupBulkDataTable(rows, info);

            //execute the bulk copy
            long st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug("[bulkInsert]" + Environment.NewLine + rows.Count() + " rows sent.");
            copy.WriteToServer(dt);
            if (Log.IsDebugEnabled) Log.Debug("[bulkInsert] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
        }

        private static DataTable SetupBulkDataTable(IEnumerable<IArea> rows, AreaInfo info)
        {
            DataTable dt = new DataTable();
            //Setup the schema
            foreach (var col in info.DBFields)
            {
                var dataType = col.Value.FieldType.GetExternalType();
                dt.Columns.Add(col.Key, dataType);
            }
            //Setup the data
            foreach (var row in rows)
            {
                var dr = dt.NewRow();
                foreach (RequestedField fld in row.Fields.Values)
                    dr[fld.Name] = QueryUtils.ToValidDbValue(fld.Value, info.DBFields[fld.Name]);
                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <inheritdoc/>
        public override void bulkUpdate(IEnumerable<IArea> rows)
        {
            if (!rows.Any())
                return;
            var info = rows.First().Information;

            //setup the datatable
            var dataTable = SetupBulkDataTable(rows, info);
            dataTable.AcceptChanges();
            foreach(DataRow dr in dataTable.Rows)
                dr.SetModified();

            //use the first row as a header to create the update query
            UpdateQuery query = new UpdateQuery();
            QueryUtils.fillQueryUpdate(query, rows.First());
            //if we have no columns to update then do nothing
            if (query.SetValues.Count == 0)
                return;

            var renderer = new QueryRenderer(this);
            renderer.SchemaMapping = SchemaMapping;
            var sql = renderer.GetSql(query);
            var parameters = renderer.ParameterList;

            //the data adapter needs to know how to match the parameter name to the corresponding column in the dataTable
            for (int i = 0; i < query.SetValues.Count; i++)
                parameters[i].SourceColumn = query.SetValues[i].Column.ColumnName;
            //the last parameter should always be the pk
            parameters[parameters.Count - 1].SourceColumn = info.PrimaryKeyName;

            //use an adapter to batch the updates
            var adapter = new SqlDataAdapter();
            adapter.UpdateCommand = CreateCommand(sql, parameters) as SqlCommand;
            adapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;
            adapter.UpdateBatchSize = 500;

            long st = DateTime.Now.Ticks;
            if (Log.IsDebugEnabled) Log.Debug("[bulkUpdate]" + Environment.NewLine + rows.Count() + " rows sent.");
            adapter.Update(dataTable);
            if (Log.IsDebugEnabled) Log.Debug("[bulkUpdate] " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
        }

        /// <inheritdoc/>
        public override void bulkDelete(IEnumerable<IArea> rows)
        {
            if (!rows.Any())
                return;
            var info = rows.First().Information;

            //put keys into a table value parameter
            var tableValueParam = QueryUtils.CreateKeyListType(rows.Select(x => x.QPrimaryKey));

            DeleteQuery queryDelete = new DeleteQuery()
                .Delete(info.QSystem, info.TableName)
                .Where(CriteriaSet.And()
                .In(info.TableName, info.PrimaryKeyName, tableValueParam));

            int linha = Execute(queryDelete);
        }
    }
}
