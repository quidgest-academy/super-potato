<template>
	<div
		:class="[...classes, $attrs.class]"
		data-testid="q-card-view"
		:data-loading="loading"
		tabindex="0"
		@click.stop="onClick"
		@keydown.space.prevent="onClick"
		@keydown.enter.prevent="onClick">
		<div class="q-card-view__overlay">
			<!-- HEADER -->
			<div
				v-if="$slots.header || hasHeaderImage"
				class="q-card-view__header">
				<!-- Image wrapper -->
				<component
					:is="hasImageCropper ? 'div' : 'v-fragment'"
					v-if="(imgSrc || $slots.image) && hasHeaderImage"
					v-bind="hasImageCropper ? { class: imgCropperClasses, style: imgCropperStyle } : {}">
					<q-skeleton-loader
						v-if="loading"
						width="100%"
						height="100%" />

					<slot
						v-else
						name="image">
						<img
							class="q-card-view__img"
							decoding="async"
							loading="lazy"
							:src="imgSrc"
							:alt="imgAlt" />
					</slot>
				</component>

				<slot
					v-else
					name="header" />
			</div>

			<!-- BODY -->
			<div class="q-card-view__body">
				<div
					v-if="(imgSrc || $slots.image) && hasBodyImage"
					:class="imgCropperClasses"
					:style="imgCropperStyle">
					<q-skeleton-loader
						v-if="loading"
						width="100%"
						height="100%" />
					<slot
						v-else
						name="image">
						<img
							class="q-card-view__img"
							decoding="async"
							loading="lazy"
							:src="imgSrc"
							:alt="imgAlt" />
					</slot>
				</div>

				<div class="q-card-view__content">
					<slot name="content.prepend" />

					<h5
						v-if="$slots.title || title"
						class="q-card-view__title"
						role="cell">
						<q-skeleton-loader
							v-if="loading"
							type="text" />
						<template v-else-if="$slots.title">
							<slot name="title" />
						</template>
						<template v-else>
							{{ title }}
						</template>
					</h5>

					<p
						v-if="$slots.subtitle || subtitle"
						class="q-card-view__subtitle"
						role="cell">
						<q-skeleton-loader
							v-if="loading"
							type="text" />
						<template v-else-if="$slots.subtitle">
							<slot name="subtitle" />
						</template>
						<template v-else>
							{{ subtitle }}
						</template>
					</p>

					<slot name="content.append" />
				</div>
			</div>

			<!-- FOOTER -->
			<div
				v-if="$slots.footer"
				class="q-card-view__footer">
				<slot name="footer" />
			</div>
		</div>

		<!-- UNDERLAY -->
		<div
			class="q-card-view__underlay"
			:style="underlayStyle">
			<slot name="underlay" />
			<template v-if="hasUnderlayImage">
				<q-skeleton-loader v-if="loading" />
				<slot
					v-else
					name="image">
					<img
						class="q-card-view__img"
						decoding="async"
						loading="lazy"
						:src="imgSrc"
						:alt="imgAlt" />
				</slot>
			</template>
		</div>
	</div>
</template>

<script setup lang="ts">
	// Components
	import { QSkeletonLoader } from '@quidgest/ui/components'

	// Types
	import type { QCardViewProps } from './types'

	// Utils
	import { computed } from 'vue'

	const props = withDefaults(defineProps<QCardViewProps>(), {
		subtype: 'card',
		size: 'regular',
		contentAlignment: 'start',
		hoverScaleAmount: 1,
		imageShape: 'rectangular'
	})

	const emit = defineEmits<{
		(e: 'click', event: MouseEvent | KeyboardEvent): void
	}>()

	/**
	 * Dynamic CSS classes applied to the root element.
	 */
	const classes = computed(() => {
		const baseClass = 'q-card-view'
		const cls: string[] = [baseClass]

		const subtype = props.subtype.replace('card-', '')
		if (subtype && subtype !== 'card') cls.push(`${baseClass}--${subtype}`)

		if (props.size && props.size !== 'regular') cls.push(`${baseClass}--size-${props.size}`)
		if (+props.hoverScaleAmount > 1) cls.push(`${baseClass}--scale-${props.hoverScaleAmount}`)
		if (props.contentAlignment === 'center') cls.push(`${baseClass}--centered`)
		if (props.loading) cls.push(`${baseClass}--loading`)

		return cls
	})

	/**
	 * Whether the image should render in the header section.
	 */
	const hasHeaderImage = computed(() =>
		['card-img-top', 'card-img-thumbnail'].includes(props.subtype)
	)

	/**
	 * Whether the image should use a cropper wrapper.
	 */
	const hasImageCropper = computed(() => props.subtype !== 'card-img-thumbnail')

	/**
	 * Whether the image should render in the body section.
	 */
	const hasBodyImage = computed(
		() => !hasHeaderImage.value && props.subtype !== 'card-img-background'
	)

	/**
	 * Whether the image should render as an underlay background.
	 */
	const hasUnderlayImage = computed(() => props.subtype === 'card-img-background')

	/**
	 * Inline styles for the underlay background.
	 */
	const underlayStyle = computed(() => {
		const style: Record<string, string> = {}
		if (props.subtype !== 'card-img-background') return style
		if (props.colorPlaceholder) style['background-color'] = props.colorPlaceholder
		return style
	})

	/**
	 * CSS classes applied to the image cropper wrapper.
	 */
	const imgCropperClasses = computed(() => {
		const baseClass = 'q-card-view__img-cropper'
		const cls: string[] = [baseClass]
		if (props.imageShape && props.imageShape !== 'rectangular') {
			cls.push(`${baseClass}--${props.imageShape}`)
		}
		return cls
	})

	/**
	 * Inline styles applied to the image cropper wrapper.
	 */
	const imgCropperStyle = computed(() => {
		const style: Record<string, string> = {}
		if (props.colorPlaceholder) style['background-color'] = props.colorPlaceholder
		return style
	})

	/**
	 * Handles click events on the card and emits them upstream.
	 */
	function onClick(event: MouseEvent | KeyboardEvent) {
		emit('click', event)
	}
</script>
