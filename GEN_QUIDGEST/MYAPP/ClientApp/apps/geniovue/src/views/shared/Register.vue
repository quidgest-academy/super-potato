<template>
	<div id="c-sticky-header">
		<div class="c-action-bar">
			<h1 class="form-header">
				{{ texts.createNewAccount }}
			</h1>
		</div>
	</div>

	<q-validation-summary :messages="validationErrors" />

	<div class="form-flow">
		<template v-if="componentOnLoadProc.loaded">
			<q-row-container
				v-for="form in formsOrdered"
				:key="form.control"
				is-large>
				<q-control-wrapper class="control-row-group">
					<q-form-container
						v-bind="form.control"
						@update:nested-model="form.handler"
						class="account-register-form" />
				</q-control-wrapper>
			</q-row-container>

			<div
				id="captcha-field"
				class="container-fluid i-captcha">
				<img
					class="i-captcha__img"
					:src="captchaImageUrl" />

				<q-button
					class="i-captcha__reset"
					:title="texts.refresh"
					@click="resetCaptcha">
					<q-icon icon="reset" />
				</q-button>

				<q-row-container>
					<q-control-wrapper class="control-join-group">
						<q-text-field
							v-bind="controls.captchaInput.props"
							v-model="userEnteredCaptchaCode" />
					</q-control-wrapper>
				</q-row-container>
			</div>

			<q-row-container
				class="form-actions"
				is-large>
				<q-control-wrapper class="control-join-group">
					<q-button
						variant="bold"
						:label="texts.register"
						:title="texts.register"
						@click="register">
						<q-icon icon="save" />
					</q-button>

					<q-button
						:label="texts.leave"
						:title="texts.leave"
						@click="cancel">
						<q-icon icon="cancel" />
					</q-button>
				</q-control-wrapper>
			</q-row-container>
		</template>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import { mapActions, mapState } from 'pinia'
	import { v4 as uuidv4 } from 'uuid'

	import _assignIn from 'lodash-es/assignIn'
	import _forEach from 'lodash-es/forEach'
	import _isEmpty from 'lodash-es/isEmpty'
	import { useSystemDataStore } from '@quidgest/clientapp/stores'
	import { useGenericDataStore } from '@quidgest/clientapp/stores'
	import { messageTypes } from '@quidgest/clientapp/constants/enums'
	import NavHandlers from '@/mixins/navHandlers.js'
	import VueNavigation from '@/mixins/vueNavigation.js'
	import fieldControlClass from '@/mixins/fieldControl.js'
	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
	import asyncProcM from '@quidgest/clientapp/composables/async'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import FormViewModelBase from '@/mixins/formViewModelBase.js'
	import hardcodedTexts from '@/hardcodedTexts.js'

	export default {
		name: 'QRegister',

		mixins: [
			NavHandlers,
			VueNavigation,
			LayoutHandlers
		],

		props: {
			/**
			 * Nested route parameters used to configure the form fields.
			 */
			nestedRouteParams: {
				type: Object,
				default: () => ({
					name: 'UserRegistration',
					location: 'UserRegistration',
					params: {
						isNested: true
					}
				})
			}
		},

		expose: [
			'navigationId'
		],

		data()
		{
			return {
				componentOnLoadProc: asyncProcM.getProcListMonitor('UserRegister', false),

				validationErrors: {},

				captchaImageUrl: '',
				userEnteredCaptchaCode: '',

				model: {
					Component: '',
					partialView: '',
					partialViewJS: '',
					PswpartialView: '',

					FormPswOrdem: 0,
					FormDataOrdem: 0,

					FormPswData: null,
					FormData: null
				},

				// It is not possible to use the model initially received because it has some fields that cannot be mapped on the server side.
				modelToSend: {
					FormPswData: new FormViewModelBase(this),
					FormData: new FormViewModelBase(this)
				},

				controls: {
					secondForm: new fieldControlClass.FormContainerControl({
						id: 'formData',
						name: 'formData',
						size: 'xxlarge',
						hasLabel: false,
						supportForm: {
							name: null,
							component: null,
							mode: 'NEW',
							fnKeySelector: () => null
						}
					}, this),
					pswForm: new fieldControlClass.FormContainerControl({
						id: 'formPswData',
						name: 'formPswData',
						size: 'xxlarge',
						hasLabel: false,
						supportForm: {
							name: null,
							component: null,
							mode: 'NEW',
							fnKeySelector: () => null
						}
					}, this),
					captchaInput: new fieldControlClass.StringControl({
						id: 'registerCaptchaUserInput',
						size: 'large',
						maxLength: 6
					}, this)
				},

				texts: {
					createNewAccount: computed(() => this.Resources[hardcodedTexts.createNewAccount]),
					register: computed(() => this.Resources[hardcodedTexts.register]),
					leave: computed(() => this.Resources[hardcodedTexts.leave]),
					refresh: computed(() => this.Resources[hardcodedTexts.refresh])
				}
			}
		},

		created()
		{
			// Load data.
			this.componentOnLoadProc.addBusy(this.fetchData(), this.Resources[hardcodedTexts.genericLoad], 300)
			this.resetCaptcha()
		},

		computed: {
			...mapState(useSystemDataStore, ['system']),

			formsOrdered()
			{
				const formData = { control: this.controls.secondForm, handler: this.handleModelUpdate }
				const formPsw = { control: this.controls.pswForm, handler: this.handlePswModelUpdate }

				if (this.model.FormDataOrdem <= this.model.FormPswOrdem)
					return [formData, formPsw]
				return [formPsw, formData]
			}
		},

		beforeUnmount()
		{
			this.controls.secondForm.destroy()
			this.controls.pswForm.destroy()
			this.componentOnLoadProc.destroy()
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'setInfoMessage',
				'clearInfoMessages'
			]),

			/**
			 * Retrieves the CAPTCHA data for user validation during registration.
			 * @returns {Object} The user-entered CAPTCHA code and its associated CAPTCHA ID.
			 */
			getCaptchaData()
			{
				// The user-entered captcha code value to be validated at the backend side
				const userEnteredCaptchaCode = this.userEnteredCaptchaCode
				// The id of a captcha instance that the user tried to solve
				const captchaId = 'registerCaptcha'

				return {
					userEnteredCaptchaCode,
					captchaId
				}
			},

			/**
			 * Resets the CAPTCHA by fetching a new image URL and clearing the user's input field.
			 */
			resetCaptcha()
			{
				const apiURL = this.netAPI.apiActionURL('Account', 'GetCaptcha'),
					uId = uuidv4()
				this.captchaImageUrl = `${apiURL}?captchaId=registerCaptcha&t=${uId}`
				this.userEnteredCaptchaCode = ''
			},

			/**
			 * Registers the user by calling the Account registration endpoint with updated form data.
			 */
			register()
			{
				if (_isEmpty(this.model.redirect))
					return

				const params = {
					FormPswData: this.modelToSend.FormPswData.serverObjModel,
					FormData: this.modelToSend.FormData.serverObjModel,
					CaptchaData: this.getCaptchaData()
				}

				return this.netAPI.postData('Account', this.model.redirect, params, this.executeRegister)
			},

			async executeRegister(data, response)
			{
				if (!response.data.Success)
				{
					this.resetCaptcha()

					if (!_isEmpty(response.data.Errors))
						this.validationErrors = response.data.Errors
					else if (typeof response.data.Message === 'string')
						genericFunctions.displayMessage(response.data.Message, 'error')

					return
				}

				this.validationErrors = {}
				this.clearInfoMessages()

				// If there are any warning messages, they will be displayed.
				if (typeof data.Warnings === 'object' && Array.isArray(data.Warnings))
				{
					data.Warnings.forEach((w) => {
						const warningProps = {
							type: messageTypes.W,
							message: w,
							icon: 'error',
							pinned: true
						}
						this.setInfoMessage(warningProps)
					})
				}

				// Sets up the success message that the user will see after leaving the form.
				const successProps = {
					type: messageTypes.OK,
					message: data.Message,
					pinned: true
				}
				this.setInfoMessage(successProps)

				this.clearHistory()

				this.navigateToRouteName('creation-success')
			},

			/**
			 * Navigates back to the main page without performing any registration.
			 */
			cancel()
			{
				this.$router.push({
					name: 'main',
					params: { culture: this.system.currentLang }
				})
			},

			/**
			 * Updates the model data for password-related information.
			 * @param {Object} nestedModel - The nested model data to be merged into the form state.
			 */
			handlePswModelUpdate(nestedModel)
			{
				_forEach(nestedModel, (fldData, fldName) => Reflect.set(this.modelToSend.FormPswData, fldName, fldData))
			},

			/**
			 * Updates the model data for general account information.
			 * @param {Object} nestedModel - The nested model data to be merged into the form state.
			 */
			handleModelUpdate(nestedModel)
			{
				_forEach(nestedModel, (fldData, fldName) => Reflect.set(this.modelToSend.FormData, fldName, fldData))
			},

			/**
			 * Sets state data based on the model received from the backend.
			 * @param {Object} modelValue - The model data to be integrated into the component's state.
			 */
			setData(modelValue)
			{
				_assignIn(this.model, modelValue)

				const id = this.$route.params.id
				const registrationType = this.$app.userRegistration.registrationTypes.find(x => x.id === id)

				this.controls.secondForm.supportForm = {
					name: this.model.partialViewJS,
					component: registrationType.component,
					mode: 'NEW',
					fnKeySelector: () => this.model.FormData.QPrimaryKey
				}

				this.controls.secondForm.formData = {
					id: this.model.FormData.QPrimaryKey,
					historyBranchId: this.navigationId,
					isNested: true,
					form: this.model.partialViewJS,
					component: registrationType.component,
					mode: 'NEW',
					modes: '',
					nestedModel: this.model.FormData
				}

				this.controls.secondForm.nestedFormConfig.uiComponents.header = false
				this.controls.secondForm.nestedFormConfig.uiComponents.headerButtons = false
				this.controls.secondForm.nestedFormConfig.uiComponents.footer = false
				this.controls.secondForm.init(true)

				this.controls.captchaInput.init(true)

				this.controls.pswForm.supportForm = {
					name: 'PSWUSER',
					component: registrationType.PswComponent,
					mode: 'NEW',
					fnKeySelector: () => this.model.FormPswData.QPrimaryKey
				}

				this.controls.pswForm.formData = {
					id: this.model.FormPswData.QPrimaryKey,
					historyBranchId: this.navigationId,
					isNested: true,
					form: 'PSWUSER',
					component: registrationType.PswComponent,
					mode: 'NEW',
					modes: '',
					nestedModel: this.model.FormPswData
				}

				this.controls.pswForm.nestedFormConfig.uiComponents.header = false
				this.controls.pswForm.nestedFormConfig.uiComponents.headerButtons = false
				this.controls.pswForm.nestedFormConfig.uiComponents.footer = false
				this.controls.pswForm.init(true)
			},

			/**
			 * Fetches initial registration data needed to set up the registration form.
			 * It calls the backend API and sets up form data based on the response.
			 */
			fetchData()
			{
				const id = this.$route.params.id
				const registrationType = this.$app.userRegistration.registrationTypes.find((x) => x.id === id)

				const params = {
					Form: registrationType.form,
					Pswform: registrationType.pswForm,
					Id: id
				}

				return this.netAPI.fetchData(
					'Account',
					'Register',
					params,
					(data) => this.setData(data))
			}
		}
	}
</script>
