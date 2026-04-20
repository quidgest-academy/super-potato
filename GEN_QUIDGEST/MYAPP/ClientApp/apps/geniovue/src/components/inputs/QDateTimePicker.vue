<template>
	<q-field
		:id="id"
		:readonly="readonly"
		:disabled="disabled"
		:size="size">
		<template #prepend>
			<a
				class="q-date-time-picker__icon-wrapper"
				@click="$emit('reset-icon-click')">
				<q-icon :icon="inputIcon" />
			</a>
		</template>

		<datepicker
			v-model="model"
			:disabled="disabled"
			:readonly="readonly"
			:time-picker="isTimePicker"
			:format="format"
			:placeholder="placeholder"
			:text-input="textInputOptions"
			:is24="!time12h"
			:locale="locale"
			:enable-time-picker="hasTimePicker"
			:enable-seconds="enableSeconds"
			:config="dateTimeConfig"
			:teleport="teleport"
			:aria-labels="{ input: $attrs?.ariaLabel }"
			hide-input-icon
			clearable
			auto-apply
			ref="datePickerPluginEl">
			<template #clear-icon="{ clear }">
				<q-button
					class="q-date-time-picker__clear"
					:title="texts.clearValue"
					variant="ghost"
					color="neutral"
					borderless
					@click="clear">
					<q-icon icon="close" />
				</q-button>
			</template>
		</datepicker>
	</q-field>
</template>

<script>
	import Datepicker from '@vuepic/vue-datepicker'

	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		clearValue: 'Clear value'
	}

	export default {
		name: 'QDateTimePicker',

		components: {
			Datepicker
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: {
				type: String,
				default: ''
			},

			/**
			 * Value of the control, could be a date, time or date with time
			 */
			modelValue: {
				type: [String, Object],
				default: null
			},

			/**
			 * If control is Read only
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * If control is a Fixed value, not to be changed with input.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Format
			 */
			format: {
				type: String
			},

			/**
			 * DateTime type (date, dateTime, dateTimeSeconds, time)
			 */
			dateTimeType: {
				type: String,
				default: 'date'
			},

			/**
			 * Set datepicker locale.
			 * Datepicker will use built in javascript locale formatter to extract month and weekday names.
			 * https://vue3datepicker.com/api/props/#locale
			 */
			locale: {
				type: String,
				default: 'en-US'
			},

			/**
			 * Sizing class for the control
			 */
			size: {
				type: String,
				default: 'medium'
			},

			/**
			 * Custom classes
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Date time input placeholder
			 */
			placeholder: {
				type: String,
				default: ''
			},

			/**
			 * Localization and customization of textual content within the component.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * If the time is in 24h format
			 */
			time12h: {
				type: Boolean,
				default: false
			},

			/**
			 * If the calendar and time picker are to be teleported into the input location
			 * this is used mainly to fix the positioning of the component on storybook
			 */
			teleport: {
				type: Boolean,
				default: true
			}
		},

		emits: [
			'update:modelValue',
			'reset-icon-click'
		],

		expose: [],

		beforeUnmount() {
			// bugfix - memory leak!
			const el = this.$refs.datePickerPluginEl
			const onScrollFn = el?.onScroll
			if (typeof onScrollFn === 'function') {
				const target = document.getElementById('app')
				target?.removeEventListener('scroll', onScrollFn)
			}

		},

		computed: {
			model: {
				get()
				{
					if (typeof this.modelValue === 'string' && this.dateTimeType === 'time')
					{
						if (this.modelValue === '')
							return null

						const timeObj = this.modelValue.split(':')
						return {
							hours: timeObj[0],
							minutes: timeObj[1]
						}
					}
					return this.modelValue
				},
				set(value)
				{
					this.$emit('update:modelValue', value)
				}
			},

			enableSeconds() {
				return this.dateTimeType === 'dateTimeSeconds'
			},

			hasTimePicker() {
				return this.dateTimeType !== 'date'
			},
			isTimePicker() {
				return this.dateTimeType === 'time'
			},

			textFormat() {
				switch (this.dateTimeType) {
					case 'date':
						return ['dd/MM/yyyy', 'dd-MM-yyyy', 'ddMMyyyy', 'dd.MM.yyyy']
					case 'dateTime':
						return ['dd/MM/yyyy HH:mm', 'dd-MM-yyyy HH:mm', 'ddMMyyyy HH:mm', 'dd.MM.yyyy HH:mm', 'dd/MM/yyyy HHmm', 'dd-MM-yyyy HHmm', 'ddMMyyyy HHmm', 'dd.MM.yyyy HHmm']
					case 'dateTimeSeconds':
						return ['dd/MM/yyyy HH:mm:ss', 'dd-MM-yyyy HH:mm:ss', 'ddMMyyyy HH:mm:ss', 'dd.MM.yyyy HH:mm:ss', 'dd/MM/yyyy HHmmss', 'dd-MM-yyyy HHmmss', 'ddMMyyyy HHmmss', 'ddMMyyyy HHmmss']
					case 'time':
						return ['HH:mm', 'HHmm']
					default:
						return ''
				}
			},

			textInputOptions() {
				return {
					format: this.textFormat
				}
			},

			dateTimeConfig() {
				return {
					closeOnAutoApply: false
				}
			},

			inputIcon() {
				return this.dateTimeType === 'time' ? 'time' : 'date'
			}
		}
	}
</script>
