<template>
	<div ref="formHeader">
		<div class="c-action-bar">
			<h1 class="form-header">
				{{ texts.profile }}
			</h1>

			<div class="c-action-bar__menu"></div>
		</div>
	</div>

	<q-validation-summary
		v-if="showErrors"
		:messages="validationErrors" />

	<div class="form-flow">
		<!-- CHANGE PASSWORD -->
		<q-row-container
			v-if="hasUsernameAuth"
			is-large>
			<q-control-wrapper class="row-line-group">
				<q-group-box-container :label="texts.changePassword">
					<q-row-container is-large>
						<q-control-wrapper class="row-line-group">
							<base-input-structure
								class="i-text"
								v-bind="controls.ValNome">
								<q-input-group :prepend-icon="{ icon: 'user' }">
									<q-text-field
										v-bind="controls.ValNome.props"
										v-model="model.ValNome.value"
										@update:model-value="model.ValNome.fnUpdateValue" />
								</q-input-group>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container is-large>
						<q-control-wrapper class="row-line-group">
							<base-input-structure
								class="i-text"
								v-bind="controls.OldPassword">
								<q-password-input
									v-bind="controls.OldPassword.props"
									v-model="model.OldPassword.value"
									@update:model-value="model.OldPassword.fnUpdateValue"
									@focus="hideErrors">
									<template #prepend>
										<span>
											<q-icon icon="password" />
										</span>
									</template>
								</q-password-input>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container is-large>
						<q-control-wrapper class="row-line-group">
							<base-input-structure
								class="i-text"
								v-bind="controls.NewPassword">
								<q-password-input
									v-bind="controls.NewPassword.props"
									v-model="model.NewPassword.value"
									@update:model-value="model.NewPassword.fnUpdateValue"
									@focus="hideErrors">
									<template #prepend>
										<span>
											<q-icon icon="password" />
										</span>
									</template>
								</q-password-input>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container is-large>
						<q-control-wrapper class="row-line-group">
							<base-input-structure
								class="i-text"
								v-bind="controls.ConfirmPassword">
								<q-password-input
									v-bind="controls.ConfirmPassword.props"
									v-model="model.ConfirmPassword.value"
									@update:model-value="model.ConfirmPassword.fnUpdateValue"
									@focus="hideErrors">
									<template #prepend>
										<span>
											<q-icon icon="password" />
										</span>
									</template>
								</q-password-input>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container is-large>
						<q-control-wrapper class="row-line-group">
							<q-password-meter
								style="width: 300px"
								:input-value="this.model.NewPassword.value" />
						</q-control-wrapper>
					</q-row-container>

					<q-row-container is-large>
						<q-control-wrapper
							id="container-ChangePassword"
							class="row-line-group">
							<q-button
								id="ChangePassword"
								variant="bold"
								:label="texts.changePassword"
								:title="texts.changePassword"
								:disabled="maintenance.isActive"
								@click="changePassword">
								<q-icon icon="reset-password" />
							</q-button>
						</q-control-wrapper>
					</q-row-container>
				</q-group-box-container>
			</q-control-wrapper>
		</q-row-container>

		<!-- REGISTER CERTIFICATE -->
		<q-row-container
			v-if="$app.authConfig.useCertificate"
			is-large>
			<q-control-wrapper class="row-line-group">
				<q-group-box-container :label="texts.registerCertificate">
					<q-row-container is-large>
						<q-control-wrapper class="row-line-group">
							<base-input-structure
								class="i-text"
								v-bind="controls.ValNome">
								<q-input-group :prepend-icon="{ icon: 'user' }">
									<q-text-field
										v-bind="controls.ValNome.props"
										v-model="model.ValNome.value"
										@update:model-value="model.ValNome.fnUpdateValue" />
								</q-input-group>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container is-large>
						<q-control-wrapper class="row-line-group">
							<base-input-structure
								class="i-text"
								v-bind="controls.Password">
								<q-password-input
									v-bind="controls.Password.props"
									v-model="model.Password.value"
									@update:model-value="model.Password.fnUpdateValue">
									<template #prepend>
										<span>
											<q-icon icon="password" />
										</span>
									</template>
								</q-password-input>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container is-large>
						<q-control-wrapper class="row-line-group">
							<q-button
								variant="bold"
								:label="texts.registerCertificate"
								:title="texts.registerCertificate"
								@click="registerCertificate">
								<q-icon icon="certificate" />
							</q-button>
						</q-control-wrapper>
					</q-row-container>
				</q-group-box-container>
			</q-control-wrapper>
		</q-row-container>

		<!-- OPEN ID CONNECT -->
		<q-row-container
			v-if="!isEmpty(model.AuthRedirectMethods)"
			is-large>
			<q-control-wrapper class="row-line-group">
				<q-group-box-container label="OpenId Connect">
					<template
						v-for="idMethod in model.AuthRedirectMethods"
						:key="idMethod.Id">
						<q-row-container is-large>
							<q-control-wrapper class="row-line-group">
								<q-button
									variant="bold"
									:label="idMethod.Description || idMethod.Id"
									:title="idMethod.Description || 'Authentication'"
									@click="clickRegisterAuth(idMethod)" />
							</q-control-wrapper>
						</q-row-container>
					</template>
				</q-group-box-container>
			</q-control-wrapper>
		</q-row-container>

		<!-- TWO FACTOR AUTHENTICATION -->
		<q-row-container
			v-if="model.Enable2FAOptions"
			is-large>
			<q-control-wrapper class="row-line-group">
				<q-group-box-container :label="texts.twoFactorAuth">
					<p>{{ texts.twoFactorAuthHelp }} {{ $app.applicationName }}</p>
					<p>{{ texts.twoFactorAuthFirstStep }}</p>
					<div>
						<q-badge
							v-if="model.Current2FA"
							color="primary">
							{{ model.Current2FA }}
						</q-badge>
					</div>
					<q-button
						class="fit-content"
						:label="texts.setup2fa"
						:title="texts.setup2fa"
						:disabled="maintenance.isActive"
						@click="change2FA">
						<q-icon icon="arrow-right" />
					</q-button>
				</q-group-box-container>
			</q-control-wrapper>
		</q-row-container>

		<!-- FORM BUTTONS -->
		<q-row-container is-large>
			<q-control-wrapper class="control-join-group">
				<q-button
					:label="texts.goBack"
					:title="texts.goBack"
					@click="cancel">
					<q-icon icon="back" />
				</q-button>
			</q-control-wrapper>
		</q-row-container>
	</div>
