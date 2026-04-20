using System;
using System.Collections.Generic;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a join of a table to a query
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class TableJoin : ICloneable
    {
        /// <summary>
        /// The table that will be joined
        /// </summary>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        public ITableSource Table
        {
            get;
            private set;
        }

        /// <summary>
        /// The type of the join
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableJoinType JoinType
        {
            get;
            private set;
        }

        /// <summary>
        /// The set of condition that link the table to the query
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public CriteriaSet OnCondition
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table">The table to join</param>
        /// <remarks>
        /// The join will be considered as an inner join
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableJoin(ITableSource table)
        {
            Table = table;
            JoinType = TableJoinType.Inner;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table">The table to join</param>
        /// <param name="type">The type of the join</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableJoin(ITableSource table, TableJoinType type)
        {
            Table = table;
            JoinType = type;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table">The table to join</param>
        /// <param name="onCondition">The set of condition that link the table to the query</param>
        /// <remarks>
        /// The join will be considered as an inner join
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableJoin(ITableSource table, CriteriaSet onCondition)
        {
            Table = table;
            OnCondition = onCondition;
            JoinType = TableJoinType.Inner;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table">The table to join</param>
        /// <param name="onCondition">The set of condition that link the table to the query</param>
        /// <param name="type">The type of the join</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableJoin(ITableSource table, CriteriaSet onCondition, TableJoinType type)
        {
            Table = table;
            OnCondition = onCondition;
            JoinType = type;
        }

        /// <summary>
        /// Checks if the join conditions can be satisfied by the specified tables
        /// </summary>
        /// <param name="availableTables">The tables to check if they satisfy the join conditions</param>
        /// <returns>True if the specified tables satisfy the join conditions, otherwise false</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public bool CanSatisfyReferences(IEnumerable<ITableSource> availableTables)
        {
            IList<ITableSource> newAvailableTables = new List<ITableSource>(availableTables);
            newAvailableTables.Add(Table);
            return OnCondition.CanSatisfyReferences(newAvailableTables);
        }

        public object Clone()
        {
            var clone = new TableJoin(
                Table == null ? null : (ITableSource)Table.Clone(),
                JoinType);

            if (OnCondition != null)
            {
                clone.OnCondition = (CriteriaSet)OnCondition.Clone();
            }

            return clone;
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is TableJoin))
			{
				return false;
			}

			TableJoin other = (TableJoin)obj;

			return this.JoinType == other.JoinType
				&& Object.Equals(this.OnCondition, other.OnCondition)
				&& Object.Equals(this.Table, other.Table);
		}

		public override int GetHashCode()
		{
			return JoinType.GetHashCode()
				^ (OnCondition == null ? 0 : OnCondition.GetHashCode())
				^ (Table == null ? 0 : Table.GetHashCode());
		}
    }
}
