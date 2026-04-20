<template>
	<!-- BEGIN: Columns Tab -->
	<q-row>
		<q-col>
			<q-switch
				:model-value="hasTextWrap"
				size="small"
				show-state-labels
				:label="texts.lineBreak"
				:true-label="texts.yesLabel"
				:false-label="texts.noLabel"
				@update:model-value="toggleTextWrap" />
		</q-col>
	</q-row>

	<q-table
		:id="tableConf.config.name"
		:rows="tableConf.rows"
		:columns="tableConf.columns"
		:config="tableConf.config"
		:texts="texts"
		@update-external="updateExternal"
		@set-row-index-property="setRowIndexProperty"
		@row-reorder="onTableListRowReorder">
		<template #[`column_visibility.prepend`]>
			<q-checkbox
				data-table-action-selected="false"
				tabindex="-1"
				:model-value="areAllVisible"
				:indeterminate="areSomeVisible"
				:title="!areAllVisible ? texts.selectAll : texts.deselectAll"
				:aria-label="!areAllVisible ? texts.selectAll : texts.deselectAll"
				@update:model-value="setAllColsVisibility" />
		</template>
		<template #order="{ cellValue, column, row }">
			<q-edit-numeric
				:key="domKey"
				data-testid="column-config-order"
				column-name="order"
				:value="cellValue"
				:table-name="tableName"
				:row-index="row.rowKey"
				:options="column"
				@update="onTableListRowReorder({ rowKey: row.rowKey, index: $event - 1 })" />
		</template>
		<template #visibility="{ cellValue, column, row }">
			<q-checkbox
				data-testid="column-config-visibility"
				data-table-action-selected="false"
				tabindex="-1"
				:model-value="!!cellValue"
				:aria-label="column.label"
				@update:model-value="setColumnVisibility(row, $event)" />
		</template>
	</q-table>

	<div class="q-table-config__visible-cols-counter">
		{{ texts.visibleColumnsText }}: {{ visibleColumns }} {{ texts.ofText.toLowerCase() }} {{ tableConf.rows.length }}

		<q-button
			id="invisible-col-help-btn"
			:title="texts.showHelp"
			class="btn-popover"
			variant="ghost"
			color="neutral">
			<q-icon icon="help" />
		</q-button>

		<q-popover
			anchor="#invisible-col-help-btn"
			:text="texts.invisibleColumnsHelpText" />
	</div>

	<q-row>
		<q-col>
			<q-button
				id="reset-column-config-btn"
				:label="texts.resetText"
				:title="texts.resetText"
				@click="resetColumnConfig">
				<q-icon icon="reset-definitions" />
			</q-button>
		</q-col>
	</q-row>
	<!-- END: Columns Tab -->
</template>

