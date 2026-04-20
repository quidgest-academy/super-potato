<template>
	<a
		tabindex="0"
		role="button"
		:class="classes"
		@click.stop="onAlertClick"
		@keydown.enter.stop="onAlertClick">
		<q-counter-widget
			v-if="widget.data"
			:value="widgetValue"
			:title="widget.Title"
			:icon="icon" />
	</a>
</template>

<script>
	import { messageTypes } from '@quidgest/clientapp/constants/enums'

	import QCounterWidget from './QCounterWidget.vue'

	export default {
		name: 'QAlertWidget',

		emits: ['init', 'navigate-to'],

		components: {
			QCounterWidget
		},

		inheritAttrs: false,

		props: {
			/**
			 * An object containing details about a widget.
			 */
			widget: {
				type: Object,
				default: () => ({})
			}
		},

		expose: [],

		computed: {
			/**
			 * Generates and assigns a specific CSS class depending on the widget data type.
			 */
			classes()
			{
				const baseClass = 'q-alert-widget'
				const classes = [baseClass]

				if (this.widget.data?.type)
					classes.push(`${baseClass}--${this.widget.data.type}`)

				return classes
			},

			/**
			 * Determines the icon to be displayed based on the widget data type.
			 */
			icon()
			{
				switch (this.widget.data?.type)
				{
					case messageTypes.W:
						return 'warning'
					case messageTypes.OK:
						return 'success'
					case messageTypes.I:
						return 'information'
					case messageTypes.E:
						return 'exclamation-sign'
				}

				return ''
			},

			/**
			 * A valid value for the counter widget (can be widget.data.count or widget.data.description).
			 */
			widgetValue()
			{
				return this.widget.data?.count ?? this.widget.data?.description
			}
		},

		methods: {
			/**
			 * Emits a 'navigate-to' event if there's a defined target in the widget data.
			 */
			onAlertClick()
			{
				if (this.widget.data?.target)
					this.$emit('navigate-to', this.widget.data.target)
			}
		},

		watch: {
			'widget.data': {
				handler(data)
				{
					if (data)
					{
						const type = data.type === messageTypes.E ? 'danger' : data.type
						this.$emit('init', { borderColor: type })
					}
				},
				deep: true,
				immediate: true
			}
		}
	}
</script>
