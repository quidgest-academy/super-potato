import _forEach from 'lodash-es/forEach'
import _isEmpty from 'lodash-es/isEmpty'
import { v4 as uuidv4 } from 'uuid'

import { useTracingDataStore } from '../stores/tracingData'
import {
	fetchData as _fetchData,
	getFile as _getFile,
	postData as _postData,
	uploadFile as _uploadFile,
	forceDownload
} from './core'
import { MAIN_HISTORY_BRANCH_ID } from './constants'
import { apiActionURL, getHistoryToSend } from './utils'

/**
 * Execution of the GET request to the server
 * @param {String} controller The controller name
 * @param {String} action The action name
 * @param {Object} params Object with additional parameters (Query String)
 * @param {Callback} _fnCallback The request callback to be executed to process the data received from the server
 * @param {Callback} _fnErrorCallback The request callback to be executed in the case of failure / error
 * @param {Object} options The Axios additional options
 * @param {string} navigationId The Navigation context Id
 * @returns The «Promise» object that is resolved
 */
export function fetchData(
	controller,
	action,
	params,
	_fnCallback,
	_fnErrorCallback,
	options,
	navigationId
) {
	const tracing = useTracingDataStore()
	const url = apiActionURL(controller, action)
	const traceId = options?.meta?.traceId || uuidv4()

	if (!options) options = {}
	if (!options.meta) options.meta = {}
	options.meta.traceId = traceId

	tracing.addRequestTrace({
		origin: 'fetchData',
		requestType: 'get',
		requestUrl: url,
		requestParams: { ...params, nav: navigationId },
		contextData: {
			controller,
			action,
			options
		},
		traceId: traceId
	})

	return _fetchData(
		controller,
		action,
		params,
		(data, response) => {
			const tracing = useTracingDataStore()
			tracing.addResponseTrace({
				traceId: options?.meta?.traceId,
				origin: 'fetchData',
				requestType: response.config.method,
				requestUrl: response.config.url,
				requestParams: response.config.params,
				requestData: response.config.data,
				responseStatus: response.status,
				responseData: response.data,
				meta: response.config.meta
			})

			// Handle server errors tracing
			const srvEventTracking = response.data?.eTracker
			if (srvEventTracking) {
				tracing.addServerErrors({
					traceId: options?.meta?.traceId,
					errors: srvEventTracking,
					contextData: {
						requestType: response.config.method,
						requestUrl: response.config.url,
						requestParams: response.config.params,
						requestData: response.config.data
					}
				})
			}

			// Call the original callback
			if (_fnCallback) return _fnCallback(data, response)
		},
		(error, response) => {
			const tracing = useTracingDataStore()
			tracing.addError({
				origin: 'fetchData',
				message: error?.message || 'REQUEST ERROR',
				contextData: {
					response,
					error
				}
			})

			// Call the original error callback
			if (_fnErrorCallback) return _fnErrorCallback(error, response)
		},
		options,
		navigationId
	)
}

/**
 * Execution of the POST request to the server
 * @param {String} controller The controller name
 * @param {String} action The action name
 * @param {Object} data Object with data to be sent to the server
 * @param {Callback} _fnCallback The request callback
 * @param {Callback} _fnErrorCallback The error callback
 * @param {Object} options The Axios additional options
 * @param {string} navigationId The Navigation context Id
 * @returns The «Promise» object
 */
