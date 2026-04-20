using System;
using System.Collections.Generic;
using System.Text;
using Quidgest.Persistence.GenericQuery;
using System.Data;

namespace Quidgest.Persistence.Dialects
{
    /// <summary>
    /// Specificities of the DBMS Oracle 10g
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class Oracle10gDialect : Dialect
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
                return ":";
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
        public Oracle10gDialect()
        {
            RegisterType(DbType.AnsiStringFixedLength, "CHAR(2000)");
            RegisterType(DbType.AnsiString, "VARCHAR2(4000)");
            RegisterType(DbType.StringFixedLength, "NCHAR(2000)");
            RegisterType(DbType.String, "NVARCHAR2(4000)");
			RegisterType(DbType.Boolean, "NUMBER(1,0)");
			RegisterType(DbType.Byte, "NUMBER(3,0)");
			RegisterType(DbType.Int16, "NUMBER(5,0)");
			RegisterType(DbType.Int32, "NUMBER(10,0)");
			RegisterType(DbType.Int64, "NUMBER(20,0)");
			RegisterType(DbType.UInt16, "NUMBER(5,0)");
			RegisterType(DbType.UInt32, "NUMBER(10,0)");
			RegisterType(DbType.UInt64, "NUMBER(20,0)");
			RegisterType(DbType.Currency, "NUMBER(20,2)");
			RegisterType(DbType.Single, "FLOAT(24)");
			RegisterType(DbType.Double, "DOUBLE");
			RegisterType(DbType.Decimal, "NUMBER(19,5)");
            RegisterType(DbType.Date, "DATE");
            RegisterType(DbType.DateTime, "TIMESTAMP(4)");
            RegisterType(DbType.Time, "TIMESTAMP(4)");
            RegisterType(DbType.Binary, "BLOB");

            RegisterType(CustomDbType.StandardAnsiString, "VARCHAR2(50)");
            RegisterType(CustomDbType.StandardDecimalSearch, "NUMBER(38,10)");

            string[] typesArr = RegisteredTypesToArray();
            
            RegisterFunction(SqlFunctionType.GroupConcat, new SqlFunctionTemplate("LISTAGG({0}, {1})"));
            RegisterFunction(SqlFunctionType.Substring, new SqlFunctionTemplate("SUBSTR({0}, {1}, {2})"));
            RegisterFunction(SqlFunctionType.Locate, new SqlFunctionTemplate("INSTR({0}, {1}, {2})"));
            RegisterFunction(SqlFunctionType.BitLength, new SqlFunctionTemplate("(VSIZE({0})*8)"));
            RegisterFunction(SqlFunctionType.Concat, new SqlFunctionTemplate("({0})", true, false, "||"));

            RegisterFunction(SqlFunctionType.CurrentTimestamp, new SqlFunctionTemplate("CURRENT_TIMESTAMP"));
            RegisterFunction(SqlFunctionType.SystemDate, new SqlFunctionTemplate("SYSDATE"));

            RegisterFunction(SqlFunctionType.Round, new SqlFunctionTemplate("ROUND({0},{1})"));

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
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
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

            string s = sql.ToString();

            int? max = null;
            if (limit != null)
            {
                max = UseMaxForLimit ? limit.Value : offset + limit.Value;
            }

            int selectInsertPoint = 6;

            // find the alias of the select fields
            IList<string> alias = new List<string>();
            int parentisisCount = 0;
            int lastSpace = selectInsertPoint;
            string lastWord = null;
            for (int i = selectInsertPoint + 1; i < s.Length; i++)
			{
                if (s[i] == ' ')
                {
                    lastWord = s.Substring(lastSpace + 1, i - lastSpace - 1);
                    lastSpace = i;
                }
                else if (s[i] == '(')
                {
                    parentisisCount++;
                }
                else if (s[i] == ')')
                {
                    parentisisCount--;
                }
                else if (parentisisCount == 0)
                {
                    if (s[i] == ',')
                    {
                        if (i > 0 && s[i - 1] != ' ')
                        {
                            lastWord = s.Substring(lastSpace + 1, i - lastSpace - 1);
                        }
                        lastSpace = i;
                        alias.Add(lastWord);
                    }
                    else if ((s[i] == 'F' || s[i] == 'f')
                        && s.IndexOf("FROM ", i, StringComparison.InvariantCultureIgnoreCase) == i)
                    {
                        alias.Add(lastWord);
                        break;
                    }
                }
			}

            // add the outter queries to satisfy the limit
            string[] aliasArr = new string[alias.Count];
            alias.CopyTo(aliasArr, 0);
            if (offset > 0)
            {
                sql.Insert(0, "SELECT " + String.Join(",", aliasArr) + " FROM (SELECT * FROM ( SELECT row_.*, ROWNUM rownum_ FROM ( ");
                sql.Append(" ) row_ ) " + (max != null ? "WHERE rownum_ <= " + max : "") + ") WHERE rownum_ > " + offset);
            }
            else
            {
                sql.Insert(0, "SELECT " + String.Join(",", aliasArr) + " FROM ( ");
                sql.Append(" ) WHERE ROWNUM <= " + max);
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
        /// <returns>DELETE FROM [<table_name>] [<alias>]</returns>
        public override string DeleteStatement(string table_name, string alias)
        {
            return string.Format("DELETE FROM {0} {1}", table_name, alias);
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
    }
}
