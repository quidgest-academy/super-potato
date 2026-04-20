<template>
	<span
		@focusin="onFocusin"
		@focusout="onFocusout">
		<tinymce
			:id="controlId"
			ref="editor"
			v-model.lazy="curValue"
			:key="domKey"
			:class="classes"
			:api-key="apiKey"
			:data-size="size"
			:init="ctrlOptions"
			:tinymce-script-src="tinymceScriptSrc"
			:disabled="disabled || readonly"
			@focus="onFocusin"
			@blur="onFocusout" />
	</span>
</template>

<script>
	import { computed } from 'vue'
	import _isEmpty from 'lodash-es/isEmpty'
	import { inputSize, inputSizeCss } from '@quidgest/clientapp/constants/enums'
	import tinymce from '@tinymce/tinymce-vue'

	export default {
		name: 'QTextEditor',

		emits: [
			'update:modelValue',
			'ctrl-initialized',
			'ctrl-focused'
		],

		components: {
			tinymce
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The string value to be edited within the TinyMCE WYSIWYG editor.
			 */
			modelValue: {
				type: String,
				required: true
			},

			/**
			 * Whether the field is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is readonly.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * An optional label for accessibility purposes. If provided, it will be set as the aria-label.
			 */
			label: String,

			/**
			 * For accessibility purposes, the id of an element that adds additional labeling to the control.
			 */
			labelId: String,

			/**
			 * Placeholder text for the editor. Can be a string or a function that returns a string.
			 */
			placeholder: {
				type: [String, Function],
				default: ''
			},

			/**
			 * An array of custom classes to be applied to the editor.
			 */
			classes: {
				type: Array,
				default: () => []
			},

			/**
			 * The size category of the field.
			 */
			size: {
				type: String,
				default: 'medium',
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value)
			},

			/**
			 * TinyMCE API key if required for initializing the editor with certain cloud features.
			 */
			apiKey: {
				type: String,
				default: 'no-api-key'
			},

			/**
			 * Set tinymce locale.
			 * This component will calculate the language to use in the tinymce editor based on this locale
			 */
			locale: {
				type: String,
				default: 'en-US'
			}
		},

		expose: [],

		data()
		{
			return {
				controlId: this.id || `i-text-edit-${this._.uid}`,
				ctrlOptions: {
					setup: (editor) => editor.on('init', () => this.$emit('ctrl-initialized')),
					mode: 'exact',
					inline: this.disabled || this.readonly,
					readonly: this.disabled || this.readonly,
					language: computed(() => this.getSupportedLanguageTinymce()),
					convert_urls: false,
					encoding: 'raw',
					resize: 'both',
					menubar: false,
					powerpaste_allow_local_images: true,
					powerpaste_word_import: 'merge',
					powerpaste_html_import: 'merge',
					style_formats_autohide: true,
					importcss_append: true,
					browser_spellcheck: true,
					contextmenu: true,
					branding: false,
					image_advtab: true,
					//skin: false,
					//content_css: false,
					textpattern_patterns: [
						{ start: '*', end: '*', format: 'italic' },
						{ start: '**', end: '**', format: 'bold' },
						{ start: '#', format: 'h1' },
						{ start: '##', format: 'h2' },
						{ start: '###', format: 'h3' },
						{ start: '####', format: 'h4' },
						{ start: '#####', format: 'h5' },
						{ start: '######', format: 'h6' },
						{ start: '1. ', cmd: 'InsertOrderedList' },
						{ start: '* ', cmd: 'InsertUnorderedList' },
						{ start: '- ', cmd: 'InsertUnorderedList' },
					],
					plugins: [
						'powerpaste',
						'autoresize',
						'textpattern',
						'importcss',
						'imagetools',
						'fullpage',
						'lists',
						'advlist',
						'autolink',
						'link',
						'image',
						'imagetools',
						'charmap',
						'print',
						'preview',
						'hr',
						'anchor',
						'pagebreak',
						'searchreplace',
						'wordcount',
						'visualblocks',
						'visualchars',
						'code',
						'fullscreen',
						'insertdatetime',
						'media',
						'nonbreaking',
						'table',
						'directionality',
						'emoticons'
					],
					toolbar1: 'cut copy paste | undo redo | styleselect | fontselect fontsizeselect | removeformat | bullist numlist | outdent indent | visualblocks ltr rtl  | searchreplace | media image link unlink | ',
					toolbar2: 'bold italic underline strikethrough | subscript superscript | forecolor backcolor | alignleft aligncenter alignright alignjustify | table | anchor pagebreak hr | charmap emoticons | print preview fullscreen | fullpage | code',
					width: inputSizeCss[this.size]
					//height: 500,
					//toolbar: ''
				},
				// Load static files - TinyMCE Self-hosted
				tinymceScriptSrc: 'Content/js/plugins/tinymce/tinymce.min.js',
				domKey: 0
			}
		},

		beforeUnmount()
		{
			this.destroyEditor()
		},

		computed: {
			/**
			 * Proxy computed property for v-model to provide two-way data binding with the TinyMCE editor.
			 */
			curValue: {
				get()
				{
					return this.modelValue
				},
				set(newValue)
				{
					this.$emit('update:modelValue', newValue)
				}
			}
		},

		methods: {
			/**
			 * Destroys the TinyMCE editor instance, ensuring proper cleanup and removal from the DOM.
			 */
			destroyEditor()
			{
				if (window.tinymce)
				{
					const editorCtrl = window.tinymce.get(this.id)
					if (editorCtrl)
					{
						editorCtrl.remove()
						editorCtrl.destroy(true)
					}
				}
			},

			/**
			 * Re-renders the text editor.
			 * @param {boolean} readonly Whether readonly mode is active
			 */
			redraw(readonly)
			{
				this.destroyEditor()
				this.ctrlOptions.readonly = readonly
				this.ctrlOptions.inline = readonly
				this.domKey++
			},

			/**
			 * Get the tinymce-compliant language string for the given locale
			 */
			getSupportedLanguageTinymce()
			{
				switch (this.locale)
				{
					case 'pt-PT':
					case 'fr-FR':
					case 'zh-CN':
					case 'zh-TW':
						return this.locale.replace('-', '_')
					case 'ar-MA':
					case 'ar':
						return 'ar'
					case 'es-ES':
					case 'es':
						return 'es'
					case 'de-DE':
					case 'de':
						return 'de'
					case 'da-DK':
					case 'da':
						return 'da'
					case 'pl-PL':
					case 'pl':
						return 'pl'
					case 'ca-ES':
					case 'ca':
						return 'ca'
					default:
						return 'en'
				}
			},

			/**
			 * Get whether an external element (popup) of the editor is focused
			 */
			getExternalElementIsFocused()
			{
				// Elements that contain editor's popup and dropdown elements
				const externalContainerElements = document.querySelectorAll('.tox.tox-silver-sink.tox-tinymce-aux')

				// When opening a popup or dropdown in the editor, the focus first goes to document.body
				// and adds a special class and/or attribute and then the focus goes to the actual element
				// in the container with the editor's external elements
				if(
					document.activeElement === document.body &&
					(
						document.body.classList.contains('tox-dialog__disable-scroll') ||
						document.body.getAttribute('data-scroll-locked') === "1"
					)
				)
					return true

				// Check if focus went to one of the editor's external elements
				externalContainerElements.forEach((element) => {
					if(element.contains(document.activeElement))
						return true
				})

				return false
			},

			/**
			 * Determine if the control has focus and emit the value
			 * @param controlHasFocus Whether an element in the main control or an element in the control's external container has focus
			 */
			emitControlFocus(controlHasFocus)
			{
				this.$emit('ctrl-focused', controlHasFocus)
			},

			/**
			 * Called when focusing on an element within the container element around the editor
			 */
			onFocusin()
			{
				this.emitControlFocus(true)
			},

			/**
			 * Called when focusing away from an element within the container element around the editor
			 */
			onFocusout()
			{
				this.emitControlFocus(this.getExternalElementIsFocused())
			}
		},

		watch: {
			disabled(newVal)
			{
				this.redraw(newVal)
			},

			readonly(newVal)
			{
				this.redraw(newVal)
			},

			locale()
			{
				this.redraw(this.disabled || this.readonly)
			}
		}
	}
</script>
