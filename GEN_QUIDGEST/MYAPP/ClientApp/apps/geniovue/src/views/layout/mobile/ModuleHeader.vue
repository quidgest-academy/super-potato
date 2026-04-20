<template>
	<q-icon
		icon="modules"
		class="nav-icon n-sidebar__icon" />

	<span
		v-if="!isEmpty(currentModuleTitle)"
		:class="[
			{ 'dropdown-toggle': Object.keys(system.availableModules).length > 1 },
			'i-change-module-placeholder'
		]">
		{{ currentModuleTitle }}
		<q-icon
			v-if="Object.keys(system.availableModules).length > 1"
			icon="expand"
			class="right" />
	</span>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts'
	import LayoutHandlers from '@/mixins/layoutHandlers'

	export default {
		name: 'ModuleHeader',

		mixins: [LayoutHandlers],

		expose: [],

		data()
		{
			return {
				texts: {
					modules: computed(() => this.Resources[hardcodedTexts.modules])
				}
			}
		}
	}
</script>
