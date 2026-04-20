import _forEach from 'lodash-es/forEach'
import _remove from 'lodash-es/remove'
import { v4 as uuidv4 } from 'uuid'
import { reactive, ref, computed, watch } from 'vue'

import { useGenericDataStore } from '../../stores/genericData'

/**
 * Ensure that no process hangs in the "busy states" list after being collapsed by the GC.
 * For example, in the case of redirect with two menus in a row (skip if only one).
 */
const registry = new FinalizationRegistry((processId) => {
	const genericDataStore = useGenericDataStore()

	genericDataStore.removeProcessFromBusyPageStack(processId)
	genericDataStore.removeAsyncProcess(processId)
})

/**
 * Class representing an asynchronous process.
 */
export class QAsyncProcess {
	/**
	 * Create an asynchronous process to be monitored.
	 * @param {Promise} cbPromise - The promise object of the process.
	 * @param {Boolean} loadingEffect - Whether the process causes a loading effect.
	 * @param {Number} loadingEffectDelay - The delay time for the loading effect to appear (milliseconds).
	 * @param {Boolean} busyState - Whether the process causes a busy state.
	 * @param {String} busyStateMessage - The message to present to the user during the busy state.
	 */
	constructor(cbPromise, loadingEffect, loadingEffectDelay, busyState, busyStateMessage) {
		this.id = uuidv4()
		this.cbPromise = cbPromise
		this.hasLoadingEffect = loadingEffect || false
		this.loadingEffectDelay = this.hasLoadingEffect ? loadingEffectDelay || 0 : 0
		this.busyState = busyState || false
		this.busyStateMessage = busyStateMessage
		this.timestamp = Date.now()
		this.concluded = ref(false)

		const genericDataStore = useGenericDataStore()

		const fnCallback = (() => {
			this.concluded.value = true

			genericDataStore.removeProcessFromBusyPageStack(this.id)
			genericDataStore.removeAsyncProcess(this.id)
		}).bind(this)

		Promise.resolve(this.cbPromise).then(fnCallback, fnCallback)

		/**
		 * Internal property, just to dispatch change event / watch effect
		 */
		this._dispatchChange = ref(0)
		if (this.loadingEffectDelay > 0) {
			this._timerId = setTimeout(
				(() => {
					if (this.busyState === true && this.concluded.value === false)
						genericDataStore.addProcessToBusyPageStack({
							id: this.id,
							message: this.busyStateMessage
						})
					this._dispatchChange.value++
				}).bind(this),
				this.loadingEffectDelay
			)
		} else if (this.busyState)
			genericDataStore.addProcessToBusyPageStack({
				id: this.id,
				message: this.busyStateMessage
			})
		else genericDataStore.addAsyncProcess(this.id)

		registry.register(this, this.id, this)
	}

	/**
	 * Get the loading effect status.
	 * @returns {Boolean} Whether the loading effect is active.
	 */
	get loadingEffect() {
		return (
			this.hasLoadingEffect === true &&
			(this.loadingEffectDelay === 0 || Date.now() > this.timestamp + this.loadingEffectDelay)
		)
	}

	/**
	 * Destroy the process and clean up.
	 */
	destroy() {
		if (this._timerId) {
			clearTimeout(this._timerId)
			this._timerId = null
		}

		const genericDataStore = useGenericDataStore()

		genericDataStore.removeProcessFromBusyPageStack(this.id)
		genericDataStore.removeAsyncProcess(this.id)
		registry.unregister(this)
	}
}

/**
 * Class representing a list of asynchronous processes.
 */
class QAsyncProcessList {
	/**
	 * Create a list of asynchronous processes.
	 */
	constructor() {
		this.processes = []
	}

	/**
	 * Check if there are any processes in the list.
	 * @returns {Boolean} Whether there are any processes.
	 */
	get hasAny() {
		return this.processes.length !== 0
	}

	/**
	 * Check if all processes have their loading effects completed.
	 * @returns {Boolean} Whether all processes are loaded.
	 */
	get allLoaded() {
		return !this.processes.some(
			(proc) => proc.concluded === false && proc.loadingEffect === true
		)
	}

