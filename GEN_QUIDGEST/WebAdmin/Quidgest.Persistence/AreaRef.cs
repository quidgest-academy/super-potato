using System;

namespace Quidgest.Persistence
{
    public class AreaRef
    {
		public string Schema
		{
			get;
			private set;
		}

		public string Table
        {
            get;
            private set;
        }

        public string Alias
        {
            get;
            private set;
        }

		public AreaRef(string table, string alias)
			: this (null, table, alias)
		{
		}

        public AreaRef(string schema, string table, string alias)
        {
            if (String.IsNullOrEmpty(table))
            {
                throw new ArgumentNullException("table");
            }

            if (String.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

			Schema = schema;
            Table = table;
            Alias = alias;
        }
    }
}
