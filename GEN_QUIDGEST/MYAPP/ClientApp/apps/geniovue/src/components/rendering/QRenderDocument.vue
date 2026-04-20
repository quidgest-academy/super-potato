<template>
	<a
		v-if="value?.fileName && value?.ticket"
		class="column-data-link"
		rel="tooltip"
		:title="Resources.DESCARREGAR58418"
		data-table-action-selected="false"
		tabindex="-1"
		@click.stop.prevent="onSelect"
		@keyup.enter="onSelect">
		{{ value.fileName }}
	</a>
</template>

<script>
	import { documentViewTypeMode } from '@quidgest/clientapp/constants/enums'

	export default {
		name: 'QRenderDocument',

		emits: ['execute-action'],

		props: {
			/**
			 * The object containing properties necessary to represent a document.
			 * It usually has a ticket for authentication, a fileName for display and download,
			 * title for tooltip, and viewType to determine how the document is to be processed.
			 */
			value: {
				type: Object,
				default: () => ({
					ticket: '',
					fileName: '',
					title: '',
					viewType: documentViewTypeMode.preview
				})
			},

			/**
			 * Object containing properties of the column.
			 */
			options: {
				type: Object,
				default: null
			}
		},

		expose: [],

		methods: {
			/**
			 * Method to execute when the anchor link is clicked.
			 * It emits the 'execute-action' event with details for the document download.
			 */
			onSelect()
			{
				const viewType = this.value?.viewType ?? documentViewTypeMode.preview
				//Area of the document column, which may be different from the area of the table.
				const area = this.options?.area

				this.$emit('execute-action', {
					action: 'download',
					ticket: this.value.ticket,
					fileName: this.value.fileName,
					viewType: viewType,
					area: area
				})
			}
		}
	}
</script>
