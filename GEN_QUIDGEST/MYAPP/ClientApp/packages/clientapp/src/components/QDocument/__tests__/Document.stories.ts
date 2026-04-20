// Components
import { QDocument } from '../'

// Types
import type { Meta, StoryObj } from '@storybook/vue3'

/**
 * An input field for uploading files.
 */
const meta: Meta<typeof QDocument> = {
	title: 'ClientApp/Document',
	component: QDocument,
	tags: ['autodocs']
}

export default meta

type Story = StoryObj<typeof QDocument>

/**
 * The default Document input.
 */
export const Default: Story = {
	args: {
		id: 'uid-1',
		resourcesPath: 'Content/img/'
	}
}
