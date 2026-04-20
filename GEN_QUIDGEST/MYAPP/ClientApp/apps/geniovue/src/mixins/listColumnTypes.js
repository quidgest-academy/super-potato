import _assignIn from 'lodash-es/assignIn'
import cloneDeep from 'lodash-es/cloneDeep'
import _get from 'lodash-es/get'
import _isEmpty from 'lodash-es/isEmpty'
import _keyBy from 'lodash-es/keyBy'
import _toString from 'lodash-es/toString'
import { markRaw, nextTick, toValue, ref, unref } from 'vue'

import listFunctions from '@/mixins/listFunctions.js'
import { systemInfo } from '@/systemInfo'
import { validateFormula } from '@/utils/formula.js'
import { deepUnwrap } from '@quidgest/clientapp/utils/deepUnwrap'
import { QEventEmitter } from '@quidgest/clientapp/plugins/eventBus'
import { ScopedWatch } from '@quidgest/clientapp/utils/scopedWatch'
import { useGenericDataStore } from '@quidgest/clientapp/stores'
import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

export class BaseColumn
{
	/**
	 * Dedicated watcher scope.
	 * @protected
	 * @type {ScopedWatch}
	 */
	_watchScope

	constructor(options, modelCtx, eventEmitter, init = true, additionalReactiveOptions = {})
	{
		// All watchers created inside this scope are collected automatically.
		// https://vuejs.org/api/reactivity-advanced#effectscope
		Object.defineProperty(this, '_watchScope', {
			value: markRaw(new ScopedWatch(/* detached? */ false)),
			enumerable: false,
			writable: true,
			configurable: true
		})

		this.order = 0
		this.sortOrder = 0
		this.sortAsc = true
		this.dataType = 'None'
		this.searchFieldType = null
		this.dataDisplay = null
		this.dataDisplayText = null
		this.component = null
		this.name = 'Undefined'
		this.area = 'Undefined'
		this.field = 'Undefined'
		this.label = ''
		this.supportForm = null
		this.params = null
		this.cellAction = false
		this.sortable = true
		this.searchable = true
		this.export = 1
		this.array = null
		this.useDistinctValues = false
		this.initialSort = false
		this.initialSortOrder = ''
		this.isDefaultSearch = false
		this.pkColumn = null
		this.isHtmlField = false
		this.multipleValues = false
		this.isSerialized = false
		this.showWhen = null
		this.visibilityListeners = []

		this._stopModelCtxWatcher = null

		const defaultVisible = toValue(additionalReactiveOptions?.visible) ?? true
		const defaultVisibilityEval = toValue(additionalReactiveOptions?.visibilityEval) ?? false

		Object.defineProperties(this, {
			visible: {
				value: ref(defaultVisible),
				configurable: true,
				writable: true,
				enumerable: false
			},
			visibilityEval: {
				value: ref(defaultVisibilityEval),
				configurable: true,
				writable: true,
				enumerable: false
			},
			_isVisible: {
				value: ref(defaultVisible && defaultVisibilityEval),
				configurable: true,
				writable: true,
				enumerable: false
			},
			/**
			 *	Whether the column is currently visible.
			 *	When using computed for isVisible, in newer Vue versions the update of refs and the computed value
			 *		did not always work properly across all projects (possibly due to some "improvement" regarding circular dependencies).
			 *	Therefore, it was replaced with an additional ref and a watcher on the other two refs, which now works better.
			 *	Note: The computed property worked before only because there were parts of the code that overwrote the computed with a primitive value.
			 */
			isVisible: {
				get() {
					return this._isVisible
				},
				set(newValue) {
					this.visible.value = !!newValue
				},
				configurable: true,
				enumerable: true
			},
			modelCtx: {
				value: null,
				configurable: true,
				writable: true,
				enumerable: false
			},
			eventEmitter: {
				value: null,
				configurable: true,
				writable: true,
				enumerable: false
			}
		})

		this.fnVisibility = async () => {
			const isVisible = toValue(this.isVisible)
			this.visibilityEval.value = await validateFormula(this.showWhen, this.modelCtx)

			// If the visibility changes, triggers the listeners.
			if (isVisible !== toValue(this.isVisible))
				for (const listener of this.visibilityListeners)
					listener(toValue(this.isVisible))
		}
		Object.defineProperty(this, 'fnVisibility', { enumerable: false })

		this._watchScope.watch(
			[this.visible, this.visibilityEval],
			([visible, visibilityEval]) => (this._isVisible.value = visible && visibilityEval))

		// Add all properties to itself
		_assignIn(this, options)

		if (init)
			this.init(modelCtx, eventEmitter)
	}

