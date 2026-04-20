import _assignIn from 'lodash-es/assignIn'

import { useGenericDataStore } from '../../stores/genericData'
import { dateDisplay, dateToISOString, isDate, isEmpty } from '../../utils/genericFunctions'
import { Base } from './base'

export class Date extends Base {
	static EMPTY_VALUE = ''

	constructor(options) {
		const genericDataStore = useGenericDataStore()

		super(
			_assignIn(
				{
					type: 'Date',
					dateFormat: genericDataStore.dateFormat.date
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	get displayValue() {
		return dateDisplay(this.value, this.dateFormat)
	}

	/**
	 * @override
	 */
	get serverValue() {
		return dateToISOString(this.value)
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return (isDate(value) && !isNaN(value)) || isEmpty(value)
	}

	/**
	 * @override
	 */
	sanitizeValue(value) {
		const sanitizedVal = super.sanitizeValue(value)

		if (isEmpty(sanitizedVal)) return this.constructor.EMPTY_VALUE

		return new window.Date(window.Date.parse(sanitizedVal))
	}
}
