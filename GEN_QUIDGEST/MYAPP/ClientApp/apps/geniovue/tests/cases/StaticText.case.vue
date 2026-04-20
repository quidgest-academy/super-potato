<template>
	<div
		class="container-fluid"
		style="flex: 1 1 auto">
		<form class="form-horizontal">
			<fieldset>
				<div class="form-flow">
					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_1"
								class="i-text"
								label="Text with HTML support:"
								:label-attrs="{ class: 'i-text__label' }">
								<q-static-text
									id="CTRL_1a"
									:text="htmlText"
									supports-html
									size="xxlarge" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>

					<q-row-container>
						<q-control-wrapper class="control-join-group">
							<base-input-structure
								id="CTRL_2"
								class="i-text"
								label="Text without HTML support:"
								:label-attrs="{ class: 'i-text__label' }">
								<q-static-text
									id="CTRL_2a"
									:text="htmlText"
									:supports-html="false"
									size="xxlarge" />
							</base-input-structure>
						</q-control-wrapper>
					</q-row-container>
				</div>
			</fieldset>
		</form>
	</div>
</template>

<script>
	import fakeData from './StaticText.mock'

	export default {
		name: 'QStaticTextContainer',

		docsfile: './docs/QStaticText.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return fakeData.simpleUsage()
		}
	}
</script>
