<template>
	<div id="maintenance_change_year_container">
		<row>
			<q-card
				class="q-card--admin-default"
				:title="Resources.AUTENTICACAO37999"
				width="block">
				<q-row-container>
					<q-control-wrapper class="control-row-group">
						<base-input-structure
							class="i-text">
							<text-input
								v-model="Model.DbUser"
								:label="Resources.BASE_DE_DADOS58234+' '+Resources.NOME_DE_UTILIZADOR58858"
								is-required
								size="xlarge" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="control-row-group">
						<base-input-structure
							class="i-text">
							<password-input
								v-model="Model.DbPsw"
								:label="Resources.PALAVRA_PASSE44126"
								is-required
								size="xlarge" />
						</base-input-structure>
					</q-control-wrapper>
					<q-card
						class="q-card--admin-border-top q-card--admin-compact"
						:title="Resources.AUTENTICACAO37999"
						variant="minor"
						width="block">
						<q-row-container>
							<q-control-wrapper
								v-if="Model.Years"
								class="control-row-group">
								<base-input-structure
									class="i-text">
									<q-select
										v-model="Model.SrcYear"
										item-value="Value"
										item-label="Text"
										:items="Model.Years"
										:label="'Source database'"
										clearable
										size="large" />
								</base-input-structure>
							</q-control-wrapper>
							<q-control-wrapper class="control-row-group">
								<base-input-structure
									class="i-text">
									<text-input
										v-model="Model.NewDBSchema"
										:label="Resources.NOME_DA_BASE_DE_DADO06329" />
								</base-input-structure>
							</q-control-wrapper>
							<q-control-wrapper class="control-row-group">
								<base-input-structure
									class="i-text">
									<q-checkbox
										v-model="Model.CriarBD"
										:label="Resources.CRIAR_A_BASE_DE_DADO55641" />
								</base-input-structure>
							</q-control-wrapper>
							<q-control-wrapper class="control-row-group">
								<base-input-structure
									class="i-text">
									<text-input
										v-model="Model.Year"
										:label="Resources.ANO33022" />
								</base-input-structure>
							</q-control-wrapper>
							<q-control-wrapper class="control-row-group">
								<base-input-structure
									class="i-text">
									<text-input
										v-model="Model.DirFilestream"
										:label="Resources.DIRETORIA_DE_FILESTR39886" />
								</base-input-structure>
							</q-control-wrapper>
							<q-control-wrapper class="control-row-group">
								<text-input v-model="Model.Timeout"
											:label="'Timeout'" />								
							</q-control-wrapper>

						<row class="footer-btn">
							<q-button
								variant="bold"
								:label="Resources.INICIAR_O_PROCESSO_D34168"
								@click="changeYear" />
						</row>
						</q-row-container>
					</q-card>
					<progress-bar
						:show="dataPB.show"
						:text="dataPB.text"
						:progress="dataPB.progress" />
				</q-row-container>
			</q-card>
		</row>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import bootbox from 'bootbox';

	export default {
		name: 'maintenance_change_year',

		mixins: [reusableMixin],

		data() {
			return {
				Model: {},
				Errors: [],
				dataPB: {
				show: false,
				text: '',
				progress: 0,
				inProcess: false
				}
			};
		},

		methods: {
			fetchData() {
				var vm = this;
				QUtils.log("Fetch data - Maintenance - Change Year");
				QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'ChangeYear')).done(function (data) {
					QUtils.log("Fetch data - OK (Maintenance - Change Year)", data);
					$.each(data.Model, function (propName, value) { vm.Model[propName] = value; });
					vm.Errors = data.Errors;
				});
			},

			changeYear() {
				var vm = this;
				QUtils.log("Request", QUtils.apiActionURL('DbAdmin', 'ChangeYear'));
				QUtils.postData('DbAdmin', 'ChangeYear', vm.Model, null, function (data) {
					QUtils.log("Response", data);
					$.each(data.Model, function (propName, value) { vm.Model[propName] = value; });
					vm.Errors = data.Errors;
					if (vm.Errors.length > 0) {
						vm.$emit('alert-class', { ResultMsg: vm.Errors, AlertType: data.AlertType || 'danger' });
					} else {
						vm.$emit('alert-class', { message: "Year change process completed successfully", AlertType: 'success' });
					}

					setTimeout(vm.checkProgress, 250);
				});
			},

			checkProgress() {
				// This function is the callback function which is executed in every 350 milli seconds.
				// Until the ajax call is success this method is invoked and the progressbar value is changed.
				var vm = this;
				QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'CheckChangeYearProgress')).done(function (data) {
				if (data.InProcess) {
					vm.dataPB.inProcess = true;
					vm.dataPB.text = data.Text;
					vm.dataPB.progress = data.Percent;
					vm.dataPB.show = true;
					setTimeout(vm.checkProgress, 250);
				}
				else {
					if (vm.dataPB.inProcess && !$.isEmptyObject(data.EndMsg)) {
					bootbox.alert(data.EndMsg);
					}
					vm.dataPB = {
					show: false,
					text: '',
					progress: 0,
					inProcess: false
					};
					vm.$eventHub.emit('app_updateYear');
				}
				});
			}
		},

		mounted() {
			setTimeout(this.checkProgress, 250);
		},

		created() {
			// Ler dados
			this.fetchData();
		}
	};
</script>
