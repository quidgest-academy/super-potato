<template>
	<!-- Must use v-on object syntax so that when the condition is false, the handler will not be added -->
	<!-- If the shortcut syntax is used, the handler will be added even when the condition is false -->
	<li
		:class="stepClasses"
		v-on="stepHandlers">
		<component
			:is="isActivated && !isSelected ? 'a' : 'span'"
			:href="isActivated && !isSelected ? '#' : null"
			role="button"
			:title="stepData.title">
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
		</component>

		<template v-if="showTitle">
			<span
				v-if="stepData.title"
				class="btn q-wizard__step-link">
				{{ stepData.title }}

				<q-icon
					v-if="isFilled"
					icon="success" />
				<q-icon
					v-else-if="isRequired || stepData.isRequired"
					icon="mandatory" />
			</span>

			<label
				v-if="stepData.caption"
				class="wizard-step-caption">
				{{ stepData.caption }}
			</label>
		</template>
	</li>
</template>

<script>
	export default {
		name: 'QWizardStep',

		emits: ['step-clicked'],

		props: {
			/**
			 * The order of the step.
			 */
			order: {
				type: Number,
				required: true
			},

			/**
			 * Whether or not the title and caption of the step should be shown.
			 */
			showTitle: {
				type: Boolean,
				default: true
			},

			/**
			 * Whether or not the step is already filled.
			 */
			isFilled: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not the step has already been activated.
			 */
			isActivated: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not the step is currently selected.
			 */
			isSelected: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not the step is disabled.
			 */
			isDisabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not the step is visible.
			 */
			isVisible: {
				type: Boolean,
				default: true
			},

			/**
			 * Whether or not the current step should be marked as required.
			 */
			isRequired: {
				type: Boolean,
				default: false
			},

			/**
			 * The data about this step.
			 */
			stepData: {
				type: Object,
				required: true,
				validator: (value) => value && Reflect.has(value, 'route') && Reflect.has(value, 'isRequired')
			}
		},

		expose: [],

		computed: {
			/**
			 * The list of classes for the step.
			 */
			stepClasses()
			{
				const classes = ['step-link', 'q-wizard__step']

				if (this.isActivated || this.isClickable)
					classes.push('filled-step')

				if (this.isSelected)
					classes.push('current-step')

				if (this.isDisabled)
					classes.push('disabled')

				if (!this.isVisible)
					classes.push('hidden')

				return classes
			},

			/**
			 * Whether or not the step is clickable.
			 */
			isClickable()
			{
				return this.isActivated && !this.isSelected || !this.isDisabled
			},

			/**
			 * The object of handlers for the step.
			 */
			stepHandlers()
			{
				return { click: this.isClickable ? (event) => this.stepClicked(event) : null }
			}
		},

		methods: {
			/**
			 * Emits the step clicked event.
			 */
			stepClicked(event)
			{
				event.stopPropagation()
				event.preventDefault()
				this.$emit('step-clicked', this.stepData.route)
			}
		}
	}
</script>
