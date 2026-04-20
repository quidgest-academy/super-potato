import '@testing-library/jest-dom'
import { render } from '@testing-library/vue'

import fakeData from '../cases/MarkdownViewer.mock.js'
import QMarkdownViewer from '@/components/markdown/QMarkdownViewer.vue'
import { MarkdownOptions } from '@/components/markdown/markdown.js'

describe('QMarkdownViewer.vue', () => {
	it('Checks if it renders Markdown with HTML', async () => {
		const data = fakeData.simpleUsage()
		const wrapper = await render(QMarkdownViewer, {
			props: {
				id: 'Test',
				modelValue: data.simpleMarkdown,
				options: new MarkdownOptions({
					enableTypographer: false
				})
			}
		})

		expect(wrapper.html()).toBe(data.simpleMarkdownHtmlResult)
	})
})