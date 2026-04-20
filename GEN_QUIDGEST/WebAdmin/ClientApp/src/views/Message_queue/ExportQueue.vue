<template>
	<q-dialog
		v-model="showDialog"
		:buttons="[]"
		:title="Resources.EXPORTACAO_DE_MQ29750">
		<template #body.content>
			<div class="q-dialog-container">
				<row>
					<text-input v-model="Model.queue" :label="Resources.NOME_DA_QUEUE56594" :isReadOnly="true" :size="'xlarge'"></text-input>
				</row>
				<row>
					<text-input v-model="Model.Qyear" :label="Resources.ANO33022" :isReadOnly="true" :size="'xlarge'"></text-input>
				</row>

				<hr />
				<section>{{Resources.CONDICAO44011}}</section>
				<row>
					<q-select
						v-model="conditionField"
						clearable
						item-value="Value"
						item-label="Text"
						:items="fields"
						:label="Resources.CAMPO46284"
						size="xlarge" />
				</row>
				<row>
					<q-select
						v-model="conditionOp"
						clearable
						item-value="Value"
						item-label="Text"
						:items="ops"
						:label="Resources.OPERACAO29482"
						size="xlarge" />
				</row>
				<row>
					<text-input v-model="conditionValue" :label="Resources.VALOR32448" :size="'xlarge'"></text-input>
				</row>

				<p>{{Resources.MENSAGENS_A_SER_EXPO34711}}: {{count}}</p>

				<row v-if="progressModel.Active">
					<q-label :label="Resources.PROGRESSO52692" />
					<div class="progress">
						<div class="progress-bar progress-bar-striped progress-bar-animated" :style="{ width: progressModel.Count + '%' }">
							{{ progressModel.Count }}%
						</div>
					</div>
					<div>{{ progressModel.Message }}</div>
				</row>
				<row v-else-if="typeof progressModel.Message === 'string' && progressModel.Message.length > 0">
					<q-alert
						type="danger"
						:text="progressModel.Message" />
				</row>
			</div>
			<div class="q-dialog__body-actions">
				<!-- TODO: After updating Quidgest UI pckage to a newer version, it should be changed to 'footer.append' slot -->
				<q-button
					@click="close"
					:label="Resources.FECHAR32496" />
				<q-button
					:disabled="disabledExportButton"
					variant="bold"
					@click="exportQueue"
					:label="Resources.EXPORTAR35632" />
			</div>
		</template>
	</q-dialog>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';

	export default {
		name: 'message_queue_export_queue',
		mixins: [reusableMixin],
		emits: ['close'],
		props: {
			Model: {
				required: true
			},
			show: {
				required: true
			}
		},
		data: function () {
			var vm = this;
			return {
				progressModel: {
					Active: false,
					Message: '',
					Count: 0
				},
				uniqueId: '',
				count: 0,
				fields: [],
				ops: ['=', '<', '>', '<=', '>=', '!=', 'IN'].map(function (x) { return { Value: x, Text: x }; }),
				conditionField: '',
				conditionOp: '=',
				conditionValue: '',
				timeoutId: null,

				disabledExportButton: false,
				showDialog: this.show
			};
		},
		methods: {
			testExportQueue: function () {
				var vm = this;
				if (!vm.Model.queue)
					return;
				var p = {
					queue: vm.Model.queue,
					year: vm.Model.Qyear,
					conditionField: vm.conditionField,
					conditionOp: vm.conditionOp,
					conditionValue: vm.conditionValue
				};

				QUtils.postData('MessageQueue', 'TestExportQueue', null, p, function (data) {
					vm.count = data.count;
					vm.fields = data.fields.map(function (x) { return { Value: x, Text: x }; });
					vm.timeoutId = null;
				});
			},
			timerTestExportQueue: function () {
				var vm = this;
				if (vm.timeoutId) clearTimeout(vm.timeoutId);
				vm.timeoutId = setTimeout(vm.testExportQueue, 3000);
			},
			exportQueue: function () {
				var vm = this;
				var p = {
					queue: vm.Model.queue,
					year: vm.Model.Qyear,
					conditionField: vm.conditionField,
					conditionOp: vm.conditionOp,
					conditionValue: vm.conditionValue,
					id: vm.uniqueId
				};

				QUtils.postData('MessageQueue', 'ExportQueues', null, p, function (data) {
					$.each(data, function (propName, value) { vm.progressModel[propName] = value; });
					vm.disabledExportButton = true;
					vm.startMonitorProgress();
				});
			},
			startMonitorProgress: function () {
				// Init monitors
				var vm = this;
				vm.progressModel.Count = 0; vm.progressModel.Message = '', vm.progressModel.Active = true;

				var intervalId = setInterval(function () {
					QUtils.postData('MessageQueue', 'Progress', null, { id: vm.uniqueId }, function (data) {
						$.each(data, function (propName, value) { vm.progressModel[propName] = value; });
						if (data.Completed) {
							clearInterval(intervalId);
							vm.disabledExportButton = false;
							vm.progressModel.Active = false;
						}
					});
				}, 250);

			},
			close: function () {
				this.$emit('close');
			},
			initForm: function () {
				this.showDialog = this.show;
				this.testExportQueue();
			}
		},
		created: function () {
			this.uniqueId = QUtils.createGuid();
		},
		mounted: function () {
			this.$nextTick(() => this.initForm());
		},
		watch: {
			show: {
				immediate: true,
				handler() { this.initForm(); }
			},
			'conditionField': 'timerTestExportQueue',
			'conditionOp': 'timerTestExportQueue',
			'conditionValue': 'timerTestExportQueue',
		}
	};
</script>
