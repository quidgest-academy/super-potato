using System;

namespace Quidgest.Persistence.GenericQuery
{
    public class QueryUnion : ICloneable
    {
        public SelectQuery Query
        {
            get;
            set;
        }

        public bool All
        {
            get;
            set;
        }

        public QueryUnion()
        {
        }

        public QueryUnion(SelectQuery query, bool all)
        {
            Query = query;
            All = all;
        }

        public object Clone()
        {
            return new QueryUnion(Query == null ? null : (SelectQuery)Query.Clone(), All);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is QueryUnion))
			{
				return false;
			}

			QueryUnion other = (QueryUnion)obj;

			return this.All == other.All
				&& Object.Equals(this.Query, other.Query);
		}

		public override int GetHashCode()
		{
			return All.GetHashCode()
				^ (Query == null ? 0 : Query.GetHashCode());
		}
    }
}
