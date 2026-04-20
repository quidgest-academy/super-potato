<template>
	<form
		class="form-horizontal"
		@submit.prevent>
		<q-row-container>
			<div class="q-ssrs-viewer">
				<iframe
					:id="id"
					:src="srcUrl"
					scrolling="no"
					:width="width"
					:height="height">
					iframes not supported
				</iframe>
			</div>
		</q-row-container>

		<q-row-container>
			<div id="footer-action-btns">
				<q-button
					:id="backBtn.id"
					:label="backBtn.text"
					@click="goBack">
					<q-icon icon="back" />
				</q-button>
			</div>
		</q-row-container>
	</form>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import NavHandlers from '@/mixins/navHandlers.js'

	/**
	 * Just a prototype to try using SSRS Viewer in MVC-like iframe.
	 * To support opening in another tab, it's need history cloning mechanism.
	 */
	export default {
		name: 'QReportingServicesViewer',

		inheritAttrs: false,

		mixins: [
			NavHandlers
		],

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: {
				type: String,
				required: true
			}
		},

		expose: [
			'navigationId'
		],

		data()
		{
			return {
				height: '100%',

				width: '100%',

				backBtn: {
					id: 'back-btn',
					icon: {
						icon: 'back',
						type: 'svg'
					},
					text: computed(() => this.Resources[hardcodedTexts.goBack])
				}
			}
		},

		mounted()
		{
			window.addEventListener('message', this.resizeIframe)
		},

		beforeUnmount()
		{
			window.removeEventListener('message', this.resizeIframe)
		},

		computed: {
			srcUrl()
			{
				// Note: It doesn't work with debug on Vite because of the proxy.
				return `/ReportViewerWebForm.aspx?id=${this.id}`
			}
		},

		methods: {
			resizeIframe(msg)
			{
				if (msg)
				{
					this.height = msg.source?.document?.body?.scrollHeight || '100%'
					this.width = msg.source?.document?.body?.scrollWidth || '100%'
				}
			}
		}
	}
</script>
