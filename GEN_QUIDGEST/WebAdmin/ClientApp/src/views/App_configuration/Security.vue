<template>
	<div id="app_config_security_container">
		<row>
			<q-card
				class="q-card--admin-default"
				:title="hardcodedTexts.authentication"
				width="block">
				<q-row-container>
					<q-select
						v-model="Security.AuthenticationMode"
						v-if="SelectLists"
						size="large"
						:items="SelectLists.AuthenticationMode"
						:label="resources.authenticationMode"
						item-value="Value"
						item-label="Text" />
					<q-select
						v-model="Security.AllowMultiSessionPerUser"
						v-if="SelectLists"
						size="large"
						:items="SelectLists.MultisessionMode"
						:label="resources.concurrentSessionsPolicy"
						item-value="Value"
						item-label="Text" />
					<q-checkbox
						v-model="Security.AllowAuthenticationRecovery"
						:label="resources.allowAuthenticationRecovery" />
					<q-checkbox
						v-model="Security.Mandatory2FA"
						:label="resources.mandatoryTwoFactorAuth" />
					<numeric-input
						v-model="Security.SessionTimeOut"
						size="large"
						:label="resources.sessionTimeout">
					</numeric-input>
				</q-row-container>
			</q-card>
		</row>

		<row>
			<q-card
				class="q-card--admin-default"
				:title="resources.passwordPolicy"
				width="block">
				<q-row-container>
					<numeric-input
						v-model="Security.MinCharacters"
						size="large"
						:label="resources.minCharacters">
					</numeric-input>
					<q-select
						v-model="Security.PasswordStrength"
						v-if="SelectLists"
						size="large"
						:items="SelectLists.PasswordStrength"
						:label="resources.authenticationMode"
						item-value="Value"
						item-label="Text" />
					<numeric-input
						v-model="Security.MaxAttempts"
						size="large"
						:label="resources.maxLoginAttempts">
					</numeric-input>
					<q-checkbox
						v-model="Security.ExpirationDateBool"
						:label="resources.passwordExpirationDays" />
					<q-text-field
						v-model="Security.ExpirationDate"
						size="large"
						:label="resources.daysToExpiration" />
					<q-select
						v-model="Security.PasswordAlgorithms"
						v-if="SelectLists"
						size="large"
						:items="SelectLists.PasswordAlgorithms"
						:label="resources.encryptionAlgorithm"
						item-value="Value"
						item-label="Text" />
					<q-checkbox
						v-model="Security.UsePasswordBlacklist"
						:label="resources.usePasswordBlacklist" />
					<q-button
						v-if="Security.UsePasswordBlacklist"
						:label="resources.managePasswordBlacklist"
						@click="showManageBlacklist" />
				</q-row-container>
			</q-card>
		</row>

		<row class="footer-btn">
			<q-button
				variant="bold"
				:label="hardcodedTexts.saveConfiguration"
				@click="SaveConfigSecurity" />
		</row>

		<hr />

		<row>
			<qtable
				:rows="identityProvidersRows"
				:columns="tIdentityProviders.columns"
				:config="tIdentityProviders.config"
				:totalRows="tIdentityProviders.total_rows"
				class="q-table--borderless">

				<template #actions="props">
					<q-button-group borderless>
						<q-button
							variant="text"
							:title="hardcodedTexts.edit"
							@click="changeIdentityProvider(props.row)">
							<q-icon icon="pencil" />
						</q-button>
						<q-button
							variant="text"
							:title="hardcodedTexts.delete"
							@click="deleteIdentityProvider(props.row)">
							<q-icon icon="bin" />
						</q-button>
					</q-button-group>
				</template>
				<template #table-footer>
					<tr>
						<td colspan="4">
							<q-button
								:label="hardcodedTexts.insert"
								@click="createIdentityProvider">
								<q-icon icon="add" />
							</q-button>
						</td>
					</tr>
				</template>
			</qtable>
		</row>

		<row>
			<qtable
				:rows="roleRows"
				:columns="tRoleProviders.columns"
				:config="tRoleProviders.config"
				:totalRows="tRoleProviders.total_rows"
				class="q-table--borderless">

				<template #actions="props">
					<q-button-group borderless>
						<q-button
							variant="text"
							:title="hardcodedTexts.edit"
							@click="changeRoleProvider(props.row)">
							<q-icon icon="pencil" />
						</q-button>
						<q-button
							variant="text"
							:title="hardcodedTexts.delete"
							@click="deleteRoleProvider(props.row)">
							<q-icon icon="bin" />
						</q-button>
					</q-button-group>
				</template>
				<template #table-footer>
					<tr>
						<td colspan="5">
							<q-button
								:label="hardcodedTexts.insert"
								@click="createRoleProvider">
								<q-icon icon="add" />
							</q-button>
						</td>
					</tr>
					<tr>
						<td colspan="5">
							<q-button :label="hardcodedTexts.executeMaintenanceTasks"
									  @click="setupProviders">
								<q-icon icon="tools" />
							</q-button>
						</td>
					</tr>
				</template>
			</qtable>
		</row>

		<hr />

		<row>
			<qtable
				:rows="userRows"
				:columns="tUsers.columns"
				:config="tUsers.config"
				:totalRows="tUsers.total_rows"
				class="q-table--borderless">

				<template #actions="props">
					<q-button-group borderless>
						<q-button
							variant="text"
							:title="hardcodedTexts.edit"
							@click="changeUser(props.row)">
							<q-icon icon="pencil" />
						</q-button>
						<q-button
							variant="text"
							:title="hardcodedTexts.delete"
							@click="deleteUser(props.row)">
							<q-icon icon="bin" />
						</q-button>
					</q-button-group>
				</template>
				<template #Type="props">
					<!-- This is a horrible and temporary solution, needs refactor -->
					{{ SelectLists.DisplayUserType.filter((t) => t.Text == props.row.Type)[0] }}
				</template>
				<template #AutoLogin="props">
					<q-icon
						v-if="props.row.AutoLogin"
						icon="check" />
					<q-icon
						v-else
						icon="close" />
				</template>
				<template #table-footer>
					<tr>
						<td colspan="4">
							<q-button
								:label="hardcodedTexts.insert"
								@click="createUser">
								<q-icon icon="add" />
							</q-button>
						</td>
					</tr>
				</template>
			</qtable>
		</row>

		<q-dialog
			id="manage_blacklist"
			v-model="showBlacklistDialog"
			:title="resources.managePasswordBlacklist"
			:buttons="buttonsBlacklist">
			<template #body.content>
				<div class="q-dialog-container">
					<q-alert
						v-if="alert.isVisible"
						ref="alertBox"
						:type="alert.alertType"
						:text="alert.message"
						:icon="alert.icon"
						:title="resources.operationStatus"
						:dismissTime="5"
						@message-dismissed="handleAlertDismissed" />
					<div>{{ resources.blacklistedPasswordsInDb }}: {{ numPasswords }}</div>
					<row>
						<div class="q-button-container">
							<input
								type="file"
								id="blacklistFile"
								@change="importB"
								accept=".txt"
								style="position:absolute;height: 0;width: 0;" />
							<q-button
								variant="bold"
								:label="hardcodedTexts.import"
								@click="clickImport" />
							<q-button
								variant="bold"
								:label="hardcodedTexts.export"
								@click="exportB" />
						</div>
					</row>
					<div>{{ resources.deleteAllBlacklistedPasswords }}</div>
					<row>
						<q-button
							variant="bold"
							color="danger"
							:label="hardcodedTexts.erase"
							@click="deleteAll">
							<q-icon icon="bin" />
						</q-button>
					</row>
					<row>
						<password-input
							v-model="password"
							class="control-row-group"
							:label="hardcodedTexts.password" />
						<div class="control-row-group q-button-container">
							<q-button
								variant="bold"
								:label="hardcodedTexts.validation"
								@click="passCheck" />
							<q-button
								:label="hardcodedTexts.add"
								@click="passAdd" />
						</div>
					</row>

					<row>
						<div>Validate service passwords</div>
						<div class="control-row-group q-button-container">
							<q-button
								variant="bold"
								:label="hardcodedTexts.validation"
								@click="servicePassCheck" />
						</div>
						<div>
							<div v-for="item in servicePassResults" class="alert alert-warning">
								<span>
									<b class="status-message">{{ item }}</b>
								</span>
							</div>
						</div>
					</row>
				</div>
			</template>
		</q-dialog>

		<q-dialog
			id="identity_provider"
			v-model="showIdentityDialog"
			:title="resources.identityProvider"
			:buttons="buttons">
			<template #body.content>
				<div class="q-dialog-container">
					<q-text-field
						v-model="rowName"
						:label="hardcodedTexts.name"
						required
						:readonly="inDeleteMode"
						size="large" />
					<q-text-field
						v-model="rowDescription"
						:label="hardcodedTexts.description"
						:readonly="inDeleteMode"
						size="large" />
					<base-input-structure
						:label="hardcodedTexts.taskTypeLabel"
						:isVisible="true"
						:showPopoverButton="true"
						:popoverTitle="hardcodedTexts.taskTypeLabel"
						:popoverText="providerHelp">
						<q-select
							v-model="rowType"
							v-if="SelectLists"
							:items="identityProviderSelect"
							size="large"
							:readonly="inDeleteMode"
							item-value="Value"
							item-label="Text" />
					</base-input-structure>
					<q-checkbox v-model="rowIs2fa"
								label="2FA"
								:readonly="inDeleteMode" />
					<div v-for="c in tempConfig" :key="c.PropertyName">
						<base-input-structure
							:label="c.DisplayName"
							:id="c.DisplayName"
							:isVisible="true"
							:isRequired="!c.Optional"
							:showPopoverButton="true"
							:popoverTitle="c.DisplayName"
							:popoverText="c.Description">
							<q-text-field
								v-model="c.PValue"
								size="large"
								:readonly="inDeleteMode"
								:required="!c.Optional" />
						</base-input-structure>
					</div>
				</div>
			</template>
		</q-dialog>

		<q-dialog
			v-model="showRoleDialog"
			:title="resources.roleProvider"
			:buttons="buttons">
			<template #body.content>
				<div class="q-dialog-container">
					<q-text-field
						v-model="roleName"
						:label="hardcodedTexts.name"
						required
						:readonly="inDeleteMode"
						size="large" />
					<base-input-structure
						:label="hardcodedTexts.taskTypeLabel"
						:isVisible="true"
						:showPopoverButton="true"
						:popoverTitle="hardcodedTexts.taskTypeLabel"
						:popoverText="providerRoleHelp">
						<q-select
							v-model="roleType"
							:items="roleProviderSelect"
							item-value="Value"
							item-label="Text"
							:readonly="inDeleteMode"
							size="large" />
					</base-input-structure>
					<div v-for="c in tempRoleConfig" :key="c.PropertyName">
						<base-input-structure
							:label="c.DisplayName"
							:id="c.DisplayName"
							:isVisible="true"
							:isRequired="!c.Optional"
							:showPopoverButton="true"
							:popoverTitle="c.DisplayName"
							:popoverText="c.Description">
							<q-text-field
								v-model="c.PValue"
								:required="!c.Optional"
								:readonly="inDeleteMode"
								size="large" />
						</base-input-structure>
					</div>
				</div>
			</template>
		</q-dialog>

		<q-dialog
			v-model="showUserDialog"
			:title="resources.fixedUser"
			:buttons="buttons">
			<template #body.content>
				<div class="q-dialog-container">
					<q-text-field
						v-model="userName"
						:class="{ 'input-error' : isSameName }"
						required
						:label="hardcodedTexts.name"
						:readonly="dialogMode != 'new'"
						size="large">
						<template #extras v-if="isSameName">
							<q-icon icon="information-outline" />
							{{ resources.thisNameAlreadyExists }}
						</template>
					</q-text-field>
					<q-select
						v-model="userType"
						required
						v-if="SelectLists"
						:label="hardcodedTexts.taskTypeLabel"
						:items="SelectLists.DisplayUserType"
						item-value="Value"
						item-label="Text"
						:readonly="inDeleteMode"
						size="large" />
					<q-checkbox
						v-model="userAutoLogin"
						:label="resources.autoLogin"
						:readonly="inDeleteMode" />
					<password-input
						v-model="userPassword"
						:label="hardcodedTexts.password"
						:isReadOnly="inDeleteMode"
						:size="'large'">
					</password-input>
				</div>
			</template>
		</q-dialog>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin, ReadProviderConfig, WriteProviderConfig } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import { reactive, computed } from 'vue';
	import QAlert from '@/components/QAlert.vue';
	import { texts } from '@/resources/hardcodedTexts.ts';

	export default {
		name: 'security',
		components: { QAlert },
		emits: ['update-model', 'alert-class'],
		props: {
			model: {
				required: true
			},
			SelectLists: {
				required: true
			},
			resources: {
				type: Object,
				required: true
			}
		},
		mixins: [reusableMixin],
		data() {
			return {
				dialogMode: '',
				numPasswords: 0,
				password: '',
				resultMsg: '',
				statusError: false,
				servicePassResults: [],
				showBlacklistDialog: false,
				buttonsBlacklist: [],
				buttons: [],
				showIdentityDialog: false,
				identityProvidersRows: [],
				rowName: "",
				rowDescription: "",
				rowType: "",
				rowIs2fa: false,
				tempConfig: [],
				showUserDialog: false,
				userRows: [],
				userName: "",
				userType: "",
				userAutoLogin: false,
				userPassword: "",
				userNum: 0,
				tempRoleConfig: [],
				showRoleDialog: false,
				roleRows: [],
				roleNum: 0,
				roleName: "",
				roleType: "",
				rolePrecond: "",
				temp: {},
				alert: {
					isVisible: false,
					alertType: 'info',
					message: ''
				},
				tIdentityProviders: {
					rows: [],
					columns: [
					{
						label: this.resources.actions,
						name: "actions",
						slot_name: "actions",
						sort: false,
						column_classes: "thead-actions",
						row_text_alignment: 'text-center',
						column_text_alignment: 'text-center'
					},
					{
						label: computed(() => this.Resources[texts.name]),
						name: "Name",
						sort: true,
						initial_sort: true,
						initial_sort_order: "asc"
					},
					{
						label: computed(() => this.Resources[texts.type]),
						name: "Type",
						sort: true
					},
					{
						label: '2FA',
						name: "Is2FA",
						sort: false
					},
					{
						label: computed(() => this.Resources[texts.configuration]),
						name: "Options",
						sort: false
					}],
					config: {
						table_title: this.resources.identityProvidersTitle,
						pagination: false,
						pagination_info: false,
						global_search: {
							visibility: false
						}
					}
				},
				tRoleProviders: {
					rows: [],
					columns: [
					{
						label: this.resources.actions,
						name: "actions",
						slot_name: "actions",
						sort: false,
						column_classes: "thead-actions",
						row_text_alignment: 'text-center',
						column_text_alignment: 'text-center'
					},
					{
						label: computed(() => this.Resources[texts.name]),
						name: "Name",
						sort: true,
						initial_sort: true,
						initial_sort_order: "asc"
					},
					{
						label: computed(() => this.Resources[texts.type]),
						name: "Type",
						sort: true
					},
					{
						label: computed(() => this.Resources[texts.configuration]),
						name: "Options",
						sort: false
					}],
					config: {
						table_title: this.resources.roleProvidersTitle,
						pagination: false,
						pagination_info: false,
						global_search: {
							visibility: false
						}
					}
				},
				tUsers: {
					rows: [],
					columns: [
						{
							label: this.resources.actions,
							name: "actions",
							slot_name: "actions",
							sort: false,
							column_classes: "thead-actions",
							row_text_alignment: 'text-center',
							column_text_alignment: 'text-center'
						},
						{
							label: computed(() => this.Resources[texts.name]),
							name: "Name",
							sort: true,
							initial_sort: true,
							initial_sort_order: "asc"
						},
						{
							label: computed(() => this.Resources[texts.type]),
							name: "Type",
							slot_name: 'Text',
							sort: true
						},
						{
							label: this.resources.autoLogin,
							name: "AutoLogin",
							slot_name: 'AutoLogin',
							sort: true
						}],
					config: {
						table_title: this.resources.fixedUsersTitle,
						pagination: false,
						pagination_info: false,
						global_search: {
							visibility: false
						}
					}
				}
			};
		},
		computed: {
			isSameName() {
				return this.userRows.some(prop => prop.Name.toLowerCase() === this.userName.toLowerCase()) && this.dialogMode === 'new'
			},
			invalidUserProps() {
				return this.userName === '' || this.userType === '' || (this.dialogMode === 'new' && this.isSameName)
			},
			invalidIdentityProps() {
				return this.rowName === '' || this.rowType === '' || this.tempConfig.some(c => c.Optional === false && (!c.PValue || c.PValue.trim() === ''))
			},
			invalidRoleProps() {
				return this.roleName === '' || this.roleType === '' || this.tempRoleConfig.some(c => c.Optional === false && (!c.PValue || c.PValue.trim() === ''))
			},
			inDeleteMode() {
				return this.dialogMode === 'delete';
			},
			Security() {
				return reactive(!$.isEmptyObject(this.currentApp) && !$.isEmptyObject(this.model) ? (this.model[this.currentApp] || {}) : {});
			},
			identityProviderSelect() {
				return this.SelectLists.IdentityProviderTypeList.map(x => ({
					Text: x.DisplayName,
					Value: x.TypeFullName
				}));
			},
			providerHelp() {
				return this.SelectLists.IdentityProviderTypeList.find(x => x.TypeFullName == this.rowType)?.Description
			},
			roleProviderSelect() {
				return this.SelectLists.RoleProviderTypeList.map(x => ({
					Text: x.DisplayName,
					Value: x.TypeFullName
				}));
			},
			providerRoleHelp() {
				return this.SelectLists.RoleProviderTypeList.find(x => x.TypeFullName == this.roleType)?.Description
			},
			hardcodedTexts() {
				return {
					insert: this.Resources[texts.insert],
					edit: this.Resources[texts.edit],
					delete: this.Resources[texts.delete],
					name: this.Resources[texts.name],
					taskTypeLabel: this.Resources[texts.taskTypeLabel],
					description: this.Resources[texts.description],
					password: this.Resources[texts.password],
					saveConfiguration: this.Resources[texts.saveConfiguration],
					save: this.Resources[texts.save],
					cancel: this.Resources[texts.cancel],
					import: this.Resources[texts.import],
					export: this.Resources[texts.export],
					validation: this.Resources[texts.validation],
					add: this.Resources[texts.add],
					erase: this.Resources[texts.erase],
					changesSavedSuccess: this.Resources[texts.changesSavedSuccess],
					passwordVulnerableToKnownPasswords: this.Resources[texts.passwordVulnerableToKnownPasswords],
					securityLabel: this.Resources[texts.securityLabel],
					pathsLabel: this.Resources[texts.pathsLabel],
					configuration: this.Resources[texts.configuration],
					roleProvidersTitle: this.Resources[texts.roleProvidersTitle],
					fixedUsersTitle: this.Resources[texts.fixedUsersTitle],
					authentication: this.Resources[texts.authentication],
					executeMaintenanceTasks: this.Resources[texts.executeMaintenanceTasks]
				};
			},
		},
		methods: {
			SaveConfigSecurity() {
				QUtils.log("SaveConfigSecurity - Request", QUtils.apiActionURL('Config', 'SaveConfigSecurity'));
				QUtils.postData('Config', 'SaveConfigSecurity', this.Security, null, (data) => {
					QUtils.log("SaveConfigSecurity - Response", data);
					if (data.Success) {
						this.$emit('alert-class', { ResultMsg: this.hardcodedTexts.changesSavedSuccess, AlertType: 'success' });
						this.statusError = false;
					} else {
						this.$emit('alert-class', { ResultMsg: data.Message, AlertType: 'danger' });
						this.statusError = true;
					}
				});
			},
			clickImport() {
				const elem = document.getElementById('blacklistFile');
				elem.click();
			},
			async importB(e) {

				let selection = e.target.files || e.dataTransfer.files;
				if (!selection.length)
					return;

				const formData = new FormData();
				const file = selection[0];
				formData.append("file", file);

				this.resultMsg = "";
				this.statusError = false;

				const uri = QUtils.apiActionURL('Config', 'BlacklistUpload');
				const response = await fetch(uri, {
					method: "POST",
					body: formData,
				});

				if(response.ok)
				{
					const data = await response.json();
					if (data.Success) {
						this.resultMsg = this.hardcodedTexts.changesSavedSuccess;
						this.statusError = false;
						this.numPasswords = data.numPasswords;
					} else {
						this.resultMsg = data.Message;
						this.statusError = true;
					}
				}
			},
			exportB() {
				var downloadUrl = QUtils.apiActionURL('Config', 'BlacklistDownload');
				window.open(downloadUrl, "_self")
			},
			passCheck() {
				const params = {
					password: this.password
				};
				this.resultMsg = "";
				this.statusError = false;

				QUtils.postData('Config', 'BlacklistPasswordCheck', params, null, (data) => {
					if (data.Success) {
						if(data.found) {
							this.setAlert('danger', {ResultMsg: this.hardcodedTexts.passwordVulnerableToKnownPasswords });
						} else {
							this.setAlert('success', "ok");
						}
					} else {
						this.setAlert('danger', {ResultMsg: data.Message})
					}
				});
			},
			servicePassCheck() {
				this.resultMsg = "";
				this.statusError = false;
				this.servicePassResults = [];

				QUtils.postData('Config', 'ServicePasswordCheck', {}, null, function (data) {
					if (data.Success) {
						if(data.resultList && data.resultList.length > 0) {
							this.servicePassResults = data.resultList;
						} else {
							this.resultMsg = "ok";
							this.statusError = false;
						}
					} else {
						this.resultMsg = data.Message;
						this.statusError = true;
					}
				});
			},
			passAdd() {
				this.resultMsg = "";
				this.statusError = false;

				const params = {
					password: this.password
				};
				QUtils.postData('Config', 'BlacklistPasswordAdd', params, null, function (data) {
					if (data.Success) {
						this.resultMsg = this.hardcodedTexts.changesSavedSuccess;
						this.statusError = false;
						this.numPasswords = data.numPasswords;
					} else {
						this.resultMsg = data.Message;
						this.statusError = true;
					}
				});
			},
			deleteAll() {
				this.resultMsg = "";
				this.statusError = false;
				QUtils.postData('Config', 'BlacklistPasswordClear', {}, null, function (data) {
					if (data.Success) {
						this.resultMsg = this.hardcodedTexts.changesSavedSuccess;
						this.statusError = false;
						this.numPasswords = data.numPasswords;
					} else {
						this.resultMsg = data.Message;
						this.statusError = true;
					}
				});
			},
			showManageBlacklist() {
				this.getbuttonsBlacklist()
				this.showBlacklistDialog = true;
			},
			getbuttonsBlacklist() {
				this.buttonsBlacklist.push({
					id: 'cancel-btn',
					props: {
						label: this.hardcodedTexts.cancel
					},
					action: () => {
						this.buttonsBlacklist = [],
						this.password = ''
					}
				})
			},
			updateAlert(data) {
				this.Model.ResultMsg = data.ResultMsg;
				if (data.AlertType) {
				this.setAlert(data.AlertType, data.ResultMsg);
				} else {
					this.setAlert('info', data.ResultMsg);
				}
			},
			handleConnectionTested(result) {
				if (result.Success) {
					this.setAlert('success', 'Connection success');
				} else {
					this.setAlert('danger', result.message || 'Connection failed');
				}
			},
			setAlert(type, message) {
				this.alert.isVisible = true;
				this.alert.alertType = type;
				this.alert.message = message;

				this.$nextTick(() => {
					if (this.$refs.alertBox) {
						this.$refs.alertBox.$el.scrollIntoView({ behavior: 'smooth' });
					}
				});
			},
			handleAlertDismissed() {
				this.alert.isVisible = false;
			},
			getButtonsDialog(dialogType) {
				let isDisabled

				if (dialogType === 'userDialog') {
					isDisabled = this.invalidUserProps;
				} else if (dialogType === 'identityDialog') {
					isDisabled = this.invalidIdentityProps;
				} else if (dialogType === 'roleDialog') {
					isDisabled = this.invalidRoleProps;
				} else {
					isDisabled = false;
				}
				switch(this.dialogMode) {
					case 'delete':
						this.buttons.push({
							id: 'delete-btn',
							props: {
								label: this.hardcodedTexts.erase,
								variant: 'bold',
								color: "danger"
							},
							action: () => {
								if (dialogType === 'userDialog') {
									this.SaveUserCfg()
								} else if (dialogType === 'identityDialog') {
									this.SaveIdentityProvider()
								} else {
									this.SaveRoleProvider()
								}
							}
						});
						break;
					case 'edit':
					case 'new':
						this.buttons.push({
							id: 'save-btn',
							props: {
								label: this.hardcodedTexts.save,
								variant: 'bold',
								disabled: isDisabled
							},
							action: () => {
								if (dialogType === 'userDialog') {
									this.SaveUserCfg()
								} else if (dialogType === 'identityDialog') {
									this.SaveIdentityProvider()
								}
								else {
									this.SaveRoleProvider()
								}
							}
						});
						break;
					default:
						break;
					}

				this.buttons.push({
					id: 'cancel-btn',
					props: {
						label: this.hardcodedTexts.cancel
					},
					action: () => {
						if (dialogType === 'userDialog') {
							this.clearUserCfg()
						} else if (dialogType === 'identityDialog') {
							this.clearIdentityProviderValues()
						}
						else {
							this.clearRoleProvider()
						}
					}
				})
			},

			SaveIdentityProvider() {
				let config = WriteProviderConfig(this.tempConfig, this.rowType, this.SelectLists.IdentityProviderTypeList);
				const idProValues = {
					Name: this.rowName,
					Description: this.rowDescription,
					Is2FA: this.rowIs2fa,
					Type: this.rowType,
					Options: config,
					FormMode: this.dialogMode,
					Rownum: this.rowNum
				}
				QUtils.postData('Config', 'SaveIdentityProvider', idProValues, { appId: this.$store.state.currentApp }, (data) => {
					if (data.success) {
						switch (idProValues.FormMode) {
							case 'new':
								this.identityProvidersRows.push(
									{
										FormMode: this.dialogMode,
										Name: this.rowName,
										Description: this.rowDescription,
										Is2FA: this.rowIs2fa,
										Type: this.rowType,
										Options: config,
										Rownum: this.identityProvidersRows.length
									}
								)
								break;
							case 'edit':
								const newPropIndex = this.identityProvidersRows.findIndex(value => value.Rownum == this.rowNum)
								this.identityProvidersRows[newPropIndex].Type = this.rowType;
								this.identityProvidersRows[newPropIndex].Name = this.rowName;
								this.identityProvidersRows[newPropIndex].Description = this.rowDescription;
								this.identityProvidersRows[newPropIndex].Is2FA = this.rowIs2fa;
								this.identityProvidersRows[newPropIndex].Options = config;
								this.identityProvidersRows[newPropIndex].Rownum = this.rowNum;
								break;
							case 'delete':
								this.identityProvidersRows = this.identityProvidersRows.filter(prop => prop.Name != this.rowName).sort((a, b) => a.rowNum - b.rowNum);
								this.identityProvidersRows.forEach((identityProvidersRows, idx) => {
									identityProvidersRows.Rownum = idx
								})
								break;
							default:
								break;
						}
						this.clearIdentityProviderValues()
						// Update model data
						this.$emit('update-model')
					}
				});
			},
			clearIdentityProviderValues(){
				this.dialogMode = '';
				this.rowType = '';
				this.rowName = '';
				this.rowDescription = '';
				this.rowIs2fa = false;
				this.tempConfig = [];
				this.buttons = [];
			},
			showIdentityProviderModal(mode) {
				this.dialogMode = mode;
				this.getButtonsDialog('identityDialog');
				this.showIdentityDialog = true;
			},
			changeIdentityProvider(identityProvidersRows) {
				this.rowName = identityProvidersRows.Name;
				this.rowDescription = identityProvidersRows.Description;
				this.rowIs2fa = identityProvidersRows.Is2FA;
				this.rowType = identityProvidersRows.Type;
				this.rowNum = identityProvidersRows.Rownum;

				this.tempConfig = ReadProviderConfig(this.rowType, identityProvidersRows.Options, this.SelectLists.IdentityProviderTypeList);

				this.showIdentityProviderModal('edit');
			},
			deleteIdentityProvider(identityProvidersRows) {
				this.rowName = identityProvidersRows.Name;
				this.rowDescription = identityProvidersRows.Description;
				this.rowIs2fa = identityProvidersRows.Is2FA;
				this.rowType = identityProvidersRows.Type;
				this.rowNum = identityProvidersRows.Rownum;

				this.tempConfig = ReadProviderConfig(this.rowType, identityProvidersRows.Options, this.SelectLists.IdentityProviderTypeList);

				this.showIdentityProviderModal('delete');
			},
			createIdentityProvider() {
				this.showIdentityProviderModal('new');
			},
			SaveUserCfg() {
				const userValues = {
					Name: this.userName,
					Type: this.userType,
					AutoLogin: this.userAutoLogin,
					Password: this.userPassword,
					FormMode: this.dialogMode,
					Rownum: this.userNum
				}
				QUtils.postData('Config', 'SaveUserCfg', userValues, null, (data) => {
					if (data.success) {
						switch (userValues.FormMode) {
							case 'new':
								this.userRows.push(data.users);
							break;
							case 'edit':
								const newUserRowsIndex = this.userRows.findIndex(value => value.Rownum == this.userNum)
								this.userRows[newUserRowsIndex].Type = this.userType;
								this.userRows[newUserRowsIndex].AutoLogin = this.userAutoLogin;
								this.userRows[newUserRowsIndex].Password = this.userPassword;
								this.userRows[newUserRowsIndex].Name = this.userName;
								break;
							case 'delete':
								this.userRows = this.userRows.filter(prop => prop.Name != this.userName).sort((a, b) => a.userNum - b.userNum);
								this.userRows.forEach((userRows, idx) => {
									userRows.Rownum = idx
								})
								break;
							default:
							break;
						}
					}
					else {
						this.$emit('alert-class', { ResultMsg: data.Message, AlertType: 'danger' });
					}

					this.clearUserCfg()
					// Update model data
					this.$emit('update-model')
				});
			},
			typeMapping(userType) {
				const typeMapping = {
					'Regular': 'Normal',
					'Guest': 'Guest',
					'Admin': 'Administrator'
				}
				return typeMapping[userType]
			},
			clearUserCfg(){
				this.dialogMode = '',
				this.userName = '',
				this.userType = '',
				this.userAutoLogin = false,
				this.userPassword = '',
				this.buttons = []
			},
			showUserModal(mode) {
				this.dialogMode = mode;
				this.getButtonsDialog('userDialog');
				this.showUserDialog = true;
			},
			changeUser(userRows) {
				const mappedType = this.typeMapping(userRows.Type)
				const userTypeObj = this.SelectLists.DisplayUserType.find(item => item.Text === mappedType);
				this.userName = userRows.Name
				this.userType = userTypeObj.Value
				this.userAutoLogin = userRows.AutoLogin
				this.userPassword = userRows.Password
				this.showUserModal('edit');
			},
			deleteUser(userRows) {
				const mappedType = this.typeMapping(userRows.Type)
				const userTypeObj = this.SelectLists.DisplayUserType.find(item => item.Text === mappedType);
				this.userName = userRows.Name
				this.userType = userTypeObj.Value
				this.userAutoLogin = userRows.AutoLogin
				this.userPassword = userRows.Password
				this.showUserModal('delete');
			},
			createUser() {
				this.showUserModal('new');
			},
			SaveRoleProvider() {
				let roleConfig = WriteProviderConfig(this.tempRoleConfig, this.roleType, this.SelectLists.RoleProviderTypeList);
				const roleValues = {
					Name: this.roleName,
					Type: this.roleType,
					Precond: this.rolePrecond,
					Options: roleConfig,
					FormMode: this.dialogMode,
					Rownum: this.roleNum
				}
				QUtils.postData('Config', 'SaveRoleProvider', roleValues, { appId: this.$store.state.currentApp }, (data) => {
					if (data.success) {
						switch (roleValues.FormMode) {
							case 'new':
								this.roleRows.push(
									{
										Name: this.roleName,
										Type: this.roleType,
										Precond: this.rolePrecond,
										Options: roleConfig,
										FormMode: this.dialogMode,
										Rownum: this.roleRows.length
									}
								)
							break;
							case 'edit':
								const newRoleRowsIndex = this.roleRows.findIndex(value => value.Rownum == this.roleNum)
								this.roleRows[newRoleRowsIndex].Type = this.roleType;
								this.roleRows[newRoleRowsIndex].Precond = this.rolePrecond;
								this.roleRows[newRoleRowsIndex].Options = roleConfig;
								this.roleRows[newRoleRowsIndex].Name = this.roleName;
								break;
							case 'delete':
								this.roleRows = this.roleRows.filter(prop => prop.Name != this.roleName).sort((a, b) => a.roleNum - b.roleNum);
								this.roleRows.forEach((roleRows, idx) => {
									roleRows.Rownum = idx
								})
								break;
							default:
								break;
						}
					}
					else {
						this.$emit('alert-class', { ResultMsg: data.Message, AlertType: 'danger' });
						}

					this.clearRoleProvider()
					// Update model data
					this.$emit('update-model')
				});
			},
			clearRoleProvider() {
				this.dialogMode = '',
				this.roleName = '',
				this.roleType = '',
				this.rolePrecond = '',
				this.tempRoleConfig = [],
				this.buttons = []
			},
			showRoleProviderModal(mode) {
				this.dialogMode = mode;
				this.getButtonsDialog("roleDialog");
				this.showRoleDialog = true;
			},
			changeRoleProvider(roleRows) {
				this.roleName = roleRows.Name
				this.roleType = roleRows.Type
				this.rolePrecond = roleRows.Precond
				this.tempRoleConfig = ReadProviderConfig(roleRows.Type, roleRows.Options, this.SelectLists.RoleProviderTypeList);
				this.showRoleProviderModal('edit');
			},
			deleteRoleProvider(roleRows) {
				this.roleName = roleRows.Name
				this.roleType = roleRows.Type
				this.rolePrecond = roleRows.Precond
				this.tempRoleConfig = ReadProviderConfig(roleRows.Type, roleRows.Options, this.SelectLists.RoleProviderTypeList);
				this.showRoleProviderModal('delete');
			},
			createRoleProvider() {
				this.showRoleProviderModal('new');
			},
			setupProviders() {
				QUtils.postData('Config', 'SetupProviders', {}, { appId: this.$store.state.currentApp }, (data) => {
					if (data.success) {
						this.$emit('alert-class', { ResultMsg: "ok", AlertType: 'success' });
					}
					else {
						this.$emit('alert-class', { ResultMsg: data.Message, AlertType: 'danger' });
					}
				});
			},
		},
		created() {
			const url = QUtils.apiActionURL('Config', 'ManagePasswordBlacklist');
			QUtils.FetchData(url).done(function (data) {
				this.numPasswords = data.numPasswords;
			});
		},
		updated() {
			this.userRows = this.Security.Users || [];
			this.identityProvidersRows = this.Security.IdentityProviders || [];
			this.roleRows = this.Security.RoleProviders || [];
		},
		mounted() {
			this.userRows = this.Security.Users || [];
			this.identityProvidersRows = this.Security.IdentityProviders || [];
			this.roleRows = this.Security.RoleProviders || [];
		},
		watch: {
			invalidUserProps(newValue) {
				if (this.buttons.length > 0)
					this.buttons[0].props.disabled = newValue
			},
			invalidIdentityProps(newValue) {
				if (this.buttons.length > 0)
					this.buttons[0].props.disabled = newValue
			},
			invalidRoleProps(newValue) {
				if (this.buttons.length > 0)
					this.buttons[0].props.disabled = newValue
			},
			rowType(newValue) {
				if (newValue && this.showIdentityDialog) {
					//if the dialog is open and the provider type changes, try to match up the current options with the new type list of properties
					const propIndex = this.identityProvidersRows.findIndex(value => value.Rownum == this.rowNum);
					this.tempConfig = ReadProviderConfig(newValue, this.identityProvidersRows[propIndex].Options, this.SelectLists.IdentityProviderTypeList);
				}
			},
			roleType(newValue) {
				if (newValue && this.showRoleDialog) {
					//if the dialog is open and the provider type changes, try to match up the current options with the new type list of properties
					const propIndex = this.roleRows.findIndex(value => value.Rownum == this.roleNum);
					this.tempRoleConfig = ReadProviderConfig(newValue, this.roleRows[propIndex].Options, this.SelectLists.RoleProviderTypeList);
				}
			}
		}
	};
</script>
