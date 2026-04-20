<template>
	<component
		:is="wrapperComponent"
		:size="wrapperSize">
		<template
			v-if="currencySymbol"
			#prepend>
			<span>{{ currencySymbol }}</span>
		</template>

		<q-text-field
			ref="input"
			v-bind="$attrs"
			:id="controlId"
			role="textbox"
			:class="styleClass"
			:readonly="readonly"
			:disabled="disabled"
			:required="isRequired"
			:aria-labelledby="$attrs.ariaLabel ? null : labelId"
			:size="inputSize"
			:placeholder="inputPlaceholder"
			@keydown="onKeydown"
			@cut="onCut"
			@paste="onPaste"
			@drop="onDrop"
			@beforeinput="onBeforeInput"
			@input="onInput"
			@change="onChange" />
	</component>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@quidgest/clientapp/constants/enums'
	import VFragment from '@/components/VFragment.vue'

	export default {
		name: 'QNumeric',

		emits: ['update:modelValue'],

		components: { VFragment },

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * For accessibility (aria-labelledby)
			 * ID, which refers to element that have the text needed for labeling
			 */
			labelId: String,

			/**
			 * Holds the selection result
			 */
			modelValue: Number,

			/**
			 * Size of the input
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Whether the field is readonly.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * For mandatory input
			 */
			isRequired: {
				type: Boolean,
				default: false
			},

			/**
			 * Select currency symbol
			 */
			currencySymbol: {
				type: String,
				default: ''
			},

			/**
			 * Thousand seperator symbol
			 */
			thousandsSeparator: {
				type: String,
				default: ''
			},

			/**
			 * Decimal point symbol
			 */
			decimalPoint: {
				type: String,
				default: '.'
			},

			/**
			 * Maximum integers digits count
			 */
			maxIntegers: {
				type: Number,
				default: 10
			},

			/**
			 * Maximum decimals digits count
			 */
			maxDecimals: {
				type: Number,
				default: 0
			},

			/**
			 * Custom classes to be added to the field
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Whether or not to show a placeholder when the field is empty
			 */
			showEmptyMessage: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				styleClass: [
					'q-numeric-input',
					...this.classes
				],

				/**
				 * Used to control whether to update the control value when the model value changes.
				 * Prevented when the user is changing the value in the control
				 * to prevent the cursor from jumping to the end of the control
				 */
				updateControlOnChange: true,
				/**
				 * FOR: OVERRIDE INPUT KEY
				 * Used to override the input from a key with another key
				 */
				overrideInputKey: null
			}
		},

		mounted()
		{
			// Only update control value if a value is defined, if not, the placeholder will show
			if (this.modelValue !== undefined && this.modelValue !== null)
				// Set control value
				this.updateControlValue()

			if (this.showEmptyMessage)
				this.getInputElement().value = ''
		},

		computed: {
			controlId()
			{
				return this.id || `q-numeric-${this._.uid}`
			},

			inputPlaceholder()
			{
				// Add '1' to the beginning so the formatting function does not remove the 0s
				// since they would be leading 0s
				const placeholderWholeNumber = '1' + '0'.repeat(this.maxIntegers)
				const placeholderFractionNumber = '0'.repeat(this.maxDecimals)

				// Add whole number
				let placeholderNumber = placeholderWholeNumber

				// If there are fraction number digits, add decimal point and fraction number
				if (this.maxDecimals > 0)
					placeholderNumber += this.decimalPoint + placeholderFractionNumber

				// Format number
				const placeholderFormattedNumber = this.getFormattedInputValue(placeholderNumber, this.decimalPoint, this.thousandsSeparator, this.decimalPoint, this.thousandsSeparator)

				// Remove the unneeded '1' and leading thousands separator so the formatted number has all 0s
				const startChar = placeholderFormattedNumber.charAt(1) === this.thousandsSeparator ? 2 : 1

				return placeholderFormattedNumber.slice(startChar)
			},

			/**
			 * FOR: OVERRIDE INPUT KEY
			 * Hash table of key codes that cause a different key than what was pressed to be input.
			 * Keys of this hastable are the event.code property
			 * Values of this hashtable are the character values that should be used
			 */
			keyCodeOverrides()
			{
				return {
					'NumpadDecimal': this.decimalPoint
				}
			},

			wrapperComponent()
			{
				return this.currencySymbol ? 'q-input-group' : 'v-fragment'
			},

			/**
			 * Determines the size of the wrapper component,
			 * depending on whether the currency symbol is shown or not.
			 */
			wrapperSize()
			{
				return this.currencySymbol ? this.size : undefined
			},

			/**
			 * Determines the size of the main input,
			 * depending on whether the currency symbol is shown or not.
			 */
			inputSize()
			{
				return this.currencySymbol ? 'block' : this.size
			}
		},

		methods: {
			/**
			 * Get the HTML input element
			 * @return {DOMElement} HTML input element
			 */
			getInputElement()
			{
				return this?.$refs?.input?.inputRef
			},

			/**
			 * Get the cursor position
			 * @return {number} Cursor position
			 */
			getCursorPosition()
			{
				const inputElem = this.getInputElement()
				return inputElem?.selectionDirection === 'backward' ? inputElem?.selectionStart : inputElem?.selectionEnd
			},

			/**
			 * Set the cursor position
			 * @param {number} index The index to set the position at
			 */
			setCursorPosition(index)
			{
				this.getInputElement()?.setSelectionRange(index, index)
			},

			/**
			 * Called when the input value is confirmed, including focusing away
			 * @param {string} val The input value as text
			 * @param {string} decimalPoint Decimal point character used in value
			 * @param {string} thousandsSeparator Thousands separator character used in value
			 * @param {string} outputDecimalPoint Decimal point character used in output
			 * @param {string} outputThousandsSeparator Thousands separator character used in output
			 * @return {string} Value formatted as a string
			 */
			getFormattedInputValue(val, decimalPoint, thousandsSeparator, outputDecimalPoint, outputThousandsSeparator)
			{
				if (!val) return ''

				// Get minus sign
				const hasSign = val.charAt(0) === '-'

				if (hasSign && val.length === 1)
					return '0'

				// Get index where whole number starts
				const wholeNumberIndex = hasSign ? 1 : 0

				// Get index of decimal point
				const decimalPointIndex = val.indexOf(decimalPoint)

				//BEGIN: Get whole and fractional parts of the number
				let wholeNumberUnformatted = ''
				let wholeNumber = ''
				let fractionNumber = ''
				if (decimalPointIndex < 0)
					wholeNumberUnformatted = val.slice(wholeNumberIndex)
				else
				{
					wholeNumberUnformatted = val.slice(wholeNumberIndex, decimalPointIndex)
					fractionNumber = val.slice(decimalPointIndex + 1)
				}

				// Skip leading 0s to remove them from whole number value
				let idxWholeNumberUnf = 0
				while (idxWholeNumberUnf < wholeNumberUnformatted?.length)
				{
					const currentChar = wholeNumberUnformatted[idxWholeNumberUnf]

					// If digit greater than 0
					if (this.isNumericChar(currentChar) && currentChar !== '0')
						break

					idxWholeNumberUnf++
				}
				// Copy only digits to whole number value
				// Don't copy thousand separators (at this point they might be in the wrong places)
				while (idxWholeNumberUnf < wholeNumberUnformatted?.length)
				{
					const currentChar = wholeNumberUnformatted[idxWholeNumberUnf]

					// If digit
					if (this.isNumericChar(currentChar))
						wholeNumber = wholeNumber.concat(currentChar)

					idxWholeNumberUnf++
				}

				// Set whole number to 0 if it is empty
				if (wholeNumber === undefined || wholeNumber === null || wholeNumber === '')
					wholeNumber += '0'

				// Add remaining 0s to fraction number
				while (fractionNumber.length < this.maxDecimals)
					fractionNumber = fractionNumber.concat('0')
				//END: Get whole and fractional parts of the number

				// Format number
				let formattedValue = hasSign ? '-' : ''

				// Iterate the whole part of the number
				for (let idx = 0; idx < wholeNumber.length; idx++)
				{
					// Add the current character
					formattedValue += wholeNumber[idx]

					// Add thousands separators
					const wholeNumberPlaceIndex = wholeNumber.length - (idx + 1)
					if (wholeNumberPlaceIndex % 3 === 0 && wholeNumberPlaceIndex > 0 && outputThousandsSeparator)
						formattedValue += outputThousandsSeparator
				}
				// If there are decimal digits, add the decimal point and digits
				if (decimalPointIndex >= 0 || fractionNumber?.length > 0)
					formattedValue += outputDecimalPoint + fractionNumber

				// Remove decimal point if it's not necessary
				if (formattedValue.charAt(formattedValue?.length - 1) === outputDecimalPoint)
					formattedValue = formattedValue.slice(0, -1)

				// Remove negative sign if the number is 0
				if (formattedValue.charAt(0) === '-' && parseInt(wholeNumber) === 0 && (parseInt(fractionNumber) === 0 || fractionNumber?.length === 0))
					formattedValue = formattedValue.slice(1)

				return formattedValue
			},

			/**
			 * Validate if the character is a number
			 * @param {string} value Character
			 * @return {boolean} Whether the value is valid numeric character
			 */
			isNumericChar(value)
			{
				return !isNaN(parseInt(value))
			},

			/**
			 * Validate character
			 * @param {string} value Character
			 * @return {boolean} Whether the value is valid
			 */
			isValidChar(value)
			{
				return this.isNumericChar(value) || value === '-' || value === this.decimalPoint || value === this.thousandsSeparator
			},

			/**
			 * Validate text being input
			 * @param {string} value Text value
			 * @return {boolean} Whether the value is valid
			 */
			validateInput(value)
			{
				let currentChar = null

				// Iterate string value
				for (let idx = 0; idx < value?.length; idx++)
				{
					currentChar = value[idx]
					// Check for characters that are not valid
					if (!this.isValidChar(currentChar))
						return false
				}

				// All characters are valid
				return true
			},

			/**
			 * Validate whole text value
			 * @param {string} value Text value
			 * @return {boolean} Whether the value is valid
			 */
			validateValue(value)
			{
				let decimalPointFound = false
				let wholeNumberCount = 0
				let fractionNumberCount = 0
				let currentChar = null

				// Iterate string value
				for (let idx = 0; idx < value?.length; idx++)
				{
					currentChar = value[idx]

					// Count number of digits
					if (this.isNumericChar(currentChar))
					{
						if (decimalPointFound)
							fractionNumberCount++
						else
							wholeNumberCount++
					}

					// Check that negative sign only appears once
					if (idx > 0 && currentChar === '-')
						return false

					// Check that decimal point only appears once for floats and not at all for integers
					if (currentChar === this.decimalPoint)
					{
						if (this.maxDecimals === 0 || decimalPointFound)
							return false
						decimalPointFound = true
					}

					// Check that there are no thousand separators after the decimal point
					if (decimalPointFound && currentChar === this.thousandsSeparator)
						return false

					// Check for other characters that are not valid
					if (!this.isValidChar(currentChar))
						return false
				}

				// Check if number of whole number or fractional number digits is greater than the maximum allowed
				if (wholeNumberCount > this.maxIntegers || fractionNumberCount > this.maxDecimals)
					return false

				return true
			},

			/**
			 * Get value with text input added in
			 * @param {string} value Current text
			 * @param {string} input New text being input
			 * @param {number} selectStart Selection start index
			 * @param {number} selectEnd Selection end index
			 * @return {boolean} Whether the value with the new input is valid
			 */
			getValueWithInput(currentValue, input, selectStart, selectEnd)
			{
				if (typeof currentValue !== 'string')
					currentValue = ''

				if (typeof input !== 'string')
					input = ''

				// Get the value with the input added
				return currentValue.substring(0, selectStart) + input + currentValue.substring(selectEnd)
			},

			/**
			 * Validate value with text input
			 * @param {string} value Current text
			 * @param {string} input New text being input
			 * @param {number} selectStart Selection start index
			 * @param {number} selectEnd Selection end index
			 * @return {boolean} Whether the value with the new input is valid
			 */
			validateValueWithInput(currentValue, input, selectStart, selectEnd)
			{
				if (typeof currentValue !== 'string')
					currentValue = ''

				if (typeof input !== 'string')
					input = ''

				// Get the value with the input added
				const newValue = this.getValueWithInput(currentValue, input, selectStart, selectEnd)

				// Validate the value with the input added
				return this.validateValue(newValue)
			},

			/**
			 * Validate value with text input
			 * @param {string} input New text being input
			 * @return {boolean} Whether the value with the new input is valid
			 */
			validateControlValueWithInput(input)
			{
				if (typeof input !== 'string')
					input = ''

				// Get current control value
				const currentValue = this.getInputElement().value

				// Get selection range
				const selectStart = this.getInputElement()?.selectionStart
				const selectEnd = this.getInputElement()?.selectionEnd

				// Validate the value with the input added
				return this.validateValueWithInput(currentValue, input, selectStart, selectEnd)
			},

			/**
			 * Given the offset in an value value, get the corresponding offset in the formatted value
			 * @param {string} value Text value
			 * @param {string} formattedValue Text value after being formatted
			 * @param {number} offset Offset in the text value
			 * @return {number} Offset in the formatted value
			 */
			getFormattedValueOffset(value, formattedValue, offset)
			{
				// The offset, ignoring thousands separators and leading 0s
				let numberValueOffset = 0
				let numberFormattedValueOffset = 0
				let afterLeadZeros = false
				let currentChar = null

				// Prevent going past the end of the value
				if (offset > value?.length)
					offset = value.length

				// Iterate value to the given offset
				for (let idx = 0; idx < offset; idx++)
				{
					currentChar = value[idx]

					// Check whether the current position is after the leading 0s
					if ((this.isNumericChar(currentChar) && currentChar !== '0')
						|| (currentChar === '0' && idx < offset - 1 && value[idx + 1] === this.decimalPoint)
						|| currentChar === this.decimalPoint)
						afterLeadZeros = true

					// Count valid characters, except for the thousands separators and leading 0s
					if (this.isValidChar(currentChar) && currentChar !== this.thousandsSeparator
						&& (currentChar !== '0' || afterLeadZeros))
						numberValueOffset++

					// If there are no whole number digits, count the 0 that gets added
					if (currentChar === this.decimalPoint && (idx === 0 || !this.isNumericChar(value[idx - 1])))
						numberValueOffset++
				}

				let formattedValueIndex = 0
				// Iterate formatted value until the same number of numeric characters are found,
				// ignoring thousands separators and leading 0s
				while (formattedValueIndex < formattedValue?.length && numberFormattedValueOffset < numberValueOffset)
				{
					currentChar = formattedValue[formattedValueIndex]

					// Count valid characters, except for the thousands separators and leading 0s
					if (this.isValidChar(currentChar) && currentChar !== this.thousandsSeparator)
						numberFormattedValueOffset++

					formattedValueIndex++
				}

				return formattedValueIndex
			},

			/**
			 * Initialize internal properties when starting input. Must be called first any time input happens.
			 */
			initForInput()
			{
				// FOR: OVERRIDE INPUT KEY
				// Reset to avoid conflicts with other input
				this.overrideInputKey = null

				// Prevent the update from changing the control value, which moves the cursor to the end of the control
				this.updateControlOnChange = false
			},

			/**
			 * Keydown handler for the input
			 * @param {object} event The event
			 */
			onKeydown(event)
			{
				// Initialize internal properties before input
				this.initForInput()

				// Get the key
				const key = event?.key

				// Get the key code
				const keyCode = event?.code

				// Get the input value of the key or null if it doesn't produce output
				let input = key?.length === 1 && event?.ctrlKey === false && event?.altKey === false ? key : null

				// FOR: OVERRIDE INPUT KEY
				// Whether the key pressed will be overridden
				const keyCodeOverride = this.keyCodeOverrides[keyCode]
				if (keyCodeOverride)
					input = keyCodeOverride

				// Get selection range
				let selectStart = this.getInputElement()?.selectionStart
				let selectEnd = this.getInputElement()?.selectionEnd

				const currentValue = this.getInputElement().value

				let isValid = false

				// If key pressed does not produce output
				if (!input)
				{
					// Delete key only and not at the end
					if (key === 'Delete' && event?.ctrlKey === false && event?.altKey === false && selectEnd < currentValue?.length)
					{
						// Nothing selected, select the character after the cursor
						// so the right character is spliced out to check the new value
						if (selectStart === selectEnd)
							selectEnd++
					}
					// Backspace key only and not at the beginning
					else if (key === 'Backspace' && event?.ctrlKey === false && event?.altKey === false && selectStart > 0)
					{
						// Nothing selected, select the character before the cursor
						// so the right character is spliced out to check the new value
						if (selectStart === selectEnd)
							selectStart--
					}
					else
						return
				}

				// If the key produces output, validate what the new value would be with this output included
				isValid = this.validateValueWithInput(currentValue, input, selectStart, selectEnd)

				// If resulting value is invalid, prevent operation
				if (!isValid)
					event.preventDefault()

				// FOR: OVERRIDE INPUT KEY
				// Ex.: If the decimal point on the number pad was pressed
				else if (keyCodeOverride)
					this.overrideInputKey = keyCodeOverride
			},

			/**
			 * Cut handler for input
			 * @param {object} event The event
			 */
			onCut(event)
			{
				// Initialize internal properties before input
				this.initForInput()

				const isValid = this.validateControlValueWithInput('')

				// If resulting value is invalid, prevent operation
				if (!isValid)
					event.preventDefault()
			},

			/**
			 * Paste handler for input
			 * @param {object} event The event
			 */
			onPaste(event)
			{
				// Initialize internal properties before input
				this.initForInput()

				const input = event.clipboardData.getData('Text')

				const isValid = this.validateControlValueWithInput(input)

				// If resulting value is invalid, prevent operation
				if (!isValid)
					event.preventDefault()
			},

			/**
			 * Drop handler for input
			 * @param {object} event The event
			 */
			onDrop(event)
			{
				// Prevent drop
				event.preventDefault()
			},

			/**
			 * Beforeinput handler for the input. Called before the control input event.
			 * @param {object} event The event
			 */
			onBeforeInput(event)
			{
				let input = event?.data

				// FOR: OVERRIDE INPUT KEY
				if (this.overrideInputKey)
					input = this.overrideInputKey

				// Validate text being input
				const isValid = this.validateControlValueWithInput(input)

				// If text being input is invalid, prevent operation
				if (!isValid)
					event.preventDefault()
			},

			/**
			 * Input handler for the input. Called by the control input event.
			 * @param {object} event The event
			 */
			onInput(event)
			{
				// FOR: OVERRIDE INPUT KEY
				// If the decimal point on the number pad was pressed
				if (this.overrideInputKey)
				{
					const input = this.overrideInputKey

					// Reset so it does not get applied to any other input after this
					this.overrideInputKey = null

					// Get selection range
					const selectStart = this.getInputElement()?.selectionStart
					const selectEnd = this.getInputElement()?.selectionEnd

					const currentValue = this.getInputElement().value

					// Get the value with the overrideInputKey replacing the key that was pressed
					const newValue = this.getValueWithInput(currentValue, input, selectStart - 1, selectEnd)

					// Set the vaue of the input, using the overrideInputKey character, and set the cursor position
					this.getInputElement().value = newValue
					this.setCursorPosition(selectStart)

					event.preventDefault()

					// Trigger input event since this should normally happen when changing the value
					const newEvent = new InputEvent('input', { 'data': this.overrideInputKey })
					this.getInputElement().dispatchEvent(newEvent)
				}
			},

			/**
			 * Change handler for the input. Called when the input value is confirmed, including focusing away.
			 */
			onChange()
			{
				// Prevent the update from changing the control value, which moves the cursor to the end of the control
				this.updateControlOnChange = false

				// Get the cursor position
				let cursorPosition = this.getCursorPosition()

				// Get formatted value
				const formattedValue = this.getFormattedInputValue(this.getInputElement().value, this.decimalPoint, this.thousandsSeparator, this.decimalPoint, this.thousandsSeparator)

				// Get new cursor position, accounting for difference in formatting
				cursorPosition = this.getFormattedValueOffset(this.getInputElement().value, formattedValue, cursorPosition)

				// Set the input value
				this.getInputElement().value = formattedValue

				// Set the cursor position
				this.setCursorPosition(cursorPosition)

				this.emitModelValue()
			},

			/**
			 * Update the control value
			 */
			updateControlValue()
			{
				// Set control value
				this.getInputElement().value = this.getFormattedInputValue(this.modelValue?.toString(), '.', null, this.decimalPoint, this.thousandsSeparator)

				// Confirm the value so it is kept when the update hook is triggered
				// which happens when changing form modes
				const event = new Event('input')
				this.getInputElement().dispatchEvent(event)
			},

			/**
			 * Emit the model value
			 */
			emitModelValue()
			{
				// Get formatted string value
				const strValue = this.getFormattedInputValue(this.getInputElement().value, this.decimalPoint, this.thousandsSeparator, '.', null)

				// Convert to number
				// If strValue is empty "" it means the user
				// has cleared the field value, thus it should translate to 0
				let value = 0
				if(strValue) value = this.maxDecimals === 0 ? parseInt(strValue) : parseFloat(strValue)

				if (!isNaN(value))
					this.$emit('update:modelValue', value)
			}
		},

		watch: {
			modelValue()
			{
				// Control whether to update the control value
				// Prevented when the user is changing the value in the control
				// to prevent the cursor from jumping to the end of the control
				if (this.updateControlOnChange)
				{
					// Set control value
					this.updateControlValue()
				}
				else
					this.updateControlOnChange = true
			}
		}
	}
</script>
