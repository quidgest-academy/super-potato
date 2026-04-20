// Data
import { listConfig, cards } from './QCards.mock'

// Components
import QCards from '@/components/rendering/cards/QCards.vue'

// Types
import type { Meta, StoryObj } from '@storybook/vue3'

/**
 * `QCards` offers a simple API for rendering headings, text, images, icons, and other card content.
 */
const meta: Meta<typeof QCards> = {
	title: 'Views/Cards',
	component: QCards,
	tags: ['autodocs']
}

export default meta
type Story = StoryObj<typeof QCards>

// ──────────────────────────────────────────────
// Layouts
// ──────────────────────────────────────────────

/**
 * Displays a standard grid layout with multiple fixed-width cards.
 */
export const FixedWidthGrid: Story = {
	args: {
		cards: Array(3).fill(cards[0]),
		listConfig
	}
}

/**
 * Displays a flexible grid layout where columns adapt based on screen size.
 */
export const FlexibleGrid: Story = {
	args: {
		cards: Array(3).fill(cards[0]),
		gridMode: 'columns',
		listConfig
	}
}

/**
 * Centers the cards inside the grid layout.
 */
export const GridCentered: Story = {
	args: {
		cards: Array(3).fill(cards[0]),
		containerAlignment: 'center',
		listConfig
	}
}

/**
 * Renders the cards in a carousel layout for horizontal navigation.
 */
export const Carousel: Story = {
	args: {
		cards: Array(10).fill(cards[0]),
		displayMode: 'carousel',
		listConfig
	}
}

// ──────────────────────────────────────────────
// Sizing & Variants
// ──────────────────────────────────────────────

/**
 * Shows cards in a smaller, more compact variant.
 */
export const SmallCards: Story = {
	args: {
		cards: Array(4).fill(cards[0]),
		size: 'small',
		listConfig
	}
}

/**
 * Applies a card subtype, such as an image-top layout.
 */
export const WithSubtype: Story = {
	args: {
		subtype: 'card-img-top',
		cards: Array(3).fill(cards[0]),
		listConfig
	}
}

// ──────────────────────────────────────────────
// Visuals
// ──────────────────────────────────────────────

/**
 * Displays cards with circular images.
 */
export const WithCircularImages: Story = {
	args: {
		cards: Array(3).fill(cards[0]),
		imageShape: 'circular',
		listConfig
	}
}

/**
 * Demonstrates hover scaling effect on cards.
 */
export const WithHoverScale: Story = {
	args: {
		cards: Array(3).fill(cards[0]),
		hoverScaleAmount: '1.05',
		listConfig
	}
}

// ──────────────────────────────────────────────
// Insert Cards
// ──────────────────────────────────────────────

/**
 * Adds a custom insert card at the end of the list.
 */
export const InsertCard: Story = {
	args: {
		cards: Array(2).fill(cards[0]),
		customInsertCard: true,
		listConfig
	}
}

/**
 * Uses a primary styled insert card.
 */
export const InsertCardPrimary: Story = {
	args: {
		cards: Array(2).fill(cards[0]),
		customInsertCard: true,
		customInsertCardStyle: 'primary',
		listConfig
	}
}

/**
 * Uses an image-styled insert card.
 */
export const InsertCardImage: Story = {
	args: {
		cards: Array(2).fill(cards[0]),
		customInsertCard: true,
		customInsertCardStyle: 'image',
		listConfig
	}
}

// ──────────────────────────────────────────────
// Actions
// ──────────────────────────────────────────────

/**
 * Places inline actions in the card footer.
 */
export const ActionsInlineFooter: Story = {
	args: {
		cards: Array(3).fill(cards[0]),
		actionsPlacement: 'footer',
		actionsStyle: 'inline',
		listConfig
	}
}

/**
 * Places dropdown actions in the card header.
 */
export const ActionsDropdownHeader: Story = {
	args: {
		cards: Array(3).fill(cards[0]),
		actionsPlacement: 'header',
		actionsStyle: 'dropdown',
		listConfig
	}
}

/**
 * Aligns actions to the end of the card.
 */
export const ActionsEndAligned: Story = {
	args: {
		cards: Array(3).fill(cards[0]),
		actionsAlignment: 'end',
		listConfig
	}
}

// ──────────────────────────────────────────────
// States
// ──────────────────────────────────────────────

/**
 * Displays cards in a readonly (non-interactive) state.
 */
export const Readonly: Story = {
	args: {
		cards: Array(3).fill(cards[0]),
		readonly: true,
		listConfig
	}
}

/**
 * Shows a loading state with skeletons or placeholders.
 */
export const Loading: Story = {
	args: {
		loading: true
	}
}

/**
 * Displays the empty state when no cards are provided.
 */
export const Empty: Story = {
	args: {
		listConfig
	}
}
