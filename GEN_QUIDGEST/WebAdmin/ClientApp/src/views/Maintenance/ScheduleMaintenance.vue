<template>
	<div id="maintenance_schedule_container">
		<q-card
			class="q-card--admin-default"
			:title="Resources.AGENDAR_A_MANUTENCAO08879"
			width="block">
			<q-row-container>
				<datetime-picker v-model="scheduleDT" ref="scheduleDT" v-if="showScheduleDT"></datetime-picker>
				<span class="q-help__subtext">
					<q-icon icon="information-outline" />
					{{ Resources.DEIXAR_VAZIO_PARA_LI30681 }}
				</span>

				<row class="footer-btn">
					<q-button
						variant="bold"
						:label="Resources.CONFIRMAR09808"
						@click="ScheduleMaintenance" />
				</row>
			</q-row-container>

		</q-card>
	</div>
</template>

<script>
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import moment from 'moment';

	export default {
		name: 'schedule_maintenance',

		mixins: [reusableMixin],

		data() {
			return {
				showScheduleDT: true,
				scheduleDT: moment(),
				CurrentMaintenance: {},
			}
		},

		computed: {
			maintenanceTitleText() {
				var vm = this;
				return vm.CurrentMaintenance.IsActive ? vm.Resources.DESACTIVAR_MANUTENCA45568 :
					(vm.CurrentMaintenance.IsScheduled ? vm.Resources.MUDAR_AGENDAMENTO_DE08195 : vm.Resources.AGENDAR_MANUTENCAO19112);
			}
		},

		methods: {
			ScheduleMaintenance() {
				var vm = this;
				if (!vm.scheduleDT || vm.scheduleDT === '' || vm.scheduleDT === null || vm.scheduleDT === undefined) {
					QUtils.postData('Dashboard', 'DisableMaintenance', null, null, function (data) {
						QUtils.log("DisableMaintenance - Response", data);
						$.each(data.CurrentMaintenance, function (propName, value) { vm.CurrentMaintenance[propName] = value; });
						vm.$emit('alert-class', { ResultMsg: vm.Resources.MANUTENCAO_DESATIVAD51873, AlertType: 'success' });
					});
				}
				else {
					QUtils.postData('Dashboard', 'ScheduleMaintenance', { date: vm.scheduleDT }, null, function (data) {
						QUtils.log("ScheduleMaintenance - Response", data);
						$.each(data.CurrentMaintenance, function (propName, value) { vm.CurrentMaintenance[propName] = value; });
						vm.$emit('alert-class', { ResultMsg: vm.Resources.MANUTENCAO_CONFIGURA28227, AlertType: 'success' });
					});
				}
			}
		}
	}
</script>
