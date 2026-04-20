import { format, parse } from 'date-fns'
import cloneDeep from 'lodash-es/cloneDeep'
import _forEach from 'lodash-es/forEach'
import _get from 'lodash-es/get'
import _isArray from 'lodash-es/isArray'
import _isDate from 'lodash-es/isDate'
import _isEmpty from 'lodash-es/isEmpty'
import _set from 'lodash-es/set'
import tinycolor from 'tinycolor2'
import { isRef } from 'vue'

import { useDialog } from '@quidgest/ui'

import MessageBoxModel from '../../models/messageBoxModel.js'

import { formModes } from '../../constants/enums'

/**
 * Determines the readable text color based on the given background color.
 * @param {string} backgroundColor The background color for which to determine the readable text color
 * @returns {string} The readable text color (either 'black' or 'white').
 */
export function getReadableTextColor(backgroundColor) {
	// Create a tinycolor instance from the given background color
	const color = tinycolor(backgroundColor)
	// Calculate the luminance of the background color
	const luminance = color.getLuminance()
	// If the luminance is greater than 0.5, the background color is light, so use black as the text color
	// If the luminance is less than or equal to 0.5, the background color is dark, so use white as the text color
	return luminance > 0.5 ? 'black' : 'white'
}

/**
 * Changes the window's onbeforeunload event, so the user will be asked if he wants to leave, in case the form is dirty.
 * @param {boolean} isDirty True if the form is dirty, false otherwise
 */
export function setNavigationState(isDirty) {
	if (isDirty) window.onbeforeunload = () => ''
	else window.onbeforeunload = () => {}
}

/**
 * Normalize data for use in navigation parameters.
 * For now, only transforms Date objects to ISO strings within the input object.
 *
 * @param {Object|Array} obj - The input object or array that need be normalized.
 * @returns {Object|Array} A new object or array with normalized data.
 */
export function normalizeDataInNavigationParams(obj) {
	// Determine whether the input is an array or object, and initialize the output accordingly
	const newObj = Array.isArray(obj) ? [] : {}

	// Iterate through all keys in the input object or array
	for (const key in obj) {
		// If the current property is a Date object, convert it to an ISO string
		if (obj[key] instanceof Date) newObj[key] = dateToISOString(obj[key])
		// If the current property is a non-null object or array, call the function recursively
		else if (typeof obj[key] === 'object' && obj[key] !== null)
			newObj[key] = normalizeDataInNavigationParams(obj[key])
		// Otherwise, just copy the property to the output object or array
		else newObj[key] = obj[key]
	}

	// Return the output object or array
	return newObj
}

/**
 * Should be called whenever there's a route change, to update the navigation history.
 * @param {object} routeData The route where the user is navigating to
 */
export function normalizeRouteForSaveNavigation(routeData) {
	const routeBranch = routeData.meta.order
	const args = {
		routeType: routeData.meta.routeType,
		location: routeData.name,
		params: routeData.params,
		properties: {
			routeBranch: routeBranch || ''
		}
	}

	return args
}

/**
 * Should be called whenever there's a route change, to update the navigation history.
 * @param {object} routeData The route where the user is navigating to
 * @param {function} updateHistory A function that will update the navigation history
 */
export function saveNavigation(routeData, updateHistory) {
	if (typeof updateHistory !== 'function') return

	const args = normalizeRouteForSaveNavigation(routeData)

	updateHistory({ options: args })
}

/**
 * Converts the specified raw layout variables data to the format expected by the application.
 * @param {object} layoutConfig The raw layout variables data
 * @returns An object with variables in the expected format.
 */
export function getLayoutVariables(layoutConfig) {
	const layoutVariables = {}

	for (const i in layoutConfig) {
		const chosenValue = layoutConfig[i].chosen
		const defaultValue = layoutConfig[i].default
		const possibleValues = layoutConfig[i].values

		if (typeof chosenValue !== 'undefined') {
			if (Array.isArray(possibleValues) && possibleValues.includes(chosenValue))
				layoutVariables[i] = chosenValue
			else layoutVariables[i] = defaultValue
		} else layoutVariables[i] = defaultValue
	}

	return layoutVariables
}