	/**
	 * Check if all processes are concluded.
	 * @returns {Boolean} Whether all processes are concluded.
	 */
	get allConcluded() {
		return !this.processes.some((proc) => proc.concluded === false)
	}

	/**
	 * Add a process to the list.
	 * @param {QAsyncProcess} proc - The process to add.
	 * @returns {QAsyncProcess} The reactive process.
	 */
	addProcess(proc) {
		const reactiveProc = reactive(proc)
		this.processes.push(reactiveProc)
		return reactiveProc
	}

	/**
	 * Destroy all processes in the list.
	 */
	destroy() {
		_forEach(this.processes, (proc) => proc.destroy())
		this.processes.splice(0, this.processes.length)
	}
}

/**
 * Class representing a callback for asynchronous processes.
 */
class QAsyncProcessCallback {
	/**
	 * Create a callback for an asynchronous process.
	 * @param {Function} callback - The callback function to be executed.
	 * @param {Object} [context=null] - The context in which to execute the callback.
	 * @param {Array} [args=[]] - The arguments to pass to the callback.
	 */
	constructor(callback, context, args) {
		this.callback = callback
		this.context = context ?? null
		this.args = args ?? []
		this.fired = false
	}

	/**
	 * Execute the callback if it hasn't been fired yet.
	 */
	run() {
		if (!this.fired) this.callback.apply(this.context ?? null, this.args ?? [])
		this.fired = true
		this.clearMemory()
	}

	/**
	 * Clear the memory used by this callback.
	 */
	clearMemory() {
		this.callback = undefined
		this.context = undefined
		this.args = undefined
	}
}

/**
 * Class representing a monitor for a list of asynchronous processes.
 */
export class QAsyncProcessMonitor {
	/**
	 * Create a monitor for a list of asynchronous processes.
	 * @param {String} identifier - The identifier of the monitoring process.
	 * @param {Boolean} defaultValueLoaded - The default value indicating if the processes are loaded.
	 */
	constructor(identifier, defaultValueLoaded) {
		this.id = identifier
		this.uuid = uuidv4()
		this.processList = reactive(new QAsyncProcessList())

		/** List of callbacks to be executed */
		this.callbacksOnce = ref([])

		// A list of other associated process monitors
		this.monitors = ref([])

		/** Indicates if all the processes are completed */
		this._defaultValueLoaded =
			typeof defaultValueLoaded === 'boolean' ? defaultValueLoaded : true
		// Tracks changes in the list of processes and re-evaluates whether there are any pending processes
		this.loaded = computed(
			() =>
				(!this.processList.hasAny
					? this._defaultValueLoaded
					: this.processList.allLoaded) && this.monitors.value.every((m) => m.loaded)
		)

		this._stopWatcher = watch(
			this.processList,
			() => {
				if (this.processList.allConcluded) {
					_remove(this.callbacksOnce.value, (cb) => {
						cb.run()
						return true
					})
				}
			},
			{ deep: true }
		)
	}

	/**
	 * Add a process to the list of processes to be monitored.
	 * @param {Promise} cbPromise - The promise object of the process.
	 * @param {Boolean} loadingEffect - Whether the process causes a loading effect.
	 * @param {Number} loadingEffectDelay - The delay time for the loading effect to appear (milliseconds).
	 * @param {Boolean} busyState - Whether the process causes a busy state.
	 * @param {String} busyStateMessage - The message to present to the user during the busy state.
	 * @returns {Promise} The promise object of the process.
	 */
	add(cbPromise, loadingEffect, loadingEffectDelay, busyState, busyStateMessage) {
		return this.processList.addProcess(
			new QAsyncProcess(
				cbPromise,
				loadingEffect,
				loadingEffectDelay,
				busyState,
				busyStateMessage
			)
		).cbPromise
	}

	/**
	 * Add a process that causes a loading effect to the list of processes to be monitored.
	 * @param {Promise} cbPromise - The promise object of the process.
	 * @param {Number} loadingEffectDelay - The delay time for the loading effect to appear (milliseconds).
	 * @returns {Promise} The promise object of the process.
	 */
	addWL(cbPromise, loadingEffectDelay) {
		return this.add(cbPromise, true, loadingEffectDelay)
	}

