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
			name: 'ALBUM',
			area: 'PHOTO',
			actions: {
				recalculateFormulas: 'RecalculateFormulas_Album',
				updateFilesTickets: 'UpdateFilesTicketsAlbum',
				setFile: 'SetFileAlbum'
			}
		})

		/** The primary key. */
		this.ValCodphoto = reactive(new modelFieldType.PrimaryKey({
			id: 'ValCodphoto',
			originId: 'ValCodphoto',
			area: 'PHOTO',
			field: 'CODPHOTO',
			description: '',
		}).cloneFrom(values?.ValCodphoto))
		this.stopWatchers.push(watch(() => this.ValCodphoto.value, (newValue, oldValue) => this.onUpdate('photo.codphoto', this.ValCodphoto, newValue, oldValue)))

		/** The used foreign keys. */
		this.ValCodprope = reactive(new modelFieldType.ForeignKey({
			id: 'ValCodprope',
			originId: 'ValCodprope',
			area: 'PHOTO',
			field: 'CODPROPE',
			relatedArea: 'PROPE',
			description: '',
		}).cloneFrom(values?.ValCodprope))
		this.stopWatchers.push(watch(() => this.ValCodprope.value, (newValue, oldValue) => this.onUpdate('photo.codprope', this.ValCodprope, newValue, oldValue)))

		/** The remaining form fields. */
		this.ValPhoto = reactive(new modelFieldType.Image({
			id: 'ValPhoto',
			originId: 'ValPhoto',
			area: 'PHOTO',
			field: 'PHOTO',
			description: computed(() => this.Resources.PHOTO51874),
		}).cloneFrom(values?.ValPhoto))
		this.stopWatchers.push(watch(() => this.ValPhoto.value, (newValue, oldValue) => this.onUpdate('photo.photo', this.ValPhoto, newValue, oldValue)))

		this.ValTitle = reactive(new modelFieldType.String({
			id: 'ValTitle',
			originId: 'ValTitle',
			area: 'PHOTO',
			field: 'TITLE',
			maxLength: 50,
			description: computed(() => this.Resources.TITLE21885),
		}).cloneFrom(values?.ValTitle))
		this.stopWatchers.push(watch(() => this.ValTitle.value, (newValue, oldValue) => this.onUpdate('photo.title', this.ValTitle, newValue, oldValue)))

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
	}

	/**
	 * Creates a clone of the current QFormAlbumViewModel instance.
	 * @returns {QFormAlbumViewModel} A new instance of QFormAlbumViewModel
	 */
	clone()
	{
		return new ViewModel(this.vueContext, { callbacks: this.externalCallbacks }, this)
	}

	static QPrimaryKeyName = 'ValCodphoto'

	get QPrimaryKey() { return this.ValCodphoto.value }
	set QPrimaryKey(value) { this.ValCodphoto.updateValue(value) }
}
