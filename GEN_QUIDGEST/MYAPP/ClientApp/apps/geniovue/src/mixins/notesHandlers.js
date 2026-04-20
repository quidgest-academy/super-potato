import { mapActions, mapState } from 'pinia'

import netAPI from '@quidgest/clientapp/network'
import { getNoteContext } from '@/mixins/notesFunctions.js'
import {
	useNotesStore,
	useSystemDataStore,
	useUserDataStore
} from '@quidgest/clientapp/stores'

/**
 * Notes handlers mixin.
 *
 * Centralises the network calls for listing and deleting notes and keeps the store in sync.
 * The backend expects a *route-derived context* (see `getNoteContext`) to scope notes to the current UI location.
 */
export default {
	computed: {
		...mapState(useSystemDataStore, [
			'system'
		]),

		...mapState(useUserDataStore, [
			'userIsLoggedIn',
			'valuesOfPHEs'
		]),

		/**
		 * Number of notes currently available in the store.
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
	},

	methods: {
		...mapActions(useNotesStore, [
			'setNotes'
		]),

		/**
		 * Loads notes from the API for the current route context.
		 *
		 * @returns {Promise<unknown>} A promise resolved by the underlying network layer.
		 */
		fetchNotes()
		{
			if (!this.$app.isNotesAvailable || !this.userIsLoggedIn)
				return Promise.resolve(true)

			return netAPI.postData(
				'QNotesApi',
				'GetNotes',
				{
					context: getNoteContext(this.$route)
				},
				(data) => {
					this.setNotes(data)

					if(this.notesCount > 0)
						this.$eventHub.emit('open-sidebar-on-tab', 'notes-tab')
				})
		},

		/**
		 * Deletes a note by id and refreshes the list on success.
		 *
		 * @param {string|number} id - Note identifier.
		 * @returns {Promise<unknown>} A promise resolved by the underlying network layer.
		 */
		deleteNote(id)
		{
			if (!this.$app.isNotesAvailable || !this.userIsLoggedIn)
				return Promise.resolve(true)

			return netAPI.postData(
				'QNotesApi',
				'DeleteNote',
				{
					id
				},
				() => {
					this.fetchNotes()
				})
		}
	},

	watch: {
		// When the user changes module, refresh notes for the new context.
		'system.currentModule'()
		{
			this.fetchNotes()
		},

		// Some modules depend on PHE values; refresh if those values change.
		valuesOfPHEs: {
			handler()
			{
				this.fetchNotes()
			},
			deep: true
		},

		// Refresh when the route changes
		$route()
		{
			this.fetchNotes()
		}
	}
}
