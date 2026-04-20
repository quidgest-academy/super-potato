<template>
	<q-button
		borderless
		:title="texts.openReport"
		@click="loadQueryList">
		<q-icon icon="open-report" />
	</q-button>

	<teleport
		v-if="showQueryList"
		:to="`#q-modal-${modalId}-body`">
		<div class="content">
			<table class="c-table c-table-hover table-resizable">
				<thead class="c-table__head">
					<tr>
						<th>{{ texts.queryName }}</th>
						<th>{{ texts.queryAccess }}</th>
					</tr>
				</thead>

				<tbody class="c-table__body">
					<tr
						v-for="row in reportList"
						:key="`cav-query-list-row-${row.ID}`"
						@click.stop.prevent="onSelectedQueryToLoad(row.ID)">
						<td>{{ row.Title }}</td>
						<td>{{ getAccessTypeTitle(row.Acess) }}</td>
					</tr>
				</tbody>
			</table>
		</div>
	</teleport>
</template>

<script>
	import { computed, nextTick } from 'vue'
	import { mapActions } from 'pinia'

	import { useGenericDataStore } from '@quidgest/clientapp/stores'
	import { removeModal } from '@/utils/layout'
	import { fetchData } from '@quidgest/clientapp/network'
	import hardcodedTexts from '@/hardcodedTexts.js'

	export default {
		name: 'QCavOpenRecords',

		emits: ['load-query'],

		expose: [],

		data()
		{
			return {
				showQueryList: false,

				reportList: [],

				texts: {
					openReport: computed(() => this.Resources[hardcodedTexts.openReport]),
					selectQuery: computed(() => this.Resources[hardcodedTexts.selectQuery]),
					queryName: computed(() => this.Resources[hardcodedTexts.queryName]),
					queryAccess: computed(() => this.Resources[hardcodedTexts.queryAccess]),
					public: computed(() => this.Resources[hardcodedTexts.public]),
					personal: computed(() => this.Resources[hardcodedTexts.personal]),
					inactive: computed(() => this.Resources[hardcodedTexts.inactive])
				},

				modalId: 'cav-query-list'
			}
		},

		beforeUnmount()
		{
			this.fnHideQueryList()
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'setModal'
			]),

			async fnShowQueryList()
			{
				const props = {
					title: this.texts.selectQuery,
					class: 'q-dialog-form',
					buttons: [
						{
							id: 'dialog-button-close',
							action: this.fnHideQueryList,
							icon: { icon: 'close' },
							props: {
								label: computed(() => this.Resources[hardcodedTexts.close]),
								title: computed(() => this.Resources[hardcodedTexts.close])
							}
						}
					]
				}
				const modalProps = {
					id: this.modalId,
					isActive: true,
					dismissAction: this.fnHideQueryList
				}
				this.setModal(props, modalProps)

				await nextTick()
				this.showQueryList = true
			},

			fnHideQueryList()
			{
				removeModal(this.modalId)
				this.showQueryList = false
			},

			/**
			 * Requests a list of saved queries
			 */
			loadQueryList()
			{
				fetchData(
					'Cav',
					'LoadQueryList',
					null,
					(data) => { // { ID, Title, Acess, Opercria }
						this.reportList = data
						this.fnShowQueryList()
					})
			},

			getAccessTypeTitle(access)
			{
				switch (access)
				{
					case 'PUB':
						return this.texts.public
					case 'PES':
						return this.texts.personal
					case 'INA':
						return this.texts.inactive
					default:
						return access || ''
				}
			},

			onSelectedQueryToLoad(queryId)
			{
				this.$emit('load-query', queryId)
				this.fnHideQueryList()
			}
		}
	}
</script>
