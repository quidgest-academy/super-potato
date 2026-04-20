<template>
	<q-input-group
		:class="classes"
		:size="size">
		<template
			v-if="$slots.prepend"
			#prepend>
			<slot name="prepend"></slot>
		</template>
		<q-text-field
			v-model="curValue"
			:id="controlId"
			:name="name"
			autocomplete="off"
			:placeholder="placeholder"
			:disabled="disabled"
			:readonly="readonly"
			:type="inputType"
			:max-length="maxLength > 0 ? maxLength : null"
			@keyup.enter="emitEnter" />
		<template
			v-if="!readonly"
			#append>
			<q-button
				:disabled="disabled"
				:aria-label="showPasswordLabel"
				@mousedown="showPassword"
				@mouseup="hidePassword"
				@mouseleave="hidePassword"
				@touchstart="showPassword"
				@touchend="hidePassword"
				@touchcancel="hidePassword">
				<q-icon :icon="eyeIcon" />
			</q-button>
		</template>
	</q-input-group>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@quidgest/clientapp/constants/enums'

	export default {
		name: 'QPassword',

		emits: ['update:modelValue', 'keyup-enter'],

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Name attribute for the control.
			 */
			name: String,

			/**
			 * The value bound to the control, reflecting the current input.
			 */
			modelValue: {
				type: String,
				default: ''
			},

			/**
			 * Whether the field is readonly.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * The maximum allowed input length for the control.
			 */
			maxLength: {
				type: Number,
				default: -1
			},

			/**
			 * Sizing class for the control.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * An array of custom classes to be applied to the control.
			 */
			classes: {
				type: [String, Array],
				default: () => []
			},

			/**
			 * The placeholder text for the input when no value is present.
			 */
			placeholder: {
				type: String,
				default: ''
			},
			/**
			 * The text for the button to show the password.
			 */
			showPasswordLabel: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || `i-password-${this._.uid}`,
				protectedMode: true
			}
		},

		computed: {
			/**
			 * Specifies the type of input field, toggling between password visibility.
			 */
			inputType()
			{
				return this.protectedMode ? 'password' : 'text'
			},

			/**
			 * Proxy computed property to facilitate two-way binding of the modelValue prop through v-model.
			 */
			curValue: {
				get()
				{
					return this.modelValue
				},
				set(newValue)
				{
					if (this.modelValue !== newValue)
						this.$emit('update:modelValue', newValue)
				}
			},

			/**
			 * Determines the icon to be shown based on the protected mode state (either 'view' or 'password-hidden').
			 */
			eyeIcon()
			{
				return this.protectedMode ? 'view' : 'password-hidden'
			}
		},

		methods: {
			isEmpty: _isEmpty,

			showPassword()
			{
				this.protectedMode = false
			},

			hidePassword()
			{
				this.protectedMode = true
			},

			emitEnter()
			{
				this.$emit('keyup-enter')
			}
		}
	}
</script>
