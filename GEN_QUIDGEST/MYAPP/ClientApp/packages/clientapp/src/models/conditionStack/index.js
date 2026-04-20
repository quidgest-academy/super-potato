import { markRaw } from 'vue'
import { QEventEmitter } from '../../plugins/eventBus'
import { QAsyncProcessMonitor } from '../../composables/async'

/**
 * Class that represents a source in the stack.
 */
class ConditionSource {
	// Private fields.
	#condition
	#observers
	#isMetVal
	#processMonitor

	/**
	 * Creates a new condition source.
	 * @param {function} condition The condition to determine whether the source is active
	 * @param {string|array} eventIds An event to listen for, or a list of events
	 * @param {QEventEmitter} events The event emitter
	 * @param {boolean} isMetVal The value the condition should evaluate to in order for it to be met
	 * @param {QAsyncProcessMonitor} processMonitor A monitor of asynchronous processes
	 */
	constructor(condition, eventIds, events, isMetVal, processMonitor) {
		if (!['undefined', 'function'].includes(typeof condition))
			throw new TypeError('The "condition" must be a function.')
		if (!['undefined', 'string'].includes(typeof eventIds) && !Array.isArray(eventIds))
			throw new TypeError('The "eventIds" must be either a string or an array of strings.')
		if (typeof eventIds !== 'undefined' && typeof events === 'undefined')
			throw new TypeError('When defining "eventIds", the "events" must also be defined.')

		const eventList = Array.isArray(eventIds) ? eventIds : eventIds ? [eventIds] : []
		const condFunc = typeof condition === 'function' ? condition : () => isMetVal

		this.#condition = condFunc
		this.#observers = []
		this.#isMetVal = isMetVal
		this.#processMonitor = processMonitor

		this.isMet = isMetVal

		if (eventList.length > 0) events.onMany(eventList, () => this.validateCondition())
	}

	/**
	 * Factory function to create new condition sources.
	 * @param {function} condition The condition to determine whether the source is active
	 * @param {string|array} eventIds An event to listen for, or a list of events
	 * @param {QEventEmitter} events The event emitter
	 * @param {boolean} isMetVal The value the condition should evaluate to in order for it to be met
	 * @param {QAsyncProcessMonitor} processMonitor A monitor of asynchronous processes
	 * @returns A new condition source.
	 */
	static async createSource(condition, eventIds, events, isMetVal, processMonitor) {
		const source = new ConditionSource(condition, eventIds, events, isMetVal, processMonitor)
		await source.validateCondition()
		return source
	}

	/**
	 * Validates the source condition.
	 */
	async validateCondition() {
		const condPromise = this.#condition()
		this.#processMonitor?.addWL(condPromise)

		// Force a conversion to boolean for cases where the condition is returning 1 or 0.
		const result = (await condPromise) ? true : false
		this.isMet = result === this.#isMetVal
		this.notify()
	}

	/**
	 * Adds observer function to observers array.
	 * @param {function} fn The observer function to add
	 */
	addObserver(fn) {
		this.#observers.push(fn)
	}

	/**
	 * Notifies all observer functions in observers array.
	 */
	notify() {
		this.#observers.forEach((fn) => fn())
	}

	/**
	 * Destroys this condition source.
	 */
	destroy() {
		this.#observers.length = 0
		this.#processMonitor = null
	}
}

/**
 * Base class for a generic condition stack, should be extended by specific classes.
 */
class ConditionStack {
	// The MET event will be triggered when "anyMet" becomes true, and the UNMET when it becomes false.
	static MET_EVENT = 'conditions-met'
	static UNMET_EVENT = 'conditions-unmet'

