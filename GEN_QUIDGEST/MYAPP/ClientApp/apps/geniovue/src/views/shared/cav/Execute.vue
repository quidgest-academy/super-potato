<template>
	<div v-if="!isEmpty(data.results)">
		<q-row-container is-large>
			<q-control-wrapper class="control-join-group mb-3">
				<!-- Excel button -->
				<q-button
					:label="texts.excelFile"
					:title="texts.excelFile"
					@click="getExcelFile">
					<q-icon icon="download" />
				</q-button>

				<!-- Show Query button -->
				<q-button
					:label="`${texts.query} (SQL)`"
					:title="`${texts.query} (SQL)`"
					@click="fnShowSQL">
					<q-icon icon="search" />
				</q-button>
			</q-control-wrapper>
		</q-row-container>

		<q-row-container is-large>
			<q-control-wrapper class="row-line-group">
				<!-- Table generated from executed query -->
				<div class="q-table-list">
					<div class="table-and-filters-wrapper">
						<div class="table-responsive-wrapper">
							<div class="table-responsive">
								<table class="c-table">
									<tbody class="c-table__body">
										<tr
											v-for="(row, rowIndex) in data.results"
											:key="`cav-result-row-${rowIndex}`"
											:class="{
												'group-header': row.type === lineType.GroupHeader || row.type === lineType.TotalHeader
											}">
											<template
												v-for="(cell, cellIndex) in row.items"
												:key="`cav-result-row-cell-${rowIndex}-${cellIndex}`">
												<th
													v-if="
														row.type === lineType.Header ||
															row.type === lineType.TotalHeader ||
															row.type === lineType.GroupHeader
													">
													<template v-if="!isEmpty(cell)">
														{{ $t(cell) }}
													</template>
												</th>
												<td v-else>{{ cell }}</td>
											</template>
										</tr>
									</tbody>
								</table>
								<div class="c-table__footer-out">
									<div
										class="float-left"
										style="display: inline-flex">
										<div class="e-counter">
											<q-icon
												icon="sort"
												class="e-counter__icon" />

											<span class="e-counter__text">
												{{ data.record_count }}
											</span>
										</div>

										<q-button-group>
											<q-button
												:title="texts.firstPage"
												label="<<"
												:disabled="data.current_page <= 2"
												@click="executeQuery(1)">
											</q-button>

											<q-button
												:title="texts.previousPage"
												label="<"
												:disabled="data.current_page <= 1"
												@click="executeQuery(data.current_page - 1)">
											</q-button>

											<span class="e-pagination__info">
												<span>
													{{ data.current_page }} /
													{{ data.total_pages }}
												</span>
											</span>

											<q-button
												:title="texts.nextPage"
												label=">"
												:disabled="data.current_page >= data.total_pages"
												@click="executeQuery(data.current_page + 1)">
											</q-button>

											<q-button
												:title="texts.lastPage"
												label=">>"
												:disabled="data.current_page >= data.total_pages - 1"
												@click="executeQuery(data.total_pages)">
											</q-button>
										</q-button-group>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</q-control-wrapper>
		</q-row-container>

		<teleport
			v-if="showSQL"
			:to="`#q-modal-${modalId}-body`">
			<div class="content">
				{{ data.querySQL }}
			</div>
		</teleport>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import { mapActions } from 'pinia'
	import _isEmpty from 'lodash-es/isEmpty'

	import { useGenericDataStore } from '@quidgest/clientapp/stores'
	import { removeModal } from '@/utils/layout'
	import netAPI from '@quidgest/clientapp/network'
	import hardcodedTexts from '@/hardcodedTexts.js'

	export default {
		name: 'QCavExecute',

		props: {
			/**
			 * The query object containing the necessary data to execute and render the CAV query results.
			 */
			query: {
				type: Object,
				required: true
			}
		},

		expose: [],

		data()
		{
			return {
				data: {
					results: [],
					record_count: 0,
					current_page: 1,
					total_pages: 1,
					querySQL: ''
				},

				errorMessage: '',

				lineType: {
					// <= change to StringEnumConverter in C#?
					YearSeparator: 0,
					Header: 1,
					DetailLine: 2,
					GroupHeader: 3,
					TotalHeader: 4,
					TotalLine: 5
				},

				showSQL: false,

				texts: {
					excelFile: computed(() => this.Resources[hardcodedTexts.excelFile]),
					query: computed(() => this.Resources[hardcodedTexts.query]),
					firstPage: computed(() => this.Resources[hardcodedTexts.firstPage]),
					lastPage: computed(() => this.Resources[hardcodedTexts.lastPage]),
					previousPage: computed(() => this.Resources[hardcodedTexts.previousPage]),
					nextPage: computed(() => this.Resources[hardcodedTexts.nextPage])
				},

				modalId: 'cav-sql-query'
			}
		},

		created()
		{
			this.executeQuery()
		},

		beforeMount()
		{
			this.$eventHub.emit('main-container-is-visible', false)
		},

		methods: {
			...mapActions(useGenericDataStore, ['setModal']),

			removeModal,

			isEmpty: _isEmpty,

			/**
			 * Executes the CAV query to fetch results for a specific page or defaults to the first page.
			 * Updates the component's data property with the result returned by the server.
			 * @param {Number} [page] - The page number to fetch results for.
			 */
			executeQuery(page)
			{
				netAPI.postData(
					'Cav',
					'ExecuteQuery2',
					{
						query: this.query,
						page: page || 1,
						queryid: null
					},
					(data) => {
						if (data?.Success === true)
							this.data = data
						else
						{
							this.data = {
								results: [],
								record_count: 0,
								current_page: 1,
								total_pages: 1,
								querySQL: ''
							}
							// TODO: handle error display
						}
					}
				)
			},

			/**
			 * Displays the SQL query string related to the data in a modal.
			 */
			fnShowSQL()
			{
				const props = {
					title: 'SQL',
					class: 'q-dialog-form',
					buttons: [
						{
							id: 'dialog-button-close',
							action: this.fnHideSQL,
							icon: { icon: 'close', type: 'svg' },
							props: {
								label: computed(() => this.Resources[hardcodedTexts.close]),
								variant: 'bold'
							}
						}
					]
				}
				const modalProps = {
					id: this.modalId,
					isActive: true,
					dismissAction: this.fnHideSQL
				}
				this.setModal(props, modalProps)

				this.$nextTick().then(() => (this.showSQL = true))
			},

			/**
			 * Hides the modal containing the SQL query string and cleans up.
			 */
			fnHideSQL()
			{
				this.removeModal(this.modalId)
				this.showSQL = false
			},

			/**
			 * Initiates the download process for the Excel file generated from the query results.
			 */
			getExcelFile()
			{
				const url = netAPI.apiActionURL('Cav', 'GenerateExcel')
				window.location.assign(url)
			}
		}
	}
</script>
