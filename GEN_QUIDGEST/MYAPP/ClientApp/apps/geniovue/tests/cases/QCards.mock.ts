// Constants
import { thumbnail } from './QCards.assets'

// Types
import type { ListConfig, MappedRow } from '@/mixins/types'

const lorem = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit'

export const listConfig: ListConfig = {
	generalActions: [
		{
			id: 'insert',
			name: 'insert',
			title: 'Insert',
			icon: {
				icon: 'add'
			},
			isInReadOnly: true
		}
	],
	crudActions: [
		{
			id: 'show',
			name: 'SHOW',
			title: 'View',
			icon: {
				icon: 'view'
			},
			isInReadOnly: true
		},
		{
			id: 'edit',
			name: 'EDIT',
			title: 'Edit',
			icon: {
				icon: 'pencil'
			},
			isInReadOnly: false
		}
	],
	customActions: [],
	resourcesPath: 'Content/img/',
	perPage: 6,
	tableNamePlural: 'Users'
}

export const cards: MappedRow[] = [
	{
		rowKey: '1',
		title: { value: 'Card title' },
		subtitle: { value: 'Card subtitle' },
		text: [
			{
				source: { label: 'About' },
				value: lorem
			}
		],
		image: {
			value: '',
			previewData: thumbnail,
			dominantColor: '#70597e'
		},
		btnPermission: {
			editBtnDisabled: false,
			viewBtnDisabled: false,
			deleteBtnDisabled: false,
			insertBtnDisabled: false
		}
	}
]
