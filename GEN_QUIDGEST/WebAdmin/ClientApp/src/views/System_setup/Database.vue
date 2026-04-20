<template>
	<div id="system_setup_database_container">
		<row>
			<q-card
				class="q-card--admin-default"
				:title="resources.currentDataSystemTitle"
				width=block>
				<q-row-container>
					<q-text-field
						v-model="model.Server"
						:label="resources.serverName"
						required
						size="xlarge"
						:isReadOnly="isTestingConnection">
						<template #extras>
							<q-icon icon="information-outline" />
							{{ resources.serverNameInfo }}
						</template>
					</q-text-field>
					<numeric-input
						:model-value="portValue"
						size="xlarge"
						:label="hardcodedTexts.port"
						:isReadOnly="isTestingConnection"
						@update:model-value="updateSystemPort">
					</numeric-input>
					<q-select
						v-model="model.ServerType"
						v-if="model.SelectLists"
						:items="model.SelectLists.DBMS"
						:label="resources.databaseServerTypeLabel"
						required
						size="xlarge"
						item-value="Value"
						item-label="Text"
						:readonly="isTestingConnection" />
					<div v-if="model.ServerType == 2">
						<q-text-field
							v-model="model.Service"
							:label="resources.serviceIdentifierLabel"
							size="xlarge"
							:readonly="isTestingConnection" />
						<q-text-field
							v-model="model.ServiceName"
							:label="resources.serviceNameLabel"
							size="xlarge"
							:readonly="isTestingConnection" />
					</div>
					<q-text-field
						v-model="model.Schema"
						:label="resources.databaseName"
						required
						size="xlarge"
						:readonly="isTestingConnection">
						<template #extras>
							<q-icon icon="information-outline" />
							{{ resources.databaseNameInfo }}
						</template>
					</q-text-field>
					<q-checkbox
						v-model="model.DatabaseSidePk"
						label="Database-side primary key"
						:readonly="isTestingConnection" />
				</q-row-container>
			</q-card>
		</row>

		<row>
			<q-card
				:title="resources.databaseConnectionTitle"
				class="q-card--admin-default"
				width="block">
				<q-row-container>
					<q-text-field
						v-model="model.DbUser"
						:label="resources.databaseLoginLabel"
						required
						size="xlarge"
						:readonly="isTestingConnection" />
					<password-input
						v-model="model.DbPsw"
						:label="hardcodedTexts.password"
						is-required
						:showFiller="model.HasDbPsw"
						size="xlarge"
						:isReadOnly="isTestingConnection">
					</password-input>
					<password-input
						v-model="model.DbCheckPsw"
						:label="hardcodedTexts.password"
						is-required
						size="xlarge"
						:isReadOnly="isTestingConnection">
					</password-input>
					<q-checkbox
						v-model="model.ConnEncrypt"
						:label="resources.encryptConnectionLabel"
						:readonly="isTestingConnection" />
					<q-checkbox
						v-model="model.ConnWithDomainUser"
						:label="resources.domainUserLabel"
						:readonly="isTestingConnection" />
					<q-button
						id="testServer"
						:label="resources.testServerConnectionButton"
						:disabled="isTestingConnection"
						size="xlarge"
						@click="TestServerConection"
						:loading="showLoader" />
				</q-row-container>
			</q-card>
		</row>

		<row>
			<q-collapsible
				class="q-collapsible--admin-default"
				:title="resources.dataSystemLog"
				width="block">
				<q-text-field
					v-model="model.Log_Server"
					:label="resources.serverName"
					size="xlarge"
					:readonly="isTestingConnection">
					<template #extras>
						<q-icon icon="information-outline" />
						{{ resources.serverNameInfo }}
					</template>
				</q-text-field>
				<numeric-input
					v-model="model.Log_Port"
					:label="hardcodedTexts.port"
					:isReadOnly="isTestingConnection"
					size="xlarge">
				</numeric-input>
				<q-select
					v-model="model.Log_ServerType"
					v-if="model.SelectLists"
					:items="model.SelectLists.DBMS"
					:label="resources.databaseServerTypeLabel"
					size="xlarge"
					item-value="Value"
					item-label="Text"
					:readonly="isTestingConnection" />
				<div v-if="model.Log_ServerType == 2">
						<q-text-field
							v-model="model.Log_Service"
							:label="resources.serviceIdentifierLabel"
							size="xlarge"
							:readonly="isTestingConnection" />
						<q-text-field
							v-model="model.Log_ServiceName"
							:label="resources.serviceNameLabel"
							size="xlarge"
							:readonly="isTestingConnection" />
				</div>
				<q-text-field
					v-model="model.Log_Schema"
					:label="resources.databaseName"
					required
					size="xlarge"
					:readonly="isTestingConnection">
					<template #extras>
						<q-icon icon="information-outline" />
						{{ resources.databaseNameInfo }}
					</template>
				</q-text-field>
				<hr />
				<q-text-field
					v-model="model.Log_DbUser"
					:label="resources.databaseLoginLabel"
					size="xlarge"
					:readonly="isTestingConnection" />
				<password-input
					v-model="model.Log_DbPsw"
					:label="hardcodedTexts.password"
					:showFiller="model.Log_HasDbPsw"
					size="xlarge"
					:isReadOnly="isTestingConnection">
				</password-input>
				<password-input
					v-model="model.Log_DbCheckPsw"
					:label="hardcodedTexts.confirmPasswordLabel"
					size="xlarge"
					:isReadOnly="isTestingConnection">
				</password-input>
				<q-checkbox
					v-model="model.Log_ConnEncrypt"
					:label="resources.encryptConnectionLabel"
					:readonly="isTestingConnection" />
				<q-checkbox
					v-model="model.Log_ConnWithDomainUser"
					:label="resources.domainUserLabel"
					:readonly="isTestingConnection" />
			</q-collapsible>
		</row>

		<row class="footer-btn">
			<q-button
				variant="bold"
				:label="hardcodedTexts.saveConfiguration"
				:disabled="isTestingConnection"
				@click="saveConfigDatabase" />

			<data-system-badge
				:title="resources.currentDataSystemTitle" />
		</row>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import { texts } from '@/resources/hardcodedTexts.ts';

