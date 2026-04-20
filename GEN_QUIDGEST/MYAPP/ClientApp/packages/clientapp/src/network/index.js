import { forceDownload } from './core'
import { apiActionURL } from './utils'
import {
	executeServerFunction,
	fetchData,
	fetchDynamicArray,
	fetchFormData,
	fetchFormFieldData,
	getFile,
	postData,
	postFormData,
	retrieveImage,
	uploadFile
} from './wrappers'

export { MAIN_HISTORY_BRANCH_ID } from './constants'

// Re-export the NetworkAPI class
export { NetworkAPI } from './NetworkAPI'

// Re-export the axios instance
export { axiosInstance } from './axiosInstance'

export { forceDownload, simpleFetch } from './core'

// Re-export the utility functions
export * from './utils'

// Re-export the higher-level API functions from the wrappers directory
// These are the functions that include tracing and specific use-case logic
export {
	executeServerFunction,
	fetchData,
	fetchDynamicArray,
	fetchFormData,
	fetchFormFieldData,
	getFile,
	postData,
	postFormData,
	retrieveImage,
	uploadFile
}

// Export default netAPI
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
