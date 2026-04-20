import { createRouter, createWebHashHistory } from 'vue-router'

import { fetchData, postData } from '@quidgest/clientapp/network'
import {
	useGenericDataStore,
	useNavDataStore,
	useSystemDataStore,
	useUserDataStore
} from '@quidgest/clientapp/stores'

import { setI18nLanguage } from '@/plugins/i18n'
import { systemInfo } from '@/systemInfo'

import getFormsRoutes from './forms.js'
import getMainRoutes from './main.js'
import getMenusRoutes from './menus.js'
import getUserRoutes from './user.js'

export function setupRouter(i18n)
{
	const systemDataStore = useSystemDataStore()
	const genericDataStore = useGenericDataStore()
	const userDataStore = useUserDataStore()
	const navDataStore = useNavDataStore()

	const routes = [
		...getMainRoutes(),
		...getUserRoutes(),
		...getMenusRoutes(),
		...getFormsRoutes()
	]

	const router = createRouter({
		history: createWebHashHistory(),
		routes
	})

	router.beforeEach(async (to, from, next) => {
		// If the route has a fixed module, we make sure that module is the one in the params.
		if (to.meta.module)
			to.params.module = to.meta.module

		const locale = to.params.culture
		const system = to.params.system
		const module = to.params.module

		/**
		 * Stores an attempted access to a route for a form or menu.
		 * This way, if the user tries to access a route that requires authentication, it will be remembered,
		 * and after logging in, they will be redirected to that route.
		 * However, if another page is opened instead of logging in, the stored route information will be cleared in this case.
		 */
		const routeType = to.meta.routeType
		const routeData = {
			name: to.name,
			params: to.params
		}

		if (!userDataStore.userIsLoggedIn)
		{
			const systemIsAvailable = system && systemDataStore.system.availableSystems.includes(system)
			const moduleIsAvailable = module && (module === 'Public' || systemDataStore.system.availableModules[module])

			// If not allowed, save the current route data
			if ((!systemIsAvailable || !moduleIsAvailable) && (routeType === 'menu' || routeType === 'form'))
				userDataStore.setRedirectRouteAfterLogin(routeData)
			// For any additional redirection on LogOn, add to the route exception list
			else
				// Remove previously saved
				userDataStore.setRedirectRouteAfterLogin()
		}

		// Check locale.
		if (locale && !systemInfo.locale.availableLocales.find(lang => lang.language === locale))
		{
			to.params.culture = systemDataStore.system.currentLang;		
		}

		// Check system.
		if (system && !systemDataStore.system.availableSystems.includes(system))
		{
			next({ name: 'systemNotFound' })
			return
		}

		/*
		 * The system (year) must be assigned before executing 'created' because, for example,
		 * the forms when executing 'loadFormData' in created need the system to already be correct,
		 * otherwise the permissions will be validated in the wrong system and the data will also be read from the wrong database.
		 *
		 * Internally the setCurrentSystem is protected from the assignment of an empty value or one that does not exist in the available systems.
		 */
		systemDataStore.setCurrentSystem(system)

		// Check module.
		if (module && module !== 'Public' && !systemDataStore.system.availableModules[module])
		{
			next(false)
			return
		}

		// Check if it's a Home Page and assign the mandatory value of the parameters.
		if (to.meta.isHomePage)
		{
			to.params.isHomePage = 'true'
			to.params.mode = 'SHOW'
		}

		// Sets up a modal to display the route content if the route is a popup.
		if (to.name !== from.name && !to.params.noModal)
		{
			await genericDataStore.clearModals()

			if (to.meta.isPopup)
			{
				// Focus wrap will be activated after the form has loaded
				// because it needs to have focusable elements to work.
				const props = {
					class: 'q-dialog-form',
					dismissible: false,
					focusWrap: false,
					returnFocusOnDeactivate: false,
					size: 'medium'
				}

				const modalProps = {
					id: to.name,
					isActive: true,
					hasRoute: true
				}

				await genericDataStore.setModal(props, modalProps)
			}
		}
		else
			delete to.params.noModal // We only want to skip clearing the modals the first time.

		if (to.redirectedFrom && to.redirectedFrom.name)
		{
			if (to.redirectedFrom.name === 'OpenId')
			{
				if (to.params.code !== '' && to.params.id_token !== '')
					postData('Account', 'OpenIdLogin', { code: to.params.code, id_token: to.params.id_token })
			}
			else if (to.redirectedFrom.name === 'CMDLog')
			{
				const queryParams = new URLSearchParams(to.hash.substring(1))
				if (queryParams.get('access_token') !== null)
					fetchData('Account', 'CMDLogin', { access_token: queryParams.get('access_token') })
			}
			else if (to.redirectedFrom.name === 'CASLog')
			{
				if (to.params.SAMLart !== '')
					fetchData('Account', 'CASLogin', { SAMLart: to.params.SAMLart })
			}
		}

		// FOR: PREFILL_FORM_VALUES
		if (to.params.prefillValues)
		{
			try
			{
				to.params.prefillValues = JSON.parse(to.params.prefillValues)
			}
			catch
			{
				to.params.prefillValues = {}
			}
		}

		// If the user is trying to navigate to a module with an initial PHE and still hasn't chosen anything,
		// he's redirected to choose which data should be displayed.
		if (to.meta && to.meta.hasInitialPHE && to.params.noRedirect !== 'true' && typeof userDataStore.valuesOfPHEs[module] === 'undefined')
		{
			await postData('Home', 'GetEphFormAction', { EphModule: module }, (data, request) => {
				if (!request.data.Success)
					return

				const routeName = data.routeName
				// List of routes that are allowed as main or child menus
				const allowedRoutes = data.allowedRoutes ?? []
				// In the case where there is more than one menu in sequence, when navigating from one to another, we do not want to go back
				const navigationBetweenPHEMenus = (allowedRoutes.includes(to.name) && allowedRoutes.includes(from.name))

				if (routeName && routeName.length > 0)
				{
					const params = {
						...to.params,
						isPopup: true,
						noRedirect: true
					}

					// TODO: When there are no records in the second list, Back does not let it go out, in case the first one has 'Skip if just one'
					if (!navigationBetweenPHEMenus)
						next({ name: routeName, params })
					else
						next()
				}
				else
				{
					const emptyPHE = {
						key: module,
						value: ['']
					}

					// In case it's a user without PHE, sets the value as an empty string.
					userDataStore.addPHEChoice(emptyPHE)
					next()
				}
			})
		}
		else if (to.meta && to.meta.isWizard) // If it's a wizard, redirects to the correct step.
		{
			const action = `${to.meta.wizardId}_GetPath`
			const args = { formId: to.params.id || '' }

			await fetchData(to.meta.baseArea, action, args, (data, request) => {
				if (!request.data.Success || !Array.isArray(data.Path))
					return

				const wizardPath = data.Path
				if (wizardPath.length === 0)
					wizardPath.push(data.NextStep)

				const wizardMode = to.params.mode

				next({ name: data.NextStep, params: { ...to.params, wizardPath, wizardMode } })
			})
		}
		else if (to.meta && to.meta.isWizardStep)
		{
			// If no path is provided, redirects to the parent route.
			if (!Array.isArray(to.params.wizardPath))
				next({ name: to.meta.parentRoute, params: to.params })
			else
				next()
		}
		else
			next()
	})

	router.afterEach((to, from, failure) => {
		// If the navigation fails, we don't want the code in this hook to execute.
		if (failure)
			return

		// If the route has a fixed module, we make sure that module is the one in the params.
		if (to.meta.module)
			to.params.module = to.meta.module

		const locale = to.params.culture
		const system = to.params.system
		const module = to.params.module

		if (module)
			systemDataStore.setCurrentModule(module)
		if (system)
			systemDataStore.setCurrentSystem(system)
		if (locale)
			systemDataStore.setCurrentLang(locale)

		// Load locale messages.
		setI18nLanguage(i18n, systemDataStore.system.currentLang)

		// Navigation to some menu clears the navigation and current menu path.
		// It cannot be in the 'beforeCreate' of 'navHandlers' because if navigation from an M->F+ to itself is done, the code will not be executed.
		const isNewNavigation = to.params.clearHistory === 'true'

		// Delete so history isn't cleared on subsequent route changes
		delete to.params.clearHistory

		if (isNewNavigation || to.meta.isHomePage || module !== from.params.module)
		{
			if (isNewNavigation || (to.params.keepNavigation !== 'true' && !from.meta.keepNavigation && !to.meta.keepNavigation))
				navDataStore.clearHistory()

			genericDataStore.resetMenuPath()
		}
	})

	return router
}
