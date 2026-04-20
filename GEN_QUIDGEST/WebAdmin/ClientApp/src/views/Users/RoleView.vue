<template>
    <div id="role_container">
        <div class="q-stack--column">
			<h1 class="f-header__title">
			{{ Resources.GESTAO_DE_UTILIZADOR06990 }}
			</h1>
		</div>
		<hr>
        <q-card
            class="q-card--admin-default"
            width="block"
            :title="Resources.DESCRICAO_DA_FUNCAO21982">
            <q-row-container>
                <div class="description-container">
                    <q-badge
                        variant="bold" >
                        {{  getDesignation() }}
                    </q-badge>
                    <div class="module-container">
                        <span>{{ Resources.MODULO59907 }}:</span><br />
                        <span>{{ Model.ModuleDescription }}</span><br />
                    </div>
                    <div v-if="Model.Description" class="module-container">
                        <span>{{ Resources.DESIGNACAO25444 }}:</span><br />
                        <span>{{ Model.Description }}</span>
                    </div>
                </div>
                <q-row-container>
                    <qtable
                        :rows="Model.UserAboveList"
                        :columns="userList.userListColumns"
                        :config="userList.config"
                        class="q-table--borderless">
                        <!--Action column-->
                        <template #actions="props">
                            <q-button-group borderless>
                                <q-button
                                    variant="text"
                                    :title="Resources.ELIMINAR21155"
                                    @click="deleteUser(props.row)">
                                    <q-icon icon="bin" />
                                </q-button>
                            </q-button-group>
                        </template>
                        <template #role="props">
                            <q-badge
                                variant="bold" >
                                {{$t(props.row.Designation)}}
                            </q-badge>
                        </template>
                    </qtable>
                    <q-button
                        variant="bold"
                        :label="Resources.ATRIBUIR_UTILIZADORE53600"
                        @click="assignUsers" />
                </q-row-container>
                <q-card
                    class="q-card--admin-default"
                    width="block"
                    :title="Resources.HIERARQUIA22557">
                    <q-row-container>
                        <div class="col-sm">
                            <svg class="graph" ref="svg">
                                <g ref="graph"/>
                            </svg>
                        </div>
                    </q-row-container>
                </q-card>
                <row class="footer-btn">
                    <q-button
                    :label="Resources.VOLTAR01353"
                    @click.stop="navigateTo($event, 'users', false)" />
                </row>
            </q-row-container>
		</q-card>
    </div>
    <q-dialog
        v-model="showAsignUsers"
        :title="Resources.SELECIONE_OS_UTILIZA16987"
        dismissible
        :buttons="confirmButtons">
        <template #body.content>
            <div class="q-dialog-container">
                <qtable
                    ref="userAsignTable"
                    :rows="userAsign.rows"
                    :columns="userAsign.columns"
                    :config="userAsign.config"
                    class="q-table--borderless">
                </qtable>
            </div>
        </template>
    </q-dialog>
    <q-dialog
        class="alert-dialog"
		v-model="showDialog"
		:text="dialogText"
		:icon='{"icon":"check-circle-outline"}'
        :buttons="dialogBtns" />
</template>

