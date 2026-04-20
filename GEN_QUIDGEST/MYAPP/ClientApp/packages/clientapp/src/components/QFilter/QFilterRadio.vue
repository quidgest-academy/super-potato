<template>
	<q-radio-group
		v-model="model"
		:class="props.class"
		:columns="props.columns"
		:orientation="props.orientation"
		:readonly="props.readonly">
		<q-radio-button
			v-for="item in props.items"
			:key="'filter-option-' + item.key"
			:value="item.key"
			:label="item.value.toString()" />
	</q-radio-group>
</template>

<script setup lang="ts">
	// Components
	import { QRadioButton, QRadioGroup } from '@quidgest/ui/components'

	// Types
	import type { FilterValue, QFilterGroupProps } from './types'

	const props = withDefaults(defineProps<QFilterGroupProps>(), {
		items: () => []
	})

	const model = defineModel<FilterValue>()

	defineOptions({
		inheritAttrs: false
	})
</script>
