<template>
	<q-container class="c-sidebar__notes-form">
		<q-row v-if="showGenericError">
			<q-col>
				<div
					class="c-sidebar__notes-form--error">
					<q-icon icon="exclamation-sign" />
					{{ texts.errorProcessingRequest }}
				</div>
			</q-col>
		</q-row>
		<q-row>
			<q-col>
				<base-input-structure
					class="i-textarea"
					v-bind="controls.Text"
					v-on="controls.Text.handlers">
					<q-text-area
						v-bind="controls.Text.props"
						v-on="controls.Text.handlers" />
					<template v-if="model.Text.hasServerErrorMessages">
						<div
							v-for="(errorMsg, index) in model.Text.serverErrorMessages"
							:key="index"
							class="c-sidebar__notes-field--error">
							<q-icon icon="exclamation-sign" />
							{{ errorMsg }}
						</div>
					</template>
				</base-input-structure>
			</q-col>
		</q-row>
		<q-row>
			<q-col>
				<base-input-structure
					class="i-radio-container"
					v-bind="controls.RecipientType"
					v-on="controls.RecipientType.handlers">
					<q-radio-group
						v-bind="controls.RecipientType.props"
						v-on="controls.RecipientType.handlers">
						<q-radio-button
							v-for="radio in controls.RecipientType.items"
							:key="radio.key"
							:label="radio.value"
							:value="radio.key" />
					</q-radio-group>
				</base-input-structure>
			</q-col>
		</q-row>
		<q-row v-if="this.model.RecipientType.value === 'T'">
			<q-col>
				<base-input-structure
					class="i-text"
					v-bind="controls.Recipient"
					v-on="controls.Recipient.handlers">
					<q-text-field
						v-bind="controls.Recipient.props"
						@change="model.Recipient.fnUpdateValueOnChange" />
				</base-input-structure>
			</q-col>
		</q-row>
		<q-row>
			<base-input-structure
				class="i-text"
				v-bind="controls.ExpirationDate"
				v-on="controls.ExpirationDate.handlers">
				<q-date-time-picker
					v-bind="controls.ExpirationDate.props"
					:model-value="model.ExpirationDate.value"
					@reset-icon-click="model.ExpirationDate.fnUpdateValue(model.ExpirationDate.originalValue ?? new Date())"
					@update:model-value="model.ExpirationDate.fnUpdateValue($event ?? '')" />
				<template v-if="model.ExpirationDate.hasServerErrorMessages">
					<div
						v-for="(errorMsg, index) in model.ExpirationDate.serverErrorMessages"
						:key="index"
						class="c-sidebar__notes-field--error">
						<q-icon icon="exclamation-sign" />
						{{ errorMsg }}
					</div>
				</template>
			</base-input-structure>
		</q-row>
		<q-row>
			<q-col>
				<q-button
					id="notes-save"
					variant="bold"
					:title="texts.save"
					@click="save">
					<q-icon icon="save" />
				</q-button>
				<q-button
					id="notes-cancel"
					:title="texts.cancel"
					@click="cancel">
					<q-icon icon="cancel" />
				</q-button>
			</q-col>
		</q-row>
	</q-container>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'

	import netAPI from '@quidgest/clientapp/network'
	import modelFieldType from '@quidgest/clientapp/models/fields'
	import fieldControlClass from '@/mixins/fieldControl.js'
	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

	import { getNoteContext } from '@/mixins/notesFunctions.js'

	export default {
		name: 'QNoteForm',

		emits: [
			/**
			 * Requests the parent component to close the form.
			 */
			'close',
			/**
			 * Notifies the parent component that a note has been saved successfully.
			 */
			'success-save'
		],

		expose: [],

		data()
		{
			return {
				texts: {
					save: computed(() => this.Resources[hardcodedTexts.save]),
					cancel: computed(() => this.Resources[hardcodedTexts.cancel]),
					errorProcessingRequest: computed(() => this.Resources[hardcodedTexts.errorProcessingRequest]),
				},

				// Used for unexpected server/network failures.
				showGenericError: false,

				model: {
					Text: new modelFieldType.MultiLineString({
						id: 'Text',
						originId: 'Text',
						field: 'Text',
						isRequired: true,
						maxLength: 255
					}),
					RecipientType: new modelFieldType.String({
						id: 'RecipientType',
						originId: 'RecipientType',
						field: 'RecipientType',
						maxLength: 1,
						arrayOptions: [
							{ num: 1, key: 'P', value: computed(() => this.Resources[hardcodedTexts.private]) },
							{ num: 2, key: 'T', value: computed(() => this.Resources[hardcodedTexts.forOthers]) }
						]
					}),
					Recipient: new modelFieldType.String({
						id: 'Recipient',
						originId: 'Recipient',
						field: 'Recipient',
						maxLength: 50
					}),
					ExpirationDate: new modelFieldType.DateTime({
						id: 'ExpirationDate',
						originId: 'ExpirationDate',
						field: 'ExpirationDate',
						isRequired: true
					})
				},

				controls: {
					Text: new fieldControlClass.MultilineStringControl({
						modelField: 'Text',
						id: 'Notes_Text',
						name: 'Text',
						size: 'block',
						label: computed(() => this.Resources[hardcodedTexts.note]),
						mustBeFilled: true,
						maxLength: 255,
						rows: 10,
						cols: 85
					}, this),
					RecipientType: new fieldControlClass.RadioGroupControl({
						modelField: 'RecipientType',
						id: 'Notes_Recipient_Type',
						name: 'RecipientType',
						size: 'block',
						maxLength: 1,
						columns: 2
					}, this),
					Recipient: new fieldControlClass.StringControl({
						modelField: 'Recipient',
						id: 'Notes_Recipient',
						name: 'Recipient',
						size: 'block',
						label: computed(() => this.Resources[hardcodedTexts.forSpecificUser]),
						placeholder: computed(() => this.Resources[hardcodedTexts.leaveBlankForEveryone])
					}, this),
					ExpirationDate: new fieldControlClass.DateControl({
						modelField: 'ExpirationDate',
						id: 'Notes_Expiration_Date',
						name: 'ExpirationDate',
						size: 'block',
						label: computed(() => this.Resources[hardcodedTexts.expirationDate]),
						dateTimeType: 'date',
						mustBeFilled: true
					}, this)
				}
			}
		},

		mounted()
		{
			this.initForm()
		},

		computed: {

		},

		methods: {
			/**
			 * Initialises default values and initialises all field controls.
			 */
			initForm()
			{
				this.model.RecipientType.value = 'P'
				this.model.ExpirationDate.value = new Date()
				for(const controlName in this.controls)
					this.controls[controlName].init(true)
			},

			/**
			 * Performs a client-side validation of the current values of the form fields.
			 * @returns {boolean} `true` if the form is valid; otherwise `false`.
			 */
			validateFormValues()
			{
				let modelIsValid = true

				for (const i in this.controls)
				{
					const ctrl = this.controls[i]

					if (!ctrl.modelField)
						continue

					const field = this.model[ctrl.modelField]

					if (typeof field === 'undefined')
						continue

					field.clearServerErrorMessages()
					const errorMsgs = []

					// If the field is required, ensures it's filled.
					if (!field.validateValue())
					{
						const errorMsg = genericFunctions.formatString(this.Resources[hardcodedTexts.fieldIsRequired], ctrl.label)
						errorMsgs.push(errorMsg)
					}

					// If the field has a maximum number of characters, ensures it hasn't been exceeded.
					if (!field.validateSize())
					{
						const errorMsg = genericFunctions.formatString(this.Resources[hardcodedTexts.maxCharsExceeded], ctrl.label, ctrl.maxLength)
						errorMsgs.push(errorMsg)
					}

					if(errorMsgs.length !== 0)
					{
						modelIsValid = false
						field.setServerErrorMessages(errorMsgs)
					}
				}

				return modelIsValid
			},

			/**
			 * Sends the note to the API.
			 */
			save()
			{
				this.showGenericError = false
				if(!this.validateFormValues()) return

				const note = {
					text: this.model.Text.value,
					destType: this.model.RecipientType.value,
					dest: this.model.Recipient.value,
					date: this.model.ExpirationDate.value,
				}

				const context = getNoteContext(this.$route)

				netAPI.postData(
					'QNotesApi',
					'CreateNewNote',
					{
						context,
						note
					},
					(data) => {
						if(!data)
						{
							this.showGenericError = true
							return
						}
						this.$emit('success-save')
					},
					() => {
						this.showGenericError = true
					}
				)
			},

			/**
			 * Cancels the form and asks the parent to close it.
			 */
			cancel()
			{
				this.$emit('close')
			}
		}
	}
</script>
