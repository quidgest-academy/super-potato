<template>
	<div id="maintenance_container">
		<div class="q-stack--column">
			<h1 class="f-header__title">
				{{ Resources.MANUTENCAO_DA_BASE_D10092 }}
			</h1>
		</div>
		<hr>
		<q-alert
			v-if="alert.isVisible"
            :type="alert.alertType"
            :text="alert.message"
            :icon="alert.icon"
            :dismissTime="5"
            @message-dismissed="handleAlertDismissed" />
		<div>
			<QTabContainer
				v-bind="tabGroup"
				@tab-changed="changeTab('tabGroup', 'selectedTab', $event)">
				<template #tab-panel>
					<template
						v-for="tab in tabGroup.tabsList"
						:key="tab.id">
							<div v-if="tabGroup.selectedTab === tab.id" class="tab-pane c-tab__item-content" :id="tab.componentId">
								<component :is="tab.componentId" v-bind="tab.props" v-on="tab.events || {}"></component>
							</div>
					</template>
				</template>
			</QTabContainer>
		</div>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import moment from 'moment';
	import bootbox from 'bootbox';
	import { computed } from 'vue';
	import maintenance_index from './Maintenance/Index.vue';
	import maintenance_backup from './Maintenance/Backup.vue';
	import maintenance_security from './Maintenance/Security.vue';
	import maintenance_indexes from './Maintenance/Indexes.vue';
	import maintenance_data_quality from './Maintenance/DataQuality.vue';
	import maintenance_change_year from './Maintenance/ChangeYear.vue';
	import QAlert from '@/components/QAlert.vue';
	import schedule_maintenance from './Maintenance/ScheduleMaintenance.vue';

	export default {
		name: 'maintenance',
		components: {
			QAlert,
			maintenance_index,
			maintenance_backup,
			maintenance_security,
			maintenance_indexes,
			maintenance_data_quality,
			maintenance_change_year,
			schedule_maintenance,
		},
		mixins: [reusableMixin],
		data() {
			var vm = this
			return {
				Model: {},
				alert: {
					isVisible: false,
					alertType: 'info',
					message: ''
				},
				tabGroup: {
					selectedTab: 'index-tab',
					alignTabs: 'left',
					iconAlignment: 'left',
					isVisible: true,
					tabsList: [
						{
							id: 'index-tab',
							componentId: 'maintenance_index',
							name: 'index',
							label: vm.$t('MANUTENCAO49776'),
							disabled: false,
							isVisible: true,
							events: { 'alert-class': vm.updateAlert }
						},
						{
							id: 'backup-tab',
							componentId: 'maintenance_backup',
							name: 'backup',
							label: vm.$t('BACKUP51008'), 
							disabled: false,
							isVisible: true,
							events: { 'alert-class': vm.updateAlert }
						},
						{
							id: 'security-tab',
							componentId: 'maintenance_security',
							name: 'security',
							label: vm.$t('SEGURANCA53664'),
							disabled: false,
							isVisible: true,
							events: { 'alert-class': vm.updateAlert }
						},
						{
							id: 'indexes-tab',
							componentId: 'maintenance_indexes',
							name: 'indexes',
							label: vm.$t('INDICES58021'),
							disabled: false,
							isVisible: true,
							events: { 'alert-class': vm.updateAlert }
						},
						{
							id: 'data_quality-tab',
							componentId: 'maintenance_data_quality',
							name: 'data_quality',
							label: vm.$t('QUALIDADE_DE_DADOS10588'),
							disabled: false,
							isVisible: true,
							events: { 'alert-class': vm.updateAlert }
						},
						{
							id: 'change_year-tab',
							componentId: 'maintenance_change_year',
							name: 'change_year',
							label: vm.$t('MUDANCA_DE_ANO09709'),
							disabled: false,
							isVisible: true,
							events: { 'alert-class': vm.updateAlert }
						},
						{
							id: 'schedule_maintenance-tab',
							componentId: 'schedule_maintenance',
							name: 'schedule_maintenance',
							label: vm.$t('AGENDAR_MANUTENCAO19112'),
							disabled: false,
							isVisible: true,
							events: { 'alert-class': vm.updateAlert }
						}
					]
				}
			};
		},
		beforeRouteEnter(to, from, next) {
			next(vm => {
				// Check if the route has a 'tab' query parameter
				if (to.params.tabId) {
					// Set the tabGroup.selectedTab to the one passed in the params
					vm.tabGroup.selectedTab = to.params.tabId;
				}
			});
		},
		methods: {
			getTab(tab, selectedTab) {
				return _find(this[tab]['tabsList'], (x) => x.id === selectedTab)
			},

			changeTab(tab, tabProp, selectedTab) {
				this[tab][tabProp] = selectedTab
			},
			updateAlert(data) {
				this.Model.ResultMsg = data.ResultMsg;
				if (data.AlertType) {
					this.setAlert(data.AlertType, data.ResultMsg);
				} else {
					this.setAlert('info', data.ResultMsg);
				}
			},
			setAlert(type, message) {
				this.alert.isVisible = true;
				this.alert.alertType = type;
				this.alert.message = message;
			},
			handleAlertDismissed() {
				this.alert.isVisible = false;
			}
		},

		mounted() {
			var vm = this;
			vm.observer = new MutationObserver(mutations => {
				for (const m of mutations) {
					const newValue = m.target.getAttribute(m.attributeName);
					vm.$nextTick(() => {
						if (newValue && newValue.indexOf('active') > -1) {
							vm.activeTab = m.target.id;
						}
					});
				}
			});

			// Ensure the refs are available by waiting until after they're rendered
			vm.$nextTick(() => {
				Object.keys(vm.$refs).forEach(ref => {

					let element = vm.$refs[ref];

					// If the ref is a Vue component, use the root element
					if (element && element.$el) {
						element = element.$el;
					}

					// Check if the element is a DOM node
					if (element && element.nodeType === 1) {
						vm.observer.observe(element, {
							attributes: true,
							attributeFilter: ['class'],
						})
					}
				});
			});
		},
		
		beforeUnmount() {
			this.observer.disconnect();
		}
	};
</script>
