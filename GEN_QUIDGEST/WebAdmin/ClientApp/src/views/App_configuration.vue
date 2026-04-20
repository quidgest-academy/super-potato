<template>
	<div id="app_config">
		<div class="q-stack--column">
			<h1 class="f-header__title">
			{{ appConfigTexts.appConfigurationTitle }}
			</h1>
		</div>
		<hr />
		<div>
			<QTabContainer
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
	</div>
</template>

<script>
// @ is an alias to /src
import { reusableMixin } from '@/mixins/mainMixin';
import { QUtils } from '@/utils/mainUtils';
import { reactive, computed } from 'vue';
import security from './App_configuration/Security.vue';
import paths from './App_configuration/Paths.vue';
import { texts } from '@/resources/hardcodedTexts.ts';
import { AppConfigTexts } from '@/resources/viewResources.ts';

export default {
	name: 'app_config',
	mixins: [reusableMixin],
	emits: ['update-model', 'update-users'],
	components: { security, paths },
	data() {
		var vm = this;
		return {
			Model: {},
			tabGroup: {
				selectedTab: 'security-tab',
				alignTabs: 'left',
				iconAlignment: 'left',
				isVisible: true,
				alertClass: 'alert alert-danger',
				tabsList: [
					{
						id: 'security-tab',
						componentId: 'security',
						name: 'security',
						label: '',
						disabled: false,
						isVisible: true,
						props: { model: computed(() => vm.Model?.Security), SelectLists: computed(() => vm.Model?.SelectLists), resources: computed(() => vm.appConfigTexts) },
						events: { 'update-model': vm.fetchData, 'update-users': vm.updateUsers }
					},
					{
						id: 'paths-tab',
						componentId: 'paths',
						name: 'paths',
						label: '',
						disabled: false,
						isVisible: true,
						props: { model: computed(() => vm.Paths), resources: computed(() => vm.appConfigTexts) },
						events: { 'update-model': vm.setModel }
					}
				]
			}
		};
	},
	computed: {
		Paths() {
			var vm = this;
			if ($.isEmptyObject(vm.currentApp) || $.isEmptyObject(vm.Model.Paths))
			return null;
			vm.Model.Paths[vm.currentApp].app = vm.currentApp;
			return vm.Model.Paths[vm.currentApp] || null;
		},
		Cores() {
			var vm = this;
			return !$.isEmptyObject(vm.currentApp) && !$.isEmptyObject(vm.Model.Cores) ? (vm.Model.Cores[vm.currentApp] || null) : null;
		},
		hardcodedTexts() {
			return {
				securityLabel: this.Resources[texts.securityLabel],
				pathsLabel: this.Resources[texts.pathsLabel],
			};
		},
		appConfigTexts() {
			return new AppConfigTexts(this);
		},
	},
	methods: {
		fetchData() {
			var vm = this;
			QUtils.log("Fetch data - Config", QUtils.apiActionURL('Config', 'Index'));
			QUtils.FetchData(QUtils.apiActionURL('Config', 'Index')).done(function (data) {
			QUtils.log("Fetch data - OK (Config)", data);
			if(data.redirect) {
				vm.$router.replace({ name: data.redirect, params: { culture: vm.currentLang, system: vm.currentYear } });
			}
			else if (data.reload) {
				vm.currentYear = data.system;
				vm.fetchData();
			}
			else {
				vm.setModal(data);
			}
			});
		},
		setModal(data) {
			var vm = this;
			$.extend(vm.Model, data);
			// Select the first exists application
			if ($.isEmptyObject(vm.currentApp) && !$.isEmptyObject(vm.Model.Applications)) {
			vm.currentApp = vm.Model.Applications[0].Id;
			}
			// Focus on errors div
			if (!$.isEmptyObject(vm.Model.ResultMsg)) {
			window.scrollTo(0,0);
			}
		},
		getTab(tab, selectedTab) {
			return _find(this[tab]['tabsList'], (x) => x.id === selectedTab)
		},

		changeTab(tab, tabProp, selectedTab) {
			this[tab][tabProp] = selectedTab
		},
		reloadMQueues() {
			var vm = this;
			QUtils.FetchData(QUtils.apiActionURL('Config', 'ReloadMQueues')).done(function (data) {
				if (data.Success) {
					$.each(data.MQueues, function (propName, value) {
						if ($.isArray(vm.Model.MQueues[propName])) { vm.Model.MQueues[propName].splice(0); }
						$.extend(vm.Model.MQueues[propName], value);
					});
				}
			});
		},
		updateUsers(eventData) {
			if ($.isEmptyObject(this.Model.Security[eventData.currentApp].Users))
				$.extend(this.Model.Security[eventData.currentApp], reactive({ Users: [] }));
			else
				this.Model.Security[eventData.currentApp].Users.splice(0);

			$.extend(this.Model.Security[eventData.currentApp].Users, eventData.users);
		}
	},
	mounted() {
		this.tabGroup.tabsList[0].label = this.hardcodedTexts.securityLabel;
		this.tabGroup.tabsList[1].label = this.hardcodedTexts.pathsLabel;

		var vm = this;
		vm.observer = new MutationObserver(mutations => {
			for (const m of mutations) {
				const newValue = m.target.getAttribute(m.attributeName);
				vm.$nextTick(() => {
					if (~newValue.indexOf('active')) {
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
	watch: {
		// call again the method if the route changes
		'$route': 'fetchData',
		'currentApp': 'fetchData'
	}
};
</script>
