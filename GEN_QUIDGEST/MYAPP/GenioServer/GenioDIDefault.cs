using CSGenio.core.messaging;
using CSGenio.core.scheduler;
using CSGenio.persistence;
using CSGenio.core.di;
using CSGenio.core.ai;
using GenioServer.security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;


namespace CSGenio
{
    /// <summary>
    /// DI Helper to register the common implementations of each service
    /// </summary>
    public static class GenioDIDefault
    {
        /// <summary>
        /// Registers all the services
        /// </summary>
        public static void Use() 
        {
            UseLog();
            UseDatabase();
            UseMessaging();
            UseScheduler();
            UseElasticSearch();
            UseQueries();
            UseHttpFactory();
            UseAiAgent();
        }

        public static void UseDatabase()
        {
            GenioDI.SpFactory = PersistenceFactoryExtension.getPersistentSupport;
        }

        public static void UseLog()
        {
            GenioDI.Log = new Log4NetImpl();
        }

        public static void UseMessaging()
        {
            GenioDI.Messaging = new MessagingService();
        }

        public static void UseScheduler()
        {
            GenioDI.Scheduler = new SchedulerService();
        }

        public static void UseQueries()
        {
            //TODO: register this data in its own service manager (QueryOverrideManager)
            PersistentSupport.SetControlQueries(
                GenioServer.persistence.PersistentSupportExtra.ControlQueries,
                GenioServer.persistence.PersistentSupportExtra.ControlQueriesOverride);
            GenioServer.framework.OverrideQueryDeclaring.Use();
        }

        public static void UseElasticSearch()
        {
            //TODO: CSGenio.business.ElasticsearchQueriesExtra.Use();
        }

        /// <summary>
        /// Registers the default AI agent. Must be used after the HttpFactory
        /// </summary>
        public static void UseAiAgent()
        {
            GenioDI.RegisterService<IChatbotService>(new ChatbotService());
        }


        public static void UseHttpFactory()
        {
            GenioDI.HttpFactory = new ServiceCollection()
                .AddHttpClient()
                .BuildServiceProvider()
                .GetRequiredService<IHttpClientFactory>();
        }

    }

}
