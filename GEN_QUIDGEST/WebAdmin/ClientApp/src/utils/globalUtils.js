import moment from 'moment'
// JavaScript extensions
/**
 * Same as .Net string.format
 * @returns {string} - Formated string
 */
//String.format = function () {
export function string_format() {
	var s = arguments[0]
	for (var i = 0; i < arguments.length - 1; i++) {
		var reg = new RegExp('\\{' + i + '\\}', 'gm')
		s = s.replace(reg, arguments[i + 1])
	}

	return s
}

/**
 * POST data to Server
 * @param {string} url - Action URL
 * @param {Object} data - Data in Json format
 * @param {Function=} callback - Callback function
 * @returns {DeferredPermissionRequest} - Request callback
 */
export function postJSON(url, data, callback) {
	if (!$.isEmptyObject(data) && typeof data === 'object') {
		$.each(data, function (index, value) {
			if (!$.isEmptyObject(value) && typeof value === 'object' && value._isAMomentObject) {
				data[index] = value.toISOString()
			}
		})
	}

	return $.ajax({
		type: 'POST',
		url: url,
		dataType: 'json',
		contentType: 'application/json; charset=UTF-8',
		data: JSON.stringify(data),
		success: callback,
		cache: false,
		traditional: true,
		// Override jQuery's strict JSON parsing
		converters: {
			'text json': function (result) {
				try {
					// First try to use native browser parsing
					if (typeof JSON === 'object' && typeof JSON.parse === 'function') {
						return JSON.parse(result, function (key, value) {
							if (typeof value === 'string') {
								// Convert .Net DateTime to JavaScript date format (momentjs)
								var patternCSharp = /Date\(([^)]+)\)/,
									patternJSON =
										/(\d{4}-\d{2}-\d{2})[T](\d{2}:\d{2}:\d{2}.?(\d{3})?)[Z]?/
								if (patternCSharp.test(value)) {
									return moment.utc(value)
								} else if (patternJSON.test(value)) {
									return moment.utc(value)
								} else {
									return value
								}
							}
							return value
						})
					} else {
						// Fallback to jQuery's parser
						console.warn('parseJSON', result)
						return $.parseJSON(result)
					}
				} catch {
					console.log('Warning: Could not parse expected JSON response.')
					return {} // Empty JS object.
				}
			}
		}
	})
}
