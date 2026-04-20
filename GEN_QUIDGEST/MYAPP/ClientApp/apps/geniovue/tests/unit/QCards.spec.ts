// Components
import QCardView from '@/components/rendering/cards/QCardView.vue'

// Utils
import { describe, it, expect } from 'vitest'
import { render } from './utils'

describe('QCardView', () => {
	it('renders the card title when title prop is passed', () => {
		const title = 'Test Card Title'

		const { getByText } = render(QCardView, {
			props: {
				title
			}
		})

		expect(getByText(title)).toBeTruthy()
	})

	it('renders the card subtitle when subtitle prop is passed', () => {
		const subtitle = 'Test Card Subtitle'

		const { getByText } = render(QCardView, {
			props: {
				subtitle
			}
		})

		expect(getByText(subtitle)).toBeTruthy()
	})

	it('renders the card size based on the size prop', () => {
		const size = 'small'

		const { getByTestId } = render(QCardView, {
			props: {
				size
			}
		})

		expect(getByTestId('q-card-view').classList).toContain(`q-card-view--size-${size}`)
	})

	it('renders the card content aligned to center when contentAlignment prop is set to center', () => {
		const contentAlignment = 'center'

		const { getByTestId } = render(QCardView, {
			props: {
				contentAlignment
			}
		})

		expect(getByTestId('q-card-view').classList).toContain('q-card-view--centered')
	})
})
