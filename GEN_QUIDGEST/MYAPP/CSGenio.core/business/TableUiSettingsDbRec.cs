using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSGenio.business
{
	/// <summary>
	/// For old table column configurations in RAZOR applications
	/// </summary>
    public class TableUiSettingsDbRec : UserUiSettings
    {
        /// <summary>
        /// Gets the user settings.
        /// </summary>
        public CSGenioAlstusr UserSettings { get; private set; }

        /// <summary>
        /// Gets the list of column configurations.
        /// </summary>
        public List<CSGenioAlstcol> UserColumns { get; private set; }

        /// <summary>
        /// Gets the list of rendering configurations.
        /// </summary>
        public List<CSGenioAlstren> UserRenderings { get; private set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TableUiSettingsDbRec(string key) : base(key) { }

        /// <summary>
        /// Loads user interface settings from cache or database.
        /// </summary>
        /// <param name="sp">The persistence support instance for database operations.</param>
        /// <param name="uuid">The unique identifier for the settings.</param>
        /// <param name="user">The user for whom to load settings.</param>
        /// <returns>A UserUiSettings instance containing the loaded settings.</returns>
        public static TableUiSettingsDbRec Load(PersistentSupport sp, string uuid, User user)
        {
            string cacheKey = GenerateCacheKey(uuid, user);
            TableUiSettingsDbRec settings = GetFromCache(cacheKey);

            if (settings == null)
            {
                settings = new TableUiSettingsDbRec(cacheKey);
                settings.LoadUserSettings(sp, uuid, user);
                settings.CacheSettings();
            }

            return settings;
        }

        /// <summary>
        /// Retrieves settings from cache.
        /// </summary>
        protected new static TableUiSettingsDbRec GetFromCache(string cacheKey)
        {
            return UserUiSettings.GetFromCache(cacheKey) as TableUiSettingsDbRec;
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
        /// Loads user-specific configurations (columns, renderings).
        /// </summary>
        private void LoadUserConfigurations(PersistentSupport sp, User user)
        {
            UserColumns = CSGenioAlstcol.searchList(sp, user, CriteriaSet.And()
                .Equal(CSGenioAlstcol.FldCodlstusr, UserSettings.ValCodlstusr)
                .Equal(CSGenioAlstcol.FldZzstate, 0))
                .OrderBy(x => x.ValPosicao)
                .ToList();

            UserRenderings = CSGenioAlstren.searchList(sp, user, CriteriaSet.And()
                .Equal(CSGenioAlstren.FldCodlstusr, UserSettings.ValCodlstusr)
                .Equal(CSGenioAlstren.FldZzstate, 0))
                .OrderBy(x => x.ValPosicao)
                .ToList();
        }
    }
}