	/**
	 * Asynchronously initializes the necessary properties and listeners in the column.
	 * @param {ViewModel} modelCtx The model context
	 * @param {QEventEmitter} eventEmitter The event emitter instance
	 */
	async init(modelCtx, eventEmitter)
	{
		// Needs to wait so that the "modelCtx" and "eventEmitter" are already set.
		await nextTick()

		this.eventEmitter = unref(eventEmitter)

		if (this.showWhen && this.eventEmitter instanceof QEventEmitter)
		{
			this.modelCtx = unref(modelCtx)
			this._stopModelCtxWatcher?.()
			this._stopModelCtxWatcher = this._watchScope.watch(() => modelCtx, (value) => this.modelCtx = unref(value), { deep: true })

			this.eventEmitter.offMany(this.showWhen.dependencyEvents, this.fnVisibility)
			this.eventEmitter.onMany(this.showWhen.dependencyEvents, this.fnVisibility)
			await this.fnVisibility()
		}
		else
			this.visibilityEval.value = true
	}

	/**
	 * Adds the specified listener to be executed when the column's visibility changes.
	 * @param {function} listener The listener function to add
	 */
	addVisibilityListener(listener)
	{
		if (typeof listener !== 'function')
			throw new Error('The "listener" argument should be a function.')
		this.visibilityListeners.push(listener)
	}

	/**
	 * Creates a clone of this column.
	 * @returns The column clone.
	 */
	clone()
	{
		const sourceWithoutReactive = deepUnwrap(this)
		const clonedThis = cloneDeep(sourceWithoutReactive)
		// Remove computed propperties
		// - The reason isVisible is not protected from override is that we have generated code that overrides isVisible with primitive, on columns defined as non-visible.
		delete clonedThis.isVisible
		delete clonedThis._stopModelCtxWatcher

		// This is needed since non-enumerable properties won't be set, as "cloneDeep" will ignore them.
		const additionalReactiveOptions = {
			visible: toValue(this.visible),
			visibilityEval: toValue(this.visibilityEval)
		}

		// TODO: Arrays after deepUnwrap lose reactivity in the Computed of translated texts.
		// 		 Check if they need a clone with creation of the new array object.
		const clonedCol = new this.constructor(clonedThis, this.modelCtx, this.eventEmitter, false, additionalReactiveOptions)

		return clonedCol
	}

	/**
	 * Normalizes a value based on the column type.
	 * @param {any} value Field value
	 * @returns The normalized value.
	 */
	getNormalizedValue(value)
	{
		return value
	}

	destroy()
	{
		if (this._stopModelCtxWatcher)
			this._stopModelCtxWatcher()
		this._stopModelCtxWatcher = null

		// to dispose all effects in the scope
		this._watchScope?.dispose()
		this._watchScope = null

		this.modelCtx = null

		this.visibilityListeners.splice(0)
		this.visibilityListeners = null
		if (this.showWhen && this.eventEmitter instanceof QEventEmitter)
			this.eventEmitter.offMany(this.showWhen.dependencyEvents, this.fnVisibility)
		this.eventEmitter = null

		delete this.label
		delete this.array
	}
}

export class TextColumn extends BaseColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			dataType: 'Text',
			searchFieldType: 'text',
			dataDisplay: listFunctions.textDisplayCell,
			dataDisplayText: listFunctions.textDisplayCell
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}
}

export class NumericColumn extends BaseColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		const genericDataStore = useGenericDataStore()

		super(_assignIn({
			dataType: 'Numeric',
			searchFieldType: 'num',
			dataDisplay: listFunctions.numericDisplayCell,
			dataDisplayText: listFunctions.numericDisplayCell,
			maxDigits: 0,
			decimalPlaces: 0,
			numberFormat: {
				decimalSeparator: genericDataStore.numberFormat.decimalSeparator,
				groupSeparator: genericDataStore.numberFormat.thousandsSeparator,
				negativeFormat: genericDataStore.numberFormat.negativeFormat
			},
			showTotal: true,
			columnClasses: 'c-table__cell-numeric row-numeric',
			columnHeaderClasses: 'c-table__head-numeric'
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}

	/**
	 * @override
	 */
	async init(modelCtx, eventEmitter)
	{
		super.init(modelCtx, eventEmitter)

		// Add class for column with row re-order controls
		if (this.sortOrder > 0)
			this.columnHeaderClasses += ' thead-order'
	}

	/**
	 * Normalizes the specified value as numeric.
	 * @param {any} value The value of the column
	 * @returns {number} A numeric value, or undefined.
	 */
	getNormalizedValue(value)
	{
		const groupSep = this.numberFormat.groupSeparator
		value = value?.toString()
			.replace(groupSep ? new RegExp('\\' + groupSep, 'g') : '', '')
			.replace(new RegExp('\\' + this.numberFormat.decimalSeparator, 'g'), '.')

		const numericVal = Number(value)
		return Number.isFinite(numericVal) ? numericVal : undefined
	}
}

export class CurrencyColumn extends NumericColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			dataType: 'Currency',
			searchFieldType: 'num',
			dataDisplay: listFunctions.currencyDisplayCell,
			dataDisplayText: listFunctions.currencyDisplayCell,
			currency: systemInfo.system.baseCurrency.code,
			currencySymbol: systemInfo.system.baseCurrency.symbol
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}
}

