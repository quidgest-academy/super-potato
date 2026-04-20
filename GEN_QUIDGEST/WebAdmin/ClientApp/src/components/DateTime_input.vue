<template>
	<div class="i-date-picker">
		<q-label
			v-if="label"
			:for="id"
			:label="label" />

		<Datepicker
			:id="id"
			:model-value="modelValue"
			class="input-medium"
			:readonly="isReadOnly"
			data-ref="cur_elem"
			@update:model-value="updateValue" />
	</div>
</template>

<script>
	import { getCurrentInstance } from 'vue'
	import moment from 'moment'
	// https://www.npmjs.com/package/vue-bootstrap-datetimepicker
	// http://eonasdan.github.io/bootstrap-datetimepicker/Options/

	import Datepicker from '@vuepic/vue-datepicker'
	import '@vuepic/vue-datepicker/dist/main.css'

	export default {
		name: 'DatetimeInput',
		components: { Datepicker },
		props: {
			/**
			 * The currently selected date-time value.
			 */
			modelValue: {
				default: null,
				required: true,
				validator(modelValue) {
					return (
						modelValue === null ||
						modelValue instanceof Date ||
						typeof modelValue === 'string' ||
						modelValue instanceof String ||
						modelValue instanceof moment
					)
				}
			},

			/**
			 * The label of the date-time input.
			 */
			label: {
				type: String,
				default: ''
			},

			/**
			 * True if the input should be in a read-only state, false otherwise.
			 */
			isReadOnly: {
				type: Boolean,
				default: false
			}
		},
		emits: ['update:modelValue', 'dp-hide', 'dp-show', 'dp-change', 'dp-error', 'dp-update'],

		expose: [],

		data: function () {
			return {
				id: null,
				dp: null,
				// jQuery DOM
				elem: null,
				// http://eonasdan.github.io/bootstrap-datetimepicker/Options/
				config: {
					showClear: true
				},
				events: ['hide', 'show', 'change', 'error', 'update']
			}
		},

		watch: {
			/**
			 * Listen to change from outside of component and update DOM
			 *
			 * @param newValue
			 */
			modelValue(newValue) {
				if (this.dp) this.dp.date(newValue || null)
			},

			/**
			 * Watch for any change in options and set them
			 *
			 * @param newConfig Object
			 */
			config: {
				deep: true,
				handler(newConfig) {
					if (this.dp) this.dp.options(newConfig)
				}
			}
		},

		mounted() {
			this.id = 'input_t_' + getCurrentInstance().uid
		},

		/**
		 * Free up memory
		 */
		beforeUnmount() {
			/* istanbul ignore else */
			if (this.dp) {
				this.dp.destroy()
				this.dp = null
				this.elem = null
			}
		},

		methods: {
			/**
			 * Update v-model upon change triggered by date-picker itself
			 *
			 * @param event
			 */
			onChange(event) {
				const formattedDate = event.date ? event.date.format(this.dp.format()) : null
				this.$emit('update:modelValue', formattedDate)
			},

			updateValue(newValue) {
				this.$emit('update:modelValue', newValue)
			},

			pluginClick() {
				if (!this.isReadOnly && this.dp) {
					this.dp.toggle()
				}
			}
		}
	}
</script>
