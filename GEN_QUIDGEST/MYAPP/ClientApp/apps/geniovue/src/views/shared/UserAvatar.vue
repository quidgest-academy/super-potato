<template>
	<li class="dropdown n-menu__aside-item">
		<div class="d-flex align-items-center">
			<q-button
				v-if="userIsLoggedIn && $app.appAlerts.length > 0"
				id="sidebar-collapse"
				class="nav-link n-menu__aside-link"
				aria-haspopup="true"
				aria-expanded="true"
				:tabindex="$attrs.tabindex"
				@click.stop.prevent="toggleAlert">
				<span
					data-toggle="tooltip"
					data-placement="left"
					:title="texts.alerts">
					<q-icon icon="notifications" />
				</span>
				<span
					class="e-badge e-badge--highlight"
					aria-hidden="true">
					{{ notifications.length }}
				</span>
				<span class="hidden-elem">
					{{ texts.alerts }}
				</span>
			</q-button>

			<q-button
				id="user-avatar"
				ref="userAvatarButton"
				class="n-menu__aside-link removecaret"
				variant="text"
				data-table-action-selected="false"
				aria-haspopup="true"
				:aria-expanded="showUserOptionsMenu"
				:tabindex="$attrs.tabindex">
				<img
					class="avatar"
					data-toggle="tooltip"
					data-placement="left"
					aria-hidden="true"
					:src="avatarSrc"
					:alt="texts.userAvatar"
					:title="userData.name" />
				<span class="hidden-elem">
					{{ texts.user }}
				</span>
			</q-button>

			<q-popover
				class="user-settings__popover"
				v-model="showUserOptionsMenu"
				anchor="#user-avatar"
				placement="bottom-end"
				:arrow="false"
				role="menu">
				<div
					ref="userOptionsMenu"
					tabindex="-1"
					class="c-user__dropdown"
					aria-labelledby="userOptionsMenu">
					<div class="q-card-view__content">
						<div class="q-card-view__title n-module__title">{{ fullName }}</div>
						<div
							class="q-card-view__subtitle n-module__subtitle">
							{{ userRole }}
						</div>
					</div>

					<ul class="c-sidebar__list">
						<li
							v-for="menu in model.EPHUserAvatarMenus"
							:key="menu.Title"
							class="c-sidebar__list-item">
							<q-router-link
								data-toggle="tooltip"
								data-placement="top"
								class="c-sidebar__list-link"
								:title="getMenuText(menu.Title)"
								:link="getMenuRoute(menu)"
								:tabindex="$attrs.tabindex">
								<i
									v-if="menu.Font"
									:class="[menu.Font, 'c-header__icon']"></i>
								{{ getMenuText(menu.Title) }}
							</q-router-link>
						</li>

						<li
							v-for="menu in model.UserAvatarMenus"
							:key="menu.Title"
							class="c-sidebar__list-item">
							<q-router-link
								data-toggle="tooltip"
								data-placement="top"
								class="c-sidebar__list-link"
								:title="getMenuText(menu.Title)"
								:link="getMenuRoute(menu)"
								:tabindex="$attrs.tabindex">
								<i
									v-if="menu.Font"
									:class="['glyphicons', `glyphicons-${menu.Font}`, 'c-header__icon']"></i>
								{{ getMenuText(menu.Title) }}
							</q-router-link>
						</li>

						<template
							v-for="module in system.availableModules"
							:key="module.id">
							<li
								v-if="system.currentModule === module.id"
								class="c-sidebar__list-item">
								<a
									:href="`Content/Manual/${module.id}Manual.pdf?v=${$app.genio.buildVersion}`"
									data-toggle="tooltip"
									target="_blank"
									rel="noopener noreferrer"
									class="c-sidebar__list-link"
									data-placement="top"
									:title="texts.userHelp"
									:tabindex="$attrs.tabindex">
									<span>
										<q-icon icon="user-help" />
										{{ texts.userHelp }}
									</span>
								</a>
							</li>
						</template>

						<template v-if="config.LoginType !== 'AD'">
							<li
								class="c-sidebar__list-item"
								v-if="hasUserSettings">
								<a
									role="button"
									href="#"
									data-toggle="tooltip"
									class="c-sidebar__list-link"
									data-placement="top"
									:title="texts.userSettings"
									:tabindex="$attrs.tabindex"
									@click.prevent="userSettings">
									<span>
										<q-icon icon="reset-password" />
										{{ texts.userSettings }}
									</span>
								</a>
							</li>

							<li class="c-sidebar__list-item">
								<a
									role="button"
									href="#"
									data-toggle="tooltip"
									data-placement="top"
									class="c-sidebar__list-link"
									:title="texts.leave"
									:tabindex="$attrs.tabindex"
									@click.prevent="logOff">
									<span>
										<q-icon icon="exit" />
										{{ texts.leave }}
									</span>
								</a>
							</li>
						</template>
					</ul>
				</div>
			</q-popover>
		</div>
	</li>
</template>

