<template>
	<q-layout
		v-if="mainAppLoadMonitor.loaded"
		v-show="isReady"
		:custom-classes="layoutCustomClasses"
		:loading="isLoading"
		:loading-menus="!isPublicModule && !menuLoadMonitor.loaded">
		<template #layout-loading-effect>
			<q-page-busy-state
				:processes="busyPageStateStack"
				:resources-path="$app.resourcesPath" />

			<template v-if="progressBar.isVisible">
				<teleport
					defer
					:to="`#${progressBar.containerId}-body`">
					<q-progress
						v-bind="progressBar.props"
						v-on="progressBar.handlers" />
				</teleport>

				<teleport
					v-if="progressBar.modalProps.buttons?.length > 0"
					defer
					:to="`#${progressBar.containerId}-footer`">
					<template
						v-for="btn in progressBar.modalProps.buttons"
						:key="btn.id">
						<q-button
							:id="btn.id"
							:label="btn.text"
							:variant="btn.variant"
							:disabled="btn.disabled"
							:icon-pos="btn.iconPos"
							:class="btn.classes"
							@click="btn.action">
							<q-icon
								v-if="btn.icon"
								v-bind="btn.icon" />
						</q-button>
					</template>
				</teleport>
			</template>
		</template>

		<template #layout-header>
			<q-sidebar
				@changed-sidebar-width="setSidebarWidth"
				@focus-control="(...args) => $eventHub.emit('focus-control', ...args)" />
		</template>

		<template #layout-content>
			<q-info-message-container
				v-if="fixedInfoMessages.length > 0"
				pinned>
				<q-info-message
					v-for="infoMessage in fixedInfoMessages"
					:key="infoMessage.message"
					v-bind="infoMessage"
					:text="infoMessage.isResource ? Resources[infoMessage.message] : infoMessage.message"
					:dismiss-time="infoMessage.dismissTime ?? 5"
					@message-dismissed="removeInfoMessage" />
			</q-info-message-container>

			<div class="content-wrapper">
				<template v-if="isPublicRoute && isFullScreenPage">
					<router-view
						:key="mainRouteKey"
						:route="displayRoute" />
				</template>
				<template v-else-if="userIsLoggedIn || (isPublicRoute && !isFullScreenPage) || $app.layout.LoginStyle !== 'single_page'">
					<div
						v-if="showContent"
						id="main"
						:class="mainClasses">
						<q-breadcrumbs
							:is-visible="breadcrumbsIsVisible"
							:navigation-id="navigationId" />

						<div
							v-show="mainContainerIsVisible"
							id="form-container"
							:class="[{ reportmodeon: reportingModeCAV }, containerClasses, 'content']">
							<teleport
								v-if="showInfoMessageArea"
								to=".q-modal-info-messages"
								:disabled="!showInfoMessagesInPopup">
								<q-info-message-container>
									<q-info-message
										v-for="infoMessage in relativeInfoMessages"
										:key="infoMessage.message"
										v-bind="infoMessage"
										:text="infoMessage.isResource ? Resources[infoMessage.message] : infoMessage.message"
										:dismiss-time="infoMessage.dismissTime ?? 5"
										@message-dismissed="removeInfoMessage" />
								</q-info-message-container>
							</teleport>

							<template v-for="view in routerViews">
								<router-view
									v-if="view.isVisible"
									:key="view.key"
									:route="view.route" />
							</template>
						</div>

						<q-cav-container v-if="reportingModeCAV" />
					</div>
				</template>
				<template v-else>
					<q-log-on />
				</template>
			</div>

			<q-cookies
				v-if="cookieBanner.isVisible"
				v-bind="cookieBanner.props"
				@set-cookie="handleSetCookie" />

			<q-footer v-if="$app.layout.FooterEnable && (showContent || isPublicRoute)" />
		</template>
	</q-layout>

	<q-dialog
		v-for="modal in modals"
		:key="modal.id"
		:model-value="modal.isActive"
		:id="`q-modal-${modal.id}`"
		v-bind="modal.props"
		@update:model-value="(val) => onModalUpdateModelValue(modal, val)">
		<template #[`header.append`]>
			<div :id="`q-modal-${modal.id}-header`" />
		</template>

		<template #body>
			<div
				v-show="modal.id === latestModalId"
				class="q-modal-info-messages" />
			<div :id="`q-modal-${modal.id}-body`" />
		</template>

		<template #[`footer.append`]>
			<div :id="`q-modal-${modal.id}-footer`" />
		</template>
	</q-dialog>

	<q-dialog-provider />

	<q-suggestions
		v-if="!isEmpty(suggestionsPopupData.component)"
		:key="suggestionsKey"
		v-bind="suggestionsPopupData" />

	<q-debug-window
		v-if="showDebugWindow"
		@close="closeDebugWindow" />
