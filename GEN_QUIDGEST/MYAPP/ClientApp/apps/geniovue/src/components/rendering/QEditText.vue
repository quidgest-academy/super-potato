<template>
	<q-text-field
		:id="`${tableName}_${rowIndex}_${columnName}`"
		:model-value="value"
		:max-length="options.dataLength"
		:size="size"
		:class="classes"
		:disabled="options.disabled"
		:readonly="options.readonly"
		:placeholder="placeholder"
		:aria-label="options?.label"
		@update:model-value="$emit('update', $event)" />
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@quidgest/clientapp/constants/enums'

	export default {
		name: 'QEditText',

		emits: ['update', 'loaded'],

		props: {
			/**
			 * The actual value of the text input.
			 */
			value: {
				type: String,
				default: ''
			},

			/**
			 * The table name from the database, used for generating a unique ID for the input.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the row, which along with tableName and columnName, helps generate a unique ID.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The column name from the database, used in generating a unique ID for the input.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * Object containing various options to control the behavior of the input.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Class to apply to the input for sizing.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * Additional CSS classes to apply to the input.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * Placeholder text for the input when empty.
			 */
			placeholder: {
				type: String,
				default: ''
			}
		},

		expose: [],

		mounted()
		{
			this.$emit('loaded')
		}
	}
</script>