<script>
	import { computed } from 'vue'
	import { mapState } from 'pinia'
	import _merge from 'lodash-es/merge'

	import { useSystemDataStore } from '@quidgest/clientapp/stores'
	import { useGenericDataStore } from '@quidgest/clientapp/stores'
	import { fetchData } from '@quidgest/clientapp/network'
	import { logOff } from '@/utils/user.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import AuthHandlers from '@/mixins/authHandlers.js'
	import hardcodedTexts from '@/hardcodedTexts.js'

	import QRouterLink from '@/views/shared/QRouterLink.vue'

	/**
	 * User avatar, responsible for render the avatar image and the associated dropdown menu
	 */
	export default {
		name: 'UserAvatar',

		inheritAttrs: false,

		components: {
			QRouterLink
		},

		mixins: [
			LayoutHandlers,
			AuthHandlers
		],

		expose: [],

		data()
		{
			return {
				model: {
					Avatar: {},
					UserAvatarMenus: [],
					EPHUserAvatarMenus: []
				},

				texts: {
					options: computed(() => this.Resources[hardcodedTexts.options]),
					alerts: computed(() => this.Resources[hardcodedTexts.alerts]),
					user: computed(() => this.Resources[hardcodedTexts.user]),
					userAvatar: computed(() => this.Resources[hardcodedTexts.userAvatar]),
					userHelp: computed(() => this.Resources[hardcodedTexts.userHelp]),
					userSettings: computed(() => this.Resources[hardcodedTexts.userSettings]),
					leave: computed(() => this.Resources[hardcodedTexts.leave]),
					year: computed(() => this.Resources[hardcodedTexts.year])
				},

				showUserOptionsMenu: false
			}
		},

		mounted()
		{
			this.fetchMenuEntries()
		},

		computed: {
			...mapState(useSystemDataStore, [
				'system'
			]),

			...mapState(useGenericDataStore, [
				'notifications'
			]),

			fullName()
			{
				if (this.model.Avatar && this.model.Avatar.fullname)
					return this.model.Avatar.fullname
				return this.userData.name
			},

			userRole()
			{
				if (this.model.Avatar && this.model.Avatar.position)
					return this.model.Avatar.position
				return this.texts.user
			},

			avatarSrc()
			{
				if (this.model.Avatar && this.model.Avatar.image)
					return this.model.Avatar.image
				return `${this.$app.resourcesPath}user_avatar.png?v=${this.$app.genio.buildVersion}`
			}
		},

		methods: {
			logOff,

			/**
			 * Emits an event to toggle the alerts tab.
			 */
			toggleAlert()
			{
				this.$eventHub.emit('toggle-sidebar-on-tab', 'alerts-tab')
			},

			/**
			 * Clears all the current data.
			 */
			clearModel()
			{
				this.model = {
					Avatar: {},
					UserAvatarMenus: [],
					EPHUserAvatarMenus: []
				}
			},

			/**
			 * Get the necessary data to render the component (Avatar image, Fullname, Position, custom menu list and EPH menu list).
			 */
			fetchMenuEntries()
			{
				if (this.userIsLoggedIn)
				{
					fetchData(
						'Account',
						'UserAvatar',
						{},
						(data) => {
							_merge(this.model, data)

							this.setOpenIdAuth(data.HasOpenIdAuth)
							this.set2FAOptions(data.Has2FAOptions)
						})
				}
				else
					this.clearModel()
			},

			/**
			 * Build the route for avatar custom menu list.
			 */
			getMenuRoute(menu)
			{
				if (typeof menu !== 'object')
					menu = {}

				let routeName = 'home'
				if (!this.isEmpty(menu.Action))
					routeName = menu.Action

				const routeParams = {
					name: routeName,
					params: {
						culture: this.system.currentLang,
						system: this.system.currentSystem,
						module: this.system.currentModule
					}
				}

				if (!this.isEmpty(menu.RecordID))
				{
					routeParams.params.id = menu.RecordID
					routeParams.params.mode = 'SHOW'
				}

				return routeParams
			},

			/**
			 * Gets the translation for the given menu title id.
			 */
			getMenuText(menuTitleId)
			{
				return !this.isEmpty(menuTitleId) ? this.Resources[menuTitleId] : ''
			},

			/**
			 * Navigates to the user's profile.
			 */
			userSettings()
			{
				if (this.hasUserSettings)
					this.$router.push({ name: 'profile' })

				this.closeOverlay()
			},

			/**
			 * Closes the right side bar and the navigation menu.
			 */
			closeSidebars() {
				if (this.mobileLayoutActive) {
					this.collapseSidebar()
					this.$eventHub.emit('user-options-menu-open');
				}
			},

			closeOverlay() {
				this.showUserOptionsMenu = false
			}
		},

		watch: {
			'system.currentModule'()
			{
				this.fetchMenuEntries()
			},

			showUserOptionsMenu(val) {
				if (val)
					this.closeSidebars()
			}
		}
	}
</script>
