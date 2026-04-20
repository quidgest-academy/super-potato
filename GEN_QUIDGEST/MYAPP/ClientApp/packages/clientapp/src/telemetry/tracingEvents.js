import _get from 'lodash-es/get'
import _toSafeInteger from 'lodash-es/toSafeInteger'
import { v4 as uuidv4 } from 'uuid'

import { deepUnwrap } from '../utils/deepUnwrap'

/**
 * Function to retrieve the call stack.
 * @returns {string} Call stack as a string.
 */
function getCallStack() {
	return new Error().stack
}

/**
 * Enumeration of trace event types.
 * @enum {string}
 */
export const TraceEventType = {
	TRACE: 'trace',
	WARNING: 'warning',
	ERROR: 'error',
	REQUEST: 'request',
	RESPONSE: 'response',
	SERVER_ERROR: 'server error'
}

/**
 * Represents a trace event.
 */
export class TraceEvent {
	/**
	 * Creates a TraceEvent instance.
	 * @param {Object} options - Event options.
	 * @param {string} [options.origin=''] - Origin of the event.
	 * @param {string} [options.message=''] - Event message.
	 * @param {string} [options.callStack=getCallStack()] - Call stack when the event occurred.
	 * @param {*} options.contextData - Context data associated with the event.
	 * @param {number} [options.timestamp=Date.now()] - Timestamp of the event.
	 */
	constructor(options) {
		this.uid = uuidv4()

		this.traceId = _get(options, 'traceId', this.uid)

		this.origin = _get(options, 'origin', '')
		this.message = _get(options, 'message', '')
		this.callStack = _get(options, 'callStack', getCallStack())
		this.timestamp = _get(options, 'timestamp', Date.now())

		try {
			this.contextData = JSON.stringify(deepUnwrap(_get(options, 'contextData')))
		} catch (err) {
			this.contextData = `TraceEvent - Failed to get context data. ${err?.message}`
			// To facilitate debugging during development
			if (import.meta.env.DEV)
				// eslint-disable-next-line no-console
				console.error(this.contextData, err)
		}

		this.type = TraceEventType.TRACE
	}
}

/**
 * Represents a warning event.
 */
export class WarningEvent extends TraceEvent {
	/**
	 * Creates a WarningEvent instance.
	 * @param {Object} options - Event options.
	 */
	constructor(options) {
		super(options)
		this.type = TraceEventType.WARNING
	}
}

/**
 * Represents an error event.
 */
export class ErrorEvent extends TraceEvent {
	/**
	 * Creates an ErrorEvent instance.
	 * @param {Object} options - Event options.
	 */
	constructor(options) {
		super(options)
		this.type = TraceEventType.ERROR
	}
}

/**
 * Represents a request event.
 */
export class RequestEvent extends TraceEvent {
	/**
	 * Creates a RequestEvent instance.
	 * @param {Object} options - Event options.
	 * @param {string} [options.requestType=''] - The type of the request.
	 * @param {string} [options.requestUrl=''] - The URL of the request.
	 * @param {*} options.requestParams - Parameters of the request.
	 * @param {*} options.requestData - Data associated with the request.
	 */
	constructor(options) {
		super(options)

		/**
		 * The type of the request.
		 * @type {string}
		 */
		this.requestType = _get(options, 'requestType', '')

		/**
		 * The URL of the request.
		 * @type {string}
		 */
		this.requestUrl = _get(options, 'requestUrl', '')

		/**
		 * Parameters of the request.
		 * @type {*}
		 */
		this.requestParams = _get(options, 'requestParams')

		/**
		 * Data associated with the request.
		 * @type {*}
		 */
		this.requestData = _get(options, 'requestData')

		this.type = TraceEventType.REQUEST
	}
}

/**
 * Represents a response event.
 */
export class ResponseEvent extends RequestEvent {
	/**
	 * Creates a ResponseEvent instance.
	 * @param {Object} options - Event options.
	 * @param {string} [options.responseStatus=''] - The status of the response.
	 * @param {*} options.responseData - Data associated with the response.
	 */
	constructor(options) {
		super(options)

		/**
		 * The status of the response.
		 * @type {string}
		 */
		this.responseStatus = _get(options, 'responseStatus', '')

		/**
		 * Data associated with the response.
		 * @type {*}
		 */
		this.responseData = _get(options, 'responseData')

		const startAt = _toSafeInteger(_get(options, 'meta.requestStartAt', 0)),
			endAt = _toSafeInteger(_get(options, 'meta.requestEndAt', 0))

		/**
		 * The request duration.
		 * @type {Number}
		 */
		this.time = endAt - startAt

		this.type = TraceEventType.RESPONSE
	}
}

export class ServerErrorEvent extends TraceEvent {
	/**
	 * Creates an ServerErrorEvent instance.
	 * @param {Object} options - Event options.
	 */
	constructor(options) {
		super(options)
		this.type = TraceEventType.SERVER_ERROR
	}
}

/**
 * Module exporting TraceEventType and event classes
 * @module
 */
export default {
	TraceEventType,
	TraceEvent,
	WarningEvent,
	ErrorEvent
}
