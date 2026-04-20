using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a select field
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class SelectField : ISqlExpression
    {
        /// <summary>
        /// The field expression
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public ISqlExpression Expression
        {
            get;
            set;
        }

        /// <summary>
        /// The field alias
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string Alias
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="expression">The expression that defines the field value</param>
        /// <param name="alias">The field alias</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectField(ISqlExpression expression, string alias)
        {
            Expression = expression;
            Alias = alias;
        }

        public object Clone()
        {
            return new SelectField(
                Expression == null ? null : (ISqlExpression)Expression.Clone(),
                Alias);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is SelectField))
			{
				return false;
			}

			SelectField other = (SelectField)obj;

			return this.Alias == other.Alias
				&& Object.Equals(this.Expression, other.Expression);
		}

		public override int GetHashCode()
		{
			return (Alias == null ? 0 : Alias.GetHashCode())
				^ (Expression == null ? 0 : this.Expression.GetHashCode());
		}
    }
}
