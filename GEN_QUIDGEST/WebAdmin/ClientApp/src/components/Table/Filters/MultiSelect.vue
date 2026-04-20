<template>
	<div>
		<div class="dropdown">
			<a
				:id="'multifilter_' + column.name"
				class="btn btn-secondary dropdown-toggle"
				href="#"
				role="button"
				data-toggle="dropdown"
				aria-haspopup="true"
				aria-expanded="false">
				{{ title }}
			</a>
			<div
				ref="vbt_dropdown_menu"
				class="dropdown-menu scrollable-menu"
				aria-labelledby="dropdownMenuLink">
				<multi-select-all-item
					v-if="!isSingleMode && showSelectAllCheckbox"
					:text="selectAllCheckboxText"
					:is-all-options-selected="isAllOptionsSelected"
					@on-deselect-all-option="deselectAllOptions"
					@on-select-all-option="selectAllOptions"></multi-select-all-item>
				<multi-select-item
					v-for="(option, key) in options"
					:key="key"
					:index="key"
					:option="option"
					:is-single-mode="isSingleMode"
					:selected-option-indexes="selected_option_indexes"
					@on-select-option="addOption"
					@on-deselect-option="removeOption"></multi-select-item>
			</div>
		</div>
	</div>
</template>

<script>
	import { findIndex, range, filter, includes, has, cloneDeep } from 'lodash-es'

	import MultiSelectItem from './MultiSelectItem.vue'
	import MultiSelectAllItem from './MultiSelectAllItem.vue'

	export default {
		name: 'MultiSelect',
		components: {
			MultiSelectItem,
			MultiSelectAllItem
		},
		props: {
			/**
			 * Object that contains all necessary information regarding a column of the table.
			 */
			column: {
				type: Object,
				default: () => {}
			},

			/**
			 * Column filter options.
			 */
			options: {
				type: Array,
				default: () => {}
			}
		},
		emits: ['update-multi-select-filter'],

		expose: [],

		data: function () {
			return {
				selected_option_indexes: [],
				canEmit: false
			}
		},

		computed: {
			optionsCount() {
				return this.options.length
			},

			title() {
				const title = this.column.filter.placeholder
					? this.column.filter.placeholder
					: 'Select options'

				if (this.selected_option_indexes.length === 0) {
					return title
				}

				if (
					this.selected_option_indexes.length > 0 &&
					this.selected_option_indexes.length <= 1
				) {
					return this.options[this.selected_option_indexes[0]].name
				} else {
					return this.selected_option_indexes.length + ' selected'
				}
			},

			mode() {
				let mode = 'single'
				if (has(this.column.filter, 'mode') && this.column.filter.mode === 'multi') {
					mode = 'multi'
				}
				return mode
			},

			isSingleMode() {
				return this.mode === 'single'
			},

			isAllOptionsSelected() {
				return this.options.length === this.selected_option_indexes.length
			},

			showSelectAllCheckbox() {
				if (!has(this.column.filter, 'select_all_checkbox')) {
					return true
				} else {
					return this.column.filter.select_all_checkbox.visibility
				}
			},

			selectAllCheckboxText() {
				if (!has(this.column.filter, 'select_all_checkbox')) {
					return 'Select All'
				} else {
					return has(this.column.filter.select_all_checkbox, 'text')
						? this.column.filter.select_all_checkbox.text
						: 'Select All'
				}
			}
		},

		watch: {
			selected_option_indexes(newVal) {
				if (!this.canEmit) return

				const filtered_options = filter(this.options, (option, index) => {
					return includes(newVal, index)
				})

				const payload = {}
				payload.column = cloneDeep(this.column)
				payload.selected_options = []

				filtered_options.forEach((option) => {
					payload.selected_options.push(option.value)
				})

				this.$emit('update-multi-select-filter', payload)
			}
		},

		mounted() {
			this.$refs.vbt_dropdown_menu.addEventListener(
				'click',
				function (e) {
					e.stopPropagation()
				},
				false
			)

			this.$eventHub.on('reset-query', () => {
				this.selected_option_indexes = []
			})

			const lastIndex = this.optionsCount - 1

			if (has(this.column, 'filter.init.value')) {
				if (this.isSingleMode) {
					const index = this.column.filter.init.value
					if (index > lastIndex || index < 0) return
					this.addOption(index)
				} else {
					if (Array.isArray(this.column.filter.init.value)) {
						this.column.filter.init.value.forEach((index) => {
							if (index > lastIndex || index < 0) return
							this.addOption(index)
						})
					} else {
						console.log("Initial value for 'multi' mode should be an array")
					}
				}
			}

			this.$nextTick().then(() => {
				this.canEmit = true
			})
		},
		methods: {
			addOption(index) {
				if (this.isSingleMode) {
					this.resetSelectedOptions()
					this.selected_option_indexes.push(index)
				} else {
					const res = findIndex(this.selected_option_indexes, function (option_index) {
						return option_index === index
					})
					if (res === -1) {
						this.selected_option_indexes.push(index)
					}
				}
			},

			selectAllOptions() {
				this.resetSelectedOptions()
				this.selected_option_indexes = range(this.options.length)
			},

			deselectAllOptions() {
				this.selected_option_indexes = []
			},

			removeOption(index) {
				if (this.isSingleMode) {
					this.resetSelectedOptions()
				} else {
					const res = findIndex(this.selected_option_indexes, function (option_index) {
						return option_index === index
					})
					if (res > -1) {
						this.selected_option_indexes.splice(res, 1)
					}
				}
			},

			resetSelectedOptions() {
				this.selected_option_indexes = []
			}
		}
	}
</script>
