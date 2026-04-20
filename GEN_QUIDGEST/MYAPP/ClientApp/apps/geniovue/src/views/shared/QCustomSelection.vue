<template>
	<teleport
		v-if="!isHidden"
		to="#q-modal-form-custom-selection-body">
		<div :class="['form-flow']">
			<q-row-container is-large>
				<q-control-wrapper>
					<q-table
						v-if="tableManu.rows.length > 0"
						v-bind="tableManu"
						v-on="tableManu.handlers" />
				</q-control-wrapper>
			</q-row-container>
		</div>

		<q-row-container>
			<div id="footer-action-btns">
				<template
					v-for="btn in formButtons"
					:key="btn.id">
					<q-button
						v-if="btn.isActive && btn.isVisible && btn.showInFooter"
						:id="`bottom-${btn.id}`"
						:label="btn.text"
						:variant="btn.variant"
						:class="btn.classes"
						:disabled="btn.disabled"
						:icon-pos="btn.iconPos"
						@click="
							btn.action();
							btn.emitAction ? $emit(btn.emitAction.name, btn.emitAction.params) : null
						">
						<q-icon
							v-if="btn.icon"
							v-bind="btn.icon" />
					</q-button>
				</template>
			</div>
		</q-row-container>
	</teleport>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import controlClass from '@/mixins/fieldControl.js'
	import listHandlers from '@/mixins/listHandlers.js'
	import NavHandlers from '@/mixins/navHandlers.js'
	import listColumnTypes from '@/mixins/listColumnTypes.js'

	export default {
		name: 'QCustomSelection',

		emits: [
			'confirm-selection',
			'cancel-selection'
		],

		mixins: [
			listHandlers,
			NavHandlers
		],

		props: {
			/**
			 * Fields to be shown on the list
			 */
			fields: {
				type: Array,
				default: () => []
			},

			/**
			 * Whether to show or hide the control
			 */
			isHidden: {
				type: Boolean,
				default: true
			}
		},

		expose: [],

		data()
		{
			return {
				tableManu: new controlClass.TableListControl({
					rows: [],
					totalRows: 0,
					headerLevel: 1,
					columnsOriginal: [
						new listColumnTypes.TextColumn({
							order: 1,
							name: 'Record',
							field: 'Record',
							label: computed(() => this.Resources[hardcodedTexts.options]),
							sortable: true,
							initialSort: true
						})
					],
					config: {
						name: 'Records',
						tableTitle: '',
						showFooter: true,
						initialSortColumnName: 'Record',
						initialSortColumnOrder: 'asc',
						crudActions: [],
						perPage: 15,
						numVisiblePaginationButtons: 3,
						showRecordCount: false,
						rowBgColorSelected: '#e0e0e0',
						rowClickActionInternal: 'selectSingle',
						showRowActionText: false
					},
					rowsSelected: {},
					rowsChecked: {},
					handlers: {
						selectRow: eventData => this.tableManu.onSelectRow(eventData),
						unselectRow: eventData => this.tableManu.onUnselectRow(eventData),
						unselectAllRows: () => this.tableManu.onUnselectAllRows()
					}
				}, this),

				formButtons: {
					confirmBtn: {
						id: 'confirm-btn',
						icon: {
							icon: 'check'
						},
						type: 'form-action',
						text: 'Confirm',
						variant: 'bold',
						showInHeader: true,
						showInFooter: true,
						isActive: true,
						isVisible: true,
						disabled: false,
						action: () => {
							if (this.tableManu && this.tableManu.rowsSelected)
								this.$emit('confirm-selection', this.tableManu.rowsSelected)
							else
								this.$emit('confirm-selection')
						}
					},
					cancelBtn: {
						id: 'cancel-btn',
						icon: {
							icon: 'cancel'
						},
						type: 'form-action',
						text: 'Cancel',
						showInHeader: true,
						showInFooter: true,
						isActive: true,
						isVisible: true,
						disabled: false,
						action: () => this.$emit('cancel-selection')
					}
				}
			}
		},

		mounted()
		{
			this.tableManu.init()
			this.init()
		},

		beforeUnmount()
		{
			this.tableManu.destroy()
		},

		methods: {
			init()
			{
				// Initialize table rows
				if (this.fields)
				{
					/* Reset Props */
					// This needs to be done since this method can be called on watch:
					this.tableManu.rows = []
					this.tableManu.rowsSelected = {}
					this.tableManu.rowsChecked = {}
					/* ----------- */

					this.fields.forEach((elem, index) => {
						this.tableManu.rows.push({
							Rownum: index,
							rowKey: index,
							FormMode: '',
							Fields: {
								Record: elem
							},
							btnPermission: {
								editBtnDisabled: true,
								viewBtnDisabled: true,
								deleteBtnDisabled: true,
								insertBtnDisabled: true
							}
						})
					})

					this.tableManu.totalRows = this.fields.length
				}
			}
		},
		watch: {
			fields()
			{
				this.init()
			},

			tableManu: {
				handler(val)
				{
					if (val.rowsSelected && Object.keys(val.rowsSelected).length > 0)
						this.formButtons.confirmBtn.disabled = false
					else
						this.formButtons.confirmBtn.disabled = true
				},
				deep: true
			}
		}
	}
</script>
