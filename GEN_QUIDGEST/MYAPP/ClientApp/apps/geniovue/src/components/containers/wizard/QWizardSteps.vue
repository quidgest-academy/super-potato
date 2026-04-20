<template>
	<div :class="containerClasses">
		<template v-if="showScroll">
			<q-button
				class="horiz-carousel-btn carousel-control-prev"
				:style="{ 'pointer-events': canScrollLeft ? 'inherit' : 'none' }"
				:title="texts.showPrevSteps"
				:disabled="!canScrollLeft"
				@click.stop.prevent="scrollLeft">
				<span
					class="carousel-control-prev-icon"
					aria-hidden="true" />
			</q-button>

			<q-button
				class="horiz-carousel-btn carousel-control-next"
				:style="{ 'pointer-events': canScrollRight ? 'inherit' : 'none' }"
				:title="texts.showNextSteps"
				:disabled="!canScrollRight"
				@click.stop.prevent="scrollRight">
				<span
					class="carousel-control-next-icon"
					aria-hidden="true" />
			</q-button>
		</template>

		<ol :class="stepListClasses">
			<template
				v-for="(s, i) in activatedSteps"
				:key="`${s.order}-${s.isVisible}`">
				<q-wizard-step
					:order="i + 1"
					:show-title="showTitle"
					:is-required="isRequired && isStepSelected(stepList[s.order - 1])"
					:is-visible="s.isVisible"
					:is-filled="isStepFilled(stepList[s.order - 1])"
					:is-activated="isStepActivated(stepList[s.order - 1])"
					:is-selected="isStepSelected(stepList[s.order - 1])"
					:is-disabled="isStepDisabled(stepList[s.order - 1])"
					:step-data="stepList[s.order - 1]"
					@step-clicked="stepClicked" />
			</template>
		</ol>
	</div>
</template>

