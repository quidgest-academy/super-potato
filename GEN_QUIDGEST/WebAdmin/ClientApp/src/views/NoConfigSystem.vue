<template>
	<div id="dashboard_container">
		<div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.CONFIGURACOES19326 }}
			</h1>
		</div>
		<hr />
		<div v-if="!loaded" class="card text-center">
			<div class="card-body">
				<q-spinner-loader id="tab-loader" />
				<h4>{{ Resources.A_CARREGAR___34906 }}</h4>
			</div>
		</div>
		<div v-else>
			<div v-if="!Model.HasConfig" class="alert alert-danger">
				<h4>{{ Resources.NO_CONFIGURATION_FIL40493 }}</h4>
				<p><span v-html="Model.ResultErrors"></span></p>
				<q-button
					:label="Resources.CRIAR_CONFIGURACAO_D17273"
					@click.stop="createNewConfig" />
			</div>
			<br>

			<div v-if="Model.IsBetaTesting" class="alert alert-warning">
				<p></p>
				<p><b>{{ Resources.AMBIENTE_DE_QUALIDAD42119 }}</b></p>
			</div>
		</div>
	</div>
</template>

<script>
// @ is an alias to /src
import { reusableMixin } from '@/mixins/mainMixin';
import { QUtils } from '@/utils/mainUtils';
import bootbox from 'bootbox';
import moment from 'moment';

export default {
	name: 'no_configuration',
	mixins: [reusableMixin],
	data() {
		var vm = this;
		return {
			loaded: false,
			Model: {},
			modules: [],
			style: {
				dtClass: 'col-sm-2 textRight',
				ddClass: 'col-sm-10'
			},
			UsersCount: 0,
			queryParams: {
				sort: [],
				filters: [],
				global_search: "",
				per_page: 10,
				page: 1,
				component: "user",
			}
		};
	},
	methods: {
		fetchData() {
			var vm = this;
			QUtils.log("Fetch data - Dashboard");
			QUtils.FetchData(QUtils.apiActionURL('Dashboard', 'Index')).done(function (data) {
				QUtils.log("Fetch data - OK (Dashboard)", data);
				$.each(data.model, function (propName, value) { vm.Model[propName] = value; });
				vm.loaded = true;
			});
		},
		needsBasicConfig() {
			return !this.Model.HasConfig || (this.Model.DBSize === 0 && this.Model.VersionDb === 0) || this.UsersCount === 0
		},
		showDBButton() {
			return this.Model.HasDiffVersion || this.Model.VersionDb != -1
		},
		createNewConfig() {
			var vm = this;
			//Call method that creates a configuration file
			QUtils.postData('Dashboard', 'CreateConfiguration', null, null, function (data) {
					if (data.Success) {
						bootbox.alert(vm.Resources.NEW_CONFIGURATION_CR61652);
						vm.navigateTo(null, 'system_setup');
					}
					else {
						bootbox.alert(vm.Resources.THERE_WAS_AN_ERROR_C44163 + "<br>" + data.Message);
					}
					vm.fetchData();
			});
		}
	},
	created() {
		this.modules = [];
		this.modules.push({
			Codiprog: 'FOR',
			Prog: 'My application',
			Platafor: 'VUE',
			Vate: '01/01/0001'
		});
		// Ler dados
		this.fetchData();
	},
	watch: {
		// call again the method if the route changes
		'$route': 'fetchData'
	}
};
</script>
