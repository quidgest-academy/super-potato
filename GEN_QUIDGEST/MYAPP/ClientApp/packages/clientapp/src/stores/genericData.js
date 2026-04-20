/*****************************************************************
 *                                                               *
 * This store holds generic data about the application,          *
 * also defining functions to access and mutate it.              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'
import _remove from 'lodash-es/remove'
import merge from 'lodash-es/merge'

import { focusElement } from '../utils/genericFunctions'

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
		menus: {},

		menuPath: [],

		infoMessages: [],

		modals: [],

		reportingModeCAV: false,

		suggestionModeOn: false,

		notifications: [],

		homepages: {},

		isPublicRoute: false,

		isFullScreenPage: false,

		asyncProcesses: [],

		busyPageStateStack: [],

		shouldShowCookies: true,

		numberFormat: {
			decimalSeparator: ',',
			thousandsSeparator: ' ',
			negativeFormat: '-'
		},

		dateFormat: {
			date: 'dd/MM/yyyy',
			dateTime: 'dd/MM/yyyy HH:mm',
			dateTimeSeconds: 'dd/MM/yyyy HH:mm:ss',
			time: 'HH:mm'
		},

		maintenance: {
			isActive: false,
			isScheduled: false,
			schedule: undefined
		}
	}
}

//----------------------------------------------------------------
// Variable getters
//----------------------------------------------------------------

const getters = {
	/**
	 * True if there are any menus to show, false otherwise.
	 * @param {object} state The current global state
	 */
	hasMenus(state) {
		return state.menus && state.menus.MenuList && state.menus.MenuList.length > 0
	},

	/**
	 * The id of the latest modal/popup that has a route, or an empty string if there are no popups or none of them have routes.
	 * @param {object} state The current global state
	 */
	latestModalId(state) {
		let id = ''

		for (const modal of state.modals) if (modal.hasRoute && modal.isActive) id = modal.id

		return id
	},

	/**
	 * A list of the info messages that are fixed on the screen.
	 * @param {object} state The current global state
	 */
	fixedInfoMessages(state) {
		return state.infoMessages.filter((message) => message.pinned)
	},

	/**
	 * A list of the info messages that aren't fixed on the screen.
	 * @param {object} state The current global state
	 */
	relativeInfoMessages(state) {
		return state.infoMessages.filter((message) => !message.pinned)
	},

	/**
	 * Whether there are any asynchronous processes currently running.
	 * @param {object} state The current global state
	 */
	isLoading(state) {
		return state.asyncProcesses.length > 0 || state.busyPageStateStack.length > 0
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
	/**
	 * Sets the format used by numeric inputs in the application.
	 * @param {object} numberFormat The formats of the numbers
	 */
	setNumberFormat(numberFormat) {
		if (typeof numberFormat !== 'object' || numberFormat === null) return

		this.numberFormat.decimalSeparator = numberFormat.DecimalSeparator ?? ','
		this.numberFormat.thousandsSeparator = numberFormat.GroupSeparator ?? ' '
		this.numberFormat.negativeFormat = numberFormat.NegativeFormat ?? '-'
	},

	/**
	 * Sets the format used by date inputs in the application.
	 * @param {object} dateFormat The formats of the dates
	 */
	setDateFormat(dateFormat) {
		if (typeof dateFormat !== 'object' || dateFormat === null) return
		if (
			!dateFormat.date &&
			!dateFormat.dateTime &&
			!dateFormat.dateTimeSeconds &&
			!dateFormat.time
		)
			return

		for (const i in dateFormat) {
			// Get property name starting with lowercase letter
			const propName = i.substring(0, 1).toLowerCase() + i.substring(1)
			this.dateFormat[propName] = dateFormat[i]
		}
	},

	/**
	 * Sets the available menus.
	 * @param {string} menus The available menus
	 */
	setMenus(menus) {
		if (typeof menus !== 'object' || menus === null) return

		this.menus = menus
	},

	/**
	 * Adds a new alert to the list of currently displayed info messages.
	 * @param {object} alertProps The properties of the alert
	 */
	setInfoMessage(alertProps) {
		if (
			typeof alertProps !== 'object' ||
			alertProps === null ||
			typeof alertProps.message !== 'string'
		)
			return
		if (typeof alertProps.isDismissible === 'undefined') alertProps.isDismissible = true
		if (typeof alertProps.isResource === 'undefined') alertProps.isResource = false

		const length = this.infoMessages.length

		if (length === 0) alertProps.id = 1
		else alertProps.id = this.infoMessages[length - 1].id + 1

		const duplicatedResult = this.infoMessages.find(
			(item) => item.type === alertProps.type && item.message === alertProps.message
		)
		if (duplicatedResult !== undefined) return

		this.infoMessages.push(alertProps)
	},

	/**
	 * Removes the specified alert from the list of currently displayed info messages.
	 * @param {number} alertId The id of the alert
	 */
	removeInfoMessage(alertId) {
		if (typeof alertId !== 'number' || !Number.isInteger(alertId)) return
		if (alertId < 1) return

		for (let i = 0; i < this.infoMessages.length; i++) {
			if (this.infoMessages[i].id !== alertId) continue

			this.infoMessages.splice(i, 1)
			return
		}
	},

	/**
	 * Clears the list of info messages.
	 */
	clearInfoMessages() {
		this.infoMessages = []
	},

	/**
	 * Sets the list of notifications.
	 */
	setNotifications(notifications) {
		this.notifications = notifications
	},

	/**
	 * Removes the specified alert from the list of currently displayed alerts.
	 * @param {number} alertId The id of the alert
	 */
	removeNotification(alertId) {
		for (let i = 0; i < this.notifications.length; i++) {
			if (this.notifications[i].id !== alertId) continue

			this.notifications.splice(i, 1)
			return
		}
	},

	/**
	 * Clears the list of notifications.
	 */
	clearNotifications() {
		this.notifications = []
	},

	/**
	 * Adds a new modal/popup to be displayed.
	 * @param {object} props The dialog component properties
	 * @param {object} modalProps The modal properties
	 */
	setModal(props = {}, modalProps = {}) {
		// Default dialog props
		const dialogProps = {
			dismissible: true,
			size: 'large',
			buttons: null
		}

		merge(dialogProps, props)
		props = dialogProps

		if (typeof modalProps.id !== 'string') modalProps.id = this.modals.length.toString()

		// If no return element is specified, use the element that opened the popup
		if (modalProps.returnElement === undefined || modalProps.returnElement === null)
			modalProps.returnElement =
				document.activeElement?.id ??
				'' /* Why is the entire HTMLElement used (before) !? TODO: Improve this code */

		let index = -1

		for (let i = 0; i < this.modals.length; i++) {
			if (this.modals[i].id !== modalProps.id) continue

			index = i
			break
		}

		if (index < 0) // Creates a new modal.
		{
			if (typeof modalProps.isActive !== 'boolean') modalProps.isActive = true
			if (typeof modalProps.hasRoute !== 'boolean') modalProps.hasRoute = false

			this.modals.push({ ...modalProps, props })
		} else // Updates the props of an existing modal.
		{
			this.modals[index] = {
				...this.modals[index],
				...modalProps,
				props: {
					...this.modals[index].props,
					...props
				}
			}
		}
	},

	/**
	 * Removes the specified modal/popup, or the last one if no id is passed.
	 * @param {string} modalId The id of the modal
	 */
	removeModal(modalId) {
		const length = this.modals.length
		if (length === 0) return

		let id = modalId
		if (typeof modalId !== 'string') id = this.modals[length - 1]

		for (let i = 0; i < this.modals.length; i++) {
			if (this.modals[i].id !== id) continue

			const removedModalArr = this.modals.splice(i, 1)

			// Focus on the element that opened the popup, if it still exists.
			const returnElement = removedModalArr[0]?.returnElement
			if (returnElement !== undefined && returnElement !== null) focusElement(returnElement)

			return removedModalArr[0]
		}
	},

	/**
	 * Clears the list of modals/popups.
	 */
	clearModals() {
		this.modals = []
	},

	/**
	 * Sets the state of the CAV mode.
	 * @param {boolean} isVisible Whether or not the CAV mode is active
	 */
	setReportingModeCAV(isActive) {
		if (typeof isActive !== 'boolean') return

		this.reportingModeCAV = isActive

		if (this.reportingModeCAV) this.suggestionModeOn = false
	},

	/**
	 * Sets the state of the Suggestion mode.
	 * @param {boolean} isVisible Whether or not the Suggestion mode is active
	 */
	setSuggestionMode(isActive) {
		if (typeof isActive !== 'boolean') return

		this.suggestionModeOn = isActive

		if (this.suggestionModeOn) this.reportingModeCAV = false
	},

	/**
	 * Toggles the state of the Suggestion mode.
	 */
	toggleSuggestionMode() {
		this.setSuggestionMode(!this.suggestionModeOn)
	},

	/**
	 * Adds a new entry to the menus path.
	 * @param {string} menuId The id of the menu
	 */
	addToMenuPath(menuId) {
		if (typeof menuId !== 'string' || menuId.length < 1) return
		if (this.menuPath.includes(menuId)) return

		this.menuPath.push(menuId)
	},

	/**
	 * Removes an entry from the menus path.
	 * @param {string} menuId The id of the menu
	 */
	removeFromMenuPath(menuId) {
		if (typeof menuId !== 'string' || menuId.length < 1) return

		for (let i = 0; i < this.menuPath.length; i++)
			if (this.menuPath[i].startsWith(menuId)) this.menuPath.splice(i, 1)
	},

	/**
	 * Clears the menus path.
	 */
	clearMenuPath() {
		this.menuPath = []
	},

	/**
	 * Resets the menus path and adds the first menu to it.
	 * @param {boolean} loadFirstMenu Whether or not to load the first menu
	 */
	resetMenuPath(loadFirstMenu = false) {
		this.clearMenuPath()

		if (this.menus?.MenuList?.length === 0 || !loadFirstMenu) return

		const menu = this.menus.MenuList[0]
		this.addToMenuPath(menu.Order)
	},

	/**
	 * Sets the available home pages.
	 * @param {object} homepages The available home pages
	 */
	setHomePages(homepages) {
		if (typeof homepages !== 'object' || homepages === null) return

		this.homepages = homepages
	},

	/**
	 * Sets whether the route is public.
	 * @param {boolean} isPublicRoute Whether or not the route is public
	 */
	setPublicRoute(isPublicRoute) {
		if (typeof isPublicRoute !== 'boolean') return

		this.isPublicRoute = isPublicRoute
	},

	/**
	 * Sets whether this is a full-screen page.
	 * @param {boolean} isFullScreenPage Whether or not it's a full-screen page
	 */
	setFullScreenPage(isFullScreenPage) {
		if (typeof isFullScreenPage !== 'boolean') return

		this.isFullScreenPage = isFullScreenPage
	},

	/**
	 * Adds the specified process to the stack of running processes.
	 * @param {string} processId The id of the process
	 */
	addAsyncProcess(processId) {
		if (typeof processId !== 'string' || processId.length === 0) return

		this.asyncProcesses.push(processId)
	},

	/**
	 * Removes the process with the specified id from the stack of running processes.
	 * @param {any} processId The id of the process
	 */
	removeAsyncProcess(processId) {
		_remove(this.asyncProcesses, (procId) => procId === processId)
	},

	/**
	 * Adds the specified process to the stack of page blocking processes.
	 * @param {object} process The process
	 */
	addProcessToBusyPageStack(process) {
		if (typeof process !== 'object' || !Reflect.has(process, 'id')) return

		this.busyPageStateStack.push(process)
	},

	/**
	 * Removes the process with the specified id from the stack of page blocking processes.
	 * @param {any} processId The id of the process
	 */
	removeProcessFromBusyPageStack(processId) {
		_remove(this.busyPageStateStack, (proc) => proc.id === processId)
	},

	/**
	 * Sets whether the cookies are visible.
	 * @param {boolean} showCookies The value of the cookies visibility
	 */
	setShowCookies(showCookies) {
		if (typeof showCookies !== 'boolean') return

		this.shouldShowCookies = showCookies
	},

	/**
	 * Updates the maintenance information.
	 * @param {object} maintenance The updated maintenance information
	 */
	setMaintenanceStatus(maintenance) {
		this.maintenance.isActive = maintenance.IsActive
		this.maintenance.isScheduled = maintenance.IsScheduled
		this.maintenance.schedule = maintenance.Schedule
	},

	/**
	 * Resets the generic data.
	 */
	resetStore() {
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

export const useGenericDataStore = defineStore('genericData', {
	state,
	getters,
	actions
})
