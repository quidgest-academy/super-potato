<template>
	<div
		class="container-fluid"
		role="dialog"
		aria-hidden="true">
		<q-row-container is-large>
			<q-control-wrapper class="row-line-group">
				<q-group-box-container
					id="user-register-acc-info"
					:label="texts.accountInfo"
					class="c-groupbox--minor">
					<q-row-container>
						<q-control-wrapper class="row-line-group">
							<base-input-structure
								class="i-text"
								v-bind="controls.Nome">
								<q-input-group
									:prepend-icon="{ icon: 'user' }"
									size="xlarge">
									<q-text-field
										v-bind="controls.Nome.props"
										:model-value="model.ValNome.value"
										@update:model-value="model.ValNome.fnUpdateValue" />
								</q-input-group>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="row-line-group">
							<base-input-structure
								class="i-text"
								v-bind="controls.Email">
								<q-input-group
									:prepend-icon="{ icon: 'envelope' }"
									size="xlarge">
									<q-text-field
										v-bind="controls.Email.props"
										:model-value="model.ValEmail.value"
										@update:model-value="model.ValEmail.fnUpdateValue" />
								</q-input-group>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="row-line-group">
							<base-input-structure
								class="i-text"
								v-bind="controls.Password">
								<q-password-input
									v-bind="controls.Password"
									size="xlarge"
									:model-value="model.ValPassword.value"
									@update:model-value="model.ValPassword.fnUpdateValue">
									<template #prepend>
										<span>
											<q-icon icon="lock" />
										</span>
									</template>
								</q-password-input>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="row-line-group">
							<base-input-structure
								class="i-text"
								v-bind="controls.ConfirmPassword">
								<q-password-input
									v-bind="controls.ConfirmPassword"
									:model-value="model.ConfirmValPassword.value"
									size="xlarge"
									@update:model-value="model.ConfirmValPassword.fnUpdateValue">
									<template #prepend>
										<span>
											<q-icon icon="lock" />
										</span>
									</template>
								</q-password-input>
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<div class="control-group">
						<div class="row-fluid">
							<q-password-meter :input-value="this.model.ValPassword.value" />
						</div>
					</div>
				</q-group-box-container>
			</q-control-wrapper>
		</q-row-container>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import _forEach from 'lodash-es/forEach'

	import modelFieldType from '@quidgest/clientapp/models/fields'
	import fieldControlClass from '@/mixins/fieldControl.js'
	import hardcodedTexts from '@/hardcodedTexts.js'

	export default {
		name: 'QAccountInfo',

		emits: [
			'update:nestedModel'
		],

		props: {
			/**
			 * Object representing the nested model's current state and properties.
			 */
			nestedModel: {
				type: Object,
				required: true
			}
		},

		expose: [],

		data()
		{
			const authConfig = this.$app.authConfig

			return {
				model: {
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
						required: true
					}),
					ValEmail: new modelFieldType.String({
						id: 'ValNome',
						originId: 'ValNome',
						area: 'PSW',
						field: 'EMAIL',
						required: true
					}),
					ValPassword: new modelFieldType.String({
						id: 'ValPassword',
						originId: 'ValPassword',
						area: 'PSW',
						field: 'PASSWORD',
						isRequired: true
					}),
					ConfirmValPassword: new modelFieldType.String({
						id: 'ConfirmValPassword',
						originId: 'ConfirmValPassword',
						area: 'PSW',
						field: 'ConfirmPassword',
						isRequired: true
					})
				},

				controls: {
					Nome: new fieldControlClass.StringControl({
						modelField: 'ValNome',
						valueChangeEvent: 'fieldChange:psw.nome',
						id: 'Nome',
						name: 'Nome',
						/*size: 'xlarge',*/
						label: computed(() => this.Resources[hardcodedTexts.user]),
						maxLength: authConfig.maxUsrSize,
						isRequired: true
					}, this),
					Email: new fieldControlClass.StringControl({
						modelField: 'ValEmail',
						valueChangeEvent: 'fieldChange:psw.email',
						id: 'Email',
						name: 'Email',
						/*size: 'xlarge',*/
						label: computed(() => this.Resources[hardcodedTexts.email]),
						maxLength: 254,
						isRequired: true
					}, this),
					Password: new fieldControlClass.StringControl({
						modelField: 'ValPassword',
						valueChangeEvent: 'fieldChange:psw.password',
						id: 'Password',
						name: 'Password',
						/*size: 'xlarge',*/
						label: computed(() => this.Resources[hardcodedTexts.password]),
						labelPosition: '',
						maxLength: authConfig.maxPswSize,
						isRequired: true
					}, this),
					ConfirmPassword: new fieldControlClass.StringControl({
						modelField: 'ConfirmValPassword',
						valueChangeEvent: 'fieldChange:psw.confirmpassword',
						id: 'ConfirmValPassword',
						name: 'ConfirmValPassword',
						/*size: 'xlarge',*/
						label: computed(() => this.Resources[hardcodedTexts.confirm]),
						labelPosition: '',
						maxLength: authConfig.maxPswSize,
						isRequired: true
					}, this)
				},

				texts: {
					accountInfo: computed(() => this.Resources[hardcodedTexts.accountInfo])
				}
			}
		},

		created()
		{
			this.hydrate(this.nestedModel)
			this.initFormControls()
		},

		methods: {
			/**
			 * Hydrates the model with provided raw data.
			 * @param {Object} rawData - The raw data to be used for hydrating the model.
			 */
			hydrate(rawData)
			{
				for (const fld in this.model)
					this.model[fld].updateValue(rawData[fld])
			},

			/**
			 * Initializes form controls upon component creation or when the nestedModel prop changes.
			 */
			initFormControls()
			{
				_forEach(this.controls, (ctrl) => ctrl.init(true))
			}
		},

		watch: {
			nestedModel: {
				handler(newValue)
				{
					this.hydrate(newValue)
				},
				deep: true
			},

			'model.ValNome.value'()
			{
				this.$emit('update:nestedModel', this.model)
			},

			'model.ValEmail.value'()
			{
				this.$emit('update:nestedModel', this.model)
			},

			'model.ValPassword.value'()
			{
				this.$emit('update:nestedModel', this.model)
			},

			'model.ConfirmValPassword.value'()
			{
				this.$emit('update:nestedModel', this.model)
			}
		}
	}
</script>
