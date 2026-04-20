<template>
	<q-button
		borderless
		:title="texts.saveQuery"
		:disabled="inMaintenance"
		@click="showDialog">
		<q-icon icon="save" />
	</q-button>

	<teleport
		v-if="isDialogVisible"
		:to="`#q-modal-${modalId}-body`">
		<q-row-container>
			<q-control-wrapper class="control-join-group">
				<base-input-structure
					id="cav-save-query-name"
					:class="['i-text', { 'i-text--disabled': false }]"
					:label="texts.queryName"
					:label-attrs="{ class: 'i-text__label' }">
					<q-text-field
						id="cav-save-query-name"
						size="small"
						v-model="queryName"
						:max-length="15" />
				</base-input-structure>
			</q-control-wrapper>
		</q-row-container>

		<q-row-container>
			<q-control-wrapper class="control-join-group">
				<base-input-structure
					id="cav-save-query-access"
					:class="['i-text', { 'i-text--disabled': false }]"
					:label="texts.queryAccess"
					:label-attrs="{ class: 'i-text__label' }">
					<q-radio-group
						id="cav-save-query-access"
						v-model="accessType">
						<q-radio-button
							v-for="radio in accessTypes"
							:key="radio.key"
							:value="radio.key"
							:label="radio.value" />
					</q-radio-group>
				</base-input-structure>
			</q-control-wrapper>
		</q-row-container>

		<q-row-container v-if="overrideVisible">
			<q-control-wrapper class="control-join-group">
				<q-checkbox
					id="cav-save-query-is-override"
					v-model="isOverride"
					:label="texts.overlapQuery" />
			</q-control-wrapper>
		</q-row-container>
	</teleport>
</template>

<script>
	import { computed, nextTick } from 'vue'

	import { useGenericDataStore } from '@quidgest/clientapp/stores'
	import { removeModal } from '@/utils/layout'
	import hardcodedTexts from '@/hardcodedTexts.js'
	import cavArrays from '@/api/genio/cavArrays.js'

	export default {
		name: 'QCavSaveQuery',

		emits: ['save-query'],

		props: {
			/**
			 * Holds the unique query identifier.
			 */
			queryId: {
				type: String,
				default: 'new'
			},

			/**
			 * The title or name to be given to the query when saved.
			 */
			title: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data()
		{
			return {
				accessType: 'PUB',

				queryName: this.title || '',

				isOverride: false,

				accessTypes: cavArrays.accessType.setResources(this.$getResource).elements,

				texts: {
					queryName: computed(() => this.Resources[hardcodedTexts.queryName]),
					queryAccess: computed(() => this.Resources[hardcodedTexts.queryAccess]),
					overlapQuery: computed(() => this.Resources[hardcodedTexts.overlapQuery]),
					close: computed(() => this.Resources[hardcodedTexts.close]),
					save: computed(() => this.Resources[hardcodedTexts.save]),
					saveQuery: computed(() => this.Resources[hardcodedTexts.saveQuery])
				},

				modalId: 'cav-save',

				isDialogVisible: false
			}
		},

		computed: {
			/**
			 * Computes the currentQueryId, defaulting to 'new' if the 'queryId' prop is not provided.
			 */
			currentQueryId()
			{
				return this.queryId || 'new'
			},

			/**
			 * Determines whether the override option should be visible based on currentQueryId.
			 * If the queryId is 'new', the option to override is hidden, else it's shown.
			 */
			overrideVisible()
			{
				return this.currentQueryId !== 'new'
			},

			/**
			 * Whether the system is currently in maintenance.
			 */
			inMaintenance()
			{
				const genericDataStore = useGenericDataStore()
				return genericDataStore.maintenance.isActive
			}
		},

		methods: {
			/**
			 * Opens the modal dialog.
			 */
			async showDialog()
			{
				const props = {
					title: this.texts.saveQuery,
					class: 'q-dialog-form',
					size: 'small',
					buttons: [
						{
							id: 'dialog-button-save',
							action: this.saveQuery,
							icon: { icon: 'save' },
							props: {
								label: this.texts.save,
								title: this.texts.save,
								variant: 'bold'
							}
						},
						{
							id: 'dialog-button-close',
							action: this.hideDialog,
							icon: { icon: 'close' },
							props: {
								label: this.texts.close,
								title: this.texts.close
							}
						}
					]
				}
				const modalProps = {
					id: this.modalId,
					isActive: true,
					dismissAction: this.hideDialog
				}

				const genericDataStore = useGenericDataStore()
				genericDataStore.setModal(props, modalProps)

				await nextTick()
				this.isDialogVisible = true
			},

			/**
			 * Closes the modal dialog.
			 */
			hideDialog()
			{
				removeModal(this.modalId)
				this.isDialogVisible = false
			},

			/**
			 * Emits a 'save-query' event with the query data when the save button is clicked.
			 */
			saveQuery()
			{
				const data = {
					id: this.currentQueryId,
					title: this.queryName,
					queryOverride: this.isOverride,
					accessType: this.accessType
				}

				this.$emit('save-query', data)
			}
		}
	}
</script>
