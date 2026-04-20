import '@testing-library/jest-dom'
import { mount, render } from './utils'
import { fireEvent } from '@testing-library/vue'
import { flushPromises } from '@vue/test-utils'
import { vi } from 'vitest'

import listFunctions from '@/mixins/listFunctions'
import injectFunctions from '../injectFunctions'

import QTreeTableRow from '@/components/table/QTreeTableRow'

const configSimple = {
	row: {
		hasChildren: { value: true },
		children: [
			{
				hasChildren: { value: true },
				children: [
					{
						hasChildren: { value: false },
						children: [],
						Text: '711 DB CARA3',
						Area: 'MENU',
						Form: 'MENUW',
						Key: 'c7497575-ce83-47af-aec6-16a09334f8a9',
						Rownum: 'c7497575-ce83-47af-aec6-16a09334f8a9',
						rowKey: 'c7497575-ce83-47af-aec6-16a09334f8a9',
						Action: '',
						Image: null,
						Fields: {
							ValCodmenu: 'c7497575-ce83-47af-aec6-16a09334f8a9',
							ValNum: '711',
							ValNome: 'CARA3',
							ValTipomenu: 'DB',
							isValid: true
						}
					}
				],
				Text: '71 DB CARA2',
				Area: 'MENU',
				Form: 'MENUW',
				Key: 'a7497575-ce83-47af-aec6-16a09334f8a9',
				Rownum: 'a7497575-ce83-47af-aec6-16a09334f8a9',
				rowKey: 'a7497575-ce83-47af-aec6-16a09334f8a9',
				Action: '',
				Image: null,
				Fields: {
					ValCodmenu: 'a7497575-ce83-47af-aec6-16a09334f8a9',
					ValNum: '71',
					ValNome: 'CARA2',
					ValTipomenu: 'DB',
					isValid: true
				}
			},
			{
				hasChildren: { value: false },
				children: [],
				Text: '72 DB CARA3',
				Area: 'MENU',
				Form: 'MENUW',
				Key: 'b7497575-ce83-47af-aec6-16a09334f8a9',
				Rownum: 'b7497575-ce83-47af-aec6-16a09334f8a9',
				rowKey: 'b7497575-ce83-47af-aec6-16a09334f8a9',
				Action: '',
				Image: null,
				Fields: {
					ValCodmenu: 'b7497575-ce83-47af-aec6-16a09334f8a9',
					ValNum: '72',
					ValNome: 'CARA3',
					ValTipomenu: 'DB',
					isValid: true
				}
			}
		],
		Text: '7 M Características',
		Area: 'MENU',
		Form: 'MENUW',
		Key: '6d1e5aa6-2bba-4f8c-b9a8-838469dba74a',
		Rownum: '6d1e5aa6-2bba-4f8c-b9a8-838469dba74a',
		rowKey: '6d1e5aa6-2bba-4f8c-b9a8-838469dba74a',
		Action: '',
		Image: null,
		Fields: {
			ValCodmenu: '6d1e5aa6-2bba-4f8c-b9a8-838469dba74a',
			ValNum: '7',
			ValNome: 'Características',
			ValTipomenu: 'M',
			isValid: true
		}
	},
	uniqueId: 'a7497575-ce83-47af-aec6-16a09334f8a9',
	rowIndex: 0,
	columns: [
		{
			order: 1,
			dataType: 'Text',
			searchFieldType: 'text',
			component: null,
			name: 'ValNum',
			area: 'MENU',
			field: 'NUM',
			label: 'Número',
			supportForm: 'MENUW',
			params: { 'type': 'form', 'formName': 'MENUW', 'mode': 'SHOW' },
			cellAction: true,
			hasTreeShowHide: true,
			isVisible: true,
			sortable: true,
			array: null,
			useDistinctValues: false,
			initialSort: false,
			initialSortOrder: '',
			isDefaultSearch: false,
			pkColumn: 'ValCodmenu',
			dataLength: 30
		},
		{
			order: 2,
			dataType: 'Array',
			searchFieldType: 'enum',
			component: null,
			name: 'ValTipomenu',
			area: 'MENU',
			field: 'TIPOMENU',
			label: 'Tipo de Menu',
			supportForm: null,
			params: null,
			cellAction: false,
			isVisible: true,
			sortable: false,
			array: {
				'': 'Sem Continuação',
				'M': 'Menu',
			},
			useDistinctValues: false,
			initialSort: false,
			initialSortOrder: '',
			isDefaultSearch: false,
			pkColumn: 'ValCodmenu',
			dataLength: 2,
			arrayType: 'C'
		},
		{
			order: 3,
			dataType: 'Text',
			searchFieldType: 'text',
			component: null,
			name: 'ValNome',
			area: 'MENU',
			field: 'NOME',
			label: 'Descrição do menu',
			supportForm: null,
			params: null,
			cellAction: false,
			isVisible: true,
			sortable: true,
			array: null,
			useDistinctValues: false,
			initialSort: false,
			initialSortOrder: '',
			isDefaultSearch: false,
			pkColumn: 'ValCodmenu',
			dataLength: 100
		}
	],
	tableName: 'DTMENU',
	propRowClasses: 'q-tree-table-row',
	rowTitle:'',
	isValid:'true',
	cellTitles: {
		ValNum: '1',
		ValTipomenu: 'Menu',
		ValNome: 'Tabelas'
	},
	rowSelectedForGroup: false, // is row selected? prop
	level: 1,
	texts: [],
	expandIcon: 'add',
	collapseIcon: 'minus-sign',
	toggleShowChildren: () => {},
}

