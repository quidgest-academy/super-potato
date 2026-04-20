using CSGenio;
using CSGenio.framework;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenioServer.security;

/// <summary>
/// Common interface for role providers
/// </summary>
public interface IRoleProvider
{
    /// <summary>
    /// Unique identifier for this provider
    /// </summary>
    string Id { get; }
    /// <summary>
    /// Validates a user identity in the provider and in case of success obtains the associated roles for the user.
    /// It will convert an external identity into a normalized internal username and userid.
    /// </summary>
    /// <param name="identity">The identity to validate</param>
    /// <returns>A user with normalized username, userid and roles</returns>
    User Authorize(GenioIdentity identity);
    /// <summary>
    /// True is this providers has the capability to manage its user directory
    /// </summary>
    bool HasUserDirectory { get; }
    /// <summary>
    /// Automates the creation of a new user in the provider
    /// </summary>
    /// <param name="user">The user to create</param>
    /// <param name="claims">Optional set of attributes to set on the user if possible</param>
    /// <param name="credential">The initial credential to associate (some providers may not support some types of credential)</param>
    void CreateNewUser(User user, Dictionary<string, object> claims = null, CredentialSecret credential = null);
    /// <summary>
    /// Associates an external user identity to an existing internal identity
    /// </summary>
    /// <param name="user">The user to which the identity will e associated</param>
    /// <param name="identity">The identity to associate</param>
    void RegisterExternalId(User user, GenioIdentity identity);
    /// <summary>
    /// Changes the enabled status of a user
    /// </summary>
    /// <param name="user">User to modify</param>
    /// <param name="status">New status for the user</param>
    void SetUserEnabled(User user, int status);
    /// <summary>
    /// Get a list of all the users that the provider is able to manage
    /// </summary>
    /// <param name="numRecords">Maximum number of users to obtain</param>
    /// <param name="page">The page of users to obtain</param>
    /// <param name="criteria">A criteria set to filter the users (some providers may not support this)</param>
    /// <returns>A list of users</returns>
    List<User> ListUsers(int numRecords = 50, int page = 0, CriteriaSet criteria = null);
    /// <summary>
    /// Creates all the roles and metadata necessary for the provider to match the role needs of this application.
    /// </summary>
    void SetupUserDirectory();
    /// <summary>
    /// Adds a new credential or updates an existing one for the user
    /// </summary>
    /// <param name="user">The user where to store the credential</param>
    /// <param name="credential">The credential to add (some providers may not support some credential types)</param>
    void StoreCredential(User user, CredentialSecret credential);
}

/// <summary>
/// Base role provider to facilitate implementation. 
/// Most role providers should inherit from this class.
/// </summary>
public abstract class BaseRoleProvider : IRoleProvider
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="config">The configuration object that parametrizes the options of this provider</param>
    protected BaseRoleProvider(RoleProviderCfgEl config)
    {
        Id = config.Name;

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
                throw new FrameworkException($"Invalid provider parameters", "BaseRoleProvider.ctor", $"Property {p.Name} is mandatory for provider {t.Name}");
        }
    }

    /// <inheritdoc/>
    public string Id { get; protected set; }

    /// <inheritdoc/>
    public abstract User Authorize(GenioIdentity identity);

    /// <inheritdoc/>
    public virtual bool HasUserDirectory => false;
    /// <inheritdoc/>
    public virtual void CreateNewUser(User user, Dictionary<string, object> claims = null, CredentialSecret credential = null)
    {
        throw new InvalidOperationException("Providers must supply this method if HasUserDirectory is true.");
    }
    /// <inheritdoc/>
    public virtual void RegisterExternalId(User user, GenioIdentity identity)
    {
    }
    /// <inheritdoc/>
    public virtual void SetUserEnabled(User user, int status)
    {
    }
    /// <inheritdoc/>
    public virtual List<User> ListUsers(int numRecords = 50, int page = 0, CriteriaSet criteria = null)
    {
        throw new InvalidOperationException("Providers must supply this method if HasUserDirectory is true.");
    }
    /// <inheritdoc/>
    public virtual void SetupUserDirectory()
    {
        throw new InvalidOperationException("Providers must supply this method if HasUserDirectory is true.");
    }

    /// <inheritdoc/>
    public virtual void StoreCredential(User user, CredentialSecret credential)
    {
    }
}
