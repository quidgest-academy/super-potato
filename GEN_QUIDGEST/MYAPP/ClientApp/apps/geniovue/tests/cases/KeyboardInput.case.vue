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
							label="Keyboard Input">
							<div class="step-column">
								<div class="keyboard-box">
									<div class="keyboard-filter">
										<h1>Input Placement</h1>
										<select v-model="inputPlacement">
											<option value="top">top</option>
											<option value="bottom">bottom</option>
											<option value="left">left</option>
											<option value="right">right</option>
										</select>
										<h1>Variants</h1>
										<select v-model="kbStyle">
											<option value="primary">primary</option>
											<option value="secondary">secondary</option>
										</select>
									</div>
                                    
									<div class="keyboard-prop"> 
										<q-keyboard-input
											v-model="text"
											:input-placement="inputPlacement"
											:kb-style="kbStyle"
											:title="customTitle" />
									</div>
								</div>
							</div>
						</base-input-structure>
					</q-control-wrapper>
				</q-row-container>
			</div>
		</fieldset>
	</div>
</template>

<script>
	import QKeyboardInput from '@/components/rendering/QKeyboardInput.vue'

	export default {
		name: 'QKeyboardContainer',

		components: {
			QKeyboardInput,
		},

		inheritAttrs: false,

		expose: [],

		data()
		{
			return {
				text: 0,
				inputPlacement: 'top', 
				kbStyle: 'primary', 
				customTitle: 'Enter'
			}
		}
	}
</script>

<!-- eslint-disable-next-line vue/no-restricted-block -->
<style scoped>
	.keyboard-box {
		display: flex;
		flex-direction: row;
		width: 100%;
		height: 450px;
	}

	.keyboard-filter {
		display: flex;
		flex-direction: column;
		width: 300px;
		align-items: center;
		justify-content: center;
		margin-top: -0.9rem;
	}

	.keyboard-prop {
		display: flex;
		align-items: center;
		justify-content: center;
		width: 45vw;
		height: 55vh;
	}

	.step-column {
		display: flex;
		flex-direction: column;
	}
</style>
