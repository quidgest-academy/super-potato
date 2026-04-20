<template>
  <div id="migrate_files_container">
    <row v-if="!isEmptyObject(Model.ResultMsg)">
      <div class="alert alert-info">
        <span><b class="status-message">{{ Model.ResultMsg }}</b></span>
      </div>
      <br />
    </row>

    <row>
      <h4>{{ Model.FileCount + " " }}{{ Resources.FILES_TO_MIGRATE44223 }}</h4>
      <q-button
        variant="bold"
        :label="Resources.MIGRATE24124"
        @click="startMigration" />
    </row>

    <row>
      <qtable :rows="tFiles.rows"
              :columns="tFiles.columns"
              :config="tFiles.config"
              :totalRows="tFiles.total_rows"
              :enableExport="false">

            <template #Documid="props">
                {{ props.row.Documid }}
            </template>
            <template #Name="props">
                {{ props.row.Name }}
            </template>
            <template #Size="props">
                {{ props.row.Size.toFixed(2) }} MB
            </template>
            <template #Table="props">
                {{ props.row.Table }}
            </template>
            <template #Field="props">
                {{ props.row.Field }}
            </template>
      </qtable>
    </row>

    <progress-bar :show="dataPB.show" :text="dataPB.text" :progress="dataPB.progress" :withButton="true" :buttonText="Model.ButtonText" @onButtonClick="cancelMigration">
    </progress-bar>
  </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
  import { QUtils } from '@/utils/mainUtils';
  import bootbox from 'bootbox';

  export default {
    name: 'migrate_files',
    mixins: [reusableMixin],
    data: function () {
      var vm = this;
      return {
        Model: {},
        dataPB: {
          show: false,
          text: '',
          progress: 0
        },
        tFiles: {
            rows: [],
            total_rows: 0,
            columns: [
            {
                label: () => vm.Resources.DOCUMENT_ID65187,
                name: "Documid",
                slot_name: "Documid",
                sort: true,
                initial_sort: true,
                initial_sort_order: "asc",
                row_text_alignment: 'text-left',
                column_text_alignment: 'text-left'
            },
            {
                label: () => vm.Resources.FILE_NAME29387,
                name: "Name",
                slot_name: "Name",
                sort: true,
                row_text_alignment: 'text-center',
                column_text_alignment: 'text-center'
            },
            {
                label: () => vm.Resources.FILE_SIZE42792,
                name: "Size",
                slot_name: "Size",
                sort: true,
                row_text_alignment: 'text-center',
                column_text_alignment: 'text-center'
            },
            {
                label: () => vm.Resources.TABLE15475,
                name: "Table",
                slot_name: "Table",
                sort: true,
                row_text_alignment: 'text-center',
                column_text_alignment: 'text-center'
            },
            {
                label: () => vm.Resources.FIELD12991,
                name: "Field",
                slot_name: "Field",
                sort: true,
                row_text_alignment: 'text-center',
                column_text_alignment: 'text-center'
            }],
            config: {
                pagination_info: false
            }
        },
        isCancelled: false,
      };
    },
    methods: {
      fetchData: function () {
        var vm = this;
        QUtils.log("Fetch data - Maintenance - Migrate Files");
        QUtils.FetchData(QUtils.apiActionURL('dbadmin', 'MigrateFiles')).done(function (data) {
          QUtils.log("Fetch data - OK (Maintenance - Migrate Files)", data);
          $.each(data, function (propName, value) { vm.Model[propName] = value; });

          //Set progress bar Cancel button text
          vm.Model['ButtonText'] = vm.Resources.CANCELAR49513;

          vm.setTableData();
        });
      },
      startMigration: function () {
        var vm = this;
        vm.showPB = true;
        QUtils.log("Request", QUtils.apiActionURL('DbAdmin', 'MigrateFiles'));
        QUtils.postData('DbAdmin', 'MigrateFiles', null, null, function (data) {
          QUtils.log("Response", data);
          if(data.Success){
            setTimeout(vm.checkProgress, 250);
          }
          else{
            bootbox.alert(data.Message);
          }
        });
      },
      setTableData: function () {
          this.tFiles.rows = this.Model.MigrateFiles || [];
          this.tFiles.total_rows = this.tFiles.rows.length;
      },
      checkProgress: function (callBack) {
        var vm = this;

        if(vm.isCancelled){
          vm.dataPB.show = false;
          bootbox.alert(vm.Resources.OPERATION_CANCELLED_59653);
          return;
        }

        QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'GetFileMigrationStatus')).done(function (data) {
          if (data.InProcess && data.Percent < 100) {
            vm.dataPB.text = data.Text;
            vm.dataPB.progress = data.Percent;
            vm.dataPB.show = true;
            setTimeout(()=>vm.checkProgress(callBack), 500);
          }
          else {
            if (!$.isEmptyObject(data.EndMsg)) {
                if (callBack) {
                    callBack();
                }
                else {
                    bootbox.alert(data.EndMsg);
                }
              vm.fetchData();
            }
            vm.dataPB = {
              show: false,
              text: '',
              progress: 0
            };
          }
        });
      },
      cancelMigration: function () {
        var vm = this;
        QUtils.log("Request", QUtils.apiActionURL('DbAdmin', 'CancelMigrationTask'));
        QUtils.FetchData(QUtils.apiActionURL('dbadmin', 'CancelMigrationTask')).done(function (data) {
          QUtils.log("Request - OK (Maintenance - Cancel Migration Task)", data);
          if(data.Success){
            vm.isCancelled = true;
          }
          else {
            bootbox.alert(vm.Resources.THERE_HAS_BEEN_AN_ER10390 + ":<br />"
            + data.Message);

          }
        });
      }
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
