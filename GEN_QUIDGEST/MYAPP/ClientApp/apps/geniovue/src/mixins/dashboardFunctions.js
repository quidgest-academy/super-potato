import _forEach from 'lodash-es/forEach'

import { hydrateAlert } from './alertFunctions.js'

/**
 * Hydrates the dashboard data.
 * @param {object} dashboardControl The dashboard control object
 * @param {object} viewModel The dashboard view model (C#)
 */
export function hydrateDashboardData(dashboardControl, viewModel)
{
	if (typeof viewModel === 'undefined')
		return

	dashboardControl.widgets = []

	_forEach(viewModel.Widgets, (widget) => {
		widget.uuid = widget.Rowkey
			? `w-${widget.Id}-${widget.Rowkey}`
			: `w-${widget.Id}`

		switch (widget.Type)
		{
			case 0: // Alert widget
				widget.component = 'q-alert-widget'
				widget.requiresAdditionalData = true
				break
			case 1: // Bookmark widget
			case 4: // Menu widget
				if (widget.RenderSubmenus)
					widget.component = 'q-sub-menus-widget'
				else
					widget.component = 'q-menu-widget'
				break
			case 2: // Custom widget
			case 3: // Custom paginated widget
				widget.component = 'q-custom-widget'
				break
			default:
				widget.component = ''
				break
		}

		dashboardControl.widgets.push(widget)
	})
}

/**
 * Hydrates widget data based on the widget type.
 * @param {Object} widget - The widget object containing information about the widget type.
 * @param {Object} data - The data object containing information to hydrate the widget.
 * @returns {Object|undefined} - The hydrated widget data or undefined if the widget type is not supported.
 */
export function hydrateWidgetData(widget, data)
{
	switch (widget.Type)
	{
		case 0: // Alert widget
			return hydrateAlert(data, false)
		default:
			return undefined
	}
}
