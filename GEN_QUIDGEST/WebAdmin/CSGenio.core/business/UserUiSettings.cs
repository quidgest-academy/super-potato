using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSGenio.business
{
    /// <summary>
	/// Manages user interface settings and configurations.
	/// Handles the persistence and caching of UI settings for individual users.
	/// </summary>
	public abstract class UserUiSettings
	{
		#region User Settings Properties

		/// <summary>
		/// Gets the cache key for this settings instance.
		/// </summary>
		protected string Key { get; }

		#endregion

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		protected UserUiSettings(string key)
		{
			Key = key;
		}

		#region Public Methods

		/// <summary>
		/// Invalidates the cached settings for a specific user.
		/// </summary>
		/// <param name="uuid">The unique identifier for the settings.</param>
		/// <param name="user">The user whose settings should be invalidated.</param>
		public static void Invalidate(string uuid, User user)
		{
			string cacheKey = GenerateCacheKey(uuid, user);
			QCache.Instance.User.Invalidate(cacheKey);
		}

		#endregion

		#region Private Helper Methods

		/// <summary>
		/// Generates a cache key for the specified user and UUID.
		/// </summary>
		protected static string GenerateCacheKey(string uuid, User user) 
			=> $"lstUser_{uuid};{user.Codpsw};{user.Year}";

		/// <summary>
		/// Retrieves settings from cache.
		/// </summary>
		protected static UserUiSettings GetFromCache(string cacheKey)
			=> QCache.Instance.User.Get(cacheKey) as UserUiSettings;

		/// <summary>
		/// Caches the current settings instance.
		/// </summary>
		protected void CacheSettings()
		{
			QCache.Instance.User.Put(Key, this, TimeSpan.FromHours(1));
		}

		#endregion
	}
}