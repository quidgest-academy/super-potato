<template>
	<div class="row">
		<div class="col">
			<div class="ms-selectable g-reporting__flow-content-selectable">
				<ul class="ms-list">
					<li
						v-for="(table, tIndex) in selectedItems"
						:key="table.Id"
						id="ms-FieldsSelected-optgroup-@i"
						class="ms-optgroup-container g-reporting__flow-content-selectable-item">
						<ul class="ms-optgroup">
							<li class="ms-optgroup-label">
								<div>
									<q-icon
										icon="list"
										class="q-icon--reporting mr-2" />

									{{ table.Description }}

									<q-button @click="showTableFields(tIndex)">
										<q-icon
											:icon="table.modeClass"
											class="q-icon--group" />
									</q-button>
								</div>
							</li>

							<ul
								v-show="table.mode"
								v-for="(field, fIndex) in table.Fields"
								:key="field.Description">
								<!-- This is used when iterating selected fields of a table-->
								<!-- Parent handles the selection of fields here we just display fields that haven't been chosen once -->
								<div v-if="!allowDuplicates">
									<li
										v-if="!field.isSelected"
										class="g-reporting__elem-selectable ms-optgroup g-reporting__flow-content-droppable-item"
										@click.stop.prevent="emitField(fIndex, tIndex)">
										<div
											v-if="field.Title !== ''"
											class="g-reporting__elem-selectable">
											{{ field.Title }}
										</div>

										<q-icon
											icon="circle-arrow-right"
											class="q-icon--reporting" />
									</li>
								</div>
								<!-- This is used when iterating all fields of a table -->
								<!-- We always display all fields regardless of already been chosen once -->
								<div v-else>
									<li
										class="g-reporting__elem-selectable ms-optgroup g-reporting__flow-content-droppable-item"
										@click.stop.prevent="emitField(fIndex, tIndex)">
										<div class="g-reporting__elem-selectable">
											{{ field.Description }}
										</div>

										<q-icon
											icon="circle-arrow-right"
											class="q-icon--reporting" />
									</li>
								</div>
							</ul>
						</ul>
					</li>
				</ul>
			</div>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QCavDropdownSelectedFields',

		emits: ['selected-field'],

		props: {
			/**
			 * Array to hold the previously selected fields
			 */
			fields: {
				type: Array,
				default: () => []
			},

			/**
			 * If true when you select a field it doesn't disappear
			 */
			allowDuplicates: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				selectedItems: []
			}
		},

		mounted()
		{
			this.selectedItems = this.fields
			this.addDropDown()
		},

		methods: {
			addDropDown()
			{
				// Used for handling the dropdown
				this.selectedItems.forEach((table) => {
					table.modeClass = 'add'
					table.mode = false
				})
			},

			showTableFields(index)
			{
				// Switch the class that is rendered
				if (this.selectedItems[index].mode)
					this.selectedItems[index].modeClass = 'add'
				else
					this.selectedItems[index].modeClass = 'minus-sign'

				this.selectedItems[index].mode = !this.selectedItems[index].mode
			},

			emitField(fIndex, tIndex)
			{
				const field = this.selectedItems[tIndex].Fields[fIndex]
				this.$emit('selected-field', field)
			}
		}
	}
</script>
