<template>
	<div :class="dashboardClasses">
		<div class="c-action-bar">
			<span class="form-header">
				<h1>{{ title }}</h1>
			</span>

			<div class="c-action-bar__menu">
				<q-button
					v-if="!inEditMode"
					borderless
					:title="texts.editButtonTitle"
					:disabled="readonly"
					@click="edit">
					<q-icon icon="pencil" />
				</q-button>
				<q-button-group
					v-else
					borderless>
					<q-button
						:title="texts.compactButtonTitle"
						@click="compact">
						<q-icon icon="compact" />
					</q-button>
					<q-button
						:title="texts.saveButtonTitle"
						:disabled="!isDirty"
						@click="save">
						<q-icon icon="save" />
					</q-button>
					<q-button
						:title="texts.cancelButtonTitle"
						@click="cancel">
						<q-icon icon="cancel" />
					</q-button>
				</q-button-group>
			</div>
		</div>

		<div
			class="grid-stack"
			@dragover.prevent
			@drop.prevent="onDrop($event)">
			<template
				v-for="widget in visibleWidgets"
				:key="widget.uuid">
				<q-widget
					:widget="widget"
					:group="getWidgetGroup(widget)"
					:config="widgetConfig"
					:texts="texts"
					:border-color="widget.BorderStyle"
					v-slot="scopeFromQWidget"
					v-bind="widget.customProps"
					@delete-widget="deleteWidget(widget.uuid)"
					@record-change="onRecordChange(widget.uuid, $event)"
					@fetch-data="onFetchData(widget.uuid)">
					<component
						:is="widget.component"
						v-bind="scopeFromQWidget"
						@init="(...args) => setCustomProps(widget.uuid, ...args)"
						@navigate-to="$emit('navigate-to', $event)" />
				</q-widget>
			</template>

			<div
				v-if="!visibleWidgets.length && !inEditMode"
				class="no-widgets">
				<img
					class="no-widgets__image"
					:src="`${resourcesPath}no-widgets.png`"
					:alt="texts.noRecordsText" />

				<h2 class="no-widgets__message">
					{{ texts.noDataText }}
				</h2>

				<q-button
					variant="bold"
					:label="texts.addWidgetText"
					@click="edit">
					<q-icon icon="pencil" />
				</q-button>
			</div>
		</div>

		<teleport
			:to="target"
			v-if="target">
			<q-widget-panel
				v-if="inEditMode"
				:texts="texts"
				:widgets="widgets"
				:groups="groups"
				@add-widget="addWidget"
				@dragstart="onDragStart" />
		</teleport>
	</div>
</template>

