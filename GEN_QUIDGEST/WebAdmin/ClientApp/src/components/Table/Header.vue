<template>
	<thead>
		<tr>
			<th v-if="checkboxRows" />
			<slot
				name="columns"
				:columns="columns">
				<th
					v-for="(column, key, index) in columns"
					:key="index"
					class="text-center"
					:class="{ 'vbt-sort-cursor': isSortableColumn(column) }"
					v-on="
						isSortableColumn(column)
							? { click: () => $emit('update-sort', column) }
							: {}
					">
					<slot
						name="column"
						:column="column"
						>{{ column.label }}</slot
					>

					<template v-if="isSortableColumn(column)">
						<template v-if="!isSort(column)">
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
			</slot>
		</tr>
	</thead>
</template>

<script>
	import has from 'lodash-es/has'

	export default {
		name: 'QTableHeader',

		props: {
			/**
			 * Array of table columns.
			 */
			columns: {
				type: Array,
				default: () => []
			},

			/**
			 * The object containing the necessary data to render query results.
			 */
			query: {
				type: Object,
				default: () => {}
			},

			/**
			 * True if the table has a checkbox column, false otherwise.
			 */
			checkboxRows: {
				type: Boolean,
				default: false
			}
		},

		emits: ['update-sort'],

		expose: [],

		data() {
			return {
				select_all_rows: false
			}
		},

		methods: {
			isSort(column) {
				if (this.query.sort.name === null) {
					return false
				}

				return this.query.sort.name === column.name
			},

			isSortableColumn(column) {
				if (!has(column, 'sort')) {
					return false
				} else {
					return column.sort
				}
			}
		}
	}
</script>
