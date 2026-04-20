// Common definitions, enumerations, default strings, etc.

import markdownit from 'markdown-it'
import markdownItAttrs from 'markdown-it-attrs'

/**
 * Default text labels for the editor or toolbar components.
 */
export const DEFAULT_TEXTS = {
	editor: 'Editor',
	editorAndPreview: 'Editor & Preview',
	preview: 'Preview',
	fullScreen: 'Full screen',

	// Toolbar
	addBoldText: 'Add bold text',
	addItalicText: 'Add italic text',
	addHeadingText: 'Add heading text',
	addStrikethroughText: 'Add strikethrough text',
	insertQuote: 'Insert a quote',
	insertCodeBlock: 'Insert code block',
	addLink: 'Add a link',
	addBulletList: 'Add a bullet list',
	addNumberedList: 'Add a numbered list',
	addCheckList: 'Add a checklist',
	addHorizontalRule: 'Add a horizontal rule',
	addTable: 'Add a table',
	addImage: 'Add an image'
}

/**
 * The different modes of displaying the editor.
 */
export const viewModes = {
	EDIT: 'edit',       // Text area only
	BOTH: 'both',       // Text area + preview side by side
	PREVIEW: 'preview'  // Preview only
}

/**
 * Options object for markdown settings.
 */
export class MarkdownOptions
{
	/**
	 * Merges user options into a default configuration.
	 * 
	 * @param {Object|MarkdownOptions} [options={}] - Custom markdown options.
	 * @returns {MarkdownOptions} A configured MarkdownOptions instance.
	 */
	constructor(options = {})
	{
		// Define default values for each property.
		const defaults = {
			allowAttributes: false,
			allowCheckList: false,
			allowImage: true,
			enableTypographer: true
		}

		// let user allow custom HTML attributes or not
		this.allowAttributes = options?.allowAttributes ?? defaults.allowAttributes

		// let user allow add check list or not
		this.allowCheckList = options?.allowCheckList ?? defaults.allowCheckList

		// let user allow add image or not
		this.allowImage = options?.allowImage ?? defaults.allowImage

		// Enable some language-neutral replacement + quotes beautification
		// For the full list of replacements, see https://github.com/markdown-it/markdown-it/blob/master/lib/rules_core/replacements.mjs
		this.enableTypographer = options?.enableTypographer ?? defaults.enableTypographer
	}

	/**
	 * Static factory method to create a MarkdownOptions instance.
	 * This method is useful when it might have received a reactive object
	 * or an existing instance, and we want to ensure a new instance is constructed.
	 *
	 * @param {Object|MarkdownOptions} [options={}] Custom markdown options.
	 * @returns {MarkdownOptions} A new MarkdownOptions instance.
	 */
	static create(options = {}) {
		return new MarkdownOptions(options)
	}
}

/**
 * Converts Markdown content to Html.
 * @param {String} markdownString Markdown content
 * @param {Object|MarkdownOptions} options Markdown converter settings.
 * @returns {String} Html
 */
export function convertMarkdown(markdownString, options)
{
	const markdownOptions = MarkdownOptions.create(options)

	const md = markdownit({
		typographer: markdownOptions.enableTypographer
	})

	if(!markdownOptions.allowImage)
		md.disable('image')
	
	if(markdownOptions.allowAttributes) {
		md.use(markdownItAttrs, {
			allowedAttributes: ['id', 'class']
		})
	}

	const source = typeof markdownString === 'string' ? markdownString : ''
	const result = md.render(source)
	return result
}

export default {
	MarkdownOptions,
	convertMarkdown,
	DEFAULT_TEXTS,
	viewModes
}
