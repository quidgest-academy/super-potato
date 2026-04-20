using System;
using System.Text;
using Quidgest.Persistence.GenericQuery;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Quidgest.Persistence.Dialects
{
    /// <summary>
    /// Specificities of the DBMS Postgres
    /// </summary>
    public class PostgresDialect : Dialect
    {
        /// <inheritdoc/>
		public override string FunctionAsTableTemplate => "table({0})";
        /// <inheritdoc/>
        public override string NamedPrefix => "@";
        /// <inheritdoc/>
        public override char OpenQuote => '"';
        /// <inheritdoc/>
        public override char CloseQuote => '"';
        /// <inheritdoc/>
        public override bool UseNamedPrefixInSql => true;
        /// <inheritdoc/>
        public override bool UseNamedPrefixInParameter => true;
        /// <inheritdoc/>
        public override bool SupportsLimit => true;
        /// <inheritdoc/>
        public override bool SupportsLimitOffset => true;
        /// <inheritdoc/>
        public override bool SupportsOutput => true;
        /// <inheritdoc/>
        public override bool BindLimitParametersFirst => false;
        /// <inheritdoc/>
        public override bool SupportsLocking => false;
        /// <inheritdoc/>
        public override bool UseAsOnAlias => false;
        /// <inheritdoc/>
        public override bool UseSchemaOnTableName => false;

        /// <summary>
        /// Constructor. Initializes the supported functions.
        /// </summary>
        public PostgresDialect()
        {
            RegisterType(DbType.AnsiStringFixedLength, "char(2000)");
            RegisterType(DbType.AnsiString, "varchar");
            RegisterType(DbType.StringFixedLength, "char(2000)");
            RegisterType(DbType.String, "varchar");
			RegisterType(DbType.Boolean, "int2");
			RegisterType(DbType.Byte, "int2");
			RegisterType(DbType.Int16, "int2");
			RegisterType(DbType.Int32, "int4");
			RegisterType(DbType.Int64, "int8");
			RegisterType(DbType.UInt16, "int2");
			RegisterType(DbType.UInt32, "int4");
			RegisterType(DbType.UInt64, "int8");
			RegisterType(DbType.Currency, "numeric(20,2)");
			RegisterType(DbType.Single, "float4");
			RegisterType(DbType.Double, "float8");
			RegisterType(DbType.Decimal, "numeric(19,5)");
            RegisterType(DbType.Date, "timestamp");
            RegisterType(DbType.DateTime, "timestamp");
            RegisterType(DbType.Time, "time");
            RegisterType(DbType.Binary, "bytea");

            RegisterType(CustomDbType.StandardAnsiString, "varchar");
            RegisterType(CustomDbType.StandardDecimalSearch, "numeric(38,10)");

            string[] typesArr = RegisteredTypesToArray();

            RegisterFunction(SqlFunctionType.Locate, new SqlFunctionTemplate("position({1} in substr({0}, {2}))"));
            RegisterFunction(SqlFunctionType.Trim, new SqlFunctionTemplate("trim(both FROM {0})"));
            RegisterFunction(SqlFunctionType.Length, new SqlFunctionTemplate("char_length({0})"));
            RegisterFunction(SqlFunctionType.BitLength, new SqlFunctionTemplate("bit_length({0})"));
            RegisterFunction(SqlFunctionType.Module, new SqlFunctionTemplate("(({0}) % ({1}))"));
            RegisterFunction(SqlFunctionType.Extract, new SqlFunctionTemplate("extract({0}, {1})", false, false, ",", new[] { "second", "minute", "hour", "day", "month", "year" }));
            RegisterFunction(SqlFunctionType.Concat, new SqlFunctionTemplate("({0})", true, false, "+"));
            RegisterFunction(SqlFunctionType.Round, new SqlFunctionTemplate("round({0}, {1})"));

            RegisterFunction(SqlFunctionType.GroupConcat, new SqlFunctionTemplate("string_agg({0}, {1})")); //TODO: support for ORDER BY

            RegisterFunction(SqlFunctionType.CurrentTimestamp, new SqlFunctionTemplate("CURRENT_TIMESTAMP"));
            RegisterFunction(SqlFunctionType.SystemDate, new SqlFunctionTemplate("CURRENT_TIMESTAMP"));

            RegisterFunction(SqlFunctionType.Second, new SqlFunctionTemplate("extract(second FROM {0})"));
            RegisterFunction(SqlFunctionType.Minute, new SqlFunctionTemplate("extract(minute FROM {0})"));
            RegisterFunction(SqlFunctionType.Hour, new SqlFunctionTemplate("extract(hour FROM {0})"));
            RegisterFunction(SqlFunctionType.Day, new SqlFunctionTemplate("extract(day FROM {0})"));
            RegisterFunction(SqlFunctionType.Month, new SqlFunctionTemplate("extract(month FROM {0})"));
            RegisterFunction(SqlFunctionType.Year, new SqlFunctionTemplate("extract(year FROM {0})"));
            RegisterFunction(SqlFunctionType.Week, new SqlFunctionTemplate("extract(week FROM {0})"));

            RegisterFunction(SqlFunctionType.Cast, new SqlFunctionTemplate("cast({0} AS {1})", false, false, "AS", typesArr));
            RegisterFunction(SqlFunctionType.Custom, new SqlFunctionTemplate("{0}({1})", true, false, ",", typesArr));
            RegisterFunction(SqlFunctionType.SysCustom, new SqlFunctionTemplate("{0}({1})", true, false, ",", typesArr));
        }



        /// <inheritdoc/>
        public override void AddLimitString(StringBuilder sql, int offset, int? limit)
        {
            if (sql == null)
                throw new ArgumentNullException("sql");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", offset, "Value cannot be lesser than 0");
            if (limit != null && limit < 0)
                throw new ArgumentOutOfRangeException("limit", limit, "Value cannot be lesser than 0");

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

        /// <inheritdoc/>
        public override void AddOutputString(StringBuilder sql, IList<ColumnReference> columns)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));
            if (columns == null)
                throw new ArgumentNullException(nameof(sql));
            if (columns.Count == 0)
                return;

            sql.Append(" RETURNING ");
            sql.Append(string.Join(",", columns.Select(x => QuoteForColumnName(x.ColumnName))));
        }

        /// <inheritdoc/>
        public override string QuoteForAliasName(string aliasName)
        {
            return IsQuoted(aliasName) ? aliasName : Quote(aliasName.ToLower());
        }

        /// <inheritdoc/>
        public override string QuoteForColumnName(string columnName)
        {
            return IsQuoted(columnName) ? columnName : Quote(columnName.ToLower());
        }

        /// <inheritdoc/>
        public override string QuoteForTableName(string tableName)
        {
            return IsQuoted(tableName) ? tableName : Quote(tableName.ToLower());
        }

        /// <inheritdoc/>
        public override string DeleteStatement(string table_name, string alias)
        {
            return string.Format("DELETE FROM {0} AS {1}", table_name, alias);
        }

        /// <inheritdoc/>
        public override string UpdateStatement(string table_name, string alias, string set_Clause)
        {
            return string.Format("UPDATE {0} AS {1} {2}", table_name, alias, set_Clause);
        }

        /// <inheritdoc/>
        public override object GetValidDateTime(DateTime dateTime)
        {
            return dateTime.ToString("'yyyy-MM-ddTHH:mm:ss.fff'");
        }
    }
}
