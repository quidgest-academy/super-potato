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
			data-key="AGENT"
			:data-identifier="primaryKeyValue"
			:data-loading="!formInitialDataLoaded || !isActiveForm">
			<template v-if="formControl.initialized && showFormBody">
				<q-row v-if="controls.AGENT___PSEUDNEWGRP01.isVisible">
					<q-col v-if="controls.AGENT___PSEUDNEWGRP01.isVisible">
						<q-group-box-container
							v-if="controls.AGENT___PSEUDNEWGRP01.isVisible"
							v-bind="controls.AGENT___PSEUDNEWGRP01"
							:id="getControlId(controls.AGENT___PSEUDNEWGRP01)"
							:no-border="controls.AGENT___PSEUDNEWGRP01.borderless">
							<!-- Start AGENT___PSEUDNEWGRP01 -->
							<q-row v-if="controls.AGENT___AGENTBIRTHDAT.isVisible || controls.AGENT___AGENTAGE_____.isVisible || controls.AGENT___AGENTEMAIL___.isVisible || controls.AGENT___AGENTTELEPHON.isVisible">
								<q-col
									v-if="controls.AGENT___AGENTBIRTHDAT.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___AGENTBIRTHDAT.isVisible"
										class="i-text"
										v-bind="controls.AGENT___AGENTBIRTHDAT.wrapperProps"
										:id="getControlId(controls.AGENT___AGENTBIRTHDAT)"
										v-on="controls.AGENT___AGENTBIRTHDAT.handlers"
										:loading="controls.AGENT___AGENTBIRTHDAT.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-date-time-picker
											v-if="controls.AGENT___AGENTBIRTHDAT.isVisible"
											v-bind="controls.AGENT___AGENTBIRTHDAT.props"
											:id="getControlId(controls.AGENT___AGENTBIRTHDAT)"
											:model-value="model.ValBirthdat.value"
											@reset-icon-click="model.ValBirthdat.fnUpdateValue(model.ValBirthdat.originalValue ?? new Date())"
											@update:model-value="model.ValBirthdat.fnUpdateValue($event ?? '')" />
									</base-input-structure>
								</q-col>
								<q-col
									v-if="controls.AGENT___AGENTAGE_____.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___AGENTAGE_____.isVisible"
										class="i-text"
										v-bind="controls.AGENT___AGENTAGE_____.wrapperProps"
										:id="getControlId(controls.AGENT___AGENTAGE_____)"
										v-on="controls.AGENT___AGENTAGE_____.handlers"
										:loading="controls.AGENT___AGENTAGE_____.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-numeric-input
											v-if="controls.AGENT___AGENTAGE_____.isVisible"
											v-bind="controls.AGENT___AGENTAGE_____.props"
											:id="getControlId(controls.AGENT___AGENTAGE_____)"
											@update:model-value="model.ValAge.fnUpdateValue" />
									</base-input-structure>
								</q-col>
								<q-col
									v-if="controls.AGENT___AGENTEMAIL___.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___AGENTEMAIL___.isVisible"
										class="i-text"
										v-bind="controls.AGENT___AGENTEMAIL___.wrapperProps"
										:id="getControlId(controls.AGENT___AGENTEMAIL___)"
										v-on="controls.AGENT___AGENTEMAIL___.handlers"
										:loading="controls.AGENT___AGENTEMAIL___.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-text-field
											v-bind="controls.AGENT___AGENTEMAIL___.props"
											:id="getControlId(controls.AGENT___AGENTEMAIL___)"
											@blur="onBlur(controls.AGENT___AGENTEMAIL___, model.ValEmail.value)"
											@change="model.ValEmail.fnUpdateValueOnChange" />
									</base-input-structure>
								</q-col>
								<q-col
									v-if="controls.AGENT___AGENTTELEPHON.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___AGENTTELEPHON.isVisible"
										class="i-text"
										v-bind="controls.AGENT___AGENTTELEPHON.wrapperProps"
										:id="getControlId(controls.AGENT___AGENTTELEPHON)"
										v-on="controls.AGENT___AGENTTELEPHON.handlers"
										:loading="controls.AGENT___AGENTTELEPHON.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-mask
											v-if="controls.AGENT___AGENTTELEPHON.isVisible"
											v-bind="controls.AGENT___AGENTTELEPHON.props"
											:id="getControlId(controls.AGENT___AGENTTELEPHON)"
											:model-value="model.ValTelephon.value"
											@change="model.ValTelephon.fnUpdateValueOnChange" />
									</base-input-structure>
								</q-col>
							</q-row>
							<q-row v-if="controls.AGENT___AGENTACTIVE__.isVisible || controls.AGENT___CBORNCOUNTRY_.isVisible || controls.AGENT___CADDRCOUNTRY_.isVisible">
								<q-col
									v-if="controls.AGENT___AGENTACTIVE__.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___AGENTACTIVE__.isVisible"
										class="i-text"
										v-bind="controls.AGENT___AGENTACTIVE__.wrapperProps"
										:id="getControlId(controls.AGENT___AGENTACTIVE__)"
										v-on="controls.AGENT___AGENTACTIVE__.handlers"
										:loading="controls.AGENT___AGENTACTIVE__.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<template #label>
											<q-checkbox
												v-if="controls.AGENT___AGENTACTIVE__.isVisible"
												v-bind="controls.AGENT___AGENTACTIVE__.props"
												:id="getControlId(controls.AGENT___AGENTACTIVE__)"
												v-on="controls.AGENT___AGENTACTIVE__.handlers" />
										</template>
									</base-input-structure>
								</q-col>
								<q-col
									v-if="controls.AGENT___CBORNCOUNTRY_.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___CBORNCOUNTRY_.isVisible"
										class="i-text"
										v-bind="controls.AGENT___CBORNCOUNTRY_.wrapperProps"
										:id="getControlId(controls.AGENT___CBORNCOUNTRY_)"
										v-on="controls.AGENT___CBORNCOUNTRY_.handlers"
										:loading="controls.AGENT___CBORNCOUNTRY_.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-lookup
											v-if="controls.AGENT___CBORNCOUNTRY_.isVisible"
											v-bind="controls.AGENT___CBORNCOUNTRY_.props"
											:id="getControlId(controls.AGENT___CBORNCOUNTRY_)"
											v-on="controls.AGENT___CBORNCOUNTRY_.handlers" />
										<q-see-more-agent-cborncountry
											v-if="controls.AGENT___CBORNCOUNTRY_.seeMoreIsVisible"
											v-bind="controls.AGENT___CBORNCOUNTRY_.seeMoreParams"
											v-on="controls.AGENT___CBORNCOUNTRY_.handlers" />
									</base-input-structure>
								</q-col>
								<q-col
									v-if="controls.AGENT___CADDRCOUNTRY_.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___CADDRCOUNTRY_.isVisible"
										class="i-text"
										v-bind="controls.AGENT___CADDRCOUNTRY_.wrapperProps"
										:id="getControlId(controls.AGENT___CADDRCOUNTRY_)"
										v-on="controls.AGENT___CADDRCOUNTRY_.handlers"
										:loading="controls.AGENT___CADDRCOUNTRY_.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-lookup
											v-if="controls.AGENT___CADDRCOUNTRY_.isVisible"
											v-bind="controls.AGENT___CADDRCOUNTRY_.props"
											:id="getControlId(controls.AGENT___CADDRCOUNTRY_)"
											v-on="controls.AGENT___CADDRCOUNTRY_.handlers" />
										<q-see-more-agent-caddrcountry
											v-if="controls.AGENT___CADDRCOUNTRY_.seeMoreIsVisible"
											v-bind="controls.AGENT___CADDRCOUNTRY_.seeMoreParams"
											v-on="controls.AGENT___CADDRCOUNTRY_.handlers" />
									</base-input-structure>
								</q-col>
							</q-row>
							<q-row v-if="controls.AGENT___AGENTNRPROPS_.isVisible || controls.AGENT___AGENTPROFIT__.isVisible || controls.AGENT__AGENT__AVERAGE_PRICE.isVisible">
								<q-col
									v-if="controls.AGENT___AGENTNRPROPS_.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___AGENTNRPROPS_.isVisible"
										class="i-text"
										v-bind="controls.AGENT___AGENTNRPROPS_.wrapperProps"
										:id="getControlId(controls.AGENT___AGENTNRPROPS_)"
										v-on="controls.AGENT___AGENTNRPROPS_.handlers"
										:loading="controls.AGENT___AGENTNRPROPS_.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-numeric-input
											v-if="controls.AGENT___AGENTNRPROPS_.isVisible"
											v-bind="controls.AGENT___AGENTNRPROPS_.props"
											:id="getControlId(controls.AGENT___AGENTNRPROPS_)"
											@update:model-value="model.ValNrprops.fnUpdateValue" />
									</base-input-structure>
								</q-col>
								<q-col
									v-if="controls.AGENT___AGENTPROFIT__.isVisible || controls.AGENT__AGENT__AVERAGE_PRICE.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___AGENTPROFIT__.isVisible"
										class="i-text"
										v-bind="controls.AGENT___AGENTPROFIT__.wrapperProps"
										:id="getControlId(controls.AGENT___AGENTPROFIT__)"
										v-on="controls.AGENT___AGENTPROFIT__.handlers"
										:loading="controls.AGENT___AGENTPROFIT__.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-numeric-input
											v-if="controls.AGENT___AGENTPROFIT__.isVisible"
											v-bind="controls.AGENT___AGENTPROFIT__.props"
											:id="getControlId(controls.AGENT___AGENTPROFIT__)"
											@update:model-value="model.ValProfit.fnUpdateValue" />
									</base-input-structure>
									<base-input-structure
										v-if="controls.AGENT__AGENT__AVERAGE_PRICE.isVisible"
										class="i-text"
										v-bind="controls.AGENT__AGENT__AVERAGE_PRICE.wrapperProps"
										:id="getControlId(controls.AGENT__AGENT__AVERAGE_PRICE)"
										v-on="controls.AGENT__AGENT__AVERAGE_PRICE.handlers"
										:loading="controls.AGENT__AGENT__AVERAGE_PRICE.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-numeric-input
											v-if="controls.AGENT__AGENT__AVERAGE_PRICE.isVisible"
											v-bind="controls.AGENT__AGENT__AVERAGE_PRICE.props"
											:id="getControlId(controls.AGENT__AGENT__AVERAGE_PRICE)"
											@update:model-value="model.ValAverage_price.fnUpdateValue" />
									</base-input-structure>
								</q-col>
							</q-row>
							<q-row v-if="controls.AGENT___AGENTLASTPROP.isVisible || controls.AGENT___AGENTPHOTOGRA.isVisible">
								<q-col
									v-if="controls.AGENT___AGENTLASTPROP.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___AGENTLASTPROP.isVisible"
										class="i-text"
										v-bind="controls.AGENT___AGENTLASTPROP.wrapperProps"
										:id="getControlId(controls.AGENT___AGENTLASTPROP)"
										v-on="controls.AGENT___AGENTLASTPROP.handlers"
										:loading="controls.AGENT___AGENTLASTPROP.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-numeric-input
											v-if="controls.AGENT___AGENTLASTPROP.isVisible"
											v-bind="controls.AGENT___AGENTLASTPROP.props"
											:id="getControlId(controls.AGENT___AGENTLASTPROP)"
											@update:model-value="model.ValLastprop.fnUpdateValue" />
									</base-input-structure>
								</q-col>
								<q-col
									v-if="controls.AGENT___AGENTPHOTOGRA.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___AGENTPHOTOGRA.isVisible"
										class="q-image"
										v-bind="controls.AGENT___AGENTPHOTOGRA.wrapperProps"
										:id="getControlId(controls.AGENT___AGENTPHOTOGRA)"
										v-on="controls.AGENT___AGENTPHOTOGRA.handlers"
										:loading="controls.AGENT___AGENTPHOTOGRA.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-image
											v-if="controls.AGENT___AGENTPHOTOGRA.isVisible"
											v-bind="controls.AGENT___AGENTPHOTOGRA.props"
											:id="getControlId(controls.AGENT___AGENTPHOTOGRA)"
											v-on="controls.AGENT___AGENTPHOTOGRA.handlers" />
									</base-input-structure>
								</q-col>
							</q-row>
							<q-row v-if="controls.AGENT___AGENTNAME____.isVisible">
								<q-col
									v-if="controls.AGENT___AGENTNAME____.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.AGENT___AGENTNAME____.isVisible"
										class="i-text"
										v-bind="controls.AGENT___AGENTNAME____.wrapperProps"
										:id="getControlId(controls.AGENT___AGENTNAME____)"
										v-on="controls.AGENT___AGENTNAME____.handlers"
										:loading="controls.AGENT___AGENTNAME____.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-text-field
											v-bind="controls.AGENT___AGENTNAME____.props"
											:id="getControlId(controls.AGENT___AGENTNAME____)"
											@blur="onBlur(controls.AGENT___AGENTNAME____, model.ValName.value)"
											@change="model.ValName.fnUpdateValueOnChange" />
									</base-input-structure>
								</q-col>
							</q-row>
							<!-- End AGENT___PSEUDNEWGRP01 -->
						</q-group-box-container>
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

	import FormViewModel from './QFormAgentViewModel.js'

	const requiredTextResources = ['QFormAgent', 'hardcoded', 'messages']

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FORM_INCLUDEJS AGENT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QFormAgent',

		components: {
			QSeeMoreAgentCborncountry: defineAsyncComponent(() => import('@/views/forms/FormAgent/dbedits/AgentCborncountrySeeMore.vue')),
			QSeeMoreAgentCaddrcountry: defineAsyncComponent(() => import('@/views/forms/FormAgent/dbedits/AgentCaddrcountrySeeMore.vue')),
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
					name: 'AGENT',
					location: 'form-AGENT',
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
				componentOnLoadProc: asyncProcM.getProcListMonitor('QFormAgent', false),

				interfaceMetadata: {
					id: 'QFormAgent', // Used for resources
					requiredTextResources
				},

				formInfo: {
					type: 'normal',
					name: 'AGENT',
					route: 'form-AGENT',
					area: 'AGENT',
					primaryKey: 'ValCodagent',
					designation: computed(() => this.Resources.AGENT00994),
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
					AGENT___PSEUDNEWGRP01: new fieldControlClass.GroupControl({
						id: 'AGENT___PSEUDNEWGRP01',
						name: 'NEWGRP01',
						size: 'block',
						label: computed(() => this.Resources.AGENT_INFORMATION06141),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						borderless: false,
						isCollapsible: false,
						anchored: false,
						directChildren: ['AGENT___AGENTBIRTHDAT', 'AGENT___AGENTAGE_____', 'AGENT___AGENTEMAIL___', 'AGENT___AGENTTELEPHON', 'AGENT___AGENTACTIVE__', 'AGENT___CBORNCOUNTRY_', 'AGENT___CADDRCOUNTRY_', 'AGENT___AGENTNRPROPS_', 'AGENT___AGENTPROFIT__', 'AGENT__AGENT__AVERAGE_PRICE', 'AGENT___AGENTLASTPROP', 'AGENT___AGENTPHOTOGRA', 'AGENT___AGENTNAME____'],
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					AGENT___AGENTBIRTHDAT: new fieldControlClass.DateControl({
						modelField: 'ValBirthdat',
						valueChangeEvent: 'fieldChange:agent.birthdat',
						id: 'AGENT___AGENTBIRTHDAT',
						name: 'BIRTHDAT',
						size: 'small',
						label: computed(() => this.Resources.BIRTHDATE22743),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						dateTimeType: 'date',
						controlLimits: [
						],
					}, this),
					AGENT___AGENTAGE_____: new fieldControlClass.NumberControl({
						modelField: 'ValAge',
						valueChangeEvent: 'fieldChange:agent.age',
						id: 'AGENT___AGENTAGE_____',
						name: 'AGE',
						size: 'mini',
						label: computed(() => this.Resources.AGE28663),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						isFormulaBlocked: true,
						maxIntegers: 3,
						maxDecimals: 0,
						controlLimits: [
						],
					}, this),
					AGENT___AGENTEMAIL___: new fieldControlClass.StringControl({
						modelField: 'ValEmail',
						valueChangeEvent: 'fieldChange:agent.email',
						id: 'AGENT___AGENTEMAIL___',
						name: 'EMAIL',
						size: 'xxlarge',
						label: computed(() => this.Resources.E_MAIL42251),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						maxLength: 80,
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					AGENT___AGENTTELEPHON: new fieldControlClass.MaskControl({
						modelField: 'ValTelephon',
						valueChangeEvent: 'fieldChange:agent.telephon',
						id: 'AGENT___AGENTTELEPHON',
						name: 'TELEPHON',
						size: 'small',
						label: computed(() => this.Resources.TELEPHONE28697),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						maxLength: 14,
						controlLimits: [
						],
					}, this),
					AGENT___AGENTACTIVE__: new fieldControlClass.BooleanControl({
						modelField: 'ValActive',
						valueChangeEvent: 'fieldChange:agent.active',
						id: 'AGENT___AGENTACTIVE__',
						name: 'ACTIVE',
						size: 'mini',
						label: computed(() => this.Resources.ACTIVE03270),
						placeholder: '',
						labelPosition: computed(() => this.$app.layout.CheckboxLabelAlignment),
						container: 'AGENT___PSEUDNEWGRP01',
						controlLimits: [
						],
					}, this),
					AGENT___CBORNCOUNTRY_: new fieldControlClass.LookupControl({
						modelField: 'TableCbornCountry',
						valueChangeEvent: 'fieldChange:cborn.country',
						id: 'AGENT___CBORNCOUNTRY_',
						name: 'COUNTRY',
						size: 'medium',
						label: computed(() => this.Resources.COUNTRY_OF_BIRTH00722),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						externalCallbacks: {
							getModelField: vm.getModelField,
							getModelFieldValue: vm.getModelFieldValue,
							setModelFieldValue: vm.setModelFieldValue
						},
						externalProperties: {
							modelKeys: computed(() => vm.modelKeys)
						},
						lookupKeyModelField: {
							name: 'ValCborn',
							dependencyEvent: 'fieldChange:agent.cborn'
						},
						dependentFields: () => ({
							set 'cborn.codcount'(value) { vm.model.ValCborn.updateValue(value) },
							set 'cborn.country'(value) { vm.model.TableCbornCountry.updateValue(value) },
						}),
						controlLimits: [
						],
					}, this),
					AGENT___CADDRCOUNTRY_: new fieldControlClass.LookupControl({
						modelField: 'TableCaddrCountry',
						valueChangeEvent: 'fieldChange:caddr.country',
						id: 'AGENT___CADDRCOUNTRY_',
						name: 'COUNTRY',
						size: 'medium',
						label: computed(() => this.Resources.COUNTRY_OF_RESIDENCE06288),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						externalCallbacks: {
							getModelField: vm.getModelField,
							getModelFieldValue: vm.getModelFieldValue,
							setModelFieldValue: vm.setModelFieldValue
						},
						externalProperties: {
							modelKeys: computed(() => vm.modelKeys)
						},
						lookupKeyModelField: {
							name: 'ValCodcaddr',
							dependencyEvent: 'fieldChange:agent.codcaddr'
						},
						dependentFields: () => ({
							set 'caddr.codcount'(value) { vm.model.ValCodcaddr.updateValue(value) },
							set 'caddr.country'(value) { vm.model.TableCaddrCountry.updateValue(value) },
						}),
						controlLimits: [
						],
					}, this),
					AGENT___AGENTNRPROPS_: new fieldControlClass.NumberControl({
						modelField: 'ValNrprops',
						valueChangeEvent: 'fieldChange:agent.nrprops',
						id: 'AGENT___AGENTNRPROPS_',
						name: 'NRPROPS',
						size: 'large',
						label: computed(() => this.Resources.NUMBER_OF_PROPERTIES01169),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						isFormulaBlocked: true,
						maxIntegers: 5,
						maxDecimals: 0,
						controlLimits: [
						],
					}, this),
					AGENT___AGENTPROFIT__: new fieldControlClass.CurrencyControl({
						modelField: 'ValProfit',
						valueChangeEvent: 'fieldChange:agent.profit',
						id: 'AGENT___AGENTPROFIT__',
						name: 'PROFIT',
						size: 'medium',
						label: computed(() => this.Resources.PROFIT55910),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						isFormulaBlocked: true,
						maxIntegers: 11,
						maxDecimals: 2,
						controlLimits: [
						],
					}, this),
					AGENT__AGENT__AVERAGE_PRICE: new fieldControlClass.CurrencyControl({
						modelField: 'ValAverage_price',
						valueChangeEvent: 'fieldChange:agent.average_price',
						id: 'AGENT__AGENT__AVERAGE_PRICE',
						name: 'AVERAGE_PRICE',
						size: 'medium',
						label: computed(() => this.Resources.AVERAGEPRICE13700),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						isFormulaBlocked: true,
						maxIntegers: 9,
						maxDecimals: 2,
						controlLimits: [
						],
					}, this),
					AGENT___AGENTLASTPROP: new fieldControlClass.CurrencyControl({
						modelField: 'ValLastprop',
						valueChangeEvent: 'fieldChange:agent.lastprop',
						id: 'AGENT___AGENTLASTPROP',
						name: 'LASTPROP',
						size: 'large',
						label: computed(() => this.Resources.LAST_PROPERTY_SOLD__49162),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						isFormulaBlocked: true,
						maxIntegers: 9,
						maxDecimals: 2,
						controlLimits: [
						],
					}, this),
					AGENT___AGENTPHOTOGRA: new fieldControlClass.ImageControl({
						modelField: 'ValPhotography',
						valueChangeEvent: 'fieldChange:agent.photography',
						id: 'AGENT___AGENTPHOTOGRA',
						name: 'PHOTOGRA',
						size: 'mini',
						label: computed(() => this.Resources.PHOTOGRAPHY38058),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						height: 50,
						width: 30,
						dataTitle: computed(() => genericFunctions.formatString(vm.Resources.IMAGEM_UTILIZADA_PAR17299, vm.Resources.PHOTOGRAPHY38058)),
						maxFileSize: 10485760, // In bytes.
						maxFileSizeLabel: '10 MB',
						controlLimits: [
						],
					}, this),
					AGENT___AGENTNAME____: new fieldControlClass.StringControl({
						modelField: 'ValName',
						valueChangeEvent: 'fieldChange:agent.name',
						id: 'AGENT___AGENTNAME____',
						name: 'NAME',
						size: 'xlarge',
						helpControl: {
							shortHelp: {
								type: 'Tooltip',
								text: computed(() => this.Resources.____114843),
							},
							detailedHelp: {
								type: 'Popover',
								text: computed(() => this.Resources.____1_VERBOSE59661),
							}
						},
						label: computed(() => this.Resources.AGENT_S_NAME42642),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'AGENT___PSEUDNEWGRP01',
						maxLength: 50,
						mustBeFilled: true,
						controlLimits: [
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
					'AGENT___PSEUDNEWGRP01',
				]),

				tableFields: readonly([
				]),

				timelineFields: readonly([
				]),

				/**
				 * The Data API for easy access to model variables.
				 */
				dataApi: {
					Agent: {
						get ValActive() { return vm.model.ValActive.value },
						set ValActive(value) { vm.model.ValActive.updateValue(value) },
						get ValAge() { return vm.model.ValAge.value },
						set ValAge(value) { vm.model.ValAge.updateValue(value) },
						get ValAverage_price() { return vm.model.ValAverage_price.value },
						set ValAverage_price(value) { vm.model.ValAverage_price.updateValue(value) },
						get ValBirthdat() { return vm.model.ValBirthdat.value },
						set ValBirthdat(value) { vm.model.ValBirthdat.updateValue(value) },
						get ValCborn() { return vm.model.ValCborn.value },
						set ValCborn(value) { vm.model.ValCborn.updateValue(value) },
						get ValCodagent() { return vm.model.ValCodagent.value },
						set ValCodagent(value) { vm.model.ValCodagent.updateValue(value) },
						get ValCodcaddr() { return vm.model.ValCodcaddr.value },
						set ValCodcaddr(value) { vm.model.ValCodcaddr.updateValue(value) },
						get ValEmail() { return vm.model.ValEmail.value },
						set ValEmail(value) { vm.model.ValEmail.updateValue(value) },
						get ValLastprop() { return vm.model.ValLastprop.value },
						set ValLastprop(value) { vm.model.ValLastprop.updateValue(value) },
						get ValName() { return vm.model.ValName.value },
						set ValName(value) { vm.model.ValName.updateValue(value) },
						get ValNrprops() { return vm.model.ValNrprops.value },
						set ValNrprops(value) { vm.model.ValNrprops.updateValue(value) },
						get ValPhotography() { return vm.model.ValPhotography.value },
						set ValPhotography(value) { vm.model.ValPhotography.updateValue(value) },
						get ValProfit() { return vm.model.ValProfit.value },
						set ValProfit(value) { vm.model.ValProfit.updateValue(value) },
						get ValTelephon() { return vm.model.ValTelephon.value },
						set ValTelephon(value) { vm.model.ValTelephon.updateValue(value) },
					},
					Caddr: {
						get ValCountry() { return vm.model.TableCaddrCountry.value },
						set ValCountry(value) { vm.model.TableCaddrCountry.updateValue(value) },
					},
					Cborn: {
						get ValCountry() { return vm.model.TableCbornCountry.value },
						set ValCountry(value) { vm.model.TableCbornCountry.updateValue(value) },
					},
					keys: {
						/** The primary key of the AGENT table */
						get agent() { return vm.model.ValCodagent },
						/** The foreign key to the CBORN table */
						get cborn() { return vm.model.ValCborn },
						/** The foreign key to the CADDR table */
						get caddr() { return vm.model.ValCodcaddr },
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
// USE /[MANUAL FOR FORM_CODEJS AGENT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		beforeUnmount()
		{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR COMPONENT_BEFORE_UNMOUNT AGENT]/
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
// USE /[MANUAL FOR BEFORE_LOAD_JS AGENT]/
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
// USE /[MANUAL FOR FORM_LOADED_JS AGENT]/
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
// USE /[MANUAL FOR BEFORE_APPLY_JS AGENT]/
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
// USE /[MANUAL FOR AFTER_APPLY_JS AGENT]/
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
// USE /[MANUAL FOR BEFORE_SAVE_JS AGENT]/
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
// USE /[MANUAL FOR AFTER_SAVE_JS AGENT]/
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
// USE /[MANUAL FOR BEFORE_DEL_JS AGENT]/
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
// USE /[MANUAL FOR AFTER_DEL_JS AGENT]/
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
// USE /[MANUAL FOR BEFORE_EXIT_JS AGENT]/
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
// USE /[MANUAL FOR AFTER_EXIT_JS AGENT]/
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
// USE /[MANUAL FOR DLGUPDT AGENT]/
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
// USE /[MANUAL FOR CTRLBLR AGENT]/
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
// USE /[MANUAL FOR CTRLUPD AGENT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.afterControlUpdate(controlField, fieldValue)
			},
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FUNCTIONS_JS AGENT]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		watch: {
		}
	}
</script>
