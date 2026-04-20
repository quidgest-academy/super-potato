import { coverageConfigDefaults, defineConfig } from 'vitest/config'

export default defineConfig({
	test: {
		coverage: {
			provider: 'v8',
			reporter: ['cobertura', 'text'],
			exclude: ['dist/', 'storybook-static/', ...coverageConfigDefaults.exclude]
		},
		outputFile: 'coverage/junit.xml',
		reporters: ['default', 'junit'],
		projects: ['apps/*', 'packages/*']
	}
})
