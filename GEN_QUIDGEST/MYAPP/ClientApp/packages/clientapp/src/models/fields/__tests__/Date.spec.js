import { createTestingPinia } from '@pinia/testing'
import { beforeEach, describe, expect, it } from 'vitest'

import { dateDisplay, dateToISOString } from '../../../utils/genericFunctions'
import { Date } from '../date'

describe('Date', () => {
	let date

	beforeEach(() => {
		createTestingPinia()
		date = new Date()
	})

	it.each([null, undefined, ''])(
		'Empty date is correctly displayed when the value is %s',
		(emptyValue) => {
			date.updateValue(emptyValue)

			expect(date.value).toStrictEqual(Date.EMPTY_VALUE)
			expect(date.displayValue).toStrictEqual('')
			expect(date.serverValue).toStrictEqual('')
		}
	)

	it('Valid date is correctly displayed', () => {
		const validDate = new window.Date('2024-06-06T18:38:00')
		date.updateValue(validDate)

		expect(date.value).toStrictEqual(validDate)
		expect(date.displayValue).toStrictEqual(dateDisplay(date.value, date.dateFormat))
		expect(date.serverValue).toStrictEqual(dateToISOString(date.value))
	})

	it('Invalid date reverts to empty default', () => {
		const invalidDate = new window.Date('2024-15-06T39:70:00')
		date.updateValue(invalidDate)

		expect(date.value).toStrictEqual(Date.EMPTY_VALUE)
		expect(date.displayValue).toStrictEqual('')
		expect(date.serverValue).toStrictEqual('')
	})
})
