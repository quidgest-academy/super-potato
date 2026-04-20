<template>
	<q-router-link
		v-for="breadcrumb in breadcrumbs"
		:key="breadcrumb.route.name"
		:class="[{ 'dropdown-item': breadcrumbs.length > 1 }, 'n-breadcrumb__link']"
		:link="breadcrumb.route"
		:title="breadcrumb.hoverText"
		:is-disabled="breadcrumb.isActive">
		<q-icon :icon="breadcrumb.icon" />

		<span
			v-if="!isEmpty(breadcrumb.text)"
			class="n-breadcrumb__text">
			{{ breadcrumb.text }}
		</span>

		<span
			v-if="!isEmpty(breadcrumb.humanKey) && $app.layout.BreadcrumbsContent === 'detailed'"
			class="n-breadcrumb__text">
			{{ breadcrumb.humanKey }}
		</span>
	</q-router-link>
</template>

<script>
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import QRouterLink from '@/views/shared/QRouterLink.vue'

	export default {
		name: 'QBreadcrumbsContent',

		components: {
			QRouterLink
		},

		mixins: [LayoutHandlers],

		props: {
			/**
			 * An array of breadcrumb items that represent the navigation path.
			 */
			breadcrumbs: {
				type: Array,
				required: true
			}
		},

		expose: []
	}
</script>
