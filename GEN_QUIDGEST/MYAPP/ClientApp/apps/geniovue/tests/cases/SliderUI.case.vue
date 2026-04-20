<template>
	<div class="component">
		<div class="options">
			<div class="input-group">
				<label for="labelSelect">label:</label>
				<input
					id="labelSelect"
					type="text"
					v-model="label" />
			</div>
			<div class="input-group">
				<label for="inputType">Input Type:</label>
				<select
					id="inputType"
					v-model="inputType">
					<option value="def">Default</option>
					<option value="sec">Sec</option>
					<option value="box">Box</option>
				</select>
			</div>
			<div class="input-group">
				<label for="selectTheme">Select Theme:</label>
				<select
					id="selectTheme"
					v-model="selectTheme">
					<option value="primary">Primary</option>
					<option value="secondary">Secondary</option>
					<option value="blue">Blue</option>
				</select>
			</div>
			<div class="input-group">
				<input
					type="checkbox"
					id="limits"
					v-model="limits" />
				<label for="limits">Limits</label>
			</div>
			<div class="input-group">
				<label for="minimum"> Min</label>
				<input
					type="number"
					id="minimum"
					v-model="minValue" />
				<label for="minimum"> Max</label>
				<input
					type="number"
					id="maximum"
					v-model="maxValue" />
			</div>
			<div class="input-group">
				<label for="v-model-input"> Model Value </label>
				<input
					type="number"
					id="v-model-input"
					v-model="value" />
			</div>
		</div>
		<q-slider
			:label="label"
			:input-type="inputType"
			:show-limits="limits"
			:theme="selectTheme"
			:min-value="minValue"
			:max-value="maxValue"
			:model-value="value"
			@update:model-value="handleChange" />
	</div>
</template>

<script>
	import QSlider from '@/components/rendering/QSliderUI.vue'

	export default {
		name: 'QSliderContainer',

		docsfile: './docs/inputs/QSlider.md',

		components: {
			QSlider
		},

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				value: 0,
				inputType: 'def',
				limits: true,
				selectTheme: 'primary',
				minValue: 0,
				maxValue: 100,
				label: 'Label'
			}
		},

		methods: {
			/**
			 * Called on @update:modelValue
			 * @param {Number} newValue
			 */
			handleChange(newValue)
			{
				this.value = newValue
			}
		}
	}
</script>

<!-- eslint-disable-next-line vue/no-restricted-block -->
<style scoped>
	.component {
		display: inline-flex;
		gap: 50px;
	}

	.options {
		border: thin solid black;
		border-top: none;
		border-bottom: none;
		border-left: none;
		width: 200px;
		height: 200px;
	}
</style>
