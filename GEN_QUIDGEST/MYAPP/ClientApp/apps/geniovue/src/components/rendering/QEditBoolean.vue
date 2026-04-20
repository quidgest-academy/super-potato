<template>
	<q-checkbox
		:id="`${props.tableName}_${props.rowIndex}_${props.columnName}`"
		:class="classes"
		:model-value="props.value"
		data-table-action-selected="false"
		tabindex="-1"
		:readonly="props.options.readonly"
		:aria-label="props.options.label"
		@update:model-value="updateValue" />
</template>

<script setup lang="ts">
	import { onMounted } from 'vue'

	type Options = {
		readonly?: boolean
		label?: string
	}

	type QEditBooleanProps = {
		/** * The checked value of the checkbox, can be a boolean or a number corresponding to true or false. */
		value?: boolean | number
		/** * The name of the table in the database, used to construct the checkbox ID. */
		tableName: string
		/** * The index of the current row, used together with tableName and columnName to construct the checkbox ID. */
		rowIndex: number | string
		/** * The name of the column in the database, used to construct the checkbox ID. */
		columnName: string
		/** * Options for the checkbox such as readOnly status. */
		options?: Options
		/** * An array of additional classes to apply to the checkbox. */
		classes?: string[]
	}

	const props = defineProps<QEditBooleanProps>()

	const emit = defineEmits<{
		(e: 'update', value: number): void
		(e: 'loaded'): void
	}>()

	onMounted(() => {
		emit('loaded')
	})

	function updateValue(event: boolean | number) {
		let newValue = 0

		if (typeof event === 'number') newValue = event === 0 ? 0 : 1
		else if (typeof event === 'boolean') newValue = event ? 1 : 0

		emit('update', newValue)
	}
</script>
