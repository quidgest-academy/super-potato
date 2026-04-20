<template>
	<div class="q-cards-carousel__container">
		<q-row
			ref="container"
			class="q-cards-carousel"
			@scroll.passive="onScroll">
			<slot></slot>
		</q-row>

		<div
			v-if="showPrevBtn"
			class="q-cards-carousel__btn-container q-cards-carousel__btn-container-prev">
			<q-button
				class="q-cards-carousel__btn"
				borderless
				elevated
				pill
				@click="prev">
				<q-icon icon="step-back" />
			</q-button>
		</div>

		<div
			v-if="showNextBtn"
			class="q-cards-carousel__btn-container q-cards-carousel__btn-container-next">
			<q-button
				class="q-cards-carousel__btn"
				borderless
				elevated
				pill
				@click="next">
				<q-icon icon="step-forward" />
			</q-button>
		</div>
	</div>
</template>

<script setup lang="ts">
	import { QButton, QIcon, QRow } from '@quidgest/ui/components'
	import { nextTick, onBeforeUnmount, onMounted, ref, useTemplateRef } from 'vue'

	/** Delta value to determine if we are close enough to the edges. */
	const delta = 2.5
	/** Whether to show the previous button. */
	const showPrevBtn = ref(false)
	/** Whether to show the next button. */
	const showNextBtn = ref(false)

	/** Timeout ID for updating controls. */
	let updateId: number | undefined = undefined
	/** Reference to the container element. */
	const containerRef = useTemplateRef('container')
	/** ResizeObserver to react to container size/content changes. */
	let resizeObserver: ResizeObserver | undefined

	// --- Lifecycle hooks ---

	onMounted(() => {
		// Initial measurement
		updateControls()

		// Observe container resizing or content changes
		const el = containerRef.value?.$el
		if (el) {
			resizeObserver = new ResizeObserver(() => {
				updateControls()
			})
			resizeObserver.observe(el)
		}
	})

	onBeforeUnmount(() => {
		clearTimeout(updateId)
		resizeObserver?.disconnect()
	})

	/**
	 * Checks if there are previous cards to scroll to.
	 *
	 * @return True if there are previous cards, false otherwise.
	 */
	function hasPrev(): boolean {
		const container = containerRef.value?.$el
		if (!container) return false
		if (container.scrollLeft === 0) return false

		const firstCard = container.children[0]
		const containerVWLeft = container.getBoundingClientRect().left ?? 0
		const firstCardLeft = firstCard?.getBoundingClientRect()?.left ?? 0

		return Math.abs(containerVWLeft - firstCardLeft) >= delta
	}

	/**
	 * Checks if there are next cards to scroll to.
	 *
	 * @return True if there are next cards, false otherwise.
	 */
	function hasNext(): boolean {
		const container = containerRef.value?.$el
		if (!container) return false
		return container.scrollWidth > container.scrollLeft + container.clientWidth + delta
	}

	/**
	 * Scrolls to the previous set of cards.
	 */
	function prev() {
		const container = containerRef.value?.$el
		if (!container) return

		const left = container.getBoundingClientRect().left
		const x = left - container.clientWidth - delta
		const card = findPrevCard(x)

		if (card) {
			const offset = card.getBoundingClientRect().left - left
			scrollTo(container.scrollLeft + offset)
			return
		}

		scrollTo(container.scrollLeft - container.clientWidth)
	}

	/**
	 * Scrolls to the next set of cards.
	 */
	function next() {
		const container = containerRef.value?.$el
		if (!container) return

		const left = container.getBoundingClientRect().left
		const x = left + container.clientWidth + delta
		const card = findNextCard(x)

		if (card) {
			const offset = card.getBoundingClientRect().left - left
			if (offset > delta) {
				scrollTo(container.scrollLeft + offset)
				return
			}
		}

		scrollTo(container.scrollLeft + container.clientWidth)
	}

	/**
	 * Finds the previous card that is at or after the given x-coordinate.
	 *
	 * @param x - The x-coordinate to check against.
	 * @return The found card element or undefined if none found.
	 */
	function findPrevCard(x: number) {
		const cards = containerRef.value?.$el.children
		if (!cards?.length) return undefined

		for (let i = 0; i < cards.length; i++) {
			const card = cards[i].getBoundingClientRect()
			if (card.left <= x && x <= card.right) return cards[i]
			else if (x <= card.left) return cards[i]
		}

		return undefined
	}

	/**
	 * Finds the next card that is at or after the given x-coordinate.
	 *
	 * @param x - The x-coordinate to check against.
	 * @return The found card element or undefined if none found.
	 */
	function findNextCard(x: number) {
		const cards = containerRef.value?.$el.children
		if (!cards?.length) return undefined

		for (let i = 0; i < cards.length; i++) {
			const card = cards[i].getBoundingClientRect()
			if (card.right <= x) continue
			else if (card.left <= x) return cards[i]
			if (x <= card.left) return cards[i]
		}

		return undefined
	}

	/**
	 * Smoothly scrolls the container to the specified offset.
	 *
	 * @param offset - The horizontal offset to scroll to.
	 */
	function scrollTo(offset: number) {
		containerRef.value?.$el.scrollTo({ left: offset, behavior: 'smooth' })
	}

	/**
	 * Updates the visibility of the previous and next buttons
	 * based on the current scroll position.
	 */
	function updateControls() {
		nextTick(() => {
			// Ensure measurement after paint
			requestAnimationFrame(() => {
				showPrevBtn.value = hasPrev()
				showNextBtn.value = hasNext()
			})
		})
	}

	/**
	 * Handles the scroll event of the container.
	 * Debounces the updateControls function to avoid excessive calls.
	 */
	function onScroll() {
		window.clearTimeout(updateId)
		updateId = window.setTimeout(updateControls, 100)
	}

	defineOptions({
		inheritAttrs: false
	})
</script>
