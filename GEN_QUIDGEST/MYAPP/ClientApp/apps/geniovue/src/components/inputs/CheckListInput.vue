<template>
	<q-checkbox
		v-for="(option, optionIdx) in selectOptions"
		:key="optionIdx"
		v-model="selectOptions[optionIdx].selected"
		:disabled="disabled"
		:readonly="readonly"
		:label="option.value" />
</template>

<script>
	import { unref } from 'vue'
	import cloneDeep from 'lodash-es/cloneDeep'
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@quidgest/clientapp/constants/enums'

	export default {
		name: 'QCheckList',

		emits: ['update:modelValue'],

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The string vaue to be edited by the input.
			 */
			modelValue: {
				type: Array,
				default: () => []
			},

			/**
			 * Sizing class for the control.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Whether the field is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is readonly.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is required.
			 */
			isRequired: {
				type: Boolean,
				default: false
			},

			/**
			 * Options available for selection.
			 */
			options: {
				type: Array,
				default: () => []
			},

			/**
			 * Additional classes to be applied to the control.
			 */
			classes: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		data() {
			return {
				selectOptions: [],

				controlId: this.id || this._.uid
			}
		},

		mounted() {
			// Set selected options
			// Must be called this way because it will be modified when checking or unchecking options
			this.selectOptions = this.setSelectedOptions()
		},

		computed: {
			/**
			 * Get array of selected option values.
			 */
			selectedOptionsValue() {
				const selectValues = []
				let selectOption = {}

				for (const idx in this.selectOptions) {
					selectOption = this.selectOptions[idx]
					if (selectOption.selected !== false) selectValues.push(selectOption.value)
				}

				return selectValues
			}
		},

		methods: {
			/**
			 * Get array of all possible options, each with a property set to whether the option is selected or not.
			 * @returns {Array} An array of options with their selected state.
			 */
			setSelectedOptions() {
				// Make copy of all options
				const selectOptions = cloneDeep(this.options)

				// Set property of each option to it's state, selected or not, based on whether it's value is in the modelValue array
				for (const idx in selectOptions) {
					const selectOption = selectOptions[idx]
					// Option's value is in the modelValue array
					if (
						this.modelValue.findIndex((elem) => elem === unref(selectOption.value)) > -1
					)
						selectOption.selected = true
					// Option's value is in not the modelValue array
					else selectOption.selected = false
				}

				return selectOptions
			}
		},

		watch: {
			selectOptions: {
				handler() {
					this.$emit('update:modelValue', this.selectedOptionsValue)
				},
				deep: true
			}
		}
	}
</script>
