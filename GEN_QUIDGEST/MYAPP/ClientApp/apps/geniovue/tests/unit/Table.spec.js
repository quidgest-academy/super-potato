import { nextTick } from 'vue'
import { expect, vi } from 'vitest'
import { within } from '@testing-library/dom'
import { fireEvent } from '@testing-library/vue'
import { flushPromises } from '@vue/test-utils'
import cloneDeep from 'lodash-es/cloneDeep'
import '@testing-library/jest-dom'

import { render } from './utils'
import QTable from '@/components/table/QTable.vue'
import fakeData from '../cases/Table.mock.js'

const global = {
	stubs: ['inline-svg']
}

describe('QTable.vue', () => {
	let tableTest
	beforeEach(() => tableTest = fakeData.getTableTest())

	it('Table with row data displays rows', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})

		const rowCount = tableTest.rows.length

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		expect(rows).toHaveLength(rowCount)
	})

	it('Table with no row data displays <empty> row', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: [],
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})

		// When it's empty, there will be a row with a placeholder.
		const rowCount = 1

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		expect(rows).toHaveLength(rowCount)
		const emptyRow = await wrapper.findByText('No data to show')
		expect(emptyRow).toBeInTheDocument()
	})

	it('Table in normal mode displays insert button, clicking button emits insert event', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				headerLevel: 1,
				readonly: false
			}
		})

		const generalAction = tableTest.config.generalActions[0]

		await flushPromises()
		await vi.dynamicImportSettled()

		// Insert button
		const button = await wrapper.findByTitle(generalAction.title)
		expect(button).toBeInTheDocument()

		// Click insert button and check emit
		await fireEvent.click(button)

		const rowAction = await wrapper.emitted('row-action')

		expect(rowAction).toBeTruthy()
		expect(rowAction[0][0].name).toBe(generalAction.name)
		expect(rowAction[0][0].params.formName).toBe(generalAction.params.formName)
		expect(rowAction[0][0].params.mode).toBe(generalAction.params.mode)
		expect(rowAction[0][0].params.type).toBe(generalAction.params.type)
	})

	it('Table in read-only mode does not display insert button', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				headerLevel: 1,
				readonly: true
			}
		})

		await nextTick()

		// No insert button
		const button = await wrapper.queryByTitle(tableTest.config.generalActions[0].title)
		expect(button).toBeNull()
	})

	it('Invalid row is highlighted', async () => {
		const cssClasses = fakeData.cssClasses
		const dataRows = fakeData.rowsInvalid, dataColumns = fakeData.columns01
		const wrapper = render(QTable, {
			global,
			props: {
				rows: dataRows,
				columns: dataColumns,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})
		const rowNum = 0

		const rowCount = dataRows.length

		await nextTick()

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		expect(rows).toHaveLength(rowCount)

		expect(rows[rowNum]).toHaveClass(cssClasses.invalidRow)
	})

	it('Rows with color #00A000 and background color #E0E0E0, have those colors in their style', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		expect(rows[4]).toHaveStyle('color: #00A000')
		expect(rows[6]).toHaveStyle('color: #00A000')

		expect(rows[3]).toHaveStyle('background-color: #E0E0E0')
		expect(rows[5]).toHaveStyle('background-color: #E0E0E0')
	})

	it('Cells with color #C08000, have that color in their style', async () => {
		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: tableTest.columns.value,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})

		// Get index of column with foregroundColor property
		const columnIdx = tableTest.rows[0].Fields.columns.findIndex(col => col.foregroundColor)
		let domColumnIdx = columnIdx + 1
		// Account for extra column if table has checklist
		if (tableTest.config.rowsCheckable !== undefined && tableTest.config.rowsCheckable !== false)
			domColumnIdx++

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		let cells = await within(rows[0]).queryAllByRole('cell')
		expect(cells[domColumnIdx]).toHaveStyle('color: #C08000')
		cells = await within(rows[1]).queryAllByRole('cell')
		expect(cells[domColumnIdx]).toHaveStyle('color: #C08000')
		cells = await within(rows[2]).queryAllByRole('cell')
		expect(cells[domColumnIdx]).toHaveStyle('color: #C08000')
		cells = await within(rows[3]).queryAllByRole('cell')
		expect(cells[domColumnIdx]).toHaveStyle('color: #C08000')
	})

	it('Cell with column scroll has truncated text followed by (...)', async () => {
		// Copy columns and add scrollData property
		const columnsScroll = cloneDeep(tableTest.columns.value)
		columnsScroll[2].scrollData = 5

		const wrapper = render(QTable, {
			global,
			props: {
				rows: tableTest.rows,
				columns: columnsScroll,
				config: tableTest.config,
				totalRows: tableTest.totalRows,
				headerLevel: 1,
				readonly: tableTest.readonly
			}
		})
		const rowIdx = 2

		await nextTick()

		// Get index of column with scroll
		const columnIdx = columnsScroll.findIndex(obj => obj.scrollData !== undefined)
		let domColumnIdx = columnIdx
		// Account for extra column if table has checklist
		if (tableTest.config.rowsCheckable !== undefined && tableTest.config.rowsCheckable !== false)
			domColumnIdx++

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get rows
		const rows = await wrapper.getAllByTestId('table-row')

		let cells = []
		cells = await within(rows[rowIdx]).queryAllByRole('cell')
		expect(cells[domColumnIdx + 1]).toHaveTextContent('thing (...)')
	})
})
