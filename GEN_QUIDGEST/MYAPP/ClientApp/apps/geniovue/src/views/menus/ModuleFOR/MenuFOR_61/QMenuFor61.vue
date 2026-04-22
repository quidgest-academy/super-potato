<template>
	<teleport
		v-if="menuModalIsReady"
		:to="`#${uiContainersId.body}`"
		:disabled="!menuInfo.isPopup">
		<div
			class="form-horizontal"
			@submit.prevent>
			<q-row-container>
				<q-table
					v-bind="controls.menu"
					v-on="controls.menu.handlers">
					<template #header>
						<q-table-config
							:table-ctrl="controls.menu"
							v-on="controls.menu.handlers">
						</q-table-config>
					</template>
					<!-- USE /[MANUAL FOR CUSTOM_TABLE FOR_Menu_61]/ -->
				</q-table>
			</q-row-container>
		</div>
	</teleport>

	<teleport
		v-if="menuModalIsReady && hasButtons"
		:to="`#${uiContainersId.footer}`"
		:disabled="!menuInfo.isPopup">
		<q-row-container>
			<div id="footer-action-btns">
				<template
					v-for="btn in menuButtons"
					:key="btn.id">
					<q-button
						v-if="btn.isVisible"
						:id="btn.id"
						:label="btn.text"
						:variant="btn.variant"
						:disabled="btn.disabled"
						:icon-pos="btn.iconPos"
						:class="btn.classes"
						@click="btn.action">
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
	/* eslint-disable @typescript-eslint/no-unused-vars */
	import asyncProcM from '@quidgest/clientapp/composables/async'
	import qEnums from '@quidgest/clientapp/constants/enums'
	import netAPI from '@quidgest/clientapp/network'
	import openQSign from '@quidgest/clientapp/plugins/qSign'
	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
	import { computed, readonly } from 'vue'

	import MenuHandlers from '@/mixins/menuHandlers.js'
	import controlClass from '@/mixins/fieldControl.js'
	import listFunctions from '@/mixins/listFunctions.js'
	import listColumnTypes from '@/mixins/listColumnTypes.js'
	import { resetProgressBar, setProgressBar } from '@/utils/layout.js'

	import { loadResources } from '@/plugins/i18n.js'

	import hardcodedTexts from '@/hardcodedTexts'
	import qApi from '@/api/genio/quidgestFunctions.js'
	import qFunctions from '@/api/genio/projectFunctions.js'
	import qProjArrays from '@/api/genio/projectArrays.js'
	/* eslint-enable @typescript-eslint/no-unused-vars */

	import MenuViewModel from './QMenuFOR_61ViewModel.js'

	const requiredTextResources = ['QMenuFOR_61', 'hardcoded', 'messages']

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FORM_INCLUDEJS FOR_MENU_61]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QMenuFor61',

		mixins: [
			MenuHandlers
		],

		inheritAttrs: false,

		props: {
			/**
			 * Whether or not the menu is used as a homepage.
			 */
			isHomePage: {
				type: Boolean,
				default: false
			}
		},

		expose: [
			'navigationId',
			'onBeforeRouteLeave',
			'updateMenuNavigation'
		],

		data()
		{
			// eslint-disable-next-line
			const vm = this
			return {
				componentOnLoadProc: asyncProcM.getProcListMonitor('QMenuFOR_61', false),

				interfaceMetadata: {
					id: 'QMenuFOR_61', // Used for resources
					requiredTextResources
				},

				menuInfo: {
					id: '61',
					isMenuList: true,
					designation: computed(() => this.Resources.PROPERTIES34868),
					acronym: 'FOR_61',
					name: 'PROPE',
					route: 'menu-FOR_61',
					order: '61',
					controller: 'PROPE',
					action: 'FOR_Menu_61',
					isPopup: false
				},

				model: new MenuViewModel(this),

				controls: {
					menu: new controlClass.TableListControl({
						fnHydrateViewModel: (data) => vm.model.hydrate(data),
						id: 'FOR_Menu_61',
						controller: 'PROPE',
						action: 'FOR_Menu_61',
						hasDependencies: false,
						isInCollapsible: false,
						tableModeClasses: [
							'q-table--full-height',
							'page-full-height'
						],
						columnsOriginal: [
							new listColumnTypes.TextColumn({
								order: 1,
								name: 'Agent.ValName',
								area: 'AGENT',
								field: 'NAME',
								label: computed(() => this.Resources.AGENT_S_NAME42642),
								dataLength: 50,
								scrollData: 30,
								export: 1,
								pkColumn: 'ValCodagent',
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.CurrencyColumn({
								order: 2,
								name: 'ValPrice',
								area: 'PROPE',
								field: 'PRICE',
								label: computed(() => this.Resources.PRICE06900),
								scrollData: 12,
								maxDigits: 9,
								decimalPlaces: 2,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.NumericColumn({
								order: 3,
								name: 'ValBuildage',
								area: 'PROPE',
								field: 'BUILDAGE',
								label: computed(() => this.Resources.BUILDING_AGE27311),
								scrollData: 4,
								maxDigits: 4,
								decimalPlaces: 0,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.NumericColumn({
								order: 4,
								name: 'ValFloornr',
								area: 'PROPE',
								field: 'FLOORNR',
								label: computed(() => this.Resources.FLOOR19993),
								scrollData: 2,
								maxDigits: 2,
								decimalPlaces: 0,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.NumericColumn({
								order: 5,
								name: 'ValId',
								area: 'PROPE',
								field: 'ID',
								scrollData: 5,
								maxDigits: 5,
								decimalPlaces: 0,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.TextColumn({
								order: 6,
								name: 'City.ValCity',
								area: 'CITY',
								field: 'CITY',
								label: computed(() => this.Resources.CITY42505),
								dataLength: 50,
								scrollData: 30,
								export: 1,
								pkColumn: 'ValCodcity',
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.ArrayColumn({
								order: 7,
								name: 'ValBuildtyp',
								area: 'PROPE',
								field: 'BUILDTYP',
								label: computed(() => this.Resources.BUILDING_TYPE57152),
								dataLength: 1,
								scrollData: 1,
								export: 1,
								array: computed(() => new qProjArrays.QArrayBuildtyp(vm.$getResource).elements),
								arrayDisplayMode: 'D',
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.DateColumn({
								order: 8,
								name: 'ValDtconst',
								area: 'PROPE',
								field: 'DTCONST',
								label: computed(() => this.Resources.CONSTRUCTION_DATE18132),
								scrollData: 8,
								dateTimeType: 'date',
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.TextColumn({
								order: 9,
								name: 'ValDescript',
								area: 'PROPE',
								field: 'DESCRIPT',
								label: computed(() => this.Resources.DESCRIPTION07383),
								scrollData: 30,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.DateColumn({
								order: 10,
								name: 'ValDtsold',
								area: 'PROPE',
								field: 'DTSOLD',
								label: computed(() => this.Resources.SOLD_DATE37976),
								scrollData: 8,
								dateTimeType: 'date',
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.ArrayColumn({
								order: 11,
								name: 'ValTypology',
								area: 'PROPE',
								field: 'TYPOLOGY',
								label: computed(() => this.Resources.BUILDING_TYPOLOGY54011),
								scrollData: 1,
								maxDigits: 1,
								decimalPlaces: 0,
								export: 1,
								array: computed(() => new qProjArrays.QArrayTypology(vm.$getResource).elements),
								arrayDisplayMode: 'D',
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.TextColumn({
								order: 12,
								name: 'ValTitle',
								area: 'PROPE',
								field: 'TITLE',
								label: computed(() => this.Resources.TITLE21885),
								dataLength: 50,
								scrollData: 30,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.ImageColumn({
								order: 13,
								name: 'ValPhoto',
								area: 'PROPE',
								field: 'PHOTO',
								label: computed(() => this.Resources.MAIN_PHOTO16044),
								dataTitle: computed(() => genericFunctions.formatString(vm.Resources.IMAGEM_UTILIZADA_PAR58591, vm.Resources.MAIN_PHOTO16044)),
								scrollData: 3,
								sortable: false,
								searchable: false,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
						],
						config: {
							name: 'FOR_Menu_61',
							serverMode: true,
							pkColumn: 'ValCodprope',
							tableAlias: 'PROPE',
							tableNamePlural: computed(() => this.Resources.PROPERTIES34868),
							viewManagement: '',
							showLimitsInfo: true,
							tableTitle: computed(() => this.Resources.PROPERTIES34868),
							showAlternatePagination: true,
							permissions: {
							},
							searchBarConfig: {
								visibility: true
							},
							allowColumnFilters: true,
							allowColumnSort: true,
							crudActions: [
								{
									id: 'show',
									name: 'show',
									title: computed(() => this.Resources.CONSULTAR57388),
									icon: {
										icon: 'view'
									},
									isInReadOnly: true,
									params: {
										action: vm.openFormAction,
										type: 'form',
										formName: 'PROPERTY',
										mode: 'SHOW',
										isControlled: true
									}
								},
								{
									id: 'edit',
									name: 'edit',
									title: computed(() => this.Resources.EDITAR11616),
									icon: {
										icon: 'pencil'
									},
									isInReadOnly: true,
									params: {
										action: vm.openFormAction,
										type: 'form',
										formName: 'PROPERTY',
										mode: 'EDIT',
										isControlled: true
									}
								},
								{
									id: 'duplicate',
									name: 'duplicate',
									title: computed(() => this.Resources.DUPLICAR09748),
									icon: {
										icon: 'duplicate'
									},
									isInReadOnly: true,
									params: {
										action: vm.openFormAction,
										type: 'form',
										formName: 'PROPERTY',
										mode: 'DUPLICATE',
										isControlled: true
									}
								},
								{
									id: 'delete',
									name: 'delete',
									title: computed(() => this.Resources.ELIMINAR21155),
									icon: {
										icon: 'delete'
									},
									isInReadOnly: true,
									params: {
										action: vm.openFormAction,
										type: 'form',
										formName: 'PROPERTY',
										mode: 'DELETE',
										isControlled: true
									}
								}
							],
							generalActions: [
								{
									id: 'insert',
									name: 'insert',
									title: computed(() => this.Resources.INSERIR43365),
									icon: { icon: 'add' },
									isInReadOnly: true,
									params: {
										action: vm.openFormAction,
										type: 'form',
										formName: 'PROPERTY',
										mode: 'NEW',
										repeatInsertion: false,
										isControlled: true
									}
								},
							],
							generalCustomActions: [
							],
							groupActions: [
							],
							customActions: [
							],
							MCActions: [
							],
							rowClickAction: {
								id: 'RCA_FOR_611',
								name: 'form-PROPERTY',
								isVisible: true,
								params: {
									isRoute: true,
									limits: [
										{
											identifier: 'id',
											fnValueSelector: (row) => row.ValCodprope
										},
									],
									isControlled: true,
									action: vm.openFormAction, type: 'form', mode: 'SHOW', formName: 'PROPERTY'
								}
							},
							formsDefinition: {
								'PROPERTY': {
									fnKeySelector: (row) => row.Fields.ValCodprope,
									isPopup: false
								},
							},
							defaultSearchColumnName: 'ValTitle',
							defaultSearchColumnNameOriginal: 'ValTitle',
							defaultColumnSorting: {
								columnName: 'ValDtconst',
								sortOrder: 'asc'
							}
						},
						globalEvents: ['changed-AGENT', 'changed-CITY', 'changed-PROPE'],
						uuid: 'b720d491-ed3d-40f1-91ef-c85d9c3ac854',
						allSelectedRows: 'false',
						headerLevel: 1,
						isActiveControl: computed(() => this.isActiveMenu)
					}, this),
				}
			}
		},

		beforeRouteEnter(to, _, next)
		{
			// called before the route that renders this component is confirmed.
			// does NOT have access to `this` component instance,
			// because it has not been created yet when this guard is called!

			next((vm) => vm.updateMenuNavigation(to))
		},

		beforeRouteLeave(to, _, next)
		{
			this.onBeforeRouteLeave(next)
		},

		mounted()
		{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FORM_CODEJS FOR_MENU_61]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		beforeUnmount()
		{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR COMPONENT_BEFORE_UNMOUNT FOR_MENU_61]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		methods: {
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FUNCTIONS_JS FOR_61]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR LISTING_CODEJS FOR_MENU_61]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		}
	}
</script>
