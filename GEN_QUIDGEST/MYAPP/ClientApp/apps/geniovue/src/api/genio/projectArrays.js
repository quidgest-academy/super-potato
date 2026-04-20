/* eslint-disable @typescript-eslint/no-unused-vars */
import { computed, reactive, readonly } from 'vue'
import _merge from 'lodash-es/merge'

import netAPI from '@quidgest/clientapp/network'

/**
 * Represents a single option with a key, resourceId and reactive value.
 * Uses a WeakRef to hold the translation function so that the component
 * may be garbage-collected once it is no longer referenced elsewhere.
 */
export class Option {
	/**
	* @param {number} key - Unique identifier for this option.
	* @param {string} resourceId - Key used to look up the translated text.
	* @param {Function} fnResources - Function (resourceId -> string) from the Vue component.
	*/
	constructor({ key, resourceId, fnResources, helpResourceId, helpResourceVerboseId, group, icon } = {}) {
		this.key = key
		this.resourceId = resourceId
		this.helpResourceId = helpResourceId
		this.helpResourceVerboseId = helpResourceVerboseId
		this.group = group
		this.icon = icon

		// Store a weak reference to the translation function.
		// .deref() will return undefined if the original function has been
		// garbage-collected, avoiding retention of the component proxy.
		Object.defineProperty(this, '_weakFn', {
			value: typeof fnResources === 'function' ? new WeakRef(fnResources) : null,
			enumerable: false,
			configurable: true
		})

		// Create a computed property for the translated value. The computed
		// only depends on the weak reference, so when the component unmounts
		// and the function is reclaimed, this will fall back to resourceId.
		Object.defineProperty(this, 'value', {
			value: computed(() => {
				const fn = this._weakFn?.deref()
				return typeof fn === 'function'
					? fn(this.resourceId)
					: this.resourceId
			}),
			enumerable: true,
			configurable: true
		})

		if(typeof this.helpResourceId === 'string') {
			Object.defineProperty(this, 'description', {
				value: computed(() => {
					const fn = this._weakFn?.deref()
					return typeof fn === 'function'
						? fn(this.helpResourceId)
						: this.helpResourceId
				}),
				enumerable: false,
				configurable: true
			})
		}

		if(typeof this.helpResourceVerboseId === 'string') {
			Object.defineProperty(this, 'descriptionVerbose', {
				value: computed(() => {
					const fn = this._weakFn?.deref()
					return typeof fn === 'function'
						? fn(this.helpResourceVerboseId)
						: this.helpResourceVerboseId
				}),
				enumerable: false,
				configurable: true
			})
		}
	}
}

export class GroupOption {
	constructor({ id, resourceId, fnResources } = {}) {
		this.id = id
		this.resourceId = resourceId

		// Store a weak reference to the translation function.
		// .deref() will return undefined if the original function has been
		// garbage-collected, avoiding retention of the component proxy.
		Object.defineProperty(this, '_weakFn', {
			value: typeof fnResources === 'function' ? new WeakRef(fnResources) : null,
			enumerable: false,
			configurable: true
		})

		// Create a computed property for the translated value. The computed
		// only depends on the weak reference, so when the component unmounts
		// and the function is reclaimed, this will fall back to resourceId.
		if(typeof this.resourceId === 'string') {
			Object.defineProperty(this, 'title', {
				value: computed(() => {
					const fn = this._weakFn?.deref()
					return typeof fn === 'function'
						? fn(this.resourceId)
						: this.resourceId
				}),
				enumerable: true,
				configurable: true
			})
		}
	}
}

/* eslint-enable @typescript-eslint/no-unused-vars */
/**
 * The buildtyp array.
 */
