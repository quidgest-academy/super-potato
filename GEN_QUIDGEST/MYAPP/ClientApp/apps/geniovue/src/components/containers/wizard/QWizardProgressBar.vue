<template>
	<div class="col-12">
		<div class="wizard-progress">
			<div
				class="progress"
				role="progressbar"
				aria-valuemin="0"
				aria-valuemax="100"
				:aria-valuenow="progressPercent"
				:aria-valuetext="progressPercent + '%'"
				:aria-label="title">
				<div
					class="q-progress progress-bar progress-bar-striped"
					:style="progressStyle" />
			</div>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QWizardProgressBar',

		props: {
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
			 * The list of all the steps in the wizard.
			 */
			stepList: {
				type: Array,
				required: true
			}
		},

		expose: [],

		computed: {
			/**
			 * The style of the progress bar.
			 */
			progressStyle()
			{
				return {
					width: `${this.progressPercent}%`,
					pointerEvents: 'none'
				}
			},

			/**
			 * The progress as a percent.
			 */
			progressPercent()
			{
				return (this.currentStep / this.stepList.length) * 100
			}
		}
	}
</script>
