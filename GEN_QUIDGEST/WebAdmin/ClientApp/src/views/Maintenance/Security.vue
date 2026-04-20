<template>
	<div id="maintenance_security_container">
		<row>
			<q-card
				class="q-card--admin-default"
				:title="Resources.AUTENTICACAO37999"
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
				</q-row-container>
			</q-card>
		</row>
		<row>
			<q-card
				class="q-card--admin-default"
				title="Transparent Data Encryption"
				width="block">
				<q-row-container>
					<div class="q-help__info-banner">
						<div class="q-help__info-banner-header">
							<q-icon icon="information-outline" />
							<h5 for="ResultMsg">Database Encryption Key (DEK)</h5>
						</div>
						<div class="q-help__info-banner-body">
							<span style="white-space: pre-line">
								{{ Resources.E_UMA_CHAVE_USADA_PE64608 }}<br>
								<b>{{ Resources.COMO_FUNCIONA_41713 }}</b><br><br>
								<b>{{ Resources._1__CRIACAO_DO_DEK_32052 }}</b>
								{{ Resources.UM_DEK_E_CRIADO_DENT62777 }}<br>
								<b>{{ Resources._2__PROTECAO_DO_DEK_44233 }}</b>
								{{ Resources.O_DEK_E_PROTEGIDO_PO18921 }}<br>
								<b>{{ Resources._3__CRIPTOGRAFIA_DESC36757 }}</b>
								{{ Resources.AO_ABRIR_O_BANCO_DE_49156 }}<br><br>
								<b>{{ Resources.IMPORTANTE_LEMBRAR_30050 }}</b><br><br>
								{{ Resources.__O_DEK_NAO_PODE_SER11689 }}<br>
								{{ Resources.__SEM_O_CERTIFICADO_09927 }}
							</span>
						</div>
					</div>
					<q-control-wrapper class="control-row-group">
						<base-input-structure
							class="i-text">
							<password-input v-model="Model.MasterPsw" :label="Resources.CHAVE_MESTRA09773" is-required></password-input>
							<span class="q-help__subtext">
								<q-icon icon="information-outline" />
								{{ Resources.A_SENHA_DEVE_TER_ENT28631 }}
							</span>
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="control-row-group">
						<base-input-structure
							class="i-text">
							<q-select
								v-model="Model.Encryption"
								v-if="Model.SelectLists"
								item-value="Value"
								item-label="Text"
								:items="Model.SelectLists.DisplayEncrypt"
								:label="Resources.ALGORITMO_DE_ENCRIPT09649"
								required />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="control-row-group">
						<base-input-structure
							class="i-text">
							<q-checkbox
								v-model="Model.MasterKey"
								:label="Resources.CRIACAO_DA_CHAVE_MES19380" />
						</base-input-structure>
					</q-control-wrapper>

					<row class="footer-btn">
						<q-button
							variant="bold"
							:label="Resources.GRAVAR_CONFIGURACAO36308"
							@click="SaveTDE" />
						<q-button
							:label="Resources.STATUS62033"
							@click="CheckStatusTDE" />

						<data-system-badge
							:title="Resources.SISTEMA_DE_DADOS_ATU09110" />
					</row>
				</q-row-container>
			</q-card>
		</row>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';

	export default {
		name: 'maintenance_security',

		mixins: [reusableMixin],

		data() {
			return {
				Model: {}
			};
		},

		methods: {
			fetchData() {
				var vm = this;
				QUtils.log("Fetch data - Maintenance - Secuity");
				QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'Security')).done(function (data) {
				QUtils.log("Fetch data - OK (Maintenance - Secuity)", data);
				$.each(data, function (propName, value) { vm.Model[propName] = value; });
				});
			},

			SaveTDE() {
				this.submitAction('SaveTDE');
			},

			CheckStatusTDE() {
				this.submitAction('CheckStatusTDE');
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
				});
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
