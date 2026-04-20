import '@testing-library/jest-dom'
import { shallowMount } from './utils'

import TabContainer from '@/components/containers/TabContainer.vue'

describe('TabContainer.vue', () => {
	let wrapper
	const alignValueLeft = 'left'
	const iconAlignmentValue = 'right'

	beforeEach(() => {
		wrapper = shallowMount(TabContainer, {
			props: {
				selectedTab: 1,
				
				tabsList: [
					{
						id: 1,
						label: 'Tab one',
						disabled: false,
						icon:'building',
						isVisible: true
					},
					{
						id: 2,
						label: 'Tab Two',
						disabled: true,
						icon:'download',
						isVisible: false
					},
					{
						id: 3,
						label: 'Tab Three',
						disabled: false,
						icon:'book',
						isVisible: false
					},
					{
						id: 4,
						label: 'Tab Four',
						disabled: false,
						isVisible: false
					}
				],
				alignTabs: alignValueLeft,
				iconAlignment: iconAlignmentValue,
				isVisible: true
			}
		})
	})

	afterEach(() => {
		wrapper.unmount()
	})

	it('Check alignTabs property gets respective valid values', async () => {
		// Checking props getting proper values
		// Align: Left =>  justify-content-start
		expect(wrapper.exists('ul.justify-content-start')).toBeTruthy()
	})

	it('Check isVisible property gets respective valid values', async () => {
		// Checking props getting proper values
		expect(wrapper.find('div').element).not.toBeVisible()
	})
})
