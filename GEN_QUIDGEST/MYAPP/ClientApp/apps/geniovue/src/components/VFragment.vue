<template>
	<slot></slot>
</template>

<script>
	/*
	 * Used like a container element but without creating a container element.
	 * Used to add elements or text and conditionally create a container element.
	 */
	export default {
		name: 'VFragment',

		inheritAttrs: false,

		expose: []
	}
</script>