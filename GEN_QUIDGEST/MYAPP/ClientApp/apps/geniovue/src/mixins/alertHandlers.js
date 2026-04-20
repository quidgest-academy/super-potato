import { mapActions, mapState } from 'pinia'

import netAPI from '@quidgest/clientapp/network'
import VueNavigation from '@/mixins/vueNavigation.js'
import { useGenericDataStore } from '@quidgest/clientapp/stores'
import { useSystemDataStore } from '@quidgest/clientapp/stores'
import { useUserDataStore } from '@quidgest/clientapp/stores'
import { hydrateAlert } from './alertFunctions.js'

export default {
	mixins: [
		VueNavigation
	],

	computed: {
		...mapState(useSystemDataStore, [
			'system'
		]),

		...mapState(useUserDataStore, [
			'userIsLoggedIn',
			'valuesOfPHEs'
		])
	},

	methods: {
		...mapActions(useGenericDataStore, [
			'setNotifications'
		]),

		fetchAlerts()
		{
			if (!this.userIsLoggedIn)
				return Promise.resolve(true)

			return netAPI.postData(
				'Alerts',
				'Index',
				null,
				(data) => {
					if (data && Array.isArray(data))
					{
						const alerts = []

						data.forEach((alertData) => {
							const alert = hydrateAlert(alertData, true)

							if (alert)
								alerts.push(alert)
						})

						this.setNotifications(alerts)
					}
					else
						this.setNotifications([])
				},
				undefined,
				undefined,
				this.navigationId)
		},

		onAlertClick(target)
		{
			if (target.Type === 'menu')
				this.navigateToRouteName(`menu-${target.Name}`, {})
			else if (target.Type === 'form')
				this.navigateToForm(target.Name, 'SHOW', target.Id, {})
		}
	},

	watch: {
		'system.currentModule'()
		{
			this.fetchAlerts()
		},

		valuesOfPHEs: {
			handler()
			{
				this.fetchAlerts()
			},
			deep: true
		}
	}
}
