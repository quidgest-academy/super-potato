import { forceDownload } from './core'
import { apiActionURL } from './utils'
import {
	executeServerFunction,
	fetchData,
	fetchDynamicArray,
	fetchFormData,
	fetchFormFieldData,
	postData,
	postFormData,
	retrieveImage,
	uploadFile
} from './wrappers'

/**
 * The proxy class for the network API with applying the navigation identifier.
 */

export class NetworkAPI {
	constructor(navigationId) {
		this.navigationId = navigationId

		this.apiActionURL = apiActionURL
		this.executeServerFunction = executeServerFunction
		this.forceDownload = forceDownload
		this.retrieveImage = retrieveImage
		this.fetchDynamicArray = fetchDynamicArray
		this.uploadFile = uploadFile
	}

	fetchData(controller, action, params, _fnCallback, _fnErrorCallback, options) {
		return fetchData(
			controller,
			action,
			params,
			_fnCallback,
			_fnErrorCallback,
			options,
			this.navigationId
		)
	}

	postData(controller, action, data, _fnCallback, _fnErrorCallback, options) {
		return postData(
			controller,
			action,
			data,
			_fnCallback,
			_fnErrorCallback,
			options,
			this.navigationId
		)
	}

	fetchFormData(controller, formName, formMode, params, _fnCallback, options) {
		return fetchFormData(
			controller,
			formName,
			formMode,
			params,
			_fnCallback,
			this.navigationId,
			options
		)
	}

	fetchFormFieldData(controller, formName, field, params, _fnCallback) {
		return fetchFormFieldData(
			controller,
			formName,
			field,
			params,
			_fnCallback,
			this.navigationId
		)
	}

	postFormData(controller, formName, formMode, params, _fnCallback, _fnErrorCallback, headers) {
		return postFormData(
			controller,
			formName,
			formMode,
			params,
			_fnCallback,
			_fnErrorCallback,
			headers,
			this.navigationId
		)
	}
}
