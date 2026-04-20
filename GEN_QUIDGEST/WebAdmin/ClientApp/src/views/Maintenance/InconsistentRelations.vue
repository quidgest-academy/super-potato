<template>
	<div id="maintenance_data_quality_inconsistent_relations_container">
		<row>
			<q-collapsible
				class="q-collapsible--admin-default"
				:title="Resources[Model.IncoherenceTitle]"
				:subtitle="getIncoherenceText()"
				width="block">
				<q-row-container>
					<div v-if="Model.IncoherenceType == 'IncoherentRelation'">
						<q-control-wrapper
							v-if="Model.SelectLists"
							class="control-row-group">
							<base-input-structure
								class="i-text">
								<q-select
									v-if="Model.SelectLists"
									v-model="Model.RelationMode"
									item-value="Value"
									item-label="Text"
									:items="Model.SelectLists.RelationsMode"
									:label="Resources.TIPO_DE_PESQUISA13226"
									size="large" />
							</base-input-structure>
						</q-control-wrapper>
						<q-control-wrapper class="control-row-group">
							<base-input-structure
								class="i-text">
								<q-checkbox
									v-model="Model.NullsIsChecked"
									:label="Resources.CONSIDERAR_AS_CHAVES25518" />
							</base-input-structure>
						</q-control-wrapper>
					</div>
					<q-control-wrapper class="control-row-group">
						<base-input-structure
							class="i-text">
							<q-checkbox
								v-model="Model.ViewsIsChecked"
								:label="Resources.CONSIDERAR_AS_VIEWS07942" />
						</base-input-structure>
					</q-control-wrapper>

					<row class="footer-btn">
						<q-button
							:label="Resources.EXECUTAR_ATUALIZACAO61833"
							@click="DataQualityStart" />
						<data-system-badge
							:title="Resources.SISTEMA_DE_DADOS_ATU09110" />
					</row>

				</q-row-container>

				<row v-if="!isEmptyObject(Model.IncoherentRelations)">
					<hr />
					<row>
						<div class="alert alert--info">
							<div class="alert-header">
								<q-icon
									class="alert-icon"
									icon="information" />
								<label>
									<b>
										{{ Resources.ULTIMA_VERIFICACAO35305 }}&#58;
									</b>
								</label>
								{{ formatDate(Model.LastUpdate) }}
							</div>
							<div v-html="getIncoherentTitle()" />
						</div>
					</row>
					<qtable :rows="tIncoherentRelations.rows"
						:columns="tIncoherentRelations.columns"
						:config="tIncoherentRelations.config"
						:totalRows="tIncoherentRelations.total_rows"
						class="q-table--borderless">
					</qtable> 
				</row>

				<row v-else-if="!isEmptyObject(Model.OrphanRelations)">
					<hr />
					<row>
						<div class="alert alert--info">
							<div class="alert-header">
								<q-icon
									class="alert-icon"
									icon="information" />
								<label><b>{{ Resources.ULTIMA_VERIFICACAO35305 }}&#58;</b></label>
									{{ formatDate(Model.LastUpdate) }}
							</div>
							<div v-html="getOrphanTitle()" />
						</div>
					</row>
					<qtable :rows="tOrphanRelations.rows"
						:columns="tOrphanRelations.columns"
						:config="tOrphanRelations.config"
						:totalRows="tOrphanRelations.total_rows"
						class="q-table--borderless">
					</qtable>
				</row>

				<row v-if="Model.Active">
					<q-label
						:for="'progressbarVw_' + Model.Num"
						:label="Resources.PROGRESSO52692" />
					<div
						class="progress"
						:id="'progressbarVw_' + Model.Num">
						<div
							class="progress-bar progress-bar-striped progress-bar-animated"
							:style="{ width: dataPB.progress + '%' }">
							{{ dataPB.progress }}%
						</div>
					</div>
					<div>
						{{ dataPB.message }}
					</div>
				</row>
			</q-collapsible>
		</row>
	</div>
</template>

