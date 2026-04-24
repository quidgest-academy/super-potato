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
					<!-- USE /[MANUAL FOR CUSTOM_TABLE FOR_Menu_71]/ -->
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

	import MenuViewModel from './QMenuFOR_71ViewModel.js'

	const requiredTextResources = ['QMenuFOR_71', 'hardcoded', 'messages']

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FORM_INCLUDEJS FOR_MENU_71]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QMenuFor71',

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
				componentOnLoadProc: asyncProcM.getProcListMonitor('QMenuFOR_71', false),

				interfaceMetadata: {
					id: 'QMenuFOR_71', // Used for resources
					requiredTextResources
				},

				menuInfo: {
					id: '71',
					isMenuList: true,
					designation: computed(() => this.Resources.STATISTICS11845),
					acronym: 'FOR_71',
					name: 'STATS',
					route: 'menu-FOR_71',
					order: '71',
					controller: 'STATS',
					action: 'FOR_Menu_71',
					isPopup: false
				},

				model: new MenuViewModel(this),

				controls: {
					menu: new controlClass.TableSpecialRenderingControl({
						fnHydrateViewModel: (data) => vm.model.hydrate(data),
						id: 'FOR_Menu_71',
						controller: 'STATS',
						action: 'FOR_Menu_71',
						hasDependencies: false,
						isInCollapsible: false,
						tableModeClasses: [
							'q-table--full-height',
							'page-full-height'
						],
						columnsOriginal: [
							new listColumnTypes.CurrencyColumn({
								order: 1,
								name: 'ValProfit',
								area: 'STATS',
								field: 'PROFIT',
								label: computed(() => this.Resources.PROFIT55910),
								scrollData: 12,
								maxDigits: 9,
								decimalPlaces: 2,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.NumericColumn({
								order: 2,
								name: 'ValNotsold',
								area: 'STATS',
								field: 'NOTSOLD',
								label: computed(() => this.Resources.PROPERTIES_NOT_SOLD48533),
								scrollData: 3,
								maxDigits: 3,
								decimalPlaces: 0,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.TextColumn({
								order: 3,
								name: 'ValCountry',
								area: 'STATS',
								field: 'COUNTRY',
								label: computed(() => this.Resources.COUNTRY64133),
								dataLength: 50,
								scrollData: 30,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.NumericColumn({
								order: 4,
								name: 'ValSold',
								area: 'STATS',
								field: 'SOLD',
								label: computed(() => this.Resources.PROPERTIES_SOLD24318),
								scrollData: 3,
								maxDigits: 3,
								decimalPlaces: 0,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
						],
						config: {
							name: 'FOR_Menu_71',
							serverMode: true,
							pkColumn: 'ValCodstats',
							tableAlias: 'STATS',
							tableNamePlural: computed(() => this.Resources.STATISTICS11845),
							viewManagement: '',
							showLimitsInfo: true,
							tableTitle: computed(() => this.Resources.STATISTICS11845),
							showAlternatePagination: true,
							permissions: {
								canView: false,
								canEdit: false,
								canDuplicate: false,
								canDelete: false,
								canInsert: false
							},
							searchBarConfig: {
								visibility: true
							},
							allowColumnFilters: true,
							allowColumnSort: true,
							generalCustomActions: [
							],
							groupActions: [
							],
							customActions: [
							],
							MCActions: [
							],
							rowClickAction: {
							},
							formsDefinition: {
							},
							defaultSearchColumnName: '',
							defaultSearchColumnNameOriginal: '',
							defaultColumnSorting: {
								columnName: 'ValCountry',
								sortOrder: 'asc'
							}
						},
						globalEvents: ['changed-STATS'],
						uuid: '504c53a8-afd2-497c-8bc1-6523e0ae37ee',
						allSelectedRows: 'false',
						viewModes: [
							{
								id: 'LIST',
								type: 'list',
								subtype: '',
								label: computed(() => this.Resources.LISTA13474),
								order: 1,
								mappingVariables: readonly({
								}),
								styleVariables: {
								},
								groups: {
								}
							},
							{
								id: 'CHART',
								type: 'chart',
								subtype: 'genericgraph',
								label: computed(() => this.Resources.GRAFICO38823),
								order: 2,
								mappingVariables: readonly({
									xaxis: {
										allowsMultiple: false,
										sources: [
											'STATS.COUNTRY',
										]
									},
									yaxis: {
										allowsMultiple: true,
										sources: [
											'STATS.PROFIT',
										]
									},
								}),
								styleVariables: {
									chartType: {
										rawValue: 'column',
										isMapped: false
									},
									firstColor: {
										rawValue: 'undefined',
										isMapped: false
									},
									chartColorArray: {
										rawValue: 'Highcharts Default',
										isMapped: false
									},
									invertColorArray: {
										rawValue: false,
										isMapped: false
									},
									xaxisType: {
										rawValue: 'linear',
										isMapped: false
									},
									yaxisType: {
										rawValue: 'linear',
										isMapped: false
									},
									graphTitle: {
										rawValue: undefined,
										isMapped: false
									},
									description: {
										rawValue: undefined,
										isMapped: false
									},
									alignDescription: {
										rawValue: 'left',
										isMapped: false
									},
									yaxisName: {
										rawValue: undefined,
										isMapped: false
									},
									xaxisName: {
										rawValue: undefined,
										isMapped: false
									},
									groupType: {
										rawValue: 'join',
										isMapped: false
									},
									inverted: {
										rawValue: false,
										isMapped: false
									},
									showLabels: {
										rawValue: true,
										isMapped: false
									},
									showLegend: {
										rawValue: true,
										isMapped: false
									},
									widthPercentage: {
										rawValue: 100,
										isMapped: false
									},
									showPieLabel: {
										rawValue: 'outside',
										isMapped: false
									},
									lineMarker: {
										rawValue: 'enabled',
										isMapped: false
									},
									heightPx: {
										rawValue: 400,
										isMapped: false
									},
									pieInnerSizePercentage: {
										rawValue: 0,
										isMapped: false
									},
									showBreaks: {
										rawValue: false,
										isMapped: false
									},
									enableHover: {
										rawValue: true,
										isMapped: false
									},
									zoomType: {
										rawValue: 'x',
										isMapped: false
									},
									legendLayout: {
										rawValue: 'horizontal',
										isMapped: false
									},
									legendXPosition: {
										rawValue: 0,
										isMapped: false
									},
									showLastN: {
										rawValue: -1,
										isMapped: false
									},
									legendYPosition: {
										rawValue: 0,
										isMapped: false
									},
									legendFloating: {
										rawValue: false,
										isMapped: false
									},
									legendAlign: {
										rawValue: 'center',
										isMapped: false
									},
									legendVerticalAlign: {
										rawValue: 'bottom',
										isMapped: false
									},
									stackingType: {
										rawValue: 'undefined',
										isMapped: false
									},
									valuesDecimals: {
										rawValue: 0,
										isMapped: false
									},
								},
								groups: {
								}
							},
						],
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
// USE /[MANUAL FOR FORM_CODEJS FOR_MENU_71]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		beforeUnmount()
		{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR COMPONENT_BEFORE_UNMOUNT FOR_MENU_71]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		methods: {
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FUNCTIONS_JS FOR_71]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR LISTING_CODEJS FOR_MENU_71]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		}
	}
</script>
