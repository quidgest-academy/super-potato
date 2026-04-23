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
			data-key="PROPERTY"
			:data-identifier="primaryKeyValue"
			:data-loading="!formInitialDataLoaded || !isActiveForm">
			<template v-if="formControl.initialized && showFormBody">
				<q-row v-if="controls.PROPERTYPROPEID______.isVisible">
					<q-col
						v-if="controls.PROPERTYPROPEID______.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.PROPERTYPROPEID______.isVisible"
							class="i-text"
							v-bind="controls.PROPERTYPROPEID______.wrapperProps"
							:id="getControlId(controls.PROPERTYPROPEID______)"
							v-on="controls.PROPERTYPROPEID______.handlers"
							:loading="controls.PROPERTYPROPEID______.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-numeric-input
								v-if="controls.PROPERTYPROPEID______.isVisible"
								v-bind="controls.PROPERTYPROPEID______.props"
								:id="getControlId(controls.PROPERTYPROPEID______)"
								@update:model-value="model.ValId.fnUpdateValue" />
						</base-input-structure>
					</q-col>
				</q-row>
				<q-row v-if="controls.PROPERTYPROPESOLD____.isVisible || controls.PROPERTYPROPEDTSOLD__.isVisible">
					<q-col
						v-if="controls.PROPERTYPROPESOLD____.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.PROPERTYPROPESOLD____.isVisible"
							class="i-text"
							v-bind="controls.PROPERTYPROPESOLD____.wrapperProps"
							:id="getControlId(controls.PROPERTYPROPESOLD____)"
							v-on="controls.PROPERTYPROPESOLD____.handlers"
							:loading="controls.PROPERTYPROPESOLD____.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<template #label>
								<q-checkbox
									v-if="controls.PROPERTYPROPESOLD____.isVisible"
									v-bind="controls.PROPERTYPROPESOLD____.props"
									:id="getControlId(controls.PROPERTYPROPESOLD____)"
									v-on="controls.PROPERTYPROPESOLD____.handlers" />
							</template>
						</base-input-structure>
					</q-col>
					<q-col
						v-if="controls.PROPERTYPROPEDTSOLD__.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.PROPERTYPROPEDTSOLD__.isVisible"
							class="i-text"
							v-bind="controls.PROPERTYPROPEDTSOLD__.wrapperProps"
							:id="getControlId(controls.PROPERTYPROPEDTSOLD__)"
							v-on="controls.PROPERTYPROPEDTSOLD__.handlers"
							:loading="controls.PROPERTYPROPEDTSOLD__.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-date-time-picker
								v-if="controls.PROPERTYPROPEDTSOLD__.isVisible"
								v-bind="controls.PROPERTYPROPEDTSOLD__.props"
								:id="getControlId(controls.PROPERTYPROPEDTSOLD__)"
								:model-value="model.ValDtsold.value"
								@reset-icon-click="model.ValDtsold.fnUpdateValue(model.ValDtsold.originalValue ?? new Date())"
								@update:model-value="model.ValDtsold.fnUpdateValue($event ?? '')" />
						</base-input-structure>
					</q-col>
				</q-row>
				<q-row v-if="controls.PROPERTY__PROPE__LASTVISIT.isVisible">
					<q-col
						v-if="controls.PROPERTY__PROPE__LASTVISIT.isVisible"
						cols="auto">
						<base-input-structure
							v-if="controls.PROPERTY__PROPE__LASTVISIT.isVisible"
							class="i-text"
							v-bind="controls.PROPERTY__PROPE__LASTVISIT.wrapperProps"
							:id="getControlId(controls.PROPERTY__PROPE__LASTVISIT)"
							v-on="controls.PROPERTY__PROPE__LASTVISIT.handlers"
							:loading="controls.PROPERTY__PROPE__LASTVISIT.props.loading"
							:reporting-mode-on="reportingModeCAV"
							:suggestion-mode-on="suggestionModeOn">
							<q-date-time-picker
								v-if="controls.PROPERTY__PROPE__LASTVISIT.isVisible"
								v-bind="controls.PROPERTY__PROPE__LASTVISIT.props"
								:id="getControlId(controls.PROPERTY__PROPE__LASTVISIT)"
								:model-value="model.ValLastvisit.value"
								@reset-icon-click="model.ValLastvisit.fnUpdateValue(model.ValLastvisit.originalValue ?? new Date())"
								@update:model-value="model.ValLastvisit.fnUpdateValue($event ?? '')" />
						</base-input-structure>
					</q-col>
				</q-row>
				<q-row v-if="controls.PROPERTYPSEUDNEWGRP05.isVisible">
					<q-col v-if="controls.PROPERTYPSEUDNEWGRP05.isVisible">
						<q-accordion
							v-if="controls.PROPERTYPSEUDNEWGRP05.isVisible"
							:id="getControlId(controls.PROPERTYPSEUDNEWGRP05)"
							v-model="controls.PROPERTYPSEUDNEWGRP05.openChild">
							<!-- Start PROPERTYPSEUDNEWGRP05 -->
							<q-accordion-item
								v-if="controls.PROPERTYPSEUDNEWGRP02.isVisible"
								:id="getControlId(controls.PROPERTYPSEUDNEWGRP02) + '-container'"
								value="PROPERTYPSEUDNEWGRP02"
								:title="controls.PROPERTYPSEUDNEWGRP02.label">
								<!-- Start PROPERTYPSEUDNEWGRP02 -->
								<q-row v-if="controls.PROPERTYCITY_CITY____.isVisible || controls.PROPERTYCOUNTCOUNTRY_.isVisible || controls.PROPERTYPROPETAX_____.isVisible">
									<q-col
										v-if="controls.PROPERTYCITY_CITY____.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYCITY_CITY____.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYCITY_CITY____.wrapperProps"
											:id="getControlId(controls.PROPERTYCITY_CITY____)"
											v-on="controls.PROPERTYCITY_CITY____.handlers"
											:loading="controls.PROPERTYCITY_CITY____.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-lookup
												v-if="controls.PROPERTYCITY_CITY____.isVisible"
												v-bind="controls.PROPERTYCITY_CITY____.props"
												:id="getControlId(controls.PROPERTYCITY_CITY____)"
												v-on="controls.PROPERTYCITY_CITY____.handlers" />
											<q-see-more-propertycity-city
												v-if="controls.PROPERTYCITY_CITY____.seeMoreIsVisible"
												v-bind="controls.PROPERTYCITY_CITY____.seeMoreParams"
												v-on="controls.PROPERTYCITY_CITY____.handlers" />
										</base-input-structure>
									</q-col>
									<q-col
										v-if="controls.PROPERTYCOUNTCOUNTRY_.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYCOUNTCOUNTRY_.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYCOUNTCOUNTRY_.wrapperProps"
											:id="getControlId(controls.PROPERTYCOUNTCOUNTRY_)"
											v-on="controls.PROPERTYCOUNTCOUNTRY_.handlers"
											:loading="controls.PROPERTYCOUNTCOUNTRY_.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-text-field
												v-bind="controls.PROPERTYCOUNTCOUNTRY_.props"
												:id="getControlId(controls.PROPERTYCOUNTCOUNTRY_)"
												@blur="onBlur(controls.PROPERTYCOUNTCOUNTRY_, model.CityCountValCountry.value)"
												@change="model.CityCountValCountry.fnUpdateValueOnChange" />
										</base-input-structure>
									</q-col>
									<q-col
										v-if="controls.PROPERTYPROPETAX_____.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYPROPETAX_____.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYPROPETAX_____.wrapperProps"
											:id="getControlId(controls.PROPERTYPROPETAX_____)"
											v-on="controls.PROPERTYPROPETAX_____.handlers"
											:loading="controls.PROPERTYPROPETAX_____.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-numeric-input
												v-if="controls.PROPERTYPROPETAX_____.isVisible"
												v-bind="controls.PROPERTYPROPETAX_____.props"
												:id="getControlId(controls.PROPERTYPROPETAX_____)"
												@update:model-value="model.ValTax.fnUpdateValue" />
										</base-input-structure>
									</q-col>
								</q-row>
								<!-- End PROPERTYPSEUDNEWGRP02 -->
							</q-accordion-item>
							<q-accordion-item
								v-if="controls.PROPERTYPSEUDNEWGRP03.isVisible"
								:id="getControlId(controls.PROPERTYPSEUDNEWGRP03) + '-container'"
								value="PROPERTYPSEUDNEWGRP03"
								:title="controls.PROPERTYPSEUDNEWGRP03.label">
								<!-- Start PROPERTYPSEUDNEWGRP03 -->
								<q-row v-if="controls.PROPERTYPROPETYPOLOGY.isVisible">
									<q-col
										v-if="controls.PROPERTYPROPETYPOLOGY.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYPROPETYPOLOGY.isVisible"
											class="i-radio-container"
											v-bind="controls.PROPERTYPROPETYPOLOGY.wrapperProps"
											:id="getControlId(controls.PROPERTYPROPETYPOLOGY)"
											v-on="controls.PROPERTYPROPETYPOLOGY.handlers"
											:label-position="labelAlignment.topleft"
											:loading="controls.PROPERTYPROPETYPOLOGY.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-radio-group
												v-if="controls.PROPERTYPROPETYPOLOGY.isVisible"
												v-bind="controls.PROPERTYPROPETYPOLOGY.props"
												:id="getControlId(controls.PROPERTYPROPETYPOLOGY)"
												v-on="controls.PROPERTYPROPETYPOLOGY.handlers">
												<q-radio-button
													v-for="radio in controls.PROPERTYPROPETYPOLOGY.items"
													:key="radio.key"
													:label="radio.value"
													:value="radio.key" />
											</q-radio-group>
										</base-input-structure>
									</q-col>
								</q-row>
								<q-row v-if="controls.PROPERTYPROPEBUILDTYP.isVisible || controls.PROPERTYPROPEGRDSIZE_.isVisible || controls.PROPERTYPROPEFLOORNR_.isVisible">
									<q-col
										v-if="controls.PROPERTYPROPEBUILDTYP.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYPROPEBUILDTYP.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYPROPEBUILDTYP.wrapperProps"
											:id="getControlId(controls.PROPERTYPROPEBUILDTYP)"
											v-on="controls.PROPERTYPROPEBUILDTYP.handlers"
											:loading="controls.PROPERTYPROPEBUILDTYP.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-select
												v-if="controls.PROPERTYPROPEBUILDTYP.isVisible"
												v-bind="controls.PROPERTYPROPEBUILDTYP.props"
												:id="getControlId(controls.PROPERTYPROPEBUILDTYP)"
												@update:model-value="model.ValBuildtyp.fnUpdateValue" />
										</base-input-structure>
									</q-col>
									<q-col
										v-if="controls.PROPERTYPROPEGRDSIZE_.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYPROPEGRDSIZE_.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYPROPEGRDSIZE_.wrapperProps"
											:id="getControlId(controls.PROPERTYPROPEGRDSIZE_)"
											v-on="controls.PROPERTYPROPEGRDSIZE_.handlers"
											:loading="controls.PROPERTYPROPEGRDSIZE_.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-numeric-input
												v-if="controls.PROPERTYPROPEGRDSIZE_.isVisible"
												v-bind="controls.PROPERTYPROPEGRDSIZE_.props"
												:id="getControlId(controls.PROPERTYPROPEGRDSIZE_)"
												@update:model-value="model.ValGrdsize.fnUpdateValue" />
										</base-input-structure>
									</q-col>
									<q-col
										v-if="controls.PROPERTYPROPEFLOORNR_.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYPROPEFLOORNR_.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYPROPEFLOORNR_.wrapperProps"
											:id="getControlId(controls.PROPERTYPROPEFLOORNR_)"
											v-on="controls.PROPERTYPROPEFLOORNR_.handlers"
											:loading="controls.PROPERTYPROPEFLOORNR_.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-numeric-input
												v-if="controls.PROPERTYPROPEFLOORNR_.isVisible"
												v-bind="controls.PROPERTYPROPEFLOORNR_.props"
												:id="getControlId(controls.PROPERTYPROPEFLOORNR_)"
												@update:model-value="model.ValFloornr.fnUpdateValue" />
										</base-input-structure>
									</q-col>
								</q-row>
								<q-row v-if="controls.PROPERTYPROPESIZE____.isVisible || controls.PROPERTYPROPEBATHNR__.isVisible">
									<q-col
										v-if="controls.PROPERTYPROPESIZE____.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYPROPESIZE____.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYPROPESIZE____.wrapperProps"
											:id="getControlId(controls.PROPERTYPROPESIZE____)"
											v-on="controls.PROPERTYPROPESIZE____.handlers"
											:loading="controls.PROPERTYPROPESIZE____.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-numeric-input
												v-if="controls.PROPERTYPROPESIZE____.isVisible"
												v-bind="controls.PROPERTYPROPESIZE____.props"
												:id="getControlId(controls.PROPERTYPROPESIZE____)"
												@update:model-value="model.ValSize.fnUpdateValue" />
										</base-input-structure>
									</q-col>
									<q-col
										v-if="controls.PROPERTYPROPEBATHNR__.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYPROPEBATHNR__.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYPROPEBATHNR__.wrapperProps"
											:id="getControlId(controls.PROPERTYPROPEBATHNR__)"
											v-on="controls.PROPERTYPROPEBATHNR__.handlers"
											:loading="controls.PROPERTYPROPEBATHNR__.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-numeric-input
												v-if="controls.PROPERTYPROPEBATHNR__.isVisible"
												v-bind="controls.PROPERTYPROPEBATHNR__.props"
												:id="getControlId(controls.PROPERTYPROPEBATHNR__)"
												@update:model-value="model.ValBathnr.fnUpdateValue" />
										</base-input-structure>
									</q-col>
								</q-row>
								<q-row v-if="controls.PROPERTYPROPEDTCONST_.isVisible || controls.PROPERTYPROPEBUILDAGE.isVisible">
									<q-col
										v-if="controls.PROPERTYPROPEDTCONST_.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYPROPEDTCONST_.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYPROPEDTCONST_.wrapperProps"
											:id="getControlId(controls.PROPERTYPROPEDTCONST_)"
											v-on="controls.PROPERTYPROPEDTCONST_.handlers"
											:loading="controls.PROPERTYPROPEDTCONST_.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-date-time-picker
												v-if="controls.PROPERTYPROPEDTCONST_.isVisible"
												v-bind="controls.PROPERTYPROPEDTCONST_.props"
												:id="getControlId(controls.PROPERTYPROPEDTCONST_)"
												:model-value="model.ValDtconst.value"
												@reset-icon-click="model.ValDtconst.fnUpdateValue(model.ValDtconst.originalValue ?? new Date())"
												@update:model-value="model.ValDtconst.fnUpdateValue($event ?? '')" />
										</base-input-structure>
									</q-col>
									<q-col
										v-if="controls.PROPERTYPROPEBUILDAGE.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYPROPEBUILDAGE.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYPROPEBUILDAGE.wrapperProps"
											:id="getControlId(controls.PROPERTYPROPEBUILDAGE)"
											v-on="controls.PROPERTYPROPEBUILDAGE.handlers"
											:loading="controls.PROPERTYPROPEBUILDAGE.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-numeric-input
												v-if="controls.PROPERTYPROPEBUILDAGE.isVisible"
												v-bind="controls.PROPERTYPROPEBUILDAGE.props"
												:id="getControlId(controls.PROPERTYPROPEBUILDAGE)"
												@update:model-value="model.ValBuildage.fnUpdateValue" />
										</base-input-structure>
									</q-col>
								</q-row>
								<!-- End PROPERTYPSEUDNEWGRP03 -->
							</q-accordion-item>
							<q-accordion-item
								v-if="controls.PROPERTYPSEUDNEWGRP04.isVisible"
								:id="getControlId(controls.PROPERTYPSEUDNEWGRP04) + '-container'"
								value="PROPERTYPSEUDNEWGRP04"
								:title="controls.PROPERTYPSEUDNEWGRP04.label">
								<!-- Start PROPERTYPSEUDNEWGRP04 -->
								<q-row v-if="controls.PROPERTYAGENTNAME____.isVisible || controls.PROPERTYAGENTPHOTOGRA.isVisible || controls.PROPERTYAGENTEMAIL___.isVisible">
									<q-col
										v-if="controls.PROPERTYAGENTNAME____.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYAGENTNAME____.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYAGENTNAME____.wrapperProps"
											:id="getControlId(controls.PROPERTYAGENTNAME____)"
											v-on="controls.PROPERTYAGENTNAME____.handlers"
											:loading="controls.PROPERTYAGENTNAME____.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-lookup
												v-if="controls.PROPERTYAGENTNAME____.isVisible"
												v-bind="controls.PROPERTYAGENTNAME____.props"
												:id="getControlId(controls.PROPERTYAGENTNAME____)"
												v-on="controls.PROPERTYAGENTNAME____.handlers" />
											<q-see-more-propertyagentname
												v-if="controls.PROPERTYAGENTNAME____.seeMoreIsVisible"
												v-bind="controls.PROPERTYAGENTNAME____.seeMoreParams"
												v-on="controls.PROPERTYAGENTNAME____.handlers" />
										</base-input-structure>
									</q-col>
									<q-col
										v-if="controls.PROPERTYAGENTPHOTOGRA.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYAGENTPHOTOGRA.isVisible"
											class="q-image"
											v-bind="controls.PROPERTYAGENTPHOTOGRA.wrapperProps"
											:id="getControlId(controls.PROPERTYAGENTPHOTOGRA)"
											v-on="controls.PROPERTYAGENTPHOTOGRA.handlers"
											:loading="controls.PROPERTYAGENTPHOTOGRA.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-image
												v-if="controls.PROPERTYAGENTPHOTOGRA.isVisible"
												v-bind="controls.PROPERTYAGENTPHOTOGRA.props"
												:id="getControlId(controls.PROPERTYAGENTPHOTOGRA)"
												v-on="controls.PROPERTYAGENTPHOTOGRA.handlers" />
										</base-input-structure>
									</q-col>
									<q-col
										v-if="controls.PROPERTYAGENTEMAIL___.isVisible"
										cols="auto">
										<base-input-structure
											v-if="controls.PROPERTYAGENTEMAIL___.isVisible"
											class="i-text"
											v-bind="controls.PROPERTYAGENTEMAIL___.wrapperProps"
											:id="getControlId(controls.PROPERTYAGENTEMAIL___)"
											v-on="controls.PROPERTYAGENTEMAIL___.handlers"
											:loading="controls.PROPERTYAGENTEMAIL___.props.loading"
											:reporting-mode-on="reportingModeCAV"
											:suggestion-mode-on="suggestionModeOn">
											<q-text-field
												v-bind="controls.PROPERTYAGENTEMAIL___.props"
												:id="getControlId(controls.PROPERTYAGENTEMAIL___)"
												@blur="onBlur(controls.PROPERTYAGENTEMAIL___, model.AgentValEmail.value)"
												@change="model.AgentValEmail.fnUpdateValueOnChange" />
										</base-input-structure>
									</q-col>
								</q-row>
								<!-- End PROPERTYPSEUDNEWGRP04 -->
							</q-accordion-item>
							<!-- End PROPERTYPSEUDNEWGRP05 -->
						</q-accordion>
					</q-col>
				</q-row>
				<q-row v-if="controls.PROPERTYPSEUDNEWGRP01.isVisible">
					<q-col v-if="controls.PROPERTYPSEUDNEWGRP01.isVisible">
						<q-group-box-container
							v-if="controls.PROPERTYPSEUDNEWGRP01.isVisible"
							v-bind="controls.PROPERTYPSEUDNEWGRP01"
							:id="getControlId(controls.PROPERTYPSEUDNEWGRP01)"
							:no-border="controls.PROPERTYPSEUDNEWGRP01.borderless">
							<!-- Start PROPERTYPSEUDNEWGRP01 -->
							<q-row v-if="controls.PROPERTYPROPEAVERAGE_.isVisible">
								<q-col
									v-if="controls.PROPERTYPROPEAVERAGE_.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.PROPERTYPROPEAVERAGE_.isVisible"
										class="i-text"
										v-bind="controls.PROPERTYPROPEAVERAGE_.wrapperProps"
										:id="getControlId(controls.PROPERTYPROPEAVERAGE_)"
										v-on="controls.PROPERTYPROPEAVERAGE_.handlers"
										:loading="controls.PROPERTYPROPEAVERAGE_.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-numeric-input
											v-if="controls.PROPERTYPROPEAVERAGE_.isVisible"
											v-bind="controls.PROPERTYPROPEAVERAGE_.props"
											:id="getControlId(controls.PROPERTYPROPEAVERAGE_)"
											@update:model-value="model.ValAverage.fnUpdateValue" />
									</base-input-structure>
								</q-col>
							</q-row>
							<q-row v-if="controls.PROPERTYPROPEPRICE___.isVisible || controls.PROPERTYPROPETITLE___.isVisible || controls.PROPERTYPROPEPHOTO___.isVisible">
								<q-col
									v-if="controls.PROPERTYPROPEPRICE___.isVisible || controls.PROPERTYPROPETITLE___.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.PROPERTYPROPEPRICE___.isVisible"
										class="i-text"
										v-bind="controls.PROPERTYPROPEPRICE___.wrapperProps"
										:id="getControlId(controls.PROPERTYPROPEPRICE___)"
										v-on="controls.PROPERTYPROPEPRICE___.handlers"
										:loading="controls.PROPERTYPROPEPRICE___.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-numeric-input
											v-if="controls.PROPERTYPROPEPRICE___.isVisible"
											v-bind="controls.PROPERTYPROPEPRICE___.props"
											:id="getControlId(controls.PROPERTYPROPEPRICE___)"
											@update:model-value="model.ValPrice.fnUpdateValue" />
									</base-input-structure>
									<base-input-structure
										v-if="controls.PROPERTYPROPETITLE___.isVisible"
										class="i-text"
										v-bind="controls.PROPERTYPROPETITLE___.wrapperProps"
										:id="getControlId(controls.PROPERTYPROPETITLE___)"
										v-on="controls.PROPERTYPROPETITLE___.handlers"
										:loading="controls.PROPERTYPROPETITLE___.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-text-field
											v-bind="controls.PROPERTYPROPETITLE___.props"
											:id="getControlId(controls.PROPERTYPROPETITLE___)"
											@blur="onBlur(controls.PROPERTYPROPETITLE___, model.ValTitle.value)"
											@change="model.ValTitle.fnUpdateValueOnChange" />
									</base-input-structure>
								</q-col>
								<q-col
									v-if="controls.PROPERTYPROPEPHOTO___.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.PROPERTYPROPEPHOTO___.isVisible"
										class="q-image"
										v-bind="controls.PROPERTYPROPEPHOTO___.wrapperProps"
										:id="getControlId(controls.PROPERTYPROPEPHOTO___)"
										v-on="controls.PROPERTYPROPEPHOTO___.handlers"
										:loading="controls.PROPERTYPROPEPHOTO___.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-image
											v-if="controls.PROPERTYPROPEPHOTO___.isVisible"
											v-bind="controls.PROPERTYPROPEPHOTO___.props"
											:id="getControlId(controls.PROPERTYPROPEPHOTO___)"
											v-on="controls.PROPERTYPROPEPHOTO___.handlers" />
									</base-input-structure>
								</q-col>
							</q-row>
							<q-row v-if="controls.PROPERTYPROPEDESCRIPT.isVisible">
								<q-col
									v-if="controls.PROPERTYPROPEDESCRIPT.isVisible"
									cols="auto">
									<base-input-structure
										v-if="controls.PROPERTYPROPEDESCRIPT.isVisible"
										class="i-textarea"
										v-bind="controls.PROPERTYPROPEDESCRIPT.wrapperProps"
										:id="getControlId(controls.PROPERTYPROPEDESCRIPT)"
										v-on="controls.PROPERTYPROPEDESCRIPT.handlers"
										:loading="controls.PROPERTYPROPEDESCRIPT.props.loading"
										:reporting-mode-on="reportingModeCAV"
										:suggestion-mode-on="suggestionModeOn">
										<q-text-area
											v-if="controls.PROPERTYPROPEDESCRIPT.isVisible"
											v-bind="controls.PROPERTYPROPEDESCRIPT.props"
											:id="getControlId(controls.PROPERTYPROPEDESCRIPT)"
											v-on="controls.PROPERTYPROPEDESCRIPT.handlers" />
									</base-input-structure>
								</q-col>
							</q-row>
							<!-- End PROPERTYPSEUDNEWGRP01 -->
						</q-group-box-container>
					</q-col>
				</q-row>
				<q-row v-if="controls.PROPERTYPSEUDFIELD001.isVisible">
					<q-col v-if="controls.PROPERTYPSEUDFIELD001.isVisible">
						<q-table
							v-if="controls.PROPERTYPSEUDFIELD001.isVisible"
							v-bind="controls.PROPERTYPSEUDFIELD001"
							:id="getControlId(controls.PROPERTYPSEUDFIELD001)"
							v-on="controls.PROPERTYPSEUDFIELD001.handlers">
							<template #header>
								<q-table-config
									:table-ctrl="controls.PROPERTYPSEUDFIELD001"
									v-on="controls.PROPERTYPSEUDFIELD001.handlers" />
							</template>
							<!-- USE /[MANUAL FOR CUSTOM_TABLE PROPERTYPSEUDFIELD001]/ -->
						</q-table>
					</q-col>
				</q-row>
				<q-row v-if="controls.PROPERTYPSEUDFIELD002.isVisible">
					<q-col v-if="controls.PROPERTYPSEUDFIELD002.isVisible">
						<q-table
							v-if="controls.PROPERTYPSEUDFIELD002.isVisible"
							v-bind="controls.PROPERTYPSEUDFIELD002"
							:id="getControlId(controls.PROPERTYPSEUDFIELD002)"
							v-on="controls.PROPERTYPSEUDFIELD002.handlers">
							<template #header>
								<q-table-config
									:table-ctrl="controls.PROPERTYPSEUDFIELD002"
									v-on="controls.PROPERTYPSEUDFIELD002.handlers" />
							</template>
							<!-- USE /[MANUAL FOR CUSTOM_TABLE PROPERTYPSEUDFIELD002]/ -->
						</q-table>
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

	import FormViewModel from './QFormPropertyViewModel.js'

	const requiredTextResources = ['QFormProperty', 'hardcoded', 'messages']

