using CSGenio.framework;
using CSGenio.framework.Geography;
using Npgsql;
using NpgsqlTypes;
using Quidgest.Persistence.Dialects;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Data;

namespace CSGenio.persistence
{
    /// <summary>
    /// Conector for Postgres database
    /// </summary>
    public class PersistentSupportPostgres: PersistentSupport
    {

        /// <inheritdoc/>
        public override IDbDataParameter CreateParameter(object value)
        {
            if(value is GeographicPoint gp)
            {
                return new NpgsqlParameter()
                {
                    Value = new NpgsqlPoint(gp.lng, gp.lat),
                };
            }
            return base.CreateParameter(value);
        }

        /// <inheritdoc/>
        public override IDbDataParameter CreateParameter()
        {
            return new NpgsqlParameter();
        }

        private static readonly Dialect m_dialect_singleton = new PostgresDialect();

        /// <summary>
        /// Contructor
        /// </summary>
        public PersistentSupportPostgres()
        {
			Dialect = m_dialect_singleton;
        }

        /// <inheritdoc/>
        protected override void BuildConnection(DataSystemXml dataSystem, string login, string password, int connectionTimeout = 0)
        {
            var csb = new NpgsqlConnectionStringBuilder();

            csb.Host = dataSystem.Server;
            if (!string.IsNullOrEmpty(dataSystem.Port))
                csb.Port = int.Parse(dataSystem.Port);
            csb.Database = IsMaster ? "postgres" : dataSystem.Schemas[0].Schema;
            if (login == null || password == null)
            {
                csb.Username = dataSystem.LoginDecode();
                csb.Password = dataSystem.PasswordDecode();
            }
            else
            {
                csb.Username = login;
                csb.Password = password;
            }
            //if (ReadOnly)
            //    csb.ApplicationIntent = ApplicationIntent.ReadOnly;
            //if (dataSystem.Schemas[0].ConnWithDomainUser)
            //    csb.IntegratedSecurity = true;

            if (!string.IsNullOrEmpty(ClientId))
                csb.ApplicationName = ClientId;
            else
                csb.ApplicationName = Configuration.Program;

            if (connectionTimeout > 0)
                csb.CommandTimeout = connectionTimeout;

            Connection = new NpgsqlConnection(csb.ToString());
        }

        /// <inheritdoc/>
        protected override string TransformSchemaName(string schema)
        {
            return schema + ".public";
        }

        /// <inheritdoc/>
        public override bool IsErrorTransient(Exception ex)
        {
            if (ex is PostgresException pe)
            {
                if(pe.SqlState == "40P01")
                    return true;
            }
            else if(ex is NpgsqlException ne)
            {
                if(ne.IsTransient) 
                    return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public override void openConnection()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Log.Debug("openConnection");
                    Connection.Open();
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Database connection error.", "openConnection", "Error opening connection: " + ex.Message, ex);
            }
        }

        /// <inheritdoc/>
        public override IDbDataAdapter CreateAdapter(string query)
        {
            IDbCommand command = CreateCommand(query);
            return new NpgsqlDataAdapter((NpgsqlCommand)command);
        }

        /// <inheritdoc/>
        public override string generatePrimaryKey(string id_object, string id_field, int size, FieldType format)
        {
            if (format == FieldType.KEY_GUID)
                return Guid.NewGuid().ToString();

            var command = CreateCommand("select nextval('" + id_object + "_" + id_field + "_seq'::regclass)");
            Int64 codeStart = Convert.ToInt64(command.ExecuteScalar());

            var res = codeStart.ToString();

            if (format == FieldType.KEY_VARCHAR)
                res = res.PadLeft(size);

            return res;
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

            var command = CreateCommand("select nextval('" + id_object + "_" + id_field + "_seq'::regclass) from generate_series(1, " + range + ")");
            using(var reader = command.ExecuteReader())
            {
                while(reader.Read())
                    codes.Add(reader.GetValue(0).ToString());
            }

            if (format == FieldType.KEY_VARCHAR)
            {
                for (int i = 0; i < range; i++)
                    codes[i] = codes[i].PadLeft(size);
            }

            return codes;
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
                throw new PersistenceException(ex.UserMessage, "PersistentSupportNpgsql.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions.ToString(), identifier) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupportNpgsql.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions.ToString(), identifier) + ex.Message, ex);
            }
        }

        public override IDbConnection GetConnectionToServer()
        {
            var builder = new NpgsqlConnectionStringBuilder(Connection.ConnectionString);

            if (String.IsNullOrEmpty(builder.Password))
            {
                //After a connection is opened the first time, it's password is cleared.
                //If we have no password either it was opened before (we don't need a new connection), or it was never given, and it will fail either way.
                return this.Connection;
            }

            builder.Database = "postgres";
            NpgsqlConnection connection = new NpgsqlConnection(builder.ConnectionString);
            return connection;
        }

        public override bool CheckIfDatabaseExists(string database)
        {
            if (String.IsNullOrEmpty(database))
                database = this.Connection.Database;

            //If a connection was already opened, we already know that it exists
            if (database == this.Connection.Database && this.Connection.State == ConnectionState.Open)
                return true;

            string query = $"SELECT datname FROM pg_catalog.pg_database WHERE datname = '{database}';";

            //Open a new one to the server
            var connection = (NpgsqlConnection)GetConnectionToServer();
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                object result = command.ExecuteScalar();

                // If result is not null, the database exists
                connection.Close();
                return result != null;
            }
        }

        public override void Drop(string schema)
        {
            throw new NotImplementedException();
        }
    }
}
