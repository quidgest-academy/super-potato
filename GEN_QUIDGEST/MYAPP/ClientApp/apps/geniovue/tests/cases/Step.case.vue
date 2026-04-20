<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<fieldset>
			<div class="form-flow">
				<q-row-container>
					<q-control-wrapper class="control-join-group">
						<base-input-structure
							id="CTRL_1"
							label="Step Input">
							<div class="step-column">
								<div class="step-container">
									<div class="step-filter">
										<h1>Variants</h1>
										<select v-model="live.stepProperties.stepStyle">
											<option value="primary">primary</option>
											<option value="secondary">secondary</option>
											<option value="danger">danger</option>
										</select>
									</div>
									<div class="live-tab-sep" />
									<div
										class="step-block"
										id="live">
										<q-step
											v-model="text"
											@update:model-value="handleChange"
											v-bind="live.stepProperties" />
									</div>
								</div>
								<div class="live-tab-sep-bottom" />
							</div>
						</base-input-structure>
					</q-control-wrapper>
				</q-row-container>
			</div>
		</fieldset>
	</div>
</template>

<script>
	import QStep from '@/components/rendering/QStep.vue'

	export default {
		name: 'QStepContainer',

		components: {
			QStep
		},

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				text: 0,
				live: {
					stepProperties: {
						stepStyle: 'primary', // default layout
						step: 1,
						minValue: 0,
						maxValue: 10,
						maxDecimals: 0,
						enableInput: false
					}
				}
			}
		},

		methods: {
			handleChange(newText)
			{
				this.text = newText
			}
		}
	}
</script>

<!-- eslint-disable-next-line vue/no-restricted-block -->
<style scoped>
	.step-container {
		display: flex;
		flex-direction: row;
		width: 100%;
		height: 150px;
	}

	.step-filter {
		display: flex;
		flex-direction: column;
		width: 250px;
		align-items: center;
		justify-content: center;
		margin-top: -1.3rem;
	}

	.step-block {
		display: flex;
		width: 400px;
		align-items: center;
		justify-content: center;
	}

	.live-tab-sep {
		width: 1px;
		background-color: lightgray;
		margin-bottom: 10px;
	}

	.step-column {
		display: flex;
		flex-direction: column;
	}

	.live-tab-sep-bottom {
		display: flex;
		flex-direction: row;
		width: 100%;
		height: 1px;
		background-color: lightgray;
	}
</style>
