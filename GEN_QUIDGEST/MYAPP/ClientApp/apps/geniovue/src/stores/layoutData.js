/*****************************************************************
 *                                                               *
 * This store holds data specific for the horizontal layout,     *
 * also defining functions to access and mutate it.              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
		layoutType: 'horizontal',

		navbarHeight: 0,

		collapsibleNavbarState: 'closed',

		headerText: '',

		// Used to render the second level of menus in the double_navbar menu.
		childrenMenus: {},

		sidebarIsCollapsed: true,

		sidebarIsVisible: false,

		navBarIsVisible: false,

		isAccordionMenu: false,
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
	/**
	 * Sets the current height of the header.
	 * @param {number} height The current height of the header (in pixels)
	 */
	setHeaderHeight(height)
	{
		if (typeof height !== 'number')
			return

		this.headerHeight = height
	},

	/**
	 * Gets the state of the collapsible navbar.
	 */
	getCollapsibleNavbarState()
	{
		return this.collapsibleNavbarState
	},

	/**
	 * Sets the state of the collapsible navbar.
	 * @param {string} state The state of the collapsible navbar
	 */
	setCollapsibleNavbarState(state)
	{
		if (typeof state !== 'string' || !(['closed', 'opening', 'open', 'closing'].includes(state)))
			return

		this.collapsibleNavbarState = state
	},

	/**
	 * Sets the current height of the navbar.
	 * @param {number} height The current height of the navbar (in pixels)
	 */
	setNavbarHeight(height)
	{
		if (typeof height !== 'number')
			return

		this.navbarHeight = height
	},

	/**
	 * Sets the children of the given menu to render in the second level of the double_navbar menu.
	 * @param {object} menu The current menu to render it's children
	 */
	setChildrenMenus(menu)
	{
		if (typeof menu !== 'object')
			return

		this.childrenMenus = menu
	},

	/**
	 * Sets a custom text for the header
	 * @param {string} text The text to set
	 */
	setHeaderText(text)
	{
		this.headerText = text
	},

	/**
	 * Sets the collapse state of the mobile layout sidebar.
	 * @param {boolean} isCollapsed Whether or not the sidebar is collapsed
	 */
	setSidebarCollapseState(isCollapsed)
	{
		if (typeof isCollapsed !== 'boolean')
			return

		this.sidebarIsCollapsed = isCollapsed
	},

	/**
	 * Sets the visibility of the mobile layout sidebar.
	 * This value is updated right away when expanding and collapsing,
	 * so it's more like the state that the sidebar should be in / is going to.
	 * When collapsing, it will be false before the sidebar is actually invisible.
	 * @param {boolean} isVisible Whether or not the sidebar is visible
	 */
	setSidebarVisibility(isVisible)
	{
		if (typeof isVisible !== 'boolean')
			return

		this.sidebarIsVisible = isVisible

		//If true, the value for navBarIsVisible must also change to true right away
		if(this.sidebarIsVisible)
			this.setNavBarVisibility(isVisible)
	},

	/**
	 * Sets the visibility of the mobile layout navigation bar.
	 * This is used to indicate the actual visibility in real-time.
	 * This is needed because, with transitions, the visibility should
	 * not be changed to hidden until the transition finishes.
	 * @param {boolean} isVisible Whether or not the sidebar is visible
	 */
	setNavBarVisibility(isVisible)
	{
		if (typeof isVisible !== 'boolean')
			return

		this.navBarIsVisible = isVisible
	},

	/**
	 * Resets the layout info.
	 */
	resetStore()
	{
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

export const useLayoutDataStore = defineStore('layoutData', {
	state,
	actions
})
