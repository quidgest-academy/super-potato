<template>
	<base-input-structure
		id="CTRL_1"
		label="Height: 100px, width: 100px">
		<q-image
			:image="img1.image"
			:modal-image="img1.imagePreview"
			:height="100"
			:width="100"
			:popup-is-visible="popupIsVisible"
			@open-image-preview="showImagePreview('img1')"
			@close-image-preview="closeImagePreview('img1')"
			@delete-image="handleImageDelete('img1')"
			@submit-image="handleImageUpload('img1', $event)"
			@show-popup="openPopup($event)"
			@hide-popup="closePopup($event)" />
	</base-input-structure>

	<br />
	<br />

	<base-input-structure
		id="CTRL_2"
		label="Control with image provided as url and in preview mode. (Height: 350px, width: 500px)">
		<q-image
			:image="img2.image"
			:modal-image="img2.imagePreview"
			is-preview-mode
			:height="350"
			:width="500"
			:popup-is-visible="popupIsVisible"
			@open-image-preview="showImagePreview('img2', 'imageURL')"
			@close-image-preview="closeImagePreview('img2')"
			@show-popup="openPopup($event)"
			@hide-popup="closePopup($event)" />
	</base-input-structure>

	<br />
	<br />

	<base-input-structure
		id="CTRL_3"
		label="Control with image provided as data object and in read only mode. (Height: 100px, width: 100px)">
		<q-image
			:image="img3.image"
			:modal-image="img3.imagePreview"
			readonly
			:height="100"
			:width="100"
			:popup-is-visible="popupIsVisible"
			@open-image-preview="showImagePreview('img3')"
			@close-image-preview="closeImagePreview('img3')"
			@show-popup="openPopup($event)"
			@hide-popup="closePopup($event)" />
	</base-input-structure>

	<br />
	<br />

	<base-input-structure
		id="CTRL_4"
		label="Control with image provided as data object and in preview mode. (Height: 100px, width: 100px)">
		<q-image
			:image="img4.image"
			:modal-image="img4.imagePreview"
			is-preview-mode
			:height="100"
			:width="100"
			:popup-is-visible="popupIsVisible"
			@open-image-preview="showImagePreview('img4')"
			@close-image-preview="closeImagePreview('img4')"
			@show-popup="openPopup($event)"
			@hide-popup="closePopup($event)" />
	</base-input-structure>

	<br />
	<br />

	<base-input-structure
		id="CTRL_5"
		label="Control with max image size (1024KB).">
		<q-image
			:image="img5.image"
			:modal-image="img5.imagePreview"
			:height="100"
			:width="100"
			:max-image-size="1024"
			:popup-is-visible="popupIsVisible"
			@open-image-preview="showImagePreview('img5')"
			@close-image-preview="closeImagePreview('img5')"
			@delete-image="handleImageDelete('img5')"
			@submit-image="handleImageUpload('img5', $event)"
			@show-popup="openPopup($event)"
			@hide-popup="closePopup($event)" />
	</base-input-structure>
</template>

<script>
	import BaseInputStructure from '@/components/inputs/BaseInputStructure.vue'
	import QImage from '@/components/inputs/image/QImage.vue'

	import mockData from './Image.mock.js'

	function getNewObj(img)
	{
		return {
			image: img,
			imagePreview: null
		}
	}

	export default {
		name: 'QImageContainer',

		docsfile: './docs/inputs/image/QImage.md',

		emits: [
			'show-popup',
			'hide-popup'
		],

		components: {
			BaseInputStructure,
			QImage
		},

		inheritAttrs: false,

		props: {
			/**
			 * Whether the popup is currently visible.
			 */
			popupIsVisible: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		data()
		{
			return {
				img1: getNewObj(mockData.image),
				img2: getNewObj(mockData.imageURL),
				img3: getNewObj(mockData.image),
				img4: getNewObj(mockData.image),
				img5: getNewObj(mockData.image)
			}
		},

		methods: {
			handleImageUpload(img, data)
			{
				const reader = new FileReader()
				reader.onload = (e) => {
					this[img].image = e.target.result
				}
				reader.readAsDataURL(data)
			},

			handleImageDelete(img)
			{
				this[img].image = null
			},

			showImagePreview(img, preview)
			{
				if (typeof preview !== 'string')
					preview = 'imagePreview'

				this[img].imagePreview = mockData[preview]
			},

			closeImagePreview(img)
			{
				this[img].imagePreview = null
			},

			openPopup(modalData)
			{
				this.$emit('show-popup', modalData)
			},

			closePopup(modalId)
			{
				this.$emit('hide-popup', modalId)
			}
		}
	}
</script>
