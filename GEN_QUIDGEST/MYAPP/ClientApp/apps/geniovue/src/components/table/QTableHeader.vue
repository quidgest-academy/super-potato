<template>
	<thead class="c-table__head">
		<tr
			ref="headerRowRef"
			v-bind="$attrs"
			:index="rowIndex">
			<slot
				name="columns"
				:columns="columns">
				<th
					v-for="column in columns"
					:key="column.name"
					:id="headerCellIds[column.name]"
					:class="columnClasses(column)"
					:aria-sort="getSortingAttribute(column)"
					:data-column-name="column.name"
					@mousedown="onColumnMouseDown"
					@mousemove="onColumnMouseMove"
					@mouseup="onColumnMouseUp">
					<!-- BEGIN: TABLE LIST TOTALIZER TITLE COLUMN -->
					<div
						v-if="isTotalizerColumn(column)"
						class="column-header-content">
						<q-icon icon="sigma" />
					</div>
					<!-- BEGIN: FOR: TABLE LIST ROW ACTIONS -->
					<div
						v-else-if="isActionsColumn(column) || isDragAndDropColumn(column)"
						class="column-header-content">
						<q-icon icon="actions" />
						<span class="hidden-elem">{{ column.label }}</span>
					</div>
					<!-- END: FOR: TABLE LIST ROW ACTIONS -->
					<!-- BEGIN: Checklist header cell content -->
					<div
						v-else-if="isChecklistColumn(column)"
						class="column-header-content">
						<slot :name="`column_${getCellSlotName(column)}.prepend`" />
						<slot
							:name="`column_${getCellSlotName(column)}`"
							:column="column">
							<q-action-list
								borderless
								data-table-action-selected="false"
								placement="bottom-start"
								tabindex="-1"
								variant="text"
								:groups="checklistGroups"
								:items="checklistActions"
								:readonly="readonly || rowCount < 1"
								:title="texts.selectOptions"
								@click="onChecklistClick">
								<q-icon :icon="checklistIcon" />
							</q-action-list>
						</slot>
						<slot :name="`column_${getCellSlotName(column)}.append`" />
					</div>
					<!-- END: Checklist header cell content -->
					<!-- BEGIN: Extended row action column -->
					<div
						v-else-if="isExtendedActionsColumn(column)"
						class="extended-row-header">
						<slot
							:name="getCellSlotName(column)"
							:column="column">
							<span
								v-if="hasExtendedAction('remove-reset')"
								:key="column.name">
								<q-button
									:title="texts.resetText"
									data-table-action-selected="false"
									tabindex="-1"
									@click="emit('unselect-all-rows')">
									<q-icon icon="reset" />
								</q-button>
							</span>
						</slot>
					</div>
					<!-- END: Extended row action column -->
					<!-- BEGIN: Header cell content -->
					<div
						v-else
						class="column-header-content">
						<q-button
							v-if="allowColumnSort && isSortableColumn(column)"
							:id="getColumnId(column)"
							borderless
							class="column-header-text"
							variant="text"
							:title="getColumnTitle(column)"
							:data-control-type="getDataType(column)"
							data-table-action-selected="false"
							tabindex="-1"
							@click="toggleSorting(column)">
							<slot :name="`column_${getCellSlotName(column)}.prepend`" />
							<slot
								:name="`column_${getCellSlotName(column)}`"
								:column="column">
								{{ column.label }}
							</slot>
							<slot :name="`column_${getCellSlotName(column)}.append`" />
							<q-icon
								:class="{
									'q-table-header__no-sorting': getSorting(column) === 'undefined'
								}"
								:icon="getSortIcon(column)" />
						</q-button>
						<div
							v-else
							class="column-header-text">
							<slot :name="`column_${getCellSlotName(column)}.prepend`" />
							<slot
								:name="`column_${getCellSlotName(column)}`"
								:column="column">
								{{ column.label }}
							</slot>
							<slot :name="`column_${getCellSlotName(column)}.append`" />
						</div>

						<q-button
							v-if="allowFilters && isSearchableColumn(column)"
							:id="getTableColumnFilterId(tableName, column.name)"
							borderless
							:class="[
								'q-table-header__filter',
								{ 'q-table-header__filter--active': isFiltered(filters, column) }
							]"
							size="small"
							variant="text"
							data-control-type="edit-filter"
							:title="texts.filtersText"
							data-table-action-selected="false"
							tabindex="-1"
							@click="editFilter(column)">
							<q-icon icon="filter" />
						</q-button>
					</div>
					<!-- END: Header cell content -->
				</th>
			</slot>
		</tr>

		<tr
			v-if="props.loading"
			class="q-table-list-progress">
			<th :colspan="columns.length">
				<div class="q-table-list-progress__loader">
					<q-line-loader />
				</div>
			</th>
		</tr>
	</thead>
