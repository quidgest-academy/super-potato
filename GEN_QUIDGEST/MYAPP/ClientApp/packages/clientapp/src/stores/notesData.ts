/**
 * Notes store (Pinia).
 */

import { defineStore } from 'pinia'

/**
 * Front-end representation of a note.
 */
type QNote = {
	id: string
	createdBy: string
	dest: string
	text: string
	date: Date
}

export const useNotesStore = defineStore('notesData', {
	state: (): { notes: QNote[] } => ({
		notes: []
	}),
	getters: {

	},
	actions: {
		/**
		 * Replaces the current list of notes.
		 * @param items Notes returned by the API.
		 */
		setNotes(items: ReadonlyArray<QNote>) {
			this.notes =  Array.isArray(items) ? [...items] : []
		},

		/**
		 * Clears all notes and restores the initial state.
		 */
		resetStore() {
			this.notes = []
		}
	}
})
