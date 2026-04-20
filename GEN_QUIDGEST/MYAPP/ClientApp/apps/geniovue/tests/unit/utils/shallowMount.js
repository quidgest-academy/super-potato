// Components
import components from '@/components'

// Plugins
import framework from './framework'

// Utils
import { shallowMount as _shallowMount } from '@vue/test-utils'
import merge from 'lodash-es/merge'

export function shallowMount(component, options)
{
	const _options = merge(options, {
		global: {
			plugins: [framework, components]
		}
	})

	return _shallowMount(component, _options)
}
