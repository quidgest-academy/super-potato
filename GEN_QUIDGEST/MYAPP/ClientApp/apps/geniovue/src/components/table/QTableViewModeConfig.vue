<template>
	<q-toggle-group
		v-if="items.length > 1"
		v-model="model"
		required
		borderless>
		<q-toggle-group-item
			v-for="item in items"
			:key="item.value"
			:value="item.value"
			:title="item.title">
			<q-icon :icon="item.icon" />
		</q-toggle-group-item>
	</q-toggle-group>
</template>

<script setup>
	import { computed } from 'vue'

	const props = defineProps({
		/**
		 * An array of available view modes that the user can toggle between.
		 */
		viewModes: {
			type: Array,
			default: () => []
		},

		/**
		 * An object containing localized text strings for the titles of the view mode toggle buttons.
		 */
		texts: {
			type: Object,
			required: true
		}
	})

	const model = defineModel({ type: String, required: true })

	const items = computed(() =>
		props.viewModes.map((viewMode) => {
			return {
				value: viewMode.id,
				title: getViewModeButtonTitle(viewMode),
				icon: getViewModeIcon(viewMode)
			}
		})
	)

	function getViewModeIcon(viewMode) {
		// FIXME: generate view mode icon
		return viewMode.id === 'LIST' ? 'list-view' : 'alternative-view'
	}

	function getViewModeButtonTitle(viewMode) {
		// FIXME: generate view mode title
		return viewMode.id === 'LIST'
			? props.texts.toListViewButtonTitle
			: props.texts.toAlternativeViewButtonTitle
	}
</script>
