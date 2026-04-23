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
			name: 'PROPERTY',
			area: 'PROPE',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_Property',
				updateFilesTickets: 'UpdateFilesTicketsProperty',
				setFile: 'SetFileProperty'
			}
		})

		/** The primary key. */
		this.ValCodprope = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodprope',
			originId: 'ValCodprope',
			area: 'PROPE',
			field: 'CODPROPE',
			description: '',
		}).cloneFrom(values?.ValCodprope))
		this.stopWatchers.push(watch(() => this.ValCodprope.value, (newValue, oldValue) => this.onUpdate('prope.codprope', this.ValCodprope, newValue, oldValue)))

		/** The used foreign keys. */
		this.ValCodcity = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodcity',
			originId: 'ValCodcity',
			area: 'PROPE',
			field: 'CODCITY',
			relatedArea: 'CITY',
			description: '',
		}).cloneFrom(values?.ValCodcity))
		this.stopWatchers.push(watch(() => this.ValCodcity.value, (newValue, oldValue) => this.onUpdate('prope.codcity', this.ValCodcity, newValue, oldValue)))

		this.ValCodagent = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodagent',
			originId: 'ValCodagent',
			area: 'PROPE',
			field: 'CODAGENT',
			relatedArea: 'AGENT',
			description: '',
		}).cloneFrom(values?.ValCodagent))
		this.stopWatchers.push(watch(() => this.ValCodagent.value, (newValue, oldValue) => this.onUpdate('prope.codagent', this.ValCodagent, newValue, oldValue)))

		/** The remaining form fields. */
		this.ValId = reactive(new modelFieldType.Number({
			id: 'ValId',
			originId: 'ValId',
			area: 'PROPE',
			field: 'ID',
			maxDigits: 5,
			decimalDigits: 0,
			description: '',
		}).cloneFrom(values?.ValId))
		this.stopWatchers.push(watch(() => this.ValId.value, (newValue, oldValue) => this.onUpdate('prope.id', this.ValId, newValue, oldValue)))

		this.ValSold = reactive(new modelFieldType.Boolean({
			id: 'ValSold',
			originId: 'ValSold',
			area: 'PROPE',
			field: 'SOLD',
			description: computed(() => this.Resources.SOLD59824),
		}).cloneFrom(values?.ValSold))
		this.stopWatchers.push(watch(() => this.ValSold.value, (newValue, oldValue) => this.onUpdate('prope.sold', this.ValSold, newValue, oldValue)))

		this.ValDtsold = reactive(new modelFieldType.Date({
			id: 'ValDtsold',
			originId: 'ValDtsold',
			area: 'PROPE',
			field: 'DTSOLD',
			fillWhen: {
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: [PROPE->SOLD]==1
					return (this.ValSold.value ? 1 : 0)===1
				},
				dependencyEvents: ['fieldChange:prope.sold'],
				isServerRecalc: false,
				isEmpty: qApi.emptyD,
			},
			showWhen: {
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: [PROPE->SOLD]==1
					return (this.ValSold.value ? 1 : 0)===1
				},
				dependencyEvents: ['fieldChange:prope.sold'],
				isServerRecalc: false,
				isEmpty: qApi.emptyD,
			},
			description: computed(() => this.Resources.SOLD_DATE37976),
		}).cloneFrom(values?.ValDtsold))
		this.stopWatchers.push(watch(() => this.ValDtsold.value, (newValue, oldValue) => this.onUpdate('prope.dtsold', this.ValDtsold, newValue, oldValue)))

		this.ValLastvisit = reactive(new modelFieldType.Date({
			id: 'ValLastvisit',
			originId: 'ValLastvisit',
			area: 'PROPE',
			field: 'LASTVISIT',
			isFixed: true,
			description: computed(() => this.Resources.LAST_VISIT61343),
		}).cloneFrom(values?.ValLastvisit))
		this.stopWatchers.push(watch(() => this.ValLastvisit.value, (newValue, oldValue) => this.onUpdate('prope.lastvisit', this.ValLastvisit, newValue, oldValue)))

		this.ValAverage = reactive(new modelFieldType.Number({
			id: 'ValAverage',
			originId: 'ValAverage',
			area: 'PROPE',
			field: 'AVERAGE',
			maxDigits: 12,
			decimalDigits: 0,
			isFixed: true,
			valueFormula: {
				stopRecalcCondition() { return false },
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: Average()
					return qFunctions.Average()
				},
				dependencyEvents: [],
				isServerRecalc: false,
				isEmpty: qApi.emptyN,
			},
			description: computed(() => this.Resources.AVERAGEPRICE13700),
		}).cloneFrom(values?.ValAverage))
		this.stopWatchers.push(watch(() => this.ValAverage.value, (newValue, oldValue) => this.onUpdate('prope.average', this.ValAverage, newValue, oldValue)))

		this.TableCityCity = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TableCityCity',
			originId: 'ValCity',
			area: 'CITY',
			field: 'CITY',
			maxLength: 50,
			description: computed(() => this.Resources.CITY42505),
			ignoreFldSubmit: true,
		}).cloneFrom(values?.TableCityCity))
		this.stopWatchers.push(watch(() => this.TableCityCity.value, (newValue, oldValue) => this.onUpdate('city.city', this.TableCityCity, newValue, oldValue)))

		this.CityCountValCountry = reactive(new modelFieldType.String({
			id: 'CityCountValCountry',
			originId: 'ValCountry',
			area: 'COUNT',
			field: 'COUNTRY',
			maxLength: 50,
			isFixed: true,
			description: computed(() => this.Resources.COUNTRY64133),
		}).cloneFrom(values?.CityCountValCountry))
		this.stopWatchers.push(watch(() => this.CityCountValCountry.value, (newValue, oldValue) => this.onUpdate('count.country', this.CityCountValCountry, newValue, oldValue)))

		this.ValPrice = reactive(new modelFieldType.Number({
			id: 'ValPrice',
			originId: 'ValPrice',
			area: 'PROPE',
			field: 'PRICE',
			maxDigits: 9,
			decimalDigits: 2,
			description: computed(() => this.Resources.PRICE06900),
		}).cloneFrom(values?.ValPrice))
		this.stopWatchers.push(watch(() => this.ValPrice.value, (newValue, oldValue) => this.onUpdate('prope.price', this.ValPrice, newValue, oldValue)))

		this.ValTypology = reactive(new modelFieldType.Number({
			id: 'ValTypology',
			originId: 'ValTypology',
			area: 'PROPE',
			field: 'TYPOLOGY',
			maxDigits: 1,
			decimalDigits: 0,
			arrayOptions: computed(() => new qProjArrays.QArrayTypology(vm.$getResource).elements),
			description: computed(() => this.Resources.BUILDING_TYPOLOGY54011),
		}).cloneFrom(values?.ValTypology))
		this.stopWatchers.push(watch(() => this.ValTypology.value, (newValue, oldValue) => this.onUpdate('prope.typology', this.ValTypology, newValue, oldValue)))

		this.ValBuildtyp = reactive(new modelFieldType.String({
			id: 'ValBuildtyp',
			originId: 'ValBuildtyp',
			area: 'PROPE',
			field: 'BUILDTYP',
			maxLength: 1,
			arrayOptions: computed(() => new qProjArrays.QArrayBuildtyp(vm.$getResource).elements),
			description: computed(() => this.Resources.BUILDING_TYPE57152),
		}).cloneFrom(values?.ValBuildtyp))
		this.stopWatchers.push(watch(() => this.ValBuildtyp.value, (newValue, oldValue) => this.onUpdate('prope.buildtyp', this.ValBuildtyp, newValue, oldValue)))

		this.ValGrdsize = reactive(new modelFieldType.Number({
			id: 'ValGrdsize',
			originId: 'ValGrdsize',
			area: 'PROPE',
			field: 'GRDSIZE',
			maxDigits: 9,
			decimalDigits: 0,
			showWhen: {
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: [PROPE->BUILDTYP]=="house"
					return this.ValBuildtyp.value==="house"
				},
				dependencyEvents: ['fieldChange:prope.buildtyp'],
				isServerRecalc: false,
				isEmpty: qApi.emptyN,
			},
			description: computed(() => this.Resources.GROUND_SIZE01563),
		}).cloneFrom(values?.ValGrdsize))
		this.stopWatchers.push(watch(() => this.ValGrdsize.value, (newValue, oldValue) => this.onUpdate('prope.grdsize', this.ValGrdsize, newValue, oldValue)))

		this.ValFloornr = reactive(new modelFieldType.Number({
			id: 'ValFloornr',
			originId: 'ValFloornr',
			area: 'PROPE',
			field: 'FLOORNR',
			maxDigits: 2,
			decimalDigits: 0,
			showWhen: {
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: [PROPE->BUILDTYP]=="apartment"
					return this.ValBuildtyp.value==="apartment"
				},
				dependencyEvents: ['fieldChange:prope.buildtyp'],
				isServerRecalc: false,
				isEmpty: qApi.emptyN,
			},
			description: computed(() => this.Resources.FLOOR19993),
		}).cloneFrom(values?.ValFloornr))
		this.stopWatchers.push(watch(() => this.ValFloornr.value, (newValue, oldValue) => this.onUpdate('prope.floornr', this.ValFloornr, newValue, oldValue)))

		this.ValSize = reactive(new modelFieldType.Number({
			id: 'ValSize',
			originId: 'ValSize',
			area: 'PROPE',
			field: 'SIZE',
			maxDigits: 8,
			decimalDigits: 0,
			description: computed(() => this.Resources.SIZE__M2_57059),
		}).cloneFrom(values?.ValSize))
		this.stopWatchers.push(watch(() => this.ValSize.value, (newValue, oldValue) => this.onUpdate('prope.size', this.ValSize, newValue, oldValue)))

		this.ValBathnr = reactive(new modelFieldType.Number({
			id: 'ValBathnr',
			originId: 'ValBathnr',
			area: 'PROPE',
			field: 'BATHNR',
			maxDigits: 2,
			decimalDigits: 0,
			description: computed(() => this.Resources.BATHROOMS_NUMBER52698),
		}).cloneFrom(values?.ValBathnr))
		this.stopWatchers.push(watch(() => this.ValBathnr.value, (newValue, oldValue) => this.onUpdate('prope.bathnr', this.ValBathnr, newValue, oldValue)))

		this.ValDtconst = reactive(new modelFieldType.Date({
			id: 'ValDtconst',
			originId: 'ValDtconst',
			area: 'PROPE',
			field: 'DTCONST',
			description: computed(() => this.Resources.CONSTRUCTION_DATE18132),
		}).cloneFrom(values?.ValDtconst))
		this.stopWatchers.push(watch(() => this.ValDtconst.value, (newValue, oldValue) => this.onUpdate('prope.dtconst', this.ValDtconst, newValue, oldValue)))

		this.ValBuildage = reactive(new modelFieldType.Number({
			id: 'ValBuildage',
			originId: 'ValBuildage',
			area: 'PROPE',
			field: 'BUILDAGE',
			maxDigits: 4,
			decimalDigits: 0,
			isFixed: true,
			valueFormula: {
				stopRecalcCondition() { return false },
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: Year([Today])- Year([PROPE->DTCONST])
					return qApi.Year(qApi.Today())-qApi.Year(this.ValDtconst.value)
				},
				dependencyEvents: ['fieldChange:prope.dtconst'],
				isServerRecalc: false,
				isEmpty: qApi.emptyN,
			},
			description: computed(() => this.Resources.BUILDING_AGE27311),
		}).cloneFrom(values?.ValBuildage))
		this.stopWatchers.push(watch(() => this.ValBuildage.value, (newValue, oldValue) => this.onUpdate('prope.buildage', this.ValBuildage, newValue, oldValue)))

		this.TableAgentName = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TableAgentName',
			originId: 'ValName',
			area: 'AGENT',
			field: 'NAME',
			maxLength: 50,
			description: computed(() => this.Resources.AGENT_S_NAME42642),
			ignoreFldSubmit: true,
		}).cloneFrom(values?.TableAgentName))
		this.stopWatchers.push(watch(() => this.TableAgentName.value, (newValue, oldValue) => this.onUpdate('agent.name', this.TableAgentName, newValue, oldValue)))

		this.AgentValPhotography = reactive(new modelFieldType.Image({
			id: 'AgentValPhotography',
			originId: 'ValPhotography',
			area: 'AGENT',
			field: 'PHOTOGRA',
			isFixed: true,
			description: computed(() => this.Resources.PHOTOGRAPHY38058),
		}).cloneFrom(values?.AgentValPhotography))
		this.stopWatchers.push(watch(() => this.AgentValPhotography.value, (newValue, oldValue) => this.onUpdate('agent.photography', this.AgentValPhotography, newValue, oldValue)))

		this.AgentValEmail = reactive(new modelFieldType.String({
			id: 'AgentValEmail',
			originId: 'ValEmail',
			area: 'AGENT',
			field: 'EMAIL',
			maxLength: 80,
			isFixed: true,
			description: computed(() => this.Resources.E_MAIL42251),
		}).cloneFrom(values?.AgentValEmail))
		this.stopWatchers.push(watch(() => this.AgentValEmail.value, (newValue, oldValue) => this.onUpdate('agent.email', this.AgentValEmail, newValue, oldValue)))

		this.ValProfit = reactive(new modelFieldType.Number({
			id: 'ValProfit',
			originId: 'ValProfit',
			area: 'PROPE',
			field: 'PROFIT',
			maxDigits: 9,
			decimalDigits: 2,
			isFixed: true,
			valueFormula: {
				stopRecalcCondition() { return false },
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: iif([PROPE->SOLD]==1,[PROPE->PRICE],0)
					return qApi.iif((this.ValSold.value ? 1 : 0)===1,this.ValPrice.value,0)
				},
				dependencyEvents: ['fieldChange:prope.sold', 'fieldChange:prope.price'],
				isServerRecalc: false,
				isEmpty: qApi.emptyN,
			},
			description: computed(() => this.Resources.PROFIT55910),
		}).cloneFrom(values?.ValProfit))
		this.stopWatchers.push(watch(() => this.ValProfit.value, (newValue, oldValue) => this.onUpdate('prope.profit', this.ValProfit, newValue, oldValue)))

		this.ValTax = reactive(new modelFieldType.Number({
			id: 'ValTax',
			originId: 'ValTax',
			area: 'PROPE',
			field: 'TAX',
			maxDigits: 2,
			decimalDigits: 2,
			isFixed: true,
			valueFormula: {
				stopRecalcCondition() { return false },
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: getCityTax([PROPE->CODPROPE])
					return qFunctions.getCityTax(this.ValCodprope.value)
				},
				dependencyEvents: ['fieldChange:prope.codprope'],
				isServerRecalc: false,
				isEmpty: qApi.emptyN,
			},
			description: computed(() => this.Resources.TAX37977),
		}).cloneFrom(values?.ValTax))
		this.stopWatchers.push(watch(() => this.ValTax.value, (newValue, oldValue) => this.onUpdate('prope.tax', this.ValTax, newValue, oldValue)))

		this.ValTitle = reactive(new modelFieldType.String({
			id: 'ValTitle',
			originId: 'ValTitle',
			area: 'PROPE',
			field: 'TITLE',
			maxLength: 50,
			description: computed(() => this.Resources.TITLE21885),
		}).cloneFrom(values?.ValTitle))
		this.stopWatchers.push(watch(() => this.ValTitle.value, (newValue, oldValue) => this.onUpdate('prope.title', this.ValTitle, newValue, oldValue)))

		this.ValPhoto = reactive(new modelFieldType.Image({
			id: 'ValPhoto',
			originId: 'ValPhoto',
			area: 'PROPE',
			field: 'PHOTO',
			description: computed(() => this.Resources.MAIN_PHOTO16044),
		}).cloneFrom(values?.ValPhoto))
		this.stopWatchers.push(watch(() => this.ValPhoto.value, (newValue, oldValue) => this.onUpdate('prope.photo', this.ValPhoto, newValue, oldValue)))

		this.ValDescript = reactive(new modelFieldType.MultiLineString({
			id: 'ValDescript',
			originId: 'ValDescript',
			area: 'PROPE',
			field: 'DESCRIPT',
			description: computed(() => this.Resources.DESCRIPTION07383),
		}).cloneFrom(values?.ValDescript))
		this.stopWatchers.push(watch(() => this.ValDescript.value, (newValue, oldValue) => this.onUpdate('prope.descript', this.ValDescript, newValue, oldValue)))
	}

	/**
	 * Creates a clone of the current QFormPropertyViewModel instance.
	 * @returns {QFormPropertyViewModel} A new instance of QFormPropertyViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodprope'

	get QPrimaryKey() { return this.ValCodprope.value }
	set QPrimaryKey(value) { this.ValCodprope.updateValue(value) }
}
