import { type App, type Plugin } from 'vue'
import { FRAMEWORK_CONFIG_KEY } from './symbols'
import type { FrameworkConfig } from './types'

export function createFramework(config: FrameworkConfig): Plugin {
	const install = (app: App) => {
		app.provide(FRAMEWORK_CONFIG_KEY, config)
	}

	return { install }
}
