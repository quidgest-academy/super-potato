<template>
	<q-markdown-viewer
		v-if="showAlternativeView"
		:id="viewerId"
		:model-value="modelValue"
		:options="options" />
	<div
		v-else
		:id="ctrlId"
		class="q-markdown-editor"
		:class="[
			sizeClass,
			{
				'q-markdown-editor--fullscreen': isPreviewFullScreen
			}
		]"
		ref="mainContainer">
		<div class="q-markdown-editor__header">
			<!-- Markdown logo -->
			<q-icon
				class="q-markdown-editor__header--logo"
				icon="language-markdown-outline"
				size="40" />

			<!-- View mode -->
			<q-toggle-group
				:model-value="currentViewMode"
				@update:model-value="setViewMode">
				<q-toggle-group-item
					class="q-markdown-editor__viewmode-btn"
					:value="viewModes.EDIT"
					:label="texts.editor"
					data-control-type="view-mode">
					<q-icon
						icon="new-suggestion"
						size="20" />
				</q-toggle-group-item>
				<q-toggle-group-item
					class="q-markdown-editor__viewmode-btn"
					:value="viewModes.BOTH"
					:label="texts.editorAndPreview"
					data-control-type="view-mode">
					<q-icon
						icon="locker-multiple"
						size="20" />
				</q-toggle-group-item>
				<q-toggle-group-item
					class="q-markdown-editor__viewmode-btn"
					:value="viewModes.PREVIEW"
					:label="texts.preview"
					data-control-type="view-mode">
					<q-icon
						icon="view"
						size="20" />
				</q-toggle-group-item>
			</q-toggle-group>
		</div>
		<!-- Main Body: if in 'both' mode, we show editor (left) & preview (right).
			if in 'edit' mode, we show only editor, and if 'preview', only the preview.
		-->
		<div
			class="q-markdown-editor__body"
			ref="bodyContainer">
			<!--
				Left Side: Editor (containing toolbar above the textarea).
				Visible in either 'edit' or 'both' modes.
			-->
			<div
				v-if="currentViewMode === viewModes.EDIT || currentViewMode === viewModes.BOTH"
				class="q-markdown-editor__edit"
				:class="{ 'q-markdown-editor__edit--full-width': currentViewMode === viewModes.EDIT }"
				:style="{ width: currentViewMode === viewModes.BOTH ? editorColumnWidth + '%' : '100%' }"
				ref="textEditorContainer">
				<!-- Toolbar -->
				<toolbar
					@action="handleAction"
					:texts="texts"
					:options="options"
					:disabled="disabled || readonly" />
				<!-- Markdown input area -->
				<textarea
					class="q-markdown-editor__textarea"
					:readonly="readonly"
					:disabled="disabled"
					ref="textarea"
					:value="modelValue"
					@input="onInput"
					@paste="handlePaste"
					@keydown="handleKeydown"
					@scroll="onEditorScroll"
					resize="none">
				</textarea>
			</div>
			<!-- Resizer handle -->
			<div
				v-if="currentViewMode === viewModes.BOTH"
				class="q-markdown-editor__divider"
				ref="resizer"
				@mousedown="onDividerMouseDown">
				<q-icon
					icon="drag-vertical"
					size="20" />
			</div>
			<!--
				Right Side: Preview
				Visible in 'preview' or 'both' modes.
			-->
			<div
				v-if="currentViewMode === viewModes.PREVIEW || currentViewMode === viewModes.BOTH"
				class="q-markdown-editor__preview"
				:class="{
					'q-markdown-editor__preview--full-width': currentViewMode === viewModes.PREVIEW,
					'q-markdown-editor__preview--full-screen': isPreviewFullScreen
				}"
				:style="previewContainerStyle"
				ref="previewContainer">
				<!-- Full screen mode-->
				<q-button
					class="q-markdown-editor__viewmode-btn"
					borderless
					data-control-type="full-screen"
					:description="texts.fullScreen"
					@click="togglePreviewFullScreen">
					<q-icon
						icon="aspect-ratio"
						size="20" />
				</q-button>
				<!-- Preview content-->
				<q-markdown-viewer
					:id="viewerId"
					:model-value="modelValue"
					:options="options" />
			</div>
		</div>
	</div>
