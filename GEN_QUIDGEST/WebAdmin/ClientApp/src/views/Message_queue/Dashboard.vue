<template>
<div id="message_queue_dashboard_container">
	<row>
		<br />
		<div class="title-container--with-badge">
			<q-label for="dashchart" :label="Resources.MENSAGENS53948" />
			<data-system-badge
				:title="Resources.SISTEMA_DE_DADOS_ATU09110" />
		</div>
		<div ref="plot_area" style="width:100%; height:520px;"></div>
	</row>
</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import 'jquery.flot';
	import 'jquery.flot/jquery.flot.time';

	export default {
		name: 'message_queue_dashboard',
		mixins: [reusableMixin],
		data: function () {
			return {
				plotCtrl: {
					data: {},
					options: {
						series: {
							bars: {
								show: true,
								barWidth: 872000
							},
							stack: true
						},
						xaxes: [{
							mode: 'time'
						}],
						yaxes: [{
							min: 0
						},],

						grid: {
							hoverable: true //IMPORTANT! this is needed for tooltip to work
							, borderWidth: 2,
							backgroundColor: { colors: ["#ffffff", "#EDF5FF"] }
						},
						colors: ["#4682B4", "#228B22"],
						tooltip: true,
						tooltipOpts: {
							content: "%x => %y.0 Msg",
							xDateFormat: "%y-%m-%d %h:%M"
						}
					}
				}
			};
		},
		computed: {
		},
		methods: {
			fetchData: function () {
				var vm = this;
				QUtils.log("Fetch data - Message Queue - Dashboard");
				QUtils.FetchData(QUtils.apiActionURL('MessageQueue', 'GetChartData')).done(function (data) {
					QUtils.log("Fetch data - OK (Message Queue - Dashboard)", data);
					vm.plotCtrl.data = data;
					$.plot($(vm.$refs.plot_area), vm.plotCtrl.data, vm.plotCtrl.options);
				});
			}
		},
		mounted() {
			// Ler dados
			this.fetchData();
		}
	};
</script>
