<template>
	<row>
		<q-card
			id="system_setup_elasticsearch_container"
			:title="resources.elasticsearchTitle"
			width="block"
			class="q-card--admin-default">
			<q-row-container>
				<qtable
					:rows="core"
					:columns="cfgCores.columns"
					:config="cfgCores.config"
					:totalRows="cfgCores.total_rows"
					class="q-table--borderless">
					<template #actions="props">
					<q-button-group borderless>
						<q-button
							variant="text"
							:title="hardcodedTexts.edit"
							@click="editCore(props.row)">
							<q-icon icon="pencil" />
						</q-button>
						<q-button
							variant="text"
							:title="hardcodedTexts.delete"
							@click="deleteCore(props.row)">
							<q-icon icon="bin" />
						</q-button>
					</q-button-group>
					</template>
					<template #table-footer>
						<tr>
							<td colspan="2">
							<q-button
								:label="hardcodedTexts.insert"
								@click="createCore">
								<q-icon icon="add" />
							</q-button>
							</td>
						</tr>
					</template>
				</qtable>
			</q-row-container>
		</q-card>
	</row>

	<q-dialog
		v-model="showDialog"
		:title="resources.elasticsearchSearchEngineTitle"
		:buttons="buttons">
		<template #body.content>
			<div class="q-dialog-container">
				<q-text-field
					v-model="rowIndex"
					:label="hardcodedTexts.indexLabel"
					:readonly="inEditMode || inDeleteMode"
					required
					size="large" />
				<q-text-field
					v-model="rowId"
					:label="hardcodedTexts.id"
					:readonly="inEditMode || inDeleteMode"
					required
					size="large"/>
				<q-text-field
					v-model="rowArea"
					:label="hardcodedTexts.areaLabel"
					:readonly="inDeleteMode"
					size="large" />
				<q-text-field
					v-model="rowUrlfscrawler"
					:label="resources.fscrawlerLabel"
					:readonly="inDeleteMode"
					size="large" />
				<q-text-field
					v-model="rowUrl"
					:label="hardcodedTexts.url"
					:readonly="inDeleteMode"
					size="large" />
				<q-text-field
					v-model="rowElasticUser"
					:label="hardcodedTexts.user"
					:readonly="inDeleteMode"
					size="large" />
				<password-input
					v-model="rowElasticPsw"
					:label="hardcodedTexts.password"
					:isReadOnly="inDeleteMode"
					size="large">
				</password-input>
			</div>
		</template>
	</q-dialog>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import { texts } from '@/resources/hardcodedTexts.ts';
	import { computed } from 'vue';

	export default {
		name: 'elasticsearch',

		props: {
			model: {
				required: true
			},
			Cores: {
				required: true
			},
			SelectLists: {
				required: true
			},
			resources: {
				type: Object,
				required: true
			}
		},

		mixins: [reusableMixin],

		emits: ['update-model', 'alert-class'],

		data() {
			return {
				showDialog: false,
				buttons: [],
				core: [],
				dialogMode: '',
				rowIndex: '',
				rowId: '',
				rowArea: '',
				rowUrl: '',
				rowUrlfscrawler: '',
				rowElasticUser: '',
				rowElasticPsw: '',
				rowNum: 0,
				cfgCores: {
					rows: [],
					columns: [{
						label: this.resources.actions,
						name: "actions",
						slot_name: "actions",
						sort: false,
						column_classes: "thead-actions",
						row_text_alignment: 'text-center',
						column_text_alignment: 'text-center'
					},
					{
						label: computed(() => this.Resources[texts.indexLabel]),
						name: "Index",
						sort: false
					},
					{
						label: computed(() => this.Resources[texts.id]),
						name: "Id",
						sort: false
					},
					{
						label: computed(() => this.Resources[texts.areaLabel]),
						name: "Area",
						sort: false
					},
					{
						label: this.resources.fscrawlerLabel,
						name: "Urlfscrawler",
						sort: false
					},
					{
						label: computed(() => this.Resources[texts.url]),
						name: "Url",
						sort: false
					},
					{
						label: computed(() => this.Resources[texts.user]),
						name: "ElasticUser",
						sort: false
					}],
					config: {
						table_title: this.resources.elasticsearchSearchEngineTitle,
						global_search: {
						classes: "qtable-global-search",
						showRefreshButton: true,
						searchDebounceRate: 1000
						},
						server_mode: false,
						preservePageOnDataChange: true
					},
					queryParams: {
						sort: [],
						filters: [],
						global_search: "",
						per_page: 10,
						page: 1,
					}
				},
			};
		},
		computed: {
			inDeleteMode() {
				return this.dialogMode === 'delete';
			},
			inEditMode() {
				return this.dialogMode === 'edit';
			},
			hardcodedTexts() {
				return {
					edit: this.Resources[texts.edit],
					delete: this.Resources[texts.delete],
					insert: this.Resources[texts.insert],
					id: this.Resources[texts.id],
					user: this.Resources[texts.user],
					password: this.Resources[texts.password],
					url: this.Resources[texts.url],
					erase: this.Resources[texts.erase],
					save: this.Resources[texts.save],
					cancel: this.Resources[texts.cancel]
				}
			},
		},
		methods: {
			showCoreModal(mode) {
				this.dialogMode = mode;
				this.getButtonsDialog();
				this.showDialog = true;
			},
			createCore() {
				var url = QUtils.apiActionURL('Config', 'GetNewCoreCfg');
				QUtils.FetchData(url).done((data) => {
					this.showCoreModal('new');
				});
			},
			editCore(core) {
				this.rowId = core.Id
				this.rowIndex = core.Index
				this.rowArea = core.Area
				this.rowElasticPsw = core.ElasticPsw
				this.rowElasticUser = core.ElasticUser
				this.rowUrl = core.Url
				this.rowUrlfscrawler = core.Urlfscrawler
				this.rowNum = core.Rownum,
				this.showCoreModal('edit');
			},
			deleteCore(core) {
				this.rowId = core.Id
				this.rowIndex = core.Index
				this.rowArea = core.Area
				this.rowElasticPsw = core.ElasticPsw
				this.rowElasticUser = core.ElasticUser
				this.rowUrl = core.Url
				this.rowUrlfscrawler = core.Urlfscrawler
				this.rowNum = core.Rownum,
				this.showCoreModal('delete');
			},
			SaveCoreCfg() {
				const coreValues = {
					FormMode: this.dialogMode,
					Id: this.rowId,
					Index: this.rowIndex,
					Area: this.rowArea,
					ElasticPsw: this.rowElasticPsw,
					Url: this.rowUrl,
					Urlfscrawler: this.rowUrlfscrawler,
					ElasticUser: this.rowElasticUser,
					Rownum: this.rowNum
				}
				QUtils.postData('Config', 'SaveCoreCfg', coreValues, null, (data) => {
					if (data.Success) {
						switch (coreValues.FormMode) {
						case 'new':
							this.core.push(
							{
								FormMode: this.dialogMode,
								Id: this.rowId,
								Index: this.rowIndex,
								Area: this.rowArea,
								ElasticPsw: this.rowElasticPsw,
								Url: this.rowUrl,
								Urlfscrawler: this.rowUrlfscrawler,
								ElasticUser: this.rowElasticUser,
								Rownum: this.core.length
							}
						)
						break;
						case 'edit':
							const newCoreIndex = this.core.findIndex(value => value.Id == this.rowId)
							this.core[newCoreIndex].Index = this.rowIndex;
							this.core[newCoreIndex].Area = this.rowArea;
							this.core[newCoreIndex].ElasticPsw = this.rowElasticPsw;
							this.core[newCoreIndex].Url = this.rowUrl;
							this.core[newCoreIndex].Urlfscrawler = this.rowUrlfscrawler;
							this.core[newCoreIndex].ElasticUser = this.rowElasticUser;
							this.core[newCoreIndex].Rownum = this.rowNum;
							break;
						case 'delete':
							this.core = this.core.filter(prop => prop.Id != this.rowId).sort((a, b) => a.rowNum - b.rowNum);
							this.core.forEach((core, idx) => {
								core.Rownum = idx
							})
							break;
						default:
							break;
						}
						// Update model data
						this.$emit('update-model')
					}

					this.clearCoreValues()
				});
			},
			getButtonsDialog() {
				switch(this.dialogMode) {
					case 'delete':
						this.buttons.push({
							id: 'delete-btn',
							props: {
								label: this.hardcodedTexts.erase,
								variant: 'bold',
								color: "danger"
							},
							action: () => {
								this.SaveCoreCfg()
							}
						});
						break;
					case 'edit':
					case 'new':
						this.buttons.push({
							id: 'save-btn',
							props: {
								label: this.hardcodedTexts.save,
								variant: 'bold',
								disabled: this.invalidProps
							},
							action: () => {
								this.SaveCoreCfg()
							}
						});
						break;
					default:
						break;
					}

				this.buttons.push({
					id: 'cancel-btn',
					props: {
						label: this.hardcodedTexts.cancel
					},
					action: () => this.clearCoreValues()
				})
			},
			clearCoreValues(){
				this.dialogMode = ''
				this.rowId = ''
				this.rowIndex = '',
				this.rowArea = '',
				this.rowElasticPsw = '',
				this.rowUrl = '',
				this.rowUrlfscrawler = ''
				this.rowElasticUser = ''
				this.buttons = []
			}
		},
		mounted() {
			this.core = this.model.Cores || [];
		}
	};
</script>
