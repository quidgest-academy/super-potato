<template>
	<div class="q-custom-widget">
		<component
			v-if="widget && hasData"
			:is="widget.Component"
			:key="key + domVersionKey"
			v-bind="params" />
		<div
			v-else
			class="no-widgets">
			<img
				class="no-widgets__image"
				:src="`${config.resourcesPath}no-widgets.png`"
				:alt="texts.noDataText" />

			<h2 class="no-widgets__message">
				{{ texts.noDataText }}
			</h2>
		</div>
	</div>
</template>

<script>
	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		noDataText: 'No data to show'
	}

	export default {
		name: 'QCustomWidget',

		inheritAttrs: false,

		props: {
			/**
			 * The widget object containing configuration and state.
			 */
			widget: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The current page number used for paging through keys, if applicable.
			 */
			page: {
				type: Number,
				default: 0
			},

			/**
			 * A version key used to force DOM to recreate the component when changed.
			 */
			domVersionKey: {
				type: Number,
				default: 0
			},

			/**
			 * An object containing localised text strings.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Configuration object, including paths to resources.
			 */
			config: {
				type: Object,
				default: () => ({})
			}
		},

		expose: [],

		computed: {
			/**
			 * Determines if paging is used based on whether Keys is an array inside the widget.
			 */
			usePages()
			{
				return Array.isArray(this.widget.Keys)
			},

			/**
			 * Checks if the widget contains data to display, depending on paging usage.
			 */
			hasData()
			{
				return !this.usePages || this.widget.Keys.length !== 0
			},

			/**
			 * Retrieves the correct key based on the current page, or the Rowkey from the widget.
			 */
			key()
			{
				if (this.usePages)
					return this.widget.Keys.length >= this.page ? this.widget.Keys[this.page] : null

				return this.widget.Rowkey
			},

			/**
			 * Builds parameters for binding to the component, including ID, mode, and nested flag.
			 */
			params()
			{
				return {
					id: this.key,
					mode: 'SHOW',
					isNested: true
				}
			}
		}
	}
</script>
