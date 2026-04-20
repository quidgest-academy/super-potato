<template>
	<li>
		<q-icon
			icon="paired"
			:class="{ active: ctrl.isActive }" />

		<a
			href="#"
			role="button"
			:class="['anchored', { active: ctrl.isActive }]"
			@click.stop.prevent="$emit('focus-control', ctrl.id, false, 'start', 'instant')">
			{{ ctrl.label }}
		</a>

		<ul v-if="ctrl.anchoredChildren.length > 0">
			<template
				v-for="child in ctrl.anchoredChildren"
				:key="child.id">
				<q-anchor-element
					:ctrl="child"
					:is-visible="ctrl.isVisible && isVisible"
					@focus-control="(...args) => $emit('focus-control', ...args)" />
			</template>
		</ul>
	</li>
</template>

<script>
	export default {
		name: 'QAnchorElement',

		emits: ['focus-control'],

		inheritAttrs: false,

		props: {
			/**
			 * The group associated to this anchor.
			 */
			ctrl: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Whether or not the control is currently visible.
			 */
			isVisible: {
				type: Boolean,
				default: true
			}
		},

		expose: []
	}
</script>
