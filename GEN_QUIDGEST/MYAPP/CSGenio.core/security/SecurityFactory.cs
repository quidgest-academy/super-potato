using CSGenio;
using CSGenio.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace GenioServer.security
{

	/// <summary>
	/// Implements the security login for the server application. All user identification and autorization
	/// services and policies are configured and abstracted here.
	/// </summary>
	public static class SecurityFactory
	{

		/// <summary>
		/// The authentication algorithm to use when multiple providers are configured
		/// </summary>
		public static AuthenticationMode AuthenticationMode { get; set; }

		/// <summary>
		/// If the authentication should be recovered when it expires
		/// </summary>
		public static bool AllowAuthenticationRecovery { get; set; }

		/// <summary>
		/// The list of identity providers
		/// </summary>
		private static List<IIdentityProvider> m_idProviders = new List<IIdentityProvider>();
		/// <summary>
		/// The role provider
		/// </summary>
		private static List<IRoleProvider> m_roleProviders = new List<IRoleProvider>();

		public static IEnumerable<IIdentityProvider> IdentityProviderList => m_idProviders;
		public static IEnumerable<IRoleProvider> RoleProviderList => m_roleProviders;

		/// <summary>
		/// Static constructor
		/// </summary>
		static SecurityFactory()
		{
			if (Configuration.Security == null)
			{
				AuthenticationMode = AuthenticationMode.AcceptOnFirstSucess;
				AllowAuthenticationRecovery = Configuration.LoginType == Configuration.LoginTypes.AD;
				if (Configuration.LoginType == Configuration.LoginTypes.PUREAD)
				{
					m_idProviders.Add(ParseIdentityProvider(new IdentityProviderCfgEl()
					{
						Name = "ldap",
						Description = "Ldap",
						Type = typeof(LdapIdentityProvider).FullName,
						Options = { { "Dominio", Configuration.Domain } }
					}));
				}
				else
				{
					m_idProviders.Add(ParseIdentityProvider(new IdentityProviderCfgEl()
					{
						Name = "quidgest",
						Description = "Quidgest",
						Type = typeof(QuidgestIdentityProvider).FullName
					}));
				}
				m_roleProviders.Add(ParseRoleProvider(new RoleProviderCfgEl()
				{
					Name = "quidgest",
					Type = typeof(QuidgestRoleProvider).FullName
				}));
			}
			else
			{
				AuthenticationMode = Configuration.Security.AuthenticationMode;
				AllowAuthenticationRecovery = Configuration.Security.AllowAuthenticationRecovery;

				//aqui deve ir ler das configurações e inicializar a cadeia de providers
				foreach (IdentityProviderCfgEl provider in Configuration.Security.IdentityProviders)
				{
					m_idProviders.Add(ParseIdentityProvider(provider));
				}
				foreach (RoleProviderCfgEl provider in Configuration.Security.RoleProviders)
				{
					m_roleProviders.Add(ParseRoleProvider(provider));
				}
			}
		}

		/// <summary>
		/// Instantiates a identity provider of the correct type class, and configures its options
		/// </summary>
		/// <param name="config">The configuration element</param>
		/// <returns>An instatiated Identity provider</returns>
		public static IIdentityProvider ParseIdentityProvider(IdentityProviderCfgEl config)
		{
			Type providerType = Type.GetType(config.Type);

			IIdentityProvider provider = Activator.CreateInstance(providerType, config) as IIdentityProvider
				?? throw new NotImplementedException(config.Type + " does not implement interface GenioServer.security.IIdentityProvider");
			provider.Id = config.Name;

			return provider;
		}

		/// <summary>
		/// Instantiates a role provider of the correct type class, and configures its options
		/// </summary>
		/// <param name="config">The configuration element</param>
		/// <returns>An instatiated Role provider</returns>
		public static IRoleProvider ParseRoleProvider(RoleProviderCfgEl config)
		{
			Type providerType = Type.GetType(config.Type);
			IRoleProvider provider = Activator.CreateInstance(providerType, config) as IRoleProvider
				?? throw new NotImplementedException(config.Type + " does not implement interface GenioServer.security.IRoleProvider");

			return provider;
		}


		/// <summary>
		/// Authenticate user given a credential
		/// </summary>
		/// <param name="credential">The credentials the user presented</param>
		/// <param name="providerId">Optionally specify the provider id you want to authenticate with, or null to use the full provider chain</param>
		/// <returns>A autenticated user with associated roles</returns>
		public static User Authenticate(Credential credential, string providerId = null)
		{
			GenioIdentity id = null;

			//use the provider chain to authenticate the user
			if (string.IsNullOrEmpty(providerId))
			{
				foreach (IIdentityProvider provider in m_idProviders)
				{
					id = provider.Authenticate(credential);
					
					if (AuthenticationMode == AuthenticationMode.AcceptOnFirstSucess && id != null)
						break;
				}
			}
			//use the specified provider to authenticate the user
			else
			{
				IIdentityProvider provider = m_idProviders.FirstOrDefault(p => p.Id == providerId)
					?? throw new ArgumentException($"No identity provider found with id '{providerId}'");
				id = provider.Authenticate(credential);
			}

			if (id is null)
				return null;

			return Authorize(id);
		}


		/// <summary>
		/// Authorize user given a previously authenticated identity
		/// </summary>
		/// <param name="identity">The known identity</param>
		/// <returns>A authorized user with associated roles</returns>
		public static User Authorize(GenioIdentity identity)
		{
			foreach (var cfg in m_roleProviders)
			{
				User res = cfg.Authorize(identity);
				if (res != null)
					return res;
			}
			throw new FrameworkException("Authorization failed. No user returned from providers.", "SecurityFactory.Authorize", "Authorization failed");
		}

		/// <summary>
		/// Associates an external user identity to an existing internal identity
		/// </summary>
		/// <param name="user">The user to which the identity will e associated</param>
		/// <param name="identity">The identity to associate</param>
		public static void RegisterExternalId(User user, GenioIdentity identity)
		{
			foreach (var cfg in m_roleProviders)
				cfg.RegisterExternalId(user, identity);
		}

		/// <summary>
		/// Automates the creation of a new user in the provider
		/// </summary>
		/// <param name="user">The user to create</param>
		/// <param name="claims">Optional set of attributes to set on the user if possible</param>
		/// <param name="credential">The initial credential to associate (some providers may not support some types of credential)</param>
		public static void CreateNewUser(User user, Dictionary<string, object> claims = null, CredentialSecret credential = null)
		{
			foreach (var cfg in m_roleProviders)
				cfg.CreateNewUser(user, claims, credential);
		}
		/// <summary>
		/// Changes the enabled status of a user
		/// </summary>
		/// <param name="user">User to modify</param>
		/// <param name="status">New status for the user</param>
		public static void SetUserEnabled(User user, int status)
		{
			foreach (var cfg in m_roleProviders)
				cfg.SetUserEnabled(user, status);
		}
		/// <summary>
		/// Checks if the system needs to have password management features
		/// </summary>
		public static bool HasPasswordManagement()
		{
			//For now only systems with Quidgest identity provider manage passwords
			return m_idProviders.Any(x => x is QuidgestIdentityProvider);
		}

		/// <summary>
		/// Determines whether username and password authentication is enabled.
		/// </summary>
		/// <remarks>
		/// This method returns true if either QuidgestIdentityProvider or LdapIdentityProvider is present in the list of identity providers.
		/// This is used to determine if username and password authentication is enabled, assuming that either QuidgestIdentityProvider
		/// or LdapIdentityProvider supports this method of authentication.
		/// Note: This method checks for the presence of specific identity providers but does not verify the specific login mode (e.g., AD, Certificate, Username/Password) they are configured for. 
		/// 	Further checks might be necessary to determine the exact authentication mode each provider supports.
		/// </remarks>
		public static bool HasUsernameAuth()
		{
			return m_idProviders.Any(x => x.HasUsernameAuth());
		}


		private static string m_guestName = Configuration.Security
			?.Users
			?.FirstOrDefault((x) => x.Type == UserType.Guest)
			?.Name
			?? "guest";


		/// <summary>
		/// True if a guest user should be allowed on the public site pages, 
		/// false if all users must be redirected to login page immediately
		/// </summary>
		public static bool AutoLoginGuest => Configuration.Security
			?.Users
			?.FirstOrDefault((x) => x.Type == UserType.Guest)
			?.AutoLogin
			?? false;

		/// <summary>
		/// Check if this username can be considered a guest
		/// </summary>
		/// <param name="username">The username to check</param>
		/// <returns>Trues if its a guest username, false otherwise</returns>
		public static bool IsGuest(string username) => username == m_guestName;

		/// <summary>
		/// Check if this identity can be considered a guest
		/// </summary>
		/// <param name="identity">The identity to check</param>
		/// <returns>Trues if its a guest identity, false otherwise</returns>
		public static bool IsGuest(IIdentity identity)
		{
			return IsGuest(identity.Name);
		}

		/// <summary>
		/// Creates a guest user
		/// </summary>
		/// <returns>A guest user</returns>
		public static User GetGuest()
		{
			User user = new User(m_guestName, "", Configuration.DefaultYear);
			user.Language = System.Threading.Thread.CurrentThread.CurrentCulture.Name.Replace("-", "").ToUpperInvariant();
			user.Years.Add(Configuration.DefaultYear);
			user.Public = true;

			foreach (string module in Configuration.Modules)
				user.AddModuleRole(module, Role.UNAUTHORIZED);
			user.AddModuleRole("Public", Role.UNAUTHORIZED);

			return user;
		}

		/// <summary>
		/// Creates a admin user that will perform tasks on behalf of a normal user.
		/// </summary>
		/// <remarks>
		/// The returned user retains the original user's name, year, and current module, 
		/// but is assigned the administrator role for the current module.
		/// </remarks>
		/// <param name="user">The user to be elevated. The user must have a valid name and current module assigned.</param>
		/// <returns>A new <see cref="User"/> instance with the administrator role assigned to the user's current module.</returns>
		public static User ElevateUserToAdmin(User user)
		{
			User adminUser = new User(user.Name, "", user.Year);
			adminUser.CurrentModule = user.CurrentModule ?? "Public";
			foreach(var module in Configuration.Modules)
				adminUser.AddModuleRole(module, CSGenio.framework.Role.ADMINISTRATION);
			return adminUser;
		}

		/// <summary>
		/// Requests the settings supported by this provider for creating a new credential
		/// </summary>
		/// <param name="providerId">Id of the Provider</param>
		/// <param name="username">The username for which the new credentials are being requested</param>
		/// <returns>A opaque string representing the settings for this request to use. The client side UI must know how parse this information.</returns>
		public static string NewCredentialRequest(string providerId, string username)
		{
			IIdentityProvider provider = IdentityProviderList.FirstOrDefault(x => x.Id == providerId);
			return provider.NewCredentialRequest(username);
		}

		/// <summary>
		/// Checks that the user can correctly acknowledge a challenge, and creates a secret that can be stored with the user
		/// </summary>
		/// <param name="providerId">Id of the Provider</param>
		/// <param name="username">Username that requested the challenge</param>
		/// <param name="originalChallenge">The original challenge that was sent to the user</param>
		/// <param name="assertion">Proof sent by the user that he has the key for the challenge</param>
		public static void StoreCredential(string providerId, User user, string originalRequest, string credential)
		{
			IIdentityProvider provider = IdentityProviderList.FirstOrDefault(x => x.Id == providerId);
			//use the identity provider to parse the challenge into a CredentialSecret
			CredentialSecret secret = provider.NewCredentialCreate(user.Name, originalRequest, credential);
			//pass the secret into the role provider so it can store it in the appropriate user attributes
			foreach (var role in RoleProviderList)
				role.StoreCredential(user, secret);
		}
		
		/// <summary>
		/// Extracts the credential type of a identity provider
		/// </summary>
		/// <param name="provider">The provider</param>
		/// <returns>The name of the credential type</returns>
		public static string GetCredentialType(IIdentityProvider provider)
		{
			var credentials = provider.GetType().GetCustomAttributes(typeof(CredentialProviderAttribute), false);
			if (credentials is null || credentials.Length == 0)
				return "";
			if (credentials[0] is not CredentialProviderAttribute attr)
				return "";
			return attr.CredentialType.Name;
		}
	}
}
