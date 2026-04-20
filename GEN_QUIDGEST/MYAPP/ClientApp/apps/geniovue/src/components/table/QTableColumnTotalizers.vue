<template>
	<tr>
		<template
			v-for="column in columnsWithTotal"
			:key="column.name">
			<td
				v-if="canShowColumn(column)"
				:class="cellClasses(column)">
				<span v-if="canShowSelectedTotal(column) && allSelected">
					{{ formatColumnValueDisplay(column, column.total) + ' / ' }}
				</span>
				<span v-else-if="canShowSelectedTotal(column)">
					{{ formatColumnValueDisplay(column, column.selectedTotal) + ' / ' }}
				</span>
				<span>
					{{ formatColumnValueDisplay(column, column.total) }}
				</span>
			</td>
		</template>
	</tr>
</template>
<script>
	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
	import { getColumnTotalValueDisplay } from '@/mixins/listFunctions'
	import isEmpty from 'lodash-es/isEmpty'

	export default {
		name: 'QTableColumnTotalizers',

		inject: [
			'canShowColumn',
			'isActionsColumn',
			'isExtendedActionsColumn',
			'isChecklistColumn',
			'isDragAndDropColumn',
			'isDataColumn'
		],

		expose: [],

		data() {
			return {
				selectedRowsOffset: [], // The array of offsets to apply to the totalizer.Selected server values, handling row selection without making requests.
				selectNone: false // True if the last selection was the "Select None" action. This is necessary, as not all selected rows are stored in the rows array.
			}
		},

		props: {
			/**
			 * The configuration of the table columns, including header names, keys, and additional properties.
			 */
			columns: {
				type: Array,
				required: true
			},

			/**
			 * The sum of all records in the table (for columns where the totalizer is active).
			 */
			totalizers: {
				type: Array,
				required: true
			},

			/**
			 * True for multiple selection lists, false otherwise.
			 */
			multipleSelection: {
				type: Boolean,
				default: false
			},

			/**
			 * Rows of the current page of the table.
			 */
			rows: {
				type: Array,
				default: () => []
			},

			/**
			 * Hashmap of the table's selected rows.
			 */
			selectedRows: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Index of the table's current page.
			 */
			currentPage: {
				type: Number,
				default: 1
			},

			/**
			 * True if all records are selected, false otherwise (comes from the table component).
			 */
			allSelected: {
				type: Boolean,
				default: false
			}
		},

		mounted() {
			this.initSelectedRowsOffset()
		},

		computed: {
			/**
			 * The columns array, with two added properties: total, the sum of all rows, and selected, the sum of the selected rows.
			 */
			columnsWithTotal() {
				return this.columns.map(column => {
					if (!this.isDataColumn(column)) {
						return { ...column, total: '', selectedTotal: '' }
					}

					const key = genericFunctions.formatColumnIdentifier(column.area, column.field)
					const totalizer = this.totalizers.find(t => t.Column === key) || {}

					return {
						...column,
						total: !isEmpty(totalizer) ? totalizer.Total : '',
						selectedTotal: !isEmpty(totalizer) ? this.getSelectedTotalValue(totalizer) : ''
					}
				})
			},

			/**
			 * Array of keys corresponding to currently selected rows.
			 */
			selectedRowsArray() {
				return Object.keys(this.selectedRows)
			}
		},

		methods: {
			/**
			 * Computes the css classes of the cell.
			 * @param column {Object} The column of the table.
			 * @returns the array of classes.
			 */
			cellClasses(column) {
				if (column.isTotalizerTitle) return 'q-table-list__column-totalizers--title'
			},

			/**
			 * Method needed to call getColumnTotalValueDisplay from listFunctions in the HTML template of the component.
			 * @param column {Object} The column of the table.
			 * @param total {Number} The value to display, in its numeric format.
			 * @returns the value to display in the totalizers row, in a string format.
			 */
			formatColumnValueDisplay(column, total) {
				return getColumnTotalValueDisplay(column, total)
			},

			/**
			 * Method that initializes the array used to add the newly selected rows (client-side values) to the server-side "Selected" value of the totalizer.
			 * Only for multiple selection lists.
			 */
			initSelectedRowsOffset() {
				this.selectedRowsOffset = this.totalizers.map(totalizer => ({
					column: totalizer.Column,
					offset: 0.0
				}))
			},

			/**
			 * True if the column should show the selected rows totalizer, false otherwise.
			 * @param column {Object} The column of the table.
			 * Only for multiple selection lists.
			 */
			canShowSelectedTotal(column) {
				return this.multipleSelection && this.isDataColumn(column) && column.totalizer
			},

			/**
			 * Handles row selection or deselection and determines the changed row IDs.
			 * Only for multiple selection lists.
			 * @param previousRows {Array} The previously selected rows.
			 * @param newRows {Array} The updated array of selected rows.
			 */
			onSelectedRowsChange(previousRows, newRows) {
				// Determine if it's a selection or deselection
				const isSelection = newRows.length > previousRows.length

				const changedRowsId = this.getChangedRows(previousRows, newRows)
				this.updateSelectedRowsOffset(changedRowsId, isSelection)
			},

			/**
			 * Determines the selected/deselected row IDs.
			 * Only for multiple selection lists.
			 * @param previousRows {Array} The previously selected rows.
			 * @param newRows {Array} The updated array of selected rows.
			 * @returns {Array} The array of IDs corresponding to the selected/deselected rows.
			 */
			getChangedRows(oldRows, newRows) {
				return newRows.length > oldRows.length
					? newRows.filter(rowId => !oldRows.includes(rowId))  // Rows added
					: oldRows.filter(rowId => !newRows.includes(rowId))  // Rows removed
			},

			/**
			 * Updates the offsets of the totalizers based on selected/deselected rows.
			 * Only for multiple selection lists.
			 * @param rowIds {string[]} The IDs of the altered rows.
			 * @param isSelection {boolean} Whether the rows were selected (true) or deselected (false).
			 */
			updateSelectedRowsOffset(rowIds, isSelection) {
				const changedRows = this.rows.filter(row => rowIds.includes(row.rowKey))

				// If the changed rows array is shorter than the rowIds, changes were made in another page of the table - "Select None" (since "Select All" doesn't affect offsets)
				if (changedRows.length < rowIds.length && !this.selectNone) {
					this.handleSelectNone()
					this.selectNone = true
				}
				else {
					this.updateOffsets(changedRows, isSelection)
					this.selectNone = false
				}

			},

			/**
			 * Method that alters the totalizer offsets when using the table's "Select None" action.
			 * Only for multiple selection lists.
			 */
			handleSelectNone() {
				this.selectedRowsOffset.forEach(totalizerOffset => {
					const serverSelected = this.totalizers.find(t => t.Column === totalizerOffset.column).Selected
					totalizerOffset.offset = -serverSelected // Nullify server selection value after "Select None"
				})
			},

			/**
			 * Method that alters the totalizer offsets when selecting/deselecting rows.
			 * Only for multiple selection lists.
			 * @param rows {Object[]} The altered rows.
			 * @param isSelection {boolean} Whether the rows were selected (true) or deselected (false).
			 */
			updateOffsets(rows, isSelection) {
				rows.forEach(row => {
					this.selectedRowsOffset.forEach(totalizerOffset => {
						const [area, field] = totalizerOffset.column.split('.')
						const column = this.columns.find(
							col =>
								this.isDataColumn(col) &&
								col.area.toLowerCase() === area.toLowerCase() &&
								col.field.toLowerCase() === field.toLowerCase()
						)
						const offsetValue = row.Fields[column.name]
						totalizerOffset.offset += isSelection ? offsetValue : -offsetValue
					})
				})
			},

			/**
			 * Computes the "Selected Total" value of a column, including server value and offset.
			 * Only for multiple selection lists.
			 * @param totalizer {Object} The totalizer for the column.
			 * @returns {number} The computed total value for selected rows.
			 */
			getSelectedTotalValue(totalizer) {
				const columnOffset = this.selectedRowsOffset.find(offset => offset.column === totalizer.Column)?.offset || 0
				return totalizer.Selected + columnOffset
			}
		},

		watch: {
			/**
			 * Resets the offset array after changing pages on the table.
			 * Only for multiple selection lists.
			 */
			currentPage() {
				if (this.multipleSelection)
					this.initSelectedRowsOffset()
			},

			/**
			 * Reacts to row selection and updates the offset array accordingly.
			 * Only for multiple selection lists.
			 */
			selectedRowsArray(newValue, oldValue) {
				if (this.multipleSelection)
					this.onSelectedRowsChange(oldValue, newValue)
			}
		}
	}
</script>
