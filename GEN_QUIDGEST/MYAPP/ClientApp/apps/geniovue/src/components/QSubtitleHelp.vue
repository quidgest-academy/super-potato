<template>
	<span class="q-help__subtitle">{{ subtitleText }}</span>
</template>

<script>
	import HelpControl from '@/mixins/helpControls.js'

	export default {
		name: 'SubtitleHelp',

		mixins: [HelpControl],

		expose: []
	}
</script>
