using System;
using System.Collections.Generic;
using System.Text;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a literal value. It must be used only for literal, otherwise it will work like 'SqlValue'(Creating parameters).
    /// This should only be used for edge cases not supported by SqlValue
    /// </summary>
    public class SqlLiteral : ISqlExpression
    {
        /// <summary>
        /// The value to use in the sql
        /// </summary>
        public object Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public SqlLiteral(object value)
        {
            Value = value;
        }

        public object Clone()
        {
            return new SqlLiteral(Value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SqlLiteral))
            {
                return false;
            }

            SqlLiteral other = (SqlLiteral)obj;

            return Object.Equals(this.Value, other.Value);
        }

        public override int GetHashCode()
        {
            return (Value == null ? 0 : Value.GetHashCode());
        }
    }
}
