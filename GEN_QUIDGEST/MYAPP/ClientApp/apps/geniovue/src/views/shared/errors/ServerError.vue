<template>
	<error
		image-name="server-error.png"
		:error-title="errorTitle" />
</template>

<script>
	import hardcodedTexts from '@/hardcodedTexts.js'

	import Error from './Error.vue'

	export default {
		name: 'QServerError',

		components: {
			Error
		},

		expose: [],

		computed: {
			errorTitle()
			{
				return this.Resources[hardcodedTexts.internalServerError]
			}
		}
	}
</script>
