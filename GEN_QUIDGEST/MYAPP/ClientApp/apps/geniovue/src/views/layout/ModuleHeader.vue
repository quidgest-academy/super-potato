<template>
	<q-icon icon="modules" />

	<span
		v-if="!isEmpty(currentModuleTitle)"
		:class="[
			{ 'dropdown-toggle': Object.keys(system.availableModules).length > 1 },
			'i-change-module-placeholder'
		]">
		{{ currentModuleTitle }}
	</span>
</template>

<script>
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	export default {
		name: 'ModuleHeader',

		mixins: [LayoutHandlers],

		expose: []
	}
</script>
