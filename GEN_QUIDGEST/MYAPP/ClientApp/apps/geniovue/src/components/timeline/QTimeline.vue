<template>
	<div :class="$attrs.class">
		<!-- Timeline header -->
		<div
			v-if="showSummary"
			class="row">
			<div class="col">
				<div class="float-right">
					<span class="c-timeline__scale">
						{{ scale }}
					</span>

					<q-button
						data-testid="refresh-btn"
						:label="texts.reset"
						@click="resetSelection">
						<q-icon icon="reset" />
					</q-button>
				</div>
			</div>
		</div>

		<!-- Timeline horizontal summary -->
		<q-timeline-summary
			v-if="showSummary"
			data-testid="horizontal"
			:time-line-data="timeLineData"
			:config="config"
			:grouped-data="groupedData"
			:selected-group-key="selectedGroupKey"
			@selected-group="selectedGroup" />

		<!-- Timeline builder -->
		<q-timeline-collapsible :show="isExpanded">
			<div
				:class="tipoTimeline === 'S'
					? 'c-simple_accordion__panel-body'
					: 'c-accordion__panel-body'">
				<div
					:class="tipoTimeline === 'S'
						? 'c-simple_timeline__container c-simple_timeline--alternate'
						: 'c-timeline__container c-timeline--alternate'">
					<template
						v-for="(groups, groupKey) in groupedData"
						:key="groupKey">
						<template v-if="selectedGroupKey === '' || selectedGroupKey === groupKey">
							<div
								v-for="tlItem in groups"
								:key="tlItem.Identifier"
								class="c-timeline__item"
								data-testid="item-card">
								<div v-if="tlItem.Texto && tlItem.TipoTimeLine !== 'S'">
									<q-timeline-circle
										:circlestyle="tlItem.Background"
										:icon="tlItem.Icon" />
								</div>
								<q-timeline-item
									data-testid="vertical-timeline"
									:aria-expanded="isExpanded"
									role="definition"
									aria-label="vertical-timeline"
									:aria-hidden="isExpanded"
									:tl-item="tlItem"
									:date-time-format="config.dateTimeFormat"
									@form-popup="$emit('show-popup', $event)" />
							</div>
						</template>
					</template>
				</div>
			</div>
		</q-timeline-collapsible>
	</div>
</template>

