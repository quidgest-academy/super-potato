import _assignIn from 'lodash-es/assignIn'
import _cloneDeep from 'lodash-es/cloneDeep'
import _findIndex from 'lodash-es/findIndex'
import _flatMap from 'lodash-es/flatMap'
import _forEach from 'lodash-es/forEach'
import _get from 'lodash-es/get'
import _has from 'lodash-es/has'
import _isEmpty from 'lodash-es/isEmpty'
import _isEqual from 'lodash-es/isEqual'
import _some from 'lodash-es/some'
import { v4 as uuidv4 } from 'uuid'

import { Base } from './base'

export class GridTableListValue {
	constructor(fieldValue) {
		this.elements = []
		this.newElements = _get(fieldValue, 'newElements', [])
		this.newRecordTemplate = _get(fieldValue, 'newRecordTemplate')
		this.removedElements = _get(fieldValue, 'removedElements', [])
	}

	/**
	 * A list of the elements that have been changed.
	 */
	get editedElements() {
		return this.elements.filter(
			(row) => row.isDirty && !this.removedElements.includes(row.QPrimaryKey)
		)
	}

	/**
	 * A list of the elements that are empty and serve only as placeholder (should always be one).
	 */
	get emptyRows() {
		return this.newElements.filter((row) => !row.isDirty)
	}

	/**
	 * The number of elements.
	 */
	get rowCount() {
		return this.elements.length + this.newElements.length - this.emptyRows.length
	}

	/**
	 * Whether the grid is dirty.
	 */
	get isDirty() {
		return _some([
			_some(this.newElements, (el) => el.isDirty),
			_some(this.editedElements),
			_some(this.removedElements)
		])
	}

	/**
	 * The value in the format expected by the server-side.
	 */
	get serverValue() {
		// For existing rows, we only send those that are edited (dirty)
		// and are not marked to be deleted.
		const svrEditedElements = _flatMap(this.editedElements, (row) => row.serverObjModel)

		// For new rows, we must clear the client-side key.
		// Only those that are not empty (dirty) are sent.
		const svrNewElements = _flatMap(
			this.newElements.filter((row) => row.isDirty),
			(row) => {
				row.QPrimaryKey = null
				return row.serverObjModel
			}
		)

		return {
			editedElements: svrEditedElements,
			newElements: svrNewElements,
			removedElements: this.removedElements
		}
	}

	/**
	 * Hydrates and returns a new view model.
	 * @param {object} viewModelData The view model data
	 * @param {object} viewModelClass The class for the grid view model
	 * @param {object} vueContext The Vue context in which this value will be used
	 * @returns A new view model of type viewModelClass.
	 */
	getViewModel(viewModelData, viewModelClass, vueContext) {
		if (viewModelData === undefined || viewModelClass === undefined || vueContext === undefined)
			return undefined

		let viewModel

		if (viewModelData instanceof viewModelClass) {
			// Instance is a viewmodel, likely obtained from the navData store.
			// We can simply clone it.
			viewModel = viewModelData.clone()
		} else {
			// Not a viewmodel, so likely server-side data.
			viewModel = new viewModelClass(vueContext)
			viewModel.hydrate(viewModelData)
		}

		return viewModel
	}

	/**
	 * Adds a view model object to the list of new elements.
	 * @param {object} viewModelData The view model data
	 * @param {object} viewModelClass The class for the grid view model
	 * @param {object} vueContext The Vue context in which this value will be used
	 */
	addNewModel(viewModelData, viewModelClass, vueContext) {
		const viewModel = this.getViewModel(viewModelData, viewModelClass, vueContext)
		if (viewModel !== undefined) this.newElements.push(viewModel)
	}

	/**
	 * Removes empty rows from the list of new elements. Optionally retains one last empty row.
	 * @param {boolean} full A flag indicating whether to remove all empty rows or leave one remaining
	 */
	trimEmptyRows(full) {
		let pop = this.emptyRows.length
		if (!full) pop--

		while (pop--) this.newElements.pop()

		// Ensure the row left by the trim operation has no
		// server error messages from previous attempts to save the form
		_forEach(this.emptyRows, (row) => row.clearServerErrorMessages())
	}

	/**
	 * Marks the given view model as deleted or removes it from the list of new elements.
	 * @param {object} viewModelData The view model to be marked for deletion
	 */
	markForDeletion(viewModelData) {
		// Check if this is a new row
		// New rows are removed immediately
		// instead of being marked to be deleted
		const index = this.newElements.indexOf(viewModelData)

		if (index > -1) this.newElements.splice(index, 1)
		else this.removedElements.push(viewModelData.QPrimaryKey)
	}

	/**
	 * Reverts the deletion mark from the given view model, if it was previously marked.
	 * @param {object} viewModelData The view model to undo deletion
	 */
	undoDeletion(viewModelData) {
		const index = this.removedElements.indexOf(viewModelData.QPrimaryKey)

		if (index > -1) this.removedElements.splice(index, 1)
	}

