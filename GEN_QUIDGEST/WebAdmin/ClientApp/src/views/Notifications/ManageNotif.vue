<template>
	<div id="notifications_notif_container">
		<div class="q-stack--column">
			<h1 class="f-header__title">
				{{ Resources.CONFIGURACAO_DA_MENS64174 }}
			</h1>
		</div>
		<hr>
		<QTabContainer
			v-if="!isEmptyObject(Model)"
			v-bind="tabGroup"
			@tab-changed="changeTab('tabGroup', 'selectedTab', $event)">
			<template #tab-panel>
				<template
				v-for="tab in tabGroup.tabsList"
				:key="tab.id">
					<div v-if="tabGroup.selectedTab === tab.id" class="tab-pane c-tab__item-content" :id="tab.componentId">
						<component :is="tab.componentId" v-if="tab.props.model"  v-bind="tab.props" v-on="tab.events || {}"></component>
					</div>
				</template>
			</template>
		</QTabContainer>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import bootbox from 'bootbox';
	import { reactive, computed } from 'vue';
	import notifsDB from './NotifsBD.vue';
	import messagesConfig from './MessagesConfig.vue';

	export default {
		name: 'notifications_notif',
		components: {
			notifsDB, messagesConfig
		},
		mixins: [reusableMixin],
		data() {
			var vm = this;
			return {
				Model: {},
				tNotifsOnBD: {
                    rows: [],
                    total_rows: 0,
                    columns: [],
                    config: {
                        table_title: () => vm.$t('MENSAGENS_NA_BD02187')
                    }
                },
				tMessagesConfig: {
					rows: [],
					total_rows: 0,
					columns: [{
						label: () => vm.$t('ACOES22599'),
						name: "actions",
						slot_name: "actions",
						sort: false,
						column_classes: "thead-actions",
						row_text_alignment: 'text-center',
						column_text_alignment: 'text-center'
					},
					{
						label: () => vm.$t('NOME47814'),
						name: "ValDesignac",
						sort: true,
						initial_sort: true,
						initial_sort_order: "asc"
					},
					{
						label: () => vm.$t('REMETENTE47685'),
						name: "ValFrom",
						sort: true
					},
					{
						label: () => vm.$t('DESTINATARIO22298'),
						name: "ValTo",
						sort: true,
						slot_name: 'ValTo'
					},
					{
						label: () => vm.$t('ASSUNTO16830'),
						name: "ValAssunto",
						sort: true
					},
					{
						label: () => vm.$t('MENSAGEM32641'),
						name: "ValMensagem",
						sort: true
					},
					{
						label: () => vm.$t('ATIVO_00196'),
						name: "ValAtivo",
						sort: true,
						slot_name: 'ValAtivo',
						row_text_alignment: 'text-center'
					},
					{
						label: () => vm.$t('ENVIA_EMAIL_46551'),
						name: "ValEmail",
						sort: true,
						slot_name: 'ValEmail',
						row_text_alignment: 'text-center'
					},
					{
						label: () => vm.$t('GRAVA_NA_BD_43814'),
						name: "ValGravabd",
						sort: true,
						slot_name: 'ValGravabd',
						row_text_alignment: 'text-center'
					}],
					config: {
					table_title: () => vm.$t('CONFIGURACAO_DE_MENS13340')
					}
				},
				tabGroup: {
					selectedTab: 'bd-tab',
					alignTabs: 'left',
					iconAlignment: 'left',
					isVisible: true,
					tabsList: [
					{
						id: 'bd-tab',
						componentId: 'notifsDB',
						name: 'bd',
						label: vm.$t('MENSAGENS_NA_BD02187'),
						disabled: false,
						isVisible: true,
						props: { model: computed(() => vm.Model), tNotifsOnBD: computed(() => vm.tNotifsOnBD) },
						events: { 'update-model': vm.fetchData }
					},
					{
						id: 'msgs-tab',
						componentId: 'messagesConfig',
						name: 'msgs',
						label: vm.$t('CONFIGURACAO_DE_MENS13340'), 
						disabled: false,
						isVisible: true,
						props: { model: computed(() => vm.Model), tMessagesConfig: computed(() => vm.tMessagesConfig) },
						events: { 'update-model': vm.fetchData }
					}]
				}
			};
		},
		methods: {
			fetchData() {
				var vm = this,
				mod = vm.$route.params.mod,
				idnotif = vm.$route.params.idnotif;
				QUtils.log("Fetch data - ManageNotif");
				QUtils.FetchData(QUtils.apiActionURL('Notifications', 'ManageNotif', { mod, idnotif })).done(function (data) {
					QUtils.log("Fetch data - OK (ManageNotif)", data);

					if (data.Success) {
                        $.each(data.model, function (propName, value) { vm.Model[propName] = value; });

                        vm.tNotifsOnBD.columns = [];
                        $.each(vm.Model.NotifsOnBD_headers, function(i, column) {
							vm.tNotifsOnBD.columns.push({
								label: column,
								name: column.toLowerCase(),
								sort: true
							});
                        });
                        vm.tNotifsOnBD.rows = vm.Model.NotifsOnBD_body || [];
                        vm.tNotifsOnBD.total_rows = vm.tNotifsOnBD.rows.length

						vm.tMessagesConfig.rows = vm.Model.MessagesConfig || [];
						vm.tMessagesConfig.total_rows = vm.tMessagesConfig.rows.length
                    }
                    else if (data.redirect) {
                        vm.$router.replace({ name: data.redirect });
                    }
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
		beforeUnmount() {
			if (this.observer) {
				this.observer.disconnect();
			}
		},
		mounted() {
			// Ler dados
			this.fetchData();
		},
		watch: {
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
