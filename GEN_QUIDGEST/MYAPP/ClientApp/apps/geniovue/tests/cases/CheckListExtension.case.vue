<template>
	<div class="row">
		<div class="col-12">
			<div class="control-row-group">
				<div class="control-join-group">
					<base-input-structure
						id="CTRL_1"
						label="Write mode enabled">
						<q-table
							:rows="tableTest.rows"
							:columns="tableTest.columns"
							:config="tableTest.config"
							:readonly="tableTest.readonly"
							:rows-checked="tableTest.rowsChecked" />

						<q-check-list-extension
							search-column-name="Val"
							primary-key-column-name="PrimaryKey"
							:options="tableTest.rows"
							:rows-checked="tableTest.rowsChecked"
							@remove-label="uncheckRow(tableTest.rowsChecked, $event)"
							@on-enter="checkRow(tableTest.rowsChecked, $event)" />
					</base-input-structure>

					<br />

					<base-input-structure
						id="CTRL_2"
						label="Write mode disabled">
						<q-table
							:rows="tableTest.rows"
							:columns="tableTest.columns"
							:config="tableTest.config"
							:readonly="tableTest.readonly"
							:rows-checked="tableTest.rowsChecked" />

						<q-check-list-extension
							search-column-name="Val"
							primary-key-column-name="PrimaryKey"
							disabled
							:options="tableTest.rows"
							:rows-checked="tableTest.rowsChecked"
							@remove-label="uncheckRow(tableTest.rowsChecked, $event)"
							@on-enter="checkRow(tableTest.rowsChecked, $event)" />
					</base-input-structure>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import fakeData from './CheckListExtension.mock'

	export default {
		name: 'QCheckListExtensionContainer',

		docsfile: './docs/extensions/CheckListExtension.md',

		inheritAttrs: false,

		expose: [],

		data()
		{
			return fakeData.simpleUsage()
		},

		methods: fakeData.simpleUsageMethods
	}
</script>
