<template>
	<div>
		<template v-if="order === 'asc'">
			<slot name="vbt-sort-asc-icon"> </slot>
		</template>

		<template v-else-if="order === 'desc'">
			<slot name="vbt-sort-desc-icon"> </slot>
		</template>

		<template v-else>
			<slot name="vbt-no-sort-icon"> </slot>
		</template>
	</div>
</template>

<script>
	import { findIndex } from 'lodash-es'

	export default {
		name: 'SortIcon',

		props: {
			/**
			 * Array of table column sortings.
			 */
			sort: {
				type: Array,
				default: () => []
			},

			/**
			 * Object that contains all necessary information regarding a column of the table.
			 */
			column: {
				type: Object,
				default: () => {}
			}
		},

		data: function () {
			return {}
		},

		expose: [],

		computed: {
			order() {
				const index = findIndex(this.sort, {
					vbt_col_id: this.column.vbt_col_id
				})
				if (index === -1) {
					return null
				} else {
					return this.sort[index].order
				}
			}
		}
	}
</script>
