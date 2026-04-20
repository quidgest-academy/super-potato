import { createTestingPinia } from '@pinia/testing'
import { beforeEach, describe, expect, it } from 'vitest'

import { MultiLineString } from '../multiLineString'

describe('MultiLineString', () => {
	let string

	beforeEach(() => {
		createTestingPinia()
		string = new MultiLineString()
	})

	it.each([null, undefined, ''])(
		'Empty text is correctly displayed when the value is %s',
		(emptyValue) => {
			string.updateValue(emptyValue)

			expect(string.value).toStrictEqual('')
			expect(string.displayValue).toStrictEqual('')
			expect(string.serverValue).toStrictEqual('')
		}
	)

	it('Format of a string with no new lines is the same in the server and web', () => {
		const text = 'Sample single-line text.'
		string.updateValue(text)

		expect(string.value).toStrictEqual(text)
		expect(string.displayValue).toStrictEqual(text)
		expect(string.serverValue).toStrictEqual(text)
	})

	it('Format of a string with new lines, created in the web, should be sent to the server with \\r\\n', () => {
		const text =
			'Sample multi-line text.\nThis text was created in a web form.\nWhich is why it lacks the carriage return character.'
		const serverSideText =
			'Sample multi-line text.\r\nThis text was created in a web form.\r\nWhich is why it lacks the carriage return character.'
		string.updateValue(text)

		expect(string.value).toStrictEqual(text)
		expect(string.displayValue).toStrictEqual(text)
		expect(string.serverValue).toStrictEqual(serverSideText)
	})

	it('Format of a string with new lines, created in a windows application, is sent to the server with \\r\\n', () => {
		const text =
			'Sample multi-line text.\r\nThis text was created in Windows.\r\nWhich is why it has the carriage return character.'
		string.updateValue(text)

		expect(string.value).toStrictEqual(text)
		// Web text only removes \r when the text is edited.
		expect(string.displayValue).toStrictEqual(text)
		expect(string.serverValue).toStrictEqual(text)
	})
})
