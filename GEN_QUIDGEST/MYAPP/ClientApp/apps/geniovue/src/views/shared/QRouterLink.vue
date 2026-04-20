<template>
	<router-link
		v-if="!isDisabled"
		:to="link">
		<slot></slot>
	</router-link>
	<span v-else>
		<slot></slot>
	</span>
</template>

<script>
	export default {
		name: 'QRouterLink',

		props: {
			/**
			 * The route to where we want to navigate.
			 */
			link: {
				type: [String, Object],
				required: true,
				validator: (value) => {
					if (typeof value === 'object')
						return value && typeof value.name === 'string' && typeof value.params === 'object'
					return value && value.length > 0
				}
			},

			/**
			 * Whether or not the link is currently disabled.
			 */
			isDisabled: {
				type: Boolean,
				default: false
			}
		},

		expose: []
	}
</script>
