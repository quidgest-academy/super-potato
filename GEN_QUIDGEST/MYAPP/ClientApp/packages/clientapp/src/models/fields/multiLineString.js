import _assignIn from 'lodash-es/assignIn'

import { String } from './string'

export class MultiLineString extends String {
	constructor(options) {
		super(
			_assignIn(
				{
					// No limit (varchar max)
					maxLength: -1
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	get serverValue() {
		// The server expects \r\n, but text edited through web textarea only has \n. So we convert
		// it first from server format to web format, in case the text came from the server and wasn't edited.
		const value = this.value?.replaceAll('\r\n', '\n')
		// Convert to server format.
		return value?.replaceAll('\n', '\r\n')
	}
}
