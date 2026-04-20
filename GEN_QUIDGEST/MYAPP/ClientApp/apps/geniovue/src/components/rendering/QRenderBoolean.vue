<template>
	<q-icon
		:icon="icon"
		:class="iconClasses"
		:alt="valueText"
		:aria-label="valueText" />
</template>

<script>
	import { BooleanResources } from '@/mixins/controlsResources.js'

	export default {
		name: 'QRenderBoolean',

		props: {
			/**
			 * The boolean value to determine the icon state.
			 */
			value: {
				type: Boolean,
				default: false
			},

			/**
			 * Localized text strings to be used within the component.
			 */
			texts: {
				type: Object,
				required: true
			},
		},

		expose: [],

		computed: {
			/**
			 * The icon to be rendered depending on the boolean value.
			 * 'ok' for true values, 'close' for false.
			 */
			icon()
			{
				return this.value ? 'ok' : 'close'
			},

			iconClasses()
			{
				const classes = ['render-boolean']

				this.value ? classes.push('true-icon') : classes.push('false-icon')

				return classes
			},

			resolvedTexts() {
				if (this.texts) return this.texts

				return new BooleanResources(this.$getResource)
			},

			valueText()
			{
				return this.value ? this.resolvedTexts.yesLabel : this.resolvedTexts.noLabel
			}
		}
	}
</script>
