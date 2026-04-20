// Components
import components from '@/components'

// Plugins
import framework from './framework'

// Utils
import { render as _render } from '@testing-library/vue'
import merge from 'lodash-es/merge'

export function render(component, options)
{
	const _options = merge(options, {
		global: {
			plugins: [framework, components]
		}
	})

	const renderedObj = _render(component, _options)
	renderedObj.asyncEmitted = asyncEmitted

	return renderedObj
}

function asyncEmitted(eventName, retries = 3, timeout = 300)
{
	const checkEmit = (resolve, eventName, retries, timeout) => {
		const eventPayload = this.emitted(eventName)

		if (typeof eventPayload !== 'undefined' || retries === 0)
			resolve(eventPayload)
		else
			setTimeout(() => checkEmit(resolve, eventName, retries - 1, timeout), timeout)
	}

	return new Promise((resolve) => checkEmit(resolve, eventName, retries, timeout))
}
