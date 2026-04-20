<template>
	<div
		v-if="$app.layout.MenuSearchEnable && hasMenus"
		class="n-sidebar__section menu-search">
		<q-input-group size="block">
			<q-text-field
				v-model="searchString"
				:placeholder="texts.menuSearch"
				@keyup="searchMenu" />
			<template #append>
				<q-button
					variant="bold"
					id="menu-search-btn"
					:title="texts.search"
					@click="searchMenu">
					<q-icon icon="search" />
				</q-button>
			</template>
		</q-input-group>

		<search-results
			v-if="searchString"
			:values="results" />
	</div>
</template>

<script>
	import { computed } from 'vue'

	import { fetchData } from '@quidgest/clientapp/network'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import SearchResults from '@/views/shared/SearchResults.vue'

	export default {
		name: 'QMenuSearch',

		components: {
			SearchResults
		},

		mixins: [
			LayoutHandlers
		],

		expose: [],

		data()
		{
			return {
				searchString: '',

				results: [],

				texts: {
					menuSearch: computed(() => this.Resources[hardcodedTexts.menuSearch]),
					search: computed(() => this.Resources[hardcodedTexts.search])
				}
			}
		},

		methods: {
			searchMenu()
			{
				const params = {
					searchString: this.searchString
				}

				fetchData('MenuSearch', 'Search', params, data => {
					this.results = data
				})
			}
		}
	}
</script>
