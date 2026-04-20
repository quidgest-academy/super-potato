/*****************************************************************
 *                                                               *
 * This store holds client-side copies of global tables.         *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'


//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

/**
 * State of the global tables store.
 * @returns {Object} The initial state with an empty tables object.
 */
const state = () => ({
	tables: {}
})

//----------------------------------------------------------------
// Variable getters
//----------------------------------------------------------------

/**
 * Getters for the global tables store.
 */
const getters = {
	/**
	 * Get the table data by name.
	 * @param {Object} state - The current state of the store.
	 * @returns {Function} A function that takes a table name and returns the corresponding table data.
	 */
	getTableByName(state)
	{
		return (tableName) => state.tables[tableName]
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

/**
 * Actions for the global tables store.
 */
const actions = {
	/**
	 * Load data into the store from a given viewModel.
	 * @param {Object} viewModel - The viewModel containing table data.
	 */
	loadFromViewModel(viewModel)
	{
		const globalTables = [
		]

		for (const tableName of globalTables)
		{
			const viewModelKey = `T${tableName}`

			if (Reflect.has(viewModel, viewModelKey))
			{
				const tableData = viewModel[viewModelKey]
				this.setTableData(tableName, tableData)
			}
		}
	},

	/**
	 * Set the data for a specific table.
	 * @param {string} tableName - The name of the table.
	 * @param {Object} data - The data to be set for the table.
	 */
	setTableData(tableName, data)
	{
		this.tables[tableName].hydrate(data)
	},

	/**
	 * Delete a specific table from the store.
	 * @param {string} tableName - The name of the table to delete.
	 */
	deleteTable(tableName)
	{
		delete this.tables[tableName]
	},

	/**
	 * Reset the store to its initial state.
	 */
	resetStore()
	{
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

/**
 * Define and export the global tables store.
 */
export const useGlobalTablesDataStore = defineStore('globalTablesData', {
	state,
	getters,
	actions
})
