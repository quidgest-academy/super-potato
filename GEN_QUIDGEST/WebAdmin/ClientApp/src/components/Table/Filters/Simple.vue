<template>
	<div class="form-group has-clear-right">
		<span
			v-if="showClearButton"
			class="form-control-feedback vbt-simple-filter-clear"
			@click="clearFilter">
			<slot name="vbt-simple-filter-clear-icon"> </slot>
		</span>
		<input
			v-if="filterOnPressEnter"
			ref="simple_filter_input"
			type="text"
			class="form-control"
			:placeholder="column.filter.placeholder"
			@keyup.enter="updateFilterHandler" />
		<input
			v-else
			ref="simple_filter_input"
			type="text"
			class="form-control"
			:placeholder="column.filter.placeholder"
			@keyup.stop="updateFilter" />
	</div>
</template>

<script>
	import { debounce, has } from 'lodash-es'

	export default {
		name: 'TableSimple',

		props: {
			/**
			 * Object that contains all necessary information regarding a column of the table.
			 */
			column: {
				type: Object,
				default: () => {}
			}
		},

		emits: ['update-filter', 'clear-filter'],

		expose: [],

		data: function () {
			return {
				filterOnPressEnter: false,
				debounceRate: 60
			}
		},
		computed: {
			showClearButton() {
				return this.column.filter.showClearButton === undefined
					? true
					: this.column.filter.showClearButton
			},

			updateFilter(event) {
				return debounce(this.updateFilterHandler(event), this.debounceRate)
			}
		},

		mounted() {
			if (has(this.column, 'filter.init.value')) {
				this.$refs.simple_filter_input.value = this.column.filter.init.value
			}

			if (has(this.column, 'filter.filterOnPressEnter')) {
				this.filterOnPressEnter = this.column.filter.filterOnPressEnter
			}

			if (!this.filterOnPressEnter && has(this.column, 'filter.debounceRate')) {
				this.debounceRate = this.column.filter.debounceRate
			}

			this.$eventHub.on('reset-query', () => {
				if (this.$refs.simple_filter_input) {
					this.$refs.simple_filter_input.value = ''
				}
			})
		},

		methods: {
			clearFilter() {
				this.$refs.simple_filter_input.value = ''
				this.$emit('clear-filter', this.column)
			},

			// TODO - configurable debouncing
			updateFilterHandler(event) {
				this.$emit('update-filter', {
					value: event.target.value,
					column: this.column
				})
			}
		}
	}
</script>
