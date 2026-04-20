import _assignIn from 'lodash-es/assignIn'
import _isEmpty from 'lodash-es/isEmpty'

import { timeToString } from '../../utils/genericFunctions'
import { Base } from './base'

export class Time extends Base {
	static EMPTY_VALUE = '__:__'

	constructor(options) {
		super(
			_assignIn(
				{
					type: 'Time'
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	get displayValue() {
		if (_isEmpty(super.displayValue) || super.displayValue === Time.EMPTY_VALUE) return ''

		return timeToString(this.value)
	}

	/**
	 * @override
	 */
	get serverValue() {
		return this.value !== Time.EMPTY_VALUE ? this.value : null
	}

	/**
	 * @override
	 */
	hydrate(rawDataFieldValue) {
		// Ensure instance-specific empty value representation
		// (convert '' to '__:__')
		if (_isEmpty(rawDataFieldValue)) rawDataFieldValue = Time.EMPTY_VALUE

		super.hydrate(rawDataFieldValue)
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return typeof value === 'object' || typeof value === 'string' || value === null
	}

	/**
	 * @override
	 */
	sanitizeValue(value) {
		const sanitizedVal = super.sanitizeValue(value)

		if (typeof sanitizedVal === 'object') return sanitizedVal ? timeToString(sanitizedVal) : ''

		return sanitizedVal
	}
}