<script>
	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	import QTimelineSummary from './QTimelineSummary.vue'
	import QTimelineItem from './QTimelineItem.vue'
	import QTimelineCollapsible from './QTimelineCollapsible.vue'
	import QTimelineCircle from './QTimelineCircle.vue'

	const DEFAULT_TEXTS = {
		reset: 'Reset',
		daily: 'Daily',
		weekly: 'Weekly',
		monthly: 'Monthly',
		yearly: 'Yearly'
	}

	export default {
		name: 'QTimeline',

		emits: ['show-popup'],

		components: {
			QTimelineSummary,
			QTimelineItem,
			QTimelineCollapsible,
			QTimelineCircle
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The data from which we will display the timeline.
			 */
			timeLineData: {
				type: Object,
				required: true
			},

			/**
			 * The type of the timeline.
			 */
			tipoTimeline: {
				type: String,
				required: true
			},

			/**
			 * The timeline configuration.
			 */
			config: {
				type: Object,
				required: true
			},

			/**
			 * The necessary strings to be used inside the component.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || `timeline-${this._.uid}`,
				scale: '',
				isExpanded: false,
				groupedData: null,
				selectedGroupKey: '',
				computedHeight: 'auto'
			}
		},

		mounted()
		{
			this.handleData()
		},

		computed: {
			/**
			 * Whether or not to display the timeline summary.
			 */
			showSummary()
			{
				return this.config.scale !== 'un' || this.tipoTimeline === 'S'
			}
		},

		methods: {
			/**
			 * Handles the processing of timeline data based on configuration settings.
			 * Groups the timeline items according to the configured scale,
			 * and updates the visibility of the timeline summary if necessary.
			 */
			handleData()
			{
				this.groupTimelineItems()

				if (!this.showSummary)
					this.isExpanded = true
			},

			/**
			 * Groups timeline items based on the configured scale or individually.
			 */
			groupTimelineItems()
			{
				const tlItems = this.timeLineData.rows
				tlItems.sort((a, b) => new Date(a.Data) - new Date(b.Data))
				const scale = this.config.scale

				switch (scale)
				{
					case 'yy':
						this.scale = this.texts.yearly
						this.groupedData = this.groupByYear(tlItems)
						break
					case 'mm':
						this.scale = this.texts.monthly
						this.groupedData = this.groupByMonth(tlItems)
						break
					case 'ww':
						this.scale = this.texts.weekly
						this.groupedData = this.groupByWeek(tlItems)
						break
					case 'dd':
						this.scale = this.texts.daily
						this.groupedData = this.groupByDay(tlItems)
						break
					case 'un':
						this.groupedData = tlItems.scale === "un" && tlItems.TipoTimeLine !== "S"
							? this.groupIndividually(tlItems)
							: this.groupByDay(tlItems);
						break
					default:
						break
				}
			},

			/**
			 * Organizes timeline items by year.
			 */
			groupByYear(tlItems)
			{
				const yearGroups = {}

				tlItems.forEach((row) => {
					const newDate = new Date(row.Data)
					const yearNotation = newDate.getFullYear()

					yearGroups[yearNotation] = [
						...(yearGroups[yearNotation] || []),
						row
					]
				})

				return yearGroups
			},

			/**
			 * Organizes timeline items by month.
			 */
			groupByMonth(tlItems)
			{
				const monthGroups = {}

				tlItems.forEach((row) => {
					if (row.Data)
					{
						const newDate = new Date(row.Data)
						const monthGroupNotation = 1 + newDate.getMonth() + '/' + newDate.getFullYear()

						monthGroups[monthGroupNotation] = [
							...(monthGroups[monthGroupNotation] || []),
							row
						]
					}
				})

				return monthGroups
			},

			/**
			 * Organizes timeline items by week.
			 */
			groupByWeek(tlItems)
			{
				const weekGroups = {}

				tlItems.forEach((row) => {
					const newDate = new Date(row.Data)
					const weekNotation = `${newDate.getFullYear()}-W${this.getWeek(
						newDate
					)}`

					weekGroups[weekNotation] = [
						...(weekGroups[weekNotation] || []),
						row
					]
				})

				return weekGroups
			},

			/**
			 * Organizes timeline items by day.
			 */
			groupByDay(tlItems)
			{
				const dayGroups = {}

				tlItems.forEach((row) => {
					const newDate = new Date(row.Data)
					const dayNotation =
						newDate.getUTCDate() +
						'/' +
						(1 + newDate.getMonth()) +
						'/' +
						newDate.getFullYear()

					dayGroups[dayNotation] = [
						...(dayGroups[dayNotation] || []),
						row
					]
				})

				return dayGroups
			},

			/**
			 * Organizes each timeline item individually.
			 */
			groupIndividually(tlItems)
			{
				const groups = {}

				tlItems.forEach((row) => {
					const newDate = new Date(row.Data)
					const dayNotation =
						newDate.getUTCDate() +
						'/' +
						(1 + newDate.getMonth()) +
						'/' +
						newDate.getFullYear()

					groups[dayNotation] = [row]
				})

				return groups
			},

			/**
			 * Gets the ISO week number for a given Date object.
			 */
			getWeek(rowDate)
			{
				const date = new Date(rowDate.getTime())
				date.setHours(0, 0, 0, 0)
				// Thursday in current week decides the year.
				date.setDate(date.getDate() + 3 - ((date.getDay() + 6) % 7))
				// January 4 is always in week 1.
				const week1 = new Date(date.getFullYear(), 0, 4)
				// Adjust to Thursday in week 1 and count number of weeks from date to week1.
				return (
					1 +
					Math.round(
						((date.getTime() - week1.getTime()) / 86400000 -
							3 +
							((week1.getDay() + 6) % 7)) /
							7
					)
				)
			},

			/**
			 * Resets the selection state allowing all timeline groups to be shown if no group key was selected,
			 * otherwise show the selected group.
			 */
			resetSelection()
			{
				if (this.selectedGroupKey)
					this.isExpanded = true
				else
					this.isExpanded = !this.isExpanded

				this.selectedGroupKey = ''
			},

			/**
			 * Called when a group is selected and triggers appropriate display changes.
			 */
			selectedGroup(selectGroup)
			{
				if (selectGroup === '')
					this.isExpanded = false
				else
				{
					this.selectedGroupKey = selectGroup
					this.isExpanded = true
				}
			}
		},

		watch: {
			'timeLineData.rows': {
				handler()
				{
					this.handleData()
				},
				deep: true
			}
		}
	}
</script>