<script>
	// @ is an alias to /src
	import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';

	export default {
		name: 'maintenance_data_quality_inconsistent_relations',

		mixins: [reusableMixin],

		emits: ['updateData'],

		props: {
			Model: {
				required: true,
				IncoherenceTitle: ''
			}
		},

		data() {
			var vm = this;
			return {
				dataPB: {
					progress: 0,
					message: ''
				},
				tIncoherentRelations: {
						rows: [],
						total_rows: 0,
						columns: [
								{
										label: 'Table 1',
										name: "Table1",
										sort: true,
										initial_sort: true,
										initial_sort_order: "asc"
								},
								{
										label: 'Foreign key 1',
										name: "Fk1",
										sort: true
								},
								{
										label: 'Table 2',
										name: "Table2",
										sort: true
								},
								{
										label: 'Foreign key 2',
										name: "Fk2",
										sort: true
								},
								{
										label: 'Destination',
										name: "Destination",
										sort: true
								},
								{
										label: 'Inconsistent Relations',
										name: "CountIR",
										sort: true
								},
								{
										label: 'Path',
										name: "Path",
										sort: true
								},
								{
										label: 'Query',
										name: "Sql",
										sort: true
								}
						],
						config: {
							table_title: () => vm.$t('TIPO_DE_PESQUISA13226')
						}
				},
				tOrphanRelations: {
						rows: [],
						total_rows: 0,
						columns: [
								{
										label: 'Table 1',
										name: "Table1",
										sort: true,
										initial_sort: true,
										initial_sort_order: "asc"
								},
								{
										label: 'Foreign key 1',
										name: "Fk1",
										sort: true
								},
								{
										label: 'Destination',
										name: "Destination",
										sort: true
								},
								{
										label: 'Orphan Records',
										name: "CountOrphans",
										sort: true
								},
								{
										label: 'Query',
										name: "Sql",
										sort: true
								}
						],
						config: {
							table_title: () => vm.$t('TIPO_DE_PESQUISA13226')
						}
				}
			};
		},

		computed: {
			sumIR() {
				var vm = this, s = 0;
				if ($.isEmptyObject(vm.Model) || $.isEmptyObject(vm.Model.IncoherentRelations)) { return 0; }
				$.each(vm.Model.IncoherentRelations, (i, x) => { s += x.CountIR; });
				return s;
			},

			sumOR() {
				var vm = this, s = 0;
				if ($.isEmptyObject(vm.Model) || $.isEmptyObject(vm.Model.OrphanRelations)) { return 0; }
				$.each(vm.Model.OrphanRelations, (i, x) => { s += x.CountOrphans; });
				return s;
			}
		},

		methods: {
			getIncoherenceText() {
				const incoherenceText1 = 'INCOERENCIA_DE_RELAC38138';
				const incoherenceText2 = 'REGISTOS_ORFAOS26691';
				
				if (this.Model.IncoherenceTitle === incoherenceText1) {
					return this.Resources.DIFERENTES_CAMINHOS_04528
				} else if (this.Model.IncoherenceTitle === incoherenceText2) {
					return this.Resources.CHAVE_PREENCHIDA_SEM15594
				} else {
					return '';
				}
			},

			DataQualityStart() {
				var vm = this;
				QUtils.postData('DbAdmin', 'DataQualityStart', vm.Model, null, function (data) {
					vm.startMonitorReindexProgress();
					vm.$emit('updateData', data);
				});
			},

			startMonitorReindexProgress() {
				if (this.Model.Active && this.dataPB.progress === 0) {
						setTimeout(this.checkProgress, 250);
				}
			},

			checkProgress() {
				var vm = this;
				QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'DataQualityProgress', { num: vm.Model.Num })).done(function (data) {
					vm.Model.Active = data.isActive;
					if (data.isActive) {
						vm.dataPB.progress = data.Count;
						vm.dataPB.message = data.Message;
						setTimeout(vm.checkProgress, 500);
					}
					else {
						vm.dataPB = {
							progress: 0,
							message: ''
						};
						vm.$emit('updateData');
					}
				});
			},

			fillTIncoherentRelations() {
					this.tIncoherentRelations.rows = this.Model.IncoherentRelations || [];
					this.tIncoherentRelations.total_rows = (this.Model.IncoherentRelations || []).length;
			},

			fillTOrphanRelations() {
					this.tOrphanRelations.rows = this.Model.OrphanRelations || [];
					this.tOrphanRelations.total_rows = (this.Model.OrphanRelations || []).length;
			},

			getIncoherentTitle() {
				return `<b>${this.Resources.TIPO_DE_PESQUISA13226}</b> : ${this.Model.IncoherentRelations?.length} ${this.Resources.CASOS25883} / ${this.sumIR} ${this.Resources.TOTAL49307}`
			},

			getOrphanTitle() {
				return `<b>${this.Resources.REGISTOS_ORFAOS43228}</b> :  ${this.Model.OrphanRelations?.length} ${this.Resources.CASOS25883} / ${this.sumOR} ${this.Resources.TOTAL49307}`
			}
		},

		created() {
			this.fillTIncoherentRelations();
			this.fillTOrphanRelations();
		},

		watch: {
			'Model.Active': {
				handler(newValue, oldValue) {
					if (newValue && !oldValue) {
						this.startMonitorReindexProgress();
					}
				},
				deep: true
			},

			'Model.IncoherentRelations': {
				handler() {
					this.fillTIncoherentRelations();
				},
				deep: true
			},

			'Model.OrphanRelations': {
				handler() {
					this.fillTOrphanRelations();
				},
				deep: true
			}
		}
	};
</script>
