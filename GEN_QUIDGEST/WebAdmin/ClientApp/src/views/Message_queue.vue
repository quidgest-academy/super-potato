<template>
	<div id="message_queue_container">
		<div class="q-stack--column">
				<h1 class="f-header__title">
				{{ Resources.MESSAGE_QUEUEING34227 }}
				</h1>
			</div>
		<hr>
		<QTabContainer
			v-bind="tabGroup"
			@tab-changed="changeTab('tabGroup', 'selectedTab', $event)">
			<template #tab-panel>
				<template
					v-for="tab in tabGroup.tabsList"
					:key="tab.id">
						<div v-if="tabGroup.selectedTab === tab.id" class="tab-pane c-tab__item-content" :id="tab.componentId">
							<component :is="tab.componentId" v-if="tab.props.model"  v-bind="tab.props"></component>
						</div>
				</template>
			</template>
		</QTabContainer>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin.js';
	import { QUtils } from '@/utils/mainUtils'
	import { computed } from 'vue';
	import dashboard from './Message_queue/Dashboard.vue';
	import queues from './Message_queue/Queues.vue';
	import messages from './Message_queue/Messages.vue';
	import history from './Message_queue/History.vue';
	import statistics from './Message_queue/Statistics.vue';

	export default {
		name: 'message_queue',
		mixins: [reusableMixin],
		components: { dashboard, queues, messages, history, statistics },
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
			var vm = this;
			return {
				Model: { },
				tabGroup: {
					selectedTab: 'queue-tab',
					alignTabs: 'left',
					iconAlignment: 'left',
					isVisible: true,
					tabsList: [
						{
							id: 'dash-tab',
							componentId: 'dashboard',
							name: 'dash',
							label: vm.$t('DASHBOARD51597'),
							disabled: false,
							isVisible: true,
							props: { model: computed(() => vm.Model) },
						},
						{
							id: 'queue-tab',
							componentId: 'queues',
							name: 'queue',
							label: vm.$t('EXPORTACAO_DE_MQ29750'), 
							disabled: false,
							isVisible: true,
							props: { model: computed(() => vm.Model) }
						},
						{
							id: 'db-tab',
							componentId: 'messages',
							name: 'db',
							label: vm.$t('MENSAGENS53948'),
							disabled: false,
							isVisible: true,
							props: { model: computed(() => vm.Model) }
						},
						{
							id: 'hist-tab',
							componentId: 'history',
							name: 'hist',
							label: vm.$t('HISTORICO16073'),
							disabled: false,
							isVisible: true,
							props: { model: computed(() => vm.Model) }
						},
						{
							id: 'stat-tab',
							componentId: 'statistics',
							name: 'stat',
							label: vm.$t('ESTATISTICAS03241'),
							disabled: false,
							isVisible: true,
							props: { model: computed(() => vm.Model) }
						}
					]
				}
			};
		},
		methods: {
			fetchData() {
				var vm = this;
				QUtils.log("Fetch data - Message Queue");
				QUtils.FetchData(QUtils.apiActionURL('MessageQueue', 'Index')).done(function (data) {
					QUtils.log("Fetch data - OK (Message Queue)", data);
					$.each(data, function (propName, value) { vm.Model[propName] = value; });
				});
			},
			getTab(tab, selectedTab) {
				return _find(this[tab]['tabsList'], (x) => x.id === selectedTab)
			},
			changeTab(tab, tabProp, selectedTab) {
				this[tab][tabProp] = selectedTab
			}
		},
		mounted() {
			var vm = this;
			vm.observer = new MutationObserver(mutations => {
				for (const m of mutations) {
				const newValue = m.target.getAttribute(m.attributeName);
				vm.$nextTick(() => {
					if (newValue.indexOf('active')) {
					vm.selectedTab = m.target.id;
					}
				});
				}
			});

			$.each(vm.$refs, function (ref) {
				vm.observer.observe(vm.$refs[ref], {
				attributes: true,
				attributeFilter: ['class'],
				});
			});
		},
		created() {
			// Ler dados
			this.fetchData();
		},
		beforeUnmount() {
			this.observer.disconnect();
		},
		watch: {
			// call again the method if the route changes
			'$route': 'fetchData',
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
