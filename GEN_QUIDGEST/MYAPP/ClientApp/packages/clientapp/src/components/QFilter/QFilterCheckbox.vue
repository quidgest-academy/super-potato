<template>
	<div
		:class="containerClasses"
		:style="columnStyles">
		<q-checkbox
			v-if="isBoolFilter"
			v-model="model as boolean"
			:label="props.label"
			:readonly="props.readonly" />
		<template v-else>
			<q-row
				v-for="item in props.items"
				:key="'filter-option-' + item.key"
				:gutter="1">
				<q-col cols="auto">
					<q-checkbox
						:label="item.value.toString()"
						:model-value="checkboxValue(item.key)"
						:readonly="props.readonly"
						@update:model-value="(val: boolean) => toggleSelection(item.key, val)" />
				</q-col>
			</q-row>
		</template>
	</div>
</template>

<script setup lang="ts">
	// Components
	import { QCheckbox, QCol, QRow } from '@quidgest/ui/components'

	// Types
	import type { FilterType, FilterValue, QFilterGroupProps } from './types'

	// Utils
	import { computed } from 'vue'

	const props = withDefaults(defineProps<QFilterGroupProps>(), {
		items: () => []
	})

	const model = defineModel<FilterValue>()

	/** True if the filter is based on a boolean field (single checkbox), false otherwise (checkbox group). */
	const isBoolFilter = computed(() => typeof model.value === 'boolean')

	/** Style classes for the checkbox group container. */
	const containerClasses = computed(() => {
		return [
			props.class,
			'q-filter-checkbox',
			!isBoolFilter.value
				? {
					'q-filter-checkbox--group': true,
					'q-filter-checkbox--horizontal': props.orientation === 'horizontal',
					'q-filter-checkbox--grid': props.columns > 1
				}
				: undefined
		]
	})

	/** Style variables that set the number of columns set for the component. Used in the grid layout. */
	const columnStyles = computed(() => {
		return props.columns > 1
			? {
				'--q-filter-checkbox-columns': props.columns
			}
			: undefined
	})

	/**
	 * Determines if the checkbox of each array option is checked.
	 * @param key The option key.
	 */
	function checkboxValue(key: FilterType) {
		if (!Array.isArray(model.value)) return false

		return model.value.includes(key)
	}

	/**
	 * Changes the model value of the filter, based on the state toggle of a checkbox.
	 * @param key The option key.
	 * @param checked True if the checkbox is being checked, false if it is being unchecked.
	 */
	function toggleSelection(key: FilterType, checked: boolean) {
		const filterValues = Array.isArray(model.value)
			? [...model.value]
			: []
		const idx = filterValues.indexOf(key)

		if (checked && idx === -1) filterValues.push(key)
		if (!checked && idx !== -1) filterValues.splice(idx, 1)

		model.value = filterValues
	}

	defineOptions({
		inheritAttrs: false
	})
</script>
