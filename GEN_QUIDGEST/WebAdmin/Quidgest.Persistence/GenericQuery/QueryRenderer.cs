using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Class the renders queries
    /// </summary>
    public class QueryRenderer
    {
        /// <summary>
        /// Persistent support where the query is to be runned
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IPersistentSupport PersistentSupport
        {
            get;
            private set;
        }

        /// <summary>
        /// Helper property to generate distinct parameter names
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        private int ParameterIndex
        {
            get;
            set;
        }

        /// <summary>
        /// The list of generated parameters
        /// </summary>
        private List<IDbDataParameter> Parameters
        {
            get; 
            set;
        } = new List<IDbDataParameter>();

        /// <summary>
        /// List of named parameters. Used to avoid adding two times the same parameter
        /// </summary>
        private List<string> NamedParameters {
            get; 
            set;
        } = new List<string>();


        /// <summary>
        /// The list of generated parameters
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<IDbDataParameter> ParameterList
        {
            get
            {
                return Parameters.AsReadOnly();
            }
        }
		
		/// <summary>
        /// Generates a string representing the value of each parameter in the query
        /// </summary>
        /// <returns>A string representing the value of each parameter in the query</returns>
        public string PrintParameters()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach(var p in ParameterList)
            {
                sb.Append(p.ParameterName);
                sb.Append(": \"");
                string v = p.Value.ToString();
                if (v.Length > 100)
                {
                    sb.Append(v.Substring(0, 100));
                    sb.Append("...");
                }
                else
                    sb.Append(v);
                sb.Append("\", ");
            }
            if (ParameterList.Count > 0)
                sb.Remove(sb.Length - 2, 2);
            sb.Append("}");
            
            return sb.ToString();
        }

        private QuerySchemaMapping m_schemaMapping = new QuerySchemaMapping();
        /// <summary>
        /// Mapping from placeholder schemas to configured schemas
        /// </summary>
        public QuerySchemaMapping SchemaMapping
        {
            get
            {
                return m_schemaMapping;
            }
            set
            {
                m_schemaMapping = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="persistentSupport">The persistent support where this query is to be runned</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public QueryRenderer(IPersistentSupport persistentSupport)
        {
            if (persistentSupport == null)
            {
                throw new ArgumentNullException("persistentSupport");
            }

            PersistentSupport = persistentSupport;
        }

        /// <summary>
        /// Reset all parameters created
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        protected void ClearParameters()
        {
            ParameterIndex = 0;
            Parameters.Clear();
            NamedParameters.Clear();
        }

        /// <summary>
        /// Creates a new parameter for this query
        /// </summary>
        /// <param name="value">The value for the parameter</param>
        /// <returns>The parameter created</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.08.15
        /// Reviewed:
        /// -->
        /// </remarks>
        private IDbDataParameter MakeParameter(string name, object value)
        {
            var p = PersistentSupport.CreateParameter(name, value); 
            if(!NamedParameters.Contains(name))
            {
                Parameters.Add(p);
                NamedParameters.Add(name);
            }
            return p;
        }

        private string NextParameterName()
        {
            var idx = ++ParameterIndex;
            return "param" + idx;
        }

        /// <summary>
        /// Generates the sql and the necessary parameters to run the query
        /// </summary>
        /// <returns>The sql for the query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CJP 2015.12.29
        /// Reviewed:
        /// -->
        /// </remarks>
        public string GetSql(SelectQuery query)
        {
            return GetSql(query, true);
        }

        private string GetSql(SelectQuery query, bool isMainQuery)
        {
            if (isMainQuery)
            {
                ClearParameters();
            }

            if (query.PageProp != 1 && query.PageSizeProp == null)
            {
                throw new Exception("Page number cannot be different than 1 if the page size if the page size is not specified");
            }

            var dialect = PersistentSupport.Dialect;

            StringBuilder sql = new StringBuilder("SELECT ");

            if (query.DistinctProp)
            {
                sql.Append("DISTINCT ");
            }

            if (dialect.BindLimitParametersFirst && query.NeedsPaging)
            {
                // PageSize can only be null if Page is 1. In that case (Page - 1) * PageSize will be 0 and
                // we can use whatever value for PageSize if it is null. In this case we use the default value.
                dialect.AddLimitString(sql, (query.PageProp - 1) * query.PageSizeProp.GetValueOrDefault() + query.OffsetProp, query.PageSizeProp);
            }

            for (int i = 0; i < query.SelectFields.Count; i++)
            {
                if (i > 0)
                {
                    sql.Append(", ");
                }
                sql.Append(" (");
                sql.Append(GetSql(query.SelectFields[i].Expression));
                sql.Append(") ");
                if (dialect.UseAsOnAlias)
                {
                    sql.Append("AS ");
                }
                if (isMainQuery)
                {
                    sql.Append(dialect.QuoteForAliasName(query.SelectFields[i].Alias));
                }
                else
                {
                    sql.Append(dialect.QuoteForColumnName(query.SelectFields[i].Alias));
                }
            }

            if (query.FromTable != null)
            {
                sql.Append(" FROM ");
                sql.Append(GetSql(query.FromTable) + " ");
                if (dialect.UseAsOnAlias)
                {
                    sql.Append("AS ");
                }
                sql.Append(dialect.QuoteForAliasName(query.FromTable.TableAlias));

                // [RC] 24/05/2017 - Dirty solution to add update locks to the query
                // In the future we should address this differently, since the different types of lock
                // are mutually exclusive
                //if(query.noLock && dialect.SupportsLocking)
                //    sql.Append(" WITH (NoLOCK) ");
                if (dialect.SupportsLocking)
                {
                    if (query.updateLock)
                        sql.Append(" WITH (UPDLOCK) ");
                    else if (query.noLock)
                        sql.Append(" WITH (NoLOCK) ");
                }

                if (query.Joins != null && query.Joins.Count > 0)
                {
                    AddJoin(sql, new List<ITableSource>(new[] { query.FromTable }), query.Joins,query.noLock);
                }

                if (query.WhereCondition != null)
                {
                    StringBuilder whereSql = new StringBuilder();
                    AddCriteriaExpression(whereSql, query.WhereCondition);
                    if (whereSql.Length > 0)
                    {
                        sql.Append(" WHERE ");
                        sql.Append(whereSql.ToString());
                    }
                }

                if (query.GroupByFields.Count > 0)
                {
                    sql.Append(" GROUP BY");
                    for (int i = 0; i < query.GroupByFields.Count; i++)
                    {
                        if (i > 0)
                        {
                            sql.Append(", ");
                        }
                        sql.Append(" ");
                        sql.Append(GetSql(query.GroupByFields[i] is SelectField ? ((SelectField)query.GroupByFields[i]).Expression : query.GroupByFields[i]));
                    }
                }

                if (query.HavingCondition != null)
                {
                    StringBuilder havingSql = new StringBuilder();
                    AddCriteriaExpression(havingSql, query.HavingCondition);
                    if (havingSql.Length > 0)
                    {
                        sql.Append(" HAVING ");
                        sql.Append(havingSql.ToString());
                    }
                }

                foreach (QueryUnion queryUnion in query.UnionQueries)
                {
                    sql.Append(" UNION ");
                    if (queryUnion.All)
                    {
                        sql.Append("ALL ");
                    }
                    sql.Append(GetSql(queryUnion.Query, false));
                }

                if (query.OrderByFields.Count > 0)
                {
                    sql.Append(" ORDER BY");
                    for (int i = 0; i < query.OrderByFields.Count; i++)
                    {
                        if (i > 0)
                        {
                            sql.Append(", ");
                        }
                        sql.Append(" ");

                        if (query.OrderByFields[i].ColumnIndex != null)
                        {
                            int idx = query.OrderByFields[i].ColumnIndex.Value - 1;
                            if (idx >= query.SelectFields.Count)
                            {
                                throw new IndexOutOfRangeException("Index " + (idx + 1) + " is out of range of the select fields");
                            }
							// When performing ROW_NUMBER, order_by_expression can only refer
							// to columns made available by the FROM clause (not aliases)
                            sql.Append(GetSql(query.SelectFields[idx].Expression));
                        }
                        else
                        {
                            sql.Append(GetSql(query.OrderByFields[i].Expression));
                        }
                        sql.Append(query.OrderByFields[i].Order == SortOrder.Descending ? " DESC" : " ASC");
                    }
                }
            }

            if (!dialect.BindLimitParametersFirst && query.NeedsPaging)
            {
                // PageSize can only be null if Page is 1. In that case (Page - 1) * PageSize will be 0 and
                // we can use whatever value for PageSize if it is null. In this case we use the default value.
                dialect.AddLimitString(sql, (query.PageProp - 1) * query.PageSizeProp.GetValueOrDefault() + query.OffsetProp, query.PageSizeProp);
            }

            return sql.ToString();
        }

        /// <summary>
        /// Generates the sql and the necessary parameters to run the query
        /// </summary>
        /// <returns>The sql for the query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string GetSql(InsertQuery query)
        {
            ClearParameters();

            var dialect = PersistentSupport.Dialect;

            StringBuilder sql = new StringBuilder("INSERT INTO ");
            sql.Append(GetSql(query.IntoTable));
            sql.Append(" (");

            for (int i = 0; i < query.Values.Count; i++)
            {
                if (i > 0)
                {
                    sql.Append(", ");
                }
                sql.Append(dialect.QuoteForColumnName(query.Values[i].Column.ColumnName));
            }

            sql.Append(") VALUES (");

            for (int i = 0; i < query.Values.Count; i++)
            {
                if (i > 0)
                {
                    sql.Append(", ");
                }
                sql.Append(ParseTerm(query.Values[i].Value));
            }

            sql.Append(")");

            if (dialect.SupportsOutput)
                dialect.AddOutputString(sql, query.Outputs);

            return sql.ToString();
        }

        /// <summary>
        /// Generated a multi-row insert query and the necessary parameters
        /// </summary>
        /// <param name="query">A list of individual insert queries to join into a single one</param>
        /// <returns>The query command text</returns>
        /// <remarks>Callers are responsible for ensuring parameter number limits are respected</remarks>
        public string GetSql(List<InsertQuery> query)
        {
            if (query.Count == 0)
                throw new ArgumentException("Must have at least one row", nameof(query));

            ClearParameters();

            var dialect = PersistentSupport.Dialect;

            //use the first row as the header, every row needs to have the same header
            var header = query[0];

            StringBuilder sql = new StringBuilder("INSERT INTO ");
            sql.Append(GetSql(header.IntoTable));
            sql.Append(" (");

            for (int i = 0; i < header.Values.Count; i++)
            {
                if (i > 0)
                {
                    sql.Append(", ");
                }
                sql.Append(dialect.QuoteForColumnName(header.Values[i].Column.ColumnName));
            }

            sql.Append(") VALUES");

            foreach (var row in query)
            {
                sql.Append(" (");
                for (int i = 0; i < row.Values.Count; i++)
                {
                    if (i > 0)
                    {
                        sql.Append(", ");
                    }
                    sql.Append(ParseTerm(row.Values[i].Value));
                }
                sql.Append("),");
            }
            sql.Length -= 1;

            return sql.ToString();
        }

        /// <summary>
        /// Generates the sql and the necessary parameters to run the query
        /// </summary>
        /// <returns>The sql for the query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string GetSql(DeleteQuery query)
        {
            ClearParameters();

            var dialect = PersistentSupport.Dialect;

            StringBuilder sql = new StringBuilder();
            sql.Append(dialect.DeleteStatement(GetSql(query.DeleteTable), dialect.QuoteForAliasName(query.DeleteTable.TableAlias)));

            if (query.WhereCondition != null)
            {
				StringBuilder whereSql = new StringBuilder();
				AddCriteriaExpression(whereSql, query.WhereCondition);
				if (whereSql.Length > 0)
				{
					sql.Append(" WHERE ");
					sql.Append(whereSql.ToString());
				}
            }

            return sql.ToString();
        }

        /// <summary>
        /// Generates the sql and the necessary parameters to run the query
        /// </summary>
        /// <returns>The sql for the query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: JMT 2015.04.08
        /// Reviewed:
        /// -->
        /// </remarks>
        public string GetSql(UpdateQuery query)
        {
            ClearParameters();

            var dialect = PersistentSupport.Dialect;

            StringBuilder SET_Clause = new StringBuilder("SET ");
            for (int i = 0; i < query.SetValues.Count; i++)
            {
                if (i > 0)
                {
                    SET_Clause.Append(", ");
                }
                SET_Clause.Append(GetSql(query.SetValues[i].Column));
                SET_Clause.Append(" = ");
                SET_Clause.Append(ParseTerm(query.SetValues[i].Value));
            }

            StringBuilder sql = new StringBuilder();
            if (query.Joins != null && query.Joins.Count > 0)
            {
                StringBuilder JOIN_Clause = new StringBuilder();
                AddJoin(JOIN_Clause, new List<ITableSource>(new[] { query.UpdateTable }), query.Joins, false);
                sql.Append(dialect.UpdateStatement(GetSql(query.UpdateTable), dialect.QuoteForAliasName(query.UpdateTable.TableAlias), SET_Clause.ToString(), JOIN_Clause.ToString()));
            }
            else
                sql.Append(dialect.UpdateStatement(GetSql(query.UpdateTable), dialect.QuoteForAliasName(query.UpdateTable.TableAlias), SET_Clause.ToString()));

            if (query.WhereCondition != null)
            {
				StringBuilder whereSql = new StringBuilder();
				AddCriteriaExpression(whereSql, query.WhereCondition);
				if (whereSql.Length > 0)
				{
					sql.Append(" WHERE ");
					sql.Append(whereSql.ToString());
				}
            }

            return sql.ToString();
        }

        private string GetSql(SqlValue val)
        {
            if (val.Value == null)
            {
                return PersistentSupport.Dialect.NullString;
            }
            string pname = "";
            if(String.IsNullOrEmpty(val.ParamName))
            {
                pname = NextParameterName();
            }
            else
            {
                pname = val.ParamName;                
            }

            MakeParameter(pname, val.Value);

            return (PersistentSupport.Dialect.UseNamedPrefixInSql ? PersistentSupport.Dialect.NamedPrefix : "") + pname;
        }

        private string GetSql(SqlLiteral val)
        {
            if (val.Value == null)            
                return PersistentSupport.Dialect.NullString;
            
            if(val.Value is string)
            {
                var strValue = (string)val.Value;
                //Escape '
                strValue = strValue.Replace("'", "''");
                return $"'{strValue}'";
            }
            else
            {
                return val.Value.ToString();
            }
        }

        private string GetSql(SqlKeyword keyword)
        {
            return keyword.Keyword;
        }

        private string GetSql(SqlFunction func)
        {
            Dialect dialect = PersistentSupport.Dialect;

            if (!dialect.Functions.ContainsKey(func.Function))
            {
                throw new NotSupportedException(String.Format("Function {0} is not supported in the dialect {1}.", func.Function, dialect));
            }

            var template = dialect.Functions[func.Function];

            if (!template.HasVariableNumOfArgs)
            {
                if (func.Arguments.Length != template.NumOfArgs)
                {
                    throw new ArgumentException("Incorrect number of arguments");
                }
            }

            int numOfArgs = func.Arguments.Length;

            if (numOfArgs == 0)
            {
                if (template.HasVariableNumOfArgs)
                {
                    // if the function accepts a variable number of arguments
                    // and it is being invoked with no arguments, replace the
                    // argument placeholder with the empty string
                    return String.Format(template.Template, String.Empty);
                }
                else
                {
                    // there is no argument placeholder, so the sql is the template
                    return template.Template;
                }
            }

            // convert the arguments to sql
            string[] argsSql = new string[numOfArgs];
            for (int i = 0; i < numOfArgs; i++)
            {
                var arg = func.Arguments[i];

                if (i == 0 && (func.Function == SqlFunctionType.Custom || func.Function == SqlFunctionType.SysCustom))
                {
                    // in custom functions, the 1st argument is the name of the function
                    argsSql[i] = Convert.ToString(arg);
                    continue;
                }

                if (arg == null)
                {
                    // hardcode null in the query
                    // passing null as a parameter don't have the desired result
                    argsSql[i] = dialect.NullString;
                    continue;
                }

                var expr = arg as ISqlExpression;
                if (expr != null)
                {
                    // each ISqlExpression knows what sql should generate
                    argsSql[i] = GetSql(expr);
                    continue;
                }

                var keyword = arg as SqlKeyword;
                if (keyword != null)
                {
                    argsSql[i] = GetSql(keyword);
                    continue;
                }

                var crit = arg as CriteriaSet;
                if (crit != null)
                {
                    argsSql[i] = GetSql(crit);
                    continue;
                }

                var fieldRef = arg as FieldRef;
                if (fieldRef != null)
                {
                    argsSql[i] = GetSql(fieldRef);//"[" + fieldRef.Area + "]." + "[" + fieldRef.Field + "]";
                    continue;
                }

                if (template.AllowsAsterik && (arg as String) == "*")
                {
                    argsSql[i] = "*";
                    continue;
                }

                var sortColumn = arg as ColumnSort;
                if (sortColumn != null)
                {
                    argsSql[i] = GetSql(sortColumn);
                    continue;
                }

                if (template.Keywords != null && ((arg is String) || arg is DbType || arg is CustomDbType))
                {
                    // check if the value is a keyword
                    string keyArg = null;
                    if (arg is String)
                    {
                        keyArg = arg as String;
                    }
                    else if (arg is DbType)
                    {
                        keyArg = dialect.DbTypes[(DbType)arg];
                    }
                    else if (arg is CustomDbType)
                    {
                        keyArg = dialect.CustomDbTypes[(CustomDbType)arg];
                    }
                    string key = null;
                    foreach (string x in template.Keywords)
	                {
                        if (String.Equals(x, keyArg, StringComparison.InvariantCultureIgnoreCase))
                        {
                            key = x;
                            break;
                        }
	                }

                    if (!String.IsNullOrEmpty(key))
                    {
                        // if it is a keyword use it, don't treat it like a string
                        argsSql[i] = key;
                        continue;
                    }
                }

                // otherwise, make a parameter for the value
                string pname = NextParameterName();
                MakeParameter(pname, arg);
                argsSql[i] = (dialect.UseNamedPrefixInSql ? dialect.NamedPrefix : "") + pname;
            }

            if (template.HasVariableNumOfArgs)
            {
                // don't forget to apply the argument separator to the parameters
                // in a custom function the 1st argument is the function name
                if (func.Function == SqlFunctionType.Custom || func.Function == SqlFunctionType.SysCustom)
                {
                    string[] realArgsSql = new string[argsSql.Length - 1];
                    if (realArgsSql.Length > 0)
                    {
                        for (int i = 1; i < argsSql.Length; i++)
                        {
                            realArgsSql[i - 1] = argsSql[i];
                        }
                    }
                    return String.Format(template.Template, argsSql[0], String.Join(template.ArgsSeparator, realArgsSql));
                }
                else
                {
                    return String.Format(template.Template, String.Join(template.ArgsSeparator, argsSql));
                }
            }
            else
            {
                // the argument separator is already in the template, so apply the arguments only
                return String.Format(template.Template, argsSql);
            }
        }

        /// <summary>
        /// Returns the name of the field for use in sql
        /// </summary>
        /// <param name="mainQuery">The query where the field will be used</param>
        /// <returns>The name of the field for use in sql</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        private string GetSql(SelectField field)
        {
            return PersistentSupport.Dialect.QuoteForAliasName(field.Alias);
        }

        private string GetSql(ColumnReference column)
        {
            if (String.IsNullOrEmpty(column.TableAlias))
            {
                return PersistentSupport.Dialect.QuoteForColumnName(column.ColumnName);
            }
            else
            {
                return String.Format("{0}.{1}",
                    PersistentSupport.Dialect.QuoteForAliasName(column.TableAlias),
                    PersistentSupport.Dialect.QuoteForColumnName(column.ColumnName));
            }
        }

        private string GetSql(FieldRef field)
        {
            return String.Format("{0}.{1}",
                    PersistentSupport.Dialect.QuoteForAliasName(field.Area),
                    PersistentSupport.Dialect.QuoteForColumnName(field.Field));
        }

        private string GetSql(ColumnSort column)
        {
            return string.Format("{0} {1}", GetSql(column.Expression), column.Order.Equals(SortOrder.Ascending) ? "ASC" : "DESC");
        }

        private string GetSql(ISqlExpression expression)
        {
            Dialect dialect = PersistentSupport.Dialect;

            if (expression is SelectQuery)
            {
                return "(" + GetSql((SelectQuery)expression, false) + ")";
            }
            else if (expression is SqlValue)
            {
                return GetSql((SqlValue)expression);
            }
            else if (expression is SqlLiteral)
            {
                return GetSql((SqlLiteral)expression);
            }
            else if (expression is SqlFunction)
            {
                return GetSql((SqlFunction)expression);
            }
            else if (expression is SelectField)
            {
                return GetSql((SelectField)expression);
            }
            else if (expression is ColumnReference)
            {
                return GetSql((ColumnReference)expression);
            }
            else if (expression is ColumnSort)
            {
                return GetSql((ColumnSort)expression);
            }

            throw new NotSupportedException("ISqlExpression type " + expression.GetType() + " not supported");
        }

        private string GetSql(CriteriaSet crit)
        {
            StringBuilder b = new StringBuilder();
            AddCriteriaExpression(b, crit);
            return b.ToString();
        }

        private string GetSql(TableReference table)
        {
            string result = PersistentSupport.Dialect.QuoteForTableName(table.TableName);
            if (!String.IsNullOrEmpty(table.SchemaName) && PersistentSupport.Dialect.UseSchemaOnTableName)
            {
                string translatedSchema = SchemaMapping.GetValue(table.SchemaName.ToUpperInvariant());
                result = PersistentSupport.Dialect.QuoteForSchemaName(translatedSchema) + "." + result;
            }
            return result;
        }

        private string GetSql(QueryReference query)
        {
            return "(" + GetSql(query.Query, false) + ")";
        }

		private string GetSql(FunctionReference function)
		{
            Dialect dialect = PersistentSupport.Dialect;

			return String.Format(dialect.FunctionAsTableTemplate, GetSql(function.Function));
		}

        private string GetSql(ITableSource source)
        {
            if (source is TableReference)
            {
                return GetSql((TableReference)source);
            }
            else if (source is QueryReference)
            {
                return GetSql((QueryReference)source);
            }
			else if (source is FunctionReference)
			{
				return GetSql((FunctionReference)source);
			}

            throw new NotSupportedException("ITableSource type " + source.GetType() + " not supported");
        }

        private bool IsTableInList(string tableAlias, IList<ITableSource> tables)
        {
            foreach (ITableSource table in tables)
            {
                if (table.TableAlias == tableAlias)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds the joins to a query recursively, according to the satisfied conditions
        /// </summary>
        /// <param name="mainQuery">The query where the joins will be applied</param>
        /// <param name="sql">The current generated sql</param>
        /// <param name="availableTables">The tables already present in the query</param>
        /// <param name="join">The list of all joins in the query</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        private void AddJoin(StringBuilder sql, IList<ITableSource> availableTables, IList<TableJoin> join, Boolean noLock)
        {
            var dialect = PersistentSupport.Dialect;

            // find all joins that were not applied yet and that have all the necessary tables generated already
            IList<TableJoin> links = new List<TableJoin>();
            foreach (TableJoin x in join)
            {
                if (!IsTableInList(x.Table.TableAlias, availableTables) && x.CanSatisfyReferences(availableTables))
                {
                    links.Add(x);
                }
            }

            if (links.Count == 0)
            {
                // all joins were applied, so we can end the recursion
                return;
            }

            foreach (var link in links)
            {
                availableTables.Add(link.Table);

                if (link.JoinType == TableJoinType.Left)
                {
                    sql.Append(" LEFT JOIN ");
                }
                else if (link.JoinType == TableJoinType.Right)
                {
                    sql.Append(" RIGHT JOIN ");
                }
                else if (link.JoinType == TableJoinType.Full)
                {
                    sql.Append(" FULL OUTER JOIN ");
                }
                else if (link.JoinType == TableJoinType.Cross)
                {
                    sql.Append(" CROSS JOIN ");
                }
                else
                {
                    sql.Append(" JOIN ");
                }

                sql.Append(GetSql(link.Table) + " ");
                if (dialect.UseAsOnAlias)
                {
                    sql.Append("AS ");
                }
                sql.Append(dialect.QuoteForAliasName(link.Table.TableAlias));
				
				if (noLock && dialect.SupportsLocking)
                    sql.Append(" with (NOLOCK)");
					
                if (link.JoinType != TableJoinType.Cross)
                {
                    sql.Append(" ON ");
                    StringBuilder critSql = new StringBuilder();
                    AddCriteriaExpression(critSql, link.OnCondition);
                    sql.Append(critSql.ToString());
                }
            }

            // try to add the remaining joins, now that more tables are available to satisfy the join conditions
            AddJoin(sql, availableTables, join, noLock);
        }

        /// <summary>
        /// Adds the criteria expression to the query sql
        /// </summary>
        /// <param name="mainQuery">The query where the conditions will be applied</param>
        /// <param name="sql">The current generated sql</param>
        /// <param name="conditions">The conditions to render</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.30
        /// Modified: MH 2017.09.15 - Removed forSingleTable parameter
        /// Reviewed:
        /// -->
        /// </remarks>
        private void AddCriteriaExpression(StringBuilder sql, CriteriaSet conditions)
        {
			if (conditions == null)
			{
				return;
			}

            string criteriaSetOperator = ParseCriteriaSetOperator(conditions.Operation);

            // render the individual conditions
            for (int i = 0; i < conditions.Criterias.Count; i++)
            {
                if (i > 0)
                {
                    sql.Append(" ");
                    sql.Append(criteriaSetOperator);
                    sql.Append(" ");
                }
                AddCriteria(sql, conditions.Criterias[i]);
            }

			// we need to if there is already any SQL before appending the CriteriaSet Operator
            bool first = conditions.Criterias.Count == 0;

            // render the condition groups
            for (int i = 0; i < conditions.SubSets.Count; i++)
            {
                StringBuilder subSetSql = new StringBuilder();
                AddCriteriaExpression(subSetSql, conditions.SubSets[i]);
                if (subSetSql.Length > 0)
                {
                    if (!first)
                    {
                        sql.Append(" ");
                        sql.Append(criteriaSetOperator);
                        sql.Append(" ");
                    }
                    sql.Append(subSetSql.ToString());
                    first = false;
                }
            }

			if (sql.Length > 0)
			{
				sql.Append(")");
				sql.Insert(0, "(");
				if (conditions.Operation == CriteriaSetOperator.NotAnd
					|| conditions.Operation == CriteriaSetOperator.NotOr)
				{
					sql.Insert(0, "NOT");
				}
			}
        }

        private string ParseCriteriaSetOperator(CriteriaSetOperator criteriaSetOperator)
        {
            switch (criteriaSetOperator)
            {
                case CriteriaSetOperator.And:
                    return "AND";
				case CriteriaSetOperator.NotAnd:
					return "AND";
				case CriteriaSetOperator.Or:
                    return "OR";
				case CriteriaSetOperator.NotOr:
					return "OR";
				default:
                    throw new NotSupportedException("Unsupported criteria set operator " + criteriaSetOperator.ToString());
            }
        }

        /// <summary>
        /// Adds the criteria sql to the query sql
        /// </summary>
        /// <param name="mainQuery">The query where the criteria will be applied</param>
        /// <param name="sql">The current generated sql</param>
        /// <param name="criteria">The criteria to generate the sql from</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: MH 2017.09.15 - Removed forSingleTable parameter
        /// Reviewed:
        /// -->
        /// </remarks>
        private void AddCriteria(StringBuilder sql, Criteria criteria)
        {
            object rightTerm = criteria.RightTerm;
            if ((criteria.Operation == CriteriaOperator.Like || criteria.Operation == CriteriaOperator.NotLike)
                && rightTerm is string
                && rightTerm != null)
            {
                rightTerm = ((string)rightTerm).Replace('*', '%').Replace('?', '_');
            }

            // generate each operation term
            string parsedLeftTerm = ParseTerm(criteria.LeftTerm);
            string parsedRightTerm = ParseTerm(rightTerm);

            // apply the operator
            switch (criteria.Operation)
            {
                case CriteriaOperator.Equal:
                    sql.Append(parsedLeftTerm);
                    sql.Append(criteria.RightTerm == null ? " IS " : " = ");
                    sql.Append(parsedRightTerm);
                    break;
                case CriteriaOperator.NotEqual:
                    sql.Append(parsedLeftTerm);
                    sql.Append(criteria.RightTerm == null ? " IS NOT " : " <> ");
                    sql.Append(parsedRightTerm);
                    break;
                case CriteriaOperator.Greater:
                    sql.Append(parsedLeftTerm);
                    sql.Append(" > ");
                    sql.Append(parsedRightTerm);
                    break;
                case CriteriaOperator.GreaterOrEqual:
                    sql.Append(parsedLeftTerm);
                    sql.Append(" >= ");
                    sql.Append(parsedRightTerm);
                    break;
                case CriteriaOperator.Lesser:
                    sql.Append(parsedLeftTerm);
                    sql.Append(" < ");
                    sql.Append(parsedRightTerm);
                    break;
                case CriteriaOperator.LesserOrEqual:
                    sql.Append(parsedLeftTerm);
                    sql.Append(" <= ");
                    sql.Append(parsedRightTerm);
                    break;
                case CriteriaOperator.Like:
                    sql.Append(parsedLeftTerm);
                    sql.Append(" LIKE ");
                    sql.Append(parsedRightTerm);
                    break;
                case CriteriaOperator.NotLike:
                    sql.Append(parsedLeftTerm);
                    sql.Append(" NOT LIKE ");
                    sql.Append(parsedRightTerm);
                    break;
                case CriteriaOperator.In:
                    sql.Append(parsedLeftTerm);
                    sql.Append(" IN (");
                    sql.Append(parsedRightTerm);
                    sql.Append(") ");
                    break;
                case CriteriaOperator.NotIn:
                    sql.Append(parsedLeftTerm);
                    sql.Append(" NOT IN (");
                    sql.Append(parsedRightTerm);
                    sql.Append(") ");
                    break;
                case CriteriaOperator.Exists:
                    sql.Append(" EXISTS (");
                    sql.Append(parsedLeftTerm ?? parsedRightTerm);
                    sql.Append(") ");
                    break;
                case CriteriaOperator.NotExists:
                    sql.Append(" NOT EXISTS (");
                    sql.Append(parsedLeftTerm ?? parsedRightTerm);
                    sql.Append(") ");
                    break;
                default:
                    throw new NotSupportedException("Unsupported criteria operator " + criteria.Operation.ToString());
            }
        }

        /// <summary>
        /// Generates the sql of a term
        /// </summary>
        /// <param name="term">The term</param>
        /// <returns>The sql of a term</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: MH 2017.09.15 - Removed forSingleTable parameter
        /// Reviewed:
        /// -->
        /// </remarks>
        private string ParseTerm(object term)
        {
            var dialect = PersistentSupport.Dialect;

            if (term == null)
            {
                // hardcode null in the query
                // passing null as a parameter don't have the desired result
                return dialect.NullString;
            }

            if (term is ISqlExpression expr)
            {
                // each ISqlExpression knows what sql should generate
                return GetSql(expr);
            }

            if (term is IEnumerable collection && !(collection is string) && !(collection is byte[]) && !(collection is IEnumerable<byte>))
            {
                // collections of items have their item rendered individualy and are rendered as a list of values
                // so that they may be used with the operator IN
                // strings (collections of chars) and byte arrays (files, object data) are not rendered this way
                IList<string> parsedTermsAux = new List<string>();
                foreach (object x in collection)
                {
                    parsedTermsAux.Add(ParseTerm(x));
                }
                string[] parsedTerms = new string[parsedTermsAux.Count];
                parsedTermsAux.CopyTo(parsedTerms, 0);
                return "(" + String.Join("),(", parsedTerms) + ")";
            }

            if (term is FieldRef fieldRef)
            {
                return GetSql(fieldRef);//"[" + fieldRef.Area + "]." + "[" + fieldRef.Field + "]";
            }

            if (term is DataTable datatable)
            {
                string tvpname = NextParameterName();
                MakeParameter(tvpname, term);
                return "SELECT " + datatable.Columns[0].ColumnName + " FROM " + (dialect.UseNamedPrefixInSql ? dialect.NamedPrefix : "") + tvpname;
            }

			// otherwise, make a parameter for the value
            string pname = NextParameterName();
            MakeParameter(pname, GetValidTerm(term));
            return (dialect.UseNamedPrefixInSql ? dialect.NamedPrefix : "") + pname;
        }

		public object GetValidTerm(object term)
        {
            if (term is DateTime && ((DateTime)term) == DateTime.MinValue)
            {
                return null;
            }

            if (term is Guid && ((Guid)term) == Guid.Empty)
            {
                return null;
            }

            return term;
        }
	}
}
