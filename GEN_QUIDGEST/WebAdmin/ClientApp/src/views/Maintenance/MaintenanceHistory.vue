<template>
	<row>
		<q-collapsible
			title="Maintenance History"
			class="q-collapsible--admin-default"
			width="block"
			@update:model-value="updateHistory">
			<qtable
				:rows="maintenanceHistory"
				:columns="tableConfig.columns"
				:config="tableConfig.config"
				class="q-table--borderless">

				<template #actions="props">
					<div class="q-table__actions-btns">
						<q-button
							variant="text"
							:title="Resources.DETALHES04088"
							@click="checkDetails(props.row.Id)">
							<q-icon icon="view" />
						</q-button>
					</div>
				</template>

			<template #Success="props">
				<q-icon
					:icon="props.row.Success ? 'check' : 'close'"
					:color="props.row.Success ?
						'success' :
						'danger'" />
			</template>
			</qtable>
		</q-collapsible>
	</row>
</template>
<script>
	import { reusableMixin } from '@/mixins/mainMixin'
	import { QUtils } from '@/utils/mainUtils'

	export default {
		mixins: [reusableMixin],

		data() {
			// Resources aren't available while the local Vue instance isn't created -> access global instance
			var texts = this.$root.Resources

			return {
				/**
				* Maintenance logs information.
				*/
				maintenanceHistory: [],

				/**
				* First load flag. Used to ensure that opening the collapsible for the first time will trigger the fetch method.
				*/
				firstLoad: true,

				/**
				 * The configuration of the maintenance history table.
				 */
				tableConfig: {
					config: {
						table_title: texts.ULTIMAS_20_ENTRADAS34032,
						pagination: this.showTablePagination,
						pagination_info: this.showTablePagination,
						per_page: 5,
						highlight_row_hover: false,
						global_search: {
							visibility: false
						}
					},
					columns: [
						{
							label: texts.ACOES22599,
							name: 'actions',
							sort: false,
							column_classes: 'thead-actions',
							row_text_alignment: 'text-center',
							column_text_alignment: 'text-center'
						},
						{
							label: texts.ORIGEM59855,
							name: 'Origin',
							sort: true
						},
						{
							label: texts.NOME_DO_SISTEMA_DE_D18974,
							name: 'DataSystem',
							sort: true
						},
						{
							label: texts.NOME_DA_BD63025,
							name: 'Database',
							sort: true
						},
						{
							label: texts.START_TIME30037,
							name: 'StartTime',
							sort: true,
							initial_sort: true,
							initial_sort_order: "desc"
						},
						{
							label: `${texts.DURATION40426} (ms)`,
							name: 'Duration',
							sort: true
						},
						{
							label: texts.TAREFA_BEM_SUCEDIDA33448,
							name: 'Success',
							sort: false
						}
					]
				}
			}
		},

		props: {
			/**
			* Flag that inverts whenever the maintenance history needs to be updated.
			*/
			loadHistory: {
				type: Boolean,
				default: true
			}
		},

		computed: {
			/**
			* Shows pagination in the maintenance history table if there are more than 5 logs.
			*/
			showTablePagination() {
				return this.maintenanceHistory.length > 5
			}
		},

		methods: {
			/**
			* If the maintenance history is outdated, updates it.
			*/
			updateHistory(openCollapsible) {
				if (openCollapsible && this.firstLoad) {
					this.fetchMaintenanceHistory()
					this.firstLoad = false
				}
			},

			/**
			* Fetches the last 20 maintenance logs information.
			*/
			fetchMaintenanceHistory() {
				const apiUrl = QUtils.apiActionURL('DbAdmin', 'GetMaintenanceLogs')

				QUtils.log("Request", apiUrl);
				QUtils.postData('DbAdmin', 'GetMaintenanceLogs', 20, null, (history) => {
					QUtils.log("Response", history);

					this.maintenanceHistory = history
				})
			},

			/**
			* Navigates to the log details page, according to the received logId.
			*/
			checkDetails(logId) {
				this.$router.push({ name: 'log_details', params: { logId: logId, culture: this.currentLang, system: this.currentYear } });
			}
		},

		watch: {
			/**
			* Re-fetch data from the server whenever a new database maintenance task is performed.
			*/
			loadHistory(newValue) {
				this.fetchMaintenanceHistory()
			},

			/**
			 * Adapts the table pagination based on the number of maintenance logs to present
			 */
			showTablePagination(newValue) {
				this.tableConfig.config.pagination = newValue
				this.tableConfig.config.pagination_info = newValue
			}
		}
	}
</script>
