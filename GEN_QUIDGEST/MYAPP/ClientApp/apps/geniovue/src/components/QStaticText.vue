<template>
	<div
		v-if="supportsHtml"
		:class="styleClasses"
		v-html="text" />
	<div
		v-else
		:class="styleClasses">
		{{ text }}
	</div>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@quidgest/clientapp/constants/enums'

	export default {
		name: 'QStaticText',

		inheritAttrs: false,

		props: {
			/**
			 * The text to be presented.
			 */
			text: {
				type: String,
				required: true
			},

			/**
			 * Whether or not it supports HTML.
			 */
			supportsHtml: {
				type: Boolean,
				default: false
			},

			/**
			 * The sizing class for the control.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			}
		},

		expose: [],

		computed: {
			/**
			 * An array with all the style classes of the component.
			 */
			styleClasses()
			{
				const classes = ['i-static-text']

				if (this.size)
					classes.push(`input-${this.size}`)

				return classes
			}
		}
	}
</script>
