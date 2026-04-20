export const inputSize = {
	mini: 'mini',
	small: 'small',
	medium: 'medium',
	large: 'large',
	xlarge: 'xlarge',
	xxlarge: 'xxlarge',
	block: 'block' // Whole line
}

export const inputSizeCss = {
	mini: '60px',
	small: '90px',
	medium: '150px',
	large: '210px',
	xlarge: '270px',
	xxlarge: '530px',
	block: '100%' // Whole line
}

export const labelAlignment = {
	left: 'left',
	center: 'center',
	right: 'right',
	topleft: 'topleft',
	topright: 'topright'
}

export const wizardTypes = {
	horizontal: 'horizontal', // Horizontal steps
	vertical: 'vertical', // Vertical steps
	progress: 'progress' // Progress bar
}

export const formModes = {
	new: 'NEW',
	show: 'SHOW',
	edit: 'EDIT',
	duplicate: 'DUPLICATE',
	delete: 'DELETE'
}

export const messageTypes = {
	W: 'warning',
	OK: 'success',
	I: 'info',
	E: 'error'
}

export const breadcrumbTypes = {
	home: 'home',
	menu: 'menu',
	form: 'form'
}

export const tableViewManagementModes = {
	// The user is not allowed to change the list in any way.
	none: 'N',
	// The user is allowed to customize the table but the changes are not saved.
	nonPersistent: 'S',
	// The user changes are automatically saved in a single user table configuration.
	persistOne: 'U',
	// The user can fully create and manage multiple table configurations.
	persistMany: 'M'
}

export const triggerEvents = {
	periodic: 'P',
	beforeInit: 'PI',
	afterInit: 'DI',
	beforeApply: 'PA',
	afterApply: 'DA',
	beforeSave: 'PG',
	afterSave: 'DG',
	beforeExit: 'PE',
	afterExit: 'DE'
}

/**
 * Opening mode of the file
 */
export const documentViewTypeMode = {
	// Open the file in a new tab.
	preview: 0,
	// Download the file.
	print: 1
}

export default {
	inputSize,
	labelAlignment,
	wizardTypes,
	formModes,
	messageTypes,
	breadcrumbTypes,
	tableViewManagementModes,
	triggerEvents,
	documentViewTypeMode
}
