import EventEmitter from 'eventemitter3'

// https://github.com/primus/eventemitter3
export class QEventEmitter extends EventEmitter {
	constructor() {
		super()
	}

	onMany(events, fn, context) {
		if (!events) return false
		events.forEach((event) => this.on.call(this, event, fn, context))
		return true
	}

	offMany(events, fn, context) {
		if (!events) return false
		events.forEach((event) => this.off(event, fn, context))
		return true
	}
}

const eventBus = new QEventEmitter()

export default eventBus
