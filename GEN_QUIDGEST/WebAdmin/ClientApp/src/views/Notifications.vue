<template>
  <div id="notifications_container">
  <div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.NOTIFICACOES03935 }}
			</h1>
		</div>
    <hr>
      <!-- This used to be a tab an it's now a self contained page-->
      <notifications_notifs :Notifications="Model.notifications" @update-model="fetchData"></notifications_notifs>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin.js';
  import { QUtils } from '@/utils/mainUtils';
  import notifications_notifs from './Notifications/Notifs.vue';

  export default {
    name: 'notifications',
    mixins: [reusableMixin],
    components: {
      notifications_notifs,
    },
    data: function () {
      return {
        Model: {},
        activeTab: 'notifs'
      };
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - Notifications");
        QUtils.FetchData(QUtils.apiActionURL('Notifications', 'Index')).done(function (data) {
          QUtils.log("Fetch data - OK (Notifications)", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
        });
      },
      isActiveTab: function (tabName) {
        return this.activeTab === tabName;
      }
    },
    mounted: function () {
      var vm = this;
      vm.observer = new MutationObserver(mutations => {
        for (const m of mutations) {
          const newValue = m.target.getAttribute(m.attributeName);
          vm.$nextTick(() => {
            if (~newValue.indexOf('active')) {
              vm.activeTab = m.target.id;
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
    created: function () {
      // Ler dados
      this.fetchData();
    },
    watch: {
      // call again the method if the route changes
      '$route': 'fetchData'
    }
  };
</script>
