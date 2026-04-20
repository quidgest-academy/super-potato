import _assignIn from 'lodash-es/assignIn'

import { postData } from '../../network'
import { useTracingDataStore } from '../../stores/tracingData'
import { Base } from './base'

export class Document extends Base {
	static EMPTY_VALUE = ''

	constructor(options) {
		super(
			_assignIn(
				{
					type: 'Document',
					currentDocument: null,
					tickets: {},
					properties: null,
					documentFK: null
				},
				options
			)
		)
	}

	/**
	 * @override
	 */
	get isDirty() {
		return super.isDirty || this.properties.isDirty || this.currentDocument.isDirty
	}

	/**
	 * @override
	 */
	isValidType(value) {
		return typeof value === 'string' || value === this.constructor.EMPTY_VALUE
	}

	/**
	 * Sets the tickets to retrieve every document version from the server.
	 * @param {string} primaryKey The primary key of the current record
	 * @param {string} navigationId The current navigation id
	 * @returns A promise with the response from the server.
	 */
	setTickets(primaryKey, navigationId) {
		const params = {
			fieldName: this.originId,
			keyValue: primaryKey
		}

		return new Promise((resolve) => {
			postData(
				this.area,
				'GetDocumsTickets',
				params,
				(data, request) => {
					if (request.data?.Success) {
						this.tickets = {}

						for (const i in data.tickets) {
							const t = data.tickets[i]
							this.tickets[t.id] = t.ticket
						}

						this.properties.updateValue(data.properties)
						this.documentFK.updateValue(data.properties?.documentId ?? '')

						// Sets up the current document properties.
						this.currentDocument.setup(this.id, this.tickets.main)

						resolve(true)
					} else {
						const tracingDataStore = useTracingDataStore()
						tracingDataStore.addError({
							origin: 'setTickets',
							message: `Error found while trying to retrieve the document tickets for field "${this.id}".`
						})

						resolve(false)
					}
				},
				undefined,
				undefined,
				navigationId
			)
		})
	}
}
