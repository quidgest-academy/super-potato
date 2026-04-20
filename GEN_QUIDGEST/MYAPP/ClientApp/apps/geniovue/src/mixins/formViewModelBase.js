import { Base, Document } from '@quidgest/clientapp/models/fields'
import { postData, uploadFile } from '@quidgest/clientapp/network'
import { useTracingDataStore } from '@quidgest/clientapp/stores'
import _forEach from 'lodash-es/forEach'
import _isEmpty from 'lodash-es/isEmpty'
import _isEqual from 'lodash-es/isEqual'
import _set from 'lodash-es/set'
import _some from 'lodash-es/some'
import { markRaw } from 'vue'

import ViewModelBase from '@/mixins/viewModelBase.js'

export default class FormViewModelBase extends ViewModelBase
{
	constructor(vueContext, options)
	{
		super(vueContext, options)

		// The view model metadata
		Object.defineProperty(this, 'modelInfo', {
			value: {
				name: undefined,
				area: undefined,
				actions: {
					recalculateFormulas: undefined,
					loadLookupContent: 'ReloadDBEdit',
					getLookupDependents: 'GetDependants'
				}
			},
			enumerable: false,
			configurable: true
		})

		// The web api request counters - to accept only the last one's response and discard the others
		Object.defineProperty(this, 'recalculateFormulasRequestNumber', {
			value: 0,
			enumerable: false,
			writable: true
		})

		// An object with the IDs and values that triggered the last calls to recalculateFormulas()
		Object.defineProperty(this, 'lastRecalculatedValues', {
			value: markRaw({}),
			enumerable: false,
			writable: true
		})
	}

	/**
	 * A list of the fields with unsaved changes.
	 */
	get dirtyFields()
	{
		const dirtyFields = []

		for (const modelField in this)
		{
			const fieldObj = this[modelField]

			if (fieldObj instanceof Base && fieldObj.isDirty)
				dirtyFields.push(fieldObj)
		}

		return dirtyFields
	}

	/**
	 * A list with the area and name of unsaved changes ex: AREA.FIELD.
	 */
	get dirtyFieldNames()
	{
		return this.dirtyFields.map((obj) => `${obj.area}.${obj.field}`)
	}

	/**
	 * True if the View Model has fields with unsaved changes, false otherwise.
	 */
	get isDirty()
	{
		return _some(this, (modelField) => modelField instanceof Base && modelField.isDirty && !modelField.isGlobalFilterField)
	}

	/**
	 * True if there are no validations errors, false otherwise.
	 */
	get isValid()
	{
		return !_some(this.validateModel(), (fldValidation) => !fldValidation.value || !fldValidation.size)
	}

	/**
	 * Resets the values of all model fields back to their original ones.
	 */
	resetValues()
	{
		for (const modelField in this)
		{
			const fieldObj = this[modelField]
			fieldObj.resetValue()
		}
	}

	/**
	 * Clears the values of all model fields.
	 */
	clearValues()
	{
		for (const modelField in this)
		{
			const fieldObj = this[modelField]
			fieldObj.hydrate(fieldObj.constructor.EMPTY_VALUE)
		}
	}

	/**
	 * Recalculates the server side formulas.
	 * @param {object} triggerFields An object with the fields that triggered the call
	 * @returns A promise with the response from the server.
	 */
	recalculateFormulas(triggerFields = {})
	{
		if (_isEmpty(this.modelInfo.area) || _isEmpty(this.modelInfo.actions.recalculateFormulas))
			return

		// Check if there was any change since the last call to recalculateFormulas().
		// If 'triggerFields' is empty, runs the recalculation anyway.
		if (Object.keys(triggerFields).length > 0)
		{
			let hasChanged = false

			for (const i in triggerFields)
			{
				const fieldValue = triggerFields[i]

				if (!_isEqual(this.lastRecalculatedValues[i], fieldValue))
				{
					this.lastRecalculatedValues[i] = fieldValue
					hasChanged = true
				}
			}

			// If no changes were detected, doesn't do anything.
			if (!hasChanged)
				return
		}

		const model = this.serverObjModel

		return postData(
			this.modelInfo.area,
			this.modelInfo.actions.recalculateFormulas,
			model,
			(data, request) => {
				const requestNumber = request.headers.recalculateformulasrequestnumber
				if (Number(requestNumber) !== this.recalculateFormulasRequestNumber)
					return

				if (request.data?.Success)
				{
					if (typeof data !== 'object')
						return

					for (const modelField in this)
					{
						const fieldObj = this[modelField]

						if (_isEmpty(fieldObj.area) || _isEmpty(fieldObj.field))
							continue

						if (fieldObj instanceof Base)
						{
							const fieldArea = fieldObj.area.toLowerCase()
							const fieldName = fieldObj.field.toLowerCase()
							const fieldFullName = `${fieldArea}.${fieldName}`
							const fieldValue = data[fieldFullName]

							if (typeof fieldValue !== 'undefined')
								fieldObj.updateValue(fieldValue)
						}
					}
				}
				else
				{
					const tracingDataStore = useTracingDataStore()
					tracingDataStore.addError({
						origin: 'recalculateFormulas',
						message: `Error found while trying to recalculate the formulas for form "${this.modelInfo.name}".`,
						contextData: data
					})
				}
			},
			undefined,
			{
				headers: {
					RecalculateFormulasRequestNumber: ++this.recalculateFormulasRequestNumber
				}
			},
			this.navigationId)
	}