<script>
	import QWizardStep from './QWizardStep.vue'

	export default {
		name: 'QWizardSteps',

		emits: ['step-clicked'],

		components: {
			QWizardStep
		},

		props: {
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
			 * Whether or not the steps are vertical.
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
			 * Whether or not the not yet activated steps should be visible.
			 */
			dynamicSteps: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not the not yet activated steps should be blocked.
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
				required: true
			}
		},

		expose: [],

		data()
		{
			return {
				// A list with the orders and visibility of the activated steps.
				activatedSteps: []
			}
		},

		mounted()
		{
			this.setActivatedSteps()
		},

		computed: {
			/**
			 * The list of classes for the steps container.
			 */
			containerClasses()
			{
				const classes = ['step-buttons--container']

				if (this.isVertical)
				{
					if (this.showTitle)
					{
						classes.push('col-lg-2')
						classes.push('col-md-3')
					}
					else
					{
						classes.push('col-lg-1')
						classes.push('col-md-2')
					}

					classes.push('col-12')
				}

				return classes
			},

			/**
			 * The list of classes for the list of steps.
			 */
			stepListClasses()
			{
				const classes = ['step-buttons']

				if (this.isVertical)
					classes.push('q-wizard__steps--vertical')
				else
					classes.push('q-wizard__steps--horizontal')

				return classes
			},

			/**
			 * True if the scroll arrows should be visible, false otherwise.
			 */
			showScroll()
			{
				return !this.isVertical && this.activatedSteps.length > this.visibleSteps
			},

			/**
			 * True if the left scroll arrow should be enabled, false otherwise.
			 */
			canScrollLeft()
			{
				return this.activatedSteps.length > 0 && !this.activatedSteps[0].isVisible
			},

			/**
			 * True if the right scroll arrow should be enabled, false otherwise.
			 */
			canScrollRight()
			{
				return this.activatedSteps.length > 0 && !this.activatedSteps[this.activatedSteps.length - 1].isVisible
			}
		},

		methods: {
			/**
			 * Emits the step clicked event.
			 */
			stepClicked(event)
			{
				this.$emit('step-clicked', event)
			},

			/**
			 * Checks if the provided step has already been activated or not.
			 * @param {object} step The step data
			 * @returns True if the step has been activated, false otherwise.
			 */
			isStepActivated(step)
			{
				return this.currentPath.includes(step.route)
			},

			/**
			 * Checks if the provided step is filled or not.
			 * @param {object} step The step data
			 * @returns True if the step is filled, false otherwise.
			 */
			isStepFilled(step)
			{
				return this.isStepActivated(step) && this.currentStep !== step.order
			},

			/**
			 * Checks if the provided step is selected or not.
			 * @param {object} step The step data
			 * @returns True if the step is selected, false otherwise.
			 */
			isStepSelected(step)
			{
				return this.selectedStep === step.order
			},

			/**
			 * Checks if the provided step is disabled or not.
			 * @param {object} step The step data
			 * @returns True if the step is disabled, false otherwise.
			 */
			isStepDisabled(step)
			{
				return this.blockedSteps && !this.isStepActivated(step)
			},

			/**
			 * Checks if the provided step is visible or not.
			 * @param {object} step The step data
			 * @returns True if the step is visible, false otherwise.
			 */
			isStepVisible(step)
			{
				for (const s of this.activatedSteps)
					if (s.order === step.order)
						return s.isVisible
				return false
			},

			/**
			 * Scrolls the steps to the left.
			 */
			scrollLeft()
			{
				let visibleCount = 0

				for (let i = 0; i < this.activatedSteps.length; i++)
				{
					if (this.activatedSteps[i].isVisible)
					{
						visibleCount++
						if (visibleCount > this.visibleSteps)
							this.activatedSteps[i].isVisible = false
					}
					else if (i < this.activatedSteps.length - 1 && this.activatedSteps[i + 1].isVisible)
					{
						this.activatedSteps[i].isVisible = true
						visibleCount++
					}
				}
			},

			/**
			 * Scrolls the steps to the right.
			 */
			scrollRight()
			{
				let visibleCount = 0

				for (let i = this.activatedSteps.length - 1; i >= 0; i--)
				{
					if (this.activatedSteps[i].isVisible)
					{
						visibleCount++
						if (visibleCount > this.visibleSteps)
							this.activatedSteps[i].isVisible = false
					}
					else if (i > 0 && this.activatedSteps[i - 1].isVisible)
					{
						this.activatedSteps[i].isVisible = true
						visibleCount++
					}
				}
			},

			/**
			 * Brings the specified step into focus.
			 * @param {object} step The step data
			 */
			focusOnStep(step)
			{
				if (!this.showScroll || this.isStepVisible(step))
					return

				let firstVisible, lastVisible

				// Sets the first and last visible steps.
				for (const s of this.activatedSteps)
				{
					if (s.isVisible)
					{
						if (!firstVisible)
							firstVisible = s
						lastVisible = s
					}
				}

				if (firstVisible && lastVisible)
				{
					while (!this.isStepVisible(step))
					{
						if (step.order < firstVisible.order)
							this.scrollLeft()
						else if (step.order > lastVisible.order)
							this.scrollRight()
						else
							break
					}
				}
			},

			/**
			 * Populates the list of activated steps.
			 */
			setActivatedSteps()
			{
				this.activatedSteps = []

				for (const step of this.stepList)
				{
					if (!this.dynamicSteps || this.isStepActivated(step))
					{
						const isVisible = this.isVertical || this.activatedSteps.length < this.visibleSteps
						const stepData = {
							order: step.order,
							isVisible
						}

						this.activatedSteps.push(stepData)
					}
				}

				const selectedStep = this.stepList[this.selectedStep - 1]
				this.focusOnStep(selectedStep)
			}
		},

		watch: {
			isVertical()
			{
				this.setActivatedSteps()
			}
		}
	}
</script>
