using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a sql constant value. 
    /// When rendered it will use parameters to provide safety against SQL injection and allow SQL to performe caching.
    /// This should be prefered to SqlLiteral whenever possible
    /// </summary>
    public class SqlValue : ISqlExpression
    {
        /// <summary>
        /// The value to use in the sql
        /// </summary>
        /// <remarks>
        /// Author: CX 2011.06.28
        /// </remarks>
        public object Value
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the parameter to use when rendering the query
        /// </summary>
        public string ParamName
        {
            get; private set;
        }


        /// <summary>
        /// Create a new SqlValue. The parameter name will be infered.
        /// </summary>
        /// <param name="value">The value to use in the sql</param>
        /// <remarks>
        /// Author: CX 2011.06.28
        /// </remarks>
        public SqlValue(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Create a new sql value giving a name to the parameter
        /// </summary>
        /// <param name="paramName">The name to give to the parameter</param>
        /// <param name="value">The value to use in the sql</param>
        public SqlValue(object value, string paramName)
        {
            Value = value;
            ParamName = paramName;
        }

        public object Clone()
        {
            return new SqlValue(Value);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is SqlValue))
			{
				return false;
			}

			SqlValue other = (SqlValue)obj;

			return Object.Equals(this.Value, other.Value) && this.ParamName == other.ParamName;
		}

		public override int GetHashCode()
		{
			return (Value == null ? 0 : Value.GetHashCode() + ParamName.GetHashCode());
		}
    }
}
