using CSGenio;

namespace CSGenio.config
{
    /// <summary>
    /// Service that interacts with configuration sources
    /// </summary>
    public interface IConfigurationManager
    {
       

        /// <summary>
        /// Check if the configuration exists
        /// </summary>        
        bool Exists();

        /// <summary>
        /// Get a configuration object from a configuration source. 
        /// Throws an exception if it doesn't exist any.
        /// </summary>        
        ConfigurationXML GetExistingConfig();

        /// <summary>
        /// Persists the configuration object
        /// </summary>
        /// <param name="config">Object to be persisted</param>
        void StoreConfig(ConfigurationXML config);

        /// <summary>
        /// Creates a new configuration and stores it
        /// </summary>
        ConfigurationXML CreateNewConfig();
    }
}
