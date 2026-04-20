<template>
	<div class="c-sidebar--container__section">
		<div class="c-sidebar__subtitle">
			<q-icon icon="note-text" />
			<span>{{ texts.notes }}</span>
		</div>

		<div class="c-sidebar__notes-buttoncontainer">
			<q-row>
				<q-col v-if="!noNotes">
					<q-button
						:title="texts.insert"
						:label="texts.insert"
						:disabled="formIsOpen"
						@click="openNoteForm">
						<q-icon icon="note-plus" />
					</q-button>
				</q-col>
				<q-col>
					<span class="c-sidebar__notes-counter">
						{{ texts.total }}: {{ notesCount }}
					</span>
				</q-col>
				<q-col cols="2">
					<q-button
						ref="activator"
						:title="texts.filters"
						data-type="options-button"
						@click="setOverlayState(!this.filtersOverlay)">
						<q-icon icon="filter-menu" />
					</q-button>
					<q-overlay
						v-model="filtersOverlay"
						:anchor="activatorRef?.$el"
						placement="bottom-end"
						trigger="manual"
						spy
						non-modal
						@enter="onOverlayEnter"
						@leave="onOverlayLeave">
						<q-list
							ref="filterList"
							selectable
							:model-value="filterValue"
							:items="filterOptions"
							@update:model-value="onFilterSelect">
						</q-list>
					</q-overlay>
				</q-col>
			</q-row>
		</div>

		<div class="c-sidebar__notes-container">
			<q-info-message
				v-if="showSuccessSave"
				type="success"
				:text="texts.successSave"
				is-dismissible
				dismiss-time="5"
				@message-dismissed="setSuccessMessage(false)" />

			<template v-if="noNotes && !formIsOpen">
				<q-row
					justify="center"
					class="c-sidebar__notes-empty">
					<q-col>
						<img :src="`${$app.resourcesPath}empty_card_container.png?v=${$app.genio.buildVersion}`" />
					</q-col>
				</q-row>
				<q-row class="c-sidebar__notes-empty">
					<q-col>
						<span>{{ texts.emptyNotes }}</span>
					</q-col>
				</q-row>
				<q-row class="c-sidebar__notes-empty">
					<q-col>
						<q-button
							id="notes-insert"
							:label="texts.insert"
							@click="openNoteForm">
							<q-icon icon="note-plus" />
						</q-button>
					</q-col>
				</q-row>
			</template>

			<q-note-form
				v-if="formIsOpen"
				@success-save="successSaveHandler"
				@close="closeNoteForm" />

			<q-row
				v-for="note in filteredNotes"
				:key="note.id"
				:id="note.id">
				<q-col>
					<q-note
						:id="note.id"
						:note="note"
						@delete="deleteNote(note.id)" />
				</q-col>
			</q-row>
		</div>
	</div>
</template>

<script>
	import { computed, useTemplateRef } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import QNote from './notes/QNote.vue'
	import QNoteForm from './notes/QNoteForm.vue'

	export default {
		name: 'QNotes',

		emits: [
			/**
			 * Requests the parent component to load notes for the current context.
			 */
			'fetch-notes',
			/**
			 * Requests the parent component to delete a note by id.
			 */
			'delete-note'
		],

		components: {
			QNote,
			QNoteForm
		},

		mixins: [
			LayoutHandlers
		],

		props: {
			/**
			 * Notes to display in the sidebar.
			 */
			notes: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		data()
		{
			return {
				texts: {
					notes: computed(() => this.Resources[hardcodedTexts.notes]),
					filters: computed(() => this.Resources[hardcodedTexts.filters]),
					insert: computed(() => this.Resources[hardcodedTexts.insert]),
					successSave: computed(() => this.Resources[hardcodedTexts.recordSuccessSave]),
					emptyNotes: computed(() => this.Resources[hardcodedTexts.emptyNotes]),
					total: computed(() => this.Resources[hardcodedTexts.total])
				},

				formIsOpen: false,
				showSuccessSave: false,

				activatorRef: useTemplateRef('activator'),
				filterListRef: useTemplateRef('filterList'),
				filtersOverlay: false,
				filterOptions: [
					{ key: 0, label: computed(() => this.Resources[hardcodedTexts.viewAll]) },
					{ key: 1, label: computed(() => this.Resources[hardcodedTexts.viewPersonal]) }
				],
				filterValue: 0
			}
		},

		created()
		{
			this.loadNotes()
		},

		computed: {
			/**
			 * Total number of notes.
			 *
			 * @returns {number}
			 */
			notesCount()
			{
				return Array.isArray(this.notes) ? this.notes.length : 0
			},

			/**
			 * Whether there are no notes to display.
			 *
			 * @returns {boolean}
			 */
			noNotes()
			{
				return this.notesCount === 0
			},

			/**
			 * Notes filtered according to the current filter selection.
			 */
			filteredNotes()
			{
				const notes = Array.isArray(this.notes) ? this.notes : []

				if (this.filterValue === 1)
					return notes.filter((note) => note?.destType === 'P')

				return notes
			}
		},

		methods: {
			/**
			 * Asks the parent to fetch notes for the current context.
			 */
			loadNotes()
			{
				this.$emit('fetch-notes')
			},

			/**
			 * Asks the parent to delete the specified note.
			 *
			 * @param {string|number} noteId - Note identifier.
			 */
			deleteNote(noteId)
			{
				this.$emit('delete-note', noteId)
			},

			/**
			 * Opens the note creation form.
			 */
			openNoteForm()
			{
				this.formIsOpen = true
			},

			/**
			 * Closes the note creation form.
			 */
			closeNoteForm()
			{
				this.formIsOpen = false
			},

			/**
			 * Shows or hides the «saved successfully» message.
			 *
			 * @param {boolean} isVisible - Visibility flag.
			 */
			setSuccessMessage(isVisible)
			{
				this.showSuccessSave = isVisible
			},

			/**
			 * Called when the form reports a successful save.
			 */
			successSaveHandler()
			{
				this.setSuccessMessage(true)
				this.closeNoteForm()
				this.loadNotes()
			},

			/**
			 * Updates the active filter value from the dropdown selection.
			 * @param newVal - The new selected filter
			 */
			onFilterSelect(newVal) {
				this.filterValue = newVal
				this.setOverlayState(false)
			},

			setOverlayState(isActive) {
				this.filtersOverlay = isActive
			},

			/**
			 * Sets focus to the filter button.
			 */
			focusFilterButton() {
				this.activatorRef?.$el?.focus()
			},

			/**
			 * Sets focus to the dropdown list.
			 */
			focusList() {
				this.filterListRef?.$el?.focus()
			},

			/**
			* Called when the dropdown overlay becomes visible.
			*/
			onOverlayEnter() {
				// Wait to focus until the element exists
				this.$nextTick().then(() => {
					this.focusList()
				})
			},

			/**
			* Called when the dropdown overlay becomes hidden.
			*/
			onOverlayLeave() {
				this.setOverlayState(false)
				this.focusFilterButton()
			},
		}
	}
</script>
