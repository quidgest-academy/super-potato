<template>
	<q-row-container>
		<q-control-wrapper class="control-join-group">
			<base-input-structure
				id="CTRL_1"
				label="TinyMCE Text Editor"
				:label-attrs="{ class: 'i-text__label' }"
				:class="['i-text', { 'i-text--disabled': false }]">
				<q-text-editor
					id="CTRL_1"
					v-model="modelValue" />
			</base-input-structure>
		</q-control-wrapper>
	</q-row-container>
</template>

<script>
	export default {
		name: 'QTextEditorContainer',

		docsfile: './docs/inputs/TextEditor.md',

		expose: [],

		data()
		{
			return {
				modelValue: ''
			}
		}
	}
</script>