	/**
	 * Creates a new condition stack.
	 * @param {QEventEmitter} events The event emitter
	 * @param {boolean} isMetVal The value a condition should evaluate to in order for it to be met
	 */
	constructor(events, isMetVal = true) {
		Object.defineProperties(this, {
			metConditions: {
				value: [],
				configurable: true,
				writable: true,
				enumerable: false
			},
			sources: {
				value: markRaw({}),
				configurable: true,
				writable: false,
				enumerable: false
			},
			otherStacks: {
				value: [],
				configurable: true,
				writable: false,
				enumerable: false
			},
			internalEvents: {
				value: markRaw(new QEventEmitter()),
				configurable: true,
				writable: false,
				enumerable: false
			},
			globalEvents: {
				value: undefined,
				configurable: true,
				writable: true,
				enumerable: false
			},
			processMonitor: {
				value: undefined,
				configurable: true,
				writable: true,
				enumerable: false
			},
			isMetVal: {
				value: isMetVal,
				configurable: true,
				writable: false,
				enumerable: false
			}
		})

		if (typeof events !== 'undefined') this.setEventEmitter(events)
	}

	/**
	 * The number os currently active conditions in the stack.
	 */
	get size() {
		return this.metConditions.length + this.otherStacks.reduce((res, s) => res + s.size, 0)
	}

	/**
	 * Whether any of the conditions in the stack are met.
	 */
	get anyMet() {
		return this.size > 0
	}

	/**
	 * Sets the global event emitter.
	 * @param {QEventEmitter} events The event emitter
	 */
	setEventEmitter(events) {
		if (!(events instanceof QEventEmitter))
			throw new TypeError('The "events" argument must be an instance of QEventEmitter.')
		this.globalEvents = events
	}

	/**
	 * Sets the monitor of asynchronous processes.
	 * @param {QAsyncProcessMonitor} processMonitor A monitor of asynchronous processes
	 */
	setProcessMonitor(processMonitor) {
		if (
			typeof processMonitor !== 'undefined' &&
			!(processMonitor instanceof QAsyncProcessMonitor)
		)
			throw new TypeError(
				'The "processMonitor" argument must be an instance of QAsyncProcessMonitor.'
			)
		this.processMonitor = processMonitor
	}

	/**
	 * Associates another stack to this one, this means the "anyMet" property will
	 * take into account whether that stack also has any met conditions.
	 * @param {ConditionStack} stack The stack to associate
	 */
	associateStack(stack) {
		if (!(stack instanceof ConditionStack))
			throw new TypeError('The "stack" argument must be an instance of ConditionStack.')

		// Re-emit the events emitted by the associated stack.
		stack.addOnMetListener(() => this.emitMetEvent(true))
		stack.addOnUnmetListener(() => this.emitUnmetEvent())

		this.otherStacks.push(stack)
	}

	/**
	 * Checks if the specified source is currently active.
	 * @param {string} sourceId The id of the source
	 * @returns True if the specified source is currently active, false otherwise.
	 */
	contains(sourceId) {
		return (
			this.metConditions.includes(sourceId) ||
			this.otherStacks.some((s) => s.contains(sourceId))
		)
	}

	/**
	 * Updates the stack with the condition sources currently active.
	 */
	updateStack() {
		const previousLength = this.metConditions.length

		this.metConditions = Object.entries(this.sources)
			.filter(([, value]) => value.isMet)
			.map(([key]) => key)

		// A source was added.
		if (previousLength < this.metConditions.length) this.emitMetEvent(true)
		// A source was removed.
		else if (previousLength > this.metConditions.length) this.emitUnmetEvent()
	}

	/**
	 * Adds a new condition source to the stack.
	 * @param {string} sourceId The id of the source
	 * @param {function} condition The condition to determine whether the source is active
	 * @param {string|array} eventIds An event to listen for, or a list of events
	 * @returns True if the source was successfully added, or false if it already existed in the stack.
	 */
	async add(sourceId, condition, eventIds) {
		if (typeof sourceId !== 'string' || sourceId.length === 0)
			throw new TypeError(
				'An invalid source was specified, the value should be a non-empty string.'
			)

		if (sourceId in this.sources) return false

		const source = await ConditionSource.createSource(
			condition,
			eventIds,
			this.globalEvents,
			this.isMetVal,
			this.processMonitor
		)
		source.addObserver(() => this.updateStack())
		this.sources[sourceId] = source

		if (source.isMet) this.updateStack()

		return true
	}