/**
 * Displays a popup window with information, it can also receive some user input.
 *
 * @param {string} text The message to display
 * @param {string} icon The icon of the message (e.g.: info, error, warning, success, question) (optional)
 * @param {string} title The title of the message (optional)
 * @param {object} buttons The available buttons (optional)
 * @param {object} options Additional supported options (optional)
 * @param {object} handlers Event handlers (optional)
 */
export function displayMessage(text, icon, title, buttons, options, handlers) {
	const { addDialog } = useDialog()

	// Set object in store
	const messageBox = new MessageBoxModel(text, icon, title, buttons, options, handlers)

	addDialog(messageBox.props, undefined, undefined, messageBox.handlers)
}

/**
 * Scrolls to the top of the page.
 */
export function scrollToTop() {
	document.body.scrollTop = document.documentElement.scrollTop = 0
}

/**
 * Scrolls to the bottom of the page.
 */
export function scrollToBottom() {
	window.scrollTo(0, document.body.scrollHeight)
}

/**
 * Gets the starting Y coordinate of the visible scroll area.
 */
export function scrollYStart() {
	// Header bar height (only in vertical layout)
	const headerbar = document.getElementById('header-container')
	const headerbarHeight = headerbar ? headerbar.offsetHeight : 0

	// Main navigation bar height (only in horizontal layout)
	const mainNavbar = document.getElementById('main-header-navbar')
	const mainNavbarHeight = mainNavbar ? mainNavbar.offsetHeight : 0
	const mainNabarStyle = mainNavbar ? window.getComputedStyle(mainNavbar, null) : null

	// Visible navigation bar height
	const visibleNavbarHeight = mainNabarStyle?.display === 'none' ? 0 : mainNavbarHeight

	// Form header bar height (includes form buttons and horizontal anchors)
	const sticky = document.querySelector('#form-container > .c-sticky-header')
	const stickyHeaderHeight = sticky ? sticky.offsetHeight : 0

	return headerbarHeight + visibleNavbarHeight + stickyHeaderHeight
}

/**
 * Scrolls to the specified element.
 * @param {string} id The id of the element to scroll to
 * @param {string} position The scroll position - center or start
 * @param {string} behavior The behavior of the scroll, either instant or smooth
 */
export function scrollTo(id, position = 'center', behavior = 'smooth') {
	// The behavior property should be 'instant'
	// The 'smooth' option has a bug that prevents scrolling to the right location
	const options = {
		behavior: behavior,
		block: position
	}

	const elem = document.getElementById(id)
	if (!elem) return

	const initialSPT = document.documentElement.style.scrollPaddingTop

	// Calculate and set scroll padding
	// Must be done before scrolling
	const headerHeight = scrollYStart()
	document.documentElement.style.scrollPaddingTop = `${headerHeight}px`

	elem.scrollIntoView(options)
	elem.focus()

	document.documentElement.style.scrollPaddingTop = initialSPT
}

/**
 * Focus on a DOM element.
 * @param {object || string} target A reference to the DOM element or ID of the DOM element
 */
export function focusElement(target) {
	let el = target

	// If target is a string, get DOM element with that ID
	if (typeof target === 'string' && target !== '') el = document.getElementById(target)

	if (!el) return

	// If the element can be focused, focus on it
	if (el instanceof HTMLElement && typeof el.focus === 'function') el.focus()
}

/**
 * Focus on a DOM element.
 * @param {string} selector A CSS selector
 */
export function focusElementBySelector(selector)
{
	// Type check
	if (typeof selector !== 'string') return

	const el = document.querySelector(selector)

	if (typeof el?.focus !== 'function') return

	el.focus()
}

/**
 * A function similar, yet simpler, to "string.Format()" in C#:
 * https://docs.microsoft.com/en-us/dotnet/api/system.string.format
 * Can receive any number of parameters, the first of which must be the string to format.
 * @returns A formatted string.
 */
export function formatString() {
	if (arguments.length < 1) return ''

	let string = arguments[0]

	if (typeof string !== 'string' || string.length === 0) return ''
	if (arguments.length < 2) return string

	for (let i = 0; i < arguments.length - 1; i++) {
		const reg = new RegExp('\\{' + i + '\\}', 'gm')
		string = string.replace(reg, arguments[i + 1] ?? '')
	}

	return string
}

