<template>
	<div class="error-container">
		<p v-if="imageName">
			<img
				:src="`${$app.resourcesPath}${imageName}?v=${$app.genio.buildVersion}`"
				:alt="errorTitle" />
		</p>

		<h2 v-if="errorTitle">
			{{ errorTitle }}
		</h2>

		<h6 v-if="errorMessage">
			{{ errorMessage }}
		</h6>

		<br />

		<q-button
			v-if="backButtonVisible"
			:label="goBackText"
			:title="goBackText"
			@click="back">
			<q-icon icon="back" />
		</q-button>

		<br />

		<q-button
			v-if="mainPageButtonVisible"
			:label="goMainPageText"
			:title="goMainPageText"
			@click="mainPage">
			<q-icon icon="home" />
		</q-button>
	</div>
</template>

<script>
	import NavHandlers from '@/mixins/navHandlers.js'
	import hardcodedTexts from '@/hardcodedTexts.js'

	export default {
		name: 'QError',

		mixins: [NavHandlers],

		props: {
			/**
			 * The title of the error, typically displayed as a header.
			 */
			errorTitle: {
				type: String,
				required: true
			},

			/**
			 * The name of the image file to be displayed alongside the error message.
			 * Defaults to 'generic-error.png'.
			 */
			imageName: {
				type: String,
				default: 'generic-error.png'
			},

			/**
			 * Whether the back button should be visible.
			 */
			backButtonVisible: {
				type: Boolean,
				default: true
			},

			/**
			 * Whether the Main page button should be visible.
			 */
			mainPageButtonVisible: {
				type: Boolean,
				default: false
			}
		},

		expose: [
			'navigationId'
		],

		data()
		{
			return {
				errorMessage: ''
			}
		},

		created()
		{
			const eMsgParams = this.$route.params.errorMessage

			if (typeof eMsgParams === 'string')
				this.errorMessage = eMsgParams
		},

		computed: {
			/**
			 * Computed property to get localized text for the 'Go Back' button label.
			 */
			goBackText()
			{
				return this.Resources[hardcodedTexts.goBack]
			},

			/**
			 * Computed property to get localized text for the 'Home page' button label.
			 */
			goMainPageText()
			{
				return this.Resources[hardcodedTexts.initialPage]
			}
		},

		methods: {
			/**
			 * Method to navigate to the previous page in the app's history.
			 * It ensures the error page is not re-encountered by removing it from the history stack.
			 */
			back()
			{
				/*
				 * Before navigating to the previous page, we need to remove the error page level itself.
				 * If we don't remove it, 'goBack' will redirect to the page that had the error, and in turn return to the error page.
				 */
				this.removeHistoryLevel()
				this.goBack()
			},

			/**
			 * Method to navigate to the main page.
			 */
			mainPage()
			{
				this.$router.push({ name: 'main' })
			}
		}
	}
</script>