	/**
	 * Removes an existing condition source from the stack.
	 * @param {string} sourceId The id of the source
	 * @returns True if the source was successfully removed, or false if it didn't exist in the stack.
	 */
	remove(sourceId) {
		if (typeof sourceId !== 'string' || sourceId.length === 0)
			throw new TypeError(
				'An invalid source was specified, the value should be a non-empty string.'
			)

		if (!(sourceId in this.sources)) return false

		delete this.sources[sourceId]
		this.updateStack()

		return true
	}

	/**
	 * Emits an event signaling that the conditions are met (only if "anyMet" is true).
	 * @param {boolean} justAdded If true, will only emit the event if a source was just added
	 */
	emitMetEvent(justAdded = false) {
		if (this.size === 1 || (!justAdded && this.anyMet))
			this.internalEvents.emit(this.constructor.MET_EVENT)
	}

	/**
	 * Emits an event signaling that the conditions are unmet (only if "anyMet" is false).
	 */
	emitUnmetEvent() {
		if (!this.anyMet) this.internalEvents.emit(this.constructor.UNMET_EVENT)
	}

	/**
	 * Adds the specified listener to be executed whenever "anyMet" becomes true.
	 * @param {function} listener The listener function to add
	 */
	addOnMetListener(listener) {
		if (typeof listener !== 'function')
			throw new TypeError('The "listener" argument should be a function.')
		this.internalEvents.on(this.constructor.MET_EVENT, listener)
	}

	/**
	 * Adds the specified listener to be executed whenever "anyMet" becomes false.
	 * @param {function} listener The listener function to add
	 */
	addOnUnmetListener(listener) {
		if (typeof listener !== 'function')
			throw new TypeError('The "listener" argument should be a function.')
		this.internalEvents.on(this.constructor.UNMET_EVENT, listener)
	}

	/**
	 * Removes the specified listener for the met event.
	 * @param {function} listener The listener function to remove
	 */
	removeOnMetListener(listener) {
		if (typeof listener !== 'function')
			throw new TypeError('The "listener" argument should be a function.')
		this.internalEvents.off(this.constructor.MET_EVENT, listener)
	}

	/**
	 * Removes the specified listener for the unmet event.
	 * @param {function} listener The listener function to remove
	 */
	removeOnUnmetListener(listener) {
		if (typeof listener !== 'function')
			throw new TypeError('The "listener" argument should be a function.')
		this.internalEvents.off(this.constructor.UNMET_EVENT, listener)
	}

	/**
	 * Destroys this condition stack.
	 */
	destroy() {
		this.internalEvents?.removeAllListeners()
		delete this.internalEvents

		const sourceKeys = Object.keys(this.sources ?? {})
		sourceKeys.forEach((sourceKey) => {
			if (typeof this.sources[sourceKey]?.destroy === 'function')
				this.sources[sourceKey].destroy()
			this.sources[sourceKey] = null
			delete this.sources[sourceKey]
		})

		this.otherStacks.length = 0
		this.metConditions.length = 0
		delete this.globalEvents
		delete this.processMonitor
	}
}

/**
 * Class for the "Block when" condition stack.
 */
export class BlockConditionStack extends ConditionStack {
	constructor(events) {
		super(events)
	}

	/**
	 * Emits an event signaling that the field is blocked.
	 */
	emitBlockEvent() {
		this.emitMetEvent()
	}

	/**
	 * Emits an event signaling that the field is unblocked.
	 */
	emitUnblockEvent() {
		this.emitUnmetEvent()
	}

	/**
	 * Adds the specified listener to be executed when the field becomes blocked.
	 * @param {function} listener The listener function to add
	 */
	addOnBlockListener(listener) {
		this.addOnMetListener(listener)
	}

	/**
	 * Adds the specified listener to be executed when the field becomes unblocked.
	 * @param {function} listener The listener function to add
	 */
	addOnUnblockListener(listener) {
		this.addOnUnmetListener(listener)
	}

	/**
	 * Removes the specified listener for when the field becomes blocked.
	 * @param {function} listener The listener function to remove
	 */
	removeOnBlockListener(listener) {
		this.removeOnMetListener(listener)
	}

