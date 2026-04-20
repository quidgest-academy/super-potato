<template>
	<template v-if="showRowsSelectedCount">
		<span
			v-if="allRowsSelected"
			class="selected-rows-counter">
			{{ texts.allRowsSelected }}
		</span>
		<span
			v-else
			class="selected-rows-counter">
			{{ rowsSelectedCount }} {{ texts.textRowsSelected }}
		</span>
	</template>

	<q-table-group-actions-menu
		v-if="hasRowSelectActions"
		:rows-selected-count="rowsSelectedCount"
		:group-actions="groupActions"
		:texts="texts"
		@group-action="$emit('group-action', $event)" />

	<!-- BEGIN: Record count -->
	<div
		v-if="showRecordCount"
		class="e-counter">
		<q-icon icon="counter" />

		<span class="e-counter__text">
			{{ rowCount }}
		</span>
	</div>
	<!-- END: Record count -->

	<!-- BEGIN: Pagination row -->
	<div :class="paginationClasses">
		<q-table-pagination-alt
			v-if="showAlternatePagination"
			ref="paginationAlt"
			:texts="texts"
			:page="page"
			:per-page="perPage"
			:per-page-options="perPageOptions"
			:total="rowCount"
			:has-more-pages="hasMore"
			:show-per-page-menu="showPerPageMenu"
			:per-page-label="perPageLabel"
			:disabled="disabled"
			:table-id="tableId"
			@update:page="$emit('update:page', $event)"
			@update:per-page="$emit('update:perPage', $event)" />
		<q-table-pagination
			v-else
			:texts="texts"
			:page="page"
			:per-page="perPage"
			:per-page-options="perPageOptions"
			:total="rowCount"
			:num-visible-pagination-buttons="numVisiblePaginationButtons"
			:show-per-page-menu="showPerPageMenu"
			:per-page-label="perPageLabel"
			:disabled="disabled"
			:table-id="tableId"
			@update:page="$emit('update:page', $event)"
			@update:per-page="$emit('update:perPage', $event)" />
	</div>
	<!-- END: Pagination row -->

	<!-- BEGIN: Pagination info -->
	<div
		v-if="selectedRowsInfo && isSelectable"
		class="col-md-4">
		<div class="text-right justify-content-center">
			<slot
				name="selected-rows-info"
				:selected-items-count="selectedItemsCount">
				{{ selectedItemsCount }} {{ texts.textRowsSelected }}
			</slot>
		</div>
	</div>
	<!-- END: Pagination info -->

	<!-- BEGIN: Row general action buttons -->
	<slot />
	<!-- END: Row general action buttons -->

	<!-- BEGIN: Limit information button -->
	<q-table-limit-info
		v-if="showLimits"
		:limits="tableLimits"
		:table-id="tableId"
		:table-name-plural="tableNamePlural"
		:texts="texts" />
	<!-- END: Limit information button -->
</template>

<script>
	import QTableGroupActionsMenu from './QTableGroupActionsMenu.vue'
	import QTablePagination from './QTablePagination.vue'
	import QTablePaginationAlt from './QTablePaginationAlt.vue'
	import QTableLimitInfo from './QTableLimitInfo.vue'

	export default {
		name: 'QTableFooter',

		emits: ['update:page', 'update:perPage', 'group-action'],

		components: {
			QTableGroupActionsMenu,
			QTablePagination,
			QTablePaginationAlt,
			QTableLimitInfo
		},

		props: {
			/**
			 * The text alignment for pagination controls within the footer component.
			 */
			paginationPlacement: {
				type: String,
				default: 'left'
			},

			/**
			 * Flag to determine if the alternative pagination component should be displayed.
			 */
			showAlternatePagination: {
				type: Boolean,
				default: false
			},

			/**
			 * Flag indicating if pagination controls should be displayed.
			 */
			pagination: {
				type: Boolean,
				default: false
			},

			/**
			 * The count of rows shown on the current page.
			 */
			currentPageRowsLength: {
				type: Number,
				default: 0
			},

			/**
			 * The current active page number.
			 */
			page: {
				type: Number,
				default: 0
			},

			/**
			 * The number of rows to display per page.
			 */
			perPage: {
				type: Number,
				default: 0
			},

			/**
			 * Flag to determine if the controls for selecting the number of rows per page should be shown.
			 */
			showPerPageMenu: {
				type: Boolean,
				default: false
			},

			/**
			 * Options to select the number of rows per page.
			 */
			perPageOptions: {
				type: Array,
				default: () => []
			},

			/**
			 * Flag indicating whether more pages are available.
			 */
			hasMore: {
				type: Boolean,
				default: false
			},

			/**
			 * The number of buttons to show in the pagination control for page numbers.
			 */
			numVisiblePaginationButtons: Number,

			/**
			 * Information about selected rows.
			 */
			selectedRowsInfo: {
				type: Boolean,
				default: false
			},

			/**
			 * Flag indicating if rows can be selected.
			 */
			isSelectable: {
				type: Boolean,
				default: false
			},

			/**
			 * The number of selected items.
			 */
			selectedItemsCount: {
				type: Number,
				default: 0
			},

			/**
			 * Flag to determine if the counter for selected rows should be shown.
			 */
			showRowsSelectedCount: {
				type: Boolean,
				default: false
			},

			/**
			 * Flag to determine if all rows are selected.
			 */
			allRowsSelected: {
				type: Boolean,
				default: false
			},

			/**
			 * The count of rows currently selected.
			 */
			rowsSelectedCount: {
				type: Number,
				default: 0
			},

			/**
			 * Flag to determine if a count of all rows should be displayed.
			 */
			showRecordCount: {
				type: Boolean,
				default: false
			},

			/**
			 * Localized text strings to be used within the component.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * The total number of rows, a combination of original and filtered counts.
			 */
			rowCount: {
				type: [String, Number],
				required: true
			},

			/**
			 * Flag to determine if group actions are available when rows are selected.
			 */
			hasRowSelectActions: {
				type: Boolean,
				default: false
			},

			/**
			 * An array of actions that can be applied to selected rows in groups.
			 */
			groupActions: {
				type: Array,
				default: () => []
			},

			/**
			 * Flag to determine if limit information should be shown, especially when showing a subset of data.
			 */
			showLimits: {
				type: Boolean,
				default: false
			},

			/**
			 * An array of limit options that are applied to the data set, such as restricting to specific records.
			 */
			tableLimits: {
				type: Array,
				default: () => []
			},

			/**
			 * The name of the table used for referencing in the limit information.
			 */
			tableTitle: {
				type: String,
				default: ''
			},

			/**
			 * The table ID.
			 */
			tableId: {
				type: String,
				default: ''
			},

			/**
			 * The plural form of the table name used in contexts such as messages and tooltips.
			 */
			tableNamePlural: {
				type: String,
				default: ''
			},

			/**
			 * Whether the footer content is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		computed: {
			paginationClasses() {
				const paginationClasses = ['flex-align-center', 'footer-pagination-row']

				if (this.paginationPlacement === 'right') paginationClasses.push('push-pagination-right')

				return paginationClasses
			},

			/**
			 * Label for rows per page
			 */
			perPageLabel() {
				return this.perPage < 0 ? '' : this.perPage.toString()
			}
		},

		methods: {
			/**
			 * Get index of starting row of current page
			 * @returns Number
			 */
			rowBegin() {
				return (this.page - 1) * this.currentPageRowsLength + 1
			},

			/**
			 * Get index of ending row of current page
			 * @returns Number
			 */
			rowEnd() {
				return this.page * this.currentPageRowsLength
			}
		}
	}
</script>
