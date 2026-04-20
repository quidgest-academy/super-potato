<template>
	<template v-if="!multipleValues">
		<!-- If a custom component is provided -->
		<component
			v-if="component"
			:is="component"
			v-bind="customComponentAttrs(value)" />
		<!-- If the value is empty -->
		<div
			v-else-if="isEmptyValue(value)"
			:data-field-value="true">
			--
		</div>
		<!-- Otherwise, display the value in a styled element or fragment -->
		<component
			v-else
			:is="basicDisplayElement"
			:style="basicDisplayElementStyle"
			:class="basicDisplayElementClasses">
			<span :data-field-value="true">
				{{ getCellValue(value) }}
			</span>
		</component>
	</template>
	<template v-else>
		<!-- Multiple values mode -->
		<div
			v-if="isEmptyValue(valuesToDisplay)"
			:data-field-value="true">
			--
		</div>
		<template v-else>
			<div class="q-badge-container">
				<template
					v-for="(item, index) in valuesToDisplay"
					:key="index">
					<!-- If tooltip is required -->
					<q-tooltip
						v-if="isFormatedValue(item)"
						:text="getCellTitle(item)"
						:anchor="multipleValuesRef[index]?.$el">
						<template #anchor>
							<q-badge
								:color="backgroundColor || 'primary'"
								:ref="(el) => (multipleValuesRef[index] = el)">
								<component
									v-if="component"
									:is="component"
									v-bind="customComponentAttrs(item)" />
								<div
									v-else-if="isEmptyValue(item)"
									:data-field-value="true">
									--
								</div>
								<span
									v-else
									:data-field-value="true">
									{{ getCellValue(item) }}
								</span>
							</q-badge>
						</template>
					</q-tooltip>
					<!-- If tooltip is not required -->
					<q-badge
						v-else
						:color="backgroundColor || 'primary'"
						:ref="(el) => (multipleValuesRef[index] = el)">
						<component
							v-if="component"
							:is="component"
							v-bind="customComponentAttrs(item)" />
						<div
							v-else-if="isEmptyValue(item)"
							:data-field-value="true">
							--
						</div>
						<span
							v-else
							:data-field-value="true">
							{{ item }}
						</span>
					</q-badge>
				</template>
			</div>
		</template>
	</template>
</template>

<script>
	import { ref } from 'vue'
	import _isObjectLike from 'lodash-es/isObjectLike'
	import _isArray from 'lodash-es/isArray'
	import _some from 'lodash-es/some'

	export default {
		name: 'QRenderData',

		inheritAttrs: false,

		props: {
			/**
			 * Optional component name for Vue's dynamic component. If provided, the specified component is rendered.
			 */
			component: {
				type: String,
				default: ''
			},

			/**
			 * The data value to be displayed. Can be any type.
			 */
			value: {
				type: null,
				default: () => ({})
			},

			/**
			 * A background color to apply to the basic display element (span). If specified, the span with a background color is used.
			 */
			backgroundColor: {
				type: String,
				default: ''
			}
		},

		expose: [],

		data()
		{
			return {
				/**
				 * References to elements in multiple values scenario.
				 */
				multipleValuesRef: ref({})
			}
		},

		computed: {
			/**
			 * Indicates whether multiple values should be displayed, rather than a single value.
			 */
			multipleValues()
			{
				return this.$attrs.options?.multipleValues ?? false
			},

			/**
			 * Ensures that the displayed values are always in an array form when multipleValues is true.
			 */
			valuesToDisplay()
			{
				return _isArray(this.value) ? this.value : [this.value]
			},

			/**
			 * Determines the basic display element based on whether a background color is provided.
			 * If a background color is provided, a span is used; otherwise, a fragment is used for unwrapping the content.
			 */
			basicDisplayElement()
			{
				return this.backgroundColor ? 'span' : 'v-fragment'
			},

			/**
			 * Returns the class list for the basic display element, applying e-badge if a background color is set.
			 */
			basicDisplayElementClasses()
			{
				return this.backgroundColor ? ['e-badge'] : []
			},

			/**
			 * Returns the inline style for the basic display element, setting the background color if one is provided.
			 */
			basicDisplayElementStyle()
			{
				return this.backgroundColor
					? { 'background-color': this.backgroundColor }
					: {}
			}
		},

		methods: {
			/**
			 * Checks if the value is empty. It analyses objects differently from primitive values.
			 * It returns true if the value is an object with all undefined, null, or empty values,
			 * or if the value is a primitive that is undefined, null, or empty.
			 * @param {any} value The data value to be displayed. Can be any type.
			 * @returns {boolean} True if empty, otherwise false.
			 */
			isEmptyValue(value)
			{
				if (_isObjectLike(value) && !_isArray(value))
					return !_some(value, (val) => !(val === undefined || val === null || val === ''))
				else if(_isArray(value))
					return value.length === 0

				return value === undefined || value === null || value === ''
			},

			/**
			 * Checks if the provided value is a formatted value object.
			 * A formatted value is identified by the constructor name "FormattedValueToDisplay".
			 * @param {any} value The value to check.
			 * @returns {boolean} True if the value is formatted, otherwise false.
			 */
			isFormatedValue(value)
			{
				// Without creating a direct dependency on genericFunctions, it will be checked whether the class with a formatted value was received.
				return this.multipleValues && _isObjectLike(value) && value.constructor?.name === "FormattedValueToDisplay"
			},

			/**
			 * Retrieves the displayable cell value. If the value is formatted, returns its 'value' property.
			 * Otherwise, returns the value as-is.
			 * @param {any} item The cell value. In the case of multiple text value, it will have an object with properties { value, originalValue }
			 * @returns {any} The displayable value.
			 */
			getCellValue(item)
			{
				return this.isFormatedValue(item) ? item.value : item
			},

			/**
			 * Retrieves the original value for tooltip display. If not available, returns null.
			 * @param {any} value The cell value. In the case of multiple text value, it will have an object with properties { value, originalValue }
			 * @returns {string|null} The original value or null if not present.
			 */
			getCellTitle(value)
			{
				return value?.originalValue ?? null
			},

			/**
			 * Compute attributes to be passed to the custom component, merging props, attrs, and the current value.
			 * @param {any} item The data value to be displayed. Can be any type.
			 * @returns {Object} The merged attributes object.
			 */
			customComponentAttrs(item)
			{
				const value = this.getCellValue(item)
				return {
					...this.$props,
					value,
					...this.$attrs,
					/**
					 * For cases where the specific data type component has its own application in the background,
					 * in multi-value the badge is the one that will have a color instead of the rendering value component.
					 */
					backgroundColor: this.multipleValues ? null : this.backgroundColor
				}
			}
		}
	}
</script>
