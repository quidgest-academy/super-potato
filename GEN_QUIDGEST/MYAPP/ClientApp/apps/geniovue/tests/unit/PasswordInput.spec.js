import { mount } from './utils'
import { expect } from 'vitest'

import QPasswordInput from '@/components/inputs/PasswordInput.vue'

describe('PasswordInput.vue', () => {
	let wrapper

	beforeEach(() => {
		wrapper = mount(QPasswordInput, {
			props: {
				modelValue: 'password'
			}
		})
	})

	it('Checks button is toggling protected mode on click', async () => {
		const input = await wrapper.find('input')
		expect(input.attributes()).toHaveProperty('type', 'password')
		const button = await wrapper.find('button')
		await button.trigger('mousedown')
		expect(input.attributes()).toHaveProperty('type', 'text')
		await button.trigger('mouseup')
		expect(input.attributes()).toHaveProperty('type', 'password')
	})
})
