<template>
	<q-text-field
		:model-value="model"
		:class="props.class"
		:clearable="props.clearable"
		:readonly="props.readonly"
		:size="props.size"
		@change="updateValue"
		@click:clear="updateValue" />
</template>

<script setup lang="ts">
	// Components
	import { QTextField } from '@quidgest/ui/components'

	// Types
	import type { QFilterTextProps } from './types'

	const props = withDefaults(defineProps<QFilterTextProps>(), {
		size: 'medium'
	})

	const emit = defineEmits<{
		(e: 'update:modelValue', val: string): void
	}>()

	const model = defineModel<string>()

	/**
	 * Handles the input change event.
	 * @param event - The input change event.
	 */
	function updateValue(event: Event) {
		const input = event.target as HTMLInputElement
		emit('update:modelValue', input.value)
	}

	defineOptions({
		inheritAttrs: false
	})
</script>
