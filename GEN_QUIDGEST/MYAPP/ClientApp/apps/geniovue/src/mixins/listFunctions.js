import cloneDeep from 'lodash-es/cloneDeep'
import _find from 'lodash-es/find'
import _findIndex from 'lodash-es/findIndex'
import _forEach from 'lodash-es/forEach'
import _get from 'lodash-es/get'
import has from 'lodash-es/has'
import _isEmpty from 'lodash-es/isEmpty'
import _map from 'lodash-es/map'
import _set from 'lodash-es/set'
import _toLower from 'lodash-es/toLower'
import _unionWith from 'lodash-es/unionWith'
import { isRef, markRaw, reactive, ref, shallowReactive, toValue, unref } from 'vue'

import searchFilterData from '@/api/genio/searchFilterData.js'
import { documentViewTypeMode, formModes, tableViewManagementModes } from '@quidgest/clientapp/constants/enums'
import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
import { geographicDisplay, geographicShapeDisplay } from '@quidgest/clientapp/utils/geography'
import { deepUnwrap } from '@quidgest/clientapp/utils/deepUnwrap'

import { useGlobalTablesDataStore } from '@/stores/globalTablesData.js'
import { useGenericDataStore } from '@quidgest/clientapp/stores'

/**
 * Hydrates the table data.
 * @param {object} listControl The list control object
 * @param {object} viewModel The list view model (C#)
 */
export function hydrateTableData(listControl, viewModel)
{
	if (typeof viewModel !== 'object' || viewModel === null)
		return

	// Global tables
	const globalTablesData = useGlobalTablesDataStore()
	globalTablesData.loadFromViewModel(viewModel)

	const newListRows = []

	// Default search column
	const defaultSearchColumn = getDefaultSearchColumn(listControl.columns, listControl.config.defaultSearchColumnName)
	if (defaultSearchColumn)
		listControl.config.defaultSearchColumnNameOriginal = defaultSearchColumn.name

	if (listControl.config.viewManagement === tableViewManagementModes.persistOne ||
		listControl.config.viewManagement === tableViewManagementModes.persistMany ||
		!_isEmpty(viewModel.currentTableConfig))
	{
		// Process table configuration (saved view or current configuration)
		if (!_isEmpty(viewModel.currentTableConfig))
			applyTableConfiguration(listControl, viewModel.currentTableConfig, viewModel.filtersModel)

		// List of users views (configurations)
		if (!_isEmpty(viewModel.tableConfigNames))
			listControl.config.tableConfigNames = viewModel.tableConfigNames
		else
			listControl.config.tableConfigNames = []

		// Set default table view configuration name
		if (!_isEmpty(viewModel.defaultTableConfigName))
			listControl.config.defaultTableConfigName = viewModel.defaultTableConfigName
	}

	if (typeof viewModel.table !== 'undefined')
	{
		// Row component properties
		if (listControl.rowComponent === 'q-form-container' && listControl.formName !== '')
		{
			listControl.rowFormProps = []
			listControl.type = 'Multiform'
		}

		// Rows data
		_forEach(viewModel.table.elements, (rowData, idx) => {
			newListRows.push(reactive(hydrateTableRow(listControl, rowData, idx)))

			// Multiform properties
			if (listControl.rowComponent === 'q-form-container' && listControl.formName !== '')
			{
				listControl.rowFormProps.push({
					historyBranchId: listControl.vueContext.navigationId,
					id: newListRows[idx].rowKey,
					component: listControl.component,
					mode: 'SHOW'
				})
			}
		})

		listControl.rows = shallowReactive(newListRows)

		// Pagination
		const p = viewModel.table.pagination
		if (p.hasTotal)
			listControl.totalRows = p.totalRows
		else
			listControl.totalRows = listControl.rows.length

		listControl.hasMorePages = p.hasMore
		listControl.page = p.pageNumber
		listControl.config.page = p.pageNumber

		// Totalizers
		listControl.columnTotalizers = viewModel.table.totalizers
	}

	// If table is a checklist
	if (listControl.type === 'MultipleValuesList')
	{
		if (listControl.rows !== undefined && listControl.rows !== null)
		{
			listControl.modelFieldRef.hydrate(viewModel[listControl.modelField])

			// Only keep selected records
			listControl.rows = listControl.rows.filter((rowKey) => listControl.rowsSelected[rowKey.rowKey])
		}

		// New values
		if (listControl.modelFieldOptionsRef)
		{
			// Set number of rows which is stored differently in checklists than in normal tables
			listControl.totalRows = viewModel[listControl.modelFieldOptions].length

			listControl.modelFieldOptionsRef.hydrate(viewModel[listControl.modelFieldOptions])

			_forEach(listControl.modelFieldOptionsRef.value, (rowData, rowIndex) => {
				const value = hydrateTableRow(listControl, rowData, rowIndex)

				// Check if the value is not in the list already
				if (!listControl.rows.some((obj) => obj.rowKey === value.rowKey))
					listControl.rows.push(value)
			})
		}

		// Selected rows
		if (listControl.modelFieldRef)
		{
			listControl.modelFieldRef.hydrate(viewModel[listControl.modelField])
			_forEach(listControl.modelFieldRef.value, (rowKey) => {
				listControl.rowsSelected[rowKey] = true
			})
		}
	}

	// Table limits display data
	if (typeof viewModel.tableLimitsDisplayData !== 'undefined')
		listControl.tableLimits = viewModel.tableLimitsDisplayData
}

/**
 * Get table configuration from table data
 * @param {object} listControl The list control object
 */
export function getTableConfiguration(listControl)
{
	const config = {}

	if (!_isEmpty(listControl.config.userTableConfigName))
		config.name = listControl.config.userTableConfigName

	// BEGIN: Column order and visibility
	const columns = []

	for (const column of listControl.columns ?? [])
	{
		columns.push({
			name: column.name,
			order: column.order,
			visibility: column.isVisible ? 1 : 0,
			exportability: column.export,
			sortOrder: column.sortOrder,
			sortAsc: column.sortAsc
		})
	}

	config.columnConfiguration = columns.toSorted((a, b) => a.order - b.order)
	// END: Column order and visibility

	config.filters = cloneDeep(deepUnwrap(listControl.filters))

	// BEGIN: Group filters
	for (const filter of listControl.groupFilters)
	{
		config.filters.push({
			type: 'GROUP',
			key: filter.id,
			value: Array.isArray(filter.selected) ? filter.selected.join('') : filter.selected
		})
	}
	// END: Group filters

	// BEGIN: Active Filters
	if (listControl.activeFilters)
	{
		const selected = listControl.activeFilters.selected
		config.filters.push({
			type: 'ACTIVE',
			date: (listControl.activeFilters.date.value ?? new Date()).toISOString(),
			current: selected.includes('current'),
			previous: selected.includes('previous'),
			upcoming: selected.includes('upcoming')
		})
	}
	// END: Active Filters

	// The global filter values
	config.globalFilters = listControl.relatedFilterValues

	// List of IDs that correspond to the selected rows
	config.selectedRows = getSelectedRows(listControl)

	if (listControl.config.defaultSearchColumnName)
		config.defaultSearchColumn = listControl.config.defaultSearchColumnName

	if (listControl.config.hasTextWrap !== undefined && listControl.config.hasTextWrap !== null)
		config.lineBreak = listControl.config.hasTextWrap

	if (listControl.config.perPage !== undefined && listControl.config.perPage !== null)
		config.rowsPerPage = listControl.config.perPage

	if (listControl.config.page !== undefined && listControl.config.page !== null)
		config.page = listControl.config.page

	if (!_isEmpty(listControl.activeViewModeId))
		config.activeViewMode = listControl.activeViewModeId

	return config
}

/**
 * Apply view configuration to table
 * @param {object} listControl The list control object
 * @param {object} viewCfg The view configuration object
 * @param {object} filtersModel The global filters view model
 */
