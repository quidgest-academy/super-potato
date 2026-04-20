import '@testing-library/jest-dom'
import { render } from './utils'
import { waitFor } from '@testing-library/vue'
import userEvent from '@testing-library/user-event'

import NumericInput from '@/components/inputs/NumericInput.vue'
import { expect } from 'vitest'

//--------------
// Using userEvent instead of fireEvent because of the way NumericInput was made before.
// NumericInput was using 'keyCode' and 'which' properties of KeyboardEvent that are deprecated
// see: https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/keyCode
// fireEvent sends these as 0 and will fail tests that should otherwise work

describe('NumericInput.vue', () => {
	it('render the model value', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 123
			}
		})
		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('123')
	})

	it('text characters or symbols are not allowed', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 234,
				thousandsSeparator: ' ',
				maxDecimals: 0
			}
		})

		const numericInput = await wrapper.findByRole('textbox')
		await userEvent.type(numericInput, 'abc')

		expect(numericInput).toHaveValue('234')
	})

	it('do not allow more than the maximum number of whole number and fractional digits', async () => {
		const wrapper = render(NumericInput, {
			props: {
				maxIntegers: 5,
				maxDecimals: 3,
				decimalPoint: '.',
				thousandsSeparator: ''
			}
		})
		const numericInput = await wrapper.findByRole('textbox')
		await userEvent.type(numericInput, '1234567890.12345')

		expect(numericInput).toHaveValue('12345.123')
	})

	it('verify that the number has thousand separators and a decimal point', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 123456789.12,
				maxIntegers: 9,
				maxDecimals: 2,
				decimalPoint: '.',
				thousandsSeparator: ','
			}
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('123,456,789.12')
	})

	it('verify that the number cannot have more than one decimal point', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 12345678.12,
				maxIntegers: 9,
				maxDecimals: 2,
				decimalPoint: '.',
				thousandsSeparator: ','
			}
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('12,345,678.12')

		// Set the cursor position
		await userEvent.pointer([{ target: numericInput, offset: 9, keys: '[MouseLeft]' }])

		await userEvent.keyboard('{.}')

		// Focus away to trigger the auto-correct
		await numericInput.blur()

		expect(numericInput).toHaveValue('12,345,678.12')
	})

	it('verify that the control allows negative numbers', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: -1234,
				maxIntegers: 9,
				maxDecimals: 0,
				decimalPoint: '.',
				thousandsSeparator: ','
			}
		})
		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('-1,234')
	})

	it('verify that the number can have the sign typed in the first position', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 12345678.12,
				maxIntegers: 9,
				maxDecimals: 2,
				decimalPoint: '.',
				thousandsSeparator: ','
			}
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('12,345,678.12')

		// Set the cursor position
		await userEvent.pointer([{ target: numericInput, offset: 0, keys: '[MouseLeft]' }])

		await userEvent.keyboard('{-}')

		// Focus away to trigger the auto-correct
		await numericInput.blur()

		expect(numericInput).toHaveValue('-12,345,678.12')
	})

	it('verify that the number cannot have the sign in any position other than the first', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 12345678.12,
				maxIntegers: 9,
				maxDecimals: 2,
				decimalPoint: '.',
				thousandsSeparator: ','
			}
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('12,345,678.12')

		// Set the cursor position
		await userEvent.pointer([{ target: numericInput, offset: 1, keys: '[MouseLeft]' }])

		await userEvent.keyboard('{-}')

		// Focus away to trigger the auto-correct
		await numericInput.blur()

		expect(numericInput).toHaveValue('12,345,678.12')
	})

	it('verify that the number cannot have more than one sign, even at the beginning', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: -12345678.12,
				maxIntegers: 9,
				maxDecimals: 2,
				decimalPoint: '.',
				thousandsSeparator: ','
			}
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('-12,345,678.12')

		// Set the cursor position
		await userEvent.pointer([{ target: numericInput, offset: 0, keys: '[MouseLeft]' }])

		await userEvent.keyboard('{-}')

		// Focus away to trigger the auto-correct
		await numericInput.blur()

		expect(numericInput).toHaveValue('-12,345,678.12')
	})

	it('verify that pasting a valid number works', async () => {
		const wrapper = render(NumericInput, {
			props: {
				maxIntegers: 9,
				maxDecimals: 3,
				decimalPoint: '.',
				thousandsSeparator: ','
			},
		})

		const numericInput = await wrapper.findByRole('textbox')
		await numericInput.focus()
		await userEvent.paste('123456789.12')
		// Focus away to trigger the auto-correct which will add the remaining decimal digit 0s
		await numericInput.blur()

		expect(numericInput).toHaveValue('123,456,789.120')
	})

	it('verify that pasting an invalid number is prevented', async () => {
		const wrapper = render(NumericInput, {
			props: {
				maxIntegers: 9,
				maxDecimals: 3,
				decimalPoint: '.',
				thousandsSeparator: ','
			},
		})

		const numericInput = await wrapper.findByRole('textbox')
		await numericInput.focus()
		await userEvent.paste('123456.12345')
		// Focus away to trigger the auto-correct which will add the remaining decimal digit 0s
		await numericInput.blur()

		expect(numericInput).toHaveValue('')
	})

	it('verify that pasting over a selection that results in a valid number works', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 12345678.12,
				maxIntegers: 9,
				maxDecimals: 3,
				decimalPoint: '.',
				thousandsSeparator: ','
			},
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('12,345,678.120')
		await numericInput.focus()
		// Select a range of text
		await userEvent.pointer([{ target: numericInput, offset: 9, keys: '[MouseLeft>]' }, { offset: 12 }])
		await userEvent.paste('1.9')
		// Focus away to trigger the auto-correct
		await numericInput.blur()

		expect(numericInput).toHaveValue('12,345,671.920')
	})

	it('verify that pasting over a selection that results in an invalid number is prevented', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 12345678.12,
				maxIntegers: 9,
				maxDecimals: 3,
				decimalPoint: '.',
				thousandsSeparator: ','
			},
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('12,345,678.120')
		await numericInput.focus()
		// Select a range of text
		await userEvent.pointer([{ target: numericInput, offset: 9, keys: '[MouseLeft>]' }, { offset: 12 }])
		await userEvent.paste('1.98')
		// Focus away to trigger the auto-correct
		await numericInput.blur()

		expect(numericInput).toHaveValue('12,345,678.120')
	})

	it('verify that deleting a selection that results in a valid number works', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 12345678.12,
				maxIntegers: 9,
				maxDecimals: 3,
				decimalPoint: '.',
				thousandsSeparator: ','
			},
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('12,345,678.120')
		await numericInput.focus()
		// Select a range of text
		await userEvent.pointer([{ target: numericInput, offset: 9, keys: '[MouseLeft>]' }, { offset: 13 }])
		await userEvent.keyboard('{Delete}')
		// Focus away to trigger the auto-correct
		await numericInput.blur()

		expect(numericInput).toHaveValue('12,345,670.000')
	})

	it('verify that deleting a selection that results in an invalid number is prevented', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 12345678.12,
				maxIntegers: 9,
				maxDecimals: 3,
				decimalPoint: '.',
				thousandsSeparator: ','
			},
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('12,345,678.120')
		await numericInput.focus()
		// Select a range of text
		await userEvent.pointer([{ target: numericInput, offset: 9, keys: '[MouseLeft>]' }, { offset: 11 }])
		await userEvent.keyboard('{Delete}')
		// Focus away to trigger the auto-correct
		await numericInput.blur()

		expect(numericInput).toHaveValue('12,345,678.120')
	})

	it('verify that cutting a selection that results in a valid number works', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 12345678.12,
				maxIntegers: 9,
				maxDecimals: 3,
				decimalPoint: '.',
				thousandsSeparator: ','
			},
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('12,345,678.120')
		await numericInput.focus()
		// Select a range of text
		await userEvent.pointer([{ target: numericInput, offset: 9, keys: '[MouseLeft>]' }, { offset: 13 }])
		await userEvent.cut()
		// Focus away to trigger the auto-correct
		await numericInput.blur()

		expect(numericInput).toHaveValue('12,345,670.000')
	})

	it('verify that cutting a selection that results in an invalid number is prevented', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 12345678.12,
				maxIntegers: 9,
				maxDecimals: 3,
				decimalPoint: '.',
				thousandsSeparator: ','
			},
		})

		const numericInput = await wrapper.findByRole('textbox')
		expect(numericInput).toHaveValue('12,345,678.120')
		await numericInput.focus()
		// Select a range of text
		await userEvent.pointer([{ target: numericInput, offset: 9, keys: '[MouseLeft>]' }, { offset: 11 }])
		await userEvent.cut()
		// Focus away to trigger the auto-correct
		await numericInput.blur()

		expect(numericInput).toHaveValue('12,345,678.120')
	})

	it('verify cursor position after adding a positive integer value', async () => {
		const wrapper = render(NumericInput, {
			props: {
				maxIntegers: 9,
				maxDecimals: 0,
				decimalPoint: '.',
				thousandsSeparator: ''
			}
		})
		const numericInput = await wrapper.findByRole('textbox')

		await userEvent.type(numericInput, '12345')

		expect(numericInput).toHaveValue('12345')
		expect(numericInput.selectionStart).toBe(5)
	})

	it('verify that pressing the number pad decimal point key inputs the defined decimal point \
		and that the auto-correct happens when confirming the value, keeping the cursor in the right place', async () => {
		const wrapper = render(NumericInput, {
			props: {
				maxIntegers: 5,
				maxDecimals: 2,
				decimalPoint: ',',
				thousandsSeparator: ' '
			}
		})
		const numericInput = await wrapper.findByRole('textbox')

		// Type whole number digits
		await userEvent.type(numericInput, '12345')

		// Press the number pad decimal point key
		// This has to be done by the keyboard method using this custom keyboard map
		// because the default map doesn't implement number pad keys
		await userEvent.keyboard('[NumpadDecimal]', { keyboardMap: [{ code: 'NumpadDecimal', key: '.' }] })

		// Type fractional digits
		await userEvent.type(numericInput, '67')

		// Press enter to confirm the value and trigger auto-correct
		await userEvent.keyboard('{Enter}')

		// Focus away to confirm the value and trigger auto-correct
		numericInput.blur()

		// Wait for the auto-correct to run, which requires a small amount of time
		// Checking the value without waiting will cause the test to fail
		await waitFor(() =>
		{ // wait for this function to not throw an error
			expect(numericInput).toHaveDisplayValue(['12 345,67'])
		}, { container: document.body, timeout: 100 })
	})

	it('verify that selecting some text and pressing the number pad decimal point key inputs the defined decimal point, replacing the text, \
		and that the auto-correct happens when confirming the value, keeping the cursor in the right place', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 12345.67,
				maxIntegers: 5,
				maxDecimals: 2,
				decimalPoint: ',',
				thousandsSeparator: ' '
			}
		})
		const numericInput = await wrapper.findByRole('textbox')

		// Type whole number digits
		await userEvent.pointer([{ target: numericInput, offset: 5, keys: '[MouseLeft>]' }, { offset: 7 }])

		// Press the number pad decimal point key
		// This has to be done by the keyboard method using this custom keyboard map
		// because the default map doesn't implement number pad keys
		await userEvent.keyboard('[NumpadDecimal]', { keyboardMap: [{ code: 'NumpadDecimal', key: '.' }] })

		// Press enter to confirm the value and trigger auto-correct
		await userEvent.keyboard('{Enter}')

		// Focus away to confirm the value and trigger auto-correct
		numericInput.blur()

		// Wait for the auto-correct to run, which requires a small amount of time
		// Checking the value without waiting will cause the test to fail
		await waitFor(() => { // wait for this function to not throw an error
			expect(numericInput).toHaveDisplayValue(['1 234,67'])
		}, { container: document.body, timeout: 100 })
	})

	it('should emit 0 when the field value is cleared', async () => {
		const wrapper = render(NumericInput, {
			props: {
				modelValue: 1234
			}
		})

		const numericInput = await wrapper.findByRole('textbox')
		
		await userEvent.clear(numericInput)
		await numericInput.blur()

		const emitted = wrapper.emitted()
	
		// Wait for the auto-correct to run, which requires a small amount of time
		// Checking the value without waiting will cause the test to fail
		await waitFor(() => { // wait for this function to not throw an error
			expect(emitted['update:modelValue'][0]).toEqual([0])
		}, { container: document.body, timeout: 100 })
	})
})
