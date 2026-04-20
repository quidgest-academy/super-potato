import _assignIn from 'lodash-es/assignIn'

import { useGenericDataStore } from '../../stores/genericData'
import { Date } from './date'

export class DateTime extends Date {
	constructor(options) {
		const genericDataStore = useGenericDataStore()

		super(
			_assignIn(
				{
					type: 'DateTime',
					dateFormat: genericDataStore.dateFormat.dateTime
				},
				options
			)
		)
	}
}
