// Components
import Lookup from '@/components/inputs/QLookup.vue'

// Utils
import { mount } from './utils'
import { describe, expect, it } from 'vitest'

describe('Lookup', () => {
	it('renders with default props', () => {
		const wrapper = mount(Lookup)

		// Assert that the component is rendered
		expect(wrapper.exists()).toBe(true)
	})

	it('emits on-search when input value changes', async () => {
		const wrapper = mount(Lookup)

		const input = wrapper.get('input')
		await input.setValue('Search')

		// Check if the on-search event is emitted with the correct value
		expect(wrapper.emitted('on-search')).toHaveLength(1)
		expect(wrapper.emitted('on-search')[0][0]).toBe('Search')
	})

	it('emits see-more event when "See more" button is clicked', async () => {
		const wrapper = mount(Lookup, {
			props: {
				showSeeMore: true
			}
		})

		// Find the "See more" button and click it
		const seeMoreButton = wrapper.find('button[title="View more options"]')
		await seeMoreButton.trigger('click')

		// Check if the see-more event is emitted
		expect(wrapper.emitted('see-more')).toHaveLength(1)
	})

	it('does not emit view-details event when "View details" button is clicked and there is no selected value', async () => {
		const wrapper = mount(Lookup, {
			props: {
				showViewDetails: true
			}
		})

		// Find the "View details" button and click it
		const viewDetailsButton = wrapper.find('button[title="View details"]')
		await viewDetailsButton.trigger('click')

		// Check if the view-details event is not emitted
		expect(wrapper.emitted('view-details')).toBeUndefined()
	})

	it('emits view-details event with the current value when "View details" button is clicked', async () => {
		const wrapper = mount(Lookup, {
			props: {
				modelValue: 'key',
				showViewDetails: true,
				items: [{ value: 'key', label: 'value' }]
			}
		})

		// Find the "View details" button and click it
		const viewDetailsButton = wrapper.find('button[title="View details"]')
		await viewDetailsButton.trigger('click')

		// Check if the view-details event is emitted with the correct value
		expect(wrapper.emitted('view-details')).toHaveLength(1)
		expect(wrapper.emitted('view-details')[0][0]).toBe('key')
	})
})
