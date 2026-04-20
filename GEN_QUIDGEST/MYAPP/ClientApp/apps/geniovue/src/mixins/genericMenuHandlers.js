import { computed } from 'vue'
import { mapState, mapActions } from 'pinia'
import _isEmpty from 'lodash-es/isEmpty'

import { useGenericDataStore } from '@quidgest/clientapp/stores'
import { useSystemDataStore } from '@quidgest/clientapp/stores'
import { useUserDataStore } from '@quidgest/clientapp/stores'

import netAPI from '@quidgest/clientapp/network'
import { QEventEmitter } from '@quidgest/clientapp/plugins/eventBus'
import { logOff } from '@/utils/user.js'
import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
import hardcodedTexts from '@/hardcodedTexts.js'
import { removeModal } from '@/utils/layout'

import VueNavigation from '@/mixins/vueNavigation.js'
import NavHandlers from '@/mixins/navHandlers.js'
import ListHandlers from '@/mixins/listHandlers.js'

/*****************************************************************
 * This mixin defines methods to be reused in menu components.   *
 *****************************************************************/
export default {
	mixins: [
		VueNavigation,
		NavHandlers,
		ListHandlers
	],

	data()
	{
		return {
			internalEvents: new QEventEmitter(),

			menuModalIsReady: false,

			menuButtons: {
				backBtn: {
					id: 'back-btn',
					icon: {
						icon: computed(() => this.menuInfo.isPopup ? 'close' : 'back'),
						type: 'svg'
					},
					text: computed(() => this.menuInfo.isPopup ? this.Resources[hardcodedTexts.close] : this.Resources[hardcodedTexts.goBack]),
					isVisible: computed(() => this.canGoBack),
					action: this.goBack
				}
			},

			currentNavLevel: null
		}
	},

	mounted()
	{
		this.currentNavLevel = this.navigation?.currentLevel

		this.$eventHub.on('modal-is-ready', this.setMenuModalReady)

		// If there's alredy a modal for this menu, marks the menu as ready to be shown.
		const modalEl = document.getElementById(this.uiContainersId.main)
		if (modalEl !== null || this.menuInfo.route !== this.$route.name)
			this.menuModalIsReady = true
	},

	beforeUnmount()
	{
		this.$eventHub.off('modal-is-ready', this.setMenuModalReady)

		this.internalEvents?.removeAllListeners()
		this.internalEvents = null

		if(typeof this.componentOnLoadProc?.destroy === 'function')
		{
			this.componentOnLoadProc.destroy()
			this.componentOnLoadProc = null
		}
	},

	computed: {
		...mapState(useSystemDataStore, [
			'system'
		]),

		/**
		 * True if the menu is a popup, false otherwise.
		 */
		isPopup()
		{
			return this.menuInfo.isPopup
		},

		/**
		 * The data of the current user.
		 */
		userData()
		{
			const userDataStore = useUserDataStore()

			return {
				name: userDataStore.username
			}
		},

		/**
		 * True if the menu has visible buttons, false otherwise.
		 */
		hasButtons()
		{
			return Object.values(this.menuButtons).some((b) => b.isVisible)
		},

		/**
		 * Menu container identifiers.
		 */
		uiContainersId()
		{
			const menuIdentifier = `q-modal-${this.isHomePage ? `home-${this.system.currentModule}` : this.menuInfo.route}`

			return {
				main: menuIdentifier && this.isPopup ? `${menuIdentifier}` : 'app',
				body: menuIdentifier && this.isPopup ? `${menuIdentifier}-body` : 'app',
				footer: menuIdentifier && this.isPopup ? `${menuIdentifier}-footer` : 'app'
			}
		},

		/**
		 * True if there's a place to where the user can go back to, false otherwise.
		 */
		canGoBack()
		{
			let levelHigherThan2 = false

			const navLevel = this.currentNavLevel?.level
			if (typeof navLevel === 'number')
				levelHigherThan2 = navLevel > 1

			return this.menuInfo.isPopup || !this.menuInfo.isMenuList || levelHigherThan2
		}
	},

	methods: {
		...mapActions(useGenericDataStore, [
			'setInfoMessage',
			'clearInfoMessages',
			'setModal'
		]),

		...mapActions(useUserDataStore, [
			'addPHEChoice'
		]),

		removeModal,

		isEmpty: genericFunctions.isEmpty,

		/**
		 * A function called whenever a new modal is ready. If that modal is for the current menu,
		 * sets the menu as ready to be shown in it.
		 * @param {string} modalId The id of the modal
		 */
		setMenuModalReady(modalId)
		{
			if (this.menuInfo.route === modalId)
				this.menuModalIsReady = true
		},

		/**
		 * If the menu should be displayed as a popup, sets it's properties.
		 * @param {object} props The dialog component properties
		 * @param {object} modalProps The modal properties
		 */
		setModalProperties(props = {}, modalProps = {})
		{
			if (!this.menuInfo.route)
				return
			if (!this.menuInfo.isPopup)
				return

			props = {
				class: 'q-dialog-form',
				dismissible: false,
				...props
			}

			modalProps = {
				id: this.menuInfo.route,
				...modalProps
			}

			this.setModal(props, modalProps)
		},

		/**
		 * Checks if the previous history levels are valid and updates the navigation accordingly.
		 * @param {object} routeData The data of the menu route
		 */
		updateMenuNavigation(routeData)
		{
			this.menuInfo.route = routeData.name
			this.menuInfo.isPopup = routeData.meta.isPopup
		},

		/**
		 * Called after sending a server request to define the PHE.
		 * @param {object} request The response from the server
		 * @param {object} data An array with the chosen PHEs
		 */
		handleDefinedPHE(request, data)
		{
			if (request.data.Success)
			{
				// Saves the user's choice.
				const chosenPHE = {
					key: this.system.currentModule,
					value: data
				}
				this.addPHEChoice(chosenPHE)

				this.$router.push({ name: 'home' })
			}
			else
			{
				/**
				 * In the case of an error, we cannot go back because the previous menu may have 'Skip if just one,'
				 * 	which will cause it to return to where it was. And if that one also has 'Skip if just one,' it will simply enter a loop.
				 * To prevent the Back button from also entering a loop, we will simply display an error and try go to Main page.
				 */
				const message = request.data.Message || hardcodedTexts.errorProcessingRequest
				const buttons = {
					confirm: {
						label: this.Resources[hardcodedTexts.initialPage],
						action: () => this.$router.push({ name: 'home', params: { module: this.system.defaultModule } })
					},
					cancel: {
						label: this.Resources[hardcodedTexts.close]
					},
					/**
					 * Option to allow user to exit the system if unable to leave the PHE selection interface.
					 * Instead of having to manually delete cookies to lose the session.
					 */
					deny: {
						label: this.Resources[hardcodedTexts.backToLogin],
						action: () => logOff()
					}
				}

				genericFunctions.displayMessage(message, 'error', null, buttons)
			}
		},

		/**
		 * Sets the clicked row as the chosen PHE (used while choosing the initial PHE).
		 * @param {object} listCfg The configuration of the list
		 * @param {object} actionCfg The configuration of the action
		 * @param {object} rowData The data of the clicked row
		 */
		setPHEValue(listCfg, actionCfg, rowData)
		{
			if (_isEmpty(this.controls) || _isEmpty(rowData) || _isEmpty(rowData.rowKey))
				return

			const params = {
				selectedId: rowData.rowKey,
				formId: this.menuInfo.id
			}

			netAPI.postData(
				this.controls.menu.controller,
				'DefineEphForm',
				params,
				(_, request) => this.handleDefinedPHE(request, [rowData.rowKey]))
		},

		/**
		 * Sets the selected rows as the chosen PHEs (used while choosing the initial PHE).
		 * @param {object} listCfg The configuration of the list
		 * @param {object} actionData The configuration of the action and the selected rows
		 */
		setPHEValues(listCfg, actionData)
		{
			if (_isEmpty(this.controls) || _isEmpty(actionData) || _isEmpty(actionData.rowsSelected))
				return

			const data = []

			// Convert the selected row keys from object to list.
			for (const key in actionData.rowsSelected)
				if (actionData.rowsSelected[key])
					data.push(key)

			const params = {
				ids: data,
				formId: this.menuInfo.id
			}

			netAPI.postData(
				this.controls.menu.controller,
				'DefineEphFormValues',
				params,
				(_, request) => this.handleDefinedPHE(request, data))
		}
	},

	watch: {
		'menuInfo.isPopup'(val)
		{
			const modalProps = {
				isActive: val
			}

			this.setModalProperties({}, modalProps)
		}
	}
}
