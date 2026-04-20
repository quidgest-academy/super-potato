<template>
	<div
		v-if="value"
		class="q-render__array">
		<q-icon
			v-if="value.icon && showIcon"
			v-bind="value.icon" />

		<span
			v-if="showCode"
			:style="textElementStyle"
			:class="textElementClasses"
			:data-field-value="true">
			{{ value.key }}
		</span>

		<span
			v-if="showDescription"
			:style="textElementStyle"
			:class="textElementClasses"
			:data-field-value="true">
			{{ value.value }}
		</span>
	</div>
</template>

<script>
	export default {
		name: 'QRenderArray',

		inheritAttrs: false,

		props: {
			/**
			 * The value of the array element, which may include key, value, and icon.
			 */
			value: {
				type: [Object, Array],
				default: () => ({})
			},

			/**
			 * Configurable options for rendering array elements.
			 */
			options: {
				type: Object,
				required: true
			},

			/**
			 * A background color to apply to the text elements if specified.
			 */
			backgroundColor: {
				type: String,
				default: ''
			}
		},

		expose: [],

		computed: {
			/**
			 * The display mode of the array element.
			 *
			 * Possible values:
			 * - D  (Description)
			 * - C  (Code)
			 * - I  (Image)
			 * - ID (Image + Description)
			 * - CD (Code + Description)
			 */
			mode()
			{
				return this.options?.arrayDisplayMode ?? ''
			},

			/**
			 * Whether to show the icon of the array element.
			 * The icon should the displayed if the mode starts with I.
			 */
			showIcon()
			{
				return this.mode.startsWith('I')
			},

			/**
			 * Whether to show the internal code of the array element.
			 * The code should the displayed if the mode starts with C.
			 */
			showCode()
			{
				return this.mode.startsWith('C')
			},

			/**
			 * Whether to show the description of the array element.
			 * The description should the displayed if no display mode is specified,
			 * or if it ends with D.
			 */
			showDescription()
			{
				return this.mode === '' || this.mode.endsWith('D')
			},

			/**
			 * The classes to apply to the text elements.
			 */
			textElementClasses()
			{
				return this.backgroundColor ? ['e-badge'] : []
			},

			/**
			 * The style to apply to the text elements.
			 */
			textElementStyle()
			{
				return this.backgroundColor
					? { 'background-color': this.backgroundColor }
					: null
			}
		}
	}
</script>
