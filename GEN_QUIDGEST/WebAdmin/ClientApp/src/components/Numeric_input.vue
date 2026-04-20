<template>
	<div class="i-text">
		<q-label
			v-if="label"
			:for="id"
			:label="label" />
		<input
			:id="id"
			v-model="curValue"
			type="number"
			:class="style_class"
			:readonly="isReadOnly" />
	</div>
</template>

<script>
	import { getCurrentInstance } from 'vue'

	export default {
		name: 'NumericInput',

		props: {
			/**
			 * Component value.
			 */
			modelValue: {
				type: [Number, String],
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
			 * Component size.
			 */
			size: {
				type: String,
				default: 'xxlarge'
			},

			/**
			 * True if the input should be in a read-only state, false otherwise.
			 */
			isReadOnly: {
				type: Boolean,
				default: false
			},

			/**
			 * True if the input should only update to integer values, false otherwise.
			 */
			integerOnly: {
				type: Boolean,
				default: false
			}
		},

		emits: ['update:modelValue'],

		expose: [],

		data() {
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
					let valueToEmit = newValue
					if (this.integerOnly && newValue !== '' && !isNaN(newValue)) {
						valueToEmit = parseInt(newValue)
					}
					this.$emit('update:modelValue', valueToEmit)
				}
			},

			style_class() {
				return 'i-text__field i-text input-' + this.size
			}
		},

		mounted() {
			this.id = 'input_n_' + getCurrentInstance().uid
		}
	}
</script>
