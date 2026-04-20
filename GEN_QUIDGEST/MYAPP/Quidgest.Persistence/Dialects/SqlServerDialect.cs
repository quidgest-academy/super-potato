using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Quidgest.Persistence.GenericQuery;

namespace Quidgest.Persistence.Dialects
{
    /// <summary>
    /// Microsoft Sql Server dialect for the latest version
    /// </summary>
    public class SqlServerDialect : Dialect
    {
        /// <inheritdoc/>
        public override string NamedPrefix => "@";
        /// <inheritdoc/>
        public override bool UseNamedPrefixInSql => true;
        /// <inheritdoc/>
        public override bool UseNamedPrefixInParameter => true;
        /// <inheritdoc/>
        public override bool SupportsLimit => true;
        /// <inheritdoc/>
        public override bool SupportsLimitOffset => true;
        /// <inheritdoc/>
        public override bool BindLimitParametersFirst => false;
        /// <inheritdoc/>
        public override char OpenQuote => '[';
        /// <inheritdoc/>
        public override char CloseQuote => ']';

        /// <inheritdoc/>
        public SqlServerDialect()
        {
            RegisterType(DbType.AnsiStringFixedLength, "CHAR(255)");
            RegisterType(DbType.AnsiString, "VARCHAR(MAX)");
            RegisterType(DbType.Binary, "IMAGE");
            RegisterType(DbType.Boolean, "BIT");
            RegisterType(DbType.Byte, "TINYINT");
            RegisterType(DbType.Currency, "MONEY");
            RegisterType(DbType.Date, "DATE");
            RegisterType(DbType.DateTime, "DATETIME");
            RegisterType(DbType.Decimal, "DECIMAL(19,5)");
            RegisterType(DbType.DateTime2, "DATETIME2");
            RegisterType(DbType.DateTimeOffset, "DATETIMEOFFSET");
            RegisterType(DbType.Double, "DOUBLE");
            RegisterType(DbType.Guid, "UNIQUEIDENTIFIER");
            RegisterType(DbType.Int16, "SMALLINT");
            RegisterType(DbType.Int32, "INT");
            RegisterType(DbType.Int64, "BIGINT");
            RegisterType(DbType.Single, "REAL");
            RegisterType(DbType.StringFixedLength, "NCHAR(4000)");
            RegisterType(DbType.String, "NVARCHAR(MAX)");
            RegisterType(DbType.Time, "TIME");
            RegisterType(DbType.Xml, "XML");

            RegisterType(CustomDbType.StandardAnsiString, "VARCHAR(50)");
            RegisterType(CustomDbType.StandardDecimalSearch, "DECIMAL(38,10)");

            string[] typesArr = RegisteredTypesToArray();

            RegisterFunction(SqlFunctionType.Locate, new SqlFunctionTemplate("CHARINDEX({1}, {0}, {2})"));
            RegisterFunction(SqlFunctionType.Trim, new SqlFunctionTemplate("RTRIM(LTRIM({0}))"));
            RegisterFunction(SqlFunctionType.Length, new SqlFunctionTemplate("LEN({0})"));
            RegisterFunction(SqlFunctionType.BitLength, new SqlFunctionTemplate("(DATALENGTH({0})*8)"));
            RegisterFunction(SqlFunctionType.Module, new SqlFunctionTemplate("(({0}) % ({1}))"));
            RegisterFunction(SqlFunctionType.Extract, new SqlFunctionTemplate("DATEPART({0}, {1})", false, false, ",", new[] { "SECOND", "MINUTE", "HOUR", "DAY", "MONTH", "YEAR" }));
            RegisterFunction(SqlFunctionType.Concat, new SqlFunctionTemplate("({0})", true, false, "+"));
            RegisterFunction(SqlFunctionType.Round, new SqlFunctionTemplate("ROUND({0}, {1})"));

            RegisterFunction(SqlFunctionType.CurrentTimestamp, new SqlFunctionTemplate("CURRENT_TIMESTAMP"));
            RegisterFunction(SqlFunctionType.SystemDate, new SqlFunctionTemplate("GETDATE()"));

            RegisterFunction(SqlFunctionType.Second, new SqlFunctionTemplate("DATEPART(SECOND, {0})"));
            RegisterFunction(SqlFunctionType.Minute, new SqlFunctionTemplate("DATEPART(MINUTE, {0})"));
            RegisterFunction(SqlFunctionType.Hour, new SqlFunctionTemplate("DATEPART(HOUR, {0})"));
            RegisterFunction(SqlFunctionType.Day, new SqlFunctionTemplate("DATEPART(DAY, {0})"));
            RegisterFunction(SqlFunctionType.Month, new SqlFunctionTemplate("DATEPART(MONTH, {0})"));
            RegisterFunction(SqlFunctionType.Year, new SqlFunctionTemplate("DATEPART(YEAR, {0})"));
            RegisterFunction(SqlFunctionType.Week, new SqlFunctionTemplate("DATEPART(WEEK, {0})"));

            RegisterFunction(SqlFunctionType.Cast, new SqlFunctionTemplate("CAST({0} AS {1})", false, false, "AS", typesArr));
            RegisterFunction(SqlFunctionType.Custom, new SqlFunctionTemplate("dbo.{0}({1})", true, false, ",", typesArr));
            RegisterFunction(SqlFunctionType.SysCustom, new SqlFunctionTemplate("{0}({1})", true, false, ",", typesArr));
        }

