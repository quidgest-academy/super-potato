using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents the attribution of a value to a column
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class ColumnAttribution : ICloneable
    {
        /// <summary>
        /// The column that will store the value
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public ColumnReference Column
        {
            get;
            set;
        }

        /// <summary>
        /// The value to store in the column
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableAlias">The name of the table</param>
        /// <param name="columnName">The column that will store the value</param>
        /// <param name="value">The value to be stored</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public ColumnAttribution(string tableAlias, string columnName, object value)
        {
            //RS (2019.02.05) Removed table qualifier from the destination part of the attribution
            // I did not find a use case where the qualification is necessary and field names are never ambiguous in a SET attribution.
            //Column = new ColumnReference(tableAlias, columnName);
            Column = new ColumnReference(null, columnName);
            Value = value;
        }

        public object Clone()
        {
            return new ColumnAttribution(
                Column.TableAlias,
                Column.ColumnName,
                Value != null && Value is ICloneable ? ((ICloneable)Value).Clone() : Value);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is ColumnAttribution))
			{
				return false;
			}

			ColumnAttribution other = (ColumnAttribution)obj;

			return Object.Equals(this.Column, other.Column)
				&& Object.Equals(this.Value, other.Value);
		}

		public override int GetHashCode()
		{
			return (Column == null ? 0 : Column.GetHashCode())
				^ (Value == null ? 0 : Value.GetHashCode());
		}
    }
}
