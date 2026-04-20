<template>
	<div id="wrapper" style="height:100%;display: flex; flex-direction: column;">
		<div id="header-container" >
			<nav class="main-header navbar navbar-expand c-header--sidebar">
				<ul class="navbar-nav ml-auto n-menu__aside">
					<li class="nav-item dropdown n-menu__aside-item">
						<template v-if="showDataSystems">
							<q-select
								id="system-years"
								v-model="currentYear"
								:items="Years"
								item-value="Value"
								item-label="Text"
								size="fit-content">
								<template #prepend>
									<q-icon icon="data-systems" />
									<span class="navbar__data-systems">
										{{ `${Resources.SISTEMA_DE_DADOS12710}: ` }}
									</span>
								</template>
							</q-select>
						</template>
					</li>
					<li class="nav-item dropdown n-menu__aside-item">
						<template v-if="Object.keys(langs).length > 1">
							<q-select
								id="languages"
								v-model="currentLang"
								:items="langs"
								item-value="Value"
								item-label="Text"
								size="fit-content">
								<template #prepend>
									<q-icon icon="earth" />
								</template>
							</q-select>
						</template>
					</li>
				</ul>

			</nav>
			<aside class="main-sidebar n-menu--sidebar">
				<div class="n-sidebar__brand">
					<a href="#">
						<img src="./assets/img/Q_icon.png" alt="Quidgest" class="n-sidebar__img">
					</a>
					<span>
						<span class="brand-text n-sidebar__brand-text">
							FOR | Web
						</span>
						Admin
					</span>
				</div>
				<div class="sidebar n-sidebar" style="min-height: 607px;">
					<nav class="mt-2">
						<div class="n-sidebar__section">
							<ul class="nav nav-pills nav-sidebar flex-column n-sidebar__nav" data-widget="treeview" role="menu" data-accordion="true">
								<li class="nav-item n-sidebar__nav-item">
									<a class="nav-link n-sidebar__nav-link d-none" @click.stop="navigateTo($event, 'no_configuration')">
										<q-icon icon ="gauge" />
										<p>&nbsp;{{ Resources.CONFIGURACOES19326 }}</p>
									</a>
								</li>
								<li class="nav-item n-sidebar__nav-item">
									<a class="nav-link n-sidebar__nav-link" @click.stop="navigateTo($event, 'dashboard')">
										<q-icon icon ="gauge" />
										<p>&nbsp;{{ Resources.DASHBOARD51597 }}</p>
									</a>
								</li>

								<li class="nav-item n-sidebar__nav-item">
									<a class="nav-link n-sidebar__nav-link" @click.stop="tryNavigate($event, 'system_setup')">
										<q-icon icon ="tools" />
										<p>&nbsp;{{ Resources.CONFIGURACAO_DO_SIST39343 }}</p>
									</a>
								</li>

								<li class="nav-item n-sidebar__nav-item">
									<a :class="['nav-link', 'n-sidebar__nav-link', {'level-0': true, 'selected': isSelected}]"
										@click.stop="tryNavigate($event, 'app_configuration', true)">
										<q-icon icon ="application-cog-outline" />
										<p>
											&nbsp;{{ Resources.CONFIGURACAO_DA_APLI59110 }}
											<q-icon
												class="openSelect"
												icon ="chevron-down" />
										</p>
									</a>

									<ul v-if="isMenuOpen" class="nav nav-treeview">
										<li :class="['nav-item', { 'selected': currentApp === app.Id }]"
											v-for="app in Model.Applications" :key="app.Id" :value="app.Id">
											<a :class="{'nav-link': true, 'selected': currentApp === app.Id}" @click.stop="selectApp($event, app.Id)">
												<p>{{ app.Name }}</p>
											</a>
										</li>
									</ul>
								</li>

								<li class="nav-item n-sidebar__nav-item">
									<a class="nav-link n-sidebar__nav-link" @click.stop="tryNavigate($event, 'maintenance')" style="white-space: nowrap !important;">
										<q-icon icon ="database-cog" />
										<p>&nbsp;{{ Resources.MANUTENCAO_DA_BASE_D10092 }}</p>
									</a>
								</li>

								<li class="nav-item n-sidebar__nav-item">
									<a class="nav-link n-sidebar__nav-link" @click.stop="tryNavigate($event, 'users')">
										<q-icon icon ="account-cog" />
										<p>&nbsp;{{ Resources.GESTAO_DE_UTILIZADOR20428 }}</p>
									</a>
								</li>
								<hr>
								<li class="nav-item n-sidebar__nav-item">
									<a class="nav-link n-sidebar__nav-link" @click.stop="tryNavigate($event, 'email')">
										<q-icon icon ="email" />
										<p>&nbsp;{{ Resources.SERVIDOR_DE_EMAIL19063 }}</p>
									</a>
								</li>
								<li class="nav-item n-sidebar__nav-item">
									<a class="nav-link n-sidebar__nav-link" @click.stop="tryNavigate($event, 'report_management')">
										<q-icon icon ="file-chart" />
										<p>&nbsp;{{ Resources.GESTAO_DE_RELATORIOS37970 }}</p>
									</a>
								</li>
							</ul>
						</div>
					</nav>
				</div>
			</aside>

		</div>

		<div id="content-wrapper" :class="['content-wrapper']" style="flex-basis: 100%; display: flex; flex-direction: column;">
			<div class="container-fluid" style="flex: 1 1 auto;">
				<router-view></router-view>
			</div>
		</div>
	</div>
