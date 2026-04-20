/*****************************************************************
 *                                                               *
 * This store holds data about the current state of the user     *
 * navigation, also defining functions to access and mutate it.  *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'

import _isEmpty from 'lodash-es/isEmpty'

import { NavigationContext } from '../models/navigationContext'
import { MAIN_HISTORY_BRANCH_ID } from '../network/constants'

class NavigationState {
	constructor() {
		const mainHistoryBranch = new Map(),
			mainHistory = new NavigationContext(MAIN_HISTORY_BRANCH_ID)
		mainHistoryBranch.set(MAIN_HISTORY_BRANCH_ID, mainHistory)

		this.history = mainHistoryBranch
	}

	getHistory(historyBranchId) {
		if (_isEmpty(historyBranchId)) historyBranchId = MAIN_HISTORY_BRANCH_ID

		if (!this.history.has(historyBranchId)) {
			// eslint-disable-next-line no-console
			console.warn(`Requested navigation context that does not exist. ID: ${historyBranchId}`)
			return new NavigationContext(historyBranchId)
		}

		return this.history.get(historyBranchId)
	}
}

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
		navigation: new NavigationState(),

		previousNav: null
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
	/**
	 * Init navigation context with specific Id.
	 * @param {string} navigationId The Navigation context Id
	 */
	setNavigationContext(navigationId) {
		this.navigation.history.set(navigationId, new NavigationContext(navigationId))
	},

	/**
	 * Ensure that the navigation context exists.
	 * @param {string} navigationId The Navigation context Id
	 */
	beforeRequestContext(navigationId) {
		if (!this.navigation.history.has(navigationId))
			this.navigation.history.set(navigationId, new NavigationContext(navigationId))
	},

	/**
	 * Adds a new history level to the navigation.
	 * @param {object} param0
	 * {
	 *    navigationId - The Navigation context Id
	 *    options - Additional options about the level
	 *    previousLevel - The previous level
	 * }
	 */
	addHistoryLevel({ navigationId, options, previousLevel }) {
		this.navigation.getHistory(navigationId).addHistoryLevel(options, previousLevel)
	},

	/**
	 * Removes the most recent history level from the navigation.
	 * @param {string} navigationId The Navigation context Id
	 */
	removeHistoryLevel(navigationId) {
		this.navigation.getHistory(navigationId).removeNavigationLevel()
	},

	/**
	 * Removes several history levels from the navigation.
	 * @param {object} param0 The Navigation context Id and number of levels to remove
	 */
	removeHistoryLevels({ navigationId, levels }) {
		if (typeof levels !== 'number') return

		while (levels-- > 0) this.removeHistoryLevel(navigationId)
	},

	/**
	 * Remove history levels up to the indicated level. But it doesn't let remove the level 0 which corresponds to the «base» level / home page.
	 * @param {object} param0 The Navigation context Id and history level from which every highest level goes to be removed ( >= 0 )
	 */
	removeNavigationLevelsUpTo({ navigationId, upToLevel }) {
		if (typeof upToLevel !== 'number') return

		const currentLevel = this.navigation.getHistory(navigationId).currentLevel

		if (currentLevel === null) return

		while (currentLevel.level > 0 && currentLevel.level > upToLevel)
			this.removeHistoryLevel(navigationId)
	},

	/**
	 * Adds/updates a value in the entries of the current level.
	 * @param {object} param0 The Navigation context Id, key and value to set
	 */
	setEntryValue({ navigationId, key, value }) {
		if (typeof key !== 'string' || key.length === 0) return
		if (typeof value === 'undefined') return

		const navigation = this.navigation.getHistory(navigationId)
		if (navigation.currentLevel === null) return

		navigation.setEntryValue({ key, value })
	},

	/**
	 * Removes the value with the specified key from the entries of the current level.
	 * @param {object} param0 The Navigation context Id and key to remove
	 */
	removeEntryValue({ navigationId, key }) {
		if (typeof key !== 'string' || key.length === 0) return

		const navigation = this.navigation.getHistory(navigationId)
		if (navigation.currentLevel === null) return

		navigation.removeEntryValue(key)
	},

	/**
	 * Clears the values of all the entries of the current level.
	 * @param {string} navigationId The Navigation context Id
	 */
	clearEntries(navigationId) {
		const navigation = this.navigation.getHistory(navigationId)
		if (navigation.currentLevel === null) return

		navigation.clearEntries()
	},

	/**
	 * Adds/updates a value in the params of the current level.
	 * @param {object} param0 The Navigation context Id and key and value to set
	 */
	setParamValue({ navigationId, key, value }) {
		if (typeof key !== 'string' || key.length === 0) return
		if (typeof value === 'undefined') return

		const navigation = this.navigation.getHistory(navigationId)
		if (navigation.currentLevel === null) return

		navigation.setParamValue({ key, value })
	},

	/**
	 * Removes the value with the specified key from the params of the current level.
	 * @param {object} param0 The Navigation context Id and key to remove
	 */
	removeParamValue({ navigationId, key }) {
		if (typeof key !== 'string' || key.length === 0) return

		const navigation = this.navigation.getHistory(navigationId)
		if (navigation.currentLevel === null) return

		navigation.removeParamValue(key)
	},

	/**
	 * Adds/updates the values of the properties of the current level.
	 * @param {object} param0 The Navigation context Id and properties to set
	 */
	setNavProperties({ navigationId, properties }) {
		if (typeof properties !== 'object') return

		for (const i in properties) {
			const navigation = this.navigation.getHistory(navigationId)
			navigation.setProperty({ key: i, value: properties[i] })
		}
	},

	/**
	 * Removes the value with the specified key from the properties of the current level.
	 * @param {object} param0 The Navigation context Id and key to remove
	 */
	removeNavProperty({ navigationId, key }) {
		if (typeof key !== 'string' || key.length === 0) return

		this.navigation.getHistory(navigationId).removeProperty(key)
	},

	/**
	 * Updates the client-side history with the history received from the server
	 * @param {object} param0 The Navigation context Id and History received from the server
	 */
	updateHistoryByServer({ navigationId, srvHistory }) {
		if (typeof srvHistory !== 'object' || srvHistory === null || !Array.isArray(srvHistory))
			return

		this.navigation.getHistory(navigationId).applyServerChanges(srvHistory)
	},

	/**
	 * Gets the stored form values for navigation context with the provided id.
	 * @param {string} navigationId The Navigation context Id
	 */
	getFormValues(navigationId) {
		return this.navigation.getHistory(navigationId).currentLevel.formValues
	},

	/**
	 * Gets the stored containers state for navigation context with the provided id.
	 * @param {string} navigationId The Navigation context Id
	 */
	getContainersState(navigationId) {
		return this.navigation.getHistory(navigationId)?.currentLevel?.containersState
	},

	/**
	 * Gets the stored data of the currently active control.
	 * @param {string} navigationId The Navigation context Id
	 */
	getCurrentControl(navigationId) {
		const currentLevel = this.navigation.getHistory(navigationId).currentLevel
		const navLevel = currentLevel?.getFirstNotNested()
		let currentControl = navLevel?.currentControl

		if (currentLevel?.isNested && Array.isArray(currentControl?.nestedControls)) {
			const length = currentControl.nestedControls.length
			const lastNested = currentControl.nestedControls[length - 1]

			if (typeof lastNested === 'object') currentControl = lastNested
		}

		if (_isEmpty(currentControl)) return {}

		return {
			id: currentControl.id,
			data: currentControl.data
		}
	},

	/**
	 * Saves the values of the specified fields in the store.
	 * @param {object} param0 The fields data
	 */
	storeValues({ navigationId, key, formInfo, fields }) {
		this.navigation.getHistory(navigationId).storeValues({ key, formInfo, fields })
	},

	/**
	 * Saves the value of the specified field in the store.
	 * @param {object} param0 The field data
	 */
	storeValue({ navigationId, key, formInfo, field, levelNumber = '' }) {
		this.navigation.getHistory(navigationId).storeValue({ key, formInfo, field, levelNumber })
	},

	/**
	 * Saves the state of the specified container in the store.
	 * @param {object} param0 The container and it's current state
	 */
	storeContainerState({ navigationId, key, formInfo, fieldId, containerState }) {
		this.navigation
			.getHistory(navigationId)
			.storeContainerState({ key, formInfo, fieldId, containerState })
	},

	/**
	 * Saves the data of the currently active control.
	 * @param {object} param0 The data of the current control
	 */
	setCurrentControl({ navigationId, controlData }) {
		const currentLevel = this.navigation.getHistory(navigationId).currentLevel
		const navLevel = currentLevel?.getFirstNotNested()
		navLevel?.setCurrentControl(controlData, currentLevel?.isNested)
	},

	/**
	 * Removes the currently active control with the specified id.
	 * @param {object} param0 The data of the control to remove
	 */
	removeCurrentControl({ navigationId, controlId }) {
		const currentLevel = this.navigation.getHistory(navigationId).currentLevel
		const navLevel = currentLevel?.getFirstNotNested()
		navLevel?.removeCurrentControl(controlId)
	},

	/**
	 * Clears the data of the currently active control.
	 * @param {string} navigationId The Navigation context Id
	 */
	clearCurrentControl(navigationId) {
		const currentLevel = this.navigation.getHistory(navigationId).currentLevel
		const navLevel = currentLevel?.getFirstNotNested()
		navLevel?.clearCurrentControl()
	},

	/**
	 * Retrieves the previous navigation history, if it exists, and replaces the current one with it.
	 */
	retrievePreviousNav() {
		if (this.previousNav === null) return

		const previousNav = this.previousNav
		this.previousNav = this.navigation
		this.navigation = previousNav
	},

	/**
	 * Clears the current navigation history.
	 */
	clearHistory() {
		const previousNav = this.navigation
		this.resetStore()
		this.previousNav = previousNav
	},

	/**
	 * Clears the entire navigation history.
	 */
	resetStore() {
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

export const useNavDataStore = defineStore('navData', {
	state,
	actions
})
