<template>
	<transition
		name="collapsible"
		v-bind="$attrs"
		@enter="enter"
		@after-enter="afterEnter"
		@leave="leave"
		@after-leave="afterLeave">
		<div v-if="show">
			<slot></slot>
		</div>
	</transition>
</template>

<script>
	export default {
		name: 'QTimelineCollapsible',

		emits: [
			'enter',
			'after-enter',
			'leave',
			'after-leave'
		],

		inheritAttrs: false,

		props: {
			/**
			 * Determines whether the collapsible content is currently shown.
			 */
			show: {
				type: Boolean,
				default: true
			}
		},

		expose: [],

		data()
		{
			return {
				cachedStyles: null
			}
		},

		methods: {
			/**
			 * Triggers the enter transition for the collapsible element and emits the 'enter' event.
			 * @param {Element} el - The DOM element being transitioned.
			 * @param {Function} done - Callback function to call when the transition ends.
			 */
			enter(el, done)
			{
				this.detectAndCacheOrientations(el)
				this.setClosedOrientations(el)
				this.hideOverflow(el)
				this.forceRepaint(el)
				this.setTransition(el)
				this.setOpenedOrientations(el)
				this.$emit('enter', el, done)
				setTimeout(done, 350)
			},

			/**
			 * Cleans up after the enter transition completes and emits the 'after-enter' event.
			 * @param {Element} el - The DOM element that has completed transition.
			 */
			afterEnter(el)
			{
				this.unsetOverflow(el)
				this.unsetTransition(el)
				this.unsetOrientations(el)
				this.clearCachedOrientations()
				this.$emit('after-enter', el)
			},

			/**
			 * Triggers the leave transition for the collapsible element and emits the 'leave' event.
			 * @param {Element} el - The DOM element being transitioned.
			 * @param {Function} done - Callback function to call when the transition ends.
			 */
			leave(el, done)
			{
				this.detectAndCacheOrientations(el)
				this.setOpenedOrientations(el)
				this.hideOverflow(el)
				this.forceRepaint(el)
				this.setTransition(el)
				this.setClosedOrientations(el)
				this.$emit('leave', el, done)
				setTimeout(done, 350)
			},

			/**
			 * Cleans up after the leave transition completes and emits the 'after-leave' event.
			 * @param {Element} el - The DOM element that has completed transition.
			 */
			afterLeave(el)
			{
				this.unsetOverflow(el)
				this.unsetTransition(el)
				this.unsetOrientations(el)
				this.clearCachedOrientations()
				this.$emit('after-leave', el)
			},

			/**
			 * Caches the computed styles for orientations if not already cached.
			 * @param {Element} el - The DOM element to inspect.
			 */
			detectAndCacheOrientations(el)
			{
				if (this.cachedStyles)
					return
				const visibility = el.style.visibility
				el.style.visibility = 'hidden'
				el.style['backface-visibility'] = 'hidden'
				this.cachedStyles = this.detectRelevantOrientations(el)
				el.style.visibility = visibility
			},

			/**
			 * Clears the cached styles for orientations.
			 */
			clearCachedOrientations()
			{
				this.cachedStyles = null
			},

			/**
			 * Determines the relevant styles for orientations to be cached.
			 * @param {Element} el - The DOM element to inspect.
			 * @returns {Object} An object containing relevant style properties.
			 */
			detectRelevantOrientations(el)
			{
				return {
					height: el.offsetHeight + 'px'
				}
			},

			/**
			 * Sets the transition properties on the element.
			 * @param {Element} el - The DOM element to apply the transition to.
			 */
			setTransition(el)
			{
				el.style.transition = 'height 350ms'
			},

			/**
			 * Removes the transition properties from the element.
			 * @param {Element} el - The DOM element to remove the transition from.
			 */
			unsetTransition(el)
			{
				el.style.transition = ''
			},

			/**
			 * Sets the overflow to 'hidden' on the element to mask the content.
			 * @param {Element} el - The DOM element to apply the style to.
			 */
			hideOverflow(el)
			{
				el.style.overflow = 'hidden'
			},

			/**
			 * Resets the overflow property on the element.
			 * @param {Element} el - The DOM element to reset the style on.
			 */
			unsetOverflow(el)
			{
				el.style.overflow = ''
			},

			/**
			 * Sets the closed orientations on the element, typically setting the height to '0'.
			 * @param {Element} el - The DOM element to modify.
			 */
			setClosedOrientations(el)
			{
				Object.keys(this.cachedStyles).forEach((key) => {
					el.style[key] = '0'
				})
			},

			/**
			 * Sets the open orientations on the element using the cached styles.
			 * @param {Element} el - The DOM element to modify.
			 */
			setOpenedOrientations(el)
			{
				Object.keys(this.cachedStyles).forEach((key) => {
					el.style[key] = this.cachedStyles[key]
				})
			},

			/**
			 * Resets style properties for orientations on the element to their initial state.
			 * @param {Element} el - The DOM element to modify.
			 */
			unsetOrientations(el)
			{
				Object.keys(this.cachedStyles).forEach((key) => {
					el.style[key] = ''
				})
			},

			/**
			 * Forces a repaint of the element by accessing the 'height' property.
			 * @param {Element} el - The DOM element to repaint.
			 */
			forceRepaint(el)
			{
				void getComputedStyle(el)['height']
			},

			/**
			 * Retrieves the computed value of a CSS property for a given element.
			 * @param {Element} el - The DOM element to inspect.
			 * @param {String} style - The CSS property to retrieve.
			 * @returns {String} The computed value of the style property.
			 */
			getCssValue(el, style)
			{
				return getComputedStyle(el, null).getPropertyValue(style)
			}
		},

		watch: {
			orientation()
			{
				this.clearCachedOrientations()
			}
		}
	}
</script>
