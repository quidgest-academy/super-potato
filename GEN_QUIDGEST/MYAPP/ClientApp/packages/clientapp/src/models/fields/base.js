import _assignIn from 'lodash-es/assignIn'
import _cloneDeep from 'lodash-es/cloneDeep'
import _has from 'lodash-es/has'
import _isEmpty from 'lodash-es/isEmpty'
import _isEqual from 'lodash-es/isEqual'
import _some from 'lodash-es/some'
import { computed, isReadonly, reactive, unref } from 'vue'
import { v4 as uuidv4 } from 'uuid'

import { deepUnwrap } from '../../utils/deepUnwrap'
import { useTracingDataStore } from '../../stores/tracingData'
import { BlockConditionStack, ClearConditionStack, HideConditionStack } from '../conditionStack'
import asyncProcM from '../../composables/async'

export class Base {
	static EMPTY_VALUE = null

	constructor(options) {
		this.type = null
		this.id = null
		this.originId = null
		this.area = null
		this.field = null
		this.relatedArea = null
		this.valueFormula = null
		this.showWhenConditions = new HideConditionStack()
		this.blockWhenConditions = new BlockConditionStack()
		this.fillWhenConditions = new ClearConditionStack()
		// Ignore the field when the model is submitted to the server.
		this.ignoreFldSubmit = false
		this.isRequired = false
		this.originalValue = this.constructor.EMPTY_VALUE
		this.arrayOptions = []
		this.serverErrorMessages = []
		this.serverWarningMessages = []
		// Indicates if the field is permanently readonly, regardless of form mode.
		this.isFixed = false
		// Indicates if the field is Global Filter field
		this.isGlobalFilterField = false

		// Properties only defined in lookups
		this.options = undefined
		this.hasMore = undefined
		this.totalRows = undefined

		// This should be a private field, but unfortunately they don't work with proxies:
		// https://github.com/tc39/proposal-class-fields/issues/106
		Object.defineProperty(this, '_value', {
			value: this.constructor.EMPTY_VALUE,
			configurable: true,
			writable: true,
			enumerable: false
		})

		_assignIn(this, options)

		this.processMonitor = asyncProcM.getProcListMonitor(`${this.id ?? uuidv4()}`, true)
		this.showWhenConditions.setProcessMonitor(this.processMonitor)
		this.blockWhenConditions.setProcessMonitor(this.processMonitor)
		this.fillWhenConditions.setProcessMonitor(this.processMonitor)
	}

	/**
	 * The current value of the field.
	 */
	get value() {
		return this._value
	}

	/**
	 * Setter for the field value.
	 */
	set value(newValue) {
		this.updateValue(newValue)
	}

	/**
	 * Checks if the field's value is different from its original value (dirty).
	 * @type {boolean} True if the field's value is dirty, false otherwise.
	 */
	get isDirty() {
		return !this.isFixed && !this.hasSameValue(this.originalValue)
	}

	/**
	 * Retrieves the display value of the field.
	 * @type {string} The display value of the field.
	 */
	get displayValue() {
		return this.parseValue(this.value)
	}

	/**
	 * The value in the format expected by the server-side.
	 */
	get serverValue() {
		return this.value
	}

	/**
	 * Checks if there are any server warning messages associated with the field.
	 * @returns {boolean} True if there are server warning messages, false otherwise.
	 */
	get hasServerWarningMessages() {
		return this.serverWarningMessages.length > 0
	}

	/**
	 * Checks if there are any server error messages associated with the field.
	 * @returns {boolean} True if there are server error messages, false otherwise.
	 */
	get hasServerErrorMessages() {
		return this.serverErrorMessages.length > 0
	}

	/**
	 * Parses the given value based on the specified rules and options.
	 *
	 * If this is an array field, the display value corresponds to the corresponding
	 * value from the arrayOptions based on the current value (array key).
	 *
	 * Otherwise, it returns the string representation of the current value,
	 * or an empty string if the value is undefined or null.
	 *
	 * @param {any} value - The value to be parsed.
	 * @returns {string} The parsed value as a string.
	 */
	parseValue(value) {
		// If this is an array field, the value will correspond to the array key, not the actual value.
		if (!_isEmpty(this.arrayOptions)) {
			const option = this.arrayOptions.find((e) => e.key === value)
			return unref(option?.value)?.toString() ?? ''
		}

		return value?.toString() ?? ''
	}

