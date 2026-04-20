import _isEmpty from 'lodash-es/isEmpty'
import _merge from 'lodash-es/merge'
import { v4 as uuidv4 } from 'uuid'

import asyncProcM from '../composables/async'
import { documentViewTypeMode } from '../constants/enums'
import eventBus from '../plugins/eventBus'
import { useGenericDataStore } from '../stores/genericData'
import { useNavDataStore } from '../stores/navData'
import { useSystemDataStore } from '../stores/systemData'
import { useUserDataStore } from '../stores/userData'
import { displayMessage } from '../utils/genericFunctions'
import { axiosInstance } from './axiosInstance'
import { MAIN_HISTORY_BRANCH_ID } from './constants'
import {
	apiActionURL,
	getFileNameFromRequest,
	getHistoryToSend,
	getOptimalChunkSize,
	splitFile
} from './utils'

/**
 * Execution of a simple GET request to the server (doesn't affect the navigation)
 * @param {String} controller The controller name
 * @param {String} action The action name
 * @param {String} parameter The action parameter (normally it will be the system/year)
 * @returns The «Promise» object that is resolved just after completely executed request
 */
export function simpleFetch(controller, action, parameter = '') {
	const url = `api/${controller}/${action}/${parameter}`

	const promise = new Promise((fnResolve, fnReject) => {
		axiosInstance
			.get(url)
			.then((response) => fnResolve(response))
			.catch((error) => fnReject(error))
	})

	asyncProcM.addProcess(promise)
	return promise
}

/**
 * Execution of the GET request to the server
 * @param {String} controller The controller name
 * @param {String} action The action name
 * @param {Object} params Object with additional parameters (Query String)
 * @param {Callback} _fnCallback The request callback
 * @param {Callback} _fnErrorCallback The error callback
 * @param {Object} options The Axios additional options
 * @param {string} navigationId The Navigation context Id
 * @returns The «Promise» object
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
	if (_isEmpty(navigationId)) navigationId = MAIN_HISTORY_BRANCH_ID

	const systemDataStore = useSystemDataStore()

	if (!systemDataStore.system.currentSystem) {
		if (typeof _fnErrorCallback === 'function') _fnErrorCallback()
		return
	}

	const url = apiActionURL(controller, action),
		tokenElements = document.getElementsByName('__RequestVerificationToken'),
		antiForgeryToken = tokenElements.length > 0 ? tokenElements[0].value : null,
		axiosOptions = {
			withCredentials: true,
			headers: {
				__RequestVerificationToken: antiForgeryToken,
				// For Asp.NET Request.IsAjaxRequest()
				'X-Requested-With': 'XMLHttpRequest'
			},
			meta: {
				traceId: uuidv4()
			}
		}

	_merge(axiosOptions, options, { params: { ...params, nav: navigationId } })

	const promise = new Promise((fnResolve, fnReject) => {
		axiosInstance
			.get(url, axiosOptions)
			.then((response) => processRequest(response, _fnCallback, fnResolve))
			.catch((error) =>
				handleNonOkResponse(error.response, fnResolve, _fnErrorCallback, fnReject, error)
			)
	})

	asyncProcM.addProcess(promise)
	return promise
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
	if (_isEmpty(navigationId)) navigationId = MAIN_HISTORY_BRANCH_ID

	const systemDataStore = useSystemDataStore()

	if (!systemDataStore.system.currentSystem) {
		if (typeof _fnErrorCallback === 'function') _fnErrorCallback()
		return
	}

	const url = apiActionURL(controller, action),
		tokenElements = document.getElementsByName('__RequestVerificationToken'),
		antiForgeryToken = tokenElements.length > 0 ? tokenElements[0].value : null,
		axiosOptions = {
			withCredentials: true,
			headers: {
				__RequestVerificationToken: antiForgeryToken,
				// For Asp.NET Request.IsAjaxRequest()
				'X-Requested-With': 'XMLHttpRequest'
			},
			meta: {
				traceId: uuidv4()
			}
		},
		navigationData = getHistoryToSend(navigationId, controller.toLowerCase()),
		jsonNavigationData = JSON.stringify(navigationData)

	// Set the Navigation/History to the request
	let requestData = data ?? {}
	if (requestData instanceof FormData)
		requestData.append('jsonNavigationData', new Blob([jsonNavigationData]))
	else
		// Reassign with a shallow copy to avoid mutating data. Prevents unintended side effects in some cases where data and options share the same object.
		requestData = { ...data, jsonNavigationData }

	_merge(axiosOptions, options, { params: { nav: navigationId } })

	const promise = new Promise((fnResolve, fnReject) => {
		axiosInstance
			.post(url, requestData, axiosOptions)
			.then((response) => processRequest(response, _fnCallback, fnResolve))
			.catch((error) =>
				handleNonOkResponse(error.response, fnResolve, _fnErrorCallback, fnReject, error)
			)
	})

	asyncProcM.addProcess(promise)
	return promise
}

/**
 * Processing the Axios response.
 * @param {AxiosResponse} response Axios response object
 * @param {Callback} _fnCallback The request callback
 * @param {Function} fnResolve The «promise resolve» function
 */