const configSubTable = {
	row: {
		hasChildren: { value: true },
		children: [
			{
				hasChildren: { value: true },
				children: [
					{
						hasChildren: { value: false },
						children: [],
						Text: '711 DB CARA3',
						Area: 'MENU',
						Form: 'MENUW',
						Key: 'c7497575-ce83-47af-aec6-16a09334f8a9',
						Rownum: 'c7497575-ce83-47af-aec6-16a09334f8a9',
						rowKey: 'c7497575-ce83-47af-aec6-16a09334f8a9',
						Action: '',
						Image: null,
						Fields: {
							ValCodmenu: 'c7497575-ce83-47af-aec6-16a09334f8a9',
							ValNum: '711',
							ValNome: 'CARA3',
							ValTipomenu: 'DB',
							isValid: true
						}
					}
				],
				Text: '71 CARA2',
				Area: 'TEST',
				Form: 'TESTW',
				Key: 'a7497575-ce83-47af-aec6-16a09334f8a9',
				Rownum: 'a7497575-ce83-47af-aec6-16a09334f8a9',
				rowKey: 'a7497575-ce83-47af-aec6-16a09334f8a9',
				Action: '',
				Image: null,
				Fields: {
					ValCodtest: 'a7497575-ce83-47af-aec6-16a09334f8a9',
					ValNum: '71',
					ValNome: 'CARA2',
					isValid: true
				}
			},
			{
				hasChildren: { value: false },
				children: [],
				Text: '72 CARA3',
				Area: 'TEST',
				Form: 'TESTW',
				Key: 'b7497575-ce83-47af-aec6-16a09334f8a9',
				Rownum: 'b7497575-ce83-47af-aec6-16a09334f8a9',
				rowKey: 'b7497575-ce83-47af-aec6-16a09334f8a9',
				Action: '',
				Image: null,
				Fields: {
					ValCodtest: 'b7497575-ce83-47af-aec6-16a09334f8a9',
					ValNum: '72',
					ValNome: 'CARA3',
					isValid: true
				}
			}
		],
		Text: '7 M Características',
		Area: 'MENU',
		Form: 'MENUW',
		Key: '6d1e5aa6-2bba-4f8c-b9a8-838469dba74a',
		Rownum: '6d1e5aa6-2bba-4f8c-b9a8-838469dba74a',
		rowKey: '6d1e5aa6-2bba-4f8c-b9a8-838469dba74a',
		Action: '',
		Image: null,
		Fields: {
			ValCodmenu: '6d1e5aa6-2bba-4f8c-b9a8-838469dba74a',
			ValNum: '7',
			ValNome: 'Características',
			ValTipomenu: 'M',
			isValid: true
		}
	},
	uniqueId: 'a7497575-ce83-47af-aec6-16a09334f8a9',
	rowIndex: 0,
	columns: [
		{
			order: 1,
			dataType: 'Text',
			searchFieldType: 'text',
			component: null,
			name: 'ValNum',
			area: 'MENU',
			field: 'NUM',
			label: 'Número',
			supportForm: 'MENUW',
			params: { 'type': 'form', 'formName': 'MENUW', 'mode': 'SHOW' },
			cellAction: true,
			hasTreeShowHide: true,
			isVisible: true,
			sortable: true,
			array: null,
			useDistinctValues: false,
			initialSort: false,
			initialSortOrder: '',
			isDefaultSearch: false,
			pkColumn: 'ValCodmenu',
			dataLength: 30
		},
		{
			order: 2,
			dataType: 'Array',
			searchFieldType: 'enum',
			component: null,
			name: 'ValTipomenu',
			area: 'MENU',
			field: 'TIPOMENU',
			label: 'Tipo de Menu',
			supportForm: null,
			params: null,
			cellAction: false,
			isVisible: true,
			sortable: false,
			array: {
				'': 'Sem Continuação',
				'M': 'Menu',
			},
			useDistinctValues: false,
			initialSort: false,
			initialSortOrder: '',
			isDefaultSearch: false,
			pkColumn: 'ValCodmenu',
			dataLength: 2,
			arrayType: 'C'
		},
		{
			order: 3,
			dataType: 'Text',
			searchFieldType: 'text',
			component: null,
			name: 'ValNome',
			area: 'MENU',
			field: 'NOME',
			label: 'Descrição do menu',
			supportForm: null,
			params: null,
			cellAction: false,
			isVisible: true,
			sortable: true,
			array: null,
			useDistinctValues: false,
			initialSort: false,
			initialSortOrder: '',
			isDefaultSearch: false,
			pkColumn: 'ValCodmenu',
			dataLength: 100
		},
		{
			order: 4,
			dataType: 'Text',
			searchFieldType: 'text',
			component: null,
			name: 'ValNome',
			area: 'TEST',
			field: 'NOME',
			label: 'Nome',
			supportForm: null,
			params: null,
			cellAction: false,
			isVisible: true,
			sortable: true,
			array: null,
			useDistinctValues: false,
			initialSort: false,
			initialSortOrder: '',
			isDefaultSearch: false,
			pkColumn: 'ValCodtest',
			dataLength: 100 ,
			hasTreeShowHide: true,
		},
		{
			order: 5,
			dataType: 'Text',
			searchFieldType: 'text',
			component: null,
			name: 'ValNum',
			area: 'TEST',
			field: 'NUM',
			label: 'Num',
			supportForm: null,
			params: null,
			cellAction: false,
			isVisible: true,
			sortable: true,
			array: null,
			useDistinctValues: false,
			initialSort: false,
			initialSortOrder: '',
			isDefaultSearch: false,
			pkColumn: 'ValCodtest',
			dataLength: 100
		}
	],
	tableName: 'DTMENU',
	propRowClasses: 'q-tree-table-row',
	rowTitle:'',
	isValid:'true',
	cellTitles: {
		ValNum: '1',
		ValTipomenu: 'Menu',
		ValNome: 'Tabelas'
	},
	rowSelectedForGroup: false, // is row selected? prop
	level: 0,
	texts: [],
	expandIcon:'add',
	collapseIcon:'minus-sign',
	toggleShowChildren: () => {},
}

