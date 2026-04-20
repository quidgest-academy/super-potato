<template>
	<div id="system_setup_container">
		<div class="q-stack--column">
			<h1 class="f-header__title">
				{{ hardcodedTexts.databaseLabel }}
			</h1>
		</div>
		<hr />

		<QAlert
			v-if="alert.isVisible"
			ref="alertBox"
            :type="alert.alertType"
            :text="alert.message"
            :icon="alert.icon"
			:title="systemConfigTexts.estadoDaOperacao"
			:dismissTime="4"
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
								<component :is="tab.componentId" v-if="tab.props"  v-bind="tab.props" v-on="tab.events || {}"></component>
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
	import { reactive, computed } from 'vue';
	import database from './System_setup/Database.vue';
	import display from './System_setup/Display.vue';
	import externalservices from './System_setup/ExternalServices.vue';
	import integration from './System_setup/Integration.vue';
	import datasystems from './System_setup/DataSystems.vue';
	import system from './System_setup/System.vue';
	import extra from './System_setup/Extra.vue';
	import QAlert from '@/components/QAlert.vue';
	import { texts } from '@/resources/hardcodedTexts.ts';
	import { SystemConfigTexts } from '@/resources/viewResources.ts';

	export default {
		name: 'system_setup',
		mixins: [reusableMixin],
		components: { QAlert, database, display, externalservices, datasystems, integration, system, extra
},

		props: {
			/**
			 * An object containing current Tabs that can trigger different actions within the configuration modal.
			 * These could include showing or hiding the modal, or navigating between different sections of the configuration.
			 */
			currentTab: {
				type: Object,
				default: () => ({})
			},
		},
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
					selectedTab: 'database-tab',
					alignTabs: 'left',
					iconAlignment: 'left',
					isVisible: true,
					tabsList: [
						{
							id: 'database-tab',
							componentId: 'database',
							name: 'database',
							label: '',
							disabled: false,
							isVisible: true,
							props: {
								model: computed(() => vm.Model),
								resources: computed(() => vm.systemConfigTexts)
							},
							events: { 'connection-tested': vm.handleConnectionTested, 'update-model': vm.setModel, 'alert-class': vm.updateAlert }
						},
						{
							id: 'datasystems-tab',
							componentId: 'datasystems',
							name: 'datasystems',
							label: '',
							disabled: false,
							isVisible: true,
							props: {
								model: computed(() => vm.Model),
								resources: computed(() => vm.systemConfigTexts)
							},
							events: { 'changeTab': vm.changeTab, 'alert-class': vm.updateAlert }
						},
						{
							id: 'system-tab',
							componentId: 'system',
							name: 'system',
							label: '',
							disabled: false,
							isVisible: true,
							props: {
								model: computed(() => vm.Model),
								Scheduler: computed(() => vm.Model?.Scheduler),
								TaskList: computed(() => vm.Model?.SelectLists.SchedulerTaskList),
								resources: computed(() => vm.systemConfigTexts)
							},
							events: { 'update-model': vm.fetchData, 'alert-class': vm.updateAlert }
						},
						{
							id: 'display-tab',
							componentId: 'display',
							name: 'display',
							label: '',
							disabled: false,
							isVisible: true,
							props: { model: computed(() => vm.Model), SelectLists: computed(() => vm.Model?.SelectLists), resources: computed(() => vm.systemConfigTexts) },
							events: { 'update-model': vm.fetchData, 'alert-class': vm.updateAlert }
						},
						{
							id: 'externalservices-tab',
							componentId: 'externalservices',
							name: 'externalservices',
							label: '',
							disabled: false,
							isVisible: true,
							props: { model: computed(() => vm.Model), Cores: computed(() => vm.Cores), SelectLists: computed(() => vm.Model?.SelectLists), resources: computed(() => vm.systemConfigTexts) },
							events: { 'update-model': vm.fetchData, 'alert-class': vm.updateAlert }
						},
						{
							id: 'integration-tab',
							componentId: 'integration',
							name: 'integration',
							label: '',
							disabled: false,
							isVisible: true,
							props: { model: computed(() => vm.Model), Messaging: computed(() => vm.Model?.Messaging),
								Metadata: computed(() => vm.Model?.MessagingMetadata), reloadMQueues: vm.reloadMQueues, resources: computed(() => vm.systemConfigTexts) },
							events: { 'update-model': vm.fetchData, 'alert-class': vm.updateAlert }
						},
						{
							id: 'extra-tab',
							componentId: 'extra',
							name: 'extra',
							label: '',
							disabled: false,
							isVisible: true,
							props: { model: computed(() => vm.Model), SelectLists: computed(() => vm.Model?.SelectLists), resources: computed(() => vm.systemConfigTexts) },
							events: { 'update-model': vm.fetchData, 'alert-class': vm.updateAlert }
						},
					]
				}
			};
		},
		computed: {
			Paths() {
				if (!this.currentApp || !this.Model.Paths || Object.keys(this.Model.Paths).length === 0)
				return null;
				this.Model.Paths[this.currentApp].app = this.currentApp;
				return this.Model.Paths[this.currentApp] || null;
			},
			Cores() {
				return this.currentApp && this.Model.Cores ? this.Model.Cores[this.currentApp] : null;
			},
			hardcodedTexts() {
				return {
					databaseLabel: this.Resources[texts.databaseLabel],
					systemLabel: this.Resources[texts.systemLabel],
				};
			},
			systemConfigTexts() {
				return new SystemConfigTexts(this);
			},
		},
		methods: {
			fetchData() {
				QUtils.log("Fetch data - Config", QUtils.apiActionURL('Config', 'Index'));
				QUtils.FetchData(QUtils.apiActionURL('Config', 'Index')).done( (data) => {
					QUtils.log("Fetch data - OK (Config)", data);
					if(data.redirect) {
						this.$router.replace({ name: data.redirect, params: { culture: this.currentLang, system: this.currentYear } });
					}
					else if (data.reload) {
						this.currentYear = data.system;
						this.fetchData();
					}
					else {
						this.setModel(data);
					}
				});
			},
			setModel(data) {
				if (data.SelectLists.PropertyList) {
					for (var item in data.SelectLists.PropertyList) {
						let listProperties = data.SelectLists.PropertyList[item]

						if (listProperties.TextResourceId)
							data.SelectLists.PropertyList[item].translatedLabel = computed(() => this.Resources[listProperties.TextResourceId])
						else
							data.SelectLists.PropertyList[item].translatedLabel = listProperties.Text

						if (listProperties.TextHelpResourceId)
							data.SelectLists.PropertyList[item].translatedHelp = computed(() => this.Resources[listProperties.TextHelpResourceId])
						else
							data.SelectLists.PropertyList[item].translatedHelp = undefined

						if (listProperties.TextHelpResourceVerboseId)
							data.SelectLists.PropertyList[item].translatedHelpVerbose = computed(() => this.Resources[listProperties.TextHelpResourceVerboseId])
						else
							data.SelectLists.PropertyList[item].translatedHelpVerbose = undefined
					}
				}

				this.Model = { ...data };
				// Select the first exists application
				if (!this.currentApp && Array.isArray(this.Model.Applications) && this.Model.Applications.length) {
					this.currentApp = this.Model.Applications[0].Id;
				}
				// Focus on errors div
				if (this.Model.ResultMsg) {
					window.scrollTo(0,0);
					this.updateAlert(data);
				}
			},
			reloadMQueues() {
				QUtils.FetchData(QUtils.apiActionURL('Config', 'ReloadMQueues')).done((data) => {
					if (data.Success) {
						$.each(data.MQueues, (propName, value) => {
							if ($.isArray(this.Model.MQueues[propName])) { this.Model.MQueues[propName].splice(0); }
							$.extend(this.Model.MQueues[propName], value);
						});
					}
				});
			},
			updateUsers(eventData) {
				if (!Object.keys(this.Model.Security[eventData.currentApp].Users).length)
					$.extend(this.Model.Security[eventData.currentApp], reactive({ Users: [] }));
				else
					this.Model.Security[eventData.currentApp].Users.splice(0);

				$.extend(this.Model.Security[eventData.currentApp].Users, eventData.users);
			},

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
			handleConnectionTested(result) {
				if (result.Success) {
					this.setAlert('success', this.hardcodedTexts.connectionSuccess);
				} else {
					this.setAlert('danger', result.message || this.hardcodedTexts.connectionFailed);
				}
			},
			setAlert(type, message) {
				this.alert.isVisible = true;
				this.alert.alertType = type;
				this.alert.message = message;

				this.$nextTick(() => {
					if (this.$refs.alertBox) {
						this.$refs.alertBox.$el.scrollIntoView({ behavior: 'smooth' });
					}
				});
			},
			handleAlertDismissed() {
				this.alert.isVisible = false;
			}
		},
		mounted() {
			this.tabGroup.tabsList[0].label = this.hardcodedTexts.databaseLabel;
			this.tabGroup.tabsList[1].label = this.systemConfigTexts.sistemasDeDados;
			this.tabGroup.tabsList[2].label = this.hardcodedTexts.systemLabel;
			this.tabGroup.tabsList[3].label = this.systemConfigTexts.definicoesDoEcra;
			this.tabGroup.tabsList[4].label = this.systemConfigTexts.iaEServicosExternos;
			this.tabGroup.tabsList[5].label = this.systemConfigTexts.integracao;
			this.tabGroup.tabsList[6].label = this.systemConfigTexts.propriedadesExtra;

			this.observer = new MutationObserver(mutations => {
				for (const m of mutations) {
					const newValue = m.target.getAttribute(m.attributeName);
					this.$nextTick(() => {
						if (newValue.indexOf('active')) {
						this.selectedTab = m.target.id;
						}
					});
				}
			});
		},
		created() {
			// Ler dados
			this.fetchData();
		},
		watch: {
			// call again the method if the route changes
			'$route': 'fetchData',
			'currentApp': 'fetchData',
			currentTab: {
				handler(newValue) {
					if (newValue.selectedTab) {
						this.changeTab('tabGroup', 'selectedTab', newValue.selectedTab)
					}
				},
				deep: true
			}

		}
	};
</script>
