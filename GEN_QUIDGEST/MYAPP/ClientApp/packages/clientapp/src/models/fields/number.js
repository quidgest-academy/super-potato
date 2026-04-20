import _assignIn from 'lodash-es/assignIn'
import _toNumber from 'lodash-es/toNumber'

import { useGenericDataStore } from '../../stores/genericData'
import { numericDisplay } from '../../utils/genericFunctions'
import { Base } from './base'

export class Number extends Base {
	static EMPTY_VALUE = 0

	constructor(options) {
		const genericDataStore = useGenericDataStore()

		super(
			_assignIn(
				{
					type: 'Number',
					maxDigits: -1,
					decimalDigits: 0,
					maxIntegers: -1,
					maxDecimals: -1,
					decimalSeparator: genericDataStore.numberFormat.decimalSeparator,
					groupSeparator: genericDataStore.numberFormat.thousandsSeparator
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	get displayValue() {
		const value = _toNumber(this.value)
		if (isNaN(value)) return ''
		return numericDisplay(
			value.toFixed(this.decimalDigits),
			this.decimalSeparator,
			this.groupSeparator
		)
	}

	/**
	 * @override
	 */
	sanitizeValue(value) {
		const sanitizedVal = super.sanitizeValue(value)
		return _toNumber(sanitizedVal)
	}

	/**
	 * @override
	 */
	validateValue() {
		return super.validateValue() && (this.isRequired ? !isNaN(_toNumber(this.value)) : true)
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return !isNaN(value)
	}
}