<script>
    // @ is an alias to /src
    import { reusableMixin } from '@/mixins/mainMixin';
    import { QUtils } from '@/utils/mainUtils';
    import { PathUtils } from '@/utils/PathFinder';
    import dagreD3 from "dagre-d3";
    import * as d3 from "d3";

	export default {
		name: 'role-view',
		mixins: [reusableMixin],
        emits: ['alert-class'],
		mounted() {
            this.$nextTick(async () => {
                await this.fetchRole();
                this.redrawGraph();
            });
		},
		created() {
			// Ler dados
			this.fetchRole();
		},
        data() {
            return {
                showDialog: false,
                showAsignUsers: false,
                confirmButtons: [],
                dialogBtns: [],
                dialogText: "",
                Model: {
                    Parents: [],
                    Children : [],
                    ModuleDescription: '',
                    Description: ''
                },
                graphic: [{
                    name: String,
                    imports: [],
                    class: String
                }],
                userList: {
                    userListColumns: [
                        {
                            label: () => this.$t('ACOES22599'),
                            name: "actions",
                            slot_name: "actions",
                            sort: false,
                            column_classes: "thead-actions",
                            row_text_alignment: 'text-center',
                            column_text_alignment: 'text-center'
                        },
                        {
                            label: () =>  this.$t('UTILIZADOR52387'),
                            name: "UserName",
                            sort: true,
                            initial_sort: true,
                            initial_sort_order: "asc"
                        },
                        {
                            label: () => this.$t('ULTIMA_ALTERACAO22785'),
                            name: "ChangedDate",
                            sort: true,
                            initial_sort: false,
                        },
                        {
                            label: () => this.$t('ROLE60946'),
                            name: "role",
                            slot_name : "role",
                            sort: false,
                        },
                    ],
                    config: {
                        table_title: () => this.$t('UTILIZADORES39761'),
                        global_search: {
                            visibility: false,
                        }
                    }
                },
                userAsign: {
                    columns: [
                        {
                            label: () =>  this.$t('UTILIZADOR52387'),
                            name: "UserName",
                            sort: true,
                            initial_sort: true,
                            initial_sort_order: "asc"
                        }
                    ],
                    config: {
                        table_title: () => this.$t('TODOS_OS_UTILIZADORE41512'),
                        global_search: {
                            classes: "qtable-global-search",
							placeholder : this.$t('PESQUISAR_UTILIZADOR60804'),
							searchOnPressEnter: true,
							showRefreshButton: true,
							searchDebounceRate: 1000,
                            size: 'large'
                        },
                        checkbox_rows: true
                    }
                },
                graph: null
            };
        },
    methods: {
        getDesignation() {
            const designationKey = this.Model?.Designation;

            if (!designationKey) {
                return this.Resources.A_CARREGAR___34906;  
            }

            const translated = this.$t(designationKey);
            return translated !== designationKey ? translated : designationKey;
        },
        async fetchRole() {
            var vm = this;
            QUtils.log("Fetch data - Role");
			let data = await QUtils.FetchData(QUtils.apiActionURL('Role', 'GetRole', { module: vm.$route.params.module, roleId: vm.$route.params.role }));
			QUtils.log("Fetch data - OK (Role)");
			vm.Model = data;
			vm.RoleOnly = 'only';
        },
        async viewRole(role) {
            //In vue, instead of reloading the page, you're supposed to reload the data
            this.$route.params.role = role;
            await this.fetchRole();
            this.resetGraph();
            this.redrawGraph();
        },
        assignRole() {
            this.$router.push({
                name: 'assign_role',
                params: { role: this.$route.params.role, module: this.$route.params.module, culture: this.currentLang, system: this.currentYear }
            });
        },
        assignUsers() {
            const module = this.$route.params.module;
            const roleId = this.$route.params.role;
            const url = QUtils.apiActionURL('Role', 'GetUsersForModule', { module, roleId });
            QUtils.FetchData(url, true).then((data) => {
                if (data.Success) {
                    this.userAsign.rows = data.UserList;
                    this.getButtons() 
                    this.showAsignUsers = true;
                } else {
                    this.$emit('alert-class', { ResultMsg: this.Resources.ERRO_AO_CARREGAR_UTI62612, AlertType: 'danger' });
                }
            });
        },
        getDialogText(action) {
            switch (action) {
                case 'delete':
                    this.dialogText = this.Resources.UTILIZADOR_EXCLUIDO_17794;
                    break;
                case 'add':
                    this.dialogText = this.Resources.UTILIZADOR_ADICIONAD13862;
                    break;
                default:
                    break;
            }
        },
        getButtons() {
            this.confirmButtons = [
                {
                    id: 'confirm-btn',
                    props: {
                        label: this.Resources.ASSOCIAR58485,
                        variant: 'bold'
                    },
                    action: () => {
                        this.submit()
                        
                    }
                },
                {
                    id: 'cancel-btn',
                    props: {
                        label: this.Resources.CANCELAR49513
                    }
                }
            ]
        },
        async submit() {
            const selectedUsers = this.$refs.userAsignTable.selected_items;
            if (selectedUsers.length === 0) {
                $emit('alert-class', { ResultMsg: data.Message, AlertType: 'danger' });
            }
            const users = selectedUsers.map((user) => user.Codpsw).join(',');
            const module = this.$route.params.module;
            const roleId = this.$route.params.role;
            QUtils.postData('ManageUsers', 'AssignRoleToUsers', null, { users, module, roleId }, (data) => {
                if (data.Success) {
                    this.fetchRole();
                    this.getShowDialog('add')
                } else {
                    $emit('alert-class', { ResultMsg: data.Message, AlertType: 'danger' });
                }
            });
        },
        async deleteUser(row) {
            const cod = row.Codpsw;
            const module = this.$route.params.module;
            const roleId = this.$route.params.role;
            QUtils.postData('ManageUsers', 'RemoveUserRole', null, { cod, module, roleId }, (data) => {
                if (data.Success) {
                    this.fetchRole()
                    this.getShowDialog('delete');
                }
                else {
                    this.$emit('alert-class', { ResultMsg: data.Message, AlertType: 'danger' });
                }
            });
        },
        getShowDialog(action) {
            this.dialogBtns = [
                {
                    id: 'confirm-btn',
                    props: {
                        label: this.Resources.OK57387,
                        variant: 'bold'
                    },
                    action: () => {
                        if (!this.Model.Username) {
                            this.$router.replace({ name: 'users', params: { culture: this.currentLang, system: this.currentYear } });
                            return;
                        }
                        this.$router.replace({ name: 'users', params: { culture: this.currentLang, system: this.currentYear } });
                    }
                }
            ]
            this.getDialogText(action);
            this.showDialog = true
        },
        resetGraph() {
            while(this.graphic.length > 1){
                this.graphic.pop();
            }
        },
        redrawGraph: async function(){

            var svg = d3.select(this.$refs.svg);

            this.graph = new dagreD3.graphlib.Graph();

            this.graph.setGraph({});

            this.graph.graph().rankdir = "TB";
            this.graph.graph().ranker = "network-simplex"; //network-simplex, longest-path, tight-tree

            //graph represents the nodes of the graph
            //graphic represents the connections of the graph

            //give the right designation to each node
            this.Model.Children.forEach(child => {
                child.Designation = this.$t(child.Designation)
            });

            var vm = this;
            //Setup the label for the origin and its edges( imports )
                this.graph.setNode(this.$t(this.Model.Designation), {label: this.$t(this.Model.Designation), class: "origin"});
                vm.graphic.push({
                        name: vm.$t(vm.Model.Designation),
                        imports: vm.Model.Children,
                        class: "origin"
                });

            //Setup the children of the selected role and its edges
            //For each child we also fetch its children
            for(const element of this.Model.Children) {
                vm.graph.setNode(this.$t(element.Designation), {label: this.$t(element.Designation), class: "dests"});

                QUtils.log("Fetch data - Role");
				let data = await QUtils.FetchData(QUtils.apiActionURL('Role', 'GetRole', { module: element.Module, roleId: element.Role }));
                    QUtils.log("Fetch data - OK (Role)");
                    data.Children.forEach(child => {
                        child.Designation = vm.$t(child.Designation)
                    });

                    vm.graphic.push({
                        name: vm.$t(element.Designation),
                        imports: data.Children,
                        class: "dests"
                    });
            }

            //Setup the Parents of the selected role and its edges
            //For each parent we also fetch its children
            for(const element of this.Model.Parents) {
                vm.graph.setNode(this.$t(element.Designation), {label: this.$t(element.Designation), class: "sources"});

                QUtils.log("Fetch data - Role");
				let data = await QUtils.FetchData(QUtils.apiActionURL('Role', 'GetRole', { module: element.Module, roleId: element.Role }));
                    QUtils.log("Fetch data - OK (Role)");
                    data.Children.forEach(child => {
                        child.Designation = vm.$t(child.Designation)
                    });

                    vm.graphic.push({
                        name: vm.$t(element.Designation),
                        imports: data.Children,
                        class: "dests"
                    });
            }
            this.graphic.shift();


            // The calculated graphic is used to calculate the reachability matrix
            this.reach = PathUtils.reachabilityMatrix(this.graphic);
            this.updateEdges();

            // Set up zoom support

            this.zoom = d3.zoom().on("zoom", this.nodeZoomHandler);

            svg.call(this.zoom);
            svg.on("dblclick.zoom", null);

            // Create the renderer
            this.updateRender();

            //set up node click event
            svg.selectAll("g.node")
                .on("dblclick", this.nodeDblClickHandler);
                //.on("click", this.nodeClickHandler);
        },
        updateEdges() {
            //remove all the previous edges
            this.graph.edges().forEach( edge=> {
                this.graph.removeEdge(edge.v, edge.w);
            }, this);

            // setup the edges using the calculated graphic
            this.graphic.forEach( i=> {
                i.imports.forEach( src=> {
                    if(this.graph.node(this.$t(src.Designation)))
                    {
                        //the calculated reach is used to check if a edge is redundant
                        if(PathUtils.isPrimaryEdge(this.reach, i.name, this.$t(src.Designation)))
                        {
                            var clEdge = "";
                            var wgEdge = 1;
                            this.graph.setEdge(i.name, this.$t(src.Designation), {label:"", curve: d3.curveBasis, class: clEdge, weight: wgEdge });
                        }
                    }
                }, this);
            }, this);
        },
        updateRender() {

            var svg = d3.select(this.$refs.svg);
            this.inner = svg.select("g");
            var render = new dagreD3.render();
            // Run the renderer. This is what draws the final graph.

            render(this.inner, this.graph);

            //set scale
            this.fitSize();

        },
        fitSize() {
            var width = this.$refs.svg.clientWidth;

            // Center the graph
            //var initialScale = 0.75;
            //resize to fit
            var initialScale = (width * 1.0) / (this.graph.graph().width + 50.0);

            if(initialScale > 0.75)
                initialScale = 0.75;
            this.rescale(initialScale);
        },
        rescale(newScale) {
            var svg = d3.select(this.$refs.svg);

            var width = this.$refs.svg.clientWidth;

            // Center the graph
            this.zoom.transform(svg, d3.zoomIdentity
                .translate((width - this.graph.graph().width * newScale) / 2, 20)
                .scale(newScale)
                );

            //minimize the height of the container
            svg.attr('height', this.graph.graph().height + 40);

        },
        nodeDblClickHandler (d) {

            this.Model.Children.forEach(element => {
                if(this.$t(element.Designation) == d.path[1].__data__){
                    this.viewRole(element.Role)
                    return;
                }
            })

            this.Model.Parents.forEach(element => {
                if(this.$t(element.Designation) == d.path[1].__data__){
                    this.viewRole(element.Role)
                    return;
                }
            })
        },
        nodeZoomHandler (e) {
            this.$refs.graph.setAttribute("transform", e.transform);
        }
    }
  };
</script>