export function applyTableConfiguration(listControl, viewCfg, filtersModel)
{
	// Configuration name
	listControl.config.userTableConfigName = viewCfg.name ?? ''

	// Column order and visibility
	if (!_isEmpty(viewCfg.columnConfiguration))
	{
		// Columns that are in the configuration
		const columnsOrdered = []
		// Columns that are not in the configuration
		const columnsUnordered = []

		// Iterate original columns so all columns are used, even columns that were added after the current configuration
		_forEach(listControl.columnsOriginal, (originalColumn) => {
			const idx = _findIndex(viewCfg.columnConfiguration, ['name', originalColumn.name])

			// Clone the column
			const currentColumn = originalColumn.clone?.() ?? cloneDeep(originalColumn)

			// Keep reactivity on computed properties
			Object.keys(currentColumn).forEach((key) => {
				const descriptor = Object.getOwnPropertyDescriptor(originalColumn, key)
				if (isRef(descriptor?.value))
					currentColumn[key] = descriptor.value
			})

			// If column is in the configuration
			if (idx >= 0)
			{
				// Get column configuration data and apply it
				const customColumn = viewCfg.columnConfiguration[idx]
				currentColumn.name = customColumn.name
				currentColumn.order = customColumn.order
				currentColumn.isVisible = !!customColumn.visibility
				currentColumn.sortOrder = customColumn.sortOrder
				currentColumn.sortAsc = customColumn.sortAsc

				// Add column at the corresponding index in the array (one less than the order value)
				columnsOrdered[idx] = currentColumn
			}
			// If column is not in the configuration (added later)
			else
				columnsUnordered.push(currentColumn)
		})

		// Set columns to columns configured by user and then add any columns that do not have the order set
		// Remove any empty elements
		listControl.columnsCustom = columnsOrdered.filter((col) => col !== undefined && col !== null).concat(columnsUnordered)
	}
	else
		listControl.columnsCustom = []

	const filters = deepUnwrap(viewCfg.filters)

	if (Array.isArray(filters))
	{
		const columnFilters = filters.filter((e) => e.type === 'COLUMN')
		const groupFilters = filters.filter((e) => e.type === 'GROUP')
		const activeFilter = filters.find((e) => e.type === 'ACTIVE')

		// Column filters
		listControl.filters = cloneDeep(columnFilters)

		// Group filters
		for (const filter of listControl.groupFilters)
		{
			const storedFilter = groupFilters.find((e) => e.key === filter.id)

			if (storedFilter)
			{
				filter.selected = filter.isMultiple
					? [...storedFilter.value]
					: storedFilter.value
			}
			else
				filter.selected = cloneDeep(filter.default)
		}

		// Active filter
		if (activeFilter)
		{
			const selected = []
			if (activeFilter.current)
				selected.push('current')
			if (activeFilter.previous)
				selected.push('previous')
			if (activeFilter.upcoming)
				selected.push('upcoming')
			listControl.activeFilters.selected = selected

			// Set date
			listControl.activeFilters.date.value = new Date(activeFilter.date)
		}
	}

	// Global filters
	if (!_isEmpty(filtersModel) && !_isEmpty(listControl.filtersModel))
	{
		listControl.filtersModel.clearValues()
		listControl.filtersModel.hydrate(filtersModel)
		listControl.filtersModel.version++
	}

	// Default search column
	listControl.config.defaultSearchColumnName = viewCfg.defaultSearchColumn ?? listControl.config.defaultSearchColumnNameOriginal

	// Initial sort
	if (!listControl.columns.some((c) => c.sortOrder > 0))
		resetTableSorting(listControl)

	// Line break
	listControl.config.hasTextWrap = viewCfg.lineBreak

	// Rows per page
	listControl.config.perPageSelected = viewCfg.rowsPerPage ?? listControl.config.perPageDefault

	// View mode
	listControl.activeViewModeId = viewCfg.activeViewMode ?? listControl.activeViewModeId
}

/**
 * Resets the sorting of the specified list.
 * @param {object} listControl The list control object
 */
export function resetTableSorting(listControl)
{
	const defaultSorting = listControl.config.defaultColumnSorting
	const sortColumn = listControl.columns.find((c) => c.name === defaultSorting.columnName)

	if (sortColumn)
	{
		sortColumn.sortOrder = 1
		sortColumn.sortAsc = defaultSorting.sortOrder === 'asc'
	}
}

class TableRow
{
	constructor(rownum, fields, pkField, btnPermission)
	{
		this.Rownum = rownum
		this.Fields = fields
		this.pkField = pkField
		this.btnPermission = btnPermission

		// Reason for not being a normal getter: see in 'rowWithoutChildren'
		Object.defineProperty(this, 'rowKey', {
			get() { return !_isEmpty(this.pkField) ? this.Fields[this.pkField] : this.Rownum },
			enumerable: true,
			configurable: true
		})

		this.actionVisibility = {}
		this.actionDisability = {}
	}

	destroy()
	{
		this.btnPermission = null
		this.actionVisibility = null
		this.actionDisability = null
		this.Fields = null
	}
}

/**
 * Hydrates the table rows.
 * @param {object} listControl The list control object
 * @param {object} rowData The row data (C#)
 * @param {int} rowIndex The row index
 * @returns The hydrated row.
 */
export function hydrateTableRow(listControl, rowData, rowIndex)
{
	// Deserialize serialized fields
	for (const idx in listControl.columns)
	{
		const column = listControl.columns[idx]
		if (column.isSerialized)
		{
			const columnData = _get(rowData, column.name, null)
			if (_isEmpty(columnData))
				rowData[column.name] = undefined
			else
				rowData[column.name] = JSON.parse(columnData)
		}
	}

	const btnPermission = {
		...listControl.config.permissions,
		// CRUD conditions (disable buttons when the conditions are false)
		...rowData?.btnPermission
	}

	// Delete this property, so it won't be duplicated in every row.
	delete rowData?.btnPermission

	const pkColumn = toValue(listControl.config.pkColumn)
	const row = reactive(
		new TableRow(
			rowIndex,
			rowData,
			pkColumn,
			btnPermission
		)
	)

	// Custom actions visible / disabled status
	_forEach(listControl.config.customActions, (action) => {
		if (typeof action.checkIsVisible === 'function')
			row.actionVisibility[action.id] = action.checkIsVisible(row)
		if (typeof action.checkIsDisabled === 'function')
			row.actionDisability[action.id] = action.checkIsDisabled(row)
	})

	// The fields of the other tables have a different field identifier. TODO: Use the _get on the Table component
	_forEach(listControl.columns, (column) => Reflect.set(row.Fields, column.name, _get(rowData, column.name)))

	return row
}

/**
 * Hydrates the timeline data.
 * @param {object} timelineControl The timeline control object
 * @param {object} viewModel The list view model (C#)
 */
export function hydrateTimelineData(timelineControl, viewModel)
{
	const rows = viewModel?.table?.elements
	if (!rows)
		return

	timelineControl.timeLineData.rows = rows.filter((row) => genericFunctions.isDate(row.Data))
	if (rows.length > 0)
		timelineControl.config.scale = rows[0].Escala
}

/**
 * Create column hierarchy
 * @param {array} columns
 */
export function getColumnHierarchy(columns)
{
	const columnHierarchy = []
	const columnGroups = {}
	const extraColumns = []
	let columnLevel = 0

	// Group columns by table
	for (const key in columns)
	{
		const column = columns[key]

		// Add columns that are not data columns to separate object instead
		if (_isEmpty(column.name) || _isEmpty(column.area) || _isEmpty(column.field))
		{
			extraColumns.push(column)
			continue
		}

		// Add column level
		if (_isEmpty(columnGroups[column.area]))
		{
			columnGroups[column.area] = {
				level: columnLevel,
				columns: []
			}
			columnLevel++
		}

		// Add column
		columnGroups[column.area].columns.push(column)
	}

	// Add column groups by level to hierarchy
	for (const key in columnGroups)
	{
		const columnGroup = columnGroups[key]

		// Add column group with extra columns at the beginning
		columnHierarchy[columnGroup.level] = extraColumns.concat(columnGroup.columns)
	}

	// Add tree action property to first data column of each group
	for (const key in columnHierarchy)
	{
		const columnGroup = columnHierarchy[key]

		// Add tree action property to first data column
		for (const key in columnGroup)
		{
			const column = columnGroup[key]

			if (isVisibleColumn(column) && !_isEmpty(column.name) && !_isEmpty(column.area) && !_isEmpty(column.field))
			{
				column.hasTreeShowHide = true
				break
			}
		}
	}

	return columnHierarchy
}

