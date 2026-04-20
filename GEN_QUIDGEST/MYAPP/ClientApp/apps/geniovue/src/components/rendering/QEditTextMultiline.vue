<template>
	<base-input-structure
		:id="`${tableName}_${rowIndex}_${columnName}`"
		d-flex-inline
		:class="containerClasses"
		:label-attrs="{ class: 'i-text__label' }">
		<q-text-area
			:id="`${tableName}_${rowIndex}_${columnName}`"
			:rows="1"
			:cols="10"
			:size="size"
			:class="classes"
			:disabled="options.disabled"
			:readonly="options.readonly"
			:model-value="value"
			@update:model-value="$emit('update', $event)" />
	</base-input-structure>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { inputSize } from '@quidgest/clientapp/constants/enums'

	import BaseInputStructure from '@/components/inputs/BaseInputStructure.vue'

	export default {
		name: 'QEditTextMultiline',

		emits: ['update', 'loaded'],

		components: {
			BaseInputStructure
		},

		props: {
			/**
			 * The current value of the textarea input.
			 */
			value: {
				type: String,
				default: ''
			},

			/**
			 * The name of the table in the database, used to construct the control ID.
			 */
			tableName: {
				type: String,
				required: true
			},

			/**
			 * The index of the current row, used to construct the control ID.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name of the column in the database, used to construct the control ID.
			 */
			columnName: {
				type: String,
				required: true
			},

			/**
			 * Configuration options for the textarea such as whether it is read-only and the error display type.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Sizing class for the textarea, often indicating a relative size such as 'small', 'medium', or 'large'.
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * An array of additional classes to apply to the textarea.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of classes to be applied to the textarea container.
			 */
			containerClasses: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		mounted()
		{
			this.$emit('loaded')
		}
	}
</script>
