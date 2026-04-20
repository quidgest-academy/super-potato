import type { Icon, QButtonSize, QOverlayPlacement } from '@quidgest/ui/components'
import { DEFAULT_SUBMENU_ICONS } from './constants'

export type QActionListProps = {
	/**
	 * The list of actions.
	 */
	items: QActionListItem[]

	/**
	 * The list of groups of actions.
	 */
	groups?: QActionListGroup[]

	/**
	 * The size of the dropdown button.
	 */
	dropdownSize?: QButtonSize

	/**
	 * Custom icon for submenus.
	 */
	submenusIcon?: typeof DEFAULT_SUBMENU_ICONS

	/**
	 * The placement of the dropdown menu.
	 */
	placement?: QOverlayPlacement

	/**
	 * If the actions are all in readonly.
	 */
	readonly?: boolean

	/**
	 * Whether the buttons have borders.
	 */
	borderless?: boolean
}

export type QActionListItem = {
	/**
	 * The key of the item.
	 */
	key: string

	/**
	 * The label of the item.
	 */
	label: string

	/**
	 * The key of the group.
	 */
	group?: string

	/**
	 * The icon of the item.
	 */
	icon?: Icon

	/**
	 * Whether the item is visible.
	 */
	isVisible?: boolean

	/**
	 * Whether the item is disabled.
	 */
	disabled?: boolean

	/**
	 * The description of the item.
	 */
	description?: string

	/**
	 * List of items to show in the submenu.
	 */
	items?: QActionListItem[]
}

export type QActionListGroup = {
	/**
	 * The id of the group.
	 */
	id: string

	/**
	 * The display type of the group.
	 */
	display?: 'dropdown' | 'inline' | 'mixed'

	/**
	 * The title of the group.
	 */
	title?: string

	/**
	 * Whether the group is disabled.
	 */
	disabled?: boolean

	/**
	 * The size of the group.
	 */
	size?: QButtonSize

	/**
	 * Whether to display the labels of the options in the group.
	 */
	displayLabels?: boolean
}
