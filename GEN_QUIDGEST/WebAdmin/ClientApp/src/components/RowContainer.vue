<template>
	<div :class="['control-row-group', { 'row-large-control': isLarge }]">
		<slot></slot>
	</div>
</template>

<script>
	export default {
		name: 'QRow',

		props: {
			/**
			 * Flag to determine if the row should be rendered as large.
			 */
			isLarge: {
				type: Boolean,
				default: false
			}
		},

		expose: []
	}
</script>
