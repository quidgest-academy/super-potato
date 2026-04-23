<template>
	<teleport
		v-if="formModalIsReady && showFormHeader"
		:to="`#${uiContainersId.header}`"
		:disabled="!isPopup || isNested">
		<div
			ref="formHeader"
			:class="{ 'c-sticky-header': isStickyHeader, 'sticky-top': isStickyTop }">
			<div
				v-if="showFormHeader"
				class="c-action-bar">
				<component
					v-if="formControl.uiComponents.header && formInfo.designation"
					:is="topHeadingTag"
					:id="formTitleId"
					class="form-header">
					{{ formInfo.designation }}
				</component>

				<div class="c-action-bar__menu">
					<template
						v-for="(section, sectionId) in formButtonSections"
						:key="sectionId">
						<span
							v-if="showHeadingSep(sectionId)"
							class="main-title-sep" />

						<q-toggle-group
							v-if="formControl.uiComponents.headerButtons"
							borderless>
							<template
								v-for="btn in section"
								:key="btn.id">
								<q-toggle-group-item
									v-if="showFormHeaderButton(btn)"
									:model-value="btn.isSelected"
									:id="`top-${btn.id}`"
									:title="btn.text"
									:label="btn.label"
									:disabled="btn.disabled"
									@click="btn.action">
									<template v-if="btn.icon">
										<q-badge-indicator
											:enabled="btn.badge?.isVisible ?? false"
											:color="btn.badge?.color">
											<q-icon v-bind="btn.icon" />
										</q-badge-indicator>
									</template>
								</q-toggle-group-item>
							</template>
						</q-toggle-group>
					</template>
				</div>
			</div>

			<q-anchor-container-horizontal
				v-if="$app.layout.FormAnchorsPosition === 'form-header' && visibleGroups.length > 0"
				:anchors="anchorGroups"
				:controls="visibleControls"
				@focus-control="focusControl" />
		</div>
	</teleport>

	<teleport
		v-if="formModalIsReady && showFormBody"
		:to="`#${uiContainersId.body}`"
		:disabled="!isPopup || isNested">
		<q-validation-summary
			:messages="validationErrors"
			@error-clicked="focusField" />

		<div :class="[`float-${actionsPlacement}`, 'c-action-bar']">
			<q-button-group borderless>
				<template
					v-for="btn in formButtons"
					:key="btn.id">
					<q-button
						v-if="btn.isActive && btn.isVisible && btn.showInHeading"
						:id="`heading-${btn.id}`"
						:label="btn.text"
						:color="btn.color"
						:variant="btn.variant"
						:disabled="btn.disabled"
						:icon-pos="btn.iconPos"
						:class="btn.classes"
						@click="btn.action(); btn.emitAction ? $emit(btn.emitAction.name, btn.emitAction.params) : null">
						<q-icon
							v-if="btn.icon"
							v-bind="btn.icon" />
					</q-button>
				</template>
			</q-button-group>
		</div>

		<q-container
			fluid
			data-key="CONTACT"
			:data-identifier="primaryKeyValue"
			:data-loading="!formInitialDataLoaded || !isActiveForm">
			<template v-if="formControl.initialized && showFormBody">
				<q-row v-if="controls.CONTACT_CONTADATE____.isVisible || controls.CONTACT_PROPETITLE___.isVisible">
					<q-col
						v-if="controls.CONTACT_CONTADATE____.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.CONTACT_CONTADATE____.isVisible"
							class="i-text"
							v-bind="controls.CONTACT_CONTADATE____.wrapperProps"
							:id="getControlId(controls.CONTACT_CONTADATE____)"
							v-on="controls.CONTACT_CONTADATE____.handlers"
							:loading="controls.CONTACT_CONTADATE____.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-date-time-picker
								v-if="controls.CONTACT_CONTADATE____.isVisible"
								v-bind="controls.CONTACT_CONTADATE____.props"
								:id="getControlId(controls.CONTACT_CONTADATE____)"
								:model-value="model.ValDate.value"
								@reset-icon-click="model.ValDate.fnUpdateValue(model.ValDate.originalValue ?? new Date())"
								@update:model-value="model.ValDate.fnUpdateValue($event ?? '')" />
						</base-input-structure>
					</q-col>
					<q-col
						v-if="controls.CONTACT_PROPETITLE___.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.CONTACT_PROPETITLE___.isVisible"
							class="i-text"
							v-bind="controls.CONTACT_PROPETITLE___.wrapperProps"
							:id="getControlId(controls.CONTACT_PROPETITLE___)"
							v-on="controls.CONTACT_PROPETITLE___.handlers"
							:loading="controls.CONTACT_PROPETITLE___.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-lookup
								v-if="controls.CONTACT_PROPETITLE___.isVisible"
								v-bind="controls.CONTACT_PROPETITLE___.props"
								:id="getControlId(controls.CONTACT_PROPETITLE___)"
								v-on="controls.CONTACT_PROPETITLE___.handlers" />
							<q-see-more-contact-propetitle
								v-if="controls.CONTACT_PROPETITLE___.seeMoreIsVisible"
								v-bind="controls.CONTACT_PROPETITLE___.seeMoreParams"
								v-on="controls.CONTACT_PROPETITLE___.handlers" />
						</base-input-structure>
					</q-col>
				</q-row>
				<q-row v-if="controls.CONTACT_CONTACLIENT__.isVisible || controls.CONTACT_CONTAEMAIL___.isVisible || controls.CONTACT_CONTAPHONE___.isVisible">
					<q-col
						v-if="controls.CONTACT_CONTACLIENT__.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.CONTACT_CONTACLIENT__.isVisible"
							class="i-text"
							v-bind="controls.CONTACT_CONTACLIENT__.wrapperProps"
							:id="getControlId(controls.CONTACT_CONTACLIENT__)"
							v-on="controls.CONTACT_CONTACLIENT__.handlers"
							:loading="controls.CONTACT_CONTACLIENT__.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-text-field
								v-bind="controls.CONTACT_CONTACLIENT__.props"
								:id="getControlId(controls.CONTACT_CONTACLIENT__)"
								@blur="onBlur(controls.CONTACT_CONTACLIENT__, model.ValClient.value)"
								@change="model.ValClient.fnUpdateValueOnChange" />
						</base-input-structure>
					</q-col>
					<q-col
						v-if="controls.CONTACT_CONTAEMAIL___.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.CONTACT_CONTAEMAIL___.isVisible"
							class="i-text"
							v-bind="controls.CONTACT_CONTAEMAIL___.wrapperProps"
							:id="getControlId(controls.CONTACT_CONTAEMAIL___)"
							v-on="controls.CONTACT_CONTAEMAIL___.handlers"
							:loading="controls.CONTACT_CONTAEMAIL___.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-text-field
								v-bind="controls.CONTACT_CONTAEMAIL___.props"
								:id="getControlId(controls.CONTACT_CONTAEMAIL___)"
								@blur="onBlur(controls.CONTACT_CONTAEMAIL___, model.ValEmail.value)"
								@change="model.ValEmail.fnUpdateValueOnChange" />
						</base-input-structure>
					</q-col>
					<q-col
						v-if="controls.CONTACT_CONTAPHONE___.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.CONTACT_CONTAPHONE___.isVisible"
							class="i-text"
							v-bind="controls.CONTACT_CONTAPHONE___.wrapperProps"
							:id="getControlId(controls.CONTACT_CONTAPHONE___)"
							v-on="controls.CONTACT_CONTAPHONE___.handlers"
							:loading="controls.CONTACT_CONTAPHONE___.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-mask
								v-if="controls.CONTACT_CONTAPHONE___.isVisible"
								v-bind="controls.CONTACT_CONTAPHONE___.props"
								:id="getControlId(controls.CONTACT_CONTAPHONE___)"
								:model-value="model.ValPhone.value"
								@change="model.ValPhone.fnUpdateValueOnChange" />
						</base-input-structure>
					</q-col>
				</q-row>
				<q-row v-if="controls.CONTACT_CONTADESCRIPT.isVisible">
					<q-col
						v-if="controls.CONTACT_CONTADESCRIPT.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.CONTACT_CONTADESCRIPT.isVisible"
							class="i-textarea"
							v-bind="controls.CONTACT_CONTADESCRIPT.wrapperProps"
							:id="getControlId(controls.CONTACT_CONTADESCRIPT)"
							v-on="controls.CONTACT_CONTADESCRIPT.handlers"
							:loading="controls.CONTACT_CONTADESCRIPT.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-text-area
								v-if="controls.CONTACT_CONTADESCRIPT.isVisible"
								v-bind="controls.CONTACT_CONTADESCRIPT.props"
								:id="getControlId(controls.CONTACT_CONTADESCRIPT)"
								v-on="controls.CONTACT_CONTADESCRIPT.handlers" />
						</base-input-structure>
					</q-col>
				</q-row>
				<q-row v-if="controls.CONTACT__CONTA__VISIT_DATE.isVisible">
					<q-col
						v-if="controls.CONTACT__CONTA__VISIT_DATE.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.CONTACT__CONTA__VISIT_DATE.isVisible"
							class="i-text"
							v-bind="controls.CONTACT__CONTA__VISIT_DATE.wrapperProps"
							:id="getControlId(controls.CONTACT__CONTA__VISIT_DATE)"
							v-on="controls.CONTACT__CONTA__VISIT_DATE.handlers"
							:loading="controls.CONTACT__CONTA__VISIT_DATE.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-date-time-picker
								v-if="controls.CONTACT__CONTA__VISIT_DATE.isVisible"
								v-bind="controls.CONTACT__CONTA__VISIT_DATE.props"
								:id="getControlId(controls.CONTACT__CONTA__VISIT_DATE)"
								:model-value="model.ValVisit_date.value"
								@reset-icon-click="model.ValVisit_date.fnUpdateValue(model.ValVisit_date.originalValue ?? new Date())"
								@update:model-value="model.ValVisit_date.fnUpdateValue($event ?? '')" />
						</base-input-structure>
					</q-col>
				</q-row>
			</template>
		</q-container>
	</teleport>

	<q-divider v-if="!isPopup && showFormFooter" />

	<teleport
		v-if="formModalIsReady && showFormFooter"
		:to="`#${uiContainersId.footer}`"
		:disabled="!isPopup || isNested">
		<q-row v-if="showFormFooter">
			<div id="footer-action-btns">
				<template
					v-for="btn in formButtons"
					:key="btn.id">
					<q-button
						v-if="btn.isActive && btn.isVisible && btn.showInFooter"
						:id="`bottom-${btn.id}`"
						:label="btn.text"
						:color="btn.color"
						:variant="btn.variant"
						:disabled="btn.disabled"
						:icon-pos="btn.iconPos"
						:class="btn.classes"
						@click="btn.action(); btn.emitAction ? $emit(btn.emitAction.name, btn.emitAction.params) : null">
						<q-icon
							v-if="btn.icon"
							v-bind="btn.icon" />
					</q-button>
				</template>
			</div>
		</q-row>
	</teleport>
