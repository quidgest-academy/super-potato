import _assignIn from 'lodash-es/assignIn'

import { useGenericDataStore } from '../../stores/genericData'
import { DateTime } from './dateTime'

export class DateTimeSeconds extends DateTime {
	constructor(options) {
		const genericDataStore = useGenericDataStore()

		super(
			_assignIn(
				{
					type: 'DateTimeSeconds',
					dateFormat: genericDataStore.dateFormat.dateTimeSeconds
				},
				options
			)
		)
	}
}
