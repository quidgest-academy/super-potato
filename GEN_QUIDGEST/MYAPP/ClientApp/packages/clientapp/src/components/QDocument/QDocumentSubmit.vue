<template>
	<q-dialog
		v-model="model"
		class="q-document__popup"
		:title="texts.filesSubmission"
		:buttons="buttons"
		dismissible
		size="medium">
		<template #body>
			<q-row>
				<q-col>
					<q-file-upload
						:label="texts.submitHeaderLabel"
						size="block"
						v-model="submitValue" />
				</q-col>
			</q-row>

			<q-divider />

			<q-row>
				<q-col>
					<q-radio-group
						:id="`q-document-submit-options-${props.id}`"
						v-model="versionSubmitType"
						orientation="vertical">
						<q-radio-button
							v-for="radio in versionSubmitOptions"
							:key="radio.key"
							:label="radio.value"
							:value="radio.key" />
					</q-radio-group>
				</q-col>
			</q-row>

			<q-divider />

			<q-row>
				<q-col>
					<q-radio-group
						:id="`q-document-version-options-${props.id}`"
						v-model="versionType"
						:disabled="versionSubmitType === versionSubmitTypes.unlock"
						orientation="vertical">
						<q-radio-button
							v-for="radio in versionOptions"
							:key="radio.key"
							:value="radio.key"
							:label="radio.value" />
					</q-radio-group>
				</q-col>
			</q-row>
		</template>
	</q-dialog>
</template>

<script setup lang="ts">
	// Components
	import {
		QCol,
		QDialog,
		QDivider,
		QFileUpload,
		QRadioButton,
		QRadioGroup,
		QRow
	} from '@quidgest/ui/components'

	// Utils
	import { ref, computed } from 'vue'
	import { displayMessage } from '../../utils/genericFunctions'

	// Types
	import type { QButtonProps } from '@quidgest/ui/components'
	import type { SubmitFile } from './types'

	const props = defineProps<{
		/**
		 *  Unique identifier for the control
		 */
		id: string

		/**
		 * Necessary strings to be used in labels and buttons.
		 */
		texts: Record<string, string>

		/**
		 * Current version of the document.
		 */
		currentVersion?: string
	}>()

	const emit = defineEmits<{
		(e: 'submit-file', payload: SubmitFile): void
	}>()

	const buttons = [
		{
			id: 'submit',
			icon: { icon: 'upload-img' },
			props: { label: props.texts.submitLabel, variant: 'bold' } as QButtonProps,
			action: submitFileVersion
		},
		{
			id: 'cancel',
			icon: { icon: 'cancel' },
			props: { label: props.texts.cancelLabelValue } as QButtonProps
		}
	]

	const model = defineModel<boolean>()
	const submitValue = ref<File>()

	const versionTypes = { minor: 'minor', major: 'major' }
	const versionOptions = [
		{ key: versionTypes.major, value: props.texts.majorVersionLabel },
		{ key: versionTypes.minor, value: props.texts.minorVersionLabel }
	]
	const versionType = ref<string>(versionTypes.major)

	const versionSubmitTypes = { unlock: 'unlock', submit: 'submit' }
	const versionSubmitOptions = [
		{ key: versionSubmitTypes.unlock, value: props.texts.unlockHeaderLabel },
		{ key: versionSubmitTypes.submit, value: props.texts.submitFilesHeaderLabel }
	]
	const versionSubmitType = ref<string>(versionSubmitTypes.submit)

	/**
	 * Computes the next version to submit
	 */
	const versionToSubmit = computed(() => {
		const version = Number(props.currentVersion || '1')
		return versionType.value === versionTypes.major
			? (Math.floor(version) + 1).toString()
			: (version + 0.1).toFixed(1)
	})

	/**
	 * Emits the event with the newly submitted version of the document.
	 */
	function submitFileVersion() {
		if (versionSubmitType.value === versionSubmitTypes.submit && !submitValue.value) {
			displayMessage(props.texts.noFileSelected) // Show popup for empty data
			return
		}

		emit('submit-file', {
			file: submitValue.value as File,
			isNewVersion: versionSubmitType.value === versionSubmitTypes.submit,
			version: versionToSubmit.value,
			unlock: versionSubmitType.value === versionSubmitTypes.unlock
		})

		submitValue.value = undefined
		versionType.value = versionTypes.major
		versionSubmitType.value = versionSubmitTypes.submit
	}
</script>
