<template>
	<!-- TODO configurable header title position -->
	<div :class="{ card: card_mode }">
		<div
			v-if="card_mode"
			class="card-header">
			<slot name="card-header">
				<span>
					{{ card_title }}
				</span>
			</slot>
		</div>
		<div :class="{ 'card-body': card_mode }">
			<div
				v-if="showToolsRow"
				class="c-table__title-row">
				<q-label
					class="form-header"
					:label="getTableTitle()" />
				<div class="c-table__tools">
					<!-- global search text starts here -->
					<search
						:init-placeholder="global_search.placeholder"
						:init-classes="global_search.classes"
						:init-visibility="global_search.visibility"
						:init-case-sensitive="global_search.caseSensitive"
						:init-show-clear-button="global_search.showClearButton"
						:init-search-on-press-enter="global_search.searchOnPressEnter"
						:init-search-debounce-rate="global_search.searchDebounceRate"
						:init-show-refresh-button="global_search.showRefreshButton"
						:init-show-reset-button="global_search.showResetButton"
						:size="global_search.size"
						@clear-global-search="clearGlobalSearch"
						@update-global-search-handler="updateGlobalSearchHandler"
						@update-global-search="updateGlobalSearch"
						@emit-search="emitSearch"
						@reset-query="resetQuery"></search>
					<!-- global search text ends here -->

					<!-- export to excel button -->
					<q-button
						v-if="enableExport"
						:label="exportLabel"
						@click="exportExcel">
						<q-icon icon="file-export" />
					</q-button>
					<!--  export to excel button ends here -->

					<!-- action buttons starts here -->
					<div v-if="actions">
						<slot
							name="vbt-action-buttons"
							class="col-md-8">
							<div
								class="btn-group float-right"
								role="group"
								aria-label="Basic example">
								<button
									v-for="(action, ac_key, index) in actions"
									:key="index"
									type="button"
									class="btn"
									:class="getActionButtonClass(action)"
									@click="() => emitActionEvent(action)">
									<slot :name="action.btn_text_slot_name">
										<span v-html="action.btn_text"></span>
									</slot>
								</button>
							</div>
						</slot>
					</div>
					<!-- action buttons button ends here -->
				</div>
			</div>

			<div
				:class="tableWrapperClasses"
				class="vbt-table-wrapper">
				<table
					class="c-table"
					:class="tableClasses">
					<thead class="c-table__head">
						<tr>
							<th
								v-if="checkbox_rows"
								class="checkbox-column-th"></th>
							<slot
								name="columns"
								:columns="vbt_columns">
								<template
									v-for="(column, cl_key, index) in vbt_columns"
									:key="index">
									<th
										v-if="canShowColumn(column)"
										class="vbt-column-header"
										:class="columnClasses(column)"
										v-on="
											isSortableColumn(column)
												? { click: () => updateSortQuery(column) }
												: {}
										">
										<slot
											:name="'column_' + getCellSlotName(column)"
											:column="column">
											{{ getColumnLabel(column) }}
										</slot>
										<template v-if="isSortableColumn(column)">
											<SortIcon
												:sort="query.sort"
												:column="column">
												<template #vbt-sort-asc-icon>
													<slot name="sort-asc-icon"> &#x1F825; </slot>
												</template>
												<template #vbt-sort-desc-icon>
													<slot name="sort-desc-icon"> &#x1F827; </slot>
												</template>
												<template #vbt-no-sort-icon>
													<slot name="no-sort-icon">
														&#x1F825;&#x1F827;
													</slot>
												</template>
											</SortIcon>
										</template>
									</th>
								</template>
							</slot>
						</tr>
					</thead>
					<tbody class="c-table__body">
						<!-- filter row starts here -->
						<tr
							v-if="showFilterRow"
							class="filtersRow">
							<td v-show="checkbox_rows"></td>
							<template
								v-for="(column, cl_key, index) in vbt_columns"
								:key="index">
								<td
									v-if="canShowColumn(column)"
									align="center">
									<template v-if="hasFilter(column)">
										<Simple
											v-if="column.filter.type === 'simple'"
											:column="column"
											@update-filter="updateFilter"
											@clear-filter="clearFilter">
											<template #vbt-simple-filter-clear-icon>
												<slot name="simple-filter-clear-icon">
													&#x24E7;
												</slot>
											</template>
										</Simple>
										<MultiSelect
											v-if="column.filter.type === 'select'"
											:options="column.filter.options"
											:column="column"
											@update-multi-select-filter="updateMultiSelectFilter"
											@clear-filter="clearFilter"></MultiSelect>
										<template v-if="column.filter.type === 'custom'">
											<slot
												:name="column.filter.slot_name"
												:column="column">
											</slot>
										</template>
									</template>
								</td>
							</template>
						</tr>
						<!-- filter row ends here -->
						<!-- data rows stars here -->
						<row
							v-for="(row, index) in vbt_rows"
							:key="index"
							:row="row"
							:columns="vbt_columns"
							:row-index="index"
							:checkbox-rows="checkbox_rows"
							:single-row-selectable="single_row_selectable"
							:selected-items="selected_items"
							:highlight-row-hover="highlight_row_hover"
							:highlight-row-hover-color="rowHighlightColor"
							:prop-row-classes="classes?.row"
							:prop-cell-classes="classes?.cell"
							:unique-id="uniqueId"
							@add-row="handleAddRow"
							@remove-row="handleRemoveRow"
							@single-row-select="handleSingleRowSelect">
							<template
								v-for="column in columns"
								#[`vbt-${getCellSlotName(column)}`]>
								<slot
									:name="getCellSlotName(column)"
									:row="row"
									:column="column"
									:cell_value="getValueFromRow(row, column.name)">
									{{ getValueFromRow(row, column.name) }}
								</slot>
							</template>
						</row>
						<!-- empty row starts here -->
						<tr v-show="vbt_rows.length === 0">
							<td :colspan="headerColSpan">
								<slot name="empty-results"> No results found </slot>
							</td>
						</tr>
						<!-- empty row ends here -->
						<!-- data rows ends here -->
						<!-- Pagination row starts here -->
						<tr
							v-if="showPaginationRow"
							class="footer-pagination-row">
							<td :colspan="headerColSpan">
								<div class="row vbt-pagination-row no-gutters">
									<!-- pagination starts here -->
									<div class="col-md-8">
										<div v-if="pagination">
											<Pagination
												v-model:page="page"
												v-model:per_page="per_page"
												:per_page_options="per_page_options"
												:total="rowCount"
												:num-of-visible-pagination-buttons="
													numOfVisiblePaginationButtons
												">
												<template #vbt-paginataion-previous-button>
													<slot name="paginataion-previous-button">
														&laquo;
													</slot>
												</template>
												<template #vbt-paginataion-next-button>
													<slot name="paginataion-next-button">
														&raquo;
													</slot>
												</template>
											</Pagination>
										</div>
									</div>
									<!-- pagination ends here -->
									<!-- pagination info start here -->
									<div class="col-md-4">
										<div class="text-right justify-content-center">
											<template v-if="pagination_info">
												<slot
													name="pagination-info"
													:current-page-rows-length="
														currentPageRowsLength
													"
													:filtered-rows-length="filteredRowsLength"
													:original-rows-length="originalRowsLength">
													<span v-if="currentPageRowsLength !== 0">
														From {{ currentPageFirstRow }} to
														{{ currentPageRowsLength }} of
														{{ filteredRowsLength }} entries
													</span>
													<span v-else> No results found </span>
													<span>
														({{ originalRowsLength }} total records)
													</span>
												</slot>
											</template>
											<template
												v-if="
													selected_rows_info &&
													pagination_info &&
													isSelectable
												">
												<slot name="pagination-selected-rows-separator">
													|
												</slot>
											</template>
											<template v-if="selected_rows_info && isSelectable">
												<slot
													name="selected-rows-info"
													:selected-items-count="selectedItemsCount">
													{{ selectedItemsCount }} rows selected
												</slot>
											</template>
										</div>
									</div>
									<!-- pagination info ends here -->
								</div>
							</td>
						</tr>
						<!-- Pagination ends starts here -->
					</tbody>
					<tfoot class="c-table__footer">
						<slot name="table-footer"></slot>
					</tfoot>
				</table>
			</div>
		</div>
		<div
			v-if="card_mode"
			class="card-footer">
			<slot name="card-footer">
				<div class="row">
					<!-- pagination starts here -->
					<div class="col-md-6">
						<div v-if="pagination">
							<Pagination
								v-model:page="page"
								v-model:per_page="per_page"
								:per_page_options="per_page_options"
								:total="rowCount"
								:num-of-visible-pagination-buttons="numOfVisiblePaginationButtons">
								<template #vbt-paginataion-previous-button>
									<slot name="paginataion-previous-button"> &laquo; </slot>
								</template>
								<template #vbt-paginataion-next-button>
									<slot name="paginataion-next-button"> &raquo; </slot>
								</template>
							</Pagination>
						</div>
					</div>
					<!-- pagination ends here -->

					<!-- pagination info start here -->
					<div class="col-md-6">
						<div class="text-right justify-content-center">
							<template v-if="pagination_info">
								<slot
									name="pagination-info"
									:current-page-rows-length="currentPageRowsLength"
									:filtered-rows-length="filteredRowsLength"
									:original-rows-length="originalRowsLength">
									<span v-if="currentPageRowsLength !== 0">
										From 1 to {{ currentPageRowsLength }} of
										{{ filteredRowsLength }} entries
									</span>
									<span v-else> No results found </span>
									<span> ({{ originalRowsLength }} total records) </span>
								</slot>
							</template>
							<template v-if="pagination_info && selected_rows_info">
								<slot name="pagination-selected-rows-separator"> | </slot>
							</template>
							<template v-if="selected_rows_info">
								<slot
									name="selected-rows-info"
									:selected-items-count="selectedItemsCount">
									{{ selectedItemsCount }} rows selected
								</slot>
							</template>
						</div>
					</div>
					<!-- pagination info ends here -->
				</div>
			</slot>
		</div>
	</div>
