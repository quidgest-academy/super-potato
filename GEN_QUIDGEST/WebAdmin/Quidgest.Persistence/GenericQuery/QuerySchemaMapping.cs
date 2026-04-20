using System.Collections.Generic;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Declares a mapping from a placeholder schema name to the configured schema name
    /// </summary>
    public class QuerySchemaMapping
    {
        /// <summary>
        /// The identifier name of this SchemaMapping
        /// </summary>
        public string Name { get; set; }

        private Dictionary<string, string> m_map = new Dictionary<string, string>();

        /// <summary>
        /// Add a new translation to the map
        /// </summary>
        /// <param name="key">The placeholder schema</param>
        /// <param name="value">The configured schema name</param>
        public void AddMapping(string key, string value)
        {
            m_map.Add(key, value);
        }

        /// <summary>
        /// Gets the associated configured schema name
        /// </summary>
        /// <param name="key">The placeholder schema</param>
        /// <returns>The configured schema name</returns>
        public string GetValue(string key)
        {
            if (m_map.ContainsKey(key))
                return m_map[key];
            else
                return key;
        }
    }
}
