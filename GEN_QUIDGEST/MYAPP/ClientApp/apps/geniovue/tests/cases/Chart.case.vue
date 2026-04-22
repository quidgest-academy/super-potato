<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<form class="form-horizontal">
			<fieldset>
				<div class="form-flow">
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<q-chart
								:container-id="`${lineChartData.viewMode.containerId}_1`"
								:mapped-values="lineChartData.viewMode.mappedValues"
								:style-variables="lineChartData.viewMode.styleVariables"
								:config="styledTableConfig"
								@row-action="displayEmit" />
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<q-chart
								:container-id="`${lineChartData.viewMode.containerId}_2`"
								:mapped-values="lineChartData.viewMode.mappedValues"
								:style-variables="lineChartData.viewMode.styleVariables"
								:config="tableConfig"
								@row-action="displayEmit" />
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<q-chart
								:container-id="`${areaChartData.viewMode.containerId}_3`"
								:mapped-values="areaChartData.viewMode.mappedValues"
								:style-variables="areaChartData.viewMode.styleVariables"
								:config="tableConfig"
								@row-action="displayEmit" />
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<q-chart
								:container-id="`${splineChartData.viewMode.containerId}_4`"
								:mapped-values="splineChartData.viewMode.mappedValues"
								:style-variables="splineChartData.viewMode.styleVariables"
								:config="tableConfig"
								@row-action="displayEmit" />
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<q-chart
								:container-id="`${areaSplineChartData.viewMode.containerId}_5`"
								:mapped-values="areaSplineChartData.viewMode.mappedValues"
								:style-variables="areaSplineChartData.viewMode.styleVariables"
								:config="tableConfig"
								@row-action="displayEmit" />
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<q-chart
								:container-id="`${pieChartData.viewMode.containerId}_6`"
								:mapped-values="pieChartData.viewMode.mappedValues"
								:style-variables="pieChartData.viewMode.styleVariables"
								:config="tableConfig"
								@row-action="displayEmit" />
						</q-control-wrapper>
					</q-row-container>


					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<q-chart
								:container-id="`${semiPieChartData.viewMode.containerId}_7`"
								:mapped-values="semiPieChartData.viewMode.mappedValues"
								:style-variables="semiPieChartData.viewMode.styleVariables"
								:config="tableConfig"
								@row-action="displayEmit" />
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<q-chart
								:container-id="`${bubbleChartData.viewMode.containerId}_8`"
								:mapped-values="bubbleChartData.viewMode.mappedValues"
								:style-variables="bubbleChartData.viewMode.styleVariables"
								:config="tableConfig"
								@row-action="displayEmit" />
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<q-chart
								:container-id="`${columnChartData.viewMode.containerId}_9`"
								:mapped-values="columnChartData.viewMode.mappedValues"
								:style-variables="columnChartData.viewMode.styleVariables"
								:config="tableConfig"
								@row-action="displayEmit">
							</q-chart>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<q-chart
								:container-id="`${barChartData.viewMode.containerId}_10`"
								:mapped-values="barChartData.viewMode.mappedValues"
								:style-variables="barChartData.viewMode.styleVariables"
								:config="tableConfig"
								@row-action="displayEmit" />
						</q-control-wrapper>
					</q-row-container>
				</div>
			</fieldset>
		</form>
	</div>
</template>

<script>
	import QChart from '@/components/rendering/chart/QChart.vue'

	import fakeData from './Chart.mock.js'

	export default {
		name: 'QChartContainer',

		docsfile: './docs/rendering/chart/QChart.md',

		components: {
			QChart
		},

		inheritAttrs: false,

		expose: [],

		data()
		{
			return fakeData.simpleUsage()
		},

		computed: {
			// TODO: Remove this after refactoring the QChart component (the style shouldn't be in the config).
			styledTableConfig()
			{
				return {
					...this.tableConfig,
					...this.styledChartConfig
				}
			}
		},

		methods: {
			displayEmit(emittedData)
			{
				const str = JSON.stringify(emittedData)
				alert(str)
			}
		}
	}
</script>