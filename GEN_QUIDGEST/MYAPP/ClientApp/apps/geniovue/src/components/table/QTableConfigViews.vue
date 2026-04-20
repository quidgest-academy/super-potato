<template>
	<q-grid-table-list
		show-messages
		:columns="columns"
		:data="views"
		:texts="texts"
		@mark-for-deletion="onMarkForDeletion"
		@undo-deletion="onUndoDeletion">
		<template #[`actions.prepend`]="{ model }">
			<q-button
				data-testid="show-view"
				variant="text"
				:title="texts.viewText"
				:disabled="isNewView(model.id.value) || model.selected.value || newSelected > 0"
				@click="showView(model)">
				<q-icon icon="go-to" />
			</q-button>
			<q-button
				data-testid="duplicate-view"
				variant="text"
				:title="texts.duplicateText"
				:disabled="isNewView(model.id.value) || hasDuplicate(model.id.value)"
				@click="duplicateView(model)">
				<q-icon icon="duplicate" />
			</q-button>
		</template>
		<template #actions="{ model }">
			<q-button
				v-if="model.id.value === 1"
				disabled
				data-testid="delete"
				variant="text"
				:title="texts.delete">
				<q-icon icon="delete" />
			</q-button>
		</template>
		<template #[`column.name`]="{ model, row }">
			<q-text-field
				:id="getNameInputId(row)"
				:key="domKey"
				size="block"
				v-model="model.value"
				:disabled="isNewView(row.id.value) && hasDuplicate(selectedView) && !row.isDirty"
				:readonly="model.value === texts.baseTable"
				:max-length="maxNameLength"
				:placeholder="row.isDirty
					? row.basedOn.value > 0
						? `${texts.createView} ${texts.basedOn.toLowerCase()} '${getView(row.basedOn.value).name.value}'`
						: ''
					: texts.createView" />
		</template>
		<template #[`column.default`]="{ row }">
			<q-radio-group
				:key="row.id.value"
				name="default"
				:disabled="isNewView(row.id.value) && hasDuplicate(selectedView) && !row.isDirty"
				:model-value="defaultView"
				@update:model-value="setDefaultView">
				<q-radio-button :value="row.id.value" />
			</q-radio-group>
		</template>
		<template #[`column.selected`]="{ model, row }">
			<q-render-boolean
				:value="newSelected < 1 && model.value || newSelected === row.id.value"
				:texts="texts" />
		</template>
		<template #[`column.basedOn`]="{ model }">
			<q-text-field
				:model-value="model.value < 1 ? '' : getView(model.value).name.value"
				size="small"
				readonly />
		</template>
	</q-grid-table-list>
</template>

