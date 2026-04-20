import has from 'lodash-es/has'
import get from 'lodash-es/get'

import listFunctions from '@/mixins/listFunctions'

/**
 * Gets the file name without path and extension.
 * @param {string} fileName The file name
 * @returns Just the file name
 */
export function extractFilename(fileName)
{
	const path = fileName.split('/');
	const namelist = path[path.length - 1].split('.')
	return namelist[0]
}

/**
 * Get value of cell data
 * @param row {Object}
 * @param column {Object}
 * @returns any type
 */
export function getValueFromRow(row, column)
{
	return get(row.Fields, column.name)
}

/**
 * Get cell slot name
 * @param column {Object}
 * @returns String
 */
export function getCellSlotName(column)
{
	if (has(column, 'slotName'))
		return column.slotName
	return column.name.replace(/\./g,'_')
}

/**
 * Determine if column is a drag and drop column
 * @param column {Object}
 * @returns Boolean
 */
export function isDragAndDropColumn(column)
{
	return column.isDragAndDrop || false
}

/**
 * Determine if column is visible
 * @param column {Object}
 * @returns Boolean
 */
export function canShowColumn(column, hasRowDragAndDrop)
{
	//FOR: DRAG AND DROP COLUMNS
	if (column.isDragAndDrop && !hasRowDragAndDrop)
		return false
	//For all columns
	return listFunctions.isVisibleColumn(column)
}

/**
 * Determine if column is sortable
 * @param column {Object}
 * @returns Boolean
 */
export function isSortableColumn(column)
{
	return listFunctions.isSortableColumn(column)
}

/**
 * Determine if column is searchable
 * @param column {Object}
 * @returns boolean
 */
export function isSearchableColumn(column)
{
	return listFunctions.isSearchableColumn(column)
}

/**
 * Determine if column is an actions column
 * @param column {Object}
 * @returns Boolean
 */
export function isActionsColumn(column)
{
	return column.isActions || false
}

/**
 * Determine if column is an extended actions column
 * @param action {Object}
 * @returns Boolean
 */
export function isExtendedActionsColumn(column)
{
	return column.isExtendedActions || false
}

/**
 * Determine if column is an checklist column
 * @param column {Object}
 * @returns Boolean
 */
export function isChecklistColumn(column)
{
	return column.isChecklist || false
}

/**
 * Determine if column is a totalizer title column
 * @param column {Object}
 * @returns Boolean
 */
export function isTotalizerColumn(column) {
	return column.isTotalizer ?? false
}

/**
 * Determine if row is checked in checklist column
 * @param row {Object}
 * @returns Boolean
 */
export function isRowChecked(row, rowsChecked = {})
{
	for (const x in rowsChecked)
		if (x.toString() === row.rowKey.toString())
			return true
	return false
}

/**
 * Determine if row is in a valid state
 * @param row {Object}
 * @returns String
 */
export function rowIsValid(row, rowValidation = undefined)
{
	return rowValidation && !rowValidation.fnValidate(row) ? false : true
}

/**
 * Get row CSS classes
 * @param row {Object}
 * @returns String
 */
export function getRowClasses(row, columnsLevel = 0, rowValidation = undefined)
{
	if (!rowIsValid(row))
		return rowValidation.class
	return columnsLevel > 0 ? 'q-tree-table-row' : ''
}

/**
 * Get row title (for title attribute)
 * @param row {Object}
 * @returns String
 */
export function getRowTitle(row, rowValidation = undefined)
{
	if (!rowIsValid(row))
		return rowValidation.message
	return ''
}

/**
 * Determine if cell data has associated action
 * @param column {Object}
 * @returns Boolean
 */
export function hasDataAction(column)
{
	return get(column, 'cellAction', false)
}

/**
 * Determine if extended actions array has action passed
 * @param action {Object}
 * @returns Boolean
 */
export function hasExtendedAction(action, extendedActions = undefined)
{
	return extendedActions.includes(action)
}

/**
 * Get formatted string representing cell value. Calls formatting function based on column data type.
 */
export function getCellDataDisplay()
{
	return false
}

/**
 * Get string for title attribute of each cell in a row
 * @param row {Object}
 * @param columns {Object}
 * @param options {Object} [optional]
 * @returns String
 */
export function getRowCellDataTitles(table, row, columns, options, texts)
{
	const cellTitles = {}

	if (options !== undefined)
		options = {}

	for (const col in columns)
	{
		const column = columns[col]

		if (this.isDragAndDropColumn(column))
			cellTitles[column.name] = texts.rowDragAndDropTitle
		else
			cellTitles[column.name] = listFunctions.getCellValueDisplay(table, row, column, options)
	}

	return cellTitles
}

/**
 * Determine if row is selected
 * @param row {Object}
 * @returns Boolean
 */
export function isRowSelected(row, rowsSelected)
{
	for (const x in rowsSelected)
		if (x.toString() === row.rowKey.toString())
			return true
	return false
}

/**
 * Toggle selecting/deselecting single row
 * @param row {Object}
 */
export function toggleRowSelectSingle(row)
{
	let val = ''
	//If row is already selected, remove from selected rows
	if (this.isRowSelected(row))
		val += 'unselect-row'
	else
		val = ['unselect-all-rows', 'select-row']
	return val
}

/**
 * Toggle selecting/deselecting row (add to or remove from selected rows array)
 * @param row {Object}
 */
export function toggleRowSelectMultiple(row)
{
	//If row is already selected, remove from selected rows
	let val = ''
	if (this.isRowSelected(row))
		val += 'unselect-row'
	else
		val +='select-row'
	return val
}

/**
 * Creating copy of parent row & removing children array
 * @param row {Object}
 */
export function rowWithoutChildren(row)
{
	const newRow = { ...row }
	delete newRow.children
	return newRow
}

/**
 * Do action when clicking on row: default click action (emit) or select row
 * @param row {Object}
 */
export function executeRowClickAction(row, rowClickActionInternal = undefined, rowClickAction = undefined)
{
	//Remove child rows
	row = this.rowWithoutChildren(row)

	//Row actions that do not emit data outside of the component
	switch (rowClickActionInternal)
	{
		case 'selectSingle':
			this.toggleRowSelectSingle(row)
			break
		case 'selectMultiple':
			this.toggleRowSelectMultiple(row)
			break
		default:
			break
	}

	//Execute default row action
	if (Object.keys(rowClickAction).length > 0)
		return { id: this.rowClickAction.id, rowKey: row.rowKey }
	return { rowKey: row.rowKey }
}

export default {
	extractFilename,
	getValueFromRow,
	getCellSlotName,
	isDragAndDropColumn,
	canShowColumn,
	isSortableColumn,
	isSearchableColumn,
	isActionsColumn,
	isExtendedActionsColumn,
	isChecklistColumn,
	isTotalizerColumn,
	isRowChecked,
	rowIsValid,
	getRowClasses,
	getRowTitle,
	hasDataAction,
	hasExtendedAction,
	getCellDataDisplay,
	getRowCellDataTitles,
	isRowSelected,
	toggleRowSelectSingle,
	toggleRowSelectMultiple,
	rowWithoutChildren,
	executeRowClickAction
}