const simpleUsageMethods = {
	getColumnHierarchy(columns) {
		return listFunctions.getColumnHierarchy(columns)
	}
}

const _global = {
	provide: {
		getValueFromRow: 			injectFunctions.getValueFromRow,
		getCellSlotName: 			injectFunctions.getCellSlotName,
		canShowColumn: 				injectFunctions.canShowColumn,
		isSortableColumn:		 	injectFunctions.isSortableColumn,
		isSearchableColumn: 		injectFunctions.isSearchableColumn,
		isActionsColumn: 			injectFunctions.isActionsColumn,
		isExtendedActionsColumn: 	injectFunctions.isExtendedActionsColumn,
		isChecklistColumn: 			injectFunctions.isChecklistColumn,
		isDragAndDropColumn: 		injectFunctions.isDragAndDropColumn,
		isTotalizerColumn:			injectFunctions.isTotalizerColumn,
		isRowChecked: 				injectFunctions.isRowChecked,
		getRowClasses: 				injectFunctions.getRowClasses,
		getRowTitle: 				injectFunctions.getRowTitle,
		rowIsValid: 				injectFunctions.rowIsValid,
		hasDataAction: 				injectFunctions.hasDataAction,
		hasExtendedAction: 			injectFunctions.hasExtendedAction,
		getCellDataDisplay: 		injectFunctions.getCellDataDisplay,
		getRowCellDataTitles:		injectFunctions.getRowCellDataTitles,
		isRowSelected: 				injectFunctions.isRowSelected,
		executeRowClickAction: 		injectFunctions.executeRowClickAction,
		rowWithoutChildren: 		injectFunctions.rowWithoutChildren,
		allSelectedRows: 			'false'
	}
}

