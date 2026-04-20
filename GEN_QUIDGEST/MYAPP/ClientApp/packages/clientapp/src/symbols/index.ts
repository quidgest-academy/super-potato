import type { InjectionKey } from 'vue'
import type { FrameworkConfig } from '../types'

export const FRAMEWORK_CONFIG_KEY: InjectionKey<FrameworkConfig> = Symbol('framework-config')
