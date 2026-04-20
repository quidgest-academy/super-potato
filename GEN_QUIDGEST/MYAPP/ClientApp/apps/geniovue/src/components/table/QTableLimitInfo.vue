<template>
	<q-button
		:id="limitsButtonId"
		:title="texts.limitsButtonTitle">
		<q-icon icon="information" />
	</q-button>

	<!-- BEGIN: Popup of limits -->
	<q-popover :anchor="`#${limitsButtonId}`">
		<span>
			{{ texts.limitsListTitlePrepend }} <b>{{ tableNamePlural }}</b> {{ texts.limitsListTitleAppend }}:

			<template
				v-for="(limit, index) in limits"
				:key="index">
				<br />

				<span data-testid="limit-info">
					<!--
						DB: (menu list) Area
						SC: (menu list) Selection with Conditions
						SL: (menu list) Sub-area Logic (Boolean-int)
						SA: (menu list) Sub-area Array (Enumeration)
						AC: (menu list) Array Choice (Enumeration)
						HM: (menu list) History Selection
						SH: (menu list) History Manipulation
						A: (table list) Area
						F: (table list) Fixed
						N: (table list) Fixed (new)
						H: (table list) History
						EPH: User EPH
					-->
					<template
						v-if="
							(limit.Type === 'DB' ||
								limit.Type === 'SC' ||
								limit.Type === 'SL' ||
								limit.Type === 'SA' ||
								limit.Type === 'AC' ||
								limit.Type === 'HM' ||
								limit.Type === 'SH' ||
								limit.Type === 'A' ||
								limit.Type === 'F' ||
								limit.Type === 'N' ||
								limit.Type === 'H' ||
								limit.Type === 'EPH') &&
								!(limit.ApplyOnlyIfExists && limit.Value === '')
						">
						{{ limit.Area }} {{ separatorAreaField(limit.Area, limit.Field) }} {{ limit.Field }}
						{{ limit.OperatorType !== '' ? limit.OperatorType : ': ' }} <b>{{ limit.Value }}</b>
					</template>
					<!--
						DC: (menu list) N:N
						V: (table list) Form control N:N
					-->
					<template v-else-if="(limit.Type === 'DC' || limit.Type === 'V') && !(limit.ApplyOnlyIfExists && limit.Value === '')">
						{{ limit.AreaN }} {{ separatorAreaField(limit.AreaN, limit.FieldN) }} {{ limit.FieldN }}
						-&gt;
						{{ limit.Area }} {{ separatorAreaField(limit.Area, limit.Field) }} {{ limit.Field }}: <b>{{ limit.Value }}</b>
					</template>
					<!--
						E: Between dates
					-->
					<template v-else-if="limit.Type === 'E' && !(limit.ApplyOnlyIfExists && limit.ValueToCompare === '')">
						<b>
							{{ limit.Area }} {{ separatorAreaField(limit.Area, limit.Field) }} <i>{{ limit.Field }}</i>
						</b>
						&lt;=
						<b>
							{{ limit.AreaToCompare }} {{ separatorAreaField(limit.AreaToCompare, limit.FieldToCompare) }}
							<i>{{ limit.FieldToCompare }}</i>
						</b>
						:
						{{ limit.ValueToCompare !== '' ? limit.ValueToCompare : emptyTextDisplay }}
						&lt;=
						<b>
							{{ limit.AreaN }} {{ separatorAreaField(limit.AreaN, limit.FieldN) }} <i>{{ limit.FieldN }}</i>
						</b>
					</template>
					<!--
						C: Field
					-->
					<template v-else-if="limit.Type === 'C' && !(limit.ApplyOnlyIfExists && limit.ValueN === '')">
						<b>
							{{ limit.AreaN }} {{ separatorAreaField(limit.AreaN, limit.FieldN) }} <i>{{ limit.FieldN }}</i>
						</b>
						:
						{{ limit.ValueN }}
					</template>
					<!--
						SE: Cross-boundary selection
					-->
					<template v-else-if="limit.Type === 'SE'">
						{{ limit.ValueMin }}
						&lt;=
						<b>
							{{ limit.Area }} {{ separatorAreaField(limit.Area, limit.Field) }} <i>{{ limit.Field }}</i>
						</b>
						&lt;=
						{{ limit.ValueMax }}
					</template>
					<!--
						SU: Threshold selection
					-->
					<template v-else-if="limit.Type === 'SU'">
						<b>
							{{ limit.Area }} {{ separatorAreaField(limit.Area, limit.Field) }} <i>{{ limit.Field }}</i>
						</b>
						{{ ' ' + limit.OperatorThreshold + ' ' }}
						{{ limit.Value }}
					</template>
					<!--
						DM: Multiple selection
					-->
					<template v-else-if="limit.Type === 'DM'">
						&#123;
						<b>{{ limit.AreaPlural }}</b>
						&#125;
					</template>
					<!--
						AFILTER: Filter by Area
					-->
					<template v-else-if="limit.Type === 'AFILTER'">
						&#35;
						<b>{{ limit.AreaPlural }}</b>
						(>= 1)
					</template>
					<!--
						OVERRQ: Manual routine
					-->
					<template v-else-if="limit.Type === 'OVERRQ'">
						<b>{{ limit.ManualHTMLText }}</b>
					</template>
				</span>
			</template>
		</span>
	</q-popover>
	<!-- END: Popup of limits -->
</template>

<script>
	export default {
		name: 'QTableLimitInfo',

		props: {
			/**
			 * Array of limit information objects describing constraints applied to the table data.
			 */
			limits: {
				type: Array,
				default: () => []
			},

			/**
			 * Localized text strings to be used within the limits information component.
			 */
			texts: {
				type: Object,
				required: true
			},

			/**
			 * The table ID.
			 */
			tableId: {
				type: String,
				default: ''
			},

			/**
			 * The singular name of the table for use in generating limit-related text descriptions.
			 */
			tableTitle: {
				type: String,
				default: ''
			},

			/**
			 * The plural name of the table for use within limit information messages where multiple instances are discussed.
			 */
			tableNamePlural: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data() {
			return {
				emptyTextDisplay: `<${this.texts.emptyText}>`
			}
		},

		computed: {
			limitsButtonId()
			{
				return `${this.tableId}-limit-btn`
			}
		},

		methods: {
			/**
			 * Separator for area and field in string
			 * @param area {String}
			 * @param field {String}
			 * @returns String
			 */
			separatorAreaField(area, field) {
				return area === '' || field === ''
					? ''
					: ' -> '
			}
		}
	}
</script>
