using System;
using Microsoft.Extensions.Configuration;

namespace CSGenio.framework
{
    public class AppSettings
    {

        public static IConfiguration GetConfiguration()
        {

            // Fetch app settings
            return new ConfigurationBuilder()
                // appsettings.json is required
                .AddJsonFile("appsettings.json", optional: false)
#if DEBUG
                // appsettings.Development.json" is optional, values override appsettings.json
                .AddJsonFile($"appsettings.Development.json", optional: true)
#endif
                .Build();
        }


        public static string GetAttributeContractName(Type t)
        {
            // Get instance of the attribute.
            System.ServiceModel.ServiceContractAttribute attribute =
                (System.ServiceModel.ServiceContractAttribute)Attribute.GetCustomAttribute(t, typeof(System.ServiceModel.ServiceContractAttribute));

            return attribute?.ConfigurationName;

        }
    }
}