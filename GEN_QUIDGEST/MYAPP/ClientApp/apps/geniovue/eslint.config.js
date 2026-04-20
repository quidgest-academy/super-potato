import baseConfig from '../../eslint.config.js'

import { componentNames } from './src/components/index.js'

export default [
	...baseConfig,

	{
		ignores: ['public/', 'src/assets/graphics/*']
	},

	{
		files: ['**/*.vue'],
		rules: {
			'vue/no-undef-components': [
				'error',
				{
					ignorePatterns: [
						'RouterLink',
						'RouterView',
						...componentNames
					]
				}
			]
		}
	}
]