	/**
	 * Updates the value of the field.
	 *
	 * This method accepts a new value and updates the field's value accordingly.
	 * If the provided value is an object with a 'Value' property, it is treated as a special case
	 * for handling dropdown options where 'Value' represents the new value, and 'List' contains the options.
	 * If 'List' is an array, it sets the options list and tries to add the selected option to the list if not already present.
	 * Otherwise, it directly sets the provided value as the field's value.
	 *
	 * Note: To keep the context «this» and for it to work on «@update:model-value="model.ValField.updateValue"»,
	 * it needs to be declared this way and not as a function of the class.
	 *
	 * @param {any} newValue - The new value to set for the field
	 */
	updateValue(newValue) {
		let value

		// Prototype. So that it is possible to use the dropdowns that send the object with key and value of the option.
		if (!_isEmpty(newValue) && this.type === 'Lookup') {
			// The initial options list of the dropdown (lazy load - may have one option previously selected).
			if (Array.isArray(newValue.list)) {
				const items = newValue.list

				reactive(this).options = items.map((item) => ({
					key: item.key,
					// FIXME: review need for computed once i18n is refactored.
					value: computed(() => this.parseValue(item.value))
				}))

				// If for some reason the selected option is not in the list of options, add it.
				if (
					!_isEmpty(newValue.selected) &&
					!_some(newValue.list, (option) => option.key === newValue.selected)
				) {
					const selectedItem = {
						key: newValue.selected,
						// FIXME: review need for computed once i18n is refactored.
						value: computed(() => this.parseValue(newValue.value))
					}

					reactive(this).options.unshift(selectedItem)
				}

				// Total rows is unknown if query returned results and totalRows is "0"
				const totalRows = newValue.pagination?.totalRows ?? 0
				const isTotalRowsUnknown = newValue.list.length > 0 && totalRows === 0

				reactive(this).totalRows = isTotalRowsUnknown
					? undefined
					: Math.max(totalRows, this.options.length)
			}

			// If value is an object
			if (_has(newValue, 'value')) value = newValue.value
			else value = newValue
		} else value = newValue

		if (this.isValidType(value)) reactive(this)._value = this.sanitizeValue(value)
		else {
			const tracing = useTracingDataStore()
			tracing.addError({
				origin: 'updateValue',
				message: `Tried to assign an unsupported value type to "${this.id}".`,
				contextData: value
			})
		}
	}

	/**
	 * To keep the context «this» and for it to work on «@update:model-value="model.ValField.updateValue"»,
	 * it needs to be bound in a function.
	 * @param {any} newValue - The new value to set for the field
	 */
	fnUpdateValue = (newValue) => this.updateValue(newValue)

	/**
	 * Updates the value of the field from the change event.
	 * @param {object} event - The change event
	 */
	fnUpdateValueOnChange = (event) => this.updateValue(event.target?.value)

	/**
	 * Sanitizes the specified value, can be useful so the field won't be marked as dirty
	 * when assigned a different value, but still equivalent.
	 * @param {any} value - The value to sanitize
	 * @returns The sanitized value
	 */
	sanitizeValue(value) {
		if (!this.isValidType(value)) throw new Error('Unsupported value type.')
		return deepUnwrap(value)
	}

	/**
	 * Resets the current value back to it's original one.
	 */
	resetValue() {
		if (!this.isDirty) return

		this.hydrate(this.originalValue)
	}

	/**
	 * Hydrates the raw data for this field coming from the server
	 * with the necessary metadata.
	 * @param {any} rawDataFieldValue - The data to be hydrated
	 */
	hydrate(rawDataFieldValue) {
		let rawDataFieldOriginalValue = undefined

		// We are also supporting here the clone from an already existing field.
		if (rawDataFieldValue instanceof Base) {
			rawDataFieldOriginalValue = rawDataFieldValue.originalValue
			rawDataFieldValue = rawDataFieldValue.cloneValue()
		}

		this.updateValue(rawDataFieldValue)

		// Deep clone is used to ensure the object is not reactive.
		this.originalValue =
			rawDataFieldOriginalValue === undefined
				? this.cloneValue()
				: deepUnwrap(rawDataFieldOriginalValue)
	}

