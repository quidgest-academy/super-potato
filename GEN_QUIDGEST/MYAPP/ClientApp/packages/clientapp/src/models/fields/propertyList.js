import _assignIn from 'lodash-es/assignIn'
import _cloneDeep from 'lodash-es/cloneDeep'
import _forEach from 'lodash-es/forEach'

import genericFunctions from '../../utils/genericFunctions'
import { Base } from './base'

export class PropertyList extends Base {
	constructor(options) {
		super(
			_assignIn(
				{
					type: 'PropertyList',
					pkField: '',
					propCol: '',
					valueCol: '',
					typeCol: ''
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	get serverValue() {
		return {
			fields: Object.values(this.value ?? {})
		}
	}

	/**
	 * @override
	 */
	get isDirty() {
		return Object.values(this.value ?? {}).some((item) => item.isRowDirty)
	}

	/**
	 * @override
	 */
	hasSameValue(otherValue) {
		// FIXME: This shouldn't be necessary.
		return !genericFunctions.isEmpty(otherValue) || genericFunctions.isEmpty(this.value)
	}

	/**
	 * Adds a new property to the list of properties.
	 * @param {object} property The property to be added
	 */
	addProperty(property) {
		// Just in case the value is null, we need to initialize it as an empty object
		// to avoid errors when trying to add a new property to it.
		if (this.value === null) this.updateValue({})

		this.value[property.id] = property
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return typeof value === 'object'
	}

	/**
	 * @override
	 */
	updateValue(newValue) {
		const clone = _cloneDeep(newValue)
		return super.updateValue(clone)
	}

	/**
	 * This function is used to get the parsed value of a property based on the field type
	 * Its being used inside forms to display the value of the property in the UI
	 */
	getPropertyParsedValue(property) {
		return this.parseToDisplayValue(property.value, property.type)
	}

	/**
	 * Function to parse and format input value into a suitable display format based on the provided field type
	 * @param {string} value Input value to be transformed
	 * @param {string} fieldType Field type that determines the type of transformation to be applied to the input value
	 * @returns {string|number} A transformed value that is suitable for display. This can be a string, number or any other type based on the field type
	 *
	 * @example
	 * parseToDisplayValue('2021-09-25', 'date') returns '2021-09-25T00:00:00.000Z'
	 * parseToDisplayValue('Hello World!', 'string') returns 'Hello World!'
	 */
	parseToDisplayValue(value, fieldType) {
		const fieldTypeHandler = {
			date: (value) => genericFunctions.dateToISOString(value),
			boolean: (value) => genericFunctions.booleanDisplay(value),
			string: (value) => genericFunctions.textDisplay(value),
			number: (value) => parseFloat(value),
			default: (value) => value.toString()
		}

		return (fieldTypeHandler[fieldType] || fieldTypeHandler['default'])(value)
	}

	/**
	 * @override
	 */
	hydrate(listControl, viewModel) {
		const properties = viewModel?.elements

		if (!properties || properties.length === 0) {
			if (listControl !== undefined && listControl !== null)
				this.loadDefaultValues(listControl)

			return
		}

		_forEach(listControl.config.fields, (field) => {
			const property = properties.find((p) => p[this.propCol] === field.name)

			if (!property) {
				this.loadDefaultValue(field)
				return
			}

			const valueToParse = this.value?.[field.id]?.value || property[this.valueCol]
			const value = this.parseToDisplayValue(valueToParse, field.type)
			const isRowDirty = this.value?.[field.id]?.isRowDirty || field.isRowDirty

			field.rowId = property[this.pkField]
			field.defaultValue = value

			const serverData = {
				rowId: field.rowId ?? '',
				id: field.id,
				name: field.name,
				value: valueToParse,
				type: field.type,
				isRowDirty
			}

			this.addProperty(serverData)
		})

		this.originalValue = this.cloneValue()
	}

	/**
	 * Loads the default values for all the properties.
	 * @param {object} listControl The list control
	 */
	loadDefaultValues(listControl) {
		_forEach(listControl.config.fields, (field) => {
			if (!field.defaultValue) return
			this.loadDefaultValue(field)
		})
	}

	/**
	 * Loads the default value for a property.
	 * @param {object} field The property field
	 */
	loadDefaultValue(field) {
		const serverData = {
			rowId: '',
			id: field.id,
			name: field.name,
			value: field.defaultValue,
			type: field.type,
			isRowDirty: false
		}

		this.addProperty(serverData)
	}
}
