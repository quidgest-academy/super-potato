<template v-if="MultiYearStatus">
	<q-badge
		color="rgb(64,71,79)"
		:title="title">
		<q-icon icon="data-systems" />
		<span>
			{{ Year }}
		</span>
	</q-badge>
</template>

<script>
	import { mapGetters } from 'vuex'

	export default {
		props: {
			/**
			 * The text that the badge should present on hover.
			 */
			title: {
				type: String,
				default: ''
			}
		},

		expose: [],

		computed: {
			...mapGetters(['Year', 'MultiYearStatus'])
		}
	}
</script>
