/*****************************************************************
 *                                                               *
 * This store holds authentication data,                         *
 * also defining functions to access and mutate it.              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'

const state = () => ({
	hasPasswordRecovery: false,

	// It's preferable that it stays hidden and then appears rather than the opposite.
	hasUsernameAuth: false,

	hasOpenIdAuth: false,

	has2FAOptions: false
})

export const useAuthDataStore = defineStore('authData', {
	state: () => state(),
	getters: {
		/**
		 * True if user has any settings, false otherwise.
		 * @param state The current global state
		 */
		hasUserSettings(state) {
			return state.hasUsernameAuth || state.hasOpenIdAuth || state.has2FAOptions
		}
	},
	actions: {
		/**
		 * Sets whether the password recovery page is available.
		 * @param hasPasswordRecovery Whether or not the password recovery page is available
		 */
		setPasswordRecovery(hasPasswordRecovery: boolean) {
			if (typeof hasPasswordRecovery !== 'boolean') return

			this.hasPasswordRecovery = hasPasswordRecovery
		},

		/**
		 * Sets whether the username and password authentification is available.
		 * @param hasUsernameAuth Whether the username and password authentification is available
		 */
		setUsernameAuth(hasUsernameAuth: boolean) {
			if (typeof hasUsernameAuth !== 'boolean') return

			this.hasUsernameAuth = hasUsernameAuth
		},

		/**
		 * Sets whether OpenID authentication is available.
		 * @param hasOpenIdAuth Whether OpenID authentication is available
		 */
		setOpenIdAuth(hasOpenIdAuth: boolean) {
			if (typeof hasOpenIdAuth !== 'boolean') return

			this.hasOpenIdAuth = hasOpenIdAuth
		},

		/**
		 * Sets the availability of 2FA options.
		 * @param has2FAOptions Whether 2FA options are available
		 */
		set2FAOptions(has2FAOptions: boolean) {
			if (typeof has2FAOptions !== 'boolean') return

			this.has2FAOptions = has2FAOptions
		},

		/**
		 * Resets the auth data.
		 */
		resetStore() {
			Object.assign(this, state())
		}
	}
})
