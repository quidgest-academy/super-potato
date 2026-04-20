<template>
	<div :class="authenticationClasses">
		<div class="f-login">
			<div class="f-login__container">
				<div class="f-login__background">
					<div class="f-login__brand">
						<img
							:src="`${$app.resourcesPath}f-login__brand.png?v=${$app.genio.buildVersion}`"
							alt="" />
						<h1>{{ texts.appName }}</h1>

						<h5 class="q-logon-text">{{ texts.accountCreated }}</h5>
					</div>

					<p class="q-successful-registration">{{ texts.thanksForRegister }}</p>

					<q-router-link
						class="f-login__link"
						:link="{
							name: 'main',
							params: { culture: system.currentLang }
						}">
						{{ texts.backToLogin }}
					</q-router-link>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import { mapState } from 'pinia'

	import { useSystemDataStore } from '@quidgest/clientapp/stores'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import QRouterLink from '@/views/shared/QRouterLink.vue'

	export default {
		name: 'AccountCreationSuccess',

		components: {
			QRouterLink
		},

		mixins: [LayoutHandlers],

		expose: [],

		data()
		{
			return {
				texts: {
					appName: computed(() => this.Resources[hardcodedTexts.appName]),
					accountCreated: computed(() => this.Resources[hardcodedTexts.accountCreated]),
					thanksForRegister: computed(() => this.Resources[hardcodedTexts.thanksForRegister]),
					backToLogin: computed(() => this.Resources[hardcodedTexts.backToLogin])
				}
			}
		},

		computed: {
			...mapState(useSystemDataStore, [
				'system'
			])
		}
	}
</script>
