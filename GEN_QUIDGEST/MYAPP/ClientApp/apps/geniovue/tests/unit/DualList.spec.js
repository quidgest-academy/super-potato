import '@testing-library/jest-dom'
import userEvent from "@testing-library/user-event"
import { fireEvent } from '@testing-library/vue'
import { render } from './utils'

import DualListComponent from '@/components/rendering/QDualList.vue'

describe('DualListComponent', () => {
	it('filters available items correctly', async () => {
		const wrapper = render(DualListComponent,
			{
				props: {
					items: [
						{ id: 1, name: 'Item 1' },
						{ id: 2, name: 'Item 2' },
						{ id: 3, name: 'Item 3' },
						{ id: 4, name: 'Item 11' },
					],
					modelValue: [2]
				}
			})

		const inputElement = await wrapper.getAllByTestId("test-filter1")
		await userEvent.type(inputElement[0], 'Item 1')
		const searchElement = await wrapper.getByTestId('test-search1')
		await fireEvent.click(searchElement)
		const lists = await wrapper.getAllByTestId('test-items')

		await expect(lists).toHaveLength(2)
		expect(lists[1].textContent).toBe('Item 11')
	})

	it('filters selected items correctly', async () => {
		const wrapper = render(DualListComponent,
			{
				props: {
					items: [
						{ id: 1, name: 'Item 1' },
						{ id: 2, name: 'Item 2' },
						{ id: 3, name: 'Item 3' },
						{ id: 4, name: 'Item 11' },
					],
					modelValue: [2]
				}
			})

		const inputElement2 = await wrapper.getAllByTestId('test-filter2')
		await userEvent.type(inputElement2[0], 'Item 2')
		const searchElement2 = await wrapper.getByTestId('test-search2')
		await fireEvent.click(searchElement2)
		const lists2 = await wrapper.getAllByTestId('test-items2')

		expect(lists2).toHaveLength(1)
		expect(lists2[0].textContent).toBe('Item 2')
	})

	it('adds all to selected items', async () => {
		const wrapper = render(DualListComponent,
			{
				props: {
					items: [
						{ id: 1, name: 'Item 1' },
						{ id: 2, name: 'Item 2' },
						{ id: 3, name: 'Item 3' },
						{ id: 4, name: 'Item 11' },
					],
					modelValue: [1]
				}
			})

		const selectAll = await wrapper.getByTestId('test-addall')
		await fireEvent.click(selectAll)
		expect(wrapper).toEmitModelValue([1, 2, 3, 4])
	})
})

describe('DualListComponent - Black Box Testing', () => {
	it('renders the provided items correctly', async () => {
		const wrapper = render(DualListComponent,
			{
				props: {
					items: [
						{ id: 1, name: 'Item 1' },
						{ id: 2, name: 'Item 2' },
						{ id: 3, name: 'Item 3' },
						{ id: 4, name: 'Item 11' },
					],
					modelValue: [2]
				}
			}
		)

		const availableItems = await wrapper.getAllByTestId('test-items')
		expect(availableItems[0].textContent).toBe('Item 1')
		expect(availableItems[1].textContent).toBe('Item 3')
	})
})
