using System;
using System.Data;
using System.Data.SQLite;
using CSGenio.framework;
using Quidgest.Persistence.Dialects;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Generic;
using System.IO;

namespace CSGenio.persistence
{
    /// <summary>
    /// Summary description for SuportePersistenteSQLServerCE.
    /// </summary>
    public class PersistentSupportSQLite : PersistentSupport
    {
        public override IDbDataParameter CreateParameter()
        {
            return new SQLiteParameter();
        }

        /// <summary>
        /// Contructor
        /// </summary>
        public PersistentSupportSQLite()
        {
			Dialect = new SqliteDialect();
        }

        /// <inheritdoc/>
        public override bool IsErrorTransient(Exception ex)
        {
            //not implemented yet
            return false;
        }

        protected override void BuildConnection(DataSystemXml dataSystem, string login, string password, int connectionTimeout = 0)
        {
            SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder();

            string path = dataSystem.Server;
            if (!Path.IsPathRooted(dataSystem.Server))
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            csb.FullUri = "file://" + Path.Combine(path, dataSystem.Schemas[0].Schema + ".db");

            if (password == null)
            {
                csb.Password = dataSystem.PasswordDecode();
            }
            else
            {
                csb.Password = password;
            }
            csb.Version = 3;
            csb.BinaryGUID = false;

            Connection = new SQLiteConnection(csb.ToString());
        }

        /// <summary>
        /// Instancia um novo adaptador de Sql
        /// </summary>
        /// <param name="query">A query de inicializa��o do adaptador</param>
        /// <returns>Um adaptador de sql</returns>
        public override IDbDataAdapter CreateAdapter(string query)
        {
            IDbCommand command = CreateCommand(query);
            return new SQLiteDataAdapter((SQLiteCommand)command);
        }

        /// <inheritdoc/>
        public override string generatePrimaryKey(string id_object, string id_field, int size, FieldType format)
        {
            if (format == FieldType.KEY_GUID)
            {
                return Guid.NewGuid().ToString();
            }

			throw new PersistenceException(null, "PersistentSupportSQLite.generatePrimaryKey", 
				                           "Error generating primary key of type " + format.ToString() + " and size " + size.ToString() + " for object with id " + id_object + ".");
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
            throw new NotImplementedException();
        }

        public override bool CheckIfDatabaseExists(string database)
        {
            throw new NotImplementedException();
        }

        public override void Drop(string schema)
        {
            throw new NotImplementedException();
        }
    }
}
