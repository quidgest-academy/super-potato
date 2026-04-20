<template>
	<li
		v-if="menu.Separates && !root"
		class="dropdown-divider" />

	<li
		:id="menuId"
		:class="menuClasses"
		@click="closeMenu"
		@focusout="onFocusout">
		<template v-if="!isEmpty(menu.Children)">
			<make-link
				ref="menuItem"
				:menu="menu"
				:first-level="level === 0"
				:show-sub-menu="showSubMenu"
				:sub-menu-id="subMenuId"
				:first-sub-menu-as-link="hasDoubleNavbar && level === 1"
				:tabindex="$attrs.tabindex"
				@menu-action="menuAction"
				@toggle-menu="toggleMenu"
				@keyup="menuItemKeyup" />

			<ul
				ref="dropdownMenu"
				:id="subMenuId"
				:class="['dropdown-menu', { 'show': showSubMenu }]"
				@focusout="onFocusout">
				<template
					v-for="child in menu.Children"
					:key="child.Id">
					<q-menu-sub-items
						v-if="child.Type === 'ITEM'"
						ref="menuSubItem"
						:menu="child"
						:level="level + 1"
						:root="false"
						:module="module"
						:tabindex="$attrs.tabindex"
						@close-parent-menu="closeMenuAndFocusItem" />
					<li v-else-if="child.Type === 'REPORT'">
						<make-link :menu="child" />
					</li>
					<component
						v-else-if="child.Type === 'LIST'"
						:is="getMenuList(child.Id)"
						:menu="child" />
				</template>
			</ul>
		</template>
		<template v-else>
			<make-link
				:menu="menu"
				:first-level="level === 0"
				:show-sub-menu="showSubMenu"
				:tabindex="$attrs.tabindex"
				@menu-action="closeParentMenu"
				@keyup="menuItemKeyup" />
		</template>
	</li>
</template>