<script>
	import { GridStack } from 'gridstack'
	import cloneDeep from 'lodash-es/cloneDeep'
	import _forEach from 'lodash-es/forEach'
	import { defineAsyncComponent, ref } from 'vue'

	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	import QWidgetPanel from './QWidgetPanel.vue'
	import QWidget from './widgets/QWidget.vue'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		editButtonTitle: 'Edit',
		compactButtonTitle: 'Compact',
		saveButtonTitle: 'Save',
		cancelButtonTitle: 'Cancel',
		helpText: 'To add a widget to the dashboard, drag and drop or select a widget and click %s.',
		addButtonTitle: 'Add',
		addWidgetText: 'Add widget',
		noRecordsText: 'No records',
		noDataText: 'No data to show',
		previousPageText: 'Previous page',
		nextPageText: 'Next page',
		removeButtonText: 'Remove',
		refreshButtonText: 'Refresh'
	}

	export default {
		name: 'QDashboard',

		emits: ['save', 'fetch-data', 'navigate-to'],

		components: {
			QWidget,
			QWidgetPanel,
			QAlertWidget: defineAsyncComponent(() => import('./widgets/QAlertWidget.vue')),
			QMenuWidget: defineAsyncComponent(() => import('./widgets/QMenuWidget.vue')),
			QSubMenusWidget: defineAsyncComponent(() => import('./widgets/QSubMenusWidget.vue')),
			QCustomWidget: defineAsyncComponent(() => import('./widgets/QCustomWidget.vue'))
		},

		inheritAttrs: false,

		props: {
			/**
			 * Title for the dashboard.
			 */
			title: {
				type: String,
				required: true
			},

			/**
			 * Array of widgets for the dashboard.
			 */
			widgets: {
				type: Array,
				default: () => []
			},

			/**
			 * Array of groups for the dashboard widgets.
			 */
			groups: {
				type: Array,
				default: () => []
			},

			/**
			 * Texts object for various messages.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Path for the resources.
			 */
			resourcesPath: {
				type: String,
				required: true
			},

			/**
			 * Indicates whether the dashboard is in a read-only or interactive state.
			 */
			readonly: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				inEditMode: false,
				isDirty: false,
				target: ref(null)
			}
		},

		mounted()
		{
			// Wait until DOM is ready to load gridstack
			this.$nextTick().then(() => {
				this.init()
			})
		},

		beforeUnmount()
		{
			window.removeEventListener('resize', this.resizeGrid)
			if(this.grid && typeof this.grid.destroy === 'function')
				this.grid.destroy()
			this.grid = null
		},

		computed: {
			/**
			 * The widget configuration.
			 */
			widgetConfig()
			{
				return {
					inEditMode: this.inEditMode,
					resourcesPath: this.resourcesPath
				}
			},

			/**
			 * Filtered list of visible widgets.
			 */
			visibleWidgets()
			{
				// Making a copy because `.sort()` mutates self
				const widgets = [...this.widgets]
				return widgets.filter((w) => w.Visible).sort((a, b) => (a.Order > b.Order ? 1 : -1))
			},

			/**
			 * Dashboard classes based on the edit mode and widget visibility.
			 */
			dashboardClasses()
			{
				const isDisabled = !this.inEditMode,
					isEmpty = !this.visibleWidgets.length && !this.inEditMode

				return [
					'q-dashboard',
					{
						'q-dashboard--disabled': isDisabled,
						'q-dashboard--empty': isEmpty
					}
				]
			}
		},

		methods: {
			/**
			 * Initializes the gridstack instance with specific options and sets up
			 * necessary event listeners for resizing and widget management.
			 */
			init()
			{
				// Init gridstack.
				this.grid = GridStack.init({
					float: true,
					cellHeight: '90px',
					animate: true,
					disableResize: true
				})

				// Immediately resize the grid to fit the initial window dimension.
				this.resizeGrid()

				// Adjust when future resizes happen.
				window.addEventListener('resize', this.resizeGrid)

				// Listen to relevant gridstack events.
				this.grid.on('added', (_, els) => this.handleChange(els))
				this.grid.on('change', (_, els) => this.handleChange(els))
				this.grid.enableMove(false)

				// FIXME: change when the RightSideBar is refactored
				const el = document.querySelector('#widgets-panel')
				if (el) this.target = el

				// Register the initial position of the widgets
				// This is necessary because new widgets
				// are always assigned a default position
				// ...so we need to update internal position
				// with the position assigned by gridstack
				this.updateInternalPosition(this.grid.engine.nodes)
			},

			/**
			 * Enters edit mode.
			 */
			edit()
			{
				// Temp list to aid the 'cancel' and 'save' operations.
				this.previousGrid = this.exportGrid()
				this.enable()
			},

			/**
			 * Compacts the grid.
			 */
			compact()
			{
				this.grid.compact()
			},

			/**
			 * Saves changes.
			 */
			save()
			{
				this.$emit('save', this.widgets)
				this.disable()
			},

			/**
			 * Cancels edit mode and revert changes.
			 */
			cancel()
			{
				// Revert changes made by the user.
				this.widgets.forEach((widget) =>
				{
					const prev = this.previousGrid.find((w) => w.uuid === widget.uuid)

					const inUseBeforeEdit = prev.Visible
					const inUseAfterEdit = widget.Visible
					widget.Rowkey = prev.Rowkey

					if (inUseBeforeEdit && !inUseAfterEdit)
					{
						// Widget has been removed, let's add it back.
						widget.Hposition = prev.Hposition
						widget.Vposition = prev.Vposition
						this.addWidget(widget.uuid)
					}
					else if (!inUseBeforeEdit && inUseAfterEdit)
					{
						// Widget has been added, let's remove it.
						this.deleteWidget(widget.uuid)
					}
					else if (inUseAfterEdit && prev.Hposition !== -1 && prev.Vposition !== -1)
					{
						// Revert potential position changes of widgets in use.
						widget.Hposition = prev.Hposition
						widget.Vposition = prev.Vposition

						// Make gridstack aware of the revert.
						this.grid.update(widget.uuid, {
							x: widget.Hposition,
							y: widget.Vposition
						})
					}
				})

				this.disable()
			},

			/**
			 * Exports the current grid configuration.
			 * @returns {Array} - Cloned array of widgets.
			 */
			exportGrid()
			{
				return cloneDeep(this.widgets)
			},

			/**
			 * Resizes the grid based on the window dimension.
			 */
			resizeGrid()
			{
				// Possible values: 'moveScale' | 'move' | 'scale' | 'none'.
				const layout = 'none'
				const width = document.body.clientWidth

				if (width < 700)
					this.grid.column(1, layout)
				else if (width < 950)
					this.grid.column(6, layout)
				else
					this.grid.column(12, layout)
			},

			/**
			 * Enables edit mode.
			 */
			enable()
			{
				this.isDirty = false
				this.inEditMode = true
				this.grid.enableMove(true)
				this.$eventHub?.emit('open-sidebar-on-tab', 'widgets-panel')
			},

			/**
			 * Disables edit mode.
			 */
			disable()
			{
				this.isDirty = false
				this.inEditMode = false
				this.grid.enableMove(false)
				this.$eventHub?.emit('toggle-sidebar-on-tab', 'widgets-panel')
			},

			/**
			 * Adds a widget to the dashboard.
			 * @param {string} uuid - The UUID of the widget to add.
			 */
			addWidget(uuid)
			{
				const widget = this.getWidget(uuid)

				if (widget)
				{
					// Making the widget visible
					// will make Vue add it to the DOM.
					widget.Visible = true

					// Wait until the DOM is ready.
					this.$nextTick().then(() => {
						// Now we just tell gridstack
						// that the widget exists.
						this.grid.makeWidget(widget.uuid)
					})
				}
			},

			/**
			 * Deletes a widget from the dashboard.
			 * @param {string} uuid - The UUID of the widget to delete.
			 */
			deleteWidget(uuid)
			{
				const widget = this.getWidget(uuid)

				// Tell gridstack the widget no longer exists.
				this.grid.removeWidget(widget.uuid)

				// Remove it from the DOM.
				widget.Hposition = -1
				widget.Vposition = -1
				widget.Visible = false
				this.isDirty = true
			},

			/**
			 * Handles the start of a drag operation.
			 * @param {DragEvent} ev - The drag event object.
			 */
			onDragStart(ev)
			{
				ev.ev.dataTransfer.setData('widget-id', ev.uuid)
			},

			/**
			 * Handles the drop event.
			 * @param {DragEvent} ev - The drop event object.
			 */
			onDrop(ev)
			{
				const uuid = ev.dataTransfer.getData('widget-id')
				if (uuid)
					this.addWidget(uuid)
			},

			/**
			 * Retrieves a widget from the list of widgets based on its UUID.
			 *
			 * @param {string} uuid - The unique identifier of the widget to be retrieved.
			 * @returns {Object|null} The widget object if found, otherwise null.
			 */
			getWidget(uuid)
			{
				return this.widgets.find((widget) => widget.uuid === uuid)
			},

			/**
			 * Gets the group of a given widget.
			 * @param {Object} widget - The widget object.
			 * @returns {Object} - The group object.
			 */
			getWidgetGroup(widget)
			{
				return this.groups.find((group) => group.id === widget.Group)
			},

			/**
			 * Handles widget position changes made by the user.
			 * @param {Array} els - Array of grid elements.
			 */
			handleChange(els)
			{
				this.isDirty = true

				this.updateInternalPosition(els)
			},

			/**
			 * Updates the internal position of the widgets
			 * that correspond to the provided elements.
			 * @param {Array} els - Array of grid elements.
			 */
			updateInternalPosition(els)
			{
				_forEach(els, (el) => {
					const widget = this.widgets.find((w) => w.uuid === el.id)
					widget.Hposition = el.x
					widget.Vposition = el.y
				})
			},

			/**
			 * Handles the record change event for a widget.
			 * @param {string} uuid - The UUID of the widget.
			 * @param {string} rowKey - The row key.
			 */
			onRecordChange(uuid, rowKey)
			{
				const widget = this.getWidget(uuid)

				if (widget)
					widget.Rowkey = rowKey

				this.isDirty = true
			},

			/**
			 * Handles the fetch data event for a widget.
			 * @param {string} uuid - The UUID of the widget.
			 */
			onFetchData(uuid) {
				const widget = this.getWidget(uuid)

				if (widget) {
					widget.data = null
					this.$emit('fetch-data', widget)
				}
			},

			/**
			 * Sets custom properties for a widget.
			 * @param {string} uuid - The UUID of the widget.
			 * @param {Object} props - The custom properties.
			 */
			setCustomProps(uuid, props)
			{
				const widget = this.getWidget(uuid)

				if (widget)
					widget.customProps = props
			}
		}
	}
</script>
