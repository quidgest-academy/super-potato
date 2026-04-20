import baseConfig from '../../eslint.config.js'

export default [
	...baseConfig,

	{
		files: ['**/*.vue'],
		rules: {
			'vue/block-lang': ['error', { script: { lang: 'ts' } }]
		}
	}
]
