<template>
	<div id="notifications_signature_container">
		<div class="q-stack--column">
				<h1 class="f-header__title">
				{{ Resources.ASSINATURA_DE_EMAIL58979 }}
				</h1>
			</div>
		<hr>
		<template v-if="!isEmptyObject(Model.ResultMsg)">
			<div class="alert alert--info">
				<p><b class="status-message">{{ Model.ResultMsg }}</b></p>
			</div>
			<br />
		</template>
		<template v-if="Model.FormMode==3">
			<div class="alert alert--warning">
				<strong>Warning!</strong>
				{{ Resources.DESEJA_ELIMINAR_ESTA24564 }}
			</div>
		</template>

		<q-card
			class="q-card--admin-default"
			:title="Resources.DADOS_DO_REGISTO10198"
			width="block">
			<q-row-container>
				<q-control-wrapper class="row-line-group">
					<base-input-structure
						class="i-text">
						<text-input v-model="Model.ValName" :label="Resources.NOME47814" :isReadOnly="blockForm"></text-input>
					</base-input-structure>
				</q-control-wrapper>
				<q-control-wrapper class="row-line-group">
					<base-input-structure
						class="i-text">
						<image-input :getUrl="getEmailSignatureImage" :setUrl="setEmailSignatureImage" :label="Resources.IMAGEM19513"></image-input>
					</base-input-structure>
				</q-control-wrapper>
				<q-control-wrapper class="row-line-group">
					<base-input-structure
						class="i-text">
						<text-input v-model="Model.ValTextass" :label="Resources.TEXTO_APOS_A_ASSINAT02808" :isReadOnly="blockForm"></text-input>
					</base-input-structure>
				</q-control-wrapper>

				<row class="footer-btn">
					<q-button
						v-if="Model.FormMode !== 3"
						variant="bold"
						:label="Resources.GRAVAR_CONFIGURACAO36308"
						@click="SaveSignature" />
					<q-button
						v-else
						variant="bold"
						color="danger"
						:label="Resources.APAGAR04097"
						@click="SaveSignature" />
					<q-button
						:label="Resources.CANCELAR49513"
						@click="cancel" />
				</row>
			</q-row-container>

		</q-card>

	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import bootbox from 'bootbox';

	import _get from "lodash-es/get";

	export default {
		name: 'notifications_signature',

		mixins: [reusableMixin],

		data: function () {
			//var vm = this;
			return {
				Model: {}
			};
		},

		computed: {
			blockForm: function () { return this.Model.FormMode == 3; },

			getEmailSignatureImage: function () {
				var codsigna = _get(this.$route.params, 'codsigna', null);
				if ($.isEmptyObject(codsigna)) { codsigna = this.Model.ValCodsigna; }
				return QUtils.apiActionURL('Email', 'getEmailSignatureImage', { key: codsigna });
			},

			setEmailSignatureImage: function () {
				return QUtils.apiActionURL('Email', 'setEmailSignatureImage', { key: this.Model.ValCodsigna });
			}
		},

		methods: {
			fetchData: function () {
				var vm = this,
					mod = vm.$route.params.mod,
					codsigna = _get(vm.$route.params, 'codsigna', null);
				QUtils.log("Fetch data - ManageSignature");
				QUtils.FetchData(QUtils.apiActionURL('Email', 'ManageSignature', { mod, codsigna })).done(function (data) {
					QUtils.log("Fetch data - OK (ManageSignature)", data);
					if (data.Success) {
						$.each(data.model, function (propName, value) { vm.Model[propName] = value; });
					}
					else {
						bootbox.alert(data.Message, function () {
							vm.$router.replace({ name: 'notifications', params: { culture: vm.currentLang, system: vm.currentYear } });
						})
					}
				});
			},

			cancel: function () {
				var vm = this;
				vm.$router.replace({ name: 'email', params: { culture: vm.currentLang, system: vm.currentYear } });
			},

			SaveSignature: function () {
				var vm = this;
				QUtils.log("ManageSignature - Request", QUtils.apiActionURL('Notifications', 'SaveSignature'));
				  QUtils.postData('Email', 'SaveSignature', vm.Model, null, function (data) {
					  QUtils.log("ManageSignature - Response", data);
					  if (data.Success) {
						  vm.$router.replace({ name: 'email', params: { culture: vm.currentLang, system: vm.currentYear } });
					  }
					  else {
						  $.each(data.model, function (propName, value) { vm.Model[propName] = value; });
					  }
				  });
			}
		},
		
		created: function () {
			// Ler dados
			this.fetchData();
		}
	};
</script>
