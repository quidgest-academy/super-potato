<template>
	<div
		v-if="$app.layout.BookmarkEnable && userIsLoggedIn"
		class="n-sidebar__section bookmarks__container">
		<ul
			id="bookmarks-tree-view"
			class="nav nav-pills nav-sidebar n-sidebar__nav d-block">
			<li :class="[{ 'menu-open': bookmarkMenuIsOpen }, 'nav-item', 'n-sidebar__nav-item', 'has-treeview']">
				<a
					ref="menuButton"
					id="bookmarks__toggle"
					role="button"
					href="#"
					:class="['nav-link n-sidebar__nav-link', 'has-icon', 'bookmarks__menu-text']"
					@click.stop.prevent="toggleBookmarksMenu"
					@keyup="menuItemKeyup">
					<q-icon
						icon="favourites"
						class="nav-icon n-sidebar__icon" />

					<p>
						{{ texts.favorites }}
						<q-icon
							:icon="getFavoritesCollapseStateIcon"
							class="right" />
					</p>
				</a>

				<transition name="sidebar-dropdown">
					<bookmarks-content
						v-if="bookmarkMenuIsOpen"
						:classes="['d-block', 'nav', 'nav-treeview']"
						:show-titles="!sidebarIsCollapsed"
						@keyup="menuItemKeyup" />
				</transition>
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

		computed: {
			getFavoritesCollapseStateIcon() {
				return this.bookmarkMenuIsOpen ? "collapse" : "expand";
			}
		},

		methods: {
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
			}
		}
	}
</script>