</template>

<script>
	import { markRaw } from 'vue'
	import { v4 as uuidv4 } from 'uuid'
	import _isEmpty from 'lodash-es/isEmpty'

	import Toolbar from './Toolbar.vue'
	import QMarkdownViewer from './QMarkdownViewer.vue'

	import { MarkdownOptions, DEFAULT_TEXTS, viewModes } from './markdown.js'
	import { inputSize } from '@quidgest/clientapp/constants/enums'
	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	/**
	 * QMarkdownEditor
	 *
	 * A Markdown editor component with toolbar, preview,
	 * image paste, and keyboard shortcuts.
	 */
	export default {
		name: 'QMarkdownEditor',

		emits: [
			'update:modelValue',
			'error'
		],

		components: {
			Toolbar,
			QMarkdownViewer
		},

		inheritAttrs: false,

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The value bound to the control.
			 */
			modelValue: {
				type: String,
				default: ''
			},

			/**
			 * Editor size variant (for later use).
			 */
			size: {
				type: String,
				validator: (value) => _isEmpty(value) || Reflect.has(inputSize, value),
				default: inputSize.block
			},

			/**
			 * Whether the field is readonly.
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Whether the field is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},

			/**
			 * Options for Markdown (which may disable certain functionalities and toolbar buttons)
			 */
			options: {
				type: Object,
				default: () => new MarkdownOptions()
			},

			/**
			 * If true, the viewer only is rendered
			 */
			showAlternativeView: {
				type: Boolean,
				default: false
			},

			/**
			 * The selector or ID of the container
			 * to occupy when preview goes full-screen.
			 * e.g., '#form-container'
			 */
			fullScreenContainer: {
				type: String,
				default: '#form-container',
			},
		},

		expose: [],

		data() {
			return {
				// Current view mode: "edit", "both", or "preview"
				currentViewMode: viewModes.BOTH,
				viewModes,

				// Fullscreen state for the preview area
				isPreviewFullScreen: false,
				// Object holding the fullscreen container's rect values (as strings with 'px')
				fullScreenRect: {
					top: "0px",
					left: "0px",
					right: "0px",
					bottom: "0px",
					height: '100%',
					width: '100%'
				},
				// Store the ResizeObserver instance so we can disconnect later
				resizeObserver: null,

				// The editor's width ratio (0 to 1) relative to the main container (default 50%)
				editorRatio: 0.5,
				// Minimum width for the editor column (set to at least the toolbar's width)
				minEditorWidth: 210,
				// Flag to track horizontal resizing
				isResizing: false,
				// Variables for tracking drag start position and width
				startX: 0,
				startEditorWidth: 0,

				// Mapping of handlers for actions triggered by the toolbar
				toolbarActionsHandlers: markRaw({
					applyBold: { action: this.applyBold },
					applyItalic: { action: this.applyItalic },
					applyHeading: { action: this.applyHeading },
					applyStrikethrough: { action: this.applyStrikethrough },
					applyQuote: { action: this.applyQuote },
					applyCodeBlock: { action: this.applyCodeBlock },
					applyLink: { action: this.applyLink },
					applyBulletList: { action: this.applyBulletList },
					applyNumberedList: { action: this.applyNumberedList },
					applyChecklist: { action: this.applyChecklist },
					applyHorizontalRule: { action: this.applyHorizontalRule },
					applyTable: { action: this.applyTable },
					insertImageMarkdown: { action: this.insertImageMarkdown }
				}),

				// Definition of shortcuts
				actionsShortcuts: markRaw({
					/** Bold: Ctrl + B */
					b: { action: this.applyBold, ctrl: true, shift: false },
					/** Italic: Ctrl + I */
					i: { action: this.applyItalic, ctrl: true, shift: false },
					/** Link: Ctrl + K */
					k: { action: this.applyLink, ctrl: true, shift: false },
					/** Strikethrough: Ctrl + Shift + X */
					x: { action: this.applyStrikethrough, ctrl: true, shift: true },
					/** Horizontal rule (custom): Ctrl + M */
					m: { action: this.applyHorizontalRule, ctrl: true, shift: false }
				})
			}
		},

		computed: {
			/**
			 * Unique control ID
			 */
			ctrlId() {
				return this.id ?? `uid-${uuidv4()}`
			},

			/**
			 * Unique viewer ID
			 */
			viewerId() {
				return `viewer-${this.ctrlId}`
			},

			/**
			 * Dynamic class for controlling editor size (fixed widths) based on the 'size' prop (for later use).
			 */
			sizeClass() {
				return `q-markdown-editor--${this.size}`
			},

			/**
			 * Current width of the main container (in pixels)
			 */
			containerWidth() {
				return this.$refs.mainContainer ? this.$refs.mainContainer.clientWidth : 0
			},

			/**
			 * Editor column width (in percentage), calculated from the stored ratio
			 */
			editorColumnWidth() {
				return Math.round(this.editorRatio * 100)
			},

			/**
			 * Preview column width (in percentage)
			 */
			previewColumnWidth() {
				return 100 - this.editorColumnWidth
			},

			/**
			 * Compute the style for the preview container.
			 * In fullscreen mode, use fixed positioning with the measured rect.
			 * Otherwise, if in BOTH mode, use the computed preview width.
			 */
			previewContainerStyle()
			{
				// When not in fullscreen, use your normal logic
				if(!this.isPreviewFullScreen) {
					const width = `${this.currentViewMode === viewModes.BOTH ? this.previewColumnWidth : 100}%`
					return { width }
				}

				// Fullscreen mode: use the reactive fullScreenRect (all values are rounded strings)
				return {
					position: 'fixed',
					top: this.fullScreenRect.top,
					left: this.fullScreenRect.left,
					right: this.fullScreenRect.right,
					bottom: this.fullScreenRect.bottom,
					height: this.fullScreenRect.height,
					width: this.fullScreenRect.width
				}
			}
		},

		beforeUnmount() {
			document.removeEventListener('mousemove', this.onDividerMouseMove)
			document.removeEventListener('mouseup', this.onDividerMouseUp)
			window.removeEventListener('scroll', this.updateFullScreenPosition)
			window.removeEventListener('resize', this.updateFullScreenPosition)
			if (this.resizeObserver) {
				this.resizeObserver.disconnect()
				this.resizeObserver = null
			}
		},

		methods: {
			/**
			 * Whenever the user types, synchronise the new content with v-model.
			 * @param {InputEvent} event - Input event
			 */
			onInput(event) {
				this.updateValue(event.target.value)
			},

			/**
			 * Emit the updated value so it can be synced via v-model (modelValue).
			 * @param {string} newContent - The new Markdown string.
			 */
			updateValue(newContent) {
				this.$emit('update:modelValue', newContent)
			},

			/**
			 * Set the current view mode; if not in preview-only, exit fullscreen
			 * @param {string} mode - "edit", "both", or "preview"
			 */
			setViewMode(mode) {
				this.currentViewMode = mode
				// Exit full-screen mode if switching away from preview-only.
				if (mode !== viewModes.PREVIEW) {
					this.isPreviewFullScreen = false
				}
			},

			/**
			 * Update the fullscreen rectangle using the target container's bounding rect.
			 * This method safely checks if the target container exists.
			 */
			updateFullScreenPosition() {
				const fullScreenEl = document.querySelector(this.fullScreenContainer)
				if (fullScreenEl) {
					const rect = fullScreenEl.getBoundingClientRect()
					// Round all values to avoid fractional pixels issues.
					this.fullScreenRect.top = Math.round(rect.top) + 'px'
					this.fullScreenRect.left = Math.round(rect.left) + 'px'
					this.fullScreenRect.right = Math.round(rect.right) + 'px'
					this.fullScreenRect.bottom = Math.round(rect.bottom) + 'px'
					this.fullScreenRect.height = Math.round(rect.height) + 'px'
					this.fullScreenRect.width = Math.round(rect.width) + 'px'
				} else {
					this.$emit('error', { message: `Target container "${this.fullScreenContainer}" not found.` })
				}
			},

			/**
			 * Initialise a ResizeObserver to detect size changes on the target container.
			 * This observer is safely created only if the target exists.
			 */
			initResizeObserver() {
				const targetEl = document.querySelector(this.fullScreenContainer)
				if (targetEl) {
					// Disconnect any existing observer first.
					if (this.resizeObserver) {
						this.resizeObserver.disconnect()
					}
					this.resizeObserver = new ResizeObserver(() => {
						// Update the fullscreen position on every change.
						this.updateFullScreenPosition()
					})
					this.resizeObserver.observe(targetEl)
				} else {
					this.$emit('error', { message: `Target container "${this.fullScreenContainer}" not found for ResizeObserver.` })
				}
			},

			/**
			 * Set or clear fullscreen mode.
			 * When enabled, start listening to scroll/resize events and initialise the ResizeObserver.
			 * @param enable
			 */
			setFullScreenMode(enable) {
				this.isPreviewFullScreen = enable

				if (this.isPreviewFullScreen) {
					this.setViewMode(viewModes.PREVIEW)
					// Immediately update position
					this.updateFullScreenPosition()
					// Initialise observer to detect size changes (e.g. sidebar animations)
					this.initResizeObserver()
					// Also update on scroll/resize
					window.addEventListener('scroll', this.updateFullScreenPosition)
					window.addEventListener('resize', this.updateFullScreenPosition)
				} else {
					window.removeEventListener('scroll', this.updateFullScreenPosition)
					window.removeEventListener('resize', this.updateFullScreenPosition)
					if (this.resizeObserver) {
						this.resizeObserver.disconnect()
						this.resizeObserver = null
					}
				}
			},

			/**
			 * Toggle the full-screen preview mode.
			 */
			togglePreviewFullScreen() {
				this.setFullScreenMode(!this.isPreviewFullScreen)
			},

			/**
			 * This method synchronises the preview scroll with the textarea scroll.
			 */
			onEditorScroll() {
				if (this.currentViewMode !== viewModes.BOTH) return
				const editor = this.$refs.textarea
				const preview = this.$refs.previewContainer
				if (!editor || !preview) return
				// Calculate the ratio of the editor's current scroll.
				const editorScrollable = editor.scrollHeight - editor.clientHeight
				const previewScrollable = preview.scrollHeight - preview.clientHeight
				const ratio = editorScrollable > 0 ? editor.scrollTop / editorScrollable : 0
				// Set the preview scrollTop according to the same ratio.
				preview.scrollTop = ratio * previewScrollable
			},

			// -----------------------------------------------------------------
			// Resizing logic
			// -----------------------------------------------------------------

			/**
			 * Handle the beginning of horizontal resizing when the divider is dragged.
			 * @param {MouseEvent} event
			 */
			onDividerMouseDown(event) {
				this.isResizing = true
				this.startX = event.clientX


				const textEditorEl = this.$refs.textEditorContainer
				if (textEditorEl && textEditorEl.offsetWidth) {
					this.startEditorWidth = textEditorEl.offsetWidth
				} else {
					this.isResizing = false
					return
				}

				window.addEventListener('mousemove', this.onDividerMouseMove)
				window.addEventListener('mouseup', this.onDividerMouseUp)
			},

			/**
			 * Handle mouse movements during horizontal resizing.
			 * This function updates the editor's width based on the mouse movement (dx)
			 * while enforcing minimum and maximum bounds.
			 * @param {MouseEvent} event - The mouse event triggered during resizing.
			 */
			onDividerMouseMove(event) {
				if (!this.isResizing) return

				const dx = event.clientX - this.startX
				let newEditorWidth = this.startEditorWidth + dx

				// Ensure the editor column does not shrink below the toolbar's width.
				if (newEditorWidth < this.minEditorWidth) {
					newEditorWidth = this.minEditorWidth
				}

				// The subtraction of 50 pixels from the container's total width ensures that the preview column always
				// retains at least 50 pixels of width, defined at the JavaScript level in addition to CSS.
				// This minimum prevents the splitter and preview section from becoming too narrow or disappearing entirely.
				const minimumSize = 50

				// Prevent the editor column from taking too much space.
				const total = this.containerWidth
				if (newEditorWidth > total - minimumSize) {
					newEditorWidth = total - minimumSize
				}
				this.editorRatio = newEditorWidth / total
			},

			/**
			 * Handle mouseup event to end horizontal resizing.
			 * @param {MouseEvent} event
			 */
			onDividerMouseUp() {
				this.isResizing = false
				window.removeEventListener('mousemove', this.onDividerMouseMove)
				window.removeEventListener('mouseup', this.onDividerMouseUp)
			},

			// -----------------------------------------------------------
			// Formatting Actions
			// -----------------------------------------------------------

			/**
			 * Bold formatting: `**bold**`.
			 */
			applyBold() {
				this.wrapSelectedText('**', '**')
			},

			/**
			 * Italic formatting: `*italic*`.
			 */
			applyItalic() {
				this.wrapSelectedText('*', '*')
			},

			/**
			 * Heading formatting: e.g. prepends `# ` to each selected line.
			 */
			applyHeading(num) {
				num ??= 1
				// Insert a heading at the start of the selected text
				this.insertTextAtStart('#'.repeat(num))
			},

			/**
			 * Strikethrough formatting: `~~Strikethrough~~`.
			 */
			applyStrikethrough() {
				this.wrapSelectedText('~~', '~~')
			},

			/**
			 * Quote formatting: `> Quote`.
			 */
			applyQuote() {
				// Insert a quote at the start of the selected text
				this.insertTextAtStart('>')
			},

			/**
			 * Code block formatting:
			 * Inserts triple-backticks around the selection.
			 */
			applyCodeBlock() {
				// The extra newline at the end can help keep a clean separation.
				this.wrapSelectedText('\n```\n', '\n```\n')
			},

			/**
			 * Link formatting: `[link text](https://...)`.
			 */
			applyLink() {
				this.wrapSelectedText('[', '](https://)')
			},

			/**
			 * Bullet list formatting: `- item`.
			 */
			applyBulletList() {
				this.insertTextAtStart('-')
			},

			/**
			 * Numbered list formatting: `1. item`.
			 */
			applyNumberedList() {
				this.insertTextAtStart('1.')
			},

			/**
			 * Checklist formatting: `- [ ] item`.
			 */
			applyChecklist() {
				this.insertTextAtStart('- [ ]')
			},

			/**
			 * Horizontal rule: `---`.
			 */
			applyHorizontalRule() {
				// Insert on a new line
				this.insertTextAtStart('\n---\n', false, false)
			},

			/**
			 * Inserts a table snippet.
			 */
			applyTable() {
				this.insertTextAtCursor('\n|        |        |\n| ------ | ------ |\n|        |        |\n|        |        |')
			},

			/**
			 * Insert a placeholder image Markdown tag at the cursor.
			 * Actual image source can be edited by the user manually or replaced via paste.
			 */
			insertImageMarkdown() {
				this.insertTextAtCursor('![](https://)')
			},

			// -----------------------------------------------------------
			// Text Insertion & Selection Helpers
			// -----------------------------------------------------------

			/**
			 * Wraps currently selected text (or a placeholder if empty) with Markdown syntax.
			 * @param {string} startTag - The Markdown opening syntax (e.g., '**').
			 * @param {string} endTag - The Markdown closing syntax (e.g., '**').
			 */
			wrapSelectedText(startTag, endTag) {
				const textarea = this.$refs.textarea
				if (!textarea) return

				const textValue = textarea.value
				const { selectionStart, selectionEnd } = textarea

				const selectedText = textValue.slice(selectionStart, selectionEnd)
				const placeholder = selectedText || 'Your text'
				const newText = `${startTag}${placeholder}${endTag}`

				// Rebuild the content
				const before = textValue.slice(0, selectionStart)
				const after = textValue.slice(selectionEnd)
				const newContent = before + newText + after

				// Update modelValue
				this.updateValue(newContent)

				// Re-position the cursor
				this.$nextTick().then(() => {
					const newCursorPos = selectionStart + startTag.length
					textarea.selectionStart = newCursorPos
					textarea.selectionEnd = newCursorPos + placeholder.length
					textarea.focus()
				})
			},

			/**
			 * Inserts a given text snippet at the current cursor position in the textarea.
			 * If text is selected, it replaces the selection.
			 * @param {string} text - The text snippet to insert
			 */
			insertTextAtCursor(text) {
				const textarea = this.$refs.textarea
				if (!textarea) return

				const textValue = textarea.value
				const { selectionStart, selectionEnd } = textarea

				// Rebuild the content
				const before = textValue.slice(0, selectionStart)
				const after = textValue.slice(selectionEnd)
				const newContent = before + text + after

				// Update modelValue
				this.updateValue(newContent)

				// Re-position the cursor
				this.$nextTick().then(() => {
					const newPos = selectionStart + text.length
					textarea.selectionStart = newPos
					textarea.selectionEnd = newPos
					textarea.focus()
				})
			},

			/**
			 * Insert a given text snippet at the start of each selected line.
			 * Optionally requires a space after inserted text, and can decide whether
			 * to re-select the newly inserted text or place the cursor after it.
			 * @param {string} text - The text snippet to insert (e.g., '#', '>').
			 * @param {boolean} requireSpace - If true, insert a space after `text`.
			 * @param {boolean} selectInserted - If true, highlight the inserted text in the editor after insertion.
			 */
			insertTextAtStart(text, requireSpace = true, selectInserted = true) {
				const textarea = this.$refs.textarea
				if (!textarea) return

				const textValue = textarea.value
				const { selectionStart, selectionEnd } = textarea

				const linesSeected = textValue.slice(selectionStart, selectionEnd)
				const spaceOrEmpty = requireSpace ? ' ' : ''

				// Insert `text` + spaceOrEmpty at the start of every line
				const lines = linesSeected
					.split('\n')
					.map((line) => (line ? `${text}${spaceOrEmpty}${line}` :  `${text}${spaceOrEmpty}`))
				const newText = lines.join('\n')

				// Rebuild the final content
				const before = textValue.slice(0, selectionStart)
				const after = textValue.slice(selectionEnd)
				const newContent = before + newText + after

				// Update modelValue
				this.updateValue(newContent)

				// Re-position cursor
				this.$nextTick().then(() => {
					if(selectInserted) {
						// Re-select the newly inserted block
						textarea.selectionStart = selectionStart
						textarea.selectionEnd = selectionStart + newText.length
					}
					else {
						// Move cursor to the end of the inserted text
						const newPos = selectionStart + newText.length
						textarea.selectionStart = newPos
						textarea.selectionEnd = newPos
					}
					textarea.focus()
				})
			},

			// -----------------------------------------------------------
			// Clipboard & Keyboard Handling
			// -----------------------------------------------------------

			/**
			 * Handle paste events to detect images in the clipboard and insert them as base64 data URIs.
			 * Prevents default if an image is found, then inserts via Markdown `![Pasted Image](data:image/...)`.
			 * @param {ClipboardEvent} event - Clipboard event
			 */
			handlePaste(event) {
				if (!event.clipboardData || !event.clipboardData.items) return

				const { items } = event.clipboardData
				for (let i = 0; i < items.length; i++) {
					const item = items[i]
					// Check if the pasted item is an image
					if (item.type.indexOf('image') === 0) {
						event.preventDefault()
						const file = item.getAsFile()
						if (file) {
							const reader = new FileReader()
							reader.onload = (loadEvent) => {
								const base64Image = loadEvent.target.result
								const markdownImage = `\n\n![Pasted Image](${base64Image})\n\n`
								this.insertTextAtCursor(markdownImage)
							}
							reader.readAsDataURL(file)
						}
					}
				}
			},

			/**
			 * Handle keyboard shortcuts on the textarea.
			 * @param {KeyboardEvent} event - Keyboard event
			 */
			handleKeydown(event) {
				// Cross-platform check (Ctrl on Windows/Linux, Cmd on macOS).
				const ctrlOrCmd = event.ctrlKey || event.metaKey
				const isShift = event.shiftKey
				const key = event.key.toLowerCase()
				const shortcut = this.actionsShortcuts[key]

				if(shortcut && ctrlOrCmd.ctrl === ctrlOrCmd && shortcut.shift === isShift)
				{
					event.preventDefault()
					shortcut.action()
				}
			},

			/**
			 * Handler for processing the action triggered by the toolbar.
			 * @param {object} eventData - Event data, including name and additional parameters if applicable.
			 */
			handleAction(eventData) {
				const event = eventData?.event
				if(typeof event !== 'string') return

				const actionHandler = this.toolbarActionsHandlers[event]
				if(actionHandler)
					actionHandler.action(eventData.params)
			}
		}
	}
</script>
