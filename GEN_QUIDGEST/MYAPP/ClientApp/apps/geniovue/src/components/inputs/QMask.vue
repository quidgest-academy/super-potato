<template>
	<q-text-field
		v-model="curValue"
		ref="field"
		role="textbox"
		:id="controlId"
		:size="size"
		:class="classes"
		:readonly="readonly"
		:disabled="disabled"
		:required="isRequired"
		:max-length="maxLength"
		:aria-label="label"
		:aria-labelledby="labelId"
		:placeholder="placeholder"
		:data-testid="dataTestid"
		@change="$emit('change', $event)" />
</template>

<script>
	// https://github.com/beholdr/maska
	import { create } from 'maska'
	import _isEmpty from 'lodash-es/isEmpty'
	import _assignIn from 'lodash-es/assignIn'

	import { inputSize } from '@quidgest/clientapp/constants/enums'

	export default {
		name: 'QMask',

		emits: [
			'change',
			'update:modelValue'
		],

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The string value to be edited by the input.
			 */
			modelValue: String,

			/**
			 * The format to be used for masking input, can be an object specifying patterns or a string for simple masks.
			 */
			maskFormat: [Object, String],

			/**
			 * Data attribute for test ID used in testing frameworks.
			 */
			dataTestid: {
				type: String,
				default: null
			},

			/**
			 * Specifies the type of mask to use, defined by a set of pre-determined types.
			 */
			maskType: {
				type: String,
				required: true
			},

			/**
			 * Sizing class for the control.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Indicates if the control is disabled and cannot be interacted with.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Indicates if the control is read-only.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Indicates if the control should be marked as required.
			 */
			isRequired: {
				type: Boolean,
				default: false
			},

			/**
			 * Limits the maximum number of characters that can be entered.
			 */
			maxLength: {
				type: Number,
				default: null
			},

			/**
			 * An array of custom classes to apply to the input component.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * The accessible label for the input element described by a string.
			 */
			label: {
				type: String,
				default: null
			},

			/**
			 * ID which refers to the element that labels the control for accessibility purposes.
			 */
			labelId: {
				type: String,
				default: null
			},

			/**
			 * Placeholder text shown when no value is entered.
			 */
			placeholder: {
				type: String,
				default: ''
			}
		},

		expose: [],

		setup()
		{
			return {
				maskaInstance: null
			}
		},

		data()
		{
			return {
				controlId: this.id || `input-mask-${this._.uid}`
			}
		},

		mounted()
		{
			this.maskaInstance = create(this.$refs.field?.inputRef, this.getTokens())
		},

		beforeUnmount()
		{
			if(typeof this.maskaInstance?.destroy === 'function')
				this.maskaInstance.destroy()
			this.maskaInstance = null
		},

		computed: {
			/**
			 * Proxy computed property for v-model to provide two-way data binding.
			 */
			curValue: {
				get()
				{
					return this.modelValue
				},
				set(newValue)
				{
					this.$emit('update:modelValue', newValue)
				}
			}
		},

		methods: {
			/**
			 * Determines the configuration for input masking based on the maskType.
			 * @returns {Object} Configuration object for 'maska' input mask.
			 */
			getTokens()
			{
				const defaultConfig = {
					mask: '',
					tokens: {
						0: { pattern: /[0-9]/ },
						X: { pattern: /[0-9a-zA-Z]/ },
						S: { pattern: /[a-zA-Z]/ },
						A: { pattern: /[a-zA-Z]/, uppercase: true },
						a: { pattern: /[a-zA-Z]/, lowercase: true },
						'!': { escape: true },
						'*': { repeat: true }
					}
				}
				let customConfig = {}

				switch (this.maskType)
				{
					// Postal code
					case 'CP':
						customConfig = {
							mask: '0000-000'
						}
						break
					// Taxpayer Nr.
					case 'NC':
						customConfig = {
							mask: '000000000'
						}
						break
					// NIB
					case 'IB':
						customConfig = {
							mask: '0000 0000 00000000000 00'
						}
						break
					// Social Security Nr.
					case 'SS':
						customConfig = {
							mask: '00000000000'
						}
						break
					// IBAN
					case 'IN':
						customConfig = {
							mask: 'AA00 0000 0000 0000 0000 0000 0000 0000 00'
						}
						break
					// License plate
					case 'MA':
						customConfig = {
							mask: 'AA-AA-AA',
							tokens: {
								A: { pattern: /[A-Za-z0-9]/, uppercase: true }
							}
						}
						break
					case 'MP':
						customConfig =
							typeof this.maskFormat === 'string'
								? { mask: this.maskFormat }
								: this.maskFormat
						break
					case 'EM':
						customConfig = {
							mask: 'x*@y*.z*',
							tokens: {
								x: { pattern: /[0-9a-zA-Z_\-.]/, lowercase: true },
								y: { pattern: /[0-9a-zA-Z\-.]/, lowercase: true },
								z: { pattern: /[a-zA-Z]/, lowercase: true },
								'*': { repeat: true }
							}
						}
						break
					case 'UP':
						customConfig = {
							mask: 'A*',
							tokens: {
								A: { pattern: /.*/, uppercase: true },
								'*': { repeat: true }
							}
						}
						break
					case 'LO':
						customConfig = {
							mask: 'a*',
							tokens: {
								a: { pattern: /.*/, lowercase: true },
								'*': { repeat: true }
							}
						}
						break
				}

				return _assignIn(defaultConfig, customConfig)
			}
		}
	}
</script>
