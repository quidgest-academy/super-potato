<template>
	<nav
		v-if="hasMultiplePages"
		aria-label="Page navigation">
		<q-button-group>
			<!-- BEGIN: Page navigation buttons -->
			<!-- BEGIN: First page button -->
			<q-button
				:aria-label="texts.first"
				:disabled="!prevButtonEnabled"
				@click="pageHandler(1)">
				<q-icon icon="page-first" />
			</q-button>
			<!-- END: First page button -->
			<!-- BEGIN: Previous page button -->
			<q-button
				:aria-label="texts.previous"
				:disabled="!prevButtonEnabled"
				@click="pageHandler(page - 1)">
				<q-icon icon="page-previous" />
			</q-button>
			<!-- END: Previous page button -->
			<!-- BEGIN: Visible page number buttons -->
			<template v-if="totalPages > 1">
				<q-toggle
					v-for="index in range"
					:key="index"
					:model-value="index === page"
					:label="index"
					:disabled="disabled || index === page"
					:class="pageButtonClass(index, page)"
					@click="pageHandler(index)" />
			</template>
			<!-- END: Visible page number buttons -->
			<!-- BEGIN: Next page button -->
			<q-button
				:aria-label="texts.next"
				:disabled="!nextButtonEnabled"
				@click="pageHandler(page + 1)">
				<q-icon icon="page-next" />
			</q-button>
			<!-- END: Next page button -->
			<!-- BEGIN: Last page button -->
			<q-button
				:aria-label="texts.last"
				:disabled="!nextButtonEnabled"
				@click="pageHandler(totalPages)">
				<q-icon icon="page-last" />
			</q-button>
			<!-- END: Last page button -->
			<!-- END: Page navigation buttons -->
		</q-button-group>
	</nav>

	<!-- BEGIN: Number of rows per page -->
	<template v-if="showPerPageMenu && total > 0">
		<span class="i-text__label">{{ texts.rowsPerPage + ':' }}</span>
		<q-select
			:id="`${tableId}-rowspp-menu`"
			:aria-label="texts.rowsPerPage + ':'"
			:disabled="disabled"
			:items="perPageOptionsObj"
			:model-value="perPage"
			:texts="texts"
			size="mini"
			@update:model-value="perPageHandler" />
	</template>
	<!-- END: Number of rows per page -->
</template>

<script>
	import range from 'lodash-es/range'

	import listFunctions from '@/mixins/listFunctions.js'

	export default {
		name: 'QTablePagination',

		emits: ['update:page', 'update:perPage'],

		props: {
			/**
			 * The current page number displayed and managed by the pagination component.
			 */
			page: {
				type: [String, Number],
				required: true
			},

			/**
			 * The number of items (rows) to display on each page.
			 */
			perPage: {
				type: [String, Number],
				required: true
			},

			/**
			 * The total number of items (rows) available across all pages.
			 */
			total: {
				type: [String, Number],
				required: true
			},

			/**
			 * The number of visible page buttons to be displayed in the pagination component at any given time.
			 */
			numVisiblePaginationButtons: {
				type: [String, Number],
				default: 3
			},

			/**
			 * Options determining the available selections for items per page.
			 */
			perPageOptions: {
				type: Array,
				default: () => []
			},

			/**
			 * Flag indicating whether to show the dropdown menu for selecting the number of items per page.
			 */
			showPerPageMenu: {
				type: Boolean,
				default: false
			},

			/**
			 * Text label accompanying the per page options dropdown; it typically reflects the currently selected per-page value.
			 */
			perPageLabel: {
				type: String,
				default: ''
			},

			/**
			 * Localized text strings to be used for pagination-related content such as button labels.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * Whether the pagination is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * The table ID.
			 */
			tableId: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data() {
			return {
				start: this.page + 0,
				end: 0,
				goToPage: ''
			}
		},

		mounted() {
			this.calculatePageRange(true)
		},

		computed: {
			totalPages() {
				return Math.ceil(this.total / this.perPage)
			},

			hasMultiplePages() {
				return this.totalPages > 1
			},

			range() {
				return range(this.start, this.end + 1)
			},

			prevButtonEnabled() {
				return this.totalPages > 1 && this.page !== 1 && !this.disabled
			},

			nextButtonEnabled() {
				return this.totalPages > 1 && this.page < this.totalPages && !this.disabled
			},

			perPageOptionsObj() {
				return listFunctions.getPerPageOptions(this.perPageOptions)
			}
		},

		methods: {
			/**
			 * Emit event to update page number (built-in method)
			 * @param index {Number}
			 */
			pageHandler(index) {
				if (!this.disabled && index >= 1 && index <= this.totalPages) {
					this.$emit('update:page', index)
				}
			},

			/**
			 * Emit event to update number of rows per page (built-in method)
			 * @param {number} option
			 */
			perPageHandler(option) {
				if (!this.disabled && typeof option === 'number')
					this.$emit('update:perPage', option)
			},

			/**
			 * Calculate start od end pages for visible page buttons
			 */
			calculatePageRange() {
				//Skip calculating if all pages can be shown
				if (this.totalPages <= this.numVisiblePaginationButtons) {
					this.start = 1
					this.end = this.totalPages
					return
				}

				//Calculate start of range
				this.start = this.page - Math.floor(this.numVisiblePaginationButtons / 2)
				this.start = Math.max(this.start, 1)

				//Calculate end of range
				this.end = this.start + this.numVisiblePaginationButtons - 1
				if (this.end > this.totalPages) {
					this.end = this.totalPages
					this.start = this.end - this.numVisiblePaginationButtons + 1
				}
			},

			/**
			 * Get the class for the paging button
			 * @param index {Number} Index of the button
			 * @param page {Number} Active page number
			 * @returns String
			 */
			pageButtonClass(index, page) {
				// Difference between the index and active page number
				const diff = Math.abs(index - page)

				// Index is the active page
				if(diff === 0)
					return null
				// Index is next to the active page
				else if(diff === 1)
					return 'btn-page-adjacent'
				// Index is farther from the active page
				return 'btn-page-other'
			}
		},

		watch: {
			page() {
				this.calculatePageRange()
			},

			rowCount() {
				this.calculatePageRange()
			},

			totalPages() {
				this.calculatePageRange()
			}
		}
	}
</script>
