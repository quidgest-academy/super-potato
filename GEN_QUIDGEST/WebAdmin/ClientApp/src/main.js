import { createApp } from 'vue'
import 'bootstrap'
import QGlobal from './global'
import App from './App.vue'
import { setupRouter } from './router'
import store from './store'
import { setupI18n } from './plugins/i18n'
import ComponentsInit from './mixins/components'
import EventBus from './utils/eventBus'
import framework from './plugins/quidgest-ui'

import $ from 'jquery'
// export for others scripts to use: TODO: remove this after eliminating dependencies and testing
window.$ = $
window.jQuery = $

import '@/utils/globalUtils.js'

const app = createApp(App)

app.use(store)
app.use(framework)

const i18n = setupI18n()
app.config.globalProperties.$__i18n = i18n

app.use(i18n)

const router = setupRouter(i18n)

function delay(ms) {
	return new Promise((resolve) => setTimeout(resolve, ms))
}

async function retryWithDelay(maxRetries, timeout, fn) {
	try {
		return await fn()
	} catch (error) {
		if (maxRetries <= 0) throw error
		await delay(timeout)
		return retryWithDelay(maxRetries - 1, timeout, fn)
	}
}

function simpleFetch(controller, action, system = '') {
	const url = `api/${controller}/${action}?system=${system}`
	return fetch(url).then((response) => (response.status === 200 ? response.json() : null))
}

function setAppConfig(data) {
	window.QGlobal = data
	store.dispatch('setYears', data.Years)
	store.dispatch('setDefaultYear', data.defaultSystem)
	store.dispatch('changeYear', data.defaultSystem)
	store.dispatch('changeMultiYearStatus', data.Years.length > 1)
}

//Get the configuration from the backend
retryWithDelay(5, 1000, () => simpleFetch('Main', 'GetGlobalSettingsJson'))
	.then((data) => {
		if (!data) {
			console.error('ERROR: Unable to start the application!')
			return
		}

		setAppConfig(data)

		app.use(router)
		// Init global components
		ComponentsInit(app)

		app.mount('#app')
	})
	.catch((error) => {
		console.error('GetConfig error:', error)
	})

/* Create the Global event bus
For communicating between the components in different levels */
app.config.globalProperties.$eventHub = EventBus

// Global mixin applied to every vue instance
app.mixin({
	computed: {
		currentApp: {
			get() {
				return (this.$store && this.$store.getters.App) || ''
			},
			set(newValue) {
				this.$store.dispatch('changeApp', newValue)
			}
		},
		isMultiYearApp: {
			get() {
				return (this.$store && this.$store.getters.MultiYearStatus) || ''
			},
			set(newValue) {
				this.$store.dispatch('changeMultiYearStatus', newValue)
			}
		},
		currentYear: {
			get() {
				if ($.isEmptyObject(this.$store)) {
					return ''
				}
				return this.$store.getters.Year
			},
			set(newValue) {
				this.$store.dispatch('changeYear', newValue)
				this._changeRouteData(this.currentLang, newValue)
			}
		},
		currentLang: {
			get() {
				if ($.isEmptyObject(this.$store)) {
					return ''
				}
				return this.$store.getters.Language
			},
			set(newValue) {
				this.$store.dispatch('changeLanguage', newValue)
				this._changeRouteData(newValue, this.currentYear)
			}
		}
	},
	created() {
		if (this.$i18n && this.currentLang !== this.$i18n.locale) {
			this.currentLang = this.$i18n.locale
		}
		if ($.isEmptyObject(this.currentYear)) {
			this.currentYear = QGlobal.defaultSystem
		}
		if (!$.isEmptyObject(this.$route)) {
			var params = this.$route.params
			if (!$.isEmptyObject(params.culture) && this.currentLang !== params.culture)
				this.currentLang = params.culture
			if (!$.isEmptyObject(params.system) && this.currentYear !== params.system)
				this.currentYear = params.system
		}
	},
	methods: {
		setYears(years, defaultYear) {
			const _years = years || []
			this.Years = []
			$.each(_years, function (i, y) {
				this.Years.push({ Text: y, Value: y })
			})
			this.DefaultYear = defaultYear
			if ($.isEmptyObject(this.currentYear) || !$.isArray(this.currentYear, _years)) {
				this.currentYear = this.DefaultYear
			}

			this.isMultiYearApp = this.Years.length > 1
		},
		_changeRouteData(culture, system) {
			const route = this.$route
			const rName = route.name
			const rParams = Object.assign({}, route.params)

			if (rParams.culture === culture && rParams.system === system) return
			if ($.isEmptyObject(rName) || rName === 'main' || rName === 'main_params') return

			rParams.culture = culture
			rParams.system = system

			this.$router.replace(
				{ name: rName, params: rParams },
				() => {},
				() => {}
			)
		}
	}
})
