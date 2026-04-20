<template>
	<div>
		<input
			v-for="(_, index) in pin"
			:key="index"
			maxlength="1"
			:data-testid="dataTestid"
			:placeholder="pinPlaceholder(index)"
			ref="pin-input"
			:type="pinType"
			:class="[classes, { error: hasError(index), filled: isFilled(index) }]"
			v-model="pin[index]"
			@input="handleInput(index)"
			@keyup="handleKeyup(index, $event)"
			@focus="focusElement(index)"
			@blur="unfocusElement(index)" />
	</div>
</template>

<script>
	import _findLastIndex from 'lodash-es/findLastIndex'
	import _get from 'lodash-es/get'

	export default {
		name: 'QPinInput',

		emits: ['update:modelValue'],

		props: {
			/**
			 * id for testing
			 */
			dataTestid: String,

			/**
			 * number of digits in pin
			 */
			charNumber: {
				type: Number,
				default: 4
			},

			/**
			 * string to modify
			 */
			modelValue: {
				type: String,
				default: ''
			},

			/**
			 * input type
			 */
			pinType: {
				type: String,
				default: 'text',
				validator: (value) => value === 'text' || value === 'password'
			},

			/**
			 * pin color theme. can also be secondary and blue
			 */
			theme: {
				type: String,
				default: ''
			},

			/**
			 * pin filled inputs border colors. can also be light, gray, primary, secondary
			 */
			borderColor: {
				type: String,
				default: ''
			},

			/**
			 * allow the pin to receive letters
			 */
			alphanumeric: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data() {
			return {
				pin: [...new Array(this.charNumber)].map((_, index) => _get(this.modelValue, `[${index}]`, '')), // value displayed on the inputs
				error: [], // saves pin positions with error
				activeElement: null //focused element
			}
		},

		computed: {
			classes() {
				const btnPrefix = 'q-pin-input'
				const classes = [btnPrefix]
				if (this.theme) classes.push(`${btnPrefix}--${this.theme}`)
				if (this.pinType) classes.push(`${btnPrefix}--${this.pinType}`)
				if (this.borderColor) {
					if (this.borderColor === 'theme') classes.push(`${btnPrefix}--${this.theme}-border`)
					else classes.push(`${btnPrefix}--${this.borderColor}-border`)
				}
				return classes
			}
		},

		methods: {
			currentIndex() {
				const lastChar = this.alphanumeric
					? _findLastIndex(this.pin, (char) => this.isAlphanumeric(char))
					: _findLastIndex(this.pin, (char) => this.isDigit(char))
				return lastChar + 1
			},

			handleInput(index) {
				this.pin[index] = ''
			},
			handleKeyup(index, event) {
				if (this.alphanumeric) {
					//ignore modifier keys for alphanumeric
					if (event.key === 'Shift') return
					if (event.key === 'CapsLock') return
				}

				if (this.hasError(index)) {
					//clear input from errors at new user input
					if (this.modelValue[index]) this.pin[index] = this.modelValue[index]
					else this.pin[index] = ''
					this.error[index] = false
				}

				if (event.key === 'ArrowLeft') this.moveLeft(index)
				else if (event.key === 'ArrowRight') this.moveRight(index)
				else if (event.key === 'Delete') this.delete(index)
				else if (event.key === 'Backspace') this.backspace(index)
				else if (this.isDigit(event.key)) this.digit(index, event.key)
				else if (this.alphanumeric && this.isAlphanumeric(event.key)) this.digit(index, event.key)
				else {
					this.error[index] = true
					this.pin[index] = ''
				}
			},

			/*
			 * KEY EVENT METHODS
			 */
			moveLeft(index) {
				if (index > 0) this.focusElement(index - 1)
			},

			moveRight(index) {
				if (index < this.charNumber - 1) this.focusElement(index + 1)
			},

			backspace(index) {
				this.emitChange(index, '')
				this.moveLeft(index)
			},

			delete(index) {
				this.emitChange(index, '')
			},

			digit(index, digit) {
				this.emitChange(index, digit)
				this.moveRight(index)
			},

			/*
			 * PIN VISUALS METHODS
			 */
			hasError(index) {
				return this.error[index] === true
			},

			isFilled(index) {
				return this.pin[index] !== ''
			},

			pinPlaceholder(index) {
				if (!this.isFilled(index) && index !== this.activeElement) return '•'
			},

			/*
			 * FOCUS
			 */
			focusElement(index) {
				const currentIndex = this.currentIndex()
				if (index <= currentIndex) {
					this.activeElement = index
					if (index < this.charNumber) this.$refs['pin-input'][index].focus()
				} else this.focusElement(currentIndex)
			},

			unfocusElement(index) {
				if (this.hasError(index)) {
					if (this.modelValue[index]) this.pin[index] = this.modelValue[index]
					else this.pin[index] = ''
					this.error[index] = false
				}
				this.activeElement = null
			},

			/*
			 * MODELVALUE
			 */
			emitChange(index, value) {
				const newPin = []
				for (let i = 0; i < this.charNumber; i++) {
					if (this.alphanumeric) {
						if (this.isAlphanumeric(this.pin[i]) || this.pin[i] === '') newPin.push(this.pin[i])
					} else {
						if (this.isDigit(this.pin[i]) || this.pin[i] === '') newPin.push(this.pin[i])
					}
				}
				newPin[index] = value
				this.pin[index] = value
				this.$emit('update:modelValue', newPin.join(''))
			},

			/*
			 * VALIDATORS
			 */
			isDigit(value) {
				if (typeof value === 'string' || typeof value === 'number') {
					if (typeof value === 'string' && value.length === 1) {
						const charCode = value.charCodeAt(0)
						return charCode >= 48 && charCode <= 57
					} else if (typeof value === 'number') {
						return value >= 0 && value <= 9
					}
				}
				return false
			},
			isAlphanumeric(value) {
				if (this.isDigit(value)) return true
				if (typeof value === 'string' && value.length === 1) {
					const charCode = value.charCodeAt(0)
					return (charCode >= 65 && charCode <= 90) || (charCode >= 97 && charCode <= 122)
				}
				return false
			}
		},

		watch: {
			modelValue(newValue) {
				this.error.splice()
				this.pin.splice(0, Infinity, ...[...new Array(this.charNumber)].map((_, index) => _get(newValue, `[${index}]`, '')))
			},

			charNumber(newValue) {
				this.error.splice()
				this.pin.splice(0, Infinity, ...[...new Array(newValue)].map((_, index) => _get(this.modelValue, `[${index}]`, '')))
			}
		}
	}
</script>
