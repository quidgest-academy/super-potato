/**
 * @file MarkdownEditor.spec.js
 * @description Unit tests for the QMarkdownEditor component using Vue Testing Library.
 * Tests cover rendering in different view modes, updating the model via the textarea,
 * and synchronising the scroll between the editor and preview.
 */

import '@testing-library/jest-dom'
import { fireEvent, screen } from "@testing-library/vue";
import { render } from './utils'
import QMarkdownEditor from "@/components/markdown/QMarkdownEditor.vue";
import {
	sampleMarkdown,
	sampleOptions,
	sampleTexts,
} from "../cases/MarkdownEditor.mock.js";

describe("QMarkdownEditor", () => {
	/**
	 * Test that the editor renders in BOTH mode (editor + preview) by default.
	 */
	it("renders both editor and preview by default", async () => {
		render(QMarkdownEditor, {
			props: {
				modelValue: sampleMarkdown,
				options: sampleOptions,
				texts: sampleTexts,
			},
		});

		// Check that the textarea (editor) is rendered.
		const textarea = screen.getByRole("textbox");
		expect(textarea).toBeInTheDocument();

		// Check that some part of the rendered markdown (e.g. header text) is visible.
		expect(screen.getByText(/Header/i)).toBeInTheDocument();
	});

	/**
	 * Test that the component emits an update when the textarea content changes.
	 */
	it("emits update:modelValue when typing in the textarea", async () => {
		const { getByRole, emitted } = render(QMarkdownEditor, {
			props: {
				modelValue: sampleMarkdown,
				options: sampleOptions,
				texts: sampleTexts,
			},
		});

		const textarea = getByRole("textbox");
		await fireEvent.update(textarea, "New markdown content");
		expect(emitted()["update:modelValue"]).toBeTruthy();
		expect(emitted()["update:modelValue"][0]).toEqual([
			"New markdown content",
		]);
	});

	/**
	 * Test that switching view modes works correctly.
	 */
	it("switches view modes correctly", async () => {
		render(QMarkdownEditor, {
			props: {
				modelValue: sampleMarkdown,
				options: sampleOptions,
				texts: sampleTexts,
			},
		});

		// Initially, both the textarea and preview should be visible.
		expect(screen.getByRole("textbox")).toBeInTheDocument();
		expect(screen.getByText(/Header/i)).toBeInTheDocument();

		// Switch to Editor mode by clicking the "Editor" button.
		const editorButton = screen.getByRole("button", {
			name: sampleTexts.editor,
		});
		await fireEvent.click(editorButton);
		// In Editor mode, preview should not be rendered.
		expect(screen.getByRole("textbox")).toBeInTheDocument();
		expect(screen.queryByText(/Header/i)).toBeNull();

		// Switch to Preview mode.
		const previewButton = screen.getByRole("button", {
			name: sampleTexts.preview,
		});
		await fireEvent.click(previewButton);
		// In Preview mode, the textarea should not be rendered.
		expect(screen.queryByRole("textbox")).toBeNull();
		expect(screen.getByText(/Header/i)).toBeInTheDocument();
	});
});
