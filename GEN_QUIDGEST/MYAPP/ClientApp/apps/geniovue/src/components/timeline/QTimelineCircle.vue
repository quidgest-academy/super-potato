<template>
	<div
		class="c-timeline__item-section-icon"
		:style="customStyle">
		<i :class="icon"></i>
	</div>
</template>

<script>
	export default {
		name: 'QTimelineCircle',

		props: {
			/**
			 * The CSS style or color to apply to the timeline circle's background and border.
			 */
			circlestyle: String,

			/**
			 * The icon class to use for displaying an icon within the circle.
			 */
			icon: String
		},

		expose: [],

		computed: {
			/**
			 * Computes the styles to apply to the timeline circle based on the provided circle style color.
			 */
			customStyle()
			{
				return {
					backgroundColor: this.circlestyle,
					borderColor: this.circlestyle
				}
			}
		}
	}
</script>
