import isEmpty from 'lodash-es/isEmpty'

import { numericDisplay } from '../genericFunctions'

/**
 * Checks if the specified value is a coordinate (should be something like "POINT(0 0)").
 * @param {string} value - The coordinate
 * @returns True if it's a valid coordinate, false otherwise.
 */
export function validateCoordinate(value) {
	if (typeof value !== 'string') return false
	if (isEmpty(value)) return true
	if (!value.startsWith('POINT(') || !value.endsWith(')')) return false

	try {
		const coords = value.split('(')[1].split(')')[0].split(' ')
		return !isNaN(coords[0]) && !isNaN(coords[1])
	} catch {
		return false
	}
}

/**
 * Get formatted string representing a geographic coordinate.
 * @param {string|object} value - The geographic coordinate
 * @param {string} decimalSep - The decimal separator to use in numbers(e.g., ',')
 * @param {string} groupSep - The group separator to use in numbers (e.g., '.')
 * @param {string} negativeFormat - The format to use for negative numbers (e.g., '-')
 * @returns A string representation of the specified coordinate.
 */
export function geographicDisplay(value, decimalSep = '.', groupSep = '', negativeFormat = '-') {
	if (typeof value === 'string') {
		if (!validateCoordinate(value)) return ''
		return value
	}
	if (isEmpty(value) || typeof value.Lat !== 'number' || typeof value.Long !== 'number') return ''

	const x = numericDisplay(value.Lat, decimalSep, groupSep, negativeFormat, {
		minimumFractionDigits: 0,
		maximumFractionDigits: 20
	})
	const y = numericDisplay(value.Long, decimalSep, groupSep, negativeFormat, {
		minimumFractionDigits: 0,
		maximumFractionDigits: 20
	})

	return `POINT(${y} ${x})`
}

/**
 * Get formatted string representing a geographic shape.
 * @param {string|object} value - The geographic shape
 * @returns A string representation of the specified geographic shape.
 */
export function geographicShapeDisplay(value) {
	return typeof value === 'string' ? value : ''
}