export default {
	name: 'database',

	props: {
		model: {
			required: true
		},

		/**
		* WebAdmin texts.
		*/
		resources: {
			type: Object,
			required: true
		}
	},

	mixins: [reusableMixin],

	emits: ['update-model', 'connection-tested'],

	data() {
		return {
			showDialog: false,
			showLoader: false,
			isTestingConnection: false,
			redirectTimeout: null,
			globalClickHandler: null
		}
	},

	computed: {
		hardcodedTexts() {
			return {
				port: this.Resources[texts.port],
				password: this.Resources[texts.password],
				confirmPasswordLabel: this.Resources[texts.confirmPasswordLabel],
				saveConfiguration: this.Resources[texts.saveConfiguration],
				fillRequiredFields: this.Resources[texts.fillRequiredFields],
				redirectingIn3Seconds: this.Resources[texts.redirectingIn3Seconds],
				serverName: this.Resources[texts.serverName],
				databaseName: this.Resources[texts.databaseName],
				changesSavedSuccess: this.Resources[texts.changesSavedSuccess],
			}
		},

		portValue() {
			return parseInt(this.model.Port) || ''
		}
	},

	methods: {
		updateSystemPort(newVal) {
			this.model.Port = newVal.toString()
		},

		saveConfigDatabase() {
			QUtils.log("SaveConfigDatabase - Request", QUtils.apiActionURL('Config', 'SaveConfigDatabase'));
			QUtils.postData('Config', 'SaveConfigDatabase', this.model, null, (data) => {
				QUtils.log("SaveConfigDatabase - Response", data);
				if (data.ResultMsg ===  this.hardcodedTexts.changesSavedSuccess + " " +  this.hardcodedTexts.redirectingIn3Seconds) {
					this.$emit('update-model', data);
					this.redirectTimeout = setTimeout(() => {
						this.$router.push({ name: 'dashboard', params: { culture: this.currentLang, system: this.currentYear } });
					}, 3000);
					this.globalClickHandler = (event) => {
						if (event.target.closest('.nav-item')) {
							clearTimeout(this.redirectTimeout);
						}
					}
					document.addEventListener('click', this.globalClickHandler, true);
				} else {
					this.$emit('update-model', data);
				};
			});
		},

		TestServerConection() {
			// Verify that essential data is present
			if (!this.model.Server || !this.model.Schema || !this.model.DbUser || !this.model.DbPsw) {
				this.$emit('alert-class', { ResultMsg: this.hardcodedTexts.fillRequiredFields, AlertType: 'danger' });
				return;
			}

			// Prepare data to send
			const testData = {
				Server: this.model.Server,
				DbUser: this.model.DbUser,
				Schema: this.model.Schema,
				DbPsw: this.model.DbPsw,
				Port: this.model.Port,
				DBType: this.model.ServerType
			};

			// Reset the loader and testing state
			this.showLoader = true;
			this.isTestingConnection = true;

			// Make API call to test the connection
			QUtils.log("TestServerConection - Request", QUtils.apiActionURL('Config', 'TestDBConnection'));
			QUtils.postData('Config', 'TestDBConnection', testData, null, (response) => {
				QUtils.log("TestServerConection - Response", response);
				this.$emit('connection-tested', response);

				this.showLoader = false;
				this.isTestingConnection = false;
			})
		},
	},

	beforeUnmount() {
		if (this.globalClickHandler) {
			document.removeEventListener('click', this.globalClickHandler, true);
		}
	}
};
</script>
