<template>
	<div class="row">
		<div class="col-md-6">
			<div class="g-reporting__flow-search">
				<q-input-group>
					<q-text-field
						v-model="searchInput"
						:placeholder="texts.fieldSearch" />
					<template #append>
						<q-button
							id="search-button"
							:title="texts.search">
							<q-icon icon="search" />
						</q-button>
					</template>
				</q-input-group>
			</div>

			<div class="g-reporting__flow-content-selectable">
				<q-row-container is-large>
					<q-control-wrapper class="row-line-group">
						<q-accordion
							v-model="openGroup"
							id="cav-orderings-select">
							<template
								v-for="table in filteredTables"
								:key="table.Id">
								<q-accordion-item
									v-if="!isEmpty(table.Fields)"
									:value="table.Id"
									:title="getCavText(table.Description)">
									<ul>
										<template
											v-for="field in table.Fields"
											:key="field.Id">
											<li
												v-if="!isEmpty(field.Description)"
												class="g-reporting__flow-content-selectable-item"
												@click.stop.prevent="addNewOrderingField(field)">
												{{ getCavText(field.Description) }}
												<q-icon
													icon="circle-arrow-right"
													class="q-icon--reporting" />
											</li>
										</template>
									</ul>
								</q-accordion-item>
							</template>
						</q-accordion>
					</q-control-wrapper>
				</q-row-container>
			</div>
		</div>

		<div class="col-md-6">
			<div class="row">
				<div class="col">
					<div>
						<h4 class="g-reporting__flow-subtitle">{{ texts.orderingToApply }}</h4>
					</div>

					<div class="g-reporting__flow-content">
						<div class="g-reporting__flow-content-droppable">
							<draggable
								tag="ul"
								item-key="internalKey"
								:list="orderings">
								<template #item="{ element }">
									<li class="g-reporting__flow-content-droppable-item">
										<div class="row">
											<div class="col-12">
												<strong>[{{ getCavText(element.Field.tableTitle) }}]</strong>

												<q-button @click="removeOrder(element.internalKey)">
													<q-icon
														icon="bin"
														class="g-reporting__i-remove" />
												</q-button>
											</div>
										</div>

										<div class="row">
											<div class="col-12">
												{{ getCavText(element.Field.Title) }}

												<div
													class="orders"
													@click.stop.prevent="toggleDirection(element.internalKey)">
													<span>
														{{ capitalize(element.Direction) }}
													</span>

													<q-icon :icon="element.Direction === 'ASC' ? 'sort-ascending' : 'sort-descending'" />
												</div>
											</div>
										</div>
									</li>
								</template>
							</draggable>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed, defineAsyncComponent } from 'vue'
	import { v4 as uuidv4 } from 'uuid'
	import _isEmpty from 'lodash-es/isEmpty'
	import _map from 'lodash-es/map'
	import _filter from 'lodash-es/filter'
	import _startsWith from 'lodash-es/startsWith'
	import _toLower from 'lodash-es/toLower'
	import _merge from 'lodash-es/merge'
	import _find from 'lodash-es/find'
	import _findIndex from 'lodash-es/findIndex'
	import _foreach from 'lodash-es/forEach'
	import _some from 'lodash-es/some'
	import _capitalize from 'lodash-es/capitalize'

	import hardcodedTexts from '@/hardcodedTexts.js'

	export default {
		name: 'QCavOrderBySelected',

		emits: ['update-orderings'],

		components: {
			draggable: defineAsyncComponent(() => import('vuedraggable'))
		},

		props: {
			/**
			 * Tables
			 */
			tables: {
				type: Array,
				default: () => []
			},

			/**
			 * Previously selected fields
			 */
			fieldsSelectedList: {
				type: Array,
				default: () => []
			},

			/**
			 * Previously selected orderings
			 */
			selectedOrderings: {
				type: Array,
				default: () => []
			}
		},

		inject: ['getCavText'],

		expose: [],

		data()
		{
			return {
				searchInput: '',

				orderings: [],

				texts: {
					fieldSearch: computed(() => this.Resources[hardcodedTexts.fieldSearch]),
					search: computed(() => this.Resources[hardcodedTexts.search]),
					orderingToApply: computed(() => this.Resources[hardcodedTexts.orderingToApply])
				},

				openGroup: ''
			}
		},

		created()
		{
			if (!_isEmpty(this.selectedOrderings))
				_foreach(this.selectedOrderings, (order) => this.orderings.push(this.hydrateOrderField(order)))
		},

		beforeMount()
		{
			this.$eventHub.emit('main-container-is-visible', false)
		},

		beforeUnmount()
		{
			this.$emit('update-orderings', this.orderings)
		},

		computed: {
			filteredTables()
			{
				return _map(this.tables, (table) => {
					return {
						Id: table.Id,
						TableUpId: table.TableUpId,
						Description: table.Description,
						Fields: _filter(table.Fields, (field) => {
							return (
								_findIndex(this.fieldsSelectedList, (f) => f.FieldId === field.Id) !== -1 &&
								!_some(this.orderings, { Field: { FieldId: field.Id } }) &&
								(_isEmpty(this.searchInput)
									? true
									: _startsWith(_toLower(this.getCavText(field.Description)), _toLower(this.searchInput)))
							)
						})
					}
				})
			}
		},

		methods: {
			isEmpty: _isEmpty,

			capitalize: _capitalize,

			hydrateOrderField(order)
			{
				const table = _find(this.tables, (t) => t.Id === order.Field.TableId),
					field = _find(table.Fields, (f) => f.Id === order.Field.FieldId)

				const extendedOrder = _merge({}, order, {
					internalKey: uuidv4(),
					Field: {
						tableTitle: table.Description,
						Title: field.Description
					}
				})

				return extendedOrder
			},

			addNewOrderingField(field)
			{
				this.orderings.push(
					this.hydrateOrderField({
						Direction: 'ASC',
						Field: {
							FieldId: field.Id,
							TableId: field.TableId
						}
					})
				)
			},

			/**
			 * Remove order by internal key
			 */
			removeOrder(internalKey)
			{
				const idxToRemove = _findIndex(this.orderings, (o) => o.internalKey === internalKey)
				if (idxToRemove !== -1)
					this.orderings.splice(idxToRemove, 1)
			},

			toggleDirection(internalKey)
			{
				const idxToChange = _findIndex(this.orderings, (o) => o.internalKey === internalKey)

				if (idxToChange !== -1)
				{
					if (this.orderings[idxToChange].Direction === 'ASC')
						this.orderings[idxToChange].Direction = 'DESC'
					else
						this.orderings[idxToChange].Direction = 'ASC'
				}
			}
		}
	}
</script>
