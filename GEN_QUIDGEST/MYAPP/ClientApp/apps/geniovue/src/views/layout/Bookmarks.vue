<template>
	<div
		v-if="$app.layout.BookmarkEnable && userIsLoggedIn"
		class="bookmarks__container">
		<ul class="nav">
			<li
				ref="menuContainer"
				class="dropdown"
				@focusout="onFocusoutMenu">
				<a
					ref="menuButton"
					class="bookmarks__header"
					role="button"
					href="#"
					:aria-expanded="bookmarkMenuIsOpen"
					:title="texts.favorites"
					@click.stop.prevent="toggleMenu"
					@keyup="menuItemKeyup">
					<q-icon icon="bookmark" />
				</a>

				<bookmarks-content
					:classes="['dropdown-menu', { 'show': bookmarkMenuIsOpen }, 'bookmarks__content']"
					@menu-action="setBookmarkMenuState(false)"
					@add="setBookmarkMenuState(false)"
					@remove="focusItem"
					@keyup="menuItemKeyup" />
			</li>
		</ul>
	</div>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import BookmarksContent from '@/views/shared/BookmarksContent.vue'

	export default {
		name: 'QBookmarks',

		emits: ['open-menu'],

		components: {
			BookmarksContent
		},

		mixins: [
			LayoutHandlers
		],

		expose: [],

		data()
		{
			return {
				texts: {
					favorites: computed(() => this.Resources[hardcodedTexts.favorites])
				}
			}
		},

		methods: {
			/**
			 * Called when focusing away from the bookmarks button or dropdown.
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
				this.setBookmarkMenuState(false)
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
				this.setBookmarkMenuState(false)
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
				//Toggle bookmarks menu
				this.toggleBookmarksMenu()

				//Signal if opening
				if(this.bookmarkMenuIsOpen)
					this.$emit('open-menu')
			}
		}
	}
</script>
