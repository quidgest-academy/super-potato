<template>
	<br />
	<h4>Timeline Component</h4>

	<div
		class="container-fluid"
		style="flex: 1 1 auto; padding: 44px; background-color: #eeeeee">
		<!-- Timeline-1 -->
		<q-row-container is-large>
			<q-control-wrapper class="row-line-group">
				<q-timeline
					style="background-color: #fff"
					v-bind="yearlyTimelineData"
					@form-popup="formAction" />
			</q-control-wrapper>
		</q-row-container>

		<br />

		<!-- Timeline-2 -->
		<q-row-container is-large>
			<q-control-wrapper class="row-line-group">
				<q-timeline
					style="background-color: #fff"
					v-bind="monthlyTimelineData" />
			</q-control-wrapper>
		</q-row-container>

		<br />

		<!-- Timeline-3 -->
		<q-row-container is-large>
			<q-control-wrapper class="row-line-group">
				<q-timeline
					style="background-color: #fff"
					v-bind="dailyTimelineData" />
			</q-control-wrapper>
		</q-row-container>

		<br />

		<!-- Timeline-4 -->
		<q-row-container is-large>
			<q-control-wrapper class="row-line-group">
				<q-timeline
					style="background-color: #fff"
					v-bind="weeklyTimelineData" />
			</q-control-wrapper>
		</q-row-container>
	</div>
</template>

<script>
	import fakeData from './Timeline.mock'

	export default {
		name: 'QTimelineContainer',

		docsfile: './docs/timeline/QTimeline.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				yearlyTimelineData: fakeData.simpleUsage().yearlyTimeline,
				monthlyTimelineData: fakeData.simpleUsage().monthlyTimeline,
				dailyTimelineData: fakeData.simpleUsage().dailyTimeline,
				weeklyTimelineData: fakeData.simpleUsage().weeklyTimeline,
				listShow: false
			}
		},

		methods: fakeData.simpleUsageMethods
	}
</script>
