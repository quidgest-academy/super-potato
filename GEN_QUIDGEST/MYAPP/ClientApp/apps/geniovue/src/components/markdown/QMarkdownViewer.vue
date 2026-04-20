<template>
	<span
		:id="id"
		class="q-markdown-viewer"
		v-html="htmlContent">
	</span>
</template>

<script>
	import { MarkdownOptions, convertMarkdown } from './markdown.js'

	export default {
		name: 'QMarkdownViewer',

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The value bound to the control.
			 */
			modelValue: {
				type: String,
				default: ''
			},

			/**
			 * Options for Markdown (which may disable certain functionalities)
			 */
			options: {
				type: Object,
				default: () => new MarkdownOptions()
			}
		},

		expose: [],

		computed: {
			htmlContent()
			{
				return convertMarkdown(this.modelValue, this.options)
			}
		}
	}
</script>
