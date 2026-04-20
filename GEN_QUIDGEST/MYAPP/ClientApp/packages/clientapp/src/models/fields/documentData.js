import _assignIn from 'lodash-es/assignIn'
import _cloneDeep from 'lodash-es/cloneDeep'
import { readonly } from 'vue'

import { useUserDataStore } from '../../stores/userData'
import { Base } from './base'
import { Date } from './date'

/**
 * Factory function to get new instances of an empty document data.
 * @returns A new empty document data value.
 */
function getEmptyValue() {
	return {
		documentId: null,
		ticket: null,
		fileData: null,
		deleteType: -1,
		submitMode: -1,
		version: '1'
	}
}

export class DocumentData extends Base {
	constructor(options) {
		super(
			_assignIn(
				{
					versionSubmitAction: readonly({
						insert: 0, // The initial version of the file was submitted.
						submit: 1, // A new version of an already existing file was submitted.
						unlock: 2 // No new version was submitted, the editing state was simply changed.
					}),
					deleteTypes: readonly({
						current: 0, // Deletes the lastest version.
						versions: 1, // Deletes all versions except the last one.
						all: 2 // Deletes the document and all it's versions.
					}),
					value: getEmptyValue(),
					originalValue: getEmptyValue()
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	get isDirty() {
		return this.value.fileData !== null || this.value.deleteType !== -1
	}

	/**
	 * The document properties.
	 */
	get properties() {
		const properties = this.getEmptyProperties()
		properties.version = this.value.version

		if (this.value.fileData !== null) {
			const userDataStore = useUserDataStore()
			properties.author = userDataStore.username
			properties.editor = userDataStore.username

			const createdDate = new Date()
			createdDate.updateValue(this.value.fileData.lastModifiedDate)
			properties.createdDate = createdDate.displayValue

			properties.name = this.value.fileData.name
			properties.fileType = properties.name.split('.').pop().toLowerCase()
			properties.size = `${this.value.fileData.size} bytes`
		}

		return properties
	}

	/**
	 * The document data to be submitted to the server.
	 */
	get dataToSubmit() {
		if (this.value.fileData === null) return null

		return {
			fileId: `${this.value.documentId}_file`,
			ticket: this.value.ticket,
			mode: this.value.submitMode,
			version: this.value.version
		}
	}

	/**
	 * @override
	 */
	hasSameValue() {
		// FIXME: If the document is changed server-side while the user navigates to the support form,
		//        when they return, the value saved in the client-side store will be restored
		//        in favor of the newer version from the server-side.
		return true
	}

	/**
	 * @override
	 */
	updateValue(newValue) {
		super.updateValue(_cloneDeep(newValue))
	}

	/**
	 * @override
	 */
	clearValue() {
		this.value.fileData = null
		this.value.deleteType = -1
		this.value.submitMode = -1
	}

	/**
	 * Gets the properties of a document when it's empty.
	 * @returns A new object with the properties of an empty document.
	 */
	getEmptyProperties() {
		return {
			author: null,
			createdDate: null,
			documentId: null,
			editing: false,
			editor: null,
			fileType: null,
			name: null,
			size: '0 bytes',
			version: null,
			versions: null
		}
	}

	/**
	 * Sets up the necessary document properties.
	 * @param {string} id The field id
	 * @param {string} ticket The file ticket
	 */
	setup(id, ticket) {
		this.value.documentId = id
		this.value.ticket = ticket
	}

	/**
	 * Sets a new unsaved file.
	 * @param {object} file The file
	 * @param {number} submitMode The type of submit
	 * @param {string} version The document version
	 */
	setNewFile(file, submitMode, version = '1') {
		if (
			!(file instanceof File) ||
			!Object.values(this.versionSubmitAction).includes(submitMode) ||
			typeof version !== 'string'
		)
			return

		this.value.fileData = file
		this.value.submitMode = submitMode
		this.value.version = version
	}

	/**
	 * Deletes the file and possibly it's versions, depending on the specified delete type.
	 * @param {number} deleteType The type of delete action to perform
	 */
	delete(deleteType) {
		if (!Object.values(this.deleteTypes).includes(deleteType)) return

		this.value.deleteType = -1

		if (deleteType === this.deleteTypes.current) {
			if (this.value.fileData !== null) this.value.fileData = null
			else this.value.deleteType = this.deleteTypes.current
		} else if (deleteType === this.deleteTypes.versions)
			this.value.deleteType = this.deleteTypes.versions
		else {
			this.value.fileData = null
			this.value.deleteType = this.deleteTypes.all
		}
	}
}
