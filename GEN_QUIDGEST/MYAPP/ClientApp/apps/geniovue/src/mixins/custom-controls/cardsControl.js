import { computed } from 'vue'

import CustomControl from './baseControl.js'
import CardsResources from './resources/cardsResources.js'

/**
 * Cards control
 */
export default class CardsControl extends CustomControl {
	constructor(controlContext, controlOrder) {
		super(controlContext, controlOrder)

		this.texts = new CardsResources(controlContext.vueContext.$getResource)
		this.usesFullSizeImg = true

		// Cards-specific handlers
		this.handlers = {
			'update:visible': (id) => this.onUpdateVisible(id)
		}
	}

	/**
	 * Get the properties for configuring the cards component.
	 * @param {object} viewMode - The current view mode of the cards.
	 * @returns {object} - An object containing cards properties.
	 */
	getProps(viewMode) {
		return {
			id: viewMode.containerId,
			subtype: viewMode.subtype,
			cards: viewMode.mappedValues,
			// Actions
			actionsAlignment: viewMode.styleVariables.actionsAlignment?.value === 'right' ? 'end' : 'start',
			actionsPlacement: viewMode.styleVariables.actionsPlacement?.value,
			actionsStyle: viewMode.styleVariables.actionsStyle?.value,
			// Visuals
			contentAlignment: viewMode.styleVariables.contentAlignment?.value === 'center' ? 'center' : 'start',
			containerAlignment: viewMode.styleVariables.containerAlignment?.value === 'center' ? 'center' : 'start',
			imageShape: viewMode.styleVariables.imageShape?.value,
			hoverScaleAmount: viewMode.styleVariables.hoverScaleAmount?.value,
			size: viewMode.styleVariables.size?.value,
			// Insert Card
			customFollowupTarget: viewMode.styleVariables.customFollowupTarget?.value,
			customInsertCard: viewMode.styleVariables.customInsertCard?.value,
			customInsertCardStyle: viewMode.styleVariables.customInsertCardStyle?.value,
			// Display
			displayMode: viewMode.styleVariables.displayMode?.value,
			gridMode: viewMode.styleVariables.gridMode?.value,
			showColumnTitles: viewMode.styleVariables.showColumnTitles?.value,
			showEmptyColumnTitles: viewMode.styleVariables.showEmptyColumnTitles?.value,
			// Config / State
			listConfig: this.controlContext.config,
			texts: this.texts,
			readonly: computed(() => viewMode.readonly),
			loading: !this.controlContext.loaded
		}
	}

	/**
	 * Sets any additional properties that might be needed for the cards
	 * @param {object} viewMode The current view mode
	 */
	setCustomProperties(viewMode) {
		viewMode.implementsOwnInsert = viewMode.styleVariables.customInsertCard?.value ?? false
	}

	/**
	 * Handles the model value update event.
	 * @param {string} rowKey - The key of the current slide.
	 */
	onUpdateVisible(rowKey) {
		this.fetchImage(rowKey, 'image')
	}
}
