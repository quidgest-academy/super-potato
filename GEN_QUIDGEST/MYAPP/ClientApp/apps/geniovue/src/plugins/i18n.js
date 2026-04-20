import _forEach from 'lodash-es/forEach'
import { createI18n } from 'vue-i18n'

import { axiosInstance } from '@quidgest/clientapp/network'
import eventBus from '@quidgest/clientapp/plugins/eventBus'

import { systemInfo } from '@/systemInfo'

export function setupI18n(options)
{
	if (typeof options !== 'object')
	{
		options = {
			globalInjection: true,
			legacy: false,
			locale: systemInfo.locale.defaultLocale, // Set locale
			fallbackLocale: systemInfo.locale.defaultLocale // Set fallback locale
		}
	}

	const i18n = createI18n(options)
	setI18nLanguage(i18n, options.locale)
	return i18n
}

export function setI18nLanguage(i18n, locale)
{
	const oldLocal = i18n.global.locale.value
	i18n.global.locale.value = locale

	document.getElementsByTagName('html')[0].setAttribute('lang', locale)

	if (oldLocal !== locale)
		eventBus.emit('set-culture', locale)

	return locale
}

export const resourcesMixin = {
	data()
	{
		const vm = this
		return {
			Resources: new Proxy({
				__v_skip: false,
				__v_isReactive: true,
				__v_isRef: false,
				__v_isReadonly: false,
				__v_raw: true
			}, {
				get(target, prop, receiver)
				{
					if (Reflect.has(target, prop))
						return Reflect.get(target, prop, receiver)
					return vm.$tm(prop)
				}
			})
		}
	},

	created()
	{
		if (this.interfaceMetadata)
			eventBus.on('set-culture', this.loadUIResources)
	},

	beforeUnmount()
	{
		if (this.interfaceMetadata)
			eventBus.off('set-culture', this.loadUIResources)
	},

	methods: {
		$getResource(resourceId)
		{
			// Return an empty string for empty resource IDs
			// The $tm() function doesn't do this. It returns a whole object with resource IDs and values.
			if (resourceId === undefined || resourceId === null || resourceId === '')
				return ''
			return this.$tm(resourceId)
		},

		loadUIResources()
		{
			if (this.interfaceMetadata)
				loadResources(this, this.interfaceMetadata.requiredTextResources)
		}
	}
}

const _alreadyLoadedResources = [] // Not critical cache

export function loadResources(vm, resourcesIds)
{
	const i18n = vm.$__i18n,
		locale = i18n.global.locale.value

	const promises = []

	_forEach(resourcesIds, (resourceId) => {
		const resourceName = `${resourceId}.${locale}`

		if (!_alreadyLoadedResources.includes(resourceName))
		{
			_alreadyLoadedResources.push(resourceName)

			promises.push(new Promise((resolve) => {
				const version = systemInfo.genio.buildVersion

				axiosInstance.get(`translations/resources.${resourceName}.json?v=${version}`)
					.then(
						(response) => {
							// When the file is not found it receives HTML
							if (typeof response.data === 'object')
								i18n.global.mergeLocaleMessage(locale, response.data)
							resolve(true)
						},
						() => resolve(false)
					)
			}))
		}
	})

	return Promise.all(promises)
}
