<template>
	<div class="g-reporting__flow-content">
		<div class="g-reporting__flow-content-droppable">
			<template
				v-for="(group, groupId, idxGroup) in totals"
				:key="groupId">
				<q-row-container is-large>
					<q-row-container style="padding: 0.5rem 0 0.05rem 0.5rem">
						<span
							v-if="idxGroup === 0"
							class="group-title">
							{{ texts.general }}
						</span>
						<span
							v-else
							class="group-title">
							{{ texts.group }} {{ idxGroup }}
						</span>
					</q-row-container>

					<q-row-container>
						<ul v-if="!isEmpty(group)">
							<template
								v-for="totalField in group"
								:key="totalField.internalKey">
								<li class="g-reporting__flow-content-droppable-item">
									<div class="row">
										<div class="col-12">
											<strong>[{{ getCavText(totalField.tableTitle) }}]</strong>
										</div>
									</div>

									<div class="row">
										<div class="col-12">
											{{ getCavText(totalField.Title) }}
										</div>
									</div>

									<div class="row">
										<div class="col-12">
											<q-multi-check-boxes-input
												v-model="totalField.selectedTotalTypes"
												:options="totalType[totalField.type]" />
										</div>
									</div>
								</li>
							</template>
						</ul>
					</q-row-container>
				</q-row-container>
			</template>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import { v4 as uuidv4 } from 'uuid'
	import _isEmpty from 'lodash-es/isEmpty'
	import _map from 'lodash-es/map'
	import _filter from 'lodash-es/filter'
	import _assignIn from 'lodash-es/assignIn'
	import _find from 'lodash-es/find'
	import _foreach from 'lodash-es/forEach'

	import { totals as cavArraysTotals } from '@/api/genio/cavArrays.js'
	import hardcodedTexts from '@/hardcodedTexts.js'

	export default {
		name: 'QCavTotals',

		emits: [
			'update-totals'
		],

		props: {
			/**
			 * The array of tables that should be considered for generating the list of totals.
			 */
			tables: {
				type: Array,
				default: () => []
			},

			/**
			 * The array of total fields per group that should be displayed.
			 */
			totalFieldsPerGroup: {
				type: Array,
				default: () => []
			},

			/**
			 * An array of currently selected totals.
			 */
			selectedTotals: {
				type: Array,
				default: () => []
			}
		},

		inject: [
			'getCavText'
		],

		expose: [],

		data()
		{
			const cavTotals = cavArraysTotals.setResources(this.$getResource)
			return {
				totalType: {
					A: _filter(cavTotals.elements, (t) => ['COUNT', 'MAX', 'MIN'].includes(t.key)),
					D: _filter(cavTotals.elements, (t) => ['COUNT', 'MAX', 'MIN'].includes(t.key)),
					H: _filter(cavTotals.elements, (t) => ['COUNT', 'MAX', 'MIN'].includes(t.key)),
					B: _filter(cavTotals.elements, (t) => ['SUM', 'AVG', 'COUNT', 'MAX', 'MIN'].includes(t.key)),
					N: _filter(cavTotals.elements, (t) => ['SUM', 'AVG', 'COUNT', 'MAX', 'MIN'].includes(t.key)),
					T: _filter(cavTotals.elements, (t) => ['COUNT', 'MAX', 'MIN'].includes(t.key)),
					$: _filter(cavTotals.elements, (t) => ['SUM', 'AVG', 'COUNT', 'MAX', 'MIN'].includes(t.key))
				},

				totals: {},

				texts: {
					general: computed(() => this.Resources[hardcodedTexts.general]),
					group: computed(() => this.Resources[hardcodedTexts.group])
				}
			}
		},

		created()
		{
			if (!_isEmpty(this.totalFieldsPerGroup))
				_foreach(this.totalFieldsPerGroup, (group, groupIndex) => this.totals[uuidv4()] = this.hydrateTotalGroup(group, groupIndex))
		},

		beforeMount()
		{
			this.$eventHub.emit('main-container-is-visible', false)
		},

		beforeUnmount()
		{
			const eventData = []

			_foreach(this.totals, (totalGroup) => {
				const groupEventData = []
				if (!_isEmpty(totalGroup))
				{
					_foreach(totalGroup, (totalGroupField) => {
						_foreach(totalGroupField.selectedTotalTypes, (selectedTotalType) => {
							groupEventData
								.push(_assignIn({}, totalGroupField,
									{
										TotalType: selectedTotalType,
										selectedTotalTypes: undefined,
										internalKey: undefined
									}))
						})
					})
				}
				eventData.push(groupEventData)
			})

			this.$emit('update-totals', eventData)
		},

		methods: {
			isEmpty: _isEmpty,

			/**
			 * Adds the data necessary for display and interaction to each total field.
			 * @param {Object} total - The total object to hydrate.
			 * @param {Number} groupIndex - The index of the group to which the total belongs.
			 */
			hydrateTotal(total, groupIndex)
			{
				const table = _find(this.tables, (t) => t.Id === total.TableId),
					field = _find(table.Fields, (f) => f.Id === total.FieldId)

				const extendedTotal = _assignIn(total, {
					internalKey: uuidv4(),
					tableTitle: table.Description,
					Title: field.Description,
					selectedTotalTypes: [],
					type: field.Type
				})

				if (!_isEmpty(this.selectedTotals) && !_isEmpty(this.selectedTotals[groupIndex]))
					_foreach((this.selectedTotals[groupIndex][total.FieldId] || []), (t) => extendedTotal.selectedTotalTypes.push(t.TotalType))

				return extendedTotal
			},

			/**
			 * Adds necessary data for display and interaction to each group of total fields.
			 * @param {Array} group - The array of total objects to hydrate into a group.
			 * @param {Number} groupIndex - The index of the group being hydrated.
			 */
			hydrateTotalGroup(group, groupIndex)
			{
				return _map(group, (item) => this.hydrateTotal(item, groupIndex))
			}
		}
	}
</script>
