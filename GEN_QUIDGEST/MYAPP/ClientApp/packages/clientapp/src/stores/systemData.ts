import { defineStore } from 'pinia'
import { FRAMEWORK_CONFIG_KEY } from '../symbols'
import { inject, ref } from 'vue'

type Module = {
	id: string
	title: string
	vector: string
	font: string
	image: string
}

type Modules = Record<string, Module>

type VersionInfo = {
	buildVersion: number
	dbIdxVersion: number
	dbVersion: string
	genioVersion: string
	trackChangesVersion: string
	assemblyVersion: string
	generationDate: {
		year: number
		month: number
		day: number
	}
}

export const useSystemDataStore = defineStore('systemData', () => {
	const config = inject(FRAMEWORK_CONFIG_KEY)

	if (!config) {
		throw new Error(
			'Framework configuration is not provided. Please ensure the framework is properly initialized.'
		)
	}

	const locale = config.locale

	//----------------------------------------------------------------
	// State
	//----------------------------------------------------------------

	const system = {
		// System
		availableSystems: ref<string[]>([]),
		defaultSystem: ref<string | undefined>(undefined),
		currentSystem: ref<string | undefined>(undefined),

		// Module
		availableModules: ref<Modules>({}),
		defaultModule: ref<string | undefined>('Public'),
		currentModule: ref<string | undefined>('Public'),

		// Languages
		availableLocales: locale.availableLocales,
		defaultLang: locale.defaultLocale,
		currentLang: ref<string | undefined>(locale.defaultLocale),

		defaultListRows: ref<number>(0),
		schedulerLicense: ref<string | undefined>(undefined)
	}

	const versionInfo = ref<VersionInfo>({
		buildVersion: 0,
		dbIdxVersion: 0,
		dbVersion: '',
		genioVersion: '',
		trackChangesVersion: '',
		assemblyVersion: '',
		generationDate: {
			year: 0,
			month: 0,
			day: 0
		}
	})

	//----------------------------------------------------------------
	// Actions
	//----------------------------------------------------------------

	// System
	const setAvailableSystems = (systems: string[]) => {
		system.availableSystems.value = systems
	}
	const setDefaultSystem = (value: string | undefined) => {
		system.defaultSystem.value =
			value && system.availableSystems.value.includes(value) ? value : undefined
	}
	const setCurrentSystem = (value: string | undefined) => {
		system.currentSystem.value =
			value && system.availableSystems.value.includes(value) ? value : undefined
	}

	// Module
	const setAvailableModules = (modules: Modules) => {
		system.availableModules.value = modules
	}
	const setDefaultModule = (value: string | undefined) => {
		if (typeof value !== 'string' || value.length === 0) return
		if (system.defaultModule.value === value) return
		if (!system.availableModules.value[value] && value !== 'Public') return

		system.defaultModule.value = value
	}
	const setCurrentModule = (value: string | undefined) => {
		if (typeof value !== 'string' || value.length === 0) return
		if (system.currentModule.value === value) return
		if (system.availableModules.value[value] === undefined && value !== 'Public') return

		system.currentModule.value = value
	}

	// Language
	const setCurrentLang = (value: string | undefined) => {
		if (typeof value !== 'string' || value.length === 0) return
		if (system.currentLang.value === value) return
		if (!system.availableLocales.find((obj) => obj.language === value)) return

		system.currentLang.value = value
	}

	// Lists
	const setDefaultListRows = (rows: number) => {
		system.defaultListRows.value = rows
	}

	// Scheduler
	const setSchedulerLicenseKey = (license: string | undefined) => {
		system.schedulerLicense.value = license
	}

	// Version Info
	const setVersionInfo = (version: VersionInfo) => {
		versionInfo.value = version
	}

	// Utilities
	const reset = () => {
		system.currentSystem.value = system.defaultSystem.value
		system.currentModule.value = system.defaultModule.value
		system.currentLang.value = system.defaultLang
	}

	return {
		// Getters
		system,
		versionInfo,

		// Actions
		setAvailableSystems,
		setDefaultSystem,
		setCurrentSystem,
		setAvailableModules,
		setDefaultModule,
		setCurrentModule,
		setCurrentLang,
		setDefaultListRows,
		setSchedulerLicenseKey,
		setVersionInfo,

		reset
	}
})
