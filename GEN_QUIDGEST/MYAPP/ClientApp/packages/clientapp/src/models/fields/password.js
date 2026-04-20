import _assignIn from 'lodash-es/assignIn'

import { String } from './string'

export class Password extends String {
	constructor(options) {
		super(
			_assignIn(
				{
					type: 'Password',
					maxLength: -1
				},
				options
			)
		)
	}
}
