<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<form class="form-horizontal">
			<fieldset>
				<div class="form-flow">
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="Postal Code"
								readonly
								:class="['i-text', { 'i-text--required': false }]"
								:label-attrs="{ class: 'i-text__label' }">
								<q-mask
									mask-type="CP"
									v-model="masksData.mask1"
									@update:model-value="masksData.mask1 = $event" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="Taxpayer Nr."
								readonly
								:class="['i-text', { 'i-text--required': false }]"
								:label-attrs="{ class: 'i-text__label' }">
								<q-mask
									mask-type="NC"
									v-model="masksData.mask2"
									@update:model-value="masksData.mask2 = $event" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="NIB"
								readonly
								:class="['i-text', { 'i-text--required': false }]"
								:label-attrs="{ class: 'i-text__label' }">
								<q-mask
									mask-type="IB"
									v-model="masksData.mask3"
									@update:model-value="masksData.mask3 = $event" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="Social Security Nr."
								readonly
								:class="['i-text', { 'i-text--required': false }]"
								:label-attrs="{ class: 'i-text__label' }">
								<q-mask
									mask-type="SS"
									v-model="masksData.mask4"
									@update:model-value="masksData.mask4 = $event" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="IBAN"
								readonly
								:class="['i-text', { 'i-text--required': false }]"
								:label-attrs="{ class: 'i-text__label' }">
								<q-mask
									mask-type="IN"
									v-model="masksData.mask5"
									@update:model-value="masksData.mask5 = $event" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="License plate"
								readonly
								:class="['i-text', { 'i-text--required': false }]"
								:label-attrs="{ class: 'i-text__label' }">
								<q-mask
									mask-type="MA"
									v-model="masksData.mask6"
									@update:model-value="masksData.mask6 = $event" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="Email"
								readonly
								:class="['i-text', { 'i-text--required': false }]"
								:label-attrs="{ class: 'i-text__label' }">
								<q-mask
									mask-type="EM"
									v-model="masksData.mask7"
									@update:model-value="masksData.mask7 = $event" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="Manual mask"
								readonly
								:class="['i-text', { 'i-text--required': false }]"
								:label-attrs="{ class: 'i-text__label' }">
								<q-mask
									mask-type="MP"
									v-model="masksData.mask8"
									:mask-format="manualMask"
									@update:model-value="masksData.mask8 = $event" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="Upper case"
								readonly
								:class="['i-text', { 'i-text--required': false }]"
								:label-attrs="{ class: 'i-text__label' }">
								<q-mask
									mask-type="UP"
									v-model="masksData.mask9"
									@update:model-value="masksData.mask9 = $event" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>
				</div>
			</fieldset>
		</form>
	</div>
</template>

<script>
	export default {
		name: 'QMaskContainer',

		docsfile: './docs/inputs/Mask.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				manualMask: {
					mask: '000-AAA|a0a0a_SSS,XXX'
				},

				masksData: {
					mask1: '',
					mask2: '',
					mask3: '',
					mask4: '',
					mask5: '',
					mask6: '',
					mask7: '',
					mask8: '',
					mask9: ''
				}
			}
		}
	}
</script>
