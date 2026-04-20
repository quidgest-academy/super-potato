<template>
	<td>
		<div
			ref="mainWrapper"
			:id="containerId"
			:class="wrapperClasses">
			<slot name="label" />

			<slot />

			<template v-if="hasMessages">
				<template
					v-for="(type, index) in messageTypes"
					:key="index">
					<div
						v-if="messageDescription[type]"
						:class="['btn-popover', type]">
						<q-icon icon="exclamation-sign" />
						{{ messageDescription[type] }}
					</div>
				</template>
			</template>
		</div>
	</td>
</template>

<script>
	export default {
		name: 'QGridBaseInputStructure',

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * Reference to the model field object which may contain error messages and other context.
			 */
			modelFieldRef: Object
		},

		expose: [],

		computed: {
			/**
			 * ID of this container element.
			 */
			containerId()
			{
				return this.id ? `container-${this.id}` : null
			},

			/**
			 * Dynamic classes for the main wrapper element based on current state.
			 */
			wrapperClasses()
			{
				const classes = ['grid-base-input-structure', this.$attrs.class]

				if (this.hasErrorMessages)
					classes.push('error')
				else if (this.hasWarningMessages)
					classes.push('warning')
				else if (this.hasInfoMessages)
					classes.push('info')

				return classes
			},

			/**
			 * Indicates if there are any server error messages.
			 */
			hasErrorMessages()
			{
				return this.modelFieldRef?.hasServerErrorMessages ?? false
			},

			/**
			 * Indicates if there are any server warning messages.
			 */
			hasWarningMessages()
			{
				return this.modelFieldRef?.hasServerWarningMessages ?? false
			},

			/**
			 * Indicates if there are any server info messages.
			 */
			hasInfoMessages()
			{
				return (this.modelFieldRef?.serverInfoMessages?.length ?? 0) > 0
			},

			/**
			 * Indicates if there are any messages.
			 */
			hasMessages()
			{
				return this.hasErrorMessages || this.hasWarningMessages || this.hasInfoMessages
			},

			/**
			 * Gets the types of messages to be displayed in this input
			 */
			messageTypes()
			{
				const types = []

				if (this.hasErrorMessages)
					types.push('error')
				if (this.hasWarningMessages)
					types.push('warning')
				if (this.hasInfoMessages)
					types.push('info')

				return types
			},

			/**
			 * Concatenated object of error messages.
			 */
			messageDescription()
			{
				return {
					error: this.modelFieldRef?.serverErrorMessages?.join('\n'),
					warning: this.modelFieldRef?.serverWarningMessages?.join('\n'),
					info: this.modelFieldRef?.serverInfoMessages?.join('\n')
				}
			}
		}
	}
</script>