async function processRequest(response, _fnCallback, fnResolve) {
	if (response) {
		// If the initial PHE is empty but the key is still persistent, clean it and redirect to home page.
		const userDataStore = useUserDataStore()
		if (
			response.data?.Data?.InitialPHEEmpty === true &&
			typeof userDataStore.valuesOfPHEs[response.data.Data.Module] === 'object'
		) {
			userDataStore.valuesOfPHEs[response.data.Data.Module] = undefined
			eventBus.emit('response-redirect-to', {
				type: 'route',
				routeName: `home-${response.data.Data.Module}`,
				routeValues: {}
			})
		}

		if (response.status === 200) {
			const navDataStore = useNavDataStore()
			const genericDataStore = useGenericDataStore()

			const responseData = response.data ?? {},
				data = responseData.Data ?? null,
				statusCode = responseData.statusCode ?? 200,
				srvHistory = responseData.NavigationData ?? {},
				maintenance = responseData.Maintenance ?? {},
				navigationId = srvHistory.navigationId,
				history = srvHistory.historyDiff ?? null

			genericDataStore.setMaintenanceStatus(maintenance)
			navDataStore.updateHistoryByServer({ navigationId, srvHistory: history })

			if (statusCode !== 200) handleNonOkResponse(response, fnResolve, _fnCallback)
			else if (_fnCallback) {
				Promise.resolve(_fnCallback(data, response)).then(
					() => fnResolve?.(data),
					() => fnResolve?.(data)
				)
			} else fnResolve?.(data)
		}
	}
}

/**
 * Uploads a single chunk of a file.
 * @param {Object} options - Configuration options for the upload.
 * @returns {Promise<void>} A promise that resolves when the upload completes.
 */
async function uploadChunk({
	url,
	chunk,
	fileId,
	fileName,
	ticket,
	mode,
	version = '1',
	fileSize,
	start,
	end,
	_fnCallback,
	_fnErrorCallback
}) {
	const formData = new FormData()
	formData.append(fileId, chunk, fileName)
	formData.append('ticket', ticket)
	formData.append('mode', mode)
	formData.append('version', version)

	const axiosOptions = {
		withCredentials: true,
		headers: {
			'Content-Type': 'multipart/form-data',
			'Content-Range': `bytes ${start}-${end}/${fileSize}`
		}
	}

	return new Promise((fnResolve, fnReject) => {
		axiosInstance
			.post(url, formData, axiosOptions)
			.then((response) => processRequest(response, _fnCallback, fnResolve))
			.catch((error) =>
				handleNonOkResponse(error.response, fnResolve, _fnErrorCallback, fnReject, error)
			)
	})
}

/**
 * Force download of file
 * @param {string} data Request data object
 * @param {string} fileName File name
 * @param {string} fileType File MIME type
 * @param {boolean} newTab Whether it should be opened in a new tab (with preview)
 * @param {boolean} createBlob Whether it should create a blob or use the data directly
 */
export function forceDownload(data, fileName, fileType, newTab = false, createBlob = true) {
	const blobOptions = fileType ? { type: fileType } : undefined
	const url = createBlob ? window.URL.createObjectURL(new Blob([data], blobOptions)) : data
	const link = document.createElement('a')
	link.href = url

	if (newTab) link.setAttribute('target', '_blank')
	else link.setAttribute('download', fileName)

	link.click()
}

