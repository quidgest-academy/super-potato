<template>
	<div
		:id="`${props.id}-container`"
		:class="['q-document', { 'q-document--readonly': readonly }]">
		<q-input-group
			:size="props.size"
			class="q-document__input">
			<!-- Input - only to show the file name -->
			<q-text-field
				:id="props.id"
				:model-value="model"
				data-testid="document-input"
				readonly
				@click="handleInputClick" />

			<!-- Dropdown actions -->
			<q-action-list
				:groups="groups"
				:items="items"
				@click="handleDropdownClick" />
		</q-input-group>

		<!-- Invisible input used to attach files -->
		<input
			:id="`q-document-file-${props.id}`"
			ref="fileAttach"
			class="q-document__attach"
			type="file"
			data-testid="file-input"
			:accept="extensions.join(' ')"
			@change="handleUpdateFileEvent" />

		<div
			v-if="!props.disabled && !props.readonly && props.versioning && props.editing"
			class="q-document__editing">
			<q-icon icon="information" /> {{ props.texts.editingDocument }}
		</div>

		<!-- Dialog with the list of properties -->
		<q-document-properties
			v-model="showProperties"
			:texts="props.texts"
			:file-properties="fileProperties" />

		<!-- Dialog to submit new versions of the document -->
		<q-document-submit
			v-model="showFileSubmit"
			:id="props.id"
			:current-version="currentVersion"
			:texts="props.texts"
			@submit-file="submitFileVersion" />

		<!-- Dialog with the list of current versions -->
		<q-document-versions
			v-model="showVersions"
			:versions-info="props.versionsInfo"
			:texts="props.texts"
			:resources-path="props.resourcesPath"
			@get-file="getFile"
			@delete-last="handleDropdownClick('delete-last')"
			@delete-history="handleDropdownClick('delete-history')" />
	</div>
</template>

