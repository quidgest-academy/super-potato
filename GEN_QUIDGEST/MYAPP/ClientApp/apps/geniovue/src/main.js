
import { createPinia } from 'pinia'
import { createApp } from 'vue'

import { simpleFetch } from '@quidgest/clientapp/network'
import eventBus from '@quidgest/clientapp/plugins/eventBus'

import App from './App.vue'
import components from './components'
import formComponents from './components/formComponents.js'
import gridTableListFormComponents from './components/gridTableListFormComponents.js'
import { setExternalAppsPlugin } from './integratedApps.js'
import eventTracker from './plugins/eventTracker.js'
import { resourcesMixin, setupI18n } from './plugins/i18n.js'
import clientappFramework from './plugins/quidgest-clientapp'
import uiFramework from './plugins/quidgest-ui'
import { setupRouter } from './router'
import { systemInfo } from './systemInfo'
import { setAppConfig } from './utils/system'

// Global CSS
import './assets/styles/quidgest.scss'

const pinia = createPinia()
const app = createApp(App)

app.use(uiFramework)
app.use(pinia)
app.use(eventTracker)
app.use(clientappFramework)

app.config.globalProperties.$app = systemInfo

const i18n = setupI18n()
app.config.globalProperties.$__i18n = i18n

// Init router instance
const router = setupRouter(i18n)

// Parse URL to get the selected System (year)
let currentSystem = undefined
const hash = window.location.hash.slice(1)
if (hash.length > 0)
{
	const currentRoute = router.resolve(hash)
	currentSystem = currentRoute.params.system
}

async function delay(ms)
{
	return new Promise((resolve) => setTimeout(resolve, ms))
}

async function retryWithDelay(maxRetries, timeout, fn)
{
	try
	{
		return await fn()
	}
	catch (error)
	{
		if (maxRetries <= 0)
			throw error

		await delay(timeout)
		return retryWithDelay(maxRetries - 1, timeout, fn)
	}
}

// Get config from backend
retryWithDelay(5, 1000, () => simpleFetch('Config', 'GetConfig', currentSystem))
	.then((response) => {
		if (!response.data.Success)
		{
			// eslint-disable-next-line no-console
			console.error('ERROR: Unable to start the application!')
			return
		}

		setAppConfig(response.data.Data)
		app.use(i18n).use(router)

		// Init global components
		app.use(components)
		app.use(formComponents)
		app.use(gridTableListFormComponents)

		// Init external apps
		setExternalAppsPlugin(app, router)

		// Create the Global event bus
		// To communicate between components in different levels
		app.config.globalProperties.$eventHub = eventBus

		// Global mixin applied to every vue instance
		app.mixin(resourcesMixin)
		app.mount('#app')
	}).catch((error) => {
		// eslint-disable-next-line no-console
		console.error('GetConfig error:', error)
	})