<script>
	import { nextTick } from 'vue'

	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import VueNavigation from '@/mixins/vueNavigation.js'
	import menuAction from '@/mixins/menuAction.js'

	import MakeLink from './MakeLink.vue'

	export default {
		name: 'QMenuSubItems',

		emits: ['change-menu', 'close-parent-menu'],

		components: {
			MakeLink
		},

		mixins: [
			VueNavigation,
			LayoutHandlers,
			menuAction
		],

		props: {
			/**
			 * The menu object containing configuration and state data for the displayed menu link.
			 */
			menu: {
				type: Object,
				required: true
			},

			/**
			 * The module name to be used for dynamic component resolution and part of the ID for the menu item.
			 */
			module: {
				type: String,
				required: true
			},

			/**
			 * Indicates if this is the second-level menu. It affects behavior and styling.
			 */
			secondLevelMenu: {
				type: Boolean,
				default: false
			},

			/**
			 * The level of depth this menu item is at within the menu tree.
			 */
			level: {
				type: Number,
				default: 0
			},

			/**
			 * If true, indicates this is the root menu item.
			 */
			root: {
				type: Boolean,
				default: true
			}
		},

		expose: ['menu', 'openMenu', 'closeMenu', 'toggleMenu'],

		data()
		{
			return {
				showSubMenu: false,
				dropdownToLeft: false
			}
		},

		computed: {
			/**
			 * Computes the CSS class list for the menu LI element based on the menu's properties and state.
			 */
			menuClasses()
			{
				const classes = []

				if (this.isMenuOpen)
					classes.push('menu-selected')

				if (this.hasDoubleNavbar)
					classes.push('n-menu__item--double-navbar')

				if (this.menu.Children.length < 1)
					return classes

				if (this.root)
					classes.push('dropdown')
				else
					classes.push('dropdown-submenu')

				if (this.dropdownToLeft)
					classes.push('dropdown-submenu-left')

				return classes
			},

			isMenuOpen()
			{
				return this.menuIsOpen(this.menu)
			},

			menuId()
			{
				return this.module + this.menu.Id
			},

			subMenuId()
			{
				return this.menuId + '_SUBMENU'
			},
		},

		methods: {
			/**
			 * Handles the navigation and behavior when a menu item with children is clicked.
			 */
			openMenu()
			{
				if (this.secondLevelMenu && this.level === 0)
					this.openSingleMenu(this.menu)
				else
					this.showSubMenu = true

				nextTick().then(() => {
					this.dropdownToLeft = this.isSubmenuOutsideWindow(this.$refs.dropdownMenu)
				})
			},

			/**
			 * Opens a single menu and optionally executes the default action if it exists.
			 * @param {Object} menu - The menu item to open.
			 */
			openSingleMenu(menu)
			{
				this.$emit('change-menu', menu)
				this.setMenuPath(menu.Order)

				const defaultAction = menu.DefaultAction
				if (defaultAction)
					this.navigateToDefaultAction(defaultAction)
			},

			/**
			 * Closes the currently open submenu.
			 */
			closeMenu()
			{
				this.showSubMenu = false

				nextTick().then(() => {
					this.dropdownToLeft = this.isSubmenuOutsideWindow(this.$refs.dropdownMenu)
				})

				//Close all sub-menus
				for(const key in this.$refs.menuSubItem)
				{
					const curMenuComponent = this.$refs?.menuSubItem[key]
					curMenuComponent.closeMenu()
				}
			},

			/*
			 * Close sub-menu and focus on the element that opens and closes it
			 */
			closeMenuAndFocusItem()
			{
				this.closeMenu()
				this.$refs?.menuItem?.focusSubMenuToggle()
			},

			getMenuList(id)
			{
				return `QMenu${this.module}_${id}`
			},

			/*
			 * Toggle opening or closing a sub-menu
			 */
			toggleMenu(event)
			{
				if(!this.showSubMenu)
					this.openMenu()
				else
					this.closeMenu()

				event.stopPropagation()
				event.preventDefault()
			},

			/*
			 * Run menu action
			 */
			menuAction(event)
			{
				//Close parent menu
				this.closeParentMenu()

				if (this.secondLevelMenu && this.level > 0)
					this.executeMenuAction(this.menu.Children[0])

				event.stopPropagation()
				event.preventDefault()
			},

			/*
			 * Signal to close the menu that contains this item
			 */
			closeParentMenu()
			{
				this.$emit('close-parent-menu')
			},

			/*
			 * Called when pressing a key on any menu item
			 */
			menuItemKeyup(event)
			{
				const key = event?.key

				switch(key)
				{
					case 'Escape':
						if((this.secondLevelMenu && this.level === 1) || (!this.secondLevelMenu && this.level === 0))
							this.closeMenuAndFocusItem()
						else
							this.closeParentMenu()
						break;
					default:
						return
				}
			},

			/**
			 * Focusout handler for menu
			 * @param event {object} Event object
			 */
			onFocusout(event)
			{
				const focusedElem = event?.relatedTarget

				/**
				 * If focus went to the dropdown toggle element or an element in the dropdown menu,
				 * logically the focus is still on this menu
				 */
				if(focusedElem === this.$refs?.menuItem?.$refs?.menuItem?.$refs?.subMenuItem
					|| this.$refs?.dropdownMenu?.contains(focusedElem))
					return

				// If focus left this menu, close it
				this.closeMenu()
			},

			/**
			 * Checks if a container exceeds the window width
			 * @param container {object} The ref of the menu container
			 */
			isSubmenuOutsideWindow(container) {
				if (!container) return false;

				const rect = container.getBoundingClientRect();
				const windowWidth = window.innerWidth;

				return rect.right > windowWidth;
			},
		},

		watch: {
			isMenuOpen(newValue)
			{
				if (newValue && this.level === 0 && this.hasDoubleNavbar)
					this.$emit('change-menu', this.menu)
			}
		}
	}
</script>
