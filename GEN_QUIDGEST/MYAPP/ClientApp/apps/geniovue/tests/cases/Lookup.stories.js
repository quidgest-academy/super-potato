import QLookup from '@/components/inputs/QLookup.vue'

/**
 * A combination of a combobox with a set of shortcut buttons.
 */
export default {
	title: 'Inputs/Lookup',
	component: QLookup,
	tags: ['autodocs'],
}

/**
 * Displays the lookup's main combobox.
 */
export const Default = {
	args: {
		itemValue: 'key',
		itemLabel: 'label',
		items: [
			{ key: 1, label: 'Item 1' },
			{ key: 2, label: 'Item 2' },
			{ key: 3, label: 'Item 3' },
			{ key: 4, label: 'Item 4' }
		]
	},
}

/**
 * Enables the "View details" button, if there is a selected item.
 */
export const ViewDetails = {
	args: {
		modelValue: 1,
		itemValue: 'key',
		itemLabel: 'label',
		items: [
			{ key: 1, label: 'Item 1' },
			{ key: 2, label: 'Item 2' },
			{ key: 3, label: 'Item 3' },
			{ key: 4, label: 'Item 4' }
		],
		showViewDetails: true
	},
}

/**
 * Displays a "View more options" button.
 */
export const ViewMore = {
	args: {
		...Default.args,
		showSeeMore: true
	},
}

/**
 * Displays a loading animation that lets users know content is being loaded.
 */
export const Loading = {
	args: {
		...Default.args,
		loading: true
	},
}

/**
 * Conveys a read-only appearance to the field.
 * If present, the "View details" button should still be clickable.
 */
export const Readonly = {
	args: {
		...Default.args,
		modelValue: 1,
		readonly: true,
		showViewDetails: true
	}
}

/**
 * Disables the field, preventing interaction with it.
 */
export const Disabled = {
	args: {
		...Default.args,
		modelValue: 1,
		disabled: true,
		showSeeMore: true,
		showViewDetails: true
	}
}
