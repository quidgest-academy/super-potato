using Microsoft.Extensions.Configuration;
using System.Linq;
using System.ServiceModel.Description;

namespace CSGenio.framework
{
    public static class ClientBaseFactory<TChannel>
        where TChannel : class
    {
        private static readonly string m_serviceConfigurationKey = AppSettings.GetAttributeContractName(typeof(TChannel));

        private static Endpoint m_endpoint;

        public static System.ServiceModel.ChannelFactory<TChannel> GetServiceClient(string serviceConfigurationKey, string username, string password, IConfiguration config = null)
        {
            if(config != null) m_endpoint = new EndpointFactory(config, serviceConfigurationKey);
            else m_endpoint = new EndpointFactory(serviceConfigurationKey);

            if (!string.IsNullOrEmpty(username)) m_endpoint.UserName = username;
            if (!string.IsNullOrEmpty(password)) m_endpoint.Password = password;

            System.ServiceModel.ChannelFactory<TChannel> serviceClient = new System.ServiceModel.ChannelFactory<TChannel>(GetBinding(), GetEndpointAddress());

            //TODO: Handle multiple logins types
            if (!string.IsNullOrEmpty(m_endpoint.UserName))
            {
                /* Remove the Client Credentials without username and password */
                var defaultCredentials = serviceClient.Endpoint.EndpointBehaviors.FirstOrDefault(t => t.GetType() == typeof(ClientCredentials));
                serviceClient.Endpoint.EndpointBehaviors.Remove(defaultCredentials);

                /* Add the new Client Credentials with username and password */
                ClientCredentials loginCredentials = new ClientCredentials();
                loginCredentials.UserName.UserName = m_endpoint.UserName;
                loginCredentials.UserName.Password = m_endpoint.Password;
                serviceClient.Endpoint.EndpointBehaviors.Add(loginCredentials);
            }
                            
            return serviceClient;
        }

        public static System.ServiceModel.ChannelFactory<TChannel> GetServiceClient(string username, string password, IConfiguration config = null)
        {
            return GetServiceClient(m_serviceConfigurationKey, username, password, config);
        }

        public static System.ServiceModel.ChannelFactory<TChannel> GetServiceClient(string serviceConfigurationKey, IConfiguration config = null)
        {
            return GetServiceClient(serviceConfigurationKey, null, null, config);
        }

        public static System.ServiceModel.ChannelFactory<TChannel> GetServiceClient(IConfiguration config = null)
        {
            return GetServiceClient(m_serviceConfigurationKey, null, null, config);
        }

        private static System.ServiceModel.Channels.Binding GetBinding()
        {
            return m_endpoint.GetBinding();
        }

        private static System.ServiceModel.EndpointAddress GetEndpointAddress() 
        {
            return m_endpoint.GetEndpointAddress();
        }
    }
}