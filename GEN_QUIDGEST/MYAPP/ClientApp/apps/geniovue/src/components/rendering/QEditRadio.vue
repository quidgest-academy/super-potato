<template>
	<q-radio-group
		:class="containerClasses"
		:model-value="options.checkedValue"
		:name="options.name"
		:readonly="options.readonly || !editable"
		@update:model-value="update">
		<q-radio-button
			:value="row.value"
			:label="options.optionLabel"
			data-table-action-selected="false"
			tabindex="-1" />
	</q-radio-group>
</template>

<script>
	export default {
		name: 'QEditRadio',

		emits: ['loaded', 'update-external'],

		props: {
			/**
			 * Configuration options for the radio input, such as read-only status and label text.
			 */
			options: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The current row data object containing details necessary for the radio input.
			 */
			row: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Container classes to be applied to the radio input wrapper.
			 */
			containerClasses: {
				type: [Array, String],
				default: () => []
			}
		},

		expose: [],

		mounted()
		{
			this.$emit('loaded')
		},

		computed: {
			editable()
			{
				const optionName = this.options.name
				return optionName
					? this.row.Fields[optionName] ?? false
					: true
			}
		},

		methods: {
			/**
			 * Emits an 'update-external' event for any external updates of the radio input's selected value.
			 * @param {Event} event - The native event object from the radio input's change event.
			 */
			update(event)
			{
				this.$emit('update-external', event)
			}
		}
	}
</script>