export class DateColumn extends BaseColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		const genericDataStore = useGenericDataStore()

		super(_assignIn({
			dataType: 'Date',
			searchFieldType: 'date',
			dateTimeType: 'dateTime',
			dateFormats: genericDataStore.dateFormat,
			dataDisplay: listFunctions.dateDisplayCell,
			dataDisplayText: listFunctions.dateDisplayCell
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)

		this.format = genericDataStore.dateFormat[this.dateTimeType]
	}

	/**
	 * Convert a date's format to ISO based on the column the date comes from
	 * @param {string} value A string formatted as a date in a configuration defined format
	 * @returns {string} A string in ISO date format
	 */
	getNormalizedValue(value)
	{
		// Convert to date object
		const parsedDate = genericFunctions.stringToDate(value, this.format)
		return parsedDate === null
			? ''
			: parsedDate.toISOString()
	}
}

export class BooleanColumn extends BaseColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			dataType: 'Boolean',
			searchFieldType: 'bool',
			component: 'q-render-boolean',
			dataDisplay: listFunctions.booleanDisplayCell,
			dataDisplayText: listFunctions.booleanDisplayCell
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}

	/**
	 * Normalizes the specified value as boolean.
	 * @param {any} value The value of the column
	 * @returns {boolean} A boolean value, or undefined.
	 */
	getNormalizedValue(value)
	{
		const isBool = typeof value === 'boolean' ||
			value === 1 || value === 0 ||
			value === 'true' || value === 'false'
		return isBool ? Boolean(value) : undefined
	}
}

export class ImageColumn extends BaseColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			dataType: 'Image',
			component: 'q-render-image',
			cellAction: true,
			dataDisplay: listFunctions.imageDisplayCell,
			dataDisplayText: listFunctions.imageDisplayCell
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}
}

export class DocumentColumn extends BaseColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			dataType: 'Document',
			searchFieldType: 'text',
			component: 'q-render-document',
			dataDisplay: listFunctions.documentDisplayCell,
			dataDisplayText: listFunctions.documentDisplayCell,
			isSerialized: true
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}
}

export class ArrayColumn extends BaseColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			dataType: 'Array',
			component: 'q-render-array',
			searchFieldType: 'enum',
			array: [],
			dataDisplay: listFunctions.enumerationDisplayCell,
			dataDisplayText: listFunctions.enumerationDisplayCell
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}

	get arrayAsObj()
	{
		return new Proxy(_keyBy(this.array, (aElem) => _toString(aElem.key)), {
			get(target, name)
			{
				return _get({
					__v_skip: true,
					__v_isReactive: true,
					__v_isRef: false,
					__v_isReadonly: true,
					__v_raw: true
				}, name, _get(target, name, {}))
			}
		})
	}

	/**
	 * @override
	 */
	getNormalizedValue(value)
	{
		// Check if value is already a key.
		if (!_isEmpty(this.arrayAsObj[value]))
			return value

		// If it's not a key, we check if it matches any of the labels.
		const options = this.array.filter((e) => unref(e.value).toUpperCase().startsWith(value.toUpperCase()))
		return options.length === 1 ? options[0].key : undefined
	}
}

export class GeographicColumn extends BaseColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		const genericDataStore = useGenericDataStore()

		super(_assignIn({
			dataType: 'Geographic',
			dataDisplay: listFunctions.geographicDisplayCell,
			dataDisplayText: listFunctions.geographicDisplayCell,
			numberFormat: {
				decimalSeparator: genericDataStore.numberFormat.decimalSeparator,
				groupSeparator: genericDataStore.numberFormat.thousandsSeparator,
				negativeFormat: genericDataStore.numberFormat.negativeFormat
			}
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}
}

export class GeographicShapeColumn extends BaseColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			dataType: 'GeographicShape',
			dataDisplay: listFunctions.geographicShapeDisplayCell,
			dataDisplayText: listFunctions.geographicShapeDisplayCell
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}
}

export class GeometricShapeColumn extends GeographicShapeColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			dataType: 'GeometricShape'
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}
}

export class HyperLinkColumn extends BaseColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			dataType: 'Text',
			searchFieldType: 'text',
			dataDisplay: listFunctions.hyperLinkDisplayCell,
			dataDisplayText: listFunctions.hyperLinkDisplayCell,
			component: 'q-render-hyperlink'
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}
}

export class HtmlColumn extends TextColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			component: 'q-render-html',
			isHtmlField: true
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}
}

export class MarkdownColumn extends TextColumn
{
	constructor(options, modelCtx, eventEmitter, init, additionalReactiveOptions)
	{
		super(_assignIn({
			component: 'q-render-markdown',
			isHtmlField: true
		}, options), modelCtx, eventEmitter, init, additionalReactiveOptions)
	}
}

export default {
	BaseColumn,
	TextColumn,
	NumericColumn,
	CurrencyColumn,
	DateColumn,
	BooleanColumn,
	ImageColumn,
	DocumentColumn,
	ArrayColumn,
	GeographicColumn,
	GeographicShapeColumn,
	GeometricShapeColumn,
	HyperLinkColumn,
	HtmlColumn,
	MarkdownColumn
}
