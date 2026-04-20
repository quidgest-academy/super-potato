using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a function to be used in a query and it's arguments
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class SqlFunction : ISqlExpression
    {
        /// <summary>
        /// The function type
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlFunctionType Function
        {
            get;
            private set;
        }

        /// <summary>
        /// The argument list
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public object[] Arguments
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="function">The function type</param>
        /// <param name="arguments">The argument list</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlFunction(SqlFunctionType function, params object[] arguments)
        {
            Function = function;
            Arguments = arguments == null ? new object[] { } : arguments;
        }

        public object Clone()
        {
            object[] newArgs = new object[Arguments.Length];
            for (int i = 0; i < Arguments.Length; i++)
            {
                if (Arguments[i] != null && Arguments[i] is ICloneable)
                {
                    newArgs[i] = ((ICloneable)Arguments[i]).Clone();
                }
                else
                {
                    newArgs[i] = Arguments[i];
                }
            }

            return new SqlFunction(Function, newArgs);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is SqlFunction))
			{
				return false;
			}

			SqlFunction other = (SqlFunction)obj;

			bool isEqual = this.Function == other.Function
				&& this.Arguments.Length == other.Arguments.Length;
			for (int i = 0; i < this.Arguments.Length && isEqual; i++)
			{
				isEqual &= Object.Equals(this.Arguments[i], other.Arguments[i]);
			}

			return isEqual;
		}

		public override int GetHashCode()
		{
			int hash = this.Function.GetHashCode();
			for (int i = 0; i < this.Arguments.Length; i++)
			{
				hash ^= this.Arguments[i].GetHashCode();
			}

			return hash;
		}
    }
}
