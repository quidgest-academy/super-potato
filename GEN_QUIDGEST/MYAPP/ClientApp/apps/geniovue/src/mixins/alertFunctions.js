import isEmpty from 'lodash-es/isEmpty'

import { systemInfo } from '@/systemInfo'

/**
 * Retrieves app alert information based on the provided ID.
 *
 * @param {string} id - The ID of the app alert to retrieve.
 * @returns {Object|undefined} - The app alert information or undefined if not found.
 */
export function getAppAlert(id)
{
	const appAlert = systemInfo.appAlerts.find((alert) => alert.id === id)

	if (isEmpty(appAlert.module))
		return undefined

	return appAlert
}

/**
 * Hydrates alert data based on provided information.
 *
 * @param {Object} data - The data object containing information about the alert.
 * @param {Boolean} useThreshold - Set to true if the alert should not be displayed if it doesn't hit the threshold
 * @returns {Object|undefined} - The hydrated alert object or undefined if the alert is not valid.
 */
export function hydrateAlert(data, useThreshold)
{
	const appAlert = getAppAlert(data.Idalert)

	//This ensures the alert still shows properly in the dashboard when configured to not show in the alerts.
	if (appAlert && (!useThreshold || data.Count > appAlert.disableIfLowerThan))
	{
		const alert = { ...appAlert }

		// TODO: generate to systemData store if static
		alert.type = data.Type
		// TODO: generate to systemData store
		alert.target = data.Target

		if (!appAlert.isResource)
		{
			alert.title = data.Title
			alert.description = data.Content
		}
		else
		{
			alert.count = data.Count
		}

		return alert
	}

	return undefined
}
