<template>
	<div :class="[$attrs.class, 'tab-group-container']">
		<div
			v-show="tabsList.length > 0"
			role="tablist"
			:id="controlId"
			:class="containerClasses"
			:aria-label="texts.panels">
			<template
				v-for="tab in tabsList"
				:key="`${controlId}_${tab.id}`">
				<q-button
					v-if="tab.isVisible"
					ref="tabButtons"
					borderless
					:id="getTabComponentId(tab)"
					:data-testid="`tab-container-${tab.id}`"
					:disabled="tab.isBlocked"
					:class="[{ active: selectedTab === tab.id }, 'nav-item']"
					:style="{ cursor: selectedTab === tab.id ? 'text' : 'pointer' }"
					:title="tab.label"
					:tabindex="tab.id === selectedTab ? 0 : -1"
					role="tab"
					:aria-selected="tab.id === selectedTab"
					:aria-controls="tab.id"
					@click="changeActiveTab(tab)"
					@keydown.stop.prevent.left="selectPrevTab"
					@keydown.stop.prevent.right="selectNextTab"
					@keydown.stop.prevent.home="selectTabIndex(0)"
					@keydown.stop.prevent.end="selectTabIndex(selectableTabs.length - 1)">
					<span
						:id="`${controlId}-tab-content-${tab.id}`"
						:data-val-required="tab.isRequired"
						:class="[
							{
								active: selectedTab === tab.id,
								disabled: tab.isBlocked
							},
							'nav-link'
						]">
						<q-icon
							v-if="tab.icon && iconAlignment !== 'right'"
							v-bind="tab.icon" />

						{{ tab.label }}

						<q-icon
							v-if="tab.icon && iconAlignment === 'right'"
							v-bind="tab.icon" />
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
				<q-subtitle-help
					v-if="activeTab.helpControl"
					:help-control="activeTab.helpControl" />
				<slot />
			</div>
		</div>
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

	// Default texts
	const DEFAULT_TEXTS = {
		panels: 'Panels'
	}

	export default {
		name: 'QTabs',

		emits: [
			'tab-changed',
			'mounted',
			'before-unmount'
		],

		inheritAttrs: false,

		components: {
			QSubtitleHelp: defineAsyncComponent(() => import('@/components/QSubtitleHelp.vue'))
		},

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Selected tab property to define which tab should be currently selected.
			 */
			selectedTab: [String, Number],

			/**
			 * Tabs list array contains object of tabs property.
			 */
			tabsList: Array,

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
			 * Localization and customization of textual content.
			 */
			texts: {
				type: Object,
				validator: (value) => genericFunctions.validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || `tabs-container-${this._.uid}`
			}
		},

		mounted()
		{
			this.$emit('mounted')
		},

		beforeUnmount()
		{
			this.$emit('before-unmount')
		},

		computed: {
			/**
			 * The selectable tabs.
			 */
			selectableTabs()
			{
				return this.tabsList.filter((tab) => tab.isVisible !== false && tab.isBlocked !== true)
			},

			/**
			 * The index of the active tab.
			 */
			activeTabIndex()
			{
				return this.selectableTabs.findIndex((tab) => tab.id === this.selectedTab)
			},

			/**
			 * The classes to apply to the tabs container.
			 */
			containerClasses()
			{
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
			 * The index of the active tab for use in components in a simplified form.
			 */
			activeTab()
			{
				return this.tabsList[this.activeTabIndex]
			}
		},

		methods: {
			/**
			 * The id of the tab component.
			 * @param {Object} tab The selected tab
			 */
			getTabComponentId(tab)
			{
				return `${this.controlId}-tab-${tab.id}`
			},

			/**
			 * Get reference to a tab's button component.
			 * @param {Object} tab The selected tab
			 */
			getTabComponentRef(tab)
			{
				return this.$refs?.tabButtons?.find((btnRef) => btnRef.$el.id === this.getTabComponentId(tab))
			},

			/**
			 * Changes the active tab.
			 * @param {Object} tab The selected tab
			 */
			changeActiveTab(tab)
			{
				if (tab === undefined || tab === null)
					return
				if (this.selectedTab !== tab.id)
					this.$emit('tab-changed', tab.id)

				// Get reference to the tab's button component and focus on it
				const buttonRef = this.getTabComponentRef(tab)
				if (typeof buttonRef?.$el.focus !== 'function')
					return
				buttonRef.$el.focus()
			},

			/**
			 * Changes the active tab to the previous one.
			 */
			selectPrevTab()
			{
				let newActiveTabIndex = this.activeTabIndex
				if (this.activeTabIndex <= 0)
					newActiveTabIndex = this.selectableTabs.length - 1
				else
					newActiveTabIndex--

				this.selectTabIndex(newActiveTabIndex)
			},

			/**
			 * Changes the active tab to the next one.
			 */
			selectNextTab()
			{
				let newActiveTabIndex = this.activeTabIndex
				if (this.activeTabIndex >= this.selectableTabs.length - 1)
					newActiveTabIndex = 0
				else
					newActiveTabIndex++

				this.selectTabIndex(newActiveTabIndex)
			},

			/**
			 * Changes the active tab to the one with the specified ID.
			 * @param {Number} idx Index in the array of selectable tabs
			 */
			selectTabIndex(idx)
			{
				const tab = this.selectableTabs[idx]
				this.changeActiveTab(tab)
			}
		}
	}
</script>
