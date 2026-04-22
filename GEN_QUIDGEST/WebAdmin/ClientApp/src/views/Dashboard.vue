<template>
	<div id="dashboard_container">
		<div class="q-stack--column">
			<h1 class="f-header__title">
				{{ Resources.DASHBOARD51597 }}
			</h1>
		</div>
		<hr />
		<div
			v-if="!loaded"
			class="card text-center">
			<div class="card-body">
				<q-spinner-loader id="tab-loader" />
			</div>
			<h4>
				{{ Resources.A_CARREGAR___34906 }}
			</h4>
		</div>
		<div v-else>
			<!-- Errors banner -->
			<div v-if="Model.ResultErrors" class="alert alert--danger">
				<h4>{{ Resources.ERRO_35877 }}</h4>
				<p><span v-html="Model.ResultErrors"></span></p>
				<q-button
					v-if="showDBButton()"
					:label="Resources.MANUTENCAO_DA_BASE_D10092"
					@click.stop="navigateTo($event, 'maintenance')" />
				<q-button
					v-if="!Model.HasValidConfig"
					:label="Resources.INICIAR08126"
					@click.stop="navigateTo($event, 'config_migration')" />
			</div>
			<br v-if="Model.ResultErrors">
			<!-- Maintenance banner -->
			<div v-if="CurrentMaintenance.IsActive || CurrentMaintenance.IsScheduled" class="alert alert--info">
				<h4>{{ Resources.INFORMACAO46082 }}</h4>
				<div>
					<q-icon
						icon="alert"
						color="info" />
					<span>
						{{ maintenanceText }}
					</span>
				</div>
				<q-button
					:label="maintenanceBtnText"
					@click.stop="navigateTo($event, 'maintenance', false, 'schedule_maintenance-tab')" />
			</div>
			<br v-if="CurrentMaintenance.IsActive || CurrentMaintenance.IsScheduled" >
			<!-- Is Beta Test -->
			<div v-if="Model.IsBetaTesting" class="alert alert--warning">
				<p><b>
					{{ Resources.AMBIENTE_DE_QUALIDAD42119 }}
				</b></p>
			</div>

			<!-- INFORMATION -->
			<q-card
				class="q-card--admin-default"
				:title="Resources.SOBRE44896"
				width="block">
				<div class="container-fluid">
					<dl class="row">
						<dt>{{ Resources.SISTEMA05814 }}</dt>
						<dd>FOR</dd>
						<dt>{{ Resources.ACRONIMO12609 }}</dt>
						<dd>QUIDGEST</dd>
						<dt>{{ Resources.CLIENTE40500 }}</dt>
						<dd>Quidgest</dd>
					</dl>
					<dl class="row">
						<dt>{{ Resources.VERSAO_DE_SISTEMA07287 }}</dt>
						<dd>56</dd>
						<dt>{{ Resources.VERSAO_DE_BASE_DE_DA46937 }}</dt>
						<dd>{{ Model.VersionDbGen }}</dd>
						<dt>{{ Resources.APP_MIGRATION_VERSIO41495 }}</dt>
						<dd>0</dd>
						<dt>{{ Resources.VERSAO_DOS_INDICES49454 }}</dt>
						<dd>{{ Model.VersionIdxDbGen }}</dd>
						<dt>{{ Resources.VERSAO_DE_GENIO44840 }}</dt>
						<dd>382.64</dd>
						<dt>{{ Resources.GERADO_EM27272 }}</dt>
						<dd>04/22/2026</dd>
					</dl>
					<dl class="row">
						<span class="app-brand">
							<img src="@/assets/img/f-login__brand.png">
						</span>
					</dl> 
				</div>
			</q-card>
			<br />
			<q-card
				class="q-card--admin-default"
				:title="Resources.AMBIENTE12083"
				width="block">
				<template #[`header.content.append`]>
					<data-system-badge
						:title="Resources.SISTEMA_DE_DADOS_ATU09110" />
				</template>
				
				<dl class="wa-environment">
					<dt>{{ Resources.SERVIDOR_DE_SGBD19838 }}</dt>
					<dd>{{ Model.SGBDServer }}</dd>
					<dt>{{ Resources.SGBD26061 }}</dt>
					<dd>{{ Model.TpSGBD }}</dd>
					<dt>{{ Resources.VERSAO_DO_SGBD43730 }}</dt>
					<dd>{{ Model.SGBDVersion }}
						<q-icon
							v-if="Model.HasSGBDVersion"
							icon="alert-circle" />
					</dd>
					<dt>{{ Resources.BASE_DE_DADOS58234 }}</dt>
					<dd>{{ Model.DBSchema }}</dd>
					<dt>{{ Resources.TAMANHO_DA_BD56664 }}</dt>
					<dd>{{ Model.DBSize }} MB</dd>
					<dt class="version">{{ Resources.VERSAO_DA_BD12683 }}</dt>
					<dd class="version">{{ Model.VersionDb }}
						<q-icon
							v-if="Model.HasDiffIdxVersion"
							icon="alert-circle" />
					</dd>
					<dt>{{ Resources.COMPUTADOR39938 }}</dt>
					<dd>{{ Model.PCDesc }}</dd>
					<dt>{{ Resources.SISTEMA_OPERATIVO30480 }}</dt>
					<dd>{{ Model.SODesc }}</dd>
					<dt>{{ Resources.PROCESSADOR36325 }}</dt>
					<dd>{{ Model.HardwProcDesc }}</dd>
					<dt>{{ Resources.MEMORIA09056 }}</dt>
					<dd>{{ Model.HardwMemDesc }}</dd>
					<dt>{{ Resources.DRIVES34119 }}</dt>
					<dd>
						<span v-html="Model.HardwDrivDesc"></span>
					</dd>
				</dl>
			</q-card>
		</div>
	</div>
</template>

<script>
// @ is an alias to /src
import { reusableMixin } from '@/mixins/mainMixin';
import { QUtils } from '@/utils/mainUtils';

export default {
	name: 'dashboard',
	mixins: [reusableMixin],

	data() {
		return {
			loaded: false,
			Model: {},
			CurrentMaintenance: {}
		};
	},

	computed: {
		maintenanceBtnText() {
			return this.CurrentMaintenance.IsActive 
				? this.Resources.DESACTIVAR_MANUTENCA45568 
				: this.Resources.MUDAR_AGENDAMENTO_DE08195;
		},

		maintenanceText() {
			return this.CurrentMaintenance.IsActive 
				? this.Resources.O_SISTEMA_ENCONTRA_S37912 
				: this.Resources.O_SISTEMA_IRA_ENTRAR46754.replace('{0}', this.formatDate(this.CurrentMaintenance.Schedule));
		},
	},

	methods: {
		fetchData() {
			QUtils.log("Fetch data - Dashboard");
			QUtils.FetchData(QUtils.apiActionURL('Dashboard', 'Index')).done((data) => {
				QUtils.log("Fetch data - OK (Dashboard)", data)
				Object.entries(data.model).forEach(([propName, value]) => { this.Model[propName] = value; })
				if (!this.Model.HasConfig) {
					this.navigateTo(null, 'no_configuration', this.hasSubmenu);
				}
				Object.entries(data.CurrentMaintenance).forEach(([propName, value]) => {  this.CurrentMaintenance[propName] = value; })
				this.loaded = true;
			});
		},
		showDBButton() {
			return (this.Model.HasDiffVersion || this.Model.VersionDb != -1) && this.Model.HasValidConfig
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
