<template>
	<q-button
		v-if="collapsed"
		borderless
		class="q-menu-search__toggle"
		:title="texts.menuSearch"
		@click="expand">
		<q-icon icon="search" />
	</q-button>
	<div
		v-else
		class="q-menu-search"
		@focusout="collapse">
		<q-combobox
			ref="searchBoxInput"
			clearable
			size="block"
			item-value="Id"
			item-label="Text"
			filter-mode="manual"
			:items="results"
			:texts="texts"
			:loading="loading"
			:placeholder="texts.menuSearch"
			@update:model-value="navigateTo"
			@update:search="onUpdateInputValue">
			<template #prepend>
				<q-icon icon="search" />
			</template>
			<template #item="{ item }">
				<div class="q-menu-search__item">
					<span class="menu-result__text">
						{{ item.Text }}
					</span>
					<span class="menu-result__full-path">
						{{ item.ModuleText }} > {{ item.FlatMenu }}
					</span>
				</div>
			</template>
			<template
				v-if="results.length"
				#[`body.append`]>
				<div class="q-menu-search__results">
					{{ results.length }} {{ texts.results?.toLowerCase() }}
				</div>
			</template>
		</q-combobox>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import _debounce from 'lodash-es/debounce'

	import { fetchData } from '@quidgest/clientapp/network'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import MenuAction from '@/mixins/menuAction.js'
	import VueNavigation from '@/mixins/vueNavigation.js'

	export default {
		name: 'QMenuSearch',

		emits: ['update:collapsed'],

		mixins: [
			VueNavigation,
			LayoutHandlers,
			MenuAction
		],

		expose: [],

		props: {
			/**
			 * If set to true, collapses the menu search,
			 * revealing only the search button.
			 */
			collapsed: {
				type: Boolean,
				default: true
			}
		},

		data() {
			return {
				searchString: '',

				results: [],

				loading: false,

				texts: {
					noData: computed(() => this.Resources[hardcodedTexts.noData]),
					menuSearch: computed(() => this.Resources[hardcodedTexts.menuSearch]),
					results: computed(() => this.Resources[hardcodedTexts.results])
				}
			}
		},

		methods: {
			/**
			 * Debounces the searchMenu function to prevent over-fetching as the user types.
			 */
			debouncedSearchMenu()
			{
				_debounce(this.searchMenu, 1000)()
			},

			/**
			 * Expands the search box allowing the user to input their search.
			 */
			expand()
			{
				this.$emit('update:collapsed', false)
				this.$nextTick().then(() => this.$refs.searchBoxInput.triggerEl.inputRef.focus())
			},

			/**
			 * Collapses the search box, hiding the search input field.
			 */
			collapse()
			{
				this.$emit('update:collapsed', true)
			},

			/**
			 * Performs a search based on the user's input and updates the search results.
			 * @param {String} searchString - The user's search string.
			 */
			onUpdateInputValue(searchString)
			{
				this.searchString = searchString

				this.results = []
				this.loading = true

				this.debouncedSearchMenu()
			},

			/**
			 * Fetches search results from the Menu Search API endpoint.
			 */
			searchMenu()
			{
				const params = {
					searchString: this.searchString
				}

				fetchData('MenuSearch', 'Search', params, (data) => {
					this.results = data
					this.loading = false
				})
			},

			/**
			 * Navigates the user to the menu item they selected from the search results.
			 * @param {String} menuId - The ID of the selected menu.
			 */
			navigateTo(menuId)
			{
				const menu = this.results.find((m) => m.Id === menuId)
				if (menu)
					this.executeMenuAction(menu.MenuObj)
			}
		}
	}
</script>
