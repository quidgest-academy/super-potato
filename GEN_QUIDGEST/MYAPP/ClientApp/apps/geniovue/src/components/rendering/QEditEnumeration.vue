<template>
	<q-select
		:id="`${tableName}_${rowIndex}_${columnName}`"
		:key="domKey"
		:model-value="value"
		:size="size"
		:class="classes"
		:items="items"
		:texts="texts"
		:badges="isMultiple"
		:multiple="isMultiple"
		inline
		required
		item-value="key"
		item-label="value"
		@update:model-value="updateSelectedValue" />
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@quidgest/clientapp/constants/enums'

	export default {
		name: 'QEditEnumeration',

		emits: ['update', 'loaded'],

		props: {
			/**
			 * The current selected value of the enumeration.
			 */
			value: {
				type: [String, Number, Array],
				default: ''
			},

			/**
			 * The name of the table in the database, used to identify the field context.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the current row in the table, used to identify the field context.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name of the column in the database, used to identify the field context.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * A set of options to configure the enumeration field, such as the component type and error display.
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
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Additional classes to apply to the control.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Object containing localized text strings for the component's texts
			 */
			texts: {
				type: Object,
				required: true
			}
		},

		expose: [],

		data()
		{
			return {
				domKey: 0
			}
		},

		mounted()
		{
			this.$emit('loaded')
		},

		computed: {
			/**
			 * The dropdown items.
			 */
			items()
			{
				return this.options.array
			},

			/**
			 * Whether multiple selection should be allowed.
			 */
			isMultiple()
			{
				return Array.isArray(this.value)
			}
		},

		methods: {
			/**
			 * Updates the currently selected value in the QSelect component and emits it to the parent component.
			 * @param {string|number} key - The key representing the selected enumeration option.
			 */
			updateSelectedValue(key)
			{
				this.$emit('update', key)
			}
		},

		watch: {
			isMultiple()
			{
				this.domKey++
			}
		}
	}
</script>
