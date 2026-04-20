import { markRaw } from 'vue'

import hardcodedTexts from '@/hardcodedTexts.js'

function emptyResourceHandler(resourceId)
{
	return resourceId
}

/**
 * Shared icon dictionary.
 */
export const ICONS = Object.freeze({
	// text / numeric / date / enum
	'filter-equal'            : { icon: 'filter-equal',             type: 'svg' },
	'filter-different'        : { icon: 'filter-different',         type: 'svg' },
	'filter-contains'         : { icon: 'filter-contains',          type: 'svg' },
	'filter-no-contains'      : { icon: 'filter-no-contains',       type: 'svg' },
	'filter-starts-with'      : { icon: 'filter-starts-with',       type: 'svg' },
	'filter-is-like'          : { icon: 'filter-is-like',           type: 'svg' },
	'filter-between'          : { icon: 'filter-between',           type: 'svg' },

	// “has/no value”
	'filter-has-value-text'   : { icon: 'filter-has-value-text',    type: 'svg' },
	'filter-has-value-number' : { icon: 'filter-has-value-number',  type: 'svg' },
	'filter-no-value'         : { icon: 'filter-no-value',          type: 'svg' },

	// numerical / date comparisons
	'filter-greater'          : { icon: 'filter-greater',           type: 'svg' },
	'filter-less'             : { icon: 'filter-less',              type: 'svg' },
	'filter-greater-or-equal' : { icon: 'filter-greater-or-equal',  type: 'svg' },
	'filter-less-or-equal'    : { icon: 'filter-less-or-equal',     type: 'svg' },

	// boolean
	'ok'                      : { icon: 'ok',                       type: 'svg' },
	'close'                   : { icon: 'close',                    type: 'svg' }
})

class SearchFilterOperator
{
	constructor(key, resourceId, fnResources, valueCount, icon, {
		placeholderResourceId = undefined,
		inputComponent = undefined,
		defaultValue = undefined,
	} = {})
	{
		this.key = key
		this.resourceId = resourceId
		this.fnResources = fnResources ?? emptyResourceHandler
		this.valueCount = valueCount
		this.icon = ICONS[icon]

		this.placeholderResourceId = placeholderResourceId
		this.inputComponent = inputComponent
		this.defaultValue = defaultValue
	}

	get title()
	{
		return this.fnResources(this.resourceId)
	}

	get placeholder()
	{
		if (!this.placeholderResourceId)
			return ''
		const txt = this.fnResources(this.placeholderResourceId)
		return `%${txt}%`
	}

	destroy()
	{
		this.fnResources = null
		this.inputComponent = null
	}
}

