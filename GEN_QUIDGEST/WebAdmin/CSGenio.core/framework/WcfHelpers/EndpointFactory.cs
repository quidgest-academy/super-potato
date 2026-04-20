using Microsoft.Extensions.Configuration;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CSGenio.framework
{
    public class Endpoint
    {
        public string Address { get; set; }

        public string Binding { get; set; }

        public string BindingConfiguration { get; set; }
        public string DnsIdentity { get; set; }

        public string UserName { get; set; }   
        public string Password { get; set; }   

        public virtual System.ServiceModel.EndpointAddress GetEndpointAddress() => throw new NotImplementedException();

        public virtual System.ServiceModel.Channels.Binding GetBinding() => throw new NotImplementedException();

    }

    public class EndpointFactory : Endpoint
    {

        private string m_contractName { get; set; }

        private string m_enpointsConfigurationsKey = "services:endpoints";
        private string m_bindingsConfigurationsKey = "services:bindings";

        private string m_enpointConfigurationKey => $"{m_enpointsConfigurationsKey}:{m_contractName}";
        private string m_bidgingConfigurationKey => $"{m_bindingsConfigurationsKey}:{Binding}:{BindingConfiguration}";

        private IConfiguration m_configuration;

        public EndpointFactory(IConfiguration configuration, string contractName)
        {

            if (contractName is null)
                throw new ArgumentNullException(nameof(contractName));

            m_contractName = contractName;
            m_configuration = configuration;

            Endpoint endpoint = m_configuration.GetRequiredSection(m_enpointConfigurationKey).Get<Endpoint>();

            Address = endpoint.Address;
            Binding = endpoint.Binding;
            BindingConfiguration = endpoint.BindingConfiguration;
            DnsIdentity = endpoint.DnsIdentity;
            UserName = endpoint.UserName;
            Password = endpoint.Password;
        }

        public EndpointFactory(string contractName): this(AppSettings.GetConfiguration(), contractName) { }

        public override EndpointAddress GetEndpointAddress()
        {
            return new System.ServiceModel.EndpointAddress(new Uri(Address));
        }

        public override Binding GetBinding()
        {
            switch (Binding)
            {
                case "basicHttpBinding":
                    return GetBasicHttpBinding();
                default:
                    throw new NotImplementedException(Binding);
            }        
        }

        private Binding GetBasicHttpBinding()
        {
            BasicHttpBinding cfg = m_configuration.GetRequiredSection(m_bidgingConfigurationKey).Get<BasicHttpBinding>();

            if (cfg == null) throw new Exception($"Configurations for {m_bidgingConfigurationKey} not found");

            return cfg;
        }
    }
}