export class QArrayBuildtyp
{
	// eslint-disable-next-line @typescript-eslint/no-unused-vars
	constructor(fnResources)
	{
		this.type = 'C'
		this.pluralName = 'BUILDING_TYPES23872'
		this.singularName = 'BUILDING_TYPE57152'

		this.elements = [
			new Option({
				num: 1,
				key: 'apartment',
				resourceId: 'APARTMENT12665',
				fnResources,
			}),
			new Option({
				num: 2,
				key: 'house',
				resourceId: 'HOUSE01993',
				fnResources,
			}),
			new Option({
				num: 3,
				key: 'other',
				resourceId: 'OTHER37293',
				fnResources,
			}),
		]

	}
}

/**
 * The s_modpro array.
 */
export class QArrayS_modpro
{
	// eslint-disable-next-line @typescript-eslint/no-unused-vars
	constructor(fnResources)
	{
		this.type = 'C'
		this.pluralName = 'MODOS_DE_PROCESSAMEN07602'
		this.singularName = 'MODO_DE_PROCESSAMENT14469'

		this.elements = [
			new Option({
				num: 1,
				key: 'INDIV',
				resourceId: 'INDIVIDUAL42893',
				fnResources,
			}),
			new Option({
				num: 2,
				key: 'global',
				resourceId: 'GLOBAL58588',
				fnResources,
			}),
			new Option({
				num: 3,
				key: 'unidade',
				resourceId: 'UNIDADE_ORGANICA38383',
				fnResources,
			}),
			new Option({
				num: 4,
				key: 'horario',
				resourceId: 'HORARIO56549',
				fnResources,
			}),
		]

	}
}

/**
 * The s_module array.
 */
export class QArrayS_module
{
	/**
	 * Static cache to store reactive arrays by language code.
	 * Prevents multiple network requests and ensures reactive consistency.
	 * @type {Map<string, Ref[]>}
	 */
	static _langCache = new Map()

	constructor(lang)
	{
		this.type = 'C'
		this.pluralName = 'MODULES33542'
		this.singularName = 'MODULE42049'

		this.currentLang = typeof lang === 'string' ? lang.replace('-', '').toUpperCase() : null

		// Initialise cache if missing
		if (!QArrayS_module._langCache.has(this.currentLang)) {
			const array = reactive([])
			QArrayS_module._langCache.set(this.currentLang, array)

			// Fetch only once per language
			netAPI.fetchDynamicArray('S_module', this.currentLang, (res) => {
				_merge(array, res)
			})
		}

	}

	get elements()
	{
		return readonly(QArrayS_module._langCache.get(this.currentLang) || [])
	}
}

/**
 * The s_prstat array.
 */
export class QArrayS_prstat
{
	// eslint-disable-next-line @typescript-eslint/no-unused-vars
	constructor(fnResources)
	{
		this.type = 'C'
		this.pluralName = 'ESTADOS_DO_PROCESSO59118'
		this.singularName = 'ESTADO_DO_PROCESSO07540'

		this.elements = [
			new Option({
				num: 1,
				key: 'EE',
				resourceId: 'EM_EXECUCAO53706',
				fnResources,
			}),
			new Option({
				num: 2,
				key: 'FE',
				resourceId: 'EM_FILA_DE_ESPERA21822',
				fnResources,
			}),
			new Option({
				num: 3,
				key: 'AG',
				resourceId: 'AGENDADO_PARA_EXECUC11223',
				fnResources,
			}),
			new Option({
				num: 4,
				key: 'T',
				resourceId: 'TERMINADO46276',
				fnResources,
			}),
			new Option({
				num: 5,
				key: 'C',
				resourceId: 'CANCELADO05982',
				fnResources,
			}),
			new Option({
				num: 6,
				key: 'NR',
				resourceId: 'NAO_RESPONDE33275',
				fnResources,
			}),
			new Option({
				num: 7,
				key: 'AB',
				resourceId: 'ABORTADO52378',
				fnResources,
			}),
			new Option({
				num: 8,
				key: 'AC',
				resourceId: 'A_CANCELAR43988',
				fnResources,
			}),
		]

	}
}

/**
 * The s_resul array.
 */
