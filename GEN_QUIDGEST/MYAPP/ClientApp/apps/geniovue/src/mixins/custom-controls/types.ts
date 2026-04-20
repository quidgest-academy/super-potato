// ──────────────────────────────────────────────
// Custom Controls
// ──────────────────────────────────────────────

/** Represents a mapped column. */
export type MappedValue = {
	/** Column value. */
	readonly value: string

	/** Column raw data. */
	readonly rawData: unknown

	/** Background color for the column. */
	readonly bgColor?: string

	/** Text color for the column. */
	readonly textColor?: string

	/** Source metadata for the column. */
	readonly source: {
		/** Area name of the source data. */
		readonly area?: string
		/** Field name in the source data. */
		readonly field?: string
		/** Component used to render the column. */
		readonly component?: string
		/** Options passed to the component. */
		readonly componentOptions?: Record<string, unknown>
		/** Label for the column. */
		readonly label?: string
	}

	/** Preview data (e.g., base64 string). */
	previewData?: string

	/** Dominant color of the image. */
	readonly dominantColor?: string
}

/** Represents a mapped row. */
export type MappedRow = {
	/** Unique row key. */
	readonly rowKey: string

	/** Column used as the main title. */
	readonly title?: MappedValue

	/** Column used as the subtitle. */
	readonly subtitle?: MappedValue

	/** Array of columns containing text content. */
	readonly text?: readonly MappedValue[]

	/** Image-related data for the row. */
	readonly image?: MappedValue

	/** Optional follow-up action when clicking the row. */
	readonly customFollowup?: MappedValue

	/** Optional follow-up target (e.g., self or blank). */
	readonly customFollowupTarget?: MappedValue

	/** Permissions for action buttons. */
	readonly btnPermission: {
		readonly editBtnDisabled: boolean
		readonly viewBtnDisabled: boolean
		readonly deleteBtnDisabled: boolean
		readonly insertBtnDisabled: boolean
	}

	/** Visibility settings for custom actions. */
	readonly actionVisibility?: Record<string, unknown>

	/** Disability settings for custom actions. */
	readonly actionDisability?: Record<string, unknown>
}
