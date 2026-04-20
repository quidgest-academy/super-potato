<template>
	<menu-action
		ref="menuItem"
		:class="classes"
		:menu="menu"
		:show-sub-menu="showSubMenu"
		:sub-menu-id="subMenuId"
		:first-sub-menu-as-link="firstSubMenuAsLink"
		@menu-action="(...args) => $emit('menu-action', ...args)"
		@toggle-menu="(...args) => $emit('toggle-menu', ...args)"
		@keyup="(...args) => $emit('keyup', ...args)">
		<q-icon
			v-if="menu.Vector"
			:icon="menu.Vector" />
		<i
			v-else-if="menu.ImageVUE"
			class="nav-icon n-sidebar__icon section-header-icon icon-custom">
			<img
				height="14"
				:src="`${$app.resourcesPath}${menu.ImageVUE}`" />
		</i>
		<q-icon-font
			v-else-if="menu.Font"
			:icon="menu.Font"
			class="nav-icon n-sidebar__icon section-header-icon" />

		{{ Resources[menu.Title] }}

		<span
			v-if="menu.Count > 0 && !menu.HideMenuSum"
			class="e-badge e-badge--dark"
			:title="menu.Count">
			<span aria-hidden="true">{{ menu.Count }}</span>
		</span>
	</menu-action>
</template>

<script>
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import MenuAction from '@/views/shared/MenuAction.vue'

	export default {
		name: 'QMenuLink',

		emits: ['keyup', 'menu-action', 'toggle-menu'],

		components: {
			MenuAction
		},

		mixins: [
			LayoutHandlers
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
			 * Flag indicating if this menu link is part of the first level in the menu hierarchy.
			 */
			firstLevel: {
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
			},
		},

		expose: ['focusSubMenuToggle'],

		computed: {
			/**
			 * Calculates the appropriate CSS classes for the menu link based on various properties of the menu object.
			 */
			classes()
			{
				const classes = []

				if (this.firstLevel && this.menu.Children.length > 0)
				{
					classes.push('nav-link')
					classes.push('dropdown-toggle')
				}

				if (this.menu.TreeLevel <= 2 && this.hasDoubleNavbar)
					classes.push('n-menu__link--double-navbar')

				if (this.menu.Action)
				{
					if (this.menu.Action.includes('Menu'))
					{
						if (this.menu.TreeLevel === 1 && !classes.includes('nav-link'))
							classes.push('nav-link')
						else
						{
							if (this.hasDoubleNavbar)
								classes.push('n-menu__link')

							classes.push('dropdown-item')
						}
					}
					else if (this.menu.TreeLevel > 1)
						classes.push('dropdown-submenu')
				}
				else if (this.menu.TreeLevel > 1)
					classes.push('dropdown-item')

				return classes
			}
		},

		methods: {
			/*
			 * Focus on the menu item element
			 */
			focusSubMenuToggle()
			{
				this.$refs?.menuItem?.focusSubMenuToggle()
			}
		}
	}
</script>
