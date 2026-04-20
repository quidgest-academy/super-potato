using CSGenio;
using CSGenio.framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenioServer.security
{
    /// <summary>
    /// Common interface for authentication providers
    /// </summary>
    public interface IIdentityProvider
    {
        /// <summary>
        /// Unique identifier for this provider
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// User readable title for this provider
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// True if this provider will be used for 2FA
        /// </summary>
        bool Is2FA {  get; set; }

        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="credential">The user credentials</param>
        /// <returns>The user identity</returns>
        GenioIdentity Authenticate(Credential credential);

        /// <summary>
        /// Determines whether username and password authentication is enabled.
        /// </summary>
        /// <remarks>
        /// This is used to determine if username and password authentication is enabled.
        /// Note: This method checks for the presence of specific identity providers but does not verify the specific login mode (e.g., AD, Certificate, Username/Password) they are configured for. 
        /// 	Further checks might be necessary to determine the exact authentication mode each provider supports.
        /// </remarks>
        bool HasUsernameAuth();

        /// <summary>
        /// Determines if this provider requires the user to be redirected to an external page to authenticate before generating credentials.
        /// Call method <see cref="GetRedirectLoginUrl"/> to calculate the external endpoint.
        /// </summary>
        /// <returns></returns>
        bool HasRedirectLogin();

        /// <summary>
        /// Calculate the external endpoint to redirect the user to login
        /// </summary>
        /// <param name="callback">The internal callback endpoint the external login will call on success</param>
        /// <param name="state">Optional opaque state to send to the provider, that is supposed to be reflected to the callback</param>
        /// <returns>The fully formed login uri</returns>
        string GetRedirectLoginUrl(string callback, string state = null);

        /// <summary>
        /// If the external provider supports it, a endpoint to redirect the user to logout
        /// </summary>
        /// <param name="callback">The page the logout will return to uppon success</param>
        /// <param name="state">Optional opaque state to send to the provider, that is supposed to be reflected to the callback</param>
        /// <returns>The fully formed logout uri</returns>
        string GetRedirectLogoutUrl(string callback, string state = null);


        /// <summary>
        /// Creates a challenge for a login attempt
        /// </summary>
        /// <param name="username">The username for which the new credentials are being requested</param>
        /// <returns>A opaque string representing the challenge for the login to use. The client side UI must know how parse this information.</returns>
        public string AuthenticateChallenge(string username);

        /// <summary>
        /// Requests the settings supported by this provider for creating a new credential
        /// </summary>
        /// <param name="username">The username for which the new credentials are being requested</param>
        /// <returns>A opaque string representing the settings for this request to use. The client side UI must know how parse this information.</returns>
        public string NewCredentialRequest(string username);

        /// <summary>
        /// Checks that the user can correctly acknowledge a challenge, and creates a secret that can be stored with the user
        /// </summary>
        /// <param name="username">Username that requested the challenge</param>
        /// <param name="originalChallenge">The original challenge that was sent to the user</param>
        /// <param name="assertion">Proof sent by the user that he has the key for the challenge</param>
        public CredentialSecret NewCredentialCreate(string username, string originalChallenge, string assertion);

    }

    /// <summary>
    /// Abstract default implementation for IIdentityProvider.
    /// </summary>
    public abstract class BaseIdentityProvider : IIdentityProvider
    {
        /// <inheritdoc/>
        public virtual string Id { get; set; }

        /// <inheritdoc/>
        public virtual string Description { get; set; }

        /// <inheritdoc/>
        public virtual bool Is2FA { get; set; } = false;

        /// <inheritdoc/>
        public virtual bool HasRedirectLogin() => false;

        /// <inheritdoc/>
        public virtual string GetRedirectLoginUrl(string callback, string state = null) => "";

        /// <inheritdoc/>
        public virtual string GetRedirectLogoutUrl(string callback, string state = null) => "";

        /// <inheritdoc/>
        public virtual bool HasUsernameAuth() => false;

        /// <inheritdoc/>
        public abstract GenioIdentity Authenticate(Credential credential);

        /// <inheritdoc/>
        public virtual string AuthenticateChallenge(string username) => "";

        /// <inheritdoc/>
        public virtual string NewCredentialRequest(string username) => "";

        /// <inheritdoc/>
        public virtual CredentialSecret NewCredentialCreate(string username, string originalChallenge, string assertion)
        {
            throw new NotImplementedException("Storing credentials not supported");
        }
        /// <summary>
        /// Initializes the provider with all the options read from the config
        /// </summary>
        /// <param name="config">The provide configuration</param>
        /// <remarks>
        /// Subclasses need to mark properties they wish parsed with SecurityProviderOptionAttribute
        /// </remarks>
        public BaseIdentityProvider(IdentityProviderCfgEl config)
        {
            Id = config.Name;
            Description = config.Description ?? "";
            Is2FA = config.Is2FA;

            var t = GetType();
            var props = t.GetProperties();
            foreach (var p in props)
            {
                var attrs = p.GetCustomAttributes(typeof(SecurityProviderOptionAttribute), true);
                if (attrs == null || attrs.Length == 0)
                    continue;

                if (config.Options.TryGetValue(p.Name, out var optValue))
                {
                    if (p.PropertyType.IsAssignableFrom(typeof(List<string>)))
                        p.SetValue(this, optValue.Split(';').ToList());
                    else
                        p.SetValue(this, Convert.ChangeType(optValue, p.PropertyType, System.Globalization.CultureInfo.InvariantCulture), null);
                }
                else if (!(attrs[0] as SecurityProviderOptionAttribute).Optional)
                    throw new FrameworkException($"Invalid provider parameters", "BaseIdentityProvider.ctor", $"Property {p.Name} is mandatory for provider {Id}");
            }
        }


    }

    /// <summary>
    /// Declares that the class attached can handle authentication of a Credential type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CredentialProviderAttribute : Attribute
    {
        /// <summary>
        /// The Credential type
        /// </summary>
        public Type CredentialType { get; set; }

        /// <summary>
        /// Positional constructor
        /// </summary>
        /// <param name="credentialType">The Credential type</param>
        public CredentialProviderAttribute(Type credentialType)
        {
            this.CredentialType = credentialType;
        }
    }

    /// <summary>
    /// Marks a property of a Security provider as user defined
    /// This allows the UI to present an adequate editor for this property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SecurityProviderOptionAttribute : Attribute
    {
        /// <summary>
        /// Optional property, default is mandatory
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// This property is saved as a complex object json that requires further parsing
        /// </summary>
        public bool IsJson { get; set; }

        /// <summary>
        /// Positional constructor
        /// </summary>
        /// <param name="optional">Optional property, default is mandatory</param>
        public SecurityProviderOptionAttribute(bool optional=false, bool isJson=false)
        {
            Optional = optional;
            IsJson = isJson;
        }
    }

}
