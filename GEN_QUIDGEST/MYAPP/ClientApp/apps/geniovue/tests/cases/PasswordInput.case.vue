<template>
	<base-input-structure
		id="CTRL_1"
		label="Default">
		<q-password-input
			id="CTRL_1"
			model-value="password"
			@update:model-value="handleChange" />
	</base-input-structure>

	<br />

	<base-input-structure
		id="CTRL_2"
		label="Disabled">
		<q-password-input
			id="CTRL_2"
			model-value="Disabled"
			disabled />
	</base-input-structure>

	<br />

	<base-input-structure
		id="CTRL_2"
		label="Readonly">
		<q-password-input
			id="CTRL_2"
			model-value="Readonly"
			readonly />
	</base-input-structure>

	<br />

	<base-input-structure
		id="CTRL_3"
		label="With maxLength = 10">
		<q-password-input
			id="CTRL_3"
			:max-length="10"
			@update:model-value="handleChange" />
	</base-input-structure>

	<br />

	<base-input-structure
		id="CTRL_4"
		label="">
		<q-password-input
			id="CTRL_4"
			model-value="without label"
			:max-length="10"
			@update:model-value="handleChange" />
	</base-input-structure>
</template>

<script>
	import BaseInputStructure from '@/components/inputs/BaseInputStructure.vue'
	import QPasswordInput from '@/components/inputs/PasswordInput.vue'

	export default {
		name: 'QPasswordContainer',

		docsfile: './docs/inputs/PasswordInput.md',

		components: {
			BaseInputStructure,
			QPasswordInput
		},

		inheritAttrs: false,

		expose: [],

		methods: {
			handleChange(value)
			{
				alert('Password changed to: ' + value)
			}
		}
	}
</script>
