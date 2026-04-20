import {
	isRef,
	unref,
	isProxy,
	toRaw,
	type MaybeRefOrGetter,
	type Reactive,
	type ShallowReactive
} from 'vue'

/**
 * Recursively removes Vue refs / reactive proxies and produces a **plain,
 * serialisable** structure – ideal for logging, `cloneDeep`, or
 * diagnostic snapshots.
 *
 *  * Arrays, Maps, Sets and plain objects are copied recursively.
 *  * `Date`, `RegExp` and `Error` are copied with their sensible state.
 *  * Circular references are handled gracefully – the second
 *    occurrence is replaced by the string `"[Circular]"`.
 *
 * @template T
 * @param input      any value (primitive, ref, proxy…)
 * @param memo       used internally to break cycles
 * @returns          de-proxied, clone-safe value
 */
export function deepUnwrap<T>(
	input: MaybeRefOrGetter | Reactive<T> | ShallowReactive<T> | object | unknown,
	memo: WeakMap<object, boolean> = new WeakMap()
): unknown {
	// 1 · unwrap Vue Ref / reactive Proxy
	let value: unknown = isRef(input) ? unref(input) : input
	value = isProxy(value) ? toRaw(value) : value

	// 2 · primitives and functions can be returned as they are
	if (value === null || typeof value !== 'object') return value

	// 3 · circular-reference guard
	if (memo.has(value)) return '[Circular]'
	memo.set(value, true)

	/* ----- native special cases --------------------------------------- */
	if (value instanceof Date) return new Date(value.getTime())
	if (value instanceof RegExp) return new RegExp(value)
	if (value instanceof Error) {
		// copy basic properties; stack often enough for debugging
		return { name: value.name, message: value.message, stack: value.stack }
	}

	/* ----- Arrays ----------------------------------------------------- */
	if (Array.isArray(value)) {
		return value.map((v) => deepUnwrap(v, memo))
	}

	/* ----- Map / Set -------------------------------------------------- */
	if (value instanceof Map) {
		const out: Record<string, unknown> = {}
		// keys converted to string – adequate for diagnostics
		value.forEach((v, k) => {
			out[String(k)] = deepUnwrap(v, memo)
		})
		return out
	}
	if (value instanceof Set) {
		return Array.from(value, (v) => deepUnwrap(v, memo))
	}

	/* ----- plain object (or class instance) --------------------------- */
	const out: Record<PropertyKey, unknown> = {}
	for (const key of Reflect.ownKeys(value)) {
		// ignore non-enumerable/internal stuff
		if (!Object.prototype.propertyIsEnumerable.call(value, key)) continue
		out[key] = deepUnwrap((value as Record<PropertyKey, unknown>)[key], memo)
	}
	return out
}
