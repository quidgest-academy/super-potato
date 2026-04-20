<template>
	<div class="i-text">
		<div v-if="label">
			<q-label
				:required="isRequired"
				:for="id"
				:label="label" />
			<q-icon
				v-if="helpText"
				:title="helpText"
				icon="information-outline" />
		</div>
		<input
			:id="id"
			v-model="curValue"
			type="text"
			:class="inputStyle"
			:readonly="isReadOnly"
			:placeholder="placeholder" />
	</div>
</template>

<script>
	import { getCurrentInstance } from 'vue'

	export default {
		name: 'TextInput',
		props: {
			/**
			 * Value of the input.
			 */
			modelValue: {
				type: String,
				default: ''
			},

			/**
			 * Label of the input.
			 */
			label: {
				type: String,
				default: ''
			},

			/**
			 * Label of the input.
			 */
			size: {
				type: String,
				default: 'xxlarge'
			},

			/**
			 * Controls the readonly state of the input field.
			 */
			isReadOnly: {
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
			 * Placeholder of the input.
			 */
			placeholder: {
				type: String,
				default: ''
			},

			/**
			 * Help of the input.
			 */
			helpText: {
				type: String,
				default: ''
			}
		},

		emits: ['update:modelValue'],

		expose: [],

		data: function () {
			return {
				id: null
			}
		},

		computed: {
			curValue: {
				get() {
					return this.modelValue
				},
				set(newValue) {
					this.$emit('update:modelValue', newValue)
				}
			},

			inputStyle() {
				return 'i-text__field i-text input-' + this.size
			}
		},
		mounted() {
			this.id = 'input_t_' + getCurrentInstance().uid
		}
	}
</script>
