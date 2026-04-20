import _assignIn from 'lodash-es/assignIn'

import asyncProcM from '@quidgest/clientapp/composables/async'
import { forceDownload, getFileNameFromRequest, postData } from '@quidgest/clientapp/network'
import eventBus from '@quidgest/clientapp/plugins/eventBus'
import { useSystemDataStore } from '@quidgest/clientapp/stores'

export default {
	methods: {
		navigateToRouteName(routeName, options, query, prefillValues)
		{
			return navigateToRouteName(this, routeName, options, query, prefillValues)
		},

		navigateToForm(form, mode, id, options, query, prefillValues)
		{
			return navigateToForm(this, form, mode, id, options, query, prefillValues)
		},

		navigateToModule(module)
		{
			return navigateToModule(this, module)
		},

		navigateToReport(controller, action, options, preview)
		{
			return navigateToReport(this, controller, action, options, preview)
		},

		navigateToReportingServicesViewer(controller, action, options)
		{
			return navigateToReportingServicesViewer(this, controller, action, options)
		},

		linkToRouteName(routeName, options, query, prefillValues)
		{
			return linkToRouteName(this, routeName, options, query, prefillValues)
		}
	}
}

export function navigateToRouteName(vueInstance, routeName, options, query, prefillValues = {})
{
	const systemDataStore = useSystemDataStore()
	const culture = systemDataStore.system.currentLang
	const system = systemDataStore.system.currentSystem
	const module = systemDataStore.system.currentModule

	const params = _assignIn({ culture, system, module }, {
		historyBranchId: vueInstance.navigationId
	}, options, { prefillValues: JSON.stringify(prefillValues) })

	return vueInstance.$router.push({ name: routeName, params, query })
}

export function navigateToForm(vueInstance, form, mode, id, options, query, prefillValues)
{
	const params = _assignIn({ mode }, id ? { id } : {}, options),
		formRouteName = `form-${form}`

	return navigateToRouteName(vueInstance, formRouteName, params, query, prefillValues)
}

export function navigateToModule(vueInstance, module)
{
	return navigateToRouteName(vueInstance, `home-${module}`, { module })
}

export function navigateToReport(vueInstance, controller, action, options, preview)
{
	return new Promise((resolve) => {
		asyncProcM.addBusy(
			postData(
				controller,
				action,
				options,
				(_, request) => {
					const fileName = getFileNameFromRequest(request)
					const fileType = request.headers['content-type']

					if (!fileName)
					{
						resolve(false)
						return
					}

					forceDownload(request.data, fileName, fileType, preview)
					resolve(true)
				},
				undefined,
				{ responseType: 'arraybuffer' },
				vueInstance.navigationId))
	})
}

export function navigateToReportingServicesViewer(vueInstance, controller, action, options)
{
	return new Promise((resolve) => {
		postData(
			controller,
			action,
			options,
			async (data) => {
				if (data?.id)
				{
					const navResult = await navigateToRouteName(vueInstance, 'reporting-services-viewer', { id: data.id })
					resolve(navResult)
				}
				else
					resolve()
			},
			undefined,
			undefined,
			vueInstance.navigationId)
	})
}

export function processRedirect(vueInstance, data)
{
	switch (data.type)
	{
		case 'menu':
			navigateToRouteName(vueInstance, `menu-${data.menuId}`, data.routeValues)
			break
		case 'menu-mc':
			eventBus.emit(`EXEC-${data.menuId}`, (data.routeValues || {}))
			break
		case 'menu-routine':
			eventBus.emit(`EXEC-MENU-ROUTINE-${data.menuId}`, { routineName: data.routineName, params: data.routeValues })
			break
		case 'form':
			{
				const params = _assignIn({ isControlled: true }, data.routeValues || {})
				navigateToForm(vueInstance, data.formName, data.formMode, null, params)
			}
			break
		case 'route':
			navigateToRouteName(vueInstance, data.routeName, data.routeValues)
			break
		case 'report':
			navigateToReport(vueInstance, data.controller, data.reportAction, data.routeValues, data.preview)
			break
		default:
			/* log error */
			vueInstance.$eventTracker.addError({ origin: 'processRedirect', message: 'Error found while redirecting!', contextData: { type: data.type } })
	}
}

export function linkToRouteName(vueInstance, routeName, options, query, prefillValues)
{
	const systemDataStore = useSystemDataStore()
	const culture = systemDataStore.system.currentLang
	const system = systemDataStore.system.currentSystem
	const module = systemDataStore.system.currentModule

	const historyBranchId = 'navigationId' in vueInstance ? vueInstance.navigationId : 'main'

	const params = _assignIn({ culture, system, module }, {
		historyBranchId
	}, options, { prefillValues: JSON.stringify(prefillValues) })

	return vueInstance.$router.resolve({ name: routeName, params, query })?.href
}

export const vueNavigation = {
	navigateToRouteName,
	navigateToForm,
	navigateToModule,
	navigateToReport,
	navigateToReportingServicesViewer,
	processRedirect,
	linkToRouteName
}
