<template>
	<q-overlay
		:model-value="show"
		backdrop-blur
		scroll-lock
		persistent>
		<div class="progress-bar__container">
			<div
				v-if="withTitle"
				class="c-modal__header progress-bar__header">
				<h3 class="c-modal__header-title">
					{{ text }}
				</h3>
				<span
					v-if="subtext"
					class="progress-bar__subtitle">
					{{ subtext }}
				</span>
			</div>
			<div class="c-modal__body">
				<div class="progress">
					<div
						class="progress-bar progress-bar-striped progress-bar-animated"
						:style="{ width: progress + '%' }">
						<template v-if="!withTitle">{{ text }}</template>
					</div>
				</div>
			</div>

			<!-- Button -->
			<div
				v-if="withButton"
				class="progress-bar__footer">
				<q-button
					variant="bold"
					:label="buttonText"
					@click="clickButton" />
			</div>
		</div>
	</q-overlay>
</template>

<script>
	export default {
		props: {
			/**
			 * True if the progress bar should be displayed, false otherwise.
			 */
			show: {
				type: Boolean,
				default: false
			},

			/**
			 * Progress percentage (0 to 100).
			 */
			progress: {
				type: Number,
				default: 0
			},

			/**
			 * Progress bar label.
			 */
			text: {
				type: String,
				default: ''
			},

			/**
			 * Progress bar sub-label.
			 */
			subtext: {
				type: String,
				default: ''
			},

			/**
			 * True if the progress bar label should be displayed as the popup title, false to be displayed within the bar.
			 */
			withTitle: {
				type: Boolean,
				default: true
			},

			/**
			 * True if the progress bar should have a fotter button, false otherwise.
			 */
			withButton: {
				type: Boolean,
				default: false
			},

			/**
			 * Footer button label.
			 */
			buttonText: {
				type: String,
				default: ''
			}
		},

		emits: ['on-button-click'],

		expose: [],

		methods: {
			clickButton() {
				this.$emit('on-button-click')
			}
		}
	}
</script>
