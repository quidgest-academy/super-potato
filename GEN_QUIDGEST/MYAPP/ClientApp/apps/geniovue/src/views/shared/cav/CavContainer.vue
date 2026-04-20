<template>
	<div
		v-show="cavContainerOnLoadProc.loaded"
		class="c-g-reporting">
		<div class="c-action-bar">
			<q-icon icon="stats" />

			<h2 class="c-sidebar__tab-title">Reporting</h2>

			<nav class="navbar q-button-group">
				<ul class="navbar-nav">
					<li class="nav-item">
						<q-button
							borderless
							:title="texts.newReport"
							@click="createNewQuery">
							<q-icon icon="new-report" />
						</q-button>
					</li>

					<li class="nav-item">
						<cav-open-records @load-query="loadQuery" />
					</li>

					<li class="nav-item">
						<cav-save-query
							v-if="cavDataOnLoadProc.loaded"
							:query-id="currentQueryId"
							:title="model.Query.Title"
							@save-query="saveQuery" />
					</li>
				</ul>
			</nav>
		</div>

		<div class="cav-content">
			<div class="w-btn-horiz q-wizard-container">
				<div class="row-large-control">
					<div class="g-reporting__flow-tabs">
						<div class="container-fluid">
							<div class="row tabbable">
								<div class="col-12">
									<ul
										class="nav nav-tabs c-tab q-wizard__steps--horizontal"
										role="tablist">
										<li
											:class="[
												{ 'current-step': activeTab === 'fields' },
												'nav-item',
												'c-tab__item',
												'q-wizard__step',
												'required-step'
											]">
											<a
												id="cav-atab-fields"
												role="tab"
												aria-controls="fields"
												aria-selected="false"
												style="width: 100%"
												:class="[
													{ 'current-step': activeTab === 'fields' },
													'nav-link',
													'c-tab__item-header',
													'q-wizard__step-link'
												]"
												@click.prevent="showTab('fields')">
												<span class="q-wizard__step-number">1</span>
												{{ texts.fields }}
											</a>
										</li>

										<li :class="[{ 'current-step': activeTab === 'conditions' }, 'nav-item', 'c-tab__item', 'q-wizard__step']">
											<a
												id="cav-atab-conditions"
												role="tab"
												aria-controls="conditions"
												aria-selected="false"
												style="width: 100%"
												:class="[
													{ 'current-step': activeTab === 'conditions' },
													'nav-link',
													'c-tab__item-header',
													'q-wizard__step-link'
												]"
												@click.prevent="showTab('conditions')">
												<span class="q-wizard__step-number">2</span>
												{{ texts.conditions }}
											</a>
										</li>

										<li :class="[{ 'current-step': activeTab === 'groups' }, 'nav-item', 'c-tab__item', 'q-wizard__step']">
											<a
												id="cav-atab-groups"
												role="tab"
												aria-controls="groups"
												aria-selected="false"
												style="width: 100%"
												:class="[
													{ 'current-step': activeTab === 'groups' },
													'nav-link',
													'c-tab__item-header',
													'q-wizard__step-link'
												]"
												@click.prevent="showTab('groups')">
												<span class="q-wizard__step-number">3</span>
												{{ texts.groups }}
											</a>
										</li>

										<li :class="[{ 'current-step': activeTab === 'ordering' }, 'nav-item', 'c-tab__item', 'q-wizard__step']">
											<a
												id="cav-atab-ordering"
												role="tab"
												aria-controls="ordering"
												aria-selected="false"
												style="width: 100%"
												:class="[
													{ 'current-step': activeTab === 'ordering' },
													'nav-link',
													'c-tab__item-header',
													'q-wizard__step-link'
												]"
												@click.prevent="showTab('ordering')">
												<span class="q-wizard__step-number">4</span>
												{{ texts.ordering }}
											</a>
										</li>

										<li :class="[{ 'current-step': activeTab === 'totals' }, 'nav-item', 'c-tab__item', 'q-wizard__step']">
											<a
												id="cav-atab-totals"
												role="tab"
												aria-controls="totals"
												aria-selected="false"
												style="width: 100%"
												:class="[
													{ 'current-step': activeTab === 'totals' },
													'nav-link',
													'c-tab__item-header',
													'q-wizard__step-link'
												]"
												@click.prevent="showTab('totals')">
												<span class="q-wizard__step-number">5</span>
												{{ texts.totals }}
											</a>
										</li>

										<li
											:class="[
												{ 'current-step': activeTab === 'execute' },
												'nav-item',
												'c-tab__item',
												'q-wizard__step',
												'required-step'
											]">
											<a
												id="cav-atab-execute"
												role="tab"
												aria-controls="execute"
												aria-selected="false"
												style="width: 100%"
												:class="[
													{ 'current-step': activeTab === 'execute' },
													'nav-link',
													'c-tab__item-header',
													'q-wizard__step-link'
												]"
												@click.prevent="showTab('execute')">
												<span class="q-wizard__step-number">6</span>
												{{ texts.execute }}
											</a>
										</li>
									</ul>
								</div>
							</div>
						</div>

						<div class="container-fluid">
							<div class="row">
								<div class="col-12">
									<div class="c-tab__item-container g-reporting__flow-content">
										<div
											v-if="activeTab === 'fields'"
											id="cav-tab-fields"
											class="c-tab__item-content active"
											role="tabpanel"
											aria-labelledby="cav-atab-fields">
											<fields
												:fields-selected-list="model.FieldsSelectedList"
												:tables="model.Tables"
												:base-table-description="model.Query.BaseTableDescription"
												@add-cav-field="addFieldQuery"
												@remove-cav-field="removeFieldQuery" />

											<div class="mt-3 mb-1">
												<q-button
													variant="bold"
													icon-pos="end"
													:label="texts.goForward"
													:title="texts.goForward"
													@click="showTab('conditions')">
													<q-icon icon="step-forward" />
												</q-button>
											</div>
										</div>

										<div
											v-if="activeTab === 'conditions'"
											id="cav-tab-conditions"
											class="c-tab__item-content active"
											role="tabpanel"
											aria-labelledby="cav-atab-conditions">
											<conditions
												v-if="tabContentStayActive || cavDataOnLoadProc.loaded"
												:tables="model.Tables"
												:conditions-selected="model.Query.Condition"
												@update-conditions="updateCondition" />

											<div class="c-tab__item-container__actions">
												<q-button
													:label="texts.goBack"
													:title="texts.goBack"
													@click="showTab('fields')">
													<q-icon icon="step-back" />
												</q-button>

												<q-button
													variant="bold"
													icon-pos="end"
													:label="texts.goForward"
													:title="texts.goForward"
													@click="showTab('groups')">
													<q-icon icon="step-forward" />
												</q-button>
											</div>
										</div>

										<div
											v-if="activeTab === 'groups'"
											id="cav-tab-groups"
											class="c-tab__item-content active"
											role="tabpanel"
											aria-labelledby="cav-atab-groups">
											<groups
												v-if="tabContentStayActive || cavDataOnLoadProc.loaded"
												:tables="model.Tables"
												:fields-selected-list="model.FieldsSelectedList"
												:selected-groupings="model.Query.Groups"
												@update-groupings="updateGroupings" />

											<div class="c-tab__item-container__actions">
												<q-button
													:label="texts.goBack"
													:title="texts.goBack"
													@click="showTab('conditions')">
													<q-icon icon="step-back" />
												</q-button>

												<q-button
													variant="bold"
													icon-pos="end"
													:label="texts.goForward"
													:title="texts.goForward"
													@click="showTab('ordering')">
													<q-icon icon="step-forward" />
												</q-button>
											</div>
										</div>

										<div
											v-if="activeTab === 'ordering'"
											id="cav-tab-ordering"
											class="c-tab__item-content active"
											role="tabpanel"
											aria-labelledby="cav-atab-orderingab">
											<ordering
												v-if="tabContentStayActive || cavDataOnLoadProc.loaded"
												:tables="model.Tables"
												:fields-selected-list="model.FieldsSelectedList"
												:selected-orderings="model.Query.Orderings"
												@update-orderings="updateOrderings" />

											<div class="c-tab__item-container__actions">
												<q-button
													:label="texts.goBack"
													:title="texts.goBack"
													@click="showTab('groups')">
													<q-icon icon="step-back" />
												</q-button>

												<q-button
													variant="bold"
													icon-pos="end"
													:label="texts.goForward"
													:title="texts.goForward"
													@click="showTab('totals')">
													<q-icon icon="step-forward" />
												</q-button>
											</div>
										</div>

										<div
											v-if="activeTab === 'totals'"
											id="cav-tab-totals"
											class="c-tab__item-content active"
											role="tabpanel"
											aria-labelledby="cav-atab-totals">
											<totals
												v-if="tabContentStayActive || cavDataOnLoadProc.loaded"
												:tables="model.Tables"
												:total-fields-per-group="model.TotalFieldsPerGroup"
												:selected-totals="model.Totals"
												@update-totals="updateTotals" />

											<div class="c-tab__item-container__actions">
												<q-button
													:label="texts.goBack"
													:title="texts.goBack"
													@click="showTab('ordering')">
													<q-icon icon="step-back" />
												</q-button>

												<q-button
													variant="bold"
													icon-pos="end"
													:label="texts.goForward"
													:title="texts.goForward"
													@click="showTab('execute')">
													<q-icon icon="step-forward" />
												</q-button>
											</div>
										</div>

										<div
											v-if="activeTab === 'execute'"
											id="cav-tab-execute"
											class="c-tab__item-content active"
											role="tabpanel"
											aria-labelledby="cav-atab-execute">
											<execute
												v-if="tabContentStayActive || cavDataOnLoadProc.loaded"
												:query="model.Query" />

											<div class="c-tab__item-container__actions">
												<q-button
													:label="texts.goBack"
													:title="texts.goBack"
													@click="showTab('totals')">
													<q-icon icon="step-back" />
												</q-button>
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed, defineAsyncComponent } from 'vue'
	import { useRoute } from 'vue-router'
	import _assignIn from 'lodash-es/assignIn'
	import _findIndex from 'lodash-es/findIndex'
	import _isEmpty from 'lodash-es/isEmpty'

	import { loadResources } from '@/plugins/i18n.js'
	import { displayMessage } from '@quidgest/clientapp/utils/genericFunctions'
	import { postData, fetchData } from '@quidgest/clientapp/network'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import asyncProcM from '@quidgest/clientapp/composables/async'

	export default {
		name: 'QCavContainer',

		components: {
			CavOpenRecords: defineAsyncComponent(() => import('./OpenRecords.vue')),
			CavSaveQuery: defineAsyncComponent(() => import('./SaveQuery.vue')),
			Fields: defineAsyncComponent(() => import('./Fields.vue')),
			Conditions: defineAsyncComponent(() => import('./ConditionsSelected.vue')),
			Groups: defineAsyncComponent(() => import('./GroupBySelected.vue')),
			Ordering: defineAsyncComponent(() => import('./OrderBySelected.vue')),
			Totals: defineAsyncComponent(() => import('./Totals.vue')),
			Execute: defineAsyncComponent(() => import('./Execute.vue'))
		},

		provide()
		{
			return {
				getCavText: this.getCavText
			}
		},

		setup()
		{
			const route = useRoute()
			return {
				routeMetadata: computed(() => route.meta || null)
			}
		},

		expose: [],

		data()
		{
			return {
				cavContainerOnLoadProc: asyncProcM.getProcListMonitor('CavContainer', false),

				cavDataOnLoadProc: asyncProcM.getProcListMonitor('CavData', true),

				tabContentStayActive: false,

				toggleDetails: false,

				activeTab: 'fields',

				area: null,

				currentQueryId: null,

				model: {
					FieldsSelectedList: [],
					Tables: [],
					Query: {
						BaseTable: null,
						BaseTableDescription: null,
						DetailsGroup: {
							Fields: []
						},
						Title: ''
					}
				},

				texts: {
					newReport: computed(() => this.Resources[hardcodedTexts.newReport]),
					fields: computed(() => this.Resources[hardcodedTexts.fields]),
					conditions: computed(() => this.Resources[hardcodedTexts.conditions]),
					groups: computed(() => this.Resources[hardcodedTexts.groups]),
					ordering: computed(() => this.Resources[hardcodedTexts.ordering]),
					totals: computed(() => this.Resources[hardcodedTexts.totals]),
					execute: computed(() => this.Resources[hardcodedTexts.execute]),
					goForward: computed(() => this.Resources[hardcodedTexts.goForward]),
					goBack: computed(() => this.Resources[hardcodedTexts.goBack])
				}
			}
		},

		created()
		{
			this.cavContainerOnLoadProc.add(loadResources(this, ['cav', 'cavArrays']), true)
			this.$eventHub.on('set-culture', this.loadUIResources)
			this.$eventHub.on('add-cav-field', this.addFieldQuery)
		},

		beforeMount()
		{
			if (_isEmpty(this.area))
				this.area = this.getCurrentInterfaceArea()
		},

		mounted()
		{
			// Fetches the query model
			this.fetchModel()
		},

		beforeUnmount()
		{
			this.$eventHub.off('set-culture', this.loadUIResources)
			this.$eventHub.off('add-cav-field', this.addFieldQuery)

			this.$eventHub.emit('main-container-is-visible', true)

			this.cavContainerOnLoadProc.destroy()
			this.cavDataOnLoadProc.destroy()
		},

		methods: {
			loadUIResources()
			{
				loadResources(this, ['cav', 'cavArrays'])
			},

			showTab(tabId)
			{
				this.activeTab = tabId
			},

			getCurrentInterfaceArea()
			{
				if (!_isEmpty(this.routeMetadata) && !_isEmpty(this.routeMetadata.baseArea))
					return this.routeMetadata.baseArea
				return null
			},

			setModel(model, isUpdate)
			{
				if (isUpdate)
					_assignIn(this.model, model)
				else
					this.model = model

				if (_isEmpty(this.model.Query))
				{
					this.model.Query = {
						Orderings: [],
						Condition: [],
						Groups: [],
						DetailsGroup: {
							Fields: []
						},
						BaseTable: null,
						BaseTableDescription: null,
						Title: ''
					}
				}

				if (!_isEmpty(this.area) && !_isEmpty(this.model.Query.BaseTable) && this.area !== this.model.Query.BaseTable)
					this.area = this.model.Query.BaseTable

				this.model.ConditionsSelected = this.model.Query.Condition
				this.model.GroupingsSelected = this.model.Query.Groups
				this.model.OrderBySelected = this.model.Query.Orderings

				if (_isEmpty(this.model.ConditionsSelected))
				{
					this.model.ConditionsSelected = {
						Operation: 'AND',
						Operands: []
					}
				}

				this.toggleDetails = true
			},

			// Fetch the model from the server
			fetchModel()
			{
				this.cavDataOnLoadProc.add(fetchData('Cav', 'Index', { area: this.area }, this.setModel), true)
			},

			updateQuery(totals)
			{
				if (!this.cavDataOnLoadProc.loaded)
					return

				this.cavDataOnLoadProc.add(
					postData('Cav', 'UpdateQuery',
						{
							fields: this.model.FieldsSelectedList || null,
							orderby: this.model.OrderBySelected || null,
							conditions: this.model.ConditionsSelected || null,
							groupby: this.model.GroupingsSelected || null,
							area: this.area || null,
							totals: totals || null
						},
						(model) => this.setModel(model, true)
					), true)
			},

			/**
			 * Add field to the list of selected fields
			 */
			addFieldQuery(cavFieldData)
			{
				if (_isEmpty(cavFieldData))
					return

				const params = {
					tableId: cavFieldData.tableId,
					fieldId: cavFieldData.fieldId
				}

				postData('Cav', 'AddField', params, (data) => {
					if (data.Success)
						this.setModel(data.Model, true)
				})
			},

			/**
			 * Remove the field from selected fields list
			 */
			removeFieldQuery(fieldId)
			{
				const idxToRemove = _findIndex(this.model.FieldsSelectedList, (fld) => fld.FieldId === fieldId)
				if (idxToRemove !== -1)
					this.model.FieldsSelectedList.splice(idxToRemove, 1)

				this.updateQuery()
			},

			/**
			 * Update the condition with the new value
			 */
			updateCondition(conditions)
			{
				this.model.ConditionsSelected = conditions
				this.updateQuery()
			},

			/**
			 * Update the groupings with the new value
			 */
			updateGroupings(groupings)
			{
				this.model.GroupingsSelected = groupings
				this.updateQuery()
			},

			/**
			 * Update the orderings with the new value
			 */
			updateOrderings(orderings)
			{
				this.model.OrderBySelected = orderings
				this.updateQuery()
			},

			/**
			 * Update the totals with the new value
			 */
			updateTotals(totals)
			{
				this.updateQuery(totals || null)
			},

			/**
			 * Requests a new query to be used
			 */
			createNewQuery()
			{
				postData('Cav', 'NewQuery', { area: this.area || null }, (data) => {
					if (data.Success)
					{
						this.currentQueryId = 'new'
						this.setModel(data.Model, true)
						this.area = this.getCurrentInterfaceArea()
					}
					this.showTab('fields')
				})
			},

			/**
			 * Load saved query
			 */
			loadQuery(queryId)
			{
				this.cavDataOnLoadProc.add(
					fetchData(
						'Cav',
						'LoadQuery',
						{ queryid: queryId },
						(data) => {
							this.setModel(data)
							this.area = this.model.Query.BaseTable
							this.currentQueryId = queryId
							this.showTab('execute')
						}
					),
					true)
			},

			saveQuery(data)
			{
				postData(
					'Cav',
					'SaveQueryData',
					data,
					(message) => {
						displayMessage(message)
						this.currentQueryId = data.id
					})
			},

			onOpenSaveModal()
			{
				this.tabContentStayActive = true
				this.cavDataOnLoadProc.once(() => this.tabContentStayActive = false, this)
				this.updateQuery()
			},

			/**
			 * Get text from value or function property
			 * @param res {String or Function}
			 * @returns String or resource
			 */
			getCavText(res)
			{
				if (_isEmpty(res))
					return ''
				else if (typeof res === 'function')
					return res()
				return this.$tm(res)
			}
		}
	}
</script>
