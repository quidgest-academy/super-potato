import controlClass from '@/mixins/fieldControl.js'

export default {
	simpleUsage()
	{
		return {
			styledChartConfig: {
				style: {
					// applied to chart
					dimensions: {
						width: 500, // px
						height: 300 // px
					},
					// applied globally
					backgroundColor: '#FCFFC5',
					fontColor: '#551111',
					fontFamily: 'monospace',
					title: {
						fontWeight: 'bold',
						fontSize: '24px'
					},
					subtitle: {
						fontWeight: 'bold',
						fontSize: '20px'
					},
					legend: {
						fontWeight: 'normal', // default is bold
						fontSize: '14px'
					},
					// for axis based charts
					axis: {
						xAxis: {
							title: {
								fontWeight: 'bold',
								fontSize: '16px'
							},
							labels: {
								fontWeight: 'bold',
								fontSize: '12px'
							}
						},
						yAxis: {
							title: {
								fontWeight: 'bold',
								fontSize: '16px'
							},
							labels: {
								fontWeight: 'bold',
								fontSize: '12px'
							}
						}
					},
					// for non-axis based charts
					dataLabels: {
						fontWeight: 'bold',
						fontSize: '16px'
					},
					imageDimensions: {
						width: '100px',
						height: '100px'
					}
				}
			},

			tableConfig: {
				defaultPerPage: 15,
				perPage: 15,
				actionsPlacement: 'left',
				paginationPlacement: 'left',
				hasTextWrap: false,
				allowFileExport: false,
				allowFileImport: false,
				exportOptions: [
					{
						key: 'pdf',
						value: 'Portable document format (PDF)'
					},
					{
						key: 'ods',
						value: 'Spreadsheet (ODS)'
					},
					{
						key: 'xlsx',
						value: 'Excel spreadsheet (XLSX)'
					},
					{
						key: 'csv',
						value: 'Comma-separated values (CSV)'
					},
					{
						key: 'xml',
						value: 'XML format (XML)'
					}
				],
				importOptions: [
					{
						key: 'xlsx',
						value: 'Excel spreadsheet (XLSX)'
					}
				],
				importTemplateOptions: [
					{
						key: 'xlsx',
						value: 'Excel Template Download'
					}
				],
				hasRowDragAndDrop: false,
				tableTitle: 'Examples',
				tableNamePlural: 'Examples',
				allowManageViews: true,
				allowColumnFilters: true,
				showRecordCount: false,
				showRowsSelectedCount: false,
				linkRowsSelectedAndChecked: false,
				menuForJump: '',
				sortByField: false,
				showRowDragAndDropOption: false,
				showLimitsInfo: true,
				showAfterFilter: false,
				columnResizeOptions: {},
				permissions: {
					canView: true,
					canEdit: true,
					canDuplicate: true,
					canDelete: true,
					canInsert: true
				},
				crudConditions: {},
				name: 'GQT_Menu_231',
				pkColumn: 'ValCodformx',
				tableAlias: 'FORMX',
				showAlternatePagination: true,
				crudActions: [
					{
						name: 'show',
						title: 'View',
						iconSvg: 'view',
						isInReadOnly: true,
						params: {
							type: 'form',
							formName: 'FORMX',
							mode: 'SHOW',
							isControlled: true
						}
					},
					{
						name: 'edit',
						title: 'Edit',
						iconSvg: 'pencil',
						isInReadOnly: false,
						params: {
							type: 'form',
							formName: 'FORMX',
							mode: 'EDIT',
							isControlled: true
						}
					},
					{
						name: 'duplicate',
						title: 'Duplicate',
						iconSvg: 'retweet',
						isInReadOnly: false,
						params: {
							type: 'form',
							formName: 'FORMX',
							mode: 'DUPLICATE',
							isControlled: true
						}
					},
					{
						name: 'delete',
						title: 'Delete',
						iconSvg: 'delete',
						isInReadOnly: false,
						params: {
							type: 'form',
							formName: 'FORMX',
							mode: 'DELETE',
							isControlled: true
						}
					}
				],
				addAction: {
					name: 'insert',
					title: 'Insert',
					icon: 'plus-sign',
					isInReadOnly: false,
					params: {
						type: 'form',
						formName: 'FORMX',
						mode: 'NEW',
						repeatInsertion: false,
						isControlled: true
					}
				},
				customActions: [],
				MCActions: [],
				rowClickAction: {
					name: 'form-FORMX',
					type: 'follow-up',
					params: {
						limits: [
							{
								identifier: 'id'
							}
						],
						isControlled: true,
						type: 'form',
						mode: 'SHOW',
						formName: 'FORMX'
					}
				},
				formsDefinition: {
					FORMX: {
						isPopup: false
					}
				},
				defaultSearchColumnName: '',
				defaultSearchColumnNameOriginal: '',
				initialSortColumnName: '',
				initialSortColumnOrder: '',
				page: 1
			},

			lineChartData: new controlClass.TableSpecialRenderingControl({
				viewModes: [
					{
						type: 'chart'
					}
				],
				viewMode: {
					id: 'CHART',
					type: 'chart',
					subtype: 'genericgraph',
					label: 'Gráfico',
					visible: true,
					order: 2,
					mappingVariables: {
						xaxis: {
							allowsMultiple: false,
							sources: [
								'GRA.DT'
							]
						},
						yaxis: {
							allowsMultiple: true,
							sources: [
								'GRA.NUMBER'
							]
						}
					},
					styleVariables: {
						showLastN: {
							value: -1,
							isMapped: false
						},
						xaxisType: {
							value: 'datetime',
							isMapped: false
						},
						description: {
							value: '',
							isMapped: false
						},
						showLegend: {
							value: true,
							isMapped: false
						},
						alignDescription: {
							value: 'left',
							isMapped: false
						},
						showBreaks: {
							value: false,
							isMapped: false
						},
						legendVerticalAlign: {
							value: 'bottom',
							isMapped: false
						},
						zoomType: {
							value: 'x',
							isMapped: false
						},
						yaxisType: {
							value: 'linear',
							isMapped: false
						},
						firstColor: {
							value: 'undefined',
							isMapped: false
						},
						legendAlign: {
							value: 'center',
							isMapped: false
						},
						xaxisName: {
							value: 'XAxis',
							isMapped: false
						},
						chartType: {
							value: 'line',
							isMapped: false
						},
						graphTitle: {
							value: 'Graph',
							isMapped: false
						},
						legendFloating: {
							value: false,
							isMapped: false
						},
						groupType: {
							value: 'join',
							isMapped: false
						},
						widthPercentage: {
							value: 100,
							isMapped: false
						},
						valuesDecimals: {
							value: 0,
							isMapped: false
						},
						heightPx: {
							value: 400,
							isMapped: false
						},
						lineMarker: {
							value: 'enabled',
							isMapped: false
						},
						yaxisName: {
							value: 'YAxis',
							isMapped: false
						},
						inverted: {
							value: false,
							isMapped: false
						},
						pieInnerSizePercentage: {
							value: 0,
							isMapped: false
						},
						legendYPosition: {
							value: 0,
							isMapped: false
						},
						legendXPosition: {
							value: 0,
							isMapped: false
						},
						stackingType: {
							value: 'undefined',
							isMapped: false
						},
						legendLayout: {
							value: 'horizontal',
							isMapped: false
						},
						showPieLabel: {
							value: 'outside',
							isMapped: false
						},
						invertColorArray: {
							value: false,
							isMapped: false
						},
						enableHover: {
							value: true,
							isMapped: false
						},
						showLabels: {
							value: true,
							isMapped: false
						},
						chartColorArray: {
							value: 'Rainbow',
							isMapped: false
						}
					},
					mappedValues: [
						{
							rowKey: '00000000-0000-0000-0000-000000000003',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							yaxis: [
								{
									value: 1,
									rawData: 1,
									source: {
										order: 1,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Aqua',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							],
							xaxis: {
								value: '2021-11-29T00:00:00.000Z',
								rawData: '2021-11-29T00:00:00.000Z',
								source: {
									order: 4,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							}
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000007',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							yaxis: [
								{
									value: 2,
									rawData: 2,
									source: {
										order: 1,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Aqua',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							],
							xaxis: {
								value: '2021-11-24T17:55:00.000Z',
								rawData: '2021-11-24T17:55:00.000Z',
								source: {
									order: 4,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							}
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000001',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							yaxis: [
								{
									value: 3,
									rawData: 3,
									source: {
										order: 1,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Aqua',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							],
							xaxis: {
								value: '2021-11-30T00:00:00.000Z',
								rawData: '2021-11-30T00:00:00.000Z',
								source: {
									order: 4,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							}
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000006',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							yaxis: [
								{
									value: 5,
									rawData: 5,
									source: {
										order: 1,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Aqua',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							],
							xaxis: {
								value: '2021-11-27T23:50:00.000Z',
								rawData: '2021-11-27T23:50:00.000Z',
								source: {
									order: 4,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							}
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000005',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							yaxis: [
								{
									value: 7,
									rawData: 7,
									source: {
										order: 1,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Aqua',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							],
							xaxis: {
								value: '2021-11-23T23:49:56.000Z',
								rawData: '2021-11-23T23:49:56.000Z',
								source: {
									order: 4,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							}
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000004',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							yaxis: [
								{
									value: 8,
									rawData: 8,
									source: {
										order: 1,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Aqua',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							],
							xaxis: {
								value: '2021-11-24T23:41:00.000Z',
								rawData: '2021-11-24T23:41:00.000Z',
								source: {
									order: 4,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							}
						}
					],
					readonly: true,
					componentName: 'q-chart',
					containerId: 'q-chart-container'
				}
			}, {
				$getResource: resId => resId
			}),

			areaChartData: new controlClass.TableSpecialRenderingControl({
				viewModes: [
					{
						type: 'chart'
					}
				],
				viewMode: {
					id: 'CHART',
					type: 'chart',
					subtype: 'genericgraph',
					label: 'Gráfico',
					visible: true,
					order: 2,
					mappingVariables: {
						xaxis: {
							allowsMultiple: false,
							sources: [
								'GRA.DT'
							]
						},
						yaxis: {
							allowsMultiple: true,
							sources: [
								'GRA.NUMBER'
							]
						}
					},
					styleVariables: {
						legendLayout: {
							value: 'horizontal',
							isMapped: false
						},
						xaxisName: {
							value: 'XAxis',
							isMapped: false
						},
						legendYPosition: {
							value: 0,
							isMapped: false
						},
						stackingType: {
							value: 'undefined',
							isMapped: false
						},
						widthPercentage: {
							value: 100,
							isMapped: false
						},
						showLastN: {
							value: -1,
							isMapped: false
						},
						yaxisType: {
							value: 'linear',
							isMapped: false
						},
						lineMarker: {
							value: 'enabled',
							isMapped: false
						},
						showLabels: {
							value: true,
							isMapped: false
						},
						chartColorArray: {
							value: 'Highcharts default',
							isMapped: false
						},
						inverted: {
							value: false,
							isMapped: false
						},
						legendVerticalAlign: {
							value: 'bottom',
							isMapped: false
						},
						showPieLabel: {
							value: 'outside',
							isMapped: false
						},
						heightPx: {
							value: 400,
							isMapped: false
						},
						yaxisName: {
							value: 'YAxis',
							isMapped: false
						},
						zoomType: {
							value: 'x',
							isMapped: false
						},
						valuesDecimals: {
							value: 0,
							isMapped: false
						},
						xaxisType: {
							value: 'linear',
							isMapped: false
						},
						showLegend: {
							value: true,
							isMapped: false
						},
						legendXPosition: {
							value: 0,
							isMapped: false
						},
						legendAlign: {
							value: 'center',
							isMapped: false
						},
						invertColorArray: {
							value: false,
							isMapped: false
						},
						pieInnerSizePercentage: {
							value: 0,
							isMapped: false
						},
						chartType: {
							value: 'area',
							isMapped: false
						},
						firstColor: {
							value: 'undefined',
							isMapped: false
						},
						showBreaks: {
							value: false,
							isMapped: false
						},
						legendFloating: {
							value: false,
							isMapped: false
						},
						groupType: {
							value: 'join',
							isMapped: false
						},
						alignDescription: {
							value: 'left',
							isMapped: false
						},
						enableHover: {
							value: true,
							isMapped: false
						},
						graphTitle: {
							value: '',
							isMapped: false
						},
						description: {
							value: '',
							isMapped: false
						}
					},
					mappedValues: [
						{
							rowKey: '00000000-0000-0000-0000-000000000005',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-23T23:49:56.000Z',
								rawData: '2021-11-23T23:49:56.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 7,
									rawData: 7,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000007',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-24T17:55:00.000Z',
								rawData: '2021-11-24T17:55:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 2,
									rawData: 2,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000004',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-24T23:41:00.000Z',
								rawData: '2021-11-24T23:41:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 8,
									rawData: 8,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000006',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-27T23:50:00.000Z',
								rawData: '2021-11-27T23:50:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 5,
									rawData: 5,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000003',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-29T00:00:00.000Z',
								rawData: '2021-11-29T00:00:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 1,
									rawData: 1,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000001',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-30T00:00:00.000Z',
								rawData: '2021-11-30T00:00:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 3,
									rawData: 3,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						}
					],
					readonly: true,
					componentName: 'q-chart',
					containerId: 'q-chart-container'
				}
			}, {
				$getResource: resId => resId
			}),

			splineChartData: new controlClass.TableSpecialRenderingControl({
				viewModes: [
					{
						type: 'chart'
					}
				],
				viewMode: {
					id: 'CHART',
					type: 'chart',
					subtype: 'genericgraph',
					label: 'Gráfico',
					visible: true,
					order: 2,
					mappingVariables: {
						xaxis: {
							allowsMultiple: false,
							sources: [
								'GRA.DT'
							]
						},
						yaxis: {
							allowsMultiple: true,
							sources: [
								'GRA.NUMBER'
							]
						}
					},
					styleVariables: {
						yaxisName: {
							value: 'YAxis',
							isMapped: false
						},
						showBreaks: {
							value: false,
							isMapped: false
						},
						legendAlign: {
							value: 'center',
							isMapped: false
						},
						stackingType: {
							value: 'undefined',
							isMapped: false
						},
						legendFloating: {
							value: false,
							isMapped: false
						},
						enableHover: {
							value: true,
							isMapped: false
						},
						pieInnerSizePercentage: {
							value: 0,
							isMapped: false
						},
						chartColorArray: {
							value: 'Highcharts default',
							isMapped: false
						},
						showLastN: {
							value: -1,
							isMapped: false
						},
						heightPx: {
							value: 400,
							isMapped: false
						},
						xaxisType: {
							value: 'linear',
							isMapped: false
						},
						showPieLabel: {
							value: 'outside',
							isMapped: false
						},
						chartType: {
							value: 'spline',
							isMapped: false
						},
						inverted: {
							value: false,
							isMapped: false
						},
						xaxisName: {
							value: 'XAxis',
							isMapped: false
						},
						widthPercentage: {
							value: 100,
							isMapped: false
						},
						lineMarker: {
							value: 'enabled',
							isMapped: false
						},
						legendYPosition: {
							value: 0,
							isMapped: false
						},
						graphTitle: {
							value: '',
							isMapped: false
						},
						yaxisType: {
							value: 'linear',
							isMapped: false
						},
						showLegend: {
							value: true,
							isMapped: false
						},
						zoomType: {
							value: 'x',
							isMapped: false
						},
						firstColor: {
							value: 'undefined',
							isMapped: false
						},
						groupType: {
							value: 'join',
							isMapped: false
						},
						invertColorArray: {
							value: false,
							isMapped: false
						},
						showLabels: {
							value: true,
							isMapped: false
						},
						alignDescription: {
							value: 'left',
							isMapped: false
						},
						valuesDecimals: {
							value: 0,
							isMapped: false
						},
						legendLayout: {
							value: 'horizontal',
							isMapped: false
						},
						legendVerticalAlign: {
							value: 'bottom',
							isMapped: false
						},
						legendXPosition: {
							value: 0,
							isMapped: false
						},
						description: {
							value: '',
							isMapped: false
						}
					},
					mappedValues: [
						{
							rowKey: '00000000-0000-0000-0000-000000000005',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-23T23:49:56.000Z',
								rawData: '2021-11-23T23:49:56.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 7,
									rawData: 7,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000007',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-24T17:55:00.000Z',
								rawData: '2021-11-24T17:55:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 2,
									rawData: 2,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000004',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-24T23:41:00.000Z',
								rawData: '2021-11-24T23:41:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 8,
									rawData: 8,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000006',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-27T23:50:00.000Z',
								rawData: '2021-11-27T23:50:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 5,
									rawData: 5,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000003',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-29T00:00:00.000Z',
								rawData: '2021-11-29T00:00:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 1,
									rawData: 1,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000001',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-30T00:00:00.000Z',
								rawData: '2021-11-30T00:00:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 3,
									rawData: 3,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						}
					],
					readonly: true,
					componentName: 'q-chart',
					containerId: 'q-chart-container'
				}
			}, {
				$getResource: resId => resId
			}),

			areaSplineChartData: new controlClass.TableSpecialRenderingControl({
				viewModes: [
					{
						type: 'chart'
					}
				],
				viewMode: {
					id: 'CHART',
					type: 'chart',
					subtype: 'genericgraph',
					label: 'Gráfico',
					visible: true,
					order: 2,
					mappingVariables: {
						xaxis: {
							allowsMultiple: false,
							sources: [
								'GRA.DT'
							]
						},
						yaxis: {
							allowsMultiple: true,
							sources: [
								'GRA.NUMBER'
							]
						}
					},
					styleVariables: {
						showBreaks: {
							value: false,
							isMapped: false
						},
						legendFloating: {
							value: false,
							isMapped: false
						},
						valuesDecimals: {
							value: 0,
							isMapped: false
						},
						legendVerticalAlign: {
							value: 'bottom',
							isMapped: false
						},
						heightPx: {
							value: 400,
							isMapped: false
						},
						inverted: {
							value: false,
							isMapped: false
						},
						invertColorArray: {
							value: false,
							isMapped: false
						},
						legendXPosition: {
							value: 0,
							isMapped: false
						},
						lineMarker: {
							value: 'enabled',
							isMapped: false
						},
						yaxisName: {
							value: 'YAxis',
							isMapped: false
						},
						chartColorArray: {
							value: 'Highcharts default',
							isMapped: false
						},
						zoomType: {
							value: 'x',
							isMapped: false
						},
						alignDescription: {
							value: 'left',
							isMapped: false
						},
						description: {
							value: '',
							isMapped: false
						},
						groupType: {
							value: 'join',
							isMapped: false
						},
						legendLayout: {
							value: 'horizontal',
							isMapped: false
						},
						stackingType: {
							value: 'undefined',
							isMapped: false
						},
						legendYPosition: {
							value: 0,
							isMapped: false
						},
						showLastN: {
							value: -1,
							isMapped: false
						},
						firstColor: {
							value: 'undefined',
							isMapped: false
						},
						graphTitle: {
							value: '',
							isMapped: false
						},
						widthPercentage: {
							value: 100,
							isMapped: false
						},
						xaxisType: {
							value: 'linear',
							isMapped: false
						},
						chartType: {
							value: 'areaspline',
							isMapped: false
						},
						showLegend: {
							value: true,
							isMapped: false
						},
						legendAlign: {
							value: 'center',
							isMapped: false
						},
						showPieLabel: {
							value: 'outside',
							isMapped: false
						},
						showLabels: {
							value: true,
							isMapped: false
						},
						enableHover: {
							value: true,
							isMapped: false
						},
						xaxisName: {
							value: 'XAxis',
							isMapped: false
						},
						pieInnerSizePercentage: {
							value: 0,
							isMapped: false
						},
						yaxisType: {
							value: 'linear',
							isMapped: false
						}
					},
					mappedValues: [
						{
							rowKey: '00000000-0000-0000-0000-000000000005',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-23T23:49:56.000Z',
								rawData: '2021-11-23T23:49:56.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 7,
									rawData: 7,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000007',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-24T17:55:00.000Z',
								rawData: '2021-11-24T17:55:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 2,
									rawData: 2,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000004',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-24T23:41:00.000Z',
								rawData: '2021-11-24T23:41:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 8,
									rawData: 8,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000006',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-27T23:50:00.000Z',
								rawData: '2021-11-27T23:50:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 5,
									rawData: 5,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000003',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-29T00:00:00.000Z',
								rawData: '2021-11-29T00:00:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 1,
									rawData: 1,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000001',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-30T00:00:00.000Z',
								rawData: '2021-11-30T00:00:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 3,
									rawData: 3,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						}
					],
					readonly: true,
					componentName: 'q-chart',
					containerId: 'q-chart-container'
				}
			}, {
				$getResource: resId => resId
			}),

			pieChartData: new controlClass.TableSpecialRenderingControl({
				viewModes: [
					{
						type: 'chart'
					}
				],
				viewMode: {
					id: 'CHART',
					type: 'chart',
					subtype: 'genericgraph',
					label: 'Gráfico',
					visible: true,
					order: 2,
					mappingVariables: {
						xaxis: {
							allowsMultiple: false,
							sources: [
								'INFOG.TIPO'
							]
						},
						yaxis: {
							allowsMultiple: true,
							sources: [
								'INFOG.INFO'
							]
						}
					},
					styleVariables: {
						graphTitle: {
							value: 'Graph',
							isMapped: false
						},
						inverted: {
							value: false,
							isMapped: false
						},
						xaxisName: {
							value: 'XAxis',
							isMapped: false
						},
						enableHover: {
							value: true,
							isMapped: false
						},
						xaxisType: {
							value: 'linear',
							isMapped: false
						},
						description: {
							value: 'Graph description',
							isMapped: false
						},
						showLastN: {
							value: -1,
							isMapped: false
						},
						showLegend: {
							value: true,
							isMapped: false
						},
						legendLayout: {
							value: 'horizontal',
							isMapped: false
						},
						legendVerticalAlign: {
							value: 'bottom',
							isMapped: false
						},
						showBreaks: {
							value: false,
							isMapped: false
						},
						valuesDecimals: {
							value: 0,
							isMapped: false
						},
						legendFloating: {
							value: false,
							isMapped: false
						},
						heightPx: {
							value: 400,
							isMapped: false
						},
						pieInnerSizePercentage: {
							value: 0,
							isMapped: false
						},
						alignDescription: {
							value: 'left',
							isMapped: false
						},
						chartType: {
							value: 'pie',
							isMapped: false
						},
						yaxisType: {
							value: 'linear',
							isMapped: false
						},
						firstColor: {
							value: 'undefined',
							isMapped: false
						},
						lineMarker: {
							value: 'enabled',
							isMapped: false
						},
						zoomType: {
							value: 'x',
							isMapped: false
						},
						legendXPosition: {
							value: 0,
							isMapped: false
						},
						legendYPosition: {
							value: 0,
							isMapped: false
						},
						widthPercentage: {
							value: 100,
							isMapped: false
						},
						chartColorArray: {
							value: 'Highcharts default',
							isMapped: false
						},
						yaxisName: {
							value: 'YAxis',
							isMapped: false
						},
						invertColorArray: {
							value: false,
							isMapped: false
						},
						stackingType: {
							value: 'undefined',
							isMapped: false
						},
						showLabels: {
							value: true,
							isMapped: false
						},
						legendAlign: {
							value: 'center',
							isMapped: false
						},
						showPieLabel: {
							value: 'outside',
							isMapped: false
						},
						groupType: {
							value: 'join',
							isMapped: false
						}
					},
					mappedValues: [
						{
							rowKey: '1',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 'A',
								rawData: 'A',
								source: {
									order: 1,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValTipo',
									area: 'INFOG',
									field: 'TIPO',
									label: 'Tipo',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							},
							yaxis: [
								{
									value: 17,
									rawData: 17,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValInfo',
										area: 'INFOG',
										field: 'INFO',
										label: 'Info',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 3,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '2',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 'B',
								rawData: 'B',
								source: {
									order: 1,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValTipo',
									area: 'INFOG',
									field: 'TIPO',
									label: 'Tipo',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							},
							yaxis: [
								{
									value: 1,
									rawData: 1,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValInfo',
										area: 'INFOG',
										field: 'INFO',
										label: 'Info',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 3,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '3',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 'C',
								rawData: 'C',
								source: {
									order: 1,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValTipo',
									area: 'INFOG',
									field: 'TIPO',
									label: 'Tipo',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							},
							yaxis: [
								{
									value: 8,
									rawData: 8,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValInfo',
										area: 'INFOG',
										field: 'INFO',
										label: 'Info',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 3,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						}
					],
					readonly: true,
					componentName: 'q-chart',
					containerId: 'q-chart-container'
				}
			}, {
				$getResource: resId => resId
			}),

			semiPieChartData: new controlClass.TableSpecialRenderingControl({
				viewModes: [
					{
						type: 'chart'
					}
				],
				viewMode: {
					id: 'CHART',
					type: 'chart',
					subtype: 'genericgraph',
					label: 'Gráfico',
					visible: true,
					order: 2,
					mappingVariables: {
						xaxis: {
							allowsMultiple: false,
							sources: [
								'INFOG.TIPO'
							]
						},
						yaxis: {
							allowsMultiple: true,
							sources: [
								'INFOG.INFO'
							]
						}
					},
					styleVariables: {
						graphTitle: {
							value: '',
							isMapped: false
						},
						valuesDecimals: {
							value: 0,
							isMapped: false
						},
						legendVerticalAlign: {
							value: 'bottom',
							isMapped: false
						},
						yaxisName: {
							value: 'YAxis',
							isMapped: false
						},
						alignDescription: {
							value: 'left',
							isMapped: false
						},
						showLastN: {
							value: -1,
							isMapped: false
						},
						lineMarker: {
							value: 'enabled',
							isMapped: false
						},
						legendLayout: {
							value: 'horizontal',
							isMapped: false
						},
						enableHover: {
							value: true,
							isMapped: false
						},
						showLegend: {
							value: true,
							isMapped: false
						},
						inverted: {
							value: false,
							isMapped: false
						},
						legendFloating: {
							value: false,
							isMapped: false
						},
						invertColorArray: {
							value: false,
							isMapped: false
						},
						chartColorArray: {
							value: 'Highcharts default',
							isMapped: false
						},
						legendYPosition: {
							value: 0,
							isMapped: false
						},
						legendXPosition: {
							value: 0,
							isMapped: false
						},
						yaxisType: {
							value: 'linear',
							isMapped: false
						},
						groupType: {
							value: 'join',
							isMapped: false
						},
						description: {
							value: '',
							isMapped: false
						},
						firstColor: {
							value: 'undefined',
							isMapped: false
						},
						showPieLabel: {
							value: 'outside',
							isMapped: false
						},
						xaxisName: {
							value: 'XAxis',
							isMapped: false
						},
						heightPx: {
							value: 400,
							isMapped: false
						},
						chartType: {
							value: 'semi-pie',
							isMapped: false
						},
						stackingType: {
							value: 'undefined',
							isMapped: false
						},
						showBreaks: {
							value: false,
							isMapped: false
						},
						widthPercentage: {
							value: 100,
							isMapped: false
						},
						legendAlign: {
							value: 'center',
							isMapped: false
						},
						pieInnerSizePercentage: {
							value: 0,
							isMapped: false
						},
						zoomType: {
							value: 'x',
							isMapped: false
						},
						showLabels: {
							value: true,
							isMapped: false
						},
						xaxisType: {
							value: 'linear',
							isMapped: false
						}
					},
					mappedValues: [
						{
							rowKey: '1',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 'A',
								rawData: 'A',
								source: {
									order: 2,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValTipo',
									area: 'INFOG',
									field: 'TIPO',
									label: 'Tipo',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							},
							yaxis: [
								{
									value: 17,
									rawData: 17,
									source: {
										order: 3,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValInfo',
										area: 'INFOG',
										field: 'INFO',
										label: 'Info',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 3,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '2',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 'B',
								rawData: 'B',
								source: {
									order: 2,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValTipo',
									area: 'INFOG',
									field: 'TIPO',
									label: 'Tipo',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							},
							yaxis: [
								{
									value: 1,
									rawData: 1,
									source: {
										order: 3,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValInfo',
										area: 'INFOG',
										field: 'INFO',
										label: 'Info',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 3,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '3',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 'C',
								rawData: 'C',
								source: {
									order: 2,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValTipo',
									area: 'INFOG',
									field: 'TIPO',
									label: 'Tipo',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							},
							yaxis: [
								{
									value: 8,
									rawData: 8,
									source: {
										order: 3,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValInfo',
										area: 'INFOG',
										field: 'INFO',
										label: 'Info',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 3,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						}
					],
					readonly: true,
					componentName: 'q-chart',
					containerId: 'q-chart-container'
				}
			}, {
				$getResource: resId => resId
			}),

			bubbleChartData: new controlClass.TableSpecialRenderingControl({
				viewModes: [
					{
						type: 'chart'
					}
				],
				viewMode: {
					id: 'CHART',
					type: 'chart',
					subtype: 'genericgraph',
					label: 'Gráfico',
					visible: true,
					order: 2,
					mappingVariables: {
						xaxis: {
							allowsMultiple: false,
							sources: [
								'GRA.NUMBER'
							]
						},
						yaxis: {
							allowsMultiple: true,
							sources: [
								'GRA.NUMBER2'
							]
						},
						zaxis: {
							allowsMultiple: false,
							sources: [
								'GRA.NUMBER3'
							]
						}
					},
					styleVariables: {
						legendAlign: {
							value: 'center',
							isMapped: false
						},
						valuesDecimals: {
							value: 0,
							isMapped: false
						},
						legendFloating: {
							value: false,
							isMapped: false
						},
						firstColor: {
							value: 'undefined',
							isMapped: false
						},
						showPieLabel: {
							value: 'outside',
							isMapped: false
						},
						chartColorArray: {
							value: 'Highcharts default',
							isMapped: false
						},
						heightPx: {
							value: 400,
							isMapped: false
						},
						groupType: {
							value: 'join',
							isMapped: false
						},
						legendXPosition: {
							value: 0,
							isMapped: false
						},
						xaxisType: {
							value: 'linear',
							isMapped: false
						},
						stackingType: {
							value: 'undefined',
							isMapped: false
						},
						chartType: {
							value: 'bubble',
							isMapped: false
						},
						showLabels: {
							value: true,
							isMapped: false
						},
						showBreaks: {
							value: false,
							isMapped: false
						},
						enableHover: {
							value: true,
							isMapped: false
						},
						lineMarker: {
							value: 'enabled',
							isMapped: false
						},
						legendLayout: {
							value: 'horizontal',
							isMapped: false
						},
						legendYPosition: {
							value: 0,
							isMapped: false
						},
						showLegend: {
							value: true,
							isMapped: false
						},
						showLastN: {
							value: -1,
							isMapped: false
						},
						inverted: {
							value: false,
							isMapped: false
						},
						invertColorArray: {
							value: false,
							isMapped: false
						},
						widthPercentage: {
							value: 100,
							isMapped: false
						},
						legendVerticalAlign: {
							value: 'bottom',
							isMapped: false
						},
						alignDescription: {
							value: 'left',
							isMapped: false
						},
						pieInnerSizePercentage: {
							value: 0,
							isMapped: false
						},
						yaxisType: {
							value: 'linear',
							isMapped: false
						},
						zoomType: {
							value: 'x',
							isMapped: false
						},
						yaxisName: {
							value: 'YAxis',
							isMapped: false
						},
						xaxisName: {
							value: 'XAxis',
							isMapped: false
						},
						graphTitle: {
							value: '',
							isMapped: false
						},
						description: {
							value: '',
							isMapped: false
						}
					},
					mappedValues: [
						{
							rowKey: '00000000-0000-0000-0000-000000000005',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 7,
								rawData: 7,
								source: {
									order: 2,
									dataType: 'Numeric',
									searchFieldType: 'num',
									component: null,
									name: 'ValNumber',
									area: 'GRA',
									field: 'NUMBER',
									label: 'Número',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									maxDigits: 1,
									decimalPlaces: 2,
									showTotal: true,
									columnClasses: 'c-table__cell-numeric row-numeric',
									columnHeaderClasses: 'c-table__head-numeric'
								}
							},
							yaxis: [
								{
									value: 8.08,
									rawData: 8.08,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							],
							zaxis: {
								value: '9',
								rawData: '9',
								source: {
									order: 8,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValNumber3',
									area: 'GRA',
									field: 'NUMBER3',
									label: 'Number',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							}
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000007',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 2,
								rawData: 2,
								source: {
									order: 2,
									dataType: 'Numeric',
									searchFieldType: 'num',
									component: null,
									name: 'ValNumber',
									area: 'GRA',
									field: 'NUMBER',
									label: 'Número',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									maxDigits: 1,
									decimalPlaces: 2,
									showTotal: true,
									columnClasses: 'c-table__cell-numeric row-numeric',
									columnHeaderClasses: 'c-table__head-numeric'
								}
							},
							yaxis: [
								{
									value: 3.45,
									rawData: 3.45,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							],
							zaxis: {
								value: '2',
								rawData: '2',
								source: {
									order: 8,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValNumber3',
									area: 'GRA',
									field: 'NUMBER3',
									label: 'Number',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							}
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000004',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 8,
								rawData: 8,
								source: {
									order: 2,
									dataType: 'Numeric',
									searchFieldType: 'num',
									component: null,
									name: 'ValNumber',
									area: 'GRA',
									field: 'NUMBER',
									label: 'Número',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									maxDigits: 1,
									decimalPlaces: 2,
									showTotal: true,
									columnClasses: 'c-table__cell-numeric row-numeric',
									columnHeaderClasses: 'c-table__head-numeric'
								}
							},
							yaxis: [
								{
									value: 9,
									rawData: 9,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							],
							zaxis: {
								value: '10',
								rawData: '10',
								source: {
									order: 8,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValNumber3',
									area: 'GRA',
									field: 'NUMBER3',
									label: 'Number',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							}
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000006',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 5,
								rawData: 5,
								source: {
									order: 2,
									dataType: 'Numeric',
									searchFieldType: 'num',
									component: null,
									name: 'ValNumber',
									area: 'GRA',
									field: 'NUMBER',
									label: 'Número',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									maxDigits: 1,
									decimalPlaces: 2,
									showTotal: true,
									columnClasses: 'c-table__cell-numeric row-numeric',
									columnHeaderClasses: 'c-table__head-numeric'
								}
							},
							yaxis: [
								{
									value: 6,
									rawData: 6,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							],
							zaxis: {
								value: '7',
								rawData: '7',
								source: {
									order: 8,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValNumber3',
									area: 'GRA',
									field: 'NUMBER3',
									label: 'Number',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							}
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000003',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 1,
								rawData: 1,
								source: {
									order: 2,
									dataType: 'Numeric',
									searchFieldType: 'num',
									component: null,
									name: 'ValNumber',
									area: 'GRA',
									field: 'NUMBER',
									label: 'Número',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									maxDigits: 1,
									decimalPlaces: 2,
									showTotal: true,
									columnClasses: 'c-table__cell-numeric row-numeric',
									columnHeaderClasses: 'c-table__head-numeric'
								}
							},
							yaxis: [
								{
									value: 2,
									rawData: 2,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							],
							zaxis: {
								value: '3',
								rawData: '3',
								source: {
									order: 8,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValNumber3',
									area: 'GRA',
									field: 'NUMBER3',
									label: 'Number',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							}
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000001',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: 3,
								rawData: 3,
								source: {
									order: 2,
									dataType: 'Numeric',
									searchFieldType: 'num',
									component: null,
									name: 'ValNumber',
									area: 'GRA',
									field: 'NUMBER',
									label: 'Número',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									maxDigits: 1,
									decimalPlaces: 2,
									showTotal: true,
									columnClasses: 'c-table__cell-numeric row-numeric',
									columnHeaderClasses: 'c-table__head-numeric'
								}
							},
							yaxis: [
								{
									value: 4,
									rawData: 4,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							],
							zaxis: {
								value: '5',
								rawData: '5',
								source: {
									order: 8,
									dataType: 'Text',
									searchFieldType: 'text',
									component: null,
									name: 'ValNumber3',
									area: 'GRA',
									field: 'NUMBER3',
									label: 'Number',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dataLength: 50
								}
							}
						}
					],
					readonly: true,
					componentName: 'q-chart',
					containerId: 'q-chart-container'
				}
			}, {
				$getResource: resId => resId
			}),

			columnChartData: new controlClass.TableSpecialRenderingControl({
				viewModes: [
					{
						type: 'chart'
					}
				],
				viewMode: {
					id: 'CHART',
					type: 'chart',
					subtype: 'genericgraph',
					label: 'Gráfico',
					visible: true,
					order: 2,
					mappingVariables: {
						xaxis: {
							allowsMultiple: false,
							sources: [
								'GRA.DT'
							]
						},
						yaxis: {
							allowsMultiple: true,
							sources: [
								'GRA.NUMBER2'
							]
						}
					},
					styleVariables: {
						valuesDecimals: {
							value: 0,
							isMapped: false
						},
						legendXPosition: {
							value: 0,
							isMapped: false
						},
						legendVerticalAlign: {
							value: 'bottom',
							isMapped: false
						},
						groupType: {
							value: 'join',
							isMapped: false
						},
						description: {
							value: '',
							isMapped: false
						},
						xaxisName: {
							value: 'XAxis',
							isMapped: false
						},
						stackingType: {
							value: 'undefined',
							isMapped: false
						},
						showPieLabel: {
							value: 'outside',
							isMapped: false
						},
						alignDescription: {
							value: 'left',
							isMapped: false
						},
						yaxisType: {
							value: 'linear',
							isMapped: false
						},
						enableHover: {
							value: true,
							isMapped: false
						},
						showBreaks: {
							value: false,
							isMapped: false
						},
						showLabels: {
							value: true,
							isMapped: false
						},
						showLegend: {
							value: true,
							isMapped: false
						},
						legendAlign: {
							value: 'center',
							isMapped: false
						},
						chartColorArray: {
							value: 'Highcharts default',
							isMapped: false
						},
						yaxisName: {
							value: 'YAxis',
							isMapped: false
						},
						inverted: {
							value: false,
							isMapped: false
						},
						invertColorArray: {
							value: false,
							isMapped: false
						},
						heightPx: {
							value: 400,
							isMapped: false
						},
						legendFloating: {
							value: false,
							isMapped: false
						},
						widthPercentage: {
							value: 100,
							isMapped: false
						},
						graphTitle: {
							value: '',
							isMapped: false
						},
						chartType: {
							value: 'column',
							isMapped: false
						},
						legendLayout: {
							value: 'horizontal',
							isMapped: false
						},
						legendYPosition: {
							value: 0,
							isMapped: false
						},
						showLastN: {
							value: -1,
							isMapped: false
						},
						pieInnerSizePercentage: {
							value: 0,
							isMapped: false
						},
						firstColor: {
							value: 'undefined',
							isMapped: false
						},
						zoomType: {
							value: 'x',
							isMapped: false
						},
						xaxisType: {
							value: 'linear',
							isMapped: false
						},
						lineMarker: {
							value: 'enabled',
							isMapped: false
						}
					},
					mappedValues: [
						{
							rowKey: '00000000-0000-0000-0000-000000000005',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-23T23:49:56.000Z',
								rawData: '2021-11-23T23:49:56.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 8.08,
									rawData: 8.08,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000007',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-24T17:55:00.000Z',
								rawData: '2021-11-24T17:55:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 3.45,
									rawData: 3.45,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000004',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-24T23:41:00.000Z',
								rawData: '2021-11-24T23:41:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 9,
									rawData: 9,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000006',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-27T23:50:00.000Z',
								rawData: '2021-11-27T23:50:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 6,
									rawData: 6,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000003',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-29T00:00:00.000Z',
								rawData: '2021-11-29T00:00:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 2,
									rawData: 2,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000001',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-30T00:00:00.000Z',
								rawData: '2021-11-30T00:00:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 4,
									rawData: 4,
									source: {
										order: 4,
										dataType: 'Currency',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber2',
										area: 'GRA',
										field: 'NUMBER2',
										label: 'Número 2',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 0,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric',
										currency: 'EUR',
										currencySymbol: '€'
									}
								}
							]
						}
					],
					readonly: true,
					componentName: 'q-chart',
					containerId: 'q-chart-container'
				}
			}, {
				$getResource: resId => resId
			}),

			barChartData: new controlClass.TableSpecialRenderingControl({
				viewModes: [
					{
						type: 'chart'
					}
				],
				viewMode: {
					id: 'CHART',
					type: 'chart',
					subtype: 'genericgraph',
					label: 'Gráfico',
					visible: true,
					order: 2,
					mappingVariables: {
						xaxis: {
							allowsMultiple: false,
							sources: [
								'GRA.DT'
							]
						},
						yaxis: {
							allowsMultiple: true,
							sources: [
								'GRA.NUMBER'
							]
						}
					},
					styleVariables: {
						groupType: {
							value: 'join',
							isMapped: false
						},
						showLegend: {
							value: true,
							isMapped: false
						},
						enableHover: {
							value: true,
							isMapped: false
						},
						firstColor: {
							value: 'undefined',
							isMapped: false
						},
						stackingType: {
							value: 'undefined',
							isMapped: false
						},
						showLastN: {
							value: -1,
							isMapped: false
						},
						yaxisType: {
							value: 'linear',
							isMapped: false
						},
						legendLayout: {
							value: 'horizontal',
							isMapped: false
						},
						showBreaks: {
							value: false,
							isMapped: false
						},
						lineMarker: {
							value: 'enabled',
							isMapped: false
						},
						chartType: {
							value: 'bar',
							isMapped: false
						},
						showLabels: {
							value: true,
							isMapped: false
						},
						pieInnerSizePercentage: {
							value: 0,
							isMapped: false
						},
						widthPercentage: {
							value: 100,
							isMapped: false
						},
						alignDescription: {
							value: 'left',
							isMapped: false
						},
						inverted: {
							value: false,
							isMapped: false
						},
						yaxisName: {
							value: 'YAxis',
							isMapped: false
						},
						valuesDecimals: {
							value: 0,
							isMapped: false
						},
						legendVerticalAlign: {
							value: 'bottom',
							isMapped: false
						},
						description: {
							value: '',
							isMapped: false
						},
						graphTitle: {
							value: '',
							isMapped: false
						},
						xaxisType: {
							value: 'linear',
							isMapped: false
						},
						heightPx: {
							value: 400,
							isMapped: false
						},
						legendXPosition: {
							value: 0,
							isMapped: false
						},
						legendYPosition: {
							value: 0,
							isMapped: false
						},
						chartColorArray: {
							value: 'Highcharts default',
							isMapped: false
						},
						showPieLabel: {
							value: 'outside',
							isMapped: false
						},
						legendFloating: {
							value: false,
							isMapped: false
						},
						legendAlign: {
							value: 'center',
							isMapped: false
						},
						invertColorArray: {
							value: false,
							isMapped: false
						},
						xaxisName: {
							value: 'XAxis',
							isMapped: false
						},
						zoomType: {
							value: 'x',
							isMapped: false
						}
					},
					mappedValues: [
						{
							rowKey: '00000000-0000-0000-0000-000000000005',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-23T23:49:56.000Z',
								rawData: '2021-11-23T23:49:56.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 7,
									rawData: 7,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000007',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-24T17:55:00.000Z',
								rawData: '2021-11-24T17:55:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 2,
									rawData: 2,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000004',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-24T23:41:00.000Z',
								rawData: '2021-11-24T23:41:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 8,
									rawData: 8,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000006',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-27T23:50:00.000Z',
								rawData: '2021-11-27T23:50:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 5,
									rawData: 5,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000003',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-29T00:00:00.000Z',
								rawData: '2021-11-29T00:00:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 1,
									rawData: 1,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						},
						{
							rowKey: '00000000-0000-0000-0000-000000000001',
							btnPermission: {
								editBtnDisabled: false,
								viewBtnDisabled: false,
								deleteBtnDisabled: false,
								insertBtnDisabled: false
							},
							xaxis: {
								value: '2021-11-30T00:00:00.000Z',
								rawData: '2021-11-30T00:00:00.000Z',
								source: {
									order: 1,
									dataType: 'Date',
									searchFieldType: 'date',
									component: null,
									name: 'ValDt',
									area: 'GRA',
									field: 'DT',
									label: 'Data',
									supportForm: null,
									params: null,
									cellAction: false,
									visibility: true,
									sortable: true,
									array: null,
									useDistinctValues: false,
									textColor: null,
									bgColor: null,
									initialSort: false,
									initialSortOrder: '',
									isDefaultSearch: false,
									pkColumn: null,
									dateTimeType: 'dateTimeSeconds'
								}
							},
							yaxis: [
								{
									value: 3,
									rawData: 3,
									source: {
										order: 2,
										dataType: 'Numeric',
										searchFieldType: 'num',
										component: null,
										name: 'ValNumber',
										area: 'GRA',
										field: 'NUMBER',
										label: 'Número',
										supportForm: null,
										params: null,
										cellAction: false,
										visibility: true,
										sortable: true,
										array: null,
										useDistinctValues: false,
										textColor: null,
										bgColor: null,
										initialSort: false,
										initialSortOrder: '',
										isDefaultSearch: false,
										pkColumn: null,
										maxDigits: 1,
										decimalPlaces: 2,
										showTotal: true,
										columnClasses: 'c-table__cell-numeric row-numeric',
										columnHeaderClasses: 'c-table__head-numeric'
									}
								}
							]
						}
					],
					readonly: true,
					componentName: 'q-chart',
					containerId: 'q-chart-container'
				}
			}, {
				$getResource: resId => resId
			})
		}
	}
}
