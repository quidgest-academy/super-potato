using System;
using System.Data;
using MySql.Data.MySqlClient;
using CSGenio.framework;
using Quidgest.Persistence.Dialects;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Generic;
using CSGenio.business;
using System.Data.SqlClient;

namespace CSGenio.persistence
{
    /// <summary>
    /// Conector for MySql database
    /// </summary>
    public class PersistentSupportMySql: PersistentSupport
    {
        public override IDbDataParameter CreateParameter()
        {
            return new MySqlParameter();
        }

        public override IDbDataParameter CreateParameter(object value)
        {
            if (value is Guid || (value is Guid? && value != null))
            {
                return base.CreateParameter(((Guid)value).ToByteArray());
            }

            return base.CreateParameter(value);
        }

		// schema sobre o qual as queries s�o excutadas
        private string schema_bd = null;

        /// <summary>
        /// Contructor
        /// </summary>
        public PersistentSupportMySql()
        {
			Dialect = new MySqlDialect();
        }

        /// <inheritdoc/>
        public override bool IsErrorTransient(Exception ex)
        {
            if (ex is MySqlException me)
            {
                switch (me.Number)
                {
                    case 1213: //deadlock
                    case 1040: //too many connections
                    case 1205: //wait timeout
                        return true;
                }
            }
            return false;
        }

        protected override void BuildConnection(DataSystemXml dataSystem, string login, string password, int connectionTimeout = 0)
        {
            schema_bd = Configuration.GetProperty("SCHEMA_BD", null);

            MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder();

            csb.Server = dataSystem.Server;
            if (!string.IsNullOrEmpty(dataSystem.Port))
                csb.Port = uint.Parse(dataSystem.Port);
            csb.Database = IsMaster ? "" : dataSystem.Schemas[0].Schema;
            if (login == null)
            {
                csb.UserID = dataSystem.LoginDecode();
                csb.Password = dataSystem.PasswordDecode();
            }
            else
            {
                csb.UserID = login;
                csb.Password = password;
            }
            //if (!string.IsNullOrEmpty(ClientId))
            //    csb.WorkstationID = ClientId;
            //if (ReadOnly)
            //    csb.ApplicationIntent = ApplicationIntent.ReadOnly;
            //if (dataSystem.Schemas[0].ConnWithDomainUser)
            //    csb.IntegratedSecurity = true;
            if (dataSystem.Schemas[0].ConnEncrypt)
            {
                csb.IntegratedSecurity = true;
            }
            if (connectionTimeout > 0)
                csb.ConnectionTimeout = (uint)connectionTimeout;
            csb.AllowUserVariables = true;

            Connection = new MySqlConnection(csb.ToString());
        }

        /// <summary>
        /// Fun��o que abre uma conex�o
        /// </summary>
        public override void openConnection()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Log.Debug("abrirConexao.");
                    Connection.Open();
					IDbCommand comand;
					
                    // se for definido o schema sobre o qual as queries s�o executadas
                    if (!String.IsNullOrEmpty(schema_bd))
                    {
                        string querySchema_bd = "USE " + schema_bd;
                        comand = CreateCommand(querySchema_bd);
                        comand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceException("N�o foi poss�vel estabelecer liga��o � base de dados.", "PersistentSupportMySql.abrirConexao", "Error opening connection: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Instancia um novo adaptador de Sql
        /// </summary>
        /// <param name="query">A query de inicializa��o do adaptador</param>
        /// <returns>Um adaptador de sql</returns>
        public override IDbDataAdapter CreateAdapter(string query)
        {
            IDbCommand command = CreateCommand(query);
            return new MySqlDataAdapter((MySqlCommand)command);
        }

        /// <inheritdoc/>
        public override string generatePrimaryKey(string id_object, string id_field, int size, FieldType format)
        {
            if (format == FieldType.KEY_GUID)
                return Guid.NewGuid().ToString();

            MySqlCommand command = CreateCommand("updateCod") as MySqlCommand;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@p_Id_objecto", MySqlDbType.VarChar).Value = id_object.ToUpper();
            command.Parameters.Add("@p_Blksize", MySqlDbType.Int32).Value = 1;
            command.Parameters.Add("@o_proximo", MySqlDbType.Int32).Direction = ParameterDirection.Output;

            int codigoNovo = 0;
            command.ExecuteNonQuery();
            codigoNovo = Convert.ToInt32(command.Parameters["@o_proximo"].Value);

            if (codigoNovo < 1)
            {
                throw new PersistenceException(null, "PersistentSupportMySql.generatePrimaryKey", 
				                               "The primary key generated for object with id " + id_object + ", with size " + size + " and format " + format.ToString() + " is invalid: " + codigoNovo.ToString());
                // closeConnection();
                // return null;
            }

            if (format == FieldType.KEY_VARCHAR)
            {
                return codigoNovo.ToString().PadLeft(size);
            }

            return codigoNovo.ToString();
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
                throw new PersistenceException(ex.UserMessage, "PersistentSupportMySql.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions.ToString(), identifier) + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(null, "PersistentSupportMySql.getRecordPos",
											   string.Format("Error getting record position - [utilizador] {0}; [modulo] {1}; [area] {2}; [ordenacao] {3}; [valorChavePrimaria] {4}; [condicoes] {5}; [identificador] {6}: ",
											                 user.ToString(), module, area.ToString(), sorting.ToString(), primaryKeyValue, conditions.ToString(), identifier) + ex.Message, ex);
            }
        }

        public override IDbConnection GetConnectionToServer()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(Connection.ConnectionString);

            if (String.IsNullOrEmpty(builder.Password))
            {
                //After a connection is opened the first time, it's password is cleared.
                //If we have no password either it was opened before (we don't need a new connection), or it was never given, and it will fail either way.
                return this.Connection;
            }

            builder.Database = "";
            MySqlConnection connection = new MySqlConnection(builder.ConnectionString);
            return connection;
        }

        public override bool CheckIfDatabaseExists(string database)
        {
            if (String.IsNullOrEmpty(database))
                database = this.Connection.Database;

            //If a connection was already opened, we already know that it exists
            if (database == this.Connection.Database && this.Connection.State == ConnectionState.Open)
                return true;

            string query = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{database}'";

            //Open a new one to the server
            var connection = (MySqlConnection)GetConnectionToServer();
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(query, connection))
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
