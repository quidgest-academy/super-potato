<template>
	<div class="keyboard-input-popup">
		<div class="keyboard-input-popup-inner">
			<slot></slot>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QKeyboardPopup',

		inheritAttrs: false,

		expose: []
	}
</script>
