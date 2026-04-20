import _isEmpty from 'lodash-es/isEmpty'

import { postData, fetchData } from '@quidgest/clientapp/network'

/*****************************************************************
 * This mixin defines methods to be reused by wizard components. *
 *****************************************************************/
export default {
	data()
	{
		return {
			wizardPath: [],

			wizardMode: '',

			selectedStepIndex: -1
		}
	},

	mounted()
	{
		this.selectedStepIndex = this.wizardData.stepData.order
	},

	computed: {
		/**
		 * The route of the current step.
		 */
		currentStepRoute()
		{
			return this.wizardPath[this.wizardPath.length - 1]
		},

		/**
		 * The data of the currently selected step.
		 */
		selectedStep()
		{
			const index = this.wizardData.stepData.order - 1
			return this.wizardData.stepList[index]
		},

		/**
		 * True if the selected step is the same as the current, false otherwise.
		 */
		isCurrentStep()
		{
			return this.currentStepIndex === this.selectedStepIndex
		},

		/**
		 * The index of the current step.
		 */
		currentStepIndex()
		{
			for (const stepData of this.wizardData.stepList)
				if (stepData.route === this.currentStepRoute)
					return stepData.order
			return -1
		}
	},

	methods: {
		/**
		 * Finds the route of the step previous to the specified step.
		 * @param {number} stepOrder The order of the step
		 * @returns The route of the previous step, or an empty string if the specified step is the first of the wizard path or not in it at all.
		 */
		getPreviousRoute(stepOrder)
		{
			const stepData = this.wizardData.stepList[stepOrder - 1]
			const stepIndex = this.wizardPath.indexOf(stepData.route)

			if (stepIndex < 1)
				return ''

			return this.wizardPath[stepIndex - 1]
		},

		/**
		 * Handles the change of step.
		 * @param {string} stepRoute The route of the next step
		 * @param {object} wizardPath The current path of the wizard (optional)
		 */
		handleStepChange(stepRoute, wizardPath)
		{
			if (typeof stepRoute !== 'string')
				return

			if (!this.wizardMode)
				this.wizardMode = this.navigation.currentLevel.params.mode

			let stepMode = this.wizardMode
			let currentStepRoute = this.currentStepRoute

			const recordId = this.navigation.currentLevel.params.id
			const wizardPathFromParams = this.navigation.currentLevel.params.wizardPath

			if (wizardPath && Array.isArray(wizardPath))
				currentStepRoute = wizardPath[wizardPath.length - 1]

			if (this.wizardData.disallowEdit && stepRoute !== currentStepRoute)
				stepMode = this.formModes.show

			// The levels of the wizard's phases are siblings and are not hierarchical levels.
			this.navigation.removeNavigationLevel()

			const params = {
				historyBranchId: this.navigation.navigationId,
				id: recordId,
				mode: stepMode,
				isControlled: true,
				isDuplicate: false,
				keepAlerts: true,
				wizardPath: wizardPathFromParams
			}

			// If an updated path is provided, that one is used instead.
			if (wizardPath && Array.isArray(wizardPath))
				params.wizardPath = wizardPath

			this.$router.push({ name: stepRoute, params })
		},

		/**
		 * Handles the click of a step in the wizard.
		 * @param {string} clickedStep The route of the step that was clicked
		 */
		async stepClicked(clickedStep)
		{
			const handler =  `${this.wizardData.wizardId}_GetPath`
			const params = { formId: this.primaryKeyValue }

			await fetchData(
				this.formArea,
				handler,
				params,
				(data, request) => {
					if (!request.data.Success || !Array.isArray(data.Path))
						return

					const path = data.Path
					this.applyChanges(false).then((success) => {
						if (path.includes(clickedStep) && success)
							this.handleStepChange(clickedStep, path)
						else
						{
							this.$eventTracker.addError({
								origin: 'stepClicked (wizardHandlers)',
								message: `Error while changing from step "${this.formInfo.name}" in wizard "${this.wizardData.wizardId}".`
							})
						}
					})
				})
		},

		/**
		 * Clears the values of the current wizard step from the DB.
		 * @param {function} callback A function to be called after the ajax request completes (optional)
		 */
		clearCurrentStep(callback)
		{
			const handler = `${this.wizardData.wizardId}_${this.formInfo.name}_ClearData`
			const params = { id: this.primaryKeyValue }

			postData(
				this.formArea,
				handler,
				params,
				(data) => {
					if (typeof callback === 'function')
						callback(data.Success === true)
				},
				undefined,
				undefined,
				this.navigationId)
		},

		/**
		 * Goes back to the previous step of the wizard.
		 */
		goToPreviousStep()
		{
			const saveData = this.wizardData.stepData.applyOnBackward
			const clearData = this.wizardData.stepData.clearOnBackward

			const goBack = (result) => {
				if (typeof result === 'object' && (result === null || !result.Success) ||
					typeof result === 'boolean' && !result)
				{
					this.$eventTracker.addError({
						origin: 'goToPreviousStep (wizardHandlers)',
						message: `Error while going backward from step "${this.formInfo.name}" in wizard "${this.wizardData.wizardId}".`
					})
				}

				const wizardPath = [...this.wizardPath]
				if (this.isEditable && this.selectedStep.route === this.currentStepRoute)
					wizardPath.pop()

				const previousRoute = this.getPreviousRoute(this.selectedStepIndex)

				this.handleStepChange(previousRoute, wizardPath)
			}

			if (clearData)
			{
				if ([this.formModes.new, this.formModes.duplicate].includes(this.formInfo.mode))
					this.clearCurrentStep(goBack)
				else
					goBack()
			}
			else if (saveData)
			{
				if (this.isEditable)
				{
					this.applyChanges(false).then((success) => {
						if (success)
							goBack()
					})
				}
				else
					goBack()
			}
			else
				goBack()
		},

		/**
		 * Advances to the next step of the wizard.
		 */
		goToNextStep()
		{
			const goForward = (result) => {
				if (typeof result === 'object' && (result === null || !result.Success))
					return

				const handler = `${this.wizardData.wizardId}_NextStep`
				const params = {
					formId: this.primaryKeyValue,
					currentStep: this.wizardData.stepData.id
				}

				postData(
					this.formArea,
					handler,
					params,
					(data, request) => {
						if (!data || typeof data.Route !== 'string')
						{
							this.$eventTracker.addError({
								origin: 'goToNextStep (wizardHandlers)',
								message: `Error while going forward from step "${this.formInfo.name}" in wizard "${this.wizardData.wizardId}".`
							})

							this.validationErrors = !_isEmpty(request.data.Message)
								? this.parseResponseErrors({ ['']: request.data.Message })
								: {}

							return
						}

						const wizardPath = [...this.wizardPath]
						if (this.isEditable && this.selectedStep.route === this.currentStepRoute)
							wizardPath.push(data.Route)

						this.handleStepChange(data.Route, wizardPath)
					},
					undefined,
					undefined,
					this.navigationId)
			}

			if (this.isEditable)
			{
				this.applyChanges(false).then((success) => {
					if (success)
						goForward()
				})
			}
			else
				goForward()
		}
	}
}
