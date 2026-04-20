<template>
	<tr
		:id="controlId"
		:data-key="controlId"
		:class="rowClass">
		<td class="grid-table-row__state">
			<div class="grid-table-row__state-icon">
				<q-icon
					v-if="rowStateIcon"
					:icon="rowStateIcon" />

				<q-button
					v-if="hasMessages && !showMessages"
					variant="text"
					@click="toggleErrors">
					<q-icon :icon="expandIcon" />
				</q-button>
			</div>

			<template v-if="hasMessages && !showMessages">
				<q-badge :color="badgeColor">
					{{ numMessages }}
				</q-badge>
				<span class="grid-table-row__messages">
					{{ texts.messages }}
				</span>
			</template>
		</td>

		<td class="grid-table-row__action">
			<q-button-group>
				<slot
					v-if="showCustomActions"
					name="actions.prepend" />

				<slot name="actions">
					<q-button
						v-if="showDeleteBtn"
						data-testid="delete"
						variant="text"
						:title="texts.delete"
						@click="markForDeletion">
						<q-icon icon="delete" />
					</q-button>

					<q-button
						v-if="showRemoveBtn"
						data-testid="delete"
						variant="text"
						:title="texts.remove"
						@click="markForDeletion">
						<q-icon icon="remove-sign" />
					</q-button>

					<q-button
						v-if="showUndoBtn"
						data-testid="undo"
						variant="text"
						:title="texts.restore"
						@click="undoMarkForDeletion">
						<q-icon icon="undo" />
					</q-button>
				</slot>

				<slot
					v-if="showCustomActions"
					name="actions.append" />
			</q-button-group>
		</td>

		<slot />
	</tr>
</template>

<script>
	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		messages: 'Messages',
		delete: 'Delete',
		remove: 'Remove',
		restore: 'Restore'
	}

	export default {
		name: 'QGridTableRow',

		emits: {
			'mark-for-deletion': () => true,
			'undo-deletion': () => true,
			'toggle-errors': () => true
		},

		props: {
			/**
			 * The id of the row.
			 */
			id: String,

			/**
			 * Whether the error/warning messages should always be visible.
			 */
			showMessages: {
				type: Boolean,
				default: false
			},

			/**
			 * The initial state of the editable table list row.
			 */
			initialState: {
				type: String,
				default: ''
			},

			/**
			 * Deleted state mode.
			 * This is necessary because when you navigate to a different form and come back, we need to know if the row was deleted.
			 * If it was, the state is "Deleted" and the undo button appears.
			 */
			isDeletedState: {
				type: Boolean,
				default: false
			},

			/**
			 * The current mode of the form.
			 */
			mode: {
				type: String,
				default: 'EDIT'
			},

			/**
			 * The model of the form.
			 */
			nestedModel: {
				type: Object,
				default: () => ({})
			},

			/**
			 * The permissions of the form.
			 */
			permissions: {
				type: Object,
				default: () => ({})
			},

			/**
			 * Localization and customization of textual content within the component.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			}
		},

		expose: [],

		data()
		{
			return {
				expandIcon: 'expand',
				expandSVG: 'expand',
				collapseSVG: 'collapse'
			}
		},

		computed: {
			/**
			 * The unique identifier of the component.
			 */
			controlId()
			{
				return this.id || `q-grid-table-row-${this._.uid}`
			},

			/**
			 * The state of the row.
			 */
			state()
			{
				if (this.isDeletedState)
					return 'DELETED'
				else if (this.nestedModel.serverErrorMessages?.length > 0)
					return 'ERRORS'
				else if (this.nestedModel.serverWarningMessages?.length > 0)
					return 'WARNINGS'
				else if (this.nestedModel.serverInfoMessages?.length > 0)
					return 'INFO'
				else if (this.initialState === 'NEW')
					return this.nestedModel.isDirty ? 'NEW' : 'NEW--EMPTY'
				return this.nestedModel.isDirty ? 'EDITED' : ''
			},

			/**
			 * The class associated to the state of the row.
			 */
			rowClass()
			{
				return `grid-table-row${this.state !== '' ? '__' + this.state.toLowerCase() : ''}`
			},

			/**
			 * The icon associated to the state of the row.
			 */
			rowStateIcon()
			{
				switch (this.state)
				{
					case 'NEW':
						return 'add-outline'
					case 'NEW--EMPTY':
						return 'add'
					case 'EDITED':
						return 'pencil'
					case 'ERRORS':
					case 'WARNINGS':
					case 'INFO':
						return 'exclamation-sign'
					case 'DELETED':
						return 'delete'
					default:
						return ''
				}
			},

			/**
			 * The badge style to be applied based on the messages shown.
			 */
			badgeColor()
			{
				const colorMap = {
					'ERRORS': 'danger',
					'WARNINGS': 'warning'
				}

				return colorMap[this.state] ?? 'info'
			},

			/**
			 * Whether to show the "delete" button or not.
			 * This button should be visible for all pre-existing rows.
			 */
			showDeleteBtn()
			{
				return this.mode === 'EDIT'
					&& this.permissions.canDelete
					&& this.initialState === ''
			},

			/**
			 * Whether to show the "remove" button or not.
			 * This button should be visible for new rows.
			 */
			showRemoveBtn()
			{
				return this.initialState === 'NEW' && this.nestedModel.isDirty
			},

			/**
			 * Whether to show custom actions on the row.
			 * They should only be visible for actual rows, not the placeholder one at the end.
			 */
			showCustomActions()
			{
				return this.initialState !== 'NEW' || this.nestedModel.isDirty
			},

			/**
			 * Whether to show the "undo" button or not.
			 * This button should be visible for rows that are
			 * marked to be deleted.
			 */
			showUndoBtn()
			{
				return this.state === 'DELETED'
			},

			/**
			 * Indicates the number of messages.
			 */
			numMessages()
			{
				const allMessages = []
				return allMessages.concat(
					this.nestedModel?.serverErrorMessages ?? [],
					this.nestedModel?.serverWarningMessages ?? [],
					this.nestedModel?.serverInfoMessages ?? []).length
			},

			/**
			 * Indicates if there are any messages to show on the row.
			 */
			hasMessages()
			{
				return this.numMessages > 0
			}
		},

		methods: {
			/**
			 * Marks this row for deletion when the main form is saved.
			 */
			markForDeletion()
			{
				this.$emit('mark-for-deletion')
			},

			/**
			 * Undoes the "Mark for deletion" action.
			 */
			undoMarkForDeletion()
			{
				this.$emit('undo-deletion')
			},

			/**
			 * Show the list of errors, warnings and information about this model.
			 */
			toggleErrors()
			{
				this.expandIcon = this.expandIcon === this.expandSVG ? this.collapseSVG : this.expandSVG
				this.$emit('toggle-errors')
			}
		}
	}
</script>
