<template>
  <div id="notifications_notifs_container">

    <row>
        <br />
        <qtable :rows="tNotif.rows"
                :columns="tNotif.columns"
                :config="tNotif.config"
                :totalRows="tNotif.total_rows">

            <template #actions="props">
              <q-button-group borderless>
                <q-button
                  variant="text"
                  :title="Resources.EDITAR11616"
                  @click="ManageNotif(2, props.row)">
                  <q-icon icon="pencil" />
                </q-button>
                <q-button
                  variant="text"
                  :title="Resources.PROCESSAR07293"
                  @click="Send(props.row)">
                  <q-icon icon="send" />
                </q-button>
              </q-button-group>
            </template>
            <template #sendsEmail="props">
              <q-icon 
                :icon="props.row.SendsEmail ? 'check' : 'close'"
                :color="props.row.SendsEmail ? 'success' : 'danger'" />
            </template>
            <template #sendsToDatabase="props">
              <q-icon 
                :icon="props.row.SendsToDatabase ? 'check' : 'close'"
                :color="props.row.SendsToDatabase ? 'success' : 'danger'" />
            </template>
        </qtable>
    </row>
    <row>
      <q-button
        variant="bold"
        :label="Resources.PROCESSAR_TODAS18012"
        @click="SendAll">
        <q-icon icon="send" />
      </q-button>
    </row>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';

  export default {
    name: 'notifications_notifs',
    mixins: [reusableMixin],
    emits: ['update-model'],
    props: {
      Notifications: {
        required: true
      }
    },
    data: function () {
      var vm = this;
      return {
        tNotif: {
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
                name: "Notif",
                sort: true,
                initial_sort: true,
                initial_sort_order: "asc"
            },
            {
                label: () => vm.$t('PERMITE_ENVIO_DE_EMA25939'),
                name: "SendsEmail",
                sort: true,
                slot_name: 'sendsEmail'
            },
            {
                label: () => vm.$t('PERMITE_ESCRITA_NA_B48768'),
                name: "SendsToDatabase",
                sort: true,
                slot_name: 'sendsToDatabase'
            },
            {
                label: () => vm.$t('NO_DE_MENSAGENS_CONF09230'),
                name: "NumMessagesConfig",
                sort: true
            }],
                                            }
      };
    },
    methods: {
      updateTNotifData: function() {
          var vm = this;
          vm.tNotif.rows = vm.Notifications || [];
          vm.tNotif.total_rows = (vm.Notifications || []).length;
      },
      ManageNotif: function (mod, row) {
          this.$router.push({ name: 'manage_notif', params: { mod, idnotif: row.IDNotif, culture: this.currentLang, system: this.currentYear } });
      },
      Send: function (row) {
          var vm = this;
          QUtils.postData('Notifications', 'Send', null, { idnotif: row.IDNotif }, function () {
              vm.$emit('update-model');
          });
      },
      SendAll: function () {
          var vm = this;
          QUtils.postData('Notifications', 'SendAll', null, null, function () {
              vm.$emit('update-model');
          });
      }
    },
    created: function () {
      this.updateTNotifData();
    },
    watch: {
      // call again the method if the route changes
        Notifications: function () {
            this.updateTNotifData();
        }
    }
  };
</script>
