<template>
  <div id="notifications_container">
    <div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.SERVIDORES_DE_EMAIL15136 }}
			</h1>
		</div>
    <hr>
    <template v-if="!isEmptyObject(Model.ResultMsg)">
            <div class="alert alert-info">
                <p><b class="status-message">{{ Model.ResultMsg }}</b></p>
            </div>
            <br />
    </template>
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
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin.js';
  import { QUtils } from '@/utils/mainUtils';
  import { reactive, computed } from 'vue';

  import notifications_pmail from './Notifications/Pmail.vue';
  import notifications_signatures from './Notifications/Signatures.vue';
  import more from './Notifications/More.vue';

  export default {
    name: 'email',
    mixins: [reusableMixin],
    components: {
      notifications_pmail,
      notifications_signatures,
      more
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
    data: function () {
      var vm = this;
      return {
        Model: {},
        tabGroup: {
					selectedTab: 'pmail-tab',
					alignTabs: 'left',
					iconAlignment: 'left',
					isVisible: true,
					tabsList: [
						{
							id: 'pmail-tab',
							componentId: 'notifications_pmail',
							name: 'pmail',
							label: vm.$t('SERVIDORES_DE_EMAIL15136'),
							disabled: false,
							isVisible: true,
              props: { model: computed(() => vm.Model), EmailProperties: computed(() => vm.Model?.emailProperties) },
              events: { 'update-model': vm.fetchData }
						},
						{
							id: 'signatures-tab',
							componentId: 'notifications_signatures',
							name: 'signatures',
							label: vm.$t('ASSINATURAS_DE_EMAIL13716'), 
							disabled: false,
							isVisible: true,
              props: { model: computed(() => vm.Model), EmailSignatures: computed(() => vm.Model?.emailSignatures) },
              events: { 'update-model': vm.fetchData }
						},
						{
							id: 'more-tab',
							componentId: 'more',
							name: 'more',
							label: vm.$t('MAIS25935'),
							disabled: false,
							isVisible: true,
              props: { model: computed(() => vm.Model) }
						}
					]
        }
      }
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - Email");
        QUtils.FetchData(QUtils.apiActionURL('Email', 'Index')).done(function (data) {
          QUtils.log("Fetch data - OK (Email)", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
          vm.Model.emailPropertiesList = vm.Model.emailProperties.map(x => { return { Value: x.ValId, Text: x.ValId }; });
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
      this.observer.disconnect();
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
