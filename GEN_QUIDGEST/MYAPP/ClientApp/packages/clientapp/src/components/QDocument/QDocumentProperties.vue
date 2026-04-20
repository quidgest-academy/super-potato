<template>
	<q-dialog
		v-model="model"
		class="q-document__popup"
		:title="texts?.propertyLabel"
		:buttons="buttons"
		dismissible
		size="medium">
		<template
			v-if="props.fileProperties"
			#body>
			<q-row
				v-for="property in propList"
				:key="property.name">
				<q-col>
					<q-text-field
						:model-value="props.fileProperties[property.name as keyof FileProperties]"
						:label="property.label"
						readonly
						size="block" />
				</q-col>
			</q-row>
		</template>
	</q-dialog>
</template>

<script setup lang="ts">
	// Components
	import { QCol, QDialog, QRow } from '@quidgest/ui/components'

	// Types
	import type { QButtonProps } from '@quidgest/ui/components'
	import type { FileProperties } from './types'

	const props = defineProps<{
		/**
		 * Necessary strings to be used in labels and buttons.
		 */
		texts: Record<string, string>

		/**
		 * The properties of the document.
		 */
		fileProperties?: FileProperties
	}>()

	const model = defineModel<boolean>()

	const propList = [
		{ name: 'name', label: props.texts?.nameLabel },
		{ name: 'size', label: props.texts?.sizeLabel },
		{ name: 'fileType', label: props.texts?.extensionLabel },
		{ name: 'author', label: props.texts?.authorLabel },
		{ name: 'createdDate', label: props.texts?.createdDateLabel },
		{ name: 'version', label: props.texts?.currentVersionLabel },
		{ name: 'editor', label: props.texts?.editedByLabel }
	]

	const buttons = [
		{
			id: 'ok',
			icon: { icon: 'ok' },
			props: { label: props.texts.okLabel, variant: 'bold' } as QButtonProps
		}
	]
</script>
