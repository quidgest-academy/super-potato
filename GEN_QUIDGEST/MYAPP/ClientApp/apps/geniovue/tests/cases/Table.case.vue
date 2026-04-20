<template>
	<div class="q-table-list">
		<div>
			<q-table
				:name="tableTest.config.name"
				:rows="tableTest.rows"
				:columns="tableTest.columns"
				:config="tableTest.config"
				:total-rows="tableTest.totalRows"
				:filters="tableTest.filters"
				:group-filters="tableTest.groupFilters"
				:active-filters="tableTest.activeFilters"
				:header-level="1"
				:readonly="tableTest.readonly"
				:rows-checked="tableTest.rowsChecked"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="importDataAction"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction(tableTest, $event)"
				@hide-popup="hidePopupAction(tableTest, $event)">
				<template #header>
					<q-table-config
						:table-ctrl="tableTest"
						@show-popup="showPopupAction(tableTest, $event)"
						@hide-popup="hidePopupAction(tableTest, $event)" />
				</template>
			</q-table>
		</div>

		<div>
			<q-table
				:name="tableTest.config.name"
				:rows="tableTest.rows"
				:columns="tableTest.columns"
				:config="{ ...tableTest.config, tableTitle: 'BASIC TYPES (READ-ONLY)' }"
				:total-rows="tableTest.totalRows"
				:group-filters="tableTest.groupFilters"
				:active-filters="tableTest.activeFilters"
				:header-level="1"
				readonly
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="displayEmit"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction"
				@hide-popup="hidePopupAction" />
		</div>

		<div>
			<q-table
				:name="tableTest.config.name"
				:rows="tableTest.rows"
				:columns="tableTest.columns"
				:config="{ ...tableTest.config, tableTitle: 'BASIC TYPES (LIMITS)', showLimitsInfo: true }"
				:total-rows="tableTest.totalRows"
				:filters="tableTest.filters"
				:group-filters="tableTest.groupFilters"
				:active-filters="tableTest.activeFilters"
				:header-level="1"
				:readonly="tableTest.readonly"
				:rows-checked="tableTest.rowsChecked"
				:table-limits="tableLimits01"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="importDataAction"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction(tableTest, $event)"
				@hide-popup="hidePopupAction(tableTest, $event)" />
		</div>

		<div>
			<q-table
				:name="tableTest.config.name"
				:rows="tableTest.rows"
				:columns="tableTest.columns.map(obj => ({ ...obj, scrollData: 5 }))"
				:config="{ ...tableTest.config, tableTitle: 'BASIC TYPES (SCROLL)' }"
				:total-rows="tableTest.totalRows"
				:filters="tableTest.filters"
				:group-filters="tableTest.groupFilters"
				:active-filters="tableTest.activeFilters"
				:header-level="1"
				:readonly="tableTest.readonly"
				:rows-checked="tableTest.rowsChecked"
				:table-limits="tableLimits01"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="importDataAction"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction(tableTest, $event)"
				@hide-popup="hidePopupAction(tableTest, $event)" />
		</div>

		<div>
			<q-table
				:name="tableTestEdit.config.name"
				:rows="tableTestEdit.rows"
				:columns="tableTestEdit.columns"
				:config="tableTestEdit.config"
				:total-rows="tableTestEdit.totalRows"
				:group-filters="tableTestEdit.groupFilters"
				:active-filters="tableTestEdit.activeFilters"
				:header-level="1"
				:readonly="tableTestEdit.readonly"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="displayEmit"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction"
				@hide-popup="hidePopupAction" />
		</div>

		<div>
			<q-table
				:name="tableTestReorder.config.name"
				:rows="tableTestReorder.rows"
				:columns="tableTestReorder.columns"
				:config="tableTestReorder.config"
				:total-rows="tableTestReorder.totalRows"
				:group-filters="tableTestReorder.groupFilters"
				:active-filters="tableTestReorder.activeFilters"
				:header-level="1"
				:readonly="tableTestReorder.readonly"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="displayEmit"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction"
				@hide-popup="hidePopupAction" />
		</div>

		<div>
			<q-table
				:name="tableTestDates.config.name"
				:rows="tableTestDates.rows"
				:columns="tableTestDates.columns"
				:config="tableTestDates.config"
				:total-rows="tableTestDates.totalRows"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction" />
		</div>

		<div>
			<q-table
				:name="tableTestOther.config.name"
				:rows="tableTestOther.rows"
				:columns="tableTestOther.columns"
				:config="{ ...tableTestOther.config, tableTitle: 'OTHER TYPES' }"
				:total-rows="tableTestOther.totalRows"
				:filters="tableTestOther.filters"
				:group-filters="tableTestOther.groupFilters"
				:active-filters="tableTestOther.activeFilters"
				:header-level="1"
				:readonly="tableTestOther.readonly"
				:rows-checked="tableTestOther.rowsChecked"
				:table-limits="tableLimits01"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="importDataAction"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction(tableTestOther, $event)"
				@hide-popup="hidePopupAction(tableTestOther, $event)" />
		</div>

		<div>
			<q-table
				:name="tableTestOther.config.name"
				:rows="tableTestOther.rows"
				:columns="tableTestOther.columns.map(obj => ({ ...obj, scrollData: 5 }))"
				:config="{ ...tableTestOther.config, tableTitle: 'OTHER TYPES (SCROLL)' }"
				:total-rows="tableTestOther.totalRows"
				:filters="tableTestOther.filters"
				:group-filters="tableTestOther.groupFilters"
				:active-filters="tableTestOther.activeFilters"
				:header-level="1"
				:readonly="tableTestOther.readonly"
				:rows-checked="tableTestOther.rowsChecked"
				:table-limits="tableLimits01"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@on-export-data="exportDataAction"
				@on-import-data="importDataAction"
				@on-export-template="exportTemplateAction"
				@show-popup="showPopupAction(tableTestOther, $event)"
				@hide-popup="hidePopupAction(tableTestOther, $event)" />
		</div>

		<div>
			<q-table
				:name="tableTestTotaler.config.name"
				:rows="tableTestTotaler.rows"
				:columns="tableTestTotaler.columns"
				:config="tableTestTotaler.config"
				:total-rows="tableTestTotaler.totalRows"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestTotaler.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestTotaler.rowsSelected, $event)"
				@select-rows="selectRows(tableTestTotaler.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestTotaler.rowsSelected)" />
		</div>

		<div>
			<q-table
				:name="tableTestTotalerSelected.config.name"
				:rows="tableTestTotalerSelected.rows"
				:columns="tableTestTotalerSelected.columns"
				:config="tableTestTotalerSelected.config"
				:total-rows="tableTestTotalerSelected.totalRows"
				:rows-selected="tableTestTotalerSelected.rowsSelected"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestTotalerSelected.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestTotalerSelected.rowsSelected, $event)"
				@select-rows="selectRows(tableTestTotalerSelected.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestTotalerSelected.rowsSelected)" />
		</div>

		<div>
			<q-table
				:name="tableTestSelectMultiple.config.name"
				:rows="tableTestSelectMultiple.rows"
				:columns="tableTestSelectMultiple.columns"
				:config="tableTestSelectMultiple.config"
				:total-rows="tableTestSelectMultiple.totalRows"
				:rows-selected="tableTestSelectMultiple.rowsSelected"
				:rows-checked="tableTestSelectMultiple.rowsChecked"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestSelectMultiple.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestSelectMultiple.rowsSelected, $event)"
				@select-rows="selectRows(tableTestSelectMultiple.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestSelectMultiple.rowsSelected)"
				@row-group-action="rowGroupAction" />
		</div>

		<div>
			<q-table
				:name="tableTestSelectMultipleMultiAction.config.name"
				:rows="tableTestSelectMultipleMultiAction.rows"
				:columns="tableTestSelectMultipleMultiAction.columns"
				:config="tableTestSelectMultipleMultiAction.config"
				:total-rows="tableTestSelectMultipleMultiAction.totalRows"
				:rows-selected="tableTestSelectMultipleMultiAction.rowsSelected"
				:rows-checked="tableTestSelectMultipleMultiAction.rowsChecked"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestSelectMultipleMultiAction.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestSelectMultipleMultiAction.rowsSelected, $event)"
				@select-rows="selectRows(tableTestSelectMultipleMultiAction.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestSelectMultipleMultiAction.rowsSelected)"
				@row-group-action="rowGroupAction" />
		</div>

		<div>
			<q-table
				:name="tableTestSelectSingle.config.name"
				:rows="tableTestSelectSingle.rows"
				:columns="tableTestSelectSingle.columns"
				:config="tableTestSelectSingle.config"
				:total-rows="tableTestSelectSingle.totalRows"
				:rows-selected="tableTestSelectSingle.rowsSelected"
				:rows-checked="tableTestSelectSingle.rowsChecked"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestSelectSingle.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestSelectSingle.rowsSelected, $event)"
				@select-rows="selectRows(tableTestSelectSingle.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestSelectSingle.rowsSelected)" />
		</div>

		<div>
			<q-table
				:name="tableTestRemoveRows.config.name"
				:rows="tableTestRemoveRows.rows"
				:columns="tableTestRemoveRows.columns"
				:config="tableTestRemoveRows.config"
				:total-rows="tableTestRemoveRows.totalRows"
				:rows-selected="tableTestRemoveRows.rowsSelected"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction"
				@select-row="selectRow(tableTestRemoveRows.rowsSelected, $event)"
				@unselect-row="unselectRow(tableTestRemoveRows.rowsSelected, $event)"
				@select-rows="selectRows(tableTestRemoveRows.rowsSelected, $event)"
				@unselect-all-rows="unselectAllRows(tableTestRemoveRows.rowsSelected)"
				@remove-row="removeRow(tableTestRemoveRows.rows, $event)" />
		</div>

		<div>
			<q-table
				:name="tableTestPaginationNormal.config.name"
				:rows="tableTestPaginationNormal.rows"
				:columns="tableTestPaginationNormal.columns"
				:config="tableTestPaginationNormal.config"
				:total-rows="tableTestPaginationNormal.totalRows"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction" />
		</div>

		<div>
			<q-table
				:name="tableTestPaginationAlt.config.name"
				:rows="tableTestPaginationAlt.rows"
				:columns="tableTestPaginationAlt.columns"
				:config="tableTestPaginationAlt.config"
				:total-rows="tableTestPaginationAlt.totalRows"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction" />
		</div>

		<div>
			<q-table
				:name="tableTest.config.name"
				:rows="[]"
				:columns="tableTest.columns"
				:config="tableTest.config"
				:total-rows="0"
				@execute-action="executeAction"
				@row-action="rowAction"
				@cell-action="cellAction" />
		</div>

		<!--Sub-components-->
		<div>
			<h1>Sub-components</h1>
		</div>

		<div>
			<h2>Import/Export</h2>
		</div>

		<div>
			<h3>Export</h3>
			<q-table-export
				:options="exportOptions01"
				:texts="tableTest.texts" />
		</div>

		<div>
			<h3>Import</h3>
			<q-table-import
				modal-id="data-import"
				:texts="tableTest.texts"
				:options="importOptions01"
				:template-options="importTemplateOptions01"
				:data-import-response="tableTest.config.dataImportResponse" />
		</div>

		<div>
			<h2>Group Filters</h2>
		</div>

		<div>
			<h3>Single Select Filters</h3>
			<q-table-static-filters :group-filters="groupFiltersSingle01" />
		</div>

		<div>
			<h3>Multiple Select Filters</h3>
			<q-table-static-filters :group-filters="groupFiltersMultiple01" />
		</div>

		<div>
			<h3>Active Filter</h3>
			<q-table-static-filters :active-filters="activeFilters01" />
		</div>

		<div>
			<h2>Pagination</h2>
		</div>

		<div>
			<h3>Normal Pagination (No Pages)</h3>
			<q-table-pagination
				:page="1"
				:per-page="paginationNormal01.perPage"
				:total="0"
				:num-visible-pagination-buttons="paginationNormal01.numVisiblePaginationButtons" />
		</div>

		<div>
			<h3>Normal Pagination (First Page)</h3>
			<q-table-pagination
				:page="1"
				:per-page="paginationNormal01.perPage"
				:total="paginationNormal01.rowCount"
				:num-visible-pagination-buttons="paginationNormal01.numVisiblePaginationButtons" />
		</div>

		<div>
			<h3>Normal Pagination (Second Page)</h3>
			<q-table-pagination
				:page="2"
				:per-page="paginationNormal01.perPage"
				:total="paginationNormal01.rowCount"
				:num-visible-pagination-buttons="paginationNormal01.numVisiblePaginationButtons" />
		</div>

		<div>
			<h3>Normal Pagination (Middle Page)</h3>
			<q-table-pagination
				:page="paginationNormal01.page"
				:per-page="paginationNormal01.perPage"
				:total="paginationNormal01.rowCount"
				:num-visible-pagination-buttons="paginationNormal01.numVisiblePaginationButtons" />
		</div>

		<div>
			<h3>Normal Pagination (Second Last Page)</h3>
			<q-table-pagination
				:page="(paginationNormal01.rowCount / paginationNormal01.perPage) - 1"
				:per-page="paginationNormal01.perPage"
				:total="paginationNormal01.rowCount"
				:num-visible-pagination-buttons="paginationNormal01.numVisiblePaginationButtons" />
		</div>

		<div>
			<h3>Normal Pagination (Last Page)</h3>
			<q-table-pagination
				:page="paginationNormal01.rowCount / paginationNormal01.perPage"
				:per-page="paginationNormal01.perPage"
				:total="paginationNormal01.rowCount"
				:num-visible-pagination-buttons="paginationNormal01.numVisiblePaginationButtons" />
		</div>

		<div>
			<h3>Alternate Pagination (No Pages)</h3>
			<q-table-pagination-alt
				:page="1"
				:per-page="paginationAlt01.perPage"
				:total="0"
				:has-more-pages="false" />
		</div>

		<div>
			<h3>Alternate Pagination (First Page)</h3>
			<q-table-pagination-alt
				:page="1"
				:per-page="paginationAlt01.perPage"
				:total="paginationAlt01.rowCount"
				:has-more-pages="paginationAlt01.hasMore" />
		</div>

		<div>
			<h3>Alternate Pagination (Second Page)</h3>
			<q-table-pagination-alt
				:page="2"
				:per-page="paginationAlt01.perPage"
				:total="paginationAlt01.rowCount"
				:has-more-pages="paginationAlt01.hasMore" />
		</div>

		<div>
			<h3>Alternate Pagination (Middle Page)</h3>
			<q-table-pagination-alt
				:page="paginationAlt01.page"
				:per-page="paginationAlt01.perPage"
				:total="paginationAlt01.rowCount"
				:has-more-pages="paginationAlt01.hasMore" />
		</div>

		<div>
			<h3>Alternate Pagination (Last Page)</h3>
			<q-table-pagination-alt
				:page="paginationAlt01.rowCount / paginationAlt01.perPage"
				:per-page="paginationAlt01.perPage"
				:total="paginationAlt01.rowCount"
				:has-more-pages="false" />
		</div>

		<div>
			<h2>Limits</h2>
		</div>

		<div>
			<h3>Limits Info</h3>
			<q-table-limit-info
				:limits="tableLimits01"
				:table-name-plural="tableLimitsText01.tableNamePlural"
				:texts="tableTest.texts" />
		</div>

		<div>
			<h2>Checklist</h2>
		</div>

		<div>
			<h3>Row checkbox</h3>
			<q-table-checklist-checkbox
				:value="checklistCheckboxRow01.value"
				:table-name="checklistCheckboxRow01.tableName"
				:readonly="checklistCheckboxRow01.readonly"
				:row-key="checklistCheckboxRow01.rowKey" />
		</div>

		<div>
			<h3>Header checkbox</h3>
			<q-table-checklist-checkbox
				:value="checklistCheckboxRow01.value"
				:table-name="checklistCheckboxRow01.tableName"
				:readonly="checklistCheckboxRow01.readonly" />
		</div>

		<div>
			<h2>Column dropdown</h2>
		</div>

		<div>
			<h2>Active Filters</h2>
		</div>

		<div>
			<h3>Active Filters</h3>
			<q-table-active-filters
				has-filters-active
				:searchable-columns="searchableColumns01"
				:filters="filterArray01"
				:texts="tableTest.texts" />
		</div>
	</div>
</template>

<script>
	import fakeData from './Table.mock.js'

	import QTableConfig from '@/components/table/QTableConfig.vue'

	export default {
		name: 'QTableContainer',

		docsfile: './docs/table/QTable.md',

		components: {
			QTableConfig
		},

		emits: [
			'show-popup',
			'hide-popup'
		],

		inheritAttrs: false,

		expose: [],

		data()
		{
			return fakeData
		},

		methods: fakeData.simpleUsageMethods
	}
</script>
