<template>
	<div id="system_setup_paths_container">
		<row>
			<q-card
				class="q-card--admin-default"
				:title="GetTitle"
				width="block">
				<q-row-container>
					<q-text-field
						v-model="Model.pathApp"
						size="xlarge"
						:label="resources.pathAppLabel" />
					<q-text-field
						v-model="Model.pathDocuments"
						size="xlarge"
						:label="resources.pathDocumentsLabel" />
				</q-row-container>
			</q-card>
		</row>

		<row class="footer-btn">
			<q-button
				variant="bold"
				:label="hardcodedTexts.saveConfiguration"
				@click="SavePathCfg" />
		</row>

		<row>
			<q-card
				class="q-card--admin-border-top q-card--admin-compact"
				:title="resources.downloadConfigFile"
				variant="minor"
				width="block">
				<q-button
					label="Configurations.redirect.xml"
					@click="goToDownloadRedirect" />
			</q-card>
		</row>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import bootbox from 'bootbox';
	import { texts } from '@/resources/hardcodedTexts.ts';

	export default {
		name: 'paths',

		props: {
			model: {
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
				Model: this.model,
			};
		},

		methods: {
			goToDownloadRedirect() {
				window.location.href = this.DownloadRedirect;
			},
			SavePathCfg() {
				var vm = this;
				QUtils.log("SavePathCfg - Request", QUtils.apiActionURL('Config', 'SavePathCfg'))
				QUtils.postData('Config', 'SavePathCfg', vm.Model, null, function (data) {
					QUtils.log("SavePathCfg - Response", data)
					if (data.Success) {
						bootbox.alert(vm.Resources.ALTERACOES_EFETUADAS10166);
					}
					else {
						bootbox.alert(data.Message)
					}
				})
 			}
		},

		computed: {
			DownloadRedirect() {
				return QUtils.apiActionURL('Config', 'DownloadRedirect');
			},

			GetTitle() {
				return this.Resources.CAMINHOS41141 + ' ' + '(' + this.currentApp +')';
			},
			hardcodedTexts() {
				return {
					saveConfiguration: this.Resources[texts.saveConfiguration]
				};
			}
		}
	};
</script>
