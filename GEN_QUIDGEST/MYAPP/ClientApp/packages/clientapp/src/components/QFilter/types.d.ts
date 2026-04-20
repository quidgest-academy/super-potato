import type { QFieldSize } from '@quidgest/ui/components'

/**
 * Implemented filter viewmode codes:
 * Dropdown (Default, enumeration)
 * Checkbox (Checkbox group for arrays)
 * Radio (Radio group)
 * Text (Open text field)
 */
type FilterViewMode = 'checkbox' | 'dropdown' | 'radio' | 'text'

export type FilterType = string | boolean | number

export type FilterValue = Array<FilterType> | FilterType

/**
 * Array filter item.
 */
export type FilterItem = {
	key: string
	value: FilterType
	group?: string
}

/**
 * Array filter group.
 */
type FilterGroup = {
	id: string
	title: string
}

export type QFilterBaseProps = {
	/**
	 * Custom set of classes to apply to the component.
	 */
	class?: string | unknown[]

	/**
	 * True if the filter is in a read-only state, false otherwise.
	 */
	readonly?: boolean

	/**
	 * The label of the filter.
	 */
	label?: string
}

/**
 * Props for all filter modes.
 * Needed for QFilter to transmit all necessary props to the computed component.
 */
export type QFilterGenericProps = QFilterGroupProps & QFilterDropdownProps & QFilterTextProps & {
	/**
	 * Control identifier.
	 */
	id: string

	/**
	 * True if the filter is in a loading state, false otherwise.
	 */
	loading?: boolean

	/**
	 * Control texts.
	 */
	texts?: Record<string, string>

	/**
	 * Filter viewmode.
	 */
	viewMode: FilterViewMode
}

/**
 * Props for group filter modes (radio and checkbox filters).
 */
export type QFilterGroupProps = QFilterBaseProps & {
	/**
	 * Number of columns to divide the radio/checkbox options into.
	 */
	columns: number

	/**
	 * Array of filter options.
	 */
	items?: Array<FilterItem>

	/**
	 * The orientation of the radio/checkbox options shown.
	 * Either 'vertical' or 'horizontal'.
	 */
	orientation: 'vertical' | 'horizontal'
}

/**
 * Props for dropdown filter modes (comboboxes).
 */
export type QFilterDropdownProps = QFilterBaseProps & {
	/**
	 * If set to true, the filter value can be cleared.
	 */
	clearable?: boolean

	/**
	 * Array of filter options.
	 */
	items?: Array<FilterItem>

	/**
	 * Array item groups.
	 */
	groups?: Array<FilterGroup>

	/**
	 * The size category of the field.
	 */
	size?: QFieldSize
}

/**
 * Props for dropdown filter modes (comboboxes).
 */
export type QFilterTextProps = QFilterBaseProps & {
	/**
	 * If set to true, the clear button will be available.
	 */
	clearable?: boolean

	/**
	 * The size category of the field.
	 */
	size?: QFieldSize
}
