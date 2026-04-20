import CardsControl from './cardsControl.js'

/**
 * Gets the class that implements the custom control with the desired control type.
 * @param {string} controlType The type of the custom control
 * @param {object} context The view mode context
 * @param {number} viewModeOrder The order of the view mode
 * @returns The class corresponding to the specified custom control, or null.
 */
export default function getCustomControl(controlType, context, viewModeOrder)
{
	switch (controlType)
	{
		case 'cards':
			return new CardsControl(context, viewModeOrder)
	}

	return null
}
