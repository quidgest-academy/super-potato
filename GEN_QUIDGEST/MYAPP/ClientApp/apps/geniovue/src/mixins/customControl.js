import _assignInWith from 'lodash-es/assignInWith'
import cloneDeep from 'lodash-es/cloneDeep'
import _debounce from 'lodash-es/debounce'
import _isArray from 'lodash-es/isArray'
import _isEmpty from 'lodash-es/isEmpty'
import _isUndefined from 'lodash-es/isUndefined'
import _merge from 'lodash-es/merge'
import _mergeWith from 'lodash-es/mergeWith'
import { computed, reactive, watch } from 'vue'

import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

import { systemInfo } from '@/systemInfo'
import getCustomControl from './custom-controls/customControlImport.js'
import listFunctions from './listFunctions.js'

/**
 * Mixin with properties and methods common to all kinds of special renderings
 */
const customControlMixin = {
	/**
	 * The controls belonging to each of the defined view modes
	 */
	customControls: {},

	/**
	 * Initializes each of the custom controls and adds them to the custom controls object
	 */
	setCustomControls()
	{
		const viewModes = this.viewModes ?? []

		for (const viewMode of viewModes)
		{
			const type = viewMode.type
			const control = getCustomControl(type, this, viewMode.order)

			if (control !== null)
				this.customControls[type] = control
		}
	},

	/**
	 * Adds the texts of all the used custom controls to the main texts object
	 */
	addCustomTexts()
	{
		for (const i in this.customControls)
		{
			const customTexts = this.customControls[i].texts

			if (_isEmpty(customTexts))
				continue

			if (_isEmpty(this.texts))
				this.texts = {}

			for (const j in customTexts)
				if (!this.texts[j])
					this.texts[j] = customTexts[j]
		}
	},

	/**
	 * Adds the handlers of all used custom controls to the base handlers of the special rendering
	 */
	setSpecificHandlers()
	{
		for (const viewMode of this.viewModes)
		{
			// Add generic handlers
			viewMode.handlers = this.handlers

			// Add specific handlers
			const control = this.customControls[viewMode.type]
			const additionalHandlers = control?.handlers

			if (typeof control?.setListeners === 'function')
				control.setListeners()

			if (_isEmpty(additionalHandlers))
				continue

			for (const i in additionalHandlers)
				control.addHandler(i, additionalHandlers[i])
		}
	},

	/**
	 * Initializes the specified view mode with the default properties
	 * @param {object} viewMode The view mode
	 * @param {boolean} isMultiple Whether or not the view mode is over a list
	 */
	initViewMode(viewMode, isMultiple = false)
	{
		const control = this.customControls[viewMode.type]

		viewMode.mappedValues = reactive([])

		// The readonly flag is used to make a specific view mode not editable, instead of the whole control
		// Necessary, for example, so the user can't move the markers of maps someplace where the coordinates can't be edited
		const readonly = !_isEmpty(control) ? control.checkIsReadonly(isMultiple) : false
		if (readonly)
			viewMode.readonly = readonly
		else
			viewMode.readonly = computed(() => this.readonly)

		const componentName = `q-${viewMode.type}`
		viewMode.componentName = componentName
		viewMode.containerId = `${componentName}-container${!_isEmpty(this.id) ? `-${this.id}` : ''}`
	},

	/**
	 * Hydrates the data of the mapped values, so it matches the data format expected by the components
	 */
	hydrateMappedValues()
	{
		const viewModes = this.viewModes ?? []

		for (const viewMode of viewModes)
		{
			const control = this.customControls[viewMode.type]
			if (!_isEmpty(control) && typeof control.hydrateValues === 'function')
				control.hydrateValues(viewMode)
		}
	},

	/**
	 * Returns an array with all the variables that are organized within groups.
	 * @param {object} viewMode The view mode
	 */
	flatGroupedVariables(viewMode)
	{
		const res = []

		for (const groupType in viewMode.groups)
		{
			const groupsOfType = viewMode.groups[groupType]

			for (const group of groupsOfType)
				for (const i in group)
					res.push(group[i])
		}

		return res
	},

	/**
	 * Hydrates the data of the style variables, so it matches the data format expected by the components
	 * @param {object} field The field that triggered this function call (if defined, only dependant variables will be updated)
	 */
	hydrateStyleVariables(field)
	{
		const viewModes = this.viewModes ?? []
		const fieldId = field ? `${field.area}.${field.field}` : ''

		for (const viewMode of viewModes)
		{
			for (const i in viewMode.styleVariables)
				this.hydrateStyleVariable(viewMode.styleVariables[i], field)

			const groupedStyleVariables = this.flatGroupedVariables(viewMode).filter((v) => v.type === 'STYLE')
			for (const variable of groupedStyleVariables)
				this.hydrateStyleVariable(variable, field)

			// Some variable types might need further hydration in some controls
			const control = this.customControls[viewMode.type]
			if (!_isEmpty(control) && typeof control.hydrateStyleVariables === 'function')
				control.hydrateStyleVariables(viewMode, fieldId)

			this.setExtraProperties(viewMode)
		}
	},

	/**
	 * Hydrates the data of a single style variable, so it matches the data format expected by the components
	 * @param {object} variable The style variable to hydrate)
	 * @param {object} field The field that triggered this function call
	 */
	hydrateStyleVariable(variable, field)
	{
		const fieldId = field ? `${field.area}.${field.field}` : ''

		if (variable.isMapped)
		{
			if (typeof field === 'object' && fieldId !== variable.source)
				return

			const modelValue = this.getModelValue(variable.source)
			if (!_isEmpty(modelValue))
				variable.rawValue = modelValue.value
		}

		variable.value = variable.rawValue
	},

	/**
	 * Compiles a list of all the change events necessary to update mapped style variables
	 * @returns A list of dependency events
	 */
	getStyleVariableDependencyEvents()
	{
		const viewModes = this.viewModes ?? []
		const dependencyEvents = []

		const addDependency = (variable) => {
			if (!variable.isMapped)
				return

			if (!_isEmpty(variable.source) && !dependencyEvents.includes(variable.source))
				dependencyEvents.push(`fieldChange:${variable.source.toLowerCase()}`)
		}

		for (const viewMode of viewModes)
		{
			for (const i in viewMode.styleVariables)
			{
				const variable = viewMode.styleVariables[i]
				addDependency(variable)
			}

			const groupedStyleVariables = this.flatGroupedVariables(viewMode).filter((v) => v.type === 'STYLE')
			for (const variable of groupedStyleVariables)
				addDependency(variable)
		}

		return dependencyEvents
	},

	/**
	 * Retrieves the value of the specified form field from the model (only for forms)
	 * @param {string} fieldIdentifier The identifier of the form field (ex: "TABLE_NAME.FIELD_NAME")
	 * @returns The value of the specified field in the model
	 */
	getModelValue(fieldIdentifier)
	{
		if (typeof fieldIdentifier !== 'string')
			return undefined

		const fieldData = fieldIdentifier.split('.')
		if (fieldData.length !== 2)
			return undefined

		const areaName = fieldData[0]
		const fieldId = fieldData[1]

		if (areaName === 'GLOB')
		{
			const tGlob = this.vueContext.model.tGlob
			if (typeof tGlob !== 'object')
				return undefined

			const globId = `val${fieldId.replace('_', '').toLowerCase()}`

			for (const i in tGlob)
			{
				if (globId === i.toLowerCase())
				{
					return {
						value: tGlob[i].value,
						rawData: tGlob[i].value,
						source: tGlob
					}
				}
			}
		}
		else
		{
			const model = this.vueContext.model
			if (typeof model !== 'object')
				return undefined

			for (const i in model)
			{
				if (model[i].area === areaName && model[i].field === fieldId)
				{
					return {
						value: model[i].value,
						rawData: model[i].value,
						source: model[i]
					}
				}
			}
		}

		return undefined
	},

	/**
	 * Sets any necessary extra properties in the specified view mode
	 * @param {object} viewMode The view mode
	 */
	setExtraProperties(viewMode)
	{
		const control = this.customControls[viewMode.type]

		if (!_isEmpty(control))
		{
			viewMode.props = control.getProps(viewMode)

			control.setGenericCustomProps(viewMode)
			if (typeof control.setCustomProperties === 'function')
				control.setCustomProperties(viewMode)
		}
	},

	/**
	 * Should be called after a mapped value dependency changes, to update the necessary properties.
	 */
	dependencyChanged()
	{
		(_debounce(() => {
			this.setMappingValues()
			this.hydrateMappedValues()
		}, 100))()
	},

	/**
	 * Should be called after a style dependency changes, to update the necessary properties.
	 * @param {object} dependency The dependency that triggered the change event
	 */
	styleDependencyChanged(dependency)
	{
		(_debounce(() => this.hydrateStyleVariables(dependency), 100))()
	},

	/**
	 * Sets the default properties for the image value.
	 * @param {object} mappedVal The mapped value object
	 * @param {object} image The image object
	 * @param {object} usesFullSizeImg Whether the control needs to use the full sized version of the image
	 */
	async setMappedImage(mappedVal, image, usesFullSizeImg = false)
	{
		if (image === null)
		{
			mappedVal.value = `${systemInfo.resourcesPath}no_img.png`
			mappedVal.previewData = `${systemInfo.resourcesPath}unknown.png`
		}
		else
		{
			const isMultipleValue = _isArray(image)
			const base64Img = isMultipleValue ? image.map((img) => genericFunctions.imageObjToSrc(img)) : genericFunctions.imageObjToSrc(image)
			mappedVal.value = base64Img

			// Compute dominant color from preview image only if necessary
			if (usesFullSizeImg && !isMultipleValue)
				mappedVal.dominantColor = await genericFunctions.computeColorPlaceholder(base64Img)
		}
	}
}

