<template>
    <row>
		<q-card
			class="q-card--admin-default"
			:title="resources.schedulerTitle"
			width="block">
			<q-row-container>
				<q-control-wrapper class="row-line-group">
					<base-input-structure
						class="i-text">
						<q-checkbox
							v-model="model.Scheduler.Enabled"
							:label="resources.enabledLabel" />
					</base-input-structure>
				</q-control-wrapper>
				<qtable
					:rows="jobs"
					:columns="tJobs.columns"
					:config="tJobs.config"
					:totalRows="tJobs.total_rows"
					class="q-table--borderless">

					<template #actions="props">
						<q-button-group borderless>
							<q-button
								variant="text"
								:title="hardcodedTexts.edit"
								@click="changeJob(props.row)">
								<q-icon icon="pencil" />
							</q-button>
							<q-button
								variant="text"
								:title="hardcodedTexts.delete"
								@click="deleteJob(props.row)">
								<q-icon icon="bin" />
							</q-button>
						</q-button-group>
					</template>
					<template #table-footer>
						<tr>
							<td colspan="4">
								<q-button
									:label="hardcodedTexts.insert"
									@click="createJob">
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
		id="system_setup_scheduledjob"
		v-model="showDialog"
		:title="resources.scheduledTaskTitle"
		:buttons="buttons">
		<template #body.content>
			<div class="q-dialog-container">
				<div>
					<q-checkbox
						v-model="rowEnabled"
						:readonly="inDeleteMode"
						:label="resources.enabledLabel" />
				</div>
				<div>
					<q-text-field
						v-model="rowId"
						:label="hardcodedTexts.name"
						:readonly="inEditMode || inDeleteMode"
						size="xlarge"
						required />
				</div>
				<div>
					<q-select
						v-model="rowTaskType"
						:items="ScheduledJobSelect"
						:label="hardcodedTexts.taskTypeLabel"
						:readonly="inDeleteMode"
						item-value="Value"
						item-label="Text"
						size="xlarge" />
				</div>
				<div>
					<base-input-structure
						:label="'Cron'"
						:id="'CronField'"
						:isVisible="true"
						:showPopoverButton="true"
						:popoverTitle="'Cron Information'"
						:popoverText="resources.cronInfoText">
						<q-text-field
							v-model="rowCron"
							ref="Cron"
							:readonly="inDeleteMode"
							:size="'xlarge'"
							required
							placeholder="cron schedule" />
					</base-input-structure>
				</div>
				<div v-for="c in TaskList[rowTaskType]" :key="c.PropertyName">
					<q-text-field
						v-model="rowOptions[c.PropertyName]"
						:label="c.DisplayName"
						:readonly="inDeleteMode"
						:required="!c.Optional"
						size="xlarge"
						:helpText="c.Description"/>
				</div>
			</div>
		</template>
	</q-dialog>
</template>

