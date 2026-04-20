<template>
	<div
		:class="tlItem.TipoTimeLine ==='S'
			? 'c-simple_timeline__item-content'
			: 'c-timeline__item-content'"
		:style="contentStyle">
		<div
			:class="tlItem.TipoTimeLine ==='S'
				? 'c-simple_timeline__item-header'
				: 'c-timeline__item-header'"
			:style="tlItem.TipoTimeLine === 'S' ? simpleHeaderStyle : headerStyle">
			<span
				:class="tlItem.TipoTimeLine ==='S'
					? 'c-simple_timeline__item-datetime e-badge e-badge--primary'
					: 'c-timeline__item-datetime e-badge e-badge--dark'"
				:style="textColor">
				{{ formatedDated }}
			</span>
			<span
				:class="{
					'c-simple_timeline__item-title': tlItem.TipoTimeLine === 'S',
					'c-timeline__item-title': tlItem.TipoTimeLine !== 'S'}"
				:style="textColor">
				{{ tlItem.Texto }}
			</span>
		</div>
		<div :class="tlItem.TipoTimeLine === 'S' ? 'c-simple_timeline__item-text' : 'c-timeline__item-text'">
			<!--Iterate all row fields (not image kind)-->
			<span
				v-for="col in tlItem.Columns"
				:key="col.order"
				:class="tlItem.TipoTimeLine === 'S' ? 'c-simple_timeline__item-field' : 'c-timeline__item-field'">
				<template v-if="col.Valor !== ''">
					<i
						v-if="col.Icone"
						:class="[col.Icone, 'q-icon', 'mr-1']" />
					<strong v-if="col.Titulo">{{ col.Titulo }}</strong>
					&nbsp; {{ col.Valor }}
					<br />
				</template>
			</span>
			<!--Images columns in row-->
			<div v-if="tlItem.ImagesColumns.length > 0">
				<div
					class="mt-1 mb-1"
					v-for="img in tlItem.ImagesColumns"
					:key="img">
					<img
						v-if="img.Image"
						:src="img.Image" />
				</div>
			</div>
			<!-- Popup Data-Form Btn -->
			<a
				v-if="tlItem.Url"
				data-testid="popup-btn"
				class="q-button q-button--primary q-button--size-small mt-1"
				role="button"
				href="#"
				:style="headerStyle"
				@click.stop.prevent="$emit('form-popup', tlItem)">
				<q-icon
					icon="more-items"
					:style="popupBtnStyle" />
			</a>
		</div>
	</div>
</template>

<script>
	import genericFunctions from '@quidgest/clientapp/utils/genericFunctions'

	export default {
		name: 'QTimelineItem',

		emits: ['form-popup'],

		props: {
			/**
			 * The timeline item object containing details to be displayed within the timeline.
			 */
			tlItem: {
				type: Object,
				required: true
			},

			/**
			 * The format used for displaying date and time fields.
			 */
			dateTimeFormat: {
				type: String,
				required: true
			}
		},

		expose: [],

		computed: {
			/**
			 * Determines whether to apply dynamic coloring to certain parts of the timeline item.
			 */
			applyColor()
			{
				return this.tlItem.Style === 'D'
			},

			/**
			 * Sets the text color for the timeline item title, according to the background
			 * color of the item for better readability.
			 */
			textColor()
			{
				if (!this.applyColor)
					return {}

				return {
					color: genericFunctions.getReadableTextColor(this.tlItem.Background)
				}
			},

			/**
			 * Sets the style for the form-popup button icon according to the background
			 * color of the timeline item.
			 */
			popupBtnStyle()
			{
				if (!this.applyColor)
					return {}

				return {
					fill: genericFunctions.getReadableTextColor(this.tlItem.Background)
				}
			},

			/**
			 * Sets the background and border colors of the timeline header according to the
			 * tlItem's background color.
			 */
			headerStyle()
			{
				if (!this.applyColor)
					return {}

				return {
					backgroundColor: this.tlItem.Background,
					borderColor: this.tlItem.Background
				}
			},

			/**
			 * Sets the border color of the timeline item's content according to the tlItem's
			 * background color.
			 */
			contentStyle()
			{
				if (!this.applyColor)
					return {}

				return {
					borderColor: this.tlItem.Background
				}
			},

			/**
			 * Formats the date field of the timeline item, using the provided dateTimeFormat,
			 * to a human-readable date string.
			 */
			formatedDated()
			{
				const newDate = new Date(this.tlItem.Data)
				return genericFunctions.dateDisplay(newDate, this.dateTimeFormat)
			}
		}
	}
</script>
