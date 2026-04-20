<template>
	<teleport
		v-if="isReady"
		to="#q-modal-form-suggestion-index-body">
		<div
			v-if="isEmpty(model.FieldId)"
			class="content">
			<base-input-structure
				id="suggestion-text"
				class="i-textarea"
				:label-attrs="{ class: 'i-text__label' }"
				:label="texts.suggestionText">
				<q-text-area
					id="suggestion-text"
					v-model="model.SuggestionText"
					:rows="10"
					:cols="20" />
			</base-input-structure>
		</div>
		<template v-else-if="model.Elements.length > 0">
			<ul class="nav c-tab c-tab__divider">
				<li
					class="nav-item c-tab__item tl_li"
					data-tab="field">
					<a
						data-target="#field"
						class="nav-link active"
						role="tab"
						data-toggle="tab">
						{{ texts.field }}
					</a>
				</li>

				<li
					class="nav-item c-tab__item"
					data-tab="array">
					<a
						data-target="#array"
						data-toggle="tab"
						role="tab"
						class="nav-link">
						{{ texts.enumeration }}
					</a>
				</li>
			</ul>

			<div class="c-tab__item-container">
				<div
					id="field-suggestion"
					class="tab-pane c-tab__item-content active">
					<q-text-field
						v-model="model.NewLabel"
						:label="texts.fieldName" />

					<base-input-structure
						id="new-help"
						class="i-textarea"
						:label-attrs="{ class: 'i-text__label' }"
						:label="texts.helpText">
						<q-text-area
							id="new-help"
							v-model="model.NewHelp"
							:rows="10"
							:cols="20" />
					</base-input-structure>
					<p>{{ texts.fillSuggestion }}</p>
				</div>

				<div
					id="array-suggestion"
					class="tab-pane c-tab__item-content">
					<div class="container">
						<div class="row">
							<ul class="nav c-tab nav-pills flex-column col-3 nav-scrollable">
								<li
									v-for="(item, index) in model.Elements"
									:key="item.Id"
									class="nav-item c-tab__item"
									:data-tab="'el_' + item.Id">
									<a
										v-if="index === 0"
										:data-target="`#el_${item.Id}`"
										class="nav-link active"
										role="tab"
										data-toggle="tab">
										{{ item.NewLabel }}
									</a>
									<a
										v-else
										:data-target="`#el_${item.Id}`"
										class="nav-link"
										role="tab"
										data-toggle="tab">
										{{ item.NewLabel }}
									</a>
								</li>
							</ul>

							<div class="c-tab__item-container col-9">
								<template
									v-for="(element, index) in model.Elements"
									:key="element.Id">
									<div
										:class="[{ active: index === 0 }, 'tab-pane', 'c-tab__item-content']"
										:id="`el_${element.Id}`">
										<q-text-field
											v-model="model.Elements[index].NewLabel"
											:label="texts.elementDescription" />

										<base-input-structure
											:id="`new-help-${element.Id}`"
											:class="[{ 'no-resize': index !== 0 }, 'i-textarea']"
											:label-attrs="{ class: 'i-text__label' }"
											:label="texts.elementHelp">
											<q-text-area
												:id="`new-help-${element.Id}`"
												v-model="model.Elements[index].NewHelp"
												:rows="10"
												:cols="20" />
										</base-input-structure>
									</div>
								</template>
								<p>{{ texts.fillSuggestion }}</p>
							</div>
						</div>
					</div>
				</div>
			</div>
		</template>
		<div
			v-else
			class="content">
			<q-text-field
				v-model="model.NewLabel"
				:label="texts.fieldName" />

			<base-input-structure
				id="new-help"
				class="i-textarea"
				:label-attrs="{ class: 'i-text__label' }"
				:label="texts.helpText">
				<q-text-area
					id="new-help"
					v-model="model.NewHelp"
					:rows="10"
					:cols="20" />
			</base-input-structure>
			<p>{{ texts.fillSuggestion }}</p>
		</div>
	</teleport>

	<teleport
		v-if="isReady"
		to="#q-modal-form-suggestion-index-footer">
		<div class="actions">
			<q-button
				variant="bold"
				:label="texts.suggest"
				:title="texts.suggest"
				@click="saveSuggestion">
				<q-icon icon="save" />
			</q-button>

			<q-button
				:label="texts.cancel"
				:title="texts.cancel"
				@click="closePopup">
				<q-icon icon="cancel" />
			</q-button>
		</div>
	</teleport>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'
	import _merge from 'lodash-es/merge'
	import { mapActions } from 'pinia'
	import { computed } from 'vue'

	import { postData } from '@quidgest/clientapp/network'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import { displayMessage } from '@quidgest/clientapp/utils/genericFunctions'
	import { useGenericDataStore } from '@quidgest/clientapp/stores'
	import { removeModal } from '@/utils/layout'

	export default {
		name: 'QSuggestionIndex',

		props: {
			/**
			 * The parameters necessary for the suggestion component to function, including IDs and text for fields.
			 */
			params: {
				type: Object,
				default: () => ({})
			}
		},

		expose: [],

		data()
		{
			return {
				id: this.params.id,

				label: this.params.label,

				help: this.params.help,

				arrayName: this.params.arrayName,

				model: {
					FieldId: '',
					SuggestionText: '',
					Elements: []
				},

				texts: {
					newSuggestion: computed(() => this.Resources[hardcodedTexts.newSuggestion]),
					suggestionText: computed(() => this.Resources[hardcodedTexts.suggestionText]),
					field: computed(() => this.Resources[hardcodedTexts.field]),
					enumeration: computed(() => this.Resources[hardcodedTexts.enumeration]),
					fieldName: computed(() => this.Resources[hardcodedTexts.fieldName]),
					helpText: computed(() => this.Resources[hardcodedTexts.helpText]),
					elementHelp: computed(() => this.Resources[hardcodedTexts.elementHelp]),
					fillSuggestion: computed(() => this.Resources[hardcodedTexts.fillSuggestion]),
					elementDescription: computed(() => this.Resources[hardcodedTexts.elementDescription]),
					suggest: computed(() => this.Resources[hardcodedTexts.suggest]),
					cancel: computed(() => this.Resources[hardcodedTexts.cancel])
				},

				isReady: false
			}
		},

		mounted()
		{
			const props = {
				title: this.texts.newSuggestion,
				dismissible: true
			}

			const modalProps = {
				id: 'form-suggestion-index',
				isActive: true
			}

			this.setModal(props, modalProps)
			this.fetchData()
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'setModal'
			]),

			removeModal,

			isEmpty: _isEmpty,

			/**
			 * Fetches the data required for displaying the suggestion form by making two API calls.
			 */
			fetchData()
			{
				if (_isEmpty(this.params))
					return

				const params = {
					id: this.params.id,
					label: this.params.label,
					help: this.params.help,
					arrayName: this.params.arrayName
				}

				postData('Suggestion', 'List', null, (data) => {
					if (!_isEmpty(data.Suggestions))
					{
						const textValue = data.Suggestions[0].NewValue
						if (textValue)
							this.model.SuggestionText = textValue
					}
				})

				postData('Suggestion', 'Index', params, (data) => {
					_merge(this.model, data)
					this.isReady = true
				})
			},

			/**
			 * Saves the suggestion to the server using FormData and shows a message on success or failure.
			 */
			saveSuggestion()
			{
				const params = {
					SuggestionText: this.model.SuggestionText,
					NewLabel: this.model.NewLabel,
					NewHelp: this.model.NewHelp,
					FieldId: this.model.FieldId,
					ArrayName: this.arrayName,
					Elements: this.model.Elements
				}

				postData('Suggestion', 'Save', params, (data) => {
					if (data.Success)
						displayMessage(data.Message)
					this.removeModal('form-suggestion-index')
				})
			},

			/**
			 * Closes the suggestion popup modal.
			 */
			closePopup()
			{
				this.removeModal('form-suggestion-index')
			}
		}
	}
</script>
