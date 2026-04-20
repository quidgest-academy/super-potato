<template>
	<q-card-view
		v-if="props.variant === 'image'"
		v-bind="props"
		class="q-card-view--insert q-card-view--insert-image"
		@click="emit('click')">
		<template #image>
			<img
				role="cell"
				loading="lazy"
				decoding="async"
				:alt="props.texts.cardImage"
				:src="props.src" />
		</template>
		<template
			#title
			v-if="props.tableName">
			{{ props.texts.createText }}
			{{ props.tableName.toLowerCase() }}
		</template>
		<template #subtitle>
			<q-button @click="emit('click')">
				<q-icon icon="add"></q-icon>
				{{ props.texts.insertText }}
			</q-button>
		</template>
	</q-card-view>

	<q-card-view
		v-else
		v-bind="props"
		:class="['q-card-view--insert', `q-card-view--insert-${props.variant}`]"
		@click="emit('click')">
		<template #title></template>
		<template #subtitle></template>
		<template #text></template>
		<template #image></template>
		<template #underlay>
			<span>
				<q-icon icon="add" />
				{{ props.texts.insertText }}
			</span>
		</template>
	</q-card-view>
</template>

<script setup lang="ts">
	// Constants
	import { DEFAULT_TEXTS } from './constants'

	// Components
	import QCardView from './QCardView.vue'

	// Types
	import type { QInsertCardProps } from './types'

	const props = withDefaults(defineProps<QInsertCardProps>(), {
		variant: 'secondary',
		config: () => ({}),
		texts: () => DEFAULT_TEXTS
	})

	const emit = defineEmits<{
		(e: 'click'): void
	}>()
</script>
