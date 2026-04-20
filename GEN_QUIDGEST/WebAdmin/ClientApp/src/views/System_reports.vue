<template>
  <div id="system_reports_container">
    <div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.RELATORIO_DO_SISTEMA49744 }}
			</h1>
		</div>
    <hr>
    
    <row>
      <q-select
        v-model="currentApp"
        item-value="Id"
        item-label="Name"
        :items="Model.Applications" />
    </row>
    <row>
      <div v-if="!isEmptyObject(Model.ResultMsg)" class="alert alert-danger">
        <span>
          <b class="status-message" v-html="Model.ResultMsg"></b>
        </span>
      </div>
      <template v-else>
        <row>
          <q-text-field
            v-model="searchError"
            placeholder="Search for..."
            @update:model-value="searchErro">
            <template #append>
              <q-button
                variant="bold"
                @click="highlightSearchErro">
                <q-icon icon=magnify />
              </q-button>
            </template>
          </q-text-field>
        </row>

        <row>
          <q-text-area
            v-model="Model.ErrorLog"
            ref="textAreaErrorLog"
            :rows="5"
            size="xlarge"
            autosize />
        </row>
      </template>
    </row>

  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import 'jquery-highlight';

  export default {
    name: 'system_reports',
    mixins: [reusableMixin],
    data: function () {
      return {
        Model: {},
        searchError: ''
      };
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - System reports");
        QUtils.FetchData(QUtils.apiActionURL('ErrorLog', 'Index')).done(function (data) {
          QUtils.log("Fetch data - OK (System reports)", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });
          // Select the first exists application
          if ($.isEmptyObject(vm.currentApp) && !$.isEmptyObject(vm.Model.Applications)) {
            vm.currentApp = vm.Model.Applications[0].Id;
          }
        });
      },
      searchErro: function (event) {
        if (event.keyCode == 13) {
          this.highlightSearchErro();
        }
      },
      highlightSearchErro: function () {
          $(this.$refs.textAreaErrorLog).unhighlight();
          $(this.$refs.textAreaErrorLog).highlight(this.searchError);
      }
    },
    created: function () {
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
