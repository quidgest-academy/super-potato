<template>
	<div
		:id="id"
		:class="['container-fluid', 'nested-form-container', $attrs.class]"
		role="dialog"
		aria-hidden="true">
		<template v-if="activeComponent">
			<component
				:is="activeComponent"
				ref="formRef"
				:key="formProps.id"
				:buttons-override="rowComponentProps.formButtonsOverride"
				:parent-form-mode="rowComponentProps.parentFormMode"
				:parent-table-permissions="rowComponentProps.permissions"
				:actions-placement="rowComponentProps.actionsPlacement"
				is-multiple
				v-bind="formProps"
				@close="(...args) => formClose(...args)"
				@edit="(...args) => $emit('edit', ...args)"
				@deselect="(...args) => $emit('deselect', ...args)"
				@insert-form="(...args) => $emit('insert-form', ...args)"
				@update-model-id="(...args) => $emit('update-model-id', ...args)"
				@after-save-form="(...args) => $emit('after-save-form', ...args)"
				@cancel-insert="(...args) => $emit('cancel-insert', ...args)"
				@is-form-dirty="handleIsFormDirty"
				@update-form-mode="handleUpdateFormMode"
				@update:nested-model="handleModelUpdateEvent"
				@update:model-value="updateModelValue"
				@custom-event="handleCustomEvent" />
		</template>
		<div
			v-else
			class="nested-form-no-record">
			<img :src="`${resourcesPath}empty_card_container.png`" />

			<span>
				{{ texts.chooseElement }}
			</span>

			<template v-if="allowFormActions.insert">
				{{ texts.or }}
				<q-button
					id="ext-insert-new"
					variant="bold"
					:label="texts.insert"
					@click="() => $emit('insert-record')">
					<q-icon v-bind="insertIcon" />
				</q-button>
			</template>
		</div>
	</div>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { formModes } from '@quidgest/clientapp/constants/enums'
	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'
	import { NestedFormConfig } from '@/mixins/fieldControl.js'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		chooseElement: 'Choose an element from the list.',
		or: 'or',
		insert: 'Insert'
	}

	export default {
		name: 'QFormContainer',

		emits: [
			'after-save-form',
			'cancel-insert',
			'change-form-mode',
			'close',
			'closed-form',
			'custom-event',
			'deselect',
			'edit',
			'insert-form',
			'insert-record',
			'is-form-dirty',
			'update:nested-model',
			'update:model-value',
			'update-model-id'
		],

		inheritAttrs: false,

		props: {
			/**
			 * Model value.
			 */
			modelValue: [String, Number, Object],

			/**
			 * Unique identifier for the control.
			 */
			id: {
				type: String,
				required: true
			},

			/**
			 * The nested form data required to load form.
			 * {
			 *     id,
			 *     historyBranchId,
			 *     component,
			 *     mode,
			 *     nestedModel (optional)
			 * }
			 */
			formData: {
				type: Object,
				default: () => ({}),
				validator: (val) =>
					!_isEmpty(val) &&
					(val.id || val.mode === formModes.new) &&
					val.historyBranchId &&
					typeof val.component === 'string' &&
					Object.values(formModes).includes(val.mode)
			},

			/**
			 * Props for form component.
			 */
			rowComponentProps: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Configuration of the nested form.
			 */
			nestedFormConfig: {
				type: Object,
				default: () => new NestedFormConfig()
			},

			/**
			 * The resources path.
			 */
			resourcesPath: {
				type: String,
				required: true
			},

			/**
			 * The necessary strings to be used inside the component.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * The list of the allowed form actions
			 */
			allowFormActions: {
				type: Object,
				default: () => ({
					insert: false
				})
			}
		},

		expose: ['handleLeaveForm'],

		data()
		{
			return {
				activeComponent: null,
				formProps: {
					isNested: true
				},
				insertIcon: {
					icon: 'add',
					type: 'svg'
				}
			}
		},

		mounted()
		{
			this.updateFormData(this.formData)

			this.$eventHub.on('new-extended-record', this.emitModelUpdate)

			const eventData = {
				supportFormId: this.id,
				rowKey: this.modelValue ?? undefined,
				formMode: this.formProps.mode
			}
			this.handleModelUpdateEvent(eventData)
		},

		beforeUnmount()
		{
			this.$eventHub.off('new-extended-record', this.emitModelUpdate)
		},

		methods: {
			emitModelUpdate(val)
			{
				this.$emit('update:model-value', val)
			},

			/**
			 * Used to updated the form props each time the form data is updated.
			 * @param {object} newFormData The new data of the form
			 */
			updateFormData(newFormData)
			{
				const result = {
					component: null,
					props: {}
				}

				if (!_isEmpty(newFormData))
				{
					result.component = newFormData.component
					result.props = {
						id: newFormData.id,
						mode: newFormData.mode,
						isNested: true,
						modes: '',
						historyBranchId: newFormData.historyBranchId,
						nestedModel: newFormData.nestedModel,
						nestedFormConfig: this.nestedFormConfig,
						prefillValues: newFormData.prefillValues
					}
				}

				this.formClose()
				this.formProps = result.props
				this.activeComponent = result.component
			},

			/**
			 * Clears the form props when the form is closed.
			 * @param {any} args Arguments to emit
			 */
			formClose(args)
			{
				this.activeComponent = null
				this.formProps = {
					isNested: true
				}

				if (args)
					this.$emit('close', args)
			},

			/**
			 * Emits the changes in the form model to update the dirty state.
			 * @param {object} newModelValue The new model data
			 */
			handleModelUpdateEvent(newModelValue)
			{
				this.$emit('update:nested-model', newModelValue)
			},

			/**
			 * Emits a custom event defined in the form .vue file.
			 * @param {any} args Arguments to emit
			 */
			handleCustomEvent(args)
			{
				this.$emit('custom-event', args)
			},

			/**
			 * Emits the dirty state of the form container to the parent forms.
			 * "afterFormSave" refers to what situation the event was emitted in: after a form modification (false) or after saving the form (true).
			 * @param {object} eventData The event data
			 */
			handleIsFormDirty(eventData)
			{
				if (this.formData)
				{
					const data = {
						id: this.formData.id,
						isDirty: eventData.isDirty,
						afterFormSave: eventData.afterFormSave
					}

					this.$emit('is-form-dirty', data)
				}
			},

			/**
			 * Emits the new form mode to update in the form data properties.
			 * @param {string} mode The new form mode
			 */
			handleUpdateFormMode(mode)
			{
				this.$emit('change-form-mode', mode)
			},

			updateModelValue(value) {
				this.$emit("update:model-value", value);
			},

			/**
			 *  Handles the leaving of the current open form
			 * @param {function} next Function to be executed after leaving the form
			 */
			handleLeaveForm(next)
			{
				if (this.$refs.formRef)
					this.$refs.formRef.cancel(next)
				else
					next()
			}
		},

		watch: {
			formData: {
				handler(newValue)
				{
					this.updateFormData(newValue)
				},
				deep: true
			}
		}
	}
</script>
