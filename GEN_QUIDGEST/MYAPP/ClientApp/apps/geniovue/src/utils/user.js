import { postData } from '@quidgest/clientapp/network'
import eventBus from '@quidgest/clientapp/plugins/eventBus'
import {
	useAuthDataStore,
	useGenericDataStore,
	useGenericLayoutDataStore,
	useNavDataStore,
	useUserDataStore
} from '@quidgest/clientapp/stores'

import { useCustomDataStore } from '@/stores/customData.js'
import { useGlobalTablesDataStore } from '@/stores/globalTablesData.js'
import { useLayoutDataStore } from '@/stores/layoutData.js'
import { updateAFToken, updateMainConfig } from '@/utils/system'

/**
 * Resets the state of all the global stores.
 */
export function resetStoreState() {
	const authDataStore = useAuthDataStore()
	const customDataStore = useCustomDataStore()
	const genericDataStore = useGenericDataStore()
	const userDataStore = useUserDataStore()
	const navDataStore = useNavDataStore()
	const genericLayoutDataStore = useGenericLayoutDataStore()
	const layoutDataStore = useLayoutDataStore()
	const globalTablesDataStore = useGlobalTablesDataStore()

	// Tracing data store is deliberately being left out, since it might be useful to keep it's data.
	authDataStore.resetStore()
	customDataStore.resetStore()
	genericDataStore.resetStore()
	userDataStore.resetStore()
	navDataStore.resetStore()
	genericLayoutDataStore.resetStore()
	layoutDataStore.resetStore()
	globalTablesDataStore.resetStore()
}

/**
 * Resets the application state and navigates the user to the main route
 * after a successful logout.
 */
function logOffSuccess() {
	resetStoreState()

	Promise.all([updateAFToken(), updateMainConfig()]).then(() => {
		eventBus.emit('go-to-route', { name: 'main', params: { isControlled: true } })
	})
}

/**
 * Initiates a server request to log out the user and, upon success,
 * calls the logOffSuccess function to handle post-logout actions.
 */
export function logOff() {
	postData('Account', 'LogOff', {}, logOffSuccess)
}

export default {
	logOff
}
