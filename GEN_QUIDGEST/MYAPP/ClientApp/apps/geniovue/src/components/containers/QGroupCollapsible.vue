<template>
	<q-collapsible
		v-model="curValue"
		:id="`${controlId}-container`"
		:class="$attrs.class"
		:title="label"
		:subtitle="subtitleText"
		:width="size"
		:required="isRequired"
		:spacing="spacing"
		:variant="variant">
		<slot />
	</q-collapsible>
</template>

<script>
	import HelpControl from '@/mixins/helpControls.js'

	export default {
		name: 'QGroupCollapsible',

		inheritAttrs: false,

		mixins: [HelpControl],

		emits: ['update:modelValue'],

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Wether the collapsible is open
			 */
			modelValue: {
				type: Boolean,
				default: false
			},

			/**
			 * The title of the group.
			 */
			label: {
				type: String,
				required: true
			},

			/**
			 * The width of the container
			 */
			size: {
				type: String,
				default: 'block'
			},

			/**
			 * Whether or not the collapsible group contains required fields.
			 */
			isRequired: {
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
			curValue: {
				get()
				{
					return this.modelValue
				},
				set(newValue)
				{
					this.$emit('update:modelValue', newValue)
				}
			},

			containerSize()
			{
				return this.size ? 'block' : 'fit-content'
			},

			variant()
			{
				return this.$attrs.class?.includes('--audit') ? 'border-bottom' : 'default'
			},

			spacing()
			{
				return this.$attrs.class?.includes('--audit') ? 'compact' : 'comfortable'
			}
		}
	}
</script>
