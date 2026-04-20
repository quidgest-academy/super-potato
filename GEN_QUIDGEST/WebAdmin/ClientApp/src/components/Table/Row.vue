<template>
	<tr
		ref="vbt_row"
		:data-id="rowId"
		:class="rowClasses"
		@click="handleRowSelect">
		<td
			v-if="checkboxRows"
			class="checkbox-column">
			<q-checkbox
				v-model="rowSelected"
				class="q-checkbox--table"
				@update:model-value="emitSelectValue" />
		</td>
		<template v-for="(column, key) in columns">
			<td
				v-if="canShowColumn(column)"
				:key="key"
				:class="cellClasses(column)">
				<slot :name="'vbt-' + getCellSlotName(column)"> </slot>
			</td>
		</template>
	</tr>
</template>

<script>
	import { has, get, differenceWith, isEqual, includes } from 'lodash-es'

	import { isDefined } from '@/utils/common'

	export default {
		name: 'QTableRow',

		props: {
			/**
			 * Object that contains all necessary information regarding a row of the table.
			 */
			row: {
				type: Object,
				required: true
			},

			/**
			 * Row-specific classes.
			 */
			propRowClasses: {
				type: [Object, String],
				default: ''
			},

			/**
			 * Cell-specific classes.
			 */
			propCellClasses: {
				type: [Object, String],
				default: ''
			},

			/**
			 * List of row columns.
			 */
			columns: {
				type: Array,
				default: () => []
			},

			/**
			 * The row's unique identifier.
			 */
			uniqueId: {
				type: [Number, String],
				default: undefined
			},

			/**
			 * Array of the table's selected records.
			 */
			selectedItems: {
				type: Array,
				default: () => []
			},

			/**
			 * True if the table has a checkbox column, false otherwise.
			 */
			checkboxRows: {
				type: Boolean,
				default: false
			},

			/**
			 * True if hovering a row should change its color, false otherwise.
			 */
			highlightRowHover: {
				type: Boolean,
				default: false
			},

			/**
			 * Row-hover color.
			 */
			highlightRowHoverColor: {
				type: String,
				default: '#d6d6d6'
			},

			/**
			 * The index of the row within the table's records.
			 */
			rowIndex: {
				type: Number,
				required: true
			},

			/**
			 * True if the row has an action associated with it, false otherwise.
			 */
			singleRowSelectable: {
				type: Boolean,
				default: false
			}
		},

		emits: ['add-row', 'remove-row', 'single-row-select'],

		expose: [],

		data() {
			return {
				rowSelected: false,
				rowHiglighted: false
			}
		},
		computed: {
			rowClasses() {
				let classes = this.userRowClasses

				if (this.rowSelected) {
					classes += ' '
					classes += 'vbt-row-selected'
				}

				if (this.singleRowSelectable) {
					classes += ' clickable'
				}

				classes += this.rowHiglighted ? ' highlighted' : ''

				return classes
			},
			userRowClasses() {
				let classes = ''
				if (typeof this.propRowClasses === 'string') {
					return this.propRowClasses
				} else if (typeof this.propRowClasses === 'object') {
					Object.entries(this.propRowClasses).forEach(([key, value]) => {
						if (typeof value === 'boolean' && value) {
							classes += key
						} else if (typeof value === 'function') {
							const truth = value(this.row)
							if (typeof truth === 'boolean' && truth) {
								classes += ' '
								classes += key
							}
						}
					})
				}

				return classes
			},
			rowId() {
				if (!isDefined(this.uniqueId)) return 'vbt_id'

				return this.getValueFromRow(this.row, this.uniqueId)
			}
		},
		watch: {
			row: {
				handler(newVal) {
					this.checkInSelecteditems(this.selectedItems, newVal)
				},
				deep: true
			}
		},
		mounted() {
			if (this.highlightRowHover) {
				this.$refs.vbt_row.addEventListener('mouseover', () => {
					this.rowHiglighted = true
				})
				this.$refs.vbt_row.addEventListener('mouseleave', () => {
					this.rowHiglighted = false
				})
			}
			this.checkInSelecteditems(this.selectedItems, this.row)
		},
		methods: {
			emitSelectValue(newVal) {
				if (newVal) this.addRow(false)
				else this.removeRow(false)
			},

			addRow(shiftKey) {
				this.$emit('add-row', { shiftKey: shiftKey, rowIndex: this.rowIndex })
			},

			removeRow(shiftKey) {
				this.$emit('remove-row', {
					shiftKey: shiftKey,
					rowIndex: this.rowIndex
				})
			},

			handleRowSelect() {
				if (this.singleRowSelectable)
					this.$emit('single-row-select', { rowIndex: this.rowIndex })
			},

			// compare the selected items list with current row item and update checkbox accordingly
			checkInSelecteditems(selectedItems, row) {
				if (!this.checkboxRows) return

				if (isDefined(this.uniqueId)) {
					this.rowSelected = selectedItems.some(
						(item) => item[this.uniqueId] === row[this.uniqueId]
					)
					return
				}

				const difference = differenceWith(selectedItems, [row], isEqual)
				const isSelected = difference.length !== selectedItems.length
				this.rowSelected = isSelected
			},

			rowHover(state) {
				this.rowHiglighted = state
			},

			getValueFromRow(row, name) {
				return get(row, name)
			},

			cellClasses(column) {
				let classes = ''

				const default_text_alignment = 'text-left'

				//decide text alignment class - starts here
				const alignments = ['text-justify', 'text-right', 'text-left', 'text-center']
				if (
					has(column, 'row_text_alignment') &&
					includes(alignments, column.row_text_alignment)
				) {
					classes = classes + ' ' + column.row_text_alignment
				} else {
					classes = classes + ' ' + default_text_alignment
				}
				//decide text alignment class - ends here

				// adding user defined classes from column config to rows - starts here
				if (has(column, 'row_classes')) {
					classes = classes + ' ' + column.row_classes
				}
				// adding user defined classes from column config to rows - ends here

				if (typeof this.propCellClasses === 'string') {
					return this.propCellClasses
				} else if (typeof this.propCellClasses === 'object') {
					Object.entries(this.propCellClasses).forEach(([key, value]) => {
						if (typeof value === 'boolean' && value) {
							classes += ' ' + key
						} else if (typeof value === 'function') {
							const truth = value(
								this.row,
								column,
								this.getValueFromRow(this.row, column.name)
							)
							if (typeof truth === 'boolean' && truth) {
								classes += ' '
								classes += key
							}
						}
					})
				}

				return classes
			},

			getCellSlotName(column) {
				if (has(column, 'slot_name')) {
					return column.slot_name
				}

				return column.name.replace(/\./g, '_')
			},

			canShowColumn(column) {
				return column.visibility === undefined || column.visibility ? true : false
			}
		}
	}
</script>
