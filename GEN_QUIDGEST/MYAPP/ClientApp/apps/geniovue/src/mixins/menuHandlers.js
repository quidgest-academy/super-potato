import hardcodedTexts from '@/hardcodedTexts.js'
import { loadResources } from '@/plugins/i18n.js'

import { messageTypes } from '@quidgest/clientapp/constants/enums'
import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
import GenericMenuHandlers from './genericMenuHandlers.js'

/***********************************************************************
 * This mixin defines methods to be reused in regular menu components. *
 ***********************************************************************/
export default {
	mixins: [
		GenericMenuHandlers
	],

	data()
	{
		return {
			// When a PopUp form is opened, the menus behind it cannot appear to be loaded, especially because of E2E testing.
			isActiveMenu: true
		}
	},

	created()
	{
		this.componentOnLoadProc.addBusy(loadResources(this, this.interfaceMetadata.requiredTextResources), this.Resources[hardcodedTexts.genericLoad], 300)

		this.componentOnLoadProc.addWL(this.loadList())
		this.componentOnLoadProc.once(async () => {
			this.setMenuNavProperties()

			for (const i in this.controls)
			{
				await this.controls[i].init()
				this.controls[i].initData?.()
			}
		}, this)
	},

	mounted()
	{
		this.$eventTracker.addTrace({ origin: 'mounted (menuHandler)', message: 'Menu is mounted', contextData: { menuInfo: this.menuInfo } })

		// Listens for changes to the DB and updates the list accordingly.
		this.$eventHub.on('change-content-active-state', this.changeMenuActiveState)
		this.internalEvents.onMany(this.controls.menu.internalEvents, this.loadList)
		this.$eventHub.onMany(this.controls.menu.globalEvents, this.loadList)
	},

	beforeUnmount()
	{
		this.$eventTracker.addTrace({ origin: 'beforeUnmount (menuHandler)', message: 'Menu will be unmounted', contextData: { menuInfo: this.menuInfo } })
		// Removes the listeners.
		this.internalEvents?.offMany(this.controls.menu.internalEvents, this.loadList) // The generic handler, in beforeUnmount, already removes all events.
		this.$eventHub.offMany(this.controls.menu.globalEvents, this.loadList)
		this.$eventHub.off('change-content-active-state', this.changeMenuActiveState)

		this.changeMenuActiveState(false)

		if (this.controls)
		{
			const controlsIds = Object.keys(this.controls)
			controlsIds.forEach((controlId) => {
				if (typeof this.controls[controlId].destroy === 'function')
					this.controls[controlId].destroy()
				this.controls[controlId] = null
				delete this.controls[controlId]
			})
		}

		if (typeof this.model?.destroy === 'function')
		{
			this.model.destroy()
			this.model = null
		}
	},

	computed: {
		/**
		 * True if there are invalid rows, false otherwise.
		 */
		hasInvalidRows()
		{
			if (!Array.isArray(this.controls?.menu?.rows))
				return false
			return this.controls.menu.rows.filter((row) => !this.controls.menu.config.rowValidation.fnValidate(row)).length !== 0
		}
	},

	methods: {
		/**
		 * Called before navigating to a different route.
		 * @param {function} next The function that invokes the route redirect
		 */
		onBeforeRouteLeave(next)
		{
			genericFunctions.setNavigationState(false)
			next()
		},

		/**
		 * Fetches the data of the menu list from the server.
		 * @returns A promise to be resolved after the request completes.
		 */
		loadList()
		{
			return this.controls.menu.reload()
		},

		/**
		 * Sets the menu's table name in the nav properties.
		 */
		setMenuNavProperties()
		{
			const tableName = this.menuInfo.designation
			const navProps = {
				navigationId: this.navigationId,
				properties: {
					tableName: tableName
				}
			}
			this.setNavProperties(navProps)
		},

		/**
		 * Changes the menus's status.
		 * When a PopUp form is opened, the menus behind it should be marked as inactive.
		 * @param {Boolean} isActive Indicates whether the menu is currently active.
		 */
		changeMenuActiveState(isActive)
		{
			this.isActiveMenu = isActive
		},

		/**
		 * Applies the filters model to the list and reloads it.
		 * @param {object} model The filters form view model
		 */
		applyFilters(model)
		{
			this.controls.menu.updateGlobalFilters(model)
		}
	},

	watch: {
		'menuInfo.designation'()
		{
			this.setMenuNavProperties()
		},

		hasInvalidRows(val)
		{
			// If there are invalid rows, shows a warning message.
			if (val)
			{
				const warningProps = {
					type: messageTypes.W,
					message: hardcodedTexts.invalidRowsMsg,
					icon: 'error',
					dismissTime: 0,
					isResource: true
				}
				this.setInfoMessage(warningProps)
			}
		}
	}
}