	/**
	 * Add a busy process to the list of processes to be monitored.
	 * @param {Promise} cbPromise - The promise object of the process.
	 * @param {String} busyStateMessage - The message to present to the user during the busy state.
	 * @param {Number} [loadingEffectDelay=1000] - The delay time for the loading effect to appear (milliseconds).
	 * @returns {Promise} The promise object of the process.
	 */
	addBusy(cbPromise, busyStateMessage, loadingEffectDelay) {
		return this.add(
			cbPromise,
			true,
			typeof loadingEffectDelay === 'number' ? loadingEffectDelay : 1000,
			true,
			busyStateMessage
		)
	}

	/**
	 * Add an immediate busy process to the list of processes to be monitored.
	 * @param {Promise} cbPromise - The promise object of the process.
	 * @param {String} busyStateMessage - The message to present to the user during the busy state.
	 * @returns {Promise} The promise object of the process.
	 */
	addImmediateBusy(cbPromise, busyStateMessage) {
		return this.addBusy(cbPromise, busyStateMessage, 0)
	}

	/**
	 * Perform a callback as soon as all processes have finished loading.
	 * @param {Function} callback - The callback to be executed.
	 * @param {Object} [context] - The context in which to execute the callback.
	 * @param {Array} [args] - Additional arguments to pass to the callback.
	 */
	once(callback, context, args) {
		const cb = new QAsyncProcessCallback(callback, context, args)
		this.callbacksOnce.push(cb)
	}

	/**
	 * Associates the specified asynchronous process monitor to this one.
	 * @param {QAsyncProcessMonitor} monitor The monitor
	 */
	associateMonitor(monitor) {
		if (typeof monitor !== 'undefined' && !(monitor instanceof QAsyncProcessMonitor))
			throw new TypeError(
				'The "monitor" argument must be an instance of QAsyncProcessMonitor.'
			)
		if (this.monitors.some((m) => m.id === monitor.id))
			throw new Error(`A monitor with identifier "${monitor.id}" is already associated.`)
		this.monitors.push(monitor)
	}

	/**
	 * Disassociates the asynchronous process monitor with the specified identifier from this one.
	 * @param {string} id The monitor identifier
	 */
	disassociateMonitor(id) {
		const index = this.monitors.findIndex((m) => m.id === id)
		if (index !== -1) this.monitors.splice(index, 0)
	}

	/**
	 * Reset the list of processes.
	 */
	destroy() {
		this._stopWatcher?.()
		this._stopWatcher = null

		this.processList.destroy()
		this.callbacksOnce.length = 0
		this.monitors.length = 0
	}
}

/**
 * Create a Vue.js reactive object to monitor the completion of asynchronous methods.
 * @param {String} identifier - The identifier of the monitoring process.
 * @param {Boolean} defaultValueLoaded - The default value indicating if the processes are loaded.
 * @returns {Object} A Vue.js reactive object with a property indicating if all processes are completed.
 */
function getProcListMonitor(identifier, defaultValueLoaded) {
	return reactive(new QAsyncProcessMonitor(identifier, defaultValueLoaded))
}

/**
 * Create a Vue.js reactive object to monitor the completion of asynchronous page blocking methods.
 * @param {Promise} cbPromise - The promise object of the process.
 * @param {String} busyStateMessage - The message to present to the user during the busy state.
 * @returns {Object} A Vue.js reactive object with a property indicating if the process is completed.
 */
function addBusy(cbPromise, busyStateMessage) {
	return reactive(new QAsyncProcess(cbPromise, true, 1000, true, busyStateMessage))
}

/**
 * Create a Vue.js reactive object to monitor the completion of asynchronous non-blocking methods.
 * @param {Promise} cbPromise - The promise object of the process.
 * @param {String} busyStateMessage - The message to present to the user during the busy state.
 * @returns {Object} A Vue.js reactive object with a property indicating if the process is completed.
 */
function addProcess(cbPromise) {
	return reactive(new QAsyncProcess(cbPromise))
}

export default {
	getProcListMonitor,
	addBusy,
	addProcess
}
