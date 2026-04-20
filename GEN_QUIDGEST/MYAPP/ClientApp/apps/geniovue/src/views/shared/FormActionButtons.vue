<template>
	<div
		v-if="isModesVisible"
		class="c-sidebar--container__section">
		<span class="c-sidebar__subtitle">
			<q-icon icon="more-items" />

			{{ texts.formMode }}
		</span>

		<ul
			id="sidebar-modes-tree-view"
			class="nav nav-pills nav-sidebar flex-column n-sidebar__nav mt-2">
			<template
				v-for="btn in buttonsList.modesButtons"
				:key="btn.id">
				<li
					v-if="btn.isActive && btn.isVisible"
					class="nav-item">
					<q-button
						block
						:id="`sidebar-${btn.id}`"
						:variant="btn.variant"
						:label="btn.text"
						:disabled="btn.disabled"
						@click="btn.action">
						<q-icon
							v-if="btn.icon"
							v-bind="btn.icon" />
					</q-button>
				</li>
			</template>
		</ul>
	</div>

	<div
		v-if="isActionsVisible"
		class="c-sidebar--container__section">
		<span class="c-sidebar__subtitle">
			<q-icon icon="new-suggestion" />

			{{ texts.formActions }}
		</span>

		<ul
			id="sidebar-actions-tree-view"
			class="nav nav-pills nav-sidebar flex-column n-sidebar__nav mt-2">
			<template
				v-for="btn in buttonsList.actionButtons"
				:key="btn.id">
				<li
					v-if="btn.isActive && btn.isVisible"
					class="nav-item">
					<q-button
						block
						:id="`sidebar-${btn.id}`"
						:label="btn.text"
						:disabled="btn.disabled"
						@click="btn.action">
						<q-icon
							v-if="btn.icon"
							v-bind="btn.icon" />
					</q-button>
				</li>
			</template>
		</ul>
	</div>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts'

	export default {
		name: 'FormActionButtons',

		props: {
			/**
			 * Data function that defines reactive properties for localized texts.
			 */
			buttonsList: {
				type: Object,
				required: true
			}
		},

		expose: [],

		data()
		{
			return {
				texts: {
					formActions: computed(() => this.Resources[hardcodedTexts.formActions]),
					formMode: computed(() => this.Resources[hardcodedTexts.formMode])
				}
			}
		},

		computed: {
			/**
			 * True if the "modes" section should be visible, false otherwise.
			 */
			isModesVisible()
			{
				return this.checkVisibility(this.buttonsList.modesButtons)
			},

			/**
			 * True if the "actions" section should be visible, false otherwise.
			 */
			isActionsVisible()
			{
				return this.checkVisibility(this.buttonsList.actionButtons)
			}
		},

		methods: {
			/**
			 * Checks the visibility of buttons within a given list, returning true if there is at least one active and visible button.
			 * @param {Object} btnsObj - The object containing the button configurations.
			 * @returns {boolean} - True if visible buttons exist, otherwise false.
			 */
			checkVisibility(btnsObj)
			{
				if (!btnsObj || Object.keys(btnsObj).length < 1)
					return false

				for (const i in btnsObj)
					if (btnsObj[i].isActive && btnsObj[i].isVisible)
						return true

				return false
			}
		}
	}
</script>
