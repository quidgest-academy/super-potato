<template>
	<q-numeric-input
		:id="`${tableName}_${rowIndex}_${columnName}`"
		:data-testid="$attrs['data-testid']"
		:size="size"
		:classes="classes"
		:thousands-separator="options.numberFormat?.groupSeparator"
		:decimal-point="options.numberFormat?.decimalSeparator"
		:max-integers="options.maxDigits"
		:is-decimal="options.decimalPlaces !== undefined && options.decimalPlaces > 0"
		:max-decimals="options.decimalPlaces"
		:currency-symbol="options.currencySymbol"
		:readonly="options.readonly"
		:model-value="Number(value)"
		data-table-action-selected="false"
		tabindex="-1"
		:aria-label="options.label"
		@update:model-value="onUpdateModelValue" />
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@quidgest/clientapp/constants/enums'

	import QNumericInput from '@/components/inputs/NumericInput.vue'

	export default {
		name: 'QEditNumeric',

		emits: ['update', 'loaded'],

		components: {
			QNumericInput
		},

		props: {
			/**
			 * The current value of the numeric control as a number or string.
			 */
			value: {
				type: [Number, String],
				default: 0
			},

			/**
			 * The name of the database table associated with this control, used for unique ID construction.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the database row associated with this control, used for unique ID construction.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name of the database column associated with this control, used for unique ID construction.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * The options to configure the numeric input behavior, like readOnly state, currency, separators, etc.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Sizing class for the control, based on predefined options.
			 */
			size: {
				type: String,
				default: inputSize.mini,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Additional classes to apply to the control.
			 */
			classes: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		mounted()
		{
			this.$emit('loaded')
		},

		methods: {
			/**
			 * Called when the model value is updated.
			 * @param {Object} event
			 */
			onUpdateModelValue(event)
			{
				this.$emit('update', event)
			}
		}
	}
</script>
