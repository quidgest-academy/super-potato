import { useNavDataStore } from '../stores/navData'
import { useSystemDataStore } from '../stores/systemData'

/**
 * Generate the relative URL for a Web API action
 * @param {string} controller The controller name
 * @param {string} action The action name
 * @returns Relative URL for a Web API action
 */
export function apiActionURL(controller, action) {
	const systemDataStore = useSystemDataStore()
	const year = systemDataStore.system.currentSystem,
		lang = systemDataStore.system.currentLang,
		module = systemDataStore.system.currentModule

	return `api/${lang}/${year}/${module}/${controller}/${action}`
}

/**
 * Returns a History structure in the format expected by the server
 * @param navigationId The Navigation context Id
 * @param {string} currentArea The area of the current history level
 * @returns Object with current history
 */
export function getHistoryToSend(navigationId, currentArea = '') {
	const navDataStore = useNavDataStore()

	navDataStore.beforeRequestContext(navigationId)
	return navDataStore.navigation.getHistory(navigationId).historyToSend(currentArea)
}

/**
 * Determines the optimal chunk size based on the total file size.
 * @param {number} fileSize - The total size of the file in bytes.
 * @returns {number} The recommended chunk size in bytes.
 */
export function getOptimalChunkSize(fileSize) {
	if (typeof fileSize !== 'number' || fileSize <= 0)
		throw new TypeError('Invalid file size. Expected a positive number.')

	// Define 1MB in bytes for better readability
	const MB = 1024 * 1024

	// Recommended chunk sizes based on file size ranges
	if (fileSize <= 50 * MB)
		return 5 * MB // 5MB for files ≤ 50MB
	else if (fileSize <= 500 * MB) return 10 * MB // 10MB for files ≤ 500MB
	return 15 * MB // 15MB for files larger than 500MB
}

/**
 * Reads a file in chunks asyncronously without loading into memory at once.
 * @param {File} file - The file to be split.
 * @param {number} maxChunkSize - The maximum size of each chunk in bytes.
 * @returns {AsyncGenerator<{chunk: Blob, start: number, end: number}>} An async generator that yields chunks with their range.
 */
export async function* splitFile(file, maxChunkSize) {
	if (!(file instanceof File)) throw new TypeError('Provided input is not a valid File object.')

	let currentPosition = 0
	while (currentPosition < file.size) {
		const chunk = file.slice(currentPosition, currentPosition + maxChunkSize)
		const end = currentPosition + chunk.size - 1 // Use chunk.size for precise end value
		// Process only the current chunk
		yield { chunk, start: currentPosition, end }
		currentPosition += chunk.size
	}
}

/**
 * Retrieves the file name from an http file request.
 * @param {XMLHttpRequest} request The request that brings the file
 * @returns The name of the file, or null if it couldn't be found.
 */
export function getFileNameFromRequest(request) {
	const contentDisposition = request?.headers['content-disposition']

	// Try RFC 5987 / RFC 6266 filename*
	const filenameStarExp = /filename\*\s*=\s*(?<value>[^;]+)/i
	const filenameStar = filenameStarExp.exec(contentDisposition)?.groups?.value
	if (filenameStar) {
		const parts = filenameStar.split("''", 2)
		if (parts.length === 2) {
			try {
				return decodeURIComponent(parts[1]).replace(/^"|"$/g, '')
			} catch {
				// ignore and fall back
			}
		}
	}

	// Fallback: filename="..."
	const getFileNameExp = /filename="(?<filename>.*)"/
	return getFileNameExp.exec(contentDisposition)?.groups?.filename ?? null
}
