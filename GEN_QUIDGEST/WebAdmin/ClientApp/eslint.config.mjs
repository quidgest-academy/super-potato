//@ts-check

import pluginVue from 'eslint-plugin-vue'
import { defineConfigWithVueTs, vueTsConfigs } from '@vue/eslint-config-typescript'
import prettierConfig from '@vue/eslint-config-prettier'

export default defineConfigWithVueTs(
	{
		name: 'app/files-to-lint',
		files: ['**/*.{js,ts,vue}']
	},

	{
		name: 'app/files-to-ignore',
		// TODO: The only folder that should be ignored here is the /dist folder and external files (bootbox, jquery, ...). Gradually solve lint issues in .vm templates.
		ignores: [
			'**/dist',
			'src/plugins/jquery',
			'src/plugins/adminlte.js',
			'src/plugins/bootbox.js',
			'src/views/**/*.vue',
			'src/App.vue',
			'src/global.js',
			'src/router.js',
			'src/utils/PathFinder.js',
			'src/plugins/i18n.js',
			'src/mixins/mainMixin.js',
			'src/utils/mainUtils.js'
		]
	},

	...pluginVue.configs['flat/recommended'],

	// TODO: Enable this MUCH stricter preset
	// vueTsConfigs.recommendedTypeChecked
	vueTsConfigs.recommended,
	// TODO: Enable when VisualStudio handles eslint+prettier config
	prettierConfig,

	{
		rules: {
			'@typescript-eslint/no-unused-vars': 'error',
			'@typescript-eslint/no-unused-expressions': ['error', { allowTernary: true }],
			eqeqeq: 'error',
			'no-unused-vars': 'off',
			'prettier/prettier': 'off',
			'vue/block-order': ['warn', { order: ['template', 'script', 'style'] }],
			// TODO: Gradually transition to TS <script>s / Composition API in Vue components before changing these rules
			'vue/block-lang': ['error', { script: { lang: 'js' } }],
			// 'vue/component-api-style': ['error', ['script-setup']],
			'vue/custom-event-name-casing': [
				'warn',
				'kebab-case',
				{
					ignores: ['/^[a-z]+(?:-[a-z]+)*:[a-z]+(?:-[a-z]+)*$/u']
				}
			],
			'vue/define-macros-order': ['warn', { order: ['defineProps', 'defineEmits'] }],
			// 'vue/define-emits-declaration': ['error', 'type-based'],
			'vue/define-props-declaration': ['error', 'type-based'],
			'vue/eqeqeq': 'error',
			'vue/html-button-has-type': 'error',
			'vue/match-component-import-name': 'error',
			'vue/next-tick-style': 'error',
			// TODO: Re-enable once icons for ordering in tables are replaced
			// 'vue/no-bare-strings-in-template': 'error',
			'vue/no-duplicate-attr-inheritance': 'warn',
			'vue/no-empty-component-block': 'warn',
			'vue/no-multiple-objects-in-class': 'warn',
			'vue/no-ref-object-reactivity-loss': 'error',
			'vue/no-restricted-block': ['error', { element: 'style' }],
			'vue/no-root-v-if': 'error',
			'vue/no-static-inline-styles': 'error',
			'vue/no-template-target-blank': 'error',
			'vue/no-undef-properties': 'error',
			'vue/no-unsupported-features': 'error',
			'vue/no-unused-refs': 'warn',
			'vue/no-use-v-else-with-v-for': 'error',
			'vue/no-useless-mustaches': 'warn',
			'vue/no-useless-v-bind': 'warn',
			'vue/prefer-define-options': 'warn',
			'vue/prefer-prop-type-boolean-first': 'error',
			'vue/prefer-true-attribute-shorthand': 'warn',
			// 'vue/require-emit-validator': 'error',
			'vue/require-expose': 'error',
			'vue/require-macro-variable-name': 'error',
			'vue/require-prop-comment': ['warn', { type: 'JSDoc' }],
			'vue/require-typed-object-prop': 'error',
			'vue/require-typed-ref': 'error',
			'vue/v-on-handler-style': 'error',
			'vue/valid-v-slot': ['error', { allowModifiers: true }],
			'vue/no-v-html': 'off'
		}
	}
)
