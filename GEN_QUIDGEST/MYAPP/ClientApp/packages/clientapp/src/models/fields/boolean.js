import _assignIn from 'lodash-es/assignIn'

import { Base } from './base'

export class Boolean extends Base {
	static EMPTY_VALUE = false

	constructor(options) {
		super(
			_assignIn(
				{
					type: 'Boolean'
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	sanitizeValue(value) {
		const sanitizedVal = super.sanitizeValue(value)

		if (typeof sanitizedVal === 'number') return sanitizedVal === 1

		return sanitizedVal
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return typeof value === 'boolean' || [0, 1].includes(value)
	}
}
