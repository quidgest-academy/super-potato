import { existsSync, mkdirSync, readFileSync, writeFile } from 'node:fs'

import { gzip } from 'zlib'

/**
 * Writes code to a file and optionally gzips it.
 * @param {string} dest - Destination file path.
 * @param {string} code - Code to write to the file.
 * @param {boolean} [zip=false] - Whether to gzip the code or not.
 * @returns {Promise<string>} A Promise that resolves with the written code.
 */
export function writeAsset(dest, code, zip) {
	return new Promise((resolve, reject) => {
		writeFile(dest, code, (err) => {
			if (err) return reject(err)
			if (zip) {
				gzip(code, (err) => {
					if (err) return reject(err)
					resolve(code)
				})
			} else {
				resolve(code)
			}
		})
	})
}

/**
 * Reads the content of a file synchronously.
 * @param {string} path - Path to the file.
 * @returns {string} The content of the file.
 */
export function readFile(path) {
	return readFileSync(path, 'utf-8')
}

/**
 * Creates a folder if it doesn't exist.
 * @param {string} path - Path to the folder.
 */
export function createFolder(path) {
	if (!existsSync(path)) {
		mkdirSync(path, { recursive: true })
	}
}

/**
 * Executes an array of async tasks with a concurrency limit.
 *
 * @param tasks - An array of functions, each returning a Promise.
 * @param limit - The maximum number of tasks to run concurrently.
 * @returns A Promise that resolves to an array of results.
 *
 * @example
 * const tasks = urls.map(url => () => fetch(url).then(res => res.text()))
 * const results = await throttleTasks(tasks, 5)
 */
export async function throttleTasks(tasks, limit) {
	const result = []
	let taskIdx = 0

	async function worker() {
		while (taskIdx < tasks.length) {
			const index = taskIdx++
			result[index] = await tasks[index]()
		}
	}

	await Promise.all(Array.from({ length: limit }, () => worker()))
	return result
}
