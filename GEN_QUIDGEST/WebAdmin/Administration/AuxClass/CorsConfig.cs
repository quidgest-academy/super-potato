using System;

namespace Administration.AuxClass
{
    /// <summary>
    /// Represents the CORS (Cross-Origin Resource Sharing) configuration settings from the app settings.
    /// </summary>
    public class CorsConfig
    {
        /// <summary>
        /// Gets or sets the allowed origins for CORS requests. 
        /// These are the domains that are allowed to make cross-origin requests to the server.
        /// </summary>
        public string[] AllowedOrigins { get; set; } = new string[0];

        /// <summary>
        /// Gets or sets the allowed HTTP methods for CORS requests.
        /// These define the HTTP methods (GET, POST, PUT, etc.) that are permitted for cross-origin requests.
        /// </summary>
        public string[] AllowedMethods { get; set; } = new string[0];

        /// <summary>
        /// Gets or sets the allowed headers for CORS requests.
        /// These are the HTTP headers that are allowed to be included in the requests from allowed origins.
        /// </summary>
        public string[] AllowedHeaders { get; set; } = new string[0];
    }
}