/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FORM_INCLUDEJS PROPERTY]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	export default {
		name: 'QFormProperty',

		components: {
			QSeeMorePropertycityCity: defineAsyncComponent(() => import('@/views/forms/FormProperty/dbedits/PropertycityCitySeeMore.vue')),
			QSeeMorePropertyagentname: defineAsyncComponent(() => import('@/views/forms/FormProperty/dbedits/PropertyagentnameSeeMore.vue')),
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
					name: 'PROPERTY',
					location: 'form-PROPERTY',
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
				componentOnLoadProc: asyncProcM.getProcListMonitor('QFormProperty', false),

				interfaceMetadata: {
					id: 'QFormProperty', // Used for resources
					requiredTextResources
				},

				formInfo: {
					type: 'normal',
					name: 'PROPERTY',
					route: 'form-PROPERTY',
					area: 'PROPE',
					primaryKey: 'ValCodprope',
					designation: computed(() => this.Resources.PROPERTY43977),
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
					PROPERTYPROPEID______: new fieldControlClass.NumberControl({
						modelField: 'ValId',
						valueChangeEvent: 'fieldChange:prope.id',
						id: 'PROPERTYPROPEID______',
						name: 'ID',
						size: 'mini',
						label: computed(() => this.Resources.ORDER39632),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						maxIntegers: 5,
						maxDecimals: 0,
						isSequencial: true,
						controlLimits: [
						],
					}, this),
					PROPERTYPROPESOLD____: new fieldControlClass.BooleanControl({
						modelField: 'ValSold',
						valueChangeEvent: 'fieldChange:prope.sold',
						id: 'PROPERTYPROPESOLD____',
						name: 'SOLD',
						size: 'mini',
						label: computed(() => this.Resources.SOLD59824),
						placeholder: '',
						labelPosition: computed(() => this.$app.layout.CheckboxLabelAlignment),
						controlLimits: [
						],
					}, this),
					PROPERTYPROPEDTSOLD__: new fieldControlClass.DateControl({
						modelField: 'ValDtsold',
						valueChangeEvent: 'fieldChange:prope.dtsold',
						id: 'PROPERTYPROPEDTSOLD__',
						name: 'DTSOLD',
						size: 'small',
						label: computed(() => this.Resources.SOLD_DATE37976),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						dateTimeType: 'date',
						controlLimits: [
						],
					}, this),
					PROPERTY__PROPE__LASTVISIT: new fieldControlClass.DateControl({
						modelField: 'ValLastvisit',
						valueChangeEvent: 'fieldChange:prope.lastvisit',
						id: 'PROPERTY__PROPE__LASTVISIT',
						name: 'LASTVISIT',
						size: 'small',
						label: computed(() => this.Resources.LAST_VISIT61343),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						isFormulaBlocked: true,
						dateTimeType: 'date',
						controlLimits: [
						],
					}, this),
					PROPERTYPSEUDNEWGRP05: new fieldControlClass.AccordionControl({
						id: 'PROPERTYPSEUDNEWGRP05',
						name: 'NEWGRP05',
						size: 'block',
						label: computed(() => this.Resources.NEW_GROUP63448),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						isCollapsible: false,
						anchored: false,
						directChildren: ['PROPERTYPSEUDNEWGRP02', 'PROPERTYPSEUDNEWGRP03', 'PROPERTYPSEUDNEWGRP04'],
						controlLimits: [
						],
					}, this),
					PROPERTYPSEUDNEWGRP01: new fieldControlClass.GroupControl({
						id: 'PROPERTYPSEUDNEWGRP01',
						name: 'NEWGRP01',
						size: 'block',
						label: computed(() => this.Resources.MAIN_INFO53406),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						borderless: false,
						isCollapsible: false,
						anchored: false,
						directChildren: ['PROPERTYPROPEAVERAGE_', 'PROPERTYPROPEPRICE___', 'PROPERTYPROPETITLE___', 'PROPERTYPROPEPHOTO___', 'PROPERTYPROPEDESCRIPT'],
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					PROPERTYPSEUDFIELD001: new fieldControlClass.TableListControl({
						id: 'PROPERTYPSEUDFIELD001',
						name: 'FIELD001',
						size: 'block',
						label: computed(() => this.Resources.ALBUM37050),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						headerLevel: computed(() => this.baseHeadingLevel + 1),
						controller: 'PROPE',
						action: 'Property_ValField001',
						hasDependencies: false,
						isInCollapsible: false,
						columnsOriginal: [
							new listColumnTypes.ImageColumn({
								order: 1,
								name: 'ValPhoto',
								area: 'PHOTO',
								field: 'PHOTO',
								label: computed(() => this.Resources.PHOTO51874),
								dataTitle: computed(() => genericFunctions.formatString(vm.Resources.IMAGEM_UTILIZADA_PAR58591, vm.Resources.PHOTO51874)),
								scrollData: 3,
								sortable: false,
								searchable: false,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.TextColumn({
								order: 2,
								name: 'ValTitle',
								area: 'PHOTO',
								field: 'TITLE',
								label: computed(() => this.Resources.TITLE21885),
								dataLength: 50,
								scrollData: 30,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
						],
						config: {
							name: 'ValField001',
							serverMode: true,
							pkColumn: 'ValCodphoto',
							tableAlias: 'PHOTO',
							tableNamePlural: computed(() => this.Resources.PHOTOS_ALBUM58513),
							viewManagement: '',
							showLimitsInfo: true,
							tableTitle: computed(() => this.Resources.ALBUM37050),
							perPage: 4,
							showAlternatePagination: true,
							permissions: {
							},
							searchBarConfig: {
								visibility: false
							},
							allowColumnFilters: false,
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
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'ALBUM',
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
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'ALBUM',
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
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'ALBUM',
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
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'ALBUM',
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
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'ALBUM',
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
								id: 'RCA__ALBUM',
								name: '_ALBUM',
								title: '',
								isInReadOnly: true,
								params: {
									isRoute: true,
									canExecuteAction: vm.applyChanges,
									action: vm.openFormAction,
									type: 'form',
									formName: 'ALBUM',
									mode: 'SHOW',
									isControlled: true
								}
							},
							formsDefinition: {
								'ALBUM': {
									fnKeySelector: (row) => row.Fields.ValCodphoto,
									isPopup: true
								},
							},
							defaultSearchColumnName: 'ValTitle',
							defaultSearchColumnNameOriginal: 'ValTitle',
							defaultColumnSorting: {
								columnName: '',
								sortOrder: 'asc'
							}
						},
						globalEvents: ['changed-PHOTO', 'changed-PROPE'],
						uuid: 'Property_ValField001',
						allSelectedRows: 'false',
						controlLimits: [
							{
								identifier: ['id', 'prope'],
								dependencyEvents: ['fieldChange:prope.codprope'],
								dependencyField: 'PROPE.CODPROPE',
								fnValueSelector: (model) => model.ValCodprope.value
							},
						],
					}, this),
					PROPERTYPSEUDFIELD002: new fieldControlClass.TableListControl({
						id: 'PROPERTYPSEUDFIELD002',
						name: 'FIELD002',
						size: 'block',
						label: computed(() => this.Resources.CONTACTS55742),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						headerLevel: computed(() => this.baseHeadingLevel + 1),
						controller: 'PROPE',
						action: 'Property_ValField002',
						hasDependencies: true,
						isInCollapsible: false,
						columnsOriginal: [
							new listColumnTypes.DateColumn({
								order: 1,
								name: 'ValDate',
								area: 'CONTA',
								field: 'DATE',
								label: computed(() => this.Resources.DATE18475),
								scrollData: 8,
								dateTimeType: 'date',
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.TextColumn({
								order: 2,
								name: 'ValClient',
								area: 'CONTA',
								field: 'CLIENT',
								label: computed(() => this.Resources.CLIENT_NAME39245),
								dataLength: 50,
								scrollData: 30,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
							new listColumnTypes.TextColumn({
								order: 3,
								name: 'ValDescript',
								area: 'CONTA',
								field: 'DESCRIPT',
								label: computed(() => this.Resources.DESCRIPTION07383),
								scrollData: 30,
								export: 1,
							}, computed(() => vm.model), computed(() => vm.internalEvents)),
						],
						config: {
							name: 'ValField002',
							serverMode: true,
							pkColumn: 'ValCodconta',
							tableAlias: 'CONTA',
							tableNamePlural: computed(() => this.Resources.CONTACTS55742),
							viewManagement: '',
							showLimitsInfo: true,
							tableTitle: computed(() => this.Resources.CONTACTS55742),
							perPage: 4,
							showAlternatePagination: true,
							permissions: {
							},
							searchBarConfig: {
								visibility: false
							},
							allowColumnFilters: false,
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
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'CONTACT',
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
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'CONTACT',
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
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'CONTACT',
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
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'CONTACT',
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
									isInReadOnly: false,
									params: {
										canExecuteAction: vm.applyChanges,
										action: vm.openFormAction,
										type: 'form',
										formName: 'CONTACT',
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
								id: 'RCA__CONTACT',
								name: '_CONTACT',
								title: '',
								isInReadOnly: true,
								params: {
									isRoute: true,
									canExecuteAction: vm.applyChanges,
									action: vm.openFormAction,
									type: 'form',
									formName: 'CONTACT',
									mode: 'SHOW',
									isControlled: true
								}
							},
							formsDefinition: {
								'CONTACT': {
									fnKeySelector: (row) => row.Fields.ValCodconta,
									isPopup: true
								},
							},
							defaultSearchColumnName: 'ValClient',
							defaultSearchColumnNameOriginal: 'ValClient',
							defaultColumnSorting: {
								columnName: '',
								sortOrder: 'asc'
							}
						},
						globalEvents: ['changed-PROPE', 'changed-CONTA'],
						uuid: 'Property_ValField002',
						allSelectedRows: 'false',
						controlLimits: [
							{
								identifier: ['id', 'prope'],
								dependencyEvents: ['fieldChange:prope.codprope'],
								dependencyField: 'PROPE.CODPROPE',
								fnValueSelector: (model) => model.ValCodprope.value
							},
						],
					}, this),
					PROPERTYPROPEAVERAGE_: new fieldControlClass.NumberControl({
						modelField: 'ValAverage',
						valueChangeEvent: 'fieldChange:prope.average',
						id: 'PROPERTYPROPEAVERAGE_',
						name: 'AVERAGE',
						size: 'medium',
						label: computed(() => this.Resources.AVERAGEPRICE13700),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP01',
						isFormulaBlocked: true,
						maxIntegers: 12,
						maxDecimals: 0,
						controlLimits: [
						],
					}, this),
					PROPERTYPSEUDNEWGRP02: new fieldControlClass.GroupControl({
						id: 'PROPERTYPSEUDNEWGRP02',
						name: 'NEWGRP02',
						size: 'block',
						label: computed(() => this.Resources.LOCALIZATION34148),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP05',
						isInAccordion: true,
						borderless: false,
						isCollapsible: true,
						anchored: false,
						directChildren: ['PROPERTYCITY_CITY____', 'PROPERTYCOUNTCOUNTRY_', 'PROPERTYPROPETAX_____'],
						controlLimits: [
						],
					}, this),
					PROPERTYCITY_CITY____: new fieldControlClass.LookupControl({
						modelField: 'TableCityCity',
						valueChangeEvent: 'fieldChange:city.city',
						id: 'PROPERTYCITY_CITY____',
						name: 'CITY',
						size: 'xxlarge',
						label: computed(() => this.Resources.CITY42505),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP02',
						externalCallbacks: {
							getModelField: vm.getModelField,
							getModelFieldValue: vm.getModelFieldValue,
							setModelFieldValue: vm.setModelFieldValue
						},
						externalProperties: {
							modelKeys: computed(() => vm.modelKeys)
						},
						lookupKeyModelField: {
							name: 'ValCodcity',
							dependencyEvent: 'fieldChange:prope.codcity'
						},
						dependentFields: () => ({
							set 'city.codcity'(value) { vm.model.ValCodcity.updateValue(value) },
							set 'city.city'(value) { vm.model.TableCityCity.updateValue(value) },
							set 'count.country'(value) { vm.model.CityCountValCountry.updateValue(value) },
						}),
						controlLimits: [
						],
					}, this),
					PROPERTYCOUNTCOUNTRY_: new fieldControlClass.StringControl({
						modelField: 'CityCountValCountry',
						valueChangeEvent: 'fieldChange:count.country',
						dependentModelField: 'ValCodcount',
						dependentChangeEvent: 'fieldChange:city.codcount',
						id: 'PROPERTYCOUNTCOUNTRY_',
						name: 'COUNTRY',
						size: 'xlarge',
						label: computed(() => this.Resources.COUNTRY64133),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP02',
						maxLength: 50,
						controlLimits: [
						],
					}, this),
					PROPERTYPROPEPRICE___: new fieldControlClass.CurrencyControl({
						modelField: 'ValPrice',
						valueChangeEvent: 'fieldChange:prope.price',
						id: 'PROPERTYPROPEPRICE___',
						name: 'PRICE',
						size: 'medium',
						label: computed(() => this.Resources.PRICE06900),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP01',
						maxIntegers: 9,
						maxDecimals: 2,
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					PROPERTYPSEUDNEWGRP03: new fieldControlClass.GroupControl({
						id: 'PROPERTYPSEUDNEWGRP03',
						name: 'NEWGRP03',
						size: 'block',
						label: computed(() => this.Resources.DETAILS19591),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP05',
						isInAccordion: true,
						borderless: false,
						isCollapsible: true,
						anchored: false,
						directChildren: ['PROPERTYPROPETYPOLOGY', 'PROPERTYPROPEBUILDTYP', 'PROPERTYPROPEGRDSIZE_', 'PROPERTYPROPEFLOORNR_', 'PROPERTYPROPESIZE____', 'PROPERTYPROPEBATHNR__', 'PROPERTYPROPEDTCONST_', 'PROPERTYPROPEBUILDAGE'],
						controlLimits: [
						],
					}, this),
					PROPERTYPROPETYPOLOGY: new fieldControlClass.RadioGroupControl({
						modelField: 'ValTypology',
						valueChangeEvent: 'fieldChange:prope.typology',
						id: 'PROPERTYPROPETYPOLOGY',
						name: 'TYPOLOGY',
						label: computed(() => this.Resources.BUILDING_TYPOLOGY54011),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP03',
						maxIntegers: 1,
						maxDecimals: 0,
						arrayName: 'typology',
						columns: 5,
						controlLimits: [
						],
					}, this),
					PROPERTYPROPEBUILDTYP: new fieldControlClass.ArrayStringControl({
						modelField: 'ValBuildtyp',
						valueChangeEvent: 'fieldChange:prope.buildtyp',
						id: 'PROPERTYPROPEBUILDTYP',
						name: 'BUILDTYP',
						size: 'small',
						label: computed(() => this.Resources.BUILDING_TYPE57152),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP03',
						maxLength: 1,
						arrayName: 'buildtyp',
						helpShortItem: 'None',
						helpDetailedItem: 'None',
						controlLimits: [
						],
					}, this),
					PROPERTYPROPEGRDSIZE_: new fieldControlClass.NumberControl({
						modelField: 'ValGrdsize',
						valueChangeEvent: 'fieldChange:prope.grdsize',
						id: 'PROPERTYPROPEGRDSIZE_',
						name: 'GRDSIZE',
						size: 'small',
						label: computed(() => this.Resources.GROUND_SIZE01563),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP03',
						maxIntegers: 9,
						maxDecimals: 0,
						controlLimits: [
						],
					}, this),
					PROPERTYPROPEFLOORNR_: new fieldControlClass.NumberControl({
						modelField: 'ValFloornr',
						valueChangeEvent: 'fieldChange:prope.floornr',
						id: 'PROPERTYPROPEFLOORNR_',
						name: 'FLOORNR',
						size: 'small',
						label: computed(() => this.Resources.FLOOR19993),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP03',
						maxIntegers: 2,
						maxDecimals: 0,
						controlLimits: [
						],
					}, this),
					PROPERTYPROPESIZE____: new fieldControlClass.NumberControl({
						modelField: 'ValSize',
						valueChangeEvent: 'fieldChange:prope.size',
						id: 'PROPERTYPROPESIZE____',
						name: 'SIZE',
						size: 'small',
						label: computed(() => this.Resources.SIZE__M2_57059),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP03',
						maxIntegers: 8,
						maxDecimals: 0,
						controlLimits: [
						],
					}, this),
					PROPERTYPROPEBATHNR__: new fieldControlClass.NumberControl({
						modelField: 'ValBathnr',
						valueChangeEvent: 'fieldChange:prope.bathnr',
						id: 'PROPERTYPROPEBATHNR__',
						name: 'BATHNR',
						size: 'small',
						label: computed(() => this.Resources.BATHROOMS_NUMBER52698),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP03',
						maxIntegers: 2,
						maxDecimals: 0,
						controlLimits: [
						],
					}, this),
					PROPERTYPROPEDTCONST_: new fieldControlClass.DateControl({
						modelField: 'ValDtconst',
						valueChangeEvent: 'fieldChange:prope.dtconst',
						id: 'PROPERTYPROPEDTCONST_',
						name: 'DTCONST',
						size: 'small',
						label: computed(() => this.Resources.CONSTRUCTION_DATE18132),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP03',
						dateTimeType: 'date',
						controlLimits: [
						],
					}, this),
					PROPERTYPROPEBUILDAGE: new fieldControlClass.NumberControl({
						modelField: 'ValBuildage',
						valueChangeEvent: 'fieldChange:prope.buildage',
						id: 'PROPERTYPROPEBUILDAGE',
						name: 'BUILDAGE',
						size: 'small',
						label: computed(() => this.Resources.BUILDING_AGE27311),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP03',
						isFormulaBlocked: true,
						maxIntegers: 4,
						maxDecimals: 0,
						controlLimits: [
						],
					}, this),
					PROPERTYPSEUDNEWGRP04: new fieldControlClass.GroupControl({
						id: 'PROPERTYPSEUDNEWGRP04',
						name: 'NEWGRP04',
						size: 'block',
						label: computed(() => this.Resources.AGENT_INFORMATION06141),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP05',
						isInAccordion: true,
						borderless: false,
						isCollapsible: true,
						anchored: false,
						directChildren: ['PROPERTYAGENTNAME____', 'PROPERTYAGENTPHOTOGRA', 'PROPERTYAGENTEMAIL___'],
						controlLimits: [
						],
					}, this),
					PROPERTYAGENTNAME____: new fieldControlClass.LookupControl({
						modelField: 'TableAgentName',
						valueChangeEvent: 'fieldChange:agent.name',
						id: 'PROPERTYAGENTNAME____',
						name: 'NAME',
						size: 'xxlarge',
						helpControl: {
							shortHelp: {
								type: '',
								text: '',
							},
						},
						label: computed(() => this.Resources.AGENT_S_NAME42642),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP04',
						externalCallbacks: {
							getModelField: vm.getModelField,
							getModelFieldValue: vm.getModelFieldValue,
							setModelFieldValue: vm.setModelFieldValue
						},
						externalProperties: {
							modelKeys: computed(() => vm.modelKeys)
						},
						lookupKeyModelField: {
							name: 'ValCodagent',
							dependencyEvent: 'fieldChange:prope.codagent'
						},
						dependentFields: () => ({
							set 'agent.codagent'(value) { vm.model.ValCodagent.updateValue(value) },
							set 'agent.name'(value) { vm.model.TableAgentName.updateValue(value) },
							set 'agent.photography'(value) { vm.model.AgentValPhotography.updateValue(value) },
							set 'agent.email'(value) { vm.model.AgentValEmail.updateValue(value) },
						}),
						controlLimits: [
						],
					}, this),
					PROPERTYAGENTPHOTOGRA: new fieldControlClass.ImageControl({
						modelField: 'AgentValPhotography',
						valueChangeEvent: 'fieldChange:agent.photography',
						dependentModelField: 'ValCodagent',
						dependentChangeEvent: 'fieldChange:prope.codagent',
						id: 'PROPERTYAGENTPHOTOGRA',
						name: 'PHOTOGRA',
						size: 'mini',
						label: computed(() => this.Resources.PHOTOGRAPHY38058),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP04',
						height: 50,
						width: 30,
						dataTitle: computed(() => genericFunctions.formatString(vm.Resources.IMAGEM_UTILIZADA_PAR17299, vm.Resources.PHOTOGRAPHY38058)),
						maxFileSize: 10485760, // In bytes.
						maxFileSizeLabel: '10 MB',
						controlLimits: [
						],
					}, this),
					PROPERTYAGENTEMAIL___: new fieldControlClass.StringControl({
						modelField: 'AgentValEmail',
						valueChangeEvent: 'fieldChange:agent.email',
						dependentModelField: 'ValCodagent',
						dependentChangeEvent: 'fieldChange:prope.codagent',
						id: 'PROPERTYAGENTEMAIL___',
						name: 'EMAIL',
						size: 'xxlarge',
						label: computed(() => this.Resources.E_MAIL42251),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP04',
						maxLength: 80,
						controlLimits: [
						],
					}, this),
					PROPERTYPROPETAX_____: new fieldControlClass.NumberControl({
						modelField: 'ValTax',
						valueChangeEvent: 'fieldChange:prope.tax',
						id: 'PROPERTYPROPETAX_____',
						name: 'TAX',
						size: 'mini',
						label: computed(() => this.Resources.TAX37977),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP02',
						isFormulaBlocked: true,
						maxIntegers: 2,
						maxDecimals: 2,
						controlLimits: [
						],
					}, this),
					PROPERTYPROPETITLE___: new fieldControlClass.StringControl({
						modelField: 'ValTitle',
						valueChangeEvent: 'fieldChange:prope.title',
						id: 'PROPERTYPROPETITLE___',
						name: 'TITLE',
						size: 'xxlarge',
						label: computed(() => this.Resources.TITLE21885),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP01',
						maxLength: 50,
						mustBeFilled: true,
						controlLimits: [
						],
					}, this),
					PROPERTYPROPEPHOTO___: new fieldControlClass.ImageControl({
						modelField: 'ValPhoto',
						valueChangeEvent: 'fieldChange:prope.photo',
						id: 'PROPERTYPROPEPHOTO___',
						name: 'PHOTO',
						size: 'mini',
						label: computed(() => this.Resources.MAIN_PHOTO16044),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP01',
						height: 50,
						width: 30,
						dataTitle: computed(() => genericFunctions.formatString(vm.Resources.IMAGEM_UTILIZADA_PAR17299, vm.Resources.MAIN_PHOTO16044)),
						maxFileSize: 10485760, // In bytes.
						maxFileSizeLabel: '10 MB',
						controlLimits: [
						],
					}, this),
					PROPERTYPROPEDESCRIPT: new fieldControlClass.MultilineStringControl({
						modelField: 'ValDescript',
						valueChangeEvent: 'fieldChange:prope.descript',
						id: 'PROPERTYPROPEDESCRIPT',
						name: 'DESCRIPT',
						size: 'xxlarge',
						label: computed(() => this.Resources.DESCRIPTION07383),
						placeholder: '',
						labelPosition: computed(() => this.labelAlignment.topleft),
						container: 'PROPERTYPSEUDNEWGRP01',
						rows: 5,
						cols: 80,
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
					'PROPERTYPSEUDNEWGRP05',
					'PROPERTYPSEUDNEWGRP02',
					'PROPERTYPSEUDNEWGRP03',
					'PROPERTYPSEUDNEWGRP04',
					'PROPERTYPSEUDNEWGRP01',
				]),

				tableFields: readonly([
					'PROPERTYPSEUDFIELD001',
					'PROPERTYPSEUDFIELD002',
				]),

				timelineFields: readonly([
				]),

				/**
				 * The Data API for easy access to model variables.
				 */
				dataApi: {
					Agent: {
						get ValEmail() { return vm.model.AgentValEmail.value },
						set ValEmail(value) { vm.model.AgentValEmail.updateValue(value) },
						get ValName() { return vm.model.TableAgentName.value },
						set ValName(value) { vm.model.TableAgentName.updateValue(value) },
						get ValPhotography() { return vm.model.AgentValPhotography.value },
						set ValPhotography(value) { vm.model.AgentValPhotography.updateValue(value) },
					},
					City: {
						get ValCity() { return vm.model.TableCityCity.value },
						set ValCity(value) { vm.model.TableCityCity.updateValue(value) },
					},
					Count: {
						get ValCountry() { return vm.model.CityCountValCountry.value },
						set ValCountry(value) { vm.model.CityCountValCountry.updateValue(value) },
					},
					Prope: {
						get ValAverage() { return vm.model.ValAverage.value },
						set ValAverage(value) { vm.model.ValAverage.updateValue(value) },
						get ValBathnr() { return vm.model.ValBathnr.value },
						set ValBathnr(value) { vm.model.ValBathnr.updateValue(value) },
						get ValBuildage() { return vm.model.ValBuildage.value },
						set ValBuildage(value) { vm.model.ValBuildage.updateValue(value) },
						get ValBuildtyp() { return vm.model.ValBuildtyp.value },
						set ValBuildtyp(value) { vm.model.ValBuildtyp.updateValue(value) },
						get ValCodagent() { return vm.model.ValCodagent.value },
						set ValCodagent(value) { vm.model.ValCodagent.updateValue(value) },
						get ValCodcity() { return vm.model.ValCodcity.value },
						set ValCodcity(value) { vm.model.ValCodcity.updateValue(value) },
						get ValCodprope() { return vm.model.ValCodprope.value },
						set ValCodprope(value) { vm.model.ValCodprope.updateValue(value) },
						get ValDescript() { return vm.model.ValDescript.value },
						set ValDescript(value) { vm.model.ValDescript.updateValue(value) },
						get ValDtconst() { return vm.model.ValDtconst.value },
						set ValDtconst(value) { vm.model.ValDtconst.updateValue(value) },
						get ValDtsold() { return vm.model.ValDtsold.value },
						set ValDtsold(value) { vm.model.ValDtsold.updateValue(value) },
						get ValFloornr() { return vm.model.ValFloornr.value },
						set ValFloornr(value) { vm.model.ValFloornr.updateValue(value) },
						get ValGrdsize() { return vm.model.ValGrdsize.value },
						set ValGrdsize(value) { vm.model.ValGrdsize.updateValue(value) },
						get ValId() { return vm.model.ValId.value },
						set ValId(value) { vm.model.ValId.updateValue(value) },
						get ValLastvisit() { return vm.model.ValLastvisit.value },
						set ValLastvisit(value) { vm.model.ValLastvisit.updateValue(value) },
						get ValPhoto() { return vm.model.ValPhoto.value },
						set ValPhoto(value) { vm.model.ValPhoto.updateValue(value) },
						get ValPrice() { return vm.model.ValPrice.value },
						set ValPrice(value) { vm.model.ValPrice.updateValue(value) },
						get ValProfit() { return vm.model.ValProfit.value },
						set ValProfit(value) { vm.model.ValProfit.updateValue(value) },
						get ValSize() { return vm.model.ValSize.value },
						set ValSize(value) { vm.model.ValSize.updateValue(value) },
						get ValSold() { return vm.model.ValSold.value },
						set ValSold(value) { vm.model.ValSold.updateValue(value) },
						get ValTax() { return vm.model.ValTax.value },
						set ValTax(value) { vm.model.ValTax.updateValue(value) },
						get ValTitle() { return vm.model.ValTitle.value },
						set ValTitle(value) { vm.model.ValTitle.updateValue(value) },
						get ValTypology() { return vm.model.ValTypology.value },
						set ValTypology(value) { vm.model.ValTypology.updateValue(value) },
					},
					keys: {
						/** The primary key of the PROPE table */
						get prope() { return vm.model.ValCodprope },
						/** The foreign key to the AGENT table */
						get agent() { return vm.model.ValCodagent },
						/** The foreign key to the CITY table */
						get city() { return vm.model.ValCodcity },
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
// USE /[MANUAL FOR FORM_CODEJS PROPERTY]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		beforeUnmount()
		{
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR COMPONENT_BEFORE_UNMOUNT PROPERTY]/
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
// USE /[MANUAL FOR BEFORE_LOAD_JS PROPERTY]/
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
// USE /[MANUAL FOR FORM_LOADED_JS PROPERTY]/
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
// USE /[MANUAL FOR BEFORE_APPLY_JS PROPERTY]/
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
// USE /[MANUAL FOR AFTER_APPLY_JS PROPERTY]/
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
// USE /[MANUAL FOR BEFORE_SAVE_JS PROPERTY]/
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
// USE /[MANUAL FOR AFTER_SAVE_JS PROPERTY]/
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
// USE /[MANUAL FOR BEFORE_DEL_JS PROPERTY]/
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
// USE /[MANUAL FOR AFTER_DEL_JS PROPERTY]/
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
// USE /[MANUAL FOR BEFORE_EXIT_JS PROPERTY]/
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
// USE /[MANUAL FOR AFTER_EXIT_JS PROPERTY]/
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
// USE /[MANUAL FOR DLGUPDT PROPERTY]/
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
// USE /[MANUAL FOR CTRLBLR PROPERTY]/
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
// USE /[MANUAL FOR CTRLUPD PROPERTY]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

				this.afterControlUpdate(controlField, fieldValue)
			},
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR FUNCTIONS_JS PROPERTY]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
		},

		watch: {
		}
	}
</script>
