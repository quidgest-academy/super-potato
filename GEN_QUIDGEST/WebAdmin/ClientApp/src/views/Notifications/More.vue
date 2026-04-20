<template>
	<div>
		<row>
			<q-select
				v-model="model.passwordRecovery"
				:items="model.emailPropertiesList"
				item-value="Value"
				item-label="Text"
				:label="Resources.PASSWORD_RECOVERY53114" />
		</row>
		<row>
			<q-button
				variant="bold"
				:label="Resources.GRAVAR45301"
				@click="SaveEmail" />
		</row>
    </div>
</template>

<script>
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';

	export default {
		name: "more",
		mixins: [reusableMixin],
		props: {
			model: {
				required: true
			}
		},
		methods: {
			SaveEmail() {
				var vm = this;
				QUtils.log("ManageProperties - Request", QUtils.apiActionURL('Email', 'SaveEmail'));
				var params = { userRegistration: vm.model.userRegistration, passwordRecovery : vm.model.passwordRecovery};
				QUtils.postData('Email', 'SaveEmail', null, params, function (data) {
					vm.model.ResultMsg = data.ResultMsg;
				});
			},
		}
	}
</script>