	/**
	 * Initialization of field value formula events
	 */
	initFieldsValueFormula()
	{
		_forEach(this, (modelField) => {
			// Field value formulas
			if (modelField.valueFormula)
			{
				if (typeof modelField.valueFormula.runFormula !== 'function')
				{
					modelField.valueFormula.runFormula = (originFieldData) => {
						if (modelField.valueFormula.stopRecalcCondition.call(this))
							return

						const execCondition = modelField.valueFormula.execCondition
						if (typeof execCondition === 'function' && !execCondition.call(this))
							return

						const params = {
							originField: originFieldData?.modelField,
							currentField: modelField
						}

						// If it's a server-side recalculation, it's value will be set when the recalculateFormulas() function is called.
						if (!modelField.valueFormula.isServerRecalc)
						{
							const evalResult = modelField.valueFormula.fnFormula.call(this, params)
							Promise.resolve(evalResult).then((value) => modelField.updateValue(value))
						}
					}
				}

				this.internalEvents.offMany([...modelField.valueFormula.dependencyEvents, 'CALC_FIELDS_FORMULAS'], modelField.valueFormula.runFormula)
				this.internalEvents.onMany([...modelField.valueFormula.dependencyEvents, 'CALC_FIELDS_FORMULAS'], modelField.valueFormula.runFormula)
			}
		})
	}

	/**
	 * Forces the recalculation of the DB fields formulas.
	 */
	calcFieldsFormulas()
	{
		this.emitInternalEvent('CALC_FIELDS_FORMULAS')
	}

	/**
	 * Forces the recalculation of the "Show when" formulas.
	 */
	calcShowWhenFormulas()
	{
		this.emitInternalEvent('CALC_SHOW_WHEN_FORMULAS')
	}

	/**
	 * Forces the recalculation of the "Block when" formulas.
	 */
	calcBlockWhenFormulas()
	{
		this.emitInternalEvent('CALC_BLOCK_WHEN_FORMULAS')
	}

	/**
	 * Forces the recalculation of the "Fill when" formulas.
	 */
	calcFillWhenFormulas()
	{
		this.emitInternalEvent('CALC_FILL_WHEN_FORMULAS')
	}

	/**
	 * Performs validations over the model fields.
	 * @returns The validation results.
	 */
	validateModel()
	{
		const modelValidations = {}

		_forEach(this, (modelField, modelFieldName) => {
			if (modelField instanceof Base)
			{
				_set(modelValidations, modelFieldName, {
					fieldName: modelFieldName,
					// If the field is required, ensures it's filled.
					value: modelField.validateValue(),
					// If the field has a maximum number of characters, ensures it hasn't been exceeded.
					size: modelField.validateSize()
				})
			}
		})

		return modelValidations
	}

	/**
	 * Updates the documents tickets with write permissions, in case the form's view model is valid.
	 * @param {boolean} isApply True if the method is being called during an apply, false if it's a save
	 * @returns A boolean with the result of the server request.
	 */
	async updateFilesTickets(isApply = false)
	{
		if (_isEmpty(this.modelInfo.actions.updateFilesTickets))
			return false

		const tickets = [],
			documentFields = Object.values(this).filter((e) => e instanceof Document && e.isDirty && e.type !== 'Lookup')

		for (const field of documentFields)
		{
			const currentDocument = field.currentDocument.value
			const ticketInfo = {
				fieldId: field.id,
				ticket: currentDocument.ticket
			}
			tickets.push(ticketInfo)
		}

		if (tickets.length > 0)
		{
			const params = {
				tickets,
				model: this.serverObjModel,
				isApply
			}

			const promise = new Promise((resolve) => {
				postData(
					this.modelInfo.area,
					this.modelInfo.actions.updateFilesTickets,
					params,
					(data, request) => {
						if (request.data?.Success)
						{
							for (const ticketInfo of data.tickets)
							{
								const currentDocument = this[ticketInfo.fieldId].currentDocument.value
								currentDocument.ticket = ticketInfo.ticket
							}

							resolve(true)
						}
						else
						{
							const tracingDataStore = useTracingDataStore()
							tracingDataStore.addError({
								origin: 'updateFilesTickets',
								message: `Error found while trying to validate the model of form "${this.modelInfo.name}".`,
								contextData: request.data
							})

							// If something goes wrong, reset the tickets.
							for (const field of documentFields)
							{
								const areaKeyField = this.vueContext.dataApi.keys[field.area.toLowerCase()]
								field.setTickets(areaKeyField.value, this.navigationId)
							}

							resolve(false)
						}
					},
					undefined,
					undefined,
					this.navigationId)
			})

			return await Promise.resolve(promise)
		}

		return true
	}