<script>
	import { computed, nextTick } from 'vue'

	import { deepUnwrap } from '@quidgest/clientapp/utils/deepUnwrap'
	import { displayMessage } from '@quidgest/clientapp/utils/genericFunctions'
	import { BooleanColumn, NumericColumn, TextColumn } from '@/mixins/listColumnTypes.js'
	import listFunctions from '@/mixins/listFunctions.js'

	import QTable from './QTable.vue'

	export default {
		name: 'QTableConfigColumns',

		emits: [
			'reset',
			'toggle-text-wrap',
			'update:columns'
		],

		components: {
			QTable
		},

		inheritAttrs: false,

		props: {
			/**
			 * An object with the texts that are used in the popup, allowing for localization of UI elements like titles and buttons.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * The name of the associated table.
			 */
			tableName: {
				type: String,
				default: ''
			},

			/**
			 * An array of column configuration objects used to toggle visibility and order within the table.
			 */
			columns: {
				type: Array,
				default: () => []
			},

			/**
			 * The list of filters applied to the table.
			 */
			filters: {
				type: Array,
				default: () => []
			},

			/**
			 * Indicates whether the table rows should be presented with text wrapping.
			 */
			hasTextWrap: {
				type: Boolean,
				default: false
			},

			/**
			 * The name of the column that has been designated as the default search column.
			 */
			defaultSearchColumnName: {
				type: String,
				required: true
			},

			/**
			 * The base path to the directory containing resources related to the column configuration component.
			 */
			resourcesPath: {
				type: String,
				required: true
			}
		},

		expose: [],

		data()
		{
			return {
				domKey: 0,
				tableConf: {
					rows: [],
					columns: [
						new NumericColumn({
							order: 1,
							name: 'order',
							label: computed(() => this.texts.orderText),
							sortOrder: 1,
							maxDigits: 4
						}),
						new TextColumn({
							order: 2,
							name: 'name',
							label: computed(() => this.texts.nameOfColumnText)
						}),
						new TextColumn({
							order: 3,
							name: 'defaultSearch',
							label: computed(() => this.texts.defaultKeywordSearchText),
							component: 'q-edit-radio',
							checkedValue: ''
						}),
						new BooleanColumn({
							order: 4,
							name: 'visibility',
							label: computed(() => this.texts.visibleText)
						})
					],
					config: {
						name: `${this.tableName}-column-config`,
						hasRowDragAndDrop: true,
						resourcesPath: this.resourcesPath
					}
				},
				defaultSearchColumnNameCfg: ''
			}
		},

		beforeUnmount()
		{
			if (this.tableConf.rows instanceof Array)
				this.tableConf.rows.length = 0
			this.tableConf = null
		},

		computed: {
			/**
			 * The number of currently visible columns
			 */
			visibleColumns()
			{
				return this.tableConf.rows.filter((column) => column.Fields.visibility).length
			},

			/**
			 * True if all columns are visible, false otherwise
			 */
			areAllVisible()
			{
				return this.visibleColumns === this.tableConf.rows.length
			},

			/**
			 * True if only some columns are visible, false otherwise
			 */
			areSomeVisible()
			{
				return this.visibleColumns > 0 && this.visibleColumns < this.tableConf.rows.length
			}
		},

		methods: {
			/**
			 * Set property in a row in the table object
			 * @param {string, number} index The row index
			 * @param {string} propertyName Property name
			 * @param {object} propertyValue Property value
			 */
			setRowIndexProperty(index, propertyName, propertyValue)
			{
				listFunctions.setRowIndexProperty(this.tableConf, index, propertyName, propertyValue)
			},

			/**
			 * Checks if any of the filtered columns was hidden, and, if so, asks the user to confirm the change.
			 * @param {object} row The changed row
			 */
			validateFilterColumns(row)
			{
				const hiddenFilterCols = listFunctions.getHiddenFilterColumns(this.columns, this.tableConf.rows, this.filters)
				if (hiddenFilterCols.length > 0)
				{
					let confirmed = false

					const buttons = {
						confirm: {
							label: this.texts.yesLabel,
							action: () => {
								confirmed = true
								this.updateColumns(hiddenFilterCols)
							}
						},
						cancel: {
							label: this.texts.noLabel
						}
					}
					const handlers = {
						onLeave: () => {
							if (!confirmed)
								this.tableConf.rows = this.getColumnsCfgRows()

							// After closing the dialog, return the focus back to the checkbox that opened it.
							if (typeof row === 'object')
								document.querySelector(`#${this.tableConf.config.name}_row-${row.Rownum - 1} [role="checkbox"]`)?.focus()
						}
					}

					displayMessage(this.texts.hideColumnConfirm, 'question', null, buttons, null, handlers)
				}
				else
					this.updateColumns()
			},

			/**
			 * Sets the visibility status of the specified row
			 * @param {object} row The changed row
			 * @param {boolean} isVisible Whether the column is visible
			 */
			setColumnVisibility(row, isVisible)
			{
				row.Fields.visibility = isVisible ? 1 : 0
				this.validateFilterColumns(row)
			},

			/**
			 * Sets the visibility status of all rows
			 */
			setAllColsVisibility()
			{
				const visibility = !this.areAllVisible ? 1 : 0
				this.tableConf.rows.forEach((r) => r.Fields.visibility = visibility)
				this.validateFilterColumns()
			},

			/**
			 * Called when updating the value of a cell
			 * @param row {Object}
			 * @param column {Object}
			 * @param value {Object}
			 */
			updateExternal(row, column, event)
			{
				if (column.name === 'defaultSearch')
					this.defaultSearchColumnNameCfg = event
			},

			/**
			 * Create column config array
			 * @returns Array
			 */
			getColumnsCfgRows()
			{
				const rows = []

				let thisIdx = 1,
					colOrder = 1

				for (const column of this.columns)
				{
					// Columns with a false show-when condition shouldn't be displayed here - visibility is determined by the condition, not the user
					if (!column.visibilityEval)
						continue

					const columnCfg = {
						Rownum: 0,
						Fields: {}
					}

					// Row value
					columnCfg.value = column.name

					// Column name
					columnCfg.Fields.name = column.label

					// Column order
					columnCfg.Fields.order = colOrder++

					// Column has default search option
					columnCfg.Fields.defaultSearch = false
					if (listFunctions.isSearchableColumn(column))
						columnCfg.Fields.defaultSearch = true

					// Column visibility
					columnCfg.Fields.visibility = listFunctions.isVisibleColumn(column) ? 1 : 0

					// RowKey and Rownum
					columnCfg.rowKey = columnCfg.Rownum = thisIdx++

					rows.push(columnCfg)
				}

				return rows
			},

			/**
			 * Set the default search column
			 */
			setDefaultSearchColumnName()
			{
				const defaultSearchColumn = this.tableConf.columns.find((x) => x.name === 'defaultSearch')
				defaultSearchColumn.checkedValue = this.defaultSearchColumnName
				this.defaultSearchColumnNameCfg = this.defaultSearchColumnName
			},

			/**
			 * Called when the columns are reordered
			 * @param event The event payload
			 */
			onTableListRowReorder(event)
			{
				const row = listFunctions.getRowByKeyPath(this.tableConf.rows, event?.rowKey),
					orderingColumn = this.tableConf.columns.find((col) => col.sortOrder > 0)

				listFunctions.setCellValue(row, orderingColumn, event.index + 1)
				this.tableConf.rows = listFunctions.reCalcCellOrder(this.tableConf.rows, row, orderingColumn)
				this.updateColumns()

				// We need to force the re-render of the order inputs, otherwise their value won't be updated.
				this.domKey++

				setTimeout(() => {
					// Focus on the row that was just changed.
					const focusIndex = Math.max(0, Math.min(event.index, this.tableConf.rows.length - 1))
					document.getElementById(`${this.tableConf.config.name}_row-${focusIndex}`)?.focus()
				}, 0)
			},

			/**
			 * Emits the event to toggle the text wrap
			 */
			toggleTextWrap()
			{
				this.$emit('toggle-text-wrap')
			},

			/**
			 * Reset the column configuration
			 */
			resetColumnConfig()
			{
				this.$emit('reset')
			},

			/**
			 * Emits the event to update the columns
			 * @param hiddenFilterCols The list of hidden filtered columns
			 */
			updateColumns(hiddenFilterCols = [])
			{
				let filters = null

				// Remove filters associated to hidden columns
				if (hiddenFilterCols.length > 0)
				{
					filters = deepUnwrap(this.filters)

					for (const column of hiddenFilterCols)
					{
						for (let i = 0; i < filters.length; i++)
						{
							const filter = filters[i]
							if (filter.field === listFunctions.columnFullName(column))
							{
								// If there are sub filters, the first sub filter becomes the main filter,
								// otherwise, the filter is just removed from the list
								if (filter.subFilters.length > 0)
									filters[i] = listFunctions.removeFirstFilterCondition(filter)
								else
									filters.splice(i--, 1)
							}
							else
							{
								for (let j = 0; j < filter.subFilters.length; j++)
								{
									const subFilter = filter.subFilters[j]
									if (subFilter.field === listFunctions.columnFullName(column))
										filter.subFilters.splice(j--, 1)
								}
							}
						}
					}
				}

				// Clone the columns and apply the changes to them
				const columns = this.columns.map((c) => c.clone())

				for (const columnCfg of this.tableConf.rows)
				{
					// Find column configuration data
					const currentColumn = columns.find((c) => c.name === columnCfg.value)
					if (currentColumn)
					{
						// Set column order and visibility
						currentColumn.order = columnCfg.Fields.order
						currentColumn.isVisible = !!columnCfg.Fields.visibility
					}
				}

				const payload = {
					columns: columns.toSorted((a, b) => a.order - b.order),
					defaultSearchColumn: this.defaultSearchColumnNameCfg,
					filters // Will be null if there are no changes
				}
				this.$emit('update:columns', payload)
			}
		},

		watch: {
			columns: {
				async handler()
				{
					this.tableConf.rows = this.getColumnsCfgRows()
					this.tableConf.config.perPage = this.tableConf.rows.length

					await nextTick()

					// Ensure the default search column is currently visible.
					this.defaultSearchColumnNameCfg = listFunctions.getDefaultSearchColumn(this.columns, this.defaultSearchColumnNameCfg)?.name ?? ''
				},
				deep: true,
				immediate: true
			},

			defaultSearchColumnName: {
				handler()
				{
					this.setDefaultSearchColumnName()
				},
				immediate: true
			},

			defaultSearchColumnNameCfg()
			{
				this.updateColumns()
			}
		}
	}
</script>
