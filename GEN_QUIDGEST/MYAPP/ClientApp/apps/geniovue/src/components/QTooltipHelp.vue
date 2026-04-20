<template>
	<q-tooltip
		:anchor="anchor"
		:text="tooltipText" />
</template>

<script>
	import HelpControl from '@/mixins/helpControls.js'

	export default {
		name: 'TooltipHelp',

		expose: [],

		mixins: [HelpControl],
		
		props: {
			/**
			* Unique identifier for the control.
			*/
			anchor: String
		}
	}
</script>
