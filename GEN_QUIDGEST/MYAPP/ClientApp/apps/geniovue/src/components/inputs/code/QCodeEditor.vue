<template>
	<div
		class="q-code-editor__wrapper"
		@focusout="updateModelValue">
		<div class="q-code-editor__options">
			<q-button
				v-if="!readonly"
				class="q-code-editor__diffs-btn"
				:label="texts.showChanges"
				:disabled="!aceEditorMode"
				@click="updateShowDiff">
				<q-icon icon="retweet" />
			</q-button>
			<div class="q-code-editor__theme-picker">
				<span>
					{{ texts.theme }}
				</span>
				<q-toggle-group
					:model-value="themeValue"
					:disabled="!aceEditorMode"
					@update:model-value="updateDarkMode">
					<q-toggle-group-item
						value="light"
						:title="texts.light">
						<q-icon icon="light-theme" />
					</q-toggle-group-item>
					<q-toggle-group-item
						value="dark"
						:title="texts.dark">
						<q-icon icon="dark-theme" />
					</q-toggle-group-item>
				</q-toggle-group>
			</div>
		</div>
		<div class="q-code-editor__container">
			<div
				v-if="!differActive"
				ref="editor"
				class="q-code-editor__editor"
				tabindex="0">
			</div>
			<div
				v-else
				ref="editorDiffer"
				:class="differClasses"
				:style="{ height: aceEditorDifferHeight }"
				tabindex="0">
			</div>
		</div>
	</div>
</template>

