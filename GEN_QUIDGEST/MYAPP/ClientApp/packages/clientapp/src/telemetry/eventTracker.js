import { shallowReactive } from 'vue'
import _filter from 'lodash-es/filter'
import _forEach from 'lodash-es/forEach'
import _get from 'lodash-es/get'
import _groupBy from 'lodash-es/groupBy'
import _isArray from 'lodash-es/isArray'
import _isEmpty from 'lodash-es/isEmpty'
import _isString from 'lodash-es/isString'
import _map from 'lodash-es/map'
import _orderBy from 'lodash-es/orderBy'
import _uniq from 'lodash-es/uniq'
import { TelemetryHandler } from './telemetryHandler'
import {
	TraceEvent,
	WarningEvent,
	ResponseEvent,
	RequestEvent,
	ServerErrorEvent,
	TraceEventType
} from './tracingEvents'

/**
 * Maximum number of events to be stored by default.
 * @type {number}
 */
const DEFAULT_MAX_EVENTS_STACK = 75

/**
 * Class for tracking and managing events.
 */
export class EventTracker {
	/**
	 * Creates an EventTracker instance.
	 * @param {Object} options - Tracker options.
	 * @param {boolean} [options.active=false] - Whether event tracking is active.
	 * @param {boolean} [options.enableTracing=false] - Wether to enable tracing or not (logs are always on)
	 */
	constructor(options) {
		/**
		 * Array to store events.
		 * @type {Array}
		 */
		this.events = shallowReactive([])

		/**
		 * Helpers to link events, such as RequestEvent and ResponseEvent
		 * where the key is the traceid (which is shared) and the value is
		 * the span object instance.
		 * @type {object}
		 */
		this.eventLinks = shallowReactive({})

		this.active = _get(options, 'active', false)

		/**
		 * Maximum number of events to be stored.
		 * @type {number}
		 */
		this.maxEventsStack = _get(options, 'maxEventsStack', DEFAULT_MAX_EVENTS_STACK)

		/**
		 * Start telemetry handler class.
		 */
		this.enableTracing = _get(options, 'enableTracing', false)
		this.telemetryHandler = new TelemetryHandler(this.enableTracing)
	}

	/**
	 * Resets the event tracker by clearing stored events.
	 * It also restarts the telemetryHandler instance to make sure the
	 * enableTracing is applied correctly
	 */
	reset() {
		this.events.splice()

		if (typeof this.telemetryHandler?.dispose === 'function') this.telemetryHandler.dispose()
		this.telemetryHandler = new TelemetryHandler(this.enableTracing)
	}

	/**
	 * Adds an event to the tracker.
	 * @param {TraceEvent|WarningEvent|ErrorEvent|RequestEvent|ResponseEvent} event - The event to be added.
	 */
	addEvent(event) {
		if (this.active && event instanceof TraceEvent) {
			this.events.push(event)

			if (this.events.length > this.maxEventsStack)
				this.events.splice(0, this.events.length - this.maxEventsStack)
		}
		return event?.traceId
	}

	/**
	 * Adds a trace event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addTrace(options) {
		const event = new TraceEvent(options)

		this.telemetryHandler.registerTrace(event)

		return this.addEvent(event)
	}

	/**
	 * Adds a warning event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addWarning(options) {
		const event = new WarningEvent(options)

		// To facilitate debugging during development, errors will be added to the console
		// as the debug window will only be accessible if the feature is activated.
		if (import.meta.env.DEV)
			// eslint-disable-next-line no-console
			console.warn('Tracing Warning', options)

		this.telemetryHandler.registerLog(event)

		return this.addEvent(event)
	}

	/**
	 * Adds an error event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addError(options) {
		const event = new ErrorEvent(options)

		// To facilitate debugging during development, warnings will be added to the console
		// as the debug window will only be accessible if the feature is activated.
		if (import.meta.env.DEV)
			// eslint-disable-next-line no-console
			console.error('Tracing Error', options)

		this.telemetryHandler.registerLog(event)

		return this.addEvent(event)
	}

	/**
	 * Adds a request trace event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addRequestTrace(options) {
		const event = new RequestEvent(options)

		this.telemetryHandler.registerTrace(event)

		return this.addEvent(event)
	}

	/**
	 * Adds a response trace event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addResponseTrace(options) {
		const event = new ResponseEvent(options)

		this.telemetryHandler.registerTrace(event)

		return this.addEvent(event)
	}

	/**
	 * Adds a Server error event to the tracker.
	 * @param {Object} options - Event options.
	 */
	addServerError(options) {
		return this.addEvent(new ServerErrorEvent(options))
	}

	/**
	 * Adds a Server error events to the tracker.
	 * It must contain an «errors» property with the array of error strings.
	 * @param {Object} options - Event options.
	 */
	addServerErrors(options) {
		const errors = _get(options, 'errors')
		const contextData = _get(options, 'contextData')
		const traceId = _get(options, 'traceId')

		if (_isArray(errors)) {
			_forEach(errors, (srvError) => {
				if (_isString(srvError)) {
					this.addServerError({
						origin: 'server',
						callStack: '-',
						message: srvError,
						contextData,
						traceId
					})
				}
			})
		}
	}

	/**
	 * Retrieves events of a specific type.
	 * @param {string} traceEventType - The type of events to retrieve.
	 * @returns {Array} An array of events of the specified type.
	 */
	getEventsOfType(traceEventType) {
		if (!_isEmpty(traceEventType))
			return _filter(this.events, (event) => event.type === traceEventType)
		return this.events
	}

	/**
	 * Retrieves events of specific types.
	 * @param {Array} traceEventTypes - The types of events to retrieve.
	 * @returns {Array} An array of events of the specified types.
	 */
	getEventsOfTypes(traceEventTypes) {
		if (_isArray(traceEventTypes) && traceEventTypes.length > 0)
			return _filter(this.events, (event) => traceEventTypes.includes(event.type))
		return this.events
	}

	/**
	 * Groups events by «traceId».
	 * @returns {Array} An array of events grouped by «traceId».
	 */
	getEventsByGroup() {
		const errors = this.getEventsOfTypes([TraceEventType.ERROR, TraceEventType.SERVER_ERROR])
		const traceIds = _uniq(_map(errors, (error) => error.traceId))
		const relatedEvents = _filter(this.events, (event) => traceIds.includes(event.traceId))
		const groups = _groupBy(_orderBy(relatedEvents, 'timestamp'), 'traceId')
		return groups
	}
}

/**
 * Module exporting TraceEventType and event classes
 * @module
 */
export default {
	EventTracker
}
