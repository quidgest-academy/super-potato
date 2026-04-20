<template>
	<div :class="classes">
		<slot></slot>
	</div>
</template>

<script>
	export default {
		name: 'QInfoMessageContainer',

		inheritAttrs: false,

		props: {
			/**
			 * Whether or not the messages should be fixed on the screen.
			 */
			pinned: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		computed: {
			classes()
			{
				const classes = []

				if (this.pinned)
					classes.push('c-alert--fixed__container')
				else
					classes.push('c-alert__container')

				return classes
			}
		}
	}
</script>
