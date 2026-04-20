<template>
	<ul
		id="search-menu-area"
		:class="searchResultClasses">
		<li
			v-if="totalCountFirst"
			class="nav-item n-sidebar__nav-item">
			<span class="menu-result__count">{{ totalResults }} {{ texts.results }}</span>
		</li>

		<li
			v-for="value in values"
			:key="value.Id"
			class="nav-item n-sidebar__nav-item menu-result">
			<menu-action
				check-children
				:menu="value.MenuObj"
				:description="value.Text"
				:has-sub-menu-toggle="false"
				@menu-action="(...args) => $emit('menu-action', ...args)">
				<span class="menu-result__text">
					<b>{{ value.Text }}</b>
				</span>

				<br />

				<span class="menu-result__full-path">
					<b>{{ value.ModuleText }} > {{ value.FlatMenu }}</b>
				</span>
			</menu-action>
		</li>

		<li
			v-if="!totalCountFirst"
			class="nav-item n-sidebar__nav-item">
			<span class="menu-result__count">{{ totalResults }} {{ texts.results }}</span>
		</li>
	</ul>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import MenuAction from '@/views/shared/MenuAction.vue'

	export default {
		name: 'QSearchResults',

		components: {
			MenuAction
		},

		mixins: [
			LayoutHandlers
		],

		emits: ['menu-action'],

		props: {
			/**
			 * Array of search result values to be displayed, each containing Id, Text, Module, MenuObj, and FlatMenu properties.
			 */
			values: {
				type: Array,
				default: () => []
			},

			/**
			 * Boolean to determine if the total count of results should be displayed above the results.
			 */
			totalCountFirst: {
				type: Boolean,
				default: false
			},

			/**
			 * Additional classes to be applied to the search results container.
			 */
			classes: {
				type: [Array, String],
				default: ''
			}
		},

		expose: [],

		data()
		{
			return {
				texts: {
					results: computed(() => this.Resources[hardcodedTexts.results])
				}
			}
		},

		computed: {
			/**
			 * Computed classes for the search result UL element based on the received classes prop.
			 */
			searchResultClasses()
			{
				let classes = ['nav', 'nav-pills', 'nav-sidebar', 'flex-column', 'n-sidebar__nav']

				if (typeof this.classes === 'string')
					classes.push(this.classes)
				else if (Array.isArray(this.classes))
					classes = [ ...classes, ...this.classes ]

				return classes
			},

			/**
			 * Computed property for the total number of search results.
			 */
			totalResults()
			{
				return this.values?.length ?? 0
			}
		}
	}
</script>
