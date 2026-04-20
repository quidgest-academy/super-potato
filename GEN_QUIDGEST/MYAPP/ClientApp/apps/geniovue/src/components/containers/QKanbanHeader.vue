<template>
	<div class="q-kanban-column__header-wrapper">
		<!-- HEADER -->
		<div class="q-kanban-column__header">
			<!-- TITLE -->
			<div class="q-kanban-column__title">
				{{ title }}
			</div>

			<!-- TITLE SLOT -->
			<div
				v-if="$slots.default"
				class="q-kanban-column__subtitle">
				<slot />
			</div>
		</div>

		<!-- ADD NEW ITEM -->
		<q-button
			class="q-kanban__add"
			@click="addNew">
			{{ texts.addItem }}
		</q-button>
	</div>
</template>

<script>
	export default {
		name: 'QKanbanHeader',
		expose: [],
		props: {
			/**
			 * The id of the column/header
			 */
			id: {
				type: String,
				default: undefined,
			},

			/**
			 * The title to show
			 */
			title: {
				type: String,
				default: 'Column',
			},

			/**
			 * Localized text strings to be used within the component.
			 */
			texts: {
				type: Object,
				required: true
			},
		},
	
		emits: ['add:card'],
	
		methods: {
			addNew() {
				this.$emit('add:card', this.id)
			},
		},
	}
</script>
