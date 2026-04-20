<template>
	<a
		role="button"
		class="q-menu-widget"
		tabindex="0"
		@keydown.enter.stop="onClick"
		@click.stop="onClick">
		<h2 class="q-menu-widget__title">{{ widget.Title }}</h2>
		<q-icon
			v-if="getMenuIcon(widget.MenuEntry)"
			v-bind="getMenuIcon(widget.MenuEntry)" />
		<q-icon
			v-else
			:icon="defaultIcon" />
	</a>
</template>

<script>
	import VueNavigation from '@/mixins/vueNavigation.js'
	import menuAction from '@/mixins/menuAction.js'
	import LayoutHandlers from '@/mixins/layoutHandlers'

	export default {
		name: 'QMenuWidget',

		mixins: [VueNavigation, menuAction, LayoutHandlers],

		inheritAttrs: false,

		props: {
			/**
			 * The widget.
			 */
			widget: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The path to the resources.
			 */
			resourcesPath: String
		},

		expose: [],

		methods: {
			onClick()
			{
				if (this.widget.MenuEntry)
					this.executeMenuAction(this.widget.MenuEntry)
			}
		},

		computed: {
			defaultIcon()
			{
				// If the widget of type favorite, return the bookmark icon, otherwise return the go-to icon.
				return this.widget.Type === 1 ? 'bookmark' : 'go-to2'
			}
		}
	}
</script>