<script>
	// Ace
	import ace, { EditSession } from 'ace-builds'
	import { UndoManager } from 'ace-builds'

	// Ace modes, themes and workers - dynamic imports avoid first launch overhead
	import './ace/ace-imports'

	// Diff checker
	import AceDiff from 'ace-diff'

	// Utils
	import { nextTick } from 'vue'
	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	// The default component texts
	const DEFAULT_TEXTS = {
		showChanges: 'Show changes',
		dark: 'Dark',
		light: 'Light',
		theme: 'Theme',
		defaultPlaceholder: 'Write your code here...',
	}

	export default {
		name: 'QCodeEditor',

		emits: [
			'update:modelValue'
		],

		props: {
			/**
			 * Unique identifier for the control.
			 */
			id: String,

			/**
			 * The value on the code editor after loading
			 */
			modelValue: {
				type: String,
				required: true
			},

			/**
			 * The mode (or language) used by the code editor
			 */
			language: {
				type: String,
				default: ''
			},

			/**
			 * True if the editor can only be read, false otherwise
			 */
			readonly: {
				type: Boolean,
				default: false
			},

			/**
			 * Defines the height of the editor, in code rows.
			 */
			rows: {
				type: Number,
				default: 15,
				validator: (value) => value > 0
			},

			/**
			 * Texts object for various messages.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			}
		},

		expose: [],

		data() {
			return {
				/**
				 * The editor object.
				 */
				aceEditor: null,

				/**
				 * The editor renderer.
				 */
				aceEditorRenderer: null,

				/**
				 * The editor's undo manager.
				 */
				aceEditorUndoManager: null,

				/**
				 * The current editor session.
				 */
				aceEditorSession: null,

				/**
				 * The current editor mode.
				 */
				aceEditorMode: 'plain-text',

				/**
				 * The editor's initial value.
				 */
				aceEditorInitialValue: '',

				/**
				 * The current differ instance.
				 */
				aceEditorDiffer: null,

				/**
				 * The current differ's value.
				 */
				aceEditorDifferValue: '',

				/**
				 * The current differ height - Differ doesn't have min/maxLines, so requires the editor renderer's height.
				 */
				aceEditorDifferHeight: '',

				/**
				 * True if the differ is currently active, false otherwise
				 */
				differActive: false,

				/**
				 * Shows the changes to the initial value made so far
				 */
				showDiff: false,

				/**
				 * True if the editor is currently using the dark theme, false otherwise
				 */
				darkMode: false,

				/**
				 * The default editor themes
				 */
				EDITOR_THEMES: {
					light: 'dawn',
					dark: 'tomorrow_night_eighties'
				},

				/**
				 * The list of supported code languages
				 */
				availableModes: {
					cpp: 'c_cpp',
					cs: 'csharp',
					js: 'javascript',
					html: 'html',
					css: 'css',
					xml: 'xml',
					java: 'java',
					sql: 'sql',
					plaintext: 'plain-text'
				}
			}
		},

		computed: {
			/**
			 * True when editing plain text, false if editing code
			 */
			plainTextMode() {
				return this.aceEditorMode === this.availableModes.plaintext
			},

			/**
			 * Classes for the editor differ - separates light and dark theme styles
			 */
			differClasses() {
				return ['q-code-editor__differ', this.darkMode ? 'dark-mode' : 'light-mode']
			},

			/**
			 * Current value of the light/dark toggle - used as model-value.
			 */
			themeValue() {
				return this.darkMode ? 'dark' : 'light'
			}
		},

		async mounted() {
			this.aceEditorInitialValue = this.modelValue
			this.aceEditorMode = this.getModeFromLanguage(this.language)
			await nextTick()
			this.initEditor()
		},

		beforeUnmount() {
			this.destroyAceEditor()
			this.destroyAceDiff()
		},

		methods: {
			initEditor() {
				// If the HTML hasn't been rendered yet, do nothing
				if (!this.$refs.editor) return

				// Remove previous
				this.destroyAceDiff()

				// Assign editor variable
				this.aceEditor = ace.edit(this.$refs.editor)

				// Configure editor options
				this.aceEditor.resize()
				this.setReadOnlyEditor(this.readonly)
				this.aceEditor.setOptions({
					enableBasicAutocompletion: true,
					enableLiveAutocompletion: true,
					autoScrollEditorIntoView: true,
					behavioursEnabled: true, // auto-pairing of quotes/brackets
					placeholder: this.texts.defaultPlaceholder
				})

				// Create the editor's undo manager and its commands
				this.undoManager = new UndoManager()
				this.aceEditor.commands.addCommands([
					{
						name: 'undo',
						bindKey: 'Ctrl-Z',
						exec: function (editor) {
							editor.session.getUndoManager().undo()
						}
					},
					{
						name: 'redo',
						bindKey: 'Ctrl-Y',
						exec: function (editor) {
							editor.session.getUndoManager().redo()
						}
					}
				]);

				// Assign editor renderer variable
				this.aceEditorRenderer = this.aceEditor.renderer

				// Configure renderer options
				this.aceEditorRenderer.setScrollMargin(2, 2, 2, 2)
				this.aceEditorRenderer.setOptions(
					{
						showPrintMargin: false,
						fontSize: '0.8rem',
						minLines: this.rows,
						maxLines: this.rows
					}
				)

				// Initialize the editor session
				this.initSession()
			},

			initSession() {
				// If the editor isn't initialized, do nothing
				if (!this.aceEditor) return

				// Store the previous session value and clear its text selection. If coming from differ, store its value instead
				const previousSessionValue = this.aceEditorDifferValue || this.modelValue
				this.clearEditorSelection()

				// Create new session and assign it to the editor
				this.aceEditorSession = new EditSession(previousSessionValue)
				this.aceEditor.setSession(this.aceEditorSession)

				// Set the editor's UndoManager for the current session
				this.aceEditorSession.setUndoManager(this.undoManager)

				// Set new mode (code language) and theme (editor style) - must be imported in ace-imports
				this.aceEditorSession.setMode(`ace/mode/${this.aceEditorMode}`)
				this.changeRendererTheme(this.darkMode)
				this.changeRendererGutter()

				// Configure session options
				this.aceEditorSession.setUseWrapMode(true)
				this.aceEditorSession.setOptions(
					{
						useWorker: true, // use web workers to do syntax checking and linting - improves performance
						tabSize: 4,
						wrap: 'printmargin',
						wrapMethod: this.plainTextMode ? 'text' : 'code'
					}
				)
				if (this.aceEditorMode === 'javascript') { // Allow JS lines not to have semicolons
					this.aceEditorSession.$worker?.send("changeOptions", [
						{ asi: true }
					])
				}

				// Set height for differ initialization
				this.aceEditorDifferHeight = this.aceEditorRenderer.getContainerElement().style.height
			},

			getSessionValue() {
				return this.aceEditorSession?.getValue()
			},

			getModeFromLanguage(lang) {
				switch (lang) {
					case 'xpo':
					case 'cshtml':
						return this.availableModes.cs
					case 'sln':
						return this.availableModes.xml
					default:
						return this.availableModes[lang] ?? this.availableModes.plaintext
				}
			},

			setReadOnlyEditor(readonly) {
				this.aceEditor?.setReadOnly(readonly)
			},

			initEditorDiffer() {
				// If the HTML hasn't been rendered yet, do nothing
				if (!this.$refs.editorDiffer) return

				const differ = new AceDiff({
					element: this.$refs.editorDiffer,
					mode: `ace/mode/${this.aceEditorMode}`,
					theme: `ace/theme/${this.darkMode ? this.EDITOR_THEMES.dark : this.EDITOR_THEMES.light}`,
					showConnectors: false,

					left: {
						content: this.aceEditorInitialValue,
						editable: false,
						copyLinkEnabled: false
					},
					right: {
						content: this.aceEditorSession?.getValue(),
						editable: false,
						copyLinkEnabled: false
					},
				})

				// Remove PrintMargin lines from editors
				const differEditors = differ.getEditors()
				differEditors.right.setShowPrintMargin(false);
				differEditors.left.setShowPrintMargin(false);

				// Remove previous
				this.destroyAceDiff()

				// Instantiate differ and store a ref to its current value
				this.aceEditorDiffer = differ
				this.aceEditorDifferValue = this.getEditedDifferValue()
			},

			changeDifferTheme(dark) {
				if (!this.aceEditorDiffer) return

				const differEditors = this.aceEditorDiffer.getEditors()
				differEditors.right.setTheme(`ace/theme/${dark ? this.EDITOR_THEMES.dark : this.EDITOR_THEMES.light}`)
				differEditors.left.setTheme(`ace/theme/${dark ? this.EDITOR_THEMES.dark : this.EDITOR_THEMES.light}`)
			},

			getEditedDifferValue() {
				if (!this.aceEditorDiffer) return

				// Get the right editor's value, as it represents the current value
				const rightEditor = this.aceEditorDiffer.getEditors().right
				return rightEditor.session.getValue()
			},

			changeRendererTheme(dark) {
				this.aceEditorRenderer?.setTheme(`ace/theme/${dark ? this.EDITOR_THEMES.dark : this.EDITOR_THEMES.light}`)
			},

			changeRendererGutter() {
				this.aceEditorRenderer?.setShowGutter(!this.plainTextMode)
			},

			changeRendererHeight(rows) {
				this.aceEditorRenderer?.setOptions({
					minLines: rows,
					maxLines: rows
				})
			},

			clearEditorSelection() {
				this.aceEditor?.clearSelection()
			},

			updateDarkMode(mode) {
				this.darkMode = mode === 'dark'
			},

			updateShowDiff() {
				this.showDiff = !this.showDiff
			},

			updateModelValue() {
				this.$emit('update:modelValue', (this.differActive ? this.getEditedDifferValue() : this.getSessionValue()))
			},


			destroyAceEditor()
			{
				this.aceEditor?.destroy()
				this.aceEditor = null
			},

			destroyAceDiff()
			{
				this.aceEditorDiffer?.destroy()
				this.aceEditorDiffer = null
			}
		},

		watch: {
			/**
			 * Creates a new session upon changing the language.
			 */
			language(newLang) {
				this.aceEditorMode = this.getModeFromLanguage(newLang)
				if (!this.differActive) // Can't change modes while diffing
					this.initSession()
			},

			/**
			 * Changes the renderer's theme.
			 */
			darkMode(dark) {
				if (this.differActive)
					this.changeDifferTheme(dark)
				else
					this.changeRendererTheme(dark)
			},

			/**
			 * Changes the renderer's height, based on its code lines.
			 */
			rows(rows) {
				this.changeRendererHeight(rows)
			},

			/**
			 * Changes the editor's editability.
			 */
			readonly(readonly) {
				this.setReadOnlyEditor(readonly)
			},

			/**
			 * Swaps between the editor and the differ.
			 */
			showDiff(show) {
				this.aceEditorDifferValue = this.getEditedDifferValue()
				this.aceEditorDifferHeight = show ? this.aceEditorRenderer.getContainerElement().style.height : this.aceEditorDifferHeight

				if(this.differActive) {
					this.destroyAceDiff()
				} else {
					this.destroyAceEditor()
				}

				this.differActive = show

				if (show)
					nextTick().then(() => { this.initEditorDiffer() })
				else
					nextTick().then(() => { this.initEditor() })
			}
		}
	}
</script>
