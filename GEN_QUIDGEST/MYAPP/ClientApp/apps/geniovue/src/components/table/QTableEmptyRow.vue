<template>
	<tr
		v-for="row in emptyRows"
		:key="row.id">
		<template
			v-for="column in columns"
			:key="'td-' + getColumnName(column) + row.id">
			<td
				v-if="canShowColumn(column)"
				:class="cellClasses(column)">
				<div class="q-skeleton__cell-loading" />
			</td>
		</template>
	</tr>
</template>

<script>
	import { v4 as uuidv4 } from 'uuid'
	import _has from 'lodash-es/has'
	import _includes from 'lodash-es/includes'
	import _times from 'lodash-es/times'

	export default {
		name: 'QTableEmptyRow',

		props: {
			/**
			 * Number of empty rows to be displayed.
			 */
			nRows: {
				type: Number,
				default: 0
			},

			/**
			 * Array of column configuration objects, each representing the corresponding header and type of content to be displayed.
			 */
			columns: {
				type: Array,
				default: () => []
			}
		},

		inject: [
			'canShowColumn'
		],

		expose: [],

		computed: {
			emptyRows()
			{
				return _times(this.nRows, () => ({ id: uuidv4() }))
			}
		},

		methods: {
			getColumnName(column)
			{
				return column?.name?.replace(/\./g,'_') || 'unknown'
			},

			/**
			 * Get CSS classes for column
			 * @param column {Object}
			 * @returns String
			 */
			cellClasses(column)
			{
				const classes = ['q-skeleton__cell']

				// BEGIN: Text alignment class
				const alignments = ['text-justify', 'text-right', 'text-left', 'text-center']

				// Undefined data type, use rowTextAlignment
				if (_has(column, 'rowTextAlignment') && _includes(alignments, column.rowTextAlignment))
					classes.push(column.rowTextAlignment)
				// END: Text alignment class

				// Adding user defined classes from column config to cells
				if (_has(column, 'columnClasses'))
					classes.push(column.columnClasses)

				return classes
			}
		}
	}
</script>
