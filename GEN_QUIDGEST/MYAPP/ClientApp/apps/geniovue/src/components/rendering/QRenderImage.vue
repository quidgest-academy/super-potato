<template>
	<img
		class="q-render-image__thumbnail"
		v-bind="imageAttrs"
		@click.stop.prevent="openPreview" />

	<q-image-preview
		v-if="fullSizeImage"
		:image="fullSizeImage"
		:data-title="this.options?.dataTitle"
		:texts="texts"
		@close-image-preview="closePreview" />
</template>

<script>
	import { defineAsyncComponent } from 'vue'
	import { imageObjToSrc, validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	const DEFAULT_TEXTS = {
		close: 'Close',
		download: 'Download'
	}

	export default {
		name: 'QRenderImage',

		emits: ['execute-action'],

		components: {
			QImagePreview: defineAsyncComponent(() => import('@/components/QImagePreview.vue'))
		},

		inheritAttrs: false,

		data()
		{
			return {
				/**
				 * The full sized image to be displayed in a modal.
				 */
				fullSizeImage: null
			}
		},

		props: {
			/**
			 * The image object containing the data and metadata necessary to render the image.
			 * data: The actual base64 image data or link to the image.
			 * dataFormat: The format of the image, e.g., 'image/png'.
			 * fileName: The name of the image file.
			 * encoding: The type of encoding used for the image data e.g., 'base64'.
			 */
			value: {
				type: Object,
				default: () => ({
					data: '',
					dataFormat: 'image/png',
					fileName: '',
					encoding: 'base64',
					isThumbnail: true
				})
			},

			/**
			 * Information about the column of the image
			 */
			options: {
				type: Object,
				required: true
			},

			/**
			 * Information about the row of the image
			 */
			row: {
				type: Object,
				required: true
			},

			/**
			 * Path for the resources.
			 */
			resourcesPath: {
				type: String,
				required: true
			},

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			}
		},

		expose: [],

		computed: {
			/**
			 * Attributes for the image element.
			 */
			imageAttrs()
			{
				let src = `${this.resourcesPath}no_img.png`

				if (this.value !== null)
					src = imageObjToSrc(this.value)

				return { src, alt: this.options?.dataTitle }
			}
		},

		methods: {
			/**
			 * Emits a request for the full image to preview.
			 */
			openPreview()
			{
				const ticket = this.row?.Fields[this.options.name]?.ticket
				if (!ticket)
					return

				this.$emit('execute-action', {
					action: 'preview-image',
					area: this.options.area,
					ticket,
					callback: this.receiveImagePreview
				})
			},

			/**
			 * Receives the requested image to preview.
			 */
			receiveImagePreview(image)
			{
				this.fullSizeImage = image
			},

			/**
			 * Closes the image preview.
			 */
			closePreview()
			{
				this.fullSizeImage = null
			}
		}
	}
</script>
