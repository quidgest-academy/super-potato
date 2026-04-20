<template>
	<component
		v-if="component"
		:is="component"
		v-bind="suggestionsProps" />
</template>

<script>
	import { defineAsyncComponent } from 'vue'
	import _isEmpty from 'lodash-es/isEmpty'

	export default {
		name: 'QSuggestions',

		components: {
			SuggestionIndex: defineAsyncComponent(() => import('./SuggestionIndex.vue')),
			SuggestionList: defineAsyncComponent(() => import('./SuggestionList.vue'))
		},

		props: {
			/**
			 * The component name to render for suggestions. Must match one of the component names defined in `components`.
			 */
			component: {
				type: String,
				default: ''
			},

			/**
			 * Parameters to be passed to the dynamic suggestion component.
			 */
			params: {
				type: Object,
				default: () => ({})
			}
		},

		expose: [],

		computed: {
			/**
			 * Computed properties to be passed as props to the dynamically rendered component.
			 */
			suggestionsProps()
			{
				return !_isEmpty(this.params) ? { params: this.params } : {}
			}
		}
	}
</script>
