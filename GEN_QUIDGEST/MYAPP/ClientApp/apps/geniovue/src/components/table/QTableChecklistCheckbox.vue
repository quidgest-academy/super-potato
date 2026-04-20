<template>
	<q-checkbox
		:model-value="value"
		:id="rowKey !== undefined ? `${tableName}_${rowKey}` : `${tableName}_all`"
		:disabled="disabled"
		:readonly="readonly"
		:size="checkBoxSize"
		:title="title"
		:data-table-action-selected="rowKey ? false : null"
		tabindex="-1"
		@update:model-value="onSelect" />
</template>

<script setup lang="ts">
	import type QCheckboxLabelSize from '@quidgest/ui/components'

	/**
	 * Typed props for QTableChecklistCheckbox
	 */
	type QTableChecklistCheckboxProps = {
		/** The current value or state of the checkbox, indicating whether it's checked (true) or unchecked (false). */
		value?: boolean

		/** A unique identifier or name associated with the parent table of the checkbox. */
		tableName: string

		/** The key or identifier for the specific row in the table. If not provided, the checkbox is placed on the header. */
		rowKey?: string | number

		/** Indicates whether the table is in a read-only state, which will affect the checkbox's interactivity. */
		readonly?: boolean

		/** A flag indicating whether the checkbox should be manually disabled, independent of the table's read-only state. */
		disabled?: boolean

		/** Text for the title attribute. */
		title?: string

		/** Check box size */
		checkBoxSize?: QCheckboxLabelSize
	}

	const props = withDefaults(defineProps<QTableChecklistCheckboxProps>(), {
		checkBoxSize: 'regular'
	})

	const emit = defineEmits<{
		(e: 'toggle-row-selected'): void
	}>()

	function onSelect() {
		if (props.rowKey !== undefined) {
			emit('toggle-row-selected')
		}
	}
</script>