<script setup lang="ts">
	// Constants
	import { inputSize } from '../../constants/enums'

	// Utils
	import { ref, nextTick, useTemplateRef } from 'vue'
	import { DEFAULT_TEXTS } from './constants'
	import { useDropdown } from './composables/useDropdown'
	import {
		displayMessage,
		validateFileExtAndSize,
		isEmpty,
		validateTexts
	} from '../../utils/genericFunctions'

	// Components
	import { QActionList } from '../QActionList'
	import QDocumentProperties from './QDocumentProperties.vue'
	import QDocumentSubmit from './QDocumentSubmit.vue'
	import QDocumentVersions from './QDocumentVersions.vue'

	// Types
	import type { QDocumentProps, SubmitFile, GetFile } from './types'

	const props = withDefaults(defineProps<QDocumentProps>(), {
		texts: () => DEFAULT_TEXTS,
		extensions: () => [],
		fileProperties: () => ({}),
		maxFileSize: 0,
		versioning: false,
		disabled: false,
		readonly: false,
		editing: false,
		currentVersion: '1',
		versions: () => ({}),
		versionsInfo: () => [],
		popupIsVisible: false,
		usesTemplates: false
	})

	/**
	 * Validates the component props.
	 */
	function validateProps(): void {
		if (!validateTexts(DEFAULT_TEXTS, props.texts))
			// eslint-disable-next-line no-console
			console.error('Invalid texts prop:', props.texts)
		if (!isEmpty(props.size as string | undefined) && !Reflect.has(inputSize, props.size || ''))
			// eslint-disable-next-line no-console
			console.error('Invalid size prop:', props.size)
	}

	const emit = defineEmits<{
		(e: 'delete-file'): void
		(e: 'delete-history'): void
		(e: 'delete-last'): void
		(e: 'edit-file'): void
		(e: 'file-error', result: number): void
		(e: 'get-file', file: GetFile): void
		(e: 'get-properties'): void
		(e: 'get-version-history'): void
		(e: 'show-templates-popup'): void
		(e: 'submit-file', file: SubmitFile): void
	}>()

	validateProps()

	const model = defineModel<string>()

	// Template Refs
	const fileAttachRef = useTemplateRef('fileAttach')

	// State refs
	const showProperties = ref<boolean>(false)
	const showFileSubmit = ref<boolean>(false)
	const showVersions = ref<boolean>(false)

	/**
	 * Logic to create the actions and groups for the dropdown
	 */
	const { items, groups } = useDropdown(props, model)

	/**
	 * Handles the click event on the dropdown.
	 * @param itemKey The key of the item
	 */
	async function handleDropdownClick(itemKey: string): Promise<void> {
		if (itemKey.includes('version-')) {
			const version = itemKey.replace('version-', '').trim()
			getFile(version)
		}

		switch (itemKey) {
			case 'attach':
				await attachFile()
				break
			case 'download':
				getFile()
				break
			case 'submit':
				setShowFileSubmit(true)
				break
			case 'edit':
				editFile()
				break
			case 'delete':
				deleteFile()
				break
			case 'delete-last':
				deleteFile(true)
				break
			case 'delete-history':
				deleteFile(false, true)
				break
			case 'properties':
				getProperties()
				break
			case 'document-history':
				viewAllVersions()
				break
			case 'create':
				createDocument()
				break
			default:
				break
		}
	}

	/**
	 * Handles the click event on the document input.
	 */
	async function handleInputClick(): Promise<void> {
		if (!model.value) await attachFile()
		else getFile(undefined, false)
	}

	/**
	 * Triggers the file attach window.
	 */
	async function attachFile(): Promise<void> {
		if (props.readonly) return

		// Clears the input before updating (doens't work without this if the model is filed already)
		await nextTick(() => fileAttachRef.value?.click())
	}

	/**
	 * Validates the attached file, if everything's ok calls the callback function, if one is provided.
	 * @param file The file
	 * @param callback The callback function
	 */
	function validateFile(file: File, callback: (file: File) => void): void {
		const validationResult: number = validateFileExtAndSize(
			file,
			props.extensions,
			props.maxFileSize || 0
		)

		if (validationResult === 0 && typeof callback === 'function') callback(file)
		else emit('file-error', validationResult)
	}

	/**
	 * Handles the input event to update the file
	 * @param event The file attach event
	 */
	function handleUpdateFileEvent(event: Event): void {
		const input = event.target as HTMLInputElement
		if (!input?.files?.[0]) return
		updateFile(input.files[0])
	}

	/**
	 * Emits an event with the attached file object
	 * @param file The file
	 * @param newVersion The new version to save
	 * @param unlock Wether or not to unlock the input
	 */
	function updateFile(file: File, newVersion?: string, unlock?: boolean): void {
		function attach(validatedfile: File) {
			emit('submit-file', {
				file: validatedfile,
				version: newVersion || props.currentVersion || '1',
				isNewVersion: unlock ? false : true
			})
		}
		validateFile(file, attach)
	}

	/**
	 * Submits a new file versions
	 * @param payload The payload with the file and new version
	 */
	function submitFileVersion(payload: SubmitFile): void {
		if (!payload.file) return
		updateFile(payload.file, payload.version, payload.unlock)
	}

	/**
	 * Emits an event to update the document to "Edit" mode
	 */
	function editFile(): void {
		emit('edit-file')
	}

	/**
	 * Confirmation window for the deletion of a document.
	 * @param question The question to present to the user
	 * @param action The action to be executed in case the user wants to proceed
	 */
	function deleteValidation(question: string, action: () => void): void {
		const buttons = {
			confirm: { label: props.texts.yesLabel, action },
			cancel: { label: props.texts.noLabel }
		}

		displayMessage(question, 'question', undefined, buttons)
	}

	/**
	 * Emits an event to delete the document attached
	 * @param last Whether or not to delete the last version of the document
	 * @param history Whether or not to delete the document history
	 */
	function deleteFile(last?: boolean, history?: boolean): void {
		if (last && !history)
			deleteValidation(props.texts.theLastVersionWillEliminate, () => emit('delete-last'))
		else if (!last && history)
			deleteValidation(props.texts.allTheVersionsExceptLastWillEliminate, () =>
				emit('delete-history')
			)
		else deleteValidation(props.texts.deleteHeaderLabel, () => emit('delete-file'))
	}

	/**
	 * Emits the event to get the specified version of the document.
	 * @param version The id of the version
	 * @param download Whether to force the file download
	 */
	function getFile(version: string | undefined = undefined, download: boolean = true): void {
		if (!model.value) return

		emit('get-file', { version: version ?? props.currentVersion, download })
	}

	/**
	 * Emits the event to show the verions history.
	 */
	function viewAllVersions(): void {
		emit('get-version-history')
		setShowVersions(true)
	}

	/**
	 * Emits the event to fetch the properties of the document from the server.
	 */
	function getProperties(): void {
		if (!model.value) return
		emit('get-properties')
		setShowProperties(true)
	}

	/**
	 * Emits an event to open the Document Templates pop up (generated next to <q-document />)
	 */
	function createDocument(): void {
		emit('show-templates-popup')
	}

	/**
	 * Sets the overlay to submit a new version
	 * @param visible Wether the overlay is visible or not
	 */
	function setShowFileSubmit(visible: boolean): void {
		showFileSubmit.value = visible
	}

	/**
	 * Sets the overlay to show the properties of the document
	 * @param visible Wether the overlay is visible or not
	 */
	function setShowProperties(visible: boolean): void {
		showProperties.value = visible
	}

	/**
	 * Sets the overlay to show the versions of the document
	 * @param visible Wether the overlay is visible or not
	 */
	function setShowVersions(visible: boolean): void {
		showVersions.value = visible
	}

	defineOptions({
		inheritAttrs: false
	})
</script>
