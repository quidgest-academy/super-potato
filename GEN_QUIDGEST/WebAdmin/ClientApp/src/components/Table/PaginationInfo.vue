<template>
	<div class="text-right justify-content-center">
		<span v-if="currentPageRowsLength !== 0">
			From 1 to {{ currentPageRowsLength }} of {{ filteredRowsLength }} entries
		</span>
		<span v-else> No results found </span>
		<span> ({{ originalRowsLength }} total records) </span>
	</div>
</template>

<script>
	export default {
		name: 'PaginationInfo',

		props: {
			/**
			 * Number of rows per page.
			 */
			currentPageRowsLength: {
				type: Number,
				default: 0
			},

			/**
			 * Number of rows per page after filtering.
			 */
			filteredRowsLength: {
				type: Number,
				default: 0
			},

			/**
			 * Total number of records.
			 */
			originalRowsLength: {
				type: Number,
				default: 0
			}
		},

		expose: []
	}
</script>
