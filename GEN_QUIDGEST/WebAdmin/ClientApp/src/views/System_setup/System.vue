<template>
	<div id="system_setup_system_container">
		<scheduler
			:model="model"
			:resources="resources"
			:TaskList="TaskList"
			@alert-class="forwardAlert"
			@update-model="forwardUpdate" />
		<audit
			:model="model"
			:resources="resources"
			@alert-class="forwardAlert" />
		<row class="footer-btn">
			<q-button
				variant="bold"
				:label="hardcodedTexts.saveConfiguration"
				@click="SaveConfig" />
		</row>
	</div>
</template>

<script>
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import scheduler from './Scheduler';
	import audit from './Audit';
	import { texts } from '@/resources/hardcodedTexts.ts';

	export default {
		name: 'system',

		components: {
			scheduler,
			audit
		},

		mixins: [reusableMixin],

		emits: ['update-model', 'alert-class'],

		props: {
			model: {
				required: true
			},
			TaskList: {
				required: false
			},
			resources: {
				type: Object,
				required: true
			}
		},

		computed: {
			hardcodedTexts() {
				return {
					changesSavedSuccess: this.Resources[texts.changesSavedSuccess],
					saveConfiguration: this.Resources[texts.saveConfiguration]
				}
			},

			systemConfigTexts() {
				return new SystemConfigTexts(this)
			},
		},

		methods: {
			forwardAlert(alertData) {
				this.$emit('alert-class', alertData);
			},
			forwardUpdate(alertData) {
				this.$emit('update-model')
			},
			SaveConfig() {
				QUtils.log("SaveSystemConfig - Request", QUtils.apiActionURL('Config', 'SaveSystemConfig'));
				QUtils.postData('Config', 'SaveSystemConfig', this.model, null, (data) => {
					QUtils.log("SaveSystemConfig - Response", data);
					this.$emit('update-model');
					if (data.Success) {
						this.$emit('alert-class', { ResultMsg: this.hardcodedTexts.changesSavedSuccess, AlertType: 'success' });
						this.statusError = false;
					} else {
						this.$emit('alert-class', { ResultMsg: data.Message, AlertType: 'danger' });
					}
				});
			}
		}
	}
</script>
