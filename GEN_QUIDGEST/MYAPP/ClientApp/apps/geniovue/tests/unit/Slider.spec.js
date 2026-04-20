import '@testing-library/jest-dom'
import { fireEvent } from '@testing-library/vue'
import { render } from './utils'

import Slider from '@/components/rendering/QSliderUI.vue'

describe('Slider', () => {
	it('renders correct input based on props', async () => {
		const wrapper = render(Slider, {
			props: {
				inputType: 'def'
			}
		})

		let display = await wrapper.findAllByRole('value-display')
		expect(display.length).toBe(1)
		expect(display[0].getAttribute('class')).toBe('numeric-input-type')

		await wrapper.rerender({ inputType: 'sec' })
		display = await wrapper.findAllByRole('value-display')
		expect(display.length).toBe(1)
		expect(display[0].getAttribute('class')).toBe('secondary-input-type')

		await wrapper.rerender({ inputType: 'box' })
		display = await wrapper.findAllByRole('value-display')
		expect(display.length).toBe(1)
		expect(display[0].getAttribute('class')).toBe('box-moving-container')
	})

	it('updates sliderValue when slider is moved', async () => {
		const wrapper = render(Slider, {
			props: {
				minValue: 0,
				maxValue: 100
			}
		})

		const slider = await wrapper.findByRole('slider-input')
		await fireEvent.update(slider, 100)
		expect(slider).toHaveValue('100')
		expect(wrapper).toEmitModelValue(100)
		await fireEvent.update(slider, 50)
		expect(slider).toHaveValue('50')
		expect(wrapper).toEmitModelValue(50)
		await fireEvent.update(slider, 1)
		expect(slider).toHaveValue('1')
		expect(wrapper).toEmitModelValue(1)
		await fireEvent.update(slider, 97)
		expect(slider).toHaveValue('97')
		expect(wrapper).toEmitModelValue(97)
	})

	it('implements min and max values correctly', async () => {
		const wrapper = render(Slider, {
			props: {
				minValue: 4,
				maxValue: 20
			}
		})

		const slider = await wrapper.findByRole('slider-input')
		expect(slider.getAttribute('min')).toBe('4')
		expect(slider.getAttribute('max')).toBe('20')
		await fireEvent.update(slider, 200)
		expect(slider).toHaveValue('20')
		expect(wrapper).toEmitModelValue(20)
		await fireEvent.update(slider, 0)
		expect(slider).toHaveValue('4')
		expect(wrapper).toEmitModelValue(4)
	})

	it('shows limits if showLimits prop is set to true', async () => {
		const wrapper = render(Slider, {
			props: {
				showLimits: false
			}
		})

		let limits = await wrapper.queryByRole('display-limits')
		expect(limits).toBeNull()

		await wrapper.rerender({ showLimits: true })
		limits = await wrapper.queryByRole('display-limits')
		expect(limits).not.toBeNull()
	})
})
