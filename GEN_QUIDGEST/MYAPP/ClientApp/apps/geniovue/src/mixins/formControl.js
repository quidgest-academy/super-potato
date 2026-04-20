import _assignIn from 'lodash-es/assignIn'
import _forEach from 'lodash-es/forEach'
import _isEmpty from 'lodash-es/isEmpty'

import { triggerEvents } from '@quidgest/clientapp/constants/enums'
import formFunctions from './formFunctions.js'
import FormViewModelBase from './formViewModelBase.js'
import eventBus from '@quidgest/clientapp/plugins/eventBus'

export class FormControlButton
{
	constructor()
	{
		this.id = undefined
		this.icon = undefined
		this.type = undefined
		this.text = ''
		this.style = ''
		this.showInHeader = false
		this.showInFooter = false
		this.isActive = false
		this.isVisible = false
		this.disabled = false
		this.action = () => false
	}
}

export class FormControlButtons
{
	constructor()
	{
		this.changeToShow = new FormControlButton()
		this.changeToEdit = new FormControlButton()
		this.changeToDuplicate = new FormControlButton()
		this.changeToDelete = new FormControlButton()
		this.changeToInsert = new FormControlButton()
	}
}

export class FormControl
{
	constructor(_vueContext)
	{
		this.vueContext = _vueContext
		// The form UI components visibility.
		this.uiComponents = {
			header: true,
			headerButtons: true,
			footer: true
		}
		this.triggerIntervalIds = []
		this.initialized = false
		this.currentController = null
	}

	async init(initTabs, isEditable, initControls = true)
	{
		if (typeof initTabs !== 'boolean')
			initTabs = true

		this.isEditable = typeof isEditable === 'boolean' ? isEditable : true
		this.initModel()

		if (initControls)
		{
			await this.initControls(initTabs)
			this.initBtns()
		}

		this.initTriggers()
		this.initialized = true

		if (this.vueContext.isNested && this.vueContext.formInfo.mode === 'NEW')
			this.vueContext.$eventHub.emit('new-extended-record', this.vueContext.primaryKeyValue)

		if (typeof this.vueContext.afterLoad === 'function')
			this.vueContext.afterLoad()
	}

	initModel()
	{
		if (this.vueContext.model instanceof FormViewModelBase)
			this.vueContext.model.initFieldsValueFormula()
	}

	async initControls(initTabs)
	{
		if (!this.vueContext.controls)
			return

		const promises = []

		_forEach(this.vueContext.controls, (ctrl) => {
			if (initTabs || ctrl.type !== 'Tabs')
				promises.push(ctrl.init(this.isEditable))
		})

		await Promise.all(promises)
	}

	async initTabs()
	{
		if (!this.vueContext.controls)
			return

		const promises = []

		_forEach(this.vueContext.controls, (ctrl) => {
			if (ctrl.type === 'Tabs')
				promises.push(ctrl.init(this.isEditable))
		})

		await Promise.all(promises)
	}

	calcFieldsFormulas()
	{
		if (this.vueContext.model instanceof FormViewModelBase)
			this.vueContext.model.calcFieldsFormulas()
	}

	calcShowWhenFormulas()
	{
		this.vueContext.internalEvents.emit('CALC_SHOW_WHEN_FORMULAS')
	}

	calcBlockWhenFormulas()
	{
		this.vueContext.internalEvents.emit('CALC_BLOCK_WHEN_FORMULAS')
	}

	calcFillWhenFormulas()
	{
		if (this.vueContext.model instanceof FormViewModelBase)
			this.vueContext.model.calcFillWhenFormulas()
	}

	calcAllInterfaceFormulas()
	{
		this.calcShowWhenFormulas()
		this.calcBlockWhenFormulas()
		this.calcFillWhenFormulas()
	}

	calcAllFormulas()
	{
		this.calcFieldsFormulas()
		this.calcAllInterfaceFormulas()
	}

	setupBtns()
	{
		// Nested forms will not change the buttons available on the side bar.
		if (this.vueContext.isNested)
			return
		else if (this.vueContext.isHomePage)
		{
			this.clearBtns()
			return
		}

		eventBus.emit('changed-form-buttons', this.vueContext.formButtonSections)
	}

	initBtns()
	{
		if (!this.vueContext.formButtons)
			return

		this.setupBtns()
		this.vueContext.internalEvents.on('form-buttons-change', () => this.setupBtns())
	}

