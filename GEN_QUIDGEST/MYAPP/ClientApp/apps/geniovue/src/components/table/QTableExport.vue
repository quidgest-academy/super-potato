<template>
	<q-action-list
		borderless
		placement="bottom-start"
		:items="exportActions"
		:groups="exportActionGroups"
		:label="texts.exportButtonTitle"
		:title="texts.exportButtonTitle"
		@click="$emit('export-data', $event)">
		<q-icon icon="file-export" />
	</q-action-list>
</template>

<script>
	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'
	import { QActionList } from '@quidgest/clientapp/components'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		exportButtonTitle: 'Export',
		actionMenuTitle: 'Actions'
	}

	export default {
		name: 'QTableExport',

		emits: ['export-data'],

		components: {
			QActionList
		},

		props: {
			/**
			 * An object containing localized texts for the component, allowing customization of displayed strings.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * An array of options available for export. Each option represents a format or export type.
			 */
			options: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		computed: {
			/**
			 * Computes the options to the dropdown
			 */
			exportActions() {
				return this.options.map((act) => ({
					key: act.id,
					label: act.text,
					group: 'export'
				}))
			},

			/**
			 * Computes the options groups for the dropdown
			 */
			exportActionGroups() {
				return [{
					id: 'export',
					display: 'dropdown'
				}]
			}
		}
	}
</script>
