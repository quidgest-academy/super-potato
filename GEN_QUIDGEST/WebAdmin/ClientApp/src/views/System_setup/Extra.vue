<template>
    <div id="system_setup_extra_container">
        <row>
			<q-card
				width="block"
				class="q-card--admin-default">
				<q-row-container>
					<row>
						<qtable :rows="advancedProps"
								:columns="tAdvP.columns"
								:config="tAdvP.config"
								:totalRows="tAdvP.total_rows"
								class="q-table--borderless">
							<template #actions="props">
								<q-button-group borderless>
									<q-button
										variant="text"
										:title="hardcodedTexts.edit"
										@click="changeAdvancedProperty(props.row)">
										<q-icon icon="pencil" />
									</q-button>
									<q-button
										variant="text"
										:title="hardcodedTexts.delete"
										@click="deleteAdvancedProperty(props.row)">
									<q-icon icon="bin" />
									</q-button>
								</q-button-group>
							</template>
							<template #table-footer>
								<tr>
									<td colspan="3">
									<q-button
										:label="hardcodedTexts.insert"
										@click="createAdvancedProperty">
										<q-icon icon="add" />
									</q-button>
									</td>
								</tr>
							</template>
						</qtable>
					</row>
				</q-row-container>
			</q-card>
		</row>

		<q-dialog
			v-model="showDialogAdvanced"
			:title="resources.property"
			:buttons="buttonsAdvanced">
			<template #body.content>
				<div class="q-dialog-container">
					<div v-if="hasInitProperties && !showNewKeyInput">
						<div id="help_container"  v-if="help">
							<div class="q-help__info-banner">
								<div class="q-help__info-banner-header">
									<q-icon icon="information-outline" />
								</div>
								<div class="q-help__info-banner-body">
									<span style="white-space: pre-line">
										{{ help }}<br>
									</span>
								</div>
							</div>
							<br />
						</div>
						<q-button v-if="!inDeleteModeAdvanced"
							@click="showNewKeyInput=true"
							:label="resources.insertNewKey">
								<q-icon icon="pencil" />
						</q-button>
						<q-select
							v-model="rowKey"
							v-if="SelectLists"
							:label="hardcodedTexts.key"
							:items="SelectLists.PropertyList"
							size="large"
							:readonly="inDeleteModeAdvanced"
							item-value="Value"
							item-label="translatedLabel">
						</q-select>
					</div>
					<div v-else>
						<q-button
							@click="showNewKeyInput=false"
							:label="resources.listDefaultKeys"
							v-if="hasInitProperties && !inDeleteModeAdvanced">
								<q-icon icon="list" />
						</q-button>
						<q-text-field
							v-model="rowKey"
							:class="{ 'input-error' : isSameKey }"
							:label="hardcodedTexts.key"
							:readonly="inDeleteModeAdvanced"
							required
							size="large">
							<template #extras v-if="isSameKey">
								<q-icon icon="information-outline" />
								{{ hardcodedTexts.thisKeyAlreadyExists }}
							</template>
						</q-text-field>
					</div>
					<div>
						<component
							:is="valueComponent"
							v-model="rowValue"
							:label="hardcodedTexts.value"
							:readonly="inDeleteModeAdvanced"
							:isReadOnly="inDeleteModeAdvanced"
							required
							size="large" />
					</div>
				</div>
			</template>
		</q-dialog>
    </div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import QAlert from '@/components/QAlert.vue';
	import numeric_input from '@/components/Numeric_input.vue';
	import { texts } from '@/resources/hardcodedTexts.ts';
	import { QTextField, QCheckbox, QPasswordField} from '@quidgest/ui/components';
	import { computed } from 'vue';

	export default {
		name: 'extra',

		components: {
			QAlert,
			QTextField,
			QCheckbox,
			QPasswordField
		},

		mixins: [reusableMixin],

		emits: ['update-model', 'alert-class'],

		props: {
			model: {
				required: true
			},
			SelectLists: {
				required: true
			},
			resources: {
				type: Object,
				required: true
			}
		},

		data() {
			return {
				showDialogAdvanced: false,
				buttonsAdvanced: [],
				advancedProps: [],
				rowKey: '',
				rowValue: '',
				dialogModeAdvanced: '',
				alert: {
					isVisible: false,
					alertType: 'info',
					message: ''
				},
				tAdvP: {
					rows: [],
					columns: [{
						label: this.resources.actions,
						name: "actions",
						slot_name: "actions",
						sort: false,
						column_classes: "thead-actions",
						row_text_alignment: 'text-center',
						column_text_alignment: 'text-center'
					},
					{
						label: computed(() => this.Resources[texts.key]),
						name: "Key",
						sort: true,
						initial_sort: true,
						initial_sort_order: "asc"
					},
					{
						label: computed(() => this.Resources[texts.value]),
						name: "Val",
						sort: true
					}],
					config: {
						table_title: this.resources.advancedProperties
					}
				},
				valueComponent: QTextField,
				showNewKeyInput : false,
				help:'',
			};
		},

		computed: {
			inEditModeAdvanced() {
				return this.dialogModeAdvanced === 'edit';
			},
			inDeleteModeAdvanced() {
				return this.dialogModeAdvanced === 'delete';
			},
			hasInitProperties() {
				return this.SelectLists.PropertyList.length > 0;
			},
			isSameKey() {
				return this.advancedProps.some(prop => prop.Key.toLowerCase() === this.rowKey.toLowerCase()) && this.dialogModeAdvanced === 'new'
			},
			invalidProps() {
				return this.rowKey === '' || this.rowValue === '' || (this.dialogModeAdvanced === 'new' && this.isSameKey)
			},
			hardcodedTexts() {
				return {
					insert: this.Resources[texts.insert],
					delete: this.Resources[texts.delete],
					edit: this.Resources[texts.edit],
					key: this.Resources[texts.key],
					value: this.Resources[texts.value],
					thisKeyAlreadyExists: this.Resources[texts.thisKeyAlreadyExists],
					valueCannotBeEmpty: this.Resources[texts.valueCannotBeEmpty],
					languageLabel: this.Resources[texts.languageLabel],
					erase: this.Resources[texts.erase],
					save: this.Resources[texts.save],
					cancel: this.Resources[texts.cancel],
				};
			},
		},

		methods: {
			getButtonsDialogAdvanced() {
				switch(this.dialogModeAdvanced) {
					case 'delete':
						this.buttonsAdvanced.push({
							id: 'delete-btn',
							props: {
								label: this.hardcodedTexts.erase,
								variant: 'bold',
								color: "danger"
							},
							action: () => {
								this.SaveMoreProperty()
							}
						});
						break;
					case 'edit':
					case 'new':
						this.buttonsAdvanced.push({
							id: 'save-btn',
							props: {
								label: this.hardcodedTexts.save,
								variant: 'bold',
								disabled: this.invalidProps
							},
							action: () => {
								this.SaveMoreProperty()
							}
						});
						break;
					default:
						break;
				}

				this.buttonsAdvanced.push({
					id: 'cancel-btn',
					props: {
						label: this.hardcodedTexts.cancel
					},
					action: () => this.clearMorePropertyValues()
				})
			},
			SaveMoreProperty() {
				const propsValues = {
					Key: this.rowKey,
					Val: this.rowValue.toString(),
					FormMode: this.dialogModeAdvanced,
				}
				QUtils.postData('Config', 'SaveMoreProperty', propsValues, { appId: this.$store.state.currentApp }, (data) => {
					if (data.emptyVal) { this.$emit('alert-class', { ResultMsg: this.hardcodedTexts.valueCannotBeEmpty, AlertType: 'danger' }); }
					else if (!data.success) { this.$emit('alert-class', { ResultMsg: this.hardcodedTexts.thisKeyAlreadyExists, AlertType: 'danger' }); }
					else {
						switch (propsValues.FormMode) {
						case 'new':
							this.advancedProps.push(
							{
								Key: this.rowKey,
								Val: this.rowValue,
								FormMode: this.dialogModeAdvanced
							}
						)
						break;
						case 'edit':
							const newPropIndex = this.advancedProps.findIndex(value => value.Key == this.rowKey)
							this.advancedProps[newPropIndex].Val = data.moreProperty.Val;
							break;
						case 'delete':
							this.advancedProps = this.advancedProps.filter(prop => prop.Key != this.rowKey);
							break;
						default:
							break;
						}
						this.clearMorePropertyValues()
						// Update model data
						this.$emit('update-model')
					}
				});
			},
			clearMorePropertyValues(){
				this.rowKey = ''
				this.rowValue = ''
				this.dialogModeAdvanced = ''
				this.buttonsAdvanced = []
				this.valueComponent = QTextField
				this.help = ''
			},
			showAdvancedPropertyModal(mode) {
				this.dialogModeAdvanced = mode;
				this.getButtonsDialogAdvanced();
				this.showDialogAdvanced = true;
			},
			changeAdvancedProperty(moreProperty) {
				this.rowKey = moreProperty.Key

				let advancedItem = this.SelectLists.PropertyList.find(item => item.Value == moreProperty.Key);
				if (advancedItem?.Type === 'P')
					this.rowValue = ''
				else
					this.rowValue = moreProperty.Val
					
				this.showAdvancedPropertyModal('edit');
			},
			deleteAdvancedProperty(moreProperty) {
				this.rowKey = moreProperty.Key
				this.rowValue = moreProperty.Val
				this.showAdvancedPropertyModal('delete');
			},
			createAdvancedProperty() {
				var vm = this;
				var url = QUtils.apiActionURL('Config', 'GetNewMorePropertyCfg');
				QUtils.FetchData(url).done((data) => {
					vm.showAdvancedPropertyModal('new');
				});
			},
		},
		mounted() {
			this.advancedProps = this.model.AdvancedProperties || [];
		},
		watch: {
			invalidProps(newValue) {
				if (this.buttonsAdvanced.length > 0)
					this.buttonsAdvanced[0].props.disabled = newValue
			},
			rowKey(newValue) {
				let advancedItem = this.SelectLists.PropertyList.find(item => item.Value === newValue);
				if (advancedItem?.Type) {
					switch (advancedItem.Type) {
						case 'C':
							this.valueComponent = QTextField
							break;
						case 'N':
							this.valueComponent = numeric_input
							break;
						case 'L':
							this.valueComponent = QCheckbox
							break;
						case 'P':
							this.valueComponent = QPasswordField
							break;
						default:
							this.valueComponent = QTextField
					}
					//Set default value if exists (Only for new properties).
					if (this.dialogModeAdvanced ==='new' && advancedItem.Default) {
						if (advancedItem.Type == 'L')
							this.rowValue = advancedItem.Default.toLowerCase() === "true"
						else
							this.rowValue = advancedItem.Default
					}
					this.help = advancedItem.translatedHelpVerbose || advancedItem.translatedHelp
				}
			}
		}
	};
</script>
