<template>
	<div>
		<nav id="pagination">
			<q-button-group>
				<!-- BEGIN: Page navigation buttons -->
				<!-- BEGIN: First page button -->
				<q-button
					aria-label="First"
					size="small"
					:disabled="!prevButtonEnabled"
					@click="goToFirstPage">
					<q-icon icon="chevron-double-left" />
				</q-button>
				<!-- END: First page button -->
				<!-- BEGIN: Previous page button -->
				<q-button
					aria-label="Previous"
					size="small"
					:disabled="!prevButtonEnabled"
					@click="goToPreviousPage">
					<q-icon icon="chevron-left" />
				</q-button>
				<!-- END: Previous page button -->
				<!-- BEGIN: Visible page number buttons -->
				<template v-if="totalPages">
					<q-button
						v-for="index in range"
						:key="index"
						size="small"
						:label="index"
						:active="index === page"
						:disabled="disabled"
						:class="pageButtonClasses(index, page)"
						@click="() => pageHandler(index)">
					</q-button>
				</template>
				<!-- END: Visible page number buttons -->
				<!-- BEGIN: Next page button -->
				<q-button
					aria-label="Next"
					size="small"
					:disabled="!nextButtonEnabled"
					@click="goToNextPage">
					<q-icon icon="chevron-right" />
				</q-button>
				<!-- END: Next page button -->
				<!-- BEGIN: Last page button -->
				<q-button
					aria-label="Last"
					size="small"
					:disabled="!nextButtonEnabled"
					@click="goToLastPage">
					<q-icon icon="chevron-double-right" />
				</q-button>
				<!-- END: Last page button -->
				<!-- END: Page navigation buttons -->
			</q-button-group>
		</nav>
	</div>
</template>

<script>
	import { range, includes } from 'lodash-es'

	export default {
		name: 'QTablePagination',

		props: {
			/**
			 * Current table page.
			 */
			page: {
				type: [String, Number],
				required: true
			},

			// TODO: Fix camelCase Lint warning -> requires server changes
			/**
			 * Number of records per page.
			 */
			per_page: {
				type: [String, Number],
				required: true
			},

			/**
			 * Number of records per page.
			 */
			total: {
				type: [String, Number],
				required: true
			},

			/**
			 * Number of visible table pagination buttons.
			 */
			numOfVisiblePaginationButtons: {
				type: [String, Number],
				default: 7
			},

			/**
			 * Whether the pagination is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			}
		},

		emits: ['update:page', 'update:per_page'],

		expose: [],

		data() {
			return {
				start: this.page + 0,
				end: 0
			}
		},

		computed: {
			totalPages() {
				return Math.ceil(this.total / this.per_page)
			},

			disablePreviousButton() {
				return this.page === this.start
			},

			disableNextButton() {
				return this.page === this.end
			},

			range() {
				return range(this.start, this.end + 1)
			},

			isEmpty() {
				return this.total === 0
			},

			prevButtonEnabled() {
				return this.totalPages > 1 && this.page !== 1 && !this.disabled
			},

			nextButtonEnabled() {
				return this.totalPages > 1 && this.page < this.totalPages && !this.disabled
			}
		},

		watch: {
			page() {
				this.calculatePageRange()
			},

			totalPages() {
				this.calculatePageRange()
			}
		},

		mounted() {
			this.calculatePageRange(true)
		},

		methods: {
			goToFirstPage() {
				this.pageHandler(1)
			},

			goToPreviousPage() {
				this.pageHandler(this.page - 1)
			},

			goToNextPage() {
				this.pageHandler(this.page + 1)
			},

			goToLastPage() {
				this.pageHandler(this.totalPages)
			},

			pageHandler(index) {
				if (index >= 1 && index <= this.totalPages) {
					this.$emit('update:page', index)
				}
			},

			perPageHandler(option) {
				this.$emit('update:per_page', option)
			},

			calculatePageRange(force = false) {
				//Skip calculating if all pages can be shown
				if (this.totalPages <= this.numOfVisiblePaginationButtons) {
					this.start = 1
					this.end = this.totalPages
					return
				}

				//Skip recalculating if the previous and next pages are already visible
				if (
					!force &&
					(includes(this.range, this.page - 1) || this.page === 1) &&
					(includes(this.range, this.page + 1) || this.page === this.totalPages)
				) {
					return
				}

				//Current page is the start page minus one
				this.start = this.page === 1 ? 1 : this.page - 1

				//Reserved entries: firstpage, ellipsis (2x), prev. page, last page, current page
				this.end = this.start + this.numOfVisiblePaginationButtons - 5

				//If the user navigates on page one or two, we set start to one (ellipsis pointless)
				//and can potentially shift up end
				if (this.start <= 3) {
					this.end += 3 - this.start
					this.start = 1
				}

				//If the user navigates on the last two pages or out of bounds, we can shift down start
				//This will also handle end overflow, substract 2 for ellipsis and last page
				if (this.end >= this.totalPages - 2) {
					this.start -= this.end - (this.totalPages - 2)
					this.end = this.totalPages
				}

				//Handle start underflow
				this.start = Math.max(this.start, 1)
			},

			isPositiveInteger(str) {
				return /^\+?(0|[1-9]\d*)$/.test(str)
			},

			/**
			 * Get the classes for the paging button
			 * @param index {Number} Index of the button
			 * @param page {Number} Active page number
			 * @returns String
			 */
			pageButtonClasses(index, page) {
				const classes = ['btn-page']
				// Difference between the index and active page number
				const diff = Math.abs(index - page)

				switch (diff) {
					case 1:
						// Index is next to the active page
						classes.push('btn-page-adjacent')
						break
					default:
						// If 0, index is the active page, else, index is farther from the active page
						if (diff !== 0) classes.push('btn-page-other')
				}

				return classes
			}
		}
	}
</script>