	/**
	 * Saves the newly uploaded files in document fields, if the form has any.
	 * @returns A list with the results of the requests sent to the server.
	 */
	async saveDocuments()
	{
		if (_isEmpty(this.modelInfo.actions.setFile))
			return []

		const promises = [],
			documentFields = Object.values(this).filter((e) => e instanceof Document && e.isDirty && e.type !== 'Lookup')

		for (const field of documentFields)
		{
			const currentDocument = field.currentDocument

			// Check for a newly uploaded file.
			if (currentDocument.value.fileData === null)
				continue

			// Submit a different request for each file.
			const promise = new Promise((resolve) => {
				uploadFile(
					field.area,
					`${this.modelInfo.actions.setFile}${field.field}`,
					currentDocument.value.fileData,
					currentDocument.dataToSubmit,
					(data) => {
						// As long as there is no "success": true/false, it's just a progress response.
						if (data?.success === true)
						{
							field.properties.updateValue(data.properties)
							field.documentFK.updateValue(data.properties.documentId)

							const areaKeyField = this.vueContext.dataApi.keys[field.area.toLowerCase()]
							field.setTickets(areaKeyField.value, this.navigationId)
							currentDocument.clearValue()

							resolve(true)
						}
						else if (data?.success === false)
						{
							const tracingDataStore = useTracingDataStore()
							tracingDataStore.addError({
								origin: 'saveDocuments',
								message: `Error found while trying to save document "${field.id}".`,
								contextData: {
									field,
									data
								}
							})

							resolve(false)
						}
						else if (data.validModel === false)
							resolve(true)
					},
					(error) => {
						const tracingDataStore = useTracingDataStore()
						tracingDataStore.addError({
							origin: 'saveDocuments',
							message: `Error found while trying to save document "${field.id}".`,
							contextData: {
								field,
								error
							}
						})

						resolve(false)
					})
			})

			promises.push(promise)
		}

		return await Promise.all(promises)
	}

	/**
	 * Saves the editing and deletion state of all the document fields in the form, if any.
	 * @returns A boolean with the result of the server request.
	 */
	async setDocumentChanges()
	{
		const unsavedChanges = [],
			documentFields = Object.values(this).filter((e) => e instanceof Document && e.isDirty && e.type !== 'Lookup')

		for (const field of documentFields)
		{
			const currentDocument = field.currentDocument.value,
				properties = field.properties,
				changes = {}

			// Check the editing state.
			if (currentDocument.fileData === null && properties.value.editing !== (properties.originalValue?.editing ?? false))
				changes.editing = properties.value.editing

			// Check for versions that should be deleted.
			if (currentDocument.deleteType !== -1)
			{
				changes.currentVersion = properties.value.version
				changes.deleteType = currentDocument.deleteType
				changes.delete = true
			}

			if (!_isEmpty(changes))
			{
				changes.ticket = currentDocument.ticket
				unsavedChanges.push(changes)
			}
		}

		// Submit a single request with all the state changes and delete operations.
		if (unsavedChanges.length > 0)
		{
			const promise = new Promise((resolve) => {
				postData(
					this.modelInfo.area,
					'SetFilesState',
					{ documents: unsavedChanges },
					(_, request) => {
						if (request.data?.Success)
						{
							for (const field of documentFields)
							{
								const areaKeyField = this.vueContext.dataApi.keys[field.area.toLowerCase()]
								field.setTickets(areaKeyField.value, this.navigationId)

								// Only reset if not also submitting a new file to replace the one that was deleted.
								if (field.currentDocument.value.submitMode === -1)
									field.currentDocument.clearValue()
							}

							resolve(true)
						}
						else
						{
							const tracingDataStore = useTracingDataStore()
							tracingDataStore.addError({
								origin: 'setDocumentChanges',
								message: `Error found while trying to set document properties in form "${this.modelInfo.name}".`,
								contextData: request.data
							})

							resolve(false)
						}
					},
					undefined,
					undefined,
					this.navigationId)
			})

			return await promise
		}

		return true
	}
}
