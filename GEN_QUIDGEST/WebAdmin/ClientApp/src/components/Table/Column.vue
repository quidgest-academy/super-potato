<template>
	<th
		class="text-center"
		:class="{ 'vbt-sort-cursor': isSortableColumn }"
		v-on="isSortableColumn ? { click: () => sort() } : {}">
		<slot
			name="column"
			:column="column"
			>{{ column.label }}</slot
		>

		<template v-if="isSortableColumn">
			<template v-if="!isSort">
				<div>
					<slot name="no-sort-icon"> &#x1F825;&#x1F827; </slot>
				</div>
			</template>

			<template v-else>
				<template v-if="query.sort.order === 'asc'">
					<div>
						<slot name="sort-asc-icon"> &#x1F825; </slot>
					</div>
				</template>

				<template v-else-if="query.sort.order === 'desc'">
					<slot name="sort-desc-icon">
						<div>&#x1F827;</div>
					</slot>
				</template>

				<template v-else>
					<div>
						<slot name="no-sort-icon"> &#x1F825;&#x1F827; </slot>
					</div>
				</template>
			</template>
		</template>
	</th>
</template>

<script>
	import has from 'lodash-es/has'

	export default {
		name: 'QTableColumn',
		components: {},
		props: {
			/**
			 * Object that contains all necessary information regarding a column of the table.
			 */
			column: {
				type: Object,
				default: () => {}
			},

			/**
			 * The object containing the necessary data to render query results.
			 */
			query: {
				type: Object,
				default: () => {}
			}
		},

		emits: ['update-sort'],

		expose: [],

		data() {
			return {}
		},

		computed: {
			isSort() {
				if (!this.query.sort.name === null) {
					return false
				}

				return this.query.sort.name === this.column.name
			},

			isSortableColumn() {
				if (!has(this.column, 'sort')) {
					return false
				} else {
					return this.column.sort
				}
			}
		},
		methods: {
			sort() {
				this.$emit('update-sort', this.column)
			}
		}
	}
</script>
