<template>
	<q-table-row
		ref="rowComp"
		v-bind="$attrs"
		data-testid="parentRow"
		:row="row"
		:row-key-path="rowKeyPath"
		:row-index="rowIndex"
		:columns="columnHierarchy[columnsLevel]"
		:table-name="tableName"
		:prop-row-classes="getRowClasses(row, columnsLevel)"
		:row-title="getRowTitle(row)"
		:is-valid="rowIsValid(row)"
		:cell-titles="getRowCellDataTitles(row, $attrs.columns)"
		:row-selected-for-group="isRowSelected(row)"
		type="TreeList"
		:level="level"
		:expand-icon="expandIcon"
		:collapse-icon="collapseIcon"
		:show-child-rows="row.showChildRows"
		:navigated-row-key-path="navigatedRowKeyPath"
		:texts="texts"
		@toggle-show-children="setChildRowsVisibility"
		@go-to-row="(...args) => $emit('go-to-row', ...args)"
		@navigate-row="(...args) => $emit('navigate-row', ...args)"
		@loaded="$emit('loaded')"
		@keydown="rowOnKeydown" />

	<template v-if="showChildren">
		<component :is="hasSubTable ? 'tr' : 'v-fragment'">
			<component
				:is="hasSubTable ? 'td' : 'v-fragment'"
				:colspan="columnHierarchy[columnsLevel] ? columnHierarchy[columnsLevel].length : null"
				:style="{ padding: 0 }">
				<component
					:is="hasSubTable ? 'table' : 'v-fragment'"
					class="c-table">
					<q-table-header
						v-if="hasSubTable"
						:columns="columnHierarchy[columnsLevel + 1] ?? null"
						:table-name="tableName + '_sub_' + row.rowKey"
						:allow-filters="false"
						:texts="texts" />
					<component
						:is="hasSubTable ? 'tbody' : 'v-fragment'"
						class="c-table__body">
						<q-tree-table-row
							ref="subRowComp"
							v-bind="$attrs"
							v-for="childRow in row.children"
							:key="childRow.rowKey"
							:id="childRow.rowKey"
							:row="childRow"
							:row-key-path="getSubRowKeyPath(childRow)"
							:row-index="rowIndex + '_' + childRow.Rownum"
							:table-name="tableName"
							:column-hierarchy="columnHierarchy"
							:prop-row-classes="getRowClasses(childRow)"
							:row-title="getRowTitle(childRow)"
							:is-valid="rowIsValid(childRow)"
							:cell-titles="getRowCellDataTitles(childRow, $attrs.columns)"
							:row-selected-for-group="isRowSelected(childRow)"
							:level="level + 1"
							:expand-icon="expandIcon"
							:collapse-icon="collapseIcon"
							:navigated-row-key-path="navigatedRowKeyPath"
							:texts="texts"
							@toggle-show-children="setChildRowsVisibility"
							@go-to-row="(...args) => $emit('go-to-row', ...args)"
							@navigate-row="(...args) => $emit('navigate-row', ...args)"
							@loaded="onSubRowLoaded"
							@unloaded="onSubRowUnloaded"
							@sub-rows-loaded="$emit('sub-rows-loaded')" />
					</component>
				</component>
			</component>
		</component>
	</template>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import { getParentMultiIndex } from '@/mixins/listFunctions.js'

	export default {
		name: 'QTreeTableRow',

		emits: [
			'loaded',
			'unloaded',
			'toggle-show-children',
			'go-to-row',
			'sub-rows-loaded',
			'navigate-row'
		],

		components: {
			QTableRow: defineAsyncComponent(() => import('./QTableRow.vue')),
			QTableHeader: defineAsyncComponent(() => import('./QTableHeader.vue'))
		},

		inheritAttrs: false,

		props: {
			/**
			 * The key path composed of row keys leading to this particular row's position within a hierarchical data structure.
			 */
			rowKeyPath: {
				type: Array,
				required: true
			},

			/**
			 * The index or identifier uniquely representing this row's position within its parent collection.
			 */
			rowIndex: {
				type: [Number, String],
				required: true
			},

			/**
			 * The name associated with the table or sub-table in which this row resides.
			 */
			tableName: {
				type: String,
				default: ''
			},

			/**
			 * Name of the icon used to visually represent the ability to expand a collapsible row.
			 */
			expandIcon: {
				type: String,
				default: 'square-plus'
			},

			/**
			 * Name of the icon used to visually represent the ability to collapse an expanded row.
			 */
			collapseIcon: {
				type: String,
				default: 'square-minus'
			},

			/**
			 * The hierarchical level of depth for this row, with 0 being the top level.
			 */
			level: {
				type: Number,
				default: 0
			},

			/**
			 * The hierarchical structure of columns for a tree table, potentially defining multiple levels of nested sub-tables.
			 */
			columnHierarchy: {
				type: Array,
				default: () => []
			},

			/**
			 * The data object for the row, which includes the cell values and potentially other state information.
			 */
			row: {
				type: Object,
				required: true
			},

			/**
			 * Specifies the index of the row that is navigated to.
			 * Can be a mulit-index which is the indexes for each level (in tree tables) separated by underscores.
			 */
			navigatedRowKeyPath: {
				type: Array,
				default: () => []
			},

			/**
			 * Localized text strings to be used within the component (for labels, headers, etc.).
			 */
			texts: {
				type: Object,
				required: true
			},
		},

		inject: ['getRowClasses', 'getRowTitle', 'rowIsValid', 'getCellDataDisplay', 'getRowCellDataTitles', 'isRowSelected', 'getRowCellDataTitles'],

		expose: [],

		data() {
			return {
				showChildren: false,
				subRowsLoaded: 0,
				subRowsUnloaded: 0
			}
		},

		unmounted() {
			//Signal that the component is unloaded
			this.$emit('unloaded')
		},

		methods: {
			getParentMultiIndex,

			/**
			 * Set whether the sub-rows are visible
			 * @param {object} eventData
			 */
			setChildRowsVisibility(eventData) {
				if (eventData?.row?.rowKey === this.row.rowKey) this.showChildren = eventData.show
				this.$emit('toggle-show-children', eventData)
			},

			/**
			 * Get the row key path for a sub row
			 * @param {object} row
			 */
			getSubRowKeyPath(row)
			{
				if(!this.rowKeyPath)
					return []
				return this.rowKeyPath.concat(row.rowKey)
			},

			/**
			 * Called when a sub-row is mounted
			 */
			onSubRowLoaded()
			{
				this.subRowsLoaded++
				if(this.subRowsLoaded === this.row?.children?.length)
				{
					this.subRowsLoaded = 0
					this.$emit('sub-rows-loaded')
				}
			},

			/**
			 * Called when a sub-row is unmounted
			 */
			onSubRowUnloaded()
			{
				this.subRowsUnloaded++
				if(this.subRowsUnloaded === this.row?.children?.length)
				{
					this.subRowsUnloaded = 0
					this.$emit('sub-rows-loaded')
				}
			},

			/**
			 * Row keydown handler
			 * @param event {object} Event object
			 */
			rowOnKeydown(event)
			{
				const key = event?.key
				const element = event?.target
				const rowComp = this.$refs?.rowComp
				const rowElem = rowComp?.$refs?.rowElem

				switch(key)
				{
					case "ArrowLeft":
						// If collapsed, focus on parent row
						if(!this.showChildren)
						{
							const index = rowElem.getAttribute('index')
							this.$emit('navigate-row', this.getParentMultiIndex(index))
						}
					case "-":
						event.preventDefault()
						// When focused on the row element, if expanded, collapse
						if(element === rowElem && this.showChildren)
						{
							event.stopPropagation()
							rowComp?.setSubRowsVisibility(false)
						}
						break;
					case "ArrowRight":
					case "+":
						event.preventDefault()
						// When focused on the row element, if collapsed, expand
						if(element === rowElem && !this.showChildren)
						{
							event.stopPropagation()
							rowComp?.setSubRowsVisibility(true)
						}
						break;
				}
			}
		},

		computed: {
			columnsLevel() {
				if (this.level >= this.columnHierarchy.length) {
					return this.columnHierarchy.length - 1
				} else if (this.level < 1) {
					return 0
				} else {
					return this.level
				}
			},

			hasSubTable() {
				if (this.columnsLevel >= this.columnHierarchy.length - 1) {
					return false
				}

				const currentLevelFirstDataColumn = this.columnHierarchy[this.columnsLevel].find((x) => x.area)
				if (!currentLevelFirstDataColumn) return false
				const subLevelFirstDataColumn = this.columnHierarchy[this.columnsLevel + 1].find((x) => x.area)
				if (!subLevelFirstDataColumn) return false

				return currentLevelFirstDataColumn.area !== subLevelFirstDataColumn.area
			}
		}
	}
</script>
