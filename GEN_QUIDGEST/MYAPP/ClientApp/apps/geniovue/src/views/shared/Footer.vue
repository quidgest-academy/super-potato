<template>
	<div id="q-footer">
<!-- eslint-disable indent, vue/html-indent, vue/script-indent -->
<!-- USE /[MANUAL FOR LAYOUT_FOOTER]/ -->
<!-- eslint-disable-next-line -->
<!-- eslint-enable indent, vue/html-indent, vue/script-indent -->
		<div class="c-footer">
			<div class="row">
				<language-items v-if="$app.layout.LanguagePlacement === 'in_footer'" />

				<div class="version-container">
					<span class="c-footer__genio-version">
						&copy; {{ $app.genio.generationDate.year }}
						<a
							href="http://www.quidgest.com/"
							target="_blank"
							rel="noopener noreferrer">
							Quidgest
						</a>
						| {{ version }} {{ $app.genio.assemblyVersion }}
					</span>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import { defineAsyncComponent } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	export default {
		name: 'QFooter',

		components: {
			LanguageItems: defineAsyncComponent(() => import('@/views/shared/LanguageItems.vue'))
		},

		mixins: [
			LayoutHandlers
		],

		expose: [],

		computed: {
			version()
			{
				return this.Resources[hardcodedTexts.version]
			}
		}
	}
</script>
