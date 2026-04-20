<template>
	<div id="system_setup_external_services_container">
		<row>
			<q-card
				class="q-card--admin-default"
				:title="resources.integrationSettingsAI"
				width="block">
				<q-row-container>
					<row>
						<q-text-field v-model="model.UrlAPIBackend"
									  :label="resources.urlAPIBackendLabel"
									  size="xxlarge">
							<template #extras>
								<div class="q-field__extras">
									<q-icon icon="information-outline" />
									{{ resources.urlAPIBackendInfo }}
								</div>
							</template>
						</q-text-field>
					</row>
					<row>
						<q-text-field v-model="model.UrlMCP"
									:label="resources.urlMCPLabel"
									size="xxlarge">
							<template #extras>
								<div class="q-field__extras">
									<q-icon icon="information-outline" />
									{{ resources.urlMCPInfo }}
								</div>
							</template>
						</q-text-field>
					</row>
					<row>
						<q-select v-model="model.MCPSecurityMode"
								  :label="resources.mcpSecurityMode"
								  :items="mcpSecurityModeOptions"
								  item-value="value"
								  item-label="label"
								  size="small">
						</q-select>
					</row>
					<div v-if="Number(model.MCPSecurityMode) === 0">
						<row>
							<q-input-group :label="resources.jwtEncryptionKey" >
								<q-password-field v-if="Number(model.MCPSecurityMode) === 0"
												  v-model="model.JWTEncryptionKey"
												  toggle
												  size="xxlarge"												  
												  :disabled="!model.JWTEncryptionKey"
												  :readonly="!!model.JWTEncryptionKey">
								</q-password-field>
								<template #append>
									<q-button @click="regenerateJWTKey">
									<q-icon icon="restore"/>
									{{ model.JWTEncryptionKey ? hardcodedTexts.regenerate : hardcodedTexts.generate }}
									</q-button>
								</template>
									
							</q-input-group>
						</row>
					</div>
				</q-row-container>
			</q-card>
		</row>
		<elasticsearch
            :model="model"
			:resources="resources"
			:Cores="Cores"
			:SelectLists="SelectLists"
            @alert-class="forwardAlert" />
        <reports
            :model="model"
			:resources="resources"
            @alert-class="forwardAlert" />
		<row class="footer-btn">
			<q-button
				variant="bold"
				:label="resources.saveConfigurationButton"
				@click="saveConfigOthers" />
		</row>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import elasticsearch from './Elasticsearch';
	import reports from './Reports';
	import { texts } from '@/resources/hardcodedTexts.ts';

	export default {
		name: 'externalservices',
		components: { elasticsearch, reports },
		props: {
			model: {
				required: true
			},
			Cores: {
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

		emits: ['update-model', 'alert-class'],

		computed: {

			mcpSecurityModeOptions() {
				return [
					{ value: 0, label: 'JWT' },
					{ value: 1, label: 'None' }
				]
			},

			hardcodedTexts() {
				return {
					changesSavedSuccess: this.Resources[texts.changesSavedSuccess],
					generate: this.Resources[texts.generate],
					regenerate: this.Resources[texts.regenerate]
				}
			}
		},

		methods: {
			saveConfigOthers() {
				QUtils.log("SaveConfigOthers - Request", QUtils.apiActionURL('Config', 'SaveConfigOthers'));
				QUtils.postData('Config', 'SaveConfigOthers', this.model, null, (data) => {
					QUtils.log("SaveConfigOthers - Response", data);
						this.$emit('alert-class', {
						ResultMsg: data.Success ? this.hardcodedTexts.changesSavedSuccess : data.Message,
						AlertType: data.Success ? 'success' : 'danger'
					});
				});
			},

			regenerateJWTKey() {
				// Generate a secure random key for JWT encryption (256-bit / 32 bytes)
				const array = new Uint8Array(32);
				crypto.getRandomValues(array);
				const base64Key = btoa(String.fromCharCode.apply(null, array));
				// Update the model with the new key
				this.model.JWTEncryptionKey = base64Key;
			},
		}
	};
</script>
