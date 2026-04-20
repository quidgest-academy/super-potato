<template>
	<div class="q-help__info-banner">
		<div class="q-help__info-banner-header">
			<q-icon icon="help" />
		</div>

		<div
			v-if="isHtml"
			class="q-help__info-banner-text"
			v-html="infoBannerText" />
		<div v-else>
			{{ infoBannerText }}
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QInfoBannerHelp',

		expose: [],

		props: {
			/**
			 * The help object.
			 */
			helpControl: Object,

			/**
			 * Whether the help content is HTML or not. If not, it is plain text.
			 */
			isHtml: {
				type: Boolean,
				default: true
			}
		},

		computed: {
			infoBannerText()
			{
				if (this.helpControl.shortHelp.text && this.helpControl.shortHelp.type === 'Info-banner')
					return this.helpControl.shortHelp.text
				else if (this.helpControl.detailedHelp.text && this.helpControl.detailedHelp.type === 'Info-banner')
					return this.helpControl.detailedHelp.text
				return ''
			}
		}
	}
</script>
