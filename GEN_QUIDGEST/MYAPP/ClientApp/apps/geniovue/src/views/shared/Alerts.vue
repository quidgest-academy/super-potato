<template>
	<div class="c-sidebar--container__section">
		<div class="c-sidebar__subtitle">
			<q-icon icon="notifications" />
			<span>{{ texts.alerts }}</span>
		</div>

		<div class="c-sidebar__alert-buttoncontainer">
			<span class="c-sidebar__alert-counter"> {{ texts.total }}: {{ alerts.length }} </span>

			<q-button-group borderless>
				<q-button
					:title="texts.refresh"
					@click="loadAlerts">
					<q-icon icon="reset" />
				</q-button>
				<q-button
					:title="texts.dismiss"
					:disabled="noAlerts"
					@click="clearAlerts">
					<q-icon icon="delete" />
				</q-button>
			</q-button-group>
		</div>

		<div class="c-sidebar__alert-container">
			<q-info-message
				v-for="alert in alerts"
				:key="alert.id"
				:id="alert.id"
				:title="replaceTag(alert.isResource ? Resources[alert.title] : alert.title, alert.tag, alert.count)"
				:text="replaceTag(alert.isResource ? Resources[alert.description] : alert.description, alert.tag, alert.count)"
				:type="alert.type"
				:icon="getIcon(alert.type)"
				:is-dismissible="alert.isDismissible"
				:on-click-target="alert.target"
				@message-dismissed="dismissAlert(alert.id)"
				@navigate-to="$emit('navigate-to', $event)" />

			<img
				v-if="noAlerts"
				:src="`${$app.resourcesPath}no-alerts.png?v=${$app.genio.buildVersion}`" />
		</div>
	</div>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'
	import LayoutHandlers from '@/mixins/layoutHandlers.js'

	import QInfoMessage from '@/components/QInfoMessage.vue'

	export default {
		name: 'QAlerts',

		emits: [
			'fetch-alerts',
			'clear-alerts',
			'dismiss-alert',
			'navigate-to'
		],

		components: {
			QInfoMessage
		},

		mixins: [
			LayoutHandlers
		],

		props: {
			/**
			 * An array of alerts to display in the sidebar.
			 */
			alerts: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		data()
		{
			return {
				texts: {
					refresh: computed(() => this.Resources[hardcodedTexts.refresh]),
					dismiss: computed(() => this.Resources[hardcodedTexts.dismiss]),
					alerts: computed(() => this.Resources[hardcodedTexts.alerts]),
					total: computed(() => this.Resources[hardcodedTexts.total])
				}
			}
		},

		mounted()
		{
			this.loadAlerts()
		},

		computed: {
			/**
			 * Computed property to check if there are no alerts present.
			 */
			noAlerts()
			{
				return this.alerts.length === 0
			}
		},

		methods: {
			/**
			 * Emits a fetch-alerts event to load alerts.
			 */
			loadAlerts()
			{
				this.$emit('fetch-alerts')
			},

			/**
			 * Emits a clear-alerts event to clear all displayed alerts.
			 */
			clearAlerts()
			{
				this.$emit('clear-alerts')
			},

			/**
			 * Returns the appropriate icon based on the alert type.
			 * @param {String} type - The type of alert.
			 * @returns {String} The corresponding icon name.
			 */
			getIcon(type)
			{
				switch (type)
				{
					case 'info':
						return 'information'
					case 'success':
						return 'success'
					case 'warning':
						return 'alert'
					case 'danger':
						return 'error'
					default:
						return ''
				}
			},

			/**
			 * Emits a dismiss-alert event for the alert with the specified id.
			 * @param {String} alertId - The unique identifier of the alert.
			 */
			dismissAlert(alertId)
			{
				this.$emit('dismiss-alert', alertId)
			},

			/**
			 * Replaces a placeholder tag within the text with a count value.
			 * @param {String} text - The text containing a placeholder tag.
			 * @param {String} tag - The tag to be replaced.
			 * @param {Number} count - The count value to replace the tag with.
			 * @returns {String} The modified text with the tag replaced by the count value.
			 */
			replaceTag(text, tag, count)
			{
				if (typeof text === 'string')
					return text.replace(tag, count)
				return ''
			}
		}
	}
</script>
