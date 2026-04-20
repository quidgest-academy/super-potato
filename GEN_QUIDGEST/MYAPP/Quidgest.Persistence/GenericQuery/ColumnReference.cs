using System;

namespace Quidgest.Persistence.GenericQuery
{
    public class ColumnReference : ISqlExpression
    {
        public string TableAlias
        {
            get;
            set;
        }

        public string ColumnName
        {
            get;
            set;
        }

        public ColumnReference(string tableAlias, string columnName)
        {
            TableAlias = tableAlias;
            ColumnName = columnName;
        }

        public ColumnReference(FieldRef field)
            : this(field.Area, field.Field)
        {
        }

        public object Clone()
        {
            return new ColumnReference(TableAlias, ColumnName);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is ColumnReference))
			{
				return false;
			}

			ColumnReference other = (ColumnReference)obj;
			return this.TableAlias == other.TableAlias
				&& this.ColumnName == other.ColumnName;
		}

		public override int GetHashCode()
		{
			return (this.TableAlias == null ? 0 : this.TableAlias.GetHashCode())
				^ (this.ColumnName == null ? 0 : this.ColumnName.GetHashCode());
		}
    }
}
