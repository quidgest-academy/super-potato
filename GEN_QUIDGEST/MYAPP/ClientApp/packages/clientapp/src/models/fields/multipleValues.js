import _assignIn from 'lodash-es/assignIn'
import _isEmpty from 'lodash-es/isEmpty'

import { Base } from './base'

export class MultipleValues extends Base {
	constructor(options) {
		super(
			_assignIn(
				{
					type: 'MultipleValues',
					_value: []
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	get displayValue() {
		return this.value === MultipleValues.EMPTY_VALUE
			? '[]'
			: JSON.stringify(this.value)
	}

	/**
	 * @override
	 */
	clearValue() {
		super.clearValue([])
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return Array.isArray(value) || value === MultipleValues.EMPTY_VALUE
	}

	/**
	 * @override
	 */
	sanitizeValue(value) {
		const sanitizedVal = super.sanitizeValue(value)

		if (_isEmpty(sanitizedVal)) return []

		return sanitizedVal
	}
}
