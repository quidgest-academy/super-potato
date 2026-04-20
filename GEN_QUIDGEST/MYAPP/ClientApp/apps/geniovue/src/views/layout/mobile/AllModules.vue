<template>
	<li
		v-for="mod in system.availableModules"
		:key="mod.id"
		:class="[{ 'n-sidebar__nav-link--active': mod.id === system.currentModule }, 'has-treeview', 'nav-item', 'n-sidebar__nav-item']">
		<a
			:href="getLinkToModule(mod.id)"
			class="d-block nav-link n-sidebar__nav-link n-sidebar__module-link"
			:data-key="mod.id"
			@click.prevent="handleClick(mod.id)"
			@keyup="(...args) => $emit('keyup', ...args)">

			<q-icon 
				v-if="getModuleIconProps(mod)"
				v-bind="getModuleIconProps(mod)" 
				class="nav-icon n-sidebar__icon"
			/>
			<p>
				<span>
					{{ Resources[mod.title] }}
				</span>
			</p>
		</a>
	</li>
</template>

<script>
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import VueNavigation from '@/mixins/vueNavigation.js'

	export default {
		name: 'QMenuAllModules',

		emits: [
			'navigate-to-module',
			'keyup'
		],

		mixins: [
			LayoutHandlers,
			VueNavigation
		],

		expose: [],

		methods: {
			handleClick(moduleId)
			{
				this.navigateToModule(moduleId)
				this.$emit('navigate-to-module')
			},

			getLinkToModule(moduleId) {
				return this.linkToRouteName(`home-${moduleId}`)
			}
		}
	}
</script>
