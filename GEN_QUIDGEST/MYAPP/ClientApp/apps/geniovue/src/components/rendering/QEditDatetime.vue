<template>
	<q-date-time-picker
		:id="`${tableName}_${rowIndex}_${columnName}`"
		:size="size"
		:classes="classes"
		:date-time-type="options.dateTimeType"
		:format="options.format"
		:disabled="options.disabled"
		:readonly="options.readonly"
		:model-value="datetimeValue"
		:teleport="options.teleport ?? false"
		:locale="locale"
		@update:model-value="update" />
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { timeToString } from '@quidgest/clientapp/utils/genericFunctions'
	import { inputSize } from '@quidgest/clientapp/constants/enums'

	import QDateTimePicker from '@/components/inputs/QDateTimePicker.vue'

	export default {
		name: 'QEditDatetime',

		emits: ['update', 'loaded'],

		components: {
			QDateTimePicker
		},

		props: {
			/**
			 * The current date or time value for the input control.
			 */
			value: {
				type: [Number, String],
				default: ''
			},

			/**
			 * The name of the table in the database, used to construct the ID for the component.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the current row, used to construct the ID for the component.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name of the column in the database, used to construct the ID for the component.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * A set of options to configure the datetime input, such as blocked state and display type.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The size class for the input component.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * An array of additional classes to apply to the input component.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Current system locale
			 */
			locale: {
				type: String,
				default: 'en-US'
			}
		},

		expose: [],

		mounted()
		{
			this.$emit('loaded')
		},

		computed: {
			/**
			 * Formats the input value according to the type of date time being edited, either Time or Date.
			 */
			datetimeValue()
			{
				if (this.options.dateTimeType === 'time' && this.value.length === 5)
				{
					const units = this.value.split(':')
					return {
						hours: parseInt(units[0]),
						minutes: parseInt(units[1])
					}
				}
				return this.value
			}
		},

		methods: {
			/**
			 * Updates the value of the control, converting time to a string format if necessary.
			 * @param {Object|String} event - The new value for the datetime input.
			 */
			update(event)
			{
				// If updating time, convert the time object to a string format.
				let updatedTime = event
				if (this.options.dateTimeType === 'time')
					updatedTime = timeToString(event)
				// If date value is a date object
				else if (typeof event?.toISOString === 'function')
				{
					// Set time units that are not used by this date time type to zero
					switch (this.options.dateTimeType)
					{
						case 'date':
							event.setHours(0, 0, 0, 0)
							break
						case 'dateTime':
							event.setSeconds(0, 0)
							break
						case 'dateTimeSeconds':
							event.setMilliseconds(0)
							break
					}

					updatedTime = event?.toISOString()
				}
				// If date value is already a string
				else
					updatedTime = event

				this.$emit('update', updatedTime)
			}
		}
	}
</script>
