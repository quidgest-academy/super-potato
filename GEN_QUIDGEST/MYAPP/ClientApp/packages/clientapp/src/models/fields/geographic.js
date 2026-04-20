import _assignIn from 'lodash-es/assignIn'

import { Base } from './base'

export class Geographic extends Base {
	constructor(options) {
		super(
			_assignIn(
				{
					type: 'Geographic'
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return typeof value === 'object'
	}
}