/**
 * Converts the specified date to a string.
 * @param {object} date The date that should be converted
 * @param {string} language The language to which the date should be converted
 * @param {string} defaultLang A fallback language, in case the first one is invalid (optional)
 * @returns The date in a string format.
 */
export function dateToString(date, currentLang, defaultLang) {
	if (!(date instanceof Date)) return ''

	const langs = [currentLang, defaultLang || 'en-GB']

	const options = {
		dateStyle: 'short',
		timeStyle: 'medium'
	}

	return new Intl.DateTimeFormat(langs, options).format(date)
}

/**
 * Alternative method of toISOString.
 * Returns a string in simplified extended ISO-8601 format but without specified time zone.
 * YYYY-MM-DDTHH:mm:ss.sss
 * @param {string} date The date time
 * @returns The date in an ISO string format.
 */
export function dateToISOString(date) {
	if (!(date instanceof Date)) return ''

	const pad = (number) => `${number < 10 ? '0' : ''}${number}`

	return (
		date.getFullYear() +
		'-' +
		pad(date.getMonth() + 1) +
		'-' +
		pad(date.getDate()) +
		'T' +
		pad(date.getHours()) +
		':' +
		pad(date.getMinutes()) +
		':' +
		pad(date.getSeconds()) +
		'.' +
		String(date.getMilliseconds()).padStart(3, '0')
	)
}

/**
 * Converts a simplified extended ISO-8601 date-time string without a time zone
 * into a Date object. The input format should match YYYY-MM-DDTHH:mm:ss.sss.
 * @param {string} isoString The date-time string in ISO-8601 format.
 * @returns {Date} A Date object representing the given date and time.
 */
export function isoStringToDate(isoString) {
	if (typeof isoString !== 'string') throw new Error('Input must be a string.')

	const isoDateTimeFormat =
		/^(\d{4})-(\d{2})-(\d{2})[T](\d{2}):(\d{2}):(\d{2})(\.\d+)?([+-]\d{2}:?\d{2}|Z)?$/
	const parts = isoString.match(isoDateTimeFormat)

	if (!parts) throw new Error('Invalid ISO-8601 date-time format.')

	// If the string ends with 'Z', treat it as UTC.
	if (parts[8] === 'Z') return new Date(isoString)
	else {
		// Extract components from the match.
		const year = parseInt(parts[1], 10),
			month = parseInt(parts[2], 10) - 1, // Adjust for 0-based index
			day = parseInt(parts[3], 10),
			hours = parseInt(parts[4], 10),
			minutes = parseInt(parts[5], 10),
			seconds = parseInt(parts[6], 10),
			// Parse milliseconds, if present; otherwise default to 0.
			milliseconds = parts[7] ? parseFloat(parts[7]) * 1000 : 0

		return new Date(year, month, day, hours, minutes, seconds, milliseconds)
	}
}

/**
 * Converts a string formatted as a date time to a Date object, based on a format string
 * @param {string} value A string formatted as a date in a configuration defined format
 * @param {string} dateFormat A string with the date format
 * @returns {Date} A Date object representing the given date and time.
 */
export function stringToDate(value, dateFormat) {
	// Date used to fill in missing pieces of parsed date
	const emptyDate = new Date(0, 0, 1, 0, 0, 0, 0)

	const date = parse(value, dateFormat, emptyDate)

	// If not a valid date
	if (isNaN(date.getTime())) return null

	// Valid date
	return date
}

/**
 * Converts the specified date time object to a string.
 * @param {object} time The date time object to be converted
 * @returns The time in a string format like: HH:mm.
 */
export function timeToString(time) {
	const hours = time?.hours?.toString().padStart(2, '0') || '00'
	const minutes = time?.minutes?.toString().padStart(2, '0') || '00'

	return `${hours}:${minutes}`
}

/**
 * Checks if the provided object has the keys 'hours', 'minutes', and 'seconds'.
 * @param {Object} obj - The object to check for time properties.
 * @returns {boolean} Returns true if the object has all three keys, otherwise false.
 */
export function hasTimeProperties(obj) {
	if (typeof obj !== 'object' || _isEmpty(obj)) return false

	return 'hours' in obj && 'minutes' in obj && 'seconds' in obj
}

/**
 * Checks if the specified value is a date.
 * @param {any} value The value
 * @returns True if it's a date, false otherwise.
 */
