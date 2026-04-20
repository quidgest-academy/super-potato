<template>
	<div id="maintenance_indexes_container">    
		<div class="q-help__info-banner">
			<div class="q-help__info-banner-header">
				<q-icon icon="information-outline" />
				<h5 for="ResultMsg">{{ Resources.DIRETRIZES_DE_INDICE35108 }}</h5>
			</div>
			<div class="q-help__info-banner-body">
				<span style="white-space: pre-line">
					{{ Resources.OS_INDICES_DO_SQL_SE63503 }}<br>
					{{ Resources.OS_INDICES_DO_SQL_SE32865 }}<br>
					{{ Resources.AS_OPERACOES_INSERT_62844 }}<br>
					{{ Resources.COMO_RESULTADO__ISSO39011 }}<br>
					{{ Resources.PARA_GRANDES_BANCOS_18723 }}<br>
					{{ Resources.UM_OBJETIVO_CRITICO_20198 }}<br>
					<br>
					{{ Resources.NO_ENTANTO__OS_DADOS11897 }}<br>
					{{ Resources.PORTANTO__E_ESSENCIA34291 }}<br>
					{{ Resources.OS_OPERADORES_SEEKS_42773 }}<br>
					<hr>
					<b class="status-info">{{ Resources.DICAS_DE_COLUNAS_19049 }}</b><br>
					<b>{{ Resources.SEEKS08850 }}</b> {{ Resources.__RECUPERA_APENAS_AS44382 }}<br>
					<b>{{ Resources.SCANS04847 }}</b> {{ Resources.__RECUPERA_TODAS_AS_39508 }}<br>
					<b>{{ Resources.LOOKUPS46147 }}</b> {{ Resources.__RECUPERA_INFORMACO07745 }}<br>
					<b>{{ Resources.MELHORIA____10458 }}</b> {{ Resources.__BENEFICIO_QUE_AS_C02927 }}<br>
					<b>{{ Resources.IMPACTO36269 }}</b> {{ Resources.__MELHORIA_MULTIPLIC23681 }}
				</span>
			</div>
		</div>

		<br />

		<maintenance_dbindexes v-for="item in Model.Indexes" :key="item.IndexType" :Model="item" @updateData="updateData"></maintenance_dbindexes>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import maintenance_dbindexes from './DbIndexes.vue';

	export default {
		name: 'maintenance_indexes',
		mixins: [reusableMixin],
		components: { maintenance_dbindexes },
		data() {
			return {
				Model: {}
			};
		},
		methods: {
			fetchData() {
				var vm = this;
				QUtils.log("Fetch data - Maintenance - Indexes");
				QUtils.FetchData(QUtils.apiActionURL('dbadmin', 'Indexes')).done(function (data) {
				QUtils.log("Fetch data - OK (Maintenance - Indexes)", data);
				$.each(data, function (propName, value) { vm.Model[propName] = value; });
				});
			},
			updateData(data) {
				var vm = this;
				if($.isEmptyObject(data)) {
				vm.fetchData();
				}
				else {
				$.each(data, function (propName, value) { vm.Model[propName] = value; });
				}
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
