<template>
    <div id="role_container">
        <h1>{{$t(Designation)}}</h1>
        <row>
            <h2>{{Resources.UTILIZADORES39761}}</h2>
            <qtable :rows="Model"
                    :columns="userListColumns"
                    :total_rows="total_rows"
                    @on-change-query="onChangeQuery"
                    :config="config"
                    :classes="classes"
                    >
                <template #actions="props">
                    <q-button-group borderless>
                        <q-button
                            variant="text"
                            :disabled="props.row.InRole"
                            :title='Resources.ASSOCIAR58485'
                            @click="addUser(props.row.Cod)">
                            <q-icon icon="add" />
                        </q-button>
                        <q-button
                            variant="text"
                            :disabled="!props.row.InRole"
                            :title='Resources.REMOVER14367'
                            @click="removeUser(props.row.Cod)">
                            <q-icon icon="close" />
                        </q-button>
                    </q-button-group>
                </template>
            </qtable>
            <q-button
                :label='Resources.VOLTAR01353'
                @click="$router.go(-1)" />
        </row>
    </div>
</template>

<script>
  // @ is an alias to /src
  import { reusableMixin } from '@/mixins/mainMixin';
    import { QUtils } from '@/utils/mainUtils';

  export default {
        name: 'role-assign',
    mixins: [reusableMixin],
        data: function () {
            var vm = this;
            return {
                Model: {
                },
                Designation : "",
                classes: {
                    //Add a specific class to assigned users
                    row: {
                        "assigned": function (row) {
                            return row.InRole;
                        }
                    }
                },
                userListColumns: [
                    {
                        label: () => vm.$t('ACOES22599'),
                        name: "actions",
                        slot_name: "actions",
                        sort: false,
                        column_classes: "thead-actions",
                        row_text_alignment: 'text-center',
                        column_text_alignment: 'text-center'
                    },
                    {
                        label: () => vm.$t('UTILIZADOR52387'),
                        name: "Name",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    }
                ],
                config: {
                    global_search: {
                        classes: "qtable-global-search",
                        // searchOnPressEnter: true,
						showRefreshButton: true,
                        searchDebounceRate: 1000
                    },
                    server_mode: true,
                    preservePageOnDataChange: true
                },
                queryParams: {
                    sort: [],
                    filters: [],
                    global_search: "",
                    per_page: 10,
                    page: 1,
                },
                total_rows: 1
            };
        },
    methods: {
        fetchUsers: function () {
            var vm = this;

            QUtils.log("Fetch data - Role");
            QUtils.FetchData(QUtils.apiActionURL('Users', 'GetUsersWithRole', $.extend(vm.queryParams, { module: vm.$route.params.module, roleId: vm.$route.params.role }))).done(function (data) {
                QUtils.log("Fetch data - OK (Role)");
                vm.Model = data.userList;
                vm.total_rows = data.total;
                vm.Designation = data.Designation;

            });
        },
        onChangeQuery: function (queryParams) {
            this.queryParams = queryParams;
            this.fetchUsers();
        },
        addUser: function (cod) {
            var vm = this;
            QUtils.postData("ManageUsers", "InsertRole",null,{ codpsw: cod, module: vm.$route.params.module, roleId: vm.$route.params.role }, function () {
                vm.fetchUsers()
            });

        },
        removeUser: function (cod) {
            var vm = this;
            QUtils.postData("ManageUsers", "DeleteRole", null,{ codpsw: cod, module: vm.$route.params.module, roleId: vm.$route.params.role }, function () {
                vm.fetchUsers()
            });
        },
        },
        created: function () {
            // Ler dados
            this.fetchUsers();
        }

  };
</script>
