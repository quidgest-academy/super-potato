<template>
	<div
		class="grid-stack-item"
		v-bind="params">
		<div :class="gridStackContentClasses">
			<div class="q-widget__menu">
				<span
					v-if="showGroupTitle"
					class="q-widget__group">
					{{ group.title }}
				</span>

				<div
					v-if="showMenubar"
					class="q-widget__menubar">
					<div
						v-if="showPaginationButtons"
						class="custom-widget-pagination">
						<q-button
							borderless
							:title="texts.previousPageText"
							:disabled="!hasPrev"
							@click="prev">
							<q-icon icon="step-back" />
						</q-button>

						<q-button
							borderless
							:title="texts.nextPageText"
							:disabled="!hasNext"
							@click="next">
							<q-icon icon="step-forward" />
						</q-button>
					</div>

					<q-button
						v-if="showRemoveButton"
						borderless
						:title="texts.removeButtonText"
						@click="onDeleteWidget">
						<q-icon icon="delete" />
					</q-button>

					<q-button
						v-if="showRefreshButton"
						borderless
						:title="texts.refreshButtonText"
						@click="refresh">
						<q-icon icon="reset" />
					</q-button>
				</div>
			</div>

			<div
				v-if="widget.data !== null"
				class="q-widget__content">
				<div
					v-if="config.inEditMode"
					class="q-widget__overlay"></div>
				<slot
					v-bind="$props"
					:page="page"
					:dom-version-key="domVersionKey" />
			</div>
			<div
				v-else
				class="q-widget__overlay">
				<q-spinner-loader />
			</div>
		</div>
	</div>
</template>

