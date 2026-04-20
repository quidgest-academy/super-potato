using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a sql keyword
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.09.07
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class SqlKeyword
    {
        /// <summary>
        /// The keyword to use in the sql
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.09.07
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public string Keyword
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The keyword to use in the sql</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.09.07
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SqlKeyword(string keyword)
        {
            Keyword = keyword;
        }

        public object Clone()
        {
            return new SqlKeyword(Keyword);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is SqlKeyword))
			{
				return false;
			}

			SqlKeyword other = (SqlKeyword)obj;

			return this.Keyword == other.Keyword;
		}

		public override int GetHashCode()
		{
			return (Keyword == null ? 0 : Keyword.GetHashCode());
		}
    }
}
