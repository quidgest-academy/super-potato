<template>
	<div id="log_details_container">
		<!-- Page header -->
		<div class="q-stack--column">
			<h1 class="f-header__title">
				{{ Resources.MAINTENANCE_DETAILS01126 }}
			</h1>
		</div>
		<hr>

		<!-- Page content -->
		<q-card
			v-if="!loaded"
			width="block"
			loading />

		<template v-else>
			<row>
				<q-card
					v-if="generalInfo"
					class="q-card--admin-default log-details__overview"
					:title="Resources.RESUMO13953"
					width="block">
					<static-text
						:model-value="generalInfo.Success ? Resources.SIM28552 : Resources.NAO06521"
						:label="Resources.TAREFA_BEM_SUCEDIDA33448"
						bold-label />
					<static-text
						:model-value="parseActionDuration(generalInfo)"
						:label="Resources.DURATION40426"
						bold-label />
					<static-text
						v-model="generalInfo.DataSystem"
						:label="Resources.NOME_DO_SISTEMA_DE_D18974"
						bold-label />
					<static-text
						v-model="generalInfo.Database"
						:label="Resources.NOME_DA_BD63025"
						bold-label />
					<static-text
						:model-value="generalInfo.StartTime"
						:label="Resources.STARTED_AT44034"
						bold-label />
				</q-card>
			</row>

			<q-card
				class="log-details__wrapper"
				width="block">
				<div class="log-details__content">
					<!-- Maintenance task list -->
					<q-card
						class="q-card--admin-default q-card--admin-fill"
						:title="Resources.TAREFAS24474"
						width="xxlarge">
						<div class="log-details__task-list">
							<q-accordion
								variant="border-bottom">
								<q-collapsible
									v-for="(group, idx) in taskGroups"
									:model-value="idx === 0"
									class="q-collapsible--admin-default"
									:title="group.Name"
									spacing="compact"
									width="block">
									<q-button
										v-for="task in getGroupTasks(group)"
										class="q-button--admin-start"
										variant="text"
										borderless
										block
										:label="task.Description"
										:active="task === selectedTask"
										@click="selectTask(task)">
										<q-icon
											:icon="isSuccessfulTask(task) ? 'check' : 'close'"
											:color="isSuccessfulTask(task) ?
												'success' :
												'danger'"/>
									</q-button>
								</q-collapsible>
							</q-accordion>
						</div>
					</q-card>

					<!-- Maintenance task details -->
					<q-card
						class="q-card--admin-default q-card--admin-fill"
						:title="selectedTaskTitle"
						width="block">
						<template
							v-if="!noSelectedTask"
							#[`header.content.append`]>
							<q-button
								variant="text"
								borderless
								@click="clearTaskSelection">
								<q-icon icon="close" />
							</q-button>
						</template>

						<div
							v-if="noSelectedTask"
							class="log-details__no-task">
							<q-icon :icon="noSelectedTaskIcon" />
							<span>
								{{ noSelectedTaskMessage }}
							</span>

							<template
								v-if="errorMessage">
								<br />
								<q-button
									:label="Resources.VOLTAR01353"
									@click="goBack">
									<q-icon icon="arrow-left-bold" />
								</q-button>
							</template>
						</div>

						<div
							v-else
							class="log-details__task-details">
							<template
								v-if="selectedTask.Result != ''">
								<static-text
									v-model="selectedTask.Result"
									:label="Resources.ERRO38355"
									bold-label />

								<br />
							</template>

							<q-collapsible
								v-for="script in selectedTask.Details"
								class="q-collapsible--admin-plain"
								:key="script.ScriptId"
								variant="border-bottom"
								spacing="compact"
								width="block"
								:title="script.ScriptId"
								:subtitle="parseActionDuration(script)">
								<div class="script-details__block--extended">
									<div
										v-for="block in script.ScriptDetails"
										:key="block.ScriptId"
										:class="{ 
											'script-details__line': true, 
											'script-details__line-error': block.Result 
										}">
										<span>
											Line {{ block.ScriptId }}
										</span>
										<div>
											<q-icon
												icon="timer-outline" />
											<span>
												{{ parseActionDuration(block) }}
											</span>
										</div>
									</div>
								</div>
							</q-collapsible>
						</div>
					</q-card>
				</div>
				
				<row class="footer-btn">
					<q-button
						:label="Resources.VOLTAR01353"
						@click="goBack">
						<q-icon icon="arrow-left-bold" />
					</q-button>
				</row>
			</q-card>
		</template>
	</div>
