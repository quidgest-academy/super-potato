//@ts-check

import lintJs from '@eslint/js'
import pluginVitest from '@vitest/eslint-plugin'
//import prettierConfig from '@vue/eslint-config-prettier'
import { defineConfigWithVueTs, vueTsConfigs } from '@vue/eslint-config-typescript'
//import storybook from 'eslint-plugin-storybook'
import lintVue from 'eslint-plugin-vue'
import globals from 'globals'

export default defineConfigWithVueTs(
	{
		name: 'app/files-to-lint',
		files: ['**/*.{js,ts,vue}']
	},

	{
		name: 'app/files-to-ignore',
		ignores: ['**/dist', '**/storybook-static', '**/coverage', '**/shims.d.ts']
	},

	lintJs.configs.recommended,
	...lintVue.configs['flat/strongly-recommended'],

	// TODO: Enable this MUCH stricter preset
	// vueTsConfigs.recommendedTypeChecked
	vueTsConfigs.recommended,

	pluginVitest.configs.recommended,
	//...storybook.configs['flat/recommended'],

	//prettierConfig,

	{
		languageOptions: {
			globals: {
				...globals.node,
				...globals.commonjs,
				...globals.jest
			}
		},
		linterOptions: {
			reportUnusedDisableDirectives: false
		}
	},

	{
		files: ['**/*.{js,ts,vue}'],
		rules: {
			// FIXME: Enable this rule when the codebase is ready
			'@typescript-eslint/no-this-alias': 'off',
			'@typescript-eslint/no-unused-expressions': ['error', { allowTernary: true }],
			'@typescript-eslint/no-unused-vars': 'warn',
			eqeqeq: 'error',
			'no-console': 'warn',
			'no-debugger': 'warn',
			'no-fallthrough': 'off',
			'no-mixed-spaces-and-tabs': 'warn',
			'no-unreachable': 'warn',
			'no-var': 'warn',
			'prefer-const': 'warn'
		}
	},

	{
		files: ['**/*.{js,ts}'],
		rules: {
			indent: ['warn', 'tab', { SwitchCase: 1 }]
		}
	},

	{
		files: ['**/*.vue'],
		rules: {
			'vue/block-lang': ['error', { script: { lang: ['js', 'ts'] } }],
			'vue/block-order': ['warn', { order: ['template', 'script', 'style'] }],
			'vue/custom-event-name-casing': [
				'warn',
				'kebab-case',
				{
					ignores: ['/^[a-z]+(?:-[a-z]+)*:[a-z]+(?:-[a-z]+)*$/u']
				}
			],
			'vue/eqeqeq': 'error',
			'vue/html-button-has-type': 'error',
			'vue/html-closing-bracket-newline': 'off',
			'vue/html-indent': ['warn', 'tab'],
			'vue/html-self-closing': 'off',
			'vue/match-component-import-name': 'error',
			'vue/next-tick-style': 'warn',
			'vue/no-duplicate-attr-inheritance': 'warn',
			'vue/no-empty-component-block': 'warn',
			'vue/no-multiple-objects-in-class': 'warn',
			'vue/no-potential-component-option-typo': ['error', { threshold: 2 }],
			'vue/no-ref-object-reactivity-loss': 'error',
			'vue/no-required-prop-with-default': 'warn',
			'vue/no-restricted-block': ['error', { element: 'style' }],
			'vue/no-template-target-blank': 'error',
			'vue/no-this-in-before-route-enter': 'error',
			'vue/no-unsupported-features': 'error',
			'vue/no-use-v-else-with-v-for': 'error',
			'vue/no-useless-mustaches': 'warn',
			'vue/no-useless-v-bind': 'warn',
			'vue/prefer-prop-type-boolean-first': 'error',
			'vue/prefer-true-attribute-shorthand': 'warn',
			'vue/require-default-prop': 'off',
			'vue/require-expose': 'error',
			'vue/require-macro-variable-name': 'error',
			'vue/require-name-property': 'warn',
			'vue/require-prop-comment': ['warn', { type: 'JSDoc' }],
			'vue/script-indent': [
				'warn',
				'tab',
				{
					baseIndent: 1,
					switchCase: 1
				}
			],
			'vue/singleline-html-element-content-newline': 'off',
			'vue/valid-define-options': 'error'
		}
	}
)
