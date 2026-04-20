using System;
using System.Collections.Generic;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents an insert query
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified: CX 2011.11.02
    /// Reviewed:
    /// -->
    /// </remarks>
    public class InsertQuery : ICloneable
    {
        /// <summary>
        /// The table where the data will be inserted
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public TableReference IntoTable
        {
            get;
            set;
        }

        /// <summary>
        /// The list of values that will be inserted
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public IList<ColumnAttribution> Values
        {
            get;
            private set;
        }

        /// <summary>
        /// List of output clauses
        /// </summary>
        /// <remarks>
        /// optional
        /// </remarks>
        public IList<ColumnReference> Outputs
        {
            get;
            private set;
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
        public InsertQuery()
        {
            Values = new List<ColumnAttribution>();
            Outputs = new List<ColumnReference>();
        }

        /// <summary>
        /// Specifies the table where the data will be inserted
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
        public InsertQuery Into(string tableName)
        {
            IntoTable = new TableReference(tableName);

            return this;
        }

        /// <summary>
        /// Specifies the table where the data will be inserted
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
        public InsertQuery Into(string schemaName, string tableName)
        {
            IntoTable = new TableReference(schemaName, tableName, tableName);

            return this;
        }

        /// <summary>
        /// Specifies the table where the data will be inserted
        /// </summary>
        /// <param name="area">The area where the data will be inserted</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public InsertQuery Into(AreaRef area)
        {
            IntoTable = new TableReference(area.Schema, area.Table, area.Table);

            return this;
        }

        /// <summary>
        /// Adds a column value to the query
        /// </summary>
        /// <param name="column">The column where the value will be inserted</param>
        /// <param name="value">The value that will be inserted</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public InsertQuery Value(string column, object value)
        {
            Values.Add(new ColumnAttribution(IntoTable.TableAlias, column, value));

            return this;
        }

        /// <summary>
        /// Adds a column value to the query
        /// </summary>
        /// <param name="field">The field where the value will be inserted</param>
        /// <param name="value">The value that will be inserted</param>
        /// <returns>The query</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public InsertQuery Value(FieldRef field, object value)
        {
            Values.Add(new ColumnAttribution(field.Area, field.Field, value));

            return this;
        }

        /// <summary>
        /// Add a column to the ouput clause of the query
        /// </summary>
        /// <param name="column">The column to output</param>
        /// <returns>The query</returns>
        public InsertQuery Output(string column)
        {
            Outputs.Add(new ColumnReference("", column));
            return this;
        }

        /// <summary>
        /// Add a column to the ouput clause of the query
        /// </summary>
        /// <param name="field">The field to output</param>
        /// <returns>The query</returns>
        public InsertQuery Output(FieldRef field)
        {
            Outputs.Add(new ColumnReference(field));
            return this;
        }

        public object Clone()
        {
            InsertQuery result = new InsertQuery();

            if (IntoTable != null)
            {
                result.IntoTable = (TableReference)IntoTable.Clone();
            }
            foreach (ColumnAttribution value in Values)
            {
                result.Values.Add((ColumnAttribution)value.Clone());
            }
            foreach (ColumnReference value in Outputs)
            {
                result.Outputs.Add((ColumnReference)value.Clone());
            }
            return result;
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is InsertQuery))
			{
				return false;
			}

			InsertQuery other = (InsertQuery)obj;

			bool isEqual = Object.Equals(this.IntoTable, other.IntoTable)
				&& this.Values.Count == other.Values.Count;

			for (int i = 0; i < this.Values.Count && isEqual; i++)
			{
				isEqual &= Object.Equals(this.Values[i], other.Values[i]);
			}

			return isEqual;
		}

		public override int GetHashCode()
		{
			int hash = (IntoTable == null ? 0 : IntoTable.GetHashCode());

			for (int i = 0; i < Values.Count; i++)
			{
				hash ^= (Values[i] == null ? 0 : Values[i].GetHashCode());
			}

			return hash;
		}
    }
}
