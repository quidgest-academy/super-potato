<template>
	<div
		v-show="isVisible"
		:id="id"
		:style="style"
		:class="['c-groupbox', { 'zone-no-border': noBorder }, $attrs.class]">
		<div
			v-if="label"
			class="c-groupbox__title"
			:help-id="id"
			:label-id="id">
			<div :style="labelStyle">
				{{ label }}
			</div>
		</div>
		<div class="form-flow">
			<slot></slot>
		</div>
	</div>
</template>

<script>
	export default {
		name: 'QGroupBoxContainer',

		inheritAttrs: false,

		props: {
			/**
			 * Component id.
			 */
			id: {
				type: String,
				default: null
			},

			/**
			 * Container label.
			 */
			label: {
				type: String,
				default: null
			},

			/**
			 * Groupbox label alignment. Either "left", "center" or "right".
			 */
			labelAlign: {
				type: String,
				default: 'left'
			},

			/**
			 * Custom style attributes.
			 */
			style: {
				type: String,
				default: null
			},

			/**
			 * The if the container is visible, false otherwise.
			 */
			isVisible: {
				type: Boolean,
				default: true
			},

			/**
			 * True if the container shuld be borderless, false otherwise.
			 */
			noBorder: {
				type: Boolean,
				default: false
			}
		},

		expose: [],

		computed: {
			labelStyle() {
				return {
					'text-align': this.labelAlign
				}
			}
		}
	}
</script>
