using Quidgest.Persistence.Dialects;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.persistence
{
    /// <summary>
    /// Microsoft SQL Server persistent support.
    /// Compatibility mode for older sql versions.
    /// </summary>
    public class PersistentSupportSQLServerCompat : PersistentSupportSQLServer
    {
        private static readonly Dialect m_dialect_singleton = new SqlServerCompatDialect();
        
        /// <summary>
        /// Contructor
        /// </summary>
        public PersistentSupportSQLServerCompat() : base() 
		{ 
			Dialect = m_dialect_singleton;
		}
		
    }
}
