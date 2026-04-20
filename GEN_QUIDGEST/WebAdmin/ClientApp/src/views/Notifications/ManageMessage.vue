<template>
	<div id="notifications_manage_message_container">
		<div class="q-stack--column">
			<h1 class="f-header__title">
				{{ Resources.CONFIGURACAO_DA_MENS64174 }}
			</h1>
		</div>
		<hr>
		<template v-if="!isEmptyObject(Model.ResultMsg)">
			<div class="alert alert-info">
				<p>
					<b class="status-message">
						{{ Model.ResultMsg }}
					</b>
				</p>
			</div>
			<br />
		</template>
		<template v-if="Model.FormMode==3">
			<div class="alert alert--warning">
				<strong>
					{{ Resources.AVISO03237 + ':' }}
				</strong>
				{{ Resources.DESEJA_ELIMINAR_ESTA24564 }}
			</div>
		</template>

		<row>
			<q-card
				width="block">
				<q-row-container>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<text-input
								v-model="Model.ValDesignac"
								:label="Resources.NOME47814"
								:isReadOnly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<q-select
								v-if="Model"
								v-model="Model.ValCodpmail"
								item-value="Value"
								item-label="Text"
								:items="Model.TableEmailProperties"
								:label="Resources.REMETENTE47685"
								:readonly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<q-checkbox
								v-model="Model.ValDestnman"
								:label="Resources.DESTINATARIO_MANUAL30643"
								:readonly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<q-select
								v-if="Model"
								v-model="Model.ValCoddestn"
								item-value="Value"
								item-label="Text"
								:items="Model.TableAllowedDestinations"
								:label="Resources.DESTINATARIO22298"
								:readonly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<text-input
								v-model="Model.ValTomanual"
								:label="Resources.DESTINATARIO_MANUAL30643"
								:isReadOnly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
				</q-row-container>
			</q-card>
		</row>

		<row>
			<q-collapsible
				class="q-collapsible--admin-default"
				title="Cc & Bcc"
				width="block">
				<q-text-field
					v-model="Model.ValCc"
					:label="'Cc'"
					:readonly="blockForm" />
				<q-text-field
					v-model="Model.ValBcc"
					:label="'Bcc'"
					:readonly="blockForm" />
			</q-collapsible>
		</row>

		<row>
			<q-card
				width="block">
				<q-row-container>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<text-input
								v-model="Model.ValAssunto"
								:label="Resources.ASSUNTO16830"
								:isReadOnly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<q-text-area
								v-model="Model.ValMensagem"
								:label="Resources.MENSAGEM32641"
								:readonly="blockForm"
								:rows="5"
								size="xxlarge"
								autosize />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<q-select
								v-if="Model"
								v-model="Model.ValSelectedTag"
								item-value="Value"
								item-label="Text"
								:items="Model.TableAllowedTags"
								:label="Resources.TAGS54909"
								:readonly="blockForm" />
							<q-button
								:label="Resources.ADICIONAR17177"
								:disabled="blockForm"
								@click="addText" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<q-select
								v-if="Model"
								v-model="Model.ValCodsigna"
								item-value="Value"
								item-label="Text"
								:items="Model.TableEmailSignatures"
								:label="Resources.ASSINATURA_DE_EMAIL58979"
								:readonly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<q-checkbox
								v-model="Model.ValHtml"
								:label="Resources.FORMATO_HTML_65194"
								:readonly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<q-checkbox
								v-model="Model.ValEmail"
								:label="Resources.ENVIA_EMAIL_46551"
								:readonly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<q-checkbox
								v-model="Model.ValGravabd"
								:label="Resources.GRAVA_NA_BD_43814"
								:readonly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
					<q-control-wrapper class="row-line-group">
						<base-input-structure
							class="i-text">
							<q-checkbox
								v-model="Model.ValAtivo"
								:label="Resources.ATIVO_00196"
								:readonly="blockForm" />
						</base-input-structure>
					</q-control-wrapper>
				</q-row-container>
			</q-card>
		</row>
		
		<row class="footer-btn">
			<q-button
				v-if="Model.FormMode !== 3"
				variant="bold"
				:label="Resources.GRAVAR_CONFIGURACAO36308"
				@click="SaveMessage" />
			<q-button
				v-else
				variant="bold"
				color="danger"
				:label="Resources.APAGAR04097"
				@click="SaveMessage" />
			<q-button
				:label="Resources.CANCELAR49513"
				@click="cancel" />
		</row>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import QAlert from '@/components/QAlert.vue';
	import { QUtils } from '@/utils/mainUtils';

	import _get from "lodash-es/get";

	export default {
		name: 'notifications_message',

		mixins: [reusableMixin],

		components: {
			QAlert
		},

		data() {
			return {
				Model: {}
			}
		},

		computed: {
			blockForm() { return this.Model.FormMode == 3; }
		},

		methods: {
			fetchData() {
				var vm = this,
				mod = vm.$route.params.mod,
				codmesgs = _get(vm.$route.params, 'codmesgs', null),
				idnotif = _get(vm.$route.params, 'idnotif', null);
				QUtils.log("Fetch data - ManageMessage");
				QUtils.FetchData(QUtils.apiActionURL('Notifications', 'ManageMessage', { mod, codmesgs, idnotif })).done(function (data) {
					QUtils.log("Fetch data - OK (ManageMessage)", data);
					if (data.Success) {
						$.each(data.model, function (propName, value) { vm.Model[propName] = value; });
					}
				});
			},

			cancel() {
				this.$router.go(-1);
			},

			SaveMessage() {
				var vm = this;
				QUtils.log("ManageMessage - Request", QUtils.apiActionURL('Notifications', 'SaveMessage'));
				QUtils.postData('Notifications', 'SaveMessage', {
					...vm.Model,
					/*
						The client side does not need to send the list options back.
						And if it is send, it will cause an error in deserialization.
					*/
					TableAllowedDestinations: null,
					TableAllowedTags: null,
					TableEmailProperties: null,
					TableEmailSignatures: null
				}, null, function (data) {
					QUtils.log("ManageMessage - Response", data);
					if (data.Success) {
						vm.$router.go(-1);
					}
					else {
						$.each(data.model, function (propName, value) { vm.Model[propName] = value; });
					}
				});
			},

			addText() {
				this.Model.ValMensagem += this.Model.ValSelectedTag;
			}
		},

		created() {
			this.fetchData();
		}
	};
</script>
