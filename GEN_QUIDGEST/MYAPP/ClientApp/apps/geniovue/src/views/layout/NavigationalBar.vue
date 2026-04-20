<template>
	<div
		v-if="$app.layout.HeaderEnable || mobileLayoutActive"
		:class="headerClasses"
		ref="topHeader">
		<q-button
			v-if="mobileLayoutActive"
			id="main-menu-toggle"
			:title="texts.mainMenu"
			:aria-expanded="collapsibleNavbarState === 'open'"
			:tabindex="defaultTabindex"
			@click="toggleSidebar">
			<q-icon-svg icon="menu-hamburger" />
		</q-button>
		<div
			v-if="$app.layout.LogoEnable && $app.layout.HeaderEnable"
			class="c-header__brand float-left">
			<q-router-link
				link="/"
				:tabindex="defaultTabindex">
				<img
					:src="`${$app.resourcesPath}logotipo_header.png?v=${$app.genio.buildVersion}`"
					:alt="texts.initialPage" />
			</q-router-link>
		</div>

		<div
			class="custom-header-text"
			v-if="$app.layout.MenuStyle === 'double_navbar' && !isEmpty(headerText)">
			<q-static-text
				supports-html
				:text="headerText" />
		</div>

		<div class="c-header__content">
			<q-select
				v-if="userIsLoggedIn && system.availableSystems.length > 1"
				id="system-years"
				:model-value="system.currentSystem"
				size="fit-content"
				:items="availableSystems"
				:groups="availableSystemsGroups"
				:aria-label="texts.systemYears"
				@update:model-value="selectSystem">
				<template #prepend>
					<q-icon icon="system-choice" />
				</template>
			</q-select>
			<q-tooltip
				anchor="#system-years"
				placement="bottom"
				:text="texts.systemYears" />

			<language-items
				v-if="$app.layout.LanguagePlacement === 'in_header'"
				:tabindex="defaultTabindex" />

			<div class="embeddedmenu__container">
				<embedded-menu
					v-if="$app.layout.LogonPlacement === 'in_header' || mobileLayoutActive"
					:tabindex="defaultTabindex" />
			</div>
		</div>
	</div>

	<nav
		id="main-header-navbar"
		:class="[...navbarClasses, 'navbar', 'navbar-expand-md', 'navbar-dark']"
		ref="navbar"
		@focusout="onFocusOut">
		<div :class="[containerClasses, { 'n-menu__navbar--double-l1': hasDoubleNavbar }]">
			<q-router-link
				v-if="$app.layout.BrandIconEnable"
				class="navbar-brand"
				link="/">
				<img
					:src="`${$app.resourcesPath}Q_icon.png?v=${$app.genio.buildVersion}`"
					:alt="texts.initialPage"
					width="30"
					height="30" />
			</q-router-link>

			<bookmarks @open-menu="closeNavigationMenus" />

			<modules @open-menu="closeNavigationMenus" />

			<div
				id="collapsible-navbar"
				:class="collapsibleNavbarClasses"
				ref="collapsibleNavbar">
				<q-line-loader v-if="loadingMenus" />
				<q-menu
					v-else
					ref="navbarMenu"
					:second-level-menu="hasDoubleNavbar"
					@change-menu="changeMenu" />
			</div>

			<div
				v-if="(!mobileLayoutActive && $app.layout.LogonPlacement === 'in_navmenu') || !$app.layout.HeaderEnable"
				class="navmenu__container">
				<embedded-menu />
			</div>
		</div>

		<div
			v-if="hasDoubleNavbar"
			class="n-menu__navbar--double-l2">
			<menu-sub-items
				v-for="child in childrenMenus.Children"
				:key="child.Id"
				ref="menuSubItem"
				root
				:menu="child"
				:module="system.currentModule"
				:level="1"
				:second-level-menu="hasDoubleNavbar"
				:tabindex="getNavItemTabindex(child, hasDoubleNavbar)" />
		</div>
	</nav>

	<!-- BEGIN: Mobile menu -->
	<div id="flat-menu-container">
		<div id="flat-menu-content">
			<menu-flat
				:key="userData.name"
				:loading="loadingMenus" />
		</div>
	</div>
	<!-- END: Mobile menu -->
</template>

