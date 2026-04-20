<template>
	<div v-if="!(showDefaultDialog && isErrorDialog)">
		<q-card
			class="q-card--admin-default"
			:title="resources.displayTexts"
			width="block">
			<div class="data-systems__inputs">
				<q-select
					v-model="model.DefaultYear"
					size="small"
					:items="defaultDsItems"
					:label="resources.defaultDataSystem">
					<template #extras>
						<q-icon icon="information-outline" />
						{{ resources.defaultDataSystemInfo }}
					</template>
				</q-select>
				<q-checkbox
					v-model="model.HideYears"
					:label="resources.hideDataSystems">
					<template #extras>
						<q-icon icon="information-outline" />
						{{ resources.hideDataSystemsInfo }}
					</template>
				</q-checkbox>
			</div>
		</q-card>

		<div
			v-if="notConfiguredDataSystems"
			class="q-table__alert">
			<q-alert
				type="warning"
				:dismissTime="1.5"
				:title="resources.invalidDataSystems"
				:text="resources.invalidDataSystemsAlert" />
		</div>

		<qtable
			:rows="dataSystems"
			:columns="tableConfig.columns"
			:config="tableConfig.config"
			:classes="dsTableRowClasses"
			class="q-table--borderless">

			<template #actions="props">
				<div class="q-table__actions-btns">
					<q-button
						variant="text"
						:title="hardcodedTexts.configure"
						@click="configureDataSystem(props.row)">
						<q-icon icon="pencil" />
					</q-button>
					<q-button
						variant="text"
						:title="hardcodedTexts.duplicate"
						@click="duplicateDataSystem(props.row)">
						<q-icon icon="duplicate" />
					</q-button>
					<q-button
						variant="text"
						:title="hardcodedTexts.erase"
						:disabled="props.row.Year === configDefaultDs"
						@click="confirmDelete(props.row)">
						<q-icon icon="bin" />
					</q-button>
				</div>
			</template>
			<template #table-footer>
				<tr>
					<td colspan="5">
						<q-button :label="resources.createNewDataSystem"
							@click="showDataSystemDialog" />
					</td>
				</tr>
			</template>
		</qtable>

		<hr>

		<row>
			<q-button
				variant="bold"
				:label="hardcodedTexts.saveConfiguration"
				@click="SaveConfigDataSystems" />
		</row>
	</div>

	<q-dialog
		v-model="showDefaultDialog"
		:title="isErrorDialog ? hardcodedTexts.error : ''"
		:icon="defaultDialogIcon"
		:text="defaultDialogText"
		:buttons="defaultDialogButtons" />

	<q-dialog
		v-model="showNewSystemDialog"
		:title="resources.createNewDataSystem"
		:buttons="newSystemDialogButtons">
		<template #body.content>
			<div
				class="data-systems__dialog--container" >
				<q-text-field
					v-model="newDsName"
					:class="{ 'data-systems__inputs--with-errors': invalidNewDataSystemName }"
					size="block"
					:label="resources.dataSystemName">
					<template #extras v-if="invalidNewDataSystemName">
						<q-icon icon="information-outline" />
						{{ resources.dataSystemNameUniqueInfo }}
					</template>
				</q-text-field>
				<q-text-field
					v-model="newDsSchema"
					:class="{ 'data-systems__inputs--with-errors': invalidNewDbName }"
					size="block"
					:label="resources.databaseName">
					<template #extras v-if="invalidNewDbName">
						<q-icon icon="information-outline" />
						{{ resources.databaseNameUniqueInfo }}
					</template>
				</q-text-field>
			</div>
		</template>
	</q-dialog>

