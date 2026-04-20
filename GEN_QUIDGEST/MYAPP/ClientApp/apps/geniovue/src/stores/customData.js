/*****************************************************************
 *                                                               *
 * This store holds custom data. It's main purpose is to allow   *
 * the usage of global properties through manual code.           *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'

//----------------------------------------------------------------
// State variables
//----------------------------------------------------------------

const state = () => {
	return {
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR STORE_INCLUDE_JS STATE]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
	}
}

//----------------------------------------------------------------
// Variable getters
//----------------------------------------------------------------

const getters = {
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR STORE_INCLUDE_JS GETTERS]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */
}

//----------------------------------------------------------------
// Actions
//----------------------------------------------------------------

const actions = {
/* eslint-disable indent, vue/html-indent, vue/script-indent */
// USE /[MANUAL FOR STORE_INCLUDE_JS ACTIONS]/
// eslint-disable-next-line
/* eslint-enable indent, vue/html-indent, vue/script-indent */

	/**
	 * Resets the custom data.
	 */
	resetStore()
	{
		Object.assign(this, state())
	}
}

//----------------------------------------------------------------
// Store export
//----------------------------------------------------------------

export const useCustomDataStore = defineStore('customData', {
	state,
	getters,
	actions
})
