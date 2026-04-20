<template>
	<q-input-group
		:id="props.id"
		:class="props.class"
		:size="size"
		data-testid="debounce-container"
		:data-debouncing="debouncing">
		<q-combobox
			v-bind="$attrs"
			:model-value="value"
			:items="items"
			:clearable="props.clearable"
			:loading="props.loading"
			:readonly="props.readonly"
			:disabled="props.disabled"
			:item-value="props.itemValue"
			:item-label="props.itemLabel"
			:filter-mode="props.filterMode"
			:empty-value="props.emptyValue"
			:texts="props.texts"
			:required="props.required"
			@show="emit('show')"
			@hide="emit('hide')"
			@before-show="emit('before-show')"
			@before-hide="emit('before-hide')"
			@update:model-value="onValueChange"
			@update:search="onInputChange">
			<template
				v-if="items.length && !props.loading"
				#[`body.append`]>
				<div class="q-lookup__results">
					{{ items.length }} / {{ props.totalRows ?? '...' }}
				</div>
			</template>
		</q-combobox>

		<template
			v-if="showSeeMore || showViewDetails"
			#append>
			<q-button
				v-if="showSeeMore"
				:id="`${id}_see-more_button`"
				:disabled="props.disabled"
				:title="texts.viewMoreOptions"
				data-testid="SeeMore"
				@click="emit('see-more')">
				<q-icon icon="list" />
			</q-button>

			<q-button
				v-if="showViewDetails"
				:id="`${id}_show-details_button`"
				:disabled="props.disabled || isEmpty"
				:title="texts.viewDetails"
				@click="emit('view-details', value)">
				<q-icon icon="go-to" />
			</q-button>
		</template>
	</q-input-group>
</template>

<script setup>
	import { computed, ref, watch } from 'vue'
	import _isEmpty from 'lodash-es/isEmpty'

	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'
	import { inputSize } from '@quidgest/clientapp/constants/enums'

	const emit = defineEmits([
		'update:modelValue',
		'before-show',
		'before-hide',
		'show',
		'hide',
		'on-search',
		'see-more',
		'view-details'
	])

	const props = defineProps({
		/**
		 * Model value.
		 */
		modelValue: [String, Number, Object],

		/**
		 * The unique control identifier.
		 */
		id: String,

		/**
		 * The list of available items for selection.
		 */
		items: {
			type: Array,
			default: () => []
		},

		/**
		 * The actual total count of rows in the database.
		 */
		totalRows: {
			type: Number,
			default: undefined
		},

		/**
		 * Property on each item that contains its value.
		 */
		itemValue: {
			type: String,
			default: 'value'
		},

		/**
		 * Property on each item that contains its title.
		 */
		itemLabel: {
			type: String,
			default: 'label'
		},

		/**
		 * The value to be used in comparisons,
		 * used to check if the field has a selected item.
		 */
		emptyValue: {
			type: [String, Number, Object],
			default: undefined
		},

		/**
		 * Necessary strings to be used in labels and buttons.
		 */
		texts: {
			type: Object,
			validator: (value) => validateTexts(DEFAULT_TEXTS, value),
			default: () => DEFAULT_TEXTS
		},

		/**
		 * The size category of the field.
		 */
		size: {
			type: String,
			default: 'medium',
			validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
		},

		/**
		 * The mode of the combobox filter.
		 */
		filterMode: {
			type: String,
			default: 'builtin',
			validator: (value) => ['manual', 'builtin'].includes(value)
		},

		/**
		 * Whether to show the «See more» button.
		 */
		showSeeMore: {
			type: Boolean,
			default: false
		},

		/**
		 * Whether to show the «View details» button.
		 */
		showViewDetails: {
			type: Boolean,
			default: false
		},

		/**
		 * Whether the items of the list are being loaded.
		 */
		loading: {
			type: Boolean,
			default: false
		},

		/**
		 * Whether the value of the component can be cleared.
		 */
		clearable: {
			type: Boolean,
			default: true
		},

		/**
		 * Whether the field is readonly.
		 */
		readonly: {
			type: Boolean,
			default: false
		},

		/**
		 * Whether the field is disabled.
		 */
		disabled: {
			type: Boolean,
			default: false
		},

		/**
		 * Custom set of classes to apply to the component.
		 */
		class: {
			type: [String, Array],
			default: undefined
		},

		/**
		 * Whether the field is required.
		 */
		required: {
			type: Boolean,
			default: false
		},

		/**
		 * Whether the control is currently debouncing.
		 */
		debouncing: {
			type: Boolean,
			default: false
		}
	})

	// Proxy model
	const value = ref(props.modelValue)
	watch(
		() => props.modelValue,
		(newVal) => {
			// Remove any existing empty items
			items.value = items.value.filter((item) => !item.isPlaceholder)

			// Add the item if it does not exist
			if (newVal && !items.value.some((item) => item[props.itemValue] === newVal)) {
				const newItem = {}

				newItem[props.itemValue] = newVal
				newItem[props.itemLabel] = '--'
				newItem.isPlaceholder = true

				items.value.push(newItem)
			}

			value.value = newVal
		}
	)

	// Proxy items
	const items = ref(props.items)
	watch(
		() => props.items,
		(newVal) => {
			items.value = newVal
		},
		{ deep: true }
	)

	/**
	 * The currently selected item.
	 */
	const selectedItem = computed(() =>
		items.value?.find((item) => item[props.itemValue] === value.value)
	)

	/**
	 * Whether the field has a selected item.
	 */
	const isEmpty = computed(() => selectedItem.value === undefined)

	/**
	 * Handles the change of a value and emits an update event.
	 *
	 * @param {String} newVal - The new value to be set.
	 */
	function onValueChange(newVal) {
		if (value.value === newVal)
			return

		value.value = newVal

		emit('update:modelValue', newVal)
	}

	/**
	 * Handles the change of an input value and emits a search event.
	 *
	 * @param {String} newVal - The new value from the input.
	 */
	function onInputChange(newVal) {
		// Prevent an immediate reload on item selection
		if (newVal !== selectedItem.value?.[props.itemLabel]) {
			emit('on-search', newVal)
		}
	}

	defineOptions({
		inheritAttrs: false
	})
</script>

<script>
	const DEFAULT_TEXTS = {
		noData: 'No data to show',
		viewDetails: 'View details',
		viewMoreOptions: 'View more options'
	}
</script>
