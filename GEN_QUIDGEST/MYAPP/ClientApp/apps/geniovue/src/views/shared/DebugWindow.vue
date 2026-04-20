<template>
	<teleport
		v-if="isReady"
		to="#q-modal-form-debug-body">
		<div class="content q-debug-window">
			<q-row-container>
				<q-control-wrapper>
					<base-input-structure id="debug-window-tabs">
						<q-tab-container
							id="debug-window-tabs"
							:selected-tab="tabContainer.selectedTab"
							:tabs-list="tabContainer.tabs"
							@tab-changed="changeTab">
							<section v-show="tabContainer.selectedTab === 1">
								<q-row-container>
									<q-control-wrapper class="control-join-group">
										<q-multi-check-boxes-input
											v-model="selectedTypes"
											:options="eventOptions" />
									</q-control-wrapper>
									<q-control-wrapper class="control-join-group float-right">
										<q-button
											label="Copy to Clipboard"
											title="Copy to Clipboard"
											@click="dumpEvents">
											<q-icon icon="copy" />
										</q-button>
									</q-control-wrapper>
								</q-row-container>
								<q-row-container>
									<q-control-wrapper>
										<table class="c-table">
											<thead class="c-table__head">
												<tr>
													<th></th>
													<th>Timestamp</th>
													<th>Type</th>
													<th>Origin</th>
													<th>Message</th>
													<th>Info</th>
												</tr>
											</thead>
											<tbody class="c-table__body">
												<template
													v-for="traceEvent in filteredEvents"
													:key="traceEvent.uid">
													<tr
														@click.stop.prevent="toggleDetails(traceEvent.uid)"
														class="c-table__row">
														<td>
															<button
																type="button"
																class="btn-popover"
																title="Print to Console"
																@click.stop.prevent="showFullEventData(traceEvent)">
																<q-icon icon="help" />
															</button>
														</td>
														<td>{{ formatTimestamp(traceEvent.timestamp) }}</td>
														<td>{{ traceEvent.type }}</td>
														<td>{{ traceEvent.origin }}</td>
														<td>{{ truncateString(traceEvent.message, 50) }}</td>
														<td>{{ getEventInfo(traceEvent) }}</td>
													</tr>
													<tr
														class="c-table__row"
														v-if="detailsIsVisible(traceEvent.uid)">
														<td colspan="6">
															<q-control-wrapper class="debug-window__event--details">
																<table class="c-table">
																	<tbody class="c-table__body">
																		<tr class="c-table__row">
																			<td>Message:</td>
																			<td>{{ traceEvent.message }}</td>
																		</tr>
																		<template
																			v-if="
																				traceEvent.type === TraceEventType.REQUEST ||
																					traceEvent.type === TraceEventType.RESPONSE
																			">
																			<tr class="c-table__row">
																				<td>Request type:</td>
																				<td>{{ traceEvent.requestType }}</td>
																			</tr>
																			<tr class="c-table__row">
																				<td>Request URL:</td>
																				<td>{{ traceEvent.requestUrl }}</td>
																			</tr>
																			<tr class="c-table__row">
																				<td>Request params:</td>
																				<td>{{ traceEvent.requestParams }}</td>
																			</tr>
																			<tr class="c-table__row">
																				<td>Request data:</td>
																				<td>{{ traceEvent.requestData }}</td>
																			</tr>
																		</template>
																		<template v-if="traceEvent.type === TraceEventType.RESPONSE">
																			<tr class="c-table__row">
																				<td>Response status:</td>
																				<td>{{ traceEvent.responseStatus }}</td>
																			</tr>
																			<tr class="c-table__row">
																				<td>Response data:</td>
																				<td>{{ traceEvent.responseData }}</td>
																			</tr>
																			<tr class="c-table__row">
																				<td>Time:</td>
																				<td>{{ traceEvent.time }} ms</td>
																			</tr>
																		</template>
																		<tr class="c-table__row">
																			<td>Context data:</td>
																			<td>{{ traceEvent.contextData }}</td>
																		</tr>
																		<tr class="c-table__row">
																			<td>Call Stack:</td>
																			<td>{{ traceEvent.callStack }}</td>
																		</tr>
																	</tbody>
																</table>
															</q-control-wrapper>
														</td>
													</tr>
												</template>
											</tbody>
										</table>
									</q-control-wrapper>
								</q-row-container>
							</section>
							<section v-show="tabContainer.selectedTab === 2">
								<q-row-container v-if="!_isEmpty(eventGroups)">
									<q-control-wrapper>
										<q-button
											label="Copy to Clipboard"
											title="Copy to Clipboard"
											@click="dumpEventGroups">
											<q-icon icon="copy" />
										</q-button>
									</q-control-wrapper>
								</q-row-container>
								<q-row-container
									v-for="(traceEventGroup, groupId) in eventGroups"
									:key="groupId">
									<q-control-wrapper>
										<button
											type="button"
											class="btn-popover"
											title="Print to Console"
											@click.stop.prevent="showFullEventData(traceEventGroup)">
											<q-icon icon="help" />
										</button>
										{{ joinEventsMessage(traceEventGroup) }}
										<hr />
									</q-control-wrapper>
								</q-row-container>
							</section>
						</q-tab-container>
					</base-input-structure>
				</q-control-wrapper>
			</q-row-container>
		</div>
	</teleport>

	<teleport
		v-if="isReady"
		to="#q-modal-form-debug-footer">
		<div class="actions">
			<q-button
				:label="texts.cancel"
				:title="texts.cancel"
				@click="closePopup">
				<q-icon icon="cancel" />
			</q-button>
		</div>
	</teleport>
