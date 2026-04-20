<template>
	<error
		image-name="not-found.png"
		:error-title="errorTitle"
		back-button-visible
		:main-page-button-visible="hasHomeButton" />
</template>

<script>
	import { mapState } from 'pinia'
	import { useSystemDataStore } from '@quidgest/clientapp/stores'

	import hardcodedTexts from '@/hardcodedTexts.js'

	import Error from './Error.vue'

	export default {
		name: 'QSystemNotFoundError',

		components: {
			Error
		},

		expose: [],

		computed: {
			...mapState(useSystemDataStore, [
				'system'
			]),

			errorTitle()
			{
				return this.Resources[hardcodedTexts.systemNotFound]
			},

			hasHomeButton()
			{
				return this.system.availableSystems?.includes(this.system.defaultSystem) ?? false
			}
		}
	}
</script>
