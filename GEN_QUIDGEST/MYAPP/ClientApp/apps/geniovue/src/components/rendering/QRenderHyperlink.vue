<template>
	<a
		v-if="value !== ''"
		:href="value"
		target="_blank"
		rel="noopener noreferrer"
		class="column-data-link"
		data-table-action-selected="false"
		tabindex="-1"
		@click.stop>
		{{ value }}
	</a>
</template>

<script>
	export default {
		name: 'QRenderHyperlink',

		props: {
			/**
			 * The hyperlink URL to be rendered inside an anchor tag. Displayed as clickable text that opens the URL in a new tab.
			 */
			value: {
				type: String,
				default: ''
			}
		},

		expose: []
	}
</script>