	/**
	 * Initializes this field with a clone of the value of the provided field.
	 * @param {object} other - The field to clone the value from
	 * @returns {this} The current instance with the cloned value.
	 */
	cloneFrom(other) {
		if (other instanceof Base) {
			/*
				The lookup fields, in addition to the value, also have a list of options.
				If we don't clone this list, when we change the form's mode,
					the GridTableList will lose the Lookups data during the recovery of the Grid's original value (resetValue).
				TODO: However, it is necessary to change the logic of changing the mode.
						It should make a request to the server to load the new form data
						OR
						Requires revision for the manwin «BEFORE_LOAD_...» and IF's based on the mode in the Load of the ViewModel.
			*/
			if (this.type === 'Lookup' && other.type === 'Lookup' && Array.isArray(other.options))
			{
				this.hydrate({
					value: other.cloneValue(),
					list: deepUnwrap(other.options),
					totalRows: other.totalRows,
					hasMore: other.hasMore
				})
			}
			else this.hydrate(other)
		}

		return this
	}

	/**
	 * Deep clones the field's value.
	 * @returns {any} A deep cloned value of the field.
	 */
	cloneValue() {
		return _cloneDeep(this._value)
	}

	/**
	 * Checks if the field's value is equal to the provided value.
	 * @param {any} otherValue - The value to compare with the field's value
	 * @returns {boolean} True if the field's value is equal to the provided value, false otherwise.
	 */
	hasSameValue(otherValue) {
		return _isEqual(this.value, otherValue)
	}

	/**
	 * Clears the field's value by setting it to the field's standard empty value.
	 * @param {any} value - A value to overwrite the standard empty value
	 */
	clearValue(value) {
		const val = typeof value === 'undefined' ? this.constructor.EMPTY_VALUE : value
		this.updateValue(val)
	}

	/**
	 * Validates the size of the field.
	 * @returns {boolean} True if the field's size is valid, false otherwise.
	 */
	validateSize() {
		return true
	}

	/**
	 * Validates the value of the field.
	 * @returns {boolean} True if the field's value is valid, false otherwise.
	 */
	validateValue() {
		return this.isRequired ? !this.isEmpty() : true
	}

	/**
	 * Checks if the specified value has a valid type.
	 * @param {any} value - The value to check
	 * @returns True if the specified value is of a valid type, false otherwise
	 */
	isValidType() {
		return true
	}

	/**
	 * Checks whether the current value of the field is equal to the empty value.
	 */
	isEmpty() {
		return this.value === this.constructor.EMPTY_VALUE
	}

	/**
	 * Set server error messages associated with the field.
	 * @param {array} errors The server errors
	 */
	setServerErrorMessages(errors) {
		this.serverErrorMessages = errors
	}

	/**
	 * Clears the server error messages associated with the field.
	 */
	clearServerErrorMessages() {
		this.serverErrorMessages.length = 0
	}

	/**
	 * Set server warning messages associated with the field.
	 * @param {array} warnings The server warnings
	 */
	setServerWarningMessages() {
		this.serverWarningMessages = []
	}

	/**
	 * Clears the server warning messages associated with the field.
	 */
	clearServerWarningMessages() {
		this.serverWarningMessages.length = 0
	}

	/**
	 * Destroys this field view model.
	 */
	destroy() {
		this.showWhenConditions?.destroy()
		this.showWhenConditions = null

		this.blockWhenConditions?.destroy()
		this.blockWhenConditions = null

		this.fillWhenConditions?.destroy()
		this.fillWhenConditions = null

		this.processMonitor?.destroy()
		this.processMonitor = null

		if (this.arrayOptions?.length > 0 && !isReadonly(this.arrayOptions))
			this.arrayOptions.length = 0

		delete this.arrayOptions
		delete this.arrayGroups
	}
}
