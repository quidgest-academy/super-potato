<template>
	<div style="margin: 1rem">
		<q-info-message
			v-for="infoMessage in infoMessages"
			:key="infoMessage.message"
			:id="infoMessage.id"
			:text="infoMessage.message"
			:type="infoMessage.type"
			:icon="infoMessage.icon"
			:dismiss-time="0"
			:is-dismissible="infoMessage.isDismissible"
			@message-dismissed="removeInfoMessage" />
	</div>
</template>

<script>
	import fakeData from './InfoMessage.mock.js'

	import QInfoMessage from '@/components/QInfoMessage.vue'

	export default {
		name: 'QInfoMessageContainer',

		inheritAttrs: false,

		expose: [],

		components: {
			QInfoMessage
		},

		data()
		{
			return {
				infoMessages: []
			}
		},

		mounted()
		{
			this.infoMessages = fakeData.simpleUsage().infoMessages
		},

		methods: {
			removeInfoMessage(messageId)
			{
				for (let i = 0; i < this.infoMessages.length; i++)
				{
					if (this.infoMessages[i].id !== messageId)
						continue

					this.infoMessages.splice(i, 1)

					setTimeout(() => {
						this.infoMessages = fakeData.simpleUsage().infoMessages
					}, 300)

					return
				}
			}
		}
	}
</script>
