import _assignIn from 'lodash-es/assignIn'

import { isEmpty } from '../../utils/genericFunctions'
import { Base } from './base'

export class String extends Base {
	static EMPTY_VALUE = ''

	constructor(options) {
		super(
			_assignIn(
				{
					type: 'String',
					maxLength: -1
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

		if (isEmpty(sanitizedVal)) return this.constructor.EMPTY_VALUE

		return sanitizedVal
	}

	/**
	 * @override
	 */
	validateSize() {
		if (this.maxLength > 0) {
			const length = this.value?.length ?? 0
			return length <= this.maxLength
		}
		return true
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return typeof value === 'string' || isEmpty(value)
	}
}