export function isDate(value) {
	return _isDate(value)
}

/**
 * Checks if the specified value is an image.
 * @param {object|string} value - The image representation, either as an object or string (url)
 * @returns True if it's a valid image, false otherwise.
 */
export function validateImageFormat(value) {
	return typeof value === 'object'
		? value === null || ('data' in value && 'dataFormat' in value && 'encoding' in value)
		: // A string value means it can be either a path for the image or a base64 representation of one.
		typeof value === 'string' && value.includes('/')
}

/**
 * Converts a given image's source to string format.
 * @param {object|string} data The raw image data or URL
 * @returns {string} The image's data in string format.
 */
export function imageObjToSrc(data) {
	// URL
	if (typeof data === 'string') return data
	else if (typeof data !== 'object' || data === null) return null
	// Base64 / inline
	return `data:image/${data.dataFormat};${data.encoding},${data.data}`
}

/**
 * Computes a placeholder color according to the colors of the specified image source.
 * @param {object} src The image source
 */
export async function computeColorPlaceholder(src) {
	if (_isEmpty(src)) return null

	return new Promise((resolve, reject) => {
		let img = new Image()

		img.onload = () => {
			const canvas = document.createElement('canvas')

			if (canvas) {
				canvas.width = img.width
				canvas.height = img.height

				const context = canvas.getContext('2d', { willReadFrequently: true })
				context.drawImage(img, 0, 0, img.width, img.height)

				const x1 = parseInt(img.width * 0.2)
				const x2 = parseInt(img.width * 0.8)
				const y1 = parseInt(img.height * 0.2)
				const y2 = parseInt(img.height * 0.8)

				const sample1 = context.getImageData(x1, y1, 1, 1).data
				const sample2 = context.getImageData(x1, y2, 1, 1).data
				const sample3 = context.getImageData(x2, y1, 1, 1).data
				const sample4 = context.getImageData(x2, y2, 1, 1).data

				const avgR = (sample1[0] + sample2[0] + sample3[0] + sample4[0]) / 4
				const avgG = (sample1[1] + sample2[1] + sample3[1] + sample4[1]) / 4
				const avgB = (sample1[2] + sample2[2] + sample3[2] + sample4[2]) / 4

				img = null

				const rgb = `rgb(${parseInt(avgR)} ${parseInt(avgG)} ${parseInt(avgB)})`

				resolve(rgb)
			}
		}

		img.onerror = (error) => {
			reject(error)
		}

		// This will trigger the `onload`
		img.src = src
	})
}

/* BEGIN: Formatting field data to display */

/**
 * Class representing the formatted value.
 * Includes the original value so we can use it in tooltips for example.
 */
export class FormattedValueToDisplay {
	constructor(value, originalValue) {
		this.value = value
		this.originalValue = originalValue
	}

	toString() {
		return this.value
	}
}

/**
 * The formatted string. In the case of multiple values, it will be a FormattedValueToDisplay object with both a short and a full string.
 * @param {any|Array.<any>} value The value to be formatted. It can be a single value or an array of values.
 * @param {function} fnFormat The formatting function.
 * @returns {any|Array.<any>} The formatted value or the original value if no formatting function is provided.
 */
export function formatValueToDisplay(value, fnFormat) {
	if (typeof fnFormat === 'function') {
		if (Array.isArray(value)) return value.map((item) => fnFormat(item))
		return fnFormat(value)
	}

	return value
}

/**
 * Get formatted string representing text
 * @param {string} value  The value to format
 * @param {object} options [optional] Optional formatting options.
 * @returns {string|FormattedValueToDisplay} The formatted string. In the case of multiple values, it will be a FormattedValueToDisplay object with both a short and a full string.
 */
export function textDisplay(value, options) {
	value = _isEmpty(value) ? '' : value

	// Optional options
	if (
		Number.isInteger(options?.scrollData) &&
		value.length > options.scrollData &&
		!options?.isHtml
	) {
		const shortText = value.substring(0, options.scrollData) + ' (...)'
		value = options.multipleValues ? new FormattedValueToDisplay(shortText, value) : shortText
	}

	return value
}

/**
 * Determine if a number is negative.
 * @param {string|number} value - The number
 * @returns {boolean} Whether the number is negative or not
 */