export function postData(
	controller,
	action,
	data,
	_fnCallback,
	_fnErrorCallback,
	options,
	navigationId
) {
	const tracing = useTracingDataStore()
	const url = apiActionURL(controller, action)
	const traceId = options?.meta?.traceId || uuidv4()

	if (!options) options = {}
	if (!options.meta) options.meta = {}
	options.meta.traceId = traceId

	// Get the navigation data
	let navigationData = null
	if (!_isEmpty(navigationId)) {
		navigationData = getHistoryToSend(navigationId, controller.toLowerCase())
	}

	// Prepare request data for tracing
	let requestDataForTracing = data
	if (!(data instanceof FormData)) {
		requestDataForTracing = { ...data }
		if (navigationData) {
			requestDataForTracing.jsonNavigationData = navigationData
		}
	}

	tracing.addRequestTrace({
		origin: 'postData',
		requestType: 'post',
		requestUrl: url,
		requestParams: { ...options?.params, nav: navigationId },
		requestData: requestDataForTracing,
		contextData: {
			controller,
			action,
			options
		},
		traceId: traceId
	})

	return _postData(
		controller,
		action,
		data,
		(data, response) => {
			// Add tracing for successful response if needed
			if (response) {
				const tracing = useTracingDataStore()
				tracing.addResponseTrace({
					traceId: options?.meta?.traceId,
					origin: 'postData',
					requestType: response.config.method,
					requestUrl: response.config.url,
					requestParams: response.config.params,
					requestData: response.config.data,
					responseStatus: response.status,
					responseData: response.data,
					meta: response.config.meta
				})

				// Handle server errors tracing
				const srvEventTracking = response.data?.eTracker
				if (srvEventTracking) {
					tracing.addServerErrors({
						traceId: options?.meta?.traceId,
						errors: srvEventTracking,
						contextData: {
							requestType: response.config.method,
							requestUrl: response.config.url,
							requestParams: response.config.params,
							requestData: response.config.data
						}
					})
				}
			}

			// Call the original callback
			if (_fnCallback) return _fnCallback(data, response)
		},
		(error, response) => {
			const tracing = useTracingDataStore()
			tracing.addError({
				origin: 'postData',
				message: error?.message || 'REQUEST ERROR',
				contextData: {
					response,
					error
				}
			})

			// Call the original error callback
			if (_fnErrorCallback) return _fnErrorCallback(error, response)
		},
		options,
		navigationId
	)
}

/**
 * Fetches, from the server, the data of the specified form.
 * @param {string} controller The name of the controller
 * @param {string} formName The name of the form
 * @param {string} formMode The mode of the form
 * @param {object} params An object with additional parameters
 * @param {function} _fnCallback A callback function (optional)
 * @param {string} navigationId The Navigation context Id
 * @param {Object} options The Axios additional options
 * @returns A «Promise» to be resolved when the request completes.
 */
export function fetchFormData(
	controller,
	formName,
	formMode,
	params,
	_fnCallback,
	navigationId,
	options
) {
	const action = `${formName}_${formMode}_GET`
	return postData(controller, action, params, _fnCallback, undefined, options, navigationId)
}

/**
 * Fetches, from the server, the data of the specified form field.
 * @param {string} controller The name of the controller
 * @param {string} formName The name of the form
 * @param {string} fieldName The name of the field
 * @param {object} params An object with additional parameters
 * @param {function} _fnCallback A callback function (optional)
 * @param {string} navigationId The Navigation context Id
 * @returns A «Promise» to be resolved when the request completes.
 */
export function fetchFormFieldData(
	controller,
	formName,
	fieldName,
	params,
	_fnCallback,
	navigationId
) {
	const action = `${formName}_${fieldName}`
	return postData(controller, action, params, _fnCallback, undefined, undefined, navigationId)
}

/**
 * Sends a POST request, to the server, with the data of the specified form.
 * @param {string} controller The name of the controller
 * @param {string} formName The name of the form
 * @param {string} formMode The mode of the form
 * @param {object} params An object with additional parameters
 * @param {function} _fnCallback A callback function to be executed in case of success (optional)
 * @param {function} _fnErrorCallback A callback function to be executed in case of failure (optional)
 * @param {object} headers An object with additional options to be included in the header (optional)
 * @param {string} navigationId The Navigation context Id
 * @returns A «Promise» to be resolved when the request completes.
 */
export function postFormData(
	controller,
	formName,
	formMode,
	params,
	_fnCallback,
	_fnErrorCallback,
	headers,
	navigationId
) {
	const action = `${formName}_${formMode}`
	return postData(
		controller,
		action,
		params,
		_fnCallback,
		_fnErrorCallback,
		headers,
		navigationId
	)
}

/**
 * Allow execution of the server side function
 * @param {string} func Function name
 * @param {*} args Function arguments
 * @returns Promise
 */
export function executeServerFunction(func, args) {
	if (typeof func === 'undefined' || typeof args === 'undefined') return

	return new Promise((fnResolve) => {
		postData('Home', 'ExecuteServerFunction', { func, args }, (data) =>
			data.Success ? fnResolve(data.Data) : fnResolve()
		)
	})
}

