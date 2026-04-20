<template>
	<div
		:data-testid="dataTestid"
		:class="classes">
		<div class="input-types">
			<div
				v-if="inputType === 'def'"
				class="numeric-input-type"
				role="value-display">
				<label
					class="form-label"
					v-if="label">
					{{ label }}
				</label>
				<q-numeric-input
					type="text"
					:model-value="Number(sliderValue)"
					readonly />
			</div>

			<div
				v-if="inputType === 'sec'"
				class="secondary-input-type"
				role="value-display">
				<label
					class="form-label sec-form-label"
					v-if="label">
					{{ label }}
				</label>
				<div class="sec-slider-value">{{ sliderValue }}</div>
			</div>

			<div
				v-if="inputType === 'box'"
				class="box-moving-container"
				role="value-display">
				<div
					class="box-input-type"
					:style="followSlider">
					<div class="label-box">{{ sliderValue }}</div>
					<div class="triangle"></div>
				</div>
			</div>
		</div>

		<div class="slider-container">
			<input
				type="range"
				class="q-slider"
				v-model="sliderValue"
				role="slider-input"
				:min="minValue"
				:max="maxValue"
				:style="paintSlider" />
			<div
				class="slider-limits-container"
				v-if="showLimits"
				role="display-limits">
				<div class="slider-limit min-slider-limit">{{ minValue }}</div>
				<div class="slider-limit max-slider-limit">{{ maxValue }}</div>
			</div>
		</div>
	</div>
</template>

<script>
	import QNumericInput from '@/components/inputs/NumericInput.vue'

	export default {
		name: 'QSlider',

		emits: ['update:modelValue'],

		components: {
			QNumericInput
		},

		props: {
			/**
			 * The test unique control identifier.
			 */
			dataTestid: String,

			/**
			 * The string value to be edited by the input.
			 */
			modelValue: {
				type: Number,
				default: 0
			},

			/**
			 * The minimum value for the slider range.
			 */
			minValue: {
				type: Number,
				default: 0
			},

			/**
			 * The maximum value for the slider range.
			 */
			maxValue: {
				type: Number,
				default: 100
			},

			/**
			 * Controls whether the label box is displayed.
			 */
			labelBox: {
				type: Boolean,
				default: true
			},

			/**
			 * Label for the input for accessibility purposes. If not provided, no label is rendered.
			 */
			label: {
				type: String,
				default: ''
			},

			/**
			 * Flag to show the numeric input associated with the slider.
			 */
			showInput: {
				type: Boolean,
				default: true
			},

			/**
			 * Determines whether to display the minimum and maximum limits of the slider.
			 */
			showLimits: {
				type: Boolean,
				default: true
			},

			/**
			 * The input type to be used for displaying the numeric value ('def', 'sec', 'box').
			 */
			inputType: {
				type: String,
				default: 'box'
			},

			/**
			 * The theme of the slider ('primary', 'secondary', etc.) affecting the slider background color.
			 */
			theme: {
				type: String,
				default: 'primary'
			}
		},

		expose: [],

		data() {
			return {
				sliderValue: this.modelValue
			}
		},

		computed: {
			/**
			 * Array of class names to be applied to the slider, derived from the provided theme.
			 */
			classes()
			{
				const btnPrefix = 'slider-ui'
				const classes = [btnPrefix]

				if (this.theme)
					classes.push(`${btnPrefix}--${this.theme}`)

				return classes
			},

			/**
			 * Computes the left percentage offset of the movable slider label box.
			 */
			leftValue()
			{
				return ((this.sliderValue - this.minValue) / (this.maxValue - this.minValue)) * 100
			},

			/**
			 * Style object to position the label box horizontally following the slider.
			 */
			followSlider()
			{
				return {
					left: `${this.leftValue}%`
				}
			},

			/**
			 * Style object to visually represent the progress bar within the slider based on the theme color.
			 */
			paintSlider()
			{
				return {
					background: `linear-gradient(90deg, var(--${this.theme}) ${this.leftValue}%, var(--light) ${this.leftValue}%)`
				}
			}
		},

		watch: {
			sliderValue(newValue) {
				this.$emit('update:modelValue', Number(newValue))
			},

			modelValue(newValue) {
				this.sliderValue = newValue
			},

			minValue() {
				if (this.sliderValue < this.minValue) {
					this.sliderValue = this.minValue
				}
			},

			maxValue() {
				if (this.sliderValue > this.maxValue) {
					this.sliderValue = this.maxValue
				}
			}
		}
	}
</script>
