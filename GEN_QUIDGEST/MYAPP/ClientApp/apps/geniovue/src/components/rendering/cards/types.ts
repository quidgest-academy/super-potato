// Types
import type { MappedRow } from '@/mixins/custom-controls/types'
import type { ListConfig } from '@/mixins/types'
import type { DEFAULT_TEXTS } from './constants'

// ──────────────────────────────────────────────
// Primitive / Enum-like Types
// ──────────────────────────────────────────────

/** Available card sizes. */
export type Size = 'regular' | 'small'

/** Alignment options for content inside a card. */
export type ContentAlignment = 'start' | 'center'

/** Possible shapes for card images. */
export type ImageShape = 'rectangular' | 'square' | 'circular'

/** Variants or layouts of a card. */
export type Subtype =
	| 'card'
	| 'card-img-top'
	| 'card-img-thumbnail'
	| 'card-img-background'
	| 'card-horizontal'

/** Styles available for insert cards. */
export type InsertCardStyle = 'image' | 'primary' | 'secondary'

/** Customizable texts used in cards and insert cards. */
export type Texts = typeof DEFAULT_TEXTS

// ──────────────────────────────────────────────
// Shared Props
// ──────────────────────────────────────────────

/** Base props shared across QCardView and QInsertCard. */
export type QCardViewBaseProps = {
	/** Card variant/subtype. */
	readonly subtype?: Subtype

	/** Size of the card. */
	readonly size?: Size

	/** Alignment of the content inside the card. */
	readonly contentAlignment?: ContentAlignment

	/** Amount of scale applied when hovering over the card. */
	readonly hoverScaleAmount?: string | number
}

// ──────────────────────────────────────────────
// QCardView
// ──────────────────────────────────────────────

/** Props for the standard card component. */
export type QCardViewProps = QCardViewBaseProps & {
	/** Main title displayed in the card. */
	readonly title?: string

	/** Subtitle displayed below the title. */
	readonly subtitle?: string

	/** Source URL for the card image. */
	readonly imgSrc?: string

	/** Alt text for the card image. */
	readonly imgAlt?: string

	/** Shape of the image displayed in the card. */
	readonly imageShape?: ImageShape

	/** Placeholder background color when no image is provided. */
	readonly colorPlaceholder?: string

	/** If true, the card shows a loading state. */
	readonly loading?: boolean
}

// ──────────────────────────────────────────────
// QInsertCard
// ──────────────────────────────────────────────

/** Props for a card used to insert new items. */
export type QInsertCardProps = QCardViewBaseProps & {
	/** Style variant of the insert card. */
	readonly variant?: InsertCardStyle

	/** Plural name of the table where the insert action will occur. */
	readonly tableName?: string

	/** Image URL for the insert card. */
	readonly src?: string

	/** Custom text labels for the insert card. */
	readonly texts?: Texts
}

// ──────────────────────────────────────────────
// QCards – Config & Props
// ──────────────────────────────────────────────

/** Props for the card grid layout. */
export type QCardGridProps = {
	/** Alignment of the cards inside the container. */
	readonly containerAlignment?: 'start' | 'center'

	/** Grid mode: fixed card width or column-based layout. */
	readonly gridMode?: 'fixed' | 'columns'
}

/** Props for the main QCards component. */
export type QCardsProps = QCardGridProps & {
	/** Unique identifier for the cards container. */
	readonly id?: string

	/** Additional CSS classes to apply to the cards container. */
	readonly class?: string

	/** Card variant/subtype. */
	readonly subtype?: Subtype

	/** List of cards to display. */
	readonly cards?: readonly MappedRow[]

	// Actions
	/** Alignment of action buttons. */
	readonly actionsAlignment?: 'start' | 'end'

	/** Placement of actions within the card. */
	readonly actionsPlacement?: 'header' | 'footer'

	/** Style of action buttons. */
	readonly actionsStyle?: 'dropdown' | 'inline' | 'mixed'

	// Visuals
	/** Alignment of the card content. */
	readonly contentAlignment?: ContentAlignment

	/** Shape of the card images. */
	readonly imageShape?: ImageShape

	/** Amount of scale applied on hover. */
	readonly hoverScaleAmount?: string | number

	/** Size of the card. */
	readonly size?: Size

	// Insert Card
	/** Target for custom follow-up links. */
	readonly customFollowupTarget?: 'self' | 'blank'

	/** If true, a custom insert card is displayed. */
	readonly customInsertCard?: boolean

	/** Style of the custom insert card. */
	readonly customInsertCardStyle?: InsertCardStyle

	// Display
	/** Display mode: grid or carousel. */
	readonly displayMode?: 'grid' | 'carousel'

	/** Whether to show column titles. */
	readonly showColumnTitles?: boolean

	/** Whether to show empty column titles. */
	readonly showEmptyColumnTitles?: boolean

	// Config / State
	/** List configuration for the cards. */
	readonly listConfig?: ListConfig

	/** Custom texts for labels and placeholders. */
	readonly texts?: Texts

	/** If true, the cards are read-only. */
	readonly readonly?: boolean

	/** If true, a loading state is displayed. */
	readonly loading?: boolean
}

// ──────────────────────────────────────────────
// Internal / Utility Types
// ──────────────────────────────────────────────

/** Internal representation of a card with props and mapped data. */
export type Card = {
	/** Unique card identifier. */
	readonly id: string

	/** Props used to render the card. */
	readonly props: QCardViewProps

	/** Mapped data associated with the card. */
	readonly mappedValue: MappedRow
}
