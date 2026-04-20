using System;
using System.Collections.Generic;
using System.Text;
using Quidgest.Persistence.GenericQuery;
using System.Data;

namespace Quidgest.Persistence.Dialects
{
    /// <summary>
    /// Specificities of the DBMS MySql
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: RS 2015.11.09
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class MySqlDialect : Dialect
    {
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
		public override string FunctionAsTableTemplate
		{
			get
			{
				return "table({0})";
			}
		}

		/// <summary>
        /// Overriden. The prefix for the sql variables for this DBMS.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override string NamedPrefix
        {
            get
            {
                return "@";
            }
        }
		
		/// <summary>
        /// Overriden. The open quote char fot his DBMS.
        /// </summary>
        public override char OpenQuote
        {
            get
            {
                return '`';
            }
        }

        /// <summary>
        /// Overriden. The close quote char for this DBMS.
        /// </summary>
        public override char CloseQuote
        {
            get
            {
                return '`';
            }
        }

        /// <summary>
        /// Overriden. True if the named prefix should be used in the sql references to da variable.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.08.15
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool UseNamedPrefixInSql
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Overriden. True if the named prefix should be used in the parameter.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.08.15
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool UseNamedPrefixInParameter
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Overriden. True if the DBMS supports a max number of results specified in the query.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool SupportsLimit
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Overriden. True if the DBMS supports skipping a specified number of results in the query.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool SupportsLimitOffset
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Overriden. True if the limit should be inserted right after the select keyword, otherwise it is inserted at the end of the query.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool BindLimitParametersFirst
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// True if the DBMS supports the WITH( NOLOCK ) statement
        /// </summary>
        public override bool SupportsLocking
        {
            get { return false; }
        }

        /// <summary>
        /// Constructor. Initializes the supported functions.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.06.30
        /// Reviewed:
        /// -->
        /// </remarks>
        public MySqlDialect()
        {
            RegisterType(DbType.AnsiStringFixedLength, "CHAR(2000)");
            RegisterType(DbType.AnsiString, "BINARY");
            RegisterType(DbType.StringFixedLength, "CHAR(2000)");
            RegisterType(DbType.String, "BINARY");
			RegisterType(DbType.Boolean, "DECIMAL(1,0)");
			RegisterType(DbType.Byte, "DECIMAL(3,0)");
			RegisterType(DbType.Int16, "DECIMAL(5,0)");
			RegisterType(DbType.Int32, "DECIMAL(10,0)");
			RegisterType(DbType.Int64, "DECIMAL(20,0)");
			RegisterType(DbType.UInt16, "DECIMAL(5,0)");
			RegisterType(DbType.UInt32, "DECIMAL(10,0)");
			RegisterType(DbType.UInt64, "DECIMAL(20,0)");
			RegisterType(DbType.Currency, "DECIMAL(20,2)");
			RegisterType(DbType.Single, "FLOAT(24)");
			RegisterType(DbType.Double, "DOUBLE");
			RegisterType(DbType.Decimal, "DECIMAL(19,5)");
            RegisterType(DbType.Date, "DATE");
            RegisterType(DbType.DateTime, "TIMESTAMP(4)");
            RegisterType(DbType.Time, "TIMESTAMP(4)");
            RegisterType(DbType.Binary, "BLOB");

            RegisterType(CustomDbType.StandardAnsiString, "BINARY");
            RegisterType(CustomDbType.StandardDecimalSearch, "DECIMAL(38,10)");

            string[] typesArr = RegisteredTypesToArray();
            
            RegisterFunction(SqlFunctionType.GroupConcat, new SqlFunctionTemplate("GROUP_CONCAT({0}, {1})"));
            RegisterFunction(SqlFunctionType.Substring, new SqlFunctionTemplate("SUBSTR({0}, {1}, {2})"));
            RegisterFunction(SqlFunctionType.Locate, new SqlFunctionTemplate("INSTR({0}, {1}/*, {2}*/)"));
            RegisterFunction(SqlFunctionType.BitLength, new SqlFunctionTemplate("(VSIZE({0})*8)"));// MySQL Alternative ?
            RegisterFunction(SqlFunctionType.Concat, new SqlFunctionTemplate("CONCAT({0})", true, false, ","));

            RegisterFunction(SqlFunctionType.CurrentTimestamp, new SqlFunctionTemplate("CURRENT_TIMESTAMP"));
            RegisterFunction(SqlFunctionType.SystemDate, new SqlFunctionTemplate("SYSDATE"));

            RegisterFunction(SqlFunctionType.Round, new SqlFunctionTemplate("ROUND({0}, {1})"));

            RegisterFunction(SqlFunctionType.Cast, new SqlFunctionTemplate("CAST({0} AS {1})", false, false, "AS", typesArr));
            RegisterFunction(SqlFunctionType.Extract, new SqlFunctionTemplate("EXTRACT({0} FROM CAST({1} AS TIMESTAMP))", false, false, "FROM", new[] { "SECOND", "MINUTE", "HOUR", "DAY", "MONTH", "YEAR" }));

            RegisterFunction(SqlFunctionType.Second, new SqlFunctionTemplate("EXTRACT(SECOND FROM CAST({0} AS TIMESTAMP))"));
            RegisterFunction(SqlFunctionType.Minute, new SqlFunctionTemplate("EXTRACT(MINUTE FROM CAST({0} AS TIMESTAMP))"));
            RegisterFunction(SqlFunctionType.Hour, new SqlFunctionTemplate("EXTRACT(HOUR FROM CAST({0} AS TIMESTAMP))"));
            RegisterFunction(SqlFunctionType.Day, new SqlFunctionTemplate("EXTRACT(DAY FROM CAST({0} AS TIMESTAMP))"));
            RegisterFunction(SqlFunctionType.Month, new SqlFunctionTemplate("EXTRACT(MONTH FROM CAST({0} AS TIMESTAMP))"));
            RegisterFunction(SqlFunctionType.Year, new SqlFunctionTemplate("EXTRACT(YEAR FROM CAST({0} AS TIMESTAMP))"));

            RegisterFunction(SqlFunctionType.ToString, new SqlFunctionTemplate("TO_CHAR({0})"));
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
        public override bool UseAsOnAlias
        {
            get { return false; }
        }

        /// <summary>
        /// Overriden. Adds the limit clause to the query.
        /// </summary>
        /// <param name="sql">The generated sql</param>
        /// <param name="offset">the value of the offset parameter</param>
        /// <param name="limit">the value of the limit parameter</param>
        public override void AddLimitString(StringBuilder sql, int offset, int? limit)
        {
            if (sql == null)
            {
                throw new ArgumentNullException("sql");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", offset, "Value cannot be lesser than 0");
            }

            if (limit != null && limit < 0)
            {
                throw new ArgumentOutOfRangeException("limit", limit, "Value cannot be lesser than 0");
            }

            if (limit != null)
            {
                sql.Append(" LIMIT ");
                sql.Append(limit);
            }

            if (offset != 0)
            {
                sql.Append(" OFFSET ");
                sql.Append(offset);
            }
        }

        public override string QuoteForAliasName(string aliasName)
        {
            return IsQuoted(aliasName) ? aliasName : Quote(aliasName.ToLower());
        }

        public override string QuoteForColumnName(string columnName)
        {
            return IsQuoted(columnName) ? columnName : Quote(columnName.ToUpper());
        }

        public override string QuoteForTableName(string tableName)
        {
            return IsQuoted(tableName) ? tableName : Quote(tableName.ToUpper());
        }

        /// <summary>
        /// Get DELETE statement with alias
        /// </summary>
        /// <param name="table_name">Table name</param>
        /// <param name="alias">Alias</param>
        /// <returns>DELETE [<alias>] FROM [<table_name>] [<alias>]</returns>
        public override string DeleteStatement(string table_name, string alias)
        {
            return string.Format("DELETE {0} FROM {1} {0}", alias, table_name);
        }

        /// <summary>
        /// Get UPDATE statement with alias
        /// </summary>
        /// <param name="table_name">Table name</param>
        /// <param name="alias">Alias</param>
        /// <param name="set_Clause">SET Clause</param>
        /// <returns>UPDATE [<table_name>] [<alias>] SET expression</returns>
        public override string UpdateStatement(string table_name, string alias, string set_Clause)
        {
            return string.Format("UPDATE {0} {1} {2}", table_name, alias, set_Clause);
        }

        /// <summary>
        /// For every type of database is required use the different format of datetime value (when used on CriteriaSet as where condition)
        /// TODO: Nullable DateTime ?
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>Returns the DateTime in valid format for specific type of database</returns>
        public override object GetValidDateTime(DateTime dateTime)
        {
            return string.Format("STR_TO_DATE('{0}', '%Y %m %d %H %i %S')", dateTime.ToString("yyyy MM dd hh mm ss"));
        }
    }
}
