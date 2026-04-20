using System;
using System.Collections.Generic;
using System.Text;
using Quidgest.Persistence;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents an update query
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified: CX 2011.11.02
    /// Reviewed:
    /// -->
    /// </remarks>
    public class UpdateQuery : ICloneable
    {
        /// <summary>
        /// The table thet will be updated
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableReference UpdateTable
        {
            get;
            set;
        }

        /// <summary>
        /// The list of values that will be updated
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<ColumnAttribution> SetValues
        {
            get;
            private set;
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
        public UpdateQuery()
        {
            SetValues = new List<ColumnAttribution>();
            Joins = new List<TableJoin>();
        }

        /// <summary>
        /// Specifies the table where the data will be updated
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
        public UpdateQuery Update(string tableName)
        {
            UpdateTable = new TableReference(tableName);

            return this;
        }

        /// <summary>
        /// Specifies the table where the data will be updated
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
        public UpdateQuery Update(string schemaName, string tableName)
        {
            UpdateTable = new TableReference(schemaName, tableName, tableName);

            return this;
        }

        /// <summary>
        /// Specifies the table where the data will be updated
        /// </summary>
        /// <param name="area">The area we want to update</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Update(AreaRef area)
        {
            UpdateTable = new TableReference(area.Schema, area.Table, area.Alias);

            return this;
        }

        /// <summary>
        /// Adds a column and its new value to the query
        /// </summary>
        /// <param name="column">The column that will be updated</param>
        /// <param name="value">The value that will be stored</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Set(string column, object value)
        {
            SetValues.Add(new ColumnAttribution(UpdateTable.TableAlias, column, value));

            return this;
        }

        /// <summary>
        /// Adds a column and its new value to the query
        /// </summary>
        /// <param name="field">The field that will be updated</param>
        /// <param name="value">The value that will be stored</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Set(FieldRef field, object value)
        {
            SetValues.Add(new ColumnAttribution(field.Area, field.Field, value));

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
        public UpdateQuery Where(CriteriaSet criteria)
        {
            WhereCondition = criteria;

            return this;
        }


        /// <summary>
        /// The list of the tables to be joinned in this query an their join conditions
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<TableJoin> Joins
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(string tableName)
        {
            return Join(tableName, tableName, TableJoinType.Inner);
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(string tableName, string tableAlias)
        {
            return Join(tableName, tableAlias, TableJoinType.Inner);
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.10.03
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(string schemaName, string tableName, string tableAlias)
        {
            return Join(schemaName, tableName, tableAlias, TableJoinType.Inner);
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(string tableName, TableJoinType type)
        {
            return Join(tableName, tableName, type);
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(string tableName, string tableAlias, TableJoinType type)
        {
            Joins.Add(new TableJoin(new TableReference(tableName, tableAlias), type));

            return this;
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="tableAlias">The alias for the table</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.10.03
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(string schemaName, string tableName, string tableAlias, TableJoinType type)
        {
            Joins.Add(new TableJoin(new TableReference(schemaName, tableName, tableAlias), type));

            return this;
        }

        /// <summary>
        /// Adds a join of a select query
        /// </summary>
        /// <param name="query">The query to join with</param>
        /// <param name="alias">The alias for the query</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(SelectQuery query, string alias)
        {
            return Join(query, alias, TableJoinType.Inner);
        }

        /// <summary>
        /// Adds a join of a select query
        /// </summary>
        /// <param name="query">The query to join with</param>
        /// <param name="alias">The alias for the query</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified: CX 2011.07.14
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(SelectQuery query, string alias, TableJoinType type)
        {
            Joins.Add(new TableJoin(new QueryReference(query, alias), type));

            return this;
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="area">The area we want the data from</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(AreaRef area)
        {
            return Join(area.Schema, area.Table, area.Alias, TableJoinType.Inner);
        }

        /// <summary>
        /// Adds a join of a table
        /// </summary>
        /// <param name="area">The area we want the data from</param>
        /// <param name="type">The type of the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(AreaRef area, TableJoinType type)
        {
            return Join(area.Schema, area.Table, area.Alias, type);
        }

        /// <summary>
        /// Adds a collectin of joins to the query
        /// </summary>
        /// <param name="joins">The joins to add</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.09.19
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery Join(IEnumerable<TableJoin> joins)
        {
            if (joins != null)
            {
                foreach (TableJoin join in joins)
                {
                    this.Joins.Add(join);
                }
            }

            return this;
        }

        /// <summary>
        /// Specifies the on clause for the last added join
        /// </summary>
        /// <param name="conditions">The set of conditions for the join</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public UpdateQuery On(CriteriaSet conditions)
        {
            if (Joins.Count == 0)
            {
                throw new Exception("There is no JOIN specified in the query");
            }

            if (Joins[Joins.Count - 1].OnCondition != null)
            {
                throw new Exception("Current JOIN already has it's ON clause specified");
            }

            Joins[Joins.Count - 1].OnCondition = conditions;

            return this;
        }


        public object Clone()
        {
            UpdateQuery result = new UpdateQuery();

            if (UpdateTable != null)
            {
                result.UpdateTable = (TableReference)UpdateTable.Clone();
            }
            foreach (ColumnAttribution set in SetValues)
            {
                result.SetValues.Add((ColumnAttribution)set.Clone());
            }
            foreach (TableJoin join in Joins)
            {
                result.Joins.Add((TableJoin)join.Clone());
            }
            if (WhereCondition != null)
            {
                result.WhereCondition = (CriteriaSet)WhereCondition.Clone();
            }

            return result;
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is UpdateQuery))
			{
				return false;
			}

			UpdateQuery other = (UpdateQuery)obj;

			bool isEqual = Object.Equals(this.UpdateTable, other.UpdateTable)
				&& Object.Equals(this.WhereCondition, other.WhereCondition)
				&& this.SetValues.Count == other.SetValues.Count;

			for (int i = 0; i < this.SetValues.Count && isEqual; i++)
			{
				isEqual &= Object.Equals(this.SetValues[i], other.SetValues[i]);
			}

            for (int i = 0; i < this.Joins.Count && isEqual; i++)
            {
                isEqual &= Object.Equals(this.Joins[i], other.Joins[i]);
            }

			return isEqual;
		}

		public override int GetHashCode()
		{
			int hash = (UpdateTable == null ? 0 : UpdateTable.GetHashCode())
				^ (WhereCondition == null ? 0 : WhereCondition.GetHashCode());

			for (int i = 0; i < SetValues.Count; i++)
			{
				hash ^= (SetValues[i] == null ? 0 : SetValues[i].GetHashCode());
			}

            for (int i = 0; i < this.Joins.Count; i++)
            {
                hash ^= this.Joins[i] == null ? 0 : this.Joins[i].GetHashCode();
            }

			return hash;
		}
    }
}
