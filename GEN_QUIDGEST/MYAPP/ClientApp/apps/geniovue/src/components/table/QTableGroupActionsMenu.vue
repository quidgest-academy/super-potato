<template>
	<q-action-list
		v-if="numVisibleActions > 1"
		data-testid="options-btn"
		placement="bottom-start"
		variant="bold"
		:disabled="rowsSelectedCount === 0"
		:groups="groupActionGroups"
		:items="groupActionItems"
		:label="texts.groupActionsText"
		:title="texts.groupActionsText"
		@click="groupActionOption">
		<q-icon icon="checkbox-multiple-marked" />
	</q-action-list>
	<q-button
		v-else-if="followUpAction"
		data-testid="options-btn"
		variant="bold"
		:label="followUpAction.title"
		:title="followUpAction.title"
		:disabled="rowsSelectedCount === 0"
		@click="groupAction(followUpAction)">
		<q-icon
			v-if="followUpAction.icon"
			v-bind="followUpAction.icon" />
	</q-button>
</template>

<script>
	import { numArrayVisibleActions } from '@/mixins/listFunctions.js'
	import { QActionList } from '@quidgest/clientapp/components'

	export default {
		name: 'QTableGroupActionsMenu',

		emits: ['group-action'],

		components: { QActionList },

		props: {
			/**
			 * The number of rows currently selected in the table, used to determine whether to enable or disable group actions.
			 */
			rowsSelectedCount: {
				type: Number,
				default: 0
			},

			/**
			 * An array of actions that can be applied to the group of selected rows.
			 */
			groupActions: {
				type: Array,
				default: () => []
			},

			/**
			 * Specifies the placement of the group action button relative to the table rows, which can be 'left' or 'right'.
			 */
			actionsPlacement: {
				type: String,
				default: 'left'
			},

			/**
			 * Localized text strings to be used in the group actions menu for labels, titles, and accessibility.
			 */
			texts: {
				type: Object,
				required: true
			}
		},

		expose: [],

		data()
		{
			return {
				groupActionGroups: [
					{
						id: 'default',
						display: 'dropdown'
					}
				]
			}
		},

		computed: {
			/**
			 * Determine total number of actions that are visible
			 */
			numVisibleActions()
			{
				return numArrayVisibleActions(this.groupActions, false)
			},

			/**
			 * Create group action
			 */
			followUpAction()
			{
				return this.numVisibleActions === 1 ? this.groupActions[0] : null
			},

			/**
			 * The list of possible actions.
			 */
			groupActionItems()
			{
				return this.groupActions.map((act) => ({
					...act,
					key: act.id,
					label: act.title,
					group: 'default'
				}))
			}
		},

		methods: {
			/**
			 * Emit data for executing action
			 * @param {object} action
			 */
			groupAction(action)
			{
				this.$emit('group-action', { action })
			},

			/**
			 * Emit data for executing action
			 * @param {string} optionKey The identifier of the selected action
			 */
			groupActionOption(optionKey)
			{
				const action = this.groupActionItems.find((e) => e.key === optionKey)
				this.groupAction(action)
			}
		}
	}
</script>
