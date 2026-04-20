<template>
	<div
		:id="ctrlId"
		:class="[`input-${size}`, 'i-list-editor']">
		<q-label
			v-if="label"
			:for="ctrlId"
			:label="label">
			<template
				v-if="helpText"
				#append>
				<q-icon
					v-if="helpText"
					class="field-help"
					icon="information-outline"
					:title="helpText" />
			</template>
		</q-label>
		<div class="i-list-editor__content">
			<ul class="i-list-editor__list">
				<li
					v-for="(_, index) in localItems"
					:key="index"
					class="i-list-editor__item">
					<q-text-field
						v-model="localItems[index]"
						class="i-list-editor__item-field"
						size="block"
						:label="`${defaultEditText} ${index + 1}`"
						:readonly="isReadOnly"
						@update:model-value="(value) => updateItem(index, value)" />
					<q-button
						v-if="!isReadOnly"
						class="i-list-editor__item-remove"
						variant="text"
						:title="`${defaultRemoveText} ${index + 1}`"
						@click="() => removeItem(index)">
						<q-icon icon="bin" />
					</q-button>
				</li>
			</ul>
			<q-button
				v-if="!isReadOnly"
				class="i-list-editor__add-button"
				variant="bold"
				:label="defaultAddText"
				@click="addItem">
				<q-icon icon="add" />
			</q-button>
		</div>
	</div>
</template>

<script>
	import { ref, watch, defineComponent, getCurrentInstance } from 'vue'

	export default defineComponent({
		name: 'ListEditor',
		props: {
			/**
			 * Component id.
			 */
			id: {
				type: String,
				default: ''
			},

			/**
			 * Component value.
			 */
			modelValue: {
				type: Array,
				default: () => []
			},

			/**
			 * Component size.
			 */
			size: {
				type: String,
				default: 'xxlarge'
			},

			/**
			 * True if the input should be in a read-only state, false otherwise.
			 */
			isReadOnly: {
				type: Boolean,
				default: false
			},

			/**
			 * Component label.
			 */
			label: {
				type: String,
				default: null
			},

			/**
			 * Content of the component's help.
			 */
			helpText: {
				type: String,
				default: null
			},

			/**
			 * Text of the add button title.
			 */
			addText: {
				type: String,
				default: 'Add item'
			},

			/**
			 * Text of the remove button title.
			 */
			removeText: {
				type: String,
				default: 'Remove item'
			},

			/**
			 * Text of the edit button label.
			 */
			editText: {
				type: String,
				default: 'Edit item'
			}
		},

		emits: ['update:modelValue'],

		expose: [],

		setup(props, { emit }) {
			const vm = getCurrentInstance()
			const ctrlId = props.id || 'input_' + vm.uid

			// Local copy of the items for reactive editing
			const localItems = ref(Array.isArray(props.modelValue) ? [...props.modelValue] : [])

			// Watch for changes in props.items to keep LocalItems in sync
			watch(
				() => props.modelValue,
				(newItems) => {
					localItems.value = Array.isArray(newItems) ? [...newItems] : []
				},
				{ immediate: true }
			)

			// Emits the updated list to parent component
			const updateItem = (index, value) => {
				localItems.value[index] = value
				emit('update:modelValue', localItems.value)
			}

			// Add a new empty item to the list
			const addItem = () => {
				if (props.isReadOnly) return
				localItems.value.push('')
				emit('update:modelValue', localItems.value)
			}

			// Remove an item by index
			const removeItem = (index) => {
				if (props.isReadOnly) return
				localItems.value.splice(index, 1)
				emit('update:modelValue', localItems.value)
			}

			return {
				ctrlId,
				localItems,
				updateItem,
				addItem,
				removeItem,
				defaultAddText: props.addText,
				defaultRemoveText: props.removeText,
				defaultEditText: props.editText
			}
		}
	})
</script>
