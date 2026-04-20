<template>
	<div
		v-if="isVisible"
		:class="['f-login' , ...authenticationClasses]">
		<div class="f-login__container">
			<div class="f-login__background">
				<div class="f-login__brand">
					<img
						:src="`${$app.resourcesPath}f-login__brand.png?v=${$app.genio.buildVersion}`"
						alt="" />
					<h1>{{ texts.appName }}</h1>
				</div>

				<p class="q-logon-text">{{ texts.changePassword }}</p>

				<div class="form-flow">
					<q-password-input
						v-bind="controls.NewPassword"
						:model-value="model.NewPassword.value"
						:placeholder="texts.newPassword"
						:show-password-label="texts.showPassword"
						:classes="{ error: newPasswordError && showError }"
						size="block"
						@update:model-value="updateNewPasswordValue">
						<template #prepend>
							<span>
								<q-icon icon="lock" />
							</span>
						</template>
					</q-password-input>

					<div
						v-if="newPasswordError && showError"
						class="i-text__error">
						<q-icon icon="exclamation-sign" />
						{{ validationErrors["NewPassword"][0] }}
					</div>

					<q-password-input
						v-bind="controls.ConfirmPassword"
						:placeholder="texts.confirmPassword"
						:show-password-label="texts.showPassword"
						:classes="{ error: confirmPasswordError && showError }"
						size="block"
						:model-value="model.ConfirmPassword.value"
						@keyup-enter="resetPassword"
						@update:model-value="updateConfirmPasswordValue">
						<template #prepend>
							<span>
								<q-icon icon="lock" />
							</span>
						</template>
					</q-password-input>


					<div
						v-if="errorMessage"
						class="i-text__error">
						<q-icon icon="exclamation-sign" />
						{{ errorMessage }}
					</div>

					<div class="q-logon-btns-container">
						<q-button
							block
							:class="['q-button--login', 'text-uppercase']"
							:label="texts.reset"
							:title="texts.reset"
							:loading="loading"
							@click="resetPassword" />
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import { mapState } from 'pinia'
	import _isEmpty from 'lodash-es/isEmpty'
	import _forEach from 'lodash-es/forEach'

	import { useSystemDataStore } from '@quidgest/clientapp/stores'
	import { useAuthDataStore } from '@quidgest/clientapp/stores'
	import { QEventEmitter } from '@quidgest/clientapp/plugins/eventBus'
	import NavHandlers from '@/mixins/navHandlers.js'
	import VueNavigation from '@/mixins/vueNavigation.js'
	import modelFieldType from '@quidgest/clientapp/models/fields'
	import fieldControlClass from '@/mixins/fieldControl.js'
	import FormViewModelBase from '@/mixins/formViewModelBase.js'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	class ViewModel extends FormViewModelBase
	{
		constructor(vueContext)
		{
			super(vueContext)

			this.NewPassword = new modelFieldType.String({
				id: 'NewPassword',
				originId: 'NewPassword',
				field: 'NewPassword',
				isRequired: true
			})

			this.ConfirmPassword = new modelFieldType.String({
				id: 'ConfirmPassword',
				originId: 'ConfirmPassword',
				field: 'ConfirmPassword',
				isRequired: true
			})
		}
	}

	export default {
		name: 'RecoverPasswordChange',

		mixins: [
			NavHandlers,
			VueNavigation,
			LayoutHandlers
		],

		expose: [
			'navigationId'
		],

		data()
		{
			return {
				validationErrors: {},

				loading: false,

				showError: false,

				isVisible: false,

				internalEvents: new QEventEmitter(),

				model: new ViewModel(this),

				controls: {
					NewPassword: new fieldControlClass.StringControl({
						id: 'NewPassword',
						modelField: 'NewPassword',
						valueChangeEvent: 'fieldChange:NewPassword',
						name: 'NewPassword',
						label: computed(() => this.Resources[hardcodedTexts.newPassword]),
						maxLength: 50,
						labelAttrs: null
					}, this),
					ConfirmPassword: new fieldControlClass.StringControl({
						id: 'ConfirmPassword',
						modelField: 'ConfirmPassword',
						valueChangeEvent: 'fieldChange:ConfirmPassword',
						name: 'ConfirmPassword',
						label: computed(() => this.Resources[hardcodedTexts.confirmPassword]),
						maxLength: 50,
						labelAttrs: null
					}, this)
				},

				texts: {
					appName: computed(() => this.Resources[hardcodedTexts.appName]),
					changePassword: computed(() => this.Resources[hardcodedTexts.changePassword]),
					reset: computed(() => this.Resources[hardcodedTexts.reset]),
					newPassword: computed(() => this.Resources[hardcodedTexts.newPassword]),
					confirmPassword: computed(() => this.Resources[hardcodedTexts.confirmPassword]),
					showPassword: computed(() => this.Resources[hardcodedTexts.showPassword])
				}
			}
		},

		created()
		{
			if (this.hasPasswordRecovery === false)
				this.navigateToRouteName('main')
			else
			{
				this.isVisible = true
				this.initFormControls(true)
			}
		},

		beforeUnmount()
		{
			this.destroyFormControls()
		},

		computed: {
			...mapState(useSystemDataStore, [
				'system'
			]),

			...mapState(useAuthDataStore, [
				'hasPasswordRecovery'
			]),

			newPasswordError()
			{
				return !this.isEmpty(this.validationErrors) && this.validationErrors["NewPassword"]
			},

			confirmPasswordError()
			{
				return !this.isEmpty(this.validationErrors) && this.validationErrors["ConfirmPassword"]
			},

			genericError()
			{
				return !this.isEmpty(this.validationErrors) && this.validationErrors["error"]
			},

			errorMessage()
			{
				// Theres really not a better way do it this at the moment, if the user changed the language at runtime the message won't translate automatically
				if(this.confirmPasswordError) return this.validationErrors["ConfirmPassword"][0]
				if(this.genericError) return this.validationErrors["error"][0]
				else return undefined
			},

		},

		methods: {
			setData(modelValue)
			{
				if (_isEmpty(modelValue))
					return

				for (const fld in this.model)
					this.model[fld].updateValue(modelValue[fld])
			},

			hideError()
			{
				this.showError = false
			},

			async resetPassword()
			{
				this.loading = true
				await this.netAPI.postData('Account', 'RecoverPasswordChange', this.model.serverObjModel, this.resetPasswordSuccess)
				this.loading = false
			},

			resetPasswordSuccess(_, response)
			{
				const responseData = response.data
				this.setData(responseData.Data)

				if (response.data.Success)
					this.navigateToRouteName('password-recovery-change-success')
				else
				{
					if (!_isEmpty(response.data.Errors)){
						this.validationErrors = response.data.Errors
						this.showError = true
						this.model.ConfirmPassword.value = ''
					}
				}
			},

			initFormControls()
			{
				this.controls.NewPassword.init(true)
				this.controls.ConfirmPassword.init(true)
			},

			destroyFormControls()
			{
				_forEach(this.controls, (ctrl) => ctrl.destroy())
			},

			updateNewPasswordValue(newVal)
			{
				delete this.validationErrors["NewPassword"] // When the user starts typing hide the error message
				this.model.NewPassword.fnUpdateValue(newVal)
			},

			updateConfirmPasswordValue(newVal){
				delete this.validationErrors["ConfirmPassword"]
				this.model.ConfirmPassword.fnUpdateValue(newVal)
			}
		}
	}
</script>
