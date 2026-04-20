import { mount, shallowMount } from './utils'

import QStep from '@/components/rendering/QStep.vue'

describe('WhiteBox Testing - QStep.vue', () => {
	let wrapper

	beforeEach(() => {
		wrapper = shallowMount(QStep, {
			propsData: {
				modelValue: 5,
				step: 1
			}
		})
	})

	it("decrements modelValue when 'minus' is inputted and modelValue is greater than 0", async () => {
		await wrapper.vm.inputDigit('minus')

		// trigger the setter of computedModelValue
		wrapper.vm.computedModelValue = wrapper.vm.localModelValue

		expect(wrapper).toEmitModelValue(4)
	})


	it("doesn't decrement modelValue when 'minus' is inputted and modelValue is 0", async () => {

		const factory = (propsData) => {
			return shallowMount(QStep, {
				propsData: {
					...propsData,
				},
			})
		}

		const wrapper = factory({ modelValue: 0 })
		await wrapper.vm.inputDigit('minus')
		expect(wrapper.emitted('update:modelValue')).toBeFalsy()
	})

	it("increments modelValue when 'plus' is inputted", async () => {
		await wrapper.vm.inputDigit('plus')

		// trigger the setter of computedModelValue
		wrapper.vm.computedModelValue = wrapper.vm.localModelValue

		expect(wrapper).toEmitModelValue(6)
	})

	it("doesn't change modelValue when other strings are inputted", async () => {
		await wrapper.vm.inputDigit('other')
		expect(wrapper.emitted('update:modelValue')).toBeFalsy()
	})
})

describe('BlackBox Testing - QStep.vue', () => {
	it('increments modelValue when plus button is clicked', async () => {
		const wrapper = mount(QStep, {
			propsData: {
				modelValue: 5,
				step: 1
			}
		})

		const buttons = wrapper.findAll('.q-step__digit') //Both minus and plus buttons have the same class
		await buttons.at(1).trigger('mousedown') //Clicks on plus button

		// trigger the setter of computedModelValue
		wrapper.vm.computedModelValue = wrapper.vm.localModelValue

		expect(wrapper).toEmitModelValue(6)
	})

	it('decrements modelValue when minus button is clicked and modelValue is greater than 0', async () => {
		const wrapper = mount(QStep, {
			propsData: {
				modelValue: 5,
				step: 1
			}
		})

		const minusButton = wrapper.find('.q-step__digit')
		await minusButton.trigger('mousedown')

		// trigger the setter of computedModelValue
		wrapper.vm.computedModelValue = wrapper.vm.localModelValue

		expect(wrapper).toEmitModelValue(4)
	})

	it("doesn't decrement modelValue when minus button is clicked and modelValue is 0", async () => {
		const wrapper = mount(QStep, {
			propsData: {
				modelValue: 0,
				step: 1
			}
		})

		const minusButton = wrapper.find('.q-step__digit')
		await minusButton.trigger('mousedown')

		expect(wrapper.emitted('update:modelValue')).toBeFalsy()
	})

	it("doesn't change modelValue when other string is emitted to inputDigit", async () => {
		const wrapper = mount(QStep, {
			propsData: {
				modelValue: 5,
				step: 1
			}
		})

		// we directly call the internal method here to simulate a wrong input
		// while this might seem like white box testing, it can be argued that this still simulates a user behavior
		// (although it's unlikely to happen with proper button click handlers)
		await wrapper.vm.inputDigit('other')

		expect(wrapper.emitted('update:modelValue')).toBeFalsy()
	})
})