/**
 * Get records per page options formatted for menu
 * @param {array} perPageOptionsArray
 */
export function getPerPageOptions(perPageOptionsArray)
{
	const perPageOptions = []

	for (const option of perPageOptionsArray)
	{
		perPageOptions.push({
			key: option,
			value: option.toString()
		})
	}

	return perPageOptions
}

/**
 * Get whether per page menu should be visible
 * @param perPageOptionsArray {number}
 * @param defaultPerPage {number}
 * @param rowCount {number}
 * @param page {number}
 * @param showAlternatePagination {boolean}
 * @param hasMorePages {boolean}
 * @returns {boolean}
 */
export function getPerPageMenuVisible(perPageOptionsArray, defaultPerPage, rowCount, page, showAlternatePagination, hasMorePages)
{
	if (!Array.isArray(perPageOptionsArray) || perPageOptionsArray.length < 1)
		return false

	const sortedPerPageOptions = perPageOptionsArray.concat([defaultPerPage]).sort((a, b) => a - b)

	if (sortedPerPageOptions.length < 2)
		return false

	// If rowCount is only the number of records for the current page
	if (showAlternatePagination)
	{
		// Has multiple pages or has more records than lowest per-page option
		return hasMorePages || page > 1 || rowCount > sortedPerPageOptions[0]
	}

	return rowCount > sortedPerPageOptions[0]
}

/**
 * Determine number of actions that are visible in the array
 * (also accounting for read-only mode).
 * @param actionArray {Array}
 * @param isReadOnly {Boolean}
 */
export function numArrayVisibleActions(actionArray, isReadOnly)
{
	if (!Array.isArray(actionArray))
		return 0
	if (isReadOnly === false)
		return actionArray.filter((a) => a.isVisible !== false).length
	return actionArray.filter((a) => a.isInReadOnly !== false && a.isVisible !== false).length
}

/**
 * 1st - Reduce redundant data that comes from server (Rownum).
 * 2nd - Reduce overhead when converting Tree into reactive (children).
 */
class TreeRow
{
	constructor(row, fnRowModel, parentRow, options)
	{
		this.rowKey = row.Key
		this.Rownum = row.Key

		this.Area = row.Area
		this.Form = row.Form

		this.BranchId = row.BId
		this._fnRowModel = fnRowModel

		this._fields = row.Fields
		this.Fields = markRaw(typeof this._fnRowModel === 'function' ? this._fnRowModel(row.Fields) : row.Fields)
		this.Fields.isValid = true

		this.alreadyLoaded = !_isEmpty(row.Children)

		Object.defineProperty(this, '_originalChildren', {
			value: row.Children,
			writable: true,
			enumerable: false
		})

		Object.defineProperty(this, '_parsedChildren', {
			writable: true,
			enumerable: false
		})

		// Hidden (cloneDeep problems)
		Object.defineProperty(this, 'hasChildren', {
			value: ref(!_isEmpty(this._originalChildren)),
			writable: true,
			enumerable: false
		})

		// FOR: tree table select row on return
		// Store row level
		if (parentRow !== undefined && parentRow !== null)
		{
			this.level = parentRow.level + 1
			this.ParentRowKey = parentRow.rowKey
		}
		else
			this.level = 0

		if (options !== undefined && options !== null)
		{
			// FOR: tree table select row on return
			/*
				Adding the 'showChildRows' property is needed so that when this is passed to the tree table row component
				the rows that need to be expanded will be expanded and the one that needs to be selected will be loaded.
			*/
			if (Array.isArray(options.rowKeyToScroll) &&
				this.level < options.rowKeyToScroll.length - 1 &&
				this.rowKey === options.rowKeyToScroll[this.level])
				this.showChildRows = true

			if (options.rownum !== undefined && options.rownum !== null)
				this.Rownum = options.rownum
		}
	}

	get children()
	{
		let rownum = 0
		if (!this._parsedChildren)
			this._parsedChildren = reactive(_map(this._originalChildren, (row) => new TreeRow(row, this._fnRowModel, this, { rownum: rownum++ })))
		return this._parsedChildren
	}

	hydrateChildrenData(rowsData, rowKeyToScroll)
	{
		let rownum = 0
		this._originalChildren = rowsData
		this._parsedChildren = reactive(_map(this._originalChildren, (row) => {
			row.Fields = { ...this._fields, ...row.Fields }
			return new TreeRow(row, this._fnRowModel, this, { rowKeyToScroll: rowKeyToScroll, rownum: rownum++ })
		}))
		this.hasChildren = !_isEmpty(this._originalChildren)
	}

	destroy()
	{
		for (const childRowIdx in this._parsedChildren) {
			if (this._parsedChildren[childRowIdx] instanceof TreeRow) {
				this._parsedChildren[childRowIdx].destroy()
			}
		}

		this._fnRowModel = null
		if (this._originalChildren instanceof Array)
			this._originalChildren.length = 0
		this._originalChildren = null
		if (this._parsedChildren instanceof Array)
			this._parsedChildren.length = 0
		this._parsedChildren = null
		this._fields = null
		this.Fields = null
	}
}

/**
 * Hydrates the tree table data.
 * @param {object} listControl The list control object
 * @param {object} viewModel The list view model (C#)
 */
export function hydrateTreeTableData(listControl, viewModel, rowKeyToScroll)
{
	if (typeof viewModel === 'undefined')
		return

	// Remove previous data
	listControl.rows = []

	if (typeof viewModel.Tree === 'undefined')
		return

	let rownum = 0

	listControl.rows = reactive(_map(viewModel.Tree, (row) => new TreeRow(row, listControl.config.treeListDefinitions?.rowModel, null, { rowKeyToScroll: rowKeyToScroll, rownum: rownum++ })))
}

class CalendarEvent
{
	constructor()
	{
		this.id = null
		this.title = null
		this.description = null
		this.start = null
		this.end = null
		this.color = null
		this.allDay = false
		this.resourceId = null
		this.rendering = ''
	}
}

class CalendarResource
{
	constructor()
	{
		this.id = null
		this.title = null
		this.columnLabel = null
		this.group = null
		this.groupLabel = null
		this.children = []
	}
}

class CalendarResourceChild
{
	constructor()
	{
		this.id = null
		this.title = null
	}
}

/**
 * Hydrates the Calendar data.
 * @param {object} listControl The list control object
 * @param {object} viewModel The list view model (C#)
 */
export function hydrateCalendarData(listControl, viewModel)
{
	const events = [], resources = []

	_forEach(viewModel.table.elements, (row) => {
		const event = new CalendarEvent(),
			resource = new CalendarResource(),
			resourceChild = new CalendarResourceChild()
		let dates = 0,
			texts = 0,
			bools = 0

		event.id = !_isEmpty(listControl.config.pkColumn) ? _get(row, listControl.config.pkColumn) : null

		_forEach(listControl.columns, (column) => {
			if (isVisibleColumn(column))
			{
				const columnValue = _get(row, column.name)
				switch (column.dataType)
				{
					case 'Text':
						{
							switch (texts)
							{
								case 0:
									event.title = columnValue
									break
								case 1:
									event.description = columnValue
									break
								case 2:
									event.color = columnValue
									break
								case 3:
									event.resourceId = columnValue
									resource.id = columnValue
									break
								case 4:
									resource.columnLabel = column.label
									resource.title = columnValue
									break
								case 5:
									resource.group = columnValue
									break
								case 6:
									resource.groupLabel = columnValue
									break
								case 7:
									// If the resource has children, the id of the event must be linked to the children instead of the resource.
									event.resourceId = columnValue
									resourceChild.id = columnValue
									break
								case 8:
									resourceChild.title = columnValue
									break
							}
							texts++
						}
						break
					case 'Date':
						{
							switch (dates)
							{
								case 0:
									event.start = columnValue
									break
								case 1:
									event.end = columnValue
									break
							}
							dates++
						}
						break
					case 'Boolean':
						{
							switch (bools)
							{
								case 0:
									event.allDay = columnValue
									break
								case 1:
									event.rendering = columnValue ? 'background' : ''
									break
							}
							bools++
						}
						break
				}
			}
		})

		events.push(event)

		const resIndex = _findIndex(resources, { id: resource.id })
		if (resIndex === -1)
		{
			resource.children.push(resourceChild)
			resources.push(resource)
		}
		else if (_findIndex(resources[resIndex].children, resourceChild) === -1)
			resources[resIndex].children.push(resourceChild)
	})

	listControl.events = events
	listControl.resources = resources
}