<script>
	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		previousPageText: 'Previous page',
		nextPageText: 'Next page',
		removeButtonText: 'Remove',
		refreshButtonText: 'Refresh'
	}

	export default {
		name: 'QWidget',

		emits: [
			'fetch-data',
			'delete-widget',
			'record-change'
		],

		inheritAttrs: false,

		props: {
			/**
			 * The core object of the widget containing its state and configuration.
			 */
			widget: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The group information object for the widget, often containing title and identifier.
			 */
			group: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Overall configuration object for the widget, possibly containing edit states and other settings.
			 */
			config: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Localized text strings to be used for button titles and other widget text content.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * The color to be used for the widget's border, represented as a string.
			 */
			borderColor: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data()
		{
			return {
				fetchedData: null,
				page: 0,
				domVersionKey: 0
			}
		},

		mounted()
		{
			this.setInitialPage()

			if (
				this.widget.RefreshMode === 2 &&
				this.widget.RefreshRate > 0 &&
				!this.config.inEditMode
			)
			{
				// Automatic (periodic) refresh.
				this.startRefreshWorker()
			}
		},

		computed: {
			/**
			 * Object containing parameters bound to the widget template's root div element.
			 */
			params()
			{
				const props = {
					id: this.widget.uuid,
					'gs-id': this.widget.uuid,
					'gs-w': this.widget.Width,
					'gs-h': this.widget.Height
				}

				if (this.widget.Hposition >= 0)
					props['gs-x'] = this.widget.Hposition

				if (this.widget.Vposition >= 0)
					props['gs-y'] = this.widget.Vposition

				if (this.widget.Rowkey)
					props['class'] = [`w-${this.widget.Id}`]

				return props
			},

			/**
			 * An array of class names to be applied to the grid stack content holding the widget.
			 */
			gridStackContentClasses()
			{
				const baseClass = 'q-widget'
				const classes = ['grid-stack-item-content', baseClass, this.styleClass]

				if (this.borderColor)
					classes.push(`${baseClass}--border-${this.borderColor}`)

				// Alert widget with the apply color set to the background.
				if (this.widget.Type === 0 && this.widget?.ApplyColorTo === 0)
					classes.push(`q-widget--${this.borderColor}`)

				return classes
			},

			/**
			 * The style class to be applied to the widget based on it's Style property.
			 */
			styleClass()
			{
				const classes = {
					primary: 'q-widget--primary',
					secondary: 'q-widget--secondary',
					info: 'q-widget--info',
					success: 'q-widget--success',
					warning: 'q-widget--warning',
					danger: 'q-widget--danger',
					neutral: 'q-widget--neutral'
				}

				return classes[this.widget.Style] || ''
			},

			/**
			 * Indicates whether the widget's keys allow for pagination controls.
			 */
			hasPagination()
			{
				return this.widget.Keys && this.widget.Keys.length > 1
			},

			/**
			 * Determines if the "previous page" pagination button should be enabled.
			 */
			hasPrev()
			{
				return this.page > 0
			},

			/**
			 * Whether to show the group title.
			 */
			showGroupTitle()
			{
				return !this.group.hideGroup && this.group.title
			},

			/**
			 * Determines if the "next page" pagination button should be enabled.
			 */
			hasNext()
			{
				if (this.widget.Keys)
					return this.page + 1 < this.widget.Keys.length
				return false
			},

			/**
			 * Whether to show the pagination buttons.
			 */
			showPaginationButtons()
			{
				const inEditModeWithPagination = this.config.inEditMode && this.widget.Type === 3 && this.hasPagination
				const AggregatedWidgetWithPagination = this.widget.InstantionMethod === 0 && this.widget.Type === 3 && this.hasPagination

				return inEditModeWithPagination || AggregatedWidgetWithPagination
			},

			/**
			 * Whether to show the pagination buttons.
			 */
			showRemoveButton()
			{
				return !this.widget.Required && this.config.inEditMode
			},

			/**
			 * Whether to show the pagination buttons.
			 */
			showRefreshButton()
			{
				return this.widget.RefreshMode === 1 && !this.config.inEditMode
			},

			/**
			 * Whether to show the pagination buttons.
			 */
			showMenubar()
			{
				return this.showPaginationButtons
					|| this.showRemoveButton
					|| this.showRefreshButton
			}
		},

		methods: {
			/**
			 * Fetch data corresponding to the widget.
			 */
			fetchWidgetData()
			{
				this.$emit('fetch-data', this.widget)
			},

			/**
			 * Sets initial page number by reading the position of Rowkey within the widget Keys, if present.
			 */
			setInitialPage()
			{
				if (!this.widget.Keys)
					return

				const idx = this.widget.Keys.indexOf(this.widget.Rowkey)
				if (idx !== -1)
					this.page = idx
			},

			/**
			 * Starts an interval to refresh the widget based on its RefreshRate, if applicable.
			 */
			startRefreshWorker()
			{
				if (!this.refreshWorker)
				{
					// Refresh rate in seconds converted to milliseconds.
					const refreshRate = this.widget.RefreshRate * 1000
					this.refreshWorker = setInterval(() => this.refresh(), refreshRate)
				}
			},

			/**
			 * Stops the interval that refreshes the widget.
			 */
			stopRefreshWorker()
			{
				if (this.refreshWorker)
				{
					clearInterval(this.refreshWorker)
					this.refreshWorker = null
				}
			},

			/**
			 * Decrement the current page index of the widget's pagination, if possible.
			 */
			prev()
			{
				if (this.page > 0)
				{
					this.page--
					this.$emit('record-change', this.widget.Keys[this.page])
				}
			},

			/**
			 * Increment the current page index of the widget's pagination, if possible.
			 */
			next()
			{
				if (this.widget.Keys.length > this.page + 1)
				{
					this.page++
					this.$emit('record-change', this.widget.Keys[this.page])
				}
			},

			/**
			 * Refreshes the widget's data.
			 * Custom widgets reload by changing a version key to force the widget form to update
			 * Other widgets fetch the data normally.
			 */
			refresh()
			{
				if (this.widget.Type !== 2 && this.widget.Type !== 3) {
					this.fetchWidgetData()
				}
				else {
					this.domVersionKey++
				}
			},

			onDeleteWidget()
			{
				this.$emit('delete-widget', this.widget.uuid)
			}
		},

		watch: {
			widget: {
				handler()
				{
					if (!this.widget.Visible)
						this.stopRefreshWorker()

					this.setInitialPage()
				},
				deep: true
			},

			config()
			{
				if (this.widget.RefreshMode === 2 && this.config.inEditMode)
					this.stopRefreshWorker()
				else if (
					this.widget.RefreshMode === 2 &&
					this.widget.RefreshRate > 0 &&
					!this.config.inEditMode
				)
				{
					// Automatic (periodic) refresh
					this.startRefreshWorker()
				}
			}
		}
	}
</script>
