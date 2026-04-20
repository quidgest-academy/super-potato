using CSGenio.core.ai;
using CSGenio.core.messaging;
using CSGenio.core.scheduler;
using CSGenio.framework;
using CSGenio.persistence;
using GenioServer.security;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;

namespace CSGenio.core.di
{
    /// <summary>
    /// Service locator for GenioServer singleton classes.
    /// Allows for injection of proxys during testing and more reusable program startup.
    /// </summary>
    /// <seealso cref="GenioDIDefault"/>
    public static class GenioDI
    {
        /// <summary>
        /// Constructor for the specific database vendor providers
        /// </summary>
        public static Func<DatabaseType, PersistentSupport> SpFactory { get; set; } = null;

        /// <summary>
        /// Access to the Messaging service
        /// </summary>
        public static MessagingService Messaging { get; set; } = null;

        /// <summary>
        /// Access to the Scheduler service
        /// </summary>
        public static SchedulerService Scheduler { get; set; } = null;

        /// <summary>
        /// Error logger
        /// </summary>
        public static ILogImpl Log { get; set; } = new CSGenio.core.logger.NullLogger();

        /// <summary>
        /// EPH association manager for user registration functions
        /// </summary>
        public static IMetricsOtlp MetricsOtlp { get; set; } = null;


        /// <summary>
        /// Factory needed to create HttpClients
        /// </summary>
        public static IHttpClientFactory HttpFactory { get; set; } = null;

        private static ConcurrentDictionary<Type, object> _serviceRegistry { get; set; } = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Registers a service instance as a singleton in the GenioDI container.
        /// If the type already exists, it will be replaced.
        /// </summary>
        /// <typeparam name="T">The interface or base type to be registered</typeparam>
        /// <param name="instance">The actual service instance</param>
        /// <exception cref="ArgumentNullException">Thrown if instance is null</exception>
        public static void RegisterService<T>(T instance)
        {
            if (instance == null) 
                throw new ArgumentNullException(nameof(instance), "Service instance cannot be null.");
            
            _serviceRegistry[typeof(T)] = instance;
        }

        /// <summary>
        /// Retrieves the registered service of a given type.
        /// </summary>
        /// <typeparam name="T">The interface or base type to retrieve</typeparam>
        /// <returns>The registered service instance</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no service of the given type is registered</exception>
        public static T GetService<T>()
        {
            if (_serviceRegistry.TryGetValue(typeof(T), out var service))
                return (T)service;
            else 
                throw new KeyNotFoundException($"No registered service found for type {typeof(T).FullName}");
        }

        /// <summary>
        /// Checks if a service is registered.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <returns>True if the service is registered</returns>
        public static bool IsServiceRegistered<T>()
        {
            return _serviceRegistry.ContainsKey(typeof(T));
        }


        //-----------
        // TODO:
        // 1 - Configuration needs to be moved from static class to instance and its singleton kept here
        // 2 - OverrideQuery class is being reallocated multiple times and can be registered here
        // 3 - ElasticSearch service has conditional generated classes, needs restructuring to have always some stub class 
        //-----------
    }
}
  