/**
 * Get flat array of rows from a tree (use with treeList pattern).
 * @param {object} rows The tree list row
 * @param {string} childkey the name of property that contain the child rows
 */
export function getRowsFlatArray(rows, childkey)
{
	const flattenItems = (items, key) => {
		return items.reduce((flattenedItems, item) => {
			flattenedItems.push(item)

			if (Array.isArray(item[key]))
				flattenedItems = flattenedItems.concat(flattenItems(item[key], key))

			return flattenedItems
		}, [])
	}

	return flattenItems(rows, childkey)
}

/**
 * Search a row from a tree (use with treeList pattern).
 * @param {object} rows The tree list row
 * @param {string} childkey the name of property that contain the child rows
 * @param {object} criteria a array object with the select criteria, Ex: ['key', 27]
 */
export function searchTreeRow(rows, childkey, criteria)
{
	return _find(getRowsFlatArray(rows, childkey), criteria)
}

/**
 * Determine column used for default search
 * @param {array} columns columns
 * @param {string} defaultSearchColumnName default search column name
 * @returns Object
 */
export function getDefaultSearchColumn(columns, defaultSearchColumnName)
{
	const searchableColumns = getSearchableColumns(columns)
	if (searchableColumns.length === 0) return null

	return searchableColumns.find((column) => column.name === defaultSearchColumnName) || searchableColumns[0]
}

/**
 * Get primary key of row.
 * @param table {Object}
 * @param row {Object}
 * @returns String
 */
export function getRowPk(table, row)
{
	return row.Fields[table.pkColumn]
}

/**
 * Get a row object from the row key
 * @param rows {Object}
 * @param rowKey {String}
 * @returns Object
 */
export function getRowByKey(rows, rowKey)
{
	return rows.find((row) => row.rowKey === rowKey)
}

/**
 * Get a row object from the row key path
 * @param rows {Object}
 * @param dataInfo {Object} Can reveice just the ID in string, an array containing multiple ID's or an object with multipleSelection and an array of ID's
 * @returns Object
 */
export function getRowByKeyPath(rows, dataInfo)
{
	// If it comes from "mark items from list to" we use just the array with the ID's
	if (dataInfo?.multipleSelection)
		dataInfo = dataInfo.rowKeyPath
	// If dataInfo contains only a key, convert it into an array so can be used in cycle
	else if (!Array.isArray(dataInfo))
		dataInfo = [dataInfo]

	if (dataInfo?.length === 0)
		return

	let currentRows = rows
	let levelRow, rowKey, idx

	for (idx in dataInfo)
	{
		// Find row with key at the current path level
		rowKey = dataInfo[idx]
		levelRow = currentRows.find((row) => row.rowKey === rowKey)

		// If no rows have the key at this level, no row with this key path exists
		if (!levelRow)
			return null

		// If the row found has sub-rows, search them in the next iteration
		if (Array.isArray(levelRow.children) && levelRow.children.length > 0)
			currentRows = levelRow.children
		else
			break
	}

	// If all levels in the row key path have been searched, the last row found is the result
	if (parseInt(idx) === dataInfo.length - 1)
		return levelRow

	// If not all rows were searched, the row was not found
	return null
}

/**
 * Get a row key path from the rows and specific row
 * @param rows {Object}
 * @param row {Object}
 * @returns Array
 */
export function getRowKeyPath(rows, row)
{
	const rowKeyPath = [row?.rowKey]
	const allRows = getRowsFlatArray(rows, 'children')
	let currentRow = row

	while (currentRow?.ParentRowKey && parseInt(currentRow.level) > 0)
	{
		currentRow = allRows.find(
			(row) => parseInt(row.level) === parseInt(currentRow.level) - 1 && row.rowKey === currentRow.ParentRowKey
		)
		if (currentRow === undefined)
			break

		rowKeyPath.unshift(currentRow?.rowKey)
	}

	return rowKeyPath
}

/**
 * Get a row object from the row's multi-index
 * @param rows {Object}
 * @param multiIndex {String} row index (index for each level separated by underscores)
 * @returns Object
 */
export function getRowByMultiIndex(rows, multiIndex)
{
	let indexPath = null

	if (typeof multiIndex === 'number')
		indexPath = [multiIndex]
	else
	{
		// Get index path as an array of integers from multi-index
		indexPath = multiIndex?.split('_')
		for (const idx in indexPath)
			indexPath[idx] = parseInt(indexPath[idx])
	}

	let currentRow = null
	let currentRows = rows
	// Iterate rows at current level to find the index at the current level
	for (const idx in indexPath)
	{
		// Get row at this level
		currentRow = currentRows[indexPath[idx]]

		// Set sub-rows as next level of rows to search
		if (currentRow?.children)
			currentRows = currentRow.children
	}

	return currentRow
}

/**
 * Get array of row objects from list of row keys.
 * @param rows {Array}
 * @param rowKeys {Object}
 * @returns Array
 */
export function getRowsFromKeyHash(rows, rowKeys)
{
	const rtnRows = []
	let row = {}

	for (const idx in rows)
	{
		row = rows[idx]
		if (rowKeys[row.rowKey] !== undefined)
			rtnRows.push(row)
	}

	return rtnRows
}

/**
 * Get parent multi-index.
 * A unique row identifier that accounts for cases with multiple levels like in tree tables.
 * This is the indexes for each level, joined with underscores.
 * @param multiIndex {String}
 */
export function getParentMultiIndex(multiIndex)
{
	if (multiIndex === undefined || multiIndex === null)
		return

	const multiIndexArray = multiIndex.split('_')

	// Remove last level index
	multiIndexArray.pop()

	return multiIndexArray.join('_')
}

/**
 * Set property in a row in the table object
 * @param {object} listConf The list configuration
 * @param {string, number} index The row index
 * @param {string} propertyName Property name
 * @param {object} propertyValue Property value
 */
export function setRowIndexProperty(listConf, index, propertyName, propertyValue)
{
	// Get row
	const row = getRowByMultiIndex(listConf.rows, index)

	// If row does not exist
	if (!row)
		return

	// Set property
	row[propertyName] = propertyValue
}

/**
 * Get value of cell data.
 * @param row {Object}
 * @param column {Object}
 * @returns any type
 */
export function getCellValue(row, column)
{
	return _get(row.Fields, column.name, _get(row.Fields, `${_toLower(column.area)}.${_toLower(column.field)}`))
}

/**
 * Get value of cell data.
 * @param table {Object}
 * @param row {Object}
 * @param column {Object}
 * @returns any type
 */
export function getTableCellValue(table, row, column)
{
	return _get(row.Fields, column.name)
}

/**
 * Get value of cell data.
 * @param row {Object}
 * @param columnName {String}
 * @returns any type
 */
export function getCellNameValue(row, columnName)
{
	return _get(row.Fields, columnName)
}

/**
 * Set the value of a cell.
 * @param row {Object}
 * @param column {Object}
 * @param value {Object}
 */
export function setCellValue(row, column, value)
{
	_set(row.Fields, column.name, value)
}

/**
 * Full column name
 * @param {object} column
 */
export function columnFullName(column)
{
	return column ? `${column.area}.${column.field}` : ''
}

/**
 * Get column reference from table and column names
 * @param columns {Array}
 * @param tableName {String}
 * @param columnName {String}
 * @returns {Object}
 */
export function getColumnFromTableAndColumnNames(columns, tableName, columnName)
{
	return columns.find((obj) => obj.area === tableName && obj.field === columnName)
}

/**
 * Get column reference from full TABLE.COLUMN name
 * @param columns {Array}
 * @param columnName {String}
 * @returns {Object}
 */
export function getColumnFromTableColumnName(columns, columnName)
{
	if (_isEmpty(columnName))
		return ''

	const columnNameProps = columnName.split('.')
	if (columnNameProps.length < 2)
		return ''

	return getColumnFromTableAndColumnNames(columns, columnNameProps[0], columnNameProps[1])
}