</template>

<script setup>
	// Utils
	import {
		columnFullName,
		columnIsFiltered,
		getTableColumnDropdownSortAscId,
		getTableColumnDropdownSortDescId,
		getTableColumnFilterId,
		isSearchableColumn,
		isSortableColumn
	} from '@/mixins/listFunctions.js'
	import has from 'lodash-es/has'
	import includes from 'lodash-es/includes'
	import { computed, inject, ref, useTemplateRef } from 'vue'

	import { QActionList } from '@quidgest/clientapp/components'

	defineOptions({
		name: 'QTableHeader',
		inheritAttrs: false
	})

	const emit = defineEmits([
		'check-all-rows',
		'check-current-page-rows',
		'check-none-rows',
		'show-filters',
		'unselect-all-rows',
		'update-sort'
	])

	const props = defineProps({
		/**
		 * Localized text strings to be used within the table header component.
		 */
		texts: {
			type: Object,
			required: true
		},

		/**
		 * An array containing column configurations, each object defines a column's properties in the table.
		 */
		columns: {
			type: Array,
			default: () => []
		},

		/**
		 * The unique name associated with the table instance.
		 */
		tableName: {
			type: String,
			default: ''
		},

		/**
		 * Flag indicating whether the table is currently in read-only mode.
		 */
		readonly: {
			type: Boolean,
			default: false
		},

		/**
		 * Flag indicating whether sorting is allowed on table columns.
		 */
		allowColumnSort: {
			type: Boolean,
			default: false
		},

		/**
		 * Flag indicating whether filters are allowed on table columns.
		 */
		allowFilters: {
			type: Boolean,
			default: true
		},

		/**
		 * The details of existing filters currently applied on the table columns.
		 */
		filters: {
			type: Array,
			default: () => []
		},

		/**
		 * The total count of rows in the table.
		 */
		rowCount: {
			type: Number,
			default: 0
		},

		/**
		 * Whether the table is loading.
		 */
		loading: {
			type: Boolean,
			default: false
		},

		/**
		 * Whether the header content is disabled.
		 */
		disabled: {
			type: Boolean,
			default: false
		},

		/**
		 * The row index. Can be a multi-index which has the index for each level (in tree tables) separated by underscores.
		 */
		rowIndex: {
			type: String,
			default: 'h'
		},

		/**
		 * The total count of rows selected in the table.
		 */
		rowsSelectedCount: {
			type: Number,
			default: 0
		},

		/**
		 * Whether all rows are selected in the table.
		 */
		allSelectedRows: {
			type: String,
			default: 'false'
		},

		/**
		 * Current system locale.
		 */
		locale: {
			type: String,
			default: 'en-US'
		},

		/**
		 * IDs for header cells.
		 */
		headerCellIds: {
			type: Object,
			default: () => ({})
		}
	})

	const mouseDown = ref(false)
	const mouseMove = ref(false)

	// Template refs
	const headerRowRef = useTemplateRef('headerRowRef')

	const getCellSlotName = inject('getCellSlotName')
	const isActionsColumn = inject('isActionsColumn')
	const isChecklistColumn = inject('isChecklistColumn')
	const isDragAndDropColumn = inject('isDragAndDropColumn')
	const isExtendedActionsColumn = inject('isExtendedActionsColumn')
	const isTotalizerColumn = inject('isTotalizerColumn')
	const hasExtendedAction = inject('hasExtendedAction')

	const nextSort = {
		asc: 'desc',
		desc: 'undefined',
		undefined: 'asc'
	}

	const checklistGroups = computed(() => [
		{
			id: 'default',
			display: 'dropdown'
		}
	])

	const checklistActions = computed(() => [
		{ key: 'all', label: props.texts.allRecordsText, group: 'default', icon: { icon: 'apply' } },
		{ key: 'page', label: props.texts.currentPageText, group: 'default', icon: { icon: 'check' } },
		{ key: 'none', label: props.texts.noneText, group: 'default', icon: { icon: 'remove' } }
	])

	const checklistIcon = computed(() => {
		return props.allSelectedRows === 'true'
			? 'checkbox-checked'
			: props.rowsSelectedCount > 0
				? 'minus-box'
				: 'checkbox-unchecked'
	})

	/**
	 * Checks if the specified column is being filtered.
	 * @param filters The filters
	 * @param column The column
	 * @returns True if it's filtered, false otherwise.
	 */
	function isFiltered(filters, column) {
		return filters.some(
			(f) =>
				f.active &&
				(f.field === `${column.area}.${column.field}` || isFiltered(f.subFilters, column))
		)
	}

	/**
	 * Get CSS classes for this column
	 * @param column {Object}
	 * @returns String
	 */
	function columnClasses(column) {
		const classes = ['q-table__column-header']

		if (isSortableColumn(column)) classes.push('q-table__column-header--sortable')

		if (isFiltered(props.filters, column)) classes.push('q-table__column-header--filtered')

		const alignments = ['text-justify', 'text-right', 'text-left', 'text-center']
		if (
			has(column, 'columnTextAlignment') &&
			includes(alignments, column.columnTextAlignment)
		) {
			classes.push(column.columnTextAlignment)
		}

		if (has(column, 'columnHeaderClasses')) {
			classes.push(column.columnHeaderClasses)
		}

		return classes.join(' ')
	}

	/**
	 * Fired on mouse down on header element
	 */
	function onColumnMouseDown() {
		mouseDown.value = true
		mouseMove.value = false
	}

	/**
	 * Fired on mouse move on header element
	 */
	function onColumnMouseMove() {
		mouseMove.value = true
	}

	/**
	 * Fired on mouse up on header element
	 */
	function onColumnMouseUp() {
		mouseDown.value = false
		mouseMove.value = false
	}

	/**
	 * Executes the action selected in the dropdown
	 * @param {string} optionKey The identifier of the selected action
	 */
	function onChecklistClick(optionKey) {
		const action =
			optionKey === 'all'
				? 'check-all-rows'
				: optionKey === 'page'
					? 'check-current-page-rows'
					: 'check-none-rows'
		emit(action)
	}

	/**
	 * Edit column filter
	 * @param {Object} column
	 */
	function editFilter(column) {
		const columnName = columnIsFiltered(props.filters, column) ? null : columnFullName(column)
		emit('show-filters', columnName)
	}

	/**
	 * Gets the column sorting
	 * @param column The column
	 */
	function getSorting(column) {
		const sortColumn = props.columns.find((c) => c.name === column.name)
		return sortColumn?.sortOrder > 0 ? (sortColumn.sortAsc ? 'asc' : 'desc') : 'undefined'
	}

	/**
	 * Gets the column sorting attribute
	 * @param column The column
	 */
	function getSortingAttribute(column) {
		const sortMap = {
			asc: 'ascending',
			desc: 'descending',
			undefined: undefined
		}
		return sortMap[getSorting(column)]
	}

	/**
	 * Toggles the column sorting
	 * @param column The column
	 */
	function toggleSorting(column) {
		if (props.allowColumnSort && !isSortableColumn(column)) return

		const sorting = nextSort[getSorting(column)]
		emit('update-sort', column.name, sorting)
	}

	/**
	 * Gets the column sorting icon
	 * @param column The column
	 */
	function getSortIcon(column) {
		const sorting = getSorting(column)
		return sorting === 'undefined'
			? 'sorting'
			: `sort-${sorting === 'asc' ? 'ascending' : 'descending'}`
	}

	/**
	 * Gets the column id
	 * @param column The column
	 */
	function getColumnId(column) {
		const sorting = getSorting(column)
		return sorting === 'undefined'
			? ''
			: sorting === 'asc'
				? getTableColumnDropdownSortAscId(props.tableName, column.name)
				: getTableColumnDropdownSortDescId(props.tableName, column.name)
	}

	/**
	 * Gets the column title
	 * @param column The column
	 */
	function getColumnTitle(column) {
		const sorting = nextSort[getSorting(column)]
		return sorting === 'asc'
			? props.texts.sortAscendingText
			: sorting === 'desc'
				? props.texts.sortDescendingText
				: props.texts.removeSortText
	}

	/**
	 * Gets the column data control type
	 * @param column The column
	 */
	function getDataType(column) {
		const sorting = getSorting(column)
		return sorting === 'asc' ? 'sort-asc' : sorting === 'desc' ? 'sort-desc' : ''
	}

	defineExpose({
		headerRowRef
	})
</script>
