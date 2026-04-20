<template>
	<q-row
		:class="`q-cards-grid--${props.gridMode}`"
		:justify="props.containerAlignment">
		<slot></slot>
	</q-row>
</template>

<script setup lang="ts">
	// Components
	import { QRow } from '@quidgest/ui/components'

	// Types
	import type { QCardGridProps } from './types'

	const props = withDefaults(defineProps<QCardGridProps>(), {
		gridMode: 'fixed',
		containerAlignment: 'start'
	})

	defineOptions({
		inheritAttrs: false
	})
</script>