/**
 * Uploads a file in multiple chunks.
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
	if (!(file instanceof File)) throw new TypeError('Invalid file input. Expected a File object.')

	const url = apiActionURL(controller, action)
	// Set optimal chunk size if not provided
	maxChunkSize = maxChunkSize ?? getOptimalChunkSize(file.size)

	for await (const { chunk, start, end } of splitFile(file, maxChunkSize)) {
		try {
			// Process chunks one at a time
			const uploadResult = await uploadChunk({
				url,
				chunk,
				...params,
				fileSize: file.size,
				fileName: file.name,
				start,
				end,
				_fnCallback,
				_fnErrorCallback
			})

			// As long as there's no "success": true/false, it's just a progress response.
			if (uploadResult?.success === false) {
				return // Stop uploading further chunks on server error
			}
		} catch {
			return // Stop uploading further chunks on error
		}
	}
}

/**
 * Gets the desired file from the server.
 * @param {string} baseArea The area to which the file belongs
 * @param {string} ticket The file ticket
 * @param {number} viewType The file view mode
 * @param {string} navigationId The Navigation context Id
 */
export function getFile(baseArea, ticket, viewType, navigationId = MAIN_HISTORY_BRANCH_ID) {
	if (!Object.values(documentViewTypeMode).includes(viewType))
		viewType = documentViewTypeMode.preview

	const params = {
		ticket,
		viewType
	}

	asyncProcM.addBusy(
		postData(
			baseArea,
			'GetFile',
			params,
			async (_, response) => {
				const fileName = getFileNameFromRequest(response)
				const fileType = response.headers['content-type']

				// Here we check if there was a server-side error, if so we present an error message and do nothing.
				if (fileType.includes('application/json')) {
					// The response content comes as a byte array, so we need to parse it first.
					const data = new Blob([response.data], { type: fileType })
					const content = await data.text()
					const result = JSON.parse(content)

					if (result.Success === false) {
						displayMessage(result.Message, 'error')
						return
					}
				}

				// Should open in a new tab only if it's defined with "preview" and the file type allows it.
				const newTab =
					!fileType.includes('application/octet-stream') &&
					(!fileName || viewType === documentViewTypeMode.preview)
				forceDownload(response.data, fileName, fileType, newTab)
			},
			undefined,
			{ responseType: 'arraybuffer' },
			navigationId
		)
	)
}

/**
 * Handles an error in the request.
 * @param {AxiosResponse} response Axios response object (data, status, statusText, headers, config, request?)
 * @param {Function} fnResolve The «promise resolve» function
 * @param {Callback} _fnCallback The request callback
 * @param {Function} fnReject The «promise reject» function
 * @param {Error} error The request error object
 */
async function handleNonOkResponse(response, fnResolve, _fnCallback, fnReject, error) {
	if (response) {
		const responseData = response.data ?? {},
			data = responseData.Data ?? null,
			statusCode = responseData.statusCode ?? response.status

		// Skip if only one, in addition to redirecting to the next one, may still need to load the list data.
		if (typeof _fnCallback === 'function' && data) await _fnCallback(data, response)

		switch (statusCode) {
			case 302:
				eventBus.emit('response-redirect-to', responseData)
				break
			case 403:
				eventBus.emit('response-redirect-to', {
					type: 'route',
					routeName: 'permissionError',
					routeValues: { errorMessage: responseData.message || 'Permission error (403)' }
				})
				break
			case 404:
				eventBus.emit('response-redirect-to', {
					type: 'route',
					routeName: 'notFound',
					routeValues: { errorMessage: responseData.message || 'Not found (404)' }
				})
				break
			case 500:
				eventBus.emit('response-redirect-to', {
					type: 'route',
					routeName: 'serverError',
					routeValues: {}
				})
				break
		}

		fnResolve?.(data)
	} else {
		if (error?.name === 'CanceledError') {
			// Request was aborted; silently ignore.
			// A practical case: exiting the form before the table lists have finished loading.
			// To prevent callback from being processed after exiting, the request will be canceled and ignored.
			fnReject?.(error)
		} else {
			fnResolve?.(null)
		}
	}

	// FIXME: Temporary workaround just to avoid infinitely calling "GetIfUserLogged" when itself fails.
	if (!response?.config.url.includes('GetIfUserLogged')) eventBus.emit('check-user-is-logged-in')
}

export default {
	apiActionURL,
	simpleFetch,
	fetchData,
	postData,
	forceDownload,
	getFile,
	uploadFile
}