<script>
	import { mapActions } from 'pinia'
	import { computed, defineAsyncComponent } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import { updateMainConfig } from '@/utils/system'
	import { useSystemDataStore } from '@quidgest/clientapp/stores'

	import Bookmarks from './Bookmarks.vue'
	import QMenu from './Menu.vue'
	import MenuSubItems from './MenuSubItems.vue'
	import Modules from './Modules.vue'

	import MenuFlat from './mobile/MenuFlat.vue'

	import EmbeddedMenu from '@/views/shared/EmbeddedMenu.vue'
	import QRouterLink from '@/views/shared/QRouterLink.vue'

	export default {
		name: 'QNavigationBar',

		components: {
			LanguageItems: defineAsyncComponent(() => import('@/views/shared/LanguageItems.vue')),
			EmbeddedMenu,
			QRouterLink,
			Bookmarks,
			Modules,
			QMenu,
			MenuSubItems,
			MenuFlat
		},

		mixins: [LayoutHandlers],

		props: {
			/**
			 * Whether or not the menu structure is loading.
			 */
			loadingMenus: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				showLogin: false,

				texts: {
					initialPage: computed(() => this.Resources[hardcodedTexts.initialPage]),
					systemYears: computed(() => this.Resources[hardcodedTexts.systemYears]),
					mainMenu: computed(() => this.Resources[hardcodedTexts.mainMenu])
				},

				navbar: '',

				header: '',

				headerResizeObserver: new ResizeObserver(() => {
					this.updateHeaderValues()
				})
			}
		},

		mounted()
		{
			this.setListeners()

			// Update header and navbar height values
			this.$nextTick().then(() => {
				this.updateHeaderValues()

				this.setObservedHeaderElements()

				// Update whether header and navbar size changes are reacted to when window is resized
				window.addEventListener('resize', this.setObservedHeaderElements)
			})
		},

		beforeUnmount()
		{
			this.removeListeners()

			window.removeEventListener('resize', this.setObservedHeaderElements)
		},

		computed: {
			headerClasses()
			{
				const classes = ['border-bottom', 'c-header']

				// When header is not enabled, apply styles like navbar on mobile
				if (!this.$app.layout.HeaderEnable && this.mobileLayoutActive)
					classes.push('c-header__disable')

				if (this.hasDoubleNavbar)
					classes.push('c-header--double-navbar')

				return classes
			},

			navbarClasses()
			{
				const classes = []

				if (this.hasDoubleNavbar)
					classes.push('n-menu__navbar--double')

				return classes
			},

			/**
			 * Classes for collapsible navbar. Must have the class corresponding to it's state
			 * so it displays the right way. (closed, opening, open, closing)
			 */
			collapsibleNavbarClasses()
			{
				const classes = []

				classes.push(this.collapsibleNavbarState)

				return classes
			},

			hasHeader()
			{
				return this.$app.layout.HeaderEnable
			},

			/**
			 * Tabindex value for all action items in the nav bar.
			 * Should only be used in the double nav bar.
			 */
			defaultTabindex()
			{
				if (this.hasDoubleNavbar)
					return 1
				return null
			},

			availableSystems()
			{
				return (this.system.availableSystems || []).map((availableSystem) => ({
					key: availableSystem,
					value: availableSystem.toString(),
					disabled: this.system.currentSystem === availableSystem,
					group: availableSystem
				}))
			},

			availableSystemsGroups()
			{
				return (this.system.availableSystems || []).map((availableSystem) => ({
					id: availableSystem
				}))
			}
		},

		methods: {
			...mapActions(useSystemDataStore, [
				'setCurrentSystem'
			]),

			/**
			 * Change the system (year) in the store and redirect to the home page.
			 * We should not preserve the current page because the user may not even have access to it on the other system.
			 * @param {String} selectedSystem Selected system
			 */
			selectSystem(selectedSystem)
			{
				this.setCurrentSystem(selectedSystem)
				// Before opening the home page, we must update the configuration to have the menu list for this system.
				updateMainConfig(() => {
					this.$router.push({
						name: 'home',
						params: {
							culture: this.system.currentLang,
							system: selectedSystem,
							module: this.system.currentModule
						}
					})
				})
			},

			changeMenu(menu)
			{
				this.setChildrenMenus(menu)
			},

			/**
			 * Close navigation menus
			 */
			closeNavigationMenus()
			{
				this.$refs?.navbarMenu?.closeMenu()
			},

			/**
			 * Called when focus leaves an element in the navbar
			 */
			onFocusOut(event)
			{
				// Main navbar element
				const navbar = this.$refs?.navbar
				// Element that gets focus
				const focusedElem = event?.relatedTarget
				// If focus goes to an element within the navbar, logically the 'focus' is on the navbar
				if (navbar.contains(focusedElem))
					return

				// Focus went to an element outside the navbar
				// If double nav bar, close sub menus
				if (this.hasDoubleNavbar)
				{
					for (const key in this.$refs.menuSubItem)
					{
						const curMenuComponent = this.$refs?.menuSubItem[key]
						curMenuComponent.closeMenu()
					}
				}
				// If normal nav bar, close menus
				else
					this.closeNavigationMenus()
			},

			/**
			 * Update the header and navbar height values
			 */
			updateHeaderValues()
			{
				this.header = this.$refs.topHeader
				this.navbar = this.$refs.navbar

				// Calculate header height
				const topHeaderHeight = this.header ? this.header.offsetHeight : 0
				const navbarHeight = this.navbar ? this.navbar.offsetHeight : 0
				const totalHeaderHeight = topHeaderHeight + navbarHeight

				// Set properties and CSS variables
				this.setHeaderHeightValues(totalHeaderHeight)

				if (this.navbar)
					this.setNavbarValues(navbarHeight)
			},

			/**
			 * Set whether the header and navbar elements are observed to react to size changes
			 */
			setObservedHeaderElements()
			{
				this.header = this.$refs.topHeader
				this.navbar = this.$refs.navbar

				// Remove observers
				this.headerResizeObserver.disconnect()

				// Set observers on the elements that exist
				if (this.header)
					this.headerResizeObserver.observe(this.header)
				if (this.navbar)
					this.headerResizeObserver.observe(this.navbar)
			}
		},

		watch: {
			visibleHeaderHeight: {
				handler(val)
				{
					document.documentElement.style.setProperty('--visible-header-height', val)
				},
				immediate: true
			},

			// Update the double navbar when the menu list changes, so the first menu is shown
			'menus.MenuList': {
				handler(newVal)
				{
					if (!this.hasDoubleNavbar)
						return

					if (!this.isEmpty(newVal))
					{
						const menu = this.menus.MenuList[0]
						this.setChildrenMenus(menu)
						this.setMenuPath(menu.Order)
					}
					else
					{
						this.setChildrenMenus({})
						this.clearMenuPath()
					}
				},
				immediate: true,
				deep: true
			}
		}
	}
</script>
