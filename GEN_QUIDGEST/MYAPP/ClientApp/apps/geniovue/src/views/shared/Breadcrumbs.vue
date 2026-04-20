<template>
	<div
		v-if="hasBreadcrumbs"
		:class="containerClasses">
		<nav aria-label="breadcrumb">
			<ol class="n-breadcrumb">
				<li
					v-for="(breadcrumbs, idx) in breadcrumbEntries"
					:key="`breadcrumb-${idx}`"
					:class="[{ active: breadcrumbs[0].isActive }, 'n-breadcrumb__item']">
					<template v-if="breadcrumbs.length > 1">
						<a
							id="dropdown-menu-link"
							class="dropdown-toggle dropdown n-breadcrumb__link"
							role="button"
							href="#"
							data-toggle="dropdown"
							aria-haspopup="true"
							aria-expanded="false">
							<q-icon
								icon="more-items"
								class="n-breadcrumb__icon" />
						</a>

						<div
							class="dropdown-menu"
							aria-labelledby="dropdown-menu-link">
							<breadcrumbs-content :breadcrumbs="breadcrumbs" />
						</div>
					</template>
					<breadcrumbs-content
						v-else
						:breadcrumbs="breadcrumbs" />
				</li>
			</ol>
		</nav>
	</div>
</template>

