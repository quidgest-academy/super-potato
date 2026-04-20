<template>
	<div
		ref="droppableArea"
		class="g-reporting__flow-content"
		style="height: 100%">
		<div class="g-reporting__flow-content-header">
			<h4>
				<q-icon icon="list" />
				{{ getCavText(baseTableDescription) }}
			</h4>
		</div>

		<div class="g-reporting__flow-content-subtitle">
			<q-icon
				icon="stats"
				class="q-icon--reporting" />
			{{ texts.fields }}
		</div>

		<div class="g-reporting__flow-content-droppable">
			<ul>
				<template
					v-for="field in fields"
					:key="field.id">
					<li class="g-reporting__flow-content-droppable-item">
						<div
							class="delete"
							style="position: relative; float: right; cursor: pointer">
							<a
								@click.stop.prevent="removeItem(field.id)"
								:title="texts.delete">
								<q-icon
									icon="bin"
									class="q-icon--reporting" />
								<span class="hidden-elem">{{ texts.delete }}</span>
							</a>
						</div>

						<div class="text-list">
							<q-icon
								icon="list"
								class="q-icon--reporting" />
							{{ $getResource(field.tableTitle) }}
						</div>

						<div class="text-list">
							<strong>{{ $getResource(field.title) }}</strong>
						</div>
					</li>
				</template>
			</ul>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import _reduce from 'lodash-es/reduce'
	import _find from 'lodash-es/find'

	// Core SortableJS (without default plugins)
	import Sortable from 'sortablejs/modular/sortable.core.esm.js'

	import hardcodedTexts from '@/hardcodedTexts.js'

	/**
	 * @displayName Fields
	 */
	export default {
		name: 'QCavFields',

		emits: [
			'add-cav-field',
			'remove-cav-field'
		],

		props: {
			/**
			 * Array to hold the previously selected fields
			 */
			fieldsSelectedList: {
				type: Array,
				default: () => []
			},

			/**
			 * List of tables read from CAV metadata
			 */
			tables: {
				type: Array,
				default: () => []
			},

			/**
			 * Base table for the report
			 */
			baseTableDescription: {
				type: String,
				default: ''
			}
		},

		inject: [
			'getCavText'
		],

		expose: [],

		data()
		{
			return {
				texts: {
					fields: computed(() => this.Resources[hardcodedTexts.fields]),
					delete: computed(() => this.Resources[hardcodedTexts.delete])
				},

				sortablePlugin: null
			}
		},

		beforeMount()
		{
			this.$eventHub.emit('main-container-is-visible', true)
		},

		mounted()
		{
			this.initialize()
		},

		beforeUnmount()
		{
			this.draggableDestroy()
		},

		computed: {
			fields()
			{
				return _reduce(this.fieldsSelectedList, (acc, item) => {
					const [tableName] = item.FieldId.split('.')
					const table = _find(this.tables, ['Id', tableName])
					const field = _find(table?.Fields, ['Id', item.FieldId])

					if (table && field)
					{
						acc.push({
							id: item.FieldId,
							title: field.Description,
							tableTitle: table.Description
						})
					}

					return acc
				}, [])
			}
		},

		methods: {
			removeItem(fieldId)
			{
				this.$emit('remove-cav-field', fieldId)
			},

			draggableOnAdd(evt)
			{
				/**
				 * Workaround to put the original control back in place.
				 * SortableJS when creating clone, leaves it in place of the original control.
				 */
				evt.clone.replaceWith(evt.item)

				const cavFieldData = JSON.parse(evt.originalEvent.dataTransfer.getData('cav-field-info'))
				this.$emit('add-cav-field', cavFieldData)
			},

			async initialize()
			{
				this.draggableDestroy()

				try
				{
					this.sortablePlugin = Sortable.create(Array.isArray(this.$refs.droppableArea) ? this.$refs.droppableArea[0] : this.$refs.droppableArea, {
						group: { name: 'cav-fields', pull: false, put: true },
						sort: false,
						draggable: '[data-draggable]',
						dataIdAttr: 'id',
						onAdd: this.draggableOnAdd
					})
				}
				catch
				{
					this.draggableDestroy()
				}
			},

			draggableDestroy()
			{
				if (this.sortablePlugin && this.sortablePlugin.destroy)
					this.sortablePlugin.destroy()
				this.sortablePlugin = null
			}
		}
	}
</script>
