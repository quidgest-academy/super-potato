<template>
	<template v-if="props.registrationTypes.length === 1">
		<q-button
			v-if="isButton"
			id="register-btn"
			variant="bold"
			block
			:title="Resources[registrationTypes[0].designation]"
			:label="Resources[registrationTypes[0].designation]"
			@click="navigateToRegisterRoute" />

		<q-router-link
			v-else
			id="link-register"
			class="f-login__link"
			:link="{
				name: 'user-register',
				params: {
					id: registrationTypes[0].id
				}
			}">
			{{ Resources[registrationTypes[0].designation] }}
		</q-router-link>
	</template>

	<template v-else>
		<q-button
			v-if="isButton"
			id="register-btn"
			ref="buttonAnchor"
			variant="bold"
			block
			:label="Resources[hardcodedTexts.register]" />

		<a
			v-else
			id="link-register"
			ref="hyperlinkAnchor"
			class="f-login__link">
			{{ Resources[hardcodedTexts.register] }}
		</a>

		<q-overlay
			:anchor="anchor"
			placement="bottom">
			<q-router-link
				v-for="regType in props.registrationTypes"
				:key="regType.designation"
				class="dropdown-item"
				:link="{
					name: 'user-register',
					params: {
						id: regType.id
					}
				}">
				{{ Resources[regType.designation] }}
			</q-router-link>
		</q-overlay>
	</template>
</template>

<script setup>
	import { computed, ref } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import QRouterLink from '@/views/shared/QRouterLink.vue'

	const buttonAnchor = ref(null)
	const hyperlinkAnchor = ref(null)

	const props = defineProps({
		/**
		 * The different types of registration that the application supports
		 */
		registrationTypes: {
			type: Object,
			default: () => ({})
		},

		/**
		 * The display style, button | hyperlink
		 */
		displayStyle: {
			type: String,
			default: 'button'
		}
	})

	const isButton = computed(() => props.displayStyle === 'button')
	const anchor = computed(() => isButton.value ? buttonAnchor.value?.$el : hyperlinkAnchor.value)

	const emit = defineEmits(['navigate-to-register-route'])

	function navigateToRegisterRoute()
	{
		emit('navigate-to-register-route')
	}
</script>