	/**
	 * Sets the whole value of the grid table list including its elements, new elements, and removed elements.
	 * @param {object} newValue The new value object representing the grid state
	 * @param {object} viewModelClass The class for the grid view model
	 * @param {object} vueContext The Vue context in which this value will be used
	 */
	setValue(newValue, viewModelClass, vueContext) {
		if (newValue === null || viewModelClass === undefined || vueContext === undefined) return

		const elements = [],
			newElements = []

		_forEach(_get(newValue, 'elements', []), (viewModelData) => {
			const viewModel = this.getViewModel(viewModelData, viewModelClass, vueContext)
			if (viewModel !== undefined) elements.push(viewModel)
		})

		_forEach(_get(newValue, 'newElements', []), (viewModelData) => {
			const viewModel = this.getViewModel(viewModelData, viewModelClass, vueContext)
			if (viewModel !== undefined) newElements.push(viewModel)
		})

		// For cases where more than one process updates the value, we need to update all at the same time and not push to the central property.
		// Bug case: Initial load of form and restore of the last tab (SelectTab).
		this.elements.splice(0, Infinity, ...elements)
		this.newElements.splice(0, Infinity, ...newElements)
		this.removedElements.splice(0, Infinity, ..._get(newValue, 'removedElements', []))
		this.newRecordTemplate = _get(newValue, 'newRecordTemplate', this.newRecordTemplate)
	}

	/**
	 * Returns an object representing the current state of the grid, with elements, new elements, and removed elements.
	 * @returns {object} An object containing the current state of the grid.
	 */
	getCurrentState() {
		return {
			elements: this.elements.filter(
				(row) => !this.removedElements.includes(row.QPrimaryKey)
			),
			removedElements: this.elements.filter((row) =>
				this.removedElements.includes(row.QPrimaryKey)
			),
			newElements: this.newElements.filter((row) => row.isDirty)
		}
	}

	/**
	 * Returns an object representing the current state of the grid suitable for server communication.
	 * @param {boolean} removedElementsOnlyKey True to return only the key of removed elements (defaults to false)
	 * @param {boolean} elementsOnlyDirty True to return only the elements that are dirty (have been modified)
	 * @returns {object} An object containing the current state of the grid in a server-compatible format.
	 */
	getCurrentStateSrvObject(removedElementsOnlyKey = false, elementsOnlyDirty = false) {
		const currentState = this.getCurrentState()

		return {
			elements: _flatMap(
				elementsOnlyDirty
					? currentState.elements.filter((row) => row.isDirty)
					: currentState.elements,
				(row) => row.serverObjModel
			),
			removedElements: _flatMap(currentState.removedElements, (row) =>
				removedElementsOnlyKey ? row.QPrimaryKey : row.serverObjModel
			),
			newElements: _flatMap(currentState.newElements, (row) => row.serverObjModel)
		}
	}

	/**
	 * Set server error messages associated with the field.
	 * @param {object} errors The server errors
	 * @param {array} key The model field path
	 */
	setServerErrorMessages(errors, key) {
		const rowsListName = key.shift()
		const rowId = key.shift()

		if (rowsListName === 'editedElements' || rowsListName === 'removedElements') {
			const modelIndex = _findIndex(this.elements, (row) => row.QPrimaryKey === rowId)
			const rowModel = _get(this.elements, modelIndex)
			rowModel?.setServerErrorMessages(key, errors)
		} else if (rowsListName === 'newElements') {
			const rowModel = _get(this.newElements, rowId)
			rowModel?.setServerErrorMessages(key, errors)
		}
	}

	/**
	 * Clears server error messages for all elements and new elements.
	 */
	clearServerErrorMessages() {
		_forEach(this.elements, (el) => el.clearServerErrorMessages())
		_forEach(this.newElements, (el) => el.clearServerErrorMessages())
	}

	/**
	 * Set server warning messages associated with the field.
	 * @param {object} errors The server errors
	 * @param {array} key The model field path
	 */
	setServerWarningMessages(errors, key) {
		const rowsListName = key.shift()
		const rowId = key.shift()

		if (rowsListName === 'editedElements' || rowsListName === 'removedElements') {
			const modelIndex = _findIndex(this.elements, (row) => row.QPrimaryKey === rowId)
			const rowModel = _get(this.elements, modelIndex)
			rowModel?.setServerWarningMessages(key, errors)
		} else if (rowsListName === 'newElements') {
			const rowModel = _get(this.newElements, rowId)
			rowModel?.setServerWarningMessages(key, errors)
		}
	}

	/**
	 * Clears server warning messages for all elements and new elements.
	 */
	clearServerWarningMessages() {
		_forEach(this.elements, (el) => el.clearServerWarningMessages())
		_forEach(this.newElements, (el) => el.clearServerWarningMessages())
	}

	/**
	 * Deep clones the field's value.
	 */
	clone() {
		const clone = new GridTableListValue()

		this.elements.forEach((model) => clone.elements.push(model.clone()))
		this.newElements.forEach((model) => clone.newElements.push(model.clone()))

		clone.removedElements = _cloneDeep(this.removedElements)
		clone.newRecordTemplate = _cloneDeep(this.newRecordTemplate)

		return clone
	}
}

