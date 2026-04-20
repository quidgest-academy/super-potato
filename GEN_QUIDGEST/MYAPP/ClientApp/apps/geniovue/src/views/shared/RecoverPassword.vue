<template>
	<div
		v-if="isVisible"
		:class="authenticationClasses">
		<div class="f-login">
			<div class="f-login__container">
				<div class="f-login__background">
					<div class="f-login__brand">
						<img
							:src="`${$app.resourcesPath}f-login__brand.png?v=${$app.genio.buildVersion}`"
							alt="" />
						<h1>{{ texts.appName }}</h1>
					</div>
					<div
						id="recover-password-container"
						class="form-flow">
						<template v-if="!model.IsEmailSent">
							<p class="q-logon-text">{{ texts.enterEmail }}</p>

							<q-input-group
								size="block"
								:prepend-icon="{ icon: 'envelope' }"
								:class="{ error: emailError && showError }">
								<q-text-field
									v-model="model.Email.value"
									:placeholder="texts.email"
									v-bind="controls.Email"
									@update:model-value="model.Email.fnUpdateValue"
									@input="hideError"
									@keyup.enter="resetPassword" />
							</q-input-group>
							<div
								id="captcha-field"
								class="i-captcha">
								<q-input-group size="block">
									<img
										class="i-captcha__img_recovery"
										alt="captcha"
										:src="captchaImageUrl" />

									<q-button
										class="i-captcha__reset"
										:title="texts.refresh"
										@click="resetCaptcha">
										<q-icon icon="reset" />
									</q-button>
								</q-input-group>
								<q-control-wrapper v-show="controls.captchaInput.isVisible">
									<base-input-structure
										v-bind="controls.captchaInput"
										v-on="controls.captchaInput.handlers"
										:loading="controls.captchaInput.props.loading"
										:class="captcha - recovery - control">
										<q-text-field
											v-model="model.captchaInput.value"
											:placeholder="texts.enterCaptcha"
											v-bind="controls.captchaInput"
											@update:model-value="model.captchaInput.fnUpdateValue"
											@input="hideError"
											@keyup.enter="resetPassword" />
									</base-input-structure>
								</q-control-wrapper>
							</div>

							<div
								v-for="(erro, index) in errors"
								:key="index"
								id="error-message"
								class="i-text__error">
								<q-icon icon="exclamation-sign" />
								{{ erro }}
							</div>

							<div class="q-logon-btns-container">
								<q-button
									block
									id="reset-button"
									class="q-button--login"
									:label="texts.reset"
									:loading="loading"
									:data-loading="loading"
									:title="texts.reset"
									@click="resetPassword" />
							</div>
						</template>
						<template v-else>
							<p class="q-logon-text">{{ successMessage }}</p>
						</template>

						<div>
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
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import { mapState } from 'pinia'

	import { useSystemDataStore } from '@quidgest/clientapp/stores'
	import { useAuthDataStore } from '@quidgest/clientapp/stores'
	import NavHandlers from '@/mixins/navHandlers.js'
	import VueNavigation from '@/mixins/vueNavigation.js'
	import modelFieldType from '@quidgest/clientapp/models/fields'
	import fieldControlClass from '@/mixins/fieldControl.js'
	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import { v4 as uuidv4 } from 'uuid'
	import QRouterLink from '@/views/shared/QRouterLink.vue'

	export default {
		name: 'RecoverPassword',

		components: {
			QRouterLink
		},

		mixins: [
			NavHandlers,
			VueNavigation,
			LayoutHandlers
		],

		expose: [
			'navigationId'
		],

		data() {
			return {
				validationErrors: {},

				captchaImageUrl: '',

				captchaId: 'recoveryCaptcha', // Define a constante no data()

				showError: false,

				isVisible: false,

				loading: false,

				model: {
					IsEmailSent: false,

					Email: new modelFieldType.String({
						id: 'Email',
						originId: 'Email',
						area: '',
						field: 'EMAIL',
						required: true
					}),
					captchaInput: new modelFieldType.String({
						id: 'userEnteredCaptchaCode',
						originId: 'userEnteredCaptchaCode',
						area: '',
						field: 'userEnteredCaptchaCode',
						required: true
					})
				},

				controls: {
					Email: new fieldControlClass.StringControl({
						id: 'Email',
						modelField: 'Email',
						valueChangeEvent: 'fieldChange:email',
						name: 'Email',
						maxLength: 254,
						hasLable: false,
						isRequired: true
					}, this),
					captchaInput: new fieldControlClass.StringControl({
						id: 'userEnteredCaptchaCode',
						size: 'block',
						maxLength: 254
					}, this)
				},

				texts: {
					appName: computed(() => this.Resources[hardcodedTexts.appName]),
					enterEmail: computed(() => this.Resources[hardcodedTexts.enterEmail]),
					email: computed(() => this.Resources[hardcodedTexts.email]),
					reset: computed(() => this.Resources[hardcodedTexts.reset]),
					backToLogin: computed(() => this.Resources[hardcodedTexts.backToLogin]),
					enterCaptcha: computed(() => this.Resources[hardcodedTexts.enterCaptcha]),
				}
			}
		},

		created() {
			if (this.hasPasswordRecovery === false)
				this.navigateToRouteName('main')
			else {
				this.isVisible = true
				this.fetchData()
				this.controls.Email.init(true)
			}

			this.controls.captchaInput.init(true)
			this.resetCaptcha()
		},

		beforeUnmount() {
			this.controls.Email.destroy()
		},

		computed: {
			...mapState(useSystemDataStore, [
				'system'
			]),

			...mapState(useAuthDataStore, [
				'hasPasswordRecovery'
			]),

			successMessage() {
				return genericFunctions.formatString(this.Resources[hardcodedTexts.passwordRecoverEmail], this.model.Email.value)
			},

			emailError() {
				return !this.isEmpty(this.validationErrors) && this.validationErrors["Email"];
			},
			captchError() {
				return !this.isEmpty(this.validationErrors) && this.validationErrors["userEnteredCaptchaCode"];
			},

			errors() {
				return Object.values(this.validationErrors).flat();

			}
		},

		methods: {
			setData(modelValue) {
				this.model.IsEmailSent = modelValue.IsEmailSent
				this.model.Email.updateValue(modelValue.Email)
			},

			fetchData() {
				return this.netAPI.fetchData('Account', 'RecoverPassword', null, (_, response) => this.setData(response.data.Data))
			},

			/**
			 * Retrieves the CAPTCHA data for user validation during registration.
			 * @returns {Object} The user-entered CAPTCHA code and its associated CAPTCHA ID.
			 */
			getCaptchaData() {
				// The user-entered captcha code value to be validated at the backend side
				const userEnteredCaptchaCode = this.model.captchaInput.value

				return {
					userEnteredCaptchaCode,
					captchaId: this.captchaId
				}
			},

			/**
			 * Resets the CAPTCHA by fetching a new image URL and clearing the user's input field.
			 */
			resetCaptcha() {
				const apiURL = this.netAPI.apiActionURL('Account', 'GetCaptcha'), uId = uuidv4()

				this.captchaImageUrl = `${apiURL}?captchaId=${this.captchaId}&t=${uId}`; // Usa a constante

				this.model.captchaInput.updateValue('')
			},

			hideError() {
				this.showError = false
			},

			async resetPassword() {
				this.loading = true
				const params = {
					Email: this.model.Email.value,
					CaptchaData: this.getCaptchaData()
				}

				await this.netAPI.postData('Account', 'RecoverPassword', params, this.handleResetPassword)
				this.loading = false
			},

			handleResetPassword(_, response) {
				const data = response.data
				this.showError = !this.isEmpty(data.Errors)
				if (this.showError) {
					this.validationErrors = data.Errors
					this.resetCaptcha()
				}
				else {
					this.validationErrors = {}
					this.model.IsEmailSent = data.Data.IsEmailSent
				}
			}
		}
	}
</script>
