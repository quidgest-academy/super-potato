<template>
	<teleport
		v-if="isReady"
		to="#q-modal-form-suggestion-list-body">
		<q-table
			:rows="tableRows"
			:columns="tableColumns"
			:config="tableConfig" />
	</teleport>
</template>

<script>
	import { computed } from 'vue'
	import { mapActions } from 'pinia'

	import { useGenericDataStore } from '@quidgest/clientapp/stores'
	import { postData } from '@quidgest/clientapp/network'
	import hardcodedTexts from '@/hardcodedTexts'

	import _merge from 'lodash-es/merge'

	export default {
		name: 'QSuggestionList',

		expose: [],

		data()
		{
			return {
				model: {},

				tableConfig: {
					showFooter: false,
					allowColumnConfiguration: false
				},

				tableColumns: [
					{
						order: 1,
						dataType: 'Text',
						label: computed(() => this.Resources[hardcodedTexts.type]),
						name: 'SuggestionType',
						sortable: true
					},
					{
						order: 2,
						dataType: 'Text',
						label: computed(() => this.Resources[hardcodedTexts.field]),
						name: 'OldValue',
						sortable: true
					},
					{
						order: 3,
						dataType: 'Text',
						label: computed(() => this.Resources[hardcodedTexts.suggestedText]),
						name: 'NewValue',
						sortable: true
					}
				],

				isReady: false
			}
		},

		mounted()
		{
			const props = {
				title: computed(() => this.Resources[hardcodedTexts.suggestions]),
				dismissible: true
			}

			const modalProps = {
				id: 'form-suggestion-list',
				dismissAction: this.goBack,
				isActive: true
			}

			this.setModal(props, modalProps)
			this.fetchData()
		},

		computed: {
			tableRows()
			{
				const rows = []

				if (this.model.Suggestions && this.model.Suggestions.length > 0)
				{
					for (let i = 0; i < this.model.Suggestions.length; i++)
					{
						const row = {
							Rownum: i,
							Fields: this.model.Suggestions[i]
						}
						rows.push(row)
					}
				}

				return rows
			}
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'setModal'
			]),

			fetchData()
			{
				postData('Suggestion', 'List', null, (data) => {
					_merge(this.model, data)
					this.isReady = true
				})
			}
		}
	}
</script>
