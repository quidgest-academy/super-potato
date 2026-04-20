<template>
	<div class="container-fluid">
		<fieldset class="form-fieldset">
			<legend>Pin Input Settings</legend>
			<div class="form-flow">
				<q-row-container>
					<q-control-wrapper class="control-join-group">
						<div
							class="input-group"
							id="input-length-container">
							<label for="input-length">Input Length:</label>
							<input
								type="number"
								id="input-length"
								v-model="inputLength" />
						</div>
						<div class="input-group">
							<label for="allow-alphanumeric">Alphanumeric</label>
							<input
								type="checkbox"
								name="allow-alphanumeric"
								v-model="alphanumeric" />
						</div>
						<div class="input-group">
							<label for="inputType">Input Type:</label>
							<select
								id="inputType"
								v-model="inputType">
								<option value="text">Text</option>
								<option value="password">Password</option>
							</select>
						</div>
						<div class="input-group">
							<label for="selectedTheme">Select Theme:</label>
							<select
								id="selectedTheme"
								v-model="selectedTheme">
								<option value="primary">Primary</option>
								<option value="secondary">Secondary</option>
								<option value="blue">Blue</option>
							</select>
						</div>
						<div class="input-group">
							<label for="selectBorder">Select Border:</label>
							<select
								id="selectBorder"
								v-model="selectBorder">
								<option value="">Default</option>
								<option value="borderless">Borderless</option>
								<option value="semi-borderless">Semi-Borderless</option>
								<option value="light">Light</option>
								<option value="gray">Gray</option>
								<option value="theme">Theme</option>
							</select>
						</div>
						<div class="input-group">
							<label for="v-model-input"> Model Value </label>
							<input
								type="text"
								id="v-model-input"
								v-model="pinValue" />
						</div>
					</q-control-wrapper>
				</q-row-container>
				<div class="pin-input-container">
					<base-input-structure
						id="CTRL_1"
						label="Input pin">
						<q-pin-input
							:char-number="inputLength"
							:pin-type="inputType"
							:theme="selectedTheme"
							:border-color="selectBorder"
							:model-value="pinValue"
							:alphanumeric="alphanumeric"
							@update:model-value="handleChange" />
					</base-input-structure>
				</div>
			</div>
		</fieldset>
	</div>
</template>

<script>
	import QPinInput from '@/components/rendering/QPinInput.vue'

	export default {
		name: 'QPinContainer',

		docsfile: './docs/inputs/PinInput.md',

		components: {
			QPinInput
		},

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				text: '',
				inputType: 'text',
				selectedTheme: 'primary',
				selectBorder: '',
				pinValue: '',
				inputLength: 10,
				alphanumeric: false
			}
		},

		methods: {
			/**
			 * Called on @update:modelValue
			 * @param {String} newValue
			 */
			handleChange(newValue)
			{
				this.pinValue = newValue
			}
		}
	}
</script>

<!-- eslint-disable-next-line vue/no-restricted-block -->
<style scoped>
	.container-fluid {
		flex: 1 1 auto;
		margin: 20px;
	}

	.form-fieldset {
		border: 1px solid #ccc;
		padding: 20px;
		border-radius: 8px;
	}

	.form-fieldset legend {
		font-size: 18px;
		font-weight: bold;
	}

	.form-flow {
		margin-top: 20px;
		display: inline;
	}

	.input-group {
		display: flex;
		align-items: center;
		margin-bottom: 10px;
	}

	.input-group label {
		margin-right: 8px;
	}

	.input-group select {
		padding: 6px 12px;
		border: 1px solid #ccc;
		border-radius: 4px;
	}

	.input-group select:focus {
		outline: none;
		border-color: #80bdff;
		box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
	}

	.pin-input-container {
		margin-top: 20px;
	}

	#input-length {
		width: 100px;
	}
</style>
