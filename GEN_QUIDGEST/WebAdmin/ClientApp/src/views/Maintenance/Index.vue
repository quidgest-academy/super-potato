<template>
	<q-card
		v-if="!isLoaded && !showDialog"
		width="block"
		loading />
		
	<div v-else>
		<q-alert
			v-if="!isEmptyObject(currentModel.ResultMsg)"
			type="info"
			:text="currentModel.ResultMsg" />

		<row>
			<q-card
				class="q-card--admin-default"
				:title="Resources.DEFINICOES_DA_BASE_D46524"
				width="block">
				<q-row-container>
					<q-text-field
						v-model="currentModel.DbUser"
						:label="Resources.NOME_DE_UTILIZADOR58858"
						required
						size="xlarge"
						@keyup.enter="Reindex" />
					<password-input
						v-model="currentModel.DbPsw"
						:label="Resources.PALAVRA_PASSE44126"
						is-required
						size="xlarge"
						@keyup.enter="Reindex" />


					<br />

					<div v-if="isMultiYearApp">
						<q-card
							class="q-card--admin-border-top q-card--admin-compact"
							:title="Resources.INFORMACAO_DA_APLICA42351"
							variant="minor"
							width="block">
							<q-row-container>
								<static-text
									v-model="currentModel.VersionApp"
									bold-label
									orientation="vertical"
									:label="Resources.VERSAO_DA_APLICACAO45955" />
								<static-text
									v-model="currentModel.VersionReIdx"
									bold-label
									orientation="vertical"
									:label="Resources.VERSAO_DOS_SCRIPTS52566" />
								<static-text
									v-model="currentModel.VersionUpgrScripts"
									bold-label
									orientation="vertical"
									:label="Resources.VERSAO_DOS_SCRIPTS_D32532" />
							</q-row-container>
						</q-card>

						<div
							v-if="notReindexedDataSystems">
							<q-alert
								type="danger"
								:title="Resources.SISTEMAS_DE_DADOS_IN53544"
								:text="[
									Resources.EXISTEM_SISTEMAS_DE_48665,
									Resources.POR_FAVOR__ESCOLHA_U35243
								]" />
						</div>

						<qtable
							:rows="maintenanceModels"
							:columns="tableConfig.columns"
							:config="tableConfig.config"
							:classes="tableRowClasses"
							class="q-table--borderless"
							@on-select-row="updateSelectedModels"
							@on-unselect-row="updateSelectedModels">

							<template #StartTime="props">
								<span v-if="props.row.LastLogInfo">
									{{ props.row.LastLogInfo.StartTime }}
								</span>
							</template>
							<template #Success="props">
								<q-icon
									v-if="props.row.LastLogInfo"
									:icon="props.row.LastLogInfo.Success ? 'check' : 'close'"
									:color="props.row.LastLogInfo.Success  ?
										'success' :
										'danger'" />
							</template>
						</qtable>
					</div>
					<div v-else>
						<q-card
							class="q-card--admin-border-top"
							:title="Resources.INFO27076"
							variant="minor"
							width="block">
							<q-row-container>
								<static-text
									v-model="currentModel.DBSchema"
									bold-label
									orientation="vertical"
									:label="Resources.BASE_DE_DADOS58234" />
								<static-text
									v-model="currentModel.DBSize"
									bold-label
									orientation="vertical"
									:label="Resources.TAMANHO_DA_BD56664" />
							</q-row-container>
						</q-card>
						<q-card
							class="q-card--admin-border-top"
							:title="Resources.SCHEMA58822"
							variant="minor"
							width="block">
							<q-row-container>
								<static-text
									v-model="currentModel.VersionDb"
									:label="Resources.DATABASE_VERSION15344"
									bold-label />
								<static-text
									v-model="currentModel.VersionApp"
									bold-label
									orientation="vertical"
									:label="Resources.VERSAO_DA_APLICACAO45955" />
								<static-text
									v-model="currentModel.VersionReIdx"
									bold-label
									orientation="vertical"
									:label="Resources.VERSAO_DOS_SCRIPTS52566" />
							</q-row-container>
						</q-card>
						<q-card
							class="q-card--admin-border-top"
							:title="Resources.VERSAO61228"
							variant="minor"
							width="block">
							<q-row-container>
								<static-text
									v-model="currentModel.VersionUpgrIndx"
									bold-label
									orientation="vertical"
									:label="Resources.DATABASE_VERSION15344" />
								<static-text
									v-model="currentModel.VersionUpgrScripts"
									bold-label
									orientation="vertical"
									:label="Resources.APPLICATION_VERSION32207" />
							</q-row-container>
						</q-card>
					</div>
				</q-row-container>
			</q-card>
		</row>
		
		<row>
			<q-collapsible
				:title="Resources.OPCOES_AVANCADAS_DE_63606"
				class="q-collapsible--admin-default"
				width="block">
				<q-card
					v-if="currentModel.LastLogInfo"
					class="q-card--admin-default"
					:title="Resources.LAST_MAINTENANCE_JOB39831"
					variant="minor"
					width="block">
					<div class="database-options__last-rdx">
						<static-text
							:model-value="currentModel.LastLogInfo.Success ? Resources.SIM28552 : Resources.NAO06521"
							:label="Resources.TAREFA_BEM_SUCEDIDA33448"
							bold-label />
						<static-text
							:model-value="`${ currentModel.LastLogInfo.Duration }ms`"
							:label="Resources.DURATION40426"
							bold-label />
						<static-text
							v-model="currentModel.LastLogInfo.DataSystem"
							:label="Resources.NOME_DO_SISTEMA_DE_D18974"
							bold-label />
						<static-text
							v-model="currentModel.LastLogInfo.Database"
							:label="Resources.NOME_DA_BD63025"
							bold-label />
						<static-text
							:model-value="currentModel.LastLogInfo.StartTime"
							:label="Resources.STARTED_AT44034"
							bold-label />
					</div>
				</q-card>
				<br />
				<div class="database-options">
					<q-checkbox 
						v-model="currentModel.Zero" 
						:label="Resources.REINDEXACAO_COMPLETA51519" />
				</div>
				<template v-for="group in reindexGroups"
					:key="group.Id">
					<div class="database-options">
						<q-checkbox
							v-model="reindexGroupsState[group.Name]"
							:indeterminate="getGroupFunctions(group).some(func => func.Value)"
							:label="group.Name"
							@update:model-value="(newVal) => { groupValueUpdate(group, newVal) }" />
					</div>
					<div 
						style="text-align:left; margin-left: 20px;">
						<template 
							v-for="sqlFunc in getGroupFunctions(group)"
							:key="sqlFunc.Id">
							<div 
								v-if="sqlFunc.Selectable"
								class="database-options">
								<q-checkbox
									v-model="sqlFunc.Value"
									:label="sqlFunc.Description"
									:id="sqlFunc.Id"
									@update:model-value="itemValueUpdate(group)" />
								<q-button
									v-if="formatDate(sqlFunc.LastRun) != '-'"
									variant="text"
									size="small"
									:label="Resources._INFO15849"
									:title="sqlFunc.Result"
									@click="changeSelectedScript(sqlFunc)">
									<q-icon
										:color="sqlFunc.Result.length > 0 ? 'danger' : 'success'"
										:icon="sqlFunc.Result.length > 0 ? 'close' : 'check'" />
								</q-button>
							</div>
						</template>
					</div>
				</template>
			</q-collapsible>
		</row>

		<maintenance-history
			:load-history="loadHistory" />

		<row>
			<numeric-input v-model="currentModel.Timeout" :label="'Timeout'" size="small" />
		</row>

		<row class="footer-btn">
			<q-button
				variant="bold"
				:label="Resources.EXECUTAR_TAREFAS_DE_40767"
				@click="Reindex" />
		</row>

		<q-overlay
			v-model="showScriptOverlay"
			backdrop-blur
			scroll-lock
			persistent
			trigger="manual"
			@keydown="onDetailsKeydown">
			<div
				id="database-script-details"
				class="modal-content">
				<div class="modal-header">
					<h5 class="modal-title">
						{{ selectedScript.Description }}
					</h5>
					<q-button
						variant="text"
						borderless
						@click="closeOverlay">
						<q-icon icon="close" />
					</q-button>
				</div>
				<div class="modal-body">
					<static-text
						v-if="selectedScript.Result != ''"
						v-model="selectedScript.Result"
						:label="Resources.ERRO38355"
						bold-label />

					<q-accordion
						variant="border-bottom">
						<q-collapsible
							v-for="file in selectedScript.Details"
							:key="file.ScriptId"
							class="q-collapsible--admin-default"
							spacing="compact"
							width="block"
							:title="file.ScriptId"
							:subtitle="`${ (file.Duration).toLocaleString('en') } ms`">
							<div class="script-details__block">
								<div
									v-for="block in file.ScriptDetails"
									:key="block.ScriptId"
									:class="{ 
										'script-details__line': true, 
										'script-details__line-error': block.Result 
									}">
									<span>
										Line {{ block.ScriptId }}
									</span>
									<div>
										<q-icon icon="timer-outline" />
										<span>
											{{ (block.Duration).toLocaleString('en') }} ms
										</span>
									</div>
								</div>
							</div>
						</q-collapsible>
					</q-accordion>
				</div>
			</div>
		</q-overlay>

		<q-dialog
			v-model="showDialog"
			icon=""
			:title="isErrorDialog ? Resources.ERRO38355 : ''"
			:text="dialogText"
			:buttons="dialogButtons" />

		<progress-bar
			:show="dataPB.show"
			:text="dataPB.text"
			:subtext="dataPB.subtext"
			:progress="dataPB.progress"
			:with-button="true"
			:button-text="Resources.CANCELAR49513"
			@on-button-click="cancelReindex" />
	</div>
