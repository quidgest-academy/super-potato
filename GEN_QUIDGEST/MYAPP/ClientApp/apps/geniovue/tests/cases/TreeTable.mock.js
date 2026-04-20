import listFunctions from '@/mixins/listFunctions'
import controlClass from '@/mixins/fieldControl'

export default {
	simpleUsage() {
		return {
			treeTest: new controlClass.TreeTableListControl({
				rows: [
					{
						ParentRownum: 0, //parentID
						Rownum: 1,
						FormMode: "",
						Fields: {
							"PrimaryKey": "81cc095a-03f7-43a6-a820-087c8d41a83d",
							"Key": 'Parent1',
							"Val": 'level-1',
							"Text": "Lorem ipsum dolor",
							"Numeric": 45,
							"Date": "2021-02-16 23:24:12",
							"Boolean": 1,
							"Array": "1",
							"Currency": 27.18,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 1, //parentID
						Rownum: 2,
						FormMode: "",
						Fields: {
							"PrimaryKey": "e669f856-2ee3-49ee-bf0f-13eaa21c7b18",
							"Key": 'child1',
							"Val": 'level-2',
							"Text": "sit amet",
							"Numeric": 260,
							"Date": "",
							"Boolean": 1,
							"Array": "3",
							"Currency": 67.86,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 2, //parentID
						Rownum: 3,
						FormMode: "",
						Fields: {
							"PrimaryKey": "54420e72-68b4-41c2-a41e-1d7afbcb6924",
							"Key": 'child1_1',
							"Val": 'level-3',
							"Text": "consectetur adipiscing elit",
							"Numeric": 2800,
							"Date": "2021-03-04 07:58:31",
							"Boolean": 0,
							"Array": "3",
							"Currency": 84.02,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 3, //parentID
						Rownum: 13,
						FormMode: "",
						Fields: {
							"PrimaryKey": "47556088-f88e-47fd-b618-323112f96176",
							"Key": "those",
							"Val": "thangs",
							"Text": "sed do eiusmod tempor",
							"Numeric": 14000,
							"Date": "2021-01-28 03:50:20",
							"Boolean": 1,
							"Array": "5",
							"Currency": 63.79,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 13, //parentID
						Rownum: 15,
						FormMode: "",
						Fields: {
							"PrimaryKey": "97c468d4-8b0f-4d8a-b40b-37841684e234",
							"Key": "wow",
							"Val": "cool",
							"Text": "incididunt ut labore",
							"Numeric": 330000,
							"Date": "2021-02-23 23:46:50",
							"Boolean": 0,
							"Array": "3",
							"Currency": 601.7,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 3, //parentID
						Rownum: 14,
						FormMode: "",
						Fields: {
							"PrimaryKey": "c33510bf-85e1-42c0-b5fe-3ab6d0f1adcf",
							"Key": "fgh",
							"Val": "asfd",
							"Text": "et dolore magna aliqua",
							"Numeric": 2400000,
							"Date": "2021-03-08 16:17:47",
							"Boolean": 1,
							"Array": "5",
							"Currency": 24.49,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 2,
						Rownum: 4,
						FormMode: "",
						Fields: {
							"PrimaryKey": "295e851b-f97a-45e7-ac22-4fdedd769e6a",
							"Key": "ui",
							"Val": "mgh",
							"Text": "Ut enim ad minim veniam",
							"Numeric": 80000000,
							"Date": "2021-02-12 09:40:16",
							"Boolean": 1,
							"Array": "1",
							"Currency": 688.2,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 1,
						Rownum: 5,
						FormMode: "",
						Fields: {
							"PrimaryKey": "ee7ecd97-84fd-4ec8-a780-6f46583a3108",
							"Key": "yif",
							"Val": "nm",
							"Text": "quis nostrud exercitation ullamco",
							"Numeric": 18.01,
							"Date": "2021-01-28 17:00:27",
							"Boolean": 0,
							"Array": "1",
							"Currency": 9.028,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 5,
						Rownum: 6,
						FormMode: "",
						Fields: {
							"PrimaryKey": "557e6424-39da-435b-ae56-895ed932a0b7",
							"Key": "sgj",
							"Val": "dyn",
							"Text": "laboris nisi ut aliquip ex ea",
							"Numeric": 220.23,
							"Date": "2021-03-01 12:48:48",
							"Boolean": 0,
							"Array": "3",
							"Currency": 57.13,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: false
						}
					},
					{
						ParentRownum: 0,
						Rownum: 7,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "sefr",
							"Val": "nkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: false
						}
					},
					{
						ParentRownum: 0,
						Rownum: 8,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "tefr",
							"Val": "mkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 0,
						Rownum: 9,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "wefr",
							"Val": "qkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
				],
				totalRows: 12,
				columnsOriginal: [
					{
						label: "",
						name: "PrimaryKey",
						dataType: "Text",
						isVisible: false,
					},
					{
						label: "KEY",
						name: "Key",
						area: "table",
						field: "key",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
						initialSort: true,
						initialSortOrder: "asc",
						params: {
							"type": "form",
							"formName": "FORMX",
							"mode": "SHOW",
							"isPopup": false
						},
						hasTreeShowHide:true
					},
					{
						label: "VALUE",
						name: "Val",
						area: "table",
						field: "val",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false
					},
					{
						label: "Text",
						name: "Text",
						area: "table",
						field: "text",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
					},
					{
						label: "Numeric",
						name: "Numeric",
						area: "table",
						field: "numeric",
						dataType: "Numeric",
						dataDisplay: listFunctions.numericDisplayCell,
						dataSearch: listFunctions.numericSearchCell,
						decimalPlaces: 3,
						columnClasses: "c-table__cell-numeric row-numeric",
						columnHeaderClasses: "c-table__head-numeric",
						sortable: false,
					},
					{
						label: "Date",
						name: "Date",
						area: "table",
						field: "date",
						dataType: "Date",
						dataDisplay: listFunctions.dateDisplayCell,
						dataSearch: listFunctions.dateSearchCell,
						dateTimeType: "dateTimeSeconds",
						sortable: false,
					},
					{
						label: "Boolean",
						name: "Boolean",
						area: "table",
						field: "boolean",
						dataType: "Boolean",
						dataDisplay: listFunctions.booleanDisplayCell,
						dataSearch: listFunctions.booleanSearchCell,
						sortable: false,
						supportForm: "",
						component: "q-render-boolean",
					},
					{
						label: "Array",
						name: "Array",
						area: "table",
						field: "array",
						dataType: "Array",
						dataDisplay: listFunctions.enumerationDisplayCell,
						dataSearch: listFunctions.enumerationSearchCell,
						array: {
							"1": "Low",
							"3": "Medium",
							"5": "High",
						},
						sortable: false,
					},
					{
						label: "Currency",
						name: "Currency",
						area: "table",
						field: "currency",
						dataType: "Currency",
						dataDisplay: listFunctions.currencyDisplayCell,
						dataSearch: listFunctions.currencySearchCell,
						decimalPlaces: 2,
						currency: "eur",
						columnClasses: "c-table__cell-numeric row-numeric",
						columnHeaderClasses: "c-table__head-numeric",
						sortable: false,
					},
					{
						label: "HyperLink",
						name: "HyperLink",
						area: "table",
						field: "hyperlink",
						dataType: "HyperLink",
						dataDisplay: listFunctions.hyperLinkDisplayCell,
						dataSearch: listFunctions.hyperLinkSearchCell,
						component: "q-render-hyperlink",
					}
				],
				config: {
					name: "DFLDS",
					pkColumn: "PrimaryKey",
					tableTitle: "Basic Types",
					numberFormat: {
						decimalSeparator: ",",
						groupSeparator: ".",
					},
					dateFormats: {
						date: "yyyy/MM/dd",
						dateTime: "yyyy/MM/dd HH:mm",
						dateTimeSeconds: "yyyy/MM/dd HH:mm:ss",
						hours: "HH:mm",
						use12Hour: false,
					},
					supportForm: {
						name: "FORMX",
						type: "popup",
						mode: "insert",
						repeatInsert: false,
					},
					crudActions: [
						{ "name":"show_table", "title": "custom", "icon": "duplicate", "iconSvg": "duplicate", isInReadOnly: false, "params": {"type": "form", "formName": "FORMY", "mode": "SHOW"} },
						{ "name":"SHOW", "title": "CONSULTAR57388", "iconSvg": "view", isInReadOnly: true, "params": {"type": "form", "formName": "FORMX", "mode": "SHOW"}, "separator": true },
						{ "name":"EDIT", "title": "EDITAR11616", "iconSvg": "pencil", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "EDIT"} },
						{ "name":"DUPLICATE", "title": "DUPLICAR09748", "iconSvg": "duplicate", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "DUPLICATE"} },
						{ "name":"DELETE", "title": "ELIMINAR21155", "iconSvg": "delete", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "DELETE"} }
					],
					addAction: { "name":"INSERT", "title": "INSERIR", "isInsertEnabled": () => true, "icon": "plus-sign", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "NEW"} },
					rowClickAction: { "name":"EDIT", "title": "EDITAR11616", "icon": "pencil", "params": {"type": "form", "formName": "FORMX", "mode": "EDIT"} },
					actionsPlacement: "left",
					rowValidation: {
						fnValidate: (row) => row.Fields.isValid,
						message: 'ATENCAO__ESTA_FICHA_24725'
					},
				},
				readonly: false,
				groupFilters: [
					{
						id: 'filter_GQT_Menu_111_DEVOLUCAO',
						isMultiple: false,
						selected: '3',
						filters: [
							{
								key: '1',
								value: 'To return'
							},
							{
								key: '2',
								value: 'Returned'
							},
							{
								key: '3',
								value: 'All'
							}
						]
					}
				],
				activeFilters: {
					selected: ['upcoming'],
					items: [
						{
							key: 'current',
							value: 'Active'
						},
						{
							key: 'previous',
							value: 'Inactive'
						},
						{
							key: 'upcoming',
							value: 'Futures'
						}
					],
					dateValue: {
						type: "date",
						title: "Date",
						value: ""
					}
				},
				dataImportResponse: {},
			}, this),
			// Multilevel tree data without cell action
			multiLevelTreeData: new controlClass.TreeTableListControl({
				rows: [
					{
						ParentRownum: 0, //parentID
						Rownum: 1,
						FormMode: "",
						Fields: {
							"PrimaryKey": "81cc095a-03f7-43a6-a820-087c8d41a83d",
							"Key": 'Parent1',
							"Val": 'level-1',
							"Text": "Lorem ipsum dolor",
							"Numeric": 45,
							"Date": "2021-02-16 23:24:12",
							"Boolean": 1,
							"Array": "1",
							"Currency": 27.18,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 1, //parentID
						Rownum: 2,
						FormMode: "",
						Fields: {
							"PrimaryKey": "e669f856-2ee3-49ee-bf0f-13eaa21c7b18",
							"Key": 'child1',
							"Val": 'level-2',
							"Text": "sit amet",
							"Numeric": 260,
							"Date": "",
							"Boolean": 1,
							"Array": "3",
							"Currency": 67.86,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 2, //parentID
						Rownum: 3,
						FormMode: "",
						Fields: {
							"PrimaryKey": "54420e72-68b4-41c2-a41e-1d7afbcb6924",
							"Key": 'child1_1',
							"Val": 'level-3',
							"Text": "consectetur adipiscing elit",
							"Numeric": 2800,
							"Date": "2021-03-04 07:58:31",
							"Boolean": 0,
							"Array": "3",
							"Currency": 84.02,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 3, //parentID
						Rownum: 13,
						FormMode: "",
						Fields: {
							"PrimaryKey": "47556088-f88e-47fd-b618-323112f96176",
							"Key": "those",
							"Val": "thangs",
							"Text": "sed do eiusmod tempor",
							"Numeric": 14000,
							"Date": "2021-01-28 03:50:20",
							"Boolean": 1,
							"Array": "5",
							"Currency": 63.79,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 13, //parentID
						Rownum: 15,
						FormMode: "",
						Fields: {
							"PrimaryKey": "97c468d4-8b0f-4d8a-b40b-37841684e234",
							"Key": "wow",
							"Val": "cool",
							"Text": "incididunt ut labore",
							"Numeric": 330000,
							"Date": "2021-02-23 23:46:50",
							"Boolean": 0,
							"Array": "3",
							"Currency": 601.7,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 15, //parentID
						Rownum: 14,
						FormMode: "",
						Fields: {
							"PrimaryKey": "c33510bf-85e1-42c0-b5fe-3ab6d0f1adcf",
							"Key": "fgh",
							"Val": "asfd",
							"Text": "et dolore magna aliqua",
							"Numeric": 2400000,
							"Date": "2021-03-08 16:17:47",
							"Boolean": 1,
							"Array": "5",
							"Currency": 24.49,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 14,
						Rownum: 4,
						FormMode: "",
						Fields: {
							"PrimaryKey": "295e851b-f97a-45e7-ac22-4fdedd769e6a",
							"Key": "ui",
							"Val": "mgh",
							"Text": "Ut enim ad minim veniam",
							"Numeric": 80000000,
							"Date": "2021-02-12 09:40:16",
							"Boolean": 1,
							"Array": "1",
							"Currency": 688.2,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 4,
						Rownum: 5,
						FormMode: "",
						Fields: {
							"PrimaryKey": "ee7ecd97-84fd-4ec8-a780-6f46583a3108",
							"Key": "yif",
							"Val": "nm",
							"Text": "quis nostrud exercitation ullamco",
							"Numeric": 18.01,
							"Date": "2021-01-28 17:00:27",
							"Boolean": 0,
							"Array": "1",
							"Currency": 9.028,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 5,
						Rownum: 6,
						FormMode: "",
						Fields: {
							"PrimaryKey": "557e6424-39da-435b-ae56-895ed932a0b7",
							"Key": "sgj",
							"Val": "dyn",
							"Text": "laboris nisi ut aliquip ex ea",
							"Numeric": 220.23,
							"Date": "2021-03-01 12:48:48",
							"Boolean": 0,
							"Array": "3",
							"Currency": 57.13,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: false
						}
					},
					{
						ParentRownum: 0,
						Rownum: 7,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "sefr",
							"Val": "nkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: false
						}
					},
					{
						ParentRownum: 0,
						Rownum: 8,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "tefr",
							"Val": "mkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 0,
						Rownum: 9,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "wefr",
							"Val": "qkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
				],
				totalRows: 12,
				columnsOriginal: [
					{
						label: "",
						name: "PrimaryKey",
						dataType: "Text",
						isVisible: false,
					},
					{
						label: "KEY",
						name: "Key",
						area: "table",
						field: "key",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
						initialSort: true,
						initialSortOrder: "asc",
						params: {
							"type": "form",
							"formName": "FORMX",
							"mode": "SHOW",
							"isPopup": false
						},
						hasTreeShowHide:true
					},
					{
						label: "VALUE",
						name: "Val",
						area: "table",
						field: "val",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false
					},
					{
						label: "Text",
						name: "Text",
						area: "table",
						field: "text",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
					},
					{
						label: "Numeric",
						name: "Numeric",
						area: "table",
						field: "numeric",
						dataType: "Numeric",
						dataDisplay: listFunctions.numericDisplayCell,
						dataSearch: listFunctions.numericSearchCell,
						decimalPlaces: 3,
						columnClasses: "c-table__cell-numeric row-numeric",
						columnHeaderClasses: "c-table__head-numeric",
						sortable: false,
					},
					{
						label: "Boolean",
						name: "Boolean",
						area: "table",
						field: "boolean",
						dataType: "Boolean",
						dataDisplay: listFunctions.booleanDisplayCell,
						dataSearch: listFunctions.booleanSearchCell,
						sortable: false,
						supportForm: "",
						component: "q-render-boolean",
					},
					{
						label: "Currency",
						name: "Currency",
						area: "table",
						field: "currency",
						dataType: "Currency",
						dataDisplay: listFunctions.currencyDisplayCell,
						dataSearch: listFunctions.currencySearchCell,
						decimalPlaces: 2,
						currency: "eur",
						columnClasses: "c-table__cell-numeric row-numeric",
						columnHeaderClasses: "c-table__head-numeric",
						sortable: false,
					},
				],
				config: {
					name: "MDFLDS",
					pkColumn: "PrimaryKey",
					tableTitle: "Multi level Tree",
					numberFormat: {
						decimalSeparator: ",",
						groupSeparator: ".",
					},
					dateFormats: {
						date: "yyyy/MM/dd",
						dateTime: "yyyy/MM/dd HH:mm",
						dateTimeSeconds: "yyyy/MM/dd HH:mm:ss",
						hours: "HH:mm",
						use12Hour: false,
					},
					supportForm: {
						name: "FORMX",
						type: "popup",
						mode: "insert",
						repeatInsert: false,
					},
					crudActions: [
						{ "name":"show_table", "title": "custom", "icon": "duplicate", "iconSvg": "duplicate", isInReadOnly: false, "params": {"type": "form", "formName": "FORMY", "mode": "SHOW"} },
						{ "name":"SHOW", "title": "CONSULTAR57388", "iconSvg": "view", isInReadOnly: true, "params": {"type": "form", "formName": "FORMX", "mode": "SHOW"}, "separator": true },
						{ "name":"EDIT", "title": "EDITAR11616", "iconSvg": "pencil", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "EDIT"} },
						{ "name":"DUPLICATE", "title": "DUPLICAR09748", "iconSvg": "duplicate", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "DUPLICATE"} },
						{ "name":"DELETE", "title": "ELIMINAR21155", "iconSvg": "delete", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "DELETE"} }
					],
					addAction: { "name":"INSERT", "title": "INSERIR", "isInsertEnabled": () => true, "icon": "plus-sign", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "NEW"} },
					rowClickAction: { "name":"EDIT", "title": "EDITAR11616", "icon": "pencil", "params": {"type": "form", "formName": "FORMX", "mode": "EDIT"} },
					actionsPlacement: "left",
					rowValidation: {
						fnValidate: (row) => row.Fields.isValid,
						message: 'ATENCAO__ESTA_FICHA_24725'
					},
				},
				readonly: false,
				groupFilters: [
					{
						id: 'filter_GQT_Menu_111_DEVOLUCAO',
						isMultiple: false,
						selected: '3',
						filters: [
							{
								key: '1',
								value: 'To return'
							},
							{
								key: '2',
								value: 'Returned'
							},
							{
								key: '3',
								value: 'All'
							}
						]
					}
				],
				activeFilters: {
					selected: ['upcoming'],
					items: [
						{
							key: 'current',
							value: 'Active'
						},
						{
							key: 'previous',
							value: 'Inactive'
						},
						{
							key: 'upcoming',
							value: 'Futures'
						}
					],
					dateValue: {
						type: "date",
						title: "Date",
						value: ""
					}
				},
			}, this),
			iconTreeData: new controlClass.TreeTableListControl({
				rows: [
					{
						ParentRownum: 0, //parentID
						Rownum: 1,
						FormMode: "",
						Fields: {
							"PrimaryKey": "81cc095a-03f7-43a6-a820-087c8d41a83d",
							"Key": 'Parent1',
							"Val": 'level-1',
							"Text": "Lorem ipsum dolor",
							"Numeric": 45,
							"Date": "2021-02-16 23:24:12",
							"Boolean": 1,
							"Array": "1",
							"Currency": 27.18,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 1, //parentID
						Rownum: 2,
						FormMode: "",
						Fields: {
							"PrimaryKey": "e669f856-2ee3-49ee-bf0f-13eaa21c7b18",
							"Key": 'child1',
							"Val": 'level-2',
							"Text": "sit amet",
							"Numeric": 260,
							"Date": "",
							"Boolean": 1,
							"Array": "3",
							"Currency": 67.86,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 2, //parentID
						Rownum: 3,
						FormMode: "",
						Fields: {
							"PrimaryKey": "54420e72-68b4-41c2-a41e-1d7afbcb6924",
							"Key": 'child1_1',
							"Val": 'level-3',
							"Text": "consectetur adipiscing elit",
							"Numeric": 2800,
							"Date": "2021-03-04 07:58:31",
							"Boolean": 0,
							"Array": "3",
							"Currency": 84.02,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 3, //parentID
						Rownum: 13,
						FormMode: "",
						Fields: {
							"PrimaryKey": "47556088-f88e-47fd-b618-323112f96176",
							"Key": "those",
							"Val": "thangs",
							"Text": "sed do eiusmod tempor",
							"Numeric": 14000,
							"Date": "2021-01-28 03:50:20",
							"Boolean": 1,
							"Array": "5",
							"Currency": 63.79,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 13, //parentID
						Rownum: 15,
						FormMode: "",
						Fields: {
							"PrimaryKey": "97c468d4-8b0f-4d8a-b40b-37841684e234",
							"Key": "wow",
							"Val": "cool",
							"Text": "incididunt ut labore",
							"Numeric": 330000,
							"Date": "2021-02-23 23:46:50",
							"Boolean": 0,
							"Array": "3",
							"Currency": 601.7,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 15, //parentID
						Rownum: 14,
						FormMode: "",
						Fields: {
							"PrimaryKey": "c33510bf-85e1-42c0-b5fe-3ab6d0f1adcf",
							"Key": "fgh",
							"Val": "asfd",
							"Text": "et dolore magna aliqua",
							"Numeric": 2400000,
							"Date": "2021-03-08 16:17:47",
							"Boolean": 1,
							"Array": "5",
							"Currency": 24.49,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 14,
						Rownum: 4,
						FormMode: "",
						Fields: {
							"PrimaryKey": "295e851b-f97a-45e7-ac22-4fdedd769e6a",
							"Key": "ui",
							"Val": "mgh",
							"Text": "Ut enim ad minim veniam",
							"Numeric": 80000000,
							"Date": "2021-02-12 09:40:16",
							"Boolean": 1,
							"Array": "1",
							"Currency": 688.2,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 4,
						Rownum: 5,
						FormMode: "",
						Fields: {
							"PrimaryKey": "ee7ecd97-84fd-4ec8-a780-6f46583a3108",
							"Key": "yif",
							"Val": "nm",
							"Text": "quis nostrud exercitation ullamco",
							"Numeric": 18.01,
							"Date": "2021-01-28 17:00:27",
							"Boolean": 0,
							"Array": "1",
							"Currency": 9.028,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 5,
						Rownum: 6,
						FormMode: "",
						Fields: {
							"PrimaryKey": "557e6424-39da-435b-ae56-895ed932a0b7",
							"Key": "sgj",
							"Val": "dyn",
							"Text": "laboris nisi ut aliquip ex ea",
							"Numeric": 220.23,
							"Date": "2021-03-01 12:48:48",
							"Boolean": 0,
							"Array": "3",
							"Currency": 57.13,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: false
						}
					},
					{
						ParentRownum: 0,
						Rownum: 7,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "sefr",
							"Val": "nkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: false
						}
					},
					{
						ParentRownum: 0,
						Rownum: 8,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "tefr",
							"Val": "mkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 0,
						Rownum: 9,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "wefr",
							"Val": "qkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
				],
				totalRows: 12,
				columnsOriginal: [
					{
						label: "",
						name: "PrimaryKey",
						dataType: "Text",
						isVisible: false,
					},
					{
						label: "KEY",
						name: "Key",
						area: "table",
						field: "key",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
						initialSort: true,
						initialSortOrder: "asc",
						params: {
							"type": "form",
							"formName": "FORMX",
							"mode": "SHOW",
							"isPopup": false
						},
						cellAction: true,
						hasTreeShowHide:true
					},
					{
						label: "VALUE",
						name: "Val",
						area: "table",
						field: "val",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false
					},
					{
						label: "Text",
						name: "Text",
						area: "table",
						field: "text",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
					},
					{
						label: "Numeric",
						name: "Numeric",
						area: "table",
						field: "numeric",
						dataType: "Numeric",
						dataDisplay: listFunctions.numericDisplayCell,
						dataSearch: listFunctions.numericSearchCell,
						decimalPlaces: 3,
						columnClasses: "c-table__cell-numeric row-numeric",
						columnHeaderClasses: "c-table__head-numeric",
						sortable: false,
					},
					{
						label: "Boolean",
						name: "Boolean",
						area: "table",
						field: "boolean",
						dataType: "Boolean",
						dataDisplay: listFunctions.booleanDisplayCell,
						dataSearch: listFunctions.booleanSearchCell,
						sortable: false,
						supportForm: "",
						component: "q-render-boolean",
					},
					{
						label: "Currency",
						name: "Currency",
						area: "table",
						field: "currency",
						dataType: "Currency",
						dataDisplay: listFunctions.currencyDisplayCell,
						dataSearch: listFunctions.currencySearchCell,
						decimalPlaces: 2,
						currency: "eur",
						columnClasses: "c-table__cell-numeric row-numeric",
						columnHeaderClasses: "c-table__head-numeric",
						sortable: false,
					},
				],
				config: {
					name: "IDFLDS",
					pkColumn: "PrimaryKey",
					tableTitle: "Customize Icons",
					numberFormat: {
						decimalSeparator: ",",
						groupSeparator: ".",
					},
					dateFormats: {
						date: "yyyy/MM/dd",
						dateTime: "yyyy/MM/dd HH:mm",
						dateTimeSeconds: "yyyy/MM/dd HH:mm:ss",
						hours: "HH:mm",
						use12Hour: false,
					},
					supportForm: {
						name: "FORMX",
						type: "popup",
						mode: "insert",
						repeatInsert: false,
					},
					crudActions: [
						{ "name":"show_table", "title": "custom", "icon": "duplicate", "iconSvg": "duplicate", isInReadOnly: false, "params": {"type": "form", "formName": "FORMY", "mode": "SHOW"} },
						{ "name":"SHOW", "title": "CONSULTAR57388", "iconSvg": "view", isInReadOnly: true, "params": {"type": "form", "formName": "FORMX", "mode": "SHOW"}, "separator": true },
						{ "name":"EDIT", "title": "EDITAR11616", "iconSvg": "pencil", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "EDIT"} },
						{ "name":"DUPLICATE", "title": "DUPLICAR09748", "iconSvg": "duplicate", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "DUPLICATE"} },
						{ "name":"DELETE", "title": "ELIMINAR21155", "iconSvg": "delete", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "DELETE"} }
					],
					addAction: { "name":"INSERT", "title": "INSERIR", "isInsertEnabled": () => true, "icon": "plus-sign", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "NEW"} },
					rowClickAction: { "name":"EDIT", "title": "EDITAR11616", "icon": "pencil", "params": {"type": "form", "formName": "FORMX", "mode": "EDIT"} },
					actionsPlacement: "left",
					rowValidation: {
						fnValidate: (row) => row.Fields.isValid,
						message: 'ATENCAO__ESTA_FICHA_24725'
					},
				},
				readonly: false,
				groupFilters: [
					{
						id: 'filter_GQT_Menu_111_DEVOLUCAO',
						isMultiple: false,
						selected: '3',
						filters: [
							{
								key: '1',
								value: 'To return'
							},
							{
								key: '2',
								value: 'Returned'
							},
							{
								key: '3',
								value: 'All'
							}
						]
					}
				],
				activeFilters: {
					selected: ['upcoming'],
					items: [
						{
							key: 'current',
							value: 'Active'
						},
						{
							key: 'previous',
							value: 'Inactive'
						},
						{
							key: 'upcoming',
							value: 'Futures'
						}
					],
					dateValue: {
						type: "date",
						title: "Date",
						value: ""
					}
				},
			}, this),
			invalidTreeData: new controlClass.TreeTableListControl({
				rows: [
					{
						ParentRownum: 0, //parentID
						Rownum: 1,
						FormMode: "",
						Fields: {
							"PrimaryKey": "81cc095a-03f7-43a6-a820-087c8d41a83d",
							"Key": 'Parent1',
							"Val": 'level-1',
							"Text": "Lorem ipsum dolor",
							"Numeric": 45,
							"Date": "2021-02-16 23:24:12",
							"Boolean": 1,
							"Array": "1",
							"Currency": 27.18,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 1, //parentID
						Rownum: 2,
						FormMode: "",
						Fields: {
							"PrimaryKey": "e669f856-2ee3-49ee-bf0f-13eaa21c7b18",
							"Key": 'child1',
							"Val": 'level-2',
							"Text": "sit amet",
							"Numeric": 260,
							"Date": "",
							"Boolean": 1,
							"Array": "3",
							"Currency": 67.86,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 2, //parentID
						Rownum: 0,
						FormMode: "",
						Fields: {
							"PrimaryKey": "54420e72-68b4-41c2-a41e-1d7afbcb6924",
							"Key": 'Invalid Rownum',
							"Val": 'level-3',
							"Text": "consectetur adipiscing elit",
							"Numeric": 2800,
							"Date": "2021-03-04 07:58:31",
							"Boolean": 0,
							"Array": "3",
							"Currency": 84.02,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 3, //parentID
						Rownum: 13,
						FormMode: "",
						Fields: {
							"PrimaryKey": "47556088-f88e-47fd-b618-323112f96176",
							"Key": "those",
							"Val": "thangs",
							"Text": "sed do eiusmod tempor",
							"Numeric": 14000,
							"Date": "2021-01-28 03:50:20",
							"Boolean": 1,
							"Array": "5",
							"Currency": 63.79,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 13, //parentID
						Rownum: 15,
						FormMode: "",
						Fields: {
							"PrimaryKey": "97c468d4-8b0f-4d8a-b40b-37841684e234",
							"Key": "wow",
							"Val": "cool",
							"Text": "incididunt ut labore",
							"Numeric": 330000,
							"Date": "2021-02-23 23:46:50",
							"Boolean": 0,
							"Array": "3",
							"Currency": 601.7,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 3, //parentID
						Rownum: 14,
						FormMode: "",
						Fields: {
							"PrimaryKey": "c33510bf-85e1-42c0-b5fe-3ab6d0f1adcf",
							"Key": "fgh",
							"Val": "asfd",
							"Text": "et dolore magna aliqua",
							"Numeric": 2400000,
							"Date": "2021-03-08 16:17:47",
							"Boolean": 1,
							"Array": "5",
							"Currency": 24.49,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 2,
						Rownum: 4,
						FormMode: "",
						Fields: {
							"PrimaryKey": "295e851b-f97a-45e7-ac22-4fdedd769e6a",
							"Key": "ui",
							"Val": "mgh",
							"Text": "Ut enim ad minim veniam",
							"Numeric": 80000000,
							"Date": "2021-02-12 09:40:16",
							"Boolean": 1,
							"Array": "1",
							"Currency": 688.2,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 1,
						Rownum: 5,
						FormMode: "",
						Fields: {
							"PrimaryKey": "ee7ecd97-84fd-4ec8-a780-6f46583a3108",
							"Key": "yif",
							"Val": "nm",
							"Text": "quis nostrud exercitation ullamco",
							"Numeric": 18.01,
							"Date": "2021-01-28 17:00:27",
							"Boolean": 0,
							"Array": "1",
							"Currency": 9.028,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 5,
						Rownum: 6,
						FormMode: "",
						Fields: {
							"PrimaryKey": "557e6424-39da-435b-ae56-895ed932a0b7",
							"Key": "sgj",
							"Val": "dyn",
							"Text": "laboris nisi ut aliquip ex ea",
							"Numeric": 220.23,
							"Date": "2021-03-01 12:48:48",
							"Boolean": 0,
							"Array": "3",
							"Currency": 57.13,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: false
						}
					},
					{
						ParentRownum: 0,
						Rownum: 0,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "sefr",
							"Val": "nkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: false
						}
					},
					{
						ParentRownum: 0,
						Rownum: 8,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "tefr",
							"Val": "mkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 0,
						Rownum: 9,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "wefr",
							"Val": "qkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
				],
				totalRows: 12,
				columnsOriginal: [
					{
						label: "",
						name: "PrimaryKey",
						dataType: "Text",
						isVisible: false,
					},
					{
						label: "KEY",
						name: "Key",
						area: "table",
						field: "key",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataDisplayText: listFunctions.textTextCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
						initialSort: true,
						initialSortOrder: "asc",
						supportForm: {
							"type": "form",
							"formName": "FORMX",
							"mode": "SHOW",
							"isPopup": false
						},
						cellAction: true,
						hasTreeShowHide:true,
						scrollData: 5,
					},
					{
						label: "VALUE",
						name: "Val",
						area: "table",
						field: "val",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataDisplayText: listFunctions.textTextCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
						scrollData: 5,
					},
					{
						label: "Text",
						name: "Text",
						area: "table",
						field: "text",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataDisplayText: listFunctions.textTextCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
						scrollData: 5,
					},
					{
						label: "Numeric",
						name: "Numeric",
						area: "table",
						field: "numeric",
						dataType: "Numeric",
						dataDisplay: listFunctions.numericDisplayCell,
						dataDisplayText: listFunctions.numericTextCell,
						dataSearch: listFunctions.numericSearchCell,
						decimalPlaces: 3,
						columnClasses: "c-table__cell-numeric row-numeric",
						columnHeaderClasses: "c-table__head-numeric",
						sortable: false,
						scrollData: 5,
					},
					{
						label: "Date",
						name: "Date",
						area: "table",
						field: "date",
						dataType: "Date",
						dataDisplay: listFunctions.dateDisplayCell,
						dataDisplayText: listFunctions.dateTextCell,
						dataSearch: listFunctions.dateSearchCell,
						dateTimeType: "dateTimeSeconds",
						sortable: false,
						scrollData: 5,
					},
					{
						label: "Boolean",
						name: "Boolean",
						area: "table",
						field: "boolean",
						dataType: "Boolean",
						dataDisplay: listFunctions.booleanDisplayCell,
						dataDisplayText: listFunctions.booleanTextCell,
						dataSearch: listFunctions.booleanSearchCell,
						component: "q-render-boolean",
						sortable: false,
						scrollData: 2,
					},
					{
						label: "Array",
						name: "Array",
						area: "table",
						field: "array",
						dataType: "Array",
						dataDisplay: listFunctions.enumerationDisplayCell,
						dataDisplayText: listFunctions.enumerationTextCell,
						dataSearch: listFunctions.enumerationSearchCell,
						array: {
							"1": "Low",
							"3": "Medium",
							"5": "High",
						},
						sortable: false,
						scrollData: 5,
					},
					{
						label: "Currency",
						name: "Currency",
						area: "table",
						field: "currency",
						dataType: "Currency",
						dataDisplay: listFunctions.currencyDisplayCell,
						dataDisplayText: listFunctions.currencyTextCell,
						dataSearch: listFunctions.currencySearchCell,
						decimalPlaces: 2,
						currency: "eur",
						columnClasses: "c-table__cell-numeric row-numeric",
						columnHeaderClasses: "c-table__head-numeric",
						sortable: false,
						scrollData: 5,
					},
					{
						label: "HyperLink",
						name: "HyperLink",
						area: "table",
						field: "hyperlink",
						dataType: "HyperLink",
						dataDisplay: listFunctions.hyperLinkDisplayCell,
						dataDisplayText: listFunctions.hyperLinkTextCell,
						dataSearch: listFunctions.hyperLinkSearchCell,
						component: "q-render-hyperlink",
						scrollData: 5,
					}
				],
				config: {
					name: "NDFLDS",
					pkColumn: "PrimaryKey",
					tableTitle: "Invalid Tree Data",
					numberFormat: {
						decimalSeparator: ",",
						groupSeparator: ".",
					},
					dateFormats: {
						date: "yyyy/MM/dd",
						dateTime: "yyyy/MM/dd HH:mm",
						dateTimeSeconds: "yyyy/MM/dd HH:mm:ss",
						hours: "HH:mm",
						use12Hour: false,
					},
					supportForm: {
						name: "FORMX",
						type: "popup",
						mode: "insert",
						repeatInsert: false,
					},
					crudActions: [
						{ "name":"show_table", "title": "custom", "icon": "duplicate", "iconSvg": "duplicate", isInReadOnly: false, "params": {"type": "form", "formName": "FORMY", "mode": "SHOW"} },
						{ "name":"SHOW", "title": "CONSULTAR57388", "iconSvg": "view", isInReadOnly: true, "params": {"type": "form", "formName": "FORMX", "mode": "SHOW"}, "separator": true },
						{ "name":"EDIT", "title": "EDITAR11616", "iconSvg": "pencil", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "EDIT"} },
						{ "name":"DUPLICATE", "title": "DUPLICAR09748", "iconSvg": "duplicate", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "DUPLICATE"} },
						{ "name":"DELETE", "title": "ELIMINAR21155", "iconSvg": "delete", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "DELETE"} }
					],
					addAction: { "name":"INSERT", "title": "INSERIR", "isInsertEnabled": () => true, "icon": "plus-sign", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "NEW"} },
					rowClickAction: { "name":"EDIT", "title": "EDITAR11616", "icon": "pencil", "params": {"type": "form", "formName": "FORMX", "mode": "EDIT"} },
					actionsPlacement: "left",
					rowValidation: {
						fnValidate: (row) => row.Fields.isValid,
						message: 'ATENCAO__ESTA_FICHA_24725'
					}
				}
			}, this),
			longTreeData: new controlClass.TreeTableListControl({
				rows: [
					{
						ParentRownum: 0, //parentID
						Rownum: 1,
						FormMode: "",
						Fields: {
							"PrimaryKey": "81cc095a-03f7-43a6-a820-087c8d41a83d",
							"Key": 'This is Rownum:1 & ParentRownum:0',
							"Val": 'level-1',
							"Text": "Lorem ipsum dolor",
							"Numeric": 45,
							"Date": "2021-02-16 23:24:12",
							"Boolean": 1,
							"Array": "1",
							"Currency": 27.18,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 1, //parentID
						Rownum: 2,
						FormMode: "",
						Fields: {
							"PrimaryKey": "e669f856-2ee3-49ee-bf0f-13eaa21c7b18",
							"Key": 'This is Rownum:2 & ParentRownum:1',
							"Val": 'level-2',
							"Text": "sit amet",
							"Numeric": 260,
							"Date": "2021-01-14 13:57:43",
							"Boolean": 1,
							"Array": "3",
							"Currency": 67.86,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 2, //parentID
						Rownum: 3,
						FormMode: "",
						Fields: {
							"PrimaryKey": "54420e72-68b4-41c2-a41e-1d7afbcb6924",
							"Key": 'This is Rownum:2 & ParentRownum:2',
							"Val": 'level-3',
							"Text": "consectetur adipiscing elit",
							"Numeric": 2800,
							"Date": "2021-03-04 07:58:31",
							"Boolean": 0,
							"Array": "3",
							"Currency": 84.02,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 3, //parentID
						Rownum: 13,
						FormMode: "",
						Fields: {
							"PrimaryKey": "47556088-f88e-47fd-b618-323112f96176",
							"Key": 'This is Rownum:13 & ParentRownum:3',
							"Val": "thangs",
							"Text": "sed do eiusmod tempor",
							"Numeric": 14000,
							"Date": "2021-01-28 03:50:20",
							"Boolean": 1,
							"Array": "5",
							"Currency": 63.79,
							"Checkbox": 0,
							"HyperLink": "http://www.google.com",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 13, //parentID
						Rownum: 15,
						FormMode: "",
						Fields: {
							"PrimaryKey": "97c468d4-8b0f-4d8a-b40b-37841684e234",
							"Key": 'This is Rownum:15 & ParentRownum:13',
							"Val": "cool",
							"Text": "incididunt ut labore",
							"Numeric": 330000,
							"Date": "2021-02-23 23:46:50",
							"Boolean": 0,
							"Array": "3",
							"Currency": 601.7,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 15, //parentID
						Rownum: 14,
						FormMode: "",
						Fields: {
							"PrimaryKey": "c33510bf-85e1-42c0-b5fe-3ab6d0f1adcf",
							"Key": 'This is Rownum:14 & ParentRownum:15',
							"Val": "asfd",
							"Text": "et dolore magna aliqua",
							"Numeric": 2400000,
							"Date": "2021-03-08 16:17:47",
							"Boolean": 1,
							"Array": "5",
							"Currency": 24.49,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 14,
						Rownum: 4,
						FormMode: "",
						Fields: {
							"PrimaryKey": "295e851b-f97a-45e7-ac22-4fdedd769e6a",
							"Key": 'This is Rownum:4 & ParentRownum:14',
							"Val": "mgh",
							"Text": "Ut enim ad minim veniam",
							"Numeric": 80000000,
							"Date": "2021-02-12 09:40:16",
							"Boolean": 1,
							"Array": "1",
							"Currency": 688.2,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 4,
						Rownum: 5,
						FormMode: "",
						Fields: {
							"PrimaryKey": "ee7ecd97-84fd-4ec8-a780-6f46583a3108",
							"Key": 'This is Rownum:5 & ParentRownum:4',
							"Val": "nm",
							"Text": "quis nostrud exercitation ullamco",
							"Numeric": 18.01,
							"Date": "2021-01-28 17:00:27",
							"Boolean": 0,
							"Array": "1",
							"Currency": 9.028,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 5,
						Rownum: 6,
						FormMode: "",
						Fields: {
							"PrimaryKey": "557e6424-39da-435b-ae56-895ed932a0b7",
							"Key": 'This is Rownum:6 & ParentRownum:5',
							"Val": "dyn",
							"Text": "laboris nisi ut aliquip ex ea",
							"Numeric": 220.23,
							"Date": "2021-03-01 12:48:48",
							"Boolean": 0,
							"Array": "3",
							"Currency": 57.13,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: false
						}
					},
					{
						ParentRownum: 0,
						Rownum: 7,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": 'This is Rownum:7 & ParentRownum:0',
							"Val": "nkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: false
						}
					},
					{
						ParentRownum: 0,
						Rownum: 8,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": 'This is Rownum: 8 & ParentRownum: 0',
							"Val": "mkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
					{
						ParentRownum: 0,
						Rownum: 9,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": 'This is Rownum: 9 & ParentRownum: 0',
							"Val": "qkef",
							"Text": "commodo consequat",
							"Numeric": 70000000.0456,
							"Date": "0001-01-01 00:00:00",
							"Boolean": 0,
							"Array": "5",
							"Currency": 8.675,
							"Checkbox": 0,
							"HyperLink": "",
							"Image": "",
							"Document": "",
							"Action": "",
							"Geographic": "",
							"Unknown": "",
							isValid: true
						}
					},
				],
				totalRows: 12,
				columnsOriginal: [
					{
						label: "",
						name: "PrimaryKey",
						dataType: "Text",
						isVisible: false,
					},
					{
						label: "KEY",
						name: "Key",
						area: "table",
						field: "key",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
						initialSort: true,
						initialSortOrder: "asc",
						params: {
							"type": "form",
							"formName": "FORMX",
							"mode": "SHOW",
							"isPopup": false
						},
						cellAction: true,
						hasTreeShowHide:true
					},
					{
						label: "VALUE",
						name: "Val",
						area: "table",
						field: "val",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false
					},
					{
						label: "Text",
						name: "Text",
						area: "table",
						field: "text",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
					},
					{
						label: "Numeric",
						name: "Numeric",
						area: "table",
						field: "numeric",
						dataType: "Numeric",
						dataDisplay: listFunctions.numericDisplayCell,
						dataSearch: listFunctions.numericSearchCell,
						decimalPlaces: 3,
						columnClasses: "c-table__cell-numeric row-numeric",
						columnHeaderClasses: "c-table__head-numeric",
						sortable: false,
					},
					{
						label: "Boolean",
						name: "Boolean",
						area: "table",
						field: "boolean",
						dataType: "Boolean",
						dataDisplay: listFunctions.booleanDisplayCell,
						dataSearch: listFunctions.booleanSearchCell,
						sortable: false,
						supportForm: "",
						component: "q-render-boolean",
					},
					{
						label: "Currency",
						name: "Currency",
						area: "table",
						field: "currency",
						dataType: "Currency",
						dataDisplay: listFunctions.currencyDisplayCell,
						dataSearch: listFunctions.currencySearchCell,
						decimalPlaces: 2,
						currency: "eur",
						columnClasses: "c-table__cell-numeric row-numeric",
						columnHeaderClasses: "c-table__head-numeric",
						sortable: false,
					},
				],
				config: {
					name: "LDFLD",
					pkColumn: "PrimaryKey",
					tableTitle: "Large Tree Cells",
					numberFormat: {
						decimalSeparator: ",",
						groupSeparator: ".",
					},
					dateFormats: {
						date: "yyyy/MM/dd",
						dateTime: "yyyy/MM/dd HH:mm",
						dateTimeSeconds: "yyyy/MM/dd HH:mm:ss",
						hours: "HH:mm",
						use12Hour: false,
					},
					supportForm: {
						name: "FORMX",
						type: "popup",
						mode: "insert",
						repeatInsert: false,
					},
					crudActions: [
						{ "name":"show_table", "title": "custom", "icon": "duplicate", "iconSvg": "duplicate", isInReadOnly: false, "params": {"type": "form", "formName": "FORMY", "mode": "SHOW"} },
						{ "name":"SHOW", "title": "CONSULTAR57388", "iconSvg": "view", isInReadOnly: true, "params": {"type": "form", "formName": "FORMX", "mode": "SHOW"}, "separator": true },
						{ "name":"EDIT", "title": "EDITAR11616", "iconSvg": "pencil", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "EDIT"} },
						{ "name":"DUPLICATE", "title": "DUPLICAR09748", "iconSvg": "duplicate", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "DUPLICATE"} },
						{ "name":"DELETE", "title": "ELIMINAR21155", "iconSvg": "delete", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "DELETE"} }
					],
					addAction: { "name":"INSERT", "title": "INSERIR", "isInsertEnabled": () => true, "icon": "plus-sign", isInReadOnly: false, "params": {"type": "form", "formName": "FORMX", "mode": "NEW"} },
					rowClickAction: { "name":"EDIT", "title": "EDITAR11616", "icon": "pencil", "params": {"type": "form", "formName": "FORMX", "mode": "EDIT"} },
					actionsPlacement: "left",
					rowValidation: {
						fnValidate: (row) => row.Fields.isValid,
						message: 'ATENCAO__ESTA_FICHA_24725'
					},
				},
				readonly: false,
				groupFilters: [
					{
						id: 'filter_GQT_Menu_111_DEVOLUCAO',
						isMultiple: false,
						selected: '3',
						filters: [
							{
								key: '1',
								value: 'To return'
							},
							{
								key: '2',
								value: 'Returned'
							},
							{
								key: '3',
								value: 'All'
							}
						]
					}
				],
				activeFilters: {
					selected: ['upcoming'],
					items: [
						{
							key: 'current',
							value: 'Active'
						},
						{
							key: 'previous',
							value: 'Inactive'
						},
						{
							key: 'upcoming',
							value: 'Futures'
						}
					],
					dateValue: {
						type: "date",
						title: "Date",
						value: ""
					}
				},
			}, this),
			tableTestRemoveRows: new controlClass.TreeTableListControl({
				rows: [
					{
						Rownum: 0,
						FormMode: "",
						Fields: {
							"PrimaryKey": "81cc095a-03f7-43a6-a820-087c8d41a83d",
							"Key": "this",
							"Val": "thing",
							"Numeric1": 5.344,
							"Numeric2": 8.84,
							"Numeric3": 23,
							"Currency1": 4.24,
							"Currency2": 35.37,
							"Currency3": 6.38,
							isValid: true
						}
					},
					{
						Rownum: 1,
						FormMode: "",
						Fields: {
							"PrimaryKey": "e669f856-2ee3-49ee-bf0f-13eaa21c7b18",
							"Key": "that",
							"Val": "stuff",
							"Numeric1": 2.568,
							"Numeric2": 6.03,
							"Numeric3": 18,
							"Currency1": 3.89,
							"Currency2": 47.45,
							"Currency3": 12.56,
							isValid: true
						}
					},
					{
						Rownum: 2,
						FormMode: "",
						Fields: {
							"PrimaryKey": "54420e72-68b4-41c2-a41e-1d7afbcb6924",
							"Key": "these",
							"Val": "things",
							"Numeric1": 4.846,
							"Numeric2": 10.37,
							"Numeric3": 26,
							"Currency1": 4.35,
							"Currency2": 53.50,
							"Currency3": 67.35,
							isValid: true
						}
					},
					{
						Rownum: 3,
						FormMode: "",
						Fields: {
							"PrimaryKey": "47556088-f88e-47fd-b618-323112f96176",
							"Key": "those",
							"Val": "thangs",
							"Numeric1": 8.662,
							"Numeric2": 7.87,
							"Numeric3": 85,
							"Currency1": 5.23,
							"Currency2": 28.37,
							"Currency3": 45.24,
							isValid: true
						}
					},
					{
						Rownum: 4,
						FormMode: "",
						Fields: {
							"PrimaryKey": "97c468d4-8b0f-4d8a-b40b-37841684e234",
							"Key": "wow",
							"Val": "cool",
							"Numeric1": 6.983,
							"Numeric2": 5.73,
							"Numeric3": 67,
							"Currency1": 7.38,
							"Currency2": 67.34,
							"Currency3": 34.34,
							isValid: true
						}
					},
					{
						Rownum: 5,
						FormMode: "",
						Fields: {
							"PrimaryKey": "c33510bf-85e1-42c0-b5fe-3ab6d0f1adcf",
							"Key": "fgh",
							"Val": "asfd",
							"Numeric1": 9.275,
							"Numeric2": 12.53,
							"Numeric3": 32,
							"Currency1": 5.28,
							"Currency2": 73.85,
							"Currency3": 46.54,
							isValid: true
						}
					},
					{
						Rownum: 6,
						FormMode: "",
						Fields: {
							"PrimaryKey": "295e851b-f97a-45e7-ac22-4fdedd769e6a",
							"Key": "ui",
							"Val": "mgh",
							"Numeric1": 3.827,
							"Numeric2": 57.54,
							"Numeric3": 57,
							"Currency1": 7.55,
							"Currency2": 89.50,
							"Currency3": 25.00,
							isValid: true
						}
					},
					{
						Rownum: 7,
						FormMode: "",
						Fields: {
							"PrimaryKey": "ee7ecd97-84fd-4ec8-a780-6f46583a3108",
							"Key": "yif",
							"Val": "nm",
							"Numeric1": 8.325,
							"Numeric2": 57.67,
							"Numeric3": 86,
							"Currency1": 6.64,
							"Currency2": 68.84,
							"Currency3": 45.75,
							isValid: true
						}
					},
					{
						Rownum: 8,
						FormMode: "",
						Fields: {
							"PrimaryKey": "557e6424-39da-435b-ae56-895ed932a0b7",
							"Key": "sgj",
							"Val": "dyn",
							"Numeric1": 7.745,
							"Numeric2": 38.98,
							"Numeric3": 76,
							"Currency1": 5.36,
							"Currency2": 67.68,
							"Currency3": 64.58,
							isValid: true
						}
					},
					{
						Rownum: 9,
						FormMode: "",
						Fields: {
							"PrimaryKey": "92bb35d5-b86c-4f33-935f-9cbd3f261777",
							"Key": "sefr",
							"Val": "nkef",
							"Numeric1": 6.926,
							"Numeric2": 38.57,
							"Numeric3": 27,
							"Currency1": 9.36,
							"Currency2": 78.93,
							"Currency3": 86.46,
							isValid: true
						}
					},
				],
				totalRows: 10,
				columnsOriginal: [
					{
						label: "",
						name: "PrimaryKey",
						dataType: "Text",
						isVisible: false,
					},
					{
						label: "KEY",
						name: "Key",
						area: "table",
						field: "key",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false,
						initialSort: true,
						initialSortOrder: "asc",
					},
					{
						label: "VALUE",
						name: "Val",
						area: "table",
						field: "val",
						dataType: "Text",
						dataDisplay: listFunctions.textDisplayCell,
						dataSearch: listFunctions.textSearchCell,
						sortable: false
					},
					{
						label: "Numeric1",
						name: "Numeric1",
						area: "table",
						field: "numeric1",
						dataType: "Numeric",
						dataDisplay: listFunctions.numericDisplayCell,
						dataSearch: listFunctions.numericSearchCell,
						decimalPlaces: 3,
						sortable: false,
						showTotal: true,
						isVisible: false,
					},
					{
						label: "Numeric2",
						name: "Numeric2",
						area: "table",
						field: "numeric2",
						dataType: "Numeric",
						dataDisplay: listFunctions.numericDisplayCell,
						dataSearch: listFunctions.numericSearchCell,
						decimalPlaces: 2,
						sortable: false,
						showTotal: true,
						isVisible: false,
					},
					{
						label: "Numeric3",
						name: "Numeric3",
						area: "table",
						field: "numeric3",
						dataType: "Numeric",
						dataDisplay: listFunctions.numericDisplayCell,
						dataSearch: listFunctions.numericSearchCell,
						decimalPlaces: 0,
						sortable: false,
						isVisible: false,
					},
					{
						label: "Currency1",
						name: "Currency1",
						area: "table",
						field: "currency1",
						dataType: "Currency",
						dataDisplay: listFunctions.currencyDisplayCell,
						dataSearch: listFunctions.currencySearchCell,
						decimalPlaces: 2,
						currency: "eur",
						sortable: false,
						showTotal: true,
						isVisible: false,
					},
					{
						label: "Currency2",
						name: "Currency2",
						area: "table",
						field: "currency2",
						dataType: "Currency",
						dataDisplay: listFunctions.currencyDisplayCell,
						dataSearch: listFunctions.currencySearchCell,
						decimalPlaces: 2,
						currency: "eur",
						sortable: false,
						showTotal: true,
						isVisible: false,
					},
					{
						label: "Currency3",
						name: "Currency3",
						area: "table",
						field: "currency3",
						dataType: "Currency",
						dataDisplay: listFunctions.currencyDisplayCell,
						dataSearch: listFunctions.currencySearchCell,
						decimalPlaces: 2,
						currency: "eur",
						sortable: false,
						isVisible: false,
					},
				],
				config: {
					name: "RDFLDS",
					pkColumn: "PrimaryKey",
					tableTitle: "Remove Rows",
					numberFormat: {
						decimalSeparator: ",",
						groupSeparator: ".",
					},
					dateFormats: {
						date: "yyyy/MM/dd",
						dateTime: "yyyy/MM/dd HH:mm",
						dateTimeSeconds: "yyyy/MM/dd HH:mm:ss",
						hours: "HH:mm",
						use12Hour: false,
					},
					supportForm: {
						name: "FORMX",
						type: "popup",
						mode: "insert",
						repeatInsert: false,
					},
					actionsPlacement: "left",
					rowClickActionInternal: "",
					rowBgColorSelected: "#e0e0e0",
					showSelectedForGroupCount: false,
					extendedActions: [
						"remove",
					],
					perPage: 5,
				}
			}, this),
		}
	},
	simpleUsageMethods: {
		runAction(eventName, emittedAction) {
			const str = eventName + ":\n" + JSON.stringify(emittedAction);
			alert(str);
		},
		displayEmit(emittedAction) {
			const str = JSON.stringify(emittedAction);
			alert(str);
		},
		displayAction(eventName, emittedAction) {
			const str = eventName + ":\n" + JSON.stringify(emittedAction);
			alert(str);
		},

		executeAction(emittedAction) {
			this.runAction("execute-action", emittedAction);
		},
		rowAction(emittedAction) {
			this.runAction("row-action", emittedAction);
		},
		cellAction(emittedAction) {
			this.runAction("cell-action", emittedAction);
		},
		exportDataAction(emittedAction) {
			this.displayAction("export-data-action", emittedAction);
		},
		importDataAction(emittedAction) {
			this.displayAction("import-data-action", emittedAction);
		},
		exportTemplateAction(emittedAction) {
			this.displayAction("export-template-action", emittedAction);
		},
		showImportPopupAction(emittedAction) {
			this.displayAction("show-import-popup-action", emittedAction);
		},
		hideImportPopupAction(emittedAction) {
			this.displayAction("hide-import-popup-action", emittedAction);
		},
		//FOR: EXTENDED ROW ACTIONS - REMOVE
		removeRow(rows, rowNum) {
			const rowIdx = rows.findIndex((elem) => elem.Rownum === rowNum);
			rows.splice(rowIdx, 1);
		},
		arrayToTree(rows) {
			// 1. Creating all nodes of the tree.

			const allNodes = {};
			rows.forEach((row) => {
				//	Rownum should not be equal to 0.
				if (row.Rownum === 0) {
					// eslint-disable-next-line no-console
					console.error("Row has invalid Rownum");
				}
				allNodes[row.Rownum] = { ...row, children: [] };
			});

			const treeDataRows = [];
			rows.forEach((row) => {
				if (row.ParentRownum) {
				// It's a child Node.
				// push it to parent Node.
					if (allNodes[row.ParentRownum])
						allNodes[row.ParentRownum].children.push(allNodes[row.Rownum]);
				} else {
				// Root Nodes
				// push row to treeDataRows
					treeDataRows.push(allNodes[row.Rownum]);
				}
			});
			return treeDataRows;
		},
		getColumnHierarchy(columns) {
			return listFunctions.getColumnHierarchy(columns);
		}
	}
}
