export type LocaleConfig = {
	/** The default locale code (e.g., 'en-US', 'fr-FR') */
	defaultLocale: string
	/** Array of available locale configurations */
	availableLocales: {
		/** The full locale code (e.g., 'en-US', 'fr-FR') */
		language: string
		/** Short language acronym (e.g., 'EN', 'FR') */
		acronym: string
		/** Human-readable display name for the language (e.g., 'English', 'Français') */
		displayName: string
	}[]
}

export type FrameworkConfig = {
	/**
	 * Internationalization and localization settings.
	 */
	locale: LocaleConfig
}
