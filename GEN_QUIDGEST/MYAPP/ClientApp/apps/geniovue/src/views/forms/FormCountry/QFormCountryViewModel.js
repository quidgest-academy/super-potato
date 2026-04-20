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
			name: 'COUNTRY',
			area: 'COUNT',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_Country',
				updateFilesTickets: 'UpdateFilesTicketsCountry',
				setFile: 'SetFileCountry'
			}
		})

		/** The primary key. */
		this.ValCodcount = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodcount',
			originId: 'ValCodcount',
			area: 'COUNT',
			field: 'CODCOUNT',
			description: '',
		}).cloneFrom(values?.ValCodcount))
		this.stopWatchers.push(watch(() => this.ValCodcount.value, (newValue, oldValue) => this.onUpdate('count.codcount', this.ValCodcount, newValue, oldValue)))

		/** The remaining form fields. */
		this.ValCountry = reactive(new modelFieldType.String({
			id: 'ValCountry',
			originId: 'ValCountry',
			area: 'COUNT',
			field: 'COUNTRY',
			maxLength: 50,
			description: computed(() => this.Resources.COUNTRY64133),
		}).cloneFrom(values?.ValCountry))
		this.stopWatchers.push(watch(() => this.ValCountry.value, (newValue, oldValue) => this.onUpdate('count.country', this.ValCountry, newValue, oldValue)))
	}

	/**
	 * Creates a clone of the current QFormCountryViewModel instance.
	 * @returns {QFormCountryViewModel} A new instance of QFormCountryViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodcount'

	get QPrimaryKey() { return this.ValCodcount.value }
	set QPrimaryKey(value) { this.ValCodcount.updateValue(value) }
}
