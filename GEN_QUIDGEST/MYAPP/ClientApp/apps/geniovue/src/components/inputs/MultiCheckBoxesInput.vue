<template>
	<q-row>
		<q-col
			v-for="(column, columnId) in columnList"
			:key="columnId">
			<q-checkbox
				v-for="item in column"
				:key="item.itemId"
				v-model="item.isChecked"
				:disabled="disabled"
				:readonly="readonly"
				:label="item.option.value" />
		</q-col>
	</q-row>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'
	import _filter from 'lodash-es/filter'
	import _findIndex from 'lodash-es/findIndex'
	import _map from 'lodash-es/map'

	import { inputSize, labelAlignment } from '@quidgest/clientapp/constants/enums'

	export default {
		name: 'QCheckboxGroup',

		emits: ['update:modelValue'],

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The string vaue to be edited by the input
			 */
			modelValue: {
				type: Array,
				required: true
			},

			/**
			 * The list of the options
			 */
			options: {
				type: Array,
				default: () => []
			},

			/**
			 * Number of columns
			 */
			numberOfColumns: {
				type: Number,
				default: 0
			},

			/**
			 * Sizing class for the control
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * The label position
			 */
			labelPosition: {
				type: String,
				default: '',
				validator: (value) => _isEmpty(value) || Reflect.has(labelAlignment, value)
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
			}
		},

		expose: [],

		data()
		{
			return {
				/**
				 * The unique control identifier
				 */
				controlId: this.id || this._.uid,
				columnList: {}
			}
		},

		created()
		{
			this.setData()
		},

		methods: {
			setData()
			{
				if (!_isEmpty(this.options))
				{
					const vm = this,
						numColumns = this.numberOfColumns === 0 ? this.options.length : this.numberOfColumns

					let midCount = Math.ceil(this.options.length / numColumns)

					if (!Number.isSafeInteger(midCount))
						midCount = 1

					this.columnList =  {}

					for (let col = 0; col < numColumns; col++)
					{
						Reflect.set(this.columnList, `column-${this.controlId}-${col}`,
							_map(this.options.slice(col * midCount, col * midCount + midCount),
								(option) => {
									return {
										get isChecked()
										{
											return _findIndex(vm.modelValue || [], o => o === this.option.key) !== -1
										},
										set isChecked(value)
										{
											if (value)
												vm.$emit('update:modelValue', [...(vm.modelValue || []), this.option.key])
											else
												vm.$emit('update:modelValue', _filter(vm.modelValue || [], o => o !== this.option.key))
										},
										option,
										itemId: `mcb-item-${this.controlId}-${col}`
									}
								}))
					}
				}
			}
		}
	}
</script>
