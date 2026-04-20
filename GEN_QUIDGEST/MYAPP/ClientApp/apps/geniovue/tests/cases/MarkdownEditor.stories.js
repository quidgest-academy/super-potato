/**
 * @file MarkdownEditor.stories.js
 * @description Storybook stories for the QMarkdownEditor component.
 * Demonstrates various view modes and fullscreen behaviour.
 */

import QMarkdownEditor from "@/components/markdown/QMarkdownEditor.vue"
import {
	sampleMarkdown,
	sampleOptions,
	sampleTexts,
} from "./MarkdownEditor.mock.js"

export default {
	title: "Inputs/MarkdownEditor",
	component: QMarkdownEditor,
}

/**
 * Story showing the editor in simple mode.
 */
export const Simple = {
	argTypes: {
		modelValue: { control: "text" },
		showAlternativeView: { control: "boolean" },
		fullScreenContainer: { control: "text" },
		size: { control: "text" }
	},
	args: {
		modelValue: sampleMarkdown,
		options: sampleOptions,
		texts: sampleTexts,
		showAlternativeView: false,
		fullScreenContainer: "body",
		size: "block"
	}
}