// Search filter condition operators
export class SearchFilterConditionOperators
{
	constructor(fnResources)
	{
		this.fnResources = fnResources ?? emptyResourceHandler
		const _getResource = this.fnResources

		this.text = {
			EQ: new SearchFilterOperator('EQ', hardcodedTexts.isEqualTo, _getResource, 1, 'filter-equal'),
			NOTEQ: new SearchFilterOperator('NOTEQ', hardcodedTexts.notEqual, _getResource, 1, 'filter-different'),
			CON: new SearchFilterOperator('CON', hardcodedTexts.contains, _getResource, 1, 'filter-contains'),
			NOTCON: new SearchFilterOperator('NOTCON', hardcodedTexts.notContains, _getResource, 1, 'filter-no-contains'),
			STRTWTH: new SearchFilterOperator('STRTWTH', hardcodedTexts.startsWith, _getResource, 1, 'filter-starts-with'),
			LIKE: new SearchFilterOperator('LIKE', hardcodedTexts.isLike, _getResource, 1, 'filter-is-like', { placeholderResourceId: hardcodedTexts.keyword }),
			SET: new SearchFilterOperator('SET', hardcodedTexts.hasValue, _getResource, 0, 'filter-has-value-text'),
			NOTSET: new SearchFilterOperator('NOTSET', hardcodedTexts.noValue, _getResource, 0, 'filter-no-value')
		}

		this.num = {
			EQ: new SearchFilterOperator('EQ', hardcodedTexts.isEqualTo, _getResource, 1, 'filter-equal'),
			NOTEQ: new SearchFilterOperator('NOTEQ', hardcodedTexts.notEqual, _getResource, 1, 'filter-different'),
			GREAT: new SearchFilterOperator('GREAT', hardcodedTexts.greaterThan, _getResource, 1, 'filter-greater'),
			LESS: new SearchFilterOperator('LESS', hardcodedTexts.lessThan, _getResource, 1, 'filter-less'),
			GREATEQ: new SearchFilterOperator('GREATEQ', hardcodedTexts.greaterOrEqual, _getResource, 1, 'filter-greater-or-equal'),
			LESSEQ: new SearchFilterOperator('LESSEQ', hardcodedTexts.lessOrEqual, _getResource, 1, 'filter-less-or-equal'),
			BETW: new SearchFilterOperator('BETW', hardcodedTexts.isBetween, _getResource, 2, 'filter-between'),
			SET:  new SearchFilterOperator('SET', hardcodedTexts.hasValue, _getResource, 0, 'filter-has-value-number'),
			NOTSET: new SearchFilterOperator('NOTSET', hardcodedTexts.noValue, _getResource, 0, 'filter-no-value')
		}

		this.bool = {
			TRUE: new SearchFilterOperator('TRUE', hardcodedTexts.isTrue, _getResource, 0, 'ok'),
			FALSE: new SearchFilterOperator('FALSE', hardcodedTexts.isFalse, _getResource, 0, 'close')
		}

		this.date = {
			EQ: new SearchFilterOperator('EQ', hardcodedTexts.isEqualTo, _getResource, 1, 'filter-equal'),
			NOTEQ: new SearchFilterOperator('NOTEQ', hardcodedTexts.notEqual, _getResource, 1, 'filter-different'),
			AFTEQ: new SearchFilterOperator('AFTEQ', hardcodedTexts.afterOrEqual, _getResource, 1, 'filter-greater-or-equal'),
			BEFEQ: new SearchFilterOperator('BEFEQ', hardcodedTexts.beforeOrEqual, _getResource, 1, 'filter-less-or-equal'),
			BETW: new SearchFilterOperator('BETW', hardcodedTexts.isBetween, _getResource, 2, 'filter-between'),
			SET: new SearchFilterOperator('SET', hardcodedTexts.hasValue, _getResource, 0, 'filter-has-value-number'),
			NOTSET: new SearchFilterOperator('NOTSET', hardcodedTexts.noValue, _getResource, 0, 'filter-no-value')
		}

		this.enum = {
			IS: new SearchFilterOperator('IS', hardcodedTexts.is, _getResource, 1, 'filter-equal'),
			ISNOT: new SearchFilterOperator('ISNOT', hardcodedTexts.isNot, _getResource, 1, 'filter-different'),
			IN: new SearchFilterOperator('IN', hardcodedTexts.oneOf, _getResource, 1, 'filter-between', { defaultValue: [] }),
			SET: new SearchFilterOperator('SET', hardcodedTexts.hasValue, _getResource, 0, 'filter-has-value-text'),
			NOTSET: new SearchFilterOperator('NOTSET', hardcodedTexts.noValue, _getResource, 0, 'filter-no-value')
		}
	}

	destroy()
	{
		this.fnResources = null
		const opTypes = ['text', 'num', 'bool', 'date', 'enum']
		opTypes.forEach((opType) => {
			for (const operator in this[opType])
			{
				if (this[opType][operator] instanceof SearchFilterOperator)
					this[opType][operator].destroy()
			}
		})
	}
}

export function getWithTranslation(fnResources)
{
	return markRaw(new SearchFilterConditionOperators(fnResources))
}

export function searchBarOperator(dataType, searchValue)
{
	switch (dataType)
	{
		case 'text':
			return 'CON'
		case 'num':
		case 'date':
			return 'EQ'
		case 'bool':
			return searchValue?.toUpperCase() === 'FALSE' ? 'FALSE' : 'TRUE'
		case 'enum':
			return 'IS'
	}

	return ''
}

export function defaultValue(column)
{
	let value = ''
	switch (column?.searchFieldType)
	{
		case 'text':
		case 'date':
			value = ''
			break
		case 'num':
			value = 0
			break
		case 'bool':
			value = false
			break
		case 'enum':
			if (column.array?.length > 0)
				value = column.array[0].key
			break
	}
	return value
}

// Components used by advanced filters, column filters and editable fields in normal tables
// (different than the ones in the editable table lists)
export const inputComponents = Object.freeze({
	text: 'q-edit-text',
	num: 'q-edit-numeric',
	bool: 'q-edit-boolean',
	date: 'q-edit-datetime',
	enum: 'q-edit-enumeration'
})

export default {
	SearchFilterConditionOperators,
	getWithTranslation,
	searchBarOperator,
	defaultValue,
	inputComponents
}
