<template>
	<q-input-group
		v-if="tableCtrl.config.allowManageViews || configActionsCount > 0"
		data-testid="table-config"
		size="block">
		<q-select
			v-if="tableCtrl.config.allowManageViews && tableCtrl.config.tableConfigNames.length > 0"
			:model-value="selectedViewId"
			:items="savedConfigViews"
			:texts="tableCtrl.texts"
			:groups="viewsGroups"
			size="small"
			:aria-label="tableCtrl.texts.viewText"
			@update:model-value="confirmAndSetSelectedViewById" />

		<q-button
			v-if="tableCtrl.config.allowManageViews"
			borderless
			:disabled="!tableCtrl.confirmChanges"
			:title="tableCtrl.texts.saveChanges"
			@click="saveCurrentView()">
			<q-badge-indicator
				color="highlight"
				:enabled="tableCtrl.confirmChanges">
				<q-icon icon="save" />
			</q-badge-indicator>
		</q-button>

		<q-button
			v-if="configActionsCount === 1"
			:id="controlId"
			data-testid="table-config-details"
			borderless
			:title="tableCtrl.texts.tableConfig"
			@click="onConfigSelect()">
			<q-icon icon="table-configuration" />
		</q-button>
		<q-action-list
			v-else-if="configActionsCount > 1"
			:id="controlId"
			data-testid="table-config-details"
			borderless
			placement="bottom-start"
			:title="tableCtrl.texts.tableConfig"
			:items="configItems"
			:groups="configGroups"
			@click="onConfigSelect">
			<q-icon icon="table-configuration" />
		</q-action-list>
	</q-input-group>

	<q-table-config-popup
		v-if="tableCtrl.popupIsVisible && tableCtrl.selectedConfigTab"
		:modal-id="`q-modal-${modalId}`"
		:tab="tableCtrl.selectedConfigTab"
		:table-ctrl="tableCtrl"
		:config-data="configData"
		v-on="configHandlers">
		<template #default>
			<q-select
				v-if="tableCtrl.config.allowManageViews"
				:model-value="selectedUnsavedViewId"
				:items="allConfigViews"
				:texts="tableCtrl.texts"
				:groups="viewsGroups"
				:label="tableCtrl.texts.selectedView"
				:disabled="!canSwitchViews"
				size="medium"
				@update:model-value="confirmAndSetSelectedViewById" />
		</template>
		<template #filters="{ onFiltersUpdate }">
			<slot
				name="filters"
				:on-filters-update="onFiltersUpdate" />
		</template>
	</q-table-config-popup>
</template>

