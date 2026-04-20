// Components
import { QActionList } from '..'

// Types
import type { Meta, StoryObj } from '@storybook/vue3'

/**
 * An overlay with a list of actions.
 */
const meta: Meta<typeof QActionList> = {
	title: 'ClientApp/Action List',
	component: QActionList,
	tags: ['autodocs']
}

export default meta

type Story = StoryObj<typeof QActionList>

/**
 * The default component with basic actions.
 */
export const Default: Story = {
	args: {
		items: [
			{
				key: 'SHOW',
				label: 'Show'
			},
			{
				key: 'EDIT',
				label: 'Edit'
			},
			{
				key: 'DUPLICATE',
				label: 'Duplicate'
			},
			{
				key: 'DELETE',
				label: 'Delete'
			}
		]
	}
}

/**
 * The default component using some icons.
 */
export const Icons: Story = {
	args: {
		items: [
			{
				key: 'SHOW',
				label: 'Show',
				icon: { icon: 'view' }
			},
			{
				key: 'EDIT',
				label: 'Edit',
				icon: { icon: 'pencil' }
			},
			{
				key: 'DUPLICATE',
				label: 'Duplicate',
				icon: { icon: 'duplicate' }
			},
			{
				key: 'DELETE',
				label: 'Delete',
				icon: { icon: 'delete' }
			}
		]
	}
}