</template>
<script>
	import { reusableMixin } from '@/mixins/mainMixin'
	import { QUtils } from '@/utils/mainUtils'

	export default {
		mixins: [reusableMixin],

		data() {
			return {
				/**
				* Essential info of the current maintenance job.
				*/
				generalInfo: null,

				/**
				* Groups of maintenance tasks.
				*/
				taskGroups: [],

				/**
				* Maintenance task details.
				*/
				taskDetails: [],

				/**
				* Details of the selected maintenance task.
				*/
				selectedTask: null,

				/**
				* True if the page content is loaded, false otherwise.
				*/
				loaded: false,

				/**
				* Error message from the server, if it exists.
				*/
				errorMessage: ''
			}
		},

		props: {
			/**
			* ID of the current maintenance log.
			*/
			logId: {
				type: Number,
				required: true
			}
		},

		computed: {
			/**
			* True if no task is currently selected, false otherwise.
			*/
			noSelectedTask() {
				return !this.selectedTask
			},

			/**
			* Icon to present in the details card, when no task is selected.
			*/
			noSelectedTaskIcon() {
				return this.errorMessage ? 'close-thick' : 'archive-remove-outline'
			},

			/**
			* Message to present in the details card, when no task is selected.
			*/
			noSelectedTaskMessage() {
				return this.errorMessage ? this.errorMessage : this.Resources.NO_DATA_TO_DISPLAY_F23828
			},

			/**
			* Title to present in the details card.
			*/
			selectedTaskTitle() {
				return this.noSelectedTask ? '' : `${this.Resources.DETALHES04088}: ${this.selectedTask.Description}`
			}
		},

		mounted() {
			this.fetchLogDetails()
		},

		methods: {
			/**
			* Fetches the information of the current maintenance log, based on its ID.
			*/
			fetchLogDetails() {
				const apiUrl = QUtils.apiActionURL('DbAdmin', 'GetMaintenanceLogDetails')

				QUtils.log("Request", apiUrl);
				QUtils.postData('DbAdmin', 'GetMaintenanceLogDetails', this.logId, null, (details) => {
					QUtils.log("Response", details);

					if(details.ResultMsg) {
						this.errorMessage = details.ResultMsg
						this.loaded = true
						return
					}

					// From all tasks and groups, isolate the ones that have been run in this particular maintenance
					this.taskDetails = this.getExecutedTasks(details.FunctionDetails)
					this.taskGroups = this.getTaskGroups(details.FunctionGroups)
					this.generalInfo = details.Info

					this.loaded = true
				})
			},

			/**
			* Filters the tasks array, returning the ones that have been executed in the current maintenance job.
			*/
			getExecutedTasks(tasks) {
				return tasks.filter(task => this.formatDate(task.LastRun) != '-')
			},

			/**
			* Filters the task groups array, returning the ones that contain tasks that have been executed in the current maintenance job.
			*/
			getTaskGroups(groups) {
				const taskIds = this.taskDetails.map(func => func.Id)

				return groups.filter(grp =>
					grp.GroupItems.some(item => taskIds.includes(item))
				);
			},

			/**
			* Filters the tasks array by task group.
			*/
			getGroupTasks(group) {
				return this.taskDetails.filter(func => group.GroupItems.includes(func.Id))
			},

			/**
			* Returns true if a task was successful, false otherwise.
			*/
			isSuccessfulTask(func) {
				return func.Result === ''
			},

			/**
			* Selects a task to show its details.
			*/
			selectTask(task) {
				this.selectedTask = task
			},

			/**
			* Unselects all tasks.
			*/
			clearTaskSelection() {
				this.selectedTask = null
			},

			/**
			* Parses an action duration from int (number of milliseconds) to string format.
			*/
			parseActionDuration(action) {
				return `${(action.Duration).toLocaleString('en')}ms`
			},

			/**
			* Returns to the maintenance tab.
			*/
			goBack() {
				this.$router.back()
			}
		}
	}
</script>

