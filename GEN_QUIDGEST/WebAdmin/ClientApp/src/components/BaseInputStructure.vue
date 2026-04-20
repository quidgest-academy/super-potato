<template v-if="isVisible">
	<div :class="[...classes]">
		<q-label
			v-if="showLabel"
			:id="labelId"
			:label="label"
			:for="id"
			:required="isRequired && !(readonly || disabled)">
			<template
				v-if="showPopoverButton"
				#append>
				<q-button
					:id="popoverButtonId"
					borderless
					size="small"
					variant="text"
					color="neutral">
					<q-icon icon="information-outline" />
				</q-button>
				<q-popover
					:anchor="`#${popoverButtonId}`"
					:title="popoverTitle"
					:text="popoverText" />
			</template>
		</q-label>
		<slot />
	</div>
</template>

<script>
	import { getCurrentInstance } from 'vue'
	import _isEmpty from 'lodash-es/isEmpty'

	export default {
		name: 'QBaseInputStructure',

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: {
				type: String,
				default: ''
			},

			/**
			 * The label text for the input field.
			 */
			label: {
				type: String,
				default: ''
			},

			/**
			 * Flag indicating if the label is to be displayed.
			 */
			hasLabel: {
				type: Boolean,
				default: true
			},

			/**
			 * Controls the readonly state of the input field.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Disables the input field, preventing user interaction.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Determines if the input field is marked as required.
			 */
			isRequired: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether or not the control is currently visible.
			 */
			isVisible: {
				type: Boolean,
				default: true
			},

			/**
			 * An array of additional CSS classes to apply to the component.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * True if the control should show the popover help button, false otherwise.
			 */
			showPopoverButton: {
				type: Boolean,
				default: false
			},

			/**
			 * Title of the popover help.
			 */
			popoverTitle: {
				type: String,
				default: ''
			},

			/**
			 * Text of the popover help.
			 */
			popoverText: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data() {
			return {
				controlId: `container-${this.id || getCurrentInstance().uid}`,
				sortablePlugin: null,
				popoverButtonId: `popover-btn-${this.id}`
			}
		},

		computed: {
			/**
			 * The identifier for the label element associated with the control.
			 */
			labelId() {
				return `label_${this.id}`
			},

			showLabel() {
				return this.hasLabel && !this.isEmpty(this.label)
			}
		},

		methods: {
			isEmpty: _isEmpty
		}
	}
</script>
