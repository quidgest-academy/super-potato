<template>
	<q-card
		:class="classes"
		width="block"
		borderless
		v-on="{
			...($parent.$attrs.onClick ? { click: onClick } : {}),
		}"
	>
		<template #header>
			<!-- CARD HEADER -->
			<div class="q-kanban-card__header">
				<div
					v-if="card.type"
					class="q-kanban-card__type">
					<q-icon :icon="taskIcon" />
					{{ card.type }}
				</div>
			</div>
		</template>

		<q-button
			v-if="actionItems.length > 0 && rowActionDisplay !== 'inline'"
			class="q-kanban-card__actions-btn"
			ref="actionsBtn"
			variant="text"
			@click="cardActions"
		>
			<q-icon icon="actions" />
		</q-button>

		<q-overlay
			class="q-kanban-card__actions"
			:model-value="actionsOverlay"
			:anchor="$refs.actionsBtn?.$el"
			placement="right-start"
			@leave="onOverlayLeave"
		>
			<q-list :items="actionItems">
				<template #item="{ item }">
					<div
						class="q-kanban-card__actions-item"
						@click.stop.prevent="() => executeCardAction(item)"
					>
						<q-icon :icon="item.icon" />
						<span>{{ item.label }}</span>
					</div>
				</template>
			</q-list>
		</q-overlay>

		<!-- CARD CONTENT -->
		<div class="q-kanban-card__content">
			<div class="q-kanban-card__name">
				{{ card.title }}
			</div>
			<div
				v-if="card.description"
				class="q-kanban-card__extra">
				{{ card.description }}
			</div>

			<div
				v-if="card.additionalInformation"
				class="q-kanban-card__content-spacer"
			/>

			<div
				v-for="additional in card.additionalInformation"
				:key="additional"
				class="q-kanban-card__extra"
			>
				{{ additional.value }}
			</div>

			<div
				v-if="card.status"
				class="q-kanban-card__content-spacer" />

			<div
				v-if="card.status"
				class="q-kanban-card__status">
				<div />
				{{ card.status }}
			</div>
		</div>

		<!-- CARD FOOTER -->
		<template #footer>
			<div
				v-if="card.date || card.author"
				class="q-kanban-card__footer">
				<div class="q-kanban-card__date">
					<q-icon icon="date" />
					{{ card.date ? card.date : 'DD/MM' }}
				</div>
				<div class="q-kanban-card__author">
					{{ card.author }}
					<div class="q-kanban-card__author-icon">
						<q-icon icon="user_kanban" />
					</div>
				</div>
			</div>
			<div
				v-if="rowActionDisplay === 'inline'"
				class="q-kanban-card__actions-inline">
				<q-button
					v-for="item in actionItems"
					variant="text"
					:key="item"
					class="q-kanban-card__actions-item"
					@click="() => executeCardAction(item)"
				>
					<q-icon :icon="item.icon" />
				</q-button>
			</div>
		</template>
	</q-card>
</template>

<script>
	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		saveText: 'Save',
		viewText: 'View',
		deleteText: 'Delete',
		duplicateText: 'Duplicate',
		titlePlaceholder: 'Insert title (required)',
		setDate: 'Set due date',
		setAuthor: 'Assign',
		addItem: '+ Add item',
	}

	export default {
		name: 'KanbanCard',

		expose: [],

		emits: ['click', 'click:action'],

		props: {
			/**
			 * The card to show
			 */
			card: {
				type: Object,
				required: true,
			},

			/**
			 * The crud actions of the card
			 */
			crudActions: {
				type: Array,
				required: true,
			},

			/**
			 * The display for the card actions
			 */
			rowActionDisplay: {
				type: String,
				default: 'dropdown'
			}
		},

		data() {
			return {
				texts: DEFAULT_TEXTS,
				actionsOverlay: false,
			}
		},

		computed: {
			classes() {
				// const type =
				// 	this.card.type !== 'Task'
				// 		? `q-kanban-card--${this.card.type?.replace(/\s+/g, '-').toLowerCase()}`
				// 		: undefined
				// const status = `q-kanban-card--${this.card.status
				// 	?.replace(/\s+/g, '-')
				// 	?.toLowerCase()}`

				return [
					'q-kanban-card q-card--clickable',
					// type,
					// status,
					{
						'q-kanban-card--warning': this.card.warning,
					},
				]
			},
			taskIcon() {
				// TO DO: add icons to Genio itself
				const icons = {
					Task: 'task',
					Issue: 'incidentes-internal',
					Request: 'changes-external',
					Support: 'support',
				}
				return icons[this.card.type] || icons.Task
			},

			actionItems() {
				const allActions = []
				this.crudActions.forEach((action) => {
					allActions.push({
						key: action.id,
						label: action.title,
						group: 'crud-actions',
						icon: action.icon.icon,
					})
				})

				return allActions
			},
		},
		methods: {
			onClick() {
				this.$emit('click', this.card)
			},

			cardActions() {
				this.actionsOverlay = true
			},

			onOverlayLeave() {
				this.actionsOverlay = false
			},

			executeCardAction(action) {
				this.$emit('click:action', { action: action.key, card: this.card })
			},
		},
	}
</script>
