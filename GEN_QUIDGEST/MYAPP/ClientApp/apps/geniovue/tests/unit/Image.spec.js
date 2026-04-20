import { nextTick } from 'vue'
import { fireEvent } from '@testing-library/vue'
import { flushPromises } from '@vue/test-utils'
import { vi } from 'vitest'

import { render } from './utils'
import QImage from '@/components/inputs/image/QImage.vue'
import fakeData from '../cases/Image.mock.js'

const factory = (customProps = {}) =>
	render(QImage, {
		props: {
			height: 400,
			width: 300,
			...customProps
		}
	})

describe('QImage.vue', () => {
	it('Checks that, when no image is passed, delete and edit buttons are hidden.', () => {
		const { queryByTestId } = factory()

		const editButton = queryByTestId('edit-btn')
		const deleteButton = queryByTestId('delete-btn')
		expect(editButton).toBeNull()
		expect(deleteButton).toBeNull()
	})

	it('Checks that, when "disabled" is true, buttons are disabled.', () => {
		const { getByTestId } = factory({ image: fakeData.image, disabled: true })

		const submitButton = getByTestId('submit-btn')
		const deleteButton = getByTestId('delete-btn')
		expect(submitButton).toHaveProperty('disabled')
		expect(deleteButton).toHaveProperty('disabled')
	})

	it('Checks that, when "readonly" is true, buttons are hidden.', () => {
		const { queryByTestId } = factory({ image: fakeData.image, readonly: true })

		const submitButton = queryByTestId('submit-btn')
		const deleteButton = queryByTestId('delete-btn')
		expect(submitButton).toBeNull()
		expect(deleteButton).toBeNull()
	})

	it('Checks that, when an image is selected, the "submit-image" event is emitted.', async () => {
		const wrapper = factory({ image: fakeData.image })
		const submitButton = wrapper.getByTestId('submit-btn')
		await fireEvent.click(submitButton)

		// Simulate image upload
		const input = wrapper.getByTestId('file-input')
		const file = new File([new ArrayBuffer(1)], 'file.jpg')
		// Supposedly you could replace the lines below with upload(), but it does not work
		Object.defineProperty(input, 'files', { value: [file] })
		await fireEvent.update(input)

		await flushPromises()
		await vi.dynamicImportSettled()

		const image = await wrapper.asyncEmitted('submit-image')

		expect(image).toBeTruthy()
	})

	it('Checks that, when the image is clicked, the "open-image-preview" event is emitted.', async () => {
		const wrapper = factory({ image: fakeData.image })
		const previewImage = wrapper.getByTestId('main-img')
		await fireEvent.click(previewImage)
		await nextTick()
		expect(wrapper.emitted('open-image-preview')).toBeTruthy()
	})
})