/**
 * Get column reference from name
 * @param table {Object}
 * @param columnName {String}
 * @returns {Object}
 */
export function getTableColumnFromName(table, columnName)
{
	return table.columns.find((obj) => obj.name === columnName)
}

/**
 * Recalculate values for all cells in a column to be in order.
 * @param rows {Array}
 * @param changedRow {Object}
 * @param orderColumn {Object}
 */
export function reCalcCellOrder(rows, changedRow, orderColumn)
{
	const newRows = deepUnwrap(rows)
	const curIdx = newRows.findIndex((r) => r.rowKey === changedRow.rowKey)
	const newChangedRow = newRows[curIdx]

	// Check bounds
	if (curIdx < 0 || curIdx >= newRows.length)
		return

	// New index of this row
	let newIdx = getCellValue(newChangedRow, orderColumn) - 1
	// Check bounds
	if (newIdx < 0)
		newIdx = 0
	else if (newIdx >= newRows.length)
		newIdx = newRows.length - 1

	// Remove row from current location
	newRows.splice(curIdx, 1)
	// Add row at new location
	newRows.splice(newIdx, 0, newChangedRow)

	// Iterate rows and assign new values to the "order" column
	let idx = 1
	for (const row of newRows)
		setCellValue(row, orderColumn, idx++)

	return newRows
}

/* BEGIN: Formatting field data to display */

/**
 * Get formatted string representing text in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function textDisplayCell(row, column, options)
{
	const value = getCellValue(row, column)
	const fnGetDisplayValue = (val) => genericFunctions.textDisplay(val, options)
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing a number in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function numericDisplayCell(row, column, options)
{
	const value = getCellValue(row, column)
	const fnGetDisplayValue = (val) => {
		return genericFunctions.numericDisplay(val, column.numberFormat?.decimalSeparator, column.numberFormat?.groupSeparator, column.numberFormat?.negativeFormat, { minimumFractionDigits: column.decimalPlaces, maximumFractionDigits: column.decimalPlaces }, options)
	}
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing a currency in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function currencyDisplayCell(row, column, options)
{
	const value = getCellValue(row, column)
	if (column.currency === undefined)
		return numericDisplayCell(row, column, options)

	const fnGetDisplayValue = (val) => {
		return genericFunctions.currencyDisplay(val, column.numberFormat?.decimalSeparator, column.numberFormat?.groupSeparator, column.numberFormat?.negativeFormat, column.decimalPlaces, column.currency, options.lcid, 'narrowSymbol', options)
	}
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing a date/time in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function dateDisplayCell(row, column, options)
{
	const formats = column.dateFormats ?? {}
	const dateTimeStr = getCellValue(row, column)

	const fnGetDisplayValue = (val) => {
		let dateTimeFormat

		if (Object.keys(formats).includes(column.dateTimeType))
			dateTimeFormat = column.dateFormats[column.dateTimeType]
		else if (column.dateTimeType === 'time')
		{
			dateTimeFormat = formats.hours
			val = '0001-01-01 ' + val
		}

		return genericFunctions.dateDisplay(val, dateTimeFormat, column.dateTimeType, column.dateFormats?.use12Hour, options)
	}

	return genericFunctions.formatValueToDisplay(dateTimeStr, fnGetDisplayValue)
}

/**
 * Get formatted string representing a boolean in a cell.
 * @param row {Object}
 * @param column {Object}
 * @returns Boolean
 */
