<template>
	<a
		v-if="hasLink || hasRoutine"
		:class="$attrs.class"
		:href="getLinkToMenu(menu)"
		:aria-current="isCurrentPage ? 'page' : null"
		:tabindex="$attrs.tabindex"
		@click.prevent="menuNavigation"
		@keyup="$emit('keyup', $event)">
		<slot></slot>
	</a>
	<a
		v-if="hasChildren && hasSubMenuToggle"
		ref="subMenuItem"
		:class="[hasLink ? 'dropdown-toggle' : $attrs.class]"
		role="button"
		:aria-controls="subMenuId"
		:aria-expanded="showSubMenu"
		:tabindex="$attrs.tabindex ?? 0"
		@click="menuToggle"
		@keydown.enter="menuToggle"
		@keyup="$emit('keyup', $event)">
		<slot v-if="!hasLink && !hasRoutine"></slot>
	</a>
</template>

<script>
	import VueNavigation from '@/mixins/vueNavigation.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import menuAction from '@/mixins/menuAction.js'

	export default {
		name: 'QMenuAction',

		inheritAttrs: false,

		emits: ['keyup', 'menu-action', 'toggle-menu'],

		mixins: [
			VueNavigation,
			LayoutHandlers,
			menuAction
		],

		props: {
			/**
			 * Menu object containing action information and child menu items.
			 */
			menu: {
				type: Object,
				required: true
			},

			/**
			 * Description text for the menu action, to be used for accessibility or display purposes.
			 */
			description: {
				type: String,
				default: ''
			},

			/**
			 * Flag indicating if there should be a button to toggle showing the sub-menu.
			 */
			hasSubMenuToggle: {
				type: Boolean,
				default: true
			},

			/**
			 * Flag indicating if the sub-menu is visible.
			 */
			showSubMenu: {
				type: Boolean,
				default: false
			},

			/**
			 * Sub-menu ID.
			 */
			subMenuId: {
				type: String,
				default: ''
			},

			/**
			 * Flag indicating if the first sub-menu item should also be the main link.
			 */
			firstSubMenuAsLink: {
				type: Boolean,
				default: false
			}
		},

		expose: ['focusSubMenuToggle'],

		computed: {
			/**
			 * Whether this menu item is a link.
			 */
			hasLink()
			{
				let link = this.menu.RouteName

				if (this.firstSubMenuAsLink)
					link = this.menu?.Children[0]?.RouteName

				return link !== undefined && link !== null
			},

			/**
			 * Whether this menu item has a routine.
			 */
			hasRoutine()
			{
				return this.menu.Action === 'GenGenio.MenuRotinaManual'
			},

			/**
			 * Whether this menu item had sub items.
			 */
			hasChildren()
			{
				return Array.isArray(this.menu.Children) && this.menu.Children.length > 0
			},

			/**
			 * Whether this menu item link is the current page.
			 */
			isCurrentPage()
			{
				return this.menu.RouteName === this.$route.name
			}
		},

		methods: {
			/**
			 * Handles navigation when the menu item is clicked.
			 * Executes menu action only if there are no child menu items to display, or if the checkChildren flag is false.
			 */
			menuNavigation(event)
			{
				this.$emit('menu-action', event)

				this.executeMenuAction(this.menu)
			},

			/*
			 * Signal to toggle dropdown menu
			 */
			menuToggle(event)
			{
				this.$emit('toggle-menu', event)
			},

			/*
			 * Focus on the menu item element
			 */
			focusSubMenuToggle()
			{
				const subMenuItem = this.$refs?.subMenuItem
				if (typeof subMenuItem?.focus === 'function')
					subMenuItem?.focus()
			}
		}
	}
</script>
