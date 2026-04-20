<template>
	<div class="i-text">
		<q-label
			:required="isRequired"
			:for="id"
			:label="label" />
		<input
			:id="id"
			v-model="curValue"
			type="password"
			:class="style_class"
			:readonly="isReadOnly"
			@focus="onFocus"
			@blur="onUnfocus" />
	</div>
</template>

<script>
	import { getCurrentInstance } from 'vue'

	export default {
		name: 'PasswordInput',

		props: {
			/**
			 * Component value.
			 */
			modelValue: {
				type: String,
				default: ''
			},

			/**
			 * True if the component should show the password circles over the characters, false otherwise.
			 */
			showFiller: {
				type: Boolean,
				default: false
			},

			/**
			 * Component value.
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
			 * True if the input should is required, false otherwise.
			 */
			isRequired: {
				type: Boolean,
				default: false
			}
		},

		emits: ['update:model-value'],

		expose: [],

		data() {
			return {
				id: null,
				vShowFiller: this.showFiller
			}
		},

		computed: {
			curValue: {
				get() {
					// Display password circles when showFiller is enabled and
					// password is empty
					if (!this.modelValue && this.vShowFiller) return 'ThisIsTrash'
					return this.modelValue
				},
				set(newValue) {
					// Remove circles after first user interaction
					if (!newValue) this.vShowFiller = false

					this.$emit('update:model-value', newValue)
				}
			},

			style_class() {
				return 'i-text__field i-text input-' + this.size
			}
		},

		watch: {
			showFiller(newVal) {
				this.vShowFiller = newVal
			}
		},

		mounted() {
			this.id = 'input_t_' + getCurrentInstance().uid
		},

		methods: {
			onFocus() {
				this.vShowFiller = false
			},

			onUnfocus() {
				if (this.showFiller && !this.modelValue) this.vShowFiller = true
			}
		}
	}
</script>
