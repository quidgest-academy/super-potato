import _merge from 'lodash-es/merge'
import { computed } from 'vue'

import { useGenericDataStore, useSystemDataStore } from '@quidgest/clientapp/stores'

export class BaseProperty {
	/**
	 * Base constructor
	 * @param {object} options
	 * @param {object} vueContext
	 */
	constructor(options, vueContext)
	{
		this.getResource = vueContext.$getResource

		this.rowId = ''
		this.id = ''
		this.component = ''
		this.name = ''
		this.label = ''
		this.description = ''
		this.group = ''
		this.isRowDirty = false
		this.defaultValue = ''
		this.props = {
			modelValue: undefined,
			disabled: false,
			readonly: false,
			required: false
		}

		_merge(this, options)
	}

	destroy()
	{
		this.getResource = null
	}
}

export class StringProperty extends BaseProperty {
	constructor(options, vueContext)
	{
		super({
			component: 'q-text-field',
			type: 'string',
			props: {
				maxLength: 20
			}
		}, vueContext)

		_merge(this, options)
	}
}

export class MultilineTextProperty extends BaseProperty {
	constructor(options, vueContext)
	{
		super({
			component: 'q-text-area',
			type: 'multiline',
			props: {
				rows: 2,
				cols: 20,
				resize: 'none',
			}
		}, vueContext)

		_merge(this, options)
	}
}

export class BooleanProperty extends BaseProperty {
	constructor(options, vueContext)
	{
		super({
			component: 'q-switch',
			type: 'boolean',
			defaultValue: false,
			props: {
				size: 'small'
			}
		}, vueContext)

		_merge(this, options)
	}
}

export class DateProperty extends BaseProperty {
	constructor(options, vueContext)
	{
		const systemDataStore = useSystemDataStore()
		const genericDataStore = useGenericDataStore()

		super({
			component: 'q-date-time-picker',
			type: 'date',
			dateFormat: genericDataStore.dateFormat.date,
			props: {
				format: 'date',
				locale: computed(() => systemDataStore.system.currentLang)
			}
		}, vueContext)

		_merge(this, options)
	}
}

export class ArrayProperty extends BaseProperty {
	constructor(options, vueContext)
	{
		super({
			component: 'q-select',
			type: 'array',
			array: [],
			props: {
				items: [],
				groups: [],
				clearable: true,
			}
		}, vueContext)

		_merge(this, options)

		this.init()
	}

	get elements() {
		return this.array.elements
	}

	get groups() {
		return this.array.groups
	}

	init()
	{
		this.setElements()

		if(Array.isArray(this.groups) && this.groups.length > 0)
		{
			this.setGroups()
		}
	}

	setGroups(){
		this.props.groups = Object.values(this.groups).map(group => {
			return {
				id: group.id,
				title: computed(() => this.getResource(group.resourceId))
			}
		})
	}

	setElements(){
		this.props.items = Object.values(this.elements).map(element => {
			return {
				key: element.key,
				group: element.group ?? '',
				icon: element.icon ?? null,
				value: computed(() => this.getResource(element.resourceId))
			}
		})
	}

	destroy()
	{
		super.destroy()

		this.array.length = 0
	}
}

export class NumberProperty extends BaseProperty {
	constructor(options, vueContext)
	{
		const genericDataStore = useGenericDataStore()

		super({
			component: 'q-numeric-input',
			type: 'number',
			defaultValue: 0,
			props: {
				thousandsSeparator: genericDataStore.numberFormat.thousandsSeparator,
				decimalPoint: genericDataStore.numberFormat.decimalSeparator,
				maxDigits: 0,
				decimalDigits: 0,
				isDecimal: true,
				modelValue: 0
			}
		}, vueContext)

		_merge(this, options)
	}
}

export default {
	BaseProperty,
	BooleanProperty,
	DateProperty,
	MultilineTextProperty,
	NumberProperty,
	ArrayProperty,
	StringProperty
}
