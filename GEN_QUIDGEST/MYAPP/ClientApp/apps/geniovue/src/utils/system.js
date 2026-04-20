import { simpleFetch } from '@quidgest/clientapp/network'
import {
	useAuthDataStore,
	useGenericDataStore,
	useSystemDataStore,
	useTracingDataStore,
	useUserDataStore
} from '@quidgest/clientapp/stores'

/**
 * Update the main configuration
 * @param {CallableFunction} afterUpdateCallback Callback to be executed after update of the main configuration
 * @returns { Promise } Returns the fetch promise
 */
export function updateMainConfig(afterUpdateCallback) {
	// The GetConfig action always requires the system for which it should return the configuration (especially when it requires permissions validation).
	const systemDataStore = useSystemDataStore()

	return simpleFetch('Config', 'GetConfig', systemDataStore.system.currentSystem).then(
		(response) => {
			if (!response.data.Success) return

			setAppConfig(response.data.Data)

			if (typeof afterUpdateCallback === 'function') afterUpdateCallback(response.data.Data)
		}
	)
}

/**
 * Update the Anti Forgery token
 * @returns { Promise } Returns the fetch promise
 */
export function updateAFToken() {
	return simpleFetch('Config', 'GetAFToken').then((response) => {
		if (!response.data.Success) return

		document.getElementsByName('__RequestVerificationToken').forEach((elem) => {
			elem.value = response.data.Data.formToken
		})
	})
}

/**
 * Sets the configuration of the entire application.
 * @param {object} data The app data
 */
export function setAppConfig(data) {
	if (typeof data !== 'object' || data === null) return

	const systemDataStore = useSystemDataStore()
	const genericDataStore = useGenericDataStore()
	const authDataStore = useAuthDataStore()
	const userDataStore = useUserDataStore()
	const tracingDataStore = useTracingDataStore()

	// Set the system data
	if (data.availableModules) systemDataStore.setAvailableModules(data.availableModules)
	if (data.defaultModule) systemDataStore.setDefaultModule(data.defaultModule)
	if (data.currentModule) systemDataStore.setCurrentModule(data.currentModule)
	if (data.years) systemDataStore.setAvailableSystems(data.years)
	if (data.defaultSystem) systemDataStore.setDefaultSystem(data.defaultSystem)

	// Internally the setCurrentSystem is protected from the assignment of an empty value or one that does not exist in the available systems.
	systemDataStore.setCurrentSystem(data.currentSystem || data.defaultSystem)

	if (data.defaultListRows) systemDataStore.setDefaultListRows(data.defaultListRows)
	if (data.schedulerLicense) systemDataStore.setSchedulerLicenseKey(data.schedulerLicense)

	// Set the generic data
	if (data.numberFormat) genericDataStore.setNumberFormat(data.numberFormat)
	if (data.dateFormat) genericDataStore.setDateFormat(data.dateFormat)
	if (data.homePages) genericDataStore.setHomePages(data.homePages)

	// Set the authentication data
	authDataStore.setUsernameAuth(data.hasUsernameAuth)
	authDataStore.setPasswordRecovery(data.hasPasswordRecovery)

	// Set the user data
	if (data.userName) userDataStore.setUserData({ Name: data.userName })

	// Set the tracing data
	if (typeof data.eventTracking === 'boolean')
		tracingDataStore.activateEventTracker(data.eventTracking)
	if (typeof data.enableTracing === 'boolean')
		tracingDataStore.setTracingState(data.enableTracing)

	// Set the version info
	if (data.versionInfo)
		systemDataStore.setVersionInfo(data.versionInfo)
}