<script>
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import BaseInputStructure from '@/components/BaseInputStructure.vue'
	import { texts } from '@/resources/hardcodedTexts.ts';
	import { computed} from 'vue';

	export default {
		name: 'scheduler',

		components: {
			BaseInputStructure
		},

		mixins: [reusableMixin],

		emits: ['alert-class', 'update-model'],

		props: {
			model: {
				required: true
			},
			TaskList: {
				required: false
			},
			resources: {
				type: Object,
				required: true
			}
		},

		data () {
			return {
				showDialog: false,
				jobs: [],
				buttons: [],
				dialogMode: '',
				rowOptions: {},
				rowCron: '',
				rowEnabled: false,
				rowTaskType: '',
				rowId: '',
				temp: {},
				alert: {
					isVisible: false,
					alertType: 'info',
					message: ''
				},
				tJobs: {
					total_rows: 0,
					columns: [
					{
						label: this.resources.actions,
						name: "actions",
						slot_name: "actions",
						sort: false,
						column_classes: "thead-actions",
						row_text_alignment: 'text-center',
						column_text_alignment: 'text-center'
					},
					{
						label: computed(() => this.Resources[texts.active]),
						name: "Enabled",
						sort: true,
					},
					{
						label: computed(() => this.Resources[texts.name]),
						name: "Id",
						sort: true,
						initial_sort: true,
						initial_sort_order: "asc"
					},
					{
						label: computed(() => this.Resources[texts.taskTypeLabel]),
						name: "TaskType",
						sort: true
					},
					{
						label: () => "Cron",
						name: "Cron",
						sort: false
					}],
					config: {
						table_title: this.resources.scheduledTasksTitle,
						pagination: false,
						pagination_info: false,
						global_search: {
							visibility: false
						}
					}
				},
			}
		},

		computed: {
			inDeleteMode() {
				return this.dialogMode === 'delete';
			},
			inEditMode() {
				return this.dialogMode === 'edit';
			},
			ScheduledJobSelect() {
				return Object.keys(this.TaskList).map(x => ({
					Text: x,
					Value: x
				}));
			},
			hardcodedTexts() {
				return {
					edit: this.Resources[texts.edit],
					delete: this.Resources[texts.delete],
					insert: this.Resources[texts.insert],
					erase: this.Resources[texts.erase],
					save: this.Resources[texts.save],
					cancel: this.Resources[texts.cancel],
					taskTypeLabel: this.Resources[texts.taskTypeLabel],
					name: this.Resources[texts.name],
				}
			},
		},

		methods: {
			SaveScheduledJob() {
				const schedulerValues = {
					Data: {
						Id: this.rowId,
						Cron: this.rowCron,
						Enabled: this.rowEnabled,
						TaskType: this.rowTaskType,
						Options: this.rowOptions || {}
					},
					FormMode: this.dialogMode,
				}

				QUtils.postData('Config', 'SaveScheduledJob', schedulerValues, null, (data) => {
					if (!data.Success) {
						this.$emit('alert-class', { ResultMsg: data.Message, AlertType: 'danger' });
					}

					this.clearSchedulerValues()
					// Update model data
					this.$emit('update-model')
				});
			},

			clearSchedulerValues() {
				this.dialogMode = ''
				this.rowOptions = {}
				this.rowCron = ''
				this.rowEnabled = false
				this.rowTaskType = ''
				this.rowId = ''
				this.buttons = []
			},

			showScheduledJobModal(mode, jobs) {
				this.dialogMode = mode
				this.getButtonsDialog()
				this.showDialog = true
			},
			getButtonsDialog() {
				switch(this.dialogMode) {
					case 'delete':
						this.buttons.push({
							id: 'delete-btn',
							props: {
								label: this.hardcodedTexts.erase,
								variant: 'bold',
								color: 'danger'
							},
							action: () => {
								this.SaveScheduledJob()
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
								this.SaveScheduledJob()
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
					action: () => this.clearSchedulerValues()
				})
			},
			changeJob(jobs) {
				this.rowId = jobs.Id
				this.rowOptions = jobs.Options || {}
				this.rowCron = jobs.Cron
				this.rowEnabled = jobs.Enabled
				this.rowTaskType = jobs.TaskType
				this.showScheduledJobModal('edit', jobs);
			},
			deleteJob(jobs) {
				this.rowId = jobs.Id
				this.rowOptions = jobs.Options
				this.rowCron = jobs.Cron
				this.rowEnabled = jobs.Enabled
				this.rowTaskType = jobs.TaskType
				this.showScheduledJobModal('delete', jobs);
			},
			createJob() {
				let jobs = {
					rowId: '',
					TaskType: '',
					rowCron: '',
					rowEnabled: true,
					rowOptions: {}
				};
				this.showScheduledJobModal('new', jobs);
			}
		},

		mounted() {
			this.jobs = this.model.Scheduler.Jobs || [];
		},

		watch: {
			rowTaskType(newTaskType) {
				if (!this.rowOptions) {
					this.rowOptions = {};
				}
				this.TaskList[newTaskType]?.forEach(task => {
					if (!this.rowOptions.hasOwnProperty(task.PropertyName)) {
						this.rowOptions[task.PropertyName] = '';
					}
				});
			},
			model: {
				handler(newModel) {
					this.jobs = newModel.Scheduler.Jobs || [];
				},
				deep: true
			}
		}
	}
</script>
