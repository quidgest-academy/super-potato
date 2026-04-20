<template>
    <div class="modal fade" ref="modalForm" id="slotreport" tabindex="-1" role="dialog" aria-labelledby="slotreport_Title" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="system_setup_core_Title">Slot report</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" @click="close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <row>
                        <q-select 
                            v-model="Model.ValSlotid"
                            :label="Resources.IDENTIFICADOR_DE_SLO30549"
                            item-value="Value"
                            item-label="Text"
                            :items="slotReportIds"
                            :readonly="blockForm"
                            size="xlarge" />
                    </row>
                    <row>
                        <text-input v-model="Model.ValReport" :label="Resources.RELATORIO62426" :isReadOnly="blockForm" :size="'xlarge'"></text-input>
                    </row>
                    <row>
                        <text-input v-model="Model.ValTitulo" :label="Resources.TITULO39021" :isReadOnly="blockForm" :size="'xlarge'"></text-input>
                    </row>
                </div>
                <div class="modal-footer">
                    <q-button
                        v-if="Model.FormMode == 'delete'"
                        variant="bold"
                        color="danger"
                        :label="Resources.APAGAR04097"
                        @click="fnSubmit" />
                    <q-button
                        v-else
                        variant="bold"
                        :label="Resources.GRAVAR45301"
                        @click="fnSubmit" />
                    <q-button
                        :label="Resources.CANCELAR49513"
                        @click="close" />
                </div>
            </div>
        </div>
    </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';

    export default {
        name: 'slotreport',
        emits: ['close'],
        mixins: [reusableMixin],
        props: {
            Model: {
                required: true
            },
            show: {
                required: true
            }
        },
        computed: {
            /**
            * Control form fields (enable/disable)
            */
            blockForm: function () {
                return this.Model.FormMode == 'show' || this.Model.FormMode == 'delete';
            },

            /**
            * Return the select options
            * @return {{string, string}[]} a list of object (slot report ids and text description )
            */
            slotReportIds: function () {
                var vm = this;
                return [
                ];
            }
        },
        methods: {
            /**
            * save the report slot
            */
            fnSubmit: function () {
                var vm = this;
                QUtils.postData('ManageReports', 'SaveReportSpot', vm.Model, null, function () { vm.$emit('close', true); });
            },
            /**
            * close the modal
            */
            close: function () {
                this.$emit('close', false);
            },
            /**
            * Modal init (Show or hide)
            */
            initForm: function () {
                if (this.show) { $(this.$refs.modalForm).modal('show'); }
                else { $(this.$refs.modalForm).modal('hide'); }
            }
        },
        mounted: function () {
            this.initForm();
        },
        watch: {
            'show': 'initForm'
        }
    };
</script>
