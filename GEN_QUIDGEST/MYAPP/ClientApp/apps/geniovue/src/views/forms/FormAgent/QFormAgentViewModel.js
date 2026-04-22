/* eslint-disable @typescript-eslint/no-unused-vars */
import { computed, reactive, watch } from 'vue'
import _merge from 'lodash-es/merge'

import FormViewModelBase from '@/mixins/formViewModelBase.js'
import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
import modelFieldType from '@quidgest/clientapp/models/fields'

import hardcodedTexts from '@/hardcodedTexts.js'
import netAPI from '@quidgest/clientapp/network'
import qApi from '@/api/genio/quidgestFunctions.js'
import qFunctions from '@/api/genio/projectFunctions.js'
import qProjArrays from '@/api/genio/projectArrays.js'
/* eslint-enable @typescript-eslint/no-unused-vars */

/**
 * Represents a ViewModel class.
 * @extends FormViewModelBase
 */
export default class ViewModel extends FormViewModelBase
{
	/**
	 * Creates a new instance of the ViewModel.
	 * @param {object} vueContext - The Vue context
	 * @param {object} options - The options for the ViewModel
	 * @param {object} values - A ViewModel instance to copy values from
	 */
	// eslint-disable-next-line @typescript-eslint/no-unused-vars
	constructor(vueContext, options, values)
	{
		super(vueContext, options)
		// eslint-disable-next-line @typescript-eslint/no-unused-vars
		const vm = this.vueContext

		// The view model metadata
		_merge(this.modelInfo, {
			name: 'AGENT',
			area: 'AGENT',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_Agent',
				updateFilesTickets: 'UpdateFilesTicketsAgent',
				setFile: 'SetFileAgent'
			}
		})

