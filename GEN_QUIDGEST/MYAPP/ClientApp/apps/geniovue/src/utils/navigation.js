
import eventBus from '@quidgest/clientapp/plugins/eventBus'
import { useNavDataStore } from '@quidgest/clientapp/stores'
import { useSystemDataStore } from '@quidgest/clientapp/stores'

/**
 * Goes back to the previous navigation level, if it exists.
 * @param {string} navigationId The id of the current navigation
 * @param {boolean} hasInitialPHE Whether or not the function is being called from a place with an initial PHE
 */
export function goBack(navigationId = 'main', hasInitialPHE = false)
{
	const systemDataStore = useSystemDataStore()
	const navDataStore = useNavDataStore()
	const navigation = navDataStore.navigation.getHistory(navigationId)
	let currentLevelWasEmpty = false

	if (navigation.currentLevel === null)
	{
		currentLevelWasEmpty = true

		if (navDataStore.previousNav !== null && navDataStore.previousNav.currentLevel !== null)
			navDataStore.retrievePreviousNav()
		else
		{
			// In case there's nothing to go back to, it should go to main.
			eventBus.emit('go-to-route', { name: 'main' })
			return
		}
	}

	// If the last route was skipped because of a "skip if just one" condition,
	// then we need to go back to the route before that last one.
	while (navigation.currentLevel?.params?.skipLastMenu === 'true')
		navigation.removeNavigationLevel()

	if (navigation.previousLevel === null || hasInitialPHE)
	{
		if (navDataStore.previousNav !== null && navDataStore.previousNav.currentLevel !== null)
		{
			// Replace the current navigation with the previous one.
			navDataStore.retrievePreviousNav()

			const params = {
				...navigation.currentLevel.params,
				culture: systemDataStore.system.currentLang, // We don't want to change the language when navigating back.
				keepNavigation: true
			}

			eventBus.emit('go-to-route', { name: navigation.currentLevel.location, params })
		}
		else
		{
			const module = systemDataStore.system.defaultModule
			const params = {
				culture: systemDataStore.system.currentLang,
				system: systemDataStore.system.currentSystem,
				module
			}

			eventBus.emit('go-to-route', { name: `home-${module}`, params })
		}
	}
	else
	{
		// If it's not an empty level, we can remove the current level
		if (!currentLevelWasEmpty)
			navigation.removeNavigationLevel()

		const level = navigation.currentLevel
		const params = {
			...level.params,
			culture: systemDataStore.system.currentLang // We don't want to change the language when navigating back.
		}

		eventBus.emit('go-to-route', { name: level.location, params })
	}
}
