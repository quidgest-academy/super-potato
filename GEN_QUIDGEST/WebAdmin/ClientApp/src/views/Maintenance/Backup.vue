<template>
	<div id="maintenance_backup_container">
		<row>
			<q-card
				class="q-card--admin-default"
				:title="Resources.AUTENTICACAO_DE_BACK21740"
				width="block">
				<q-row-container>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<text-input v-model="Model.DbUser" :label="Resources.NOME_DE_UTILIZADOR58858" is-required size="xlarge"></text-input>
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<password-input v-model="Model.DbPsw" :label="Resources.PALAVRA_PASSE44126" is-required size="xlarge"></password-input>
						</base-input-structure>
					</q-control-wrapper>

					<row class="footer-btn">
						<q-button
							variant="bold"
							:label="Resources.EXECUTAR_BACKUP43010"
							@click="startBackup" />

						<data-system-badge
							:title="Resources.SISTEMA_DE_DADOS_ATU09110" />
					</row>
				</q-row-container>
			</q-card>
		</row>
		<br>
		<row>
			<qtable
				:rows="tBackups.rows"
				:columns="tBackups.columns"
				:config="tBackups.config"
				:totalRows="tBackups.total_rows"
				class="q-table--borderless">

				<template #actions="props">
					<q-button-group borderless>
						<q-button
							variant="text"
							:title="Resources.RESTAURAR57043"
							@click="restoreBackupFile(props.row)">
							<q-icon icon="restore" />
						</q-button>
						<q-button
							variant="text"
							:title="Resources.ELIMINAR21155"
							@click="deleteBackupFile(props.row)">
							<q-icon icon="bin" />
						</q-button>
					</q-button-group>
				</template>
				<template #Date="props">
					{{ formatDate(props.row.Date) }}
				</template>
				<template #Size="props">
					{{ props.row.Size }} Mb
				</template>
			</qtable>
		</row>
		<progress-bar :show="showPB" :withTitle="false" :text="Resources.BACKUP51008"></progress-bar>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import bootbox from 'bootbox';

	export default {
		name: 'maintenance_backup',

		mixins: [reusableMixin],

		emits: ['alert-class'],

		data() {
			var vm = this;
			return {
				Model: {},
				showPB: false,
				tBackups: {
					rows: [],
					total_rows: 0,
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
						label: () => vm.$t('DATA18071'),
						name: "Date",
						slot_name: 'Date',
						sort: true,
						initial_sort: true,
						initial_sort_order: "asc"
					},
					{
						label: 'Backup file',
						name: "Filename",
						sort: true
					},
					{
						label: () => vm.$t('TAMANHO__48454'),
						name: "Size",
						slot_name: 'Size',
						sort: true
					}],
					config: {
						table_title: () => vm.$t('RESTAURAR57043'),
						pagination_info: false
					}
				}
			};
		},

		methods: {
			fetchData() {
				var vm = this;
				QUtils.log("Fetch data - Maintenance - Backup");
				QUtils.FetchData(QUtils.apiActionURL('dbadmin', 'Backup')).done(function (data) {
				QUtils.log("Fetch data - OK (Maintenance - Backup)", data);
				$.each(data, function (propName, value) { vm.Model[propName] = value; });
				vm.setTableData();
				});
			},

			restoreBackupFile(bf) {
				var vm = this;
				bootbox.confirm({
					title: vm.Resources.RESTAURAR_BACKUP53628,
					message: vm.Resources.ESTA_OPERACAO_IRA_SU08117,
					buttons: {
						confirm: {
						label: vm.Resources.RESTAURAR57043,
						className: 'btn-primary'
						},
						cancel: {
						label: vm.Resources.CANCELAR49513,
						className: 'btn-secondary'
						}
					},
					callback(result) {
						if (result) {
						vm.Model.BackupItem = bf.Filename;
						vm.submitAction('Restore');
						}
					}
				});
			},

			deleteBackupFile(bf) {
				var vm = this;
				bootbox.confirm({
				title: vm.Resources.APAGAR_BACKUP27193,
				message: vm.Resources.TEM_A_CERTEZA_QUE_QU10641,
				buttons: {
					confirm: {
					label: vm.Resources.APAGAR04097,
					className: 'btn-danger'
					},
					cancel: {
					label: vm.Resources.CANCELAR49513,
					className: 'btn-secondary'
					}
				},
				callback(result) {
					if (result) {
					vm.Model.BackupItem = bf.Filename;
					vm.submitAction('DeleteBackup');
					}
				}
				});
			},

			startBackup() {
				this.submitAction('Backup');
			},

			submitAction(action) {
				var vm = this;
				vm.showPB = true;
				QUtils.log("Request", QUtils.apiActionURL('DbAdmin', action));
				QUtils.postData('DbAdmin', action, vm.Model, null, function (data) {
					QUtils.log("Response", data);
					$.each(data, function (propName, value) { vm.Model[propName] = value; });
					vm.showPB = false;
					vm.Model.BackupItem = null;
					if (data.ResultMsg) {
						vm.$emit('alert-class', { ResultMsg: data.ResultMsg, AlertType: data.AlertType || 'info' });
					}
					vm.setTableData();
				});
			},

			setTableData() {
				this.tBackups.rows = this.Model.BackupFiles || [];
				this.tBackups.total_rows = this.tBackups.rows.length;
			}
		},

		created() {
			// Ler dados
			this.fetchData();
		},

		watch: {
			// call again the method if the route changes
			'$route': 'fetchData'
		}
	};
</script>
