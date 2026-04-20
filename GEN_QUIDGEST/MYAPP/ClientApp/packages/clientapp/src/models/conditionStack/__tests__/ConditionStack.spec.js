import { nextTick } from 'vue'
import { describe, expect, it } from 'vitest'

import { QEventEmitter } from '../../../plugins/eventBus'
import {
	BlockConditionStack,
	ClearConditionStack,
	HideConditionStack,
	RequiredConditionStack
} from '../'

const testSource = 'test_source',
	testEvt = 'test_event'

describe('ConditionStack', () => {
	it.each([
		['block', 'unblocked', new BlockConditionStack()],
		['fill', 'unblocked', new ClearConditionStack()],
		['hide', 'visible', new HideConditionStack()],
		['required', 'not required', new RequiredConditionStack()]
	])('Empty %s stack makes the field %s', (_, __, stack) => {
		expect(stack.size).toStrictEqual(0)
		expect(stack.anyMet).toStrictEqual(false)
	})

	it.each([
		['block', 'blocked', new BlockConditionStack()],
		['fill', 'blocked', new ClearConditionStack()],
		['hide', 'hidden', new HideConditionStack()],
		['required', 'required', new RequiredConditionStack()]
	])('Non-empty %s stack makes the field %s', async (_, __, stack) => {
		await stack.add(testSource)
		expect(stack.size).toStrictEqual(1)
		expect(stack.anyMet).toStrictEqual(true)
	})

	it.each([
		['block', new BlockConditionStack()],
		['fill', new ClearConditionStack()],
		['hide', new HideConditionStack()],
		['required', new RequiredConditionStack()]
	])('Adding the same %s source twice will only work the first time', async (_, stack) => {
		let result = await stack.add(testSource)
		expect(result).toStrictEqual(true)
		expect(stack.size).toStrictEqual(1)

		result = await stack.add(testSource)
		expect(result).toStrictEqual(false)
		expect(stack.size).toStrictEqual(1)
	})

	it.each([
		['blocked by a block source', BlockConditionStack.MET_EVENT, new BlockConditionStack()],
		['blocked by a fill source', ClearConditionStack.MET_EVENT, new ClearConditionStack()],
		['hidden', HideConditionStack.MET_EVENT, new HideConditionStack()],
		['required', RequiredConditionStack.MET_EVENT, new RequiredConditionStack()]
	])('When the field becomes %s, the "%s" event is emitted', async (_, __, stack) => {
		let emits = 0
		stack.addOnMetListener(() => emits++)

		await stack.add(testSource)
		expect(stack.anyMet).toStrictEqual(true)
		expect(emits).toStrictEqual(1)
	})

	it.each([
		['unblocked by a block source', BlockConditionStack.UNMET_EVENT, new BlockConditionStack()],
		['unblocked by a fill source', ClearConditionStack.UNMET_EVENT, new ClearConditionStack()],
		['visible', HideConditionStack.UNMET_EVENT, new HideConditionStack()],
		['not required', RequiredConditionStack.UNMET_EVENT, new RequiredConditionStack()]
	])('When the field becomes %s, the "%s" event is emitted', async (_, __, stack) => {
		let emits = 0
		stack.addOnUnmetListener(() => emits++)

		await stack.add(testSource)
		expect(stack.anyMet).toStrictEqual(true)
		expect(emits).toStrictEqual(0)

		stack.remove(testSource)
		expect(stack.anyMet).toStrictEqual(false)
		expect(emits).toStrictEqual(1)
	})

	it.each([
		['block', BlockConditionStack.MET_EVENT, new BlockConditionStack()],
		['fill', ClearConditionStack.MET_EVENT, new ClearConditionStack()],
		['hide', HideConditionStack.MET_EVENT, new HideConditionStack()],
		['required', RequiredConditionStack.MET_EVENT, new RequiredConditionStack()]
	])(
		'Even when adding various sources to a %s stack, the "%s" event is emitted only once',
		async (_, __, stack) => {
			let emits = 0
			stack.addOnMetListener(() => emits++)

			await stack.add(testSource)
			await stack.add(`${testSource}2`)
			await stack.add(`${testSource}3`)

			expect(stack.anyMet).toStrictEqual(true)
			expect(stack.size).toStrictEqual(3)
			expect(emits).toStrictEqual(1)
		}
	)

	it.each([
		['block', BlockConditionStack.MET_EVENT, new BlockConditionStack()],
		['fill', ClearConditionStack.MET_EVENT, new ClearConditionStack()],
		['hide', HideConditionStack.MET_EVENT, new HideConditionStack()],
		['required', RequiredConditionStack.MET_EVENT, new RequiredConditionStack()]
	])(
		'Calling "emitMetEvent", in a %s stack, will only trigger the "%s" event if at least one condition is met',
		async (_, __, stack) => {
			let emits = 0
			stack.addOnMetListener(() => emits++)

			stack.emitMetEvent()
			expect(stack.anyMet).toStrictEqual(false)
			expect(emits).toStrictEqual(0)

			await stack.add(testSource)
			expect(stack.anyMet).toStrictEqual(true)
			stack.emitMetEvent()
			stack.emitMetEvent()
			expect(emits).toStrictEqual(3)
		}
	)

	it.each([
		['block', BlockConditionStack.UNMET_EVENT, new BlockConditionStack()],
		['fill', ClearConditionStack.UNMET_EVENT, new ClearConditionStack()],
		['hide', HideConditionStack.UNMET_EVENT, new HideConditionStack()],
		['required', RequiredConditionStack.UNMET_EVENT, new RequiredConditionStack()]
	])(
		'Calling "emitUnmetEvent", in a %s stack, will only trigger the "%s" event if no conditions are met',
		async (_, __, stack) => {
			let emits = 0
			stack.addOnUnmetListener(() => emits++)

			stack.emitUnmetEvent()
			expect(stack.anyMet).toStrictEqual(false)
			expect(emits).toStrictEqual(1)

			await stack.add(testSource)
			expect(stack.anyMet).toStrictEqual(true)
			stack.emitUnmetEvent()
			expect(emits).toStrictEqual(1)

			stack.remove(testSource)
			expect(stack.anyMet).toStrictEqual(false)
			stack.emitUnmetEvent()
			stack.emitUnmetEvent()
			expect(emits).toStrictEqual(4)
		}
	)

	it.each([
		['block', 'blocked', true, new BlockConditionStack()],
		['fill', 'blocked', false, new ClearConditionStack()],
		['hide', 'hidden', false, new HideConditionStack()],
		['required', 'required', true, new RequiredConditionStack()]
	])(
		'Adding a conditional source to a %s stack will make the field %s only when it evaluates to %s',
		async (_, __, isMet, stack) => {
			const events = new QEventEmitter()
			stack.setEventEmitter(events)

			await stack.add(testSource, () => !isMet, testEvt)

			expect(stack.size).toStrictEqual(0)
			expect(stack.anyMet).toStrictEqual(false)

			isMet = !isMet
			events.emit(testEvt)

			await nextTick()

			expect(stack.size).toStrictEqual(1)
			expect(stack.anyMet).toStrictEqual(true)
		}
	)

	it.each([
		['block', 'blocked', true, new BlockConditionStack()],
		['fill', 'blocked', false, new ClearConditionStack()],
		['hide', 'hidden', false, new HideConditionStack()],
		['required', 'required', true, new RequiredConditionStack()]
	])(
		'Adding an asynchronous conditional source to a %s stack will make the field %s only when it evaluates to %s',
		async (_, __, isMet, stack) => {
			const events = new QEventEmitter()
			stack.setEventEmitter(events)

			stack.addOnMetListener(() => {
				// This will only be triggered after 200ms.
				expect(stack.size).toStrictEqual(1)
				expect(stack.anyMet).toStrictEqual(true)
			})

			// Make the function only evaluate after 200ms, to simulate a server request.
			const fn = () => new Promise((resolve) => setTimeout(() => resolve(!isMet), 200))
			await stack.add(testSource, fn, testEvt)

			expect(stack.size).toStrictEqual(0)
			expect(stack.anyMet).toStrictEqual(false)

			isMet = !isMet
			events.emit(testEvt)

			// Here no conditions should be met yet, as the function is asynchronous.
			expect(stack.size).toStrictEqual(0)
			expect(stack.anyMet).toStrictEqual(false)
		}
	)

	it.each([
		['block', 'blocked', () => new BlockConditionStack()],
		['fill', 'blocked', () => new ClearConditionStack()],
		['hide', 'hidden', () => new HideConditionStack()],
		['required', 'required', () => new RequiredConditionStack()]
	])(
		'Having a stack associated to a %s stack will make the field %s even if that stack is empty',
		async (_, __, stackFactory) => {
			const stack1 = stackFactory()
			const stack2 = stackFactory()
			const stack3 = stackFactory()
			const stack4 = stackFactory()

			stack1.associateStack(stack2)
			stack2.associateStack(stack3)
			stack2.associateStack(stack4)

			let emits = 0
			stack1.addOnMetListener(() => emits++)

			await stack3.add(testSource)
			await stack3.add(`${testSource}2`)
			await stack4.add(testSource)

			expect(stack4.size).toStrictEqual(1)
			expect(stack4.anyMet).toStrictEqual(true)
			expect(stack3.size).toStrictEqual(2)
			expect(stack3.anyMet).toStrictEqual(true)
			expect(stack2.size).toStrictEqual(3)
			expect(stack2.anyMet).toStrictEqual(true)
			expect(stack1.size).toStrictEqual(3)
			expect(stack1.anyMet).toStrictEqual(true)
			expect(emits).toStrictEqual(1)
		}
	)
})
