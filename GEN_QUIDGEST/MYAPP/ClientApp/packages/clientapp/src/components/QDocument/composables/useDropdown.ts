// Utils
import { computed } from 'vue'

// Types
import type { Ref } from 'vue'
import type { QDocumentProps } from '../types'
import type { QActionListGroup, QActionListItem } from '../../QActionList'

export function useDropdown(props: QDocumentProps, model: Ref<string | undefined>) {
	/**
	 * Orders versions values and creates a list
	 */
	const orderedVersions = computed(() =>
		Object.entries(props.versions ?? {})
			.sort(([a], [b]) => Number(b) - Number(a))
			.map(([key, value]) => ({ key, value, dirty: !value?.length }))
	)

	const versionCount = computed<number>(() => orderedVersions.value?.length ?? 0)
	const canAttach = computed<boolean>(
		() =>
			!props.readonly &&
			!props.editing &&
			(!props.versioning || !model.value || versionCount.value === 0)
	)
	const canSubmit = computed<boolean>(
		() => !props.readonly && !!props.versioning && !!props.editing
	)
	const canEdit = computed<boolean>(
		() =>
			!props.readonly &&
			!!props.versioning &&
			!props.editing &&
			!!model.value &&
			versionCount.value > 0
	)
	const canShowVersions = computed<boolean>(
		() => !!props.versioning && !props.editing && versionCount.value > 1
	)
	const canDelete = computed<boolean>(() => !props.readonly)
	const canCreate = computed<boolean>(() => !props.readonly && !!props.usesTemplates)

	/**
	 * Create a single action for each version
	 */
	const singleVersionsActions = computed<QActionListItem[]>(
		() =>
			orderedVersions.value.slice(0, 5)?.map((version) => {
				// Change dirty icon when package version updates
				return {
					key: `version-${version.key}`,
					icon: { icon: 'download' },
					label: `${version.key}` || '',
					group: 'versions',
					extraInfo: version.dirty ? '⚠︎' : ''
				}
			}) || { key: '', label: '', group: '' }
	)

	/**
	 * Versions submenu group structure
	 */
	const versionsGroups = [
		{ id: 'default', title: '' },
		{ id: 'versions', title: '' },
		{ id: 'deletes', title: '' }
	]

	/**
	 * Versions submenu actions
	 */
	const versionsActions = computed<QActionListItem[]>(() => [
		{
			key: 'document-history',
			icon: { icon: 'properties' },
			label: props.texts?.viewAll || '',
			group: 'default'
		},
		{
			key: 'document-delete',
			icon: { icon: 'delete' },
			label: props.texts?.deleteLastLabel || '',
			group: 'deletes'
		},
		{
			key: 'delete-history',
			icon: { icon: 'delete' },
			label: props.texts?.deleteHistoryLabel || '',
			group: 'deletes'
		},
		...singleVersionsActions.value
	])

	/**
	 * List with all the items (including subitems)
	 */
	const items = computed<QActionListItem[]>(() => {
		if (!model.value)
			return [
				{
					key: 'attach',
					icon: { icon: 'upload-img' },
					label: props.texts?.attachLabel || '',
					group: 'default',
					disabled: props.readonly
				}
			]

		return [
			{
				key: 'download',
				icon: { icon: 'download' },
				label: props.texts?.downloadLabel || '',
				group: 'default'
			},
			{
				key: 'attach',
				icon: { icon: 'attachment' },
				label: props.texts?.attachLabel || '',
				group: 'default',
				isVisible: canAttach.value
			},
			{
				key: 'submit',
				icon: { icon: 'upload' },
				label: props.texts?.submitLabel || '',
				group: 'default',
				isVisible: canSubmit.value
			},
			{
				key: 'edit',
				icon: { icon: 'pencil' },
				label: props.texts?.editLabel || '',
				group: 'default',
				isVisible: canEdit.value
			},
			{
				key: 'delete',
				icon: { icon: 'delete' },
				label: props.texts?.deleteLabel || '',
				group: 'default',
				isVisible: canDelete.value
			},
			{
				key: 'versions',
				icon: { icon: 'list' },
				label: props.texts?.versionsLabel || '',
				group: 'default',
				isVisible: canShowVersions.value,
				items: versionsActions.value,
				groups: versionsGroups
			},
			{
				key: 'history',
				icon: { icon: 'delete' },
				label: props.texts?.viewAll || '',
				group: 'versions-history'
			},
			{
				key: 'delete-last',
				icon: { icon: 'delete' },
				label: props.texts?.deleteLastLabel || '',
				group: 'versions-actions',
				isVisible: canDelete.value
			},
			{
				key: 'delete-history',
				icon: { icon: 'delete' },
				label: props.texts?.deleteHistoryLabel || '',
				group: 'versions-actions',
				isVisible: canDelete.value
			},
			{
				key: 'create',
				icon: { icon: 'plus' },
				label: props.texts?.createDocument || '',
				group: 'default',
				isVisible: canCreate.value
			},
			{
				key: 'properties',
				icon: { icon: 'properties' },
				label: props.texts?.propertyLabel || '',
				group: 'extra'
			}
		]
	})

	/**
	 * Creates the default structure for the groups in the dropdown
	 */
	const groups = computed<QActionListGroup[]>(() => {
		if (!model.value) return [{ display: 'inline', id: 'default', title: '' }]

		return [
			{ display: 'dropdown', id: 'default', title: '' },
			{ display: 'dropdown', id: 'extra', title: '' }
		]
	})

	return { items, groups }
}
