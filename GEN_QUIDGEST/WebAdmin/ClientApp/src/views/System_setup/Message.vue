<template>
    <row>
		<q-card
			class="q-card--admin-border-top q-card--admin-compact"
			:title="resources.messageBrokerTitle"
			width="block">
			<q-row-container>
				<q-checkbox
					v-model="Messaging.Enabled"
					:label="resources.enabledLabel" />
				<q-text-field
					v-model="Messaging.Host.Provider"
					label="Provider"
					readonly
					size="xlarge" />
				<q-text-field
					v-model="Messaging.Host.Endpoint"
					label="Endpoint"
					placeholder="amqp://localhost"
					size="xlarge" />
				<q-text-field
					v-model="Messaging.Host.Username"
					:label="hardcodedTexts.username"
					size="xlarge" />
				<password-input
					v-model="Messaging.Host.Password"
					:label="hardcodedTexts.password"
					show-filler
					size="xlarge" />
			</q-row-container>
		</q-card>
	</row>
	<row>
		<q-card
			class="q-card--admin-border-top q-card--admin-compact"
			:title="resources.publishTitle"
			width="block">
			<q-row-container>
				<div v-for="pub in EnabledPublications">
					<q-checkbox
						:modelValue="pub.enabled"
						@update:modelValue="togglePub(pub, $event)"
						:label="pub.id + ' - ' + pub.description" />
				</div>
			</q-row-container>
		</q-card>
	</row>
	<row>
		<q-card
			class="q-card--admin-border-top q-card--admin-compact"
			:title="resources.subscribeTitle"
			width="block">
			<q-row-container>
				<template v-for="sub in EnabledSubscriptions">
					<q-checkbox
						:modelValue="sub.enabled"
						@update:modelValue="toggleSub(sub, $event)"
						:label="sub.id + ' - ' + sub.description" />
				</template>
			</q-row-container>
		</q-card>
	</row>
	<row class="footer-btn">
		<q-button
			variant="bold"
			:label="Resources.GRAVAR_CONFIGURACAO36308"
			@click="SaveConfigMessaging" />
	</row>
</template>

<script>
	import { QUtils } from '@/utils/mainUtils';
	import { reusableMixin } from '@/mixins/mainMixin';
	import { texts } from '@/resources/hardcodedTexts.ts';

	export default {
		name: 'message',

		emits: ['alert-class', 'update-model'],

		mixins: [reusableMixin],

		props: {
			Metadata: {
				required: true
			},
			Messaging: {
				required: true
			},
			resources: {
				type: Object,
				required: true
			}
		},

		computed: {
			EnabledPublications() {
				const vm = this
				return this.Metadata.Publishers.map(p => {
					return {
						id: p.Id,
						description: p.Description,
						enabled: vm.Messaging.EnabledPublications.indexOf(p.Id) != -1
					}
				});
			},
			EnabledSubscriptions() {
				const vm = this
				return this.Metadata.Subscribers.map(p => {
					return {
						id: p.Id,
						description: p.Description,
						enabled: vm.Messaging.EnabledSubscriptions.indexOf(p.Id) != -1
					}
				});
			},
			hardcodedTexts() {
				return {
					username: this.Resources[texts.username],
					password: this.Resources[texts.password],
				}
			},
		},

		methods: {
			togglePub(pub, checked) {
				this.makeSetHave(this.Messaging.EnabledPublications, pub.id, checked);
			},
			toggleSub(sub, checked) {
				this.makeSetHave(this.Messaging.EnabledSubscriptions, sub.id, checked);
			},
			makeSetHave(set, value, have) {
				let ix = set.indexOf(value);
				if(!have) { //set should not have the item
					if(ix != -1) {
						set.splice(ix, 1) //remove the item
					}
				}
				else { //set should have the item
					if(ix == -1) {
						set.push(value); //add the item
					}
				}
			},
			SaveConfigMessaging() {
				var vm = this;
				QUtils.log("SaveConfigMessaging - Request", QUtils.apiActionURL('Config', 'SaveConfigMessaging'));
				QUtils.postData('Config', 'SaveConfigMessaging', vm.Messaging, null, function (data) {
					QUtils.log("SaveConfigMessaging - Response", data);          
					vm.$emit('update-model');
					vm.$emit('alert-class', { 
						ResultMsg: data.Success ? vm.Resources.ALTERACOES_EFETUADAS10166 : data.Message, 
						AlertType: data.Success ? 'success' : 'danger' 
					});
				});
			},
		}
	};
</script>
