import { defineConfig, defineProject, mergeConfig } from 'vitest/config'
import viteConfig from './vite.config'

export default defineConfig((configEnv) =>
	mergeConfig(
		viteConfig(configEnv),
		defineProject({
			test: {
				include: ['**/*.spec.{js,ts}'],
				globals: true,
				setupFiles: ['./tests/global.mocks.js', './tests/matchers/index.js'],
				environment: 'happy-dom'
			}
		})
	)
)
