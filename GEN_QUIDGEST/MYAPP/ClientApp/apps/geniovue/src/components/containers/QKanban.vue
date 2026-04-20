<template>
	<div
		:id="id"
		:class="['q-kanban', $attrs.class]">
		<template
			v-for="column in sortedColumns"
			:key="column.id">
			<div
				class="q-kanban-column"
				:draggable="false"
				@dragover.prevent
				@dragenter.stop.prevent="() => handleDragEnter(column)"
				@drop.stop.prevent="() => handleDragDrop(column)">
				<!-- HEADER -->
				<q-kanban-header
					v-bind="column"
					:id="column.id"
					:title="column.title"
					:texts="texts"
					@add:card="addCard"
				>
					<slot
						name="column"
						:column="column.value" />
				</q-kanban-header>

				<!-- CONTENT -->
				<div
					v-for="card in cardsByColumn(column.id)"
					:key="card.id"
					:class="[
						'q-kanban-item',
						{ 'q-kanban-item__ghost': isDragging(card) },
					]"
					:draggable="true"
					@dragover.prevent
					@dragstart="(event) => handleDragStart(event, card)"
					@dragenter.stop.prevent="() => handleDragEnter(column)"
					@drop.stop.prevent="() => handleDragDrop(column)"
				>
					<slot :item="card.value" />
				</div>

				<!-- EMPTY SPACE -->
				<div
					class="q-kanban-column__empty"
					:draggable="false"
				></div>
			</div>
		</template>
	</div>
</template>

<script>
	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	const DEFAULT_TEXTS = {
		addItem: 'Insert',
		columnPlaceholder: 'Column Name'
	}

	export default {
		name: 'QKanban',
		expose: [],
		props: {
			/**
			 * The id of the kanban
			 */
			id: {
				type: String,
				default: undefined,
			},

			/**
			 * The list of columns
			 */
			columns: {
				type: Array,
				required: true,
			},

			/**
			 * The list of card
			 */
			cards: {
				type: Array,
				required: true,
			},

			/**
			 * Localization and customization of textual content within the table component.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			},
		},

		emits: ['click:add', 'update:position'],

		data() {
			return {
				draggingCard: null,
				ghostElement: null,
			}
		},

		computed: {
			sortedColumns() {
				return this.columns.slice().sort((a, b) => a.order - b.order)
			},
			updatedCards() {
				return this.cards.map((card) =>
					card.id === this.draggingCard?.id ? this.draggingCard : card
				)
			},
			sortedCardsByColumn() {
				return this.columns.reduce((columns, column) => {
					columns[column.id] = this.updatedCards
						.filter((card) => card.column === column.id)
						.sort((a, b) =>
							a.order && b.order
								? a.order - b.order
								: a.id.localeCompare(b.id)
						)
					return columns
				}, {})
			},
		},

		mounted() {
			// Prevent dropping outside allowed areas
			window.addEventListener(
				'dragover',
				this.onDragover,
				false
			)
			window.addEventListener(
				'drop',
				this.onDrop,
				false
			)
		},

		beforeUnmount()
		{
			window.removeEventListener('dragover', this.onDragover)
			window.removeEventListener('drop', this.onDrop)
		},

		methods: {
			onDragover(event) {
				event.preventDefault()
			},

			onDrop(event) {
				event.preventDefault()
				this.draggingCard = null
			},

			cardsByColumn(columnId) {
				return this.sortedCardsByColumn[columnId] || []
			},
			addCard(columnId) {
				this.$emit('click:add', columnId)
			},
			isDragging(card) {
				return card.id === this.draggingCard?.id
			},
			handleDragStart(event, card) {
				this.draggingCard = card

				// Set up the drag ghost
				const draggedItem = event.target
				if (!draggedItem) return
				const slotContent =
					draggedItem.querySelector('[slot]') || draggedItem

				this.ghostElement = slotContent.cloneNode(true)
				if (!this.ghostElement) return
				this.ghostElement.style.width =
					draggedItem?.getBoundingClientRect()?.width.toString() + 'px'
				this.ghostElement.style.position = 'fixed'
				this.ghostElement.style.top = '-100vh'
				document.body.appendChild(this.ghostElement)
				event.dataTransfer?.setDragImage(this.ghostElement, 0, 0)
			},
			handleDragEnter(column) {
				if (this.draggingCard && this.draggingCard.column !== column.id)
					this.draggingCard.column = column.id
			},
			handleDragDrop(column) {
				if (this.ghostElement) {
					this.ghostElement.remove()
					this.ghostElement = null
				}

				const card = { ...this.draggingCard }
				this.draggingCard = null

				if (!card) return

				card.column = column.id
				this.$emit('update:position', card)
			},
		},
	}
</script>
