<template>
	<q-row
		v-if="hasActiveFilters || groupFilters.length > 0"
		class="q-table__static-filters"
		align="center"
		:gutter="0">
		<!-- BEGIN: Active Filters -->
		<q-col
			v-if="hasActiveFilters"
			cols="auto">
			<q-row :gutter="4">
				<q-col
					v-for="item in activeFilters.items"
					:key="item.key"
					class="q-table__static-filter"
					cols="auto">
					<q-checkbox
						:id="`${id}-${item.id}`"
						:model-value="activeFilters.selected?.includes(item.key)"
						size="small"
						:label="item.value"
						:readonly="disabled"
						@update:model-value="updateActiveFilterSelected(item.key, $event)" />
				</q-col>

				<q-col cols="auto">
					<div class="q-table__static-filters-date">
						<q-label :for="activeFilters.date.id">
							{{ texts.onDate }}
						</q-label>

						<q-date-time-picker
							:id="`${id}-${activeFilters.date.id}`"
							:model-value="activeFilters.date.value"
							:size="activeFilters.date.type === 'date' ? 'small' : 'medium'"
							:format="dateFormat"
							:locale="locale"
							:disabled="disabled"
							@update:model-value="updateActiveFilterDateValue" />
					</div>
				</q-col>
			</q-row>
		</q-col>
		<!-- END: Active Filters -->

		<!-- BEGIN: Group Filters -->
		<q-col
			v-for="(filter, groupIndex) in groupFilters"
			:key="groupIndex"
			cols="auto">
			<q-row :gutter="4">
				<q-col
					v-if="hasActiveFilters || groupIndex > 0"
					cols="auto">
					<q-divider direction="vertical" />
				</q-col>

				<template v-if="filter.isMultiple">
					<q-col
						v-for="item in filter.items"
						:key="item.key"
						class="q-table__static-filter"
						cols="auto">
						<q-checkbox
							:id="`${id}-${item.id}`"
							:model-value="filter.selected?.includes(item.key)"
							size="small"
							:label="item.value"
							:readonly="disabled"
							@update:model-value="updateGroupFilterSelected(groupIndex, item.key, $event)" />
					</q-col>
				</template>
				<q-col
					v-else
					class="q-table__static-filter"
					cols="auto">
					<q-radio-group
						:model-value="filter.selected"
						orientation="horizontal"
						:readonly="disabled"
						@update:model-value="updateGroupFilterSelected(groupIndex, $event)">
						<q-radio-button
							v-for="item in filter.items"
							:key="item.key"
							:value="item.key"
							:label="item.value" />
					</q-radio-group>
				</q-col>
			</q-row>
		</q-col>
		<!-- END: Group Filters -->
	</q-row>
</template>

<script>
	export default {
		name: 'QTableStaticFilters',

		emits: {
			'update:activeFilters': (payload) => typeof payload === 'object',
			'update:groupFilters': (payload) => Array.isArray(payload)
		},

		props: {
			/**
			 * The identifier of the component.
			 */
			id: String,

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
			 * Localization or custom text strings that are used within the static filters interface, aiding in text consistency and localization.
			 */
			texts: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Current system locale
			 */
			locale: {
				type: String,
				default: 'en-US'
			},

			/**
			 * Date formats
			 */
			dateFormats: {
				type: Object,
				required: true
			},

			/**
			 * Whether the control is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		computed: {
			/**
			 * The date format.
			 */
			dateFormat()
			{
				return this.hasActiveFilters
					? this.dateFormats[this.activeFilters.date.type]
					: null
			},

			/**
			 * True if there's an active filter, false otherwise.
			 */
			hasActiveFilters()
			{
				return !!this.activeFilters
			}
		},

		methods: {
			/**
			 * Update active filters.
			 * @param filterId {string}
			 * @param value {boolean}
			 */
			updateActiveFilterSelected(filterId, value)
			{
				let selected = [...this.activeFilters.selected]

				selected = selected.filter((f) => f !== filterId)
				if (value)
					selected.push(filterId)

				const activeFilters = {
					...this.activeFilters,
					selected
				}

				this.$emit('update:activeFilters', activeFilters)
			},

			/**
			 * Update active filters.
			 * @param value {Object}
			 */
			updateActiveFilterDateValue(value)
			{
				const date = { ...this.activeFilters.date }
				date.value = value

				const activeFilters = {
					...this.activeFilters,
					date
				}

				this.$emit('update:activeFilters', activeFilters)
			},

			/**
			 * Update group filters.
			 * @param groupIndex {number}
			 * @param selected {string}
			 * @param value {boolean}
			 */
			updateGroupFilterSelected(groupIndex, filterId, value)
			{
				const groupFilter = this.groupFilters[groupIndex]
				const groupFilters = [...this.groupFilters]

				let selected

				if (groupFilter.isMultiple)
				{
					selected = groupFilter.selected.filter((f) => f !== filterId)
					if (value)
						selected.push(filterId)
				}
				else
					selected = filterId

				groupFilters[groupIndex] = {
					...groupFilters[groupIndex],
					selected
				}

				this.$emit('update:groupFilters', groupFilters)
			}
		}
	}
</script>
