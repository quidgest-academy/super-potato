<template>
	<span class="q-help__subtext">
		<q-icon icon="help" />
		{{ subtextText }}
	</span>
</template>

<script>

	export default {
		name: 'SubtextHelp',

		expose: [],

		props: {
			/**
			 * The help object
			 */
			helpControl: Object
		},

		computed: {
			subtextText() {
				if(!this.helpControl) return ""
				if(this.helpControl.shortHelp.text)
					return this.helpControl.shortHelp.text
				return ""
			}
		}
	}
</script>
