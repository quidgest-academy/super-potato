using CSGenio.framework;
using CSGenio.persistence;
using GenioServer.security;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Security.Principal;
using System.Text;
using Log = CSGenio.framework.Log;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace GenioMVC.Models.Navigation
{
    /// <summary>
    /// Context aggregator for a user
    /// </summary>
    [Serializable]
	public class UserContext
	{
		/// <summary>
		/// The active navigations
		/// </summary>
		public ConcurrentDictionary<string, NavigationContext> Navigations { get; private set; }

		[NonSerialized]
		private PersistentSupport? m_suportePersistente;
		public PersistentSupport PersistentSupport
		{
			get
			{
				if (m_suportePersistente != null)
					return m_suportePersistente;
				m_suportePersistente =  PersistentSupport.getPersistentSupport(User.Year,User.Name);

				return m_suportePersistente;
			}
		}

		public void SetPersistenceReadOnly(bool readOnly)
		{
			if (m_suportePersistente != null)
				m_suportePersistente.closeConnection();
			m_suportePersistente = PersistentSupport.getPersistentSupport(User.Year, User.Name, null, readOnly);
		}

		private User? m_utilizador;
		public User User
		{
			get
			{
				// We only calculate the user once per request thread, so this method can be called multiple times efficiently
				if (m_utilizador != null)
					return m_utilizador;

                //Look into the session if we already established our identity
                string? user_identity = m_httpContext.Session.GetString("user.identity");
				string guest_identity = SecurityFactory.GetGuest().Name ?? "";

                //If the identity is not in session, then check if we need to autologin the user
                //We also retry autologin if the session previously knew we are a guest user but our http request is signaling as authenticated
				if (user_identity == null || (user_identity == guest_identity && m_httpContext.User.Identity.IsAuthenticated))
					user_identity = AutologinIdentity();
				//If user is in session but the authentication subsystem says he is not currently authenticated then his session should no
				// longer be valid and he should be downgraded to a guest user.
				//This can happen for example if the authentication cookie expired before the session cookie, or was explicitly deleted.
				else if( user_identity != null && user_identity != guest_identity && !m_httpContext.User.Identity.IsAuthenticated)
				{
					user_identity = guest_identity;
					QCache.Instance.User.Invalidate("user." + user_identity);
                }
				//if we have the user already in cache we use that instead of reconstructing it
				else
				{
					// GetUserObjectFromCache will perform a deep clone of the User object retrieved from the cache to ensure that
					// multiple threads do not inadvertently corrupt its transient state (such as year, language, and location), which
					// may differ in each thread. This helps to ensure thread safety and consistency of the cached User object.
					m_utilizador = GetUserObjectFromCache(user_identity);
					if (m_utilizador != null) return m_utilizador;
				}

				//otherwise we have no other choice than to recreate the entire user from persistence
				if (user_identity != null && user_identity != guest_identity)
					m_utilizador = GetUserObjectFromPersistence(user_identity);

				//We don't have a user so there is no other possible identity other than guest
				if (m_utilizador == null)
					m_utilizador = GetUserObjectAsGuest();

				//update the session and cache
				user_identity = m_utilizador.Name;
                m_httpContext.Session.SetString("user.identity", user_identity);
				if (user_identity != guest_identity)
					QCache.Instance.User.Put("user." + user_identity, m_utilizador);

				return m_utilizador;
			}

			set
			{
				if (value != null)
				{
					m_httpContext.Session.SetString("user.identity", value.Name);
					if (!SecurityFactory.IsGuest(value.Name))
						QCache.Instance.User.Put("user." + value.Name, value);
				}
				m_utilizador = value;
			}
		}

		/// <summary>
		/// Creates a virtual guest user to represent an unauthenticated unknown user
		/// </summary>
		/// <returns></returns>
		private User GetUserObjectAsGuest()
		{
			//If the user identity is valid but we are unable to authorize it then we revert back to a guest identity
			User user = SecurityFactory.GetGuest();
			user.SessionId = m_httpContext.Session.Id;
			user.Location = m_httpContext.GetIpAddress();
			return user;
		}

		/// <summary>
		/// Retrieves the user information and permissions from the database persistence
		/// </summary>
		/// <param name="user_identity"></param>
		/// <returns></returns>
		private User? GetUserObjectFromPersistence(string user_identity)
		{
			try
			{
				var user = SecurityFactory.Authorize(new()
				{
					AuthenticationType = "internal",
					Name = user_identity,
					IsAuthenticated = true,
					IdProperty = GenioIdentityType.InternalId
				});
				user.SessionId = m_httpContext.Session.Id;
				user.Location = m_httpContext.GetIpAddress();
				user.Year = GetYearFromRoute();
				user.Language = Thread.CurrentThread.CurrentCulture.Name.Replace("-", "").ToUpperInvariant();
				user.CurrentModule = GetModuleFromRoute();

				user = UserFactory.ReadEphs(user);
				// An attempt will be made to recover the Initial EPH if necessary
				TryRestoreInitialEPH(ref user);
				return user;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// This method retrieves a deep clone of a User object from cache using the specified user identity.
		/// Will perform a deep clone of the User object retrieved from the cache to ensure that
		/// multiple threads do not inadvertently corrupt its transient state (such as year, language, and location), which may differ in each thread.
		/// This helps to ensure thread safety and consistency of the cached User object.
		/// </summary>
		/// <param name="user_identity"></param>
		/// <returns></returns>
		private User? GetUserObjectFromCache(string user_identity)
		{
			// Retrieve the user object from cache
			var userCache = QCache.Instance.User.Get("user." + user_identity) as User;

			// If the user object is found in cache
			if (userCache != null)
			{
				try
				{
					// Deep clone the user object to create a new instance of the User class.
					// We need to clone the user so that multiple threads don't corrupt
					//  its transient state (like year, language, location) that can be different in each thread
					var user = userCache.Clone() as User;
					// An attempt will be made to recover the Initial EPH if necessary
					TryRestoreInitialEPH(ref user);
					return user;
				}
				catch
				{
					// If an exception occurs during cloning, return null
					return null;
				}
			}

			// If the user object is not found in cache, return null
			return null;
		}

		/// <summary>
		/// Get the system (year) that comes in the route data or return the default.
		/// The function is now public to avoid code duplication.
		/// </summary>
		/// <returns>The system (year) that comes in the route data or the default.</returns>
		public string GetYearFromRoute()
		{
			string year = Configuration.DefaultYear;
			if (m_httpContext.Request.RouteValues.TryGetValue("system", out var value))
			{
				year = value?.ToString() ?? Configuration.DefaultYear;
	
            	// Ensure that the year exists in the configuration
	            if (!Configuration.Years.Contains(year))
					year = Configuration.DefaultYear;
			}
			return year;
		}

		private string GetModuleFromRoute()
		{
			string module = string.Empty;
            if (m_httpContext.Request.RouteValues.TryGetValue("module", out var value))
                module = value?.ToString() ?? string.Empty;

			return module;
		}

		private string AutologinIdentity()
		{
			try
			{
				string? identity_name = null;
				IPrincipal principal = m_httpContext.User;

                if (principal?.Identity != null && principal.Identity.IsAuthenticated)
                {
					if (Configuration.LoginType == Configuration.LoginTypes.AD)
					{
						var id = principal.Identity;
						if (id is WindowsIdentity && id.Name != null)
							id = new GenericIdentity(id.Name.Substring(id.Name.LastIndexOf('\\') + 1));
						identity_name = id.Name;

						// log login (audit)
						var user = new User(identity_name, m_httpContext.Session.Id, Configuration.DefaultYear, m_httpContext.GetIpAddress());
						CSGenio.framework.Audit.registLoginOut(user, Resources.Resources.ENTRADA31905,
											Resources.Resources.ENTRADA_ATRAVES_DE_A53025, m_httpContext.GetHostName(), m_httpContext.GetIpAddress());

					}
					else if(principal.Identity.AuthenticationType == "LegacyForms")
					{
                        identity_name = principal.Identity.Name;

                        // log login (audit)
                        var user = new User(identity_name, m_httpContext.Session.Id, Configuration.DefaultYear, m_httpContext.GetIpAddress());
                        CSGenio.framework.Audit.registLoginOut(user, Resources.Resources.ENTRADA31905,
                                            Resources.Resources.ENTRADA_ATRAVES_DE_C07809, m_httpContext.GetHostName(), m_httpContext.GetIpAddress());
                    }
                }

				if (identity_name == null && SecurityFactory.AutoLoginGuest)
				{
					//create a guest user
					identity_name = SecurityFactory.GetGuest().Name;
				}

				return identity_name;
			}
			catch
			{
				//revert to guest identity anyway. We don't support null User property for now, so we need to return something.
				return "guest";
			}
		}

        /// <summary>
        /// An attempt will be made to recover the Initial EPH if necessary.
        /// </summary>
        /// <param name="user">The reference to the User</param>
        private void TryRestoreInitialEPH(ref User? user)
		{
			// If necessary, try to restore the initial EPHs
			if (user != null && !user.EphOk)
			{
				Dictionary<string, InitialEPHCache>? initialEphCache = GetInitialEph();
                UserFactory.FillEphRuntime(user, initialEphCache);
			}
		}

		public Dictionary<string, InitialEPHCache> ?GetInitialEph()
		{
            //var initialEphCache = m_httpContext.Session["user.eph.initial"] as Dictionary<string, InitialEPHCache>;
            //TODO: The code that puts this into session must serialize the data in the same way.
            Dictionary<string, InitialEPHCache>? initialEphCache = null;
            if (m_httpContext.Session.TryGetValue("user.eph.initial", out var rawValue))
            {
                var utf8 = Encoding.UTF8.GetString(rawValue);
                initialEphCache = JsonConvert.DeserializeObject<Dictionary<string, InitialEPHCache>>(utf8);
            }

			return initialEphCache;
        }

        public void SetInitialEph(Dictionary<string, InitialEPHCache> initialEphCache)
        {
            string utf8 = JsonConvert.SerializeObject(initialEphCache);
            var bytes = Encoding.UTF8.GetBytes(utf8);
			m_httpContext.Session.Set("user.eph.initial", bytes);
        }

        private readonly HttpContext m_httpContext;
        private readonly IConfiguration m_configuration;

        /// <summary>
        /// ctor
        /// </summary>
        public UserContext(HttpContext context, IConfiguration configuration)
		{
			m_httpContext = context;
			m_configuration = configuration;
			Navigations = new ConcurrentDictionary<string, NavigationContext>();
		}

		/// <summary>
		/// Remove older navigations that are no longer used to free up memory
		/// </summary>
		public void RemoveExpiredNavigations()
		{
			try
			{
				var expiredKeys = Navigations
						.Where(n => !n.Value.IsValid())
						.Select(p => p.Key);
				foreach (string key in expiredKeys)
					_ = Navigations.TryRemove(key, out _);
			}
			catch (System.Exception e)
			{
				Log.Error("Error on RemoveExpiredNavigations; " + e.Message);
			}
		}

		public bool NavigationsContainsKey(string key)
		{
			if (string.IsNullOrEmpty(key))
				return false;
			return Navigations.ContainsKey(key);
		}

		public bool NavigationsGet(string key, out NavigationContext nav)
		{
			if (string.IsNullOrEmpty(key))
			{
				nav = null;
				return false;
			}

			return Navigations.TryGetValue(key, out nav);
		}

		public string NavigationsAdd(NavigationContext nav)
		{
			// Generate a key that is not yet in use
			var newId = NavigationContext.createWinId();
			while (Navigations.ContainsKey(newId))
				newId = NavigationContext.createWinId(10);

			if (Navigations.TryAdd(newId, nav))
			{
				nav.NavigationId = newId;
				return newId;
			}
			else
			{
				// As a last resort, if it fails, we will try to use a larger key
				newId = Guid.NewGuid().ToString();
				if (Navigations.TryAdd(newId, nav))
				{
					nav.NavigationId = newId;
					return newId;
				}
			}

			// There should be some handling of cases when it fails
			Log.Error("Failed to insert a new navigation context.");
			nav.NavigationId = string.Empty;
			return string.Empty;
		}

		public string NavigationsClone(string sourceKey, out NavigationContext navContext)
		{
			if (NavigationsGet(sourceKey, out NavigationContext sourceNav))
				navContext = sourceNav.Clone();
			else
				navContext = new NavigationContext(this);

			var newKey = NavigationsAdd(navContext);

			return newKey;
		}

		private NavigationContext? _currentNavigation;
		public void SetNavigation(NavigationContext navigation)
		{
			this._currentNavigation = navigation;
		}

		/// <summary>
		/// Deixa de ser guardado no CurrentNavidation e passa a ser gerido aqui
		/// No CurrentNavigation passam todos os metodos a static
		/// </summary>
		[JsonIgnore]
		public NavigationContext CurrentNavigation
		{
			get
			{
				if (_currentNavigation == null)
				{
					_currentNavigation = new NavigationContext(this);
					_currentNavigation.SaveOriginal();
				}

				return _currentNavigation;
			}
		}

		public void Destroy()
		{
			m_httpContext.Session.Remove("user.identity");
			QCache.Instance.User.Invalidate("user." + m_httpContext.User.Identity.Name);
		}
	}
}
