import _isEmpty from 'lodash-es/isEmpty'
import { mapActions, mapState } from 'pinia'

import {
	useGenericDataStore,
	useGenericLayoutDataStore,
	useSystemDataStore,
	useUserDataStore
} from '@quidgest/clientapp/stores'
import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

import { loadResources } from '@/plugins/i18n.js'
import { systemInfo } from '@/systemInfo'

/***************************************************************************
 * Mixin with handlers to be reused by both layouts.                       *
 ***************************************************************************/
export default {
	created()
	{
		loadResources(this, ['QLayout'])
	},

	data()
	{
		return {
			config: {
				QAEnvironment: 0,
				LoginType: 'NORMAL'
			},

			options: {
				autoCollapseSize: 992
			}
		}
	},

	computed: {
		...mapState(useSystemDataStore, [
			'maintenance',
			'system'
		]),

		...mapState(useGenericLayoutDataStore, [
			'headerHeight',
			'layoutConfig',
			'pageScroll',
			'progressBar',
			'bookmarkMenuIsOpen',
			'moduleMenuIsOpen',
			'rightSidebarIsCollapsed',
			'rightSidebarIsVisible',
			'mobileLayoutActive'
		]),

		...mapState(useGenericDataStore, [
			'maintenance',
			'hasMenus',
			'menus',
			'menuPath',
			'isPublicRoute',
			'isFullScreenPage'
		]),

		...mapState(useUserDataStore, [
			'userIsLoggedIn'
		]),

		/**
		 * The title of the selected module.
		 */
		currentModuleTitle()
		{
			const currentModId = this.system.currentModule

			if (this.system.availableModules[currentModId])
			{
				const moduleNameId = this.system.availableModules[currentModId].title
				return this.Resources[moduleNameId]
			}

			return currentModId
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
		 * The margin the main content gets from the sides of the page.
		 */
		containerClasses()
		{
			return systemInfo.layout.ContainerWidth === 'reduced' ? 'container' : 'container-fluid'
		},

		authenticationClasses()
		{
			if (systemInfo.layout.AuthenticationStyle === 'default')
				return []

			return [`layout-${systemInfo.layout.AuthenticationStyle}`]
		},

		maintenanceDate() {
			const genericDataStore = useGenericDataStore()
			return genericFunctions.dateDisplay(this.maintenance.schedule, genericDataStore.dateFormat.dateTimeSeconds)
		},
	},

	methods: {
		...mapActions(useGenericLayoutDataStore, [
			'setHeaderHeight',
			'setBookmarkMenuState',
			'setModuleMenuState',
			'setRightSidebarCollapseState',
			'setRightSidebarVisibility',
			'setPageScroll',
			'updateLayoutMobileState'
		]),

		...mapActions(useGenericDataStore, [
			'addToMenuPath',
			'removeFromMenuPath',
			'clearMenuPath'
		]),

		isEmpty: _isEmpty,

		/**
		 * Sets the current page scroll.
		 */
		updatePageScroll()
		{
			this.setPageScroll(window.scrollY)
		},

		/**
		 * Sets the current menu path, according to the specified menu id.
		 * @param {string} menuId The id of the menu
		 */
		setMenuPath(menuId)
		{
			if (typeof menuId !== 'string')
				return

			this.clearMenuPath()

			for (let i = 0; i < menuId.length; i++)
			{
				const id = menuId.substring(0, i + 1)
				this.addToMenuPath(id)
			}
		},

		/**
		 * Checks if the specified dropdown menu is open.
		 * @param {object} menu The menu to check
		 * @returns True if the dropdown menu is open, false otherwise.
		 */
		menuIsOpen(menu)
		{
			return this.menuPath.includes(menu.Order)
		},

		/**
		 * Gets the icon for the given menu.
		 * @param {object} menuEntry The menu to check the icon
		 * @returns An object with the icon and the type.
		 */
		getMenuIcon(menuEntry)
		{
			const data = {
				icon: '',
				type: 'svg'
			}

			if (menuEntry.Vector)
				data.icon = menuEntry.Vector
			else if (menuEntry.Font)
			{
				data.icon = menuEntry.Font
				data.type = 'font'
			}
			else if (menuEntry.Image)
			{
				data.icon = menuEntry.Image
				data.type = 'img'
			}
			else
				return undefined

			return data
		},

		/**
		 * Toggles the bookmarks menu.
		 */
		toggleBookmarksMenu()
		{
			this.setBookmarkMenuState(!this.bookmarkMenuIsOpen)
		},

		/**
		 * Toggles the modules menu.
		 */
		toggleModulesMenu()
		{
			this.setModuleMenuState(!this.moduleMenuIsOpen)
		},

		/**
		 * Adds listeners common to all layouts.
		 */
		setGenericListeners()
		{
			window.addEventListener('resize', this.updateLayoutMobileState)
			window.addEventListener('scroll', this.updatePageScroll)
		},

		/**
		 * Removes listeners common to all layouts.
		 */
		removeGenericListeners()
		{
			window.removeEventListener('resize', this.updateLayoutMobileState)
			window.removeEventListener('scroll', this.updatePageScroll)
		}
	}
}