export function isNegative(value) {
	if (typeof value === 'number')
		return value < 0

	if (typeof value === 'string' && value?.length > 0)
		return value.at(0) === '-' || (value.at(0) === '(' && value.at(0) === ')')

	return false
}

/**
 * Formats a number's negative display.
 * @param {string|number} value - The number to format
 * @param {string} negativeFormat - The format to use for negative numbers (e.g., '-')
 * @param {boolean} makeNegative - Whether to make the number negative
 * @returns {string} The formatted number as a string
 */
export function numericNegativeDisplay(value, negativeFormat, makeNegative) {
	let strValue = value ? value.toString() : ''

	if (makeNegative || isNegative(value))
	{
		// Remove negative symbols since they will be added based on the parameter
		strValue = strValue.replace(/-/g, '').replace(/\(/g, '').replace(/\)/g, '')

		switch(negativeFormat)
		{
			case '()':
				strValue = '(' + strValue + ')'
				break
			case '-':
			default:
				strValue = '-' + strValue
				break
		}
	}

	return strValue
}

/**
 * Formats a number with custom thousands and decimal separators.
 * @param {string|number} value - The number to format
 * @param {string} decimalSep - The character to use as decimal separator (e.g., ',')
 * @param {string} groupSep - The character to use as thousands separator (e.g., '.')
 * @param {string} negativeFormat - The format to use for negative numbers (e.g., '-')
 * @param {object} numberFormatOptions - Extra formatting options
 * @returns {string} The formatted number as a string
 *
 * @example
 * numericDisplay(5453.04, '.', ',') // "5.453,04"
 * numericDisplay(5000000, '.', ',') // "5.000.000"
 */
export function numericDisplay(value, decimalSep, groupSep, negativeFormat, numberFormatOptions) {
	let strValue = ''

	if (numberFormatOptions !== undefined)
		strValue = new Intl.NumberFormat('en-US', numberFormatOptions).format(value)
	else strValue = new Intl.NumberFormat('en-US').format(value)

	strValue = strValue.replace(/,/g, ';').replace('.', ':')
	strValue = strValue.replace(':', decimalSep ?? '.').replace(/;/g, groupSep ?? ',')

	strValue = numericNegativeDisplay(strValue, negativeFormat)

	return strValue
}

/**
 * Get formatted string representing a currency
 * @param value {string}
 * @param decimalSep {string}
 * @param groupSep {string}
 * @param negativeFormat {string}
 * @param decimalPlaces {Number}
 * @param currencyCode {string}
 * @param lcidCode {string}
 * @param symbolType {string}
 * @returns String
 */
export function currencyDisplay(
	value,
	decimalSep,
	groupSep,
	negativeFormat,
	decimalPlaces,
	currencyCode,
	lcidCode,
	symbolType
) {
	// Optional symbol type: "symbol", "narrowSymbol", "code", "name"
	let symbolTypeUse = 'symbol'
	if (symbolType !== undefined) symbolTypeUse = symbolType

	const valueIsNegative = isNegative(value)
	const valueAbsolute = Math.abs(parseFloat(value))

	// Get number formatted according to location code without currency symbol
	const strNumber = new Intl.NumberFormat(lcidCode, {
		minimumFractionDigits: decimalPlaces,
		maximumFractionDigits: decimalPlaces
	}).format(valueAbsolute)
	// Get number formatted according to location code with currency symbol
	const strCurrency = new Intl.NumberFormat(lcidCode, {
		minimumFractionDigits: decimalPlaces,
		maximumFractionDigits: decimalPlaces,
		style: 'currency',
		currency: currencyCode.toUpperCase(),
		currencyDisplay: symbolTypeUse
	}).format(valueAbsolute)
	// Get number formatted according to application configuration
	const strNumberDisplay = numericDisplay(valueAbsolute, decimalSep, groupSep, negativeFormat, {
		minimumFractionDigits: decimalPlaces,
		maximumFractionDigits: decimalPlaces
	})

	// In number formatted according to location code with currency symbol, replace number with number formatted according to application configuration
	let strValue = strCurrency.replace(strNumber, strNumberDisplay)
	if (valueIsNegative)
		strValue = numericNegativeDisplay(strValue, negativeFormat, true)

	return strValue
}

