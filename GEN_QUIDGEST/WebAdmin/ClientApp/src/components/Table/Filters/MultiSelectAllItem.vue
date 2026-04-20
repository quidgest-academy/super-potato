<template>
	<div>
		<a
			class="dropdown-item"
			href=""
			@click.prevent="handleSelect">
			<div class="custom-control custom-checkbox">
				<input
					v-model="option_selected"
					type="checkbox"
					class="custom-control-input vbt-checkbox" />
				<q-label
					class="custom-control-label"
					:label="text" />
			</div>
		</a>
	</div>
</template>

<script>
	export default {
		name: 'MultiSelectAllItem',
		props: {
			/**
			 * True if all filter options are selected, false otherwise.
			 */
			isAllOptionsSelected: {
				type: Boolean,
				default: false
			},

			/**
			 * Filter label.
			 */
			text: {
				type: String,
				default: 'Any'
			}
		},
		emits: ['on-select-all-option', 'on-deselect-all-option'],

		expose: [],

		data() {
			return {
				option_selected: false
			}
		},

		watch: {
			isAllOptionsSelected(newVal) {
				this.option_selected = newVal
			}
		},

		methods: {
			handleSelect() {
				if (this.option_selected) {
					this.$emit('on-deselect-all-option')
				} else {
					this.$emit('on-select-all-option')
				}
			}
		}
	}
</script>
