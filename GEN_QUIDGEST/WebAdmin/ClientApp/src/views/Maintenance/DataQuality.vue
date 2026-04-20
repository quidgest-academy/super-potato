<template>
	<div id="maintenance_data_quality_container">
		<maintenance_data_quality_inconsistent_relations v-for="item in Model.Incoherencies" :key="item.IncoherenceType" :Model="item" @updateData="updateData"></maintenance_data_quality_inconsistent_relations>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import maintenance_data_quality_inconsistent_relations from './InconsistentRelations.vue';

	export default {
		name: 'maintenance_data_quality',
		mixins: [reusableMixin],
		components: { maintenance_data_quality_inconsistent_relations },
		data() {
			return {
				Model: {}
			};
		},
		methods: {
			fetchData() {
				var vm = this;
				QUtils.log("Fetch data - Maintenance - Data Quality");
				QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'DataQuality')).done(function (data) {
					QUtils.log("Fetch data - OK (Maintenance - Data Quality)", data);
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
