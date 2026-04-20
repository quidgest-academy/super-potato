<template>
  <div id="config_migration_container">
    <br />
    <row>
      <div v-if="!isEmptyObject(Model.model.ResultMsg)" class="alert alert-danger">
        <span>
          <b class="status-message" v-html="Model.model.ResultMsg"></b>
        </span>
      </div>
    </row>
    <row>
      <numeric-input v-model="Model.model.ConfigVersion" :label="Resources.VERSAO_DO_FICHEIRO_D63572" :is-read-only="true"></numeric-input>
    </row>
    <row>
      <numeric-input v-model="Model.model.CurVersion" :label="Resources.VERSAO_ATUAL00037" :is-read-only="true"></numeric-input>
    </row>

    <row>
      <div class="q-button-container">
        <q-button
            variant="bold"
            :label="Resources.INICIAR08126"
            @click="MigrateConfig" />
        <q-button
            :label="Resources.CANCELAR49513"
            @click="cancel" />
      </div>
    </row>

  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';

  export default {
    name: 'config_migration',
    mixins: [reusableMixin],
    data: function () {
      return {
        Model: { model:{} }
      };
    },
    methods: {
      fetchData: function() {
        var vm = this;
        QUtils.FetchData(QUtils.apiActionURL('ConfigMigration', 'Index')).done(function (data) {
          $.each(data, function (propName, value) { vm.Model[propName] =value; });
        });
      },
      MigrateConfig: function() {
        var vm = this;
        QUtils.postData('ConfigMigration', 'MigrateConfig', vm.Model, null, function(data) {
          if(data.redirect) {
            bootbox.alert({
              message: vm.Resources.A_OPERACAO_FOI_CONCL36721,
              callback: function () { vm.cancel(false); }
            });
          } else {
            $.each(data, function (propName, value) { vm.Model[propName] = value; });
          }
        });
      },
      cancel: function (showDialog) {
        var vm = this;
        var fnExit = function() {
          vm.$router.replace({ name: 'system_setup', params: { culture: vm.currentLang, system: vm.currentYear } });
        };

        if(showDialog) {
          bootbox.confirm({
              message: vm.Resources.TEM_A_CERTEZA_QUE_DE50516,
              buttons: {
                  confirm: {
                      label: vm.Resources.SIM28552,
                      className: 'btn-success'
                  },
                  cancel: {
                      label: vm.Resources.NAO06521,
                      className: 'btn-danger'
                  }
              },
              callback: function (result) {
                  if (result) {
                      fnExit();
                  }
              }
          });
        }
        else {
          fnExit();
        }
      }
    },
    created: function () {
        // Ler dados
        this.fetchData();
    }
  };
</script>
