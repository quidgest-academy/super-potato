<template>
	<div class="modal-fbody">
		<div class="container-fluid">
			<q-row-container>
				<q-control-wrapper class="control-join-group">
					<base-input-structure
						id="text-create-base"
						class="i-text"
						:label="texts.queryName"
						:label-attrs="{ class: 'i-text__label' }">
						<q-text-field
							id="text-create-input"
							size="small"
							v-model="queryName"
							:max-length="15" />
					</base-input-structure>
				</q-control-wrapper>
			</q-row-container>

			<q-row-container>
				<q-control-wrapper class="control-join-group">
					<base-input-structure
						id="text-persistence-base"
						class="i-text"
						:label="texts.saveQuery"
						:label-attrs="{ class: 'i-text__label' }">
						<q-radio-group
							id="text-persistence-input"
							v-model="radioOption"
							:value="accessTypes">
							<q-radio-button
								v-for="radio in radioOption"
								:key="radio.key"
								:value="radio.key"
								:label="radio.value" />
						</q-radio-group>
					</base-input-structure>
				</q-control-wrapper>
			</q-row-container>

			<q-row-container>
				<q-control-wrapper class="control-join-group">
					<q-button
						variant="bold"
						:label="texts.create"
						:title="texts.create"
						@click="hidePopup">
						<q-icon icon="ok" />
					</q-button>
				</q-control-wrapper>
			</q-row-container>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'

	export default ({
		name: 'QCavCreateQuery',

		emits: ['hide-create-popup'],

		expose: [],

		data()
		{
			return {
				radioOption: '',

				queryName: '',

				empty: '',

				isOverride: true,

				accessTypes: [
					{
						key: '1',
						value: {
							text: 'Yes',
							selected: false
						},
					},
					{
						key: '2',
						value: {
							text: 'No',
							selected: true
						}
					}
				],

				texts: {
					queryName: computed(() => this.Resources[hardcodedTexts.queryName]),
					saveQuery: computed(() => this.Resources[hardcodedTexts.saveQuery]),
					create: computed(() => this.Resources[hardcodedTexts.create])
				}
			}
		},

		methods: {
			hidePopup()
			{
				if (this.radioOption === '1')
					this.$emit('hide-create-popup', true)
				else
					this.$emit('hide-create-popup', false)
			}
		}
	})
</script>
