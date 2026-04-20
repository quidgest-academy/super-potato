<template>
	<div
		v-show="isVisible"
		:class="[$attrs.class, 'tab-group-container']">
		<div
			v-show="tabsList.length > 0"
			:id="controlId"
			role="tablist"
			:class="containerClasses"
			:aria-labelledby="labelId">
			<template
				v-for="tab in tabsList"
				:key="`${controlId}_${tab.id}`">
				<q-button
					v-if="tab.isVisible"
					:id="getTabComponentId(tab)"
					ref="tabButtons"
					borderless
					:data-testid="getTabComponentId(tab)"
					:disabled="tab.isBlocked"
					:class="[{ active: selectedTab === tab.id }, 'nav-item']"
					:title="tab.label"
					:tabindex="tab.id === selectedTab ? 0 : -1"
					role="tab"
					:aria-selected="tab.id === selectedTab"
					:aria-controls="tab.id"
					@click="() => changeActiveTab(tab)"
					@keydown.stop.prevent.left="selectPrevTab"
					@keydown.stop.prevent.right="selectNextTab"
					@keydown.stop.prevent.home="selectFirstTab"
					@keydown.stop.prevent.end="selectLastTab">
					<span
						:id="`tab_link_${tab.id}`"
						:data-testid="`tab_link_${tab.id}`"
						:class="[
							{
								active: selectedTab === tab.id,
								disabled: tab.isBlocked
							},
							'nav-link'
						]">
						{{ tab.label }}
					</span>
				</q-button>
			</template>
		</div>

		<div
			class="c-tab__item-container"
			tabindex="0">
			<div
				v-if="activeTab"
				class="c-tab__item-content active">
				<slot name="tab-panel"></slot>
			</div>
		</div>
	</div>
</template>

<script>
	import { getCurrentInstance } from 'vue'

	export default {
		name: 'QTabs',

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: {
				type: String,
				default: ''
			},

			/**
			 * Selected tab property to define which tab should be currently selected.
			 */
			selectedTab: {
				type: [String, Number],
				default: ''
			},

			/**
			 * Tabs list array contains object of tabs property.
			 */
			tabsList: {
				type: Array,
				default: () => []
			},

			/**
			 * Align property to define the alignment of tabs (possible values: center, left, right, justify).
			 */
			alignTabs: {
				type: String,
				default: 'left'
			},

			/**
			 * Icon alignment property to define where the icon should append (possible values: left, right).
			 */
			iconAlignment: {
				type: String,
				default: 'left'
			},

			/**
			 * Visible property to hide and show tabs.
			 */
			isVisible: {
				type: Boolean,
				default: true
			}
		},

		emits: ['tab-changed', 'mounted', 'before-unmount'],

		expose: [],

		data() {
			return {
				controlId: this.id || `tab-container-${getCurrentInstance().uid}`
			}
		},

		computed: {
			/**
			 * The id of the component's label.
			 */
			labelId() {
				return `label_${this.controlId}`
			},

			/**
			 * The selectable tabs.
			 */
			selectableTabs() {
				return this.tabsList.filter(
					(tab) => tab.isVisible !== false && tab.isBlocked !== true
				)
			},

			/**
			 * The index of the active tab.
			 */
			activeTabIndex() {
				return this.selectableTabs.findIndex((tab) => tab.id === this.selectedTab)
			},

			/**
			 * The classes to apply to the tabs container.
			 */
			containerClasses() {
				return [
					this.alignTabs === 'center'
						? 'justify-content-center'
						: this.alignTabs === 'justify'
							? 'nav-fill'
							: this.alignTabs === 'right'
								? 'justify-content-end'
								: 'justify-content-start',
					'nav',
					'nav-tabs',
					'c-tab',
					'c-tab__divider',
					'c-tab__list'
				]
			},

			/**
			 * The tab object of the active tab for use in components in a simplified form.
			 */
			activeTab() {
				return this.tabsList[this.activeTabIndex]
			}
		},

		mounted() {
			this.$emit('mounted')
		},

		beforeUnmount() {
			this.$emit('before-unmount')
		},

		methods: {
			/**
			 * The id of the tab component.
			 * @param {Object} tab The selected tab
			 */
			getTabComponentId(tab) {
				return `tab-container-${tab.id}`
			},

			/**
			 * Get reference to a tab's button component.
			 * @param {Object} tab The selected tab
			 */
			getTabComponentRef(tab) {
				return this.$refs?.tabButtons?.find(
					(btnRef) => btnRef.$el.id === this.getTabComponentId(tab)
				)
			},

			/**
			 * Changes the active tab.
			 * @param {Object} tab The selected tab
			 */
			changeActiveTab(tab) {
				if (tab === undefined || tab === null) return
				if (this.selectedTab !== tab.id) this.$emit('tab-changed', tab.id)

				// Get reference to the tab's button component and focus on it
				const buttonRef = this.getTabComponentRef(tab)
				if (typeof buttonRef?.$el.focus !== 'function') return
				buttonRef.$el.focus()
			},

			/**
			 * Changes the active tab to the previous one.
			 */
			selectPrevTab() {
				let newActiveTabIndex = this.activeTabIndex
				if (this.activeTabIndex <= 0) newActiveTabIndex = this.selectableTabs.length - 1
				else newActiveTabIndex--

				this.selectTabIndex(newActiveTabIndex)
			},

			/**
			 * Changes the active tab to the next one.
			 */
			selectNextTab() {
				let newActiveTabIndex = this.activeTabIndex
				if (this.activeTabIndex >= this.selectableTabs.length - 1) newActiveTabIndex = 0
				else newActiveTabIndex++

				this.selectTabIndex(newActiveTabIndex)
			},

			/**
			 * Changes the active tab to the first one.
			 */
			selectFirstTab() {
				this.selectTabIndex(0)
			},

			/**
			 * Changes the active tab to the last one.
			 */
			selectLastTab() {
				this.selectTabIndex(this.selectableTabs.length - 1)
			},

			/**
			 * Changes the active tab to the previous one.
			 * @param {Number} idx Index in the array of selectable tabs
			 */
			selectTabIndex(idx) {
				const tab = this.selectableTabs[idx]
				this.changeActiveTab(tab)
			}
		}
	}
</script>
