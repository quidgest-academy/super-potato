<template>
	<div
		:class="['alert', `alert--${messageType}`]"
		role="alert">
		<div class="c-alert__header">
			<h4
				v-if="title"
				class="alert__title">
				{{ title }}
			</h4>

			<div
				v-if="isString"
				class="alert__text">
				{{ text }}
			</div>

			<div
				v-if="isArray"
				class="alert__list">
				<span
					v-for="(message, index) in text"
					:key="index"
					class="status-message"
					>{{ message }}</span
				>
			</div>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QAlert',

		inheritAttrs: false,

		props: {
			/**
			 * The unique identifier for this message in the list of messages.
			 */
			id: {
				type: [Number, String],
				default: -1
			},

			/**
			 * The title to be displayed.
			 */
			title: {
				type: String,
				default: ''
			},

			/**
			 * The text to be displayed.
			 */
			text: {
				type: [String, Array],
				required: true
			},

			/**
			 * The type of the message (can be: "success", "info", "warning" or "danger").
			 */
			type: {
				type: String,
				default: 'info'
			},

			/**
			 * The icon to use next to the message's text.
			 */
			icon: {
				type: String,
				default: ''
			},

			/**
			 * The time, in seconds, after which the message will be automatically dismissed (only if it's dismissible).
			 * A value of 0 or less means the message won't be automatically dismissed.
			 */
			dismissTime: {
				type: Number,
				default: 0
			}
		},

		emits: ['message-dismissed'],

		expose: [],

		data() {
			return {
				timeoutId: null
			}
		},

		computed: {
			messageType() {
				return this.type
			},

			// Computed property to check if `text` is a string
			isString() {
				return typeof this.text === 'string'
			},

			// Computed property to check if `text` is an array
			isArray() {
				return Array.isArray(this.text)
			}
		},

		mounted() {
			// Sets the timeout to automatically dismiss the message when the time limit is reached.
			if (this.dismissTime > 0)
				this.timeoutId = setTimeout(this.dismissMessage, this.dismissTime * 2000)
		},

		beforeUnmount() {
			if (this.timeoutId !== null) clearTimeout(this.timeoutId)
		},

		methods: {
			dismissMessage() {
				this.$emit('message-dismissed', this.id)
			}
		}
	}
</script>