/**
 * Retrieve an image from the server.
 * @param baseArea {String}
 * @param params {Object}
 * @param callback {Function}
 * @returns Promise
 */
export function retrieveImage(baseArea, params, callback) {
	return fetchData(baseArea, 'GetImage', params, (data) => {
		if (typeof callback === 'function') callback(data)
	})
}

/**
 * Gets the desired file from the server with tracing.
 * @param {string} baseArea The area to which the file belongs
 * @param {string} ticket The file ticket
 * @param {number} viewType The file view mode
 * @param {string} navigationId The Navigation context Id
 */
export function getFile(baseArea, ticket, viewType, navigationId = MAIN_HISTORY_BRANCH_ID) {
	// Add tracing
	const tracing = useTracingDataStore()
	tracing.addTrace({
		origin: 'getFile',
		message: 'Getting file',
		contextData: {
			baseArea,
			ticket,
			viewType,
			navigationId
		}
	})

	return _getFile(baseArea, ticket, viewType, navigationId)
}

/**
 * Fetches the elements of a dynamic array from the server.
 * @param {String} array The identifier of the array to fetch
 * @param {String} lang The system language
 * @param {Function} callback The callback method to update the array
 */
export function fetchDynamicArray(array, lang, callback) {
	const result = []

	fetchData('Arrays', array, { lang: lang }, (data) => {
		let i = 1

		_forEach(data, (el) => {
			result.push({
				num: i++,
				key: el.Key,
				text: el.Text,
				group: el.Group,
				get value() {
					return this.text
				}
			})
		})

		if (typeof callback === 'function') callback(result)
	})
}

/**
 * Uploads a file in multiple chunks with tracing.
 * @param {string} controller - The API controller handling the upload.
 * @param {string} action - The API action for the upload.
 * @param {File} file - The file to be uploaded.
 * @param {Object} params - Additional parameters for the upload.
 * @param {Function} _fnCallback - Success callback function.
 * @param {Function} _fnErrorCallback - Error callback function.
 * @param {number} [maxChunkSize] - The maximum chunk size.
 * @returns {Promise<void>} A promise that resolves when all chunks are uploaded.
 */
export async function uploadFile(
	controller,
	action,
	file,
	params,
	_fnCallback,
	_fnErrorCallback,
	maxChunkSize
) {
	// Add tracing before upload
	const tracing = useTracingDataStore()
	const url = apiActionURL(controller, action)

	tracing.addRequestTrace({
		origin: 'uploadFile',
		requestType: 'post',
		requestUrl: url,
		requestParams: params,
		requestData: {
			fileSize: file?.size,
			fileName: file?.name
		},
		contextData: {
			controller,
			action
		}
	})

	try {
		// Track upload start
		tracing.addTrace({
			origin: 'uploadFile',
			message: 'Starting file upload',
			contextData: {
				fileName: file?.name,
				fileSize: file?.size,
				params
			}
		})

		await _uploadFile(
			controller,
			action,
			file,
			params,
			(data) => {
				tracing.addTrace({
					origin: 'uploadFile',
					message: 'Chunk uploaded successfully',
					contextData: {
						data,
						params
					}
				})

				if (_fnCallback) return _fnCallback(data)
			},
			(error) => {
				tracing.addError({
					origin: 'uploadFile',
					message: 'Error uploading chunk',
					contextData: {
						error,
						params
					}
				})

				if (_fnErrorCallback) return _fnErrorCallback(error)
			},
			maxChunkSize
		)

		// Track upload completion
		tracing.addTrace({
			origin: 'uploadFile',
			message: 'All chunks uploaded successfully',
			contextData: {
				fileName: file?.name,
				fileSize: file?.size,
				params
			}
		})
	} catch (error) {
		// Track upload failure
		tracing.addError({
			origin: 'uploadFile',
			message: 'File upload failed',
			contextData: {
				error,
				fileName: file?.name,
				fileSize: file?.size,
				params
			}
		})
		throw error
	}
}

export default {
	apiActionURL,
	fetchData,
	postData,
	fetchFormData,
	fetchFormFieldData,
	postFormData,
	executeServerFunction,
	forceDownload,
	retrieveImage,
	getFile,
	fetchDynamicArray,
	uploadFile
}
