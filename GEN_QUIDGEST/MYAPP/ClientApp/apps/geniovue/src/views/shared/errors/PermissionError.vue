<template>
	<error
		image-name="permission-error.png"
		:error-title="errorTitle" />
</template>

<script>
	import hardcodedTexts from '@/hardcodedTexts.js'

	import Error from './Error.vue'

	export default {
		name: 'QPermissionError',

		components: {
			Error
		},

		expose: [],

		computed: {
			errorTitle()
			{
				return this.Resources[hardcodedTexts.noPermissionError]
			}
		}
	}
</script>
