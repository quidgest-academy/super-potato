<template>
	<div
		id="cookies"
		class="q-cookies">
		<a
			:href="filePath"
			target="blank"
			class="q-cookies__field">
			{{ text }}
		</a>

		<q-button
			class="q-cookies__button"
			@click="setCookie">
			{{ buttonText }}
		</q-button>
	</div>
</template>

<script>
	export default {
		name: 'QCookies',

		emits: ['set-cookie'],

		inheritAttrs: false,

		props: {
			/**
			 * The text to be used in the body of the component.
			 */
			text: {
				type: String,
				required: true
			},

			/**
			 * The text in the cookies button.
			 */
			buttonText: {
				type: String,
				required: true
			},

			/**
			 * Path of the file to be used.
			 */
			filePath: {
				type: String,
				required: true
			}
		},

		expose: [],

		methods: {
			setCookie()
			{
				this.$emit('set-cookie', true)
			}
		}
	}
</script>
