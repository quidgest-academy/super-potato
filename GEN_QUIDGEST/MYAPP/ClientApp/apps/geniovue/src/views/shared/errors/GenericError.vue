<template>
	<error
		image-name="generic-error.png"
		:error-title="errorTitle" />
</template>

<script>
	import hardcodedTexts from '@/hardcodedTexts.js'

	import Error from './Error.vue'

	export default {
		name: 'QGenericError',

		components: {
			Error
		},

		expose: [],

		computed: {
			errorTitle()
			{
				return this.Resources[hardcodedTexts.errorProcessingRequest]
			}
		}
	}
</script>
