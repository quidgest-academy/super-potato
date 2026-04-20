<template>
	<div
		class="q-note"
		role="note">
		<div class="q-note__header">
			<div class="q-note__title">
				<div
					v-if="!_isEmpty(note.createdBy)"
					class="q-note__title-from">
					{{ texts.from }} {{ note.createdBy }}
				</div>
				<div
					v-if="!_isEmpty(note.dest)"
					class="q-note__title-for">
					{{ texts.to }} {{ note.dest }}
				</div>
				<div class="q-note__title-date">{{ formattedDate }}</div>
			</div>

			<q-button
				size="small"
				borderless
				@click="deleteNote">
				<q-icon icon="delete" />
			</q-button>
		</div>

		<div class="mt-2">
			<div class="q-note__text">
				{{ note.text }}
			</div>
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'
	import _isEmpty from 'lodash-es/isEmpty'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import { useGenericDataStore } from '@quidgest/clientapp/stores'

	import { dateDisplay } from '@quidgest/clientapp/utils/genericFunctions'

	export default {
		name: 'QNote',

		emits: ['delete'],

		inheritAttrs: false,

		props: {
			/**
			 * The unique identifier for this note in the list of notes.
			 */
			id: {
				type: [Number, String],
				default: -1
			},

			/**
			 * Note data to render.
			 */
			note: {
				type: Object
			}
		},

		expose: [],

		data()
		{
			return {
				texts: {
					from: computed(() => this.Resources[hardcodedTexts.from]),
					to: computed(() => this.Resources[hardcodedTexts.to])
				},
			}
		},

		computed: {
			/**
			 * Note creation date formatted using the user's configured date format.
			 *
			 * @returns {string}
			 */
			formattedDate()
			{
				if (!(this.note?.date instanceof Date)) return ''

				const genericDataStore = useGenericDataStore()
				return dateDisplay(this.note.date, genericDataStore.dateFormat.date)
			}
		},

		methods: {
			_isEmpty,

			/**
			 * Emits the delete event for this note.
			 */
			deleteNote()
			{
				this.$emit('delete', this.id)
			}
		}
	}
</script>
