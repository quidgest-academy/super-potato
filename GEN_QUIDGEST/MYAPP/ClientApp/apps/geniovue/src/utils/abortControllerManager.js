/**
 * Manages AbortController instances by category, so you can:
 *  - get a fresh signal for a given key (aborting any previous one),
 *  - clear a key once its request has finished,
 *  - dispose of all controllers at once.
 */
export class AbortControllerManager {
	constructor() {
		/**
		 * @private
		 * @type {Map<string, AbortController>}
		 */
		this.controllers = new Map()
	}

	/**
	 * Obtain an AbortSignal for the given key, aborting any in-flight request for that same key.
	 *
	 * @param {string} key – Identifier for this category of request.
	 * @returns {AbortSignal} – The signal you should pass to fetch/axios.
	 */
	getSignal(key) {
		// Abort the previous controller, if any
		const existing = this.controllers.get(key)
		if (existing) {
			existing.abort()
		}

		// Create, store and return the new controller’s signal
		const controller = new AbortController()
		this.controllers.set(key, controller)
		return controller.signal
	}

	/**
	 * Remove the controller for a given key from the map, without aborting it.
	 * Use this in your `.finally()` so that the map doesn’t retain completed controllers.
	 *
	 * @param {string} key – Identifier for the category to clear.
	 */
	clear(key) {
		this.controllers.delete(key)
	}

	/**
	 * Abort _all_ managed controllers and clear the map.
	 * Call this in your component’s teardown (e.g. in `beforeUnmount`) to ensure
	 * no pending requests remain.
	 */
	dispose() {
		for (const controller of this.controllers.values()) {
			controller.abort()
		}
		this.controllers.clear()
	}
}