	/**
	 * Removes the specified listener for when the field becomes unblocked.
	 * @param {function} listener The listener function to remove
	 */
	removeOnUnblockListener(listener) {
		this.removeOnUnmetListener(listener)
	}
}

/**
 * Class for the "Fill when" condition stack.
 */
export class ClearConditionStack extends ConditionStack {
	constructor(events) {
		super(events, false)
	}

	/**
	 * Emits an event signaling that the field is cleared.
	 */
	emitClearEvent() {
		this.emitMetEvent()
	}

	/**
	 * Emits an event signaling that the field is fillable.
	 */
	emitFillEvent() {
		this.emitUnmetEvent()
	}

	/**
	 * Adds the specified listener to be executed when the field becomes cleared.
	 * @param {function} listener The listener function to add
	 */
	addOnClearListener(listener) {
		this.addOnMetListener(listener)
	}

	/**
	 * Adds the specified listener to be executed when the field becomes fillable.
	 * @param {function} listener The listener function to add
	 */
	addOnFillListener(listener) {
		this.addOnUnmetListener(listener)
	}

	/**
	 * Removes the specified listener for when the field becomes cleared.
	 * @param {function} listener The listener function to remove
	 */
	removeOnClearListener(listener) {
		this.removeOnMetListener(listener)
	}

	/**
	 * Removes the specified listener for when the field becomes fillable.
	 * @param {function} listener The listener function to remove
	 */
	removeOnFillListener(listener) {
		this.removeOnUnmetListener(listener)
	}
}

/**
 * Class for the "Show when" condition stack.
 */
export class HideConditionStack extends ConditionStack {
	constructor(events) {
		super(events, false)
	}

	/**
	 * Emits an event signaling that the field is hidden.
	 */
	emitHideEvent() {
		this.emitMetEvent()
	}

	/**
	 * Emits an event signaling that the field is visible.
	 */
	emitShowEvent() {
		this.emitUnmetEvent()
	}

	/**
	 * Adds the specified listener to be executed when the field becomes hidden.
	 * @param {function} listener The listener function to add
	 */
	addOnHideListener(listener) {
		this.addOnMetListener(listener)
	}

	/**
	 * Adds the specified listener to be executed when the field becomes visible.
	 * @param {function} listener The listener function to add
	 */
	addOnShowListener(listener) {
		this.addOnUnmetListener(listener)
	}

	/**
	 * Removes the specified listener for when the field becomes hidden.
	 * @param {function} listener The listener function to remove
	 */
	removeOnHideListener(listener) {
		this.removeOnMetListener(listener)
	}

	/**
	 * Removes the specified listener for when the field becomes visible.
	 * @param {function} listener The listener function to remove
	 */
	removeOnShowListener(listener) {
		this.removeOnUnmetListener(listener)
	}
}

/**
 * Class for the required condition stack.
 */
export class RequiredConditionStack extends ConditionStack {
	constructor(events) {
		super(events)
	}

	/**
	 * Emits an event signaling that the field is required.
	 */
	emitRequireEvent() {
		this.emitMetEvent()
	}

	/**
	 * Emits an event signaling that the field isn't required.
	 */
	emitUnrequireEvent() {
		this.emitUnmetEvent()
	}

	/**
	 * Adds the specified listener to be executed when the field becomes required.
	 * @param {function} listener The listener function to add
	 */
	addOnRequireListener(listener) {
		this.addOnMetListener(listener)
	}

	/**
	 * Adds the specified listener to be executed when the field becomes unrequired.
	 * @param {function} listener The listener function to add
	 */
	addOnUnrequireListener(listener) {
		this.addOnUnmetListener(listener)
	}

	/**
	 * Removes the specified listener for when the field becomes required.
	 * @param {function} listener The listener function to remove
	 */
	removeOnRequireListener(listener) {
		this.removeOnMetListener(listener)
	}

	/**
	 * Removes the specified listener for when the field becomes unrequired.
	 * @param {function} listener The listener function to remove
	 */
	removeOnUnrequireListener(listener) {
		this.removeOnUnmetListener(listener)
	}
}