</template>

<script>
	import { computed } from 'vue'
	import { mapState, mapActions } from 'pinia'

	import _map from 'lodash-es/map'
	import _capitalize from 'lodash-es/capitalize'
	import _truncate from 'lodash-es/truncate'
	import _toString from 'lodash-es/toString'
	import _join from 'lodash-es/join'
	import _isEmpty from 'lodash-es/isEmpty'

	import { dateDisplay } from '@quidgest/clientapp/utils/genericFunctions'
	import { removeModal } from '@/utils/layout'

	import { useTracingDataStore } from '@quidgest/clientapp/stores'
	import { useGenericDataStore } from '@quidgest/clientapp/stores'

	import { TraceEventType } from '@quidgest/clientapp/telemetry'
	import hardcodedTexts from '@/hardcodedTexts.js'

	export default {
		name: 'DebugWindow',

		emits: ['close'],

		expose: [],

		data()
		{
			return {
				// Tracing events
				TraceEventType,
				eventOptions: _map(TraceEventType, (eventType) => {
					return { key: eventType, value: _capitalize(eventType) }
				}),
				selectedTypes: [],

				// Events details
				visibleDetails: {},

				// Tabs
				tabContainer: {
					selectedTab: 1,
					tabs: [
						{
							id: 1,
							name: 'Events tracker',
							label: 'Events tracker',
							disabled: false,
							isVisible: true
						},
						{
							id: 2,
							name: 'Errors group',
							label: 'Errors group',
							disabled: false,
							isVisible: true
						}
					]
				},

				isReady: false,

				texts: {
					cancel: computed(() => this.Resources[hardcodedTexts.cancel])
				}
			}
		},

		created()
		{
			// eslint-disable-next-line no-console
			console.warn('Debug window')

			const props = {
				title: 'Debug',
				size: 'large'
			}

			const modalProps = {
				id: 'form-debug',
				isActive: true
			}

			this.setModal(props, modalProps)
		},

		mounted()
		{
			this.isReady = true
		},

		computed: {
			...mapState(useTracingDataStore, [
				'eventTracker'
			]),

			filteredEvents()
			{
				return this.eventTracker.getEventsOfTypes(this.selectedTypes)
			},

			eventGroups()
			{
				return this.eventTracker.getEventsByGroup()
			}
		},

		methods: {
			...mapActions(useGenericDataStore, [
				'setModal'
			]),

			removeModal,

			_isEmpty,

			closePopup()
			{
				this.$emit('close')
				this.removeModal('form-debug')
			},

			changeTab(tabId)
			{
				this.tabContainer.selectedTab = tabId
			},

			formatTimestamp(timestamp)
			{
				try
				{
					return dateDisplay(timestamp, 'd MMM HH:mm:ss.SSS')
				}
				catch
				{
					return timestamp || '-'
				}
			},

			truncateString(inputString, maxSize)
			{
				return _truncate(inputString, {
					length: maxSize,
					omission: ' [...]',
					separator: ' '
				})
			},

			/**
			 * Function to obtain relevant information
			 * @param {*} event
			 */
			getEventInfo(event)
			{
				let info = ''
				if (event.type === TraceEventType.REQUEST || event.type === TraceEventType.RESPONSE)
				{
					info += _toString(event.requestUrl)

					if (event.type === TraceEventType.RESPONSE)
						info += ` (${event.time} ms)`
				}
				return info
			},

			showFullEventData(traceEvent)
			{
				// eslint-disable-next-line no-console
				console.log('Debug - event data:', traceEvent)
			},

			joinEventsMessage(traceEventsGroup)
			{
				return _join(
					_map(traceEventsGroup, (event) => this.truncateString(event.message || '', 150)),
					' -> '
				)
			},

			detailsIsVisible(uid)
			{
				return this.visibleDetails[uid] === true
			},

			toggleDetails(uid)
			{
				this.visibleDetails[uid] = !this.visibleDetails[uid]
			},

			/**
			 * Copies a JSON object to the clipboard.
			 *
			 * @param {Object} jsonObj - The JSON object to be copied.
			 */
			copyToClipboard(jsonObj)
			{
				try
				{
					// Convert the JSON object to a string.
					// The second argument "null" and third argument "2" are used for formatting.
					const jsonString = JSON.stringify(jsonObj, null, 2)

					// Use the Clipboard API to copy the text.
					navigator.clipboard
						.writeText(jsonString)
						.then(() => {
							// eslint-disable-next-line no-console
							console.log('JSON copied to clipboard')
						})
						.catch((err) => {
							// eslint-disable-next-line no-console
							console.error('Failed to copy text: ', err)
						})
				}
				catch (e)
				{
					// eslint-disable-next-line no-console
					console.error('Debug dump', e)
				}
			},

			dumpEvents()
			{
				this.copyToClipboard(this.filteredEvents)
			},

			dumpEventGroups()
			{
				this.copyToClipboard(this.eventGroups)
			}
		}
	}
</script>