        /// <inheritdoc/>
        public override void AddLimitString(StringBuilder sql, int offset, int? limit)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Value cannot be lesser than 0");
            if (limit != null && limit < 0)
                throw new ArgumentOutOfRangeException(nameof(limit), limit, "Value cannot be lesser than 0");

            if (offset == 0)
            {
                //first page is trivial top query
                string s = sql.ToString();
                int selectInsertPoint = s.StartsWith("SELECT DISTINCT") ? 15 : 6;
                sql.Insert(selectInsertPoint, " TOP " + limit + " ");
                return;
            }

            sql.Append(" OFFSET ");
            sql.Append(offset);
            sql.Append(" ROWS FETCH NEXT ");
            sql.Append(limit);
            sql.Append(" ROWS ONLY ");
        }

        /// <inheritdoc/>
        public override string QuoteForSchemaName(string schemaName)
        {
            if (string.IsNullOrEmpty(schemaName))
                return string.Empty;

            string[] parts = schemaName.Split('.');
            for (int i = 0; i < parts.Length; i++)
                if (!IsQuoted(parts[i]))
                    parts[i] = Quote(parts[i]);

            return string.Join(".", parts);
        }

        /// <inheritdoc/>
        public override string DeleteStatement(string table_name, string alias)
        {
            return string.Format("DELETE {0} FROM {1} AS {0}", alias, table_name);
        }

        /// <inheritdoc/>
        public override string UpdateStatement(string table_name, string alias, string set_Clause)
        {
            return string.Format("UPDATE {0} {1} FROM {2} AS {0}", alias, set_Clause, table_name);
        }

        /// <inheritdoc/>
        public override bool SupportsOutput => true;

        /// <inheritdoc/>
        public override void AddOutputString(StringBuilder sql, IList<ColumnReference> columns)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));
            if (columns == null)
                throw new ArgumentNullException(nameof(sql));
            if (columns.Count == 0)
                return;

            //find the insertion point for the output clause
            var str = sql.ToString();
            var ix = str.IndexOf(") VALUES (");
            if (ix == -1)
                return;

            //build up the output clause
            StringBuilder aux = new StringBuilder();
            aux.Append(" OUTPUT ");
            foreach (var col in columns)
            {
                aux.Append("inserted.");
                aux.Append(col.ColumnName);
                aux.Append(",");
            }
            aux.Length--;

            //modify the original query
            sql.Insert(ix+1, aux.ToString());
        }
    }
}
