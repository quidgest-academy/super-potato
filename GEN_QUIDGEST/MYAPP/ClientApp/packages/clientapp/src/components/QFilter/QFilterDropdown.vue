<template>
	<q-combobox
		v-model="model"
		item-label="value"
		item-value="key"
		:class="props.class"
		:clearable="props.clearable"
		:groups="props.groups"
		:items="props.items"
		:readonly="props.readonly"
		:size="props.size" />
</template>

<script setup lang="ts">
	// Components
	import { QCombobox } from '@quidgest/ui/components'

	// Types
	import type { FilterValue, QFilterDropdownProps } from './types'

	const props = withDefaults(defineProps<QFilterDropdownProps>(), {
		groups: () => [],
		items: () => [],
		size: 'medium'
	})

	const model = defineModel<FilterValue>()

	defineOptions({
		inheritAttrs: false
	})
</script>
