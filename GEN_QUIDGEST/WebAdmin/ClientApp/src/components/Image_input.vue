<template>
	<div class="i-image">
		<div v-if="label">
			<q-label
				:for="id"
				:label="label" />
		</div>
		<img :src="_src" />
		<template v-if="!isReadOnly && !isEmptyObject(setUrl)">
			<div class="row">
				<div class="col-12">
					<input
						type="file"
						@change="onFileChange" />
				</div>
			</div>
		</template>
	</div>
</template>

<script>
	import { getCurrentInstance } from 'vue'
	import { reusableMixin } from '@/mixins/mainMixin'
	import bootbox from 'bootbox'

	export default {
		name: 'ImageInput',

		mixins: [reusableMixin],

		props: {
			/**
			 * Image source url.
			 */
			getUrl: {
				type: String,
				default: ''
			},

			/**
			 * Image set url.
			 */
			setUrl: {
				type: String,
				default: ''
			},

			/**
			 * Input label.
			 */
			label: {
				type: String,
				default: ''
			},

			/**
			 * True if the input should be in a read-only state, false otherwise.
			 */
			isReadOnly: {
				type: Boolean,
				default: false
			}
		},

		emits: ['file-change', 'update-src'],

		expose: [],

		data() {
			return {
				id: null,
				v: Date.now()
			}
		},

		computed: {
			_src: function () {
				const tSrc = typeof this.getUrl === 'function' ? this.getUrl() : this.getUrl
				return tSrc + (tSrc.indexOf('?') === -1 ? '?' : '&') + 'v=' + this.v
			}
		},

		mounted() {
			this.id = 'input_img_' + getCurrentInstance().uid
		},

		methods: {
			onFileChange(e) {
				const files = e.target.files || e.dataTransfer.files
				if (!files.length) return

				const formData = new FormData()
				formData.append('image', files[0])

				jQuery.ajax({
					url: this.setUrl,
					type: 'POST',
					data: formData,
					processData: false,
					contentType: false,
					success: function (result) {
						if (result.Success) {
							this.updateSrc()
						} else {
							bootbox.alert(result.Message)
						}
					}
				})

				this.$emit('file-change')
			},

			updateSrc() {
				this.v = Date.now()

				this.$emit('update-src')
			},

			isEmptyObject(obj) {
				if (typeof obj === 'string') return obj.length === 0

				return Object.getOwnPropertyNames(obj).length === 0
			}
		}
	}
</script>
