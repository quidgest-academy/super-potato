import _assignIn from 'lodash-es/assignIn'

import { validateCoordinate } from '../../utils/geography'
import { String } from './string'

export class Coordinate extends String {
	constructor(options) {
		super(
			_assignIn(
				{
					type: 'Coordinate'
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return super.isValidType(value) && validateCoordinate(value)
	}
}
