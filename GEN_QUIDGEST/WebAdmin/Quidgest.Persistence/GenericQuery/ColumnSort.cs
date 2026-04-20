using System;

namespace Quidgest.Persistence.GenericQuery
{
    public class ColumnSort : ICloneable
    {
        public int? ColumnIndex
        {
            get;
            private set;
        }

        public ISqlExpression Expression
        {
            get;
            private set;
        }

        public SortOrder Order
        {
            get;
            private set;
        }

        public ColumnSort(int columnIndex, SortOrder order)
        {
            if (columnIndex < 1)
            {
                throw new ArgumentOutOfRangeException("columnIndex", columnIndex, "Value must be greater than or equal to 1");
            }

            ColumnIndex = columnIndex;
            Expression = null;
            Order = order;
        }

        public ColumnSort(ISqlExpression expression, SortOrder order)
        {
            ColumnIndex = null;
            Expression = expression;
            Order = order;
        }

        public object Clone()
        {
            if (ColumnIndex == null)
            {
                return new ColumnSort(
                    Expression == null ? null : (ISqlExpression)Expression.Clone(),
                    Order);
            }
            else
            {
                return new ColumnSort(
                    ColumnIndex.Value,
                    Order);
            }
        }

		public override bool Equals(object obj)
		{
			if (obj == null && !(obj is ColumnSort))
			{
				return false;
			}

			ColumnSort other = (ColumnSort)obj;

			return Object.Equals(this.ColumnIndex, other.ColumnIndex)
				&& Object.Equals(this.Expression, other.Expression)
				&& this.Order == other.Order;
		}

		public override int GetHashCode()
		{
			return (this.ColumnIndex == null ? 0 : this.ColumnIndex.GetHashCode())
				^ (this.Expression == null ? 0 : this.Expression.GetHashCode())
				^ this.Order.GetHashCode();
		}
    }
}
