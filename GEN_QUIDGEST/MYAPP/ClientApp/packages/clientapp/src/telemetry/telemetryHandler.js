import { apiActionURL } from '../network/utils'
import { ErrorEvent, WarningEvent } from './tracingEvents'

export class TelemetryHandler {
	/**
	 * Base constructor
	 */
	constructor(enableTracing = false) {
		this.eventQueue = []
		this.batchInterval = 30000
		this.isSending = false
		this.enableTracing = enableTracing

		this._interval = null

		this.startBatching()

		// Capture global errors
		this.error_globalhandler = (event) => {
			this.registerLog(
				new ErrorEvent({
					origin: event.filename,
					message: event.message,
					callStack: event.error?.stack || ''
				})
			)
		}
		window.addEventListener('error', this.error_globalhandler)

		// Capture unhandled promise rejections
		this.unhandledrejection_globalhandler = (event) => {
			this.registerLog(
				new ErrorEvent({
					origin: window.location.href,
					message: event.reason?.message || 'Unhandled promise rejection',
					callStack: event.reason?.stack || ''
				})
			)
		}
		window.addEventListener('unhandledrejection', this.unhandledrejection_globalhandler)
	}

	/**
	 * Register a trace
	 * @param {object} event ResponseEvent or RequestEvent
	 */
	registerTrace(/*event*/) {
		//For now, client side traces have been completely disabled until they are refactored
		//we should not be tracing requests-responses, we should be tracing user interactions
		//because request-response is already being traced server side.
		/*
		if (!this.enableTracing)
			return

		const params = {
			telemetryType: 'Trace',
			...event
		}

		this.addToQueue(params)
		*/
	}

	/**
	 * Register a log
	 * @param {object} event WarningEvent or ErrorEvent
	 */
	registerLog(event) {
		let telemetryType = 'InfoLog'
		if (event instanceof WarningEvent) telemetryType = 'WarningLog'
		else if (event instanceof ErrorEvent) telemetryType = 'ErrorLog'

		const params = {
			telemetryType,
			...event
		}

		this.addToQueue(params)
	}

	/**
	 * Add an event to the queue
	 * @param {object} event
	 */
	addToQueue(event) {
		this.eventQueue.push(event)
	}

	/**
	 * Send batched events to the backend
	 */
	sendBatch() {
		if (this.eventQueue.length === 0 || this.isSending) return

		this.isSending = true
		const eventsToSend = [...this.eventQueue]
		this.eventQueue = [] // Clear the queue before sending to avoid duplication

		try {
			const url = apiActionURL('InternalProcess', 'RegisterTelemetry'),
				tokenElements = document.getElementsByName('__RequestVerificationToken'),
				antiForgeryToken = tokenElements.length > 0 ? tokenElements[0].value : null

			fetch(url, {
				method: 'POST',
				headers: {
					__RequestVerificationToken: antiForgeryToken,
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ events: eventsToSend })
			}).then((data) => {
				if (import.meta.env.DEV)
					// eslint-disable-next-line no-console
					console.log('Batch sent successfully:', data)
			})
		} catch (error) {
			if (import.meta.env.DEV)
				// eslint-disable-next-line no-console
				console.error('Failed to send telemetry batch:', error)

			// Add events back into the queue if sending fails
			this.eventQueue = [...eventsToSend, ...this.eventQueue]
		} finally {
			this.isSending = false
		}
	}

	/**
	 * Start the batching process
	 */
	startBatching() {
		if (this._interval) clearInterval(this._interval)
		this._interval = setInterval(this.sendBatch.bind(this), this.batchInterval)
	}

	dispose() {
		if (this._interval) clearInterval(this._interval)
		this._interval = null
		if (this.eventQueue?.length > 0) this.eventQueue.length = 0

		window.removeEventListener('error', this.error_globalhandler)
		this.error_globalhandler = null
		window.removeEventListener('unhandledrejection', this.unhandledrejection_globalhandler)
		this.unhandledrejection_globalhandler = null
	}
}

export default {
	TelemetryHandler
}
