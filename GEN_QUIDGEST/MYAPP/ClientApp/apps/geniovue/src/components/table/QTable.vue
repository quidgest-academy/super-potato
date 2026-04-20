<template>
	<div
		:id="controlId"
		:class="['q-table-list', ...classes, sizeClass, $attrs.class, isListVisible ? tableModeClasses : null, { 'q-table-list--loading': !loaded }]"
		:data-loading="!loaded"
		data-testid="table-container">
		<div
			v-if="showHeader"
			class="q-table__head justify-content-between">
			<q-row
				v-if="tableTitle.length > 0 || $slots.title || hasActionBar"
				:gutter="4">
				<!-- BEGIN: Table title -->
				<q-col
					v-if="tableTitle.length > 0 || $slots.title"
					class="c-table__title"
					cols="auto">
					<component
						v-if="hasLabel"
						:is="headerTag"
						:id="`label_${controlId}`">
						<slot
							name="title"
							:table-title="tableTitle">
							{{ tableTitle }}
						</slot>
					</component>
					<q-popover-help
						v-if="popoverText"
						:id="id"
						:help-control="helpControl"
						:label="tableTitle"
						:texts="texts" />
				</q-col>
				<!-- END: Table title -->

				<template v-if="hasActionBar">
					<q-col cols="auto">
						<slot name="header" />
					</q-col>

					<q-col
						v-if="$props.viewModes.length > 0"
						cols="auto">
						<q-table-view-mode-config
							:model-value="$props.activeViewModeId"
							:view-modes="$props.viewModes"
							:texts="$props.texts"
							@update:model-value="(newVal) => emitEvent('update:active-view-mode', newVal)" />
					</q-col>

					<!-- BEGIN: Row reorder toggle -->
					<q-col
						v-if="showRowDragAndDropOption && !readonly"
						cols="auto">
						<q-button
							:id="controlId + '-row-reorder-btn'"
							borderless
							:variant="hasRowDragAndDrop ? 'tonal' : 'outlined'"
							:title="texts.rowDragAndDropTitle"
							@click="toggleShowRowDragAndDrop">
							<q-icon icon="reorder" />
						</q-button>
					</q-col>
					<!-- END: Row reorder toggle -->

					<q-col
						v-if="allowFileExport || allowFileImport"
						cols="auto">
						<q-button-group>
							<!-- BEGIN: Export menu -->
							<q-table-export
								v-if="allowFileExport"
								:options="exportOptions"
								:texts="texts"
								@export-data="exportData" />
							<!-- END: Export menu -->
							<!-- BEGIN: Import menu -->
							<q-table-import
								v-if="allowFileImport"
								modal-id="data-import"
								:options="importOptions"
								:template-options="importTemplateOptions"
								:data-import-response="dataImportResponse"
								:texts="texts"
								@import-data="importData"
								@show-import-popup="emitEvent('show-popup', importModalProps)"
								@hide-import-popup="emitEvent('hide-popup', 'data-import')"
								@export-template="(format) => exportTemplate(format)" />
							<!-- END: Import menu -->
						</q-button-group>
					</q-col>

					<!-- BEGIN: Toggle show/hide filters -->
					<template v-if="hasFilters">
						<q-col cols="auto">
							<q-switch
								:model-value="filtersVisible"
								size="small"
								show-state-labels
								:label="texts.filtersText"
								:true-label="texts.showText"
								:false-label="texts.hideText"
								@update:model-value="toggleShowFilters" />

							<q-button
								v-if="filtersVisible"
								data-testid="clear-filters"
								pill
								size="small"
								class="q-table__clear-filters"
								:label="texts.clear"
								:title="texts.clear"
								@click="emitEvent('clear-filters')">
								<q-icon icon="remove" />
							</q-button>
						</q-col>
					</template>
					<!-- END: Toggle show/hide filters -->
				</template>
			</q-row>

			<!-- BEGIN: Global search text -->
			<q-row
				v-if="showSearchBar && searchBarConfig.visibility"
				:gutter="4"
				justify="end">
				<q-col cols="auto">
					<q-table-search
						:default-search-column="defaultSearchColumn"
						:searchable-columns="searchableColumns"
						:placeholder="`${texts.searchText} ${defaultSearchColumnLabel}`"
						:show-refresh-button="searchBarConfig.showRefreshButton"
						:texts="texts"
						:disabled="!loaded"
						@search-by-column="searchByColumn" />
				</q-col>
			</q-row>
			<!-- END: Global search text -->

			<q-tooltip-help
				v-if="tooltipText"
				:help-control="helpControl"
				:anchor="anchorId"
				:label="tableTitle" />
		</div>

		<q-subtitle-help
			v-if="subtitleText"
			:help-control="helpControl"
			:id="id" />

		<q-info-banner-help
			v-if="hasInfoBanner"
			:help-control="helpControl"
			:id="id" />

		<component
			:is="isListVisible && !(rowComponent === 'q-form-container' && formName !== '') ? 'div' : 'v-fragment'"
			class="table-and-filters-wrapper">
			<!-- BEGIN: Filters -->
			<div
				v-if="hasFilters"
				v-show="filtersVisible">
				<div
					v-if="!!$slots.filters"
					class="q-table__global-filters">
					<slot name="filters" />
				</div>

				<q-table-static-filters
					v-if="hasStaticFilters"
					:id="`${config.name}-filters`"
					:active-filters="activeFilters"
					:group-filters="groupFilters"
					:date-formats="dateFormats"
					:disabled="!loaded"
					:locale="locale"
					:texts="texts"
					:check-box-size="checkBoxSize"
					:radio-button-size="radioButtonSize"
					@update:active-filters="updateActiveFilters"
					@update:group-filters="updateGroupFilters" />

				<q-row
					v-if="config.showApplyButton"
					:gutter="4">
					<q-col cols="auto">
						<q-button
							data-testid="apply-filters"
							variant="bold"
							:label="texts.applyText"
							:title="texts.applyText"
							:disabled="!unappliedFilters"
							@click="emitEvent('refresh')">
							<q-icon icon="ok" />
						</q-button>
					</q-col>
				</q-row>

				<q-table-current-filters
					v-if="hasCustomFilters"
					:filters="filters"
					:searchable-columns="searchableColumns"
					:texts="texts"
					:filter-operators="filterOperators"
					@update:filters="emitEvent('update:filters', $event)"
					@show-filters="showConfig({ selectedTab: 'filters', selectedFilter: $event })" />
			</div>
			<!-- END: Filters -->
			<div
				v-if="isListVisible"
				:class="['table-responsive-wrapper', tableWrapperClasses, { 'text-nowrap': !hasTextWrap }]"
				ref="tableWrapperElem">
				<div v-if="rowComponent === 'q-form-container' && formName !== ''">
					<template v-if="permissions.canView !== undefined && permissions.canView !== null ? permissions.canView : true">
						<div
							v-for="(row, index) in vbtRows"
							:key="row.rowKey"
							class="multiform c-multiform__section">
							<component
								:is="rowComponent"
								:id="row.rowKey"
								:form-data="rowFormProps[index]"
								:row-component-props="{
									...rowComponentProps,
									permissions: permissions,
									actionsPlacement: 'actionsPlacement'
								}"
								:resources-path="config.resourcesPath"
								is-multiple
								@edit="
									rowComponentProps.parentFormMode === 'EDIT' &&
										(permissions.canEdit !== undefined && permissions.canEdit !== null ? permissions.canEdit : true)
										? onMultiformSelect(row)
										: null
								"
								@deselect="emitEvent('set-array-sub-prop-where', 'rowFormProps', 'id', row.rowKey, 'mode', 'SHOW')">
							</component>
						</div>
						<div
							v-if="newRowID !== ''"
							class="multiform c-multiform__section">
							<component
								:is="rowComponent"
								:id="newRowID"
								:form-data="{ form: formName, id: newRowID, mode: 'NEW' }"
								:row-component-props="rowComponentPropsInsert"
								:resources-path="config.resourcesPath"
								is-multiple
								@insert-form="(...args) => emitEvent('insert-form', ...args)"
								@cancel-insert="(...args) => emitEvent('cancel-insert', ...args)">
							</component>
						</div>
					</template>
				</div>
				<div
					v-else
					class="table-responsive-container">
					<div
						:class="['table-responsive', tableContainerClasses]"
						ref="tableContainerElem"
						:id="tableContainerId"
						tabindex="0"
						@keydown="tableOnKeyDown"
						@focusout="tableOnFocusout"
						@scroll="updateScrollers">
						<!-- FOR: COLUMN RESIZE, uses ref property -->
						<table
							:class="['c-table', tableClasses]"
							ref="tableElem"
							:aria-label="tableTitle"
							:role="type === 'TreeList' ? 'treegrid' : null">
							<caption class="hidden-elem">
								{{ tableTitle }}
							</caption>
							<q-table-header
								ref="headerRowElem"
								:filters="filters"
								:columns="topLevelColumns"
								:table-name="name"
								:readonly="tableIsReadonly"
								:allow-filters="allowColumnFilters"
								:allow-column-sort="allowColumnSort && !hasRowDragAndDrop"
								:row-count="totalRows"
								:texts="texts"
								:locale="locale"
								:loading="!loaded"
								:disabled="!loaded"
								:rows-selected-count="rowsSelectedCount"
								:all-selected-rows="allSelectedRows"
								:header-cell-ids="getHeaderCellIds(vbtColumns)"
								@update-sort="changeInitialSort"
								@check-all-rows="checkAllRows"
								@check-current-page-rows="checkCurrentPageRows"
								@check-none-rows="checkNoneRows"
								@unselect-all-rows="emitEvent('unselect-all-rows')"
								@update:filters="emitEvent('update:filters', $event)"
								@show-filters="showConfig({ selectedTab: 'filters', columnName: $event })"
								@focusin="rowOnFocusin"
								@focusout="rowOnFocusout">
								<!-- Custom columns -->
								<template
									v-for="col in vbtColumns"
									#[`column_${getCellSlotName(col)}.prepend`]>
									<slot :name="`column_${getCellSlotName(col)}.prepend`" />
								</template>
								<template
									v-for="col in vbtColumns"
									#[`column_${getCellSlotName(col)}`]="slotProps">
									<slot
										:name="`column_${getCellSlotName(col)}`"
										v-bind="slotProps" />
								</template>
								<template
									v-for="col in vbtColumns"
									#[`column_${getCellSlotName(col)}.append`]>
									<slot :name="`column_${getCellSlotName(col)}.append`" />
								</template>
							</q-table-header>
							<tbody
								class="c-table__body"
								ref="tbody"
								data-testid="table-body">
								<!-- BEGIN: data rows -->
								<template v-if="vbtRows.length > 0">
									<component
										:is="rowComponent"
										v-for="(row, index) in vbtRows"
										:key="row.rowKey + '_' + rowDomKey"
										ref="rowElems"
										:id="row.rowKey"
										:table-name="name"
										:row="row"
										:columns="vbtColumns"
										:column-hierarchy="columnHierarchy"
										:row-key-path="[row.rowKey]"
										:row-index="index"
										:row-count="vbtRows.length"
										:navigated-row-key-path="navigatedRowKeyPath"
										:prop-row-classes="getRowClasses(row)"
										:is-valid="rowIsValid(row)"
										:row-title="getRowTitle(row)"
										:crud-actions="crudActions"
										:custom-actions="customActions"
										:general-actions="generalActions"
										:readonly="tableIsReadonly"
										:bg-color-selected="rowBgColorSelected"
										:row-selected-for-group="isRowSelected(row)"
										:cell-titles="getRowCellDataTitles(row, vbtColumns)"
										:header-cell-ids="getHeaderCellIds(vbtColumns)"
										:sort-order-column="sortOrderColumn"
										:expand-icon="expandIcon"
										:collapse-icon="collapseIcon"
										:row-action-display="rowActionDisplay"
										:disable-checkbox="blockTableCheck"
										:row-key-to-scroll="rowKeyToScroll"
										:texts="texts"
										:resources-path="config.resourcesPath"
										:check-box-size="checkBoxSize"
										@row-click="(...args) => executeRowClickAction(...args)"
										@row-action="(emitAction) => emitEvent('row-action', emitAction)"
										@row-reorder="rowReorder"
										@execute-action="(...args) => emitEvent('execute-action', ...args)"
										@cell-action="(...args) => executeActionCell(...args)"
										@remove-row="removeRow(row.rowKey)"
										@toggle-row-selected="toggleRowSelectMultiple(row)"
										@update="(...args) => updateCell(...args)"
										@update-external="(...args) => emitEvent('update-external', ...args)"
										@toggle-show-children="setChildRowsVisibility"
										@go-to-row="(...args) => goToRow(...args)"
										@navigate-row="(...args) => navigateToRowByMultiIndex(...args)"
										@focusin="rowOnFocusin"
										@focusout="rowOnFocusout"
										@loaded="onRowLoaded"
										@sub-rows-loaded="onSubRowsLoaded">
										<!-- Custom columns -->
										<template
											v-for="col in vbtColumns"
											#[getCellSlotName(col)]="slotProps">
											<slot
												:name="getCellSlotName(col)"
												v-bind="slotProps" />
										</template>
									</component>
								</template>
								<!-- BEGIN: No results row -->
								<tr
									v-else-if="loaded"
									class="c-table__row--empty"
									data-testid="table-row">
									<td :colspan="headerColSpan">
										<slot name="empty-results">
											<img
												v-if="emptyRowImgPath"
												:src="emptyRowImgPath"
												:alt="emptyTextToShow" />
											{{ emptyTextToShow }}
										</slot>
									</td>
								</tr>
								<template v-else>
									<tr
										v-for="i in 3"
										:key="i"
										data-testid="table-row">
										<td
											v-for="j in headerColSpan"
											:key="j">
											<q-skeleton-loader type="text" />
										</td>
									</tr>
								</template>
								<!-- END: No results row -->
								<!-- END: data rows -->
							</tbody>
							<tfoot v-if="hasNewRecordRow || hasColumnTotalizers">
								<q-table-row
									v-if="hasNewRecordRow"
									:table-name="name"
									:key="newRow.Rownum"
									:row="newRow"
									:columns="vbtColumns"
									row-index="new"
									unique-id="new_row"
									:bg-color-selected="rowBgColorSelected"
									:cell-titles="getRowCellDataTitles(newRow, vbtColumns)"
									@update="(...args) => setCellValue(...args)">
									<template #checklist>
										<q-button
											:label="texts.removeText"
											:title="texts.removeText"
											@click="emitRowsDelete(rowsSelected)">
											<q-icon icon="remove" />
										</q-button>
									</template>
									<template #actions>
										<q-button
											:label="texts.insertText"
											:title="texts.insertText"
											@click="emitRowAdd(newRow)">
											<q-icon icon="add" />
										</q-button>
									</template>
								</q-table-row>

								<!-- BEGIN: Column totalizers -->
								<q-table-column-totalizers
									v-if="hasColumnTotalizers"
									:class="totalizersClasses"
									:columns="vbtColumns"
									:totalizers="columnTotalizers"
									:multiple-selection="showRowsSelectedTotalizer"
									:rows="vbtRows"
									:selected-rows="rowsSelected"
									:current-page="page"
									:all-selected="allSelected" />
								<!-- END: Column totalizers -->
							</tfoot>
						</table>
					</div>
					<q-button
						v-if="hasHorizontalScrollers"
						v-show="scrollHorizLeftVisible"
						variant="bold"
						class="q-table_scroll--left"
						tabindex="-1"
						@mousedown="scrollButtonLeftOnMousedown()"
						@mouseup="setScrollButtonLeftPressed(false)"
						@touchstart="scrollButtonLeftOnMousedown()"
						@touchend="setScrollButtonLeftPressed(false)"
						@touchcancel="setScrollButtonLeftPressed(false)"
						@touchmove="setScrollButtonLeftPressed(false)">
						<q-icon icon="page-previous" />
					</q-button>
					<q-button
						v-if="hasHorizontalScrollers"
						v-show="scrollHorizRightVisible"
						variant="bold"
						class="q-table_scroll--right"
						tabindex="-1"
						@mousedown="scrollButtonRightOnMousedown()"
						@mouseup="setScrollButtonRightPressed(false)"
						@touchstart="scrollButtonRightOnMousedown()"
						@touchend="setScrollButtonRightPressed(false)"
						@touchcancel="setScrollButtonRightPressed(false)"
						@touchmove="setScrollButtonRightPressed(false)">
						<q-icon icon="page-next" />
					</q-button>
				</div>
				<!-- BEGIN: Footer -->
				<!-- Adding class hiddenFooter since using the v-if to hide the footer is causing an issue (scrolling down causes the page to go up and down) -->
				<div
					v-show="showFooter && !isFooterEmpty"
					class="c-table__footer-out"
					ref="tableFooterElem">
					<q-table-footer
						:pagination-placement="paginationPlacement"
						:pagination="showPagination && !hasRowDragAndDrop"
						:show-per-page-menu="perPageMenuVisible"
						:current-page-rows-length="currentPageRowsLength"
						:page="page"
						:per-page="perPage"
						:per-page-options="sortedPerPageOptions"
						:has-more="hasMorePages"
						:show-rows-selected-count="showRowsSelectedCount"
						:all-rows-selected="allSelected"
						:rows-selected-count="rowsSelectedCount"
						:has-row-select-actions="hasRowSelectActions"
						:group-actions="groupActions"
						:show-record-count="showRecordCount"
						:row-count="totalRows"
						:show-alternate-pagination="showAlternatePagination"
						:num-visible-pagination-buttons="numVisiblePaginationButtons"
						:show-limits="showLimits"
						:table-limits="tableLimits"
						:table-id="controlId"
						:table-name-plural="tableNamePlural"
						:texts="texts"
						:disabled="!loaded"
						@update:page="goToPage"
						@update:per-page="setPerPage"
						@group-action="rowGroupAction">
						<q-action-list
							variant="outlined"
							:groups="generalActionGroups"
							:items="generalActionItems"
							:readonly="tableIsReadonly"
							@click="executeGeneralAction" />
					</q-table-footer>
				</div>
				<!-- END: Footer -->
			</div>
			<template v-else-if="activeViewMode">
				<component
					v-if="activeViewMode.props"
					:is="`q-${activeViewMode.type}`"
					:texts="texts"
					v-bind="activeViewMode.props"
					v-on="activeViewMode.handlers ?? {}">
					<template #empty-image>
						<img
							v-if="emptyRowImgPath"
							:src="emptyRowImgPath"
							:alt="texts.emptyText" />
					</template>
					<template #empty-text>
						{{ emptyTextToShow }}
					</template>
				</component>
				<!-- BEGIN: Footer -->
				<!-- Adding class hiddenFooter since using the v-if to hide the footer is causing an issue (scrolling down causes the page to go up and down) -->
				<div
					v-show="showFooter && !isFooterEmpty"
					:class="['c-sr__footer-out', 'c-table__footer-out']">
					<q-table-footer
						:pagination="showPagination"
						:show-per-page-menu="false"
						:current-page-rows-length="currentPageRowsLength"
						:page="page"
						:per-page="perPage"
						:per-page-options="sortedPerPageOptions"
						:has-more="hasMorePages"
						:show-rows-selected-count="showRowsSelectedCount"
						:rows-selected-count="rowsSelectedCount"
						:has-row-select-actions="hasRowSelectActions"
						:group-actions="groupActions"
						:show-record-count="showRecordCount"
						:row-count="totalRows"
						:show-alternate-pagination="showAlternatePagination"
						:num-visible-pagination-buttons="numVisiblePaginationButtons"
						:show-limits="showLimits"
						:table-limits="tableLimits"
						:table-id="controlId"
						:table-name-plural="tableNamePlural"
						:texts="texts"
						:disabled="!loaded"
						@update:page="goToPage"
						@update:per-page="setPerPage"
						@group-action="rowGroupAction">
						<q-action-list
							v-if="!activeViewMode.implementsOwnInsert"
							variant="outlined"
							:groups="generalActionGroups"
							:items="generalActionItems"
							:readonly="tableIsReadonly"
							@click="executeGeneralAction" />
					</q-table-footer>
				</div>
				<!-- END: Footer -->
			</template>
		</component>
	</div>
