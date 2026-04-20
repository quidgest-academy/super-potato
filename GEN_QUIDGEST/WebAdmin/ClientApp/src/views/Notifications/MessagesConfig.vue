<template>
    <qtable
		:rows="tMessagesConfig.rows"
		:columns="tMessagesConfig.columns"
		:config="tMessagesConfig.config"
		:totalRows="tMessagesConfig.total_rows">
		<template #actions="props">
			<q-button-group borderless>
				<q-button
					variant="text"
					:title="Resources.EDITAR11616"
					@click="ManageMessage(2, props.row)">
					<q-icon icon="pencil" />
				</q-button>
				<q-button
					variant="text"
					:title="Resources.APAGAR04097"
					@click="ManageMessage(3, props.row)">
					<q-icon icon="bin" />
				</q-button>
			</q-button-group>
		</template>
		<template #ValTo="props">
			{{ props.row.ValTo }}{{ props.row.ValTomanual }}
		</template>
		<template #ValAtivo="props">
			<q-icon 
				:icon="props.row.ValAtivo ? 'check' : 'close'"
				:color="props.row.ValAtivo ? 'success' : 'danger'" />
		</template>
		<template #ValEmail="props">
			<q-icon 
				:icon="props.row.ValEmail ? 'check' : 'close'"
				:color="props.row.ValEmail ? 'success' : 'danger'" />
		</template>
		<template #ValGravabd="props">
			<q-icon 
				:icon="props.row.ValGravabd ? 'check' : 'close'"
				:color="props.row.ValGravabd ? 'success' : 'danger'" />
		</template>
		<template #table-footer>
			<tr>
				<td colspan="2">
					<q-button
					:label="Resources.INSERIR43365"
					@click="ManageMessage(1)">
					<q-icon icon="add" />
					</q-button>
				</td>
			</tr>
		</template>
	</qtable>
	<row>
		<q-button
			:label="Resources.CANCELAR49513"
			@click="cancel" />
	</row>
</template>

<script>
	import { reusableMixin } from '@/mixins/mainMixin';

	export default {
		name: 'messagesConfig',
		mixins: [reusableMixin],
		emits: ['update-model'],
		props: {
			model: {
				required: true
			},
			tMessagesConfig: {
				type: Object,
				required: true
			},
		},
		data() {
			return {
				Model: {}
			}
		},
		methods: {
			ManageMessage(mod, row) {
				var codmesgs = $.isEmptyObject(row) ? null : row.ValCodmesgs,
					idnotif = this.$route.params.idnotif;
				this.$router.push({ name: 'manage_message', params: { mod, codmesgs, idnotif, culture: this.currentLang, system: this.currentYear } });
			},
			cancel() {
				this.$router.replace({ name: 'notifications', params: { culture: this.currentLang, system: this.currentYear } });
			}
		}
	}
</script>
