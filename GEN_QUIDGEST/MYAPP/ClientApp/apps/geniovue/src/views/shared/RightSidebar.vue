<template>
	<div
		v-if="userIsLoggedIn"
		v-show="!isSidebarEmpty"
		class="wrapper">
		<div class="c-right-sidebar__control">
			<!-- This button must use the v-show because the button must always exist -->
			<q-button
				v-show="rightSidebarIsCollapsed"
				ref="sidebarOpenButton"
				:id="sidebarOpenButtonId"
				class="right-sidebar-control"
				:title="texts.open"
				@click="openSidebar">
				<q-icon icon="step-back" />
			</q-button>
		</div>

		<div
			ref="sidebar"
			id="right-sidenav"
			:class="classes"
			@transitionend="onTransitionEnd">
			<div class="c-right-sidebar__container">
				<nav class="c-right-sidebar__header">
					<div class="nav flex-column nav-pills">
						<q-button
							ref="sidebarCloseButton"
							id="close-sidebar-btn"
							:title="texts.close"
							:disabled="disableButtons"
							@click="closeSidebar">
							<q-icon icon="close" />
						</q-button>

						<q-toggle
							v-if="$app.isNotesAvailable && userIsLoggedIn"
							id="q-notes-open"
							:model-value="isActive('notes-tab')"
							:title="texts.notes"
							:disabled="disableButtons"
							@click="toggleSidebarTab('notes-tab')">
							<q-badge-indicator
								v-if="notesCount > 0"
								color="highlight">
								<q-icon icon="note-text" />
							</q-badge-indicator>
							<q-icon
								v-else
								icon="note-text" />
						</q-toggle>

						<q-toggle
							v-if="$app.isCavAvailable && !suggestionModeOn"
							:model-value="reportingModeCAV"
							id="advanced-report-mode-toggle"
							:title="texts.enterInReport"
							:disabled="disableButtons"
							@click="toggleReportingMode">
							<q-icon icon="stats" />
						</q-toggle>

						<q-toggle
							v-if="showFormAnchors && !suggestionModeOn && $app.layout.FormAnchorsPosition === 'sidebar'"
							id="form-tree-toggle"
							:model-value="isActive('form-anchors-tab')"
							:title="texts.formAreas"
							:disabled="disableButtons"
							@click="toggleSidebarTab('form-anchors-tab')">
							<q-icon icon="list-bordered" />
						</q-toggle>

						<q-toggle
							v-show="showFormActions && !suggestionModeOn"
							id="form-actions-toggle"
							:model-value="isActive('form-actions-tab')"
							:title="texts.formActions"
							:disabled="disableButtons"
							@click="toggleSidebarTab('form-actions-tab')">
							<q-icon icon="more-items" />
						</q-toggle>

						<q-toggle
							v-if="$app.appAlerts.length > 0 && !suggestionModeOn"
							id="alerts-btn"
							:model-value="isActive('alerts-tab')"
							:title="texts.alerts"
							:disabled="disableButtons"
							@click="toggleSidebarTab('alerts-tab')">
							<q-badge-indicator
								:enabled="notifications.length > 0"
								color="highlight">
								<q-icon icon="notifications" />
							</q-badge-indicator>
						</q-toggle>

						<q-toggle
							v-if="$app.isSuggestionsAvailable"
							id="suggestion-mode-toggle"
							:model-value="suggestionModeOn"
							:title="suggestionModeOn ? texts.closeSuggestions : texts.enterInSuggestion"
							:disabled="disableButtons"
							@click="toggleSuggestionModeOn">
							<q-icon :icon="suggestionModeOn ? 'suggestion-mode-close' : 'suggestion-mode'" />
						</q-toggle>

						<q-button
							v-if="suggestionModeOn"
							id="suggestion-view"
							:title="texts.suggestions"
							:disabled="disableButtons"
							@click="openSuggestionList">
							<q-icon icon="suggestion-mode-view" />
						</q-button>

						<q-button
							v-if="suggestionModeOn"
							id="suggestion-open"
							:title="texts.suggest"
							:disabled="disableButtons"
							@click="openSuggestionMode">
							<q-icon icon="new-suggestion" />
						</q-button>

						<q-toggle
							v-if="$app.isChatBotAvailable"
							id="chatbot-toggle"
							:model-value="isActive('chatbot-tab')"
							title="ChatBot"
							class="nav-link"
							:disabled="disableButtons"
							@click="toggleChatBot()">
							<q-icon-img
								:icon="`${$app.resourcesPath}chatbot.png?v=${$app.genio.buildVersion}`"
								alt="ChatBot" />
						</q-toggle>
					</div>
				</nav>

				<div class="c-right-sidebar__content">
					<div
						v-show="extendedTab === 'form-actions-tab' && showFormActions"
						id="form-actions-tab"
						class="c-tab__item--sidebar">
						<form-action-buttons :buttons-list="formModeData" />
					</div>

					<div
						v-if="$app.appAlerts.length > 0"
						v-show="extendedTab === 'alerts-tab'"
						id="alerts-tab">
						<alerts
							:alerts="notifications"
							@fetch-alerts="fetchAlerts"
							@clear-alerts="clearNotifications"
							@dismiss-alert="removeNotification"
							@navigate-to="onAlertClick" />
					</div>

					<div
						v-show="extendedTab === 'form-anchors-tab' && showFormAnchors"
						id="form-anchors-tab">
						<q-anchor-container-vertical
							:title="texts.formAreas"
							:tree="formAnchorsTree"
							@focus-control="(...args) => $emit('focus-control', ...args)" />
					</div>

					<div
						v-if="$app.isChatBotAvailable"
						v-show="extendedTab === 'chatbot-tab'"
						id="chatbot-tab"
						ref="chatbotTab">
						<q-chat-bot
							:username="userData.name"
							:project-path="$app.applicationName"
							:date-format="dateFormat.dateTimeSeconds"
							:agent-data="currentAgent"
							:available-agents="availableAgents"
							:api-endpoint="chatbotProxyUrl"
							@direct-agent-chat="setAgentData"
							@apply-fields="applyFields" />
					</div>

					<div
						v-if="$app.isNotesAvailable && userIsLoggedIn"
						v-show="extendedTab === 'notes-tab'"
						id="notes">
						<q-notes
							:notes="notes"
							@fetch-notes="fetchNotes"
							@delete-note="deleteNote" />
					</div>

					<div v-show="extendedTab === 'widgets-panel'">
						<div id="widgets-panel" />
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { computed, defineAsyncComponent, nextTick } from 'vue'
	import { mapState, mapActions } from 'pinia'

	import { useGenericDataStore } from '@quidgest/clientapp/stores'
	import { useAiDataStore } from '@quidgest/clientapp/stores'
	import { useNotesStore } from '@quidgest/clientapp/stores'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'
	import AlertHandlers from '@/mixins/alertHandlers.js'
	import NotesHandlers from '@/mixins/notesHandlers.js'

	export default {
		name: 'QSidebar',

		emits: [
			'changed-sidebar-width',
			'focus-control'
		],

		components: {
			QAnchorContainerVertical: defineAsyncComponent(() => import('@/components/containers/QAnchorContainerVertical.vue')),
			FormActionButtons: defineAsyncComponent(() => import('./FormActionButtons.vue')),
			Alerts: defineAsyncComponent(() => import('./Alerts.vue')),
			QChatBot: defineAsyncComponent(() => import('@quidgest/chatbot')),
			QNotes: defineAsyncComponent(() => import('./QNotes.vue'))
		},

		mixins: [
			AlertHandlers,
			LayoutHandlers,
			NotesHandlers
		],

		expose: [],

		data()
		{
			return {
				formModeData: {},

				formAnchorsTree: [],

				extendedTab: '',

				focusedSidebarButtonId: null,

				texts: {
					open: computed(() => this.Resources[hardcodedTexts.open]),
					alerts: computed(() => this.Resources[hardcodedTexts.alerts]),
					close: computed(() => this.Resources[hardcodedTexts.close]),
					formActions: computed(() => this.Resources[hardcodedTexts.formActions]),
					enterInReport: computed(() => this.Resources[hardcodedTexts.enterInReport]),
					enterInSuggestion: computed(() => this.Resources[hardcodedTexts.enterInSuggestion]),
					suggest: computed(() => this.Resources[hardcodedTexts.suggest]),
					suggestions: computed(() => this.Resources[hardcodedTexts.suggestions]),
					closeSuggestions: computed(() => this.Resources[hardcodedTexts.closeSuggestions]),
					formAreas: computed(() => this.Resources[hardcodedTexts.formAreas]),
					notes: computed(() => this.Resources[hardcodedTexts.notes]),
				}
			}
		},

		mounted()
		{
			if (this.options?.autoCollapseSize)
			{
				this.autoCloseSidebar(false)
				//Must be called here to finalize visibilty because the open/close transition does not happen when loading
				this.onTransitionEnd()
				window.addEventListener('resize', this.autoCloseSidebar)
			}

			this.$eventHub.on('changed-form-buttons', (sections) => {
				this.formModeData = sections
			})

			this.$eventHub.on('changed-form-anchors', (tree) => {
				this.formAnchorsTree = tree
			})

			this.$eventHub.on('open-sidebar-on-tab', (tabId) => {
				if (this.extendedTab === tabId)
					return

				this.openSidebar()
				this.toggleSidebarTab(tabId)
			})

			this.$eventHub.on('toggle-sidebar-on-tab', (tabId) => {
				this.toggleSidebarTab(tabId)
			})

			this.$eventHub.on('toggle-sidebar', (state) => {
				if (state === 'expand' && this.mobileLayoutActive)
					this.closeSidebar()
			})

			this.$eventHub.on('user-options-menu-open', () => {
				if (this.mobileLayoutActive)
					this.closeSidebar()
			})

			// Sets the default state for the sidebar (closed or opened).
			if (this.$app.layout.DefaultSidebarState !== 'opened' && !this.$app.isChatBotAvailable)
				this.closeSidebar()

			// Emits the initial width of the sidebar.
			this.onSidebarWidthChange()
		},

		beforeUnmount()
		{
			if (this.options?.autoCollapseSize)
				window.removeEventListener('resize', this.autoCloseSidebar)

			this.$eventHub.off('changed-form-buttons')
			this.$eventHub.off('changed-form-tree')
			this.$eventHub.off('open-sidebar-on-tab')
			this.$eventHub.off('toggle-sidebar-on-tab')
			this.$eventHub.off('user-options-menu-open')

			this.onSidebarWidthChange()
		},

		computed: {
			...mapState(useGenericDataStore, [
				'reportingModeCAV',
				'suggestionModeOn',
				'notifications',
				'dateFormat'
			]),

			...mapState(useAiDataStore, [
				'chatbotProxyUrl',
				'currentAgent',
				'availableAgents'
			]),

			...mapState(useNotesStore, [
				'notes'
			]),

			/**
			 * True if the button to toggle the form actions should be visible, false otherwise.
			 */
			showFormActions()
			{
				return Object.keys(this.formModeData).length > 0
			},

			/**
			 * The width of the right sidebar (in rem).
			 */
			sidebarWidth()
			{
				let width = 0

				if (this.userIsLoggedIn && !this.rightSidebarIsCollapsed)
				{
					if (this.extendedTab)
					{
						if (this.extendedTab === 'chatbot-tab')
							width = 35
						else
							width = 18.75
					}
					else if (!this.isSidebarEmpty)
						width = 3.125
				}

				return width
			},

			/**
			 * True if the form anchors should be visible, false otherwise.
			 */
			showFormAnchors()
			{
				return Array.isArray(this.formAnchorsTree) && this.formAnchorsTree.length > 0
			},

			/**
			 * True if the buttons of the sidebar should be disabled, false otherwise.
			 */
			disableButtons()
			{
				return this.extendedTab === 'widgets-panel'
			},

			/**
			 * True if the alerts tab should be visible, false otherwise.
			 */
			hasAlerts()
			{
				return this.$app.appAlerts.length > 0
			},

			/**
			 * True if the alerts tab should be visible, false otherwise.
			 */
			showAlerts()
			{
				return this.hasAlerts && this.extendedTab === 'alerts-tab'
			},

			/**
			 * True if the sidebar is empty, false otherwise.
			 */
			isSidebarEmpty()
			{
				return !this.showFormActions &&
					!this.$app.isNotesAvailable &&
					!this.$app.isCavAvailable &&
					!this.$app.isSuggestionsAvailable &&
					!this.$app.isChatBotAvailable &&
					!this.showFormAnchors &&
					!this.disableButtons &&
					!this.hasAlerts
			},

			/**
			 * The component classes.
			 */
			classes()
			{
				const classes = ['c-right-sidebar']

				if (!this.rightSidebarIsVisible)
					classes.push('invisible')

				return classes
			},

			/**
			 * ID of the DOM element to open the sidebar.
			 */
			sidebarOpenButtonId()
			{
				return "open-sidebar-btn"
			}
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'setSuggestionMode',
				'toggleSuggestionMode',
				'clearNotifications',
				'removeNotification'
			]),

			...mapActions(useAiDataStore, [
				'setCurrentAgent'
			]),

			...mapActions(useNotesStore, [
				'loadNotes'
			]),

			onSidebarWidthChange()
			{
				if (this.userIsLoggedIn && !this.isSidebarEmpty)
					this.$emit('changed-sidebar-width', this.sidebarWidth)
				else
					this.$emit('changed-sidebar-width', 0)
			},

			async toggleChatBot()
			{
				this.toggleSidebarTab('chatbot-tab')
				this.setCurrentAgent({ id: '' })

				const isOpen = this.extendedTab === 'chatbot-tab'

				// Scroll to bottom of chat, if opening
				if (isOpen)
				{
					// Wait until content has fully opened
					await nextTick()
					const chatbotMessages = this.$refs.chatbotTab.querySelector('.q-chatbot__messages-container')
					if(chatbotMessages)
						chatbotMessages.scrollTop = chatbotMessages.scrollHeight
				}

				return isOpen
			},

			applyFields(fields)
			{
				this.$eventHub.emit('apply-agent-fields', fields)
			},

			setAgentData(agentId, userPrompt)
			{
				const agentData = {
					agentId: agentId,
					userPrompt: userPrompt
				}
				this.$eventHub.emit('set-agent-data', agentData)
			},

			openSidebar()
			{
				/*
				 * Check if the open button was focused and save this value which is checked after the CSS transition ends.
				 * This must be done here because the button element disappears and loses focus during this function,
				 * after setting the collapse state.
				 */
				if (document?.activeElement?.id === this.sidebarOpenButtonId)
					this.focusedSidebarButtonId = this.sidebarOpenButtonId

				this.setRightSidebarCollapseState(false)
			},

			closeSidebar()
			{
				this.setRightSidebarCollapseState(true)
				this.extendedTab = ''
			},

			/**
			 * Called when a CSS transition for the right sidebar finishes
			 */
			onTransitionEnd()
			{
				// If the right sidebar is being closed, set the actual value for visibility to false.
				// Must be done here, after the transition ends so it doesn't disappear before the CSS transition.
				if (this.rightSidebarIsCollapsed)
					this.setRightSidebarVisibility(false)

				// Check if any of the sidebar buttons were focused and, if so, decide which one the focus should move to.
				// Must be done here, after the CSS transition ends so the element that will be focused is visible and focusable.
				const sidebarOpenButton = this.$refs?.sidebarOpenButton
				const sidebarCloseButton = this.$refs?.sidebarCloseButton
				const sidebar = this.$refs?.sidebar

				// If the open button was focused
				if (this.focusedSidebarButtonId === this.sidebarOpenButtonId)
				{
					// Focus on the close button
					sidebarCloseButton?.$el?.focus()
					this.focusedSidebarButtonId = null
				}
				// If any of the buttons within the sidebar were focused
				else if (sidebar?.contains(document?.activeElement))
				{
					// Focus on the open button
					sidebarOpenButton?.$el?.focus()
				}
			},

			toggleSidebarTab(tabId)
			{
				if (this.extendedTab === tabId)
					this.extendedTab = ''
				else
				{
					this.extendedTab = tabId

					// Close any other menu that is currently open.
					if (this.mobileLayoutActive)
						this.$eventHub.emit('right-sidebar-open')
				}
			},

			toggleReportingMode()
			{
				this.$eventHub.emit('toggle-reporting-mode')
			},

			toggleSuggestionModeOn()
			{
				this.toggleSuggestionMode()

				if (this.suggestionModeOn)
					this.extendedTab = ''
			},

			openSuggestionMode()
			{
				const params = {
					id: '',
					label: '',
					help: '',
					arrayName: '',
				}

				this.$eventHub.emit('show-suggestion-popup', 'SuggestionIndex', params)
			},

			openSuggestionList()
			{
				this.$eventHub.emit('show-suggestion-popup', 'SuggestionList', {})
			},

			/**
			 * Collapses the right sidebar when a certain screen size is reached.
			 * @param {boolean} resize Whether or not the window is being resized
			 */
			autoCloseSidebar(resize = true)
			{
				if (resize && !this.options?.autoCollapseSize || this.extendedTab === 'widgets-panel')
					return

				if (this.options?.autoCollapseSize && window.innerWidth <= this.options.autoCollapseSize)
					this.closeSidebar()
				else
					this.openSidebar()
			},

			isActive(buttonId)
			{
				return this.extendedTab === buttonId
			}
		},

		watch: {
			isSidebarEmpty()
			{
				this.onSidebarWidthChange()
			},

			sidebarWidth()
			{
				this.onSidebarWidthChange()
			},

			$route(to)
			{
				if (typeof to.name !== 'string' || !to.name.startsWith('form'))
				{
					this.formModeData = {}
					this.formAnchorsTree = []

					if (this.extendedTab === 'form-actions-tab' || this.extendedTab === 'form-anchors-tab')
						this.extendedTab = ''
				}
			}
		}
	}
</script>