/**
 * Get formatted string representing a date/time
 * @param dateTime {string, object}
 * @param dateTimeFormat {string}
 * @returns String
 */
export function dateDisplay(dateTime, dateTimeFormat) {
	let date

	if (_isDate(dateTime)) date = dateTime
	else if (typeof dateTime === 'string' || dateTime instanceof String) {
		// NULL dates
		if (isEmpty(dateTime)) return ''

		// Parse
		date = new Date(dateTime)

		// If date is invalid, return raw value
		if (isNaN(date.getTime())) return ''
	} else return ''

	// If date is valid, return formatted value
	return format(date, dateTimeFormat ?? 'dd/MM/yyyy HH:mm:ss')
}

/**
 * Get formatted string representing a boolean
 * @param value {string}
 * @returns Boolean
 */
export function booleanDisplay(value) {
	if (typeof value === 'boolean') return value
	else if (typeof value === 'number') return value !== 0
	else if (typeof value === 'string') return value.toLowerCase() === 'true'

	return false
}

/**
 * Get formatted string representing an image.
 * @param {object|string} value - The image representation, either as an object or string (url)
 * @returns {string|object} - Formatted string or object representing the image.
 */
export function imageDisplay(value) {
	if (validateImageFormat(value)) return value
	return ''
}

/**
 * Get formatted string representing a document
 * @param value {object}
 * @param options {object} [optional]
 * @returns String || Object
 */
export function documentDisplay(value, options) {
	const rtnValue = cloneDeep(value)

	if (_isEmpty(rtnValue)) return null

	// Optional options
	if (!_isEmpty(options)) {
		// Column scroll
		if (options.scrollData !== undefined && rtnValue.fileName.length > options.scrollData)
			rtnValue.fileName = rtnValue.fileName.substring(0, options.scrollData) + ' (...)'

		// Output object instead of string
		if (options.outputObject === true) return rtnValue
	}

	return _get(rtnValue, 'fileName', null)
}

/**
 * Get formatted string representing a radio button
 * @param value {object}
 * @returns String || Object
 */
export function radioDisplay(value) {
	return cloneDeep(value)
}

/**
 * Get value from an enumeration
 * @param enumeration {object}
 * @param value {string}
 * @returns String
 */
export function getValueFromEnumeration(enumeration, value) {
	return _get(enumeration, value, '')
}

/**
 * Get formatted string representing a value from an enumeration
 * @param enumeration {object}
 * @param value {string}
 * @returns String
 */
export function enumerationDisplay(enumeration, value) {
	return getValueFromEnumeration(enumeration, value)
}

/* END: Formatting field data to display */

/**
 * Checks if value is empty. A value is considered empty unless it's an arguments object, array, string, or
 * jQuery-like collection with a length greater than 0 or an object with own enumerable properties.
 * @param value The value to inspect.
 * @return Returns true if value is empty, else false.
 */
export function isEmpty(value) {
	return isDate(value) ? false : _isEmpty(value)
}

/**
 * Merge arrays, accounting for whether they are reactive or not.
 * @param {object/array} objValue Destination array
 * @param {object/array} srcValue Source array
 * @returns An array with all the elements.
 */
export function mergeOptions(objValue, srcValue) {
	if (isRef(objValue)) {
		if (_isArray(objValue.value)) {
			objValue.value.push(...(srcValue || []))
			return objValue
		}
	} else if (_isArray(objValue) && _isArray(srcValue)) {
		objValue.push(...(srcValue || []))
		return objValue
	}
}

/**
 * Validates whether `requiredTexts` is a subset of `texts`.
 * @param {object} requiredTexts The required texts
 * @param {object} texts The provided texts
 * @returns True if `requiredTexts` is a subset of `texts`, false otherwise.
 */
export function validateTexts(requiredTexts, texts) {
	const requiredKeys = Object.keys(requiredTexts)
	const textsKeys = new Set(Object.keys(texts))
	return requiredKeys.every((key) => textsKeys.has(key))
}

/**
 * Checks if the specified file has a valid extension and size, according to the provided parameters.
 * @param {object} file The file to check
 * @param {array} extensions The allowed extensions (an empty list means all extensions are valid)
 * @param {number} maxSize The max allowed size, in bytes (0 means there's no max size)
 * @returns 0 if the specified file is valid, 1 if it's extension is invalid and 2 if it's size is invalid.
 */