		/** The primary key. */
		this.ValCodagent = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodagent',
			originId: 'ValCodagent',
			area: 'AGENT',
			field: 'CODAGENT',
			description: '',
		}).cloneFrom(values?.ValCodagent))
		this.stopWatchers.push(watch(() => this.ValCodagent.value, (newValue, oldValue) => this.onUpdate('agent.codagent', this.ValCodagent, newValue, oldValue)))

		/** The used foreign keys. */
		this.ValCborn = reactive(new modelFieldType.ForeignKey({
			id: 'ValCborn',
			originId: 'ValCborn',
			area: 'AGENT',
			field: 'CBORN',
			relatedArea: 'CBORN',
			description: '',
		}).cloneFrom(values?.ValCborn))
		this.stopWatchers.push(watch(() => this.ValCborn.value, (newValue, oldValue) => this.onUpdate('agent.cborn', this.ValCborn, newValue, oldValue)))

		this.ValCodcaddr = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodcaddr',
			originId: 'ValCodcaddr',
			area: 'AGENT',
			field: 'CODCADDR',
			relatedArea: 'CADDR',
			description: '',
		}).cloneFrom(values?.ValCodcaddr))
		this.stopWatchers.push(watch(() => this.ValCodcaddr.value, (newValue, oldValue) => this.onUpdate('agent.codcaddr', this.ValCodcaddr, newValue, oldValue)))

		/** The remaining form fields. */
		this.ValName = reactive(new modelFieldType.String({
			id: 'ValName',
			originId: 'ValName',
			area: 'AGENT',
			field: 'NAME',
			maxLength: 50,
			description: computed(() => this.Resources.AGENT_S_NAME42642),
		}).cloneFrom(values?.ValName))
		this.stopWatchers.push(watch(() => this.ValName.value, (newValue, oldValue) => this.onUpdate('agent.name', this.ValName, newValue, oldValue)))

		this.ValBirthdat = reactive(new modelFieldType.Date({
			id: 'ValBirthdat',
			originId: 'ValBirthdat',
			area: 'AGENT',
			field: 'BIRTHDAT',
			description: computed(() => this.Resources.BIRTHDATE22743),
		}).cloneFrom(values?.ValBirthdat))
		this.stopWatchers.push(watch(() => this.ValBirthdat.value, (newValue, oldValue) => this.onUpdate('agent.birthdat', this.ValBirthdat, newValue, oldValue)))

		this.ValAge = reactive(new modelFieldType.Number({
			id: 'ValAge',
			originId: 'ValAge',
			area: 'AGENT',
			field: 'AGE',
			maxDigits: 3,
			decimalDigits: 0,
			isFixed: true,
			valueFormula: {
				stopRecalcCondition() { return false },
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: Age([AGENT->BIRTHDAT])
					return qFunctions.Age(this.ValBirthdat.value)
				},
				dependencyEvents: ['fieldChange:agent.birthdat'],
				isServerRecalc: false,
				isEmpty: qApi.emptyN,
			},
			description: computed(() => this.Resources.AGE28663),
		}).cloneFrom(values?.ValAge))
		this.stopWatchers.push(watch(() => this.ValAge.value, (newValue, oldValue) => this.onUpdate('agent.age', this.ValAge, newValue, oldValue)))

		this.ValEmail = reactive(new modelFieldType.String({
			id: 'ValEmail',
			originId: 'ValEmail',
			area: 'AGENT',
			field: 'EMAIL',
			maxLength: 80,
			description: computed(() => this.Resources.E_MAIL42251),
		}).cloneFrom(values?.ValEmail))
		this.stopWatchers.push(watch(() => this.ValEmail.value, (newValue, oldValue) => this.onUpdate('agent.email', this.ValEmail, newValue, oldValue)))

		this.ValTelephon = reactive(new modelFieldType.String({
			id: 'ValTelephon',
			originId: 'ValTelephon',
			area: 'AGENT',
			field: 'TELEPHON',
			maxLength: 14,
			maskType: 'MP',
			maskFormat: '+000 000000000',
			maskRequired: '+000 000000000',
			description: computed(() => this.Resources.TELEPHONE28697),
		}).cloneFrom(values?.ValTelephon))
		this.stopWatchers.push(watch(() => this.ValTelephon.value, (newValue, oldValue) => this.onUpdate('agent.telephon', this.ValTelephon, newValue, oldValue)))

		this.TableCbornCountry = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TableCbornCountry',
			originId: 'ValCountry',
			area: 'CBORN',
			field: 'COUNTRY',
			maxLength: 50,
			description: computed(() => this.Resources.COUNTRY64133),
			ignoreFldSubmit: true,
		}).cloneFrom(values?.TableCbornCountry))
		this.stopWatchers.push(watch(() => this.TableCbornCountry.value, (newValue, oldValue) => this.onUpdate('cborn.country', this.TableCbornCountry, newValue, oldValue)))

		this.TableCaddrCountry = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TableCaddrCountry',
			originId: 'ValCountry',
			area: 'CADDR',
			field: 'COUNTRY',
			maxLength: 50,
			description: computed(() => this.Resources.COUNTRY64133),
			ignoreFldSubmit: true,
		}).cloneFrom(values?.TableCaddrCountry))
		this.stopWatchers.push(watch(() => this.TableCaddrCountry.value, (newValue, oldValue) => this.onUpdate('caddr.country', this.TableCaddrCountry, newValue, oldValue)))

		this.ValNrprops = reactive(new modelFieldType.Number({
			id: 'ValNrprops',
			originId: 'ValNrprops',
			area: 'AGENT',
			field: 'NRPROPS',
			maxDigits: 5,
			decimalDigits: 0,
			isFixed: true,
			description: computed(() => this.Resources.NUMBER_OF_PROPERTIES01169),
		}).cloneFrom(values?.ValNrprops))
		this.stopWatchers.push(watch(() => this.ValNrprops.value, (newValue, oldValue) => this.onUpdate('agent.nrprops', this.ValNrprops, newValue, oldValue)))

		this.ValProfit = reactive(new modelFieldType.Number({
			id: 'ValProfit',
			originId: 'ValProfit',
			area: 'AGENT',
			field: 'PROFIT',
			maxDigits: 11,
			decimalDigits: 2,
			isFixed: true,
			description: computed(() => this.Resources.PROFIT55910),
		}).cloneFrom(values?.ValProfit))
		this.stopWatchers.push(watch(() => this.ValProfit.value, (newValue, oldValue) => this.onUpdate('agent.profit', this.ValProfit, newValue, oldValue)))

		this.ValAverage_price = reactive(new modelFieldType.Number({
			id: 'ValAverage_price',
			originId: 'ValAverage_price',
			area: 'AGENT',
			field: 'AVERAGE_PRICE',
			maxDigits: 9,
			decimalDigits: 2,
			isFixed: true,
			valueFormula: {
				stopRecalcCondition() { return false },
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: averagePriceAgent([AGENT->CODAGENT])
					return qFunctions.averagePriceAgent(this.ValCodagent.value)
				},
				dependencyEvents: ['fieldChange:agent.codagent'],
				isServerRecalc: false,
				isEmpty: qApi.emptyN,
			},
			description: computed(() => this.Resources.AVERAGEPRICE13700),
		}).cloneFrom(values?.ValAverage_price))
		this.stopWatchers.push(watch(() => this.ValAverage_price.value, (newValue, oldValue) => this.onUpdate('agent.average_price', this.ValAverage_price, newValue, oldValue)))

		this.ValLastprop = reactive(new modelFieldType.Number({
			id: 'ValLastprop',
			originId: 'ValLastprop',
			area: 'AGENT',
			field: 'LASTPROP',
			maxDigits: 9,
			decimalDigits: 2,
			isFixed: true,
			description: computed(() => this.Resources.LAST_PROPERTY_SOLD__49162),
		}).cloneFrom(values?.ValLastprop))
		this.stopWatchers.push(watch(() => this.ValLastprop.value, (newValue, oldValue) => this.onUpdate('agent.lastprop', this.ValLastprop, newValue, oldValue)))

		this.ValPhotography = reactive(new modelFieldType.Image({
			id: 'ValPhotography',
			originId: 'ValPhotography',
			area: 'AGENT',
			field: 'PHOTOGRA',
			description: computed(() => this.Resources.PHOTOGRAPHY38058),
		}).cloneFrom(values?.ValPhotography))
		this.stopWatchers.push(watch(() => this.ValPhotography.value, (newValue, oldValue) => this.onUpdate('agent.photography', this.ValPhotography, newValue, oldValue)))
	}

	/**
	 * Creates a clone of the current QFormAgentViewModel instance.
	 * @returns {QFormAgentViewModel} A new instance of QFormAgentViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodagent'

	get QPrimaryKey() { return this.ValCodagent.value }
	set QPrimaryKey(value) { this.ValCodagent.updateValue(value) }
}
