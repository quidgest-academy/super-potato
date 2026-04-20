<template>
	<div>
		<a
			class="dropdown-item"
			href=""
			@click.prevent="handleSelect">
			<div
				v-if="isSingleMode"
				class="custom-control custom-radio">
				<input
					v-model="selected_value"
					type="radio"
					class="custom-control-input"
					:value="option.value" />
				<q-label
					class="custom-control-label"
					:label="option.name" />
			</div>
			<div
				v-else
				class="custom-control custom-checkbox">
				<input
					v-model="option_selected"
					type="checkbox"
					class="custom-control-input vbt-checkbox" />
				<q-label
					class="custom-control-label"
					:label="option.name" />
			</div>
		</a>
	</div>
</template>

<script>
	import { includes, cloneDeep } from 'lodash-es'

	export default {
		name: 'MultiSelectItem',
		props: {
			/**
			 * Object that contains all necessary information regarding a column of the table.
			 */
			column: {
				type: Object,
				default: () => {}
			},

			/**
			 * Filter option.
			 */
			option: {
				type: Object,
				default: () => {
					return {
						name: 'option one',
						value: 'option one'
					}
				}
			},

			/**
			 * Filter option index.
			 */
			index: {
				type: [Number, String],
				default: 0
			},

			/**
			 * True if the column filter is a radio group, false if it is a group of checkboxes.
			 */
			isSingleMode: {
				type: Boolean,
				default: true
			},

			/**
			 * True if all column filter options are selected, false otherwise.
			 */
			isAllOptionsSelected: {
				type: Boolean,
				default: false
			},

			/**
			 * Array of selected option indexes.
			 */
			selectedOptionIndexes: {
				type: Array,
				default: () => []
			}
		},

		emits: ['on-select-option', 'on-deselect-option'],

		expose: [],

		data: function () {
			return {
				option_selected: false,
				selected_value: ''
			}
		},

		watch: {
			selectedOptionIndexes: {
				handler: function (newVal) {
					const new_selected_option_indices = cloneDeep(newVal)
					this.option_selected = includes(new_selected_option_indices, this.index)
				},
				deep: true
			},

			option_selected(newVal) {
				if (newVal) {
					this.selected_value = this.option.value
				} else {
					this.selected_value = ''
				}
			}
		},

		methods: {
			handleSelect() {
				if (this.option_selected) {
					this.$emit('on-deselect-option', this.index)
				} else {
					this.$emit('on-select-option', this.index)
				}
			}
		}
	}
</script>