	initTriggers()
	{
		if (typeof this.vueContext.getTriggers !== 'function')
			return

		// Get all the periodic triggers.
		const triggers = this.vueContext.getTriggers(triggerEvents.periodic)

		// Ensure there are no leftover triggers.
		this.destroyTriggers()

		// Schedule execution of periodic events.
		_forEach(triggers, (t) => {
			const intervalId = setInterval(() => formFunctions.executeTriggerAction(t), t.periodicity * 1000)
			this.triggerIntervalIds.push(intervalId)
		})
	}

	clearBtns()
	{
		// Nested forms will not change the buttons available on the side bar.
		if (this.vueContext.isNested)
			return

		eventBus.emit('changed-form-buttons', {})
	}

	/**
	 * Defines the form's anchors.
	 */
	setFormAnchors(controlsTree, open = false)
	{
		eventBus.emit('changed-form-anchors', controlsTree)
		if (open)
			eventBus.emit('open-sidebar-on-tab', 'form-anchors-tab')
	}

	/**
	 * Defines if the form is in editable mode.
	 * In addition to being locked/unlocked, some controls may be invisible in non-editable modes.
	 * @param {boolean} isEditableForm
	 */
	setFormModeBlockAndVisibility(isEditable)
	{
		this.isEditable = typeof isEditable === 'boolean' ? isEditable : true
		if (this.vueContext.controls)
		{
			_forEach(this.vueContext.controls, (ctrl) => {
				ctrl.setFormModeBlockAndVisibility?.(this.isEditable)
			})
		}
	}

	/**
	 * Change the form UI components visibility.
	 * @param {object} options { header, headerButtons, footer }
	 */
	setUIComponents(options)
	{
		_assignIn(this.uiComponents, options)
	}

	/**
	 * Setup of the various listeners for changes to the DB, which will update the respective list whenever changes occur.
	 */
	initListOnDBChangeEvent()
	{
		for (const tableName of this.vueContext.tableFields ?? [])
		{
			const table = this.vueContext.controls[tableName]
			if (_isEmpty(table))
				continue

			// We give an id to the list reload method, so the listener can be correctly removed later.
			/**
			 * Reloads the data of the specified list.
			 * @param {array} dirtyFields A list of dirty fields
			 * @param {string} typeSourceForm The type of source form that originates the change event
			 * @returns {Promise}
			 */
			table.reloadList = (dirtyFields, typeSourceForm) =>
			{
				// Pop-up forms, after closing, perform navigation through the route, so it
				// becomes unnecessary to update the list because the entire form will already be reloaded.
				if(typeSourceForm === 'popup')
					return Promise.resolve(true)

				return this.vueContext.reloadList(tableName, dirtyFields)
			}
			this.vueContext.internalEvents.onMany(table.internalEvents, table.reloadList)
			eventBus.onMany(table.globalEvents, table.reloadList)
		}
	}

	/**
	 * Remove of the various listeners for changes to the DB, which will update the respective list whenever changes occur.
	 */
	removeListOnDBChangeEvent()
	{
		for (const tableName of this.vueContext.tableFields ?? [])
		{
			const table = this.vueContext.controls[tableName]
			if (!_isEmpty(table))
			{
				this.vueContext.internalEvents?.offMany(table.internalEvents, table.reloadList)
				eventBus.offMany(table.globalEvents, table.reloadList)
			}
		}
	}

	destroy()
	{
		// If there's a request pending, cancel it
		this.currentController?.abort()
		this.currentController = null

		this.clearBtns()

		this.removeListOnDBChangeEvent()

		if (this.vueContext.model instanceof FormViewModelBase && this.vueContext.formInfo?.type !== 'virtual')
			this.vueContext.model.destroy()

		const controlsIds = Object.keys(this.vueContext.controls ?? {})
		controlsIds.forEach((controlId) => {
			if (typeof this.vueContext.controls[controlId].destroy === 'function')
				this.vueContext.controls[controlId].destroy()
			this.vueContext.controls[controlId] = null
			delete this.vueContext.controls[controlId]
		})

		this.destroyTriggers()

		this.vueContext.internalEvents?.removeAllListeners()
		this.vueContext.internalEvents = null
		this.vueContext.model = null
		this.vueContext.controls = null
		this.vueContext = null
	}

	destroyTriggers()
	{
		if (_isEmpty(this.triggerIntervalIds))
			return

		_forEach(this.triggerIntervalIds, (intervalId) => clearInterval(intervalId))

		this.triggerIntervalIds.length = 0
	}
}

export default {
	FormControl,
	FormControlButtons,
	FormControlButton
}
