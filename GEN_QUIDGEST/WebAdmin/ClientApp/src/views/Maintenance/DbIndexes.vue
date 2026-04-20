<template>
	<div id="maintenance_dbindexes_container">
		<row>
			<q-collapsible
				class="q-collapsible--admin-default"
				:title="Resources[Model.IndexTitle]"
				:subtitle="getIndexesText()"
				width="block">
				<span v-if="isUsageText()">
					{{ Resources.VERIFICAR_INDICES_SQ51609 }}
				</span>
				
				<row class="footer-btn">
					<q-button
						:label="Resources.EXECUTAR_ATUALIZACAO61833"
						@click="DBIndexesStart" />
					<data-system-badge
						:title="Resources.SISTEMA_DE_DADOS_ATU09110" />
				</row>

				<row v-if="!isEmptyObject(Model.UnusedIndexesList)">
					<hr />
					<row v-if="!isEmptyObject(formatDate(Model.LastUpdate))">
						<div class="alert alert--info">
							<div class="alert-header">
								<q-icon
									class="alert-icon"
									icon="information" />
								<label>
									<b>
										{{ Resources.ULTIMA_VERIFICACAO35305 + ':' }}
									</b>
								</label>
								{{ formatDate(Model.LastUpdate) }}
							</div>
							<div v-html="getUnusedTitle()" />
						</div>
					</row>
					<qtable
						:rows="tUnusedIndexes.rows"
						:columns="tUnusedIndexes.columns"
						:config="tUnusedIndexes.config"
						:totalRows="tUnusedIndexes.total_rows"
						class="q-table--borderless">
					</qtable>
				</row>

				<row v-if="!isEmptyObject(Model.RecommendedIndexesList)">
					<hr />
					<row v-if="!isEmptyObject(formatDate(Model.LastUpdate))">
						<div class="alert alert--info">
							<div class="alert-header">
								<q-icon
									class="alert-icon"
									icon="information" />
								<label>
									<b>
										{{ Resources.ULTIMA_VERIFICACAO35305 }}
									</b>
								</label>
								{{ formatDate(Model.LastUpdate) }}
							</div>
							<div v-html="getRecommendedTitle()" />
						</div>
					</row>
					<qtable :rows="tRecommendedIndexes.rows"
						:columns="tRecommendedIndexes.columns"
						:config="tRecommendedIndexes.config"
						:totalRows="tRecommendedIndexes.total_rows"
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
		name: 'maintenance_dbindexes',

		mixins: [reusableMixin],

		emits: ['updateData'],

		props: {
			Model: {
				required: true
			}
		},
		
		data() {
			var vm = this;
			return {
				dataPB: {
					progress: 0,
					message: ''
				},
				tUnusedIndexes: {
					rows: [],
					total_rows: 0,
					columns: [
						{
							label: () => vm.$t('ORDEM38897'),
							name: "OrderCol",
							sort: true,
							initial_sort: true,
							initial_sort_order: "asc"
						},
						{
							label: () => vm.$t('TABELA44049'),
							name: "ObjectName",
							sort: true
						},
						{
							label: () => vm.$t('INDICE13974'),
							name: "IndexName",
							sort: true
						},
						{
							label: 'Seeks',
							name: "UserSeeks",
							sort: true
						},
						{
							label: 'Scans',
							name: "UserScans",
							sort: true
						},
						{
							label: 'Lookups',
							name: "UserLookups",
							sort: true
						},
						{
							label: 'Updates',
							name: "UserUpdates",
							sort: true
						},
						{
							label: () => vm.$t('REGISTOS53981'),
							name: "TableRows",
							sort: true
						},
						{
							label: () => vm.$t('COLUNAS06085'),
							name: "ColumnNames",
							sort: true
						},
						{
							label: () => vm.$t('QUERY_DE_ELIMINACAO04472'),
							name: "Drop_Index",
							sort: true
						}
					],
					config: {
						table_title: () => vm.$t(this.Resources[this.Model.IndexTitle]),
					}
				},
				tRecommendedIndexes: {
					rows: [],
					total_rows: 0,
					columns: [
						{
							label: () => vm.$t('ORDEM38897'),
							name: "OrderCol",
							sort: true,
							initial_sort: true,
							initial_sort_order: "asc"
						},
						{
							label: () => vm.$t('TABELA44049'),
							name: "TableName",
							sort: true
						},
						{
							label: () => vm.$t('COLUNAS_COMPARADAS_P40915'),
							name: "EqualityColumns",
							sort: true
						},
						{
							label: () => vm.$t('COLUNAS_COMPARADAS_P18476'),
							name: "InequalityColumns",
							sort: true
						},
						{
							label: () => vm.$t('COLUNAS_INCLUIDAS_NA12269'),
							name: "IncludedColumns",
							sort: true
						},
						{
							label: () => vm.$t('ULTIMO_SEEK53997'),
							name: "Last_User_Seek",
							sort: true
						},
						{
							label: 'Seeks',
							name: "UserSeeks",
							sort: true
						},
						{
							label: 'Scans',
							name: "UserScans",
							sort: true
						},
						{
							label: () => vm.$t('MELHORIA____10458'),
							name: "Avg_User_Impact",
							sort: true
						},
						{
							label: () => vm.$t('IMPACTO36269'),
							name: "Avg_Estimated_Impact",
							sort: true
						},
						{
							label: () => vm.$t('QUERY_DE_CRIACAO34118'),
							name: "Create_Statement",
							sort: true
						}
					],
					config: {}
				}
			};
		},

		methods: {
			getIndexesText() {
				const indexesText1 = 'INDICES_POUCO_USADOS47529';
				const indexesText2 = 'INDICES_RECOMENDADOS25802';
				
				if (this.Model.IndexTitle === indexesText1) {
					return this.Resources.INDICES_DE_BAIXO_USO57852
				} else if (this.Model.IndexTitle === indexesText2) {
					return this.Resources.DURANTE_A_MANUTENCAO34201
				} else {
					return ''
				}
			},

			isUsageText() {
				const indexesText1 = 'INDICES_POUCO_USADOS47529';
				return this.Model.IndexTitle === indexesText1
			},

			DBIndexesStart() {
				var vm = this;
				QUtils.postData('DbAdmin', 'DBIndexesStart', vm.Model, null, function (data) {
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
				QUtils.FetchData(QUtils.apiActionURL('DbAdmin', 'IndexesProgress', { num: vm.Model.Num })).done(function (data) {
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

			fillTUnusedIndexes() {
				this.tUnusedIndexes.rows = this.Model.UnusedIndexesList || [];
				this.tUnusedIndexes.total_rows = (this.Model.UnusedIndexesList || []).length;
			},

			fillTRecommendedIndexes() {
				this.tRecommendedIndexes.rows = this.Model.RecommendedIndexesList || [];
				this.tRecommendedIndexes.total_rows = (this.Model.RecommendedIndexesList || []).length;
			},

			getUnusedTitle() {
				return `<b>${this.Resources.NUMERO_DE_CASOS30917}</b> : ${this.Model.UnusedIndexesList?.length ?? 0}`
			},

			getRecommendedTitle() {
				return `${this.Resources.NUMERO_DE_CASOS30917} : ${this.Model.RecommendedIndexesList?.length ?? 0}`
			}
		},

		created() {
			this.fillTUnusedIndexes();
			this.fillTRecommendedIndexes();
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

			'Model.UnusedIndexes': {
				handler() {
					this.fillTUnusedIndexes();
				},
				deep: true
			},
			
			'Model.RecommendedIndexes': {
				handler() {
					this.fillTRecommendedIndexes();
				},
				deep: true
			}
		}
	};
</script>
