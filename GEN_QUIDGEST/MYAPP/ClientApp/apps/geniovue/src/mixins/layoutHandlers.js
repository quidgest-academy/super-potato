import { mapState, mapActions } from 'pinia'

import { useLayoutDataStore } from '@/stores/layoutData.js'

import GenericLayoutHandler from '@/mixins/genericLayoutHandlers.js'

/***************************************************************************
 * Mixin with handlers to be reused by the horizontal layout components.   *
 ***************************************************************************/
export default {
	mixins: [
		GenericLayoutHandler
	],

	computed: {
		...mapState(useLayoutDataStore, [
			'layoutType',
			'collapsibleNavbarState',
			'headerHeight',
			'navbarHeight',
			'headerText',
			'childrenMenus',
			'sidebarIsCollapsed',
			'sidebarIsVisible',
			'navBarIsVisible',
			'stickyThreshold',
			'isAccordionMenu'
		]),

		/**
		 * True if the MenuStyle layout Variable is set to "double_navbar", false otherwise.
		 */
		hasDoubleNavbar()
		{
			return this.$app.layout.MenuStyle === 'double_navbar'
		},

		/**
		 * The visible vertical space (in pixels) occupied by this layout's header.
		 */
		visibleHeaderHeight()
		{
			const topDiff = this.headerHeight - this.pageScroll
			return topDiff > this.navbarHeight ? topDiff : this.navbarHeight
		},

		/**
		 * Gets the current module icon, to display in the mobile layout sidebar.
		 */
		currentModuleIcon()
		{
			const currentModId = this.system.currentModule
			const module = this.system.availableModules[currentModId]
			const data = {
				icon: 'menu-hamburger',
				type: 'svg'
			}

			if (module)
				return this.getModuleIconProps(module, data.icon)
			return data
		},
	},

	methods: {
		...mapActions(useLayoutDataStore, [
			'getCollapsibleNavbarState',
			'setCollapsibleNavbarState',
			'setHeaderHeight',
			'setNavbarHeight',
			'setHeaderText',
			'setChildrenMenus',
			// Mobile
			'setSidebarCollapseState',
			'setSidebarVisibility',
			'setNavBarVisibility'
		]),

		/**
		 * Sets the height for the "visible-header-height" CSS property.
		 * @param {number} height The current height (in pixels)
		 */
		setVisibleHeaderHeight(height)
		{
			document.documentElement.style.setProperty('--visible-header-height', height)
		},

		/**
		 * Sets the height for the "navbar-height" CSS property.
		 * @param {number} height The current height (in pixels)
		 */
		setNavbarHeightProp(height)
		{
			document.documentElement.style.setProperty('--navbar-height', height)
		},

		/**
		 * Sets the height for the "navbar-height" CSS property and the "headerHeight" property.
		 * @param {number} height The current height (in pixels)
		 */
		setHeaderHeightValues(height)
		{
			this.setHeaderHeight(height)
			this.setVisibleHeaderHeight(height)
		},

		/**
		 * Sets the height for the "navbar-height" CSS property and the "navbarHeight" property.
		 * @param {number} height The current height (in pixels)
		 */
		setNavbarValues(height)
		{
			this.setNavbarHeight(height)
			this.setNavbarHeightProp(height)
		},

		/**
		* Expands the mobile layout sidebar.
		*/
		expandSidebar() {
			this.setSidebarVisibility(true)
			this.setSidebarCollapseState(false)
			this.$eventHub.emit('toggle-sidebar', 'expand')
		},

		/**
		 * Collapses the mobile layout sidebar.
		 */
		collapseSidebar() {
			if (this.options.autoCollapseSize && window.innerWidth <= this.options.autoCollapseSize)
				this.setSidebarVisibility(false)

			this.setSidebarCollapseState(true)
			this.$eventHub.emit('toggle-sidebar', 'collapse')
		},

		/**
		 * Collapses the mobile layout sidebar when a certain screen size is reached.
		 * @param {boolean} resize Whether or not the window is being resized
		 */
		autoCollapseSidebar(resize = true) {
			if (resize && !this.options.autoCollapseSize)
				return

			if (this.options.autoCollapseSize && window.innerWidth <= this.options.autoCollapseSize)
				this.collapseSidebar()
			else
				this.setSidebarVisibility(true)
		},

		/**
		 * Toggles the mobile layout sidebar.
		 */
		toggleSidebar() {
			if (this.sidebarIsCollapsed)
				this.expandSidebar()
			else
				this.collapseSidebar()
		},

		/**
		 * Expands a dropdown menu in the mobile layout sidebar.
		 * @param {object} menu The menu
		 */
		expandDropdownMenu(menu) {
			// If the menu is an accordion, removes all entries outside the current branch.
			if (this.isAccordionMenu) {
				const otherMenus = []
				for (const menuId of this.menuPath)
					if (!menu.Order.startsWith(menuId))
						otherMenus.push(menuId)

				for (const menuId of otherMenus)
					this.removeFromMenuPath(menuId)

				this.removeFromMenuPath(menu.Order)
			}

			this.addToMenuPath(menu.Order)
		},

		/**
		 * Collapses a dropdown menu in the mobile layout sidebar.
		 * @param {object} menu The menu
		 */
		collapseDropdownMenu(menu) {
			this.removeFromMenuPath(menu.Order)
		},

		/**
		 * Toggles a dropdown menu in the mobile layout sidebar.
		 * @param {object} menu The menu
		 */
		toggleDropdownMenu(menu) {
			if (this.menuIsOpen(menu))
				this.collapseDropdownMenu(menu)
			else
				this.expandDropdownMenu(menu)
		},

		/**
		 * Gets the icon for the given module.
		 * @param {object} module The module
		 * @param {string} defaultIcon The default icon
		 * @returns The icon properties.
		 */
		getModuleIconProps(module, defaultIcon = '') {
			const data = {
				icon: defaultIcon,
				type: 'svg'
			}

			if (module.vector)
				data.icon = module.vector
			else if (module.font) {
				data.icon = module.font
				data.type = 'font'
			}
			else if (module.image) {
				data.icon = module.image
				data.type = 'img'
			}
			else if (this.isEmpty(defaultIcon))
				return undefined

			return data
		},

		/**
		 * Sets the necessary listeners to control the state of the layout.
		 */
		setListeners()
		{
			this.setGenericListeners()

			this.$eventHub.on('right-sidebar-open', this.collapseSidebar)
			this.$eventHub.on('user-options-menu-open', this.collapseSidebar)
		},

		/**
		 * Removes all the listeners.
		 */
		removeListeners()
		{
			this.removeGenericListeners()

			this.$eventHub.off('right-sidebar-open', this.collapseSidebar)
			this.$eventHub.off('user-options-menu-open', this.collapseSidebar)
		},

		/**
		 * Get the tabindex for an item in the navigation menu
		 * @param {object} menu The menu object
		 * @param {boolean} hasDoubleNavbar Whether the menu has a double navigation bar
		 * @returns integer
		 */
		getNavItemTabindex(menu, hasDoubleNavbar)
		{
			if(hasDoubleNavbar)
			{
				// Get character code for value of the top level menu order
				// Can be a number or letter
				const topOrder = menu?.Order?.charCodeAt(0)

				// Get numeric value based on the character code
				// Character code for a number
				// Subtract the lowest character code for a number to get a value from 1 to 9
				if(topOrder >= 48 && topOrder <= 57)
					return topOrder - 48
				// Character code for a lower case letter
				// Subtract the lowest character code for a lower case letter to get a value from 10 to 36
				else if(topOrder >= 65 && topOrder <= 90)
					return topOrder - 55
				// Character code for an upper case letter
				// Subtract the lowest character code for an upper case letter to get a value from 10 to 36
				else if(topOrder >= 97 && topOrder <= 122)
					return topOrder - 87
			}

			//Normal navigation bar or invalid order value
			return 0
		}
	}
}