</template>

<script>
	/* eslint-disable @typescript-eslint/no-unused-vars */
	import { computed, defineAsyncComponent, readonly } from 'vue'
	import { useRoute } from 'vue-router'

	import FormHandlers from '@/mixins/formHandlers.js'
	import formFunctions from '@/mixins/formFunctions.js'
	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'
	import listFunctions from '@/mixins/listFunctions.js'
	import listColumnTypes from '@/mixins/listColumnTypes.js'
	import modelFieldType from '@quidgest/clientapp/models/fields'
	import fieldControlClass from '@/mixins/fieldControl.js'
	import qEnums from '@quidgest/clientapp/constants/enums'
	import { resetProgressBar, setProgressBar } from '@/utils/layout.js'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import netAPI from '@quidgest/clientapp/network'
	import asyncProcM from '@quidgest/clientapp/composables/async'
	import qApi from '@/api/genio/quidgestFunctions.js'
	import qFunctions from '@/api/genio/projectFunctions.js'
	import qProjArrays from '@/api/genio/projectArrays.js'
	/* eslint-enable @typescript-eslint/no-unused-vars */

	import FormViewModel from './QFormContactViewModel.js'

	const requiredTextResources = ['QFormContact', 'hardcoded', 'messages']

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FORM_INCLUDEJS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QFormContact',

		components: {
			QSeeMoreContactPropetitle: defineAsyncComponent(() => import('@/views/forms/FormContact/dbedits/ContactPropetitleSeeMore.vue')),
			QSeeMoreContactPropeid: defineAsyncComponent(() => import('@/views/forms/FormContact/dbedits/ContactPropeidSeeMore.vue')),
		},

		mixins: [
			FormHandlers
		],

		props: {
			/**
			 * Parameters passed in case the form is nested.
			 */
			nestedRouteParams: {
				type: Object,
				default: () => ({
					name: 'CONTACT',
					location: 'form-CONTACT',
					params: {
						isNested: true
					}
				})
			}
		},

		expose: [
			'cancel',
			'initFormProperties',
			'navigationId'
		],

		setup(props)
		{
			const route = useRoute()

			return {
				/*
				 * As properties are reactive, when using $route.params, then when we exit it updates cached components.
				 * Properties have no value and this creates an error in new versions of vue-router.
				 * That's why the value has to be copied to a local property to be used in the router-link tag.
				 */
				currentRouteParams: props.isNested ? {} : route.params
			}
		},

		data()
		{
			// eslint-disable-next-line
			const vm = this
			return {
				componentOnLoadProc: asyncProcM.getProcListMonitor('QFormContact', false),

				interfaceMetadata: {
					id: 'QFormContact', // Used for resources
					requiredTextResources
				},

				formInfo: {
					type: 'popup',
					name: 'CONTACT',
					route: 'form-CONTACT',
					area: 'CONTA',
					primaryKey: 'ValCodconta',
					designation: computed(() => this.Resources.CONTACT59247),
					identifier: '', // Unique identifier received by route (when it's nested).
					mode: '',
					availableAgents: [],
				},

				formButtons: {
					changeToShow: {
						id: 'change-to-show-btn',
						icon: {
							icon: 'view',
							type: 'svg'
						},
						type: 'form-mode',
						text: computed(() => vm.Resources[hardcodedTexts.view]),
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isSelected: computed(() => vm.formModes.show === vm.formInfo.mode),
						isVisible: computed(() => vm.authData.isAllowed && [vm.formModes.show, vm.formModes.edit, vm.formModes.delete].includes(vm.formInfo.mode)),
						action: vm.changeToShowMode
					},
					changeToEdit: {
						id: 'change-to-edit-btn',
						icon: {
							icon: 'pencil',
							type: 'svg'
						},
						type: 'form-mode',
						text: computed(() => vm.Resources[hardcodedTexts.edit]),
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isSelected: computed(() => vm.formModes.edit === vm.formInfo.mode),
						isVisible: computed(() => vm.authData.isAllowed && [vm.formModes.show, vm.formModes.edit, vm.formModes.delete].includes(vm.formInfo.mode)),
						action: vm.changeToEditMode
					},
					changeToDuplicate: {
						id: 'change-to-dup-btn',
						icon: {
							icon: 'duplicate',
							type: 'svg'
						},
						type: 'form-mode',
						text: computed(() => vm.Resources[hardcodedTexts.duplicate]),
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isSelected: computed(() => vm.formModes.duplicate === vm.formInfo.mode),
						isVisible: computed(() => vm.authData.isAllowed && vm.formModes.new !== vm.formInfo.mode),
						action: vm.changeToDupMode
					},
					changeToDelete: {
						id: 'change-to-delete-btn',
						icon: {
							icon: 'delete',
							type: 'svg'
						},
						type: 'form-mode',
						text: computed(() => vm.Resources[hardcodedTexts.delete]),
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isSelected: computed(() => vm.formModes.delete === vm.formInfo.mode),
						isVisible: computed(() => vm.authData.isAllowed && [vm.formModes.show, vm.formModes.edit, vm.formModes.delete].includes(vm.formInfo.mode)),
						action: vm.changeToDeleteMode
					},
					changeToInsert: {
						id: 'change-to-insert-btn',
						icon: {
							icon: 'add',
							type: 'svg'
						},
						type: 'form-insert',
						text: computed(() => vm.Resources[hardcodedTexts.insert]),
						label: computed(() => vm.Resources[hardcodedTexts.insert]),
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isSelected: computed(() => vm.formModes.new === vm.formInfo.mode),
						isVisible: computed(() => vm.authData.isAllowed && vm.formModes.duplicate !== vm.formInfo.mode),
						action: vm.changeToInsertMode
					},
					repeatInsertBtn: {
						id: 'repeat-insert-btn',
						icon: {
							icon: 'save-new',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources[hardcodedTexts.repeatInsert]),
						variant: 'bold',
						showInHeader: true,
						showInFooter: true,
						isActive: false,
						isVisible: computed(() => vm.authData.isAllowed && vm.formInfo.mode === vm.formModes.new),
						action: () => vm.saveForm(true)
					},
					saveBtn: {
						id: 'save-btn',
						icon: {
							icon: 'save',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources.GRAVAR45301),
						variant: 'bold',
						showInHeader: true,
						showInFooter: true,
						isActive: true,
						isVisible: computed(() => vm.authData.isAllowed && vm.isEditable),
						action: vm.saveForm,
						badge: {
							isVisible: computed(() => vm.model?.isDirty === true),
							color: 'highlight'
						}
					},
					confirmBtn: {
						id: 'confirm-btn',
						icon: {
							icon: 'check',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources[vm.isNested ? hardcodedTexts.delete : hardcodedTexts.confirm]),
						variant: 'bold',
						showInHeader: true,
						showInFooter: true,
						isActive: true,
						isVisible: computed(() => vm.authData.isAllowed && (vm.formInfo.mode === vm.formModes.delete || vm.isNested)),
						action: vm.deleteRecord
					},
					cancelBtn: {
						id: 'cancel-btn',
						icon: {
							icon: 'cancel',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources.CANCELAR49513),
						showInHeader: true,
						showInFooter: true,
						isActive: true,
						isVisible: computed(() => vm.authData.isAllowed && vm.isEditable),
						action: vm.leaveForm
					},
					resetCancelBtn: {
						id: 'reset-cancel-btn',
						icon: {
							icon: 'cancel',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources[hardcodedTexts.cancel]),
						showInHeader: true,
						showInFooter: true,
						isActive: false,
						isVisible: computed(() => vm.authData.isAllowed && vm.isEditable),
						action: () => vm.model.resetValues(),
						emitAction: {
							name: 'deselect',
							params: {}
						}
					},
					editBtn: {
						id: 'edit-btn',
						icon: {
							icon: 'pencil',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources[hardcodedTexts.edit]),
						variant: 'bold',
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isVisible: computed(() => vm.authData.isAllowed && vm.parentFormMode !== vm.formModes.show && vm.parentFormMode !== vm.formModes.delete),
						action: () => {},
						emitAction: {
							name: 'edit',
							params: {}
						}
					},
					deleteQuickBtn: {
						id: 'delete-btn',
						icon: {
							icon: 'bin',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.Resources[hardcodedTexts.delete]),
						variant: 'bold',
						showInHeader: true,
						showInFooter: false,
						isActive: false,
						isVisible: computed(() => vm.authData.isAllowed && vm.parentFormMode !== vm.formModes.show && (typeof vm.permissions.canDelete === 'boolean' ? vm.permissions.canDelete : true)),
						action: vm.deleteRecord
					},
					backBtn: {
						id: 'back-btn',
						icon: {
							icon: 'back',
							type: 'svg'
						},
						type: 'form-action',
						text: computed(() => vm.isPopup ? vm.Resources[hardcodedTexts.close] : vm.Resources[hardcodedTexts.goBack]),
						showInHeader: true,
						showInFooter: true,
						isActive: true,
						isVisible: computed(() => !vm.authData.isAllowed || !vm.isEditable),
						action: vm.leaveForm
					}
				},

				controls: {
					CONTACT_CONTADATE____: new fieldControlClass.DateControl({
						modelField: 'ValDate',
						valueChangeEvent: 'fieldChange:conta.date',
						id: 'CONTACT_CONTADATE____',
						name: 'DATE',
						size: 'small',
						label: computed(() => this.Resources.DATE18475),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						dateTimeType: 'date',
						controlLimits: [
						],
					}, this),
					CONTACT_PROPETITLE___: new fieldControlClass.LookupControl({
						modelField: 'TablePropeTitle',
						valueChangeEvent: 'fieldChange:prope.title',
						id: 'CONTACT_PROPETITLE___',
						name: 'TITLE',
						size: 'xxlarge',
						label: computed(() => this.Resources.TITLE21885),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						externalCallbacks: {
							getModelField: vm.getModelField,
							getModelFieldValue: vm.getModelFieldValue,
							setModelFieldValue: vm.setModelFieldValue
						},
						externalProperties: {
							modelKeys: computed(() => vm.modelKeys)
						},
						lookupKeyModelField: {
							name: 'ValCodprope',
							dependencyEvent: 'fieldChange:conta.codprope'
						},
						dependentFields: () => ({
							set 'prope.codprope'(value) { vm.model.ValCodprope.updateValue(value) },
							set 'prope.title'(value) { vm.model.TablePropeTitle.updateValue(value) },
						}),
						controlLimits: [
						],
					}, this),
					CONTACT_CONTACLIENT__: new fieldControlClass.StringControl({
						modelField: 'ValClient',
						valueChangeEvent: 'fieldChange:conta.client',
						id: 'CONTACT_CONTACLIENT__',
						name: 'CLIENT',
						size: 'xxlarge',
						label: computed(() => this.Resources.CLIENT_NAME39245),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						maxLength: 50,
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					CONTACT_CONTAEMAIL___: new fieldControlClass.StringControl({
						modelField: 'ValEmail',
						valueChangeEvent: 'fieldChange:conta.email',
						id: 'CONTACT_CONTAEMAIL___',
						name: 'EMAIL',
						size: 'xxlarge',
						label: computed(() => this.Resources.EMAIL_DO_CLIENTE30111),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						maxLength: 80,
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					CONTACT_CONTAPHONE___: new fieldControlClass.MaskControl({
						modelField: 'ValPhone',
						valueChangeEvent: 'fieldChange:conta.phone',
						id: 'CONTACT_CONTAPHONE___',
						name: 'PHONE',
						size: 'medium',
						label: computed(() => this.Resources.PHONE_NUMBER20774),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						maxLength: 14,
						controlLimits: [
						],
					}, this),
					CONTACT_CONTADESCRIPT: new fieldControlClass.MultilineStringControl({
						modelField: 'ValDescript',
						valueChangeEvent: 'fieldChange:conta.descript',
						id: 'CONTACT_CONTADESCRIPT',
						name: 'DESCRIPT',
						size: 'xxlarge',
						label: computed(() => this.Resources.DESCRIPTION07383),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						rows: 5,
						cols: 80,
						controlLimits: [
						],
						requiredConditions: {
							// eslint-disable-next-line @typescript-eslint/no-unused-vars
							fnFormula(params)
							{
								// Formula: isEmptyC([CONTA->CLIENT])
								if ((this.ValClient.value === ''))
									return true
								return false
							},
							dependencyEvents: ['fieldChange:conta.client'],
							isServerRecalc: false,
						},
					}, this),
					CONTACT__CONTA__VISIT_DATE: new fieldControlClass.DateControl({
						modelField: 'ValVisit_date',
						valueChangeEvent: 'fieldChange:conta.visit_date',
						id: 'CONTACT__CONTA__VISIT_DATE',
						name: 'VISIT_DATE',
						size: 'small',
						label: computed(() => this.Resources.VISIT_DATE27188),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						dateTimeType: 'date',
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					CONTACT_PROPEID______: new fieldControlClass.LookupControl({
						modelField: 'TablePropeId',
						valueChangeEvent: 'fieldChange:prope.id',
						id: 'CONTACT_PROPEID______',
						name: 'ID',
						size: 'small',
						label: computed(() => this.Resources.PROPERTY43977),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						externalCallbacks: {
							getModelField: vm.getModelField,
							getModelFieldValue: vm.getModelFieldValue,
							setModelFieldValue: vm.setModelFieldValue
						},
						externalProperties: {
							modelKeys: computed(() => vm.modelKeys)
						},
						lookupKeyModelField: {
							name: 'ValCodprope',
							dependencyEvent: 'fieldChange:conta.codprope'
						},
						dependentFields: () => ({
							set 'prope.codprope'(value) { vm.model.ValCodprope.updateValue(value) },
							set 'prope.id'(value) { vm.model.TablePropeId.updateValue(value) },
						}),
						controlLimits: [
							{
								identifier: 'conta.visit_date',
								dependencyEvents: ['fieldChange:conta.visit_date'],
								dependencyField: 'CONTA.VISIT_DATE',
								fnValueSelector: (model) => model.ValVisit_date.value
							},
						],
					}, this),
				},

				model: new FormViewModel(this, {
					callbacks: {
						onUpdate: this.onUpdate,
						setFormKey: this.setFormKey
					}
				}),

				groupFields: readonly([
				]),

				tableFields: readonly([
				]),

				timelineFields: readonly([
				]),

				/**
				 * The Data API for easy access to model variables.
				 */
				dataApi: {
					Conta: {
						get ValClient() { return vm.model.ValClient.value },
						set ValClient(value) { vm.model.ValClient.updateValue(value) },
						get ValCodprope() { return vm.model.ValCodprope.value },
						set ValCodprope(value) { vm.model.ValCodprope.updateValue(value) },
						get ValDate() { return vm.model.ValDate.value },
						set ValDate(value) { vm.model.ValDate.updateValue(value) },
						get ValDescript() { return vm.model.ValDescript.value },
						set ValDescript(value) { vm.model.ValDescript.updateValue(value) },
						get ValEmail() { return vm.model.ValEmail.value },
						set ValEmail(value) { vm.model.ValEmail.updateValue(value) },
						get ValPhone() { return vm.model.ValPhone.value },
						set ValPhone(value) { vm.model.ValPhone.updateValue(value) },
						get ValVisit_date() { return vm.model.ValVisit_date.value },
						set ValVisit_date(value) { vm.model.ValVisit_date.updateValue(value) },
					},
					Prope: {
						get ValId() { return vm.model.TablePropeId.value },
						set ValId(value) { vm.model.TablePropeId.updateValue(value) },
						get ValTitle() { return vm.model.TablePropeTitle.value },
						set ValTitle(value) { vm.model.TablePropeTitle.updateValue(value) },
					},
					keys: {
						/** The primary key of the CONTA table */
						get conta() { return vm.model.ValCodconta },
						/** The foreign key to the PROPE table */
						get prope() { return vm.model.ValCodprope },
					},
					get extraProperties() { return vm.model.extraProperties },
				},
			}
		},

		beforeRouteEnter(to, _, next)
		{
			// Called before the route that renders this component is confirmed.
			// Does NOT have access to `this` component instance, because
			// it has not been created yet when this guard is called!

			next((vm) => {
				vm.initFormProperties(to)
			})
		},

		beforeRouteLeave(to, _, next)
		{
			if (to.params.isControlled === 'true')
			{
				genericFunctions.setNavigationState(false)
				next()
			}
			else
				this.cancel(next)
		},

		beforeRouteUpdate(to, _, next)
		{
			if (to.params.isControlled === 'true')
				next()
			else
				this.cancel(next)
		},

		mounted()
		{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FORM_CODEJS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		beforeUnmount()
		{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR COMPONENT_BEFORE_UNMOUNT CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		methods: {
			/**
			 * Called before form init.
			 */
			async beforeLoad()
			{
				// Execute the "Before init" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.beforeInit)
				for (const trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('before-load-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR BEFORE_LOAD_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return true
			},

			/**
			 * Called after form init.
			 */
			async afterLoad()
			{
				// Execute the "After init" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.afterInit)
				for (const trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('after-load-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FORM_LOADED_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
			},

			/**
			 * Called before an apply action is performed.
			 */
			async beforeApply()
			{
				let applyForm = true // Set to 'false' to cancel form apply.

				// Execute the "Before apply" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.beforeApply)
				for (const trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				const ticketsPromise = this.model.updateFilesTickets(true)
				this.addBusy(ticketsPromise, this.Resources[hardcodedTexts.processing])
				const canSetDocums = await ticketsPromise

				if (canSetDocums)
				{
					let results
					const changesPromise = this.model.setDocumentChanges()
					this.addBusy(changesPromise, this.Resources[hardcodedTexts.processing])
					applyForm = await changesPromise

					if (applyForm)
					{
						const insertsPromise = this.model.saveDocuments()
						this.addBusy(insertsPromise, this.Resources[hardcodedTexts.processing])
						results = await insertsPromise
						applyForm = results.every((e) => e === true)
					}

					if (!changesPromise || (results && !results.every((e) => e === true)))
					{
						this.validationErrors = {
							Erro: this.Resources.OCORREU_UM_ERRO_AO_T51884
						}
					}
				}

				this.emitEvent('before-apply-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR BEFORE_APPLY_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return applyForm
			},

			/**
			 * Called after an apply action is performed.
			 */
			async afterApply()
			{
				// Execute the "After apply" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.afterApply)
				for (const trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('after-apply-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR AFTER_APPLY_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
			},

			/**
			 * Called before the record is saved.
			 */
			async beforeSave()
			{
				let saveForm = true // Set to 'false' to cancel form saving.

				// Execute the "Before save" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.beforeSave)
				for (const trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				const ticketsPromise = this.model.updateFilesTickets()
				this.addBusy(ticketsPromise, this.Resources[hardcodedTexts.processing])
				const canSetDocums = await ticketsPromise

				if (canSetDocums)
				{
					let results
					const changesPromise = this.model.setDocumentChanges()
					this.addBusy(changesPromise, this.Resources[hardcodedTexts.processing])
					saveForm = await changesPromise

					if (saveForm)
					{
						const insertsPromise = this.model.saveDocuments()
						this.addBusy(insertsPromise, this.Resources[hardcodedTexts.processing])
						results = await insertsPromise
						saveForm = results.every((e) => e === true)
					}

					if (!changesPromise || (results && !results.every((e) => e === true)))
					{
						this.validationErrors = {
							Erro: this.Resources.OCORREU_UM_ERRO_AO_T51884
						}
					}
				}

				this.emitEvent('before-save-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR BEFORE_SAVE_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return saveForm
			},

			/**
			 * Called after the record is saved.
			 */
			async afterSave()
			{
				// Execute the "After save" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.afterSave)
				for (const trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('after-save-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR AFTER_SAVE_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return true
			},

			/**
			 * Called before the record is deleted.
			 */
			async beforeDel()
			{
				this.emitEvent('before-delete-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR BEFORE_DEL_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return true
			},

			/**
			 * Called after the record is deleted.
			 */
			async afterDel()
			{
				this.emitEvent('after-delete-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR AFTER_DEL_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return true
			},

			/**
			 * Called before leaving the form.
			 */
			async beforeExit()
			{
				// Execute the "Before exit" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.beforeExit)
				for (const trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('before-exit-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR BEFORE_EXIT_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				return true
			},

			/**
			 * Called after leaving the form.
			 */
			async afterExit()
			{
				// Execute the "After exit" triggers.
				const triggers = this.getTriggers(qEnums.triggerEvents.afterExit)
				for (const trigger of triggers)
					await formFunctions.executeTriggerAction(trigger)

				this.emitEvent('after-exit-form')

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR AFTER_EXIT_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
			},

			/**
			 * Called whenever a field's value is updated.
			 * @param {string} fieldName The name of the field in the format [table].[field] (ex: 'person.name')
			 * @param {object} fieldObject The object representing the field in the model
			 * @param {any} fieldValue The value of the field
			 * @param {any} oldFieldValue The previous value of the field
			 */
			// eslint-disable-next-line
			onUpdate(fieldName, fieldObject, fieldValue, oldFieldValue)
			{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR DLGUPDT CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.afterFieldUpdate(fieldName, fieldObject)
			},

			/**
			 * Called whenever a field is unfocused.
			 * @param {*} fieldObject The object representing the field in the model
			 * @param {*} fieldValue The value of the field
			 */
			// eslint-disable-next-line
			onBlur(fieldObject, fieldValue)
			{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR CTRLBLR CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.afterFieldUnfocus(fieldObject, fieldValue)
			},

			/**
			 * Called whenever a control's value is updated.
			 * @param {string} controlField The name of the field in the controls that will be updated
			 * @param {object} control The object representing the field in the controls
			 * @param {any} fieldValue The value of the field
			 */
			// eslint-disable-next-line
			onControlUpdate(controlField, control, fieldValue)
			{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR CTRLUPD CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.afterControlUpdate(controlField, fieldValue)
			},
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FUNCTIONS_JS CONTACT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		watch: {
		}
	}
</script>
