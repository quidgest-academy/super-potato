<template>
	<div class="q-progress__container">
		<div
			:id="controlId"
			:class="containerClasses"
			tabindex="0">
			<div
				:class="progressClasses"
				:style="progressBarStyle">
				<span
					v-if="displayText"
					class="q-progress__bar-text"
					:style="textColor">
					{{ displayText }}
				</span>
			</div>
		</div>

		<div
			v-if="showLimits"
			class="q-progress__values">
			<span class="q-progress__values-min">{{ min }}</span>
			<span class="q-progress__values-max">{{ max }}</span>
		</div>
	</div>
</template>

<script>
	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

	export default {
		name: 'QProgress',

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the progress bar.
			 */
			id: String,

			/**
			 * The current value of the progress bar.
			 */
			modelValue: {
				type: Number,
				required: true
			},

			/**
			 * Text displayed in the progress bar.
			 */
			text: {
				type: String,
				default: ''
			},

			/**
			 * Whether the progress bar is striped.
			 */
			striped: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the progress bar has an animation.
			 */
			animated: {
				type: Boolean,
				default: true
			},

			/**
			 * Whether the progress bar is mini.
			 */
			mini: {
				type: Boolean,
				default: false
			},

			/**
			 * The color of the progress bar.
			 */
			barColor: String,

			/**
			 * Minimum value for the range of the progress bar.
			 */
			min: {
				type: Number,
				default: 0
			},

			/**
			 * Maximum value for the range of the progress bar.
			 */
			max: {
				type: Number,
				default: 100
			},

			/**
			 * Whether to display the min and max values.
			 */
			showLimits: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether to display the current value in the progress bar.
			 */
			showValue: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data() {
			return {
				controlId: this.id || `q-progress-${this._.uid}`
			}
		},

		computed: {
			/**
			 * A list of classes to apply to the progress bar's container.
			 */
			containerClasses() {
				return ['q-progress', { 'q-progress--mini': this.mini }]
			},

			/**
			 * A list of classes to apply to the progress bar.
			 */
			progressClasses() {
				return [
					'q-progress__bar',
					{
						'q-progress__bar--striped': this.striped,
						'q-progress__bar--animated': this.animated && this.percentage < 100
					}
				]
			},

			/**
			 * The percentage of the current progress.
			 */
			percentage() {
				return (((this.modelValue - this.min) / (this.max - this.min)) * 100).toFixed(2)
			},

			/**
			 * The style to apply to the progress bar.
			 */
			progressBarStyle() {
				const style = {
					width: `${this.percentage}%`
				}

				if (this.barColor) style.backgroundColor = this.barColor

				return style
			},

			/**
			 * The text to be displayed in the progress bar.
			 */
			displayText() {
				return this.text ?? (this.showValue ? this.modelValue : '')
			},

			/**
			 * A color to apply to the text of the progress bar, based on the bar's color.
			 */
			textColor() {
				return {
					color: genericFunctions.getReadableTextColor(this.barColor)
				}
			}
		}
	}
</script>
