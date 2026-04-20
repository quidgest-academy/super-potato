<template>
	<div :class="containerClasses">
		<div class="wizard-area q-wizard__content compact">
			<div class="q-wizard__content-title step-title">
				<span
					v-if="stepData.icon"
					class="q-wizard__step-icon">
					<q-icon v-bind="stepData.icon" />
				</span>
				<span
					v-else
					class="q-wizard__step-number">
					{{ order }}
				</span>

				<component
					v-if="stepData.title || stepData.isRequired"
					:is="topHeadingTag">
					<template v-if="stepData.title">
						<template v-if="stepData.icon"> &nbsp; </template>

						{{ stepData.title }}
					</template>

					<span
						v-if="isRequired || stepData.isRequired"
						class="required-step-header">
						&nbsp;*
					</span>
				</component>
			</div>

			<div
				:id="controlId"
				class="wizard-content">
				<q-subtitle-help :help-control="helpControlObject" />

				<div class="step wizard-step">
					<div class="f-wizard-body">
						<slot></slot>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import { getHeadingTagNameByLevel } from '@quidgest/clientapp/utils/genericFunctions'
	import HelpControl from '@/mixins/helpControls.js'

	export default {
		name: 'QWizardContent',

		mixins: [HelpControl],

		components: {
			QSubtitleHelp: defineAsyncComponent(() => import('@/components/QSubtitleHelp.vue'))
		},

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The order of the step.
			 */
			order: {
				type: Number,
				required: true,
				validator: (value) => value > 0
			},

			/**
			 * Whether or not the wizard is vertical.
			 */
			isVertical: {
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
			 * Whether or not the title and caption of the steps should be shown.
			 */
			showTitle: {
				type: Boolean,
				default: true
			},

			/**
			 * The data about the currently selected step.
			 */
			stepData: {
				type: Object,
				required: true,
				validator: (value) => Reflect.has(value, 'route') && Reflect.has(value, 'isRequired')
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

		computed: {
			/**
			 * The list of classes for the wizard container.
			 */
			containerClasses()
			{
				const classes = ['q-wizard__content-container']

				if (this.isVertical)
				{
					if (this.showTitle)
					{
						classes.push('col-lg-10')
						classes.push('col-md-9')
					}
					else
					{
						classes.push('col-lg-11')
						classes.push('col-md-10')
					}

					classes.push('col-sm-12')
				}

				return classes
			},

			/**
			 * The help object for wizard steps.
			 */
			helpControlObject()
			{
				return this.stepData.helpControl
			},

			/**
			 * The id of div step for show help.
			 */
			controlId()
			{
				if (this.id)
					return this.id + this.stepData.order
				return null
			},

			/**
			 * The top level heading tag name.
			 */
			topHeadingTag()
			{
				return getHeadingTagNameByLevel(this.baseHeadingLevel)
			}
		}
	}
</script>
