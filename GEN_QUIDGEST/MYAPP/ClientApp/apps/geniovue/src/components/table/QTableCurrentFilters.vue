<template>
	<q-row
		class="q-table__current-filters"
		:gutter="4">
		<q-col
			v-for="(filter, filterIdx) in filters"
			:key="filterIdx"
			cols="auto">
			<q-badge
				data-testid="table-filter"
				pill
				removable
				:disabled="!filter.active"
				:title="texts.editText"
				:data-column-id="filterIdx"
				:texts="texts"
				@click="editFilter(filterIdx)"
				@click:remove="removeFilter(filterIdx)">
				<q-icon icon="filter" />
				{{ getFilterName(filterOperators, filter, searchableColumns, texts.orText, texts.allFieldsText) }}
			</q-badge>
		</q-col>
	</q-row>
</template>

<script>
	import cloneDeep from 'lodash-es/cloneDeep'

	import listFunctions from '@/mixins/listFunctions.js'
	import searchFilterDataModule from '@/api/genio/searchFilterData'

	export default {
		name: 'QTableCurrentFilters',

		emits: ['show-filters', 'update:filters'],

		props: {
			/**
			 * Localized text strings to be used within the component for labels, titles, and accessibility.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Array of columns that can be searched, which determines which filters are applicable and how they are displayed.
			 */
			searchableColumns: {
				type: Array,
				default: () => []
			},

			/**
			 * Object where each key corresponds to a column's API field name, and its value is the filter applied to that column.
			 */
			filters: {
				type: Array,
				default: () => []
			},

			/**
			 * Predefined set of filter operators which describe how to apply filters to the data (e.g., 'equals', 'contains').
			 */
			filterOperators: {
				type: Object,
				default: () => new searchFilterDataModule.SearchFilterConditionOperators()
			}
		},

		expose: [],

		methods: {
			getFilterName: listFunctions.getFilterName,

			/**
			 * Open popup to edit the filter
			 * @param {number} filterIdx
			 */
			editFilter(filterIdx)
			{
				this.$emit('show-filters', filterIdx)
			},

			/**
			 * Remove filter
			 * @param {number} filterIdx
			 */
			removeFilter(filterIdx)
			{
				const filters = cloneDeep(this.filters)
				filters.splice(filterIdx, 1)

				this.$emit('update:filters', filters)
			}
		}
	}
</script>
