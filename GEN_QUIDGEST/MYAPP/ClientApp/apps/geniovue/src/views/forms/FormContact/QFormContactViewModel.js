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
			name: 'CONTACT',
			area: 'CONTA',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_Contact',
				updateFilesTickets: 'UpdateFilesTicketsContact',
				setFile: 'SetFileContact'
			}
		})

		/** The primary key. */
		this.ValCodconta = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodconta',
			originId: 'ValCodconta',
			area: 'CONTA',
			field: 'CODCONTA',
			description: '',
		}).cloneFrom(values?.ValCodconta))
		this.stopWatchers.push(watch(() => this.ValCodconta.value, (newValue, oldValue) => this.onUpdate('conta.codconta', this.ValCodconta, newValue, oldValue)))

		/** The used foreign keys. */
		this.ValCodprope = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodprope',
			originId: 'ValCodprope',
			area: 'CONTA',
			field: 'CODPROPE',
			relatedArea: 'PROPE',
			description: '',
		}).cloneFrom(values?.ValCodprope))
		this.stopWatchers.push(watch(() => this.ValCodprope.value, (newValue, oldValue) => this.onUpdate('conta.codprope', this.ValCodprope, newValue, oldValue)))

		/** The remaining form fields. */
		this.ValDate = reactive(new modelFieldType.Date({
			id: 'ValDate',
			originId: 'ValDate',
			area: 'CONTA',
			field: 'DATE',
			description: computed(() => this.Resources.DATE18475),
		}).cloneFrom(values?.ValDate))
		this.stopWatchers.push(watch(() => this.ValDate.value, (newValue, oldValue) => this.onUpdate('conta.date', this.ValDate, newValue, oldValue)))

		this.TablePropeTitle = reactive(new modelFieldType.String({
			type: 'Lookup',
			id: 'TablePropeTitle',
			originId: 'ValTitle',
			area: 'PROPE',
			field: 'TITLE',
			maxLength: 50,
			description: computed(() => this.Resources.TITLE21885),
			ignoreFldSubmit: true,
		}).cloneFrom(values?.TablePropeTitle))
		this.stopWatchers.push(watch(() => this.TablePropeTitle.value, (newValue, oldValue) => this.onUpdate('prope.title', this.TablePropeTitle, newValue, oldValue)))

		this.ValClient = reactive(new modelFieldType.String({
			id: 'ValClient',
			originId: 'ValClient',
			area: 'CONTA',
			field: 'CLIENT',
			maxLength: 50,
			description: computed(() => this.Resources.CLIENT_NAME39245),
		}).cloneFrom(values?.ValClient))
		this.stopWatchers.push(watch(() => this.ValClient.value, (newValue, oldValue) => this.onUpdate('conta.client', this.ValClient, newValue, oldValue)))

		this.ValEmail = reactive(new modelFieldType.String({
			id: 'ValEmail',
			originId: 'ValEmail',
			area: 'CONTA',
			field: 'EMAIL',
			maxLength: 80,
			description: computed(() => this.Resources.EMAIL_DO_CLIENTE30111),
		}).cloneFrom(values?.ValEmail))
		this.stopWatchers.push(watch(() => this.ValEmail.value, (newValue, oldValue) => this.onUpdate('conta.email', this.ValEmail, newValue, oldValue)))

		this.ValPhone = reactive(new modelFieldType.String({
			id: 'ValPhone',
			originId: 'ValPhone',
			area: 'CONTA',
			field: 'PHONE',
			maxLength: 14,
			maskType: 'MP',
			maskFormat: '+000 000000000',
			maskRequired: '+000 000000000',
			description: computed(() => this.Resources.PHONE_NUMBER20774),
		}).cloneFrom(values?.ValPhone))
		this.stopWatchers.push(watch(() => this.ValPhone.value, (newValue, oldValue) => this.onUpdate('conta.phone', this.ValPhone, newValue, oldValue)))

		this.ValDescript = reactive(new modelFieldType.MultiLineString({
			id: 'ValDescript',
			originId: 'ValDescript',
			area: 'CONTA',
			field: 'DESCRIPT',
			blockWhen: {
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				fnFormula(params)
				{
					// Formula: isEmptyC([CONTA->PHONE])
					return (this.ValPhone.value === '')
				},
				dependencyEvents: ['fieldChange:conta.phone'],
				isServerRecalc: false,
				isEmpty: qApi.emptyC,
			},
			description: computed(() => this.Resources.DESCRIPTION07383),
		}).cloneFrom(values?.ValDescript))
		this.stopWatchers.push(watch(() => this.ValDescript.value, (newValue, oldValue) => this.onUpdate('conta.descript', this.ValDescript, newValue, oldValue)))

		this.ValVisit_date = reactive(new modelFieldType.Date({
			id: 'ValVisit_date',
			originId: 'ValVisit_date',
			area: 'CONTA',
			field: 'VISIT_DATE',
			description: computed(() => this.Resources.VISIT_DATE27188),
		}).cloneFrom(values?.ValVisit_date))
		this.stopWatchers.push(watch(() => this.ValVisit_date.value, (newValue, oldValue) => this.onUpdate('conta.visit_date', this.ValVisit_date, newValue, oldValue)))

		this.PropeValId = reactive(new modelFieldType.Number({
			id: 'PropeValId',
			originId: 'ValId',
			area: 'PROPE',
			field: 'ID',
			maxDigits: 5,
			decimalDigits: 0,
			isFixed: true,
			description: '',
		}).cloneFrom(values?.PropeValId))
		this.stopWatchers.push(watch(() => this.PropeValId.value, (newValue, oldValue) => this.onUpdate('prope.id', this.PropeValId, newValue, oldValue)))
	}

	/**
	 * Creates a clone of the current QFormContactViewModel instance.
	 * @returns {QFormContactViewModel} A new instance of QFormContactViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodconta'

	get QPrimaryKey() { return this.ValCodconta.value }
	set QPrimaryKey(value) { this.ValCodconta.updateValue(value) }
}
