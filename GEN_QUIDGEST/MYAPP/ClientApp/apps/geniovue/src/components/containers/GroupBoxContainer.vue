<template>
	<div
		:id="controlId"
		tabindex="-1"
		:class="['c-groupbox', { 'c-groupbox--no-border': noBorder }, $attrs.class]">
		<div
			v-if="label"
			class="c-groupbox__title"
			:id="labelId">
			{{ label }}

			<q-popover-help
				v-if="popoverText"
				:help-control="helpControl"
				:id="id"
				:label="label"
				:texts="texts" />
		</div>

		<q-tooltip-help
			v-if="tooltipText"
			:help-control="helpControl"
			:anchor="anchorId"
			:label="label" />

		<q-subtitle-help
			v-if="subtitleText"
			:help-control="helpControl"
			:label="label" />

		<q-container
			:id="`${controlId}-content`"
			fluid>
			<slot></slot>
		</q-container>
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'
	import HelpControl from '@/mixins/helpControls.js'

	export default {
		name: 'QGroupBoxContainer',

		inheritAttrs: false,

		components: {
			QPopoverHelp: defineAsyncComponent(() => import('@/components/QPopoverHelp.vue')),
			QTooltipHelp: defineAsyncComponent(() => import('@/components/QTooltipHelp.vue')),
			QSubtitleHelp: defineAsyncComponent(() => import('@/components/QSubtitleHelp.vue'))
		},

		mixins: [HelpControl],

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Text strings which might be used to override default texts within the component.
			 */
			texts: Object,

			/**
			 * The group label.
			 */
			label: String,

			/**
			 * Whether or not the group should have a border.
			 */
			noBorder: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || `groupbox-${this._.uid}`
			}
		},

		computed: {
			labelId()
			{
				return `label_${this.controlId}`
			},

			anchorId()
			{
				return this.labelId? `#${this.labelId}` : ''
			}
		}
	}
</script>
