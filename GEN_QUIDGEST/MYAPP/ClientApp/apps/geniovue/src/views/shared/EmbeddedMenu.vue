<template>
	<ul
		v-if="showContent"
		class="n-menu__aside navbar-nav ml-auto">
		<li v-if="!userIsLoggedIn">
			<q-button
				id="logon-menu-btn"
				variant="bold"
				:label="texts.enter"
				:title="texts.enter"
				:tabindex="$attrs.tabindex"
				@click="setMenuState(!showLogin)">
				<q-icon icon="login" />
			</q-button>

			<q-log-on
				v-if="showLogin"
				@set-visibility="setMenuState($event)" />
		</li>
		<user-avatar
			v-else
			:tabindex="$attrs.tabindex" />
	</ul>
</template>

<script>
	import { computed, defineAsyncComponent } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import UserAvatar from '@/views/shared/UserAvatar.vue'

	export default {
		name: 'EmbeddedMenu',

		inheritAttrs: false,

		components: {
			QLogOn: defineAsyncComponent(() => import('@/views/shared/LogOn.vue')),
			UserAvatar
		},

		mixins: [
			LayoutHandlers
		],

		expose: [],

		data()
		{
			return {
				showLogin: false,

				texts: {
					enter: computed(() => this.Resources[hardcodedTexts.enter])
				}
			}
		},

		computed: {
			showContent()
			{
				return this.userIsLoggedIn || (this.isPublicRoute && !this.isFullScreenPage) || this.$app.layout.LoginStyle === 'embeded_page'
			}
		},

		methods: {
			setMenuState(isVisible)
			{
				this.showLogin = isVisible
			}
		}
	}
</script>
