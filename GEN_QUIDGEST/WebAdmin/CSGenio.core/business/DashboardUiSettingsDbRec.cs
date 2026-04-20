using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSGenio.business
{
	/// <summary>
	/// For dashboard configurations in RAZOR and VUE applications
	/// </summary>
    public class DashboardUiSettingsDbRec : UserUiSettings
    {
        /// <summary>
        /// Gets the user settings.
        /// </summary>
        public CSGenioAlstusr UserSettings { get; private set; }

        /// <summary>
        /// Gets the list of widget configurations.
        /// </summary>
        public List<CSGenioAusrwid> UserWidgets { get; private set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public DashboardUiSettingsDbRec(string key) : base(key) { }

        /// <summary>
        /// Loads user interface settings from cache or database.
        /// </summary>
        /// <param name="sp">The persistence support instance for database operations.</param>
        /// <param name="uuid">The unique identifier for the settings.</param>
        /// <param name="user">The user for whom to load settings.</param>
        /// <returns>A UserUiSettings instance containing the loaded settings.</returns>
        public static DashboardUiSettingsDbRec Load(PersistentSupport sp, string uuid, User user)
        {
            string cacheKey = GenerateCacheKey(uuid, user);
            DashboardUiSettingsDbRec settings = GetFromCache(cacheKey);

            if (settings == null)
            {
                settings = new DashboardUiSettingsDbRec(cacheKey);
                settings.LoadUserSettings(sp, uuid, user);
                settings.CacheSettings();
            }

            return settings;
        }

        /// <summary>
        /// Retrieves settings from cache.
        /// </summary>
        protected new static DashboardUiSettingsDbRec GetFromCache(string cacheKey)
        {
            return UserUiSettings.GetFromCache(cacheKey) as DashboardUiSettingsDbRec;
        }

        /// <summary>
        /// Loads user settings and related configurations.
        /// </summary>
        private void LoadUserSettings(PersistentSupport sp, string uuid, User user)
        {
            UserSettings = CSGenioAlstusr.searchList(sp, user, CriteriaSet.And()
                .Equal(CSGenioAlstusr.FldCodpsw, user.Codpsw)
                .Equal(CSGenioAlstusr.FldDescric, uuid)
                .Equal(CSGenioAlstusr.FldZzstate, 0))
                .FirstOrDefault();

            if (UserSettings != null)
            {
                LoadUserConfigurations(sp, user);
            }
        }

        /// <summary>
        /// Loads user-specific configurations (widgets).
        /// </summary>
        private void LoadUserConfigurations(PersistentSupport sp, User user)
        {
            UserWidgets = CSGenioAusrwid.searchList(sp, user, CriteriaSet.And()
                .Equal(CSGenioAusrwid.FldCodlstusr, UserSettings.ValCodlstusr)
                .Equal(CSGenioAusrwid.FldZzstate, 0))
                .ToList();
        }
    }
}