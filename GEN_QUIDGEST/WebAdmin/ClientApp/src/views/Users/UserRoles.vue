<template>
    <div id="container">
        <div class="row">
            <div class="col-6">
            <div class="q-help__info-banner">
                <div class="q-help__info-banner-header">
                    <q-icon icon="information-outline" />
                    <h5>{{ Resources.ATRIBUIR_RAPIDAMENTE15967 }}</h5>
                </div>
                <div class="q-help__info-banner-body">
                    <span>
                        {{ Resources._1__SELECIONE_UTILIZA54069 }}<br>
                        {{ Resources._2__SELECIONE_FUNCOES21542 }}<br>
                        {{ Resources._3__REVEJA_E_CONFIRME45679 }}<br>
                    </span>
                </div>
            </div>
            <q-group-box-container :label="Resources.DISPONIVEIS47523">
                <div class="list-group">
                    <h3>{{Resources.UTILIZADORES39761}}</h3>
                        <qtable :rows="Users.rows"
                            :columns="Users.columns"
                            :config="Users.config"
                            @on-change-query="onChangeQuery"
                            :totalRows="Users.total_rows">
                            <template #actions="props">
                                <q-button
                                    borderless
                                    :title="Resources.EDITAR11616"
                                    @click="captureUser(props.row)">
                                    <q-icon icon="arrow-right-bold" />
                                </q-button>
                            </template>
                        </qtable>
                        <br>
                        <hr>
                        <br>
                        <div id="roles-list" class="list-group">
                            <h3>{{Resources.FUNCOES16287}}</h3>
                            <q-select
                                id="module-list"
                                v-model="selectedModule"
                                :placeholder="Resources.SELECCIONAR_MODULO15000"
                                item-value="Module"
								item-label="ModuleName"
                                :items="modules" />
                            <qtable :columns="Roles.columns"
                                :rows="modulesList"
                                :total_rows="modulesList.length"
                                :config="Roles.config"
                                >
                                <template #actions="props">
                                    <q-button
                                        borderless
                                        :title="Resources.EDITAR11616"
                                        @click="captureRole(props.row)">
                                        <q-icon icon="arrow-right-bold" />
                                    </q-button>
                                </template>
                                <template #Designation="props">
                                    <q-badge
                                        variant="bold" >
                                        {{ $t(props.row.Designation) }}
                                    </q-badge>
                                </template>
                            </qtable>
                        </div>
                    </div>
                </q-group-box-container>
            </div>
            <div class="col-6">
                <q-group-box-container :label="Resources.SELECIONADOS52011">
                    <div class="list-group">
                        <h3>{{Resources.UTILIZADORES39761}}</h3>
                        <qtable :columns="UsersSelected.columns"
                            :rows="selectedUsers"
                            :totalRows="selectedUsers.length"
                            :config="Roles.config">
                            <template #actions="props">
                                <q-button
                                    borderless
                                    :title="Resources.ELIMINAR21155"
                                    @click="removeUser(props.row)">
                                    <q-icon icon="arrow-left-bold" />
                                </q-button>
                            </template>
                        </qtable>
                        <br>
                        <hr>
                    </div>
                    <div class="list-group"> <!-- v-if="selectedRoles.length > 0" -->
                        <h3>{{Resources.FUNCOES16287}}</h3>
                        <qtable :columns="RolesSelected.columns"
                            :rows="selectedRoles"
                            :total_rows="selectedRoles.length"
                            :config="Roles.config">
                        <template #actions="props">
                            <q-button
                                borderless
                                :title="Resources.ELIMINAR21155"
                                @click="removeRole(props.row)">
                                <q-icon icon="arrow-left-bold" />
                            </q-button>
                        </template>
                        <template #Designation="props">
                            <q-badge
                                variant="bold" >
                                {{ $t(props.row.Designation) }}
                            </q-badge>
                        </template>
                        </qtable>
                    </div>
                </q-group-box-container>
            </div>
        </div>
    </div>
    <div class="centerdiv">
        <q-button
            v-if="selectedRoles.length > 0 || selectedUsers.length > 0"
            :disabled="selectedRoles.length <= 0 || selectedUsers.length <= 0"
            :label="Resources.INSERIR43365"
            @click="bindConfigAdd" />
        <q-button
            v-if="selectedRoles.length > 0 || selectedUsers.length > 0"
            :disabled="selectedRoles.length <= 0 || selectedUsers.length <= 0"
            :label="Resources.REMOVER14367"
            @click="bindConfigRemove" />
    </div>
</template>