export function booleanDisplayCell(row, column, options)
{
	const value = getCellValue(row, column)
	const fnGetDisplayValue = (val) => genericFunctions.booleanDisplay(val, options)
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing a hyperlink in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function hyperLinkDisplayCell(row, column)
{
	const value = getCellValue(row, column)
	const fnGetDisplayValue = (val) => val
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing an image in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String || Object
 */
export function imageDisplayCell(row, column, options)
{
	const value = getCellValue(row, column)
	const fnGetDisplayValue = (val) => genericFunctions.imageDisplay(val, options)
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing a document in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String || Object
 */
export function documentDisplayCell(row, column, options)
{
	const value = getCellValue(row, column)
	const fnGetDisplayValue = (val) => {
		// Map the Preview or Print value
		if (val !== undefined && val !== null)
		{
			if (column.viewType === undefined || column.viewType === null)
				val.viewType = documentViewTypeMode.print
			else
				val.viewType = column.viewType
		}

		return genericFunctions.documentDisplay(val, options)
	}
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing a geographic coordinate in a cell.
 * @param row {Object}
 * @param column {Object}
 * @returns String
 */
export function geographicDisplayCell(row, column)
{
	const value = getCellValue(row, column)
	const fnGetDisplayValue = (val) => {
		return geographicDisplay(val, column.numberFormat.decimalSeparator, column.numberFormat.groupSeparator, column.numberFormat.negativeFormat)
	}
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing a geographic shape in a cell.
 * @param row {Object}
 * @param column {Object}
 * @returns String
 */
export function geographicShapeDisplayCell(row, column)
{
	const value = getCellValue(row, column)
	const fnGetDisplayValue = (val) => geographicShapeDisplay(val)
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing a value from an enumeration in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object} [optional]
 * @returns String
 */
export function enumerationDisplayCell(row, column, options)
{
	const value = getCellValue(row, column)

	if (column.array === undefined)
		return value

	const fnGetDisplayValue = (val) => genericFunctions.enumerationDisplay(column.arrayAsObj, val, options)
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing a radio button in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object} [optional]
 * @returns Boolean
 */
export function radioDisplayCell(row, column, options)
{
	const value = getCellValue(row, column)
	const fnGetDisplayValue = (val) => genericFunctions.radioDisplay(val, options)
	return genericFunctions.formatValueToDisplay(value, fnGetDisplayValue)
}

/**
 * Get formatted string representing cell value. Calls formatting function based on column data type.
 * @param table {Object}
 * @param row {Object}
 * @param column {Object}
 * @param options {Object} [optional]
 * @returns String(plain text or HTML)
 */
export function getCellValueDisplay(table, row, column, options)
{
	if (column.dataDisplay === undefined)
		return getCellValue(row, column)

	// Optional options
	// Scroll data (to truncate and add elipses)
	if (options !== undefined && options.useScroll !== undefined && options.useScroll !== false)
		options.scrollData = column.scrollData

	if (!options || options === undefined)
		options = {}

	if (column.isHtmlField)
		options.isHtml = true

	if (column.multipleValues)
		options.multipleValues = true

	return column.dataDisplay(row, column, options)
}

/**
 * Get string representing the search value of text in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function textSearchCell(row, column, options)
{
	const value = getCellValue(row, column)
	return genericFunctions.textDisplay(value, options)
}

/**
 * Get string representing the search value of a number in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function numericSearchCell(row, column, options)
{
	const value = getCellValue(row, column)
	return genericFunctions.numericDisplay(value, options.numberFormat.decimalSeparator, options.numberFormat.groupSeparator, { minimumFractionDigits: column.decimalPlaces, maximumFractionDigits: column.decimalPlaces }, options)
}

/**
 * Get string representing the search value of a currency in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function currencySearchCell(row, column, options)
{
	const value = getCellValue(row, column)
	return genericFunctions.numericDisplay(value, options.numberFormat.decimalSeparator, options.numberFormat.groupSeparator, { minimumFractionDigits: column.decimalPlaces, maximumFractionDigits: column.decimalPlaces }, options)
}

/**
 * Get string representing the search value of a date/time in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function dateSearchCell(row, column, options)
{
	return dateDisplayCell(row, column, options)
}

/**
 * Get string representing the search value of a boolean in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function booleanSearchCell(row, column, options)
{
	const value = getCellValue(row, column)
	return genericFunctions.booleanDisplay(value, options)
}

/**
 * Get string representing the search value of a hyperLink in a cell.
 * @param row {Object}
 * @param column {Object}
 * @returns String
 */
export function hyperLinkSearchCell(row, column)
{
	return getCellValue(row, column)
}

/**
 * Get string representing the search value of an image.
 * @param value {String}
 * @returns String
 */
export function imageSearch(value)
{
	if (value.altText !== undefined)
		return value.altText

	if (value.titleText !== undefined)
		return value.titleText

	return ''
}

/**
 * Get string representing the search value of an image in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function imageSearchCell(row, column, options)
{
	const value = getCellValue(row, column)
	return imageSearch(value, options)
}

/**
 * Get string representing the search value of a document.
 * @param value {String}
 * @returns String
 */
export function documentSearch(value)
{
	if (value.fileName !== undefined)
		return value.fileName

	if (value.title !== undefined)
		return value.title

	return ''
}

/**
 * Get string representing the search the value of a document in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function documentSearchCell(row, column, options)
{
	const value = getCellValue(row, column)
	return documentSearch(value, options)
}

/**
 * Get formatted string representing the search value of a geographic coordinate.
 * @param value {String}
 * @param decimalSep {String}
 * @param groupSep {String}
 * @returns String
 */
export function geographicSearch(value, decimalSep, groupSep)
{
	if (typeof value.Lat !== 'number' || typeof value.Long !== 'number')
		return ''

	return genericFunctions.numericDisplay(value.Lat, decimalSep, groupSep, { minimumFractionDigits: 0, maximumFractionDigits: 20 }) + ' ' + genericFunctions.numericDisplay(value.Long, decimalSep, groupSep, { minimumFractionDigits: 0, maximumFractionDigits: 20 })
}

/**
 * Get string representing the search value of a geographic coordinate in a cell.
 * @param row {Object}
 * @param column {Object}
 * @returns String
 */
export function geographicSearchCell(row, column)
{
	const value = getCellValue(row, column)
	return geographicSearch(value, column.numberFormat.decimalSeparator, column.numberFormat.groupSeparator)
}

/**
 * Get string representing search value of a enumeration in a cell.
 * @param row {Object}
 * @param column {Object}
 * @param options {Object}
 * @returns String
 */
export function enumerationSearchCell(row, column, options)
{
	return enumerationDisplayCell(row, column, options)
}

/**
 * Get string for search value of a cell in a row.
 * @param table {Object}
 * @param row {Object}
 * @param column {Object}
 * @param options {Object} [optional]
 * @returns String
 */
export function getCellValueSearch(table, row, column, options)
{
	if (column.dataSearch === undefined)
		return getCellValue(row, column)

	// Optional options
	if (!options || options === undefined)
		options = {}

	return column.dataSearch(row, column, options)
}

/* END: Formatting field data to display */

/**
 * Determine if column is sortable
 * @param column {Object}
 * @returns Boolean
 */
export function isSortableColumn(column)
{
	return _get(column, 'sortable', false) && column.sortable !== false
}

/**
 * Determine if column is searchable
 * @param column {Object}
 * @returns boolean
 */
export function isSearchableColumn(column)
{
	return column.searchable && !_isEmpty(column.searchFieldType) && isVisibleColumn(column)
}

/**
 * Checks if the specified column is visible
 * @param {object} column The column
 * @returns True if it's a visible column, false otherwise
 */
export function isVisibleColumn(column)
{
	return column.isVisible === undefined || unref(column.isVisible)
}

/**
 * Get searchable columns
 * @param columns {Array}
 * @returns Array
 */
export function getSearchableColumns(columns)
{
	return columns?.filter(isSearchableColumn) ?? []
}

/**
 * Get sortable columns
 * @param columns {Array}
 * @returns Array
 */
export function getSortableColumns(columns)
{
	return columns?.filter(isSortableColumn) ?? []
}

/* BEGIN: Filter functions */

/**
 * Checks if the specified column has a filter, even if inactive.
 * @param {Array} filters The table filters
 * @param {Object} column The table column
 * @returns True if the column is filtered, false otherwise.
 */
export function columnIsFiltered(filters, column)
{
	return filters.some((f) => f.field === `${column.area}.${column.field}` || columnIsFiltered(f.subFilters, column))
}

/**
 * Checks if any of the filtered columns are hidden.
 * @param {Array} columns The table columns
 * @param {Array} columnRows The rows with the table columns data
 * @param {Array} filters The table filters
 * @returns A list with the hidden filtered columns.
 */
export function getHiddenFilterColumns(columns, columnRows, filters)
{
	const hiddenFilterCols = []

	for (const rowColumn of columnRows)
	{
		const column = columns.find((c) => rowColumn.value === c.name)
		const columnWasVisible = isVisibleColumn(column) ? 1 : 0

		if (rowColumn.Fields.visibility !== columnWasVisible && columnIsFiltered(filters, column))
			hiddenFilterCols.push(column)
	}

	return hiddenFilterCols
}

/**
 * Removes the first condition from the specified filter.
 * @param {Object} filter The filter
 * @returns The filter without the first condition.
 */
export function removeFirstFilterCondition(filter)
{
	if (filter.subFilters.length === 0)
		return null

	// The first sub filter is now the main filter
	const newFilter = filter.subFilters[0]
	newFilter.useOr = filter.useOr
	newFilter.subFilters = filter.subFilters.slice(1)
	return newFilter
}

/**
 * Create search filter
 * @param {string} name : Name of search filter
 * @param {boolean} active : State of search filter (active or inactive)
 * @param {string} field : Full name of field (TABLE.COLUMN)
 * @param {string} operator : Operator code (as in operators object)
 * @param {string[]} values : Array of values
 */
export function searchFilter(name, active, field, operator, values = [])
{
	return {
		type: 'COLUMN',
		name: name,
		active: active,
		field: field,
		operator: operator,
		values: values,
		subFilters: []
	}
}

/**
 * Gets a list of the conditions applied to the specified filter
 * @param {Object} filterOperators
 * @param {Object} filter
 * @param {Array} searchableColumns
 * @param {string} allFieldsText
 * @returns {Array} A list of conditions
 */
function getFilterConditions(filterOperators, filter, searchableColumns, allFieldsText)
{
	// Filter name defined
	if (filter.name.length > 0)
		return [filter.name]

	const column = getColumnFromTableColumnName(searchableColumns, filter.field)
	// If there's no column defined, it's going to search all columns
	if (!column)
	{
		const operator = filterOperators.text.CON
		return [`${allFieldsText} ${operator.title} "${filter.values[0]}"`]
	}

	const operators = filterOperators[column.searchFieldType],
		operator = operators[filter.operator],
		conditionNames = []

	conditionNames.push(`${column.label} ${operator.title}`)

	if (operator.valueCount > 0)
	{
		// Format values
		let conditionValues = []
		// Format dates
		if (column.searchFieldType === 'date')
		{
			const genericDataStore = useGenericDataStore()
			const dateFormat = genericDataStore.dateFormat[column.dateTimeType]

			for (const value of filter.values)
				conditionValues.push(genericFunctions.dateDisplay(value, dateFormat, column.dateTimeType, false))
		}
		else if (column.searchFieldType === 'enum')
		{
			// If using array of values in first value (IN operation)
			const values = Array.isArray(filter.values[0])
				? filter.values[0]
				: filter.values
			conditionValues = deepUnwrap(values.map((value) => column.array?.find((e) => e.key === value)?.value))
		}
		else if (column.searchFieldType === 'num')
		{
			const groupSeparator = column.numberFormat.groupSeparator
			const decimalSeparator = column.numberFormat.decimalSeparator

			for (const value of filter.values)
				conditionValues.push(genericFunctions.numericDisplay(value, decimalSeparator, groupSeparator))
		}
		// No formatting needed
		else
			conditionValues = filter.values

		conditionNames[0] += ` "${conditionValues.join('", "')}"`
	}

	// If there are sub filters, add their conditions to the list as well.
	for (const subFilter of filter.subFilters)
		conditionNames.push(...getFilterConditions(filterOperators, subFilter, searchableColumns))

	return conditionNames
}

/**
 * Get name of filter, calculating if empty
 * @param {Object} filterOperators
 * @param {Object} filter
 * @param {Array} searchableColumns
 * @param {string} separator
 * @param {string} allFieldsText
 * @returns {string}
 */
export function getFilterName(filterOperators, filter, searchableColumns, separator, allFieldsText)
{
	return getFilterConditions(filterOperators, filter, searchableColumns, allFieldsText).join(` ${separator} `)
}

/**
 * Returns the selected rows from the list configuration.
 * @param {Object} listConf : The list configuration.
 * @returns {Array} An array of Ids corresponding to the table's currently selected rows.
 */
export function getSelectedRows(listConf)
{
	return listConf.config?.showRowsSelectedTotalizer && listConf.allSelectedRows === 'false'
		? Object.keys(listConf.rowsSelected)
		: []
}

/**
* Get sum of values of column in rows and return formatted string of value
* @param column {Object} the column to sum.
* @param value {Number} the value in the column.
* @returns String The sum in a string format.
*/
export function getColumnTotalValueDisplay(column, value)
{
	// Actions column shows text to show this is the totals row
	if (column.isTotalizerTitle)
		return 'Total'

	// Column to show total (numeric or currency type)
	else if (column.totalizer)
	{
		if (column.currency !== undefined)
		{
			return genericFunctions.currencyDisplay(
				value,
				column.numberFormat.decimalSeparator,
				column.numberFormat.groupSeparator,
				column.numberFormat.negativeFormat,
				column.decimalPlaces,
				column.currency,
				'en-US', // Hardcoded to show the currency symbol previous to the value
				'narrowSymbol'
			)
		}
		else
		{
			return genericFunctions.numericDisplay(
				value,
				column.numberFormat.decimalSeparator,
				column.numberFormat.groupSeparator,
				{
					minimumFractionDigits: column.decimalPlaces,
					maximumFractionDigits: column.decimalPlaces
				}
			)
		}
	}

	return ''
}

/**
 * Get operators for condition by index
 * @param {Object} filterOperators
 * @param {Object} filter
 * @returns {Object}
 */
export function getFilterOperators(filterOperators, filter, searchableColumns)
{
	return filterOperators[getColumnFromTableColumnName(searchableColumns, filter.field)?.searchFieldType]
}

/**
 * Get number of values for condition by index
 * @param {Object} filterOperators
 * @param {Object} filter
 * @param {Array} searchableColumns
 * @returns {Object}
 */
export function getFilterValueCount(filterOperators, filter, searchableColumns)
{
	if (_isEmpty(filter))
		return 0
	if (filter.operator.length < 1)
		return 0

	const operators = getFilterOperators(filterOperators, filter, searchableColumns)
	return operators?.[filter.operator]?.valueCount ?? 0
}

/**
 * Get input component for condition by index
 * @param {Object} filterOperators
 * @param {Object} filter
 * @param {array} searchableColumns
 * @returns {string}
 */
export function getFilterInputComponent(filterOperators, filter, searchableColumns)
{
	const column = getColumnFromTableColumnName(searchableColumns, filter.field)
	const operator = getFilterOperators(filterOperators, filter, searchableColumns)[filter.operator]

	return operator.inputComponent !== undefined
		? operator.inputComponent
		: searchFilterData.inputComponents[column.searchFieldType]
}

/**
 * Get placeholder for condition input by index
 * @param {Object} filterOperators
 * @param {Object} filter
 * @param {array} searchableColumns
 * @returns {string}
 */
export function getFilterPlaceholder(filterOperators, filter, searchableColumns)
{
	const operator = getFilterOperators(filterOperators, filter, searchableColumns)[filter.operator]
	if (operator === undefined || operator === null)
		return ''
	return operator.placeholder
}

/**
 * Select default operator for condition by index
 * @param {Object} filterOperators
 * @param {Object} filter
 * @param {array} searchableColumns
 */
export function setFilterDefaultOperator(filterOperators, filter, searchableColumns)
{
	const column = getColumnFromTableColumnName(searchableColumns, filter.field)
	if (_isEmpty(column))
		return

	filter.operator = searchFilterData.searchBarOperator(column.searchFieldType, '')
	setFilterDefaultValues(filterOperators, filter, searchableColumns)
}

/**
 * Set default values for condition by index
 * @param {Object} filterOperators
 * @param {Object} filter
 * @param {array} searchableColumns
 */
export function setFilterDefaultValues(filterOperators, filter, searchableColumns)
{
	const operators = getFilterOperators(filterOperators, filter, searchableColumns)
	if (_isEmpty(operators) || operators.length < 1)
		return

	const operator = operators[filter.operator]
	const valueCount = getFilterValueCount(filterOperators, filter, searchableColumns)

	filter.values = []
	for (let i = 0; i < valueCount; i++)
	{
		if (typeof operator.defaultValue === 'undefined')
		{
			const column = getColumnFromTableColumnName(searchableColumns, filter.field)
			filter.values[i] = searchFilterData.defaultValue(column)
		}
		else
			filter.values[i] = deepUnwrap(operator.defaultValue)
	}
}

/**
 * Set value of condition by index
 * @param {object} filter
 * @param {number} valueIdx : index
 * @param {object} value : value
 */
export function setFilterConditionValue(filter, valueIdx, value)
{
	filter.values[valueIdx] = value
}

/**
 * Validate filter and return errors
 * @param {object} filter - Search filter
 * @param {object} columns - Table columns
 * @param {number} valueCount - The expected value count
 * @returns {array} states
 */
export function filterValidate(filter, columns, valueCount)
{
	const column = columns.find((col) => `${col.area}.${col.field}` === filter.field),
		conditionStates = [],
		valueStates = []

	if (!column)
		return conditionStates

	let conditionState = filter.operator.length > 0 ? 'VALID' : 'INVALID'

	for (const value of filter.values)
	{
		const strValue = value?.toString() ?? ''
		let valueState = 'VALID'

		if (_isEmpty(strValue) ||
			// Enumerated fields with no checkbox selected
			(filter.operator === 'IN' && filter.values[0]?.length === 0))
		{
			valueState = 'INVALID'
			conditionState = 'INVALID'
		}

		valueStates.push(valueState)
	}

	if (valueStates.length < valueCount)
	{
		valueStates.push('INVALID')
		conditionState = 'INVALID'
	}

	conditionStates.push({
		table: column.area,
		field: column.field,
		name: column.name,
		label: column.label,
		type: column.dataType,
		state: conditionState,
		valueStates
	})

	return conditionStates
}

/* END: Filter functions */

/**
 * Get searchable columns as options for dropdown
 * @param columns {Array}
 * @returns Array
 */
export function getSearchableColumnOptions(columns)
{
	const options = []

	for (const column of columns)
		options.push({ key: columnFullName(column), value: column.label })

	return options
}

/**
 * Get filter operators as options for dropdown
 * @param filter {Object}
 * @param operators {Number}
 * @param columns {Array}
 * @returns Array
 */
export function getFilterOperatorOptions(filter, operators, columns)
{
	const options = []
	const columnOperators = getFilterOperators(operators, filter, columns)

	for (const id in columnOperators)
	{
		const operator = columnOperators[id]
		options.push({ key: operator.key, value: unref(operator.title), icon: operator.icon })
	}

	return options
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
	return column.name.replace(/\./g, '_')
}

/**
 * Determine if column is an actions column
 * @param column {Object}
 * @returns Boolean
 */
export function isActionsColumn(column)
{
	if (column.isActions !== undefined)
		return column.isActions
	return false
}

/**
 * Determine if column is an extended actions column
 * @param action {Object}
 * @returns Boolean
 */
export function isExtendedActionsColumn(column)
{
	if (column.isExtendedActions !== undefined)
		return column.isExtendedActions
	return false
}

/**
 * Determine if column is an checklist column
 * @param column {Object}
 * @returns Boolean
 */
export function isChecklistColumn(column)
{
	if (column.isChecklist !== undefined)
		return column.isChecklist
	return false
}

/**
 * Determine if column is a drag and drop column
 * @param column {Object}
 * @returns Boolean
 */
export function isDragAndDropColumn(column)
{
	if (column.isDragAndDrop !== undefined)
		return column.isDragAndDrop
	return false
}

/**
 * Determine if column is a totalizer title column
 * @param column {Object}
 * @returns Boolean
 */
export function isTotalizerColumn(column)
{
	if (column.isTotalizer !== undefined)
		return column.isTotalizer
	return false
}

/**
 * True if the column is a data column, false otherwise.
 * @param column {Object} The column of the table.
 */
export function isDataColumn(column)
{
	return !(isActionsColumn(column) || isExtendedActionsColumn(column) || isChecklistColumn(column) || isDragAndDropColumn(column) || isTotalizerColumn(column))
}

/**
 * Determine if extended actions array has action passed
 * @param action {Object}
 * @returns Boolean
 */
export function hasExtendedAction(action)
{
	return this.extendedActions.includes(action)
}

/**
 * Determine if cell data has associated action
 * @param column {Object}
 * @returns Boolean
 */
export function hasDataAction(column)
{
	return _get(column, 'cellAction', false)
}

/**
 * Creating copy of parent row & removing children array
 * @param row {Object}
 */
export function rowWithoutChildren(row)
{
	const newRow = { ...row }
	delete newRow.children
	return cloneDeep(newRow)
}

/**
 * Initialization of listeners for events on which functions
 * such as content reloading depend.
 * @param listControl {Object} The list control object
 */
export function initTableEvents(listControl)
{
	let dependencyEvents = ['RELOAD_ALL_LIST_CONTROLS']
	if (!_isEmpty(listControl.controlLimits))
	{
		_forEach(listControl.controlLimits, (controlLimit) => {
			dependencyEvents = _unionWith(dependencyEvents, controlLimit.dependencyEvents)
		})
	}

	if (listControl.vueContext.internalEvents)
	{
		// Reload the list when a dependency changes.
		if (!_isEmpty(dependencyEvents))
			listControl.vueContext.internalEvents.onMany(dependencyEvents, () => listControl.reload())

		// Reload the list when it becomes visible.
		listControl.showWhenConditions.addOnShowListener(() => {
			if (!listControl.dataRequested)
				listControl.reload()
		})

		// Updates the array with dirty rows to validate before leaving the form.
		listControl.vueContext.internalEvents.on('is-table-control-dirty', ({ id, isDirty }) => {
			listControl.onRowDirty(id, isDirty)
		})

		// Deselects selected row(s) when closing and extended support form.
		listControl.vueContext.internalEvents.on('closed-extended-support-form', ({ controlId }) => {
			if (controlId === listControl.id)
				listControl.onUnselectAllRows()
		})

		// Force list reload.
		listControl.vueContext.internalEvents.on('reload-list', ({ controlId }) => {
			if (controlId === listControl.id)
			{
				const params = {
					tableConfiguration: getTableConfiguration(listControl)
				}

				listControl.reload(params)
			}
		})
	}
}

/**
 * Checks if the table has permission to execute the specified action.
 * @param {object} permissions The button permissions
 * @param {string} actionType The action type
 * @returns True if the user has permission, false otherwise.
 */
export function tableHasPermission(permissions, actionType)
{
	if (!permissions || typeof permissions !== 'object' || typeof actionType !== 'string')
		return false

	switch (actionType.toUpperCase())
	{
		case formModes.show:
			return permissions.canView !== false
		case formModes.edit:
			return permissions.canEdit !== false
		case formModes.duplicate:
			return permissions.canDuplicate !== false && permissions.canInsert !== false && permissions.canView !== false
		case formModes.delete:
			return permissions.canDelete !== false
		case formModes.new:
		case 'INSERT': /* There should never be an INSERT option, but the ID of this button is already scattered around the templates. */
			return permissions.canInsert !== false
	}

	return true
}

/**
 * Creating copy of parent row & removing children array
 * @param action {Object}
 * @param rowPermissions {Object}
 * @param tablePermissions {Object}
 * @param isReadonlyMode {Boolean}
 */
export function actionIsAllowed(action, rowPermissions, tablePermissions, isReadonlyMode)
{
	if (action === undefined || action === null)
		return false

	// Check if action has permission
	// If the action is row-specific, use the row permissions or, if not, use the table permissions
	const hasPermission = rowPermissions
		? genericFunctions.btnHasPermission(rowPermissions, action.id)
		: tableHasPermission(tablePermissions, action.id)
	if (!hasPermission)
		return false

	// Check if action is visible
	if (action.isVisible === false)
		return false

	// If in readonly mode, check is action is allowed there
	if (toValue(isReadonlyMode))
		return action.isInReadOnly

	return true
}

/**
 * Get the ID prefix of a table column control
 * @param tableControlId {String} Table control ID (Not the table name in the DB)
 * @param columnName {String} Column name
 */
export function getTableColumnControlIdPrefix(tableControlId, columnName)
{
	return tableControlId + '_' + columnName.replace(/\./g, '_')
}

/**
 * Get the ID of a table column sort ascending button
 * @param tableControlId {String} Table control ID (Not the table name in the DB)
 * @param columnName {String} Column name
 */
export function getTableColumnDropdownSortAscId(tableControlId, columnName)
{
	return getTableColumnControlIdPrefix(tableControlId, columnName) + '_sort_asc'
}

/**
 * Get the ID of a table column sort descending button
 * @param tableControlId {String} Table control ID (Not the table name in the DB)
 * @param columnName {String} Column name
 */
export function getTableColumnDropdownSortDescId(tableControlId, columnName)
{
	return getTableColumnControlIdPrefix(tableControlId, columnName) + '_sort_desc'
}

/**
 * Get the ID of a table column filter button
 * @param tableControlId {String} Table control ID (Not the table name in the DB)
 * @param columnName {String} Column name
 */
export function getTableColumnFilterId(tableControlId, columnName)
{
	return getTableColumnControlIdPrefix(tableControlId, columnName) + '_column_filter'
}

/**
 * Get the ID of a table multi-selection dropdown toggle button
 * @param tableControlId {String} Table control ID (Not the table name in the DB)
 */
export function getTableSelectorDropdownToggleId(tableControlId)
{
	return tableControlId + '_selector'
}

export default {
	hydrateTableData,
	getTableConfiguration,
	applyTableConfiguration,
	resetTableSorting,
	hydrateTreeTableData,
	hydrateTimelineData,
	hydrateCalendarData,
	getRowsFlatArray,
	searchTreeRow,
	getDefaultSearchColumn,
	getRowPk,
	getRowByKey,
	getRowByKeyPath,
	getRowKeyPath,
	getRowByMultiIndex,
	getRowsFromKeyHash,
	getParentMultiIndex,
	setRowIndexProperty,
	getCellValue,
	getTableCellValue,
	getCellNameValue,
	setCellValue,
	columnFullName,
	getColumnFromTableAndColumnNames,
	getColumnFromTableColumnName,
	getTableColumnFromName,
	reCalcCellOrder,
	textDisplayCell,
	numericDisplayCell,
	currencyDisplayCell,
	dateDisplayCell,
	booleanDisplayCell,
	hyperLinkDisplayCell,
	imageDisplayCell,
	documentDisplayCell,
	geographicDisplayCell,
	geographicShapeDisplayCell,
	enumerationDisplayCell,
	radioDisplayCell,
	getCellValueDisplay,
	textSearchCell,
	numericSearchCell,
	currencySearchCell,
	dateSearchCell,
	booleanSearchCell,
	hyperLinkSearchCell,
	imageSearch,
	imageSearchCell,
	documentSearch,
	documentSearchCell,
	geographicSearch,
	geographicSearchCell,
	enumerationSearchCell,
	getCellValueSearch,
	isSortableColumn,
	isSearchableColumn,
	isVisibleColumn,
	getSearchableColumns,
	getSortableColumns,
	columnIsFiltered,
	getHiddenFilterColumns,
	removeFirstFilterCondition,
	searchFilter,
	getFilterName,
	getSelectedRows,
	getColumnTotalValueDisplay,
	getFilterOperators,
	getFilterValueCount,
	getFilterInputComponent,
	getFilterPlaceholder,
	setFilterDefaultOperator,
	setFilterDefaultValues,
	setFilterConditionValue,
	filterValidate,
	getSearchableColumnOptions,
	getFilterOperatorOptions,
	getCellSlotName,
	isActionsColumn,
	isExtendedActionsColumn,
	isChecklistColumn,
	isDragAndDropColumn,
	isTotalizerColumn,
	isDataColumn,
	hasExtendedAction,
	hasDataAction,
	rowWithoutChildren,
	getColumnHierarchy,
	getPerPageOptions,
	getPerPageMenuVisible,
	numArrayVisibleActions,
	initTableEvents,
	tableHasPermission,
	actionIsAllowed,
	getTableColumnControlIdPrefix,
	getTableColumnDropdownSortAscId,
	getTableColumnDropdownSortDescId,
	getTableColumnFilterId,
	getTableSelectorDropdownToggleId
}
