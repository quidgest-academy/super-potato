import _assignWith from 'lodash-es/assignWith'
import _every from 'lodash-es/every'
import _get from 'lodash-es/get'
import _has from 'lodash-es/has'
import _set from 'lodash-es/set'

export function propsConverter(route)
{
	return _assignWith({}, route.params, (_, srcValue, key) => {
		if (key === 'isHomePage' || key === 'isNested')
			return ('' + srcValue).toLocaleLowerCase() === 'true'
		else if (key === 'historyEntries' && typeof srcValue === 'string')
		{
			try
			{
				const historyEntries = JSON.parse(srcValue)
				return historyEntries
			}
			catch
			{
				return []
			}
		}

		return srcValue
	})
}

/**
 * Update query parameters and navigate to the route with potentially modified parameters and query.
 * If certain conditions described by the `meta.limitations` property are met, it may update the
 * route navigation to include specific query parameters and/or parameters.
 * The update occurs only when `alreadyAppliedLimitations` is not set to 'true' or when the query
 * does not contain all the necessary limitations.
 *
 * @param {Object} to - The target route object of the navigation.
 * @param {_} _ - Placeholder parameter, not used in the function.
 * @param {Function} next - The method to resolve the navigation. Accepts the route to navigate to.
 * @returns {void} - This function does not return a value, it calls `next()` to continue navigation.
 *
 * @example
 * // usage in a Vue router `beforeEnter` hook
 * {
 *   path: '/some-path',
 *   component: SomeComponent,
 *   beforeEnter: updateQueryParams
 * }
 */
export function updateQueryParams(to, _, next)
{
	const { meta, query, params } = to

	if (Array.isArray(meta?.limitations) && meta.limitations.length > 0)
	{
		const queryHasAll = _every(meta.limitations, (property) => _has(query, property))

		if (params.alreadyAppliedLimitations !== 'true' || queryHasAll === false)
		{
			const paramsHasAll = _every(meta.limitations, property => _has(params, property))

			if (queryHasAll || paramsHasAll)
			{
				const newQuery = { ...query }
				const newParams = { ...params, alreadyAppliedLimitations: 'true' }

				meta.limitations.forEach((limitKey) => {
					// If it comes from internal navigation
					if (paramsHasAll)
					{
						// Update query string
						const limitValue = _get(params, limitKey)
						_set(newQuery, limitKey, limitValue)
					}
					// If it comes from query string
					else if (_has(query, limitKey))
					{
						const limitValue = _get(query, limitKey)
						_set(newParams, limitKey, limitValue)
					}
				})

				// Navigate to the route with modified params and query
				next({
					...to,
					params: newParams,
					query: newQuery,
					replace: true
				})

				return
			}
		}
	}

	next()
}

export default {
	propsConverter,
	updateQueryParams
}
