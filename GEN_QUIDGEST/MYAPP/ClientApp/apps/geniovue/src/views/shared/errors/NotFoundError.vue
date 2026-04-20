<template>
	<error
		image-name="not-found.png"
		:error-title="errorTitle" />
</template>

<script>
	import hardcodedTexts from '@/hardcodedTexts.js'

	import Error from './Error.vue'

	export default {
		name: 'QNotFoundError',

		components: {
			Error
		},

		expose: [],

		computed: {
			errorTitle()
			{
				return this.Resources[hardcodedTexts.recordNotFound]
			}
		}
	}
</script>
