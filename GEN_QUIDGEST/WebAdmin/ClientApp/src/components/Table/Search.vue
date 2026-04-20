<template v-if="visibility">
	<span
		v-if="showClearButton"
		class="form-control-feedback vbt-global-search-clear"
		@click="clearGlobalSearch">
		<slot name="global-search-clear-icon"> &#x24E7; </slot>
	</span>
	<q-text-field
		v-model="searchValue"
		:size="size"
		:placeholder="placeholder"
		@keyup.enter="emitSearch">
		<template #append>
			<q-button-group>
				<q-button
					variant="text"
					size="small"
					@click="emitSearch">
					<q-icon icon="magnify" />
				</q-button>
				<q-button
					variant="text"
					size="small"
					@click="resetQuery">
					<q-icon icon="close" />
				</q-button>
			</q-button-group>
		</template>
	</q-text-field>
</template>

<script>
	export default {
		name: 'QTableSearch',
		props: {
			/**
			 * Searchbar placeholder.
			 */
			initPlaceholder: {
				type: String,
				default: ''
			},

			/**
			 * Custom searchbar classes.
			 */
			initClasses: {
				type: String,
				default: ''
			},

			/**
			 * True if the searchbar is visible, false otherwise.
			 */
			initVisibility: {
				type: Boolean,
				default: true
			},

			/**
			 * True if the searches should be case sensitive, false otherwise.
			 */
			initCaseSensitive: {
				type: Boolean,
				default: false
			},

			/**
			 * True if the searchbar should display a button to refresh results, false otherwise.
			 */
			initShowRefreshButton: {
				type: Boolean,
				default: true
			},

			/**
			 * True if the searchbar should display a button to reset the search, false otherwise.
			 */
			initShowResetButton: {
				type: Boolean,
				default: true
			},

			/**
			 * True if the searchbar should display a button to clear the search, false otherwise.
			 */
			initShowClearButton: {
				type: Boolean,
				default: false
			},

			/**
			 * True if the searchbar should search on 'Enter' press, false otherwise.
			 */
			initSearchOnPressEnter: {
				type: Boolean,
				default: false
			},

			/**
			 * Search debounce rate, in ms.
			 */
			initSearchDebounceRate: {
				type: Number,
				default: 60
			},

			/**
			 * Searchbar size.
			 */
			size: {
				type: String,
				default: 'xlarge'
			}
		},
		emits: [
			'clear-global-search',
			'update-global-search-handler',
			'update-global-search',
			'emit-search',
			'reset-query'
		],

		expose: [],

		data() {
			return {
				searchValue: '',
				placeholder: this.initPlaceholder,
				classes: this.initClasses,
				visibility: this.initVisibility,
				caseSensitive: this.initCaseSensitive,
				showRefreshButton: this.initShowRefreshButton,
				showResetButton: this.initShowResetButton,
				showClearButton: this.initShowClearButton,
				searchOnPressEnter: this.initSearchOnPressEnter,
				searchDebounceRate: this.initSearchDebounceRate
			}
		},

		computed: {
			searchBarContainerStyle() {
				return {
					display: 'block'
				}
			}
		},

		methods: {
			emitSearch() {
				this.$emit('emit-search', this.searchValue)
			},

			clearGlobalSearch() {
				this.searchValue = ''
				this.$emit('clear-global-search')
			},

			resetQuery() {
				this.searchValue = ''
				this.$emit('reset-query')
			}
		}
	}
</script>
