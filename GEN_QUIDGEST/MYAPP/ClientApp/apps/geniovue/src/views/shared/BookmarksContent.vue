<template>
	<ul :class="bookmarkClasses">
		<li
			v-for="bmark in model.Bookmarks"
			:key="bmark.MenuID"
			:data-key="`bookmark_${bmark.Module}_${bmark.MenuID}`"
			class="nav-item n-sidebar__nav-item bookmarks__item">
			<menu-action
				v-if="bmark.MenuEntryObj"
				class="bookmarks__btn--link"
				:title="bmark.Description"
				:menu="bmark.MenuEntryObj"
				:description="bmark.Description"
				:has-sub-menu-toggle="false"
				@menu-action="$emit('menu-action')"
				@keyup="(...args) => $emit('keyup', ...args)">
				{{ bmark.Description }}
				<template>
					<span class="bookmarks__text">
						{{ bmark.MenuEntryObj.Title }}
					</span>

					<br />

					<span class="bookmarks__full-path">
						{{ bmark.Description }}
					</span>
				</template>
			</menu-action>
			<span
				v-else
				class="bookmarks__text">
				{{ bmark.Description }}
			</span>

			<br />

			<q-button
				v-bind="removeBtnAttrs"
				@click="removeBookmark(bmark.Codusrcfg)"
				@keyup="(...args) => $emit('keyup', ...args)"
				class="bookmarks__btn--remove">
				<q-icon-svg icon="remove" />
			</q-button>
		</li>

		<li class="nav-item n-sidebar__nav-item bookmarks__item">
			<q-button
				v-bind="addBtnAttrs"
				@click="activateSelectionMode"
				@keyup="(...args) => $emit('keyup', ...args)"
				class="bookmarks__btn--add">
				<q-icon icon="add" />
			</q-button>
		</li>
	</ul>
</template>

<script>
	import { computed } from 'vue'
	import _assignIn from 'lodash-es/assignIn'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import Bookmarks from '@/mixins/bookmarks.js'

	import MenuAction from './MenuAction.vue'

	export default {
		name: 'BookmarksContent',

		emits: ['keyup', 'menu-action', 'add', 'remove'],

		components: {
			MenuAction
		},

		props: {
			/**
			 * Additional CSS classes to be applied to the bookmarks container.
			 */
			classes: {
				type: [Array, String],
				default: ''
			},

			/**
			 * Determines if the bookmark titles should be displayed.
			 */
			showTitles: {
				type: Boolean,
				default: true
			}
		},

		expose: [],

		data()
		{
			return {
				model: {
					Bookmarks: []
				},

				bookmarks: new Bookmarks(this.updateData),

				texts: {
					add: computed(() => this.Resources[hardcodedTexts.add]),
					remove: computed(() => this.Resources[hardcodedTexts.remove])
				}
			}
		},

		mounted()
		{
			this.bookmarks.fetchData()

			this.$eventHub.on('before-execute-menu-action', this.onMenuActionClick)
		},

		beforeUnmount()
		{
			this.$eventHub.off('before-execute-menu-action', this.onMenuActionClick)
		},

		computed: {
			/**
			 * Compiles the class list for the bookmarks container based on the received classes prop.
			 */
			bookmarkClasses()
			{
				let classes = ['bookmarks__content']

				if (typeof this.classes === 'string')
					classes.push(this.classes)
				else if (Array.isArray(this.classes))
					classes = [...classes, ...this.classes]

				return classes
			},

			/**
			 * Attributes for the remove bookmark button, including styling and optional label based on showTitles flag.
			 */
			removeBtnAttrs()
			{
				const data = {
					variant: 'text',
					size: 'small'
				}

				if (this.showTitles)
				{
					data.label = this.texts.remove
					data.title = this.texts.remove
				}

				return data
			},

			/**
			 * Attributes for the add bookmark button, including styling and optional label based on showTitles flag.
			 */
			addBtnAttrs()
			{
				const data = {
					variant: 'text',
					size: 'small'
				}

				if (this.showTitles)
				{
					data.label = this.texts.add
					data.title = this.texts.add
				}

				return data
			}
		},

		methods: {
			/**
			 * Method to update the bookmark data with newly fetched or received data.
			 * @param {Object} data - The new bookmark data to be assigned.
			 */
			updateData(data)
			{
				_assignIn(this.model, data)
			},

			/**
			 * Method to handle the event when a menu action bookmark is clicked.
			 * @param {Object} eventData - The data associated with the clicked menu action.
			 */
			onMenuActionClick(eventData)
			{
				this.bookmarks.addBookmark(eventData.module, eventData.id)
			},

			/**
			 * Method to handle the event when the bookmark button is clicked.
			 */
			activateSelectionMode()
			{
				this.bookmarks.activateSelectionMode()
				this.$emit('add')
			},

			/**
			 * Remove bookmark
			 * @param { string } id Bookmark ID
			 */
			removeBookmark(id)
			{
				this.bookmarks.removeBookmark(id)
				this.$emit('remove')
			}
		}
	}
</script>
