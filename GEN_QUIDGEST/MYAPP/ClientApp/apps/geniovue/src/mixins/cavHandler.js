import { mapState, mapActions } from 'pinia'

import { useGenericDataStore } from '@quidgest/clientapp/stores'

export default {
	data()
	{
		return {
			mainContainerIsVisible: true
		}
	},

	mounted()
	{
		this.$eventHub.on('toggle-reporting-mode', this.reportingModeToggle)
		this.$eventHub.on('main-container-is-visible', this.fnMainContainerIsVisible)
	},

	beforeUnmount()
	{
		this.$eventHub.off('toggle-reporting-mode', this.reportingModeToggle)
		this.$eventHub.off('main-container-is-visible', this.fnMainContainerIsVisible)
	},

	computed: {
		...mapState(useGenericDataStore, [
			'reportingModeCAV'
		])
	},

	methods: {
		...mapActions(useGenericDataStore, [
			'setReportingModeCAV'
		]),

		reportingModeToggle()
		{
			this.setReportingModeCAV(!this.reportingModeCAV)
		},

		fnMainContainerIsVisible(isVisible)
		{
			this.mainContainerIsVisible = isVisible
		}
	}
}
