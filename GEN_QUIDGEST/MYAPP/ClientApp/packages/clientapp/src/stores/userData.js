/*****************************************************************
 *                                                               *
 * This store holds data about the current user,                 *
 * also defining functions to access and mutate it.              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'
import _has from 'lodash-es/has'

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
		username: 'guest',

		valuesOfPHEs: {},

		/**
		 * The route to be opened after logging in, for cases when there was an attempt to
		 * navigate to a URL that requires authentication and login was completed afterward.
		 */
		routeAfterLogin: undefined
	}
}

//----------------------------------------------------------------
// Variable getters
//----------------------------------------------------------------

const getters = {
	/**
	 * True if a user is currently logged in, false otherwise.
	 * @param {object} state The current global state
	 */
	userIsLoggedIn(state) {
		return state.username !== '' && state.username !== 'guest'
	}
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
	/**
	 * Sets the current user's info, according to the data coming from the server.
	 * @param {object} data The current user's data
	 */
	setUserData(data) {
		if (typeof data !== 'object' || data === null) return

		if (_has(data, 'Name')) this.username = data.Name
		if (_has(data, 'Ephs')) this.valuesOfPHEs = data.Ephs
	},

	/**
	 * Adds a new entry to the PHE values.
	 * @param {object} data The key and value of the entry
	 */
	addPHEChoice(data) {
		if (typeof data !== 'object' || data === null) return
		if (typeof data.key !== 'string' || !Array.isArray(data.value) || data.value.length === 0)
			return

		this.valuesOfPHEs[data.key] = data.value
	},

	/**
	 * Method to save or remove the route that should be opened after successful authentication
	 * @param {object} routeData Route data
	 */
	setRedirectRouteAfterLogin(routeData) {
		this.routeAfterLogin = routeData
	},

	/**
	 * Clears the info of the current user.
	 */
	resetStore() {
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

export const useUserDataStore = defineStore('userData', {
	state,
	getters,
	actions
})
