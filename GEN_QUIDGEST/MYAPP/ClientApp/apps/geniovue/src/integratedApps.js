
/**
 * All external apps.
 */
const allExternalApps = [
]

const allParentRoutes = [
]

export function setExternalAppsPlugin(appContext, router)
{
	for (const app of allExternalApps)
	{
		if (app.hasInternalRouter)
		{
			const params = app.parameters
			params['router'] = router
			params['parent'] = allParentRoutes
			appContext.use(app.appPackage, params)
		}
		else
			appContext.use(app.appPackage, app.parameters)
	}
}