</template>

<script>
	import {
		findIndex,
		filter,
		includes,
		map,
		has,
		extend,
		isEmpty,
		isEqual,
		debounce,
		cloneDeep,
		intersectionWith,
		intersectionBy,
		orderBy,
		get,
		omit,
		clone
	} from 'lodash-es'

	import Search from './Search.vue'
	import Row from './Row.vue'
	import SortIcon from './SortIcon.vue'
	import Pagination from './Pagination.vue'
	import Simple from './Filters/Simple.vue'
	import MultiSelect from './Filters/MultiSelect.vue'

	import { reusableMixin } from '@/mixins/mainMixin.js'

	// SOURCE: https://github.com/rubanraj54/vue-bootstrap4-table

	import { QUtils } from '@/utils/mainUtils'
	import { mapState } from 'vuex'

	export default {
		name: 'QTable',

		components: {
			Search,
			Row,
			Simple,
			MultiSelect,
			SortIcon,
			Pagination
		},

		mixins: [reusableMixin],

		props: {
			/**
			 * The records to show in the table.
			 */
			rows: {
				type: Array,
				default: () => []
			},

			/**
			 * The table columns.
			 */
			columns: {
				type: Array,
				required: true
			},

			/**
			 * Total number of table records.
			 */
			totalRows: {
				type: Number,
				default: 0
			},

			/**
			 * The table configuration object.
			 */
			config: {
				type: Object,
				default: () => {}
			},

			/**
			 * Custom table classes.
			 */
			classes: {
				type: Object,
				default: () => {}
			},

			/**
			 * Table row actions.
			 */
			actions: {
				type: Array,
				default: () => []
			},

			/**
			 * Table filters.
			 */
			customFilters: {
				type: Array,
				default: () => []
			},

			/**
			 * True if the table should be exportable, false otherwise.
			 */
			enableExport: {
				type: Boolean,
				default: false
			},

			/**
			 * Export button label.
			 */
			exportLabel: {
				type: String,
				default: ''
			}
		},

		emits: [
			'on-select-row',
			'on-unselect-row',
			'on-all-select-rows',
			'on-all-unselect-rows',
			'on-single-select-row',
			'on-change-query'
		],

		expose: [],

		data() {
			return {
				vbt_rows: [],
				vbt_columns: [],
				query: {
					sort: [],
					filters: [],
					global_search: ''
				},
				page: 1,
				per_page: 10,
				original_rows: [],
				numOfVisiblePaginationButtons: 5,
				temp_filtered_results: [],
				pagination: true,
				pagination_info: true,
				checkbox_rows: false,
				selected_items: [],
				highlight_row_hover: true,
				highlight_row_hover_color: '#d6d6d6',
				single_row_selectable: false,
				allRowsSelected: false,
				multi_column_sort: false,
				card_title: '',
				table_title: '',
				global_search: {
					placeholder: 'Enter search text',
					classes: '',
					size: 'xlarge',
					visibility: true,
					caseSensitive: false,
					showRefreshButton: true,
					showResetButton: true,
					showClearButton: false,
					searchOnPressEnter: true,
					searchDebounceRate: 60,
					init: {
						value: ''
					}
				},
				per_page_options: [5, 10, 15],
				server_mode: false,
				total_rows: 0,
				card_mode: false,
				selected_rows_info: false,
				lastSelectedItemIndex: null,
				isFirstTime: true,
				isResponsive: false,
				preservePageOnDataChange: false,
				canEmitQueries: false
			}
		},

		computed: {
			...mapState(['currentLanguage']),

			currentLanguage() {
				return this.$store.state.currentLanguage
			},

			rowCount() {
				if (!this.server_mode) {
					return this.temp_filtered_results.length
				} else {
					return this.totalRows
				}
			},

			selectedItemsCount() {
				return this.selected_items.length
			},

			filteredResultsCount() {
				return this.temp_filtered_results.length
			},

			uniqueId() {
				if (!this.hasUniqueId) return undefined

				return this.vbt_columns.find((column) => column.uniqueId).name
			},

			hasUniqueId() {
				return this.vbt_columns.some((column) => column.uniqueId)
			},

			// pagination info computed properties - start
			currentPageFirstRow() {
				return (this.page - 1) * this.per_page + 1
			},

			currentPageRowsLength() {
				return this.currentPageFirstRow - 1 + this.vbt_rows.length
			},

			filteredRowsLength() {
				return this.rowCount
			},

			originalRowsLength() {
				return this.server_mode ? this.rowCount : this.rows.length
			},

			// pagination info computed properties - end
			rowHighlightColor() {
				return this.highlight_row_hover ? this.highlight_row_hover_color : ''
			},

			headerColSpan() {
				let count = this.checkbox_rows ? 1 : 0

				count += this.vbt_columns.filter((column) => this.canShowColumn(column)).length

				return count
			},

			showToolsRow() {
				return (
					this.global_search.visibility === true ||
					this.global_search.showRefreshButton === true ||
					this.global_search.showResetButton === true ||
					this.actions.length > 0 ||
					this.getTableTitle()
				)
			},

			showFilterRow() {
				let show_row = false

				this.columns.some((column) => {
					if (has(column, 'filter')) {
						show_row = true
						return true
					}
				})
				return show_row
			},

			showPaginationRow() {
				let show_pagination_row = false

				if (
					(this.card_mode === false || this.card_mode === undefined) &&
					(this.pagination === true ||
						this.pagination_info === true ||
						this.selected_rows_info === true)
				) {
					show_pagination_row = true
				}

				return show_pagination_row
			},

			currentPageSelectionCount() {
				let result = []
				if (!this.hasUniqueId) {
					result = intersectionWith(this.vbt_rows, this.selected_items, isEqual)
				} else {
					result = intersectionBy(this.vbt_rows, this.selected_items, this.uniqueId)
				}
				return result.length
			},

			tableClasses() {
				let classes = ''

				if (!this.classes) return classes

				if (typeof this.classes.table === 'string') {
					return this.classes.table
				} else if (typeof this.classes.table === 'object') {
					Object.entries(this.classes.table).forEach(([key, value]) => {
						if (typeof value === 'boolean' && value) {
							classes += key
						} else if (typeof value === 'function') {
							const truth = value(this.rows)
							if (typeof truth === 'boolean' && truth) {
								classes += ' '
								classes += key
							}
						}
					})
				}
				return classes
			},

			tableWrapperClasses() {
				const defaultClasses = this.isResponsive ? 'table-responsive' : ''

				if (
					!this.classes ||
					!this.classes.tableWrapper ||
					this.classes.tableWrapper !== ''
				) {
					return defaultClasses
				}

				return typeof this.classes.tableWrapper === 'string'
					? this.classes.tableWrapper
					: defaultClasses
			},

			isSelectable() {
				return this.checkbox_rows
			},

			updateGlobalSearch() {
				return debounce(
					this.updateGlobalSearchHandler,
					this.global_search.searchDebounceRate
				)
			}
		},

		watch: {
			'query.filters': {
				handler() {
					if (!this.server_mode) {
						this.filter(!this.preservePageOnDataChange)
					}
				},
				deep: true
			},

			'query.sort': {
				handler() {
					if (!this.server_mode) {
						this.sort()
					}
				},
				deep: true
			},

			'query.global_search': {
				handler() {
					if (!this.server_mode) {
						this.filter(!this.preservePageOnDataChange)
					}
				}
			},

			query: {
				handler() {
					if (this.server_mode) {
						this.emitQueryParams()
					}
				},
				deep: true
			},

			per_page: {
				handler() {
					if (!this.server_mode) {
						const doPaginateFilter = this.page === 1
						if (!this.preservePageOnDataChange) this.page = 1
						if (doPaginateFilter) {
							this.paginateFilter()
						}
					} else {
						this.emitQueryParams()
					}
				}
			},

			pagination: {
				handler: function () {
					if (!this.server_mode) {
						this.paginateFilter()
					} else {
						this.emitQueryParams()
					}
				}
			},

			rows: {
				handler() {
					this.vbt_rows = cloneDeep(this.rows)

					if (!this.server_mode) {
						// check if user mentioned unique id for a column, if not set unique id for all items
						this.original_rows = this.vbt_rows.map((element, index) => {
							const extra = {}
							if (!this.hasUniqueId) {
								extra.vbt_id = index + 1
							}
							return extend({}, element, extra)
						})
						this.filter(!this.preservePageOnDataChange, !this.isFirstTime)
					} else {
						if (this.preservePageOnDataChange) {
							const predictedTotalPage = Math.ceil(this.rowCount / this.per_page)
							if (predictedTotalPage !== 0) {
								this.page =
									this.page <= predictedTotalPage ? this.page : predictedTotalPage
							} else {
								this.page = 1
							}
						}
					}

					this.isFirstTime = false
				},
				deep: true
			},

			customFilters: {
				handler(newVal) {
					if (!this.server_mode) {
						newVal.forEach((customFilter) => {
							if (customFilter.name) {
								const index = this.query.filters.findIndex(
									(filter) => filter.name === customFilter.name
								)
								if (index === -1) {
									this.query.filters.push(customFilter)
								} else {
									this.query.filters[index].text = customFilter.text
								}
							}
						})
					}
				},
				deep: true
			},

			columns: {
				handler() {
					this.vbt_columns = cloneDeep(this.columns)

					this.vbt_columns = map(this.vbt_columns, function (element, index) {
						const extra = {}
						extra.vbt_col_id = index + 1
						return extend({}, element, extra)
					})

					this.initFilterQueries()
				},
				deep: true
			},

			config: {
				handler() {
					this.initConfig()
				},
				deep: true
			},

			page(newVal) {
				if (!this.server_mode) {
					this.paginateFilter()
				} else {
					this.emitQueryParams(newVal)
				}
			},

			'config.multi_column_sort': {
				handler() {
					this.resetSort()
				}
			},

			currentLanguage() {
				this.$forceUpdate()
			}
		},

		created() {
			this.initConfig()
		},

		mounted() {
			this.vbt_rows = cloneDeep(this.rows)
			this.vbt_columns = cloneDeep(this.columns)

			// check if user mentioned unique id for a column, if not set unique id for all items
			this.original_rows = this.vbt_rows.map((element, index) => {
				const extra = {}
				if (!this.hasUniqueId) {
					extra.vbt_id = index + 1
				}
				return extend({}, element, extra)
			})

			this.vbt_columns = map(this.vbt_columns, function (element, index) {
				const extra = {}
				extra.vbt_col_id = index + 1
				return extend({}, element, extra)
			})

			//this.initConfig();
			this.initialSort()
			this.initFilterQueries()

			if (this.global_search.visibility) {
				this.$nextTick().then(() => {
					this.initGlobalSearch()
				})
			}

			this.$nextTick().then(() => {
				if (!this.server_mode) {
					this.filter(false, true)
				} else {
					this.canEmitQueries = true
					this.emitQueryParams()
				}
			})

			this.handleShiftKey()
		},

		methods: {
			initConfig() {
				if (isEmpty(this.config)) {
					return
				}

				this.pagination = has(this.config, 'pagination') ? this.config.pagination : true

				this.numOfVisiblePaginationButtons = has(
					this.config,
					'numOfVisiblePaginationButtons'
				)
					? this.config.numOfVisiblePaginationButtons
					: 7

				this.per_page_options = has(this.config, 'per_page_options')
					? this.config.per_page_options
					: [5, 10, 15]

				this.per_page = has(this.config, 'per_page') ? this.config.per_page : 10

				this.page = has(this.config, 'page') ? this.config.page : 1

				this.checkbox_rows = has(this.config, 'checkbox_rows')
					? this.config.checkbox_rows
					: false

				this.highlight_row_hover = has(this.config, 'highlight_row_hover')
					? this.config.highlight_row_hover
					: true

				this.highlight_row_hover_color = has(this.config, 'highlight_row_hover_color')
					? this.config.highlight_row_hover_color
					: '#d6d6d6'

				this.single_row_selectable = has(this.config, 'single_row_selectable')
					? this.config.single_row_selectable
					: false

				this.multi_column_sort = has(this.config, 'multi_column_sort')
					? this.config.multi_column_sort
					: false

				this.pagination_info = has(this.config, 'pagination_info')
					? this.config.pagination_info
					: true

				this.card_title = has(this.config, 'card_title') ? this.config.card_title : ''

				this.table_title = has(this.config, 'table_title') ? this.config.table_title : ''

				if (has(this.config, 'global_search')) {
					this.global_search.placeholder = has(this.config.global_search, 'placeholder')
						? this.config.global_search.placeholder
						: 'Enter search text'
					this.global_search.visibility = has(this.config.global_search, 'visibility')
						? this.config.global_search.visibility
						: true
					this.global_search.caseSensitive = has(
						this.config.global_search,
						'caseSensitive'
					)
						? this.config.global_search.caseSensitive
						: false
					this.global_search.showRefreshButton = has(
						this.config.global_search,
						'showRefreshButton'
					)
						? this.config.global_search.showRefreshButton
						: true
					this.global_search.showResetButton = has(
						this.config.global_search,
						'showResetButton'
					)
						? this.config.global_search.showResetButton
						: true
					this.global_search.showClearButton = has(
						this.config.global_search,
						'showClearButton'
					)
						? this.config.global_search.showClearButton
						: false
					this.global_search.searchOnPressEnter = has(
						this.config.global_search,
						'searchOnPressEnter'
					)
						? this.config.global_search.searchOnPressEnter
						: true
					this.global_search.searchDebounceRate = has(
						this.config.global_search,
						'searchDebounceRate'
					)
						? this.config.global_search.searchDebounceRate
						: 60
					this.global_search.classes = has(this.config.global_search, 'classes')
						? this.config.global_search.classes
						: ''
					this.global_search.size = has(this.config.global_search, 'size')
						? this.config.global_search.size
						: 'xlarge'
					this.global_search.init.value = has(this.config.global_search, 'init.value')
						? this.config.global_search.init.value
						: ''
				}

				this.server_mode = has(this.config, 'server_mode') ? this.config.server_mode : false

				this.card_mode = has(this.config, 'card_mode') ? this.config.card_mode : false

				this.selected_rows_info = has(this.config, 'card_mode')
					? this.config.selected_rows_info
					: false

				this.preservePageOnDataChange = has(this.config, 'preservePageOnDataChange')
					? this.config.preservePageOnDataChange
					: false
			},

			initialSort() {
				// TODO optimze this with removing this filter
				const initial_sort_columns = filter(
					this.vbt_columns,
					(column) => has(column, 'initial_sort') && column.initial_sort === true
				)

				initial_sort_columns.some((initial_sort_column) => {
					const result = findIndex(this.query.sort, {
						vbt_col_id: initial_sort_column.vbt_col_id
					})

					if (result === -1) {
						// initial sort order validation starts here
						let initial_sort_order = 'asc'
						if (has(initial_sort_column, 'initial_sort_order')) {
							if (includes(['asc', 'desc'], initial_sort_column.initial_sort_order)) {
								initial_sort_order = initial_sort_column.initial_sort_order
							} else {
								console.log('invalid initial_sort_order, so setting it to default')
							}
						}
						// initial sort order validation ends here
						this.query.sort.push({
							vbt_col_id: initial_sort_column.vbt_col_id,
							name: initial_sort_column.name,
							order: initial_sort_order,
							caseSensitive: this.isSortCaseSensitive(initial_sort_column)
						})
					}
					// else {
					//     this.query.sort[result].order = initial_sort_column.initial_sort_order;
					// }

					// if multicolum sort sort is false, then consider only first initial sort column
					if (!this.multi_column_sort) {
						return true
					}
				})
			},

			initGlobalSearch() {
				//this.$refs.global_search.value = this.global_search.init.value;
				this.query.global_search = this.global_search.init.value
			},

			hasFilter(column) {
				return has(column, 'filter.type')
			},

			clearFilter(column) {
				const filter_index = this.getFilterIndex(column)
				if (filter_index !== -1) {
					this.query.filters.splice(filter_index, 1)
				}
			},

			getFilterIndex(column) {
				return findIndex(this.query.filters, {
					name: column.name
				})
			},

			initFilterQueries() {
				this.vbt_columns.forEach((vbt_column) => {
					if (!has(vbt_column, 'filter')) return

					if (vbt_column.filter.type === 'simple') {
						if (!has(vbt_column, 'filter.init.value')) return

						this.updateFilter({
							value: vbt_column.filter.init.value,
							column: vbt_column
						})
					} else if (vbt_column.filter.type === 'select') {
						if (!has(vbt_column, 'filter.init.value')) return

						let initialValues = []
						if (vbt_column.filter.mode === 'multi') {
							if (Array.isArray(vbt_column.filter.init.value)) {
								initialValues = vbt_column.filter.init.value
							} else {
								console.log("Initial value for 'multi' mode should be an array")
							}
						} else if (vbt_column.filter.mode === 'single') {
							if (
								Number.isInteger(vbt_column.filter.init.value) &&
								vbt_column.filter.init.value > -1
							) {
								initialValues = [vbt_column.filter.init.value]
							} else {
								console.log(
									"Initial value for 'single' mode should be a single number and greater than -1"
								)
							}
						}

						const selected_options = vbt_column.filter.options
							.filter((_, index) => includes(initialValues, index))
							.map((filtered_option) => filtered_option.value)

						this.updateMultiSelectFilter({
							selected_options: selected_options,
							column: vbt_column
						})
					}
				})
			},

			isSortCaseSensitive(column) {
				return column.sortCaseSensitive !== undefined ? column.sortCaseSensitive : true
			},

			updateSortQuery(column) {
				const result = findIndex(this.query.sort, {
					vbt_col_id: column.vbt_col_id
				})

				if (result === -1) {
					if (!this.multi_column_sort) {
						this.query.sort = []
					}
					this.query.sort.push({
						vbt_col_id: column.vbt_col_id,
						name: column.name,
						order: 'asc',
						caseSensitive: this.isSortCaseSensitive(column)
					})
				} else {
					this.query.sort[result].order =
						this.query.sort[result].order === 'asc' ? 'desc' : 'asc'
				}
			},

			isShiftSelection(shiftKey, rowIndex) {
				return (
					shiftKey === true &&
					this.lastSelectedItemIndex !== null &&
					this.lastSelectedItemIndex !== rowIndex
				)
			},

			handleAddRow(payload) {
				const row = this.vbt_rows[payload.rowIndex]
				if (this.isShiftSelection(payload.shiftKey, payload.rowIndex)) {
					const rows = this.getShiftSelectionRows(payload.rowIndex)
					rows.forEach((_row) => {
						this.addSelectedItem(_row)
					})
				} else {
					this.addSelectedItem(row)
				}

				this.$emit('on-select-row', {
					selected_items: cloneDeep(this.selected_items),
					selected_item: row
				})

				this.lastSelectedItemIndex = payload.rowIndex
			},

			getActionButtonClass(action) {
				return has(action, 'class') ? action.class : ' btn-secondary'
			},

			handleRemoveRow(payload) {
				const row = this.vbt_rows[payload.rowIndex]
				if (this.isShiftSelection(payload.shiftKey, payload.rowIndex)) {
					const rows = this.getShiftSelectionRows(payload.rowIndex)
					rows.forEach((_row) => {
						this.removeSelectedItem(_row)
					})
				} else {
					this.removeSelectedItem(row)
				}
				this.$emit('on-unselect-row', {
					selected_items: cloneDeep(this.selected_items),
					unselected_item: row
				})
				this.lastSelectedItemIndex = payload.rowIndex
			},

			addSelectedItem(item) {
				let index = -1
				if (!this.hasUniqueId) {
					index = findIndex(this.selected_items, (selected_item) => {
						return isEqual(selected_item, item)
					})
				} else {
					index = findIndex(this.selected_items, (selected_item) => {
						return selected_item[this.uniqueId] === item[this.uniqueId]
					})
				}

				if (index === -1) {
					this.selected_items.push(item)
				}
			},

			removeSelectedItem(item) {
				if (!this.hasUniqueId) {
					this.selected_items = this.selected_items.filter(
						(selected_item) => !isEqual(selected_item, item)
					)
				} else {
					this.selected_items = this.selected_items.filter(
						(selected_item) => selected_item[this.uniqueId] !== item[this.uniqueId]
					)
				}
			},

			getShiftSelectionRows(rowIndex) {
				let start = 0
				let end = 0
				if (this.lastSelectedItemIndex < rowIndex) {
					start = this.lastSelectedItemIndex
					end = rowIndex + 1
				} else if (this.lastSelectedItemIndex > rowIndex) {
					start = rowIndex
					end = this.lastSelectedItemIndex + 1
				}
				return this.vbt_rows.slice(start, end)
			},

			handleSingleRowSelect(payload) {
				const row = this.vbt_rows[payload.rowIndex]
				this.$emit('on-single-select-row', { row, rowIndex: payload.rowIndex })
			},

			updateFilter(payload) {
				const value =
					typeof payload.value === 'number' ? payload.value.toString() : payload.value
				const column = payload.column
				const filter_index = findIndex(this.query.filters, {
					name: column.name
				})
				if (filter_index === -1) {
					if (value !== '') {
						this.query.filters.push({
							type: column.filter.type,
							name: column.name,
							text: value.trim(),
							config: column.filter
						})
					}
				} else {
					if (value === '') {
						this.query.filters.splice(filter_index, 1)
					} else {
						this.query.filters[filter_index].text = value.trim()
					}
				}
			},

			updateMultiSelectFilter(payload) {
				const selected_options = payload.selected_options
				const column = payload.column

				const filter_index = findIndex(this.query.filters, {
					name: column.name
				})

				if (filter_index === -1) {
					if (selected_options.length === 0) {
						return
					}
					this.query.filters.push({
						type: column.filter.type,
						mode: column.filter.mode,
						name: column.name,
						selected_options: selected_options,
						config: column.filter
					})
				} else {
					if (selected_options.length === 0) {
						this.query.filters.splice(filter_index, 1)
					} else {
						this.query.filters[filter_index].selected_options = selected_options
					}
				}
			},

			sort() {
				if (this.query.sort.length !== 0) {
					const orders = this.query.sort.map((sortConfig) => sortConfig.order)

					this.temp_filtered_results = orderBy(
						this.temp_filtered_results,
						this.query.sort.map((sortConfig) => {
							return (row) => {
								const value = get(row, sortConfig.name)
								if (sortConfig.caseSensitive) return value !== null ? value : ''
								return value !== null ? value.toString().toLowerCase() : ''
							}
						}),
						orders
					)
				}

				this.paginateFilter()
			},

			filter(resetPage = true, isInit = false) {
				const res = filter(this.original_rows, (row) => {
					let flag = true
					this.query.filters.some((filter) => {
						if (filter.type === 'simple') {
							if (
								this.simpleFilter(get(row, filter.name), filter.text, filter.config)
							) {
								// continue to next filter
								flag = true
							} else {
								// stop here and break loop since one filter has failed
								flag = false
								return true
							}
						} else if (filter.type === 'select') {
							if (
								this.multiSelectFilter(
									get(row, filter.name),
									filter.selected_options,
									filter.config
								)
							) {
								flag = true
							} else {
								flag = false
								return true
							}
						} else if (filter.type === 'custom') {
							const index = findIndex(this.vbt_columns, { name: filter.name })
							if (index > -1) {
								const column = this.vbt_columns[index]
								if (column.filter.validator) {
									const result = column.filter.validator(
										get(row, filter.name),
										filter.text
									)
									if (result === true || result === undefined) {
										flag = true
									} else {
										flag = false
										return true
									}
								} else {
									flag = true
								}
							} else {
								flag = true
							}
						}
					})
					return flag
				})

				this.temp_filtered_results = res

				// Do global search only if global search text is not empty and
				// filtered results is also not empty
				if (this.query.global_search !== '' && this.rowCount !== 0) {
					this.temp_filtered_results = this.globalSearch(this.temp_filtered_results)
				}

				this.sort()
				if (resetPage || this.rowCount === 0) {
					this.page = 1
				} else if (!isInit) {
					const newTotalPage = Math.ceil(this.rowCount / this.per_page)
					this.page = this.page <= newTotalPage ? this.page : newTotalPage
				}
			},

			globalSearch(temp_filtered_results) {
				const global_search_results = filter(temp_filtered_results, (row) => {
					let flag = false

					this.vbt_columns.some((vbt_column) => {
						let value = get(row, vbt_column.name)
						let global_search_text = this.query.global_search

						if (value === null || typeof value === 'undefined') {
							value = ''
						}

						if (typeof value !== 'string') {
							value = value.toString()
						}

						if (typeof global_search_text !== 'string') {
							global_search_text = global_search_text.toString()
						}

						if (!this.global_search.caseSensitive) {
							value = value.toLowerCase()
							global_search_text = global_search_text.toLowerCase()
						}

						if (value.indexOf(global_search_text) > -1) {
							flag = true
							return
						}
					})

					return flag
				})

				return global_search_results
			},

			simpleFilter(value, filter_text, config) {
				if (value === null || typeof value === 'undefined') {
					value = ''
				}

				if (typeof value !== 'string') {
					value = value.toString()
				}

				if (typeof filter_text !== 'string') {
					value = filter_text.toString()
				}

				const is_case_sensitive = has(config, 'case_sensitive')
					? config.case_sensitive
					: false

				if (!is_case_sensitive) {
					value = value.toLowerCase()
					filter_text = filter_text.toLowerCase()
				}

				return value.indexOf(filter_text) > -1
			},

			multiSelectFilter(value, selected_options) {
				if (typeof value !== 'string') {
					value = value.toString().toLowerCase()
				} else {
					value = value.toLowerCase()
				}

				selected_options = map(selected_options, (option) => {
					return typeof option !== 'string'
						? option.toString().toLowerCase()
						: option.toLowerCase()
				})
				return includes(selected_options, value)
			},

			paginateFilter() {
				if (this.pagination) {
					const start = (this.page - 1) * this.per_page
					const end = start + this.per_page
					this.vbt_rows = this.temp_filtered_results.slice(start, end)
				} else {
					this.vbt_rows = cloneDeep(this.temp_filtered_results)
				}
			},

			isSortableColumn(column) {
				if (!has(column, 'sort')) {
					return false
				} else {
					return column.sort
				}
			},

			// row method starts here
			getValueFromRow(row, name) {
				return get(row, name)
			},

			getCellSlotName(column) {
				if (has(column, 'slot_name')) {
					return column.slot_name
				}
				return column.name.replace(/\./g, '_')
			},

			// row method ends here
			resetSort() {
				this.query.sort = []
				this.filter(!this.preservePageOnDataChange)
			},

			updateGlobalSearchHandler: function (value) {
				this.query.global_search = value
			},

			clearGlobalSearch() {
				this.query.global_search = ''
				//this.$refs.global_search.value = "";
			},

			resetQuery() {
				this.query = {
					sort: [],
					filters: [],
					global_search: ''
				}

				//this.$refs.global_search.value = "";
				this.$eventHub.emit('reset-query')
			},

			emitSearch(search_value) {
				this.query.global_search = search_value
			},

			emitQueryParams(page = null) {
				if (this.server_mode && this.canEmitQueries) {
					const queryParams = cloneDeep(this.query)
					const sort = map(queryParams.sort, (o) => omit(o, 'vbt_col_id'))
					const filters = map(queryParams.filters, (o) => omit(o, 'config'))
					const global_search = queryParams.global_search
					const per_page = clone(this.per_page)

					if (page === null) {
						if (this.preservePageOnDataChange) {
							page = this.page
						} else {
							this.page = 1
							page = 1
						}
					}

					const payload = {
						sort: sort,
						filters: filters,
						global_search: global_search,
						per_page: per_page,
						page: page
					}

					this.$emit('on-change-query', payload)
				}
			},

			columnClasses(column) {
				let classes = ''

				const default_text_alignment = 'text-left'

				//decide text alignment class - starts here
				const alignments = ['text-justify', 'text-right', 'text-left', 'text-center']
				if (
					has(column, 'column_text_alignment') &&
					includes(alignments, column.column_text_alignment)
				) {
					classes = classes + ' ' + column.column_text_alignment
				} else {
					classes = classes + ' ' + default_text_alignment
				}
				//decide text alignment class - ends here

				// adding user defined classes to rows - starts here
				if (has(column, 'column_classes')) {
					classes = classes + ' ' + column.column_classes
				}
				// adding user defined classes to rows - ends here

				// adding classes for sortable column - starts here
				if (this.isSortableColumn(column)) {
					classes = classes + ' vbt-sort-cursor'
				}
				// adding classes for sortable column - ends here

				return classes
			},

			getColumnLabel(column) {
				if (typeof column.label === 'function') {
					return column.label()
				} else {
					return column.label
				}
			},

			getTableTitle() {
				if (typeof this.table_title === 'function') {
					return this.table_title()
				} else {
					return this.table_title
				}
			},

			handleShiftKey() {
				;['keyup', 'keydown'].forEach((event) => {
					window.addEventListener(event, (e) => {
						document.onselectstart = function () {
							return !(e.key === 'Shift' && e.shiftKey === true)
						}
					})
				})
			},

			emitActionEvent(action) {
				const payload = {
					event_payload: cloneDeep(action.event_payload)
				}

				if (this.isSelectable) {
					payload.selectedItems = cloneDeep(this.selected_items)
				}

				this.$emit(action.event_name, payload)
			},

			canShowColumn(column) {
				return column.visibility === undefined || column.visibility ? true : false
			},

			exportExcel() {
				fetch(QUtils.apiActionURL('Users', 'ExportToExcel'), {
					method: 'GET'
				})
					.then((response) => {
						if (response.ok) {
							return response.blob()
						} else {
							const messageError = `Resources.${Genio.GetSymbolFromString('Erro ao exportar o ficheiro')}`
							throw new Error(messageError)
						}
					})
					.then((blob) => {
						const url = window.URL.createObjectURL(blob)
						const a = document.createElement('a')
						a.href = url
						a.download = 'user-list.xlsx'
						a.click()
						window.URL.revokeObjectURL(url)
					})
					.catch((error) => {
						console.error('Error:', error)
					})
			}
		}
	}
</script>
