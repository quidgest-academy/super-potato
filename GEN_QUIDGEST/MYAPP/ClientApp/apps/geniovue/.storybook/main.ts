import type { StorybookConfig } from '@storybook/vue3-vite'

const config: StorybookConfig = {
	stories: [
		'../tests/docs/**/*.mdx',
		'../tests/cases/**/*.stories.@(js|jsx|mjs|ts|tsx)',
		'../../../packages/clientapp/src/**/*.stories.@(js|jsx|mjs|ts|tsx)'
	],
	addons: [
		'@storybook/addon-links',
		'@storybook/addon-essentials',
		'@storybook/addon-interactions'
	],
	framework: {
		name: '@storybook/vue3-vite',
		options: {
			// https://github.com/storybookjs/storybook/pull/22285
			docgen: 'vue-component-meta'
		}
	},
	docs: {},
	staticDirs: ['./assets']
}

export default config
