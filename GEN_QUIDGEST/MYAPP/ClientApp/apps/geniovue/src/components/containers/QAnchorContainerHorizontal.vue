<template>
	<div class="q-anchors">
		<template
			v-for="ctrlId in anchors"
			:key="ctrlId">
			<a
				v-if="showAnchor(ctrlId)"
				href="#"
				role="button"
				:class="getClass(ctrlId)"
				@click.stop.prevent="anchorClicked(ctrlId)">
				<q-icon icon="paired" />
				{{ controls[ctrlId].label }}
			</a>
		</template>
	</div>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

	export default {
		name: 'QAnchorContainerHorizontal',

		emits: ['focus-control'],

		inheritAttrs: false,

		props: {
			/**
			 * The form controls.
			 */
			controls: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The anchors list.
			 */
			anchors: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		data()
		{
			return {
				isSelected: false,
				selectedAnchor: null,
				scrollAnchor: null
			}
		},

		mounted()
		{
			this.registerScrollSpy()
		},

		beforeUnmount()
		{
			window.removeEventListener('scroll', this.handleScroll)
		},

		methods: {
			showAnchor(ctrlId)
			{
				const ctrl = this.controls[ctrlId]

				if (ctrl.type === 'Tab' || ctrl.isCollapsible)
					return false

				return !_isEmpty(ctrl.label) && ctrl.anchored === true
			},

			selectAnchor(event)
			{
				event.preventDefault()
				this.isSelected = !this.isSelected
			},

			setScrollAnchor(ctrlId)
			{
				this.scrollAnchor = ctrlId
			},

			getClass(ctrlId)
			{
				return `q-anchors__${this.scrollAnchor === ctrlId ? 'selected' : 'title'}`
			},

			anchorClicked(ctrlId)
			{
				this.$emit('focus-control', ctrlId, false, 'start', 'instant')
				this.selectedAnchor = ctrlId
			},

			handleScroll()
			{
				let anchor

				// Get the Y coordinate starting after the layout header and form header
				const scrollYStart = genericFunctions.scrollYStart()

				for (const ctrl of this.anchors)
				{
					const target = document.getElementById(ctrl)

					if (target)
					{
						const pos = target.getBoundingClientRect()

						// We want to get the last possible anchor, otherwise, nested anchors will always be ignored.
						// Show the anchor as selected when the scroll reaches 5 pixels before the top of the corresponding zone
						if (pos.top < scrollYStart + 5 && pos.bottom > scrollYStart)
							anchor = ctrl
					}
				}

				if (this.selectedAnchor)
					this.selectedAnchor = ''

				this.setScrollAnchor(null)

				if (anchor)
					this.setScrollAnchor(anchor)
			},

			registerScrollSpy()
			{
				window.addEventListener('scroll', this.handleScroll)

				// Call handleScroll on initial mount.
				this.handleScroll()
			}
		}
	}
</script>
