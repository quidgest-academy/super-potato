<template>
	<div>
		<div class="q-stack--column">
				<h1 class="f-header__title">
				{{ Resources.GESTAO_DE_UTILIZADOR20428 }}
				</h1>
			</div>
		<hr>

		<QAlert
			v-if="alert.isVisible"
			ref="alertBox"
			:type="alert.alertType"
			:text="alert.message"
			:icon="alert.icon"
			:title="Resources.ESTADO_DA_OPERACAO38065"
			:dismissTime="5"
			@message-dismissed="handleAlertDismissed" />

		<QTabContainer
			v-bind="tabGroup"
			@tab-changed="changeTab('tabGroup', 'selectedTab', $event)">
			<template #tab-panel>
				<template
				v-for="tab in tabGroup.tabsList"
				:key="tab.id">
					<div v-if="tabGroup.selectedTab === tab.id" class="tab-pane c-tab__item-content" :id="tab.componentId">
					<component :is="tab.componentId" v-on="tab.events || {}"></component>
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
	import roles from './RoleList.vue';
	import Nroles from './UserRoles.vue';
	import allUsers from './AllUsers.vue';
	import QAlert from '@/components/QAlert.vue';

	export default {
		name: 'users',
		mixins: [reusableMixin],
		components: { roles, Nroles, allUsers, QAlert},
		data() {
			return {
			Model: {},
			alert: {
				isVisible: false,
				alertType: 'info',
				message: ''
			},
			tabGroup: {
				selectedTab: 'all-users-tab',
				alignTabs: 'left',
				iconAlignment: 'left',
				isVisible: true,
				tabsList: [
				{
					id: 'all-users-tab',
					componentId: 'allUsers',
					name: 'all-users',
					label: this.$t('UTILIZADORES39761'),
					disabled: false,
					isVisible: true,
					events: { 'alert-class': this.updateAlert }
				},
				{
					id: 'roles-tab',
					componentId: 'roles',
					name: 'roles',
					label: this.$t('ROLES61449'), 
					disabled: false,
					isVisible: true,
					events: { 'alert-class': this.updateAlert }
				},
				{
					id: 'Nroles-tab',
					componentId: 'Nroles',
					name: 'Nroles',
					label: this.$t('GESTAO_DE_FUNCOES_DE46211'),
					disabled: false,
					isVisible: true,
					events: { 'alert-class': this.updateAlert }
				}
				]
			}
			};
		},
		methods: {
			fetchData() {
				QUtils.log("Fetch data - Users");
				QUtils.FetchData(QUtils.apiActionURL('Users', 'Index')).done((data) => {
					if (data.Success) {
						// Update IdentityProviders list
						this.identityProviders = [];
						$.each(data.model.IdentityProviders, (idx, identityProvider) => {
							this.identityProviders.push({ Value: identityProvider, Text: identityProvider });
						});
						this.hasAdIdentityProviders = data.model.HasAdIdentityProviders;
					}
				});
				this.GetUserList();
				this.updateAlert(data);
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
		watch: {
			// call again the method if the route changes
			'$route': 'fetchData',
		}
	};
</script>