export class GridTableList extends Base {
	constructor(options, vueContext) {
		super(
			_assignIn(
				{
					type: 'GridTableList',
					_value: new GridTableListValue(),
					viewModelClass: undefined
				},
				options
			)
		)

		// Just to initialize the View Model of Row's (Resources + NavigationId for requests)
		this.vueContext = vueContext
	}

	/**
	 * @override
	 */
	get isDirty() {
		return this.value.isDirty
	}

	/**
	 * @override
	 */
	get serverValue() {
		return this.value.serverValue
	}

	/**
	 * The current elements in the grid.
	 */
	get elements() {
		return this.value.elements
	}

	/**
	 * The elements that have been added to the grid.
	 */
	get newElements() {
		return this.value.newElements
	}

	/**
	 * The elements that have been edited in the grid.
	 */
	get editedElements() {
		return this.value.editedElements
	}

	/**
	 * The elements that have been marked for removal.
	 */
	get removedElements() {
		return this.value.removedElements
	}

	/**
	 * A list of the elements that are empty and serve only as placeholder (should always be one).
	 */
	get emptyRows() {
		return this.value.emptyRows
	}

	/**
	 * The number of elements.
	 */
	get rowCount() {
		return this.value.rowCount
	}

	/**
	 * @override
	 */
	updateValue(newValue) {
		this.value.setValue(newValue, this.viewModelClass, this.vueContext)
	}

	/**
	 * @override
	 */
	clearServerErrorMessages() {
		this.value.clearServerErrorMessages()
	}

	/**
	 * @override
	 */
	clearServerWarningMessages() {
		this.value.clearServerWarningMessages()
	}

	/**
	 * @override
	 */
	hasSameValue(otherValue) {
		if (!(otherValue instanceof GridTableListValue)) return false

		return _isEqual(
			this.value.getCurrentStateSrvObject(),
			otherValue.getCurrentStateSrvObject()
		)
	}

	/**
	 * @override
	 */
	cloneValue() {
		return this.value.clone()
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return value instanceof GridTableListValue || value === null
	}

	/**
	 * Set server error messages associated with the model field.
	 * @param {object} errors The server errors
	 * @param {array} key The model field
	 */
	setServerErrorMessages(errors, key) {
		this.value.setServerErrorMessages(errors, key)
	}

	/**
	 * Set server warning messages associated with the model field.
	 * @param {object} warnings The server warnings
	 * @param {array} key The model field
	 */
	setServerWarningMessages(warnings, key) {
		this.value.setServerWarningMessages(warnings, key)
	}

	/**
	 * Update a single field's value for a specific model in the grid.
	 * @param {object} eventData Data describing the event that initiated the update
	 */
	setModelFieldValue(eventData) {
		const modelUId = _get(eventData, 'key'),
			fieldData = _get(eventData, 'value'),
			fieldName = _get(fieldData, 'modelField'),
			fieldValue = _get(fieldData, 'value')

		if (_isEmpty(modelUId) || _isEmpty(fieldName) || !_has(this, 'value.elements')) return

		let modelIndex = _findIndex(this.value.elements, (row) => row.uniqueIdentifier === modelUId)

		if (modelIndex !== -1) this.value.elements[modelIndex][fieldName].updateValue(fieldValue)
		else {
			modelIndex = _findIndex(
				this.value.newElements,
				(row) => row.uniqueIdentifier === modelUId
			)
			if (modelIndex !== -1)
				this.value.newElements[modelIndex][fieldName].updateValue(fieldValue)
		}
	}

	/**
	 * Adds a new model to the grid using the new record template.
	 */
	addNewModel() {
		const newModelData = _cloneDeep(this.value.newRecordTemplate)

		if (newModelData) {
			newModelData[this.viewModelClass.QPrimaryKeyName] = uuidv4()
			this.value.addNewModel(newModelData, this.viewModelClass, this.vueContext)
		}
	}

	/**
	 * Removes empty rows from the grid, optionally leaving one empty row if not full.
	 * @param {boolean} full A flag indicating whether to remove all empty rows or leave one remaining
	 */
	trimEmptyRows(full) {
		this.value.trimEmptyRows(full)
	}

	/**
	 * Marks the specified row for deletion in the grid.
	 * @param {object} row The row to be marked for deletion
	 */
	markForDeletion(row) {
		this.value.markForDeletion(row)
	}

	/**
	 * Undoes the deletion mark on the specified row, if it was previously marked.
	 * @param {object} row The row to remove from deletion
	 */
	undoDeletion(row) {
		this.value.undoDeletion(row)
	}

	/**
	 * @override
	 */
	destroy() {
		super.destroy()

		const elements = [...this.elements, ...this.newElements]
		for (const element of elements) element.destroy()

		this.elements.length = 0
		this.newElements.length = 0
		this.removedElements.length = 0
	}
}
