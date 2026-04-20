using System;
using System.Data;
using System.Text;
using Quidgest.Persistence.GenericQuery;

namespace Quidgest.Persistence.Dialects
{
    /// <summary>
    /// Specificities of the DBMS SQLite
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.08.15
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class SqliteDialect : Dialect
    {
        /// <summary>
        /// Overriden. The prefix for the sql variables for this DBMS.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.08.15
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
        /// True if the DBMS supports the WITH( NOLOCK ) statement
        /// </summary>
        public override bool SupportsLocking
        {
            get { return false; }
        }

        /// <summary>
        /// Overriden. True if the DBMS supports a max number of results specified in the query.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.08.15
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
        /// Author: CX 2011.08.15
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
        /// Author: CX 2011.08.15
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
        ///  Overriden.  True if table names should be preceeded by the schema name
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: RR 2012.03.19
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public override bool UseSchemaOnTableName
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Constructor. Initializes the supported functions.
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.08.15
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqliteDialect()
        {
            RegisterType(DbType.Binary, "BLOB");
            RegisterType(DbType.Byte, "INTEGER");
            RegisterType(DbType.Int16, "INTEGER");
            RegisterType(DbType.Int32, "INTEGER");
            RegisterType(DbType.Int64, "INTEGER");
            RegisterType(DbType.SByte, "INTEGER");
            RegisterType(DbType.UInt16, "INTEGER");
            RegisterType(DbType.UInt32, "INTEGER");
            RegisterType(DbType.UInt64, "INTEGER");
            RegisterType(DbType.Currency, "NUMERIC");
            RegisterType(DbType.Decimal, "NUMERIC");
            RegisterType(DbType.Double, "NUMERIC");
            RegisterType(DbType.Single, "NUMERIC");
            RegisterType(DbType.VarNumeric, "NUMERIC");
            RegisterType(DbType.AnsiString, "TEXT");
            RegisterType(DbType.String, "TEXT");
            RegisterType(DbType.AnsiStringFixedLength, "TEXT");
            RegisterType(DbType.StringFixedLength, "TEXT");
            RegisterType(DbType.Date, "DATETIME");
            RegisterType(DbType.DateTime, "DATETIME");
            RegisterType(DbType.Time, "DATETIME");
            RegisterType(DbType.Boolean, "INTEGER");
            RegisterType(DbType.Guid, "UNIQUEIDENTIFIER");

            string[] typesArr = RegisteredTypesToArray();
            
            RegisterFunction(SqlFunctionType.GroupConcat, new SqlFunctionTemplate("GROUP_CONCAT({0}, {1})"));
            RegisterFunction(SqlFunctionType.Locate, new SqlFunctionTemplate("CHARINDEX({1}, {0}, {2})"));
            RegisterFunction(SqlFunctionType.Cast, new SqlFunctionTemplate("CAST({0} AS {1})", false, false, "AS", typesArr));
            RegisterFunction(SqlFunctionType.Substring, new SqlFunctionTemplate("SUBSTR({0}, {1}, {2})"));

            RegisterFunction(SqlFunctionType.Second, new SqlFunctionTemplate("STRFTIME(\"%S\", {0})"));
            RegisterFunction(SqlFunctionType.Minute, new SqlFunctionTemplate("STRFTIME(\"%M\", {0})"));
            RegisterFunction(SqlFunctionType.Hour, new SqlFunctionTemplate("STRFTIME(\"%H\", {0})"));
            RegisterFunction(SqlFunctionType.Day, new SqlFunctionTemplate("STRFTIME(\"%d\", {0})"));
            RegisterFunction(SqlFunctionType.Month, new SqlFunctionTemplate("STRFTIME(\"%m\", {0})"));
            RegisterFunction(SqlFunctionType.Year, new SqlFunctionTemplate("STRFTIME(\"%Y\", {0})"));

            RegisterFunction(SqlFunctionType.Round, new SqlFunctionTemplate("round({0},{1})"));

            RegisterFunction(SqlFunctionType.Custom, new SqlFunctionTemplate("{0}({1})", true));
        }

        /// <summary>
        /// Overriden. Adds the limit clause to the query.
        /// </summary>
        /// <param name="sql">The generated sql</param>
        /// <param name="offset">the value of the offset parameter</param>
        /// <param name="limit">the value of the limit parameter</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.08.15
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
    }
}