</template>

<script>
	import 'bootstrap'

	import { mapActions, mapState } from 'pinia'
	import { v4 as uuidv4 } from 'uuid'
	import { defineAsyncComponent, shallowRef } from 'vue'
	import { isNavigationFailure } from 'vue-router'

	import { loadResources } from '@/plugins/i18n'
	import asyncProcM from '@quidgest/clientapp/composables/async'
	import netAPI from '@quidgest/clientapp/network'

	import { useGenericDataStore, useTracingDataStore } from '@quidgest/clientapp/stores'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import AuthHandlers from '@/mixins/authHandlers.js'
	import CavHandler from '@/mixins/cavHandler.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import NavHandlers from '@/mixins/navHandlers.js'
	import { navigateToRouteName, processRedirect as vueProcessSrvRedirect } from '@/mixins/vueNavigation.js'
	import { removeModal, setShowCookies } from '@/utils/layout'
	import { updateAFToken, updateMainConfig } from '@/utils/system'
	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

	import QCookies from '@/components/inputs/QCookies.vue'
	import QLayout from '@/views/layout/Layout.vue'
	import QBreadcrumbs from '@/views/shared/Breadcrumbs.vue'
	import QInfoMessageContainer from '@/views/shared/QInfoMessageContainer.vue'
	import QSidebar from '@/views/shared/RightSidebar.vue'

	export default {
		name: 'QApp',

		components: {
			QCavContainer: defineAsyncComponent(() => import('@/views/shared/cav/CavContainer.vue')),
			QFooter: defineAsyncComponent(() => import('@/views/shared/Footer.vue')),
			QLogOn: defineAsyncComponent(() => import('@/views/shared/LogOn.vue')),
			QSuggestions: defineAsyncComponent(() => import('@/views/shared/Suggestions.vue')),
			QDebugWindow: defineAsyncComponent(() => import('@/views/shared/DebugWindow.vue')),
			QInfoMessageContainer,
			QLayout,
			QBreadcrumbs,
			QSidebar,
			QCookies
		},

		mixins: [
			NavHandlers,
			LayoutHandlers,
			AuthHandlers,
			CavHandler
		],

		expose: [
			'navigationId'
		],

		data()
		{
			return {
				mainAppLoadMonitor: asyncProcM.getProcListMonitor('QMainApp', false),

				menuLoadMonitor: asyncProcM.getProcListMonitor('QMenus', false),

				// The current main route (if there are active modals, this will be the route of the content in the background).
				displayRoute: null,

				// Whether or not there's at least one visible modal.
				hasPopup: false,

				// Whether or not it's a dashboard page.
				isDashboardMenu: false,

				// Whether or not the info messages should be displayed in a modal.
				showInfoMessagesInPopup: false,

				// Whether or not the application content is ready to be displayed.
				isReady: false,

				// Whether or not the application content should be visible.
				showContent: true,

				// The width of the right sidebar.
				sidebarWidth: 0,

				// Data used for the suggestions popup, contains what popup component should render and it's params ex: id, label.
				suggestionsPopupData: {
					component: '',
					params: {}
				},

				// Used to re-render the suggestions component when the props are changed.
				suggestionsKey: 0,

				// Whether or not the debug window is currently visible.
				showDebugWindow: false
			}
		},

		created()
		{
			this.mainAppLoadMonitor.add(this.loadUIResources(), true)
			this.$eventHub.on('set-culture', this.loadUIResources)

			// Checks whether or not a user is currently logged.
			this.getIfUserLogged(false)

			this.$eventHub.on('check-user-is-logged-in', this.checkUserLoggedIn)

			this.$eventHub.on('response-redirect-to', this.processSrvRedirect)

			this.$eventHub.on('go-to-route', this.goToRoute)

			this.$eventHub.on('open-external-app', () => this.showContent = false)

			this.$eventHub.on('closed-external-app', () => this.showContent = true)

			this.$eventHub.on('navigation-id-change', this.updateNavigationId)

			// Listens for requests of full quality images.
			this.$eventHub.on('image-request', this.onImageRequest)

			this.$eventHub.on('show-suggestion-popup', this.onShowSuggestionPopup)

			if (this.isEventTracingActive)
				document.addEventListener('keydown', this.handleKeyPress)
		},

		mounted()
		{
			this.getMenus()
			this.checkCookiesState()
		},

		beforeUnmount()
		{
			this.$eventHub.off('set-culture', this.loadUIResources)
			this.$eventHub.off('check-user-is-logged-in', this.checkUserLoggedIn)
			this.$eventHub.off('response-redirect-to', this.processSrvRedirect)
			this.$eventHub.off('go-to-route', this.goToRoute)
			this.$eventHub.off('open-external-app')
			this.$eventHub.off('closed-external-app')
			this.$eventHub.off('navigation-id-change', this.updateNavigationId)
			this.$eventHub.off('image-request', this.onImageRequest)
			this.$eventHub.off('show-suggestion-popup', this.onShowSuggestionPopup)

			this.mainAppLoadMonitor.destroy()
			this.menuLoadMonitor.destroy()

			document.removeEventListener('keydown', this.handleKeyPress)
		},

		computed: {
			...mapState(useGenericDataStore, [
				'latestModalId',
				'isLoading',
				'fixedInfoMessages',
				'relativeInfoMessages',
				'modals',
				'busyPageStateStack',
				'shouldShowCookies'
			]),

			...mapState(useTracingDataStore, [
				'isEventTracingActive'
			]),

			showInfoMessageArea()
			{
				return this.relativeInfoMessages.length > 0 && (!this.hasPopup || this.latestModalId === this.$route.name)
			},

			cookieBanner()
			{
				const cookies = this.$app.cookies

				return {
					isVisible: cookies.cookieActive && this.shouldShowCookies,
					props: {
						filePath: cookies.filePath,
						text: this.Resources[cookies.cookieText],
						buttonText: this.Resources[hardcodedTexts.imAware]
					}
				}
			},

			breadcrumbsIsVisible()
			{
				return !this.isEmpty(this.displayRoute) ? this.displayRoute.meta.noBreadcrumbs !== true : true
			},

			mainClasses()
			{
				const classes = []

				if (this.displayRoute)
					classes.push(`${this.system.currentModule}-${this.displayRoute.name}`)

				return classes
			},

			mainRouteKey()
			{
				return !this.isEmpty(this.displayRoute) ? this.displayRoute.meta.routeKey : uuidv4()
			},

			routerViews()
			{
				const routerViews = []

				// The main route, which is always visible, either in foreground or in the background.
				routerViews.push({
					isVisible: true,
					key: this.mainRouteKey,
					route: this.displayRoute
				})

				// The popups route, will only be visible if a popup form or menu is currently open.
				routerViews.push({
					isVisible: this.hasPopup,
					key: uuidv4()
				})

				return routerViews
			},

			layoutCustomClasses()
			{
				return this.isDashboardMenu ? ['layout-dashboard'] : []
			},

			isPublicModule()
			{
				return this.system.currentModule === 'Public'
			}
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'removeInfoMessage',
				'clearInfoMessages',
				'setMenus',
				'setModal',
				'setPublicRoute',
				'setFullScreenPage'
			]),

			onShowSuggestionPopup(component, params)
			{
				this.suggestionsPopupData.component = component
				this.suggestionsPopupData.params = params
				this.suggestionsKey++
			},

			onImageRequest({ baseArea, params, callback }) {
				netAPI.retrieveImage(
					baseArea,
					params,
					(data) => {
						if (callback)
							callback(data)
					})
			},

			/**
			 * Changes the state of the cookies.
			 * @param {boolean} isVisible Value to change
			 */
			handleSetCookie(isVisible)
			{
				localStorage.setItem('cookieAccepted', !isVisible)
				setShowCookies(!isVisible)
			},

			/**
			 * Gets the value of the cookie in the localStorage.
			 */
			checkCookiesState()
			{
				const isAccepted = localStorage.getItem('cookieAccepted') ? localStorage.cookieAccepted : false
				setShowCookies(!isAccepted)
			},

			/**
			 * Checks whether or not a user is currently logged.
			 * @param {boolean} checkIfSame Check if it's the same user to avoid unnecessary reassignment and updates
			 */
			getIfUserLogged(checkIfSame)
			{
				return netAPI.fetchData(
					'Account',
					'GetIfUserLogged',
					{},
					(data) => {
						if (checkIfSame && data?.username === this.userData.name)
							return

						this.setUser(data)
					})
			},

			/**
			 * Checks whether or not a user is currently logged.
			 */
			checkUserLoggedIn()
			{
				this.getIfUserLogged(true)
			},

			/**
			 * Update the App navigation ID and the auxiliary components of the interface whenever a different navigation context is opened by the route.
			 * @param {object} eventData The event data that contains the new navigation Id
			 */
			updateNavigationId(eventData)
			{
				if (this.navigationId !== eventData.navigationId)
					this.navigationId = eventData.navigationId
			},

			/**
			 * Loads all the necessary texts.
			 */
			loadUIResources()
			{
				loadResources(this, ['hardcoded', 'messages', 'projectArrays', 'TreeMenu'])
			},

			/**
			 * Called after checking if a user is already logged, to set his data.
			 * @param {object} data The data of the user
			 */
			setUser(data)
			{
				const userData = {
					Name: data.username
				}

				const userIsSame = data?.username === this.userData.name

				this.setUserData(userData)
				this.isReady = true

				if (!userIsSame)
				{
					updateAFToken()
					updateMainConfig()
				}
			},

			/**
			 * Closes the specified modal.
			 * @param {object} modal The modal to close
			 */
			closeModal(modal)
			{
				let dismiss = true

				if (typeof modal.dismissAction === 'function')
					dismiss = modal.dismissAction()

				// If the modal is not from a route, remove it
				// If the modal is from a route, it will be removed when the route changes
				if (!modal.hasRoute && dismiss !== false)
					removeModal(modal.id)
			},

			/**
			 * Emits a global event to warn other components that the specified modal is now ready.
			 * @param {object} modal The modal to close
			 */
			warnModalIsReady(modal)
			{
				this.$eventHub.emit('modal-is-ready', modal.id)
			},

			/**
			 * Called when the modal's model value is updated.
			 * @param {object} modal The modal to close
			 * @param {object} modelValue The model value for whether the modal is visible
			 */
			onModalUpdateModelValue(modal, modelValue)
			{
				if (modelValue)
					this.warnModalIsReady(modal)
				else
					this.closeModal(modal)
			},

			/**
			 * Processes a redirect request.
			 * @param {object} data The redirect data
			 */
			processSrvRedirect(data)
			{
				vueProcessSrvRedirect(this, data)
			},

			/**
			 * Navigates to the specified route.
			 * @param {object} routeParams The properties of the route
			 * @param {function} successCallback A function to be called in case the route change succeeds
			 * @param {function} failCallback A function to be called in case the route change fails
			 */
			async goToRoute(routeParams = {}, successCallback, failCallback)
			{
				if (typeof successCallback !== 'function')
					successCallback = () => {}
				if (typeof failCallback !== 'function')
					failCallback = () => {}

				const { name, params, query, prefillValues } = routeParams

				if (this.isEmpty(name))
				{
					failCallback()
					return
				}

				const navResult = await navigateToRouteName(this, name, params ?? {}, query ?? {}, prefillValues ?? {})

				if (isNavigationFailure(navResult))
					failCallback(navResult)
				else
					successCallback(navResult)
			},

			/**
			 * Sets the width of the right sidebar.
			 * @param {number} sidebarWidth The width of the sidebar (in rem)
			 */
			setSidebarWidth(sidebarWidth)
			{
				this.sidebarWidth = sidebarWidth
				document.documentElement.style.setProperty('--right-sidebar-width', sidebarWidth)
			},

			/**
			 * Fetches, from the server, the menus of the current module.
			 */
			getMenus()
			{
				if (this.isPublicModule)
					return

				this.menuLoadMonitor.add(
					netAPI.fetchData(
						'Home',
						'NavigationalBar',
						{},
						(data) => {
							if (this.system.currentModule === data.Module)
								this.setMenus(data)
						}),
					true)
			},

			/**
			 * Opens the debug window.
			 */
			openDebugWindow()
			{
				if (this.isEventTracingActive)
					this.showDebugWindow = true
			},

			/**
			 * Closes the debug window.
			 */
			closeDebugWindow()
			{
				this.showDebugWindow = false
			},

			/**
			 * Handler that will be executed when the key press event is fired.
			 * @param {object} event The key press event
			 */
			handleKeyPress(event)
			{
				// Open Debug dialog.
				if (this.isEventTracingActive
					&& !this.showDebugWindow
					&& (event.altKey && event.shiftKey && event.ctrlKey))
					this.openDebugWindow()
			}
		},

		watch: {
			$route(to, from)
			{
				if (!this.showContent)
					this.$eventHub.emit('close-external-app')

				if (to.name !== from.name || !this.showContent)
				{
					if (to.meta.isPopup)
					{
						this.showInfoMessagesInPopup = true

						// In case the user opens a direct url to a popup form in a new tab, sets the home page as the background.
						if (typeof from.name === 'undefined')
						{
							to.params.noModal = true

							const homeModule = to.params.module || this.system.currentModule
							const mainRoute = this.$router.resolve({
								name: `home-${homeModule}`,
								params: {
									culture: to.params.culture,
									system: to.params.system,
									module: homeModule,
									noModal: true,
									/**
									 * In the case of direct navigation via URL, since the PHE form is a popup, the Home page is opened to provide a background.
									 * In this scenario, we must not lose the 'noRedirect' parameter to avoid entering a loop.
									 */
									noRedirect: to.params.noRedirect
								}
							})

							this.$router.replace(mainRoute).then(() => {
								/* The «routeKey» is used to cause a change in the route to force the component to re-render,
									in addition to being used in the v-key. Without causing any changes,
									when going from the pop-up to a normal form that is already open in the background,
									it does not execute 'beforeCreate' and thus does not create the history level of that form. */
								this.displayRoute = {
									...mainRoute,
									meta: {
										...mainRoute.meta,
										routeKey: uuidv4()
									}
								}
								this.$router.push(to)
							})
						}
						else
						{
							// If it is already used as a background, there is no need to update it.
							if (from.meta.isPopup !== true && this.displayRoute?.name !== from.name)
								this.displayRoute = shallowRef(from)
							this.hasPopup = true
						}
					}
					else
					{
						if (to.params.isNested !== 'true')
						{
							this.hasPopup = false
							/* The «routeKey» is used to cause a change in the route to force the component to re-render,
								in addition to being used in the v-key. Without causing any changes,
								when going from the pop-up to a normal form that is already open in the background,
								it does not execute 'beforeCreate' and thus does not create the history level of that form. */
							this.displayRoute = shallowRef({
								...to,
								meta: {
									...to.meta,
									routeKey: uuidv4()
								}
							})
						}
						else
							this.hasPopup = true

						this.showInfoMessagesInPopup = false
						this.isDashboardMenu = to.meta.isDashboardPage === true
					}

					if (to.params.keepAlerts !== 'true')
						this.clearInfoMessages()

					// Sets the current menu path.
					this.$nextTick().then(() => {
						let order

						// When the form doesn't have a base area, the 'to.meta.order' doesn't exist, so we use the order that comes from 'openedMenu'.
						if (to.meta.routeType !== 'home')
							order = to.meta.order ?? to.params.openedMenu

						if (order)
						{
							this.setMenuPath(order)
							const module = to.meta.module
							const menuId = module + order.slice(0, -1)

							if (menuId)
								setTimeout(() => genericFunctions.scrollTo(menuId), 300)
						}
					})
				}
			},

			'system.currentModule'()
			{
				this.getMenus()
			},

			userIsLoggedIn()
			{
				this.getMenus()
			},

			displayRoute(newValue)
			{
				const routeMeta = ((newValue || {}).meta || {}),
					// True if the current display route is a public page (form/menu), false otherwise.
					isPublicRoute = routeMeta.isPublicPage === true,
					// True if the current display route should be shown in full screen, false otherwise.
					isFullScreenPage = routeMeta.isFullScreenPage === true

				this.setPublicRoute(isPublicRoute)
				this.setFullScreenPage(isFullScreenPage)
			},

			mainContainerIsVisible: {
				handler(val)
				{
					const cavWidthPercentage = val ? 50 : 100
					document.documentElement.style.setProperty('--cav-width-percentage', cavWidthPercentage)
				},
				immediate: true
			}
		}
	}
</script>
