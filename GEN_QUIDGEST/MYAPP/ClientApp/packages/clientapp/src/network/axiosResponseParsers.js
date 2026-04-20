import { isValid } from 'date-fns'

import { isoStringToDate } from '../utils/genericFunctions'

/**
 * Regular expression to validate ISO 8601 date-time format.
 * Supports basic and extended formats with optional milliseconds and timezone information.
 */
const isoDateTimeFormat = /^(\d{4}-\d{2}-\d{2})[T](\d{2}:\d{2}:\d{2})(\.\d+)?([+-]\d{2}:?\d{2}|Z)?$/

/**
 * Checks if the provided value is a valid ISO 8601 date-time string.
 * @param {string} value - The value to be tested.
 * @returns {boolean} True if the value matches the ISO 8601 format, false otherwise.
 */
function isIsoDateTimeString(value) {
	return value && typeof value === 'string' && isoDateTimeFormat.test(value)
}

/**
 * Parses an ISO date-time string to a Date object.
 * If the input date is '0001-01-01T00:00:00' or invalid, it returns an empty string.
 * @param {string} value - The ISO date-time string to parse.
 * @returns {Date|string} A Date object or an empty string for invalid inputs.
 */
function parseISODateTime(value) {
	if (value === '0001-01-01T00:00:00' || !isValid(new Date(value))) return ''
	return isoStringToDate(value)
}

/**
 * Recursively processes an object to convert all ISO date-time strings to Date objects in UTC.
 * @param {Object} body - The object containing potential ISO date-time strings.
 * @returns {Object} The original object with ISO date-time strings converted to Date objects.
 */
export function handleDates(body) {
	if (body === null || body === undefined || typeof body !== 'object') return body

	for (const key of Object.keys(body)) {
		// Don't run on table configuration because converting dates in filter values will cause other problems
		if (key === 'currentTableConfig') continue

		const value = body[key]
		if (isIsoDateTimeString(value)) {
			// Converts and assigns the parsed Date object back to the property.
			body[key] = parseISODateTime(value)
		} else if (typeof value === 'object') {
			// Recursively process nested objects.
			handleDates(value)
		}
	}
}
