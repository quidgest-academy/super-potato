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
			name: 'CTAX',
			area: 'CTAX',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_Ctax',
				updateFilesTickets: 'UpdateFilesTicketsCtax',
				setFile: 'SetFileCtax'
			}
		})

		/** The primary key. */
		this.ValCodctax = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodctax',
			originId: 'ValCodctax',
			area: 'CTAX',
			field: 'CODCTAX',
			description: '',
		}).cloneFrom(values?.ValCodctax))
		this.stopWatchers.push(watch(() => this.ValCodctax.value, (newValue, oldValue) => this.onUpdate('ctax.codctax', this.ValCodctax, newValue, oldValue)))

		/** The used foreign keys. */
		this.ValCodcity = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodcity',
			originId: 'ValCodcity',
			area: 'CTAX',
			field: 'CODCITY',
			relatedArea: 'CITY',
			description: '',
		}).cloneFrom(values?.ValCodcity))
		this.stopWatchers.push(watch(() => this.ValCodcity.value, (newValue, oldValue) => this.onUpdate('ctax.codcity', this.ValCodcity, newValue, oldValue)))

		/** The remaining form fields. */
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

		this.ValTax = reactive(new modelFieldType.Number({
			id: 'ValTax',
			originId: 'ValTax',
			area: 'CTAX',
			field: 'TAX',
			maxDigits: 3,
			decimalDigits: 2,
			description: computed(() => this.Resources.TAX37977),
		}).cloneFrom(values?.ValTax))
		this.stopWatchers.push(watch(() => this.ValTax.value, (newValue, oldValue) => this.onUpdate('ctax.tax', this.ValTax, newValue, oldValue)))
	}

	/**
	 * Creates a clone of the current QFormCtaxViewModel instance.
	 * @returns {QFormCtaxViewModel} A new instance of QFormCtaxViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodctax'

	get QPrimaryKey() { return this.ValCodctax.value }
	set QPrimaryKey(value) { this.ValCodctax.updateValue(value) }
}
