<template>
	<ul
		v-if="!isEmpty(system.availableModules)"
		class="dropdown-menu">
		<template
			v-for="mod in system.availableModules"
			:key="mod.id">
			<li
				v-if="mod.id !== system.currentModule"
				class="i-change-module__item">
				<a
					class="dropdown-item"
					:href="getLinkToModule(mod.id)"
					:data-key="mod.id"
					@click.prevent="selectItem(mod.id)"
					@keyup="(...args) => $emit('keyup', ...args)">
					<i 
						v-if="mod.font"
						:class="['dropdown__icon', mod.font]"></i>
					{{ Resources[mod.title] }}
				</a>
			</li>
		</template>
	</ul>
</template>

<script>
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import VueNavigation from '@/mixins/vueNavigation.js'

	export default {
		name: 'QMenuAllModules',

		emits: ['keyup', 'menu-action'],

		mixins: [
			LayoutHandlers,
			VueNavigation
		],

		expose: [],

		methods: {
			/**
			 * Select menu item.
			 * @param {string} id Module ID
			 */
			selectItem(id)
			{
				this.$emit('menu-action')
				this.navigateToModule(id)
			},

			getLinkToModule(moduleId) {
				return this.linkToRouteName(`home-${moduleId}`)
			}
		}
	}
</script>
