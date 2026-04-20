<template>
	<div
		:class="[...layoutClasses, ...customClasses]"
		:data-loading="loading">
		<slot name="layout-loading-effect"></slot>

		<div
			v-if="this.maintenance.isScheduled || this.maintenance.isActive"
			class="q-maintenance-container">
			<span>
				<q-icon icon="alert" />
				{{ maintenanceMessage }}
			</span>
		</div>

		<template v-if="showContent">
<!-- eslint-disable indent, vue/html-indent, vue/script-indent -->
<!-- USE /[MANUAL FOR LAYOUT_HEADER]/ -->
<!-- eslint-disable-next-line -->
<!-- eslint-enable indent, vue/html-indent, vue/script-indent -->
			<navigational-bar :loading-menus="loadingMenus" />

			<slot name="layout-header"></slot>
		</template>

<!-- eslint-disable indent, vue/html-indent, vue/script-indent -->
<!-- USE /[MANUAL FOR LAYOUT_CONTENT]/ -->
<!-- eslint-disable-next-line -->
<!-- eslint-enable indent, vue/html-indent, vue/script-indent -->
		<slot name="layout-content"></slot>
	</div>
</template>

<script>
	import { defineAsyncComponent, computed } from 'vue'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import hardcodedTexts from '@/hardcodedTexts.js'

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR LAYOUT_INCLUDEJS LAYOUT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QLayout',

		components: {
			NavigationalBar: defineAsyncComponent(() => import('./NavigationalBar.vue'))
		},

		mixins: [
			LayoutHandlers
		],

		inheritAttrs: false,

		data() {
			const vm = this
			return {
				texts: {
					maintenanceActive: computed(() => this.Resources[hardcodedTexts.maintenanceActive]),
					maintenanceScheduled: computed(() => this.Resources[hardcodedTexts.maintenanceScheduled].replace('{0}', vm.maintenanceDate)),
				},
			}
		},

		props: {
			/**
			 * Custom classes to apply to the layout container.
			 */
			customClasses: {
				type: Array,
				default: () => []
			},

			/**
			 * Whether there's any asynchronous process running.
			 */
			loading: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the menu structure is loading.
			 */
			loadingMenus: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		computed: {
			/**
			 * True if the layout content should be visible, false otherwise.
			 */
			showContent()
			{
				return !this.isFullScreenPage && (this.userIsLoggedIn || this.isPublicRoute || this.$app.layout.LoginStyle !== 'single_page')
			},

			/**
			 * Classes used in the layout container.
			 */
			layoutClasses()
			{
				const classes = ['layout-container'];

				if (!this.showContent)
					classes.push('login-page')

				if (this.rightSidebarIsCollapsed)
					classes.push('right-sidebar-collapse')

				//< Mobile
				if (!this.sidebarIsVisible ||
					this.isFullScreenPage ||
					!this.userIsLoggedIn && !this.isPublicRoute && this.$app.layout.LoginStyle === 'single_page' ||
					Object.keys(this.system.availableModules).length === 0)
					classes.push('sidebar-closed')

				if (this.sidebarIsCollapsed)
					classes.push('sidebar-collapse')
				//> Mobile

				if (this.maintenance.isScheduled || this.maintenance.isActive)
					classes.push('maintenance')

				return classes
			},

			maintenanceMessage() {
				return this.maintenance.isScheduled ? this.texts.maintenanceScheduled : this.texts.maintenanceActive
			},
		},

		methods: {
			/**
			 * Sets a custom text that appears in the header.
			 */
			async setCustomHeaderText()
			{
				let text

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR LAYOUT_HEADER_TEXT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.setHeaderText(text)
			},

			/**
			 * Checks if the header text is ready to be shown.
			 */
			showHeaderText(userIsLoggedIn)
			{
				if (this.isEmpty(this.headerText) && this.$app.layout.MenuStyle === 'double_navbar' && userIsLoggedIn)
					this.setCustomHeaderText()
			},

			/**
			 * Collapse the mobile menu on bigger screen sizes.
			 */
			checkCollapseMobileMenu()
			{
				if(!this.mobileLayoutActive)
				{
					this.collapseSidebar()
					this.setSidebarVisibility(false)
				}
			}
		},

		mounted() {
			// Collapse mobile menu initially
			this.collapseSidebar()

			// Add resize handler to collapse mobile menu on bigger screen sizes
			window.addEventListener("resize", this.checkCollapseMobileMenu)
		},

		unmounted() {
			// Remove resize handler to collapse mobile menu on bigger screen sizes
			window.removeEventListener("resize", this.checkCollapseMobileMenu)
		},

		watch: {
			// If the user logged off and logged back in the custom text would disappear, this prevents that.
			headerText: {
				handler()
				{
					this.showHeaderText(this.userIsLoggedIn)
				},
				immediate: true
			},

			userIsLoggedIn: {
				handler(val)
				{
					this.showHeaderText(val)
				},
				immediate: true
			}
		}
	}
</script>