<script>

    import { reusableMixin } from '@/mixins/mainMixin';
    import { QUtils } from '@/utils/mainUtils';
    import bootbox from 'bootbox';
    import $ from 'jquery';


export default {
    name: 'Nroles',
    mixins: [reusableMixin],
    data() {
        var vm = this;
        return {
            Users: {
                rows: [],
                total_rows: 0,
                columns:[
                {
                    label: () => vm.$t('NOME47814'),
                    name: "Username",
                    sort: true,
                    initial_sort: true,
                    initial_sort_order: "asc"
                },{
                    label: () => vm.$t('ACOES22599'),
                    name: "actions",
                    slot_name: "actions",
                    sort: false,
                    column_classes: "thead-actions",
                    row_text_alignment: 'text-center',
                    column_text_alignment: 'text-center'
                }
                ],
                config: {
                single_row_selectable : true,
                    server_mode: true,
                    preservePageOnDataChange: true

                },
                queryParams: {
                    sort: [],
                    filters: [],
                    global_search: "",
                    per_page: 10,
                    page: 1,
                    component: "userRoles",
                }
            },
            UsersSelected: {
                columns:[{
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
                    name: "Username",
                    sort: true,
                    initial_sort: true,
                    initial_sort_order: "asc"
                },
                ],
                queryParams: {
                    sort: [],
                    filters: [],
                    global_search: "",
                    per_page: 5,
                    page: 1,
                }
            },
            Roles: {
                rows: [],
                total_rows: 1,
                columns: [
                    {
                        label: () => vm.$t('SIGLA14738'),
                        name: "Module",
                    },
                    {
                        label: () => vm.$t('MODULO43834'),
                        name: "ModuleName",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('ROLE60946'),
                        name: "Designation",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('ACOES22599'),
                        name: "actions",
                        slot_name: "actions",
                        sort: false,
                        column_classes: "thead-actions",
                        row_text_alignment: 'text-center',
                        column_text_alignment: 'text-center'
                    }],
                config: {
                    single_row_selectable : true,
                    per_page:20,
                }
            },RolesSelected: {
                rows: [],
                total_rows: 1,
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
                        label: () => vm.$t('SIGLA14738'),
                        name: "Module",
                    },
                    {
                        label: () => vm.$t('MODULO43834'),
                        name: "ModuleName",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    },
                    {
                        label: () => vm.$t('ROLE60946'),
                        name: "Designation",
                        sort: true,
                        initial_sort: true,
                        initial_sort_order: "asc"
                    }
                    ],
                config: {
                    single_row_selectable : true
                }
            },
            selectedUsers: [],
            selectedUsersCounter: 1,
            selectedRoles: [],
            redundantRoles:[],
            redundantRolesMessage: "",
            modulesList: [],
            selectedModule: '',
            modules: [],
            result:{},
            repeatedAssignments:[],
            invalidRemovals: [],
        }
    },
    methods:{
        reset() {
            this.selectedUsers = [];
            this.selectedRoles = [];
            this.repeatedAssignments = [];
            this.invalidRemovals = [];
            this.redundantRolesMessage = "";
            this.redundantRoles = [];
            this.selectedUsersCounter = 1;
        },
        // Users Section
        fetchUsers() {
            var vm = this;
            QUtils.log("GetUserList - Users");
            QUtils.FetchData(QUtils.apiActionURL('Users', 'GetUserList', vm.Users.queryParams)).done(function (data) {
                QUtils.log("GetUserList - OK (Users)", data);
                vm.Users.total_rows = data.recordsTotal;
                var newRows = [];
                //iterate between users
                for (let j = 0; j < data.data.length; j++) {
                    var row = {};
                    let user = data.data[j];
                    row.Codpsw = user.Codpsw;
                    row.Username = user.Nome;
                    row.UserRoles = user.privileges;
                    row.Index = j;
                    newRows.push(row);
                    }
                    vm.Users.rows = newRows;
            });
        },

        // get an user from users list
        getUser(codpsw) {
            var vm = this;
            QUtils.log("GetUserList - Users");
            QUtils.FetchData(QUtils.apiActionURL('Users', 'GetUserList', vm.Users.queryParams)).done(function (data) {
            QUtils.log("GetUserList - OK (Users)", data);
            //iterate between users
                for (let j = 0; j < data.data.length; j++) {
                    var row = {};
                    let user = data.data[j];
                    if (user.Codpsw == codpsw) {
                        row.Codpsw = user.Codpsw;
                        row.Username = user.Nome;
                        row.UserRoles = user.privileges;
                        row.Index = j;
                        vm.Users.rows.splice(j, 0, row);
                    }
                }
            });
        },

        // get user from manage users list
        fetchUser(codpsw, index) {
            var vm = this;
            var repeated = false;


            //check if a user is already selected
            vm.selectedUsers.forEach(selectedUser => {
                if(selectedUser.CodUser === codpsw){
                    repeated = true;
                }
            })

            if(!repeated){
                QUtils.log("Fetch data - User management");
                QUtils.FetchData(QUtils.apiActionURL('ManageUsers', 'Index', { mod: "2", cod: codpsw })).done(function (data) {
                QUtils.log("Fetch data - OK (User management)", data);

                    if (data.Success) {
                        data.model = Object.assign({}, data.model, { Repeated:[], Index: index })
                        if(vm.selectedRoles) {
                            vm.selectedRoles.forEach(row => {
                                vm.roleCheck(data.model, row, false)
                                if(vm.result.boolean) {
                                    data.model.Repeated.push(row)
                                }
                            })
                        }

                    vm.selectedUsers.push(data.model);
                    }
                    else if (data.redirect) {
                        vm.$router.replace({ name: data.redirect });
                    }
                });
            }
        },


        onChangeQuery(queryParams) {
            this.Users.queryParams = queryParams;
            this.fetchUsers();
        },

        //remove user from available users list
        removeUserList(row) {
            var vm = this;
            vm.Users.rows.splice(vm.Users.rows.findIndex(element =>
            element.Index === row.Index), 1);
        },

        //actions with user insert into selected list and remove from available list
        captureUser: function(row) {
            this.fetchUser(row.Codpsw, row.Index);
            this.removeUserList(row);
        },
        //remove user from selected users list and insert into available list
        removeUser(row) {
        this.selectedUsers.splice(this.selectedUsers.findIndex(element =>
        element.CodUser === row.CodUser), 1);
        this.getUser(row.CodUser);
        },


        // Roles Section

        //fetch all roles from db
        fetchRoles() {
            var vm = this;
            QUtils.FetchData(QUtils.apiActionURL('Users', 'GetRoles')).done(function (data) {
                var allRoles = data;
                var newRows = [];
                for (let i = 0; i < allRoles.length; i++) {
                    var row = {};
                    let role = allRoles[i];
                    row.Designation = role.Designation;
                    row.Module = role.Module;
                    row.ModuleName = role.ModuleName;
                    row.Role = role.Role;
                    row.Index = i;
                    newRows.push(row);
                }
                vm.Roles.rows = newRows;
                vm.Roles.total_rows = vm.Roles.rows.length;

				vm.getDistinctModules();
				vm.selectedModule = vm.modules[0].Module;
				vm.getRolesByModule();
				console.log(vm.modulesList, vm.modules);
				//This should never be done like this. This is reinventing Vue using jquery
				$("#roles-list .col").empty();
				$("#module-list").appendTo("#roles-list .col");
            });
        },
        //Get roles by a selected module
        getRolesByModule() {
            var vm = this;
            if (vm.selectedModule == "All") {
                vm.modulesList = vm.Roles.rows;
            } else {
                vm.modulesList = [];
                vm.Roles.rows.forEach(element => {
                    if (vm.selectedModule == element.Module) {

                        vm.modulesList.push(element);

                    }
                })
            }
        },

        // get a role from roles list
        getRole(designation, index) {
            var vm = this;
            QUtils.FetchData(QUtils.apiActionURL('Users', 'GetRoles')).done(function (data) {
                var allRoles = data;
                for (let i = 0; i < allRoles.length; i++) {
                    var row = {};
                    let role = allRoles[i];
                    if (role.Designation == designation && i == index) {
                        row.Designation = role.Designation;
                        row.Module = role.Module;
                        row.ModuleName = role.ModuleName;
                        row.Role = role.ModuleName;
                        row.Index = i;
                        vm.modulesList.splice(i, 0, row);
                        break;
                    }
                }
            });
        },

        //remove role from principal roles list
            removeRoleList(row) {
            var vm = this;
            vm.modulesList.splice(vm.modulesList.findIndex(element => (element.Module === row.Module) && (element.Designation === row.Designation)), 1)
        },

        // actions with role. insert into secondary list and remove from principal list
        captureRole: function(row){
            var repeated = false;

            //check if a role is already selected
            this.selectedRoles.forEach(selectedRole => {
                if((selectedRole.Module === row.Module) && (selectedRole.Designation === row.Designation)){
                    repeated = true;
                    }
                })
            if(!repeated) {
                this.selectedRoles.push(row)
                this.checkRepeated(row, true)
            }
            this.removeRoleList(row);
        },


         //remove role from selected roles list and insert into available list
        removeRole(row) {
            this.selectedRoles.splice(this.selectedRoles.findIndex(element =>
            (element.Module === row.Module) && (element.Designation === row.Designation)),1);

            this.checkRepeated(row, false);
            this.getRole(row.Designation, row.Index);
        },

        //////////////// Check and Submit/Remove ////////////////
        roleCheck(user, role, removable) {
            var entry = 0;
            var flag = false;

            if(!removable){
                user.AssignedRoles[role.Module].forEach(r => {
                    if(r.Role === role.Role) {
                        //if the role is already assigned we store it for a later display
                        this.result = {boolean: true, index: entry};
                        flag = true
                    }
                    entry += 1;
                });
            }else{
                for(let i=0 ; i < user.Repeated.length ; i++){
                    if(user.Repeated[i].Module === role.Module && user.Repeated[i].Role === role.Role) {
                //if the role is already assigned we remove it from display
                this.result = {boolean: true, index: entry};
                flag = true
                }
                entry += 1;
                }
                }

                if(!flag)
                    this.result = {boolean: false, index: -1};
        },
        roleIsAssigned(user, role) {
            var entry = 0;
            var flag = false;
            user.AssignedRoles[role.Module].forEach(r => {
                if(r.Role == role.Role) {
                    //if the role is already assigned we store it for a later display
                    this.repeatedAssignments.push({ user: user,
                            role: role
                    });
                    this.result = {boolean: true, index: entry};
                    flag = true
                }
                entry += 1;
            });
            if(!flag)
                this.result = {boolean: false, index: -1};
        },

        submit(model,codpsw) {
            var vm = this;
            QUtils.postData('ManageUsers', 'SaveConfig', model, { mod: "2", cod: codpsw }, function (data) {
                if (data.Success) {

                    if (data.ignoredRoles.length == 0) {
                        vm.$router.replace({ name: 'users', params: { culture: vm.currentLang, system: vm.currentYear } });
                    }
                    else {
                        vm.redundantRoles.push(data.ignoredRoles)
                    }
                }
                else {
                    $.each(data.model, function (propName, value) { model[propName] = value; });
                }

                //we wait for all the data to arrive in order to display all the redundant roles at once
                if(vm.selectedUsersCounter >= vm.selectedUsers.length){
                    if(vm.redundantRoles.length > 0){
                        vm.storeRedundantRoles();
                        vm.displayredundantRoles();
                    }
                    vm.reset();
                }else{
                    vm.selectedUsersCounter += 1;
                }
            });
        },

        displayInvalidActions(type) {
            var vm = this;
            var MESSAGE, message, roleMessage, i;

            if(type) MESSAGE = vm.Resources.FUNCAO_AT_ROLE_JA_TINH60074;  //error message to show a role was already assigned to a user
            else MESSAGE = vm.Resources.FUNCAO_AT_ROLE_NAO_EST14526;   //error message to show a role isnt assigned to a user, so it cant be removed

            //case for inserting invalid roles
            if(vm.repeatedAssignments.length > 0 && type) {
                message = "<br><ul>";
                for (i = 0; i < vm.repeatedAssignments.length; i++)
                {

                    roleMessage = MESSAGE.replace("@role", "<b>"+ this.repeatedAssignments[i].role.Designation + "</b>");
                    roleMessage = roleMessage.replace("@user","<b>"+ this.repeatedAssignments[i].user.Username + "</b>");

                    message += "<li>" + roleMessage + "</li>";
                }
                message += "</ul>";

                bootbox.dialog({
                    title: vm.Resources.RELATORIO_DE_GESTAO_29557,
                    message: message,
                    buttons: {
                        ok: {
                            label: '<i class="fa fa-check"></i> Ok'
                        }
                    },
                    callback() {
                        vm.$router.replace({ name: 'users', params: { culture: vm.currentLang, system: vm.currentYear } });
                    }
                });
                /*bootbox.alert(message, function () {
                    vm.$router.replace({ name: 'users', params: { culture: vm.currentLang, system: vm.currentYear } });
                });*/
            }

            //case for removing invalid roles
            else if(vm.invalidRemovals.length > 0 && !type) {
                message = "<br><ul>";
                for (i = 0; i < vm.invalidRemovals.length; i++)
                {

                    roleMessage = MESSAGE.replace("@role", "<b>"+ this.invalidRemovals[i].role.Designation + "</b>");
                    roleMessage = roleMessage.replace("@user","<b>"+ this.invalidRemovals[i].user.Username + "</b>");

                    message += "<li>" + roleMessage + "</li>";
                }

                message += "</ul>";
                bootbox.dialog({
                    title:  vm.Resources.RELATORIO_DE_GESTAO_29557,
                    message: message,
                    buttons: {
                        ok: {
                            label: '<i class="fa fa-check"></i> Ok'
                        }
                    },
                    callback() {
                        vm.$router.replace({ name: 'users', params: { culture: vm.currentLang, system: vm.currentYear } });
                    }
                });


                /*bootbox.alert(message, function () {
                    vm.$router.replace({ name: 'users', params: { culture: vm.currentLang, system: vm.currentYear } });
                });*/
            }
        },

        storeRedundantRoles() {
            var vm = this;
            //error message to show a role is redundant
            vm.redundantRolesMessage = vm.Resources.ATENCAO__ALGUMAS_PER50140 + "<br>";
            for (var i = 0; i < vm.redundantRoles.length; i++){
                var pair = vm.redundantRoles[i][0];
                var MESSAGE = vm.Resources.AT_CHILD_FOI_IGNORADO_31949;

                var child = vm.Resources[pair.child] ? vm.Resources[pair.child] : pair.child;
                var parent = vm.Resources[pair.parent] ? vm.Resources[pair.parent] : pair.parent;

                var roleMessage = MESSAGE.replace("@child", "<b>"+ child + "</b>");
                roleMessage = roleMessage.replace("@parent","<b>"+ parent + "</b>");
                vm.redundantRolesMessage += roleMessage + "<br>";
            }
        },

        displayredundantRoles() {
            var vm = this;
            if(vm.redundantRolesMessage) {
            bootbox.dialog({
                    title:  vm.Resources.RELATORIO_DE_GESTAO_29557,
                    message: vm.redundantRolesMessage,
                    buttons: {
                        ok: {
                            label: '<i class="fa fa-check"></i> Ok'
                        }
                    },
                    callback () {
                        vm.$router.replace({ name: 'users', params: { culture: vm.currentLang, system: vm.currentYear } });
                    }
                });
            }
        },
        checkRepeated(role, added) {

        //find the correct module inside the user to insert the role
            this.selectedUsers.forEach(user => {
                if(added) {
                    this.roleCheck(user, role, false)
                    if(this.result.boolean){
                        user.Repeated.push(role)

                    }
                }
                else {
                    this.roleCheck(user, role, true)
                        if(this.result.boolean){
                            var entry = 0;
                            user.Repeated.forEach(element => {
                                    if(element.Module === role.Module && element.Role === role.Role){
                                        user.Repeated.splice(entry, 1);
                                    }
                                })
                            entry += 1;
                        }
                }
            });
        },

        bindConfigAdd() {

            //find the correct module inside the user to insert the role
            this.selectedUsers.forEach(user => {
                this.selectedRoles.forEach(role => {
                    this.roleIsAssigned(user, role)
                    if(!this.result.boolean)
                        user.AssignedRoles[role.Module].push(role)
                });
                this.submit(user, user.CodUser);
            });

            this.displayInvalidActions(true);  //display invalid assignments/removals
            this.fetchData(); //update the list after remove
        },

        bindConfigRemove() {

            //find the correct module inside the user to remove the role
            this.selectedUsers.forEach(user => {
                this.selectedRoles.forEach(role => {
                    this.roleIsAssigned(user, role)
                    if(this.result.boolean)
                        user.AssignedRoles[role.Module].splice(this.result.index,1);
                    else
                        this.invalidRemovals.push({ user: user,
                                role: role
                        });
                });
                this.submit(user, user.CodUser);
            });

            this.displayInvalidActions(false);  //display invalid assignments/removals
            this.fetchData(); //update the list after insert
        },




        //Modules Section

        //get all distinct modules
        getDistinctModules() {
            var vm = this;
            var temp = vm.Roles.rows.filter((value, index, self) =>
                index === self.findIndex((t) => (
                    t.Module === value.Module && t.ModuleName === value.ModuleName
                )
                )
            );
            temp.splice(0, 0, { Module: "All", ModuleName: vm.Resources.MODULOS17298 });
			vm.modules = temp;
        },
        //fetch all data (users and roles)
        fetchData() {
            var vm = this;
            vm.fetchUsers();
            vm.fetchRoles();
        },
    },
    mounted(){
        var vm = this;
        vm.fetchData();
    },
    watch: {
        'selectedModule': 'getRolesByModule'
    }
}
</script>
