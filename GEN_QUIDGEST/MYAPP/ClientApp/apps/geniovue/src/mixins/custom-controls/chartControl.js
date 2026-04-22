import CustomControl from './baseControl.js'

/**
 * Chart control
 */
export default class ChartControl extends CustomControl
{
	constructor(controlContext, controlOrder)
	{
		super(controlContext, controlOrder)
	}

	/**
	 * Get the properties for configuring the chart component.
	 * @param {object} viewMode - The current view mode of the chart.
	 * @returns {object} - An object containing chart properties.
	 */
	getProps(viewMode)
	{
		return {
			containerId: viewMode.containerId,
			mappedValues: viewMode.mappedValues,
			styleVariables: viewMode.styleVariables,
			config: {}
		}
	}
}
