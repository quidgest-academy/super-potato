import _isEmpty from 'lodash-es/isEmpty'

import { loadResources } from '@/plugins/i18n.js'
import { postData } from '@quidgest/clientapp/network'
import listFunctions from './listFunctions.js'
import formFunctions from './formFunctions.js'
import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
import GenericMenuHandlers from './genericMenuHandlers.js'

/***********************************************************************
 * This mixin defines methods to be reused in regular menu components. *
 ***********************************************************************/
export default {
	mixins: [
		GenericMenuHandlers
	],

	created()
	{
		this.componentOnLoadProc.addImmediateBusy(loadResources(this, this.interfaceMetadata.requiredTextResources))

		this.setSelectedTab()

		// We need to wait for data from fetchListData for both firstTable and secondTable
		// before running init on controls. This is because certain operations like
		// crudConditions evaluation might depend on the data fetched from these calls.
		Promise.all([
			this.controls.firstTable.fetchListData({}),
			this.controls.secondTable.fetchListData({})
		]).then(async () => {
			for (const i in this.controls)
			{
				await this.controls[i].init()
				this.controls[i].initData?.()
			}
		})

		this.mainTable.config.showRowsSelectedCount = true
		this.mainTable.config.rowClickActionInternal = 'selectMultiple'
		this.secondaryTable.config.rowClickActionInternal = 'selectSingle'

		// Tweak the configuration of the third table.
		const config = this.controls.thirdTable.config
		config.allowManageViews = false
		config.extendedActions = [
			'remove',
			'remove-reset'
		]
		for (const i in config.permissions)
			config.permissions[i] = false
	},

	computed: {
		/**
		 * A list of the selected item values.
		 */
		selectedItems()
		{
			return Object.values(this.model.selectedRows)
		},

		/**
		 * A list of the selected item keys.
		 */
		selectedItemsKeys()
		{
			return Object.keys(this.mainTable.rowsSelected)
		},

		/**
		 * The key of the currently selected item.
		 */
		selectedItemKey()
		{
			const keys = Object.keys(this.secondaryTable.rowsSelected)
			if (keys.length > 0)
				return keys[0]
			return ''
		}
	},

	methods: {
		/**
		 * Unselects all the rows.
		 */
		clearSelectedRows()
		{
			// Unselect all rows.
			this.mainTable.onUnselectAllRows()

			// Clears the selected rows hash table.
			this.unselectAllRowsData()
		},

		/**
		 * Handles the event of selection/checking a row.
		 * @param {string} tableConf The table configuration
		 * @param {string} rowKey The id of the row
		 */
		handleSelectedRow(tableConf, rowKey)
		{
			tableConf.onSelectRow({ rowKeyPath: rowKey, multipleSelection: true })
			if (rowKey?.multipleSelection)
				rowKey = rowKey.rowKeyPath
			this.selectRowData(rowKey)
		},

		/**
		 * Handles the event of unselection/unchecking a row.
		 * @param {string} tableConf The table configuration
		 * @param {string} rowKey The id of the row
		 */
		handleUnSelectedRow(tableConf, rowKey)
		{
			tableConf.onUnselectRow(rowKey)
			if (rowKey?.multipleSelection)
				rowKey = rowKey.rowKeyPath
			this.unselectRowData(rowKey)
		},

		/**
		 * Handles the event of selection/checking rows.
		 * @param {string} tableConf The table configuration
		 * @param {array} rowKeys Array of row IDs
		 */
		handleSelectedRows(tableConf, rowKeys)
		{
			tableConf.onSelectRows(rowKeys)
			this.selectRowsData(rowKeys)
		},

		/**
		 * Handles the event of selection/checking rows.
		 * @param {string} tableConf The table configuration
		 */
		handleUnselectAllRows(tableConf)
		{
			tableConf.onUnselectAllRows()
			this.unselectAllRowsData()
		},

		/**
		 * Updates the rows of the second table after selecting something in the first.
		 * @param {string} baseArea The name of the table area
		 */
		updateListData(baseArea)
		{
			// Unselect all rows and clear selected rows hash table.
			this.clearSelectedRows()

			// Clears all the table rows.
			this.controls.secondTable.rows = []

			// Reload table with related records.
			if (!_isEmpty(baseArea) && this.selectedItemKey !== '')
			{
				const queryParams = {}
				queryParams[baseArea] = this.selectedItemKey
				this.controls.secondTable.fetchListData({ queryParams })
			}
		},

		/**
		 * Saves the changes.
		 * @param {string} action The name of the controller action
		 * @param {boolean} reloadTable Whether or not the related table should be reloaded
		 * @param {string} baseArea The name of the table area
		 */
		apply(action, reloadTable = false, baseArea)
		{
			if (_isEmpty(action))
				return

			const params = {
				selectedIds: this.selectedItemsKeys,
				destinationId: this.selectedItemKey
			}

			// Add all selected.
			const allSelected = this.navigation.currentLevel.params.allSelected ?? []
			if (allSelected.findIndex((e) => e === this.controls.firstTable.id) !== -1)
				params.AllSelected = true

			params.tableConfiguration = listFunctions.getTableConfiguration(this.controls.firstTable)

			postData(
				this.controls.firstTable.controller,
				action,
				params,
				(data) => {
					this.controls.firstTable.fetchListData({})

					// Reload table with related records.
					if (reloadTable && !_isEmpty(baseArea))
					{
						const queryParams = {}
						queryParams[baseArea] = this.selectedItemKey
						this.controls.secondTable.fetchListData({ queryParams })
					}

					let msgType = 'error'
					if (data.Success === true)
					{
						this.clearSelectedRows()
						msgType = 'success'
					}

					genericFunctions.displayMessage(data.Message, msgType)
				},
				undefined,
				undefined,
				this.navigationId)
		},

		/**
		 * Add row to hash table of selected rows.
		 * @param {string} rowKey The id of the row
		 */
		selectRowData(rowKey)
		{
			const rowKeys = {}
			rowKeys[rowKey] = true

			const rows = this.mainTable.rows,
				selectedRows = listFunctions.getRowsFromKeyHash(rows, rowKeys)

			if (selectedRows.length < 1)
				return

			this.model.selectedRows[rowKey] = selectedRows[0]
		},

		/**
		 * Add rows to hash table of selected rows.
		 * @param {object} rowKeys The ID of the rows
		 */
		selectRowsData(rowKeys)
		{
			const rows = this.mainTable.rows,
				selectedRows = listFunctions.getRowsFromKeyHash(rows, rowKeys)

			if (selectedRows.length < 1)
				return

			for (const idx in selectedRows)
				this.model.selectedRows[selectedRows[idx].rowKey] = selectedRows[idx]
		},

		/**
		 * Remove row from hash table of selected rows.
		 * @param {string} rowKey The id of the row
		 */
		unselectRowData(rowKey)
		{
			delete this.model.selectedRows[rowKey]
		},

		/**
		 * Remove all rows from hash table of selected rows.
		 */
		unselectAllRowsData()
		{
			this.model.selectedRows = {}
		},

		/**
		 * Sets the selected tab, according to the value in the store.
		 */
		setSelectedTab()
		{
			const areaName = this.menuInfo.area
			const menuName = this.menuInfo.name

			if (!formFunctions.validateStoredValues(areaName, this.containersState, this.menuInfo))
				return

			const selectedTab = this.containersState[areaName][areaName][menuName].tabGroup
			if (selectedTab && typeof selectedTab === 'string')
				this.controls.tabGroup.selectTab(selectedTab)
		}
	},

	watch: {
		'controls.tabGroup.selectedTab'(newVal)
		{
			const data = {
				navigationId: this.navigationId,
				key: this.menuInfo.area,
				formInfo: this.menuInfo,
				fieldId: 'tabGroup',
				containerState: newVal
			}

			this.storeContainerState(data)
		}
	}
}
