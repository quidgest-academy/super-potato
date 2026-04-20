<template>
	<div :class="stepClasses">
		<div class="q-step__container">
			<q-button
				class="q-step--margin-left q-step__digit"
				@mousedown="inputDigit('minus')">
				<q-icon icon="minus" />
			</q-button>

			<q-numeric-input
				v-model="stepValue"
				:classes="['q-step__numeric-input', numericInputClasses]"
				:model-value="Number(stepValue)"
				:max-decimals="maxDecimals"
				:min-value="minValue"
				:max-value="maxValue"
				:readonly="!enableInput" />

			<q-button
				class="q-step__digit"
				@mousedown="inputDigit('plus')">
				<q-icon icon="plus" />
			</q-button>
		</div>
	</div>
</template>

<script>
	import QNumericInput from '@/components/inputs/NumericInput.vue'

	export default {
		name: 'QStep',

		emits: ['update:modelValue'],

		components: {
			QNumericInput
		},

		props: {
			/**
			 * Style of the component
			 */
			stepStyle: {
				type: String,
				default: ''
			},

			/**
			 * Define the increment/decrement value for each click.
			 */
			step: {
				type: Number,
				default: 1
			},

			/**
			 * Maximum decimals digits count
			 */
			maxDecimals: {
				type: Number,
				default: 0
			},

			/**
			 * Minimum value allowed
			 */
			minValue: {
				type: Number,
				default: 0
			},

			/**
			 * Maximum value allowed
			 */
			maxValue: {
				type: Number,
				default: 100
			},

			/**
			 * Holds the selection result
			 */
			modelValue: {
				type: Number,
				default: 0
			},

			/**
			 * Allow input directly in Numeric Input
			 */
			enableInput: {
				type: Boolean,
				default: false
			}
		},

		// TODO: Remove these properties from the "expose" (only necessary for unit tests).
		expose: [
			'inputDigit'
		],

		data() {
			return {
				stepValue: this.modelValue
			}
		},

		computed: {
			stepClasses()
			{
				const stepPrefix = 'q-step'
				const classes = [stepPrefix]

				if (this.stepStyle)
					classes.push(`${stepPrefix}--${this.stepStyle}`)

				return classes
			},

			numericInputClasses()
			{
				const numericInputPrefix = 'q-step__numeric-input'
				const classesNumeric = [numericInputPrefix]

				if (!this.enableInput)
					classesNumeric.push(`${numericInputPrefix}--readonly`)

				return classesNumeric
			}
		},

		methods: {
			inputDigit(digit)
			{
				if (digit === 'minus' && this.stepValue > this.minValue) {
					this.stepValue = this.stepValue - this.step
				} else if (digit === 'plus' && this.stepValue < this.maxValue) {
					this.stepValue = this.stepValue + this.step
				} else {
					return
				}
				this.$emit('update:modelValue', this.stepValue)
			}
		},

		watch: {
			stepValue(newValue)
			{
				this.$emit('update:modelValue', Number(newValue))
			},

			modelValue(newValue)
			{
				this.stepValue = newValue
			}
		}
	}
</script>
