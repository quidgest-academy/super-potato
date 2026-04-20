<template>
	<menu-action
		ref="menuItem"
		:class="classes"
		:menu="menu"
		:show-sub-menu="showSubMenu"
		:sub-menu-id="subMenuId">
		<q-icon
			v-if="menu.Vector"
			:icon="menu.Vector" />
		<i
			v-else-if="menu.ImageVUE"
			class="nav-icon n-sidebar__icon q-icon section-header-icon icon-custom">
			<img
				height="14"
				:src="`${$app.resourcesPath}${menu.ImageVUE}?v=${$app.genio.buildVersion}`" />
		</i>
		<i
			v-else-if="menu.Font"
			:class="[menu.Font, 'nav-icon', 'n-sidebar__icon', 'q-icon', 'section-header-icon']">
		</i>
		<span
			v-else-if="menu.Sigla"
			class="e-badge--mini-menu">
			{{ menu.Sigla }}
		</span>

		<p>
			{{ Resources[menu.Title] }}

			<span
				v-if="!menu.HideMenuSum && menu.Count > 0"
				:title="menu.Count"
				class="e-badge e-badge--dark">
				<span aria-hidden="true">{{ menu.Count }}</span>
			</span>

			<q-icon
				v-if="menu.Children.length > 0"
				:icon="getMenuCollapseStateIcon"
				class="right" />
			<q-icon
				v-else-if="menuIsOpen(menu)"
				icon="ok"
				class="right" />
		</p>
	</menu-action>
</template>

<script>
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import MenuAction from '@/views/shared/MenuAction.vue'

	export default {
		name: 'QMenuLink',

		components: {
			MenuAction
		},

		mixins: [
			LayoutHandlers
		],

		props: {
			/**
			 * The menu object containing information about the menu action, icon, counts, children, and other properties.
			 */
			menu: {
				type: Object,
				required: true
			},

			/**
			 * Determines if the menu action belongs to the first level, affecting its styles and behavior.
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
		},

		expose: ['focusSubMenuToggle'],

		computed: {
			/**
			 * Determines the classes to apply to the menu action based on its level, action, and other properties.
			 */
			classes()
			{
				const classes = []

				if (this.menu.Action && !this.menu.Action.includes('Menu') && this.menu.TreeLevel > 1)
					classes.push('dropdown-submenu')
				else if (this.firstLevel ||
					this.menu.Action && this.menu.Action.includes('Menu') && this.menu.TreeLevel === 1 ||
					this.menu.TreeLevel > 1)
				{
					classes.push('nav-link')
					classes.push('n-sidebar__nav-link')
				}

				if (this.menu.Vector || this.menu.ImageVUE || this.menu.Font || this.menu.Sigla)
					classes.push('has-icon')

				if (this.menuIsOpen(this.menu))
					classes.push('n-sidebar__nav-link-selected')

				return classes
			},

			getMenuCollapseStateIcon() {
				return this.menuIsOpen(this.menu) ? "collapse" : "expand";
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
