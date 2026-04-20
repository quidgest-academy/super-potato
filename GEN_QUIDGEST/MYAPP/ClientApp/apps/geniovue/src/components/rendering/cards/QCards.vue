<template>
	<q-container
		:id="props.id"
		fluid
		data-testid="cards"
		role="rowgroup">
		<!-- Layout Component: Grid or Carousel -->
		<component
			v-if="shouldRenderContent"
			:is="LayoutComponent"
			:grid-mode="props.gridMode"
			:container-alignment="props.containerAlignment">
			<!-- Loading State -->
			<template v-if="props.loading">
				<q-col
					v-for="skeleton in 3"
					:key="skeleton"
					v-bind="colSize">
					<q-card-view
						loading
						v-bind="baseCardProps">
						<template #title></template>
						<template #subtitle></template>
						<template #text></template>
						<template #image></template>
					</q-card-view>
				</q-col>
			</template>
			<template v-else>
				<!-- Insert Card -->
				<q-col
					v-if="shouldShowInsertCard"
					v-bind="colSize">
					<q-insert-card
						:variant="props.customInsertCardStyle"
						:table-name="props.listConfig.tableNamePlural"
						:src="`${props.listConfig.resourcesPath}insert_card.png`"
						:texts="props.texts"
						:subtype="props.subtype"
						:size="props.size"
						:content-alignment="props.contentAlignment"
						:hover-scale-amount="props.hoverScaleAmount?.toString().slice(-1)"
						@click="rowAction(insertAction!)" />
				</q-col>
				<!-- Cards -->
				<q-col
					v-for="card in cards"
					:key="card.id"
					v-bind="colSize">
					<q-card-view
						v-bind="card.props"
						@click="onCardClick(card)">
						<template #title>
							<q-render-data
								:component="card.mappedValue.title?.source?.component"
								:value="card.mappedValue.title?.value"
								:background-color="card.mappedValue.title?.bgColor"
								:options="card.mappedValue.title?.source?.componentOptions"
								:resources-path="listConfig.resourcesPath" />
						</template>

						<template
							v-if="card.mappedValue.subtitle"
							#subtitle>
							<q-render-data
								:component="card.mappedValue.subtitle?.source?.component"
								:value="card.mappedValue.subtitle?.value"
								:background-color="card.mappedValue.subtitle?.bgColor"
								:options="card.mappedValue.subtitle?.source?.componentOptions"
								:resources-path="listConfig.resourcesPath" />
						</template>

						<template #[`content.append`]>
							<p
								v-for="text in card.mappedValue.text"
								:key="text.value"
								class="q-card-view__text"
								:data-field="`${text.source?.area}.${text.source?.field}`"
								role="cell">
								<span
									v-if="props.showColumnTitles"
									class="label">
									{{ text.source?.label }}:
								</span>
								<q-render-data
									:component="text.source?.component"
									:value="text.value"
									:background-color="text?.bgColor"
									:options="text.source?.componentOptions || text.source"
									:resources-path="listConfig.resourcesPath" />
							</p>
						</template>

						<template
							v-if="hasRowActions"
							#[actionsPlacement]>
							<q-row
								role="cell"
								:gutter="0"
								:justify="props.actionsAlignment">
								<q-action-list
									dropdown-size="small"
									placement="bottom-start"
									variant="outlined"
									:groups="rowActionGroups"
									:items="getRowActions(card)"
									:readonly="readonly"
									:title="texts.selectOptions"
									@click="rowOptionSelect($event, card)" />
							</q-row>
						</template>
					</q-card-view>
				</q-col>
			</template>
		</component>
		<!-- Empty State -->
		<q-row
			v-else
			role="cell"
			:justify="props.containerAlignment">
			<div class="q-cards-empty-container">
				<img
					v-if="listConfig.resourcesPath"
					:src="`${listConfig.resourcesPath}empty_card_container.png`"
					:alt="texts.noRecordsText" />
				<h5>{{ texts.emptyText }}</h5>
			</div>
		</q-row>
	</q-container>
</template>

