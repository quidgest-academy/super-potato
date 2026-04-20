using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a delete query
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified: CX 2011.11.02
    /// Reviewed:
    /// -->
    /// </remarks>
    public class DeleteQuery : ICloneable
    {
        /// <summary>
        /// The table where the data will be deleted from
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableReference DeleteTable
        {
            get;
            set;
        }

        /// <summary>
        /// The where condition
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public CriteriaSet WhereCondition
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="persistentSupport">The object with the persistent support information</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public DeleteQuery()
        {
        }

        /// <summary>
        /// Specifies the table where the data will be deleted from
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public DeleteQuery Delete(string tableName)
        {
            DeleteTable = new TableReference(tableName);

            return this;
        }

        /// <summary>
        /// Specifies the table where the data will be deleted from
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        /// <param name="tableName">The name of the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.11.02
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public DeleteQuery Delete(string schemaName, string tableName)
        {
            DeleteTable = new TableReference(schemaName, tableName, tableName);

            return this;
        }

        /// <summary>
        /// Specifies the table where the data will be deleted from
        /// </summary>
        /// <param name="area">The area we want to delete data from</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.067.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public DeleteQuery Delete(AreaRef area)
        {
			DeleteTable = new TableReference(area.Schema, area.Table, area.Alias);

            return this;
        }

        /// <summary>
        /// Specifies the where clause for the query
        /// </summary>
        /// <param name="conditions">The set of conditions for the query</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public DeleteQuery Where(CriteriaSet criteria)
        {
            WhereCondition = criteria;

            return this;
        }

        public object Clone()
        {
            DeleteQuery result = new DeleteQuery();

            if (DeleteTable != null)
            {
                result.DeleteTable = (TableReference)DeleteTable.Clone();
            }
            if (WhereCondition != null)
            {
                result.WhereCondition = (CriteriaSet)WhereCondition.Clone();
            }

            return result;
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is DeleteQuery))
			{
				return false;
			}

			DeleteQuery other = (DeleteQuery)obj;

			return Object.Equals(this.DeleteTable, other.DeleteTable)
				&& Object.Equals(this.WhereCondition, other.WhereCondition);
		}

		public override int GetHashCode()
		{
			return (DeleteTable == null ? 0 : DeleteTable.GetHashCode())
				^ (WhereCondition == null ? 0 : WhereCondition.GetHashCode());
		}
    }
}
