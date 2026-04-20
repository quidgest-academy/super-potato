import '@testing-library/jest-dom'
import { nextTick } from 'vue'
import { fireEvent } from '@testing-library/vue'
import { render } from './utils'
import userEvent from '@testing-library/user-event'

import CheckListExtension from '@/components/extensions/CheckListExtension.vue'
import fakeData from '../cases/CheckListExtension.mock'

const factory = (overrides = {}) =>
	render(CheckListExtension, {
		props: {
			searchColumnName: 'Val',
			primaryKeyColumnName: 'PrimaryKey',
			options: fakeData.simpleUsage().tableTest.rows,
			rowsChecked: fakeData.simpleUsage().tableTest.rowsChecked,
			disabled: false,
			...overrides
		}
	})

describe('CheckListExtension.vue', () => {
	it('Open search when click button', async () => {
		const wrapper = factory()
		const button = wrapper.getByTitle('Insert')
		const search = wrapper.getByTitle('Search')
		expect(search.getAttribute('aria-hidden') === 'true').toBeTruthy()
		await fireEvent.click(button)
		expect(search.getAttribute('aria-hidden') === 'false').toBeTruthy()
	})

	it('Autocomplete while typing', async () => {
		const wrapper = factory()
		const button = wrapper.getByTitle('Insert')
		const search = wrapper.getByTitle('Search')
		await fireEvent.click(button)
		await userEvent.type(search, 'th')
		expect(search.value).toBe('thing')
	})

	it('Close search on click button when search is open', async () => {
		const wrapper = factory()
		const button = wrapper.getByTitle('Insert')
		const search = wrapper.getByTitle('Search')
		expect(search.getAttribute('aria-hidden') === 'true').toBeTruthy()
		await fireEvent.click(button)
		expect(search.getAttribute('aria-hidden') === 'false').toBeTruthy()
		await fireEvent.click(button)
		expect(search.getAttribute('aria-hidden') === 'true').toBeTruthy()
	})

	it('Close search on press ESC', async () => {
		const wrapper = factory()
		const button = wrapper.getByTitle('Insert')
		const search = wrapper.getByTitle('Search')
		await fireEvent.click(button)
		await userEvent.type(search, 'ja')
		await nextTick()
		await userEvent.keyboard('{Escape}')
		await nextTick()
		expect(search.getAttribute('aria-hidden') === 'true').toBeTruthy()
	})

	it('When first suggestion get satisfied next should appear', async () => {
		const wrapper = factory()
		const button = wrapper.getByTitle('Insert')
		const search = wrapper.getByTitle('Search')
		await fireEvent.click(button)
		await userEvent.type(search, 'thing')
		await nextTick()
		expect(search.value).toBe('things')
	})

	it('When disabled insert should not be visible', async () => {
		const wrapper = factory({ disabled: true })

		const insertButton = wrapper.queryByTitle('Insert')
		expect(insertButton).toBeNull()
	})
})
