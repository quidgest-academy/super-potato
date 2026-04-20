using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a reference to a function in a sql query
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2012.03.29
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class FunctionReference : ITableSource
    {
        /// <summary>
        /// The referenced function
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2012.03.29
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlFunction Function
        {
            get;
            private set;
        }

        /// <summary>
        /// The alias of the function in the query
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2012.03.29
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string TableAlias
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="function">The function</param>
		/// <param name="alias">The function alias</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2012.03.29
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public FunctionReference(SqlFunction function, string alias)
        {
            Function = function;
            TableAlias = alias;
        }

        public object Clone()
        {
			return new FunctionReference(Function, TableAlias);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is FunctionReference))
			{
				return false;
			}

			FunctionReference other = (FunctionReference)obj;

			return Object.Equals(this.Function, other.Function)
				&& this.TableAlias == other.TableAlias;
		}

		public override int GetHashCode()
		{
			return (Function == null ? 0 : Function.GetHashCode())
				^ (TableAlias == null ? 0 : TableAlias.GetHashCode());
		}
    }
}
