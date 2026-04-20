<template>
	<div class="c-timeline_horizontal">
		<div class="c-timeline_horizontal_line">
			<div
				v-for="circle in circles"
				:key="circle.id"
				:title="circle.id"
				role="button"
				data-testid="bubble-group"
				aria-label="timeline-group"
				:class="[
					'c-timeline_circle',
					{ active: activeItem === circle.id && selectedGroupKey }
				]"
				:style="{
					left: `${circle.position}%`
				}"
				@click.stop.prevent="setActiveItem(circle)" />
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QTimelineSummary',

		emits: ['selected-group'],

		expose: [],

		data() {
			return {
				activeItem: null
			}
		},

		props: {
			/**
			 * The timeline data.
			 */
			timeLineData: {
				type: Object,
				default: () => ({
					rows: []
				})
			},

			/**
			 * The timeline configuration.
			 */
			config: {
				type: Object,
				required: true
			},

			/**
			 * The timeline data, grouped by date according to the configured scale.
			 */
			groupedData: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The key of the selected group.
			 */
			selectedGroupKey: String
		},

		computed: {
			/**
			 * Calculates and provides a list of positions for circles representing groups of events on the timeline.
			 */
			circles() {
				const tlItems = this.timeLineData.rows
				if (tlItems[0]?.Escala !== "un" && tlItems[0]?.TipoTimeLine !== "S") {
					tlItems.sort(function (a, b) {
						return new Date(a.Data) - new Date(b.Data)
					})
				}
				const circlList = []
				if (tlItems.length > 0) {
					const firstDate = this.toDateTime(new Date(tlItems[0].Data))
					const lastDate = this.toDateTime(
						new Date(tlItems[tlItems.length - 1].Data)
					);

					const scale = this.config.scale

					const lastInt =
						(lastDate.Year - firstDate.Year) * 365 +
						(lastDate.Month - firstDate.Month) * 30 +
						(lastDate.Day - firstDate.Day)

					// Create other circles
					for (const key in this.groupedData) {
						const groupDate = this.toDateTime(
							new Date(this.groupedData[key][0].Data)
						)
						const groupPosition = this.getCirclePosition(
							lastInt,
							firstDate,
							groupDate,
							scale
						)
						const circle = {
							date: groupDate,
							id: key,
							position: Math.ceil(groupPosition * 100)
						}
						circlList.push(circle)
					}
				}
				return circlList
			}
		},

		methods: {
			/**
			 * Sets the active circle to be highlighted. Emits an event when a circle is selected.
			 * @param {Object} circle - The circle object to be set as active.
			 */
			setActiveItem(circle) {
				if (this.activeItem === circle.id) {
					this.activeItem = null
					this.$emit('selected-group', '')
				}
				else {
					this.activeItem = circle.id
					this.$emit('selected-group', circle.id)
				}
			},

			/**
			 * Converts a Date into an object containing day, month, and year as integers.
			 * @param {Date} date - The date to convert.
			 * @returns {Object} The date as an object with Day, Month, and Year properties.
			 */
			toDateTime(date) {
				const dateTimeObj = {
					Day: date.getUTCDate(),
					Month: date.getMonth() + 1, // It starts month from 0
					Year: date.getFullYear()
				}
				return dateTimeObj
			},

			/**
			 * Calculates the position of a group's circle within the timeline relative to other groups.
			 * @param {Number} lastInt - The interval representing the last group's position on the scale.
			 * @param {Object} dateBegin - The starting date for the timeline as an object.
			 * @param {Object} dateEnd - The end date for the timeline as an object.
			 * @param {String} scale - The scale by which the timeline is grouped ('yy', 'mm', 'ww', 'dd').
			 * @returns {Number} The position of the group as a fraction of the timeline's total length.
			 */
			getCirclePosition(lastInt, dateBegin, dateEnd, scale) {
				let thisInt = 0

				switch (scale) {
					case 'yy':
						thisInt = (dateEnd.Year - dateBegin.Year) * 365
						break
					case 'mm':
						thisInt =
							(dateEnd.Year - dateBegin.Year) * 365 +
							(dateEnd.Month - dateBegin.Month) * 30
						break
					case 'ww':
						thisInt =
							(dateEnd.Year - dateBegin.Year) * 365 +
							(dateEnd.Month - dateBegin.Month) * 30 +
							(dateEnd.Day - dateBegin.Day) / 7
						break
					case 'dd':
						thisInt =
							(dateEnd.Year - dateBegin.Year) * 365 +
							(dateEnd.Month - dateBegin.Month) * 30 +
							(dateEnd.Day - dateBegin.Day)
						break
					case 'un':
						thisInt =
							(dateEnd.Year - dateBegin.Year) * 365 +
							(dateEnd.Month - dateBegin.Month) * 30 +
							(dateEnd.Day - dateBegin.Day)
						break
					default:
						break
				}

				return thisInt / lastInt
			}
		}
	}
</script>
