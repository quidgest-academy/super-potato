<template>
  <div id="notifications_signatures_container">
      <row>
          <br />
          <qtable :rows="tESign.rows"
                  :columns="tESign.columns"
                  :config="tESign.config"
                  :totalRows="tESign.total_rows">
              <template #actions="props">
                <q-button-group borderless>
                  <q-button
                    variant="text"
                    :title="Resources.EDITAR11616"
                    @click="ManageSignature(2, props.row)">
                    <q-icon icon="pencil" />
                  </q-button>
                  <q-button
                    variant="text"
                    :title="Resources.APAGAR04097"
                    @click="ManageSignature(3, props.row)">
                    <q-icon icon="bin" />
                  </q-button>
                </q-button-group>
              </template>
              <template #ValImage="props">
                  <img class="dbeditimage" :src="getEmailSignatureImage(props.row)" />
              </template>
              <template #table-footer>
                <tr>
                  <td colspan="2">
                    <q-button
                      :label="Resources.INSERIR43365"
                      @click="ManageSignature(1)">
                      <q-icon icon="add" />
                    </q-button>
                  </td>
                </tr>
            </template>
          </qtable>
      </row>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';

  export default {
    name: 'notifications_signatures',
    mixins: [reusableMixin],
    props: {
      EmailSignatures: {
        required: true
      }
    },
    data: function () {
      var vm = this;
      return {
        tESign: {
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
                    name: "ValName",
                    sort: true,
                    initial_sort: true,
                    initial_sort_order: "asc"
                },
                {
                    label: () => vm.$t('IMAGEM19513'),
                    name: "ValImage",
                    slot_name: 'ValImage'
                },
                {
                    label: () => vm.$t('TEXTO_APOS_A_ASSINAT02808'),
                    name: "ValTextass",
                    sort: true
                }],
                config: {
                  table_title: () => vm.$t('ASSINATURAS_DE_EMAIL13716')
                }
            },
            v: Date.now()
      };
    },
    methods: {
      updateTESignData: function() {
          var vm = this;
          vm.tESign.rows = vm.EmailSignatures || [];
          vm.tESign.total_rows = (vm.EmailSignatures || []).length;
      },
      ManageSignature: function (mod, row) {
        var vm = this, codsigna = '';
        if(!$.isEmptyObject(row) && (mod === 2 || mod === 3)) codsigna = row.ValCodsigna;
          vm.$router.push({ name: 'manage_signature', params: { mod, codsigna, culture: vm.currentLang, system: vm.currentYear } });
      },
      getEmailSignatureImage: function (row) {
          return QUtils.apiActionURL('Email', 'getEmailSignatureImage', { key: row.ValCodsigna, v: this.v });
      }
    },
    created: function () {
      this.updateTESignData();
    },
    mounted: function () {
        this.v = Date.now();
    },
    watch: {
      // call again the method if the route changes
        EmailSignatures: function () {
            this.updateTESignData();
        }
    }
  };
</script>
