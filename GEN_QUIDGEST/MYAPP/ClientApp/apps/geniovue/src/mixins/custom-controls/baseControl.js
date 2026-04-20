import _isEmpty from 'lodash-es/isEmpty'
import { reactive } from 'vue'

import eventBus from '@quidgest/clientapp/plugins/eventBus'
import { imageObjToSrc } from '@quidgest/clientapp/utils/genericFunctions'

/**
 * Generic custom control
 */
export default class CustomControl
{
	constructor(controlContext, controlOrder)
	{
		this.id = controlContext.id;
		this.controlContext = controlContext
		this.controlOrder = controlOrder
		this.handlers = {}
		this.customProperties = {}
		this.usesFullSizeImg = false
	}

	/**
	 * Checks whether or not the view mode should be blocked.
	 * @returns True if it's blocked, false otherwise.
	 */
	checkIsReadonly()
	{
		return false
	}

	/**
	 * Adds a new handler for the specified event.
	 * @param {string} id The id of the event
	 * @param {function} behavior The behavior of the handler
	 * @param {boolean} rewrite Whether or not the previous behavior should be rewritten (defaults to false).
	 */
	addHandler(id, behavior, rewrite = false)
	{
		if (typeof id !== 'string' || typeof behavior !== 'function')
			return

		const viewMode = this.controlContext.viewModes[this.controlOrder - 1]
		const prevHandler = viewMode.handlers[id]
		let behaviorFunc = behavior

		if (!rewrite && typeof prevHandler === 'function')
		{
			behaviorFunc = (...args) => {
				prevHandler(...args)
				behavior(...args)
			}
		}

		this.handlers[id] = behaviorFunc
		viewMode.handlers[id] = behaviorFunc
	}

	/**
	 * Sets a property in the control.
	 * @param {string} id The id of the property
	 * @param {any} value The value of the property
	 */
	setProperty(id, value)
	{
		if (typeof id !== 'string')
			return

		const viewMode = this.controlContext.viewModes[this.controlOrder - 1]
		viewMode[id] = value
	}

	/**
	 * Sets any additional properties that might be needed for the control.
	 * @param {object} viewMode The current view mode
	 */
	setGenericCustomProps(viewMode)
	{
		for (const i in this.customProperties)
			viewMode[i] = this.customProperties[i]
	}

	/**
	 * Fetches image data for a given row key and image object.
	 * @param {string} rowKey - The key of the row containing the image.
	 * @param {string} imageVar - The name of the image variable.
	 */
	fetchImage(rowKey, imageVar)
	{
		const viewMode = this.controlContext.viewModes[this.controlOrder - 1]
		let imageObj = viewMode.mappedValues.find((row) => row.rowKey === rowKey)[imageVar]

		// When we don't have a ticket to retrieve the image field value, we will not send a request to the server.
		if (_isEmpty(imageObj) || !_isEmpty(imageObj.previewData) || _isEmpty(imageObj.rawData?.ticket))
			return

		const column = imageObj.source,
			baseArea = column.area,
			ticket = imageObj.rawData.ticket,
			params = {
				ticket,
				nocache: Math.floor(Math.random() * 100000)
			}

		const imageData = {
			baseArea,
			params,
			callback(data)
			{
				/*
				 * While the response is arriving, the rows may change and the assignment
				 * will be made on the image object that is no longer used.
				 */
				imageObj = viewMode.mappedValues.find((row) => row.rowKey === rowKey)
				if (imageObj)
					reactive(imageObj)[imageVar].previewData = imageObjToSrc(data)
			}
		}

		eventBus.emit('image-request', imageData)
	}
}
