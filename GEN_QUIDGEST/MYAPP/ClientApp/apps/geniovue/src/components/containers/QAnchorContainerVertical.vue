<template>
	<div class="c-sidebar--container__section">
		<div class="c-sidebar__subtitle">
			<q-icon icon="list-bordered" />

			<span>{{ title }}</span>
		</div>

		<ul class="anchor-list-container form-tree">
			<template
				v-for="ctrl in anchors"
				:key="ctrl.id">
				<q-anchor-element
					:ctrl="ctrl"
					@focus-control="(...args) => $emit('focus-control', ...args)" />
			</template>
		</ul>
	</div>
</template>

<script>
	import _cloneDeep from 'lodash-es/cloneDeep'

	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

	import QAnchorElement from './QAnchorElement.vue'

	export default {
		name: 'QAnchorContainerVertical',

		emits: ['focus-control'],

		components: {
			QAnchorElement
		},

		inheritAttrs: false,

		props: {
			/**
			 * The anchors title.
			 */
			title: {
				type: String,
				default: ''
			},

			/**
			 * The tree of controls.
			 */
			tree: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		data()
		{
			return {
				anchors: []
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
			handleScroll()
			{
				// Get the Y coordinate starting after the layout header and form header
				const scrollYStart = genericFunctions.scrollYStart()

				let isThereAnActiveSession = false

				function findActiveAnchor(anchor)
				{
					const target = document.getElementById(anchor.id)

					if (target)
					{
						const anchoredChildren = anchor.anchoredChildren
						let hasAnActiveChild = false

						for (const anchoredChild of anchoredChildren)
						{
							if (findActiveAnchor(anchoredChild))
							{
								anchor.isActive = true
								hasAnActiveChild = true
								isThereAnActiveSession = true
							}
						}

						if (hasAnActiveChild)
							return true

						const pos = target.getBoundingClientRect()

						if (!isThereAnActiveSession && (pos.top < scrollYStart + 5 && pos.bottom > scrollYStart))
						{
							anchor.isActive = true
							isThereAnActiveSession = true
							return true
						}
					}

					anchor.isActive = false
					return false
				}

				// Looking for the first section whose the top is in the viewport.
				for (const ctrl of this.anchors)
					findActiveAnchor(ctrl)
			},

			registerScrollSpy()
			{
				window.removeEventListener('scroll', this.handleScroll)
				window.addEventListener('scroll', this.handleScroll)

				// Deep copy the tree so the sidebar is not reload when a section is marked as actived.
				this.anchors = _cloneDeep(this.tree)

				// Call handleScroll on initial mount.
				this.handleScroll()
			}
		},

		watch: {
			tree: {
				handler()
				{
					this.registerScrollSpy()
				},
				deep: true
			}
		}
	}
</script>
