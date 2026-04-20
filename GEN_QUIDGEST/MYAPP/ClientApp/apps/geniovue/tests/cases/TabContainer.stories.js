import QTabs from '@/components/containers/TabContainer.vue'

/**
 * Tabs allow for hiding content behind selectable items.
 */
export default {
	title: 'Containers/Tabs',
	component: QTabs,
	tags: ['autodocs']
}

/**
 * Basic tab layout with default options.
 */
export const Simple = {
	args: {
		selectedTab: 1,
		alignTabs: 'left',
		iconAlignment: 'left',
		isVisible: true,
		tabsList: [
			{
				id: 1,
				name: 'Tab one',
				label: 'Tab one',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 2,
				name: 'Tab Two',
				label: 'Tab Two',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 3,
				name: 'Tab Three',
				label: 'Tab Three',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 4,
				name: 'Tab Four',
				label: 'Tab Four',
				isBlocked: false,
				isVisible: true
			}
		]
	},
	render: (args) => ({
		components: { QTabs },

		setup() {
			return { args }
		},

		template: `
			<q-tabs v-bind="args" @tab-changed="(newVal) => args.selectedTab = newVal">
				<template
					v-for="tab in args.tabsList"
					:key="tab.id">
					<section v-show="args.selectedTab === tab.id">
						<p class="panel-text">Content for {{ tab.label }}.</p>
					</section>
				</template>
			</q-tabs>
		`
	})
}

/**
 * Tabs with icons shown on each tab.
 */
export const tabsListWithIcons = {
	args: {
		...Simple.args,
		tabsList: [
			{
				id: 1,
				name: 'Tab One',
				label: 'Tab One',
				isBlocked: false,
				icon: {
					icon: 'user'
				},
				isVisible: true
			},
			{
				id: 2,
				name: 'Tab Two',
				label: 'Tab Two',
				isBlocked: false,
				icon: {
					icon: 'download'
				},
				isVisible: true
			},
			{
				id: 3,
				name: 'Tab Three',
				label: 'Tab Three',
				isBlocked: false,
				icon: {
					icon: 'ok'
				},
				isVisible: true
			},
			{
				id: 4,
				name: 'Tab Four',
				label: 'Tab Four',
				isBlocked: false,
				icon: {
					icon: 'dark-theme'
				},
				isVisible: true
			}
		]
	},
	render: Simple.render
}

/**
 * Tabs list with some tabs hidden using the `isVisible` flag.
 */
export const tabsListWithVisible = {
	args: {
		...Simple.args,
		tabsList: [
			{
				id: 1,
				name: 'Tab one',
				label: 'Tab one',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 2,
				name: 'Tab Two',
				label: 'Tab Two',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 3,
				name: 'Tab Three',
				label: 'Tab Three',
				isBlocked: false,
				isVisible: false
			},
			{
				id: 4,
				name: 'Tab Four',
				label: 'Tab Four',
				isBlocked: false,
				isVisible: true
			}
		]
	},
	render: Simple.render
}

/**
 * Tabs list demonstrating blocked tabs using the `isBlocked` flag.
 */
export const tabsListisBlocked = {
	args: {
		...Simple.args,
		tabsList: [
			{
				id: 1,
				name: 'Tab one',
				label: 'Tab one',
				isBlocked: false,
				content: 'Tab one contents goes here ......',
				isVisible: true
			},
			{
				id: 2,
				name: 'Tab Two',
				label: 'Tab Two',
				isBlocked: true,
				content: 'Tab two contents goes here ......',
				isVisible: true
			},
			{
				id: 3,
				name: 'Tab Three',
				label: 'Tab Three',
				isBlocked: false,
				content: 'Tab three contents goes here ......',
				isVisible: true
			},
			{
				id: 4,
				name: 'Tab Four',
				label: 'Tab Four',
				isBlocked: true,
				content: 'Tab four contents goes here ......',
				isVisible: true
			}
		]
	},
	render: Simple.render
}

/**
 * Tabs list with a large number of tabs to test overflow and layout.
 */
export const multipleTabList = {
	args: {
		...Simple.args,
		tabsList: [
			{
				id: 1,
				name: 'Tab One',
				label: 'Tab One',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 2,
				name: 'Tab Two',
				label: 'Tab Two',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 3,
				name: 'Tab Three',
				label: 'Tab Three',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 4,
				name: 'Tab Four',
				label: 'Tab Four',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 5,
				name: 'Tab Five',
				label: 'Tab Five',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 6,
				name: 'Tab Six',
				label: 'Tab Six',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 7,
				name: 'Tab Seven',
				label: 'Tab Seven',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 8,
				name: 'Tab Eight',
				label: 'Tab Eight',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 9,
				name: 'Tab Nine',
				label: 'Tab Nine',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 10,
				name: 'Tab Ten',
				label: 'Tab Ten',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 11,
				name: 'Tab Eleven',
				label: 'Tab Eleven',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 12,
				name: 'Tab Twelve',
				label: 'Tab Twelve',
				isBlocked: false,
				isVisible: true
			},
			{
				id: 13,
				name: 'Tab Thirteen',
				label: 'Tab Thirteen',
				isBlocked: false,
				isVisible: true
			}
		]
	},
	render: Simple.render
}
