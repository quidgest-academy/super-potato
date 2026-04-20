<template>
  <div id="message_queue_status_container">
      <row>
          <br />
          <template v-if="!isEmptyObject(Model.StatLines)">

              <row>
                  <card>
                      <template #header>
                          <q-icon icon="send" />
                      </template>
                      <template #body>
                          <h3>{{ Model.ToSend }}</h3>
                          <div>{{ Resources.POR_ENVIAR54198 }}!</div>
                      </template>
                  </card>
                  <card>
                      <template #header>
                          <q-icon icon="thumb-up-outline" />
                      </template>
                      <template #body>
                          <h3>{{ Model.Sended }}%</h3>
                          <div>{{ Resources.ENVIADOS_COM_SUCESSO02254 }}!</div>
                      </template>
                  </card>
                  <card>
                      <template #header>
                          <q-icon icon="thumb-down-outline" />
                      </template>
                      <template #body>
                          <h3>{{ Model.Errors }}%</h3>
                          <div>{{ Resources.ERROS27091 }}!</div>
                      </template>
                  </card>
                  <card>
                      <template #header>
                          <q-icon icon="shopping" />
                      </template>
                      <template #body>
                          <div class="huge">{{ Model.Total }}</div>
                          <div>{{ Resources.TOTAL_ENVIADOS23967 }}!</div>
                      </template>
                  </card>
              </row>
              <br />
              <row>
                  <card>
                      <template #header>
                          {{ Resources.DETALHES_ESTATISTICO15183 }}
                      </template>
                      <template #body>
                          <qtable :rows="tStatLines.rows"
                                  :columns="tStatLines.columns"
                                  :totalRows="tStatLines.total_rows">
                              <template #Sended="props">
                                  {{ props.row.Sended }} ({{ props.row.SendedPercentage }}%)
                              </template>
                              <template #Errors="props">
                                  {{ props.row.Errors }} ({{ props.row.ErrorsPercentage }}%)
                              </template>
                          </qtable>
                      </template>
                  </card>
              </row>
          </template>
          <template v-if="!isEmptyObject(Model.ErroStatLines)">
              <row>
                  <card>
                      <template #header>
                          {{ Resources.ESTATISTICA_DE_ERROS29150 }}
                      </template>
                      <template #body>
                          <qtable :rows="tErroStatLines.rows"
                                  :columns="tErroStatLines.columns"
                                  :totalRows="tErroStatLines.total_rows">
                          </qtable>
                      </template>
                  </card>
              </row>
          </template>
      </row>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';

    export default {
        name: 'message_queue_status',
        mixins: [reusableMixin],
        props: {
            Model: {
                required: true
            }
        },
        data: function () {
            var vm = this;
            return {
                tStatLines: {
                    rows: [],
                    total_rows: 0,
                    columns: [{
                        label: () => vm.$t('QUEUE45251'),
                        name: "QueueId",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('POR_ENVIAR54198'),
                        name: "ToSend",
                        sort: true
                    },
                    {
                        label: () => vm.$t('ENVIADOS_COM_SUCESSO02254'),
                        name: "Sended",
                        sort: true,
                        slot_name: 'Sended'
                    },
                    {
                        label: () => vm.$t('ERROS27091'),
                        name: "Errors",
                        sort: true,
                        slot_name: 'Errors'
                    },
                    {
                        label: () => vm.$t('TOTAL_ENVIADOS23967'),
                        name: "Total",
                        sort: true
                    }]
                },
                tErroStatLines: {
                    rows: [],
                    total_rows: 0,
                    columns: [{
                        label: () => vm.$t('QUEUE45251'),
                        name: "QueueId",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('STATUS62033'),
                        name: "mqstatus",
                        sort: true
                    },
                    {
                        label: () => vm.$t('ERRO38355'),
                        name: "Errors",
                        sort: true
                    },
                    {
                        label: () => vm.$t('TOTAL49307'),
                        name: "Total",
                        sort: true
                    }]
                }
            };
        },
        created: function () {
            // Ler dados
            this.tStatLines.rows = (this.Model.StatLines || 0);
            this.tStatLines.total_rows = this.tStatLines.rows.length;

            this.tErroStatLines.rows = (this.Model.ErroStatLines || 0);
            this.tErroStatLines.total_rows = this.tErroStatLines.rows.length;
        }
    };
</script>
