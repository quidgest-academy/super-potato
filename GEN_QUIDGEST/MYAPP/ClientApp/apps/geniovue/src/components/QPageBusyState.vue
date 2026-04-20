<template>
	<transition name="loading-state">
		<div
			v-if="isVisible"
			class="q-page-busy-state">
			<span class="q-page-busy-state__overlay"></span>

			<div class="q-page-busy-state__content">
				<!-- <img class="loading__content-logo" :src="`${resourcesPath}Q_icon.png`" /> -->

				<q-spinner-loader />

				<ul
					v-if="processesWithMessages.length"
					class="q-page-busy-state__message-list">
					<li
						v-for="proc in processesWithMessages"
						:key="proc.id">
						<span class="q-page-busy-state__message-text">
							{{ proc.message }}
						</span>
					</li>
				</ul>
			</div>
		</div>
	</transition>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	export default {
		name: 'QPageBusyState',

		props: {
			/**
			 * The path to the resources.
			 */
			resourcesPath: String,

			/**
			 * The list of running processes.
			 */
			processes: {
				type: Array,
				default: () => []
			}
		},

		expose: [],

		computed: {
			isVisible()
			{
				return !_isEmpty(this.processes)
			},

			processesWithMessages()
			{
				return this.processes.filter((proc) => !_isEmpty(proc.message))
			}
		}
	}
</script>
