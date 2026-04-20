<template>
	<div :class="staticClasses">
		<q-label
			v-if="label"
			:for="id">
			<q-label
				class="q-static__label"
				:for="id"
				:label="label + ':'" />
		</q-label>
		<span
			:id="id"
			class="q-static__value">
			{{ modelValue }}
		</span>
	</div>
</template>

<script>
	import { getCurrentInstance } from 'vue'

	export default {
		name: 'StaticText',

		props: {
			/**
			 * Component value.
			 */
			modelValue: {
				type: [String, Number, Object],
				default: ''
			},

			/**
			 * Component label.
			 */
			label: {
				type: String,
				default: ''
			},

			/**
			 * True if the component should show its value in bold text, false otherwise.
			 */
			bold: {
				type: Boolean,
				default: false
			},

			/**
			 * True if the component should show its label in bold text, false otherwise.
			 */
			boldLabel: {
				type: Boolean,
				default: false
			},

			/**
			 * The static value's positioning in relation to the label - 'horizontal' (in front) or 'vertical' (below)
			 */
			orientation: {
				type: String,
				default: 'horizontal'
			}
		},

		expose: [],

		data() {
			return {
				id: null
			}
		},

		computed: {
			staticClasses() {
				return [
					'q-static',
					`q-static--${this.orientation}`,
					{
						'q-static--bold': this.bold,
						'q-static--bold-label': this.boldLabel
					}
				]
			}
		},

		mounted() {
			this.id = 'static_text_' + getCurrentInstance().uid
		}
	}
</script>
