<template>
	<!-- BEGIN: Filters Tab -->
	<div
		v-if="!!$slots.default"
		class="q-table__global-filters">
		<slot />
	</div>

	<q-table-static-filters
		v-if="hasStaticFilters"
		id="table-config-static-filters"
		:active-filters="editedActiveFilters"
		:group-filters="editedGroupFilters"
		:date-formats="dateFormats"
		:locale="locale"
		:texts="texts"
		@update:active-filters="setActiveFilters"
		@update:group-filters="setGroupFilters" />

	<q-divider v-if="hasStaticFilters" />

	<q-row>
		<q-col cols="auto">
			<q-button
				data-testid="filter-create"
				:label="texts.createFilterText"
				:title="texts.createFilterText"
				@click="addFilter()">
				<q-icon icon="filter" />
			</q-button>
		</q-col>

		<q-col cols="auto">
			<q-button
				data-testid="filter-remove"
				:disabled="editedFilters.length === 0"
				:label="texts.removeAll"
				:title="texts.removeAll"
				@click="clearFilters">
				<q-icon icon="delete" />
			</q-button>
		</q-col>
	</q-row>

	<template
		v-for="(filter, filterIdx) in editedFilters"
		:key="filterIdx">
		<q-row
			v-if="filterIdx > 0"
			:gutter="6">
			<q-col>
				<q-badge
					color="highlight"
					variant="bold"
					:title="texts.andText">
					{{ texts.andText }}
				</q-badge>
			</q-col>
		</q-row>

		<q-container
			:id="`filter_${tableName}_${filterIdx}`"
			:class="['q-table__filter', { 'q-table__filter-selected': selectedFilterIdx === filterIdx }]"
			fluid>
			<q-row :gutter="4">
				<q-col>
					<q-switch
						:model-value="filter.active"
						show-state-labels
						:label="texts.filterStatus"
						:true-label="texts.activeText"
						:false-label="texts.inactiveText"
						@update:model-value="setFilterStatus(filterIdx, filter, $event)" />
				</q-col>

				<q-col cols="auto">
					<q-button
						variant="ghost"
						:title="texts.removeText"
						@click="removeFilter(filterIdx)">
						<q-icon icon="delete" />
					</q-button>
				</q-col>
			</q-row>

			<q-row :gutter="3">
				<q-col data-control-type="field">
					<q-select
						:model-value="filter.field"
						required
						size="block"
						:items="searchableColumnOptions"
						:texts="texts"
						@update:model-value="setFilterDefaultOperator(filterIdx, $event, filter)" />
				</q-col>

				<q-col data-control-type="operator">
					<q-select
						:model-value="filter.operator"
						required
						size="block"
						:items="getFilterOperatorOptions(filter)"
						:texts="texts"
						@update:model-value="setFilterDefaultValues(filterIdx, $event, filter)" />
				</q-col>

				<q-col
					class="q-table__filter-values"
					data-control-type="value"
					:data-search-type="getFilterColumnFromName(filter, searchableColumns)?.searchFieldType"
					:cols="4">
					<template
						v-for="(_, valueIdx) in getFilterValueCount(filter)"
						:key="`${filter.field}_${valueIdx}`">
						<component
							:is="getFilterInputComponent(filter)"
							size="block"
							:classes="[{ 'q-field--invalid': getFilterValueErrorMessages(filterIdx, valueIdx).length > 0 }]"
							:table-name="`${tableName}_filters`"
							:column-name="`${filterIdx}_${valueIdx}`"
							:row-index="filterIdx"
							:options="getValueOptions(filter)"
							:value="filter.values[valueIdx]"
							:placeholder="getFilterPlaceholder(filter)"
							:texts="texts"
							:locale="locale"
							@loaded="focusSelectedFilter(valueIdx)"
							@update="setFilterConditionValue(filterIdx, filter, valueIdx, $event)" />

						<span
							v-for="(errorMessage, idx) in getFilterValueErrorMessages(filterIdx, valueIdx)"
							:key="idx"
							class="q-table__filter-error">
							<q-icon icon="exclamation-sign" />
							{{ errorMessage }}
						</span>
					</template>
				</q-col>

				<q-col
					v-if="filter.subFilters.length > 0"
					cols="auto">
					<q-button
						:title="texts.removeConditionText"
						@click="removeMainCondition(filterIdx)">
						<q-icon icon="remove" />
					</q-button>
				</q-col>
			</q-row>

			<template
				v-for="(subFilter, subFilterIdx) in filter.subFilters"
				:key="subFilterIdx">
				<q-row :gutter="2">
					<q-col>
						<q-badge
							color="secondary"
							variant="bold"
							:title="texts.orText">
							{{ texts.orText }}
						</q-badge>
					</q-col>
				</q-row>

				<q-row :gutter="3">
					<q-col data-control-type="field">
						<q-select
							:model-value="subFilter.field"
							required
							size="block"
							:items="searchableColumnOptions"
							:texts="texts"
							@update:model-value="setFilterDefaultOperator(filterIdx, $event, subFilter)" />
					</q-col>

					<q-col data-control-type="operator">
						<q-select
							:model-value="subFilter.operator"
							required
							size="block"
							:items="getFilterOperatorOptions(subFilter)"
							:texts="texts"
							@update:model-value="setFilterDefaultValues(filterIdx, $event, subFilter)" />
					</q-col>

					<q-col
						class="q-table__filter-values"
						data-control-type="value"
						:data-search-type="getFilterColumnFromName(subFilter, searchableColumns)?.searchFieldType"
						:cols="4">
						<template
							v-for="(_, valueIdx) in getFilterValueCount(subFilter)"
							:key="`${subFilter.field}_${valueIdx}`">
							<component
								:is="getFilterInputComponent(subFilter)"
								size="block"
								:classes="[{ 'q-field--invalid': getFilterValueErrorMessages(filterIdx, valueIdx, subFilterIdx).length > 0 }]"
								:table-name="`${tableName}_filters`"
								:column-name="`${subFilterIdx}_${valueIdx}`"
								:row-index="subFilterIdx"
								:options="getValueOptions(subFilter)"
								:value="subFilter.values[valueIdx]"
								:placeholder="getFilterPlaceholder(subFilter)"
								:texts="texts"
								:locale="locale"
								@update="setFilterConditionValue(filterIdx, subFilter, valueIdx, $event)" />

							<span
								v-for="(errorMessage, idx) in getFilterValueErrorMessages(filterIdx, valueIdx, subFilterIdx)"
								:key="idx"
								class="q-table__filter-error">
								<q-icon icon="exclamation-sign" />
								{{ errorMessage }}
							</span>
						</template>
					</q-col>

					<q-col cols="auto">
						<q-button
							:title="texts.removeConditionText"
							@click="removeSubFilter(filterIdx, subFilterIdx)">
							<q-icon icon="remove" />
						</q-button>
					</q-col>
				</q-row>
			</template>

			<q-row :gutter="4">
				<q-col cols="auto">
					<q-button
						:title="texts.createConditionText"
						:label="texts.createConditionText"
						@click="addSubFilter(filterIdx)">
						<q-icon icon="add" />
					</q-button>
				</q-col>
			</q-row>
		</q-container>
	</template>
	<!-- END: Filters Tab -->
