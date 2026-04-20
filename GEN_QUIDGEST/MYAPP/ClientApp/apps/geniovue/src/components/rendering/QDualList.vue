<template>
	<div class="q-dual-list">
		<div class="q-dual-list__container">
			<div class="q-dual-list__title">
				<span>{{ titleText1 }}</span>
			</div>

			<div class="q-dual-list__filter">
				<input
					v-model="temporaryAvailableFilter"
					:placeholder="`${this.titleText3} ${this.titleText1}...`"
					data-testid="test-filter1" />
				<q-button
					class="q-dual-list__cancel"
					v-if="temporaryAvailableFilter"
					@click="clearFilter1">
					<q-icon icon="remove-sign" />
				</q-button>

				<q-button
					class="q-dual-list__search"
					@click="applyAvailableFilter"
					data-testid="test-search1">
					<q-icon icon="search" />
				</q-button>
			</div>
			<div class="q-dual-list__lists">
				<div
					class="q-dual-list__items"
					v-for="item in filteredAvailableItems"
					:key="item.id"
					@click="toggleSelectionAvailable(item.id)"
					:class="{ 'selected-line': isSelected(item.id, selectedFromAvailable) }"
					data-testid="test-items">
					{{ item.name }}
				</div>
			</div>
		</div>

		<div class="q-dual-list__buttons">
			<q-button
				@click="addAllToSelected"
				data-testid="test-addall">
				<q-icon icon="dual-list-all" />
			</q-button>
			<q-button @click="addSelectedFromAvailable">
				<q-icon icon="arrow-right" />
			</q-button>
			<q-button @click="removeSelectedFromSelected">
				<q-icon icon="arrow-left" />
			</q-button>
			<q-button @click="removeAllFromSelected">
				<q-icon icon="dual-list-all-back" />
			</q-button>
		</div>

		<div class="q-dual-list__container">
			<div class="q-dual-list__title">
				<span>{{ titleText2 }}</span>
			</div>

			<div class="q-dual-list__filter">
				<input
					v-model="temporarySelectedFilter"
					:placeholder="`${this.titleText3} ${this.titleText2}...`"
					data-testid="test-filter2" />

				<q-button
					class="q-dual-list__cancel"
					v-if="temporarySelectedFilter"
					@click="clearFilter2">
					<q-icon icon="remove-sign" />
				</q-button>

				<q-button
					class="q-dual-list__search"
					@click="applySelectedFilter"
					data-testid="test-search2">
					<q-icon icon="search" />
				</q-button>
			</div>

			<div class="q-dual-list__lists">
				<div
					v-for="item in filteredSelectedItems"
					:key="item.id"
					data-testid="test-items2"
					:class="['q-dual-list__items', { 'selected-line': isSelected(item.id, selectedFromSelected) }]"
					@click="toggleSelectionSelected(item.id)">
					{{ item.name }}
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QDualList',

		emits: ['update:modelValue', 'selected-items'],

		props: {
			/**
			 * id for testing
			 */
			dataTestid: String,

			/**
			 * An array of all items 'names', both available and selected
			 */
			items: {
				type: Array,
				required: true,
				validator: (prop) => prop.every((e) => Object.keys(e).includes('name'))
			},

			/**
			 * An array of currently selected item IDs
			 */
			modelValue: {
				type: Array,
				default: () => [],
				validator: (prop) =>
					prop.every((e) => typeof e === 'string' || typeof e === 'number')
			},

			/**
			 * Title for the first list
			 */
			titleList1: {
				type: [String, Function, Object],
				default: 'Available Variables'
			},

			/**
			 * Title for the second list
			 */
			titleList2: {
				type: [String, Function, Object],
				default: 'Given Variables'
			},

			/**
			 * Title for the search input placeholder
			 */
			titleSearch: {
				type: [String, Function, Object],
				default: 'Search'
			},

			/**
			 * Title for the button that emits the selected items.
			 */
			titleButton: {
				type: [String, Function, Object],
				default: 'Submit'
			}
		},

		expose: [],

		data()
		{
			return {
				selectedFromAvailable: [],
				selectedFromSelected: [],
				availableFilter: '',
				selectedFilter: '',
				temporaryAvailableFilter: '',
				temporarySelectedFilter: ''
			}
		},

		computed: {
			// Filtering items that are available but not selected
			availableItems()
			{
				if (!Array.isArray(this.items) || !Array.isArray(this.modelValue)) {
					return []
				}
				return this.items.filter((item) => item && !this.modelValue.includes(item.id))
			},

			// Returns items that have been selected
			selectedItems()
			{
				if (!Array.isArray(this.items) || !Array.isArray(this.modelValue)) {
					return []
				}
				return this.items.filter((item) => item && this.modelValue.includes(item.id))
			},

			// Returns the 'Available Items' list, filtered by the user's search input
			filteredAvailableItems()
			{
				return this.availableItems.filter((item) =>
					item.name.toLowerCase().includes(this.availableFilter.toLowerCase())
				)
			},

			// Returns the 'Selected Items' list, filtered by the user's search input
			filteredSelectedItems()
			{
				return this.selectedItems.filter((item) =>
					item.name.toLowerCase().includes(this.selectedFilter.toLowerCase())
				)
			},

			// Returns the resource text for the title of the first list
			titleText1()
			{
				return this.titleList1
			},

			// Returns the resource text for the title of the second list
			titleText2()
			{
				return this.titleList2
			},

			// Returns the resource text for the 'search', inside of the filter
			titleText3()
			{
				return this.titleSearch
			}
		},

		methods: {
			/**
			 * Toggle the selection of items in the 'available' list.
			 * If the item is already selected, it will be deselected.
			 * If the item is not selected, it will be added to the list of selected items.
			 *
			 * @param {number|string} id - The ID of the item to be toggled.
			 */
			toggleSelectionAvailable(id)
			{
				this.selectedFromSelected = []

				if (this.selectedFromAvailable.includes(id)) {
					this.selectedFromAvailable = this.selectedFromAvailable.filter(
						(itemId) => itemId !== id
					)
				} else {
					this.selectedFromAvailable.push(id)
				}
			},

			/**
			 * Toggle the selection of items in the 'selected' list.
			 * If the item is already selected, it will be deselected.
			 * If the item is not selected, it will be added to the list of selected items.
			 *
			 * @param {number|string} id - The ID of the item to be toggled.
			 */
			toggleSelectionSelected(id)
			{
				this.selectedFromAvailable = []

				if (this.selectedFromSelected.includes(id)) {
					this.selectedFromSelected = this.selectedFromSelected.filter(
						(itemId) => itemId !== id
					)
				} else {
					this.selectedFromSelected.push(id)
				}
			},

			// Checks if a given item (by ID) is selected within a specific list
			isSelected(id, selectedList)
			{
				return selectedList.includes(id)
			},

			// Moves all items from the first list to the second list
			addAllToSelected()
			{
				const allAvailableItemIds = this.availableItems.map((item) => item.id)
				this.$emit('update:modelValue', [...this.modelValue, ...allAvailableItemIds])
			},

			// Moves the selected items from the first list to the second list
			addSelectedFromAvailable()
			{
				this.$emit('update:modelValue', [...this.modelValue, ...this.selectedFromAvailable])
				this.selectedFromAvailable = []
			},

			// Removes the selected items from the second list and places them back to the first list
			removeSelectedFromSelected()
			{
				this.$emit(
					'update:modelValue',
					this.modelValue.filter((id) => !this.selectedFromSelected.includes(id))
				)
				this.selectedFromSelected = []
			},

			// Removes all items from the second list and places them back to the first list
			removeAllFromSelected()
			{
				this.$emit('update:modelValue', [])
			},

			// Clear the filter for the first list
			clearFilter1()
			{
				this.availableFilter = ''
				this.temporaryAvailableFilter = '' // reset the temporary filter as well
			},

			// Clear the filter for the second list
			clearFilter2()
			{
				this.selectedFilter = ''
				this.temporarySelectedFilter = '' // reset the temporary filter as well
			},

			// Emits an event to the parent component with the currently selected items (excluding their IDs)
			emitSelectedItems()
			{
				const selectedItemsObjects = this.items.filter((item) =>
					this.modelValue.includes(item.id)
				)

				// Map over the selected items and return objects without the 'id' property
				const selectedItemsWithoutIds = selectedItemsObjects.map((items) => {
					const res = { ...items }
					delete res.id
					return res
				})

				this.$emit('selected-items', selectedItemsWithoutIds)
			},

			// Applies the temporary available filter value to the actual available filter. Search only happens when this method is activated.
			applyAvailableFilter()
			{
				this.availableFilter = this.temporaryAvailableFilter
			},

			// Applies the temporary selected filter value to the actual selected filter. Search only happens when this method is activated.
			applySelectedFilter()
			{
				this.selectedFilter = this.temporarySelectedFilter
			}
		}
	}
</script>
