<template>
	<div :class="keyboardInputClasses">
		<div
			class="q-keyboard__container"
			ref="keyboardContainer">
			<div class="q-keyboard__input-wrapper">
				<q-numeric-input
					type="text"
					:model-value="computedModelValue"
					@focus="togglePopupOn" />
			</div>

			<div
				v-if="popupTrigger"
				class="q-keyboard__popup">
				<div class="q-keyboard__input-popup">
					<div class="q-keyboard__decoration"></div>
					<div>
						<div
							v-for="(row, rowIndex) in keyboardRows"
							:key="rowIndex"
							:class="['line' + (rowIndex + 1), 'q-keyboard__line1']">
							<q-button
								v-for="item in row"
								:key="item"
								class="q-keyboard__digit q-keyboard__digit-edit"
								@mousedown="inputDigit(item)">
								{{ item }}
							</q-button>
						</div>
						<div class="q-keyboard__final-line">
							<i
								class="q-keyboard__icons q-keyboard--backspace"
								@mousedown="inputDigit('q-keyboard--backspace')">
								<q-icon icon="back" />
							</i>
							<div
								class="q-keyboard__enter"
								:title="title">
								<i
									class="q-keyboard__icons q-keyboard--check-circle"
									@mousedown="inputDigit('Enter')">
									<q-icon icon="check" />
								</i>
								<p class="q-keyboard__enter-title">{{ title }}</p>
							</div>
							<i
								class="q-keyboard__icons q-keyboard--cancel"
								@mousedown="inputDigit('AC')">
								<q-icon icon="remove-sign" />
							</i>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import QNumericInput from '@/components/inputs/NumericInput.vue'

	export default {
		name: 'QKeyboard',

		emits: ['update:modelValue'],

		components: {
			QNumericInput
		},

		props: {
			/**
			 * The data-testid attribute of the key.
			 */
			dataTestid: String,

			/**
			 * The position of the input.
			 */
			inputPlacement: {
				type: String,
				default: ''
			},

			/**
			 * The modelValue of the input.
			 */
			modelValue: {
				type: Number,
				default: 0
			},

			/**
			 * The style of the keyboard.
			 */
			kbStyle: {
				type: String,
				default: ''
			},

			/**
			 * The title of the Enter input.
			 */
			title: {
				type: String,
				default: 'Enter'
			},

			/**
			 * The options of keyboard characters.
			 */
			options: {
				type: Array,
				default: () => [1, 2, 3, 4, 5, 6, 7, 8, 9, 0]
			},

			/**
			 * The number of rows in the keyboard.
			 */
			rows: {
				type: Number,
				default: 2
			}
		},

		// TODO: Remove these properties from the "expose" (only necessary for unit tests).
		expose: [
			'focus',
			'popupTrigger',
			'togglePopupOn',
			'togglePopupOff'
		],

		data()
		{
			return {
				localModelValue: this.modelValue, // using a local state that mirrors the prop, but whose value can be changed within the component, so the input reflects the numbers digited in the keyboard even if it is not called by a parent component.
				inputData: [],
				popupTrigger: false
			}
		},

		mounted()
		{
			document.addEventListener('mousedown', this.onMousedown)
		},

		beforeUnmount()
		{
			// Hook to remove event listener when the component is destroyed, to avoid potential memory leaks.
			document.removeEventListener('mousedown', this.onMousedown)
		},

		computed: {
			keyboardInputClasses()
			{
				const keyboardPrefix = 'q-keyboard'
				const classes = [keyboardPrefix]

				if (this.kbStyle) classes.push(`${keyboardPrefix}--${this.kbStyle}`)

				if (this.inputPlacement)
					classes.push(`${keyboardPrefix}--inputPlacement-${this.inputPlacement}`)

				return classes
			},

			keyboardRows()
			{
				const itemsPerRow = Math.ceil(this.options.length / this.rows) // Round a number up to the nearest integer, ex: 1 => 1 / 1.1 => 2
				const result = []
				for (let i = 0; i < this.rows; i++)
					result.push(this.options.slice(i * itemsPerRow, (i + 1) * itemsPerRow)) // This slices a portion of options array to create a new inner array (or row).
				return result
			},

			computedModelValue: {
				get()
				{
					return this.localModelValue
				},
				set(value)
				{
					this.localModelValue = value
					this.$emit('update:modelValue', value)
				}
			}
		},

		methods: {
			onMousedown(event)
			{
				if (event.target && !this.$refs.keyboardContainer.contains(event.target))
					this.togglePopupOff()
			},

			inputDigit(digit)
			{
				if (digit === 'q-keyboard--backspace')
				{
					this.inputData.pop()
					this.localModelValue =
						this.inputData.length === 0 ? 0 : parseInt(this.flatArray())
				}
				else if (digit === 'AC')
				{
					this.localModelValue = 0
					this.inputData = []
				}
				else if (digit === 'Enter')
				{
					this.localModelValue =
						this.flatArray().length === 0 || isNaN(parseInt(this.flatArray()))
							? this.localModelValue
							: parseInt(this.flatArray())
				}
				else
				{
					this.inputData.push(digit)
					this.localModelValue = parseInt(this.flatArray())
				}
				// Emit the update:modelValue event here.
				this.$emit('update:modelValue', this.localModelValue)
			},

			togglePopupOn()
			{
				this.popupTrigger = true
			},

			togglePopupOff()
			{
				this.popupTrigger = false
			},

			focus()
			{
				this.popupTrigger = true
			},

			flatArray()
			{
				return this.inputData.join('')
			}
		}
	}
</script>
