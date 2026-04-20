import { mapState } from 'pinia'

import _find from 'lodash-es/find'
import _foreach from 'lodash-es/forEach'
import _get from 'lodash-es/get'
import _isEmpty from 'lodash-es/isEmpty'

import { useSystemDataStore } from '@quidgest/clientapp/stores'
import { useGenericLayoutDataStore } from '@quidgest/clientapp/stores'

import netAPI from '@quidgest/clientapp/network'
import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
import listFunctions from '@/mixins/listFunctions.js'
import qEnums from '@quidgest/clientapp/constants/enums'

/*****************************************************************
 * This mixin aggregates operations over lists, which can be     *
 * reused in menus and form components.                          *
 *****************************************************************/
export default {
	computed: {
		...mapState(useSystemDataStore, [
			'system'
		]),

		...mapState(useGenericLayoutDataStore, [
			'layoutConfig'
		])
	},

	methods: {
		/**
		 * Open support form of timeline item.
		 * @param {object} emittedAction The TimelineItem data
		 */
		timelineOpenForm(emittedAction)
		{
			if (emittedAction?.SupportForm)
			{
				const options = { isPopup: emittedAction.IsPopupForm }
				this.navigateToForm(emittedAction.SupportForm, 'SHOW', emittedAction.Identifier, options)
			}
		},

		/**
		 * Set property in table object
		 * @param {object} listConf The list configuration
		 * @param {array} propertyPath Property / sub-property names
		 * @param {object} value Property value
		 */
		setProperty(listConf, propertyPath, value)
		{
			if (typeof listConf !== 'object' || listConf === null)
				return

			// Must have propery name (and sub-property names if any) as an array
			if (!Array.isArray(propertyPath) || propertyPath.length === 0)
				return

			const length = propertyPath.length

			// Start with reference to the table model object
			let ref = listConf

			// Set reference to the property one level above the property being set
			for (let idx = 0; idx < length - 1; idx++)
				ref = ref[propertyPath[idx]]

			// From this reference, set the property
			ref[propertyPath[length - 1]] = value
		},

		/**
		 * Set sub-property in array in table object where property has value
		 * @param {object} listConf The list configuration
		 * @param {string} arrayName
		 * @param {string} propertyName
		 * @param {string} propertyValue
		 * @param {string} key
		 * @param {object} value
		 * @param {object} otherValue
		 */
		setArraySubPropWhere(listConf, arrayName, propertyName, propertyValue, key, value, otherValue)
		{
			for (const idx in listConf[arrayName])
			{
				const elem = listConf[arrayName][idx]
				if (elem[propertyName] === propertyValue)
					listConf[arrayName][idx][key] = value
				else if (otherValue !== undefined && otherValue !== null)
					listConf[arrayName][idx][key] = otherValue
			}
		},

		/**
		 * Run group action on selected rows
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		onTableListRowGroupAction(listConf, eObj)
		{
			const params = {}

			// Set selected ids
			Reflect.set(params, 'ids', Object.keys(eObj.rowsSelected))

			// Set all selected param
			params.allSelected = eObj.allSelected === 'true' || eObj.allSelected === true

			// Add table configuration
			params.tableConfiguration = listFunctions.getTableConfiguration(listConf)

			switch (eObj.action.params.type)
			{
				case 'menu':
					return netAPI.postData(
						listConf.controller,
						`${listConf.action}_Selections`,
						params,
						() => {
							// Store temporary table configuration for when returning
							listConf.setListReturnControl(null, true)

							// Go to follow-up menu list
							// Use the method defined in the action object. The same method normally used to navigate to menu lists.
							eObj.action.params.action(listConf, eObj.action, listFunctions.getRowByKeyPath(listConf.rows, params.ids[0]))
						},
						undefined,
						undefined,
						this.navigationId)
				case 'form':
					return netAPI.postData(
						listConf.controller,
						`${listConf.action}_Selections`,
						params,
						() => {
							// Store temporary table configuration for when returning
							listConf.setListReturnControl(null, true)

							// Go to follow-up form
							if (params.ids.length > 0)
							{
								const routeOptions = {}
								if (Number.isInteger(eObj.action.params.goBack))
									Reflect.set(routeOptions, 'goBack', eObj.action.params.goBack)
								this.navigateToForm(eObj.action.params.formName, eObj.action.params.mode, params.ids[0], routeOptions)
							}
						},
						undefined,
						undefined,
						this.navigationId)
				case 'routine':
					// Call routine
					eObj.action.params.actionRoutine(params)
					break
				case 'qsign':
					eObj.action.params.actionRoutine(params)
					break
				case 'report':
					return netAPI.postData(
						listConf.controller,
						`${listConf.action}_Selections`,
						params,
						// Go to follow-up report
						() => this.navigateToReport(eObj.action.params.baseArea, eObj.action.name, { allSelected: params.allSelected }),
						undefined,
						undefined,
						this.navigationId)
				default:
					if (typeof eObj.action.params.action === 'function')
						eObj.action.params.action(params)
					break
			}
		},

		/**
		 *
		 * @param {object} listConf The list configuration
		 * @param {object} eObj
		 */
		async onTableListExecuteAction(listConf, eObj)
		{
			// Download file
			if (!_isEmpty(eObj) && eObj.action === 'download' && eObj.ticket && eObj.fileName)
			{
				const area = eObj.area ?? listConf.config.tableAlias

				netAPI.getFile(
					area,
					eObj.ticket,
					eObj.viewType,
					this.navigationId)
				return
			}

			// Fetch image for preview
			if (!_isEmpty(eObj) && eObj.action === 'preview-image' && eObj.ticket)
			{
				const params = {
					ticket: eObj.ticket,
					formIdentifier: ''
				}

				netAPI.retrieveImage(
					eObj.area,
					params,
					(data) => {
						if (eObj.callback)
							eObj.callback(data)
					})
				return
			}

			// Insert in multiforms
			if (listConf.type === 'Multiform' && eObj.name === 'insert')
			{
				const addAction = _find(listConf.config.generalActions, (act) => act.id === 'insert')
				eObj.controller = listConf.config.tableAlias
				eObj.action = `${addAction.params.formName}_NEW_GET`
				listConf.onTableListInsertRow(eObj)
				return
			}

			let actionCfg = null
			let actionId = null

			// If custom action is already given
			if (eObj.action)
			{
				actionCfg = eObj.action
				actionId = eObj.action.id
			}
			else
				actionId = eObj.id

			// If the action is not defined, do nothing
			if (!actionId)
				return

			// Find the action by it's id
			// CRUD
			if (!actionCfg)
				actionCfg = _find(listConf.config.crudActions, (act) => act.id === actionId)
			if (!actionCfg)
				actionCfg = _find(listConf.config.generalActions, (act) => act.id === actionId)
			// Custom action
			if (!actionCfg)
				actionCfg = _find(listConf.config.customActions, (act) => act.id === actionId)
			// Row click action
			if (!actionCfg && listConf.config.rowClickAction && listConf.config.rowClickAction.id === actionId)
				actionCfg = listConf.config.rowClickAction
			// General custom action
			if (!actionCfg)
				actionCfg = _find(listConf.config.generalCustomActions, (act) => act.id === actionId)

			if (!actionCfg || !actionCfg.params || typeof actionCfg.params.action !== 'function')
				return

			// Get row key and row key path
			let rowKey
			let rowKeyPath
			if (eObj.rowKeyPath && Array.isArray(eObj.rowKeyPath))
			{
				rowKey = eObj.rowKeyPath[eObj.rowKeyPath.length - 1]
				rowKeyPath = eObj.rowKeyPath
			}
			else if (eObj.rowKey)
			{
				rowKey = eObj.rowKey
				rowKeyPath = [eObj.rowKey]
			}

			// Find row by row key path
			const row = listFunctions.getRowByKeyPath(listConf.rows, rowKeyPath),
				historyEntries = []

			if (listConf.type !== 'TreeList')
			{
				historyEntries.push({
					key: (listConf.config.tableAlias || '').toLowerCase(),
					value: rowKey
				})
			}

			// Prevent the action if it is not allowed
			// Added to account for actions triggered by short-cut keys
			if (!listFunctions.actionIsAllowed(actionCfg, row?.btnPermission, listConf.config.permissions, listConf.readonly))
				return

			// Check for row specific visibility conditions
			if (typeof actionCfg.checkIsVisible === 'function' && !actionCfg.checkIsVisible(row))
				return

			// Check if the action is a row-specific action
			const crudAction = _find(listConf.config.crudActions, (act) => act.id === actionId)
			const insertAction = _find(listConf.config.generalActions, (act) => act.id === actionId && act.id === 'insert')
			const rowSpecificAction = crudAction || insertAction

			if (listConf.type === 'TreeList')
			{
				// Set the right tableAlias in the navigation entry
				if (!_isEmpty(row?.Area))
				{
					_foreach(_get(listConf.config.treeListDefinitions, row.Area, []), (branchAreaKey, branchArea) => {
						historyEntries.push({
							key: branchArea,
							value: branchAreaKey(row)
						})
					})
				}

				// BEGIN: Get form name
				let rowFormName = null

				// It's needed to know if it's a CRUD action, because the action form name must be filled with the value from the "row"
				if (crudAction)
					rowFormName = row.Form

				// Shows correct form to open when inserting a record
				if (insertAction)
					rowFormName = listConf.getInsertFormName(row)

				// If the row has a form defined it overrides the action form
				if (rowFormName && rowFormName.length > 0)
					actionCfg.params.formName = rowFormName
				// BEGIN: Get form name

				// FOR: tree table select row on return
				// Tree tables: store path of row keys
				if (row === undefined || row === null)
					listConf.config.rowKeyToScroll = []
				else
					listConf.config.rowKeyToScroll = rowKeyPath
			}

			// If going to a form or sub menu list, store configuration to use when returning.
			// Calendar currently does not support this.
			const storeTableConfig = (rowSpecificAction || actionCfg.params.isRoute) && listConf.activeViewModeId !== 'CALENDAR'

			listConf.setListReturnControl(row, storeTableConfig, eObj.returnElement)
			this.setParamValue({
				navigationId: this.navigationId,
				key: 'anchor',
				value: listConf.id
			})

			// If the before execute function is defined, execute it and check if we can perform the action on the list.
			if (typeof actionCfg.params.canExecuteAction === 'function')
			{
				const canContinueExecution = await actionCfg.params.canExecuteAction()
				if (!canContinueExecution)
					return
			}

			actionCfg.params.action(listConf, actionCfg, row, historyEntries)
		},

		/**
		 * Execute action from 'MC' menu
		 * @param {object} listConf The list configuration
		 * @param {object} actionName
		 * @param {string} id
		 */
		tableListMCAction(listConf, actionName, id)
		{
			const actionMC = _find(listConf.config.MCActions, (act) => act.name === actionName)
			const row = _find(listConf.rows, (rw) => rw.rowKey === id)

			if (actionMC && row)
			{
				if (actionMC.params.isRoute)
					listConf.setListReturnControl(row, true)
				actionMC.params.action(listConf, actionMC, row)
			}
		},

		/**
		 * Navigates to a form.
		 * @param {object} listConf The list configuration
		 * @param {object} actionCfg Action configuration
		 * @param {object} row The row data object
		 * @param {array} historyEntries The History entries to be applied at the next level
		 */
		openFormAction(listConf, actionCfg, row, historyEntries)
		{
			if (actionCfg.params.type !== 'form')
				return

			// Whether or not the current context is a form.
			const isForm = typeof this.formInfo === 'object' && typeof this.isEditable === 'boolean'

			let formModes = ''
			if (actionCfg.params.restrictedModes) // Until access modes change from DBs to each Form
				formModes = genericFunctions.getDefaultFormModesForMode(actionCfg.params.mode)
			else
			{
				if (listConf.config.permissions.canView && genericFunctions.btnHasPermission(row?.btnPermission, qEnums.formModes.show))
					formModes += 'v'
				if (!isForm || this.isEditable)
				{
					if (listConf.config.permissions.canEdit && genericFunctions.btnHasPermission(row?.btnPermission, qEnums.formModes.edit))
						formModes += 'e'
					if (listConf.config.permissions.canDuplicate && genericFunctions.btnHasPermission(row?.btnPermission, qEnums.formModes.duplicate))
						formModes += 'd'
					if (listConf.config.permissions.canDelete && genericFunctions.btnHasPermission(row?.btnPermission, qEnums.formModes.delete))
						formModes += 'a'
					if (listConf.config.permissions.canInsert && genericFunctions.btnHasPermission(row?.btnPermission, qEnums.formModes.new))
						formModes += 'i'
				}
			}

			const formName = actionCfg.params.formName,
				mode = actionCfg.params.mode,
				formDef = listConf.config.formsDefinition[formName],
				options = {
					isPopup: formDef.isPopup,
					repeatInsert: actionCfg.params.repeatInsertion,
					isDuplicate: false,
					modes: formModes
				},
				query = {},
				prefillValues = actionCfg.params.prefillValues || {}
			let id = null

			// Apply history limits that cannot be applied at the form level.
			// (See description in the formHandlers prop)
			if (Array.isArray(historyEntries))
				Reflect.set(options, 'historyEntries', JSON.stringify(historyEntries))

			// GoBack pattern (menus)
			if (Number.isInteger(actionCfg.params.goBack))
				Reflect.set(options, 'goBack', actionCfg.params.goBack)

			// Controlled change for other route. e.g: Support form
			if (actionCfg.params.isControlled)
				Reflect.set(options, 'isControlled', true)

			// Other options
			if (actionCfg.params.otherOptions)
			{
				for (const prop in actionCfg.params.otherOptions)
					if (Object.prototype.hasOwnProperty.call(actionCfg.params.otherOptions, prop))
						Reflect.set(options, prop, actionCfg.params.otherOptions[prop])
			}

			const tableName = listConf.controller[0] + listConf.controller.substring(1).toLowerCase()
			const tableViewModelName = listConf.action + '_ViewModel'
			this.setEntryValue({ navigationId: this.navigationId, key: 'TableName', value: tableName })
			this.setEntryValue({ navigationId: this.navigationId, key: 'TableViewModelName', value: tableViewModelName })

			if (mode === 'DUPLICATE')
				options.isDuplicate = true

			if (mode !== 'NEW')
			{
				id = formDef.fnKeySelector(row)
				if (!_isEmpty(actionCfg.params.limits))
				{
					_foreach(actionCfg.params.limits, (limit) => {
						if (limit.identifier === 'id')
							id = limit.fnValueSelector(row.Fields)
						else
							Reflect.set(options, limit.identifier, limit.fnValueSelector(row.Fields))
					})
				}
			}
			else
				options.isControlled = true

			this.navigateToForm(formName, mode, id, options, query, prefillValues)
		},

		/**
		 * Navigates to a menu.
		 * @param {object} _ The list configuration
		 * @param {object} actionCfg Action configuration
		 * @param {object} row The row data object
		 */
		openMenuAction(_, actionCfg, row)
		{
			if (actionCfg.params.type !== 'menu')
				return

			const params = {}

			if (!_isEmpty(actionCfg.params.limits))
			{
				_foreach(actionCfg.params.limits, (limit) => {
					const limitValue = limit.fnValueSelector(row.Fields)
					Reflect.set(params, limit.identifier, limitValue)
					this.setEntryValue({ navigationId: this.navigationId, key: limit.identifier, value: limitValue })
				})
			}

			this.navigateToRouteName(`menu-${actionCfg.params.menuName}`, params)
		},

		/**
		 *
		 * @param {*} _
		 * @param {*} actionCfg
		 * @param {*} row
		 */
		openReportAction(_, actionCfg, row)
		{
			if (actionCfg.params.type !== 'report' && actionCfg.params.type !== 'ssrsViewer')
				return

			if (!_isEmpty(actionCfg.params.limits))
			{
				_foreach(actionCfg.params.limits, (limit) => {
					const limitValue = limit.fnValueSelector(row.Fields)
					Reflect.set(actionCfg.params, limit.identifier, limitValue)
					this.setEntryValue({ navigationId: this.navigationId, key: limit.identifier, value: limitValue })
				})
			}

			if (actionCfg.params.type === 'report')
				this.navigateToReport(actionCfg.params.baseArea, actionCfg.name, actionCfg.params)
			else if (actionCfg.params.type === 'ssrsViewer')
				this.navigateToReportingServicesViewer(actionCfg.params.baseArea, actionCfg.name, actionCfg.params)
		},

		/**
		 *
		 * @param {*} _
		 * @param {*} actionCfg
		 * @param {*} row
		 */
		openRoutineAction(_, actionCfg, row)
		{
			if (actionCfg.params.type !== 'routine')
				return

			const params = {}

			if (!_isEmpty(actionCfg.params.limits))
			{
				_foreach(actionCfg.params.limits, (limit) => {
					params[limit.identifier] = limit.fnValueSelector(row.Fields)
				})
			}

			if (actionCfg.params.actionRoutine)
				actionCfg.params.actionRoutine(params)
		},

		/**
		 *
		 * @param {*} _
		 * @param {*} actionCfg
		 * @param {*} row
		 */
		openQSignAction(_, actionCfg, row)
		{
			if (actionCfg.params.type !== 'qsign')
				return

			const params = {
				id: _get(row, 'rowKey', null)
			}

			if (!_isEmpty(actionCfg.params.limits))
			{
				_foreach(actionCfg.params.limits, (limit) => {
					params[limit.identifier] = limit.fnValueSelector(row.Fields)
				})
			}

			if (actionCfg.params.actionRoutine)
				actionCfg.params.actionRoutine(params)
		}
	}
}
