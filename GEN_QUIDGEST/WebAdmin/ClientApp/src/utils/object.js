import { isDefined } from '@/utils/common'

/**
 * Checks if a value is a non-null object.
 * @param {*} object - Value to check.
 * @returns {boolean} True if the value is an object, false otherwise.
 */
export function isObject(object) {
	return isDefined(object) && typeof object === 'object' && !Array.isArray(object)
}

/**
 * Creates a shallow copy of an object.
 * @param {Object} object - Object to copy.
 * @returns {Object} A shallow copy of the object.
 */
export function shallowCopy(object) {
	return Object.assign(createInstance(object), object)
}

/**
 * Merges two objects recursively, applying priority rules.
 * Defaults to target properties, except when the target property is empty or a priority rule overrides the default.
 * @param {Object} target - Base object.
 * @param {Object} [source={}] - Object to merge into the target.
 * @param {Object} [priorityRules={}] - Rules that determine which values to prioritize (target or source).
 * @returns {Object} The merged object.
 */
export function merge(target, source = {}, priorityRules = {}) {
	const merged = createInstance(target)

	const mergedKeys = new Set([...Object.keys(source), ...Object.keys(target)])

	for (const key of mergedKeys) {
		const targetProperty = target[key]
		const sourceProperty = source[key]
		const keepSourceProp = keepSource(targetProperty, priorityRules[key])

		if (isObject(targetProperty) && isObject(sourceProperty)) {
			// To keep the source property, swap target with source and pass no priority rules
			merged[key] = keepSourceProp
				? merge(sourceProperty, targetProperty)
				: merge(targetProperty, sourceProperty)

			continue
		}

		merged[key] = keepSourceProp ? source[key] : target[key]
	}

	return merged
}

/**
 * Determines if, during a merge, a source property should be kept instead of the target, based on its priority rule.
 * @param {*} targetProperty - target property value.
 * @param {boolean} priorityRule - Priority rule to apply.
 * @returns {boolean} True if the source property should be kept, false otherwise.
 */
function keepSource(targetProperty, priorityRule) {
	if (isDefined(priorityRule)) return priorityRule

	return !isDefined(targetProperty)
}

/**
 * Creates a new instance of a class while preserving its prototype.
 * @param {Object} object - Object to instantiate.
 * @returns {Object} New instance of the class.
 */
function createInstance(object) {
	return Object.create(getPrototype(object))
}

/**
 * Retrieves a class prototype. Defaults to Object.
 * @param {Object} object - Object of a class.
 * @returns {*} The class prototype.
 */
function getPrototype(object) {
	return Object.getPrototypeOf(object) ?? Object.prototype
}
