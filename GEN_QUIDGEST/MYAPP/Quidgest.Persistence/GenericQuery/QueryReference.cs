using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Represents a reference to a select query
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class QueryReference : ITableSource
    {
        /// <summary>
        /// The referenced query
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public SelectQuery Query
        {
            get;
            private set;
        }

        /// <summary>
        /// The query alias
        /// </summary>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
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
        /// <param name="query">The select query</param>
        /// <param name="alias">The query alias</param>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public QueryReference(SelectQuery query, string alias)
        {
            Query = query;
            TableAlias = alias;
        }

        public object Clone()
        {
            return new QueryReference(
                Query == null ? null : (SelectQuery)Query.Clone(),
                TableAlias);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is QueryReference))
			{
				return false;
			}

			QueryReference other = (QueryReference)obj;

			return this.TableAlias == other.TableAlias
				&& Object.Equals(this.Query, other.Query);
		}

		public override int GetHashCode()
		{
			return (TableAlias == null ? 0 : TableAlias.GetHashCode())
				^ (Query == null ? 0 : Query.GetHashCode());
		}
    }
}
