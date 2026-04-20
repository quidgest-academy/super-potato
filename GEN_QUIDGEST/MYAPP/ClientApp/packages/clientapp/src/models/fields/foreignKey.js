import _assignIn from 'lodash-es/assignIn'

import { PrimaryKey } from './primaryKey'

export class ForeignKey extends PrimaryKey {
	constructor(options) {
		super(
			_assignIn(
				{
					relatedArea: null
				},
				options
			)
		)
	}
}
