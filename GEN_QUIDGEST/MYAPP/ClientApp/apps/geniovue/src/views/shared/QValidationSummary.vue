<template>
	<div
		v-show="messagesList.length > 0"
		:class="wrapperClasses">
		<ul>
			<li
				v-for="message in messagesList"
				:key="message.id"
				@click.stop.prevent="messageClicked(message.id)">
				<div class="q-validation-error">
					<q-icon :icon="iconToDisplay" />
					{{ message.text }}
				</div>
			</li>
		</ul>
	</div>
</template>

<script>
	import { scrollToTop } from '@quidgest/clientapp/utils/genericFunctions'

	export default {
		name: 'QValidationSummary',

		emits: ['error-clicked'],

		props: {
			/**
			 * Data containing messsages to be displayed in the validation summary.
			 */
			messages: {
				type: Object,
				required: true
			},

			/**
			 * The type of the messages that are being displayed
			 */
			type: {
				type: String,
				default: "error"
			},

			/**
			 * The icon to be shown on each message
			 */
			icon: {
				type: String,
			}
		},

		expose: [],

		computed: {
			/**
			 * Computes a list of messages based on the provided messages prop.
			 */
			messagesList()
			{
				const list = []

				for (const i in this.messages)
				{
					if (Array.isArray(this.messages[i]))
						this.messages[i].forEach((e) => list.push({ id: i, text: e }))
					else
						list.push({ id: i, text: this.messages[i] })
				}

				return list
			},

			/**
			 * Computes the classes for the wrapper based on the messages type
			 */
			wrapperClasses() {
				const classes = ['validation-summary']

				if (this.type === "error")
					classes.push('validation-summary-errors')

				else if (this.type === "warning")
					classes.push('validation-summary-warnings')

				else if (this.type === "info")
					classes.push('validation-summary-info')

				return classes
			},

			/**
			 * Computes the icon to show based on the props and type
			 */
			iconToDisplay() {
				if (this.icon)
					return this.icon

				else if (this.type === 'info')
					return 'information'

				return 'error'

			}
		},

		methods: {
			/**
			 * Emits an message-clicked event when an message is clicked, passing the message's id to the listener.
			 * @param {String} id - The id of the message that was clicked.
			 */
			messageClicked(id)
			{
				this.$emit('error-clicked', id)
			}
		},

		watch: {
			messagesList(newVal)
			{
				if (newVal.length > 0)
					scrollToTop()
			}
		}
	}
</script>
