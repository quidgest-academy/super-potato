import { mapState, mapActions } from 'pinia'

import { useAuthDataStore } from '@quidgest/clientapp/stores'
import { useUserDataStore } from '@quidgest/clientapp/stores'

/***************************************************************************
 * This mixin defines methods to be reused in authentication components.   *
 ***************************************************************************/
export default {
	computed: {
		...mapState(useAuthDataStore, [
			'hasPasswordRecovery',
			'hasUsernameAuth',
			'hasUserSettings'
		]),

		userData()
		{
			const userDataStore = useUserDataStore()

			return {
				name: userDataStore.username
			}
		}
	},

	methods: {
		...mapActions(useUserDataStore, [
			'setUserData'
		]),

		...mapActions(useAuthDataStore, [
			'setPasswordRecovery',
			'setUsernameAuth',
			'setOpenIdAuth',
			'set2FAOptions'
		]),

		authRedirectButtonClick(data)
		{
			window.location.href = data.Redirect
		}
	}
}