export function validateFileExtAndSize(file, extensions = [], maxSize = 0) {
	if (typeof file !== 'object') return -1

	const fileExtension = '.' + file?.name.split('.')?.pop()?.toLowerCase()
	const exts = extensions.map((ext) => ext.toLowerCase())

	if (extensions.length > 0 && !exts.includes(fileExtension)) return 1

	if (maxSize > 0 && file?.size > maxSize) return 2

	return 0
}

/**
 * Displays a message to the user, according to the provided error code when uploading a file.
 * @param {number} error The error code
 * @param {object} errorTexts An object with the error messages
 * @param {object} extraInfo Extra info to display in the error message
 */
export function handleFileError(error, errorTexts = {}, extraInfo = {}) {
	let errorMsg

	switch (error) {
		// Invalid extension.
		case 1:
			if (
				typeof errorTexts.extensionError === 'string' &&
				Array.isArray(extraInfo.extensions) &&
				extraInfo.extensions.length > 0
			)
				errorMsg = `${errorTexts.extensionError} ${extraInfo.extensions.join(', ')}`
			break
		// Invalid size.
		case 2:
			if (
				typeof errorTexts.fileSizeError === 'string' &&
				typeof extraInfo.maxSize === 'string'
			)
				errorMsg = formatString(errorTexts.fileSizeError, extraInfo.maxSize)
			break
	}

	if (errorMsg) displayMessage(errorMsg, 'error')
}

/**
 * Checks if the user has permission to execute the specified action.
 * @param {object} permissions The button permissions
 * @param {string} actionType The action type
 * @returns True if the user has permission, false otherwise.
 */
export function btnHasPermission(permissions, actionType) {
	if (!permissions || typeof permissions !== 'object' || typeof actionType !== 'string')
		return false

	switch (actionType.toUpperCase()) {
		case formModes.show:
			return permissions.viewBtnDisabled !== true
		case formModes.edit:
			return permissions.editBtnDisabled !== true
		case formModes.duplicate:
			return permissions.duplicateBtnDisabled !== true
		case formModes.delete:
			return permissions.deleteBtnDisabled !== true
		case formModes.new:
		case 'INSERT' /* There should never be an INSERT option, but the ID of this button is already scattered around the templates. */:
			return permissions.insertBtnDisabled !== true
	}

	return true
}

/**
 * Retrieves the modes parameter for a given mode.
 * This is meant to be used as a default when no other modes are defined.
 * @param {object} mode The mode the form will be opened with
 * @returns The mode to open the form.
 */
export function getDefaultFormModesForMode(mode) {
	switch (mode) {
		case formModes.show:
			return 'v'
		case formModes.edit:
			return 'e'
		case formModes.duplicate:
			return 'd'
		case formModes.delete:
			return 'a'
		case formModes.new:
		case 'INSERT':
			return 'i'
	}
	return ''
}

/**
 * Creates a JavaScript Model structure.
 * @param {Object} row - Data in the format { 'area.field': value }
 * @param {Object} objectStructure - Object structure in the format: 'Area.ValField': () => (rowFields) => rowFields['area.field']
 * @returns {Object} - The created Model structure object.
 */
export function getModelStructureObj(row, objectStructure) {
	const obj = {}
	_forEach(objectStructure, (fnValueSelector, modelPath) =>
		_set(obj, modelPath, fnValueSelector(row))
	)
	return obj
}

/**
 * Given the field and table of the column, formats the information into a unique identifier.
 * @param {string} columnArea The area of the column's field.
 * @param {string} columnField The column's field.
 * @returns The list column identifier (of type 'table.field')
 */
export function formatColumnIdentifier(columnArea, columnField) {
	return `${columnArea.toLowerCase()}.${columnField.toLowerCase()}`
}

