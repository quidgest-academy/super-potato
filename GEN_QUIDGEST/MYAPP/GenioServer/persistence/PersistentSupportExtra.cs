using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Collections.Generic;
using static CSGenio.persistence.PersistentSupport;

namespace GenioServer.persistence
{
    public class PersistentSupportExtra
    {
        protected static IDictionary<string, ControlQueryDefinition> controlQueries;
        protected static IDictionary<string, overrideDbeditQuery> controlQueriesOverride;

        public static IDictionary<string, ControlQueryDefinition> ControlQueries { get { return controlQueries; } }
        public static IDictionary<string, overrideDbeditQuery> ControlQueriesOverride { get { return controlQueriesOverride; } }

        static PersistentSupportExtra()
        {
            //queries geradas pelo GENIO
            InitControlQueries();
        }

        private static void InitControlQueries()
        {
            controlQueries = new ControlQueryDictionary();
            controlQueriesOverride = new Dictionary<string, overrideDbeditQuery>();

        } 
    }
}
