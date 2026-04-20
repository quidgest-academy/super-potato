<template>
	<div
		:id="controlId"
		:class="['q-form-wizard-container', $attrs.class]">
		<div class="q-wizard-container">
			<div class="wizard-container">
				<div :class="wizardClasses">
					<q-wizard-steps
						v-if="hasSteps"
						:current-step="currentStep"
						:selected-step="selectedStep"
						:is-vertical="isVertical"
						:is-required="isRequired"
						:show-title="showTitle"
						:dynamic-steps="isDynamic"
						:blocked-steps="blockedSteps"
						:visible-steps="visibleSteps"
						:step-list="stepList"
						:current-path="currentPath"
						:texts="texts"
						@step-clicked="stepClicked" />
					<q-wizard-progress-bar
						v-else
						:current-step="currentStep"
						:step-list="stepList"
						:title="title" />

					<q-wizard-content
						:id="controlId"
						:order="selectedStep"
						:is-vertical="isVertical"
						:is-required="isRequired"
						:show-title="showTitle"
						:step-data="stepList[selectedStep - 1]"
						:base-heading-level="baseHeadingLevel">
						<slot></slot>
					</q-wizard-content>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'
	import { wizardTypes } from '@quidgest/clientapp/constants/enums'

	import QWizardContent from './QWizardContent.vue'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		showNextSteps: 'Click to show next steps',
		showPrevSteps: 'Click to show previous steps',
	}

	// The minimum width of the window (in pixels) for the steps to be vertical, at which point the wizard becomes horizontal.
	const MIN_VERTICAL_WIDTH = 767

	export default {
		name: 'QWizard',

		emits: ['step-clicked'],

		components: {
			QWizardSteps: defineAsyncComponent(() => import('./QWizardSteps.vue')),
			QWizardProgressBar: defineAsyncComponent(() => import('./QWizardProgressBar.vue')),
			QWizardContent
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Title of the wizard.
			 */
			title: String,

			/**
			 * The order of the current step (can range from 1 to N, where N is the size of stepList).
			 */
			currentStep: {
				type: Number,
				required: true,
				validator: (value) => value > 0
			},

			/**
			 * The order of the current step (can range from 1 to N, where N is the size of stepList).
			 */
			selectedStep: {
				type: Number,
				required: true,
				validator: (value) => value > 0
			},

			/**
			 * The type of the wizard (horizontal steps, vertical steps or progress bar).
			 */
			type: {
				type: String,
				required: true,
				validator: (value) => Reflect.has(wizardTypes, value)
			},

			/**
			 * Whether or not the title and caption of the steps should be shown.
			 */
			showTitle: {
				type: Boolean,
				default: true
			},

			/**
			 * Whether or not the not yet filled steps should be visible.
			 */
			isDynamic: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not the current step should be marked as required.
			 */
			isRequired: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not the not yet filled steps should be blocked.
			 */
			blockedSteps: {
				type: Boolean,
				default: false
			},

			/**
			 * The number of visible steps, if there are more steps a scroll will be created.
			 */
			visibleSteps: {
				type: Number,
				default: 5
			},

			/**
			 * The list of all the steps in the wizard.
			 */
			stepList: {
				type: Array,
				required: true
			},

			/**
			 * A list with the IDs of the steps that were activated.
			 */
			currentPath: {
				type: Array,
				default: () => []
			},

			/**
			 * Necessary strings to be used.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Top heading level.
			 */
			baseHeadingLevel: {
				type: Number,
				default: 2
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || `wizard-${this._.uid}`,
				windowWidth: window.innerWidth
			}
		},

		mounted()
		{
			window.addEventListener('resize', this.setWindowWidth)
		},

		beforeUnmount()
		{
			window.removeEventListener('resize', this.setWindowWidth)
		},

		computed: {
			/**
			 * The list of classes for the wizard container.
			 */
			wizardClasses()
			{
				const classes = []

				if (this.hasSteps)
				{
					classes.push('wizard-steps')

					if (this.isVertical)
						classes.push('vertical-steps')
					else
						classes.push('horizontal-steps')

					if (this.isDynamic)
						classes.push('dynamic-steps')
				}
				else
					classes.push('wizard-progress-container')

				return classes
			},

			/**
			 * True if the wizard has steps, false otherwise.
			 */
			hasSteps()
			{
				return this.type === wizardTypes.horizontal || this.type === wizardTypes.vertical
			},

			/**
			 * True if the wizard steps are vertical, false otherwise.
			 */
			isVertical()
			{
				return this.windowWidth > MIN_VERTICAL_WIDTH && this.type === wizardTypes.vertical
			}
		},

		methods: {
			/**
			 * Emits the step clicked event.
			 * @param {string} event The emitted info
			 */
			stepClicked(event)
			{
				this.$emit('step-clicked', event)
			},

			/**
			 * Sets the current width of the window.
			 */
			setWindowWidth()
			{
				this.windowWidth = window.innerWidth
			}
		}
	}
</script>
