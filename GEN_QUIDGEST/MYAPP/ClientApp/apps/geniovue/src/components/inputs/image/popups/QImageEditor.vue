<template>
	<teleport :to="`#q-modal-image-edit-${controlId}-body`">
		<q-info-message
			v-if="showEditWarning"
			type="warning"
			:text="texts.cropWarning"
			@message-dismissed="setEditWarning(false)" />

		<fieldset>
			<div class="q-image-crop__container">
				<div class="row no-gutters">
					<div class="img-container">
						<img
							id="input-image"
							class="container cropper-hidden"
							ref="imageEditor"
							:src="imageToEdit"
							:alt="dataTitle" />
					</div>
				</div>

				<!-- Edit features -->
				<div class="row no-gutters mt-2">
					<q-button-group>
						<q-button
							:title="texts.zoomIn"
							class="q-image-edit-btn"
							@click="zoom(0.1)">
							<q-icon icon="zoom-in" />
						</q-button>
						<q-button
							:title="texts.zoomOut"
							class="q-image-edit-btn"
							@click="zoom(-0.1)">
							<q-icon icon="zoom-out" />
						</q-button>
					</q-button-group>

					<q-button-group>
						<q-button
							:title="texts.moveImageRight"
							class="q-image-edit-btn"
							@click="move(10, 0)">
							<q-icon icon="move-img-right" />
						</q-button>
						<q-button
							:title="texts.moveImageLeft"
							class="q-image-edit-btn"
							@click="move(-10, 0)">
							<q-icon icon="move-img-left" />
						</q-button>
						<q-button
							:title="texts.moveImageUp"
							class="q-image-edit-btn"
							@click="move(0, -10)">
							<q-icon icon="move-img-up" />
						</q-button>
						<q-button
							:title="texts.moveImageDown"
							class="q-image-edit-btn"
							@click="move(0, 10)">
							<q-icon icon="move-img-down" />
						</q-button>
					</q-button-group>

					<q-button-group>
						<q-button
							:title="texts.rotateLeft"
							class="q-image-edit-btn"
							@click="rotate(30)">
							<q-icon icon="rotate-left" />
						</q-button>
						<q-button
							:title="texts.rotateRight"
							class="q-image-edit-btn"
							@click="rotate(-30)">
							<q-icon icon="rotate-right" />
						</q-button>
					</q-button-group>

					<q-button-group>
						<q-button
							:title="texts.flipHorizontal"
							class="q-image-edit-btn"
							@click="flipX">
							<q-icon icon="horizontal-flip" />
						</q-button>
						<q-button
							:title="texts.flipVertical"
							class="q-image-edit-btn"
							@click="flipY">
							<q-icon icon="vertical-flip" />
						</q-button>
					</q-button-group>
				</div>
			</div>
		</fieldset>
	</teleport>

	<teleport :to="`#q-modal-image-edit-${controlId}-footer`">
		<div class="actions">
			<q-button
				variant="bold"
				:label="texts.save"
				:title="texts.save"
				@click="cropImage">
				<q-icon icon="save" />
			</q-button>

			<q-button
				:label="texts.cancel"
				:title="texts.cancel"
				@click="closeEditor">
				<q-icon icon="cancel" />
			</q-button>
		</div>
	</teleport>
</template>

<script>
	import Cropper from 'cropperjs'

	import QInfoMessage from '@/components/QInfoMessage.vue'

	export default {
		name: 'QImageEditor',

		emits: {
			'close-editor': () => true,
			'image-edited': (payload) => typeof payload === 'object' && 'data' in payload && 'dataFormat' in payload && 'encoding' in payload
		},

		components: {
			QInfoMessage
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique ID for the control.
			 */
			controlId: String,

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * The image to be edited.
			 */
			imageToEdit: {
				type: [String, Object],
				required: true
			},

			/**
			 * The type of the output image (defaults to .png).
			 */
			imageType: {
				type: String,
				default: 'png'
			},

			/**
			 * Image title used for the alt attribute.
			 */
			dataTitle: {
				type: String,
				default: null
			}
		},

		expose: [],

		data()
		{
			return {
				cropper: {},

				showEditWarning: true
			}
		},

		mounted()
		{
			this.cropper = new Cropper(this.$refs.imageEditor)
		},

		beforeUnmount()
		{
			this.cropper?.destroy()
		},

		methods: {
			/**
			 * Method to set the warning visibility.
			 * @param {Boolean} isVisible - Bool value representing the visibility.
			 */
			setEditWarning(isVisible)
			{
				this.showEditWarning = isVisible
			},

			/**
			 * Method to zoom the current image.
			 * @param {Number} percent - The amount of nuance in percent to be applied in zoom.
			 */
			zoom(percent)
			{
				this.cropper?.zoom(percent)
			},

			/**
			 * Method to rotate the current image.
			 * @param {Number} deg - The amount of rotation in degrees to be applied.
			 */
			rotate(deg)
			{
				this.cropper?.rotate(deg)
			},

			/**
			 * Method to move the current image.
			 * @param {Number} offsetX - The horizontal movement value.
			 * @param {Number} offsetY - The vertical movement value.
			 */
			move(offsetX, offsetY)
			{
				this.cropper?.move(offsetX, offsetY)
			},

			/**
			 * Method to flip the current image horizontally.
			 */
			flipX()
			{
				if (!this.cropper?.imageData)
					return

				const scale = isNaN(this.cropper.imageData.scaleX) || this.cropper.imageData.scaleX > 0 ? -1 : 1

				this.cropper?.scaleX(scale)
			},

			/**
			 * Method to flip the current image vertically.
			 */
			flipY()
			{
				if (!this.cropper?.imageData)
					return

				const scale = isNaN(this.cropper.imageData.scaleY) || this.cropper.imageData.scaleY > 0 ? -1 : 1

				this.cropper?.scaleY(scale)
			},

			/**
			 * Method to close the image editor.
			 */
			closeEditor()
			{
				this.cropper?.destroy()
				this.$emit('close-editor')
			},

			/**
			 * Method to crop the current image.
			 */
			cropImage()
			{
				if (this.cropper?.getCroppedCanvas)
				{
					// Gets the cropped canvas for post processing.
					const canvas = this.cropper.getCroppedCanvas()
					const base64Img = canvas.toDataURL(this.imageType)
					const fileData = base64Img.split(',').pop()
					const imgData = {
						data: fileData,
						dataFormat: this.imageType,
						fileName: 'Image.png',
						encoding: 'base64',
						isThumbnail: false
					}

					this.$emit('image-edited', imgData)
				}
				else
					this.closeEditor()
			}
		}
	}
</script>
