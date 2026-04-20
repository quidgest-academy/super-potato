<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<form class="form-horizontal">
			<fieldset>
				<div class="form-flow">
					<!-- Currency Input -->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								label="Currency Input"
								:label-attrs="{ class: 'i-text__label' }">
								<q-numeric-input
									v-model="currencyInput"
									id="CTRL_1"
									size="small"
									:max-integers="4"
									:max-decimals="5"
									currency-symbol="€"
									thousands-separator="."
									decimal-point="," />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<!-- Currency Input (Read only) -->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_2"
								label="Currency Input (Read only)"
								:label-attrs="{ class: 'i-text__label' }">
								<q-numeric-input
									v-model="currencyInput"
									id="CTRL_2"
									size="small"
									:max-integers="4"
									:max-decimals="5"
									readonly
									currency-symbol="€"
									thousands-separator="."
									decimal-point="," />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<!-- Currency Input without decimal symbol and with more chars in currency symbol -->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_3"
								label="Currency Input without decimal symbol and with more chars in currency symbol"
								:label-attrs="{ class: 'i-text__label' }">
								<q-numeric-input
									v-model="currencyInputWithoutDecimal"
									id="CTRL_3"
									size="large"
									:max-integers="4"
									currency-symbol="CHF"
									thousands-separator="."
									decimal-point="," />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<!-- Number Input with decimal (Required) -->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_4"
								label="Number Input with decimal (Required)"
								:label-attrs="{ class: 'i-text__label' }">
								<q-numeric-input
									v-model="withDecimalInput"
									id="CTRL_4"
									size="small"
									:max-integers="4"
									:max-decimals="5"
									is-required
									thousands-separator="."
									decimal-point="," />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<!-- Number Input without decimal -->
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_5"
								label="Number Input without decimal"
								:label-attrs="{ class: 'i-text__label' }">
								<q-numeric-input
									v-model="withoutDecimalInput"
									id="CTRL_5"
									size="small"
									:max-integers="5"
									thousands-separator="."
									decimal-point="," />
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
		name: 'QNumericContainer',

		docsfile: './docs/inputs/NumericInput.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				currencyInput: 0.0,
				withoutDecimalInput: 0,
				withDecimalInput: 0,
				currencyInputWithoutDecimal: 0
			}
		}
	}
</script>
