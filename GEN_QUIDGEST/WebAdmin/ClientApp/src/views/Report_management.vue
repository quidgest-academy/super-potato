<template>
	<div id="report_management_container">
		<div class="q-stack--column">
				<h1 class="f-header__title">
					{{ Resources.GESTAO_DE_RELATORIOS37970 }}
				</h1>
			</div>
		<hr>

		<q-card
			width="block">
			<q-row-container>
				<q-control-wrapper class="row-line-group">
					<base-input-structure
						class="i-text">
						<static-text
							v-model="Model.ReportsPath"
							:label="Resources.DIRECTORIA_DE_RELATO59580" />
					</base-input-structure>
				</q-control-wrapper>
				<q-control-wrapper class="row-line-group">
					<base-input-structure
						class="i-text">
						<static-text
							v-model="Model.ReportsServerUrl"
							:label="Resources.URL_DO_SERVIDOR_DE_R44145" />
					</base-input-structure>
				</q-control-wrapper>
				<q-control-wrapper class="row-line-group">
					<base-input-structure
						class="i-text">
						<static-text
							v-model="Model.ReportsServerPath"
							:label="Resources.SUBPATH_NO_SERVIDOR_61718" />
					</base-input-structure>
				</q-control-wrapper>

				<hr />

				<row class="footer-btn">
					<q-button
						variant="bold"
						:label="Resources.VALIDACAO46021"
						@click="Check" />
					<q-button
						variant="bold"
						:label="Resources.INSTALACAO62245"
						@click="Deploy" />
				</row>

				<q-control-wrapper class="row-line-group">
					<base-input-structure
						class="i-text">
						<q-select
							v-model="deployForm.scope"
							item-value="Value"
							item-label="Text"
							:items="deployScope"
							:size="'xlarge'" />
					</base-input-structure>
					<base-input-structure
						class="i-text">
						<q-checkbox
							v-model="deployForm.dynamic" 
							:label="Resources.DINAMICO34700" />
					</base-input-structure>
					<base-input-structure
						class="i-text">
						<q-checkbox
							v-model="deployForm.delete" 
							:label="Resources.APAGAR_NAO_USADOS55190" />
					</base-input-structure>
				</q-control-wrapper>

				<qtable :rows="tReportList.rows"
					:columns="tReportList.columns"
					:config="tReportList.config"
					:totalRows="tReportList.total_rows"
					:classes="tReportList.classes">

					<template #status="props">
						<q-button
							v-if="!isEmptyObject(props.row.Error)"
							variant="text"
							color="danger"
							:title="props.row.Error"
							@click.stop="showError(props.row)">
							<q-icon icon="alert-circle" />
						</q-button>
						<q-icon
							v-else
							:icon="check" />
						{{ props.row.Status }}
					</template>
				</qtable>

				<qtable :rows="tSlotReportList.rows"
					:columns="tSlotReportList.columns"
					:config="tSlotReportList.config"
					@on-change-query="onChangeQuery"
					:totalRows="tSlotReportList.total_rows">

					<template #actions="props">
						<q-button-group borderless>
							<q-button
								variant="text"
								:title="Resources.EDITAR11616"
								@click="ManageSlotReport('edit', props.row[0])">
								<q-icon icon="pencil" />
							</q-button>
							<q-button
								variant="text"
								:title="Resources.APAGAR04097"
								@click="ManageSlotReport('delete', props.row[0])">
								<q-icon icon="bin" />
							</q-button>
						</q-button-group>
					</template>
					<template #table-footer>
						<tr>
							<td colspan="4">
								<q-button
									:label="Resources.INSERIR43365"
									@click="ManageSlotReport('new', '')">
									<q-icon icon="add" />
								</q-button>
							</td>
						</tr>
					</template>
				</qtable>

				<slotreport
					:show="slotReportModal.show"
					:Model="slotReportModal.data"
					@close="reloadSlotReport" />
				<progress-bar
					:show="Model.DeployActive"
					:text="dataPB.text"
					:progress="dataPB.progress" />
			</q-row-container>
		</q-card>	
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin.js'
	import { QUtils } from '@/utils/mainUtils'
	import bootbox from 'bootbox';
	import slotreport from './SlotReport.vue';

	export default {
		name: 'report_management',

		mixins: [reusableMixin],

		components: { slotreport },
		
		data: function () {
		var vm = this;
		return {
			dataPB: {
			text: '',
			progress: 0
			},
			Model: {},
			slotReportModal: {
				show: false,
				data: { }
			},
			deployForm: {
				scope: 'Different',
				dynamic: false,
				delete: false
			},
			/**
			* Slot report table Model
			*/
			tSlotReportList: {
				/**
				* list of rows (the first element of each row is the record key)
				*/
				rows: [],
				total_rows: 0,
				/**
				* Definition of table columns (first column is the action column)
				*/
				columns: [
					{
						label: () => vm.$t('ACOES22599'),
						name: "actions",
						slot_name: "actions",
						sort: false,
						column_classes: "thead-actions",
						row_text_alignment: 'text-center',
						column_text_alignment: 'text-center'
					},
					{
						label: () => vm.$t('RELATORIO62426'),
						name: "1",
						initial_sort: true,
						initial_sort_order: "asc",
					},
					{
						label: () => vm.$t('IDENTIFICADOR_DE_SLO30549'),
						name: "2",
						sort: true
					},
					{
						label:() => vm.$t('TITULO39021'),
						name: "3",
						sort: true
					},
					{
						label: () => vm.$t('CRIADO_EM61283'),
						name: "4",
						sort: true
					}
				],
				/**
				* Define table query paramenters (sort field, filters, etc)
				*/
				queryParams: {
					sort: [{ name: '1', order: 'asc' }],
					filters: [],
					global_search: "",
					per_page: 10,
					page: 1
				},
				/**
				* Define several table properties (Title, Buttons, etc.)
				*/
				config: {
					table_title: () => vm.$t('SLOTS_DE_RELATORIOS18277'),
					global_search: {
						classes: "qtable-global-search",
						searchOnPressEnter: true,
						showRefreshButton: true,
						//searchDebounceRate: 1000
					},
					server_mode: true,
					preservePageOnDataChange: true
				}
			},
			tReportList: {
					rows: [],
					total_rows: 0,
					columns: [
						{
							label: () => vm.$t('TIPO55111'),
							name: "ReportType",
							row_classes: 'text-nowrap'
						},
						{
							label: () => vm.$t('NOME47814'),
							name: "Name",
							initial_sort: true,
							initial_sort_order: "asc",
							sort: true
						},
						{
							label: () => vm.$t('DATA_INSTALACAO03103'),
							name: "DateInstall",
							sort: true
						},
						{
							label: () => vm.$t('DATA_FICHEIRO58453'),
							name: "DateFile",
							sort: true
						},
						{
							label: () => vm.$t('DINAMICO34700'),
							name: "Dynamic",
							sort: true
						},
						{
							label: () => vm.$t('ESTADO07788'),
							name: "Status",
							slot_name: 'status',
							sort: true
						}
					],
					config: {
					table_title: () => vm.$t('LISTA_DE_RELATORIOS46856')
					},
					classes: {
					cell: {
						'text-success': function(row, column, value) {
							return column.name === 'Status' && value === 'new';
						},
						'text-warning': function(row, column, value) {
							return column.name === 'Status' && value === 'modified';
						},
						'text-danger': function(row, column, value) {
							return column.name === 'Status' && (value === 'error' || value === 'deleted');
						}
					}
					}
				}
			};
		},

		computed: {
			deployScope: function () {
				var vm = this;
				return [
				{ Value: 'Different', Text: vm.Resources.DIFERENTES38084 },
				{ Value: 'Newer', Text: vm.Resources.RECENTES05062 },
				{ Value: 'All', Text: vm.Resources.TODOS59977 }
				];
			}
		},

		methods: {
			fetchData: function () {
				var vm = this;
				QUtils.log("Fetch data - Report management");
				QUtils.FetchData(QUtils.apiActionURL('ManageReports', 'Index')).done(function (data) {
				QUtils.log("Fetch data - OK (Report management)", data);
				$.each(data, function (propName, value) { vm.Model[propName] = value; });
				vm.fillTReportList();
				});
			},

			fillTReportList: function() {
				var vm = this;
				vm.tReportList.rows = vm.Model.ReportList || [];
				vm.tReportList.total_rows = (vm.Model.ReportList || []).length;
			},

			/**
			* Load slot report table data
			*/
			fetchReportSpotData: function () {
				var vm = this;
				var params = $.extend({}, vm.tSlotReportList.queryParams);
				QUtils.log("Fetch data - Report spot management");

				QUtils.FetchData(QUtils.apiActionURL('ManageReports', 'GetReportSpot', params)).done(function (data) {
					QUtils.log("Fetch data - OK (Report spot management)", data);
					vm.tSlotReportList.rows = data.data || [];
					vm.tSlotReportList.total_rows = data.recordsTotal || 0;
				});
			},

			Check: function () {
				this.fetchData();
			},

			Deploy: function () {
				var vm = this;
				QUtils.postData('ManageReports', 'StartDeploy', vm.deployForm, null, function (data) {
				vm.Model.DeployActive = data.DeployActive;
				vm.startMonitorProgress();
				});
			},

			startMonitorProgress: function() {
				if (this.Model.DeployActive && this.dataPB.progress === 0) {
					setTimeout(this.checkProgress, 250);
				}
			},

			checkProgress: function () {
				var vm = this;
				QUtils.FetchData(QUtils.apiActionURL('ManageReports', 'ProgressDeploy')).done(function (data) {
				vm.Model.DeployActive = !data.Completed;
				if (!data.Completed) {
					vm.dataPB.text = 'Script: ' + data.ActualScript;
					vm.dataPB.progress = data.Count;
					setTimeout(vm.checkProgress, 500);
				}
				else {
					if(data.Message) {
					bootbox.alert(data.Message)
					return;
					}

					vm.dataPB = {
					text: '',
					progress: 0
					};
					vm.fetchData();
				}
				});
			},

			showError: function (row) {
				var _html = `<span>${row.Error}</span>`;
				bootbox.alert(_html)
			},

			/**
			* Load slot report table data
			*/
			ManageSlotReport: function (mode, id) {
				var vm = this,
				url = QUtils.apiActionURL('ManageReports', 'ManageSlotReport',{ mod: mode, codreport:id });
				QUtils.FetchData(url).done(function (data) {
					vm.slotReportModal.data = data;
					vm.slotReportModal.data.FormMode = mode;
					vm.slotReportModal.show = true;
				});
			},

			/**
			* Reload slot report table data
			*/
			reloadSlotReport: function (reload) {
				this.slotReportModal.show = false;
				this.slotReportModal.data = {};
				if (reload) {
					this.fetchReportSpotData();
				}
			},

			/**
			* Search slot report
			*/
			onChangeQuery: function (queryParams) {
				var vm = this;
				$.each(queryParams, function (propName, value) { vm.tSlotReportList.queryParams[propName] = value; });
				vm.fetchReportSpotData();
			}
		},

		created: function () {
			// Ler dados
			this.fetchData();
			this.fetchReportSpotData();
		},

		watch: {
			// call again the method if the route changes
			'$route': 'fetchData'
		}
	};
</script>
