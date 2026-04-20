using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Base class to support the specificities of the DBMSs
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified: CX 2011.11.02
    /// Reviewed:
    /// -->
    /// </remarks>
    public abstract class Dialect
    {
        private readonly IDictionary<DbType, string> dbTypes;
        private readonly IDictionary<CustomDbType, string> customDbTypes;
        private readonly IDictionary<SqlFunctionType, SqlFunctionTemplate> sqlFunctions;

        /// <summary>
        /// Constructor. Initializes the supported functions.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        protected Dialect()
        {
            dbTypes = new Dictionary<DbType, string>();
            customDbTypes = new Dictionary<CustomDbType, string>();
            sqlFunctions = new Dictionary<SqlFunctionType, SqlFunctionTemplate>();

            // aritmetic operations
            RegisterFunction(SqlFunctionType.Add, new SqlFunctionTemplate("({0})", true, false, "+"));
            RegisterFunction(SqlFunctionType.Subtract, new SqlFunctionTemplate("({0})", true, false, "-"));
            RegisterFunction(SqlFunctionType.Multiply, new SqlFunctionTemplate("({0})", true, false, "*"));
            RegisterFunction(SqlFunctionType.Divide, new SqlFunctionTemplate("({0} / {1})"));

            // aggregate function
            RegisterFunction(SqlFunctionType.Count, new SqlFunctionTemplate("COUNT({0})", true, true));
            RegisterFunction(SqlFunctionType.Average, new SqlFunctionTemplate("AVG({0})"));
            RegisterFunction(SqlFunctionType.Max, new SqlFunctionTemplate("MAX({0})"));
            RegisterFunction(SqlFunctionType.Min, new SqlFunctionTemplate("MIN({0})"));
            RegisterFunction(SqlFunctionType.Sum, new SqlFunctionTemplate("SUM({0})"));
            RegisterFunction(SqlFunctionType.RowNumber, new SqlFunctionTemplate("ROW_NUMBER() OVER (ORDER BY {0})", true, false, ", ", new[] { "ASC","DESC" }));
            RegisterFunction(SqlFunctionType.GroupConcat, new SqlFunctionTemplate("STRING_AGG({0}, {1})"));

            // standard sql92 functions (can be overridden by subclasses)
            RegisterFunction(SqlFunctionType.Substring, new SqlFunctionTemplate("SUBSTRING({0}, {1}, {2})"));
            RegisterFunction(SqlFunctionType.Locate, new SqlFunctionTemplate("LOCATE({0}, {1}, {2})"));
            RegisterFunction(SqlFunctionType.Trim, new SqlFunctionTemplate("TRIM({0})"));
            RegisterFunction(SqlFunctionType.Length, new SqlFunctionTemplate("LENGTH({0})"));
            RegisterFunction(SqlFunctionType.BitLength, new SqlFunctionTemplate("BIT_LENGTH({0})"));
            RegisterFunction(SqlFunctionType.Coalesce, new SqlFunctionTemplate("COALESCE({0})", true));
            RegisterFunction(SqlFunctionType.NullIf, new SqlFunctionTemplate("NULLIF({0}, {1})"));
            RegisterFunction(SqlFunctionType.Absolute, new SqlFunctionTemplate("ABS({0})"));
            RegisterFunction(SqlFunctionType.Module, new SqlFunctionTemplate("MOD({0}, {1})"));
            RegisterFunction(SqlFunctionType.SquareRoot, new SqlFunctionTemplate("SQRT({0})"));
            RegisterFunction(SqlFunctionType.Upper, new SqlFunctionTemplate("UPPER({0})"));
            RegisterFunction(SqlFunctionType.Lower, new SqlFunctionTemplate("LOWER({0})"));
            RegisterFunction(SqlFunctionType.Cast, new SqlFunctionTemplate("CAST({0} AS {1})", false, false, "AS", new[] { "CHAR", "INT" }));
            RegisterFunction(SqlFunctionType.Extract, new SqlFunctionTemplate("EXTRACT({0} FROM {1})", false, false, "FROM", new[] { "SECOND", "MINUTE", "HOUR", "DAY", "MONTH", "YEAR" }));
            RegisterFunction(SqlFunctionType.Concat, new SqlFunctionTemplate("CONCAT({0})", true));

            RegisterFunction(SqlFunctionType.CurrentTimestamp, new SqlFunctionTemplate("CURRENT_TIMESTAMP()"));
            RegisterFunction(SqlFunctionType.SystemDate, new SqlFunctionTemplate("SYSDATE()"));

            //map second/minute/hour/day/month/year to ANSI extract(), override on subclasses
            RegisterFunction(SqlFunctionType.Second, new SqlFunctionTemplate("EXTRACT(SECOND FROM {0})"));
            RegisterFunction(SqlFunctionType.Minute, new SqlFunctionTemplate("EXTRACT(MINUTE FROM {0})"));
            RegisterFunction(SqlFunctionType.Hour, new SqlFunctionTemplate("EXTRACT(HOUR FROM {0})"));
            RegisterFunction(SqlFunctionType.Day, new SqlFunctionTemplate("EXTRACT(DAY FROM {0})"));
            RegisterFunction(SqlFunctionType.Month, new SqlFunctionTemplate("EXTRACT(MONTH FROM {0})"));
            RegisterFunction(SqlFunctionType.Year, new SqlFunctionTemplate("EXTRACT(YEAR FROM {0})"));

            RegisterFunction(SqlFunctionType.ToString, new SqlFunctionTemplate("CAST({0} AS CHAR)"));
            RegisterFunction(SqlFunctionType.Left, new SqlFunctionTemplate("LEFT({0}, {1})"));
            RegisterFunction(SqlFunctionType.Right, new SqlFunctionTemplate("RIGHT({0}, {1})"));

            RegisterFunction(SqlFunctionType.Iif, new SqlFunctionTemplate("CASE WHEN ({0}) THEN ({1}) ELSE ({2}) END"));

            RegisterFunction(SqlFunctionType.Custom, new SqlFunctionTemplate("{0}({1})", true));
            RegisterFunction(SqlFunctionType.SysCustom, new SqlFunctionTemplate("{0}({1})", true));
        }

        /// <summary>
        /// Map of the supported data types
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.07
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IDictionary<DbType, string> DbTypes
        {
            get
            {
                return dbTypes;
            }
        }

        /// <summary>
        /// Map of the supported data types
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.07
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IDictionary<CustomDbType, string> CustomDbTypes
        {
            get
            {
                return customDbTypes;
            }
        }

        /// <summary>
        /// Map of the supported functions
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IDictionary<SqlFunctionType, SqlFunctionTemplate> Functions
        {
            get
            {
                return sqlFunctions;
            }
        }

        /// <summary>
        /// Represents the null value in sql
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual string NullString
        {
            get { return "NULL"; }
        }

		/// <summary>
		/// Represents the usage of a function as a table source
		/// </summary>
		/// <remarks>
		/// <!--
		/// Author: CX 2012.03.29
		/// Modified:
		/// Reviewed:
		/// -->
		/// </remarks>
		public virtual string FunctionAsTableTemplate
		{
			get { return "{0}"; }
		}

        /// <summary>
        /// Stores a data type sq definition in the supported data types map
        /// </summary>
        /// <param name="function">The data type</param>
        /// <param name="template">The data type sql definition</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.07
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        protected void RegisterType(DbType type, string sql)
        {
            dbTypes[type] = sql;
        }

        /// <summary>
        /// Stores a data type sq definition in the supported data types map
        /// </summary>
        /// <param name="function">The data type</param>
        /// <param name="template">The data type sql definition</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.07
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        protected void RegisterType(CustomDbType type, string sql)
        {
            customDbTypes[type] = sql;
        }

        /// <summary>
        /// Stores a function definition in the supported functions map
        /// </summary>
        /// <param name="function">The function type</param>
        /// <param name="template">The function template</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        protected void RegisterFunction(SqlFunctionType function, SqlFunctionTemplate template)
        {
            sqlFunctions[function] = template;
        }

        /// <summary>
        /// Returns an array with all the registered types as strings
        /// </summary>
        /// <returns>String array</returns>
        protected string[] RegisteredTypesToArray()
        {
            string[] typesArr = new string[DbTypes.Values.Count + CustomDbTypes.Values.Count];
            DbTypes.Values.CopyTo(typesArr, 0);
            CustomDbTypes.Values.CopyTo(typesArr, 0);

            return typesArr;
        }
		
		/// <summary>
        /// True if the DBMS supports the WITH( NOLOCK ) statement
        /// </summary>
        public virtual bool SupportsLocking
        {
            get { return true; }
        }

        /// <summary>
        /// True if the DBMS supports a max number of results specified in the query
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual bool SupportsLimit
        {
            get { return false; }
        }

        /// <summary>
        /// True if the DBMS supports skipping a specified number of results in the query
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual bool SupportsLimitOffset
        {
            get { return SupportsLimit; }
        }

        /// <summary>
        /// True if the limit should be inserted right after the select keyword, otherwise it is inserted at the end of the query
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual bool BindLimitParametersFirst
        {
            get { return false; }
        }

        /// <summary>
        /// True if the vendor supports returning data from a update or insert clause
        /// </summary>
        public virtual bool SupportsOutput => false;

        /// <summary>
        /// Extends a insert query with the output clause for the chosen columns
        /// </summary>
        /// <param name="sql">The insert query</param>
        /// <param name="columns">The columns to fetch in the return values</param>
        /// <remarks>The sql stringbuilder will be modified by this function</remarks>
        public virtual void AddOutputString(StringBuilder sql, IList<ColumnReference> columns)
        {
            throw new NotImplementedException("Dialect does not support output clause");
        }

        /// <summary>
        /// True if the limit value is the number of the last row to return, otherwise it is the number of rows to return
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual bool UseMaxForLimit
        {
            get { return false; }
        }

        /// <summary>
        /// Adds the limit clause to the query
        /// </summary>
        /// <param name="sql">The generated sql</param>
        /// <param name="offset">the value of the offset parameter</param>
        /// <param name="limit">the value of the limit parameter</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual void AddLimitString(StringBuilder sql, int offset, int? limit)
        {
            throw new NotImplementedException("Dialect does not support limit");
        }

        /// <summary>
        /// True if the named prefix should be used in the sql references to da variable
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.08.15
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public abstract bool UseNamedPrefixInSql
        {
            get;
        }

        /// <summary>
        /// True if the named prefix should be used in the parameter
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.08.15
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public abstract bool UseNamedPrefixInParameter
        {
            get;
        }

        /// <summary>
        /// The prefix for the sql variables for this DBMS
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public abstract string NamedPrefix
        {
            get;
        }
		
		/// <summary>
        /// Calculates how a parameter reference should be formed for the parameter list of this DBMS
        /// </summary>
		/// <param name="parameter">Name of the parameter</param>
        /// <returns>Corretly formed identifier for binding</returns>
		public string GetParameterBindIdentifier(string parameter)
		{
			return (UseNamedPrefixInParameter ? UseNamedPrefixInParameter + parameter : parameter);
		}
		
		/// <summary>
        /// Calculates how a parameter should be placed in the query text for this DBMS
        /// </summary>
		/// <param name="parameter">Name of the parameter</param>
        /// <returns>Corretly formed identifier for binding</returns>
		public string GetParameterIdentifier(string parameter)
		{
			return (UseNamedPrefixInSql ? UseNamedPrefixInParameter + parameter : parameter);
		}

        /// <summary>
        /// The open quote char fot his DBMS
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual char OpenQuote
        {
            get { return '"'; }
        }

        /// <summary>
        /// The close quote char for this DBMS
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual char CloseQuote
        {
            get { return '"'; }
        }

        /// <summary>
        /// True if the alias has to be defined with the keyword AS, otherwise false
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.29
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual bool UseAsOnAlias
        {
            get { return true; }
        }

        // TODO: check if this property is helpfull when using blobs
        //public virtual bool UseInputStreamToInsertBlob
        //{
        //    get { return true; }
        //}

        /// <summary>
        /// Checks if the specified name is quoted
        /// </summary>
        /// <param name="name">The name to check</param>
        /// <returns>True if the name is quoted, otherwise false</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual bool IsQuoted(string name)
        {
			if (name.Length < 2) { return false; }//PMP 2020-02-24 - Added to prevent out-of-bounds exception (if name is empty or 1 char)
            return (name[0] == OpenQuote && name[name.Length - 1] == CloseQuote);
        }

        /// <summary>
        /// Quotes the specified name
        /// </summary>
        /// <param name="name">The name to quote</param>
        /// <returns>The quoted name</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        protected virtual string Quote(string name)
        {
            string quotedName = name.Replace(OpenQuote.ToString(), new string(OpenQuote, 2));

            if (OpenQuote != CloseQuote)
            {
                quotedName = name.Replace(CloseQuote.ToString(), new string(CloseQuote, 2));
            }

            return OpenQuote + quotedName + CloseQuote;
        }

        /// <summary>
        /// Quotes the specified name for use as an alias
        /// </summary>
        /// <param name="aliasName">The name to quote</param>
        /// <returns>The quoted name for use as an alias</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual string QuoteForAliasName(string aliasName)
        {
            return IsQuoted(aliasName) ? aliasName : Quote(aliasName);
        }

        /// <summary>
        /// Quotes the specified name for use as a column name
        /// </summary>
        /// <param name="columnName">The name to quote</param>
        /// <returns>The quoted name for use as a column name</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual string QuoteForColumnName(string columnName)
        {
            return IsQuoted(columnName) ? columnName : Quote(columnName);
        }

        /// <summary>
        /// Quotes the specified name for use as a table name
        /// </summary>
        /// <param name="tableName">The name to quote</param>
        /// <returns>The quoted name for use as a table name</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual string QuoteForTableName(string tableName)
        {
            return IsQuoted(tableName) ? tableName : Quote(tableName);
        }

        /// <summary>
        /// Quotes the specified name for use as a schema name
        /// </summary>
        /// <param name="schemaName">The name to quote</param>
        /// <returns>The quoted name for use as a schema name</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.11.02
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual string QuoteForSchemaName(string schemaName)
        {
            return IsQuoted(schemaName) ? schemaName : Quote(schemaName);
        }

        /// <summary>
        /// True if table names should be preceeded by the schema name
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: RR 2012.03.19
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual bool UseSchemaOnTableName
        {
            get { return true; }
        }

        /// <summary>
        /// Unquotes the specified name
        /// </summary>
        /// <param name="quoted">The name to unquote</param>
        /// <returns>The unquoted name</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual string UnQuote(string quoted)
        {
            string unquoted;

            if (IsQuoted(quoted))
            {
                unquoted = quoted.Substring(1, quoted.Length - 2);
            }
            else
            {
                unquoted = quoted;
            }

            unquoted = unquoted.Replace(new string(OpenQuote, 2), OpenQuote.ToString());

            if (OpenQuote != CloseQuote)
            {
                unquoted = unquoted.Replace(new string(CloseQuote, 2), CloseQuote.ToString());
            }

            return unquoted;
        }

        /// <summary>
        /// Unquotes a list of names
        /// </summary>
        /// <param name="quoted">The list with the names to be unquoted</param>
        /// <returns>A list with the names unquoted</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public virtual string[] UnQuote(string[] quoted)
        {
            string[] unquoted = new string[quoted.Length];

            for (int i = 0; i < quoted.Length; i++)
            {
                unquoted[i] = UnQuote(quoted[i]);
            }

            return unquoted;
        }

        /// <summary>
        /// Get DELETE statement with alias
        /// </summary>
        /// <param name="table_name">Table name</param>
        /// <param name="alias">Alias</param>
        /// <returns>DELETE FROM [<table_name>]</returns>
        public virtual string DeleteStatement(string table_name, string alias)
        {
            return string.Format("DELETE FROM {0}", table_name);
        }

        /// <summary>
        /// Get UPDATE statement with alias
        /// </summary>
        /// <param name="table_name">Table name</param>
        /// <param name="alias">Alias</param>
        /// <param name="set_Clause">SET Clause</param>
        /// <returns>UPDATE [<table_name>] SET expression</returns>
        public virtual string UpdateStatement(string table_name, string alias, string set_Clause)
        {
            return string.Format("UPDATE {0} {1}", table_name, set_Clause);
        }

        /// <summary>
        /// Get UPDATE statement with alias
        /// </summary>
        /// <param name="table_name">Table name</param>
        /// <param name="alias">Alias</param>
        /// <param name="set_Clause">SET Clause</param>
		/// <param name="join_Clause">JOIN Clause</param>
        /// <returns>UPDATE [<table_name>] SET expression JOIN clause</returns>
        public virtual string UpdateStatement(string table_name, string alias, string set_Clause, string join_Clause)
        {
            return string.Format("UPDATE {0} {1} FROM {0} {2} {3}", table_name, set_Clause, alias, join_Clause);
        }
        /// <summary>
        /// Overriden. Returns a <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="System.Object"/></returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override string ToString()
        {
            return GetType().ToString();
        }

        /// <summary>
        /// For every type of database is required use the different format of datetime value (when used on CriteriaSet as where condition)
        /// TODO: Nullable DateTime ?
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>Returns the DateTime in valid format for specific type of database</returns>
        public virtual object GetValidDateTime(DateTime dateTime)
        {
            return dateTime;
        }
    }
}
