<template>
	<component
		v-if="currentComponent"
		:is="currentComponent"
		mode="SHOW"
		is-home-page
		is-nested />
	<div v-else-if="userIsLoggedIn">
<!-- eslint-disable indent, vue/html-indent, vue/script-indent -->
<!-- USE /[MANUAL FOR INDEX_AUTHENTICATED]/ -->
<!-- eslint-disable-next-line -->
<!-- eslint-enable indent, vue/html-indent, vue/script-indent -->
<!-- eslint-disable indent, vue/html-indent, vue/script-indent -->
<!-- USE /[MANUAL FOR INDEX_AUTHENTICATED MYAPP]/ -->
<!-- eslint-disable-next-line -->
<!-- eslint-enable indent, vue/html-indent, vue/script-indent -->
		<div class="welcome-authenticated">
			<span>{{ Resources.BEM_VINDO38622 }} {{ userData.name }}</span>
		</div>
	</div>
</template>

<script>
	// eslint-disable-next-line @typescript-eslint/no-unused-vars
	import { shallowRef, defineAsyncComponent } from 'vue'
	import { mapState } from 'pinia'
	import _assignIn from 'lodash-es/assignIn'

	import { useSystemDataStore } from '@quidgest/clientapp/stores'
	import { useGenericDataStore } from '@quidgest/clientapp/stores'

	import { fetchData } from '@quidgest/clientapp/network'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import NavHandlers from '@/mixins/navHandlers.js'

	export default {
		name: 'QHome',

		mixins: [
			NavHandlers,
			LayoutHandlers
		],

		inheritAttrs: false,

		expose: [
			'navigationId'
		],

		data()
		{
			return {
				model: {},
				currentComponent: shallowRef(null),
				hasCustomHomePage: false
			}
		},

		computed: {
			...mapState(useSystemDataStore, [
				'system'
			]),

			...mapState(useGenericDataStore, [
				'homepages'
			]),
		},

		created()
		{
			this.determineComponent()
		},

		mounted()
		{
			this.loadData()
		},

		methods: {
			loadData()
			{
				fetchData('Home', 'IndexAuthenticated', {}, data => _assignIn(this.model, data))
			},

			determineComponent()
			{
				const hPage = this.homepages[this.system.currentModule] || this.homepages['Public'] || { Identifier: '' }

				switch (hPage.Identifier)
				{
					default:
						this.currentComponent = null
				}
			}
		},

		watch: {
			userIsLoggedIn()
			{
				this.determineComponent()
			},

			homepages()
			{
				this.determineComponent()
			}
		}
	}
</script>
