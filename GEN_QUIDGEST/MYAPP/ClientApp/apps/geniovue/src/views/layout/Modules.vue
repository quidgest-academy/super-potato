<template>
	<div class="modules__container">
		<ul
			v-if="Object.keys(system.availableModules).length > 1"
			class="nav">
			<li
				ref="menuContainer"
				class="dropdown"
				@focusout="onFocusoutMenu">
				<a
					ref="menuButton"
					:class="['brand', 'modules__header']"
					role="button"
					href="#"
					:aria-expanded="moduleMenuIsOpen"
					:data-key="system.currentModule"
					@click.stop.prevent="toggleMenu"
					@keyup="menuItemKeyup">
					<module-header />
				</a>

				<all-modules
					:class="['dropdown-menu', { 'show': moduleMenuIsOpen }]"
					@menu-action="setModuleMenuState(false)"
					@keyup="menuItemKeyup" />
			</li>
		</ul>
	</div>
</template>

<script>
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import ModuleHeader from './ModuleHeader.vue'
	import AllModules from './AllModules.vue'

	export default {
		name: 'QModules',

		emits: ['open-menu'],

		components: {
			ModuleHeader,
			AllModules
		},

		mixins: [
			LayoutHandlers
		],

		expose: [],

		methods: {
			/**
			 * Called when focusing away from the modules button or dropdown.
			 */
			onFocusoutMenu(event)
			{
				const menuContainer = this.$refs?.menuContainer
				const focusedElem = event?.relatedTarget
				//If the focus went to an element within the menu button or dropdown,
				//logically, the menu is still focused
				if(menuContainer.contains(focusedElem))
					return

				//Menu not focused. Close dropdown.
				this.setModuleMenuState(false)
			},

			/**
			 * Focus on the menu toggle button.
			 */
			focusItem()
			{
				//Focus on the menu toggle button
				this.$refs?.menuButton?.focus()
			},

			/**
			 * Close the menu and focus on the menu toggle button.
			 */
			closeMenuAndFocusItem()
			{
				//Focus on the menu toggle button
				this.focusItem()

				//Close dropdown
				this.setModuleMenuState(false)
			},

			/*
			 * Called when pressing a key on any menu item
			 */
			menuItemKeyup(event)
			{
				const key = event?.key

				if(key === 'Escape')
					this.closeMenuAndFocusItem()
			},

			/**
			 * Toggle the menu.
			 */
			toggleMenu()
			{
				//Toggle modules menu
				this.toggleModulesMenu()

				//Signal if opening
				if(this.moduleMenuIsOpen)
					this.$emit('open-menu')
			}
		}
	}
</script>
