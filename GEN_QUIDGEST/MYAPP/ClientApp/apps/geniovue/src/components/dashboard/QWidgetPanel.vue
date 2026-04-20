<template>
	<div class="q-widget-panel">
		<h2>Widgets</h2>

		<p class="q-widget-panel__help">
			{{ helpText }}
		</p>

		<ul class="q-widget-panel__list nav nav-treeview">
			<li
				v-for="group in panelGroups"
				:key="group.id"
				role="option"
				:class="getGroupClasses(group)">
				<div
					class="q-widget-panel__group-btn"
					@click.stop.prevent="onGroupClick(group)">
					<p>{{ group.title }}</p>

					<span class="q-widget-panel__instance-count">
						{{ group.widgets.length }}
					</span>

					<div style="margin-left: auto">
						<q-icon :icon="group.open ? 'collapse' : 'expand'" />
					</div>
				</div>

				<ul
					v-if="group.open"
					class="q-widget-panel__group-instances nav nav-treeview">
					<li
						v-for="widget in group.widgets"
						:key="widget.uuid"
						role="option"
						:class="getWidgetClasses(widget)"
						draggable="true"
						@click.stop.prevent="toggleSelected(widget)"
						@dragstart="onDragStart($event, widget)">
						<p class="q-widget-panel__instance-name">
							{{ widget.title }}
						</p>
					</li>
				</ul>
			</li>
		</ul>

		<q-button
			:disabled="!selected.length"
			:label="texts.addButtonTitle"
			@click="addSelected">
			<q-icon icon="add" />
		</q-button>
	</div>
</template>

<script>
	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		helpText: 'To add a widget to the dashboard, drag and drop or select a widget and click %s.',
		addButtonTitle: 'Add'
	}

	export default {
		name: 'QWidgetPanel',

		emits: ['add-widget', 'dragstart'],

		inheritAttrs: false,

		props: {
			/**
			 * Contains all available widgets.
			 */
			widgets: {
				type: Array,
				default: () => []
			},

			/**
			 * Contains all available groups.
			 */
			groups: {
				type: Array,
				default: () => []
			},

			/**
			 * Contains text fields: 'helpText' and 'addButtonTitle', defaults to DEFAULT_TEXTS.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			}
		},

		expose: [],

		data()
		{
			return {
				selected: [],
				panelGroups: []
			}
		},

		mounted()
		{
			this.panelGroups = this.getPanelGroups()
		},

		computed: {
			/**
			 * The help text, where the '%s' placeholder is replaced by the actual add button title.
			 */
			helpText()
			{
				return this.texts.helpText.replace('%s', `"${this.texts.addButtonTitle}"`)
			}
		},

		methods: {
			/**
			 * Deselects all selected widgets and emits 'add-widget' event for each one.
			 */
			addSelected()
			{
				this.selected.forEach((widget) => {
					widget.selected = false
					this.$emit('add-widget', widget.uuid)
				})
				this.selected.length = 0
			},

			/**
			 * Generates and return group panels consisting only non required widgets.
			 * @returns {Array} Returns a new array containing groups of non-required widgets.
			 */
			getPanelGroups()
			{
				const nonRequiredWidgets = this.widgets.filter((widget) => !widget.Required)
				const requiredGroups = this.groups.filter((group) =>
					nonRequiredWidgets.some((widget) => widget.Group === group.id)
				)
				const groups = requiredGroups
					.map((obj) => ({
						...obj,
						open: false,
						widgets: []
					}))
					.sort((a, b) => a.order - b.order)

				this.updatePanelGroups(groups)
				return groups
			},

			/**
			 * Updates 'panelGroups' array with non-visible widgets.
			 * @param {Array} groups - all available groups.
			 */
			updatePanelGroups(groups)
			{
				this.panelGroups.forEach((group) => {
					group.widgets = []
				})

				this.widgets
					.filter((w) => !w.Visible)
					.forEach((widget) => {
						const group = groups.find((g) => g.id === widget.Group)

						if (group)
						{
							group.widgets.push({
								uuid: widget.uuid,
								title: widget.Title,
								selected: false
							})
						}
					})
			},

			/**
			 * Returns CSS classes based on the state of a provided group.
			 * @param {object} group - the group to be evaluated.
			 * @returns {Array} An array of CSS class names as strings.
			 */
			getGroupClasses(group)
			{
				const baseClass = 'q-widget-panel__group'
				const classes = [baseClass, 'q-widget-panel__item']

				if (!group.widgets.length)
					classes.push(`${baseClass}-disabled`)

				if (group.open)
					classes.push(`${baseClass}-expanded`)

				return classes
			},

			/**
			 * Returns CSS classes based on the state of a provided widget.
			 * @param {object} widget - the widget to be evaluated.
			 * @returns {Array} An array of CSS class names as strings.
			 */
			getWidgetClasses(widget)
			{
				const baseClass = 'q-widget-panel__instance'
				const classes = [baseClass, 'q-widget-panel__item']

				if (widget.selected)
					classes.push(`${baseClass}-selected`)

				return classes
			},

			/**
			 * Handles 'group' click events, toggles its open state and closes others.
			 * @param {object} group - The group that was clicked.
			 */
			onGroupClick(group)
			{
				const open = group.open

				this.panelGroups.forEach((group) => {
					group.open = false
				})

				group.open = !open
			},

			/**
			 * Toggles selected state of a widget and updates the 'selected' array.
			 * @param {object} widget - The widget to be evaluated.
			 */
			toggleSelected(widget)
			{
				widget.selected = !widget.selected

				if (widget.selected)
					this.selected.push(widget)
				else
				{
					const idx = this.selected.indexOf(widget)
					this.selected.splice(idx, 1)
				}
			},

			/**
			 * Handles start of widget drag, emits 'dragstart' event.
			 * @param {Event} ev - The event that was triggered.
			 * @param {object} widget - The widget that the event was triggered on.
			 */
			onDragStart(ev, widget)
			{
				this.$emit('dragstart', {
					ev: ev,
					uuid: widget.uuid
				})

				if (widget.selected)
				{
					widget.selected = false
					const idx = this.selected.indexOf(widget)
					this.selected.splice(idx, 1)
				}
			}
		},

		watch: {
			widgets: {
				handler()
				{
					this.updatePanelGroups(this.panelGroups)
				},
				deep: true
			}
		}
	}
</script>
