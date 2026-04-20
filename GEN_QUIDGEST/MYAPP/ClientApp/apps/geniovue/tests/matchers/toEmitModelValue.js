import { matcherHint, printExpected, printReceived } from 'jest-matcher-utils'
import _isEqual from 'lodash-es/isEqual'

// Examples in:
// https://github.com/jest-community/jest-extended/blob/master/src/matchers/toBeWithin/index.js

export default function toEmitModelValue(received, expected)
{
	// Check if the modelValue event is emmited
	const emit = received.emitted('update:modelValue')

	if (!emit || emit.length === 0)
	{
		return {
			pass: false,
			message: () => matcherHint('.toEmitModelValue', 'received', 'expected') + '\n\n'
				+ `Expected ${printExpected(expected)} would have been emmited.\nNo modelValue update has been emitted`
		}
	}

	// An event is sent for every keystroke, we just check if the last one has the expected value
	// https://vue-test-utils.vuejs.org/api/wrapper/emitted.html
	const lastValue = emit.slice(-1)[0][0]
	return _isEqual(lastValue, expected) ? {
		pass: true,
		message: () => matcherHint('.not.toEmitModelValue', 'received', 'expected') + '\n\n'
				+ `Expected ${printExpected(expected)} would not have been emmited.\nEmmited: ${printReceived(lastValue)}`
	}
		: {
			pass: false,
			message: () => matcherHint('.toEmitModelValue', 'received', 'expected') + '\n\n'
				+ `Expected ${printExpected(expected)} would have been emmited.\nEmmited: ${printReceived(lastValue)}`
		}
}
