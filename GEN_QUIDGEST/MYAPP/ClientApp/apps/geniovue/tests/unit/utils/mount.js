// Components
import components from '@/components'

// Plugins
import framework from './framework'

// Utils
import { mount as _mount } from '@vue/test-utils'
import merge from 'lodash-es/merge'

export function mount(component, options)
{
	const _options = merge(options, {
		global: {
			plugins: [framework, components]
		}
	})

	return _mount(component, _options)
}