/**
 * Extends the given classes with properties specific for the special renderings pattern
 * @param {object} BaseControl The base control class
 * @param {object} TableListControl The table control class
 * @returns An object with the available special rendering types
 */
export default function getSpecialRenderingControls(BaseControl, TableListControl)
{
	/**
	 * The base class for the special rendering of input controls
	 */
	class FieldSpecialRenderingControl extends BaseControl
	{
		constructor(options, vueContext)
		{
			super({
				type: 'FieldSpecialRendering',
				viewModes: [],
				activeViewModeId: ''
			}, vueContext)

			_merge(this, options ?? {})
			_merge(this, customControlMixin)

			this.setCustomControls()
			this.addCustomTexts()

			this.stopWatcher = null
		}

		/**
		 * Initializes the necessary properties
		 * @param {boolean} isEditable Whether or not the control is editable
		 */
		async init(isEditable)
		{
			await super.init(isEditable)

			const viewModes = this.viewModes ?? []
			if (Array.isArray(viewModes) && !_isEmpty(viewModes))
				this.viewModes = cloneDeep(viewModes)

			for (const viewMode of this.viewModes)
				this.initViewMode(viewMode, false)

			if (typeof this.onDependencyChange !== 'function')
			{
				this.onDependencyChange = () => this.dependencyChanged()
				this.onStyleDependencyChange = (dependency) => this.styleDependencyChanged(dependency)

				// Watches for changes in the value of the implicitly mapped field
				this.stopWatcher = watch(() => this.modelFieldRef.value, this.onDependencyChange, { deep: true })

				const dependencyEvents = this.getDependencyEvents()
				const styleDependencyEvents = this.getStyleVariableDependencyEvents()

				// Watches for changes in the values of the other mapped fields in the form
				this.vueContext.internalEvents.offMany(dependencyEvents, this.onDependencyChange)
				this.vueContext.internalEvents.onMany(dependencyEvents, this.onDependencyChange)
				this.vueContext.internalEvents.offMany(styleDependencyEvents, this.onStyleDependencyChange)
				this.vueContext.internalEvents.onMany(styleDependencyEvents, this.onStyleDependencyChange)

				this.initCustomHandlers()
			}

			this.onDependencyChange()
			this.hydrateStyleVariables()
		}

		/**
		 * Initializes the custom handlers
		 */
		initCustomHandlers()
		{
			const handlers = {
				'update:model-value': (...args) => this.modelFieldRef.updateValue(...args)
			}

			_assignInWith(this.handlers, handlers, (objValue, srcValue) => _isUndefined(objValue) ? srcValue : objValue)

			this.setSpecificHandlers()
		}

		/**
		 * Sets the values of the mapping variables
		 */
		setMappingValues()
		{
			if (_isEmpty(this.modelFieldRef))
				return

			const viewModes = this.viewModes ?? []

			for (const viewMode of viewModes)
			{
				const variables = viewMode.mappingVariables
				const implicitVar = viewMode.implicitVariable
				const currentField = this.modelFieldRef

				viewMode.mappedValues.length = 0

				// Initializes the mapped values object
				const mappedValues = reactive({})

				// Maps the implicit variable to the current field
				const modelValue = this.getModelValue(`${currentField.area}.${currentField.field}`)
				mappedValues[implicitVar] = viewMode.implicitIsMultiple ? [modelValue] : modelValue

				for (const i in variables)
				{
					const variable = variables[i]

					if (!Array.isArray(variable.sources) || variable.sources.length === 0)
						continue

					if (variable.allowsMultiple)
					{
						const values = []
						for (const source of variable.sources)
							values.push(this.getModelValue(source))
						mappedValues[i] = values
					}
					else
						mappedValues[i] = this.getModelValue(variable.sources[0])
				}

				viewMode.mappedValues.push(mappedValues)
				this.setExtraProperties(viewMode)
			}
		}

		/**
		 * Compiles a list of all the field change events from which the special rendering depends
		 * @returns A list of dependency events
		 */
		getDependencyEvents()
		{
			const viewModes = this.viewModes ?? []
			const dependencyEvents = []

			const addDependency = (variable) => {
				const sources = variable.sources

				for (const source of sources)
					if (!_isEmpty(source) && !dependencyEvents.includes(source))
						dependencyEvents.push(`fieldChange:${source.toLowerCase()}`)
			}

			for (const viewMode of viewModes)
			{
				const variables = viewMode.mappingVariables

				for (const i in variables)
					addDependency(variables[i])

				const groupedMappingVariables = this.flatGroupedVariables(viewMode).filter((v) => v.type === 'MAP')
				for (const variable of groupedMappingVariables)
					addDependency(variable)
			}

			return dependencyEvents
		}

		destroy()
		{
			if(typeof super.destroy === 'function')
				super.destroy()

			if(this.stopWatcher)
				this.stopWatcher()
			this.stopWatcher = null
		}
	}

	/**
	 * The base class for the special rendering of multiple records controls (like tables)
	 */
	class TableSpecialRenderingControl extends TableListControl
	{
		constructor(options, vueContext, store)
		{
			super({
				type: 'TableSpecialRendering',
				// TODO: Create a "view mode manager" to encapsulate this logic
				activeViewModeId: Array.isArray(options.viewModes) && options.viewModes.length > 0 ? options.activeViewMode ? options.activeViewMode : options.viewModes[0].id : ''
			}, vueContext, store)

			_mergeWith(this, options ?? {}, genericFunctions.mergeOptions)
			_merge(this, customControlMixin)

			this.setCustomControls()
			this.addCustomTexts()

			this.stopWatcher = null
		}

		/**
		 * Initializes the necessary properties
		 * @param {boolean} isEditable Whether or not the control is editable
		 */
		async init(isEditable)
		{
			await super.init(isEditable)

			const viewModes = !_isEmpty(this.viewModes) ? this.viewModes : []
			if (Array.isArray(viewModes) && !_isEmpty(viewModes))
				this.viewModes = cloneDeep(viewModes)

			for (const viewMode of this.viewModes)
				this.initViewMode(viewMode, true)

			if (typeof this.onDependencyChange !== 'function')
			{
				this.onDependencyChange = () => this.dependencyChanged()
				this.onStyleDependencyChange = (dependency) => this.styleDependencyChanged(dependency)

				this.onViewModeChange = (viewModes) => {
					if (!Array.isArray(viewModes))
						return

					for (const viewMode of viewModes)
					{
						const mode = this.viewModes.find((v) => v.id === viewMode.id)
						mode.order = viewMode.order
						mode.visible = viewMode.visible
					}
				}

				// Watches for changes in the view mode (ex: changing from list view to alternative view)
				this.stopWatcher = watch(() => this.viewModes, (viewModes) => this.onViewModeChange(viewModes), { deep: true })

				if (!_isEmpty(this.vueContext.internalEvents))
				{
					const dependencyEvents = this.getStyleVariableDependencyEvents()

					// Watches for changes in the values of the other mapped fields in the form
					this.vueContext.internalEvents.offMany(dependencyEvents, this.onStyleDependencyChange)
					this.vueContext.internalEvents.onMany(dependencyEvents, this.onStyleDependencyChange)
				}

				this.setSpecificHandlers()
			}

			this.onViewModeChange(this.viewModes)
			this.hydrateStyleVariables()
		}

		/**
		 * Performs additional init operations after the table data is ready.
		 */
		initData()
		{
			// Performs additional init operations including checking for insert conditions
			super.initData()

			// Remove previous watcher, if it exists
			this.unwatchData?.()

			// Watches for changes in the rows and columns (ex: changing page or hiding a column)
			this.unwatchData = watch(
				[() => this.rows, () => this.columns],
				this.onDependencyChange,
				{ deep: true, immediate: true }
			)
		}

		/**
		 * Sets the values of the mapping variables
		 */
		setMappingValues()
		{
			const viewModes = this.viewModes ?? []

			for (const viewMode of viewModes)
			{
				// Nothing to do for the list
				if (viewMode.id === 'LIST')
					continue

				viewMode.mappedValues.length = 0

				// TODO: when rows are null
				this.rows.forEach((row) => {
					const mappedValues = reactive({
						rowKey: row.rowKey,
						btnPermission: row.btnPermission ?? {},
						actionVisibility: row.actionVisibility ?? {},
						actionDisability: row.actionDisability ?? {}
					})

					this.columns.forEach((column) => {
						if (_isEmpty(column) || _isEmpty(column.area))
							return

						const columnName = `${column.area}.${column.field}`

						// Find all the variables that this column is mapped to
						const mappedVariables = Object.keys(viewMode.mappingVariables).reduce((r, e) => {
							if (viewMode.mappingVariables[e].sources.includes(columnName))
								r.push(e)
							return r
						}, [])

						mappedVariables.forEach((variable) => {
							const rowColumn = row.Fields?.columns?.[column.order - 1]
							const control = this.customControls[viewMode.type]
							const value = listFunctions.getCellValueDisplay(undefined, row, column)
							const mappedVal = reactive({
								value,
								rawData: listFunctions.getCellValue(row, column),
								bgColor: rowColumn?.backgroundColor,
								textColor: rowColumn?.foregroundColor,
								source: column,
								previewData: null
							})

							if (column.dataType === 'Image')
								this.setMappedImage(mappedVal, value, control.usesFullSizeImg)

							if (viewMode.mappingVariables[variable].allowsMultiple)
							{
								if (!mappedValues[variable])
									mappedValues[variable] = []

								mappedValues[variable].push(mappedVal)
							}
							else
								mappedValues[variable] = mappedVal
						})
					})

					viewMode.mappedValues.push(mappedValues)
				})

				this.setExtraProperties(viewMode)
			}
		}

		destroy()
		{
			if(typeof super.destroy === 'function')
				super.destroy()

			if(this.unwatchData)
				this.unwatchData()
			this.unwatchData = null

			if(this.stopWatcher)
				this.stopWatcher()
			this.stopWatcher = null
		}
	}

	return {
		FieldSpecialRenderingControl,
		TableSpecialRenderingControl
	}
}