</template>

<script>
	import { cloneDeep } from 'lodash-es'

	import { reusableMixin } from '@/mixins/mainMixin'
	import { QUtils } from '@/utils/mainUtils'
	import { merge, shallowCopy } from '@/utils/object'

	import maintenanceHistory from '@/views/Maintenance/MaintenanceHistory'

	export default {
		name: 'maintenance_index',

		mixins: [reusableMixin],

		components: { maintenanceHistory },

		data() {
			// Resources aren't available while the local Vue instance isn't created -> access global instance
			var texts = this.$root.Resources

			return {
				/**
				 * All data systems in the application, along with information needed for database maintenance tasks.
				 * Static values, must only be used for consultation.
				 */
				maintenanceModels: [],

				/**
				 * Selected data systems from the data systems table. Used for multi-system maintenance.
				 */
				selectedModels: [],

				/**
				 * The currently selected data system.
				 */
				currentModel: {},

				/**
				 * Index of the data system that is currently running the maintenance tasks. Index refers to selectedModels array.
				 */
				currentMaintenanceIdx: 0,

				/**
				 * Selected models that have been prepared for a maintenance job. Used for multi-system maintenance.
				 */
				reindexModels: [],

				/**
				 * Groups of SQL routines needed for database maintenance.
				 */
				reindexGroups: {},

				/**
				 * The state of each reindex group: true for groups whose SQL routines have all been selected, false otherwise.
				 */
				reindexGroupsState: {},

				/**
				 * True if all information that the tab displays is loaded, false otherwise.
				 */
				isLoaded: false,

				/**
				 * Flag that inverts whenever the maintenance history needs to be updated.
				 */
				loadHistory: true,

				/**
				 * The script that is currently selected (information displayed in the script details overlay).
				 */
				selectedScript: undefined,

				/**
				 * True if the script details overlay is open, false otherwise.
				 */
				showScriptOverlay: false,

				/**
				 * Variables responsible for displaying messages or alerts: includes the message content, 
				 * buttons to display in the dialog and whether it should be displayed.
				 */
				dialogText: '',
				dialogButtons: [],
				showDialog: false,
				isErrorDialog: false,

				/**
				 * Variables responsible for displaying the progress bar: includes the current label, 
				 * the current progress and whether it should be displayed.
				 */
				dataPB: {
					show: false,
					text: '',
					subtext: '',
					progress: 0
				},

				/**
				 * True if the database maintenance has been cancelled, false otherwise.
				 */
				isCancelled: false,

				/**
				 * The configuration of the maintenance models table.
				 */
				tableConfig: {
					config: {
						table_title: texts.INFORMACAO_DAS_BASES08206,
						pagination: this.showTablePagination,
						pagination_info: this.showTablePagination,
						per_page: 5,
						highlight_row_hover: false,
						global_search: {
							visibility: false
						},
						checkbox_rows: true
					},
					columns: [
						{
							label: texts.NOME_DO_SISTEMA_DE_D18974,
							name: 'DSName',
							uniqueId: true,
							sort: true,
							initial_sort: true,
							initial_sort_order: "asc"
						},
						{
							label: texts.NOME_DA_BD63025,
							name: 'DBSchema',
							sort: true
						},
						{
							label: texts.TAMANHO_DA_BD56664,
							name: 'DBSize',
							sort: false
						},
						{
							label: texts.VERSAO_DA_BD12683,
							name: 'VersionDb',
							sort: false
						},
						{
							label: texts.VERSAO_DO_INDICE_DE_20717,
							name: 'VersionUpgrIndx',
							sort: false
						},
						{
							label: texts.LAST_MAINTENANCE_JOB39831,
							name: 'StartTime',
							sort: true
						},
						{
							label: texts.TAREFA_BEM_SUCEDIDA33448,
							name: 'Success',
							sort: false
						}
					]
				}
			};
		},

		computed: {
			/**
			* CSS classes for each table row.
			*/
			tableRowClasses() {
				return {
					row:
					{
						'q-table__row-error': (row) => row.VersionDb !== row.VersionApp || row.VersionUpgrIndx != row.VersionUpgrScripts
					}
				}
			},

			/**
			* True if there are data systems that need to execute the data maintenance tasks, false otherwise.
			*/
			notReindexedDataSystems() {
				return this.maintenanceModels.some(model => model.VersionDb != model.VersionApp || model.VersionUpgrIndx != model.VersionUpgrScripts)
			},

			/**
			* Shows pagination in the maintenance models table if there are more than 5 data systems.
			*/
			showTablePagination() {
				return this.maintenanceModels.length > 5
			}
		},

		mounted() {
			this.$eventHub.emit('hideDataSystems', true)
			// Check current maintenance progress
			this.checkProgress(null, true)
			
			this.fetchData()
		},

		beforeUnmount() {
			this.$eventHub.emit('hideDataSystems', false)
		},

		methods: {
			/**
			* Fetches all necessary data from the server.
			*/
			fetchData() {
				const apiUrl = QUtils.apiActionURL('DbAdmin', 'Index')

				QUtils.log("Fetch data - Maintenance");
				QUtils.FetchData(apiUrl).done((data) => {
					QUtils.log("Fetch data - OK (Maintenance)", data)

					if (data.length == 0) {
						// When no data systems are found, redirect users to the system configuration menu
						const message = this.Resources.NAO_FOI_POSSIVEL_DET43573
						const buttons = [{
							id: 'close-dialog-btn',
							props: {
								variant: 'bold',
								label: this.Resources.OK15819
							},
							action: () => {
								this.$router.push({
									name: 'system_setup', params: { culture: this.currentLang, system: this.currentYear }
								})
							}
						}]

						this.setDialog(message, buttons, true)
						return
					}

					this.setCurrentModel(data) // must be set before maintenanceModels to ensure the currentModel has reindex details
					this.resetFunctionGroups()
					this.maintenanceModels = data.map(this.getLiteModel)

					// After loading server info, set tab as ready and set maintenance history as outdated
					this.isLoaded = true
					this.loadHistory = !this.loadHistory
				})
			},

			/**
			* Executes database maintenance tasks on the selected data systems.
			*/
			Reindex() {
				this.dataPB.text = "Discovering database"
				this.dataPB.progress = 0

				if (!this.currentModel.Items.filter(e => e.Value === true && e.Selectable === true).length) {
					const message = this.Resources.NAO_FOI_SELECIONADO_45963
					this.setDialog(message, null, true)

					return
				}

				if (this.isMultiYearApp && !this.selectedModels.length) {
					const message = this.Resources.NENHUM_SISTEMA_DE_DA37349
					this.setDialog(message, null, true)

					return
				}

				this.isCancelled = false
				this.prepModels()

				this.startReindex(this.reindexModels)
			},

			/**
			* Starts the database maintenance tasks on a data system.
			*/
			startReindex(modelsToReindex) {
				const apiUrl = QUtils.apiActionURL('DbAdmin', 'Start')
				QUtils.log("Request", apiUrl)
				QUtils.postData('DbAdmin', 'Start', modelsToReindex, null, (data) => {
					QUtils.log("Response", data)
					if (!data.Success) {
						this.setDialog(data.Message)
						return
					}

					this.dataPB.show = true
					this.dataPB.progress = 1
					setTimeout(this.checkProgress, 250)
				});
			},
			/**
			* Cancels the ongoing database maintenance tasks.
			*/
			cancelReindex() {
				const apiUrl = QUtils.apiActionURL('DbAdmin', 'CancelReindex')

				QUtils.log("Request", apiUrl);
				QUtils.FetchData(apiUrl).done((data) => {
					QUtils.log("Request - OK (Maintenance - Cancel Reindexation)", data)
					if(!data.Success) {						
						const message = this.Resources.THERE_HAS_BEEN_AN_ER33167 + ":<br />"
						+ data.Message
						this.setDialog(message, null, true)

						return
					}

					//Show user that the cancel operation is running
					this.dataPB.text = 'Cancelling...'
					this.dataPB.progress = 100
					this.isCancelled = true
				});
			},

			/**
			* Checks the progress of the ongoing database maintenance tasks.
			*/
			checkProgress(callBack, firstCheck = false) {
				const apiUrl = QUtils.apiActionURL('DbAdmin', 'Progress')

				QUtils.FetchData(apiUrl).done((data) => {
					const isRunning = data.Status == 'RUNNING' || // still running
						!firstCheck && (data.Status == 'NOT_STARTED' || data.Status == 'SUCCESS') // in queue or intermediate success (e.g. success on model 1, but still needs to reindex models 2, 3)

					if (isRunning) {
						if(!this.isCancelled) {
							this.dataPB.text = `${this.Resources.SISTEMA_DE_DADOS12710}: ${data.ActualModel}`
							this.dataPB.subtext = `Script: ${data.ActualScript}`
							this.dataPB.progress = data.Count
							this.dataPB.show = true
						}
						
						setTimeout(() => this.checkProgress(callBack), 500)
						return
					}

					if(firstCheck) return

					if(data.Status == 'CANCELLED') {
						this.dataPB.show = false

						const message = this.Resources.OPERATION_CANCELLED_59653
						this.setDialog(message)

						this.fetchData()
						return
					}

					if (callBack) {
						callBack()
						return
					}

					if (data.Message) {
						this.setDialog(data.Message)
					}

					this.dataPB = { show: false, text: '', progress: 0, inProcess: false }
					this.fetchData()
				});
			},

			/**
			* Retrieves a lighter version of a maintenance model, without unnecessary details.
			*/
			getLiteModel(model) {
				const liteModel = shallowCopy(model)

				// Details are very heavy and cause lag in the user interface when used in the table
				if (liteModel.Items && Array.isArray(liteModel.Items))
					liteModel.Items = liteModel.Items.map(({ Details, ...itemInfo }) => itemInfo)

				return liteModel
			},

			/**
			* Updates the selected maintenance models array. Used for multi-system maintenance jobs.
			*/
			updateSelectedModels(selectedRows) {
				this.selectedModels = selectedRows.selected_items.map(row => row.DSName)
			},

			/**
			* Set the maintenance model to display according to the latest maintenance info.
			*/
			setCurrentModel(modelData) {
				let latestModel = null

				if (!this.isMultiYearApp || !modelData.some(model => model.LastLogInfo))
				{
					latestModel = modelData.find(model => model.DSName === this.currentYear)
				}
				else
				{
					// If there is maintenance history, the interface should be related to the latest maintenance executed
					latestModel = modelData.reduce((latest, model) => {
						const latestRdxTime = latest.LastLogInfo?.StartTime ? new Date(latest.LastLogInfo.StartTime) : new Date(0)
						const modelRdxTime = model.LastLogInfo?.StartTime ? new Date(model.LastLogInfo.StartTime) : new Date(0)

						return modelRdxTime > latestRdxTime ? model : latest
					})
				}

				// Current model cannot be the original array object - it must be a copy to use in the interface, since changes must not affect models
				this.currentModel = cloneDeep(latestModel)
			},

			/**
			* Prepares the selected models for a maintenance job.
			*/
			prepModels() {
				this.resetMaintenanceVariables()
				const liteCurrent = this.getLiteModel(this.currentModel)

				if (!this.isMultiYearApp) {
					this.reindexModels.push(liteCurrent)
					return
				}

				// maintenance models are already "lite", done during fetchData
				const selected = this.maintenanceModels.filter(m => this.selectedModels.includes(m.DSName))

				const mergeRules = {
					DbUser: true,
					DbPsw: true,
					DirFileStream: true,
					Items: true,
					Timeout: true,
					Zero: true
				}

				selected.forEach(model => {
					const prepped = merge(model, liteCurrent, mergeRules)
					this.reindexModels.push(prepped)
				})

				this.reindexModels.sort((a, b) => a.DSName.localeCompare(b.DSName, undefined, { numeric: true, sensitivity: 'base' }))
			},

			/**
			* Resets the maintenance variables to their defaults.
			*/
			resetMaintenanceVariables() {
				this.currentMaintenanceIdx = 0
				this.reindexModels = []
			},

			/**
			* Gets a specific function of the current maintenance model, based on its id.
			*/
			getFunction(sqlFunc) {
				return this.currentModel.Items.find(func => func.Id == sqlFunc)
			},

			/**
			* Gets all functions of a given SQL function group.
			*/
			getGroupFunctions(grpItem) {
				const items = []

				grpItem.GroupItems.forEach((sqlFunc) => {
					const item = this.getFunction(sqlFunc)

					if (item)
						items.push(item)
				})

				return items
			},

			/**
			* Verifies if all SQL functions in a group are checked.
			*/
			isGroupChecked(group) {
				return group.GroupItems.every(func => {
					const item = this.getFunction(func)
					if (item) {
						return item.Value
					}
					// Since not all functions in the groups are in the model reindex functions, disregard those that aren't
					return true
				})
			},

			/**
			* Resets the SQL function groups to their original state.
			*/
			resetFunctionGroups() {
				if (!this.currentModel)
					return

				this.reindexGroups = this.currentModel.reindexMenu.Reindexgroups.filter(rg => this.getGroupFunctions(rg).length > 0)

				this.reindexGroupsState = Object.fromEntries(
					this.reindexGroups.map(group => [group.Name, this.isGroupChecked(group)])
				)
			},

			/**
			* When checking the checkbox of a specific group, also checks all of the group's functions.
			*/
			groupValueUpdate(group, groupValue) {
				group.GroupItems.forEach(item => {
					const itemIdx = this.currentModel.Items.findIndex(func => func.Id == item)

					if (itemIdx > -1)
						this.currentModel.Items[itemIdx].Value = groupValue
				})
			},

			/**
			* When an SQL function is checked, verifies if the function's group is fully checked: if so, checks the group aswell.
			*/
			itemValueUpdate(itemGroup) {
				this.reindexGroupsState[itemGroup.Name] = this.isGroupChecked(itemGroup)
			},

			/**
			* Opens the script details overlay on a specific SQL function.
			*/
			changeSelectedScript(sqlFunc) {
				this.selectedScript = sqlFunc
				this.showScriptOverlay = true
			},

			/**
			* Closes the script details overlay.
			*/
			closeOverlay() {
				this.selectedScript = undefined
				this.showScriptOverlay = false
			},

			/**
			* Closes the script details overlay on 'Esc' press.
			*/
			onDetailsKeydown(event) {
				if (!event.key) return
				if ('Escape' === event.key)
					this.closeOverlay()
			},

			/**
			* Resets the contents of the dialog window.
			*/
			resetDialogVariables() {
				this.dialogButtons = [{
					id: 'ok-btn',
					props: {
						variant: 'bold',
						label: this.Resources.OK15819
					}
				}]
				this.dialogText = ''
				this.isErrorDialog = false
			},

			/**
			* Sets the contents of the dialog window and shows it.
			*/
			setDialog(message, buttons = null, isError = false) {
				this.resetDialogVariables()
				this.dialogText = message

				if (buttons)
					this.dialogButtons = buttons

				this.isErrorDialog = isError
				this.showDialog = true
			}
		},

		watch: {
			/**
			 * Adapts the table pagination based on the number of data systems to present
			 */
			showTablePagination(newValue) {
				this.tableConfig.config.pagination = newValue
				this.tableConfig.config.pagination_info = newValue
			},
		}
	}
</script>
