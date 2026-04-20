import { toValue } from 'vue'
import { useSystemDataStore } from '@quidgest/clientapp/stores'
import { useTracingDataStore } from '@quidgest/clientapp/stores'
import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

function getMenu(menuEntry)
{
	if (!menuEntry.Action && menuEntry.Children.length === 1)
		return menuEntry.Children[0]
	return menuEntry
}

function getReport(vueContext, controller, action, params, mode, preview)
{
	if (mode === 'reporting-services-viewer') // SSRS Report Viewer
		vueContext.navigateToReportingServicesViewer(controller, action, params)
	else
		vueContext.navigateToReport(controller, action, params, preview)
}

function getContent(page)
{
	const eventTracker = useTracingDataStore()
	eventTracker.addTrace({ origin: 'getContent (menuAction)', message: 'Content: ' + page + ' returned!' })
}

function executeRoutine(vueContext, routineName, params)
{
	import('@/api/genio/menuRoutines.js')
		.then((routinesModule) => {
			if (Reflect.has(routinesModule, routineName))
				routinesModule[routineName].execute(vueContext, params)
		})
		.catch((error) => {
			const eventTracker = useTracingDataStore()
			eventTracker.addError({ origin: 'executeRoutine (menuAction)', message: `Error on execute routine ${routineName}`, contextData: { error } })
		})
}

export default {
	data()
	{
		const systemDataStore = useSystemDataStore()

		return {
			currentModule: systemDataStore.system.currentModule
		}
	},

	methods: {
		/**
		 * Executes the action of the specified menu entry.
		 * @param {object} _menuEntry An object representing the menu entry
		 */
		executeMenuAction(_menuEntry)
		{
			const menuEntry = toValue(getMenu(_menuEntry))

			// Trigger before executing event
			this.$eventHub.emit('before-execute-menu-action', { module: this.currentModule, id: menuEntry.Id, menu: menuEntry })

			// TODO: Encapsulate this logic in the Generator (GenGenio.*).
			if (menuEntry.Action && menuEntry.Action === 'GenGenio.MenuRotinaManual')
				executeRoutine(this, menuEntry.ActionMVC, {}) // controller ??
			else if ((menuEntry.Action && menuEntry.Action === 'GenGenio.MenuFormNovoRegisto') || (menuEntry.IsForm && menuEntry.Mode === 'NEW'))
				this.navigateToRouteName(menuEntry.RouteName, { mode: 'NEW', id: null, openedMenu: menuEntry.Order, clearHistory: true })

			else if (menuEntry.Action && menuEntry.Action === 'GenGenio.MenuPaginaWeb')
			{
				const webPage = menuEntry.WebPage

				if (webPage.includes('http') || webPage.includes('www.'))
					window.open(webPage, '_blank')
				else
					getContent(webPage)
			}
			else if (menuEntry.Action && menuEntry.Action === 'GenGenio.MenuFormVazio')
			{
				const lastIndex = menuEntry.ActionMVC.lastIndexOf('_'),
					formName = (menuEntry.ActionMVC.substr(0, lastIndex) || '').toUpperCase()

				this.navigateToForm(formName, menuEntry.Mode, null, { modes: genericFunctions.getDefaultFormModesForMode(menuEntry.Mode), openedMenu: menuEntry.Order, clearHistory: true })
			}
			else if (menuEntry.Type === 'REPORT' && !menuEntry.RouteName.startsWith('menu-'))
				// TODO: If we are only going to have SSRS, we can change the «Mode» property to have the opening type instead of the report type (SSRS / Crystal)
				// TODO: It's need use the empty history
				getReport(this, menuEntry.Controller, menuEntry.ActionMVC, {}, menuEntry.RouteName, menuEntry.Preview)
			else if (menuEntry.RouteName)
			{
				if (menuEntry.RouteName === this.$route.name)
					return

				this.navigateToRouteName(menuEntry.RouteName, menuEntry.IsForm ? { mode: menuEntry.Mode, modes: genericFunctions.getDefaultFormModesForMode(menuEntry.Mode), openedMenu: menuEntry.Order, clearHistory: true } : { openedMenu: menuEntry.Order, clearHistory: true })
			}
		},

		/**
		 * Used in double_navbar menu.
		 * When clicking on the first level, if the menu has a default action (a menu to open, ex), it will be executed.
		 * @param {string} id The id of the menu
		 */
		navigateToDefaultAction(id)
		{
			const routeName = `menu-${id}`
			const route = this.$router.resolve({ name: routeName })

			this.navigateToRouteName(routeName, { openedMenu: route?.meta.order, clearHistory: true })
		},

		/**
		 * Method that retrieves the URL of the requested navigation. This method is used for right-click based navigation,
		 * such as "open in a new tab", since these require specification of the link to navigate to.
		 * @param {object} _menuEntry - An object representing the menu entry
		 * @returns {string} The URL of the navigation
		 */
		getLinkToMenu(_menuEntry)
		{
			const menuEntry = getMenu(_menuEntry)
			let href

			// TODO: Encapsulate this logic in the Generator (GenGenio.*).
			if (menuEntry.Action && menuEntry.Action === 'GenGenio.MenuFormNovoRegisto')
				href = this.linkToRouteName(menuEntry.RouteName, { mode: 'NEW' })
			if (menuEntry.Action && menuEntry.Action === 'GenGenio.MenuFormVazio')
			{
				const lastIndex = menuEntry.ActionMVC.lastIndexOf('_'),
					formName = (menuEntry.ActionMVC.substr(0, lastIndex) || '').toUpperCase(),
					formRouteName = `form-${formName}`

				href = this.linkToRouteName(formRouteName, { mode: menuEntry.Mode })
			}
			if (menuEntry.RouteName)
				href = this.linkToRouteName(menuEntry.RouteName, menuEntry.IsForm ? { mode: menuEntry.Mode } : {})

			return href ?? '#'
		}
	}
}