<script setup lang="ts">
	// Constants
	import { DEFAULT_TEXTS } from './constants'

	// Components
	import QRenderData from '@/components/rendering/QRenderData.vue'
	import { QActionList } from '@quidgest/clientapp/components'
	import { QCol, QContainer, QRow } from '@quidgest/ui/components'
	import QCardView from './QCardView.vue'
	import QInsertCard from './QInsertCard.vue'

	// Types
	import type { QColProps } from '@quidgest/ui/esm/components/QGrid/types.js'
	import type { Card, QCardsProps } from './types'

	// Utils
	import { computed, defineAsyncComponent, watch } from 'vue'
	import { btnHasPermission } from '@quidgest/clientapp/utils/genericFunctions'

	const props = withDefaults(defineProps<QCardsProps>(), {
		cards: () => [],
		actionsAlignment: 'start',
		actionsPlacement: 'footer',
		actionsStyle: 'dropdown',
		customFollowupTarget: 'self',
		displayMode: 'grid',
		gridMode: 'fixed',
		containerAlignment: 'start',
		hoverScaleAmount: '1.00',
		listConfig: () => ({}),
		texts: () => DEFAULT_TEXTS
	})

	const emit = defineEmits<{
		/** Emitted when a card becomes visible. */
		(e: 'update:visible', id: string): void
		/** Emitted when a row action is triggered. */
		(e: 'row-action', action: object): void
	}>()

	/** Lazily loaded components. */
	const QCardsGridLayout = defineAsyncComponent(() => import('./QCardsGridLayout.vue'))
	const QCardsCarouselLayout = defineAsyncComponent(() => import('./QCardsCarouselLayout.vue'))

	/** Selected layout component based on display mode. */
	const LayoutComponent = computed(() =>
		props.displayMode === 'carousel' ? QCardsCarouselLayout : QCardsGridLayout
	)

	/** Common props passed to all cards. */
	const baseCardProps = computed(() => ({
		subtype: props.subtype,
		size: props.size,
		contentAlignment: props.contentAlignment,
		imageShape: props.imageShape,
		hoverScaleAmount: props.hoverScaleAmount?.toString().slice(-1)
	}))

	/** Normalized cards to be rendered. */
	const cards = computed<Card[]>(() =>
		props.cards.map((val) => ({
			id: val.rowKey,
			props: {
				imgSrc: val.image?.previewData ?? val.image?.value,
				colorPlaceholder: val.image?.dominantColor,
				...baseCardProps.value
			},
			mappedValue: val
		}))
	)

	/** Insert action configuration, if available. */
	const insertAction = computed(() =>
		props.listConfig.generalActions?.find(
			(act) =>
				typeof act === 'object' &&
				act !== null &&
				'id' in act &&
				(act as { id: string }).id === 'insert'
		)
	)

	/** Column size depending on display and grid modes. */
	const colSize = computed<QColProps>(() => {
		if (props.displayMode === 'carousel') {
			return { cols: 12 }
		}

		// Fixed grid

		const isSmall = props.size === 'small'
		const isHorizontal = props.subtype === 'card-horizontal'

		if (props.gridMode === 'fixed') {
			return { cols: 'auto' }
		}

		// Flexible grid (columns)

		if (isHorizontal) {
			return {
				xl: 3,
				lg: 4,
				md: 6,
				cols: 12
			}
		}

		return {
			xxl: isSmall ? 1 : 2,
			xl: isSmall ? 2 : 3,
			lg: isSmall ? 3 : 4,
			md: isSmall ? 4 : 6,
			sm: isSmall ? 6 : 12,
			cols: isSmall ? 6 : 12
		}
	})

	/** Whether to show the "insert" card. */
	const shouldShowInsertCard = computed(() => !!insertAction.value && props.customInsertCard)

	/** Whether row actions exist for this list. */
	const hasRowActions = computed(
		() =>
			(props.listConfig.crudActions?.length ?? 0) > 0 ||
			(props.listConfig.customActions?.length ?? 0) > 0
	)

	/** Whether to render content (skeleton, insert card, or cards). */
	const shouldRenderContent = computed(
		() => props.loading || shouldShowInsertCard.value || cards.value.length > 0
	)

	/** The list of action groups. */
	const rowActionGroups = computed(
		() => {
			const display = props.actionsStyle
			return [
				{ id: 'custom', display },
				{ id: 'crud', display }
			]
		}
	)

	/**
	 * Gets the actions of the specified card.
	 *
	 * @param card - The card from which to get the actions.
	 */
	function getRowActions(card: Card): object[] {
		return [
			...(props.listConfig.customActions ?? []).map((act) => ({
				...act,
				key: act.id,
				label: act.title,
				group: 'custom',
				isVisible: card.mappedValue.actionVisibility?.[act.id] ?? act.isVisible,
				disabled: card.mappedValue.actionDisability?.[act.id] ?? act.disabled
			})),
			...(props.listConfig.crudActions ?? []).map((act) => ({
				...act,
				key: act.id,
				label: act.title,
				group: 'crud',
				disabled: !btnHasPermission(card.mappedValue.btnPermission, act.id)
			}))
		]
	}

	/**
	 * Handles click on a card.
	 * Opens a follow-up link if defined, or triggers the row click action.
	 *
	 * @param card - The card that was clicked.
	 */
	function onCardClick(card: Card): void {
		if (card.mappedValue.customFollowup !== undefined) {
			const url = card.mappedValue.customFollowup.value
			if (url) {
				const customFollowupTarget =
					card.mappedValue.customFollowupTarget?.value || props.customFollowupTarget
				window.open(url, `_${customFollowupTarget}`)
			}
		} else if (
			props.listConfig.rowClickAction &&
			Object.keys(props.listConfig.rowClickAction).length > 0
		) {
			rowAction(props.listConfig.rowClickAction, card)
		}
	}

	/**
	 * Emits a row action event for the given action and card.
	 *
	 * @param action - The action to perform.
	 * @param card - The optional card triggering the action.
	 */
	function rowAction(action: object, card?: Card): void {
		emit('row-action', { ...action, rowKey: card?.id })
	}

	/**
	 * Emits a row action event for the given key and card.
	 *
	 * @param key - The identifier of the triggered action.
	 * @param card - The card triggering the action.
	 */
	function rowOptionSelect(key: string, card: Card): void {
		const action = getRowActions(card).find((e) => e.key === key)
		rowAction(action!, card)
	}

	/**
	 * Watch for card changes to detect visibility updates.
	 * Emits `update:visible` when a card image source changes.
	 */
	watch(
		cards,
		(newVal, oldVal) => {
			newVal.forEach((card) => {
				const oldCard = oldVal?.find((s) => s.id === card.id)
				if (!oldCard || oldCard.props.imgSrc !== card.props.imgSrc) {
					emit('update:visible', card.id)
				}
			})
		},
		{ deep: true, immediate: true }
	)
</script>
