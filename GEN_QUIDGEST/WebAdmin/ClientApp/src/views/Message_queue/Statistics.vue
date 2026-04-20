<template>
	<div id="message_queue_statistics_container">
		<q-card
			width="block">
			<q-row-container>
				<q-control-wrapper class="row-line-group">
					<base-input-structure
						class="i-text">
						<q-select
							v-model="queue"
							item-value="Value"
							item-label="Text"
							:items="queuesFilter"
							:label="Resources.QUEUE45251"
							size="xlarge" />
					</base-input-structure>
					<base-input-structure
						class="i-text">
						<datetime-picker
							v-model="model.Stats.StartDate"
							:label="Resources.DATA_DE_INICIO37610" />
					</base-input-structure>
					<base-input-structure
						class="i-text">
						<datetime-picker
							v-model="model.Stats.EndDate"
							:label="Resources.DATA_DE_FIM18270" />
					</base-input-structure>
				</q-control-wrapper>

				<row class="footer-btn">
					<q-button
						variant="bold"
						:label="Resources.ATUALIZAR_ESTATISTIC07525"
						@click="UpdateStats" />

					<data-system-badge
						:title="Resources.SISTEMA_DE_DADOS_ATU09110" />
				</row>
			</q-row-container>
			
			<status 
				v-if="!isEmptyObject(Status)"
				:model="Status" />
		</q-card>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import status from './Status.vue';

	export default {
		name: 'message_queue_statistics',

		mixins: [reusableMixin],

		components: { status },

		props: {
			model: {
				required: true
			}
		},

		data: function () {
			//var vm = this;
			return {
				queue: '',
				queuesFilter: [
					{ Text: ' ', Value: '' }
				],
				Status: { }
			};
		},

		methods: {
			fillQueuesFilter: function() {
				var vm = this;
				$.each(vm.model.MQueues.Queues, function(index, q) {
					vm.queuesFilter.push({ Text: q.queue, Value: q.queue });
				});
			},
			
			UpdateStats: function () {
				var vm = this, assignedRoleId = [];
				$.each(vm.model.MQueues.Acks, function (key, value) {
					assignedRoleId.push(value.ackQueue);
				});

				QUtils.postData('MessageQueue', 'QueueProcessStats', {
					queue: vm.queue,
					dataINI: vm.model.Stats.StartDate,
					dataFIM: vm.model.Stats.EndDate,
					acks: assignedRoleId
				}, null, function (data) {
					vm.Status = data;
				});
			}
		},
		
		created: function () {
			this.fillQueuesFilter();
		}
	};
</script>