<script>
	import { watch } from 'vue'

	import { Boolean, GridTableListValue, Number, String } from '@quidgest/clientapp/models/fields'
	import { BooleanColumn, NumericColumn, TextColumn } from '@/mixins/listColumnTypes.js'

	export default {
		name: 'QTableConfigViews',

		emits: ['set-current', 'show-view', 'update:views'],

		inheritAttrs: false,

		props: {
			/**
			 * An object that provides localized text strings to be used within the views modal.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * The name of the associated table.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * An array containing the names of the saved configurations/views.
			 */
			configNames: {
				type: Array,
				default: () => []
			},

			/**
			 * The name of the default saved view configuration.
			 */
			configDefaultName: {
				type: String,
				default: ''
			},

			/**
			 * The name of the currently selected view configuration.
			 */
			configCurrentName: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data()
		{
			return {
				domKey: 0,
				maxNameLength: 50,
				views: null,
				viewsWatcher: () => {},
				newSelected: -1
			}
		},

		created()
		{
			const views = new GridTableListValue()
			let id = 1

			// Add the base table as the first view of the list.
			const baseTableView = this.setView(id, this.texts.baseTable)
			views.elements.push(baseTableView)

			// Add the remaining custom views.
			for (const configName of this.configNames)
			{
				const view = this.setView(++id, configName)
				views.elements.push(view)
			}

			this.views = views
			this.setNewRecordTemplate()

			// If there's no default, mark the base table as the default.
			if (typeof this.defaultView !== 'number')
				this.views.elements[0].default.hydrate(true)

			// Set a watcher to react to changes to the views.
			this.viewsWatcher = watch(
				() => this.views,
				() => this.updateViews(),
				{ deep: true })
		},

		beforeUnmount()
		{
			this.views = null
			this.viewsWatcher()
		},

		computed: {
			/**
			 * The list of columns.
			 */
			columns()
			{
				return [
					new NumericColumn({
						order: 1,
						name: 'id',
						isVisible: false
					}),
					new TextColumn({
						order: 2,
						name: 'name',
						label: this.texts.viewNameText
					}),
					new BooleanColumn({
						order: 3,
						name: 'default',
						label: this.texts.defaultViewText
					}),
					new BooleanColumn({
						order: 4,
						name: 'selected',
						label: this.texts.selected
					}),
					new TextColumn({
						order: 5,
						name: 'basedOn',
						label: this.texts.basedOn,
						isVisible: false
					})
				]
			},

			/**
			 * The identifier of the default view.
			 */
			defaultView()
			{
				return this.views?.elements.find((e) => e.default.value)?.id.value ??
					this.views?.newElements.find((e) => e.default.value)?.id.value
			},

			/**
			 * The identifier of the selected view.
			 */
			selectedView()
			{
				return this.views?.elements.find((e) => e.selected.value)?.id.value
			},

			/**
			 * A list of the current view names.
			 */
			viewNames()
			{
				return [
					...this.views.elements.map((e) => e.name.value),
					...this.views.newElements.map((e) => e.name.value)
				]
			},

			/**
			 * The name of the new currently selected view.
			 */
			newSelectedName()
			{
				const newView = this.views?.newElements.find((e) => e.id.value === this.newSelected)
				return newView?.name.value
			}
		},

		methods: {
			/**
			 * Gets the identifier for the configuration view name input.
			 * @param {object} model The row model corresponding to the desired view
			 * @returns The identifier for the configuration view name input.
			 */
			getNameInputId(model)
			{
				return model ? `${this.tableName}-view-name-${model.id.value}` : ''
			},

			/**
			 * Creates a new empty configuration view.
			 * @returns A new empty configuration view.
			 */
			createNewView()
			{
				const vm = this
				const newView = {
					id: new Number(),
					name: new String(),
					default: new Boolean(),
					selected: new Boolean(),
					basedOn: new Number(),
					serverErrorMessages: [],
					get QPrimaryKey() { return this.id.value },
					get uniqueIdentifier() { return this.id.value.toString() },
					get isDirty() { return vm.columns.map((c) => c.name).some((id) => this[id].isDirty) }
				}

				newView.default.hydrate(false)
				newView.selected.hydrate(false)

				return newView
			},

			/**
			 * Creates/updates a configuration view.
			 * @param {number} id The identifier of the view
			 * @param {string} name The name of the view
			 * @param {number} basedOn The identifier of a view on which this one is based
			 * @param {object} configView The view to update, if not provided a new one will be created
			 * @returns The created/updated configuration view.
			 */
			setView(id, name, basedOn = 0, configView = null)
			{
				const viewName = id === 1 ? '' : name
				const view = configView ?? this.createNewView()

				view.id.hydrate(id)
				view.selected.hydrate(name.length > 0 && viewName === this.configCurrentName)

				if (configView === null)
				{
					view.name.hydrate(name)
					view.default.hydrate(name.length > 0 && viewName === this.configDefaultName)
					view.basedOn.hydrate(basedOn)
				}
				else
				{
					view.name.updateValue(name)
					view.default.updateValue(name.length > 0 && viewName === this.configDefaultName)
					view.basedOn.updateValue(basedOn)
				}

				return view
			},

			/**
			 * Sets the new record template, which is an empty row that serves as a new element's placeholder.
			 */
			setNewRecordTemplate()
			{
				if (this.views.emptyRows.length > 0)
					return

				const newRecord = this.createNewView()
				this.views.newRecordTemplate = newRecord
				this.views.newElements.push(newRecord)
			},

			/**
			 * Tries to get the configuration view with the specified identifier.
			 * @param {number} id The identifier of the view
			 * @returns The view with the specified identifier, or null if none was found.
			 */
			getView(id)
			{
				return this.views.elements.find((e) => e.id.value === id) ?? this.views.newElements.find((e) => e.id.value === id)
			},

			/**
			 * Checks whether the view with the specified identifier is new.
			 * @param {number} id The identifier of the view
			 * @returns True if it's a new view, false otherwise.
			 */
			isNewView(id)
			{
				return this.views.newElements.some((e) => e.id.value === id)
			},

			/**
			 * Checks whether there's already a view based on the one with the specified identifier.
			 * @param {number} id The identifier of the view
			 * @returns True if there's a view based on the specified one, false otherwise.
			 */
			hasDuplicate(id)
			{
				return this.views.newElements.some((e) => e.basedOn.value === id)
			},

			/**
			 * Called when the user wants to preview a configuration view.
			 * @param {object} model The row model corresponding to the desired view
			 */
			showView(model)
			{
				this.$emit('show-view', model.id.value)
			},

			/**
			 * Duplicates the specified view.
			 * @param {object} model The row model corresponding to the view to duplicate
			 */
			duplicateView(model)
			{
				const view = this.views.newElements.at(-1)
				this.setView(this.views.rowCount + 1, '', model.id.value, view)
			},

			/**
			 * Marks the specified view/row to be deleted.
			 * @param {object} model The row model
			 */
			onMarkForDeletion(model)
			{
				this.views.markForDeletion(model)
			},

			/**
			 * Undoes the deletion of the specified view/row.
			 * @param {object} model The row model
			 */
			onUndoDeletion(model)
			{
				this.views.undoDeletion(model)
			},

			/**
			 * Sets the view with the specified identifier as the default.
			 * @param {number} id The identifier of the view
			 */
			setDefaultView(id)
			{
				if (typeof id !== 'number')
					return

				this.views.elements.forEach((e) => e.default.updateValue(e.id.value === id))
				this.views.newElements.forEach((e) => e.default.updateValue(e.id.value === id))
			},

			/**
			 * Validates the specified configuration view.
			 * @param {object} configView The configuration view
			 * @returns The number of validation errors.
			 */
			validateView(configView)
			{
				const name = configView.name.value
				let errorCount = 0

				configView.serverErrorMessages = []

				if (name.length === 0)
				{
					configView.serverErrorMessages.push(this.texts.emptyViewName)
					errorCount++
				}
				else if (this.viewNames.filter((v) => v === name).length > 1)
				{
					configView.serverErrorMessages.push(this.texts.repeatedViewName)
					errorCount++
				}

				return errorCount
			},

			/**
			 * Validates the configuration views.
			 * @returns The number of validation errors.
			 */
			validateViews()
			{
				let errorCount = 0

				// Deactivate the watcher, to avoid triggering it by setting the error messages during validation.
				this.viewsWatcher()

				this.views.elements.forEach((e) => errorCount += this.validateView(e))
				this.views.newElements.forEach((e) => errorCount += e.isDirty ? this.validateView(e) : 0)

				// Reactivate the watcher.
				this.viewsWatcher = watch(
					() => this.views,
					() => this.updateViews(),
					{ deep: true })

				return errorCount
			},

			/**
			 * Emits the event to update the views.
			 */
			updateViews()
			{
				// If the views aren't dirty, or are invalid, we don't want to apply any changes.
				if (!this.views.isDirty || this.validateViews() > 0)
				{
					this.$emit('update:views', null)
					return
				}

				const views = []

				this.views.elements.forEach((e) => {
					if (e.id.value === 1)
						return

					const view = {
						name: e.name.value,
						oldName: e.name.originalValue,
						deleted: this.views.removedElements.includes(e.QPrimaryKey)
					}
					if (e.default.value)
						view.isSelected = 1

					views.push(view)
				})

				this.views.newElements.forEach((e) => {
					if (!e.isDirty)
						return

					const basedOn = this.getView(e.basedOn.value)
					// The base table's name is empty.
					const basedOnName = basedOn.id.value === 1 ? '' : basedOn.name.originalValue
					const view = {
						name: e.name.value,
						basedOn: basedOnName
					}
					if (e.default.value)
						view.isSelected = 1

					views.push(view)
				})

				this.$emit('update:views', views)
			}
		},

		watch: {
			configCurrentName(val)
			{
				this.views.elements.forEach((e) => {
					const name = e.id.value === 1 ? '' : e.name.originalValue
					e.selected.hydrate(name === val)
				})
			},

			configNames(names)
			{
				for (const configName of names)
				{
					// If the configuration view is already in the list, ignore it.
					if (this.views.elements.some((e) => e.name.originalValue === configName))
						continue

					const view = this.setView(this.views.rowCount + 1, configName)
					this.views.elements.push(view)
				}
			},

			newSelectedName(val)
			{
				this.$emit('set-current', val)
			},

			async 'views.emptyRows.length'()
			{
				this.views.newElements.forEach((e) => {
					// When a view is created, by editing the empty row, we need to set the
					// new view's properties, to indicate that it's based on the selected one.
					if (e.isDirty && e.basedOn.value === 0)
					{
						e.id.updateValue(this.views.rowCount)
						e.basedOn.updateValue(this.selectedView)
					}
				})

				this.setNewRecordTemplate()
				// Force the re-render of the name inputs, which is necessary to
				// avoid leaving the template row with the previous name value.
				this.domKey++

				await this.$nextTick()

				// Focus on the name input of the newly created configuration view.
				const newView = this.views.newElements.findLast((e) => e.isDirty)
				if (newView)
					document.getElementById(this.getNameInputId(newView))?.focus()
			},

			'views.newElements.length'()
			{
				const newView = this.views.newElements.find((e) => e.basedOn.value === this.selectedView)
				// If a new view was created based on the selected one, set it as the new selected.
				if (newView)
					this.newSelected = newView.id.value
				// Else, if there's no longer a new view based on the selected one, reset the property.
				else if (!this.views.newElements.some((e) => e.id.value === this.newSelected))
					this.newSelected = -1
			}
		}
	}
</script>
