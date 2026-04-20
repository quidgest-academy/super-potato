<template>
	<aside
		v-if="moduleCount > 0"
		:class="classes"
		@transitionend="onTransitionEnd"
		id="sidebarMenu"
		ref="sidebarMenu"
		@focusout="onFocusOut"
		tabindex="-1">
		<div
			class="sidebar n-sidebar"
			tabindex="-1">
			<nav>
				<menu-search-box />
			</nav>

			<bookmarks />

			<modules />

			<div class="n-sidebar__section">
				<div
					v-if="loading"
					class="n-sidebar__section--loading">
					<div
						v-for="id in skeletonLoaders"
						:key="id"
						class="n-sidebar__section-menu--loading">
						<q-skeleton-loader
							type="icon"
							:style="{ opacity: getSkeletonLoaderOpacity(id) }" />
						<q-skeleton-loader
							type="text"
							:style="getSkeletonLoaderStyle(id)" />
					</div>
				</div>
				<ul
					v-else-if="!isEmpty(menus) && !isEmpty(menus.MenuList)"
					id="menu-tree-view"
					:key="system.currentModule"
					class="nav nav-pills nav-sidebar n-sidebar__nav d-block">
					<menu-sub-items
						v-for="menu in menus.MenuList"
						:key="menu.Id"
						:menu="menu"
						:module="system.currentModule"
						:level="0" />
				</ul>
			</div>
		</div>
	</aside>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import Bookmarks from './Bookmarks.vue'
	import MenuSearchBox from './MenuSearchBox.vue'
	import MenuSubItems from './MenuSubItems.vue'
	import Modules from './Modules.vue'

	const DEFAULT_SKELETON_LOADERS = 10

	export default {
		name: 'QMenu',

		components: {
			MenuSearchBox,
			Bookmarks,
			Modules,
			MenuSubItems
		},

		mixins: [LayoutHandlers],

		props: {
			/**
			 * Whether or not the menu structure is loading.
			 */
			loading: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				skeletonLoaders: DEFAULT_SKELETON_LOADERS,

				texts: {
					appName: computed(() => this.Resources[hardcodedTexts.appName]),
					initialPage: computed(() => this.Resources[hardcodedTexts.initialPage]),
					moduleItems: computed(() => this.Resources[hardcodedTexts.moduleItems])
				}
			}
		},

		computed: {
			menuColor()
			{
				return this.$app.layout.MenuBackgroundColor === 'light' ? 'sidebar-light' : ''
			},

			moduleCount()
			{
				return Object.keys(this.system.availableModules).length
			},

			classes()
			{
				const classes = ['main-sidebar', 'n-menu--sidebar', this.menuColor]

				if(!this.navBarIsVisible)
					classes.push('invisible')

				return classes
			}
		},

		methods: {
			getSkeletonLoaderStyle(order)
			{
				// The width is randomized to produce the effect
				// of menus with titles of different sizes being loaded
				const minWidth = 33
				const maxWidth = 80
				const widthPercentage = Math.floor(Math.random() * (maxWidth - minWidth + 1) + minWidth) + '%'

				return {
					width: widthPercentage,
					opacity: this.getSkeletonLoaderOpacity(order)
				}
			},

			getSkeletonLoaderOpacity(order)
			{
				return (this.skeletonLoaders - order + 1) / this.skeletonLoaders
			},

			/**
			 * Called when a CSS transition for the nav bar finishes.
			 * This is called for every element inside the nav bar with a transition as well.
			 */
			onTransitionEnd(event)
			{
				// Only execute the specific functions for the elements we want
				if (event?.target?.id === "sidebarMenu")
					this.handleSidebarTransitionEnd()
			},

			/**
			 * Does the required operations to handle the end of the navbar opening or closing transition.
			 */
			handleSidebarTransitionEnd()
			{
				/**
				 * If the nav bar is being closed, set the actual value for visibility to false.
				 * It must be done here, after the transition ends
				 * so it doesn't disappear before the transition is done.
				 */
				if (!this.sidebarIsVisible)
					this.setNavBarVisibility(false)
				else
					//give focus to the sidebar so ensure it is closed if the user clicks outside of it on mobile
					this.$refs.sidebarMenu.focus();
			},

			/*
			 * Called when focus leaves an element in the menu
			 */
			onFocusOut(event) {
				if (this.mobileLayoutActive) {
					// Main menu element
					const sidebarMenu = this.$refs?.sidebarMenu
					// Element that gets focus
					const focusedElem = event?.relatedTarget

					// If focus goes to an element within the menu, logically the 'focus' is on the menu
					if (sidebarMenu.contains(focusedElem))
						return

					// If focus goes to the button to close the menu, let the click event handle it
					if (event?.relatedTarget?.id === 'main-menu-toggle')
						return

					// Focus went to an element outside the menu
					this.collapseSidebar();
				}
			},
		}
	}
</script>
