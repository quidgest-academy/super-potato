import _assignIn from 'lodash-es/assignIn'
import _forEach from 'lodash-es/forEach'
import _isEmpty from 'lodash-es/isEmpty'

import { postData } from '@quidgest/clientapp/network'
import { hydrateDashboardData, hydrateWidgetData } from './dashboardFunctions.js'

/*****************************************************************
 * This mixin defines methods to be reused in dashboards.        *
 *****************************************************************/
export default {
	methods: {
		/**
		 * Fetches the data from the server and loads the dashboard.
		 * @param {object} dashboardControl - The dashboard control object.
		 * @param {object} params - The necessary parameters for the request.
		 * @returns {Promise} A promise that resolves once the dashboard data is fetched and loaded.
		 */
		async fetchDashboardData(dashboardControl, params)
		{
			if (_isEmpty(params))
				params = {}

			_assignIn(params, this.$route.params)

			// Set the required action property if it's empty
			if (!Reflect.has(params, 'queryParams'))
				Reflect.set(params, 'queryParams', { ...params })

			return new Promise((resolve, reject) => {
				postData(
					'Dashboard',
					dashboardControl.action,
					params,
					(data) => {
						try
						{
							hydrateDashboardData(dashboardControl, data)

							_forEach(dashboardControl.widgets, (widget) => {
								if (widget.requiresAdditionalData)
								{
									widget.data = null

									// This will fire async requests for additional widget data
									// We don't need to wait for that here,
									// widget will have dedicated loader in that case
									this.fetchWidgetData(dashboardControl, widget)
								}
							})

							resolve(true)
						}
						catch (error)
						{
							reject(error)
						}
					}
				)
			})
		},

		/**
		 * Fetches the widget data from the server.
		 * @param {object} dashboardControl The dashboard control object
		 * @param {object} widget The widget to fetch data for
		 * @returns A promise with the response from the server.
		 */
		fetchWidgetData(dashboardControl, widget)
		{
			const params = {
				widgetType: widget.Type,
				widgetId: widget.Id
			}

			postData(
				'Dashboard',
				`${dashboardControl.action}_GetWidgetData`,
				params,
				(data) => widget.data = hydrateWidgetData(widget, data)
			)
		},

		/**
		 *
		 * @param {object} dashboardControl The dashboard control object
		 * @returns A promise with the response from the server.
		 */
		onDashboardSave(dashboardControl)
		{
			const params = {
				Widgets: dashboardControl.widgets
			}

			postData('Dashboard', `${dashboardControl.action}_Save`, params)
		}
	}
}
