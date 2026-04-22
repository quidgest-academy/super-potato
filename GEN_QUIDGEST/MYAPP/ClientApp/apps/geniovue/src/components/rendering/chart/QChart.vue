<template>
	<div
		v-if="!hasMappedValues"
		class="no-records-container"
		ref="container">
		<slot name="empty-image"></slot>
		<slot name="empty-text"></slot>
	</div>
	<div
		v-else
		:id="controlId"
		:class="$attrs.class"
		ref="container">
		<chart
			v-if="ready"
			ref="chart"
			data-testid="chart"
			:options="chartOptions" />
	</div>
</template>

<script>
	import isEmpty from 'lodash-es/isEmpty'
	import has from 'lodash-es/has'

	import Highcharts from 'highcharts'
	import { Chart } from 'highcharts-vue'

	import highchartsMore from 'highcharts/highcharts-more'
	import exportingInit from 'highcharts/modules/exporting'
	import noDataToDisplay from 'highcharts/modules/no-data-to-display'
	import heatmap from 'highcharts/modules/heatmap'
	import offlineExporting from 'highcharts/modules/offline-exporting'
	import variablePie from 'highcharts/modules/variable-pie'

	import { unref } from 'vue'

	exportingInit(Highcharts)
	offlineExporting(Highcharts)
	variablePie(Highcharts)
	highchartsMore(Highcharts)
	heatmap(Highcharts)
	noDataToDisplay(Highcharts)

	// Workaround for https://github.com/highcharts/highcharts/issues/16149
	Highcharts.AST.allowedReferences.push('data:')

	/**
	 * @example ../../../../docs/schemas/QChart.schema.md
	 */
	export default {
		name: 'QChart',

		emits: [
			'row-action'
		],

		components: {
			Chart
		},

		inheritAttrs: false,

		props: {
			/**
			 * The unique identifier for the container.
			 */
			containerId: String,

			/**
			 * The data from which we will plot the chart.
			 */
			mappedValues: {
				type: Array,
				default: () => []
			},

			/**
			 * The defined style variables.
			 */
			styleVariables: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The selected configuration of the chart.
			 */
			config: {
				type: Object,
				required: true
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.containerId || `q-chart-${this._.uid}`,

				/**
				 * Holds the dataset datatypes
				 */
				vDataType: [],

				/**
				 * Holds one row and the column of the date dataset,
				 * to be displayed correctly in the chart axis
				 */
				labelRow: {},

				labelColumn: {
					x: {},
					y: {},
					z: {},
					name: {},
				},

				dateFormats: {
					date: 'dd/MM/yyyy',
					dateTime: 'dd/MM/yyyy HH:mm',
					dateTimeSeconds: 'dd/MM/yyyy HH:mm:ss',
					hours: 'HH:mm',
					use12Hour: false,
				},

				numberFormat: {
					decimalSeparator: ',',
					groupSeparator: '.',
				},

				/**
				 * Holds the final configuration
				 * that will be passed to HighCharts
				 */
				chartOptions: {},

				series: [],

				ready: false
			}
		},

		mounted()
		{
			this.handleData()

			// In unit tests, the ResizeObserver won't be available.
			if (typeof ResizeObserver !== 'undefined')
			{
				this.$nextTick().then(() => {
					const resizeObserver = new ResizeObserver(() => {
						this.$refs.chart?.chart?.reflow()
					})

					resizeObserver.observe(this.$refs.container)
				})
			}
		},

		computed: {
			hasMappedValues()
			{
				return Array.isArray(this.mappedValues) && this.mappedValues.length > 0
			}
		},

		methods: {
			/**
			 * Checks if the value is empty.
			 */
			isEmpty(value)
			{
				return (
					value === undefined
					|| typeof value === 'number' && isNaN(value)
					|| (typeof value === 'object' || typeof value === 'string') && isEmpty(value)
				)
			},

			/**
			 * Main handler to format data
			 */
			handleData()
			{
				this.ready = false

				this.prepareSeries()

				// Copy and format information that is common to all chart types
				this.addGenericOptions()

				this.addAdvancedOptions()

				// Parse and generate chart readable data
				// Depending on the data and the chart type selected
				this.formatSeriesData()

				// Add custom styles to chart
				this.addStyle()

				this.ready = true
			},

			getDateAsString(dateVal)
			{
				const dateObj = new Date(dateVal)
				const date = dateObj.toISOString().split('T')[0]
				const time = dateObj.toISOString().split('T')[1].split('.')[0]

				return `${date} ${time}`
			},

			prepareSeries()
			{
				this.series = []

				if (this.hasMappedValues)
				{
					const xaxisSample = this.mappedValues[0].xaxis
					const yaxisSample = this.mappedValues[0].yaxis

					const seriesSelector = this.mappedValues.filter((row) => 'serieSelector' in row)

					if (seriesSelector.length)
					{
						// serie-selector has been defined
						// this means that only one mapping of the yAxis is expected
						// and the values will determine the series
						// => each distinct value is a series
						seriesSelector.forEach((el) => {
							// Series selector cannot be null
							if (!el.serieSelector.rawData)
								return

							const seriesExists = this.series.filter(
								(s) => s.id === el.serieSelector.rawData
							).length

							if (!seriesExists)
							{
								this.series.push({
									id: el.serieSelector.rawData,
									name: el.serieSelector.value,
									dataset: {
										x: xaxisSample.source.label,
										y: el.serieSelector.source.label
									}
								})
							}
						})
					}
					else
					{
						// The # of series is given by the number of mapped 'yaxis' variables
						if (yaxisSample)
						{
							yaxisSample.forEach((el) => {
								this.series.push({
									id: el.source.field,
									name: el.source.label,
									dataset: {
										x: xaxisSample.source.label,
										y: el.source.label
									}
								})
							})
						}
					}

					// Compute metadata
					const sample = this.mappedValues[0]

					const xType = sample.xaxis?.source?.dataType
					const zType = sample.zaxis?.source?.dataType

					this.series.forEach((_, index) => {
						if (this.mappedValues.length)
						{
							this.vDataType.push({
								xType: xType,
								yType: sample.yaxis[index]?.source?.dataType,
								zType: zType
							})
						}
					})
				}
			},

			getTooltipFormatter(context)
			{
				if (this.chartOptions.chart.type === 'heatmap')
					return `<b>${context.point.series.name}:</b> ${context.point.value}`

				let xValue = ''
				let yValue = ''
				let zValue = ''
				let keyValue = ''
				let result = ''
				let defaultFormat = true
				let groupValue = 0

				if (!this.isEmpty(context.key))
				{
					if (this.hasDateX())
						keyValue = this.getDateAsString(context.key)
					else
						keyValue = context.key
				}
				else
					keyValue = ''

				if (!this.isEmpty(context.x))
				{
					if (this.hasDateX())
						xValue = this.getDateAsString(context.x)
					else
						xValue = context.x
				}
				else
					xValue = ''

				if (!this.isEmpty(context.y))
				{
					if (this.hasDateY())
						yValue = this.getDateAsString(context.y)
					else
						yValue = context.y
				}
				else
					yValue = ''

				if (!this.isEmpty(context.point.z))
				{
					if (this.hasDateZ())
						zValue = this.getDateAsString(context.point.z)
					else
						zValue = context.point.z
				}
				else
					zValue = ''

				if (!this.isEmpty(context.percentage))
				{
					context.percentage = has(
						this.config,
						'labels.percentageDecimalPlaces'
					)
						? context.percentage.toFixed(
							this.config.labels.percentageDecimalPlaces
						)
						: context.percentage.toFixed(1)
				}

				if (has(this.config, 'labels.point.header'))
				{
					result += this.config.labels.point.header.replace('{series.name}', context.series.name)
					result = result.replace('{point.percentage}', context.percentage)
					result = result.replace('{point.percentage}', context.percentage)
					result = result.replace('{point.key}', keyValue)
					result = result.replace('{series.color}', context.series.color)
					result = result + '<br>'
					defaultFormat = false
				}

				if (has(this.config, 'labels.point.body'))
				{
					result += this.config.labels.point.body.replace('{point.name}', keyValue)

					// Split packed bubble
					if (this.isEmpty(context.x) && this.isEmpty(context.y) && this.isEmpty(context.key))
					{
						context.series.yData.forEach((value) => {
							groupValue += value
						})

						groupValue = this.labelRow.value
						result = result.replace('{point.value}', groupValue)
					}
					else
					{
						result = result.replace('{point.value}', yValue)
						result = result.replace('{point.value}', yValue)
					}

					result = result.replace('{series.name}', context.series.name)
					result = result.replace('{point.percentage}', context.percentage)
					result = result.replace('{point.percentage}', context.percentage)
					result = result.replace('{point.x}', xValue)
					result = result.replace('{point.y}', yValue)
					result = result.replace('{point.z}', zValue)
					result = result.replace('{point.color}', context.point.color)
					result = result.replace('{series.color}', context.series.color)
					result = result.replace('{point.group}', context.series.userOptions.name)
					defaultFormat = false
				}

				return defaultFormat ? `<b>${keyValue}</b><br />${context.series.name}: <b>${yValue}</b>` : result
			},

			getPieFormatter(context)
			{
				let name = context.point.name
				const percentage = context.point.percentage.toFixed(1)

				if (this.hasDateX())
					name = this.getDateAsString(name)

				let result = ''

				if (has(this.config, 'labels.data.format'))
				{
					result = this.config.labels.data.format.replace('{point.name}', name)
					result = result.replace('{point.percentage}', percentage)
				}
				else
					result = name

				return result
			},

			/**
			 * Adds information that is common to all chart types
			 */
			addGenericOptions()
			{
				const vm = this
				let type = has(this.styleVariables, 'chartType') ? this.styleVariables.chartType.value : 'line'

				if (type === 'semi-pie')
					type = 'pie'

				this.chartOptions = {
					chart: {
						type: type,
						zoomType: has(this.styleVariables, 'zoomType')
							? this.styleVariables.zoomType.value
							: 'x',
						height: has(this.styleVariables, 'heightPx')
							? this.styleVariables.heightPx.value
							: '400',
						events: {
							load: function()
							{
								let toShow = this.series.length
								const showLastN = has(vm.styleVariables, 'showLastN')
									? parseInt(vm.styleVariables.showLastN.value)
									: -1

								if (showLastN !== -1)
									toShow = showLastN

								for (let i = 0; i < this.series.length - toShow; i++)
									this.series[i].setVisible(false, false)

								if (showLastN !== -1 && !vm.isEmpty(this.chart))
									this.chart.redraw()
							},
						},
						inverted: has(this.styleVariables, 'inverted')
							? this.styleVariables.inverted.value
							: false,
					},

					caption: {
						text: has(this.styleVariables, 'description')
							? this.styleVariables.description.value
							: '',
						align: has(this.styleVariables, 'alignDescription')
							? this.styleVariables.alignDescription.value
							: 'left',
					},

					title: {
						text: has(this.styleVariables, 'graphTitle')
							? this.styleVariables.graphTitle.value
							: '',
					},

					subtitle: {
						text: has(this.styleVariables, 'subtitle')
							? this.styleVariables.subtitle.value
							: '',
						align: has(this.styleVariables, 'subtitleAlignment')
							? this.styleVariables.subtitleAlignment.value
							: 'center',
					},

					legend: {
						layout: has(this.styleVariables, 'legendLayout')
							? this.styleVariables.legendLayout.value
							: 'horizontal',
						align: has(this.styleVariables, 'legendAlign')
							? this.styleVariables.legendAlign.value
							: 'center',
						verticalAlign: has(this.styleVariables, 'legendVerticalAlign')
							? this.styleVariables.legendVerticalAlign.value
							: 'bottom',
						floating: has(this.styleVariables, 'floating')
							? this.styleVariables.floating.value
							: false,
						enabled: has(this.styleVariables, 'showLegend')
							? this.styleVariables.showLegend.value
							: true,
					},

					tooltip: {
						enabled: has(this.styleVariables, 'enableHover')
							? this.styleVariables.enableHover.value
							: true,
						formatter: function()
						{
							return vm.getTooltipFormatter(this)
						}
					},

					xAxis: {
						title: {
							text: has(this.styleVariables, 'xaxisName')
								? this.styleVariables.xaxisName.value
								: '',
						},
						labels: {
							formatter: function()
							{
								let value = this.value

								if (value && vm.hasDateX())
									value = vm.getDateAsString(value)

								return value
							}
						}
					},

					yAxis: {
						title: {
							text: has(this.styleVariables, 'yaxisName')
								? this.styleVariables.yaxisName.value
								: '',
						},
						labels: {
							rotation: has(this.config, 'axis.yAxis.labels.rotation')
								? this.config.axis.yAxis.labels.rotation
								: 0,
							formatter: function()
							{
								let value = this.value

								if (vm.hasDateY())
									value = vm.getDateAsString(value)

								return value
							}
						},

						min: has(this.config, 'range.min')
							? this.config.range.min
							: null,

						max: has(this.config, 'range.max')
							? this.config.range.max
							: null,

						tickInterval: has(this.config, 'range.tickInterval')
							? this.config.range.tickInterval
							: null,
					},
					lang: {
						noData: 'No data to display',
					},
					credits: {
						enabled: false,
					},
				}

				this.chartOptions.plotOptions = {
					series: {},
				}

				//Since the general tooltip appears to not be working
				//We add point labels via the plotOptions of each chart type
				this.chartOptions.plotOptions.series.tooltip = {
					...(has(this.config, 'labels.point.header') && {
						headerFormat: this.config.labels.point.header,
					}),
					...(has(this.config, 'labels.point.body') && {
						pointFormat: this.config.labels.point.body,
					}),
				}

				//To enable clicking overlapped area series
				this.chartOptions.plotOptions.series.trackByArea = true

				this.chartOptions.plotOptions.series.point = {}

				//Add event listener to emit points that are clicked
				this.chartOptions.plotOptions.series.point.events = {
					click: (event) => {
						if (this.chartOptions.chart.type === 'area')
						{
							const group = this.series.group
							const x = event.chartX - this.plotX - group.translateX
							const y = event.chartY - this.plotY - group.translateY
							const d = x * x + y * y

							const rPlus = this.graphic.states.hover.radiusPlus
							const r = this.graphic.radius + (rPlus || 0)

							if (d <= r * r)
								this.emitPoint(event)
						}
						else
							this.emitPoint(event)
					},
				}

				this.chartOptions.plotOptions.series.colorByPoint = this.series.length === 1

				this.chartOptions.plotOptions.series.marker = {
					enabled: has(this.styleVariables, 'lineMarker')
						? this.styleVariables.lineMarker.value === 'enabled'
						: true,
				}

				this.chartOptions.plotOptions.series.stacking =
					has(this.styleVariables, 'stackingType') && this.styleVariables.stackingType.value !== 'undefined'
						? this.styleVariables.stackingType.value
						: ''

				const showLabels = has(this.styleVariables, 'showLabels')
					? this.styleVariables.showLabels.value
					: true

				if (showLabels)
				{
					this.chartOptions.plotOptions.series.dataLabels = {
						enabled: true,
						crop: false,
						overflow: 'none',
					}
				}

				if (has(this.config, 'dateFormats'))
				{
					this.dateFormats.date = has(this.config.dateFormats, 'date')
						? this.config.dateFormats.date
						: 'dd/MM/yyyy'
					this.dateFormats.dateTime = has(
						this.config.dateFormats,
						'dateTime'
					)
						? this.config.dateFormats.dateTime
						: 'dd/MM/yyyy HH:mm'
					this.dateFormats.dateTimeSeconds = has(
						this.config.dateFormats,
						'dateTimeSeconds'
					)
						? this.config.dateFormats.dateTimeSeconds
						: 'dd/MM/yyyy HH:mm:ss'
					this.dateFormats.hours = has(this.config.dateFormats, 'hours')
						? this.config.dateFormats.hours
						: 'HH:mm'
					this.dateFormats.use12Hour = has(
						this.config.dateFormats,
						'use12Hour'
					)
						? this.config.dateFormats.use12Hour
						: false
				}

				if (has(this.config, 'numberFormat'))
				{
					this.numberFormat.decimalSeparator = has(
						this.config.numberFormat,
						'decimalSeparator'
					)
						? this.config.numberFormat.decimalSeparator
						: ','
					this.numberFormat.groupSeparator = has(
						this.config.numberFormat,
						'groupSeparator'
					)
						? this.config.numberFormat.groupSeparator
						: '.'
				}
			},

			/**
			 * This is where chart granularity will be applied
			 */
			addAdvancedOptions()
			{
				const vm = this
				const type = this.styleVariables.chartType ? this.styleVariables.chartType.value : ''
				const hcType = this.chartOptions.chart.type
				this.chartOptions.plotOptions[hcType] = {}

				switch (type)
				{
					case 'column':
					case 'bar':
						this.chartOptions.plotOptions[hcType] = {
							...(has(this.config, 'advanced.stacking') && {
								stacking: this.config.advanced.stacking,
							}),
						}
						break
					case 'pie':
					case 'semi-pie':
					case 'variablepie':
						this.chartOptions.plotOptions[hcType] = {
							startAngle: type === 'semi-pie' ? -90 : 0,
							endAngle: type === 'semi-pie' ? 90 : 360,
							center: type === 'semi-pie' ? ['50%', '75%'] : ['50%', '50%'],
							size: type === 'semi-pie' ? '110%' : '100%',
							dataLabels: {
								formatter: function()
								{
									return vm.getPieFormatter(this).value
								}
							}
						}
						break
					case 'bubble':
						this.chartOptions.plotOptions[hcType] = {
							dataLabels: {
								enabled: true,
								format: '{point.name}',
							},
						}
						break
					case 'packedbubble':
						this.chartOptions.plotOptions[hcType] = {
							minSize: '20%',
							maxSize: '100%',
							zMin: 0,
							zMax: 1000,
							layoutAlgorithm: {
								gravitationalConstant: 0.01,
								splitSeries: has(
									this.config,
									'advanced.splitSeries'
								)
									? this.config.advanced.splitSeries
									: false,
								dragBetweenSeries: true,
								parentNodeLimit: true,
							},
							dataLabels: {
								enabled: true,
								format: '{point.name}',
							},
						}
						break
					case 'heatmap':
						this.chartOptions.plotOptions.series.colorByPoint = false
						this.chartOptions.colorAxis = {
							min: 0,
							minColor: '#FFFFFF',
							maxColor: Highcharts.getOptions().colors[0],
						}
						this.chartOptions.legend = {
							align: 'right',
							layout: 'vertical',
							verticalAlign: 'top',
							y: 30,
							symbolHeight: 280,
						}
						break
					default:
						break
				}
			},

			/**
			 * Interprets the wanted series information and generates the data in the correct format
			 */
			formatSeriesData()
			{
				switch (this.chartOptions.chart.type)
				{
					case 'line':
					case 'spline':
					case 'area':
					case 'areaspline':
					case 'column':
					case 'bar':
						this.buildDefaultData()
						break
					case 'pie':
					case 'semi-pie':
						this.buildPieData('pie')
						break
					case 'variablepie':
						this.buildPieData(this.chartOptions.chart.type)
						break
					case 'bubble':
						this.buildBubbleData()
						break
					case 'packedbubble':
						this.buildPackedBubbleData()
						break
					case 'heatmap':
						this.buildHeatmapData()
					default:
						break
				}
			},

			// TODO: Change this! The style shouldn't come from the config.
			addStyle()
			{
				if (!has(this.config, 'style'))
					return

				// Add general styles that are common to all chart types
				this.chartOptions.chart.backgroundColor = has(
					this.config,
					'style.backgroundColor'
				)
					? this.config.style.backgroundColor
					: '#ffffff'

				this.chartOptions.chart.style = {
					...(has(this.config, 'style.fontFamily') && {
						fontFamily: this.config.style.fontFamily
					})
				}

				//title
				this.chartOptions.title.style = {
					...(has(this.config, 'style.title.fontWeight') && {
						fontWeight: this.config.style.title.fontWeight
					}),
					...(has(this.config, 'style.title.fontSize') && {
						fontSize: this.config.style.title.fontSize
					}),
					...(has(this.config, 'style.fontColor') && {
						color: this.config.style.fontColor
					})
				}

				//subtitle
				this.chartOptions.subtitle.style = {
					...(has(this.config, 'style.subtitle.fontWeight') && {
						fontWeight: this.config.style.subtitle.fontWeight
					}),
					...(has(this.config, 'style.subtitle.fontSize') && {
						fontSize: this.config.style.subtitle.fontSize
					}),
					...(has(this.config, 'style.fontColor') && {
						color: this.config.style.fontColor
					})
				}

				//legend
				this.chartOptions.legend.itemStyle = {
					...(has(this.config, 'style.legend.fontWeight') && {
						fontWeight: this.config.style.legend.fontWeight
					}),
					...(has(this.config, 'style.legend.fontSize') && {
						fontSize: this.config.style.legend.fontSize
					}),
					...(has(this.config, 'style.fontColor') && {
						color: this.config.style.fontColor
					})
				}

				//axis based style
				if (has(this.config, 'axis'))
				{
					//xAxis
					this.chartOptions.xAxis.title.style = {
						...(has(
							this.config,
							'style.axis.xAxis.title.fontWeight'
						) && {
							fontWeight:
								this.config.style.axis.xAxis.title.fontWeight
						}),
						...(has(this.config, 'style.axis.xAxis.title.fontSize') && {
							fontSize: this.config.style.axis.xAxis.title.fontSize
						}),
						...(has(this.config, 'style.fontColor') && {
							color: this.config.style.fontColor
						})
					}

					this.chartOptions.xAxis.labels.style = {
						...(has(
							this.config,
							'style.axis.xAxis.labels.fontWeight'
						) && {
							fontWeight:
								this.config.style.axis.xAxis.labels.fontWeight
						}),
						...(has(
							this.config,
							'style.axis.xAxis.labels.fontSize'
						) && {
							fontSize: this.config.style.axis.xAxis.labels.fontSize
						}),
						...(has(this.config, 'style.fontColor') && {
							color: this.config.style.fontColor
						})
					}

					//yAxis
					this.chartOptions.yAxis.title.style = {
						...(has(
							this.config,
							'style.axis.yAxis.title.fontWeight'
						) && {
							fontWeight:
								this.config.style.axis.yAxis.title.fontWeight
						}),
						...(has(this.config, 'style.axis.yAxis.title.fontSize') && {
							fontSize: this.config.style.axis.yAxis.title.fontSize
						}),
						...(has(this.config, 'style.fontColor') && {
							color: this.config.style.fontColor
						})
					}

					this.chartOptions.yAxis.labels.style = {
						...(has(
							this.config,
							'style.axis.yAxis.labels.fontWeight'
						) && {
							fontWeight:
								this.config.style.axis.yAxis.labels.fontWeight
						}),
						...(has(
							this.config,
							'style.axis.yAxis.labels.fontSize'
						) && {
							fontSize: this.config.style.axis.yAxis.labels.fontSize
						}),
						...(has(this.config, 'style.fontColor') && {
							color: this.config.style.fontColor
						})
					}
				}

				//dropdown just uses the default values
				this.chartOptions.navigation = {
					buttonOptions: {
						//match with overall background
						theme: {
							...(has(this.config, 'style.backgroundColor') && {
								fill: this.config.style.backgroundColor
							})
						},

						//bar color
						...(has(this.config, 'style.fontColor') && {
							symbolStroke: this.config.style.fontColor
						})
					},

					//dropdown menu styling
					menuStyle: {
						...(has(this.config, 'style.backgroundColor') && {
							background: this.config.style.backgroundColor
						}),
						border: '1px solid black',
						padding: '5px 0'
					},

					menuItemStyle: {
						...(has(this.config, 'style.fontColor') && {
							color: this.config.style.fontColor
						}),
						...(has(this.config, 'style.fontFamily') && {
							fontFamily: this.config.style.fontFamily
						})
					}
				}

				//adding width and height to chart
				if (has(this.config, 'style.dimensions.width'))
					this.chartOptions.chart.width = this.config.style.dimensions.width

				if (has(this.config, 'style.dimensions.height'))
					this.chartOptions.chart.height = this.config.style.dimensions.height
			},

			buildDefaultData()
			{
				const categories = []
				this.chartOptions.series = []
				const filteredRows = this.getFilteredRows()

				this.labelRow = filteredRows[0]
				// use style/config

				this.labelColumn.x = has(this.styleVariables, 'xaxisName')
					? this.styleVariables.xaxisName.value
					: ''
				this.labelColumn.y = has(this.styleVariables, 'yaxisName')
					? this.styleVariables.yaxisName.value
					: ''

				if (filteredRows.length === 0)
					return

				this.series.forEach((series) => {
					const data = []

					filteredRows.forEach((row) => {
						let x = row.xaxis?.rawData
						if (!x)
							return

						// in case the category value comes from an array, we should convert the value to the description
						if (row.xaxis.source.dataType === 'Array')
						{
							const foundElement = row.xaxis.source.array.find(element => element.key === x)
							if (foundElement)
								x = unref(foundElement.value)
						}

						if ('serieSelector' in row)
						{
							// only one mapping of y is expected
							// add only if serieSelector matches current serie
							if (row.serieSelector.rawData === series.id)
							{
								const y = row.yaxis[0]

								data.push({
									y: y.rawData,
									...(has(row, 'serieColor') && {
										color: row.serieColor.value,
									}),
								})

								categories.push(x)
							}
						}
						else
						{
							const y = row.yaxis.filter(
								(y) => y.source.field === series.id
							)[0]

							categories.push(x)

							data.push({
								y: y.rawData,
								...(has(row, 'serieColor') && {
									color: row.serieColor.value,
								}),
							})
						}
					})

					this.chartOptions.series.push({
						name: series.name,
						data: data,
						...(has(series, 'seriesColor') && {
							color: series.seriesColor,
						}), //check if a series color was given
					})
				})

				this.chartOptions.xAxis.categories = categories
			},

			buildPieData(type)
			{
				if (this.series.length === 0)
				{
					this.chartOptions.series = []
					return
				}
				this.chartOptions.series = []

				const data = []

				const filteredRows = this.getFilteredRows()
				if (filteredRows.length === 0)
					return

				this.labelRow = filteredRows[0]
				this.labelColumn.x = has(this.styleVariables, 'xaxisName')
					? this.styleVariables.xaxisName.value
					: ''
				this.labelColumn.y = has(this.styleVariables, 'yaxisName')
					? this.styleVariables.yaxisName.value
					: ''

				let seriesName = ''
				let partialTotalY = 0
				let partialTotalZ = 0
				let isFirst = true

				filteredRows.forEach((row) => {
					this.series.forEach((series) => {
						if (isFirst)
						{
							// To format multiseries name
							if (seriesName !== '')
								seriesName = seriesName + ' + ' + series.name
							else
								seriesName = series.name
						}

						partialTotalY += row.yaxis.filter(
							(y) => y.source.field === series.id
						)[0].rawData

						if (type === 'variablepie')
						{
							// Only variablepie should have a 'z'
							partialTotalZ += row.zaxis
						}
					})

					if (this.hasDateX(0))
					{
						const date = row.xaxis.value.match(
							/(\d{4})-(\d{2})-(\d{2}) (\d{2}):(\d{2}):(\d{2})/
						)

						if (type === 'variablepie')
						{
							data.push({
								name: Date.UTC(
									date[1],
									date[2] - 1,
									date[3],
									date[4],
									date[5],
									date[6]
								),
								y: partialTotalY,
								z: partialTotalZ,
							})
						}
						else
						{
							data.push({
								name: Date.UTC(
									date[1],
									date[2] - 1,
									date[3],
									date[4],
									date[5],
									date[6]
								),
								y: partialTotalY,
							})
						}
					}
					else
					{
						if (type === 'variablepie')
						{
							data.push({
								name: row.xaxis.value,
								y: partialTotalY,
								z: partialTotalZ,
							})
						}
						else
						{
							data.push({
								name: row.xaxis.value,
								y: partialTotalY,
							})
						}
					}

					partialTotalY = 0
					partialTotalZ = 0

					isFirst = false
				})

				this.chartOptions.series.push({
					name: seriesName,
					colorByPoint: true,
					data: data,
					zMin: 0,
				})
			},

			buildBubbleData()
			{
				if (this.series.length === 0)
				{
					this.chartOptions.series = []
					return
				}

				this.chartOptions.series = []
				const data = []
				const series = this.series[0]
				const filteredRows = this.getFilteredRows()

				if (filteredRows.length === 0)
					return

				this.labelRow = filteredRows[0]
				this.labelColumn.x = has(this.styleVariables, 'xaxisName')
					? this.styleVariables.xaxisName.value
					: ''
				this.labelColumn.y = has(this.styleVariables, 'yaxisName')
					? this.styleVariables.yaxisName.value
					: ''
				this.labelColumn.z = has(this.styleVariables, 'zaxisName')
					? this.styleVariables.zaxisName.value
					: ''

				filteredRows.forEach((row) => {
					const x = row.xaxis?.rawData
					const y = row.yaxis[0]?.rawData
					const z = row.zaxis?.rawData
					const name = row.pointName?.value

					//in case we have point specific colors
					if (has(series, 'dataset.color'))
					{
						const color = row.Fields[series.dataset.color]

						data.push({
							x: x,
							y: y,
							z: z,
							name: name,
							color: color,
						})
					}
					else
					{
						data.push({
							x: x,
							y: y,
							z: z,
							name: name,
						})
					}
				})

				this.chartOptions.series.push({
					name: series.name,
					data: data,
				})
			},

			buildPackedBubbleData()
			{
				if (this.series.length === 0)
				{
					this.chartOptions.series = []
					return
				}

				this.chartOptions.series = []
				const series = this.series[0]
				const filteredRows = this.getFilteredRows()
				const finalSeries = []

				this.labelRow = filteredRows[0]

				if (filteredRows.length === 0)
					return

				this.labelColumn.x = has(this.styleVariables, 'xaxisName')
					? this.styleVariables.xaxisName.value
					: ''
				this.labelColumn.y = has(this.styleVariables, 'yaxisName')
					? this.styleVariables.yaxisName.value
					: ''

				const groupBySet = new Set()

				filteredRows.forEach((row) => {
					groupBySet.add(row.Fields[series.groupBy])
				})

				const groupBy = Array.from(groupBySet)

				for (const i in groupBy)
				{
					finalSeries[i] = {
						name: groupBy[i],
						data: [],
					}
				}

				filteredRows.forEach((row) => {
					const gby = row.Fields[series.groupBy]
					const name = row.Fields[series.dataset.x]
					const value = row.Fields[series.dataset.y]
					const index = groupBy.findIndex((el) => el === gby)

					finalSeries[index].data.push({
						name: name,
						value: value,
					})
				})

				this.chartOptions.series = finalSeries
			},

			buildHeatmapData()
			{
				const categories = []
				this.chartOptions.series = []
				const filteredRows = this.getFilteredRows()

				this.labelRow = filteredRows[0]

				this.labelColumn.x = has(this.styleVariables, 'xaxisName')
					? this.styleVariables.xaxisName.value
					: ''
				this.labelColumn.y = has(this.styleVariables, 'yaxisName')
					? this.styleVariables.yaxisName.value
					: ''

				if (filteredRows.length === 0) return

				const yCategories = []
				this.series.forEach((series) => {
					const data = []

					filteredRows.forEach((row) => {
						const x = row.xaxis?.rawData
						const y = row.yaxis.filter(
							(y) => y.source.field === series.id
						)[0]

						if (!x || !y)
							return

						categories.push(x)

						if (!yCategories.includes(y.value))
							yCategories.push(y.value)

						data.push([
							categories.indexOf(x),
							yCategories.indexOf(y.value),
							row.zaxis?.rawData,
						])
					})

					const name = this.labelRow.zaxis?.source.label

					this.chartOptions.series.push({
						name: name,
						data: data,
						borderWidth: 1,
					})
				})

				this.chartOptions.xAxis.categories = categories
				this.chartOptions.yAxis.categories = yCategories
			},

			/**
			 *  We have to apply the range filters to all series datasets
			 *  @returns Filtered list of rows
			 */
			getFilteredRows()
			{
				const yDatasets = []

				this.series.forEach((series) => {
					yDatasets.push(series.dataset.y)
				})

				const filteredRows = []

				if (this.hasMappedValues)
				{
					this.mappedValues.forEach((row) => {
						if (row.yaxis.every((yaxis) => this.pointWithinRange(yaxis.rawData)))
							filteredRows.push(row)
					})
				}

				return filteredRows
			},

			/**
			 * If a range is given checks if a point is valid
			 * @returns True if the point is within range
			 */
			pointWithinRange(point)
			{
				const conditionsArray = [
					has(this.config, 'range.min')
						? this.config.range.min < point
						: true,
					has(this.config, 'range.max')
						? this.config.range.max > point
						: true,
					has(this.config, 'range.min') && has(this.config, 'range.max')
						? this.config.range.min < point && this.config.range.max > point
						: true
				]

				return !conditionsArray.includes(false)
			},

			hasDateX()
			{
				return this.vDataType[0].xType?.includes('Date')
			},

			hasDateY()
			{
				return this.vDataType[0].yType?.includes('Date')
			},

			hasDateZ()
			{
				return this.vDataType[0].zType?.includes('Date')
			},

			/**
			 * Emits the clicked point key to allow parent to perfom an action
			 * @param event {Object}
			 */
			emitPoint(event)
			{
				let primaryKey = ''

				//different chart types store point information in different places
				switch (this.chartOptions.chart.type)
				{
					case 'pie':
					case 'variablepie':
					case 'packedbubble':
						primaryKey = this.getKeyByProperty(event.point.options.name)
						break
					default:
						primaryKey = this.getKeyByProperty(event.point.category)
						break
				}

				if (primaryKey)
				{
					this.$emit('row-action', {
						rowKey: primaryKey,
					})
				}
			},

			/**
			 * Given the x property returns the row key that has that value
			 * @param xValue {String}
			 */
			getKeyByProperty(xValue)
			{
				// We assume the xAxis only has unique values
				const row = this.mappedValues.filter((row) => {
					if (this.vDataType[0].xType.includes('Date'))
					{
						const dateStr = this.getDateAsString(xValue)
						return row.xaxis.value === dateStr
					}

					return row.xaxis.value === xValue
				})

				if (row.length)
					return row[0].rowKey
				return null
			},

			getPointCategoryName(point, dimension)
			{
				const series = point.series,
					isY = dimension === 'y',
					axis = series[isY ? 'yAxis' : 'xAxis']
				return axis.categories[point[isY ? 'y' : 'x']]
			}
		},

		watch: {
			mappedValues: {
				handler()
				{
					this.handleData()
				},
				deep: true
			}
		}
	}
</script>