</template>
<script>
	// Utils
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils'

	import { texts } from '@/resources/hardcodedTexts.ts';

	import { mapGetters } from 'vuex'

	export default {
		name: 'datasystems',

		emits: ['change-tab', 'alert-class'],

		mixins: [reusableMixin],

		data() {
			return {
				/**
				 * The default data system of the application. Unlike model.DefaultYear, this
				 * represents what is saved in the configuration file - not the current QSelect value.
				 */
				configDefaultDs: '',

				/**
				 * The data systems (Years) of the application.
				 */
				dataSystems: [],

				/**
				 * True if the default dialog is to be shown, false otherwise.
				 */
				showDefaultDialog: false,

				/**
				 * True if the default dialog is an error dialog, false otherwise.
				 */
				isErrorDialog: false,

				/**
				 * Text to be displayed in the default dialog.
				 */
				defaultDialogText: '',

				/**
				 * Icon to be displayed in the default dialog.
				 */
				defaultDialogIcon: {},

				/**
				 * Action buttons of the default dialog.
				 */
				defaultDialogButtons: [],

				/**
				 * True if the new data system dialog is to be shown, false otherwise.
				 */
				showNewSystemDialog: false,

				/**
				 * Action buttons of the new data system dialog.
				 */
				newSystemDialogButtons: [],

				/**
				 * The name (Year) of the data system to be created. Used in the "Create a data system" dialog.
				 */
				newDsName: '',

				/**
				 * The database name (Schema) of the data system to be created. Used in the "Create a data system" dialog.
				 */
				newDsSchema: '',

				/**
				 * The database type (SQLServer, SQLite, ...) of the data system to be created. Used in the "Create a data system" dialog when duplicating.
				 */
				newDsType: '',

				/**
				 * The server of the data system to be created. Used in the "Create a data system" dialog when duplicating.
				 */
				newDsServer: '',

				/**
				 * Checks the validity of the default data system. True if no data system.Year coincides with the default, false otherwise.
				 */
				invalidDefaultDataSystem: false,

				/**
				 * The configuration of the data systems table.
				 */
				tableConfig: {
					columns: [
						{
							label: this.resources.actions,
							name: 'actions',
							sort: false,
							column_classes: 'thead-actions',
							row_text_alignment: 'text-center',
							column_text_alignment: 'text-center'
						},
						{
							label: this.resources.dataSystemName,
							name: 'Year',
							sort: true,
							initial_sort: true,
							initial_sort_order: "asc"
						},
						{
							label: this.resources.databaseName,
							name: 'DbName',
							sort: true
						},
						{
							label: this.resources.serverName,
							name: 'DbServer',
							sort: true
						},
						{
							label: this.resources.databaseVersion,
							name: 'DbVersion',
							sort: false
						},
						{
							label: '',
							name: 'DbType',
							sort: false,
							visibility: false
						},
						{
							label: '',
							name: 'Configured',
							sort: false,
							visibility: false
						}
					],
					config: {
						table_title: this.resources.dataSystems,
						pagination: this.showDsTablePagination,
						pagination_info: this.showDsTablePagination,
						per_page: 5,
						highlight_row_hover: false,
						global_search: {
							visibility: false
						}
					}
				}
			}
		},

		props: {
			/**
			 * Application and database metadata.
			 */
			model: {
				type: Object,
				required: true
			},

			/**
			 * WebAdmin texts.
			 */
			resources: {
				type: Object,
				required: true
			}
		},

		mounted() {
			this.initDataSystems()
		},

		computed: {
			...mapGetters(['Years']),
			/**
			 * Items to display for default data system selection.
			 */
			defaultDsItems() {
				return this.dataSystems.filter(ds => ds.Configured)
					.map(ds => ({ key: ds.Year, label: ds.Year }))
					.sort((a, b) => a.key - b.key)
			},

			/**
			 * Show pagination in the data systems table if there are more than 5 records.
			 */
			showDsTablePagination() {
				return this.dataSystems.length > 5
			},

			/**
			 * Classes to apply to the table rows, based on if the data system is correctly configured.
			 */
			dsTableRowClasses() {
				return {
					row:
					{
						'q-table__row-warning': (row) => !row.Configured
					}
				}
			},

			/**
			 * True if there are data systems in the table that are not yet configured, false otherwise.
			 */
			notConfiguredDataSystems() {
				return this.dataSystems.some(ds => !ds.Configured)
			},

			/**
			 * True if the name of the data system to create is not valid (already exists), false otherwise.
			 */
			invalidNewDataSystemName() {
				return this.dataSystems.map(ds => ds.Year.toLowerCase()).includes(this.newDsName.toLowerCase())
			},

			/**
			 * True if the name of the database of the data system to create is not valid (already exists), false otherwise.
			 */
			invalidNewDbName() {
				return this.dataSystems.map(ds => ds.DbName.toLowerCase()).includes(this.newDsSchema.toLowerCase())
			},


			/**
			 * True if the data system to create is not valid (missing or repeated information), false otherwise.
			 */
			invalidNewDataSystem() {
				return this.invalidNewDataSystemName
					|| this.invalidNewDbName
					|| this.newDsName === ''
					|| this.newDsSchema === ''
			},

			hardcodedTexts() {
				return {
					configure: this.Resources[texts.configure],
					duplicate: this.Resources[texts.duplicate],
					erase: this.Resources[texts.erase],
					saveConfiguration: this.Resources[texts.saveConfiguration],
					error: this.Resources[texts.error],
					cancel: this.Resources[texts.cancel],
					ok: this.Resources[texts.ok],
					changesSavedSuccess: this.Resources[texts.changesSavedSuccess],
					create: this.Resources[texts.create]
				}
			}
		},

		methods: {
			/**
			 * Initializes the tab with the necessary information.
			 */
			initDataSystems() {
				// Set "new Data System" dialog buttons
				this.newSystemDialogButtons = [
					{
						id: 'create-ds-btn',
						props: {
							variant: 'bold',
							label: this.hardcodedTexts.create,
							disabled: this.invalidNewDataSystem
						},
						action: () => this.createDataSystem()
					},
					{
						id: 'cancel-ds-btn',
						props: {
							label: this.hardcodedTexts.cancel
						},
						action: () => this.clearNewDataSystemInfo()
					}
				]

				// Server request to fetch data systems information
				this.fetchDataSystemsInfo()
			},

			/**
			 * Server request that fetches all necessary information regarding the existing data systems.
			 */
			fetchDataSystemsInfo() {
				var fetchUrl = QUtils.apiActionURL('DataSystems', 'GetDataSystemsInfo');
				QUtils.log('fetchDataSystemsInfo - Request: ', fetchUrl)

				QUtils.FetchData(fetchUrl).done((data) => {
					if (data.Success) {
						// Sets the current default data system value
						this.configDefaultDs = this.model.DefaultYear
						this.dataSystems = data.data.dataSystemsInfo
					}
					else {
						const dialogButtons = [{
							id: 'close-dialog-btn',
							props: {
								variant: 'bold',
								label: this.hardcodedTexts.ok
							},
							action: () => {
								this.$router.push({
									name: 'dashboard', params: { culture: this.currentLang, system: this.currentYear }
								})
							}
						}]

						this.setDefaultDialog(data.message, dialogButtons, null, true)
					}
				})
			},

			/**
			 * Save changes to the data systems configuration - Default Year and Hide Years.
			 */
			SaveConfigDataSystems() {
				var postUrl = QUtils.apiActionURL('Config', 'SaveConfigDataSystems')
				QUtils.log("SaveConfigDataSystems - Request", postUrl);

				QUtils.postData('Config', 'SaveConfigDataSystems', this.model, null, (data) => {
					QUtils.log("SaveConfigDataSystems - Response", data);

					if (data.Success) {
						this.configDefaultDs = this.model.DefaultYear
						this.$emit('alert-class', { ResultMsg: this.hardcodedTexts.changesSavedSuccess, AlertType: 'success' });
					}
					else
						this.$emit('alert-class', { ResultMsg: data.Message, AlertType: 'danger' });
				});
			},

			/**
			 * Creates a new data system and appends it to the list of existing ones.
			 */
			createDataSystem() {
				var postUrl = QUtils.apiActionURL('Config', 'CreateDataSystem')
				QUtils.log("CreateDataSystem - Request", postUrl)

				// New data system parameters
				const newDsInfo = {
					year: this.newDsName,
					schema: this.newDsSchema,
					type: this.newDsType,
					server: this.newDsServer
				}

				QUtils.postData('Config', 'CreateDataSystem', newDsInfo, null, (data) => {
					QUtils.log("CreateDataSystem - Response", data);

					// Update database years
					// Note: Updating the list of years triggers an update of the years' information.
					this.updateDataSystemList()
				});

				// Clear information from previously created data system
				this.clearNewDataSystemInfo()
			},

			/**
			 * Deletes an existing data system.
			 */
			deleteDataSystem(dataSystem) {
				var postUrl = QUtils.apiActionURL('Config', 'DeleteDataSystem')
				QUtils.log("DeleteDataSystem - Request", postUrl)

				QUtils.postData('Config', 'DeleteDataSystem', dataSystem, null, (data) => {
					QUtils.log("DeleteDataSystem - Response", data);

					if (data.ResultMsg) {
						this.$emit('alert-class', { ResultMsg: data.ResultMsg, AlertType: 'danger' });
						return
					}

					this.dataSystem = this.dataSystems.filter(ds => ds.Year != data.system)
					let message = this.resources.dataSystemDeletedSuccess

					// Swap to default data system if needed
					if (this.currentYear == data.system) {
						this.$router.replace({ name: 'system_setup', params: { culture: this.currentLang, system: this.model.DefaultYear } })
						message += ` ${this.resources.currentDataSystemDefault}`
					}

					// Note: Updating the list of years triggers an update of the years' information.
					this.updateDataSystemList()

					const dialogIcon = {
						icon: 'check-circle-outline',
						color: 'success'
					}
					this.setDefaultDialog(message, null, dialogIcon, false)
				})
			},

			/**
			 * Duplicates an existing data system and appends its copy to the list of existing ones.
			 */
			duplicateDataSystem(row) {
				this.newDsType = row.DbType
				this.newDsServer = row.DbServer

				this.showDataSystemDialog()
			},

			/**
			 * Changes the currently selected data system to the one clicked on the table row and navigates to the database configuration tab.
			 */
			configureDataSystem(row) {
				// Change current data system to the one in the clicked row
				this.$router.replace({ name: 'system_setup', params: { culture: this.currentLang, system: row.Year } })

				this.$emit('change-tab', 'tabGroup', 'selectedTab', 'database-tab')
			},

			updateDataSystemList() {
				this.$eventHub.emit('app_updateYear')
			},

			confirmDelete(row) {
				const message = `${this.resources.confirmDeleteDataSystem} <b>${row.Year}</b>?`
				const buttons = [
					{
						id: 'delete-btn',
						props: {
							variant: 'bold',
							color: 'danger',
							label: this.hardcodedTexts.erase
						},
						icon: { icon: 'bin' },
						action: () => this.deleteDataSystem(row.Year)
					},
					{
						id: 'cancel-btn',
						props: {
							label: this.hardcodedTexts.cancel
						},
						icon: { icon: 'cancel' }
					},
				]

				const dialogIcon = {
					icon: 'alert',
					color: 'warning'
				}
				this.setDefaultDialog(message, buttons, dialogIcon, false)
			},

			/**
			 * Shows the dialog to create a new data system.
			 */
			showDataSystemDialog() {
				this.showNewSystemDialog = true
			},

			/**
			 * After creating a new data system, clears the information on the dialog inputs.
			 */
			clearNewDataSystemInfo() {
				this.newDsName = ''
				this.newDsSchema = ''
				this.newDsType = ''
				this.newDsServer = ''
			},

			resetDefaultDialogInfo() {
				this.defaultDialogText = ''
				this.defaultDialogIcon = {}
				this.defaultDialogButtons = [
					{
						id: 'ok-btn',
						props: {
							variant: 'bold',
							label: this.hardcodedTexts.ok
						}
					}
				]

				this.isErrorDialog = false
				this.showDefaultDialog = false
			},

			setDefaultDialog(message, buttons, icon, isError) {
				this.resetDefaultDialogInfo()

				this.defaultDialogText = message

				if (buttons)
					this.defaultDialogButtons = buttons

				if (icon)
					this.defaultDialogIcon = icon

				this.isErrorDialog = isError
				this.showDefaultDialog = true
			}
		},

		watch: {
			Years() {
				this.fetchDataSystemsInfo()
			},
			/**
			 * Adapts the table pagination based on the number of data systems to present
			 */
			showDsTablePagination(newValue) {
				this.tableConfig.config.pagination = newValue
				this.tableConfig.config.pagination_info = newValue
			},

			/**
			 * Disables/enables the dialog button to create a new data system based on the correctness of the new system information
			 */
			invalidNewDataSystem(newValue) {
				this.newSystemDialogButtons[0].props.disabled = newValue
			}
		}
	}
</script>
