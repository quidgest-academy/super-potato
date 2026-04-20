import { fireEvent } from '@testing-library/vue'
import { nextTick } from 'vue'

import { mount } from './utils'

import QKeyboardInput from '@/components/rendering/QKeyboardInput.vue'

describe('WhiteBox Testing - QKeyboardInput', () => {
	let wrapper

	beforeEach(() => {
		wrapper = mount(QKeyboardInput)
	})

	it('closes popup when clicked outside', async () => {
		// Assuming there's a method to open the popup, we open it first
		wrapper.vm.togglePopupOn()
		await nextTick()

		// Confirm that the popup is open
		expect(wrapper.vm.popupTrigger).toBe(true)

		// Simulate mousedown event. 
		await fireEvent.mouseDown(document)
		await nextTick()

		// Check that the popup has closed
		expect(wrapper.vm.popupTrigger).toBe(false)
	})

	it('emits correct value when digit button is clicked', async () => {
		await wrapper.setData({ popupTrigger: true })
		await nextTick() // Waiting for the DOM to update

		await wrapper.find('.q-keyboard__digit').trigger('mousedown') // Clicking the first digit, which is 1

		expect(wrapper).toEmitModelValue(1).toBeTruthy()
		expect(wrapper).toEmitModelValue(1)
	})


	it('resets value when "AC" button is clicked', async () => {
		await wrapper.setData({ popupTrigger: true })
		await nextTick()

		await wrapper.find('.q-keyboard--cancel').trigger('mousedown')
		await nextTick()
		expect(wrapper).toEmitModelValue(0)
	})

	it('removes last digit when "backspace" button is clicked', async () => {
		const wrapper = mount(QKeyboardInput)

		await wrapper.vm.togglePopupOn() // Simulate interaction to show the keyboard.
		await nextTick()

		const digits = wrapper.findAll('.q-keyboard__digit')

		if (digits.length === 0)
			throw new Error('No .digit elements found!')

		await digits.at(1).trigger('mousedown') // Click '2'
		await digits.at(1).trigger('mousedown') // Click '2' again

		await nextTick()
		expect(wrapper).toEmitModelValue(22) // Expect '22' after clicking '2' twice

		await wrapper.find('.q-keyboard--backspace').trigger('mousedown') // Click 'backspace'
		expect(wrapper).toEmitModelValue(2) // Expect '2' after removing last digit
	})

	it('keeps current value when "Enter" button is clicked twice', async () => {
		// Set inputData to represent '123'
		await wrapper.setData({ popupTrigger: true, inputData: ['1', '2', '3'] })
		await nextTick() // Waiting for the DOM to update

		// Simulate Enter key press
		await wrapper.find('.q-keyboard--check-circle').trigger('mousedown') // Click 'Enter'
		expect(wrapper).toEmitModelValue(123) // Expect '123' after entering '123'

		// Set inputData back to represent '123' again
		await wrapper.setData({ inputData: ['1', '2', '3'] })
		await nextTick() // Waiting for the DOM to update

		// Simulate Enter key press again
		await wrapper.find('.q-keyboard--check-circle').trigger('mousedown') // Click 'Enter' again
		expect(wrapper).toEmitModelValue(123) // Expect '123' to be unchanged
	})
})

describe('BlackBox Testing - QKeyboardInput.vue', () => {
	let wrapper

	beforeEach(() => {
		wrapper = mount(QKeyboardInput, {
			propsData: {
				modelValue: 0,
			},
			global: {
				components: {
					QKeyboardInputPopup: {
						template: '<div></div>'
					}
				}
			}
		})
	})

	it('renders', () => {
		expect(wrapper.exists()).toBe(true)
	})

	it('handles togglePopupOn method', async () => {
		await wrapper.vm.togglePopupOn()
		expect(wrapper.vm.popupTrigger).toBe(true)
	})

	it('handles togglePopupOff method', async () => {
		await wrapper.vm.togglePopupOff()
		expect(wrapper.vm.popupTrigger).toBe(false)
	})

	it('handles focus method', async () => {
		await wrapper.vm.focus()
		expect(wrapper.vm.popupTrigger).toBe(true)
	})
})