</template>

<script>
	import { defineAsyncComponent, markRaw, nextTick } from 'vue'
	import cloneDeep from 'lodash-es/cloneDeep'
	import findIndex from 'lodash-es/findIndex'
	import has from 'lodash-es/has'
	import isEmpty from 'lodash-es/isEmpty'
	import mergeWith from 'lodash-es/mergeWith'

	import Sortable from 'sortablejs'

	import { btnHasPermission, getHeadingTagNameByLevel, validateTexts } from '@quidgest/clientapp/utils/genericFunctions'
	import listFunctions from '@/mixins/listFunctions.js'
	import HelpControl from '@/mixins/helpControls.js'

	import ColumnResizeable from '@/api/genio/columnResizeable.js'
	import searchFilterData from '@/api/genio/searchFilterData.js'

	import { QActionList } from '@quidgest/clientapp/components'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		actionMenuTitle: 'Actions',
		emptyText: 'No data to show',
		importButtonTitle: 'Import',
		templateButtonTitle: 'Select a template to use.',
		submitText: 'Submit',
		applyText: 'Apply',
		preview: 'Preview',
		editText: 'Edit',
		closeText: 'Close',
		dropToUpload: 'Drag the file to upload',
		okText: 'OK',
		saveText: 'Save',
		viewText: 'View',
		duplicateText: 'Duplicate',
		cancelText: 'Cancel',
		discard: 'Discard',
		resetText: 'Reset',
		insertText: 'Insert',
		tableConfig: 'View configuration',
		baseTable: 'Base table',
		columns: 'Columns',
		configureColumns: 'Configure columns',
		configureFilters: 'Configure filters',
		manageViews: 'Manage views',
		createView: 'Create view',
		toListViewButtonTitle: 'List view',
		toAlternativeViewButtonTitle: 'Alternative view',
		orderText: 'Order',
		nameOfColumnText: 'Column name',
		visibleText: 'Visible',
		searchText: 'Search',
		forText: 'for',
		ofText: 'of',
		orText: 'Or',
		andText: 'And',
		allFieldsText: 'all fields',
		showText: 'Show',
		hideText: 'Hide',
		filtersText: 'Filters',
		filterStatus: 'Filter status',
		limitsButtonTitle: 'Limit',
		limitsListTitlePrepend: 'The information in the list of',
		limitsListTitleAppend: 'is limited by',
		allRowsSelected: 'All records selected',
		textRowsSelected: 'selected record(s)',
		groupActionsText: 'Group actions',
		createFilterText: 'Create filter',
		filterNameText: 'Filter name',
		selectedView: 'Selected view',
		createConditionText: 'Add condition',
		removeConditionText: 'Remove condition',
		removeText: 'Remove',
		removeAll: 'Remove all',
		sortAscendingText: 'Sort ascending',
		sortDescendingText: 'Sort descending',
		removeSortText: 'Remove sorting',
		clear: 'Clear',
		rowDragAndDropTitle: 'Reorder',
		exportButtonTitle: 'Export',
		defaultKeywordSearchText: 'Default search',
		lineBreak: 'Line break',
		yesLabel: 'Yes',
		noLabel: 'No',
		activeText: 'Active',
		inactiveText: 'Inactive',
		visibleColumnsText: 'Visible columns',
		invisibleColumnsHelpText: 'Invisible columns are not searchable.',
		wantToSaveChangesToView: 'Save changes to the current table view?',
		repeatedViewName: 'The name of this view is already being used in another.',
		emptyViewName: 'The view name must be filled in.',
		viewNameText: 'View name',
		defaultViewText: 'Default view',
		downloadTemplateText: 'Download the excel template file by clicking the button below',
		fillTemplateFileText: 'Fill the file with the necessary information',
		importTemplateFileText: 'After filling the file click on the submit button to import it',
		allRecordsText: 'All',
		currentPageText: 'Current page',
		noneText: 'None',
		onDate: 'On:',
		first: 'First',
		last: 'Last',
		previous: 'Previous',
		next: 'Next',
		moveUp: 'Move up',
		moveDown: 'Move down',
		insertBelow: 'Insert below',
		rowDragDropReorder: 'Drag and drop to change row order',
		rowExpand: 'Expand row',
		rowCollapse: 'Collapse row',
		close: 'Close',
		download: 'Download',
		placeholder: 'Choose...',
		clearValue: 'Clear value',
		showOptions: 'Show options',
		hideColumnConfirm: 'You have hidden columns with filters, all associated filters will be removed. Do you want to continue?',
		selectOptions: 'Select options',
		emptyTextShowAfterFilter: 'Please apply one or more filters to display the data',
		emptyTextNoMatch: 'No matching records found',
		messages: 'Messages',
		delete: 'Delete',
		remove: 'Remove',
		restore: 'Restore',
		selected: 'Selected',
		selectAll: 'Select all',
		deselectAll: 'Deselect all'
	}

	export default {
		name: 'QTable',

		emits: [
			'cancel-insert',
			'cell-action',
			'clear-filters',
			'execute-action',
			'go-to-row',
			'hide-popup',
			'init-all-selected',
			'insert-form',
			'loaded',
			'on-export-data',
			'on-export-template',
			'on-import-data',
			'refresh',
			'remove-row',
			'row-action',
			'row-add',
			'row-edit',
			'row-group-action',
			'row-reorder',
			'rows-delete',
			'rows-loaded',
			'select-row',
			'select-rows',
			'set-array-sub-prop-where',
			'set-confirm-changes',
			'set-property',
			'set-row-index-property',
			'set-selected-rows',
			'show-config',
			'show-popup',
			'toggle-rows-drag-drop',
			'tree-load-branch-data',
			'unselect-all-rows',
			'unselect-row',
			'update-cell',
			'update-external',
			'update:active-view-mode',
			'update:activeFilters',
			'update:filters',
			'update:groupFilters',
			'update:sorting'
		],

		components: {
			QTableStaticFilters: defineAsyncComponent(() => import('./QTableStaticFilters.vue')),
			QTableCurrentFilters: defineAsyncComponent(() => import('./QTableCurrentFilters.vue')),
			QTableHeader: defineAsyncComponent(() => import('./QTableHeader.vue')),
			QTableFooter: defineAsyncComponent(() => import('./QTableFooter.vue')),
			QTableSearch: defineAsyncComponent(() => import('./QTableSearch.vue')),
			QTableChecklistCheckbox: defineAsyncComponent(() => import('./QTableChecklistCheckbox.vue')),
			QTableExport: defineAsyncComponent(() => import('./QTableExport.vue')),
			QTableImport: defineAsyncComponent(() => import('./QTableImport.vue')),
			QTableRow: defineAsyncComponent(() => import('./QTableRow.vue')),
			QTreeTableRow: defineAsyncComponent(() => import('./QTreeTableRow.vue')),
			QPopoverHelp: defineAsyncComponent(() => import('@/components/QPopoverHelp.vue')),
			QTooltipHelp: defineAsyncComponent(() => import('@/components/QTooltipHelp.vue')),
			QSubtitleHelp: defineAsyncComponent(() => import('@/components/QSubtitleHelp.vue')),
			QInfoBannerHelp: defineAsyncComponent(() => import('@/components/QInfoBannerHelp.vue')),
			QActionList
		},

		inheritAttrs: false,

		mixins: [HelpControl],

		props: {
			/**
			 * The unique identifier for the table component instance.
			 */
			id: String,

			/**
			 * Flag indicating if the label is to be displayed.
			 */
			hasLabel: {
				type: Boolean,
				default: true
			},

			/**
			 * The array of data objects that represents each row of data in the table.
			 */
			rows: {
				type: Array,
				default: () => []
			},

			/**
			 * The configuration for the table columns, including header names, keys, and additional properties.
			 */
			columns: {
				type: Array,
				required: true
			},

			/**
			 * The list of column totalizers, identified by the columns's area and field
			 */
			columnTotalizers: {
				type: Array,
				default: () => []
			},

			/**
			 * The component used to render individual table rows.
			 */
			rowComponent: {
				type: String,
				default: 'q-table-row'
			},

			/**
			 * The type of the table, which can affect rendering or features (e.g., 'List', 'TreeList').
			 */
			type: {
				type: String,
				default: 'List'
			},

			/**
			 * The total number of rows available, which may be utilized for server-side pagination.
			 */
			totalRows: {
				type: Number,
				default: 0
			},

			/**
			 * Flag indicating whether there are more pages of data available for server-side pagination.
			 */
			hasMorePages: {
				type: Boolean,
				default: false
			},

			/**
			 * Custom configuration object to setup various aspects of the table component's behavior and appearance.
			 */
			config: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Localization and customization of textual content within the table component.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Indicates whether the table is in a read-only or interactive state.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Custom classes added to the table's surrounding container element.
			 */
			classes: {
				type: [Array, String],
				default: () => []
			},

			/**
			 * Custom classes added to the table's surrounding container element, only when in normal table mode.
			 */
			tableModeClasses: {
				type: [Array, String],
				default: () => []
			},

			/**
			 * Custom classes added directly to the <table> element.
			 */
			tableClasses: {
				type: [Object, String],
				default: () => ({})
			},

			/**
			 * Custom classes added to the container that usually includes the table and possibly other elements like pagination.
			 */
			tableContainerClasses: {
				type: [Object, String],
				default: () => ({})
			},

			/**
			 * Custom classes added to the wrapper element surrounding the table.
			 */
			tableWrapperClasses: {
				type: [Object, String],
				default: () => ({})
			},

			/**
			 * Size of the table.
			 */
			size: {
				type: String,
				default: ''
			},

			/**
			 * An array of actions that can be performed on the entire table, such as adding or exporting rows.
			 */
			actions: {
				type: Array,
				default: () => []
			},

			/**
			 * Definition of global filters that apply generalized filtering criteria outside individual columns.
			 */
			groupFilters: {
				type: Array,
				default: () => []
			},

			/**
			 * Object representing filters that are currently active and affecting the table data.
			 */
			activeFilters: Object,

			/**
			 * The list of filters applied to the table.
			 */
			filters: {
				type: Array,
				default: () => []
			},

			/**
			 * Whether there are any static filters still not applied to the table.
			 */
			unappliedFilters: {
				type: Boolean,
				default: false
			},

			/**
			 * An array representing the different limits that may apply to the table data or query results.
			 */
			tableLimits: {
				type: Array,
				default: () => []
			},

			/**
			 * The response payload from an import action, which could include status messages or data validation errors.
			 */
			dataImportResponse: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The header level (e.g., h1, h2) used for the table's accessible title region.
			 */
			headerLevel: {
				type: Number,
				default: 1
			},

			/**
			 * A map of selected rows by their keys, often used for batch actions or manipulation of multiple rows.
			 */
			rowsSelected: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The name of the icon to be used for expand functionality within collapsible rows or sections.
			 */
			expandIcon: {
				type: String,
				default: 'square-plus'
			},

			/**
			 * The name of the icon to be used for collapse functionality within collapsible rows or sections.
			 */
			collapseIcon: {
				type: String,
				default: 'square-minus'
			},

			/**
			 * Array of different view modes available for the table, such as a list view or card view.
			 */
			viewModes: {
				type: Array,
				default: () => []
			},

			/**
			 * The identifier for the table's current view mode out of the possible options defined in viewModes.
			 */
			activeViewModeId: {
				type: String,
				default: ''
			},

			/**
			 * The name of the form to be potentially used or referenced within row-level actions or edit states.
			 */
			formName: {
				type: String,
				default: ''
			},

			/**
			 * Object properties to be associated with each individual row component, such as form states or additional configuration.
			 */
			rowFormProps: {
				type: Array,
				default: () => []
			},

			/**
			 * Additional properties to be passed down to row components, which can include permissions or event handlers.
			 */
			rowComponentProps: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The identifier for a row that may be added to the table data as a new entry.
			 */
			newRowID: {
				type: String,
				default: ''
			},

			/**
			 * Indicates if any pending changes need to be confirmed by the user before proceeding.
			 */
			confirmChanges: {
				type: Boolean,
				default: false
			},

			/**
			 * A predefined set of operators used for creating or managing filters within the component.
			 */
			filterOperators: {
				type: Object,
				default: () => new searchFilterData.SearchFilterConditionOperators()
			},

			/**
			 * A flag to indicate the 'select all' state is active for the table.
			 */
			allSelectedRows: {
				type: String,
				default: 'false'
			},

			/**
			 * Object with properties for the header row.
			 */
			headerRow: {
				type: Object,
				default: () => ({
					isNavigated: false
				})
			},

			/**
			 * Indicates if the component has finished loading necessary data and can be interacted with or not.
			 */
			loaded: {
				type: Boolean,
				default: true
			},

			/**
			 * Current system locale
			 */
			locale: {
				type: String,
				default: 'en-US'
			}
		},

		expose: [],

		data()
		{
			return {
				unmountedComponent: false,
				controlId: this.id || this.config.name || `q-table-${this._.uid}`,
				vbtRows: [],
				vbtColumns: [],
				columnHierarchy: [],
				name: '',
				page: 1,
				perPage: 10,
				perPageDefault: 10,
				showRecordCount: false,
				showRowsSelectedTotalizer: false,
				numVisiblePaginationButtons: undefined,
				pagination: true,
				tableTitle: '',
				tableNamePlural: '',
				searchBarConfig: {
					classes: '',
					visibility: false,
					showRefreshButton: true
				},
				filtersVisible: true,
				allowColumnSort: false,
				allowColumnFilters: false,
				allowColumnResize: true,
				perPageOptions: [],
				isFirstTime: true,
				canInsert: true,
				crudActions: [],
				customActions: [],
				generalActions: [],
				generalCustomActions: [],
				rowClickAction: {},
				rowActionDisplay: 'dropdown',
				showRowActionIcon: true,
				showGeneralActionIcon: true,
				showRowActionText: true,
				showGeneralActionText: true,
				actionsPlacement: 'left',
				rowClickActionInternal: '',
				extendedActions: [],
				paginationPlacement: 'left',
				system: 0,
				pkColumn: '',
				menuForJump: '',
				groupActions: [],
				showRowsSelectedCount: false,
				showAlternatePagination: false,
				dateFormats: {
					date: 'dd/MM/yyyy',
					dateTime: 'dd/MM/yyyy HH:mm',
					dateTimeSeconds: 'dd/MM/yyyy HH:mm:ss',
					hours: 'HH:mm'
				},
				rowBgColorSelected: '#E0E0E0',
				allowFileExport: false,
				exportOptions: [],
				allowFileImport: false,
				importOptions: [],
				importTemplateOptions: [],
				showLimitsInfo: false,
				showFooter: true,
				resizableGrid: null,
				hasTextWrap: false,
				hasRowDragAndDrop: false,
				showRowDragAndDropOption: false,
				fieldsEditable: false,
				hasNewRecordRow: false,
				newRow: {
					Rownum: -1,
					Fields: {}
				},
				rowKeyToScroll: '',
				focusElement: '',
				defaultSearchColumnName: '',
				columnResizeOptions: {},
				tableContainerElem: {},
				hasHorizontalScrollers: false,
				importModalProps: {
					props: {
						size: 'medium',
						dismissible: true
					},
					modalProps: {
						id: 'data-import'
					}
				},
				permissions: {},

				// SortableJS plugin instance
				sortablePlugin: null,

				blockTableCheck: false,
				allSelected: this.allSelectedRows === 'true',
				/**
				 * Row key path of navigated row
				 */
				navigatedRowKeyPath: null,
				/**
				 * Index of navigated row in the array of all navigable row elements
				 */
				navRowIndex: null,
				/**
				 * Whether to navigate to the element that should be navigated to when the component is updated
				 */
				setNavOnUpdate: false,
				/**
				 * Array of all navigable row elements
				 */
				navRowElems: [],
				/**
				 * Index of the first data row in the array of all navigable row elements.
				 * Can be 0 or 1 depending on whether the header row has navigable elements.
				 */
				firstDataRowIndex: 0,
				/**
				 * Number of rows that need to be rendered when the row data changes
				 */
				rowsToLoad: 0,
				/**
				 * Used to change the key for each row component to cause them to re-render when changing row data.
				 * All rows are re-rendered so that when the number of rendered rows matches the number of row objects
				 * loaded, an emit can be done to signal that the rendering is totally done.
				 * Used by things that depend on certain HTML elements existing.
				 */
				rowDomKey: 0,
				/**
				 * Whether the rows should be re-rendered when the row data changes.
				 */
				rerenderRowsOnNextChange: true,
				// Whether the horizontal scroll left indicator should be visible
				scrollHorizLeftVisible: false,
				// Whether the horizontal scroll right indicator should be visible
				scrollHorizRightVisible: false,
				// Pressed state for scroll button
				scrollHorizLeftPressed: false,
				// Pressed state for scroll button
				scrollHorizRightPressed: false,
				// Resize observer to react to changes in the table size
				resizeObserver: null,
				// Horizontal scroll interval ID
				scrollHorizIntervalId: null,
				checkBoxSize: null,
				radioButtonSize: null,
				generalActionGroups: [
					{
						id: 'default',
						display: 'inline',
						displayLabels: true
					}
				]
			}
		},

		provide()
		{
			return {
				getValueFromRow: this.getValueFromRow,
				getCellSlotName: this.getCellSlotName,
				canShowColumn: this.canShowColumn,
				isActionsColumn: this.isActionsColumn,
				isExtendedActionsColumn: this.isExtendedActionsColumn,
				isChecklistColumn: this.isChecklistColumn,
				isDragAndDropColumn: this.isDragAndDropColumn,
				isTotalizerColumn: this.isTotalizerColumn,
				isDataColumn: this.isDataColumn,
				getRowClasses: this.getRowClasses,
				getRowTitle: this.getRowTitle,
				rowIsValid: this.rowIsValid,
				hasDataAction: this.hasDataAction,
				hasExtendedAction: this.hasExtendedAction,
				getCellDataDisplay: this.getCellDataDisplay,
				getRowCellDataTitles: this.getRowCellDataTitles,
				isRowSelected: this.isRowSelected
			}
		},

		created()
		{
			this.initConfig()
		},

		mounted()
		{
			this.vbtRows = this.rows

			// FOR: GETTING NAVIGABLE ROW ELEMENTS
			// Track number of rows to render
			this.setRowsToLoad()

			if (this.showRowDragAndDropOption) this.perPage = -1

			// Add columns to "new record" row
			if (this.hasNewRecordRow) this.initRow(this.newRow)

			//FOR: ROW ACTIONS, EXTENDED ACTIONS, COLUMN ORDER AND VISIBILITY
			//Must be called when loading or changing columns
			this.updateColumns()

			this.handleShiftKey()

			nextTick().then(() => {
				if (this.$refs.tableElem && this.$refs.tableContainerElem)
				{
					//FOR: COLUMN RESIZE
					this.applyColumnResizeable()
				}
			})

			if (!this.readonly && this.rowsSelectableMultiple)
			{
				//Check if everything is supposed to be selected
				this.initHeaderSelector()
			}

			this.initRowsDragAndDrop()

			nextTick().then(() => this.emitEvent('loaded'))
		},

		updated()
		{
			//DOM reference must be copied to property to it can be updated when the DOM element finally exists
			//and so the property can be passed as a prop to other componenents
			//so they will react to the change (when the DOM element finally exists)
			this.tableContainerElem = Array.isArray(this.$refs.tableContainerElem) ? this.$refs.tableContainerElem[0] : this.$refs.tableContainerElem

			// Update table navigation properties
			if (this.setNavOnUpdate)
			{
				this.emitEvent('set-property', ['config', 'setNavOnUpdate'], false)

				nextTick().then(() => {
					this.navigateToTableRowAction('first')
				})
			}
		},

		beforeUnmount()
		{
			this.unmountedComponent = true

			window.removeEventListener('keyup', this.handleShiftKeyOnSelectStart)
			window.removeEventListener('keydown', this.handleShiftKeyOnSelectStart)
			document.removeEventListener('selectstart', this.documentOnSelectStart)

			this.destroyRowsDragAndDrop()

			if (this.resizableGrid)
			{
				this.resizableGrid.destroy()
				this.resizableGrid = null
			}

			this.tableContainerElem = null

			this.navRowElems.length = 0

			if (this.vbtColumns?.length > 0) {
				this.vbtColumns.forEach(column => {
					if (typeof column.destroy === 'function')
						column.destroy()
				})
			}
			this.vbtColumns.splice(0)

			this.columnHierarchy.length = 0
			this.vbtRows = null

			this.resizeObserver?.disconnect()
			this.resizeObserver = null
		},

		computed: {
			/**
			 * Determine if the table is in a read-only state.
			 */
			tableIsReadonly()
			{
				return this.readonly || !this.loaded
			},

			/**
			 * Determine if the table header should be displayed.
			 */
			showHeader()
			{
				return this.tableTitle.length > 0 || this.hasActionBar || this.showSearchBar || this.$slots.tableTitle
			},

			/**
			 * Determine if there are columns with totalizers.
			 */
			hasColumnTotalizers()
			{
				return this.columns.some(column => column.totalizer && listFunctions.isVisibleColumn(column)) && this.vbtRows.length > 0
			},

			/**
			 * Get CSS classes for the row of totalizers.
			 * @returns The class list.
			 */
			totalizersClasses()
			{
				const classes = ['q-table-list__column-totalizers']

				if (this.hasNewRecordRow)
					classes.push('q-table-list__column-totalizers--new-record')

				return classes
			},

			/**
			 * Get CSS classes for the table size.
			 * @returns The class list.
			 */
			sizeClass()
			{
				const sizes = ['large', 'xlarge', 'xxlarge']

				return sizes.includes(this.size) ? `q-table-list--${this.size}` : ''
			},

			/**
			 * Get top-level columns based on the hierarchy or default to the full column list.
			 */
			topLevelColumns()
			{
				const columns = this.columnHierarchy[0] ?? this.vbtColumns
				return columns.filter((c) => this.canShowColumn(c))
			},

			/**
			 * Calculate total number of pages based on the current per-page setting and the number of filtered results.
			 */
			totalPages()
			{
				return Math.ceil(this.totalRows / this.perPage)
			},

			/**
			 * Determine if there is a unique identifier column among the table columns.
			 */
			uniqueId()
			{
				let uniqueId = ''

				if (!this.hasUniqueId)
				{
					uniqueId = 'vbtId'
					return uniqueId
				}

				this.vbtColumns.some((column) => {
					if (has(column, 'uniqueId') && column.uniqueId === true)
					{
						uniqueId = column.name
						return true
					}
				})

				return uniqueId
			},

			/**
			 * Determine if the table columns include a unique identifier column.
			 */
			hasUniqueId()
			{
				return this.vbtColumns.some((column) => has(column, 'uniqueId') && column.uniqueId === true)
			},

			/**
			 * Get the unique identifier for the table container element.
			 */
			tableContainerId()
			{
				return this.controlId + '-table-container'
			},

			/**
			 * Define the HTML level tag to be used for the table header.
			 */
			headerTag()
			{
				return getHeadingTagNameByLevel(this.headerLevel)
			},

			/**
			 * Get the length of the current page's row collection.
			 */
			currentPageRowsLength()
			{
				return this.vbtRows.length
			},

			/**
			 * Determine the total colspan value for the header based on visible columns.
			 */
			headerColSpan()
			{
				return this.topLevelColumns.length
			},

			/**
			 * Determine if the search bar is visible.
			 */
			showSearchBar()
			{
				return this.searchBarConfig.visibility
			},

			/**
			 * Determine if the table has multiple pages.
			 */
			hasMultiplePages()
			{
				/*
				 * Normal paging just needs the total number of pages.
				 * Alternate paging needs the page number and hasMore to calculate if there are multiple pages.
				 */
				return this.totalPages > 1 || this.page > 1 || this.hasMorePages
			},

			/**
			 * Determine if pagination should be displayed.
			 */
			showPagination()
			{
				return this.pagination && this.hasMultiplePages
			},

			/**
			 * Determine if rows can be selected (multiple).
			 */
			rowsSelectableMultiple()
			{
				return this.rowClickActionInternal === 'selectMultiple'
			},

			/**
			 * Number of rows selected
			 */
			rowsSelectedCount()
			{
				return Object.keys(this.rowsSelected).length
			},

			/**
			 * Gets the list of columns that are searchable.
			 */
			searchableColumns()
			{
				return listFunctions.getSearchableColumns(this.columns)
			},

			/**
			 * Gets the list of columns that are sortable.
			 */
			sortableColumns()
			{
				return listFunctions.getSortableColumns(this.columns)
			},

			/**
			 * Determine if any filters are available on the table.
			 */
			hasFilters()
			{
				return this.hasStaticFilters || this.hasCustomFilters || !!this.$slots.filters
			},

			/**
			 * Determine if there are any search filters available.
			 */
			hasCustomFilters()
			{
				return this.filtersActive.length > 0
			},

			/**
			 * Determine if any of the static filters are defined.
			 */
			hasStaticFilters()
			{
				return this.groupFilters.length > 0 || Object.keys(this.activeFilters ?? {}).length > 0
			},

			/**
			 * Determine if any of the static filters are actively selected.
			 */
			hasStaticFiltersActive()
			{
				return this.groupFilters.some((f) => !isEmpty(f.selected)) ||
					!isEmpty(this.activeFilters?.selected)
			},

			/**
			 * Provides a list of all currently active filters.
			 */
			filtersActive()
			{
				return this.filters.filter((f) => f.active)
			},

			/**
			 * The empty text that should be displayed.
			 */
			emptyTextToShow()
			{
				const hasAnyFilterApplied =
					(this.filtersActive?.length ?? 0) > 0 ||
					this.hasStaticFiltersActive === true

				const isEmpty = (this.vbtRows?.length ?? 0) === 0

				// Show message when table is configured to only show after filtering
				if (this.config.showAfterFilter && !hasAnyFilterApplied)
					return this.texts.emptyTextShowAfterFilter

				// Show message when filters are applied but no results are found
				if (hasAnyFilterApplied && isEmpty)
					return this.texts.emptyTextNoMatch

				// Default empty text
				return this.texts.emptyText
			},

			/**
			 * Determine if the table has a row click action.
			 */
			hasRowClickAction()
			{
				return !isEmpty(this.rowClickAction)
			},

			/**
			 * Determine if the table has row actions.
			 */
			hasRowActions()
			{
				return this.crudActions.length > 0 || this.customActions.length > 0 || this.hasNewRecordRow
			},

			/**
			 * Determine if the table has extended row actions.
			 */
			hasExtendedRowActions()
			{
				return typeof this.extendedActions !== 'undefined' && Array.isArray(this.extendedActions) && this.extendedActions.length > 0
			},

			/**
			 * Determine if the table has row select actions.
			 */
			hasRowSelectActions()
			{
				return this.rowsSelectableMultiple && (this.groupActions.length > 0 || this.menuForJump !== '')
			},

			/**
			 * Determine if the table limits information should be visible.
			 */
			hasLimits()
			{
				if (!this.tableLimits) return false
				return Object.keys(this.tableLimits).length > 0
			},

			/**
			 * Determine if the table limits information should be visible.
			 */
			showLimits()
			{
				if (!this.hasLimits) return false
				return this.showLimitsInfo
			},

			/**
			 * The currently configured view mode component or undefined if none.
			 */
			activeViewMode()
			{
				return this.viewModes.find((viewMode) => viewMode.id === this.activeViewModeId)
			},

			/**
			 * Determine if the user is allowed to customize the view modes.
			 */
			hasViewModeToggle()
			{
				return this.viewModes.length > 1
			},

			/**
			 * Determine if the table has action bar.
			 */
			hasActionBar()
			{
				return (
					this.showConfigMenu ||
					this.config.allowColumnFilters ||
					this.showRowDragAndDropOption ||
					this.allowFileExport ||
					this.allowFileImport ||
					this.hasViewModeToggle
				)
			},

			/**
			 * Determine the column used for ordering the rows.
			 */
			sortOrderColumn()
			{
				return this.columns.find((c) => c.sortOrder > 0)
			},

			/**
			 * Get the default search column based on configuration.
			 */
			defaultSearchColumn()
			{
				return listFunctions.getDefaultSearchColumn(this.columns, this.defaultSearchColumnName)
			},

			/**
			 * Get the label of the default search column.
			 */
			defaultSearchColumnLabel()
			{
				if (isEmpty(this.defaultSearchColumn?.label)) return ''
				return this.defaultSearchColumn.label
			},

			/**
			 * Determine if the list view is enabled.
			 */
			isListVisible()
			{
				// FIXME: remove by implementing a view mode manager
				// QTable SHOULD ONLY IMPLEMENT TABLE LOGIC!!! => DOES NOT CARE ABOUT VIEWMODES
				return !this.viewModes.length || this.activeViewModeId === 'LIST'
			},

			/**
			 * Get the row component properties with overrides for inserting rows.
			 */
			rowComponentPropsInsert()
			{
				const obj = cloneDeep(this.rowComponentProps)

				obj.formButtonsOverride.saveBtn.emitAction = {
					name: 'insert-form',
					params: []
				}
				obj.formButtonsOverride.resetCancelBtn.emitAction = {
					name: 'cancel-insert',
					params: []
				}

				return obj
			},

			/**
			 * Get the currently selected row.
			 */
			rowSelected()
			{
				const rowIdStr = Object.keys(this.rowsSelected)[0]
				if (rowIdStr === undefined || rowIdStr === null) return null

				const rowId = rowIdStr.split(',')

				const rowSelected = listFunctions.getRowByKeyPath(this.rows, rowId)

				if (rowSelected !== undefined && rowSelected !== null) rowSelected.rowKeyPath = rowId

				return rowSelected
			},

			/**
			 * Determine whether the table footer is empty or not.
			 */
			isFooterEmpty()
			{
				return (
					!this.showRowsSelectedCount &&
					!this.hasRowSelectActions &&
					!this.showRecordCount &&
					!this.showPagination &&
					!this.showLimits &&
					listFunctions.numArrayVisibleActions(this.generalActions, this.tableIsReadonly) === 0 &&
					listFunctions.numArrayVisibleActions(this.generalCustomActions, this.tableIsReadonly) === 0
				)
			},

			/**
			 * Determine whether to show table view configuration dropdown menu.
			 */
			showConfigMenu()
			{
				return (
					this.config.allowManageViews || (this.config.allowColumnConfiguration && this.isListVisible) || this.hasCustomFilters
				)
			},

			/**
			 * Get sorted options for the `perPage` dropdown.
			 */
			sortedPerPageOptions()
			{
				return this.perPageOptions.concat([this.perPageDefault]).toSorted((a, b) => a - b)
			},

			/**
			 * Determine if the per page menu should be visible.
			 */
			perPageMenuVisible()
			{
				return listFunctions.getPerPageMenuVisible(
					this.perPageOptions,
					this.perPageDefault,
					this.totalRows,
					this.page,
					this.showAlternatePagination,
					this.hasMorePages
				)
			},

			/**
			 * Get the path to the image to be used for an empty row.
			 */
			emptyRowImgPath()
			{
				if (!this.config.resourcesPath || !this.config.emptyRowImg) return ''

				let resourcesPath = this.config.resourcesPath
				if (resourcesPath[resourcesPath.length - 1] !== '/') resourcesPath += '/'

				return resourcesPath + this.config.emptyRowImg
			},

			/**
			 * The list of general actions.
			 */
			generalActionItems()
			{
				const permissions = { insertBtnDisabled: !this.canInsert }

				return [
					...this.generalCustomActions.map((act) => ({
						...act,
						key: act.id,
						label: act.title,
						group: 'default'
					})),
					...this.generalActions.map((act) => ({
						...act,
						key: act.id,
						label: act.title,
						group: 'default',
						isVisible: !this.tableIsReadonly,
						disabled: !btnHasPermission(permissions, act.id)
					}))
				]
			}
		},

		methods: {
			getValueFromRow: listFunctions.getCellValue,
			getCellSlotName: listFunctions.getCellSlotName,
			isActionsColumn: listFunctions.isActionsColumn,
			isExtendedActionsColumn: listFunctions.isExtendedActionsColumn,
			isChecklistColumn: listFunctions.isChecklistColumn,
			isDragAndDropColumn: listFunctions.isDragAndDropColumn,
			isTotalizerColumn: listFunctions.isTotalizerColumn,
			isDataColumn: listFunctions.isDataColumn,
			hasExtendedAction: listFunctions.hasExtendedAction,
			hasDataAction: listFunctions.hasDataAction,
			rowWithoutChildren: listFunctions.rowWithoutChildren,

			emitEvent(event, ...rawArgs)
			{
				if (!this.unmountedComponent)
					this.$emit(event, ...rawArgs)
				// To facilitate debugging during development
				else if (import.meta.env.DEV)
					// eslint-disable-next-line no-console
					console.warn(`The QTable component is emitting the event already after unmount: ${event}`)
			},

			/**
			 * Emits the event to open the configuration popup.
			 * @param props An object with properties to include in the payload
			 */
			showConfig(props)
			{
				const payload = {
					...props,
					modalProps: {
						id: `${this.controlId}-config`
					}
				}
				this.emitEvent('show-config', payload)
			},

			/**
			 * Sets all data properties from props passed in
			 */
			initConfig()
			{
				// Merge properties of config prop
				// Replace arrays with the new values instead of merging
				mergeWith(this, this.config, (objValue, srcValue) => {
					if (Array.isArray(objValue) && Array.isArray(srcValue))
						return srcValue
				})
			},

			/**
			 * Get navigation row index from the row's multi-index
			 * @param multiIndex {string} Row multi-index
			 */
			getNavRowIndexFromMultiIndex(multiIndex)
			{
				return this.navRowElems.findIndex((elem) => {
					return elem?.getAttribute('index')?.toString() === multiIndex.toString()
				})
			},

			/**
			 * Get table action sub-elements of a given element
			 * @param element {DOMElement} DOM element
			 */
			getTableRowActionElements(element)
			{
				const actionElements = []

				// Set main element as first if it has the attribute
				if(element?.hasAttribute('data-table-action-selected'))
					actionElements.push(element)

				// Get sub-elements
				const actionSubElementsNodeList = element?.querySelectorAll("[data-table-action-selected]")
				if(actionSubElementsNodeList !== undefined && actionSubElementsNodeList !== null)
					actionElements.push(...Array.from(actionSubElementsNodeList))

				return actionElements
			},

			/**
			 * Reset table action properties of sub-elements of a given element
			 * @param rowIndex {number} Row index
			 */
			resetTableRowActionProperties(rowIndex)
			{
				if(rowIndex === undefined || rowIndex === null)
					return

				const rowElems = this.navRowElems
				if(rowElems.length > 0)
				{
					// Clear the state for all action elements in this row
					const actionElements = this.getTableRowActionElements(rowElems[rowIndex])
					actionElements.forEach((actionElement) => {
						actionElement.setAttribute('data-table-action-selected', 'false')
					})
				}
			},

			/**
			 * Reset table action state
			 * @param rowIndex {number} Row index
			 */
			resetTableRowActionState(rowIndex)
			{
				// Reset action element properties
				this.resetTableRowActionProperties(rowIndex)

				// If focus is left on an element that is inconsistent with the navigation state
				// focus on the table container element
				if(document.activeElement?.getAttribute('data-table-action-selected') === 'false')
				{
					const tableContainerElem = this.getTableContainerElement()
					tableContainerElem?.focus()
				}
			},

			/**
			 * Focus on the next or previous action sub-element of a given element
			 * @param actionElements {DOMElement} DOM element
			 * @param direction {string} Direction to move focus ("first", "next" or "previous")
			 */
			focusTableRowActionElement(actionElements, direction = 'first')
			{
				if(actionElements === undefined || actionElements === null || actionElements.length === 0)
					return

				let isFocused = false

				// Set value to add or subtract from index based on direction value
				const directionIdx = direction === 'next' ? 1 : -1

				// Find focused element and focus on the next one if it exists
				actionElements.every((actionElement, index, array) => {
					// If element is focused
					if(actionElement.getAttribute('data-table-action-selected') === 'true')
					{
						// If focusing on the first element, set all others to false
						if(direction === 'first')
						{
							actionElement.setAttribute('data-table-action-selected', 'false')
							return true
						}

						isFocused = true

						// Prevent going out of bounds for adjacentActionElement
						if((directionIdx === 1 && index >= array.length - 1)
							|| (directionIdx === -1 && index === 0))
							return false

						const adjacentActionElement = actionElements[index + directionIdx]
						if(typeof adjacentActionElement.focus === 'function')
						{
							actionElement.setAttribute('data-table-action-selected', 'false')
							adjacentActionElement.setAttribute('data-table-action-selected', 'true')
							adjacentActionElement.focus()
							return false
						}
					}
					// If element is not focused go to next iteration
					return true
				})

				// None of the elements are focused. Focus on the first one.
				if(!isFocused && typeof actionElements[0].focus === 'function')
				{
					actionElements[0].setAttribute('data-table-action-selected', 'true')
					actionElements[0].focus()
				}
			},

			/**
			 * Navigate to next or previous row action
			 * @param direction {string} Direction to move focus ("first", "next" or "previous")
			 */
			navigateToTableRowAction(direction)
			{
				// Find action elements and focus on adjacent one
				const rowElems = this.navRowElems
				if(rowElems.length === 0)
					return
				const actionElements = this.getTableRowActionElements(rowElems[this.navRowIndex])
				this.focusTableRowActionElement(actionElements, direction)
			},

			/**
			 * Navigate to row
			 * @param index {number} Index of row to navigate to
			 */
			async navigateToRow(index)
			{
				const rowElems = this.navRowElems ?? []

				// Check index bounds
				if(index < 0 || index >= rowElems.length)
					return

				// Navigate to row
				if(this.navRowIndex === undefined || this.navRowIndex === null)
					this.navRowIndex = 0

				if(index !== this.navRowIndex)
				{
					// Reset state, including properties, of action elements of current row
					this.resetTableRowActionState(this.navRowIndex)
				}

				this.navRowIndex = index

				// Find action elements and focus on first one
				await nextTick()
				this.navigateToTableRowAction('first')
			},

			/**
			 * Navigate to row by multi-index
			 * @param multiIndex {string} Row multi-index
			 */
			navigateToRowByMultiIndex(multiIndex)
			{
				this.navigateToRow(this.getNavRowIndexFromMultiIndex(multiIndex))
			},

			/**
			 * Reset table navigation state
			 */
			resetNavigationState()
			{
				// Reset state, including properties, of action elements of current row
				this.resetTableRowActionState(this.navRowIndex)
				// Reset row index
				this.navRowIndex = null
			},

			/**
			 * Emit row navigation index
			 *
			 * @param multiIndex {string | number} Index of row
			 * @param isNavigated {boolean} Whether the navigation is on this row
			 */
			emitRowNavigationIndex(multiIndex, isNavigated)
			{
				if (this.unmountedComponent) return

				// If the navigation is on the header row
				if(multiIndex === 'h')
					this.emitEvent('set-property', ['headerRow', 'isNavigated'], isNavigated)
				// If the navigation is on a normal row
				else
					this.emitEvent('set-row-index-property', multiIndex, 'isNavigated', isNavigated)
			},

			/**
			 * Focusout handler for table
			 * @param event {object} event object
			 */
			tableOnFocusout(event)
			{
				const tableContainerElem = this.getTableContainerElement()
				// If focus is on the table or sub-element of the table
				// Logically the focus is on the table
				if(event.relatedTarget === tableContainerElem
					|| tableContainerElem.contains(event.relatedTarget))
					return

				this.resetNavigationState()
			},

			/**
			 * Keydown handler for table
			 * FOR: TABLE KEYBOARD OPERATION
			 * @param event {object} Event object
			 */
			tableOnKeyDown(event)
			{
				const key = event?.key
				const tableContainerElem = this.getTableContainerElement()

				switch (key)
				{
					case 'Tab':
					case 'Escape':
						this.resetNavigationState()
						// If the focused element is the table
						if(event.target === tableContainerElem)
							return
						// If the focused element is a sub-element of the table
						tableContainerElem?.focus()
						event.preventDefault()
						break
					case 'ArrowUp':
						// Navigate to the previous row
						this.navigateToRow(this.navRowIndex - 1)
						event.preventDefault()
						break
					case 'ArrowDown':
						// Navigate to the first row if not navigated to any row, other wise navigate to the next row
						if(this.navRowIndex === undefined || this.navRowIndex === null || isNaN(this.navRowIndex))
							this.navigateToRow(this.firstDataRowIndex)
						else
							this.navigateToRow(this.navRowIndex + 1)
						event.preventDefault()
						break
					case 'ArrowLeft':
						// If the focused element is the table
						// Let the normal scrolling happen
						if(event.target === tableContainerElem)
							return
						// Navigate to the previous action element
						this.navigateToTableRowAction('previous')
						event.preventDefault()
						break
					case 'ArrowRight':
						// If the focused element is the table
						// Let the normal scrolling happen
						if(event.target === tableContainerElem)
							return
						// Navigate to the next action element
						this.navigateToTableRowAction('next')
						event.preventDefault()
						break
					case 'Home':
						// Navigate to first data row
						this.navigateToRow(this.firstDataRowIndex)
						event.preventDefault()
						break
					case 'End':
						// Navigate to last data row
						this.navigateToRow(this.navRowElems?.length - 1)
						event.preventDefault()
						break
					case 'Insert': {
						// Get insert action
						const insertActions = this.generalActions.filter((action) => action.id === 'insert')
						if (!insertActions || insertActions.length === 0) return
						const insertAction = insertActions[0]

						// Set element to focus after if opening a popup form
						insertAction.returnElement = this.tableContainerId

						// Insert new record
						this.emitEvent('row-action', insertAction)
						event.preventDefault()
						break
					}
					case 'PageUp':
						// Go to previous page
						if(this.page > 1)
							this.goToPage(this.page - 1)
						event.preventDefault()
						break
					case 'PageDown':
						// Go to previous page
						if(this.hasMorePages)
							this.goToPage(this.page + 1)
						event.preventDefault()
						break
				}
			},

			/**
			 * FOR: TABLE KEYBOARD OPERATION
			 * Focusin handler for table rows
			 * Used to set keyboard navigation state when focusing on a row or row sub-element
			 * so keyboard navigation can continue from there
			 * @param event {object} Event object
			 */
			async rowOnFocusin(event)
			{
				// Actual element mousedown happens on
				const element = event?.target

				// Get action element that mousedown happened on or propagated to
				// since mousedown can originate on sub-elements
				let actionElement = element
				// If element that get's focused is a parent element of the element
				while (!actionElement?.hasAttribute('data-table-action-selected') && actionElement?.parentElement)
					actionElement = actionElement.parentElement

				// Get row element
				let rowElement = element
				while (rowElement?.tagName !== 'TR' && rowElement?.parentElement)
					rowElement = rowElement.parentElement

				// Get multi-index of row
				const multiIndex = rowElement.getAttribute('index')

				// Set row as the current navigated row
				this.navRowIndex = this.getNavRowIndexFromMultiIndex(multiIndex)

				// Reset properties of action elements of current row
				this.resetTableRowActionProperties(this.navRowIndex)

				// Set navigation state on this element
				if(actionElement?.hasAttribute('data-table-action-selected'))
					actionElement.setAttribute('data-table-action-selected', true)

				// Prevent re-rendering rows when changing the row navigated property
				this.rerenderRowsOnNextChange = false

				if (this.config.returnElement)
					await nextTick()

				// Set the row property that signals if the row is navigated
				this.emitRowNavigationIndex(multiIndex, true)
			},

			/**
			 * FOR: TABLE KEYBOARD OPERATION
			 * Focusout handler for table rows
			 * Used to update keyboard navigation state when focusing away from a row or row sub-element
			 * so keyboard navigation can continue from there
			 * @param event {object} Event object
			 */
			rowOnFocusout(event)
			{
				// Actual element focusout happens on
				const element = event?.target

				// Get row element
				let rowElement = element
				while (rowElement?.tagName !== 'TR' && rowElement?.parentElement)
					rowElement = rowElement.parentElement

				// Element focus went to, if any
				const elementFocused = event?.relatedTarget

				// If element focus went to is in the row, navigation stays on the row
				if(rowElement.contains(elementFocused))
					return

				// Reset properties of action elements
				this.resetTableRowActionProperties(this.navRowIndex)

				//Get multi-index of row
				const multiIndex = rowElement.getAttribute('index')

				// Prevent re-rendering rows when changing the row navigated property
				this.rerenderRowsOnNextChange = false

				// Set the row property that signals if the row is navigated
				this.emitRowNavigationIndex(multiIndex, false)
			},

			/**
			 * Go to page
			 * @param pageNumber {number}
			 */
			goToPage(pageNumber)
			{
				if (this.page === pageNumber)
					return

				this.page = pageNumber
				this.emitEvent('set-property', ['config', 'page'], pageNumber)

				if (!this.readonly && this.rowsSelectableMultiple)
					this.initHeaderSelector()

				this.emitEvent('refresh')
			},

			/**
			 * Set records per page
			 * @param perPage {number}
			 */
			setPerPage(perPage)
			{
				if (this.perPage === perPage)
					return

				this.perPage = perPage

				// Update property in table model object
				this.emitEvent('set-property', ['config', 'perPageSelected'], perPage)

				this.emitEvent('set-confirm-changes', true)
				this.emitEvent('refresh')
			},

			/**
			 * Initialize search
			 */
			initHeaderSelector()
			{
				this.emitEvent('init-all-selected', this.id)

				//If less than three records, disable button
				if (this.allSelected)
				{
					this.checkCurrentPageRows(true)
					this.disableAllChecks()
				}
			},

			/**
			 * Change sorting
			 * @param {string} columnName Column name
			 * @param {string} sortOrder Sort direction
			 */
			changeSort(columnName, sortOrder)
			{
				this.emitEvent('update:sorting', { columnName, sortOrder })
			},

			/**
			 * Change sorting used when loading the table
			 * @param {string} columnName Column name
			 * @param {string} sortOrder Sort direction
			 */
			changeInitialSort(columnName, sortOrder)
			{
				this.emitEvent('set-confirm-changes', true)
				this.changeSort(columnName, sortOrder)
			},

			/**
			 * Inititalize row
			 * @param row {Object}
			 * @param rowRum {number}
			 */
			initRow(row, rowRum)
			{
				if (rowRum === undefined) row.Rownum = -1
				else row.Rownum = rowRum

				row.Fields = {}
			},

			/**
			 * Updates to do when columns are changed
			 * FOR: ROW ACTIONS, EXTENDED ACTIONS, COLUMN ORDER AND VISIBILITY
			 * Must be called when loading or changing columns
			 */
			updateColumns()
			{
				//Put references to columns in vbtColumns
				this.vbtColumns.forEach(column => {
					if(typeof column.destroy === 'function')
						column.destroy()
				})
				this.vbtColumns.splice(0)

				for (const column of this.columns)
					this.vbtColumns.push(column.clone?.() ?? cloneDeep(column))

				//FOR: TABLE LIST ROW ACTIONS
				//BEGIN: Add row actions column
				if (this.hasRowActions)
				{
					const actionsColumn = {
						label: this.texts.actionMenuTitle,
						name: 'actions',
						slotName: 'actions',
						dataType: 'Action',
						isActions: true,
						columnClasses: 'row-actions',
						columnHeaderClasses: 'thead-actions',
						rowTextAlignment: 'text-center',
						columnTextAlignment: 'text-center'
					}

					if (this.actionsPlacement === 'right')
						this.vbtColumns.push(actionsColumn)
					else if (this.actionsPlacement === 'left')
						this.vbtColumns.unshift(actionsColumn)
				}
				//END: Add row actions column

				//BEGIN: Add checklist column
				const checklistColumn = {
					label: '',
					name: 'Checkbox',
					slotName: 'checklist',
					dataType: 'Checkbox',
					isChecklist: true,
					columnClasses: 'row-checklist',
					checkListName: 'CheckList Name',
					checkListTitle: 'CheckList Title'
				}

				if (this.rowsSelectableMultiple !== false)
					this.vbtColumns.unshift(checklistColumn)
				//END: Add checklist column

				//FOR: EXTENDED ROW ACTIONS
				//Add row actions column
				if (this.hasExtendedRowActions)
				{
					this.vbtColumns.unshift({
						label: '',
						name: 'ExtendedAction',
						dataType: 'ExtendedAction',
						isExtendedActions: true,
						columnClasses: 'row-extended-actions',
						columnHeaderClasses: 'thead-actions'
					})
				}

				//FOR: TABLE LIST ROW DRAG AND DROP
				//BEGIN: Add drag and drop column
				const dragColumn = {
					label: this.texts.rowDragAndDropTitle,
					name: 'dragAndDrop',
					slotName: 'dragAndDrop',
					isDragAndDrop: true,
					columnClasses: 'row-orders',
					columnHeaderClasses: 'thead-actions',
					rowTextAlignment: 'text-center',
					columnTextAlignment: 'text-center'
				}

				if (this.showRowDragAndDropOption || this.hasRowDragAndDrop)
					this.vbtColumns.unshift(dragColumn)
				//END: Add drag and drop column

				//BEGIN: Add totalizer title column (only if all columns in the table are data columns)
				if (this.columns.some(column => column.totalizer && listFunctions.isVisibleColumn(column))) {
					if (this.vbtColumns.every(column => this.isDataColumn(column))) {
						const totalizerColumn = {
							label: '',
							name: 'totalizer',
							slotName: 'totalizer',
							isTotalizer: true,
							columnClasses: 'row-orders',
							columnHeaderClasses: 'thead-actions',
							rowTextAlignment: 'text-center',
							columnTextAlignment: 'text-center'
						}

						this.vbtColumns.unshift(totalizerColumn)
					}

					// Find first non data column - this is the column that will have the ´Total´ title in the totalizer row
					const firstNonDataColumnIdx = this.vbtColumns.findIndex(column => !this.isDataColumn(column))

					this.vbtColumns[firstNonDataColumnIdx].isTotalizerTitle = true
				}
				//END: Add totalizer column

				// If tree table, create column hierarchy
				if (this.type === 'TreeList')
					this.columnHierarchy = listFunctions.getColumnHierarchy(this.vbtColumns)

				nextTick().then(() => {
					if (this.$refs.tableElem && this.$refs.tableContainerElem)
					{
						//FOR: COLUMN RESIZE
						this.applyColumnResizeable()
					}
				})
			},

			//FOR: COLUMN RESIZE
			/**
			 * Apply the column resizable feature
			 */
			applyColumnResizeable()
			{
				this.resizableGrid?.destroy()

				if (this.isListVisible && this.allowColumnResize)
				{
					this.resizableGrid = markRaw(
						new ColumnResizeable(
							this.$refs.tableElem,
							this.columnResizeOptions,
							this.$refs.tableFooterElem,
							this.$refs.tableContainerElem,
							this.$refs.tableWrapperElem
						)
					)

					this.resizableGrid.init()
				}
			},

			//FOR: TABLE LIST ACTIONS
			/**
			 * Returns true if the action is to be called and false otherwise (if false then selects the row clicked)
			 * @param row {Object}
			 */
			doClickAction(row)
			{
				if (!this.loaded) return false

				switch (this.rowClickActionInternal)
				{
					case 'selectSingle':
						this.toggleRowSelectSingle(row)
						return false
					case 'selectMultiple':
						if (!this.readonly)
							this.toggleRowSelectMultiple(row)
						return false
					default:
						return true
				}
			},

			/**
			 * Emit event and object representing action
			 * @param row {Object}
			 * @param column {Object}
			 */
			executeActionCell(row, column)
			{
				const emitAction = { row }

				if (column !== undefined) emitAction.column = column

				this.emitEvent('cell-action', emitAction)
			},

			//FOR: EXTENDED ROW ACTIONS
			disableAllChecks()
			{
				this.blockTableCheck = true
			},

			enableAllChecks()
			{
				this.blockTableCheck = false
			},

			/**
			 * Check all rows in "every" checklist column page
			 * @returns Boolean
			 */
			checkAllRows()
			{
				if (!this.allSelected)
				{
					this.checkCurrentPageRows()

					this.emitEvent('set-selected-rows', { isSelected: true, id: this.id })
					this.allSelected = true

					//Make sure no one can uncheck rows
					this.disableAllChecks()
				}
			},

			/**
			 * Check all rows in current checklist column page
			 * @returns Boolean
			 */
			async checkCurrentPageRows(isInit = false)
			{
				if (this.allSelected && !isInit)
				{
					this.emitEvent('set-selected-rows', { isSelected: false, id: this.id })
					this.allSelected = false

					//Re-enable rows
					this.enableAllChecks()
				}

				await nextTick()

				// Check all visible rows, if they aren't checked already
				const rowKeysArray = {}
				let wasUnselected = false

				for (const row of this.rows)
				{
					// If row isn't already selected, add it to selected rows
					if (!this.isRowSelected(row))
					{
						rowKeysArray[row.rowKey] = true
						wasUnselected = true
					}
				}

				if (wasUnselected)
					this.emitEvent('select-rows', rowKeysArray)
			},

			/**
			 * Uncheck all rows in "every" checklist column page
			 * @returns Boolean
			 */
			checkNoneRows()
			{
				if (this.allSelected)
				{
					this.emitEvent('set-selected-rows', { isSelected: false, id: this.id })
					this.allSelected = false

					//Re-enable rows
					this.enableAllChecks()
				}

				this.emitEvent('unselect-all-rows')
			},

			/**
			 * Get primary key of row
			 * @param row {Object}
			 * @returns String
			 */
			getRowPk(row)
			{
				if (row.Fields === undefined || this.pkColumn === undefined)
				{
					return null
				}
				return row.Fields[this.pkColumn]
			},

			/**
			 * Get primary key of row
			 * @param row {Object}
			 * @returns String
			 */
			getRowKey(row)
			{
				return this.getRowPk(row) || row.Rownum
			},

			//FOR: Formatting field data
			/**
			 * Get formatted string representing cell value. Calls formatting function based on column data type.
			 * @param row {Object}
			 * @param column {Object}
			 * @param options {Object} [optional]
			 * @returns String(plain text or HTML)
			 */
			getCellDataDisplay(row, column, options)
			{
				if (options !== undefined)
				{
					return listFunctions.getCellValueDisplay(this, row, column, options)
				}

				return listFunctions.getCellValueDisplay(this, row, column)
			},

			/**
			 * Get string for title attribute of each cell in a row
			 * @param row {Object}
			 * @param columns {Object}
			 * @returns String
			 */
			getRowCellDataTitles(row, columns)
			{
				const cellTitles = {}

				for (const column of columns)
				{
					// Columns with multiple values will display a tooltip for each badge instead of a single tooltip for the entire cell.
					if (column.isHtmlField || column.multipleValues) cellTitles[column.name] = ''
					else cellTitles[column.name] = listFunctions.getCellValueDisplay(this, row, column)
				}

				return cellTitles
			},

			/**
			 * Get string for ID attribute of each header cell
			 * @param columns {Object}
			 * @returns String
			 */
			getHeaderCellIds(columns)
			{
				const cellIds = {}

				for (const column of columns)
					cellIds[column.name] = this.name + '-' + column.name

				return cellIds
			},

			//BEGIN: Row methods
			/**
			 * Determine if row is in a valid state
			 * @param row {Object}
			 * @returns String
			 */
			rowIsValid(row)
			{
				return this.config.rowValidation?.fnValidate(row) !== false
			},

			/**
			 * Get row CSS classes
			 * @param row {Object}
			 * @returns String
			 */
			getRowClasses(row, columnsLevel = 0)
			{
				const rowClasses = []

				if (this.hasRowClickAction) rowClasses.push('c-table__row--clickable')

				if (!this.rowIsValid(row)) rowClasses.push(this.config.rowValidation.class ?? 'c-table__row--pending')
				else if (columnsLevel > 0) rowClasses.push('q-tree-table-row')

				return rowClasses.join(' ')
			},

			/**
			 * Get row title (for title attribute)
			 * @param row {Object}
			 * @returns String
			 */
			getRowTitle(row)
			{
				return this.rowIsValid(row)
					? ''
					: this.config.rowValidation.message.value
			},

			//FOR: ROW CLICK ACTION (default click action or select row)
			/**
			 * Do action when clicking on row: default click action (emit) or select row
			 * @param row {Object} Row object
			 * @param rowId {string} Row ID
			 */
			executeRowClickAction(row, rowId)
			{
				if (this.blockTableCheck || this.hasRowDragAndDrop)
					return

				//Remove child rows
				row = this.rowWithoutChildren(row)

				//Row actions that do not emit data outside of the component
				if (!this.doClickAction(row)) return

				//Execute default row action
				const rowKeyPath = listFunctions.getRowKeyPath(this.rows, row)
				if (Object.keys(this.rowClickAction).length > 0) this.emitEvent('row-action', { id: this.rowClickAction.id, rowKeyPath, returnElement: rowId })
				else this.emitEvent('row-action', { rowKeyPath, returnElement: rowId })
			},

			/**
			 * Emits an event to execute the specified action.
			 * @param {string} actionId The action identifier
			 */
			executeGeneralAction(actionId)
			{
				const action = this.generalActionItems.find((e) => e.key === actionId)
				this.emitEvent('row-action', action)
			},

			/**
			 * Toggle selecting/deselecting single row
			 * @param row {Object}
			 */
			toggleRowSelectSingle(row)
			{
				const rowKeyPath = listFunctions.getRowKeyPath(this.rows, row)

				//If row is already selected, remove from selected rows
				if (this.isRowSelected(row))
				{
					this.emitEvent('unselect-row', rowKeyPath)
				} else
				{
					//Add to selected rows
					this.emitEvent('select-row', { rowKeyPath, multipleSelection: false })
				}
			},

			/**
			 * Toggle selecting/deselecting row (add to or remove from selected rows array)
			 * @param row {Object}
			 */
			toggleRowSelectMultiple(row)
			{
				const rowKeyPath = listFunctions.getRowKeyPath(this.rows, row)

				//If row is already selected, remove from selected rows
				if (this.isRowSelected(row))
				{
					this.emitEvent('unselect-row', rowKeyPath)
				} else
				{
					//If row is not already selected, add to selected rows
					this.emitEvent('select-row', { rowKeyPath, multipleSelection: true })
				}
			},

			/**
			 * Determine if row is selected
			 * @param row {Object}
			 * @returns Boolean
			 */
			isRowSelected(row)
			{
				const rowKeyPath = listFunctions.getRowKeyPath(this.rows, row)

				for (const x in this.rowsSelected)
				{
					if (x.toString() === rowKeyPath.toString())
					{
						return true
					}
				}
				return false
			},

			//FOR: EXTENDED ROW ACTIONS
			/**
			 * Emit event to remove selected row
			 * @param rowKey {String}
			 * @returns Boolean
			 */
			removeRow(rowKey)
			{
				this.emitEvent('remove-row', rowKey)
			},
			//END: Row methods

			/**
			 * Submit selected rows
			 * @param event {String}
			 * @returns
			 */
			rowGroupAction(event)
			{
				this.emitEvent('row-group-action', {
					rowsSelected: this.rowsSelected,
					allSelected: this.allSelected,
					action: event.action
				})
			},

			/**
			 * Search by a column for a value
			 * @param column {Object}
			 * @param value {String}
			 */
			searchByColumn(column, value)
			{
				// Normalize value
				if (column !== null)
					value = column.getNormalizedValue(value)

				const columnName = listFunctions.columnFullName(column)
				const operator = searchFilterData.searchBarOperator(column?.searchFieldType ?? 'text', value)

				// If a similar filter already exists, do nothing.
				// TODO: Create a Filter type and implement this logic there.
				if (this.filters.some((f) => f.active &&
					f.field === columnName &&
					f.operator === operator &&
					f.values.length === 1 &&
					f.subFilters.length === 0 &&
					f.values[0] === value))
					return

				const searchFilter = listFunctions.searchFilter('', true, columnName, operator, [value])
				const filters = cloneDeep(this.filters)
				filters.push(searchFilter)

				this.emitEvent('update:filters', filters)
			},

			/**
			 * Emit action to export table data
			 * @param format {String}
			 * @returns Boolean
			 */
			exportData(format)
			{
				this.emitEvent('on-export-data', { format })
			},

			/**
			 * Emit action to import table data
			 * @param format {String}
			 * @returns Boolean
			 */
			importData(payload)
			{
				this.emitEvent('on-import-data', payload)
			},

			/**
			 * Emit action to download template file
			 * @param format {String}
			 * @returns Boolean
			 */
			exportTemplate(format)
			{
				this.emitEvent('on-export-template', { format })
			},

			/**
			 * Update the active filters
			 * @param activeFilters {Object}
			 */
			updateActiveFilters(activeFilters)
			{
				this.emitEvent('update:activeFilters', activeFilters)
			},

			/**
			 * Update the group filters
			 * @param groupFilters {Object}
			 */
			updateGroupFilters(groupFilters)
			{
				this.emitEvent('update:groupFilters', groupFilters)
			},

			/**
			 * listener for events when shift key is pressed
			 */
			documentOnSelectStart(event)
			{
				return !(event.key === 'Shift' && event.shiftKey === true)
			},

			/**
			 * listener for events when shift key is pressed
			 */
			handleShiftKeyOnSelectStart()
			{
				document.addEventListener('selectstart', this.documentOnSelectStart)
			},

			/**
			 * Add event listeners for events when shift key is pressed
			 */
			handleShiftKey()
			{
				window.addEventListener('keyup', this.handleShiftKeyOnSelectStart)
				window.addEventListener('keydown', this.handleShiftKeyOnSelectStart)
			},

			/**
			 * Determine if column is visible
			 * @param column {Object}
			 * @returns Boolean
			 */
			canShowColumn(column)
			{
				//FOR: DRAG AND DROP COLUMNS
				if ((this.isDragAndDropColumn(column) && !this.hasRowDragAndDrop) || (this.isActionsColumn(column) && this.hasRowDragAndDrop))
					return false
				//For all columns
				return listFunctions.isVisibleColumn(column)
			},

			/**
			 * Toggle showing/hiding filters
			 */
			toggleShowFilters()
			{
				this.filtersVisible = !this.filtersVisible
				this.emitEvent('set-property', ['config', 'filtersVisible'], this.filtersVisible)
			},

			/**
			 * Emit to add new row
			 * @param row {Object}
			 */
			emitRowAdd(row)
			{
				this.emitEvent('row-add', row)

				//Clear object for new row
				this.initRow(this.newRow)
			},

			/**
			 * Emit to update row values
			 * @param row {Object}
			 */
			emitRowEdit(row)
			{
				this.emitEvent('row-edit', row)
			},

			/**
			 * Emit to delete rows
			 * @param rowKeys {Object}
			 */
			emitRowsDelete(rowKeys)
			{
				this.emitEvent('rows-delete', rowKeys)

				//Clear checked rows so it doesn't have keys of records that don't exist
				this.emitEvent('unselect-all-rows')
			},

			/**
			 * Set the value of a cell
			 * @param row {Object}
			 * @param column {Object}
			 * @param value {Object}
			 */
			// TODO: This method is mutating props (row is an element of rows)...
			setCellValue(row, column, value)
			{
				listFunctions.setCellValue(row, column, value)
			},

			/**
			 * Called when updating the value of a cell
			 * @param row {Object}
			 * @param column {Object}
			 * @param value {Object}
			 */
			updateCell(row, column, event)
			{
				this.emitEvent('update-cell', { row, column, value: event })

				this.rerenderRows()

				//Set cell value in component data
				this.setCellValue(row, column, event)

				if (this.fieldsEditable)
					this.emitRowEdit(row, column, event)

				if (this.hasRowDragAndDrop && column.sortOrder > 0)
				{
					const index = listFunctions.getCellValue(row, column)
					this.navigateToRow(parseInt(index) - 1)
				}
			},

			/**
			 * Toggle row reorder by drag and drop
			 */
			toggleShowRowDragAndDrop()
			{
				if (!this.hasRowDragAndDrop)
				{
					// Sort by the ordering column
					if (this.sortOrderColumn !== undefined && this.sortOrderColumn !== null)
					{
						// Change sorting without saving to table configuration
						this.changeSort(this.sortOrderColumn.name, 'asc')
					}
				}

				this.emitEvent('toggle-rows-drag-drop')

				if (!this.hasRowDragAndDrop)
				{
					this.setPerPage(-1)
				}

				// FOR: GETTING NAVIGABLE ROW ELEMENTS
				// Rerender rows and re'calculate navigable rows
				this.rerenderRows()
			},

			/**
			 * Get the table DOM element
			 * @returns
			 */
			getTableElement()
			{
				const tableElem = this.$refs.tableElem

				if (tableElem === undefined || tableElem === null) return null

				if (Array.isArray(tableElem))
				{
					if (tableElem.length < 1) return null
					return tableElem[0]
				}

				return tableElem
			},

			/**
			 * Get the table wrapper DOM element
			 * @returns
			 */
			getTableWrapperElement()
			{
				const tableWrapperElem = this.$refs.tableWrapperElem
				if (tableWrapperElem === undefined || tableWrapperElem === null)
				{
					return null
				}
				if (Array.isArray(tableWrapperElem))
				{
					if (tableWrapperElem.length < 1)
					{
						return null
					}
					return tableWrapperElem[0]
				}

				return tableWrapperElem
			},

			/**
			 * Get the table container DOM element
			 * @returns
			 */
			getTableContainerElement()
			{
				const tableContainerElem = this.$refs.tableContainerElem
				if (tableContainerElem === undefined || tableContainerElem === null)
				{
					return null
				}
				if (Array.isArray(tableContainerElem))
				{
					if (tableContainerElem.length < 1)
					{
						return null
					}
					return tableContainerElem[0]
				}

				return tableContainerElem
			},

			/**
			 * Get the table footer DOM element
			 * @returns
			 */
			getTableFooterElement()
			{
				const tableFooterElem = this.$refs.tableFooterElem
				if (tableFooterElem === undefined || tableFooterElem === null)
				{
					return null
				}
				if (Array.isArray(tableFooterElem))
				{
					if (tableFooterElem.length < 1)
					{
						return null
					}
					return tableFooterElem[0]
				}

				return tableFooterElem
			},

			/**
			 * Fired on multiform select
			 * @param row {Object}
			 * @returns
			 */
			onMultiformSelect(row)
			{
				for (const idx in this.rowFormProps)
				{
					if (this.rowFormProps[idx].mode === 'EDIT')
					{
						return
					}
				}
				this.emitEvent('set-array-sub-prop-where', 'rowFormProps', 'id', row.rowKey, 'mode', 'EDIT', 'SHOW')
			},

			/**
			 * Handler for reordering rows with keyboard.
			 * @param evt {Object} { row, sortOrderColumn, shiftValue }
			 * @returns
			 */
			rowReorder(eventData)
			{
				// Emit for row reorder by the server
				const rowKey = this.getRowKey(eventData.row),
					currentIndex = findIndex(this.vbtRows, (row) => this.getRowKey(row) === rowKey),
					index = currentIndex + eventData.shiftValue
				this.emitEvent('row-reorder', { rowKey, index })
				// Set table navigation to the row
				this.navigateToRow(parseInt(index))
				this.emitEvent('set-property', ['config', 'setNavOnUpdate'], true)
				// FOR: GETTING NAVIGABLE ROW ELEMENTS
				// Make rows re-mount
				this.rerenderRows()
			},

			/**
			 * Handler for reordering rows with drag and drop.
			 * @param evt {Object}
			 * @returns
			 */
			onRowDragAndDrop(evt)
			{
				if (evt.newIndex !== evt.oldIndex)
				{
					// Emit for row reorder by the server
					const rowIndex = parseInt(evt.item.getAttribute('index')),
						row = this.vbtRows[rowIndex],
						rowKey = row ? row.rowKey : null,
						index = evt.newIndex
					if (!rowKey) return

					// Removes the class to prevent the selection of other rows text,
					// added when starting the drag
					this.$refs.tableElem.style.userSelect = 'unset'

					this.emitEvent('row-reorder', { rowKey, index })
					// Set table navigation to the row
					this.navigateToRow(parseInt(index))
					this.emitEvent('set-property', ['config', 'setNavOnUpdate'], true)
					// FOR: GETTING NAVIGABLE ROW ELEMENTS
					// Make rows re-mount
					this.rerenderRows()
				}
			},

			destroyRowsDragAndDrop()
			{
				if (this.sortablePlugin && this.sortablePlugin.destroy)
				{
					this.sortablePlugin.destroy()
				}
				this.sortablePlugin = null
			},

			initRowsDragAndDrop()
			{
				this.destroyRowsDragAndDrop()
				try
				{
					if (this.hasRowDragAndDrop)
					{
						this.sortablePlugin = Sortable.create(Array.isArray(this.$refs.tbody) ? this.$refs.tbody[0] : this.$refs.tbody, {
							draggable: 'tr',
							dataIdAttr: 'id',
							direction: 'vertical',
							chosenClass: 'q-table__row-grabbed',
							ghostClass: 'q-table__row-ghost',
							dragClass: 'q-table__row-dragging',
							handle: '.q-table__row-drag-handle',
							animation: 300,
							forceFallback: true,
							fallbackClass: 'q-table__row-dragging',
							// Adds a class to prevent the selection of other rows text
							onStart: () => this.$refs.tableElem.style.userSelect = 'none',
							onEnd: this.onRowDragAndDrop
						})
					}
				} catch {
					this.destroyRowsDragAndDrop()
				}
			},

			/**
			 * Scroll to row
			 * @param rowId {String}
			 * @param behavior {String}
			 * @returns
			 */
			scrollToRow(rowId, behavior = 'smooth')
			{
				const elem = document.getElementById(rowId)
				if (!elem) return
				const wrapper = this.tableContainerElem
				const dist = elem.offsetTop - 160
				wrapper?.scroll?.({ top: dist, left: 0, behavior })
			},

			/**
			 * Scroll to and select row
			 * @param rowKeyPath {Array/String}
			 * @param rowId {String}
			 * @returns
			 */
			goToRow(rowKeyPath, rowId)
			{
				this.emitEvent('go-to-row', rowKeyPath)
				this.scrollToRow(rowId)
			},

			setChildRowsVisibility(eventData)
			{
				if (eventData.show === true && eventData.row?.alreadyLoaded === false)
				{
					// Prevent rows from re-rendering when changing the property to show sub-rows
					this.emitEvent('set-property', ['config', 'rerenderRowsOnNextChange'], false)

					this.emitEvent('tree-load-branch-data', { row: eventData.row })
				}
			},

			/**
			 * Get all navigable row elements
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			getNavRowElements()
			{
				// Get main row elements
				const tableElem = this.getTableElement()
				const navRowElemsNL = tableElem.querySelectorAll('tr[data-table-action-selected]')
				const navRowElems = Array.from(navRowElemsNL)

				// Add header row if it has focusable elements
				const headerRowElement = this.$refs.headerRowElem?.headerRowRef
				const headerRowActionElements = this.getTableRowActionElements(headerRowElement)
				if(headerRowActionElements.length > 0)
					navRowElems.unshift(headerRowElement)

				return navRowElems
			},

			/**
			 * Set all navigable row elements
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			setNavRowElements()
			{
				/**
				 * Check if table element exists first. If not, cancel.
				 * This can happen if the table is already being unmounted because
				 * the rows are unmounted and changes to the rows trigger this function.
				 */
				const tableElem = this.getTableElement()
				if(tableElem === undefined || tableElem === null)
					return

				this.navRowElems = this.getNavRowElements()

				/**
				 * Set index of first data row.
				 * An index of 'h' is used for the header row.
				 * If element 0 of the array is the header row, the first data row is element 1
				 */
				if(this.navRowElems.length > 0 && this.navRowElems[0].getAttribute('index') === 'h')
					this.firstDataRowIndex = 1
				else
					this.firstDataRowIndex = 0
			},

			/**
			 * Set number of rows to render from row data loaded
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			setRowsToLoad()
			{
				// Track number of rows to render
				this.rowsToLoad = this.vbtRows?.length
			},

			/**
			 * Re-render rows
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			async rerenderRows()
			{
				const focusableSelector = `
					a[href],
					button:not([disabled]),
					input:not([disabled]),
					select:not([disabled]),
					textarea:not([disabled]),
					[tabindex]:not([tabindex="-1"])
				`

				// Before re-render, get the currently focused element
				const focusedIndex = Array.from(
					this.$refs.tbody.querySelectorAll(focusableSelector)
				).indexOf(document.activeElement)

				// Track number of rows to render
				this.setRowsToLoad()
				// Make rows re-mount
				this.rowDomKey++

				// After re-render, restore the focused element
				if (focusedIndex !== -1)
				{
					await nextTick()

					Array.from(
						this.$refs.tbody.querySelectorAll(focusableSelector)
					)[focusedIndex]?.focus()
				}
			},

			/**
			 * Called when a row is rendered
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			onRowLoaded()
			{
				this.rowsToLoad--
				if(this.rowsToLoad === 0)
					this.onRowsLoaded()
			},

			/**
			 * Called when rows are rendered
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			onRowsLoaded()
			{
				this.emitEvent('rows-loaded')
				this.setNavRowElements()
				// Uses setTimeout with 0 to prevent focusing before everything else has finished running
				setTimeout(() => this.setInitialFocus(), 0)

				// Update scroll indicators after all rows have loaded
				// so the table's scrollWidth property is accurate
				if (this.isListVisible)
					this.updateScrollers()
			},

			/**
			 * Called when all sub-rows are loaded
			 * FOR: GETTING NAVIGABLE ROW ELEMENTS
			 */
			onSubRowsLoaded()
			{
				this.emitEvent('rows-loaded')
				this.setNavRowElements()
			},

			/**
			 * Focus in the element specified by the focusElement property
			 */
			setInitialFocus()
			{
				if (
					this.config.focusElement === undefined ||
					this.config.focusElement === null ||
					this.config.focusElement === ''
				)
					return

				// Get element reference
				let focusElement = document.getElementById(this.config.focusElement)

				// Clear stored value
				this.emitEvent('set-property', ['config', 'focusElement'], '')

				// If element doesn't exist, focus on table container
				if (!focusElement)
					focusElement = document.getElementById(this.tableContainerId)

				if (typeof focusElement?.focus !== 'function') return
				focusElement.focus()
			},

			/**
			 * Sets the pressed state of the left scroll button.
			 * @param isPressed {Boolean} Whether the button is being pressed or not
			 */
			setScrollButtonLeftPressed(isPressed)
			{
				this.scrollHorizLeftPressed = isPressed
			},

			/**
			 * Sets the pressed state of the right scroll button.
			 * @param isPressed {Boolean} Whether the button is being pressed or not
			 */
			setScrollButtonRightPressed(isPressed)
			{
				this.scrollHorizRightPressed = isPressed
			},

			/**
			 * Scroll the table contents
			 * @param scrollAmountHoriz {Number} Amount to scroll horizontally
			 * @param scrollAmountVert {Number} Amount to scroll vertically
			 */
			scrollTable(scrollAmountHoriz, scrollAmountVert)
			{
				// Table container element
				const tableContainer = this.getTableContainerElement()
				// Scroll position before scrolling
				const scrollPosHoriz = tableContainer.scrollLeft
				const scrollPosVert = tableContainer.scrollTop

				// Scroll table contents
				tableContainer.scroll(scrollPosHoriz + scrollAmountHoriz, scrollPosVert + scrollAmountVert)
			},

			/**
			 * Called when scrolling the table contents
			 */
			async updateScrollers()
			{
				if (!this.hasHorizontalScrollers)
					return

				await nextTick()

				// Table container element
				const tableContainer = this.getTableContainerElement()

				if (!tableContainer)
					return

				// Whether the table is scrolled to the beginning horizontally
				const atBeginHoriz = tableContainer.scrollLeft < 1
				// Whether the table is scrolled to the end horizontally
				const atEndHoriz = tableContainer.scrollWidth - tableContainer.scrollLeft - tableContainer.clientWidth < 1

				// Update table scroll indicators
				this.scrollHorizLeftVisible = !atBeginHoriz
				this.scrollHorizRightVisible = !atEndHoriz

				// Update scroll button pressed states
				if (atBeginHoriz)
					this.setScrollButtonLeftPressed(false)
				if (atEndHoriz)
					this.setScrollButtonRightPressed(false)
			},

			/**
			 * Mouse down handler for the left scroll button.
			 */
			scrollButtonLeftOnMousedown()
			{
				this.setScrollButtonLeftPressed(true)

				if (this.scrollHorizIntervalId)
				{
					clearInterval(this.scrollHorizIntervalId)
					this.scrollHorizIntervalId = null
				}

				// Scroll table while scroll button is being pressed
				// Delay of 33ms to scroll at around 30fps
				// Scroll by 33px to match the normal scroll wheel speed
				// when using this delay
				this.scrollHorizIntervalId = setInterval(() => {
					// Scroll table
					this.scrollTable(-33)
					// If mouse is not down on scroll button anymore,
					// clear interval to stop scrolling
					if (!this.scrollHorizLeftPressed)
					{
						clearInterval(this.scrollHorizIntervalId)
						this.scrollHorizIntervalId = null
					}
				}, 33)
			},

			/**
			 * Mouse down handler for the right scroll button.
			 */
			scrollButtonRightOnMousedown()
			{
				this.setScrollButtonRightPressed(true)

				if (this.scrollHorizIntervalId)
				{
					clearInterval(this.scrollHorizIntervalId)
					this.scrollHorizIntervalId = null
				}

				// Scroll table while scroll button is being pressed
				// Delay of 33ms to scroll at around 30fps
				// Scroll by 33px to match the normal scroll wheel speed
				// when using this delay
				this.scrollHorizIntervalId = setInterval(() => {
					// Scroll table
					this.scrollTable(33)
					// If mouse is not down on scroll button anymore,
					// clear interval to stop scrolling
					if (!this.scrollHorizRightPressed)
					{
						clearInterval(this.scrollHorizIntervalId)
						this.scrollHorizIntervalId = null
					}
				}, 33)
			}
		},

		watch: {
			rows: {
				handler(newVal)
				{
					this.vbtRows = newVal
					this.isFirstTime = false

					if (this.allSelected) this.checkCurrentPageRows(true)

					// Track number of rows to render
					this.setRowsToLoad()

					if(this.rerenderRowsOnNextChange)
					{
						// FOR: GETTING NAVIGABLE ROW ELEMENTS
						// Rerender rows and re-calculate navigable rows
						this.rerenderRows()
					}
					else
					{
						this.rerenderRowsOnNextChange = true
						this.emitEvent('set-property', ['config', 'rerenderRowsOnNextChange'], true)
					}
				},
				deep: true
			},

			columns: {
				handler()
				{
					//FOR: ROW ACTIONS, EXTENDED ACTIONS, COLUMN ORDER AND VISIBILITY
					//Must be called when loading or changing columns
					this.updateColumns()
				},
				deep: true
			},

			activeViewModeId: {
				handler()
				{
					if (this.isListVisible)
					{
						nextTick().then(() => {
							if (this.$refs.tableElem && this.$refs.tableContainerElem)
							{
								//FOR: COLUMN RESIZE
								this.applyColumnResizeable()

								if (this.hasHorizontalScrollers)
								{
									// Update scroll indicators after all rows have loaded
									// so the table's scrollWidth property is accurate
									this.updateScrollers()

									// Check for table size changes to update scrollers
									this.resizeObserver = new ResizeObserver(() => {
										this.updateScrollers()
									})
									this.resizeObserver.observe(this.getTableElement())
									this.resizeObserver.observe(this.getTableContainerElement())
								}
							}
						})
					}
					else
					{
						this.resizeObserver?.disconnect()
						this.resizeObserver = null
					}
				},
				immediate: true
			},

			config: {
				handler()
				{
					this.initConfig()
				},
				deep: true
			},

			hasRowDragAndDrop(newValue)
			{
				if (newValue) this.initRowsDragAndDrop()
				else this.destroyRowsDragAndDrop()
			},

			allSelectedRows: {
				handler(newValue) {
					if (newValue === 'true')
						this.checkAllRows()
					else
						this.checkNoneRows()
				},
				immediate: true
			}
		}
	}
</script>