const _propsSimple = {
	uniqueId: configSimple.uniqueId,
	row: configSimple.row,
	rowKeyPath: [configSimple.row.rowKey],
	rowIndex: configSimple.rowIndex,
	texts: configSimple.texts,
	columns: configSimple.columns,
	columnHierarchy: simpleUsageMethods.getColumnHierarchy(configSimple.columns), // listFunctions.getColumnHierarchy(configSimple.columns)
	tableName: configSimple.tableName,
	level: configSimple.level,
	expandIcon: configSimple.expandIcon,
	collapseIcon: configSimple.collapseIcon,
	resourcesPath: 'Content/img/'
}

const _propsSubTable = {
	uniqueId: configSubTable.uniqueId,
	row: configSubTable.row,
	rowKeyPath: [configSubTable.row.rowKey],
	rowIndex: configSubTable.rowIndex,
	texts: configSubTable.texts,
	columns: configSubTable.columns,
	columnHierarchy: simpleUsageMethods.getColumnHierarchy(configSubTable.columns),
	tableName: configSubTable.tableName,
	level: configSubTable.level,
	expandIcon: configSubTable.expandIcon,
	collapseIcon: configSubTable.collapseIcon,
	resourcesPath: 'Content/img/'
}

describe('TreeList.vue', () => {
	it('Rows should be generating with children hidden', async () => {
		const wrapper = mount(QTreeTableRow, {
			global: _global,
			props: _propsSimple
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		const rows = await wrapper.findAll('tr')
		expect(rows.length).toBe(1)
		expect(rows[0].findAll('td').length).toBe(3)
	})

	it('Expand the first row', async () => {
		const wrapper = mount(QTreeTableRow, {
			global: _global,
			props: _propsSimple
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		let rows = await wrapper.findAll('tr')

		await fireEvent.click(rows[0].find('button[data-testid="tree-action"]').element)

		await flushPromises()
		await vi.dynamicImportSettled()

		rows = await wrapper.findAll('tr')
		const header = await wrapper.findAll('thead')
		let numHeaderRows = 0
		await header.forEach(elem => {
			numHeaderRows += elem.findAll('tr').length
		})

		expect(rows.length - numHeaderRows).toBe(3)
	})

	it('Expand and then collapse the first row', async () => {
		const wrapper = mount(QTreeTableRow, {
			global: _global,
			props: _propsSimple
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		let rows = await wrapper.findAll('tr[data-testid="table-row"]')

		await fireEvent.click(rows[0].find('button[data-testid="tree-action"]').element)

		await flushPromises()
		await vi.dynamicImportSettled()

		rows = await wrapper.findAll('tr[data-testid="table-row"]')
		const header = await wrapper.findAll('thead')
		let numHeaderRows = 0
		header.forEach(elem => {
			numHeaderRows += elem.findAll('tr').length
		})
		expect(rows.length - numHeaderRows).toBe(3)

		await fireEvent.click(rows[0].find('button').element)

		await flushPromises()
		await vi.dynamicImportSettled()


		await flushPromises()
		await vi.dynamicImportSettled()

		rows = await wrapper.findAll('tr[data-testid="table-row"]')
		expect(rows.length).toBe(1)
	})

	it('Expand a child row', async () => {
		const wrapper = mount(QTreeTableRow, {
			global: _global,
			props: _propsSimple
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		let rows = await wrapper.findAll('tr')

		await fireEvent.click(rows[0].find('button').element)

		await flushPromises()
		await vi.dynamicImportSettled()

		rows = await wrapper.findAll('tr')
		await fireEvent.click(rows[1].find('button').element)

		await flushPromises()
		await vi.dynamicImportSettled()

		rows = await wrapper.findAll('tr')
		const header = await wrapper.findAll('thead')
		let numHeaderRows = 0
		header.forEach(elem => {
			numHeaderRows += elem.findAll('tr').length
		})

		expect(rows.length - numHeaderRows).toBe(4)
	})

	it('Check for sub-header and sub-header columns after expand', async () => {
		const wrapper = mount(QTreeTableRow, {
			global: _global,
			props: _propsSubTable
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		let rows = await wrapper.findAll('tr')

		await fireEvent.click(rows[0].find('button').element)

		await flushPromises()
		await vi.dynamicImportSettled()

		const header = await wrapper.findAll('thead')
		const titles = await header[0].findAll('div.d-flex')

		titles.forEach((title, idx) => {
			expect(title.text()).toBe(_propsSubTable.columnHierarchy[1][idx].label)
		})

		rows = await wrapper.findAll('tr')
		let numHeaderRows = 0
		header.forEach(elem => {
			numHeaderRows += elem.findAll('tr').length
		})
		const subTables = await wrapper.findAll('tr table')
		expect(rows.length - numHeaderRows - subTables.length).toBe(3)
	})

	it('Check for sub-header and sub-header columns atter expand and collapse', async () => {
		const wrapper = mount(QTreeTableRow, {
			global: _global,
			props: _propsSubTable
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		let rows = await wrapper.findAll('tr')

		await fireEvent.click(rows[0].find('button').element)

		await flushPromises()
		await vi.dynamicImportSettled()

		let header = await wrapper.find('thead')
		const titles = await header.findAll('div.d-flex')

		titles.forEach((title, idx) => {
			expect(title.text()).toBe(_propsSubTable.columnHierarchy[1][idx].label)
		})

		await fireEvent.click(rows[0].find('button').element)

		await flushPromises()
		await vi.dynamicImportSettled()

		rows = await wrapper.findAll('tr')
		header = await wrapper.findAll('thead')

		expect(header.length).toBe(0)
		expect(rows.length).toBe(1)
	})

	it('Check for sub-header and sub-header columns after expand child row', async () => {
		const wrapper = mount(QTreeTableRow, {
			global: _global,
			props: _propsSubTable
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		let rows = wrapper.findAll('tr[data-testid="table-row"]')

		await fireEvent.click(rows[0].find('button').element)

		await flushPromises()
		await vi.dynamicImportSettled()

		let header = await wrapper.find('thead')
		const titles = await header.findAll('div.d-flex')

		titles.forEach((title, idx) => {
			expect(title.text()).toBe(_propsSubTable.columnHierarchy[1][idx].label)
		})

		rows = await wrapper.findAll('tr[data-testid="table-row"]');
		await fireEvent.click(rows[1].find('button').element)

		await flushPromises()
		await vi.dynamicImportSettled()

		rows = await wrapper.findAll('tr[data-testid="table-row"]')
		header = await wrapper.findAll('thead')
		let numHeaderRows = 0
		header.forEach(elem => {
			numHeaderRows += elem.findAll('tr[data-testid="table-row"]').length
		})

		const subTables = await wrapper.findAll('tr[data-testid="table-row"] table')
		expect(rows.length - numHeaderRows - subTables.length).toBe(4)
	})

	it('Focusing on the row and pressing right shows the sub-rows', async () => {
		render(QTreeTableRow, {
			global: _global,
			props: _propsSubTable
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get row element
		const rowElem = document.getElementsByTagName('TR')[0]

		// Focus on row and press right
		rowElem.focus()
		await fireEvent.keyDown(rowElem, { key: 'ArrowRight', keyCode: 39 })

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get sub-rows
		const allRowElems = document.querySelectorAll('tr[data-testid="table-row"]')
		expect(allRowElems.length).toBe(3)
	})

	it('After showing sub-rows, focusing on the row and pressing left hides the sub-rows', async () => {
		render(QTreeTableRow, {
			global: _global,
			props: _propsSubTable
		})

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get row element
		const rowElem = document.getElementsByTagName('TR')[0]

		// Focus on row and press right
		rowElem.focus()
		await fireEvent.keyDown(rowElem, { key: 'ArrowRight', keyCode: 39 })

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get sub-rows
		let allRowElems = document.querySelectorAll('tr[data-testid="table-row"]')
		expect(allRowElems.length).toBe(3)

		// Press left
		await fireEvent.keyDown(rowElem, { key: 'ArrowLeft', keyCode: 37 })

		await flushPromises()
		await vi.dynamicImportSettled()

		// Get sub-rows
		allRowElems = document.querySelectorAll('tr[data-testid="table-row"]')
		expect(allRowElems.length).toBe(1)
	})
})