<script>
	import { computed, defineAsyncComponent } from 'vue'
	import isEmpty from 'lodash-es/isEmpty'

	import { displayMessage } from '@quidgest/clientapp/utils/genericFunctions'
	import { QActionList } from '@quidgest/clientapp/components'

	export default {
		name: 'QTableConfig',

		emits: [
			'hide-config',
			'mark-config-dirty',
			'refresh',
			'save-view',
			'show-config',
			'update:config'
		],

		components: {
			QActionList,
			QTableConfigPopup: defineAsyncComponent(() => import('./QTableConfigPopup.vue'))
		},

		inheritAttrs: false,

		props: {
			/**
			 * Control object containing necessary state and configuration properties of the table component.
			 */
			tableCtrl: {
				type: Object,
				required: true
			}
		},

		expose: [],

		data()
		{
			return {
				tableId: this.tableCtrl.id || this.tableCtrl.config.name || `q-table-${this._.uid}`,
				configItems: [
					{
						key: 'views',
						label: computed(() => this.tableCtrl.texts.manageViews),
						icon: { icon: 'view-manager' },
						group: 'default',
						isVisible: computed(() => this.tableCtrl.config.configOptions.find((op) => op.id === 'views')?.visible ?? false)
					},
					{
						key: 'columns',
						label: computed(() => this.tableCtrl.texts.configureColumns),
						icon: { icon: 'list' },
						group: 'default',
						isVisible: computed(() => this.tableCtrl.config.configOptions.find((op) => op.id === 'columns')?.visible ?? false)
					},
					{
						key: 'filters',
						label: computed(() => this.tableCtrl.texts.configureFilters),
						icon: { icon: 'filter' },
						group: 'default',
						isVisible: computed(() => this.tableCtrl.config.configOptions.find((op) => op.id === 'filters')?.visible ?? false)
					},
					{
						key: 'new-view',
						label: computed(() => this.tableCtrl.texts.createView),
						icon: { icon: 'add' },
						group: 'create',
						isVisible: computed(() => this.tableCtrl.config.configOptions.find((op) => op.id === 'views')?.visible ?? false)
					}
				],
				viewsGroups: [
					{ id: 'user' },
					{ id: 'system' }
				],
				configGroups: [
					{
						id: 'default',
						display: 'dropdown'
					},
					{
						id: 'create',
						display: 'dropdown'
					}
				],
				configHandlers: {
					apply: (eventData) => this.applyConfig(eventData),
					hide: () => this.hideConfig(),
					setCurrentView: (eventData) => this.setCurrentView(eventData),
					showView: (eventData) => this.confirmAndSetSelectedViewById(eventData, true),
					'update:config': (eventData) => this.updateConfig(eventData),
					'update:views': (eventData) => this.updateViews(eventData)
				},
				configData: {},
				viewsData: null,
				newSelectedViewName: undefined
			}
		},

		beforeUnmount()
		{
			this.configItems = null
			this.configHandlers = null
			this.configData = null
			this.viewsData = null
		},

		computed: {
			/**
			 * The configuration identifier.
			 */
			controlId()
			{
				return `${this.tableId}-config-menu`
			},

			/**
			 * The configuration modal identifier.
			 */
			modalId()
			{
				return `${this.tableId}-config`
			},

			/**
			 * The list of saved views.
			 */
			savedConfigViews()
			{
				const views = []
				let viewIdx = 1

				for (const configName of this.tableCtrl.config.tableConfigNames)
				{
					views.push({
						key: ++viewIdx,
						value: configName,
						group: 'user'
					})
				}

				views.push({
					key: 1,
					value: this.tableCtrl.texts.baseTable,
					group: 'system'
				})

				return views
			},

			/**
			 * The list of all views (even the unsaved one, if any).
			 */
			allConfigViews()
			{
				const views = [...this.savedConfigViews]

				// If the view can't be switched, it means there's a new view created
				// with unsaved changes, therefore, we need to add it to the list.
				if (!this.canSwitchViews)
				{
					views.push({
						key: views.length + 1,
						value: this.newSelectedViewName,
						group: 'user'
					})
				}

				return views
			},

			/**
			 * Whether the user can switch to another view.
			 * Users can't switch to another view if they've created a new view based on the current one.
			 */
			canSwitchViews()
			{
				return typeof this.newSelectedViewName !== 'string'
			},

			/**
			 * The identifier of the selected table view.
			 */
			selectedViewId()
			{
				return this.getViewIdByName(this.tableCtrl.config.userTableConfigName)
			},

			/**
			 * The identifier of the selected table view, even if still unsaved.
			 */
			selectedUnsavedViewId()
			{
				return this.getViewIdByName(this.newSelectedViewName ?? this.tableCtrl.config.userTableConfigName)
			},

			/**
			 * The number of available actions.
			 */
			configActionsCount()
			{
				return this.configItems.filter((i) => i.isVisible).length
			},

			/**
			 * True if there are unapplied configuration changes, false otherwise.
			 */
			isDirty()
			{
				return !isEmpty(this.configData)
			},

			/**
			 * True if there are changes in the configuration or in the views, false otherwise.
			 */
			hasChanges()
			{
				return this.isDirty || this.viewsData !== null
			}
		},

		methods: {
			/**
			 * Opens the configuration popup in the specified tab.
			 * @param {string} selectedTab The desired tab
			 */
			openConfigTab(selectedTab)
			{
				this.$emit('show-config', {
					selectedTab,
					modalProps: {
						id: this.modalId,
						returnElement: this.controlId
					}
				})
			},

			/**
			 * Handles the selection of one of the configuration options.
			 * @param {string} optionId The identifier of the selected option
			 */
			onConfigSelect(optionId)
			{
				if (optionId === 'new-view')
					this.$emit('save-view', { name: '' })
				else
					this.openConfigTab(optionId)
			},

			/**
			 * Gets the view identifier according to the specified name.
			 * @param {string} name The view name
			 */
			getViewIdByName(name)
			{
				const view = this.allConfigViews.find((e) => e.value === name)
				return view?.key ?? 1
			},

			/**
			 * Gets the view name according to the specified identifier.
			 * @param {number} id The view identifier
			 */
			getViewNameById(id)
			{
				// View with identifier 1 corresponds to the base table.
				if (id === 1)
					return ''

				const view = this.allConfigViews.find((e) => e.key === id)
				return view?.value
			},

			/**
			 * Sets the selected view, using it's id.
			 * @param {number} id The view identifier
			 * @param {boolean} closePopup Whether to close the configuration popup afterwards
			 */
			setSelectedViewById(id, closePopup = false)
			{
				if (this.selectedUnsavedViewId === id)
					return

				// Remove any unsaved changes.
				this.updateConfig({})
				this.setCurrentView()

				const viewName = this.getViewNameById(id)
				if (typeof viewName === 'string')
				{
					this.$emit('refresh', viewName)
					if (closePopup)
						this.hideConfig()
				}
			},

			/**
			 * Confirms whether to save, if there are changes, and sets the selected view.
			 * @param {number} id The view identifier
			 * @param {boolean} closePopup Whether to close the configuration popup afterwards
			 */
			confirmAndSetSelectedViewById(id, closePopup = false)
			{
				if (this.selectedUnsavedViewId === id)
					return

				if ((this.tableCtrl.confirmChanges || this.isDirty) && !this.tableCtrl.readonly)
				{
					const buttons = {
						confirm: {
							label: this.tableCtrl.texts.saveText,
							action: () => this.saveCurrentView(id)
						},
						cancel: {
							label: this.tableCtrl.texts.discard,
							action: () => this.setSelectedViewById(id, closePopup)
						}
					}
					displayMessage(`${this.tableCtrl.texts.wantToSaveChangesToView}`, 'warning', null, buttons)
				}
				else
					this.setSelectedViewById(id, closePopup)
			},

			/**
			 * Saves the current view.
			 * @param {number} id The identifier of a view to change to after the save operation
			 */
			saveCurrentView(id)
			{
				this.$emit('save-view', {
					name: this.tableCtrl.config.userTableConfigName,
					config: this.configData,
					changeTo: this.getViewNameById(id)
				})

				// Remove the changes that were just saved.
				this.updateConfig({})
				this.setCurrentView()
			},

			/**
			 * Updates the user configuration.
			 * @param {object} config The configuration
			 */
			updateConfig(config)
			{
				this.configData = config
			},

			/**
			 * Updates the user configuration views.
			 * @param {array} views The configuration views
			 */
			updateViews(views)
			{
				this.viewsData = views
			},

			/**
			 * Emits the event to hide the configuration popup.
			 */
			hideConfig()
			{
				this.$emit('hide-config', this.modalId)
			},

			/**
			 * Emits the event to update the user configuration.
			 * @param {boolean} save Whether to save the changes being applied
			 */
			applyConfig(save)
			{
				if (this.hasChanges)
				{
					const config = {
						modalId: this.modalId,
						save,
						...this.configData
					}

					if (this.viewsData !== null)
						config.views = this.viewsData

					this.$emit('update:config', config)
				}
				else
					this.hideConfig()
			},

			/**
			 * Adds a current unsaved view to the list of views.
			 * @param {string} viewName The name of the view
			 */
			setCurrentView(viewName)
			{
				this.newSelectedViewName = viewName
			}
		},

		watch: {
			isDirty()
			{
				this.$emit('mark-config-dirty', this.hasChanges)
			},

			viewsData()
			{
				this.$emit('mark-config-dirty', this.hasChanges)
			},

			'tableCtrl.popupIsVisible'()
			{
				this.updateConfig({})
				this.setCurrentView()
				this.updateViews(null)
			}
		}
	}
</script>
