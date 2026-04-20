<template>
	<div
		:class="['c-alert', `c-alert--${messageType}`, { 'c-alert--banner': isBanner }]"
		role="alert"
		@click.stop.prevent="onAlertClick">
		<div class="c-alert__header">
			<q-icon :icon="messageIcon" />

			<div
				v-if="isBanner"
				class="c-alert__text">
				{{ text }}
			</div>

			<q-button
				v-if="isDismissible"
				variant="text"
				class="c-alert__dismissible"
				size="small"
				:aria-label="texts.close"
				@click="dismissMessage">
				<q-icon icon="close" />
			</q-button>
		</div>

		<div
			v-if="!isBanner"
			class="mt-2">
			<h4
				v-if="title"
				class="c-alert__title">
				{{ title }}
			</h4>

			<div
				v-if="text"
				class="c-alert__text">
				{{ text }}
			</div>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import isEmpty from 'lodash-es/isEmpty'

	import hardcodedTexts from '@/hardcodedTexts.js'

	import { messageTypes } from '@quidgest/clientapp/constants/enums'

	export default {
		name: 'QInfoMessage',

		emits: ['navigate-to', 'message-dismissed'],

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
				type: String,
				required: true
			},

			/**
			 * The type of the message (can be: "success", "info", "warning" or "error").
			 */
			type: {
				type: String,
				default: messageTypes.W,
				validator: (value) => {
					const allowedTypes = Object.keys(messageTypes).map((key) => messageTypes[key])
					return allowedTypes.includes(value)
				}
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
			},

			/**
			 * Whether or not the message is dismissible.
			 */
			isDismissible: {
				type: Boolean,
				default: true
			},

			/**
			 * The target to navigate to on alert click.
			 */
			onClickTarget: {
				type: Object,
				default: () => ({})
			}
		},

		expose: [],

		data()
		{
			return {
				timeoutId: null,
				texts: {
					close: computed(() => this.Resources[hardcodedTexts.close])
				}
			}
		},

		mounted()
		{
			// Sets the timeout to automatically dismiss the message when the time limit is reached.
			if (this.isDismissible && this.dismissTime > 0)
				this.timeoutId = setTimeout(this.dismissMessage, this.dismissTime * 1000)
		},

		beforeUnmount()
		{
			if (this.timeoutId !== null)
				clearTimeout(this.timeoutId)
		},

		computed: {
			messageIcon()
			{
				if (this.icon.length > 0)
					return this.icon

				switch (this.type)
				{
					case messageTypes.W:
						return 'warning'
					case messageTypes.OK:
						return 'success'
					case messageTypes.I:
						return 'information'
					case messageTypes.E:
						return 'exclamation-sign'
				}

				return ''
			},

			messageType()
			{
				switch (this.type)
				{
					case messageTypes.W:
					case messageTypes.OK:
					case messageTypes.I:
						return this.type
					case messageTypes.E:
						return 'danger'
				}

				return ''
			},

			isBanner()
			{
				return this.text && !this.title
			}
		},

		methods: {
			dismissMessage()
			{
				this.$emit('message-dismissed', this.id)
			},

			onAlertClick()
			{
				if (!isEmpty(this.onClickTarget))
					this.$emit('navigate-to', this.onClickTarget)
			}
		}
	}
</script>