<script>
	import { useNavDataStore } from '@quidgest/clientapp/stores'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import { formModes, breadcrumbTypes } from '@quidgest/clientapp/constants/enums'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import BreadcrumbsContent from '@/views/shared/BreadcrumbsContent.vue'

	export default {
		name: 'QBreadcrumbs',

		components: {
			BreadcrumbsContent
		},

		mixins: [
			LayoutHandlers
		],

		props: {
			/**
			 * The ID of the current navigation.
			 */
			navigationId: {
				type: String,
				default: 'main'
			},

			/**
			 * Whether or not the breadcrumbs should be visible.
			 */
			isVisible: {
				type: Boolean,
				default: true
			}
		},

		expose: [],

		computed: {
			/**
			 * True if the breadcrumbs should be visible, false otherwise.
			 */
			hasBreadcrumbs()
			{
				return this.isVisible && this.$app.layout.BreadcrumbsContent !== 'hidden' && this.breadcrumbsData.length > 1
			},

			/**
			 * A list with the breadcrumb entries in the right format to be displayed.
			 */
			breadcrumbEntries()
			{
				const entries = []
				let entryList = []
				const count = this.breadcrumbsData.length

				for (let i = 0; i < count; i++)
				{
					entryList.push(this.breadcrumbsData[i])

					// We don't want to have more than 3 levels of breadcrumbs, so we group the breadcrumb entries in the middle of the path.
					if (i > 0 && i < count - 2)
						continue

					entries.push(entryList.reverse())
					entryList = []
				}

				return entries
			},

			/**
			 * The list of breadcrumbs.
			 */
			breadcrumbsData()
			{
				return this.createBreadcrumbs()
			}
		},

		methods: {
			/**
			 * Checks whether or not the specified breadcrumb entry is the one currently active.
			 * @param {object} navEntry The data about the entry in the navigation history
			 * @returns True if the breadcrumb entry is active, false otherwise.
			 */
			isEntryActive(navEntry)
			{
				const navDataStore = useNavDataStore()
				const currentLevel = navDataStore.navigation.getHistory(this.navigationId).currentLevel
				return !this.isEmpty(currentLevel) && navEntry.location === currentLevel.location
			},

			/**
			 * Builds the route link of the specified breadcrumb entry's route name.
			 * @param {object} navEntry The data about the entry in the navigation history
			 * @returns An object with the route link of the breadcrumb entry.
			 */
			getRouteLink(navEntry)
			{
				const params = {
					...navEntry.params
				}

				// Ensures that clicking the breadcrumb will display a confirmation message, in case we are on a dirty form.
				delete params.isControlled

				return {
					name: navEntry.location,
					params
				}
			},

			/**
			 * Finds the type of the specified breadcrumb entry.
			 * @param {object} navEntry The data about the entry in the navigation history
			 * @returns A string with the type of the breadcrumb entry.
			 */
			getBreadcrumbType(navEntry)
			{
				if (navEntry.location.startsWith(breadcrumbTypes.home))
					return breadcrumbTypes.home
				else if (navEntry.location.startsWith(breadcrumbTypes.menu))
					return breadcrumbTypes.menu
				else if (navEntry.location.startsWith(breadcrumbTypes.form))
					return breadcrumbTypes.form
				return ''
			},

			/**
			 * Calculates the icon based on the type of route.
			 * @param {object} navEntry The data about the entry in the navigation history
			 * @returns A string with the icon of the breadcrumb entry.
			 */
			getBreadcrumbIcon(navEntry)
			{
				const breadcrumbType = this.getBreadcrumbType(navEntry)
				let iconType = breadcrumbType

				// If it's a form, we want to indicate the form mode through the icon.
				if (breadcrumbType === breadcrumbTypes.form && navEntry.params.mode)
					iconType = navEntry.params.mode

				switch (iconType)
				{
					case breadcrumbTypes.home:
						return 'home'
					case breadcrumbTypes.menu:
						return 'list'
					case formModes.show:
						return 'view'
					case formModes.new:
						return 'plus'
					case formModes.edit:
						return 'pencil'
					case formModes.duplicate:
						return 'duplicate'
					case formModes.delete:
						return 'remove'
					default:
						return ''
				}
			},

			/**
			 * Builds a string with the path to the menu with the specified order.
			 * @param {string} menuOrder The order of the desired menu
			 * @param {object} menus An array with the menus
			 * @returns A string with the path to the menu.
			 */
			buildMenuPath(menuOrder, menus)
			{
				if (typeof menuOrder !== 'string' || this.isEmpty(menuOrder) || !Array.isArray(menus) || menus.length === 0)
					return ''

				for (const m of menus)
				{
					if (menuOrder.startsWith(m.Order))
					{
						const nextMenu = this.buildMenuPath(menuOrder, m.Children)
						return this.Resources[m.Title] + (this.isEmpty(nextMenu) ? '' : ` > ${nextMenu}`)
					}
				}

				return ''
			},

			/**
			 * Builds the text to be shown in the specified breadcrumb entry.
			 * @param {object} navEntry The data about the entry in the navigation history
			 * @returns A string with the breadcrumb entry text.
			 */
			getBreadcrumbText(navEntry)
			{
				const breadcrumbType = this.getBreadcrumbType(navEntry)

				if (breadcrumbType === breadcrumbTypes.home)
					return Object.keys(this.system.availableModules).length > 1 ? this.currentModuleTitle : ''
				else if (navEntry.location.startsWith(breadcrumbTypes.menu))
				{
					if (this.isEmpty(navEntry.properties))
						return ''

					const entryOrder = navEntry.properties.routeBranch
					let menuPath = this.buildMenuPath(entryOrder, this.menus.MenuList)

					if (!this.isEmpty(navEntry.properties) && !this.isEmpty(navEntry.properties.tableName))
					{
						if (!this.isEmpty(menuPath))
							menuPath += ' > '
						menuPath += navEntry.properties.tableName
					}

					return menuPath
				}
				else if (breadcrumbType === breadcrumbTypes.form)
				{
					if (this.isEmpty(navEntry.properties))
						return ''

					return navEntry.properties.breadcrumbName || ''
				}

				return ''
			},

			/**
			 * Retrieves the human key of the specified breadcrumb entry.
			 * @param {object} navEntry The data about the entry in the navigation history
			 * @returns A string with the human key.
			 */
			getHumanKey(navEntry)
			{
				const breadcrumbType = this.getBreadcrumbType(navEntry)

				if (breadcrumbType === breadcrumbTypes.form)
				{
					if (this.isEmpty(navEntry.properties))
						return ''

					return navEntry.properties.humanKey || ''
				}

				return ''
			},

			/**
			 * Adds a new entry to the list of breadcrumbs.
			 * @param {object} navEntry The data about the entry in the navigation history
			 */
			createEntry(navEntry)
			{
				const breadcrumbRoute = this.getRouteLink(navEntry)
				const breadcrumbType = this.getBreadcrumbType(navEntry)
				const breadcrumbIcon = this.getBreadcrumbIcon(navEntry)
				const breadcrumbText = this.getBreadcrumbText(navEntry)
				const hoverText = breadcrumbType === breadcrumbTypes.home ? this.Resources[hardcodedTexts.initialPage] : null
				const humanKey = this.getHumanKey(navEntry)
				const isActive = this.isEntryActive(navEntry)

				return {
					text: breadcrumbText,
					hoverText: hoverText,
					icon: breadcrumbIcon,
					route: breadcrumbRoute,
					humanKey: humanKey,
					isActive: isActive
				}
			},

			/**
			 * Builds the breadcrumbs.
			 */
			createBreadcrumbs()
			{
				const navDataStore = useNavDataStore()
				const navHistory = navDataStore.navigation.getHistory(this.navigationId).convertToCollection()
				const breadcrumbsData = []

				for (const navEntry of navHistory)
				{
					if (navEntry.isNested)
						continue
					if (navEntry.params.skipLastMenu === 'true')
						breadcrumbsData.pop()

					const breadcrumbEntry = this.createEntry(navEntry)
					breadcrumbsData.push(breadcrumbEntry)
				}

				return breadcrumbsData
			}
		}
	}
</script>
