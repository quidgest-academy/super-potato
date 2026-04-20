import _assignIn from 'lodash-es/assignIn'

import { validateImageFormat } from '../../utils/genericFunctions'
import { Base } from './base'

export class Image extends Base {
	constructor(options) {
		super(
			_assignIn(
				{
					type: 'Image'
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return validateImageFormat(value)
	}
}
