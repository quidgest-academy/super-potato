using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a reference to a table in a sql query
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified: CX 2011.11.02
    /// Reviewed:
    /// -->
    /// </remarks>
    public class TableReference : ITableSource
    {
        /// <summary>
        /// The schema of the table
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.11.02
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string SchemaName
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the table
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string TableName
        {
            get;
            private set;
        }

        /// <summary>
        /// The alias of the table in the query
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string TableAlias
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <remarks>
        /// The alias is set to the table name
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableReference(string tableName)
        {
            TableName = tableName;
            TableAlias = tableName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableReference(string tableName, string tableAlias)
        {
            TableName = tableName;
            TableAlias = tableAlias;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.11.02
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableReference(string schemaName, string tableName, string tableAlias)
        {
            SchemaName = schemaName;
            TableName = tableName;
            TableAlias = tableAlias;
        }

        public object Clone()
        {
            return new TableReference(SchemaName, TableName, TableAlias);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is TableReference))
			{
				return false;
			}

			TableReference other = (TableReference)obj;

			return this.SchemaName == other.SchemaName
				&& this.TableAlias == other.TableAlias
				&& this.TableName == other.TableName;
		}

		public override int GetHashCode()
		{
			return (SchemaName == null ? 0 : SchemaName.GetHashCode())
				^ (TableAlias == null ? 0 : TableAlias.GetHashCode())
				^ (TableName == null ? 0 : TableName.GetHashCode());
		}
    }
}
