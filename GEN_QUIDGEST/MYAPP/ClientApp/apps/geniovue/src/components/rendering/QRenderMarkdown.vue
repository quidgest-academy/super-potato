<template>
	<q-markdown-viewer :model-value="value" />
</template>

<script>
	import QMarkdownViewer from '@/components/markdown/QMarkdownViewer.vue'

	export default {
		name: 'QRenderMarkdown',

		components: {
			QMarkdownViewer
		},

		props: {
			/**
			 * The markdown content to render.
			 */
			value: {
				type: String,
				default: ''
			}
		},

		expose: []
	}
</script>
