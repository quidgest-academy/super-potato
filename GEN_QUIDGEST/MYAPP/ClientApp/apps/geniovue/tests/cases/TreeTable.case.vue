<template>
	<!-- Basic Tree Structure -->
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<form class="form-horizontal">
			<fieldset>
				<div class="form-flow">
					<q-row-container>
						<q-control-wrapper class="row-line-group">
							<q-table
								id="CTRL_1"
								row-component="q-tree-table-row"
								:rows="arrayToTree(tableData.rows)"
								:columns="tableData.columns"
								:config="{...tableData.config, columnHierarchy: getColumnHierarchy(tableData.columns)}"
								:total-rows="tableData.totalRows"
								:header-level="1"
								:readonly="tableData.readonly"
								@execute-action="executeAction"
								@row-action="rowAction"
								@cell-action="cellAction"
								@on-export-data="exportDataAction"
								@on-import-data="displayEmit"
								@on-export-template="exportTemplateAction"
								@on-show-import-popup="showImportPopupAction"
								@on-hide-import-popup="hideImportPopupAction" />
						</q-control-wrapper>
					</q-row-container>
				</div>
				<!-- MultiLevel -->
				<div>
					<q-row-container>
						<q-control-wrapper class="row-line-group">
							<q-table
								id="CTRL_2"
								row-component="q-tree-table-row"
								:rows="arrayToTree(multiLevelTreeData.rows)"
								:columns="multiLevelTreeData.columns"
								:config="{...multiLevelTreeData.config, columnHierarchy: getColumnHierarchy(multiLevelTreeData.columns)}"
								:total-rows="multiLevelTreeData.totalRows"
								:header-level="1"
								:readonly="multiLevelTreeData.readonly"
								@execute-action="executeAction"
								@row-action="rowAction"
								@cell-action="cellAction"
								@on-export-data="exportDataAction"
								@on-import-data="displayEmit"
								@on-export-template="exportTemplateAction"
								@on-show-import-popup="showImportPopupAction"
								@on-hide-import-popup="hideImportPopupAction" />
						</q-control-wrapper>
					</q-row-container>
				</div>
				<!-- Customizable Icons -->
				<div>
					<q-row-container>
						<q-control-wrapper class="row-line-group">
							<div style="display: flex">
								<div style="margin-right: 24px">
									<div style="font-weight: 600">Expand Icon</div>

									<input
										type="text"
										v-model="eIcon"
										label="Expand Icon" />
								</div>
								<div>
									<div style="font-weight: 600">Collapse Icon</div>
									<input
										type="text"
										v-model="cIcon"
										label="Collapse Icon" />
								</div>
							</div>
							<q-table
								id="CTRL_3"
								row-component="q-tree-table-row"
								:rows="arrayToTree(iconTreeData.rows)"
								:columns="iconTreeData.columns"
								:config="{...iconTreeData.config, columnHierarchy: getColumnHierarchy(iconTreeData.columns)}"
								:total-rows="iconTreeData.totalRows"
								:header-level="1"
								:readonly="iconTreeData.readonly"
								:expand-icon="eIcon"
								:collapse-icon="cIcon"
								@execute-action="executeAction"
								@row-action="rowAction"
								@cell-action="cellAction"
								@on-export-data="exportDataAction"
								@on-import-data="displayEmit"
								@on-export-template="exportTemplateAction"
								@on-show-import-popup="showImportPopupAction"
								@on-hide-import-popup="hideImportPopupAction" />
						</q-control-wrapper>
					</q-row-container>
				</div>
				<!-- Row with Long Data -->
				<div>
					<q-row-container>
						<q-control-wrapper class="row-line-group">
							<q-table
								id="CTRL_4"
								row-component="q-tree-table-row"
								:rows="arrayToTree(longTreeData.rows)"
								:columns="longTreeData.columns"
								:config="{...longTreeData.config, columnHierarchy: getColumnHierarchy(longTreeData.columns)}"
								:total-rows="longTreeData.totalRows"
								:header-level="1"
								:readonly="longTreeData.readonly"
								expand-icon="play-button"
								collapse-icon="folder-open"
								@execute-action="executeAction"
								@row-action="rowAction"
								@cell-action="cellAction"
								@on-export-data="exportDataAction"
								@on-import-data="displayEmit"
								@on-export-template="exportTemplateAction"
								@on-show-import-popup="showImportPopupAction"
								@on-hide-import-popup="hideImportPopupAction" />
						</q-control-wrapper>
					</q-row-container>
				</div>
				<!--- Tree with Invalid Data -->
				<div>
					<q-row-container>
						<q-control-wrapper class="row-line-group">
							<q-table
								id="CTRL_4"
								row-component="q-tree-table-row"
								:rows="arrayToTree(invalidTreeData.rows)"
								:columns="invalidTreeData.columns"
								:config="{...invalidTreeData.config, columnHierarchy: getColumnHierarchy(invalidTreeData.columns)}"
								:total-rows="invalidTreeData.totalRows"
								:header-level="1"
								:readonly="invalidTreeData.readonly"
								expand-icon="play-button"
								collapse-icon="folder-open"
								@execute-action="executeAction"
								@row-action="rowAction"
								@cell-action="cellAction" />
						</q-control-wrapper>
					</q-row-container>
				</div>
			</fieldset>
		</form>
	</div>
</template>

<script>
	import fakeData from './TreeTable.mock'

	export default {
		name: 'QTreeTableContainer',

		docsfile: './docs/table/QTreeTableRow.md',
		
		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				tableData: fakeData.simpleUsage().treeTest,
				multiLevelTreeData: fakeData.simpleUsage().multiLevelTreeData,
				iconTreeData: fakeData.simpleUsage().iconTreeData,
				longTreeData: fakeData.simpleUsage().longTreeData,
				invalidTreeData: fakeData.simpleUsage().invalidTreeData,
				eIcon: 'play-button',
				cIcon: 'arrow-down'
			}
		},

		methods: fakeData.simpleUsageMethods
	}
</script>