/**
 * Creates a debounced version of an async function that also deduplicates in-flight calls with the same arguments.
 * @param {Function} fn - The async function to debounce and dedupe
 * @param {Object} options - Configuration options
 * @param {Function} [options.getKey] - Function to generate a key from arguments for dedupe, default is JSON.stringify
 * @param {number} [options.wait=500] - Debounce wait time in milliseconds
 * @param {boolean} [options.leading=false] - If true, execute on the leading edge (lodash default: false)
 * @param {boolean} [options.trailing=true] - If true, execute on the trailing edge (lodash default: true)
 * @returns {Function} A debounced async function that returns a Promise.
 *
 * Usage:
 * const fetchUserDeduped = dedupe(fetchUser, { wait: 500 })
 * await fetchUserDeduped(1) // fires after 500ms of inactivity, deduped if same args are in-flight
 */
export function dedupe(
	fn,
	{
		getKey = (...args) => JSON.stringify(args),
		wait = 500,
		leading = false,
		trailing = true
	} = {}
) {
	// Map to track in-flight Promises for dedupe
	const inFlight = new Map()

	// Timer for scheduling trailing calls
	let timer = null

	// Last arguments and context for trailing call
	let lastArgs = null
	let lastContext = null

	// Timestamp of last execution
	let lastCallTime = 0

	// Queue of resolve/reject for Promises waiting on the next execution
	let pendingPromises = []

	// Executes the async function with dedupe
	async function execute(args, context) {
		const key = getKey(...args)

		// If a request with the same key is in-flight, reuse it
		if (inFlight.has(key)) {
			return inFlight.get(key)
		}

		// Execute the function and track it in-flight
		const promise = (async () => {
			try {
				return await fn.apply(context, args)
			} finally {
				inFlight.delete(key)
			}
		})()

		inFlight.set(key, promise)
		return promise
	}

	// Runs all queued promises with the current args/context
	function flush(args, context) {
		// Copy and clear queue
		const promises = pendingPromises.slice()
		pendingPromises = []

		// Execute function and resolve all queued Promises
		const result = execute(args, context)
		promises.forEach(({ resolve, reject }) => {
			result.then(resolve).catch(reject)
		})
	}

	return function (...args) {
		lastArgs = args
		lastContext = this

		return new Promise((resolve, reject) => {
			// Queue this call's resolve/reject
			pendingPromises.push({ resolve, reject })

			const now = Date.now()
			const elapsed = now - lastCallTime

			// Determine if we should call on the leading edge
			const callLeading = leading && elapsed >= wait

			// Leading call executes immediately if enough time has passed
			if (callLeading) {
				lastCallTime = now
				flush(lastArgs, lastContext)
				return
			}

			// Clear previous trailing timer if any
			if (timer) {
				clearTimeout(timer)
			}

			// Schedule trailing call if enabled
			if (trailing) {
				// Remaining time until trailing call
				const remaining = Math.max(wait - elapsed, 0)

				timer = setTimeout(() => {
					lastCallTime = Date.now()
					timer = null

					// Execute the trailing call
					flush(lastArgs, lastContext)
				}, remaining)
			}
		})
	}
}

/**
 * Get the name of a heading tag based on a level.
 * @param {string} level The level.
 * @returns The tag name
 */
export function getHeadingTagNameByLevel(level) {
	// Heading tags go up to 6
	// For levels above 6 (very unlikely), a div element is used
	return level > 6 ? 'div' : 'h' + level
}

export default {
	getReadableTextColor,
	setNavigationState,
	normalizeDataInNavigationParams,
	normalizeRouteForSaveNavigation,
	saveNavigation,
	getLayoutVariables,
	displayMessage,
	scrollToTop,
	scrollToBottom,
	scrollYStart,
	scrollTo,
	formatString,
	dateToString,
	dateToISOString,
	isoStringToDate,
	stringToDate,
	timeToString,
	hasTimeProperties,
	isDate,
	validateImageFormat,
	imageObjToSrc,
	computeColorPlaceholder,
	formatValueToDisplay,
	textDisplay,
	isNegative,
	numericNegativeDisplay,
	numericDisplay,
	currencyDisplay,
	dateDisplay,
	booleanDisplay,
	imageDisplay,
	documentDisplay,
	enumerationDisplay,
	radioDisplay,
	isEmpty,
	mergeOptions,
	validateTexts,
	validateFileExtAndSize,
	handleFileError,
	btnHasPermission,
	getModelStructureObj,
	formatColumnIdentifier,
	focusElement,
	focusElementBySelector,
	getDefaultFormModesForMode,
	dedupe,
	getHeadingTagNameByLevel
}