</template>

<script>
	import _forEach from 'lodash-es/forEach'
	import _isEmpty from 'lodash-es/isEmpty'
	import { mapActions, mapState } from 'pinia'
	import { computed } from 'vue'

	import asyncProcM from '@quidgest/clientapp/composables/async'
	import { messageTypes } from '@quidgest/clientapp/constants/enums'
	import modelFieldType from '@quidgest/clientapp/models/fields'
	import { useAuthDataStore, useGenericDataStore } from '@quidgest/clientapp/stores'
	import { displayMessage } from '@quidgest/clientapp/utils/genericFunctions'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import fieldControlClass from '@/mixins/fieldControl.js'
	import NavHandlers from '@/mixins/navHandlers.js'
	import VueNavigation from '@/mixins/vueNavigation.js'

	export default {
		name: 'QProfile',

		mixins: [
			NavHandlers,
			VueNavigation
		],

		expose: [
			'navigationId'
		],

		data()
		{
			return {
				componentOnLoadProc: asyncProcM.getProcListMonitor('Profile', false),

				validationErrors: {},

				intervalId: null,

				showErrors: false,

				model: {
					Enable2FAOptions: false,

					AuthRedirectMethods: null,

					ValCodpsw: new modelFieldType.PrimaryKey({
						id: 'ValCodpsw',
						originId: 'ValCodpsw',
						area: 'PSW',
						field: 'CODPSW'
					}),

					ValNome: new modelFieldType.String({
						id: 'ValNome',
						originId: 'ValNome',
						area: 'PSW',
						field: 'NOME',
						readonly: true,
						required: true
					}),

					OldPassword: new modelFieldType.String({
						id: 'OldPassword',
						originId: 'OldPassword',
						area: 'PSEUD',
						field: 'OldPassword',
						isRequired: true
					}),

					NewPassword: new modelFieldType.String({
						id: 'NewPassword',
						originId: 'NewPassword',
						area: 'PSEUD',
						field: 'NewPassword',
						isRequired: true
					}),

					ConfirmPassword: new modelFieldType.String({
						id: 'ConfirmPassword',
						originId: 'ConfirmPassword',
						area: 'PSEUD',
						field: 'ConfirmPassword',
						isRequired: true
					}),

					Password: new modelFieldType.String({
						id: 'Password',
						originId: 'Password',
						area: 'PSEUD',
						field: 'Password',
						isRequired: true
					})
				},

				controls: {
					ValNome: new fieldControlClass.StringControl({
						id: 'ValNome',
						modelField: 'ValNome',
						valueChangeEvent: 'fieldChange:pseud.nome',
						name: 'ValNome',
						maxLength: this.$app.authConfig.maxUsrSize,
						isRequired: true,
						size: 'medium'
					}, this),

					OldPassword: new fieldControlClass.StringControl({
						id: 'OldPassword',
						modelField: 'OldPassword',
						valueChangeEvent: 'fieldChange:pseud.OldPassword',
						name: 'OldPassword',
						label: computed(() => this.Resources[hardcodedTexts.currentPassword]),
						labelPosition: '',
						maxLength: this.$app.authConfig.maxPswSize,
						isRequired: true,
						size: 'large'
					}, this),

					NewPassword: new fieldControlClass.StringControl({
						id: 'NewPassword',
						modelField: 'NewPassword',
						valueChangeEvent: 'fieldChange:pseud.NewPassword',
						name: 'NewPassword',
						label: computed(() => this.Resources[hardcodedTexts.newPassword]),
						labelPosition: '',
						maxLength: this.$app.authConfig.maxPswSize,
						isRequired: true,
						size: 'large'
					}, this),

					ConfirmPassword: new fieldControlClass.StringControl({
						id: 'ConfirmPassword',
						modelField: 'ConfirmPassword',
						valueChangeEvent: 'fieldChange:pseud.ConfirmPassword',
						name: 'ConfirmPassword',
						label: computed(() => this.Resources[hardcodedTexts.confirmPassword]),
						labelPosition: '',
						maxLength: this.$app.authConfig.maxPswSize,
						isRequired: true,
						size: 'large'
					}, this),

					Password: new fieldControlClass.StringControl({
						id: 'Password',
						modelField: 'Password',
						valueChangeEvent: 'fieldChange:pseud.Password',
						name: 'Password',
						label: computed(() => this.Resources[hardcodedTexts.password]),
						labelPosition: '',
						placeholder: computed(() => this.Resources[hardcodedTexts.currentPassword]),
						maxLength: this.$app.authConfig.maxPswSize,
						isRequired: true,
						size: 'large'
					}, this)
				},

				texts: {
					profile: computed(() => this.Resources[hardcodedTexts.profile]),
					changePassword: computed(() => this.Resources[hardcodedTexts.changePassword]),
					passwordChangeSuccess: computed(() => this.Resources[hardcodedTexts.passwordChangeSuccess]),
					registerCertificate: computed(() => this.Resources[hardcodedTexts.registerCertificate]),
					goBack: computed(() => this.Resources[hardcodedTexts.goBack]),
					twoFactorAuth: computed(() => this.Resources[hardcodedTexts.twoFactorAuth]),
					twoFactorAuthHelp: computed(() => this.Resources[hardcodedTexts.twoFactorAuthHelp]),
					twoFactorAuthFirstStep: computed(() => this.Resources[hardcodedTexts.twoFactorAuthFirstStep]),
					setup2fa: computed(() => this.Resources[hardcodedTexts.setup2fa]),
					user: computed(() => this.Resources[hardcodedTexts.user])
				}
			}
		},

		created()
		{
			this.clearHistory()

			// Load data
			this.componentOnLoadProc.addBusy(this.fetchData())
			// Only after the data is loaded from the server, init captch control
			this.componentOnLoadProc.once(() => this.initFormControls(), this)
		},

		beforeUnmount()
		{
			this.destroyFormControls()
		},

		computed: {
			...mapState(useGenericDataStore, [
				'maintenance'
			]),

			...mapState(useAuthDataStore, [
				'hasUsernameAuth'
			])
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'setInfoMessage',
				'clearInfoMessages'
			]),

			isEmpty: _isEmpty,

			initFormControls()
			{
				_forEach(this.controls, (ctrl) => ctrl.init(ctrl.name === 'ValNome' ? false : true))
			},

			destroyFormControls()
			{
				_forEach(this.controls, ctrl => ctrl.destroy())
			},

			cancel()
			{
				this.$router.push({ name: 'main' })
			},

			setData(modelValue)
			{
				for (const fld in this.model)
				{
					if (this.model[fld] instanceof modelFieldType.Base)
						this.model[fld].updateValue(modelValue[fld])
					else
						this.model[fld] = modelValue[fld]
				}
			},

			showResponseUserMsg(errors, warnings, message, success, keepAlert)
			{
				this.validationErrors = {}
				if (!keepAlert)
					this.clearInfoMessages()

				if (!_isEmpty(errors))
				{
					this.showErrors = true
					this.validationErrors = errors
				}

				if (typeof message === 'string')
					displayMessage(message, success === true ? 'success' : 'error')

				// If there are any warning messages, they will be displayed.
				if (typeof warnings === 'object' && Array.isArray(warnings))
				{
					warnings.forEach((w) => {
						const warningProps = {
							type: messageTypes.W,
							message: w,
							icon: 'error',
							pinned: true
						}
						this.showErrors = true
						this.setInfoMessage(warningProps)
					})
				}
			},

			fetchData(keepAlert)
			{
				return this.netAPI.fetchData('Home', 'Profile', null, async (data, response) => {
					this.showResponseUserMsg(response.data.Errors, (data || {}).Warnings, response.data.Message, false, keepAlert)
					this.setData(data)
				})
			},

			clickRegisterAuth(idMethod)
			{
				window.location.replace(idMethod.Redirect)
			},

			changePassword()
			{
				const _model = {}

				this.showErrors = false

				for (const fld in this.model)
				{
					if (this.model[fld] instanceof modelFieldType.Base)
						_model[fld] = this.model[fld].value
					else
						_model[fld] = this.model[fld]
				}

				return this.netAPI.postData('Home', 'Profile', _model, async (data, response) => {
					if (response.data.Success)
					{
						this.clearHistory()

						// Sets up the success message that the user will see after leaving the form.
						const successProps = {
							type: messageTypes.OK,
							message: (data || {}).Message || this.texts.passwordChangeSuccess,
							pinned: true
						}
						this.setInfoMessage(successProps)

						//Redirect to home page after successful change
						this.$router.push({ name: 'main', params: { keepAlerts: true } })
					}
					else
					{
						this.showResponseUserMsg(response.data.Errors, (data || {}).Warnings, response.data.Message, response.data.Success)
						this.setData(data)
					}
				})
			},

			hideErrors()
			{
				this.showErrors = false
			},

			registerCertificate()
			{
				this.netAPI.postData('Home', 'RefreshCert')

				if (this.intervalId !== null)
				{
					clearInterval(this.intervalId)
					this.intervalId = null
				}

				return this.netAPI.postData('Home', 'ProfileRegistarCerticado', {
					psw: this.model.Password.value,
					codpsw: this.model.ValCodpsw.value
				}, async (data, response_1) => {
					this.showResponseUserMsg(response_1.data.Errors, (data || {}).Warnings, response_1.data.Message, response_1.data.Success)

					if (response_1.data.Success)
					{
						window.location = `myscheme:|${data.rec}|selcert`
						//window.loadingModal = new QLoading('idRegCert', 'Registar Qcertificate')

						this.intervalId = setInterval(() => {
							this.netAPI.postData('Home', 'RefreshCert', null, async (d, response_2) => {
								this.showResponseUserMsg(response_2.data.Errors, (d || {}).Warnings, response_2.data.Message, response_2.data.Success)

								if (response_2.data.Success && d.success)
								{
									if (d.loading)
									{
										//window.loadingModal.show()
									}
									else if (d.loginSucess)
									{
										clearInterval(this.intervalId)
										//var url = '@Url.Action("RegisterCertificateConfirm", "Account", new RouteValueDictionary(new { serialNumber = "SC"}))'.replace("SC",d.serialcert)
										//window.location.href = url
									}
									else
									{
										clearInterval(this.intervalId)
										//window.loadingModal.hide()
										//window.location.reload()
									}
								}
							})
						}, 100)
					}
				})
			},

			change2FA()
			{
				this.$router.push({ name: 'change2fa' })
			}
		}
	}
</script>