export class QArrayS_resul
{
	// eslint-disable-next-line @typescript-eslint/no-unused-vars
	constructor(fnResources)
	{
		this.type = 'C'
		this.pluralName = 'RESULTADOS20000'
		this.singularName = 'RESULTADO50955'

		this.elements = [
			new Option({
				num: 1,
				key: 'ok',
				resourceId: 'SUCESSO65230',
				fnResources,
			}),
			new Option({
				num: 2,
				key: 'er',
				resourceId: 'ERRO38355',
				fnResources,
			}),
			new Option({
				num: 3,
				key: 'wa',
				resourceId: 'AVISO03237',
				fnResources,
			}),
			new Option({
				num: 4,
				key: 'c',
				resourceId: 'CANCELADO05982',
				fnResources,
			}),
		]

	}
}

/**
 * The s_roles array.
 */
export class QArrayS_roles
{
	/**
	 * Static cache to store reactive arrays by language code.
	 * Prevents multiple network requests and ensures reactive consistency.
	 * @type {Map<string, Ref[]>}
	 */
	static _langCache = new Map()

	constructor(lang)
	{
		this.type = 'C'
		this.pluralName = 'ROLE60946'
		this.singularName = 'ROLES61449'

		this.currentLang = typeof lang === 'string' ? lang.replace('-', '').toUpperCase() : null

		// Initialise cache if missing
		if (!QArrayS_roles._langCache.has(this.currentLang)) {
			const array = reactive([])
			QArrayS_roles._langCache.set(this.currentLang, array)

			// Fetch only once per language
			netAPI.fetchDynamicArray('S_roles', this.currentLang, (res) => {
				_merge(array, res)
			})
		}

	}

	get elements()
	{
		return readonly(QArrayS_roles._langCache.get(this.currentLang) || [])
	}
}

/**
 * The s_tpproc array.
 */
export class QArrayS_tpproc
{
	/**
	 * Static cache to store reactive arrays by language code.
	 * Prevents multiple network requests and ensures reactive consistency.
	 * @type {Map<string, Ref[]>}
	 */
	static _langCache = new Map()

	constructor(lang)
	{
		this.type = 'C'
		this.pluralName = 'PROCESS_TYPES19050'
		this.singularName = 'PROCESS_TYPE50593'

		this.currentLang = typeof lang === 'string' ? lang.replace('-', '').toUpperCase() : null

		// Initialise cache if missing
		if (!QArrayS_tpproc._langCache.has(this.currentLang)) {
			const array = reactive([])
			QArrayS_tpproc._langCache.set(this.currentLang, array)

			// Fetch only once per language
			netAPI.fetchDynamicArray('S_tpproc', this.currentLang, (res) => {
				_merge(array, res)
			})
		}

	}

	get elements()
	{
		return readonly(QArrayS_tpproc._langCache.get(this.currentLang) || [])
	}
}

/**
 * The typology array.
 */
export class QArrayTypology
{
	// eslint-disable-next-line @typescript-eslint/no-unused-vars
	constructor(fnResources)
	{
		this.type = 'N'
		this.pluralName = 'BUILDING_TYPOLOGY54011'
		this.singularName = 'BUILDING_TYPOLOGY54011'

		this.elements = [
			new Option({
				num: 1,
				key: 0,
				resourceId: 'T036607',
				fnResources,
			}),
			new Option({
				num: 2,
				key: 1,
				resourceId: 'T133664',
				fnResources,
			}),
			new Option({
				num: 3,
				key: 2,
				resourceId: 'T233813',
				fnResources,
			}),
			new Option({
				num: 4,
				key: 3,
				resourceId: 'T3_OR_MORE43214',
				fnResources,
			}),
		]

	}
}


export default {
	QArrayBuildtyp,
	QArrayS_modpro,
	QArrayS_module,
	QArrayS_prstat,
	QArrayS_resul,
	QArrayS_roles,
	QArrayS_tpproc,
	QArrayTypology,
}
