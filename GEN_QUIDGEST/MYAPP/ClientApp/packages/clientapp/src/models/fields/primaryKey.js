import _assignIn from 'lodash-es/assignIn'
import _isEmpty from 'lodash-es/isEmpty'
import { validate as uuidValidate } from 'uuid'

import { String } from './string'

export class PrimaryKey extends String {
	constructor(options) {
		super(
			_assignIn(
				{
					maxLength: 16
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	get serverValue() {
		return this.value === this.constructor.EMPTY_VALUE ? null : this.value
	}

	/**
	 * @override
	 */
	validateSize() {
		// GUIDs
		if (this.maxLength === 16) return _isEmpty(this.value) || uuidValidate(this.value)
		// Other key types
		return super.validateSize()
	}
}