</template>

<script>
	import { nextTick } from 'vue'
	import isEqual from 'lodash-es/isEqual'

	import { deepUnwrap } from '@quidgest/clientapp/utils/deepUnwrap'
	import searchFilterDataModule from '@/api/genio/searchFilterData.js'
	import listFunctions from '@/mixins/listFunctions.js'

	export default {
		name: 'QTableConfigFilters',

		emits: [
			'update:activeFilters',
			'update:groupFilters',
			'update:filters'
		],

		inheritAttrs: false,

		props: {
			/**
			 * Localized text strings to display within the popup, such as buttons and titles.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * The name of the table or data set these filters are targeting.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * An array of column definitions to which the filters can be applied.
			 */
			columns: {
				type: Array,
				default: () => []
			},

			/**
			 * The column to use, to create a new filter when opening the configuration popup.
			 */
			selectedColumn: {
				type: String,
				default: ''
			},

			/**
			 * The filter to select by default, when opening the configuration popup.
			 */
			selectedFilter: {
				type: Number,
				default: -1
			},

			/**
			 * An object representing active filters, which can be a mixture of various filter types including boolean or date.
			 */
			activeFilters: Object,

			/**
			 * An array representing groups of filters that apply globally to the data set, affecting all columns.
			 */
			groupFilters: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of filter objects that represent the current configuration.
			 */
			filters: {
				type: Array,
				default: () => []
			},

			/**
			 * Date formats
			 */
			dateFormats: {
				type: Object,
				default: () => ({})
			},

			/**
			 * A set of predefined filter operators used in building the filter conditions (e.g., 'contains', 'equals').
			 */
			filterOperators: {
				type: Object,
				default: () => new searchFilterDataModule.SearchFilterConditionOperators()
			},

			/**
			 * Current system locale
			 */
			locale: {
				type: String,
				default: 'en-US'
			}
		},

		expose: [],

		data() {
			return {
				editedActiveFilters: null,
				editedGroupFilters: [],
				editedFilters: [],
				selectedFilterIdx: -1,
				validationErrors: []
			}
		},

		computed: {
			searchableColumns() {
				return listFunctions.getSearchableColumns(this.columns)
			},

			searchableColumnOptions() {
				return listFunctions.getSearchableColumnOptions(this.searchableColumns)
			},

			hasStaticFilters() {
				return this.groupFilters.length > 0 || Object.keys(this.activeFilters ?? {}).length > 0
			}
		},

		methods: {
			/**
			 * Gets the items for the operator dropdown of the specified filter.
			 * @param filter The filter
			 * @returns A list with the items.
			 */
			getFilterOperatorOptions(filter) {
				return listFunctions.getFilterOperatorOptions(filter, this.filterOperators, this.searchableColumns)
			},

			/**
			 * Gets the options to pass to the filter value component.
			 * @param filter The filter
			 * @returns An object with the options for the filter value.
			 */
			getValueOptions(filter) {
				const column = this.getFilterColumnFromName(filter)

				return {
					array: this.getColumnArray(filter),
					currencySymbol: column?.currencySymbol,
					decimalPlaces: column?.decimalPlaces,
					maxDigits: column?.maxDigits,
					numberFormat: column?.numberFormat,
					format: column?.format,
					dateTimeType: column?.dateTimeType,
					teleport: true
				}
			},

			/**
			 * Get column of condition by index
			 * @param {Object} filter
			 * @returns {Object}
			 */
			getFilterColumnFromName(filter) {
				return listFunctions.getColumnFromTableColumnName(this.searchableColumns, filter.field)
			},

			/**
			 * Get number of values for condition by index
			 * @param {Object} filter
			 * @returns {Object}
			 */
			getFilterValueCount(filter) {
				return listFunctions.getFilterValueCount(this.filterOperators, filter, this.searchableColumns)
			},

			/**
			 * Get input component for condition by index
			 * @param {Object} filter
			 * @returns {string}
			 */
			getFilterInputComponent(filter) {
				return listFunctions.getFilterInputComponent(this.filterOperators, filter, this.searchableColumns)
			},

			/**
			 * Get placeholder for condition input by index
			 * @param {Object} filter
			 * @returns {string}
			 */
			getFilterPlaceholder(filter) {
				return listFunctions.getFilterPlaceholder(this.filterOperators, filter, this.searchableColumns)
			},

			/**
			 * Sets the status of the filter (active/inactive)
			 * @param {number} filterIdx
			 * @param {Object} filter
			 * @param {boolean} value
			 */
			setFilterStatus(filterIdx, filter, value) {
				filter.active = value
				// Also set the sub filters to active/inactive
				for (const subFilter of filter.subFilters)
					subFilter.active = value

				this.selectFilter(filterIdx)
			},

			/**
			 * Select default operator for condition by index
			 * @param {number} filterIdx
			 * @param {string} field
			 * @param {Object} filter
			 */
			setFilterDefaultOperator(filterIdx, field, filter) {
				filter.field = field ?? ''
				listFunctions.setFilterDefaultOperator(this.filterOperators, filter, this.searchableColumns)
				this.selectFilter(filterIdx)
			},

			/**
			 * Set default values for condition by index
			 * @param {number} filterIdx
			 * @param {string} operator
			 * @param {Object} filter
			 */
			setFilterDefaultValues(filterIdx, operator, filter) {
				filter.operator = operator ?? ''
				listFunctions.setFilterDefaultValues(this.filterOperators, filter, this.searchableColumns)
				this.selectFilter(filterIdx)
			},

			/**
			 * Set value of condition by index
			 * @param {number} filterIdx
			 * @param {Object} filter
			 * @param {number} valueIdx : index
			 * @param {object} value : value
			 */
			setFilterConditionValue(filterIdx, filter, valueIdx, value) {
				listFunctions.setFilterConditionValue(filter, valueIdx, value)
				this.selectFilter(filterIdx)
			},

			/**
			 * Set filter data in internal property for editing
			 */
			updateFilters() {
				this.editedFilters = deepUnwrap(this.filters.toReversed())
				this.validationErrors = []

				if (this.hasStaticFilters)
				{
					this.editedActiveFilters = deepUnwrap(this.activeFilters)
					this.editedGroupFilters = deepUnwrap(this.groupFilters)
				}
			},

			/**
			 * Clears all search filters
			 */
			clearFilters() {
				this.editedFilters = []
				this.selectFilter(-1)
			},

			/**
			 * Gets the enumeration values associated to the column of the specified filter
			 * @param filter The filter
			 * @returns A list with the enumeration values associated to the column
			 */
			getColumnArray(filter) {
				const column = this.getFilterColumnFromName(filter)
				return deepUnwrap(column?.array ?? [])
			},

			/**
			 * Sets the active filters
			 * @param filters The value of the filters
			 */
			setActiveFilters(filters) {
				this.editedActiveFilters = filters
			},

			/**
			 * Sets the group filters
			 * @param filters The value of the filters
			 */
			setGroupFilters(filters) {
				this.editedGroupFilters = filters
			},

			/**
			 * Remove filter
			 * @param {number} filterIdx The index of the filter
			 */
			removeFilter(filterIdx) {
				this.editedFilters.splice(filterIdx, 1)
				this.selectFilter(-1)
			},

			/**
			 * Remove the first condition of the filter
			 * @param {number} filterIdx The index of the filter
			 */
			removeMainCondition(filterIdx) {
				this.editedFilters[filterIdx] = listFunctions.removeFirstFilterCondition(this.editedFilters[filterIdx])
				this.selectFilter(filterIdx)
			},

			/**
			 * Add new filter
			 * @param {string} column The column to use in the filter
			 */
			async addFilter(column) {
				const filter = listFunctions.searchFilter('', true, column || listFunctions.columnFullName(this.searchableColumns[0]), '', [])
				this.editedFilters.unshift(filter)
				listFunctions.setFilterDefaultOperator(this.filterOperators, filter, this.searchableColumns)

				await nextTick()
				this.selectFilter(0)
			},

			/**
			 * Remove sub filter
			 * @param {number} filterIdx The index of the filter
			 * @param {number} subFilterIdx The index of the sub filter
			 */
			removeSubFilter(filterIdx, subFilterIdx) {
				this.editedFilters[filterIdx].subFilters.splice(subFilterIdx, 1)
				this.selectFilter(filterIdx)
			},

			/**
			 * Add new sub filter
			 * @param {number} filterIdx The index of the filter
			 */
			addSubFilter(filterIdx) {
				const filter = listFunctions.searchFilter('', true, listFunctions.columnFullName(this.searchableColumns[0]), '', [])
				this.editedFilters[filterIdx].subFilters.push(filter)
				listFunctions.setFilterDefaultOperator(this.filterOperators, filter, this.searchableColumns)
				filter.useOr = true
				this.selectFilter(filterIdx)
			},

			/**
			 * Set the currently selected filter
			 * @param {number} filterIdx The index of the filter
			 */
			selectFilter(filterIdx) {
				if (this.selectedFilterIdx === filterIdx)
					return

				this.selectedFilterIdx = filterIdx
				this.focusSelectedFilter(filterIdx)
			},

			/**
			 * Gets the identifier of the filter
			 * @param filterIdx The index of the filter
			 * @param valueIdx The index of the filter value
			 * @param subFilterIdx The index of the sub filter, in case it's one
			 */
			getFilterId(filterIdx, valueIdx, subFilterIdx = -1) {
				return `${this.tableName}_filters_${filterIdx}${subFilterIdx > -1 ? '_' + subFilterIdx : ''}_${valueIdx}`
			},

			/**
			 * Validates the specified filter
			 * @param filter The filter
			 * @param filterIdx The index of the filter
			 * @param subFilterIdx The index of the sub filter, in case it's one
			 */
			validateFilter(filter, filterIdx, subFilterIdx) {
				const valueCount = this.getFilterValueCount(filter)
				const conditionStates = listFunctions.filterValidate(filter, this.columns, valueCount)

				//Iterate filter conditions
				for (const conditionState of conditionStates)
				{
					if (conditionState.state !== 'VALID')
					{
						//Iterate values
						for (const valueIdx in conditionState.valueStates)
						{
							const valueState = conditionState.valueStates[valueIdx]
							if (valueState !== 'VALID')
							{
								this.validationErrors.push({
									id: this.getFilterId(filterIdx, valueIdx, subFilterIdx),
									fieldType: conditionState.type,
									message: `${conditionState.label} ${this.texts.isRequired}`
								})
							}
						}
					}
				}
			},

			/**
			 * Get validation error information
			 * @returns {Array}
			 */
			validateFilters() {
				this.validationErrors = []

				for (let filterIdx = 0; filterIdx < this.editedFilters.length; filterIdx++)
				{
					const editedFilter = this.editedFilters[filterIdx]
					this.validateFilter(editedFilter, filterIdx)

					for (let subFilterIdx = 0; subFilterIdx < editedFilter.subFilters.length; subFilterIdx++)
					{
						const filter = editedFilter.subFilters[subFilterIdx]
						this.validateFilter(filter, filterIdx, subFilterIdx)
					}
				}
			},

			/**
			 * Get value field error messages
			 * @param {number} filterIdx
			 * @param {number} valueIdx
			 * @param {number} subFilterIdx
			 * @returns {Array}
			 */
			getFilterValueErrorMessages(filterIdx, valueIdx, subFilterIdx) {
				const errors = this.validationErrors.filter((error) => error.id === this.getFilterId(filterIdx, valueIdx, subFilterIdx))
				return errors.map((error) => error.message)
			},

			/**
			 * Focuses on the value input of the currently selected filter
			 * @param {number} filterIdx
			 */
			async focusSelectedFilter(filterIdx)
			{
				// Wait for the components to render.
				await nextTick()

				if (filterIdx === this.selectedFilterIdx && filterIdx >= 0 && filterIdx < this.editedFilters.length)
				{
					setTimeout(() => {
						const filterForm = document.getElementById(`filter_${this.tableName}_${filterIdx}`)
						// Scroll to the filter and focus on the value input field
						filterForm?.scrollIntoView({ block: 'center', inline: 'nearest' })
						filterForm?.querySelector('[data-control-type="value"]')?.querySelector('input')?.focus()
					}, 150)
				}
			}
		},

		watch: {
			selectedFilter: {
				handler(val)
				{
					if (val !== -1)
					{
						const reversedIndex = this.filters.length - 1 - val
						this.selectFilter(reversedIndex)
					}
					else
						this.selectFilter(-1)
				},
				immediate: true
			},

			selectedColumn: {
				async handler(val)
				{
					if (val.length > 0)
					{
						await nextTick()

						this.updateFilters()
						this.addFilter(val)
					}
				},
				immediate: true
			},

			filters: {
				handler(newVal, oldVal)
				{
					// Only react if the length has changed, since the only thing that
					// can be done outside of this component is add or remove filters.
					if (newVal.length !== oldVal?.length)
						this.updateFilters()
				},
				immediate: true
			},

			editedFilters: {
				handler()
				{
					this.validateFilters()
					if (this.validationErrors.length > 0)
						return

					this.$emit('update:filters', this.editedFilters.toReversed())
				},
				deep: true
			},

			editedActiveFilters: {
				handler(newVal, oldVal)
				{
					if (!isEqual(newVal, oldVal))
						this.$emit('update:activeFilters', newVal)
				},
				deep: true
			},

			editedGroupFilters: {
				handler(newVal, oldVal)
				{
					if (!isEqual(newVal, oldVal))
						this.$emit('update:groupFilters', newVal)
				},
				deep: true
			}
		}
	}
</script>
