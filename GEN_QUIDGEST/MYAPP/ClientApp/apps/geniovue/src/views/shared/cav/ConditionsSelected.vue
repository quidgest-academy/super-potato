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
							id="cav-conditions-select">
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
												@click.stop.prevent="addNewCondition(field)">
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
			<div>
				<h4 class="g-reporting__flow-subtitle">{{ texts.conditionsToApply }}</h4>
			</div>

			<div class="g-reporting__flow-content">
				<div class="g-reporting__flow-content-droppable">
					<ul v-if="!isEmpty(conditions)">
						<li
							v-for="cond in conditions"
							:key="cond.internalKey"
							class="g-reporting__flow-content-droppable-item">
							<div style="padding: 2rem 0 1rem 0; float: left">
								<q-icon :icon="cond.completed ? 'ok' : 'remove'" />
							</div>

							<div style="padding-left: 1.5rem">
								<div class="row">
									<div class="col-12">
										<strong>[{{ getCavText(cond.tableTitle) }}]</strong>

										<q-button @click="removeCondition(cond.internalKey)">
											<q-icon
												icon="bin"
												class="g-reporting__i-remove" />
										</q-button>
									</div>
								</div>

								<div class="row">
									<div class="col-12">
										<span>{{ getCavText(cond.fieldTitle) }}</span>

										<span
											v-if="!isEmpty(cond.Operation)"
											class="result1 condition">
											{{ find(typesToOperands[cond.typeOperand], (o) => o.key === cond.Operation).value }}
										</span>

										<span
											class="result2 condition"
											style="word-break: break-word">
											{{ formatConditionValue(cond) }}
										</span>
									</div>
								</div>

								<q-row-container
									class="visible-on-hover"
									style="--display-mode: block">
									<q-control-wrapper class="control-join-group">
										<q-control-wrapper class="control-join-group">
											<base-input-structure
												:id="`o-${cond.internalKey}`"
												label-position="left"
												:class="['i-text', { 'i-text--disabled': false }]"
												:label="texts.operator"
												:label-attrs="{ class: 'i-text__label' }">
												<q-select
													v-model="cond.Operation"
													:items="typesToOperands[cond.typeOperand]" />
											</base-input-structure>
										</q-control-wrapper>
									</q-control-wrapper>
								</q-row-container>

								<q-row-container
									class="visible-on-hover"
									style="--display-mode: block">
									<q-control-wrapper class="control-join-group">
										<base-input-structure
											:id="`i-${cond.internalKey}`"
											label-position="left"
											:label="texts.value"
											:label-attrs="{ class: 'i-text__label' }"
											:class="['i-text', { 'i-text--disabled': false }]">
											<q-select
												v-if="cond.fieldType === 'B'"
												v-model="cond.Operands[1].ValueReference"
												:items="logicals" />
											<q-text-field
												v-else
												:id="cond.internalKey"
												size="small"
												style="height: 3%; margin-bottom: 2%"
												v-model="cond.Operands[1].ValueReference"
												:max-length="250" />
										</base-input-structure>
									</q-control-wrapper>
								</q-row-container>

								<q-row-container>
									<q-control-wrapper class="control-join-group">
										<q-checkbox
											:id="`show-nulls-${cond.internalKey}`"
											v-model="cond.Operands[0].ShowNulls"
											:label="texts.showEmptyLines"
											label-placement="left" />
									</q-control-wrapper>
								</q-row-container>
							</div>
						</li>
					</ul>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import { v4 as uuidv4 } from 'uuid'
	import _isEmpty from 'lodash-es/isEmpty'
	import _map from 'lodash-es/map'
	import _filter from 'lodash-es/filter'
	import _startsWith from 'lodash-es/startsWith'
	import _toLower from 'lodash-es/toLower'
	import _assignIn from 'lodash-es/assignIn'
	import _find from 'lodash-es/find'
	import _findIndex from 'lodash-es/findIndex'
	import _join from 'lodash-es/join'
	import _foreach from 'lodash-es/forEach'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import cavArrays from '@/api/genio/cavArrays.js'

	export default {
		name: 'QCavConditionsSelected',

		emits: ['update-conditions'],

		props: {
			/**
			 * The array of tables containing data used for creating query conditions.
			 */
			tables: {
				type: Array,
				default: () => []
			},

			/**
			 * The currently selected conditions represented as a structured object.
			 */
			conditionsSelected: {
				type: Object,
				default: () => ({})
			}
		},

		inject: ['getCavText'],

		expose: [],

		data()
		{
			const cavOperators = cavArrays.operators.setResources(this.$getResource)
			return {
				searchInput: '',

				conditions: [],

				logicals: cavArrays.logical.setResources(this.$getResource).elements,

				typesToOperands: {
					A: _filter(cavOperators.elements, (op) =>
						['EQ', 'GT', 'GET', 'LT', 'LET', 'NEQ', 'LIKE', 'BETWEEN', 'IN', 'ISNULL', 'ISNOTNULL'].includes(op.key)
					),
					D: _filter(cavOperators.elements, (op) =>
						['EQ', 'GT', 'GET', 'LT', 'LET', 'NEQ', 'BETWEEN', 'IN', 'ISNULL', 'ISNOTNULL'].includes(op.key)
					),
					H: _filter(cavOperators.elements, (op) =>
						['EQ', 'GT', 'GET', 'LT', 'LET', 'NEQ', 'BETWEEN', 'IN', 'ISNULL', 'ISNOTNULL'].includes(op.key)
					),
					B: _filter(cavOperators.elements, (op) => ['EQ', 'NEQ', 'ISNULL', 'ISNOTNULL'].includes(op.key)),
					N: _filter(cavOperators.elements, (op) =>
						['EQ', 'GT', 'GET', 'LT', 'LET', 'NEQ', 'BETWEEN', 'IN', 'ISNULL', 'ISNOTNULL'].includes(op.key)
					),
					T: _filter(cavOperators.elements, (op) =>
						['EQ', 'GT', 'GET', 'LT', 'LET', 'NEQ', 'BETWEEN', 'ISNULL', 'ISNOTNULL'].includes(op.key)
					),
					$: _filter(cavOperators.elements, (op) =>
						['EQ', 'GT', 'GET', 'LT', 'LET', 'NEQ', 'BETWEEN', 'IN', 'ISNULL', 'ISNOTNULL'].includes(op.key)
					),
					ARRAY: _filter(cavOperators.elements, (op) => ['EQ', 'NEQ', 'IN', 'ISNULL', 'ISNOTNULL'].includes(op.key))
				},

				texts: {
					fieldSearch: computed(() => this.Resources[hardcodedTexts.fieldSearch]),
					search: computed(() => this.Resources[hardcodedTexts.search]),
					conditionsToApply: computed(() => this.Resources[hardcodedTexts.conditionsToApply]),
					operator: computed(() => this.Resources[hardcodedTexts.operator]),
					value: computed(() => this.Resources[hardcodedTexts.value]),
					showEmptyLines: computed(() => this.Resources[hardcodedTexts.showEmptyLines])
				},

				openGroup: ''
			}
		},

		created()
		{
			if (!_isEmpty(this.conditionsSelected) && !_isEmpty(this.conditionsSelected.Operands))
				_foreach(this.conditionsSelected.Operands, (condition) => this.conditions.push(this.hydrateCondition(condition)))
		},

		beforeMount()
		{
			this.$eventHub.emit('main-container-is-visible', false)
		},

		beforeUnmount()
		{
			const data = {
				Operation: 'AND',
				Operands: this.conditions
			}

			this.$emit('update-conditions', data)
		},

		computed: {
			/**
			 * Computes the tables filtered by the search term entered by the user.
			 */
			filteredTables()
			{
				if (_isEmpty(this.searchInput))
					return this.tables

				return _map(this.tables, (table) => {
					return {
						Id: table.Id,
						TableUpId: table.TableUpId,
						Description: table.Description,
						Fields: _filter(table.Fields, (field) => _startsWith(_toLower(this.getCavText(field.Description)), _toLower(this.searchInput)))
					}
				})
			}
		},

		methods: {
			isEmpty: _isEmpty,

			find: _find,

			/**
			 * Hydrates a condition with additional necessary information.
			 * @param {Object} condition - An existing condition that needs to be hydrated.
			 * @returns {Object} The hydrated condition object ready for display and interaction.
			 */
			hydrateCondition(condition)
			{
				const fieldId = condition.Operands[0].ValueReference,
					splited = fieldId.split('.'),
					table = _find(this.tables, (t) => t.Id === splited[0]),
					field = _find(table.Fields, (f) => f.Id === fieldId),
					fieldArray = !_isEmpty(field.ArrayElements) ? field.ArrayElements[0].ArrayId : null

				const extendedCondition = _assignIn(condition, {
					internalKey: uuidv4(),
					fieldType: field.Type,
					typeOperand: _isEmpty(fieldArray) ? field.Type : 'ARRAY',
					tableTitle: table.Description,
					fieldTitle: field.Description
				})

				if (!Reflect.has(extendedCondition, 'completed'))
				{
					Object.defineProperty(extendedCondition, 'completed', {
						get()
						{
							return (
								this.Operation === 'ISNULL' ||
								this.Operation === 'ISNOTNULL' ||
								(this.Operands.length > 1 &&
									(!_isEmpty(this.Operands[1].ValueReference) ||
										this.Operands[1].ValueReference === 0 ||
										this.Operands[1].ValueReference === 1) &&
									!_isEmpty(this.Operation))
							)
						}
					})
				}

				return extendedCondition
			},

			/**
			 * Creates and adds a new condition to the conditions array based on a field's properties.
			 * @param {Object} field - The field object containing information necessary to create a new condition.
			 */
			addNewCondition(field)
			{
				this.conditions.push(
					this.hydrateCondition({
						Operands: [
							{
								Operation: 'FIELD',
								ShowNulls: false,
								ValueReference: field.Id
							},
							{
								Operation: 'LITERAL',
								ValueReference: null
							}
						],
						Operation: ''
					})
				)
			},

			/**
			 * Removes a condition from the conditions array based on the unique internal key.
			 * @param {String} internalKey - The internal key identifying which condition to remove.
			 */
			removeCondition(internalKey)
			{
				const idxToRemove = _findIndex(this.conditions, (c) => c.internalKey === internalKey)
				if (idxToRemove !== -1)
					this.conditions.splice(idxToRemove, 1)
			},

			/**
			 * Formats the value of a condition for user-friendly display if further processing is needed.
			 * @param {Object} condition - The condition object containing the value to be formatted.
			 * @returns {String} The display-ready formatted value.
			 */
			formatConditionValue(condition)
			{
				let value = condition.Operands[1].ValueReference

				if (condition.Operation === 'IN' || condition.Operation === 'BETWEEN')
				{
					const values = []
					for (let idx = 1; idx < condition.Operands.lenght; idx++)
						value.push(condition.Operands[idx].ValueReference)
					value = _join(values, ', ')
				}

				if (condition.fieldType === 'B' && (value === 0 || value === 1))
					value = _find(this.logicals, (l) => l.key === value).value

				if (_isEmpty(value))
					value = ''

				return value
			}
		}
	}
</script>
