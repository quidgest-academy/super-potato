import type { QActionListItem } from '../QActionList/types'
import type { QFieldSize } from '@quidgest/ui/components'

export type QDocumentProps = {
	/**
	 * Unique identifier for the control
	 */
	id: string

	/**
	 * The size of the input
	 */
	size?: QFieldSize

	/**
	 * Whether or not versioning is active for the document.
	 */
	versioning?: boolean

	/**
	 * The current version numbers of the document.
	 */
	versions?: Record<string, string>

	/**
	 * Whether the field is disabled.
	 */
	disabled?: boolean

	/**
	 * Whether the field is readonly.
	 */
	readonly?: boolean

	/**
	 * Whether or not the document is currently being edited by someone.
	 */
	editing?: boolean

	/**
	 * Whether the control uses document templates.
	 */
	usesTemplates?: boolean

	/**
	 * The current version of the document.
	 */
	currentVersion?: string

	/**
	 * The properties of the document
	 */
	fileProperties?: FileProperties

	/**
	 * The list of details about the version history of the document
	 */
	versionsInfo?: VersionInfo[]

	/**
	 * Extensions allowed for file select, some extension examples: .png, .jpg, .jpeg, .csv, .xls, .xlsx, .pdf.
	 */
	extensions?: string[]

	/**
	 * Maximum file size allowed, in bytes (must be a positive number).
	 */
	maxFileSize?: number

	/**
	 * The resources path.
	 */
	resourcesPath: string

	/**
	 * Necessary strings to be used in labels and buttons.
	 */
	texts?: Record<string, string>

	/**
	 * Whether or not one of the popups is currently open.
	 */
	popupIsVisible?: boolean
}

export type QDocumentDropdownItem = QActionListItem & {
	/**
	 * The action to execute when the item is clicked
	 */
	action: () => void
}

export type FileProperties = {
	/**
	 * The id of the document.
	 */
	documentId?: string

	/**
	 * The name of the document.
	 */
	name?: string

	/**
	 * The extension type of the document.
	 */
	fileType?: string

	/**
	 * The size of the document.
	 */
	size?: string

	/**
	 * The author of the document.
	 */
	author?: string

	/**
	 * The last user who edited the file.
	 */
	editor?: string

	/**
	 * The date of creation.
	 */
	createdDate?: string

	/**
	 * Whether or not the modal is in "Edit" mode.
	 */
	editing?: boolean

	/**
	 * The current version of the file.
	 */
	version?: string

	/**
	 * The list of all the versions of the document.
	 */
	versions?: string[]
}

export type GetFile = {
	/**
	 * The version of the file to get
	 */
	version: string

	/**
	 * Wether or not to download the file
	 */
	download: boolean
}

export type SubmitFile = {
	/**
	 * The new file to save
	 */
	file: File

	/**
	 * The version to update/add
	 */
	version: string

	/**
	 * Wehter it is a new version or not
	 */
	isNewVersion?: boolean

	/**
	 * Wether or not to unlock the input
	 */
	unlock?: boolean
}

export type VersionInfo = {
	/**
	 * The id of the file version
	 */
	id: string

	/**
	 * The name of the file
	 */
	fileName: string

	/**
	 * The version number
	 */
	version: string

	/**
	 * The author of the version
	 */
	author?: string

	/**
	 * The bytes used by the file
	 */
	bytes?: string

	/**
	 * When the version was created
	 */
	createdOn?: string
}
