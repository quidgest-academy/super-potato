<template>
	<q-dialog
		v-model="model"
		dismissible
		class="q-document__popup"
		size="large"
		:title="texts?.uploadDocVersionHeader"
		:buttons="buttons">
		<template #body>
			<q-row>
				<q-col>
					<q-table
						:rows="tableRows"
						:columns="tableColumns"
						:config="tableConfig"
						@row-action="findVersionToDownload($event)" />
				</q-col>
			</q-row>
		</template>
	</q-dialog>
</template>

<script setup lang="ts">
	// Components
	import { QCol, QDialog, QRow } from '@quidgest/ui/components'

	// Utils
	import { computed } from 'vue'

	// Types
	import type { QButtonProps } from '@quidgest/ui/components'
	import type { VersionInfo } from './types'

	const emit = defineEmits<{
		(e: 'get-file', fileVersion: string): void
		(e: 'delete-history'): void
		(e: 'delete-last'): void
	}>()

	const props = defineProps<{
		/**
		 * Necessary strings to be used in labels and buttons.
		 */
		texts: Record<string, string>

		/**
		 * List of versions and their informations.
		 */
		versionsInfo: VersionInfo[]

		/**
		 * The resources path.
		 */
		resourcesPath: string
	}>()

	const buttons = [
		{
			id: 'delete-last',
			icon: { icon: 'delete' },
			props: { label: props.texts.deleteLastLabel, variant: 'bold' } as QButtonProps,
			action: () => {
				emit('delete-last')
			}
		},
		{
			id: 'delete-history',
			icon: { icon: 'delete' },
			props: { label: props.texts.deleteHistoryLabel } as QButtonProps,
			action: () => {
				emit('delete-history')
			}
		},
		{
			id: 'cancel',
			icon: { icon: 'cancel' },
			props: { label: props.texts.cancelLabelValue } as QButtonProps
		}
	]

	const tableRows = computed(() => {
		const rows = []

		if (props.versionsInfo && props.versionsInfo.length > 0) {
			for (let i = 0; i < props.versionsInfo.length; i++) {
				const row = {
					Rownum: i,
					Fields: props.versionsInfo[i],
					rowKey: props.versionsInfo[i]?.id
				}

				rows.push(row)
			}
		}

		return rows
	})

	const tableColumns = [
		{
			order: 1,
			dataType: 'Text',
			label: props.texts.version,
			name: 'version',
			sortable: true
		},
		{
			order: 2,
			dataType: 'Text',
			label: props.texts.documentLabel,
			name: 'fileName',
			sortable: true
		},
		{
			order: 3,
			dataType: 'Text',
			label: props.texts.bytesLabel,
			name: 'bytes',
			sortable: true
		},
		{
			order: 4,
			dataType: 'Text',
			label: props.texts.author,
			name: 'author',
			sortable: true
		},
		{
			order: 5,
			dataType: 'Text',
			label: props.texts.createdOnLabel,
			name: 'createdOn',
			sortable: true
		}
	]

	const tableConfig = {
		customActions: [
			{
				id: 'download',
				name: 'download',
				title: props.texts.downloadLabel,
				icon: {
					icon: 'download',
					type: 'svg'
				},
				isInReadOnly: true
			}
		],
		rowValidation: {
			// eslint-disable-next-line @typescript-eslint/no-explicit-any
			fnValidate: (row: any) => row.rowKey?.length > 0,
			message: props.texts.pendingDocumentVersion
		},
		resourcesPath: props.resourcesPath
	}

	// eslint-disable-next-line @typescript-eslint/no-explicit-any
	const findVersionToDownload = (rowData: any): void => {
		if (typeof rowData?.rowKey !== 'string') return

		props.versionsInfo.forEach((version) => {
			if (version?.id === rowData.rowKey) emit('get-file', version.version)
		})
	}

	const model = defineModel<boolean>()
</script>