</template>

<script>
import '@/assets/styles/main.scss';
import '@/utils/globalUtils.js';
import { reusableMixin } from '@/mixins/mainMixin';
import { QUtils } from '@/utils/mainUtils';

import store from './store'
import { mapGetters } from 'vuex'

export default {
	name: 'app',
	mixins: [reusableMixin],
	data() {
		return {
			loaded: false,
			Model: {},
			isMenuOpen: false,
			Applications: [],
			currentSelected: null,
			isSelected: false,
			langs: [
				{ Value: 'en-US', Text: 'English' },
			],
			hideDataSystems: false
		}
	},
	computed: {
		...mapGetters(['Years', 'DefaultYear']),
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
		showDataSystems() {
			return this.Years && this.isMultiYearApp && !this.hideDataSystems
		}
	},
	methods: {
		setYears(years, defaultYear) {
			store.dispatch('setYears', Array.isArray(years) ? years : [])
			store.dispatch('setDefaultYear', defaultYear)

			if ($.isEmptyObject(this.currentYear) || this.Years.findIndex(year => year.Value === this.currentYear) === -1) {
				this.currentYear = this.DefaultYear;
			}

			this.isMultiYearApp = this.Years.length > 1
		},
		getYears() {
			var vm = this;
			QUtils.FetchData(QUtils.apiActionURL('Main', 'GetYears')).done(function (data) {
				vm.setYears(data.Years, data.DefaultYear);
			});
		},
		getConfig() {
			var vm = this;
			vm.loaded = false;
			QUtils.FetchData(QUtils.apiActionURL('Dashboard', 'Index')).done(function (data) {
				$.each(data.model, function (propName, value) { vm.Model[propName] = value; });
				vm.loaded = true;
			});
		},
		/*getMenuClasses() {
			var vm = this;
			return vm.Model.HasConfig ? 'nav-link n-sidebar__nav-link' : 'nav-link n-sidebar__nav-link disabled';
		}, */
		tryNavigate(event, route, hasSubmenu = false) {
			var vm = this;
			return vm.Model.HasConfig ? this.navigateTo(event, route, hasSubmenu) : null
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
		setApplications(applications) {
			this.Model.Applications = applications;
			if ($.isEmptyObject(this.currentApp) && !$.isEmptyObject(applications)) { this.currentApp = applications[0].Id; }
		},
		getApplications() {
			var vm = this;
			QUtils.FetchData(QUtils.apiActionURL('Main', 'GetApplications')).done(function (data) {
				vm.setApplications(data.Applications);
			});
		},
		selectApp(event, appId) {
			this.currentApp = appId;
			this.isMenuOpen = true;
			if (this.currentSelected) {
				this.currentSelected.classList.remove('selected');
			}
			event.currentTarget.classList.add('selected');
			this.currentSelected = event.currentTarget;
		}
	},
	beforeUnmount() {
		this.$eventHub.off('app_updateYear');
		this.$eventHub.off('SET_SYSTEM');
		this.$eventHub.off('SET_CULTURE');
		this.$eventHub.off('SET_APPLICATIONS');
		this.$eventHub.off('fetchSysConfig');
		this.$eventHub.off('hideDataSystems');
		this.$eventHub.off('SET_YEARS');
	},
	created() {
		var vm = this;
		QUtils.FetchData(QUtils.apiActionURL('Main', 'GetGlobalSettingsJson')).done(function (data) {
			window.QGlobal = data;
			vm.setApplications(data.Applications);
			vm.setYears(data.Years, data.defaultSystem);
		});
		this.$eventHub.on('app_updateYear', this.getYears);
		this.$eventHub.on('SET_SYSTEM', function (value) { if (vm.currentYear != value) vm.currentYear = value; });
		this.$eventHub.on('SET_CULTURE', function (value) { if (vm.currentLang != value) vm.currentLang = value; });
		this.$eventHub.on('SET_APPLICATIONS', function (value) { vm.setApplications(value); });
		this.$eventHub.on('fetchSysConfig', this.getConfig);
		this.$eventHub.on('hideDataSystems', (value) => { vm.hideDataSystems = value });
		this.getConfig();
	}
};
</script>
