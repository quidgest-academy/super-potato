<template>
	<p v-html="value"></p>
</template>

<script>
	export default {
		name: 'QRenderHtml',

		props: {
			/**
			 * The html to render.
			 */
			value: {
				type: String,
				default: ''
			}
		},

		expose: []
	}
</script>
